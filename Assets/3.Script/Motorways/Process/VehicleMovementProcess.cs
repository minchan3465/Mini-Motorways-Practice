using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Process {
	using Models;
	using Managers;
	using Utils;

	public class VehicleMovementProcess : MonoBehaviour, ISimulationProcess {
		public static VehicleMovementProcess Instance;

		public List<Vehicle> _activeVehicle = new List<Vehicle>();  // 현재 이동 중인 차량 리스트
		private Dictionary<int, Vehicle> _vehicleMap = new Dictionary<int, Vehicle>();  // ID로 차량 조회를 위한 맵

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		private void Start() {
			if (SimulationManager.Instance != null) {
				SimulationManager.Instance.RegisterProcess(this);
			}
		}

		public void RegisterVehicle(Vehicle v) {
			if (!_vehicleMap.ContainsKey(v.Id)) {
				_activeVehicle.Add(v);
				_vehicleMap.Add(v.Id, v);
			}
		}
		public Vehicle GetVehicle(int id) {
			_vehicleMap.TryGetValue(id, out Vehicle v);
			return v;
		}

		public void Tick(float dt) {
			Step(dt);
		}

		public void Step(float deltaTime) {
			for (int i = _activeVehicle.Count - 1; i >= 0; i--) {
				Vehicle v = _activeVehicle[i];

				switch (v.State) {
					case VehicleState.Driving:
					case VehicleState.Returning:
						ProcessDriving(v, deltaTime);
						break;
					case VehicleState.Arrived:
						ProcessParking(v, deltaTime);
						break;
				}
			}
		}

		private void ProcessDriving(Vehicle v, float deltaTime) {
			Lane lane = v.GetCurrentLane();
			if (lane == null) return;

			//--- [Step 2 & 3] 가감속 및 충돌 회피 로직 ---
			v.TargetSpeed = v.MaxSpeed;
			float minDistanceToBrake = 0.5f * MapSettings.TILE_SIZE; // 브레이크를 밟기 시작할 전방 거리

			// 1. 같은 레인 앞차와의 간격 유지
			foreach (int otherId in lane.VehiclesOnLane) {
				if (otherId == v.Id) continue;
				Vehicle other = GetVehicle(otherId);
				if (other == null) continue;

				float dist = other.DistanceAlongLane - v.DistanceAlongLane;
				// 자신보다 앞에 있고, 감지 거리 안에 있을 때
				if (dist > 0 && dist < minDistanceToBrake) {
					v.TargetSpeed = Mathf.Min(v.TargetSpeed, other.CurrentSpeed * 0.8f);
				}
			}

			// 2. 다음 레인(교차로) 진입 전 양보 (Yielding)
			if (v.DistanceAlongLane > lane.Length - minDistanceToBrake) {
				Lane nextLane = v.CurrentPath.Count > 1 ? v.CurrentPath.ElementAt(1) : null;
				if (nextLane != null) {
					// 다음 레인에 이미 다른 차가 있다면 (교차로 점유 확인)
					if (nextLane.VehiclesOnLane.Count > 0) {
						v.TargetSpeed = 0f; // 일단 정지하여 양보
					}
				} else {
					// 목적지 도착 직전 감속
					v.TargetSpeed = Mathf.Lerp(0.5f, v.MaxSpeed, (lane.Length - v.DistanceAlongLane) / minDistanceToBrake);
				}
			}

			// 3. 현재 속도 업데이트 (가속/감속 적용)
			float speedDiff = v.TargetSpeed - v.CurrentSpeed;
			if (speedDiff > 0) {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Acceleration * deltaTime);
			} else {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Braking * deltaTime);
			}

			// 이동 수행
			float moveStep = v.CurrentSpeed * deltaTime;
			v.DistanceAlongLane += moveStep;

			// 레인 전환 처리
			if (v.DistanceAlongLane >= lane.Length) {
				float overflow = v.DistanceAlongLane - lane.Length;

				lane.Exit(v.Id);
				lane.CancelReservation(v.Id);
				v.CurrentPath.Dequeue();
				v.DistanceAlongLane = overflow;

				Lane nextLane = v.GetCurrentLane();
				if (nextLane != null) {
					nextLane.Enter(v.Id);
				} else {
					HandleArrival(v);
				}
			}
		}
		private void ProcessParking(Vehicle v, float deltaTime) {
			v.ParkingTimer += deltaTime;
			if (v.ParkingTimer >= v.ParkingDuration) {
				StartReturnTrip(v);
			}
		}

		private void HandleArrival(Vehicle v) {
			if (v.State == VehicleState.Driving) {
				// 목적지 도착 알림 (이벤트 처리 권장)
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;
			} else if (v.State == VehicleState.Returning) {
				// 집으로 귀환 완료
				v.State = VehicleState.Ready;

				v.ClearAllReservations();
			}
		}

		private void StartReturnTrip(Vehicle v) {
			if (v.ReturnPath != null && v.ReturnPath.Count > 0) {
				v.CurrentPath = v.ReturnPath;
				v.ReturnPath = new Queue<Lane>();
				v.CurrentLaneIndex = 0;
				v.DistanceAlongLane = 0f;
				v.State = VehicleState.Returning;

				v.GetCurrentLane()?.Enter(v.Id);
			}
		}

	}
}
