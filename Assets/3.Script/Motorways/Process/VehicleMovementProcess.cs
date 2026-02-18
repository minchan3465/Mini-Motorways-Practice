using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Process {
	using Motorways.Models;

	public class VehicleMovementProcess : MonoBehaviour {
		public static VehicleMovementProcess Instance;

		public List<Vehicle> _activeVehicle = new List<Vehicle>();	//움직이는 차량 모음.
		private Dictionary<int, Vehicle> _vehicleMap = new Dictionary<int, Vehicle>();	//Vehicle ID로 빠르게 찾기.

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
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

		private void Update() {
			float deltaTime = Time.deltaTime;
			Step(deltaTime);
		}

		public void Step(float deltaTime) {
			for(int i = _activeVehicle.Count - 1; i >= 0; i--) {
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
			UpdateVisualPosition(v, lane);

			if(v.DistanceAlongLane >= lane.Length) {
				float overflow = v.DistanceAlongLane - lane.Length;

				lane.Exit(v.Id);

				v.CurrentPath.Dequeue();
				v.DistanceAlongLane = overflow;

				Lane nextLane = v.GetCurrentLane();
				if(nextLane != null) {
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
			if(v.State == VehicleState.Driving) {
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;
				
				//Desination 건물에게 알림. (Event로 처리)
			} else if (v.State == VehicleState.Returning) {
				v.State = VehicleState.Ready;
                v.CurrentPath = new Queue<Lane>();
                v.ReturnPath = new Queue<Lane>();

                //집 도착했다고 알림.
            }

			// 목적지 도달 시 로직 처리 (주차 등)
		}

		private void StartReturnTrip(Vehicle v) {
			if(v.ReturnPath != null && v.ReturnPath.Count > 0) {
				v.CurrentPath = v.ReturnPath;
				v.ReturnPath = new Queue<Lane>();
				v.CurrentLaneIndex = 0;
				v.DistanceAlongLane = 0f;
				v.State = VehicleState.Returning;

				v.GetCurrentLane()?.Enter(v.Id);
				//차 나간다고 알림.
			}
		}

		private void UpdateVisualPosition(Vehicle v, Lane lane) {
			float t = v.DistanceAlongLane / lane.Length;
			Vector3 startPos = new Vector3(lane.StartNode.x + 0.5f, 0, lane.StartNode.y + 0.5f);
			Vector3 endPos = new Vector3(lane.EndNode.x + 0.5f, 0, lane.EndNode.y + 0.5f);

			v.transform.position = Vector3.Lerp(startPos, endPos, t);
			if (startPos != endPos) {
				v.transform.rotation = Quaternion.LookRotation(endPos - startPos);
			}
		}
	}
}
