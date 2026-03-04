using UnityEngine;
using System.Linq;
using Motorways.Models;

namespace Motorways.Views {
    ///<summary>
    ///Vehicle 모델의 논리적 데이터를 바탕으로 시각적인 위치(Transform)를 업데이트합니다.
    ///</summary>
    public class VehicleView : MonoBehaviour {
        [SerializeField] private Vehicle _vehicleModel;

        private Lane _prevLane;
        private Lane _currentLane;

        public void Initialize(Vehicle model) {
            _vehicleModel = model;
        }

        private void Start() {
            if (_vehicleModel == null) {
                //모델이 명시적으로 할당되지 않은 경우 자신과 같은 GameObject에 있는지 시도해봅니다.
                TryGetComponent(out _vehicleModel);
            }
        }

        private void Update() {
            if (_vehicleModel == null) return;

            //주행 중일 때만 비주얼 업데이트 수행
            if (_vehicleModel.State == VehicleState.Driving || _vehicleModel.State == VehicleState.Returning) {
                UpdateVisuals();
            } else {
                //주행 중이 아닐 때는 캐시된 레인 초기화 (새로운 경로를 위해)
                _prevLane = null;
                _currentLane = null;
            }
        }

        private void UpdateVisuals() {
            Lane activeLane = _vehicleModel.GetCurrentLane();
            
            //현재 주행 중인 레인이나 스플라인 정보가 없으면 처리 불가
            if (activeLane == null || activeLane.PathSpline == null) return;

            if (_currentLane != activeLane) {
                _prevLane = _currentLane;
                _currentLane = activeLane;
            }

            //t 값 계산 (0 ~ 1)
            float t = _vehicleModel.DistanceAlongLane / activeLane.Length;
            t = Mathf.Clamp01(t);

            //기본 위치 및 방향 (직선 구간)
            Vector3 position = activeLane.PathSpline.Evaluate(t);
            Vector3 tangent = activeLane.PathSpline.EvaluateTangent(t);

            //부드러운 코너링(Smooth Cornering) 로직
            //현재 레인의 절반을 넘어가면 다음 레인과의 교차점을 제어점으로 삼아 부드러운 곡선을 그립니다.
            Lane nextLane = _vehicleModel.CurrentPath.Count > 1 ? _vehicleModel.CurrentPath.ElementAt(1) : null;

            if (t > 0.5f && nextLane != null && nextLane.PathSpline != null) {
                //현재 레인의 절반(0.5)에서 교차로(1.0)를 거쳐 다음 레인의 절반(0.5)으로 향하는 2차 베지어 곡선
                Vector3 p0 = activeLane.PathSpline.Evaluate(0.5f);
                Vector3 p1 = activeLane.PathSpline.Evaluate(1.0f); //교차점 중심
                Vector3 p2 = nextLane.PathSpline.Evaluate(0.5f);

                //t(0.5 ~ 1.0)를 s(0.0 ~ 0.5)로 매핑
                float s = t - 0.5f; 
                position = Utils.BezierUtils.GetPoint(p0, p1, p2, s);
                tangent = Utils.BezierUtils.GetTangent(p0, p1, p2, s);

            } else if (t <= 0.5f && _prevLane != null && _prevLane.PathSpline != null && _prevLane.EndNode == activeLane.StartNode) {
                //이전 레인의 절반에서 넘어온 상태를 이어받아 현재 레인의 절반까지 곡선을 마무리
                Vector3 p0 = _prevLane.PathSpline.Evaluate(0.5f);
                Vector3 p1 = _prevLane.PathSpline.Evaluate(1.0f); //교차점 중심
                Vector3 p2 = activeLane.PathSpline.Evaluate(0.5f);

                //t(0.0 ~ 0.5)를 s(0.5 ~ 1.0)로 매핑
                float s = 0.5f + t;
                position = Utils.BezierUtils.GetPoint(p0, p1, p2, s);
                tangent = Utils.BezierUtils.GetTangent(p0, p1, p2, s);
            }

            //Transform 업데이트 전에 차선 오프셋 적용
            //진행 방향(tangent)을 기준으로 우측(혹은 좌측)으로 약간 띄워줍니다.
            //Vector3.Cross(Vector3.up, tangent)를 하면 접선에 수직인 벡터(우측 방향)가 나옵니다.
            float laneOffset = 0.1f; //차선 오프셋 크기 (도로 폭에 맞게 조절 필요)
            Vector3 normal = Vector3.Cross(Vector3.up, tangent).normalized;
            
            //미니 모터웨이는 보통 좌측 통행을 하므로, 진행 방향의 좌측(-normal)으로 오프셋을 줍니다.
            //우측 통행을 원하시면 + normal * laneOffset 으로 변경하시면 됩니다.
            position += -normal * laneOffset;

            //Transform 업데이트
            transform.position = position;
            
            if (tangent != Vector3.zero) {
                //원작 느낌을 위해 Slerp 속도를 높여 더 빠르고 쫀득하게 회전하도록 수정
                Quaternion targetRotation = Quaternion.LookRotation(tangent);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 30f);
            }
        }
    }
}
