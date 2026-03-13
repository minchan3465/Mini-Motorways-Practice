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

			//앞차 체크
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

			//다음 차선 (교차로 진입) 및 앞차 체크
			Lane nextLane = v.CurrentPath.Count > 1 ? v.CurrentPath.ElementAt(1) : null;
			if (nextLane != null) {
				float distToNode = lane.Length - v.DistanceAlongLane;

				// 1. 다음 차선에 있는 앞차와의 거리 체크 (기존 로직)
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

				// 2. 교차로 진입 전 충돌(Intersect) 검사
				// 타일 중앙(노드)에 다다르기 직전일 때, 진입하려는 타일을 통과 중인 다른 차량들과 경로가 겹치는지 확인합니다.
				float intersectionCheckDistance = 0.6f * MapSettings.TILE_SIZE;
				if (distToNode < intersectionCheckDistance && v.TargetSpeed > 0f) {
					if (MapManager.Instance._grid.TryGetValue(lane.EndNode, out TileData intersectionTile)) {
						
						TileDirection myInDir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);
						TileDirection myOutDir = TileUtils.GetDirection(nextLane.StartNode, nextLane.EndNode);

						// 타일에 연결된 8방향 도로들 중, 누군가 주행 중인 도로를 모두 검사합니다.
						bool shouldYield = false;
						for (int i = 0; i < 8; i++) {
							Lane intersectingLane = intersectionTile.Lanes[i];
							if (intersectingLane == null) continue;

							// 내 진행 방향과 같거나 내가 진입할 다음 차선 자체라면 교차 검사를 할 필요 없음
							if (intersectingLane == lane || intersectingLane == nextLane) continue;

							// 교차하는 도로 위에 차가 있는지 확인
							if (intersectingLane.VehiclesOnLane.Count > 0) {
								foreach (int otherId in intersectingLane.VehiclesOnLane) {
									Vehicle otherVehicle = GetVehicle(otherId);
									if (otherVehicle == null || otherVehicle.State != VehicleState.Driving && otherVehicle.State != VehicleState.Returning) continue;

									// 상대방 차량의 다음 경로 찾기
									Lane otherNextLane = null;
									if (otherVehicle.CurrentPath.Count > 1) {
										// 현재 타일 중앙을 향해 오고 있는 차량이라면
										if (otherVehicle.GetCurrentLane() == intersectingLane && intersectingLane.EndNode == lane.EndNode) {
											otherNextLane = otherVehicle.CurrentPath.ElementAt(1);
										}
									}

									// 상대도 이 교차로를 지나가는 중이라면 서로의 경로가 기하학적으로 X자로 겹치는지 판별합니다.
									if (otherNextLane != null) {
										TileDirection otherInDir = TileUtils.GetDirection(intersectingLane.StartNode, intersectingLane.EndNode);
										TileDirection otherOutDir = TileUtils.GetDirection(otherNextLane.StartNode, otherNextLane.EndNode);

										// 일자 길에서 마주오는 경우는 예외 처리 (방향이 정반대면 교차가 아님)
										if (myInDir == otherOutDir && myOutDir == otherInDir) {
											continue;
										}

										// X자로 겹치는지 또는 같은 출구로 나가는지 검사
										if (TileUtils.ConnectionsIntersect(myInDir, myOutDir, otherInDir, otherOutDir)) {
											// 거리가 가까운 차량이 먼저 지나가도록 간단한 거리 기반 양보 우선순위 적용
											float myTime = distToNode / Mathf.Max(0.1f, v.CurrentSpeed);
											float otherDist = intersectingLane.Length - otherVehicle.DistanceAlongLane;
											float otherTime = otherDist / Mathf.Max(0.1f, otherVehicle.CurrentSpeed);

											// 내가 더 늦게 도착할 것 같거나, 거리가 비슷할 때 ID가 큰 쪽이 양보(데드락 방지)
											if (myTime > otherTime - 0.2f || (Mathf.Abs(myTime - otherTime) < 0.2f && v.Id > otherId)) {
												shouldYield = true;
												break;
											}
										}
									}
								}
							}
							if (shouldYield) break;
						}

						// 충돌이 예상되면 교차로 진입 직전(distToNode 가 0에 가까울 때)에 정지합니다.
						if (shouldYield) {
							if (distToNode < stopDistance) {
								v.TargetSpeed = 0f;
							} else {
								v.TargetSpeed = Mathf.Lerp(0f, v.MaxSpeed, (distToNode - stopDistance) / intersectionCheckDistance);
							}
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
