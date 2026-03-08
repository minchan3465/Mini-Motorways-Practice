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

		public List<Vehicle> _activeVehicle = new List<Vehicle>();  
		private Dictionary<int, Vehicle> _vehicleMap = new Dictionary<int, Vehicle>(); 

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
			for (int i = _activeVehicle.Count - 1; i >= 0; i--) {
				Vehicle v = _activeVehicle[i];

				switch (v.State) {
					case VehicleState.Driving:
					case VehicleState.Returning:
						ProcessDriving(v, dt);
						break;
					case VehicleState.Arrived:
						ProcessParking(v, dt);
						break;
				}
			}
		}

		// [사용자님의 앞차 거리 유지 로직 복구]
		private void ProcessDriving(Vehicle v, float dt) {
			Lane lane = v.GetCurrentLane();
			if (lane == null) return;

			v.TargetSpeed = v.MaxSpeed;
			float minDistanceToBrake = 0.5f * MapSettings.TILE_SIZE;

			// 앞차와의 간격 유지 및 부드러운 감속 로직 (사용자님 원본)
			foreach (int otherId in lane.VehiclesOnLane) {
				if (otherId == v.Id) continue;
				Vehicle other = GetVehicle(otherId);
				if (other == null) continue;

				float dist = other.DistanceAlongLane - v.DistanceAlongLane;
				if (dist > 0 && dist < minDistanceToBrake) {
					// 앞차 속도의 80%로 줄여서 거리를 벌림
					v.TargetSpeed = Mathf.Min(v.TargetSpeed, other.CurrentSpeed * 0.8f);
				}
			}

			// 교차로 진입 대기 로직 (초기 안정 버전)
			if (v.DistanceAlongLane > lane.Length - minDistanceToBrake) {
				Lane nextLane = v.CurrentPath.Count > 1 ? v.CurrentPath.ElementAt(1) : null;
				if (nextLane != null) {
					if (nextLane.VehiclesOnLane.Count > 0) {
						v.TargetSpeed = 0f;
					}
				} else {
					// 목적지 도착 감속
					v.TargetSpeed = Mathf.Lerp(0.5f, v.MaxSpeed, (lane.Length - v.DistanceAlongLane) / minDistanceToBrake);
				}
			}

			float speedDiff = v.TargetSpeed - v.CurrentSpeed;
			if (speedDiff > 0) {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Acceleration * dt);
			} else {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Braking * dt);
			}

			v.DistanceAlongLane += v.CurrentSpeed * dt;

			if (v.DistanceAlongLane >= lane.Length) {
				float overflow = v.DistanceAlongLane - lane.Length;

				lane.Exit(v.Id);
				lane.CancelReservation(v.Id); // Mothball 도로 삭제 보장
				v.CurrentPath.Dequeue();
				v.DistanceAlongLane = overflow;

				Lane next = v.GetCurrentLane();
				if (next != null) next.Enter(v.Id);
				else HandleArrival(v);
			}
		}

		private void ProcessParking(Vehicle v, float dt) {
			v.ParkingTimer += dt;
			if (v.ParkingTimer >= v.ParkingDuration) {
				StartReturnTrip(v);
			}
		}

		private void HandleArrival(Vehicle v) {
			if (v.State == VehicleState.Driving) {
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;

				if (MapManager.Instance._grid.TryGetValue(v.DestNode, out TileData targetTile)) {
					if (targetTile.Building != null) targetTile.Building.OnVehicleArrived(v.Id);
				}
			} else if (v.State == VehicleState.Returning) {
				v.State = VehicleState.Ready;
				v.ClearAllReservations();

				if (MapManager.Instance._grid.TryGetValue(v.HomeNode, out TileData targetTile)) {
					if (targetTile.Building != null) targetTile.Building.OnVehicleArrived(v.Id);
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
