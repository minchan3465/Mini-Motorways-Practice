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

			v.TargetSpeed = v.MaxSpeed;
			float minDistanceToBrake = 0.5f * MapSettings.TILE_SIZE;

			foreach (int otherId in lane.VehiclesOnLane) {
				if (otherId == v.Id) continue;
				Vehicle other = GetVehicle(otherId);
				if (other == null) continue;

				float dist = other.DistanceAlongLane - v.DistanceAlongLane;
				if (dist > 0 && dist < minDistanceToBrake) {
					v.TargetSpeed = Mathf.Min(v.TargetSpeed, other.CurrentSpeed * 0.8f);
				}
			}

			if (v.DistanceAlongLane > lane.Length - minDistanceToBrake) {
				Lane nextLane = v.CurrentPath.Count > 1 ? v.CurrentPath.ElementAt(1) : null;
				if (nextLane != null) {
					if (nextLane.VehiclesOnLane.Count > 0) {
						v.TargetSpeed = 0f;
					}
				} else {
					v.TargetSpeed = Mathf.Lerp(0.5f, v.MaxSpeed, (lane.Length - v.DistanceAlongLane) / minDistanceToBrake);
				}
			}

			float speedDiff = v.TargetSpeed - v.CurrentSpeed;
			if (speedDiff > 0) {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Acceleration * deltaTime);
			} else {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Braking * deltaTime);
			}

			float moveStep = v.CurrentSpeed * deltaTime;
			v.DistanceAlongLane += moveStep;

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
				// 목적지 도착
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;

				// 목적지 건물에 도착 알림
				if (MapManager.Instance._grid.TryGetValue(v.DestNode, out TileData targetTile)) {
					if (targetTile.Building != null) {
						targetTile.Building.OnVehicleArrived(v.Id);
					}
				}
			} else if (v.State == VehicleState.Returning) {
				// 집으로 귀환 완료
				v.State = VehicleState.Ready;
				v.ClearAllReservations();

				// 집 건물에 귀환 알림
				if (MapManager.Instance._grid.TryGetValue(v.HomeNode, out TileData targetTile)) {
					if (targetTile.Building != null) {
						targetTile.Building.OnVehicleArrived(v.Id);
					}
				}
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
			} else {
				v.State = VehicleState.Ready;
				v.ClearAllReservations();
			}
		}

	}
}
