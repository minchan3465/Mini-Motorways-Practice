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
			Step(dt);
		}

		public void Step(float deltaTime) {
			// Phase 1: 데이터 초기화 (매 틱 모든 노드의 정보 갱신)
			foreach (var tile in MapManager.Instance._grid.Values) {
				tile.InboundVehicles.Clear();
			}

			// Phase 2: 대기열 선등록 (모든 차량이 자신의 목적지 노드에 예약표를 던짐)
			foreach (var v in _activeVehicle) {
				if (v.State == VehicleState.Driving || v.State == VehicleState.Returning) {
					RegisterIntersectionInbound(v);
				}
			}

			// Phase 3: 정렬 (원작의 SortInboundVehicles - 거리순으로 우선순위 확정)
			foreach (var tile in MapManager.Instance._grid.Values) {
				if (tile.InboundVehicles.Count > 1) {
					// IsShoving(억지 진입) 상태인 차량을 최우선으로, 그 뒤를 거리순으로 정렬합니다.
					tile.InboundVehicles.Sort((a, b) => {
						if (a.IsShoving != b.IsShoving) return b.IsShoving.CompareTo(a.IsShoving);
						return a.DistanceToNode.CompareTo(b.DistanceToNode);
					});
				}
			}

			// Phase 4: 차량 주행 연산
			List<Vehicle> stuckVehicles = new List<Vehicle>();
			for (int i = _activeVehicle.Count - 1; i >= 0; i--) {
				Vehicle v = _activeVehicle[i];

				switch (v.State) {
					case VehicleState.Driving:
					case VehicleState.Returning:
						ProcessDriving(v, deltaTime);
						
						// 속도가 0에 가깝다면 정체 목록에 추가 (교착 해제용)
						if (v.CurrentSpeed < 0.01f && v.TargetSpeed < 0.01f) {
							stuckVehicles.Add(v);
						} else {
							v.StuckTimer = 0f;
							v.IsShoving = false;
						}
						break;
					case VehicleState.Arrived:
						ProcessParking(v, deltaTime);
						break;
				}
			}

			// Phase 5: 교착 상태 감지 및 해제
			if (stuckVehicles.Count > 0) {
				foreach (var v in stuckVehicles) {
					v.StuckTimer += deltaTime;
					if (v.StuckTimer > 2.5f) { 
						CheckAndBreakCycle(v);
					}
				}
			}
		}

		private void ProcessDriving(Vehicle v, float deltaTime) {
			Lane lane = v.GetCurrentLane();
			if (lane == null) return;

			v.TargetSpeed = v.MaxSpeed;
			v.BlockingVehicleId = -1;

			// 상숫값은 원작의 스케일에 맞게 조정 (1.5타일 앞을 보고, 0.4타일 이내면 멈춤)
			float lookAheadDistance = 1.5f * MapSettings.TILE_SIZE;
			float stopDistance = 0.4f * MapSettings.TILE_SIZE;

			// 1. 전방 차량 체크 (같은 차선)
			Vehicle leadingVehicle = null;
			float distToLeading = float.MaxValue;

			foreach (int otherId in lane.VehiclesOnLane) {
				if (otherId == v.Id) continue;
				Vehicle other = GetVehicle(otherId);
				if (other == null) continue;

				float d = other.DistanceAlongLane - v.DistanceAlongLane;
				if (d > 0 && d < distToLeading) {
					distToLeading = d;
					leadingVehicle = other;
				}
			}

			// 2. 교차로 진입 판정 (Intersection Entry)
			Lane nextLane = v.CurrentPath.Count > 1 ? v.CurrentPath.ElementAt(1) : null;
			float distToNode = lane.Length - v.DistanceAlongLane;

			if (distToNode < lookAheadDistance) {
				if (nextLane != null) {
					// [원작 방식] 교차로 진입 규칙 체크
					if (!v.IsShoving && !CanInboundEnter(v, lane, nextLane, out int blockingId)) {
						v.TargetSpeed = 0f;
						v.BlockingVehicleId = blockingId;
					} else if (leadingVehicle == null) {
						// 내가 진입할 수 있을 때만 다음 차선의 앞차를 확인하여 속도를 맞춥니다.
						foreach (int otherId in nextLane.VehiclesOnLane) {
							Vehicle other = GetVehicle(otherId);
							if (other == null) continue;
							float d = distToNode + other.DistanceAlongLane;
							if (d < distToLeading) {
								distToLeading = d;
								leadingVehicle = other;
							}
						}
					}
				} else {
					// 목적지 도착 전 감속
					v.TargetSpeed = Mathf.Lerp(0.5f, v.MaxSpeed, distToNode / lookAheadDistance);
				}
			}

			// 3. 앞차 속도 추종 (Cruise Control)
			if (leadingVehicle != null && distToLeading < lookAheadDistance) {
				v.BlockingVehicleId = leadingVehicle.Id;
				if (distToLeading < stopDistance) {
					v.TargetSpeed = 0f;
				} else {
					// 거리 비례 부드러운 감속
					float factor = Mathf.Clamp01((distToLeading - stopDistance) / (lookAheadDistance - stopDistance));
					v.TargetSpeed = Mathf.Min(v.TargetSpeed, leadingVehicle.CurrentSpeed * factor);
				}
			}

			// 4. 가감속 및 이동
			float speedDiff = v.TargetSpeed - v.CurrentSpeed;
			if (speedDiff > 0) {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Acceleration * deltaTime);
			} else {
				v.CurrentSpeed = Mathf.MoveTowards(v.CurrentSpeed, v.TargetSpeed, v.Braking * deltaTime * MapSettings.TILE_SIZE);
			}

			float moveStep = v.CurrentSpeed * deltaTime;
			v.DistanceAlongLane += moveStep;

			// 5. 노드 통과 및 차선 전환 (Mothball 관련 예약 해제 필수)
			if (v.DistanceAlongLane >= lane.Length) {
				float overflow = v.DistanceAlongLane - lane.Length;
				
				// [수정] 차선을 떠나는 즉시 예약을 해제해야 Mothballed 도로가 정상적으로 소멸됩니다.
				lane.Exit(v.Id);
				lane.CancelReservation(v.Id); 
				
				v.CurrentPath.Dequeue();
				v.DistanceAlongLane = overflow;

				Lane next = v.GetCurrentLane();
				if (next != null) {
					next.Enter(v.Id);
				} else {
					HandleArrival(v);
				}
			}
		}

		private void RegisterIntersectionInbound(Vehicle v) {
			Lane currLane = v.GetCurrentLane();
			Lane nextLane = v.CurrentPath.Count > 1 ? v.CurrentPath.ElementAt(1) : null;
			if (currLane == null || nextLane == null) return;

			Vector2Int nodeCoord = currLane.EndNode;
			if (MapManager.Instance._grid.TryGetValue(nodeCoord, out TileData tile)) {
				tile.InboundVehicles.Add(new InboundVehicle {
					VehicleId = v.Id,
					FromDir = TileUtils.GetOppositeDirection(TileUtils.GetDirection(currLane.StartNode, currLane.EndNode)),
					ToDir = TileUtils.GetDirection(nextLane.StartNode, nextLane.EndNode),
					DistanceToNode = currLane.Length - v.DistanceAlongLane,
					IsShoving = v.IsShoving
				});
			}
		}

		private bool CanInboundEnter(Vehicle v, Lane currLane, Lane nextLane, out int blockingId) {
			blockingId = -1;
			Vector2Int nodeCoord = currLane.EndNode;
			if (!MapManager.Instance._grid.TryGetValue(nodeCoord, out TileData tile)) return true;

			TileDirection myIn = TileUtils.GetOppositeDirection(TileUtils.GetDirection(currLane.StartNode, currLane.EndNode));
			TileDirection myOut = TileUtils.GetDirection(nextLane.StartNode, nextLane.EndNode);

			// 1. 원작의 CanTraversingVehicleContinue: 교차로 내부(노드에서 나가는 중)인 차량 체크
			foreach (var otherLane in RoadNetworkManager.Instance.AllLanes) {
				// 이 노드에서 '시작'하여 나가는 차선들을 확인
				if (otherLane.StartNode == nodeCoord) {
					foreach (int otherId in otherLane.VehiclesOnLane) {
						Vehicle other = GetVehicle(otherId);
						if (other == null || other.Id == v.Id) continue;
						
						// 노드 진입점으로부터 차량 길이(0.4타일) 내에 있다면 '교차로 점유' 상태로 간주
						if (other.DistanceAlongLane < 0.45f * MapSettings.TILE_SIZE) {
							// 같은 출구로 나가는 중(Merging)이거나, 경로가 교차하면 양보
							TileDirection oOut = TileUtils.GetDirection(otherLane.StartNode, otherLane.EndNode);
							if (myOut == oOut) {
								blockingId = other.Id;
								return false;
							}
							
							// 다른 입구(In)를 알 수 없으므로, 현재 차선 방향을 기준으로 경로 충돌 체크
							TileDirection oIn = TileUtils.GetOppositeDirection(oOut); // 단순화된 충돌 체크
							if (TileUtils.ConnectionsIntersect(myIn, myOut, oIn, oOut)) {
								blockingId = other.Id;
								return false;
							}
						}
					}
				}
			}

			// 2. 대기열 우선순위 체크 (정렬된 InboundVehicles 사용)
			foreach (var other in tile.InboundVehicles) {
				if (other.VehicleId == v.Id) {
					// 내 순례가 왔는데, 앞에 나를 막는 차가 없었다면 진입 허가!
					return true;
				}

				// 상대방이 Shoving 중이면 무조건 양보
				if (other.IsShoving) {
					blockingId = other.VehicleId;
					return false;
				}

				// 경로가 교차한다면, 리스트 상 나보다 앞에 있는(더 가까운) 차량에게 양보
				if (TileUtils.ConnectionsIntersect(myIn, myOut, other.FromDir, other.ToDir)) {
					blockingId = other.VehicleId;
					return false;
				}
			}

			return true;
		}

		private void CheckAndBreakCycle(Vehicle startV) {
			HashSet<int> visited = new HashSet<int>();
			Vehicle curr = startV;

			while (curr != null && !visited.Contains(curr.Id)) {
				visited.Add(curr.Id);
				if (curr.BlockingVehicleId == -1) break;
				curr = GetVehicle(curr.BlockingVehicleId);
				
				if (curr != null && curr.Id == startV.Id) {
					startV.IsShoving = true; // 순환 고리 발견 시 강제 진입권 부여
					break;
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
			// [수정] IsReturning 플래그를 기준으로 도착지 또는 집 판정
			if (!v.IsReturning) {
				// 목적지 도착
				v.State = VehicleState.Arrived;
				v.ParkingTimer = 0f;

				if (MapManager.Instance._grid.TryGetValue(v.DestNode, out TileData targetTile)) {
					if (targetTile.Building != null) {
						targetTile.Building.OnVehicleArrived(v.Id);
					}
				}
			} else {
				// 집 도착
				v.State = VehicleState.Ready;
				v.ClearAllReservations();

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
