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

		private void ProcessDriving(Vehicle v, float dt) {
			Lane lane = v.GetCurrentLane();
			if (lane == null) return;

			v.TargetSpeed = v.MaxSpeed;
			float lookAhead = 1.2f * MapSettings.TILE_SIZE;
			float stopDistance = 0.45f * MapSettings.TILE_SIZE;

			// 앞차 체크
			foreach (int otherId in lane.VehiclesOnLane) {
				if (otherId == v.Id) continue;
				Vehicle other = GetVehicle(otherId);
				if (other == null) continue;

				float d = other.DistanceAlongLane - v.DistanceAlongLane;
				if (d > 0 && d < lookAhead) {
					if (d < stopDistance) v.TargetSpeed = 0f;
					else v.TargetSpeed = Mathf.Min(v.TargetSpeed, other.CurrentSpeed * 0.8f);
				}
			}

			// 다음 차선 체크
			Lane nextLane = v.CurrentPath.Count > 1 ? v.CurrentPath.ElementAt(1) : null;
			if (nextLane != null) {
				float distToNode = lane.Length - v.DistanceAlongLane;
				if (distToNode < lookAhead) {
					foreach (int otherId in nextLane.VehiclesOnLane) {
						Vehicle other = GetVehicle(otherId);
						if (other == null) continue;
						float d = distToNode + other.DistanceAlongLane;
						if (d < stopDistance) {
							v.TargetSpeed = 0f;
							break;
						}
					}
				}
			} else {
				float distToDest = lane.Length - v.DistanceAlongLane;
				if (distToDest < lookAhead) {
					v.TargetSpeed = Mathf.Lerp(0.5f, v.MaxSpeed, distToDest / lookAhead);
				}
			}

			v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Acceleration * dt);
			v.DistanceAlongLane += v.CurrentSpeed * dt;

			if (v.DistanceAlongLane >= lane.Length) {
				float overflow = v.DistanceAlongLane - lane.Length;
				
				lane.Exit(v.Id);
				lane.CancelReservation(v.Id);

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
			if (!v.IsReturning) {
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;
				if (v.TargetDestination != null) v.TargetDestination.OnVehicleArrived(v.Id);
			} else {
				v.State = VehicleState.Ready;
				v.IsReturning = false;
				v.ClearAllReservations();
				if (v.HomeObject != null) v.HomeObject.OnVehicleArrived(v.Id);
			}
		}

		private void StartReturnTrip(Vehicle v) {
			v.UseReturnPath();
		}
	}
}
