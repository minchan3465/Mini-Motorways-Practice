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
        [SerializeField] private GameObject CarPinParent; 
        [SerializeField] private GameObject CarPin;
        [SerializeField] private PinView _pinView;

        //private float _pinDisplayTimer = 0f;
        private const float PIN_VISIBLE_DURATION = 1.5f; 
        private VehicleState _lastFrameState = VehicleState.Ready;

		public void Initialize(Vehicle model) {
			_vehicleModel = model;
		}

		public void UpdateColor(int groupIndex) {
            Color color = GroupColor.GetGroupColor(groupIndex);
            CarBody.material.color = color;

            if (_pinView == null && CarPin != null) CarPin.TryGetComponent(out _pinView);
            if (_pinView != null) _pinView.SetColor(color);
        }

        private void Start() {
            if (_vehicleModel == null) TryGetComponent(out _vehicleModel);
            if (CarPin != null) CarPin.TryGetComponent(out _pinView);
            if (_pinView != null) _pinView.SetVisibility(false);
            
            if (_vehicleModel != null) {
                _lastFrameState = _vehicleModel.State;
            }
        }

        private void Update() {
            if (_vehicleModel == null) return;

            //1. 상태 전이(State Transition) 감지하여 정확히 한 번만 호출
            //if (_vehicleModel.State != _lastFrameState) {
            //   OnStateChanged(_lastFrameState, _vehicleModel.State);
            //}

            //2. 비주얼 업데이트 (주행 중일 때만)
            if (_vehicleModel.State == VehicleState.Driving || 
                _vehicleModel.State == VehicleState.Returning) {
                UpdateVisuals();
            } else if (_vehicleModel.State != VehicleState.Arrived) {
                //주차 중이 아닐 때만 차선 정보 초기화
                _prevLane = null;
                _currentLane = null;
            }

            //핀 타이머 관리
            if(_vehicleModel.State == VehicleState.Arrived) {
                if(_vehicleModel.ParkingTimer <= PIN_VISIBLE_DURATION) {
                    if(!CarPin.activeSelf) _pinView.SetVisibility(true);
                } else {
                    if (CarPin.activeSelf) _pinView.SetVisibility(false);
				}
			}



            //if (_pinDisplayTimer > 0) {
            //   _pinDisplayTimer -= Time.deltaTime;
            //   if (_pinDisplayTimer <= 0) {
            //       if (_pinView != null) _pinView.SetVisibility(false);
            //   }
            //}

            //핀의 회전값 고정
            if (CarPinParent != null && CarPin.activeSelf) {
                CarPinParent.transform.rotation = Quaternion.identity;
            }

            _lastFrameState = _vehicleModel.State;
        }

        //상태가 변경되었을 때 딱 한 번만 실행되는 함수
        //private void OnStateChanged(VehicleState oldState, VehicleState newState) {
        //   if (_pinView == null) return;

        //   if (newState == VehicleState.Arrived) {
        //       //목적지에 도착했을 때 핀을 켬
        //       _pinView.SetVisibility(true);
        //       _pinDisplayTimer = PIN_VISIBLE_DURATION; //1.5초 타이머 시작
        //   } else {
        //       //그 외의 상태(출발, 대기 등)에서는 핀을 끔
        //       _pinView.SetVisibility(false);
        //       _pinDisplayTimer = 0f;
        //   }
        //}

        private void UpdateVisuals() {
            Lane activeLane = _vehicleModel.GetCurrentLane();
            if (activeLane == null || activeLane.PathSpline == null) return;

            if (_currentLane != activeLane) {
                _prevLane = _currentLane;
                _currentLane = activeLane;
            }

            float t = Mathf.Clamp01(_vehicleModel.DistanceAlongLane / activeLane.Length);
            Vector3 position = activeLane.PathSpline.Evaluate(t);
            Vector3 tangent = activeLane.PathSpline.EvaluateTangent(t);

            //베지어 곡선 기반 부드러운 코너링 로직
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

            //차량 오프셋 설정
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
