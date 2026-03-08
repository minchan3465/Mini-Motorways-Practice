using UnityEngine;
using System.Linq;
using Motorways.Models;

namespace Motorways.Views {
    // Vehicle 모델의 논리적 데이터를 바탕으로 시각적인 위치(Transform)를 업데이트합니다.
    public class VehicleView : MonoBehaviour {
        [SerializeField] private Vehicle _vehicleModel;

        private Lane _prevLane;
        private Lane _currentLane;

        [SerializeField] private MeshRenderer CarBody;
        [SerializeField] private GameObject CarPinParent; 
        [SerializeField] private GameObject CarPin; 
        private PinView _pinView;

        private float _pinDisplayTimer = 0f;
        private const float PIN_VISIBLE_DURATION = 1.5f; 
        private VehicleState _lastFrameState;

		public void Initialize(Vehicle model) {
			_vehicleModel = model;
		}

		public void UpdateColor(int groupIndex) {
            Color color = GroupColor.GetGroupColor(groupIndex);
            CarBody.material.color = color;
            
            if (_pinView == null && CarPin != null) _pinView = CarPin.GetComponent<PinView>();
            if (_pinView != null) _pinView.SetColor(color);
        }

        private void Start() {
            if (_vehicleModel == null) TryGetComponent(out _vehicleModel);
            if (CarPin != null) _pinView = CarPin.GetComponent<PinView>();
            if (_pinView != null) _pinView.SetVisibility(false);
        }

        private void Update() {
            if (_vehicleModel == null) return;

            if (_vehicleModel.State == VehicleState.Driving || 
                _vehicleModel.State == VehicleState.Returning || 
                _vehicleModel.State == VehicleState.Arrived) {
                
                if (_vehicleModel.State != VehicleState.Arrived) {
                    UpdateVisuals();
                }
                
                HandleCarPinLogic();
            } else {
                _prevLane = null;
                _currentLane = null;
                if (_pinView != null) _pinView.SetVisibility(false);
                _pinDisplayTimer = 0f;
            }

            _lastFrameState = _vehicleModel.State;
        }

        private void HandleCarPinLogic() {
            if (_pinView == null) return;

            if (_vehicleModel.State == VehicleState.Arrived && _lastFrameState != VehicleState.Arrived) {
                _pinView.SetVisibility(true);
                _pinDisplayTimer = PIN_VISIBLE_DURATION;
            }

            if (_pinDisplayTimer > 0) {
                _pinDisplayTimer -= Time.deltaTime;
                if (_pinDisplayTimer <= 0) _pinView.SetVisibility(false);
            }

            if (CarPinParent != null && CarPin.activeSelf) {
                CarPinParent.transform.rotation = Quaternion.identity;
            }
        }

        // [사용자님의 핵심 주행 비주얼 로직 복구]
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

            // 베지어 곡선 기반 부드러운 코너링 로직
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

            // 사용자님의 차량 오프셋 설정
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
