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

		public List<Vehicle> _activeVehicle = new List<Vehicle>();  //�����̴� ���� ����.
		private Dictionary<int, Vehicle> _vehicleMap = new Dictionary<int, Vehicle>();  //Vehicle ID�� ������ ã��.

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

			v.CurrentSpeed = v.MaxSpeed;
			float moveStep = v.CurrentSpeed * deltaTime;

			v.DistanceAlongLane += moveStep;

			// 
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
				//Desination �ǹ����� �˸�. (Event�� ó��)
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;
			} else if (v.State == VehicleState.Returning) {
				//�� �����ߴٰ� �˸�.
				v.State = VehicleState.Ready;

				v.ClearAllReservations();
			}

			// ������ ���� �� ���� ó�� (���� ��)
		}

		private void StartReturnTrip(Vehicle v) {
			if (v.ReturnPath != null && v.ReturnPath.Count > 0) {
				v.CurrentPath = v.ReturnPath;
				v.ReturnPath = new Queue<Lane>();
				v.CurrentLaneIndex = 0;
				v.DistanceAlongLane = 0f;
				v.State = VehicleState.Returning;

				v.GetCurrentLane()?.Enter(v.Id);
				//�� �����ٰ� �˸�.
			}
		}

	}
}
