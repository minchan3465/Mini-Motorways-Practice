using UnityEngine;
using System.Linq;
using Motorways.Models;

namespace Motorways.Views {
    //Vehicle 모델의 논리적 데이터를 바탕으로 시각적인 위치(Transform)를 업데이트합니다.
    public class VehicleView : MonoBehaviour {
        [SerializeField] private Vehicle _vehicleModel;

        private Lane _prevLane;
        private Lane _currentLane;

        [SerializeField] private MeshRenderer CarBody;
        [SerializeField] private GameObject CarPinParent; // 지붕 위 핀 오브젝트 방향용
        [SerializeField] private GameObject CarPin; // 지붕 위 핀 오브젝트

        private float _pinDisplayTimer = 0f;
        private const float PIN_VISIBLE_DURATION = 0.3f; // 귀환 시작 후 핀이 유지될 시간
        private VehicleState _lastFrameState;

        //----------------------- 메서드

		public void Initialize(Vehicle model) {
			_vehicleModel = model;
		}

		public void UpdateColor(int groupIndex) {
            Color color = GroupColor.GetGroupColor(groupIndex);
            CarBody.material.color = color;
            
            if (CarPin != null) {
                if (CarPin.TryGetComponent(out PinView pv)) {
                    pv.SetColor(color);
                }
            }
        }

        private void Start() {
            if (_vehicleModel == null) {
                TryGetComponent(out _vehicleModel);
            }
            // 시작 시 핀은 꺼둡니다.
            if (CarPin != null) CarPin.SetActive(false);
        }
        private void Update() {
            if (_vehicleModel == null) return;

            // 차량 활성화 상태 체크
            bool isActiveState = (_vehicleModel.State == VehicleState.Driving || 
                                  _vehicleModel.State == VehicleState.Returning || 
                                  _vehicleModel.State == VehicleState.Arrived);

            if (isActiveState) {
                if (_vehicleModel.State != VehicleState.Arrived) {
                    UpdateVisuals();
                }
                
                HandleCarPinLogic();
            } else {
                _prevLane = null;
                _currentLane = null;
                if (CarPin != null && CarPin.activeSelf) CarPin.SetActive(false);
                _pinDisplayTimer = 0f;
            }

            _lastFrameState = _vehicleModel.State;
        }

        private void HandleCarPinLogic() {
            if (CarPin == null) return;

            // 1. 등장 조건: 목적지 주차 시작(Arrived로 상태 전환되는 순간)에만 핀을 켜고 타이머를 시작.
            if (_vehicleModel.State == VehicleState.Arrived && _lastFrameState != VehicleState.Arrived) {
                if (!CarPin.activeSelf) CarPin.SetActive(true);
                _pinDisplayTimer = PIN_VISIBLE_DURATION;
            }

            // 2. 소멸 로직: 핀이 켜져 있다면 상태와 관계없이 타이머를 깎습니다.
            if (CarPin.activeSelf) {
                _pinDisplayTimer -= Time.deltaTime;
                if (_pinDisplayTimer <= 0) {
                    CarPin.SetActive(false);
                }
            }

            // 3. 방향 고정 (Billboard): 사용자님의 기존 설정(Quaternion.identity) 유지
            if (CarPin.activeSelf && CarPinParent != null) {
                CarPinParent.transform.rotation = Quaternion.identity;
            }
        }

        private void UpdateVisuals() {
            Lane activeLane = _vehicleModel.GetCurrentLane();
            
            if (activeLane == null || activeLane.PathSpline == null) return;

            if (_currentLane != activeLane) {
                _prevLane = _currentLane;
                _currentLane = activeLane;
            }

            float t = _vehicleModel.DistanceAlongLane / activeLane.Length;
            t = Mathf.Clamp01(t);

            Vector3 position = activeLane.PathSpline.Evaluate(t);
            Vector3 tangent = activeLane.PathSpline.EvaluateTangent(t);

            Lane nextLane = _vehicleModel.CurrentPath.Count > 1 ? _vehicleModel.CurrentPath.ElementAt(1) : null;

            if (t > 0.5f && nextLane != null && nextLane.PathSpline != null) {
                Vector3 p0 = activeLane.PathSpline.Evaluate(0.5f);
                Vector3 p1 = activeLane.PathSpline.Evaluate(1.0f);
                Vector3 p2 = nextLane.PathSpline.Evaluate(0.5f);

                float s = t - 0.5f; 
                position = Utils.BezierUtils.GetPoint(p0, p1, p2, s);
                tangent = Utils.BezierUtils.GetTangent(p0, p1, p2, s);

            } else if (t <= 0.5f && _prevLane != null && _prevLane.PathSpline != null && _prevLane.EndNode == activeLane.StartNode) {
                Vector3 p0 = _prevLane.PathSpline.Evaluate(0.5f);
                Vector3 p1 = _prevLane.PathSpline.Evaluate(1.0f);
                Vector3 p2 = activeLane.PathSpline.Evaluate(0.5f);

                float s = 0.5f + t;
                position = Utils.BezierUtils.GetPoint(p0, p1, p2, s);
                tangent = Utils.BezierUtils.GetTangent(p0, p1, p2, s);
            }

            float laneOffset = 0.2f;
            Vector3 normal = Vector3.Cross(Vector3.up, tangent).normalized;
            
            position += -normal * laneOffset;
            position += Vector3.up * 0.2f;

            transform.position = position;
            
            if (tangent != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(tangent);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 30f);
            }
        }
    }
}
