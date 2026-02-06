using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;
	using Core.Utils;
	using Core.Managers;

	//TODO : 기능화로 바꾸기 (형태 변경 x 상태로 만드는게 제일 BEST.)

	public class RoadSystem : MonoBehaviour {
		public static RoadSystem Instance = null;

		public int LatestMapUpdateFrame { get; private set; }

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		// 두 지점을 도로로 연결하고 비트마스크를 갱신.
		public void ConnectRoads(Vector2Int from, Vector2Int to) {
			//sqrMagnitude가 distance보다 훨~씬 빠름.
			//if ((from - to).sqrMagnitude > 2.25f) return;
			//근데 정확한 거리가 필요하면, Distance로 하는게 좋다네요. (어짜피 sqrMagninute와 다른점은 제곱근이냐 아니냐)
			if (Vector2Int.Distance(from, to) > 1.5f) return;
			//도로가 1개라도 있지 않는다면.
			if (!ResourceManager.Instance.HasResource(ItemType.Road, 1)) return;

			CreateRoadNode(from);
			CreateRoadNode(to); //목적지에 데이터가 없으면 도로 노드 생성

			if (!IsRoadBuildable(from) || !IsRoadBuildable(to)) return; //건설이 가능한지?

			Vector2Int dirVec = to - from;
			RoadDirection dirToTarget = DirUtiles.GetVectorDir(dirVec);
			RoadDirection dirToOrigin = DirUtiles.GetVectorDir(-dirVec);

			//[중복 체크 로직 추가]
			//이미 해당 방향으로 연결되어 있다면 자원 소모 X, false 반환?
			//아니면 그냥 덮어쓰기하고 false 반환.
			if (IsConnected(from, dirToTarget) && IsConnected(to, dirToOrigin)) return;

			//위의 마스크 계산간거 적용
			UpdateConnectionMask(from, dirToTarget, add: true);
			UpdateConnectionMask(to, dirToOrigin, add: true);

			LatestMapUpdateFrame = Time.frameCount;

			//Connect에서는 자원 차감이 없습니다.
			if (StructureManager.Instance != null) {
				StructureManager.Instance.CheckPendingRequests();
			}
		}
		public bool CreateRoadNode(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData existing)) {
				if (existing.Type.Equals(TileLogicType.Empty)) {
					if(ResourceManager.Instance.TryConsumeResource(ItemType.Road)) {
						existing.Type = TileLogicType.Road;
						existing.IsPendingRemoval = false;
						MapBootstrapper.Grid[coord] = existing;
						LatestMapUpdateFrame = Time.frameCount;
						return true;
					}
					return false;
				} else if (existing.Type.Equals(TileLogicType.Road)) {
					//이게 왜 있냐? -> 도로가 삭제 대기 기능이 추가되었기 때문.
					if(existing.IsPendingRemoval) {
						//만약 삭제 대기 중이였다면. 다시 부활시켜야죠.
						existing.IsPendingRemoval = false;
						MapBootstrapper.Grid[coord] = existing;
						//자원을 다시 소모해야할까?
						//-> RemoveRoad에서 자원 소모를 했다면, 다시 설치하는게 맞긴 함.
						//-> 근데 원작에서는 진짜 회수가 될때 자원 추가가 되니까 Skip.
						
						//+ 시각 업데이트.
					}
					return false;
				} else if (existing.Type.Equals(TileLogicType.Entrance)) {
					//건물 입구면 도로 설치는 안하니 false 반환.
					if (existing.ConnectionMask.Equals(0)) existing.ConnectionMask = RoadDirection.None;
					else return false;
				}
			} else {
				if(ResourceManager.Instance.TryConsumeResource(ItemType.Road)) {
					CellData newRoad = new CellData {
						Coordinate = coord,
						Type = TileLogicType.Road,
						ConnectionMask = RoadDirection.None,
						IsPendingRemoval = false
					};
					MapBootstrapper.Grid.Add(coord, newRoad);
					LatestMapUpdateFrame = Time.frameCount;
					return true;
				}
			}
			return false;
		}
		public void RemoveRoad(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData target)) {
				if (target.Type != TileLogicType.Road) return;
				if (target.IsPendingRemoval) return; //이미 삭제 대기중이면 넘기쇼
				if (IsRoadInUse(coord)) {
					target.IsPendingRemoval = true;
					MapBootstrapper.Grid[coord] = target;

					//자원 반환 안함;
					//ResourceManager.Instance.AddResource(ItemType.Road, 1);
				} else {
					ForceRemoveRoad(coord, target);
				}
			}
		}

		//차량이 타일을 벗어날 때 호출하는 함수...
		public void NotifyCarExitedTile(Vector2Int coord) {
			if(MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if(data.IsPendingRemoval) {
					//만약 삭제 대기 중인 도로인가?
					if(!IsRoadInUse(coord)) {
						//진짜 마지막으로 확인해보고 삭제합니다.
						ForceRemoveRoad(coord, data);

					}
				}
			}
		}

		public bool IsRoadBuildable(Vector2Int coord) {
			if(MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				//도로거나 빈 땅여야 합니다.

				return data.Type.Equals(TileLogicType.Road) || 
					   data.Type.Equals(TileLogicType.Empty) ||
					   data.Type.Equals(TileLogicType.Entrance);
			}
			return true;
		}
		//------------------- 보조들
		private void CheckAndRemoveVisual(Vector2Int center, Vector2Int offset) {
			//나중에 추가할거
			Vector2Int neighbor = center + offset;
		}

		private bool IsConnected(Vector2Int pos, RoadDirection dir) {
			if (MapBootstrapper.Grid.TryGetValue(pos, out CellData data)) {
				return (data.ConnectionMask & dir) != 0;
			}
			return false;
		}
		private bool IsRoadInUse(Vector2Int coord) {
			//현재 모든 차량의 경로를 뒤져서 이 타일을 쓰는지 확인
			// 성능이 걱정되지만, 차량이 100~200대여도 단순 Loop라 프레임 드랍은 거의 없음.

			// StructureManager가 모든 차를 알고 있다고 가정 (혹은 House를 순회)
			// 편의상 FindObjectsOfType을 쓰거나, Manager에 Car List를 관리하는 게 좋음.
			// 여기서는 최적화를 위해 StructureManager에 `List<CarMovement> AllCars`를 추가했다고 가정.

			// 임시: 씬의 모든 차 검색 (나중에 Manager 리스트로 교체 권장)

			var cars = FindObjectsOfType<CarMovement>();

			foreach (var car in cars) {
				if (car.IsUsingTile(coord)) return true;
			}
			return false;

		}
		//진짜 없에는 코드.
		private void ForceRemoveRoad(Vector2Int coord, CellData target) {
			target.Type = TileLogicType.Empty;
			RoadDirection oldMask = target.ConnectionMask;
			target.ConnectionMask = RoadDirection.None;
			MapBootstrapper.Grid[coord] = target;

			UpdateNeighborsOnRemove(coord, oldMask);

			LatestMapUpdateFrame = Time.frameCount;

			CheckAndRemoveVisual(coord, new Vector2Int(0, 1));  // North
			CheckAndRemoveVisual(coord, new Vector2Int(0, -1)); // South
			CheckAndRemoveVisual(coord, new Vector2Int(1, 0));  // East
			CheckAndRemoveVisual(coord, new Vector2Int(-1, 0)); // West
																// 대각선 포함
			CheckAndRemoveVisual(coord, new Vector2Int(1, 1));
			CheckAndRemoveVisual(coord, new Vector2Int(1, -1));
			CheckAndRemoveVisual(coord, new Vector2Int(-1, -1));
			CheckAndRemoveVisual(coord, new Vector2Int(-1, 1));

			ResourceManager.Instance.AddResource(ItemType.Road, 1);
		}
		//도로가 하나만 존재한다면 (1칸짜리 고립된 도로)
		public void CleanupifIsolated(Vector2Int coord) {
			if(MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if(data.Type.Equals(TileLogicType.Road) && data.ConnectionMask.Equals(RoadDirection.None)) {
					RemoveRoad(coord);
				}
			}
		}

		private void UpdateNeighborsOnRemove(Vector2Int center, RoadDirection mask) {
			//해당 도로가 사라졌으니, 이웃된 도로에게 업데이트를 시킵니다!
			//만약 북쪽에 연결되어있다면, 북쪽 이웃을 찾아가서 남쪽 연결을 끊어야합니다. (반대편이 연결되었다는걸 인식X)
			//노가다가 힘들긴 하지만... 필요한 작업이긴 합니다.
			CheckAndDisconnect(center, mask, RoadDirection.North, new Vector2Int(0, 1));
			CheckAndDisconnect(center, mask, RoadDirection.South, new Vector2Int(0, -1));
			CheckAndDisconnect(center, mask, RoadDirection.East, new Vector2Int(1, 0));
			CheckAndDisconnect(center, mask, RoadDirection.West, new Vector2Int(-1, 0));
			CheckAndDisconnect(center, mask, RoadDirection.NorthEast, new Vector2Int(1, 1));
			CheckAndDisconnect(center, mask, RoadDirection.NorthWest, new Vector2Int(-1, 1));
			CheckAndDisconnect(center, mask, RoadDirection.SouthEast, new Vector2Int(1, -1));
			CheckAndDisconnect(center, mask, RoadDirection.SouthWest, new Vector2Int(-1, -1));
		}

		private void CheckAndDisconnect(Vector2Int center, RoadDirection mymask, RoadDirection dirToCheck, Vector2Int offset) {
			if(mymask.HasFlag(dirToCheck)) {
				Vector2Int neighborPos = center + offset;
				RoadDirection inverseDir = DirUtiles.GetVectorDir(-offset);
				UpdateConnectionMask(neighborPos, inverseDir, add: false);

				CleanupifIsolated(neighborPos);
			}
		}

		private void UpdateConnectionMask(Vector2Int coord, RoadDirection dir, bool add) {
			if(MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if(data.Type.Equals(TileLogicType.Road) || data.Type.Equals(TileLogicType.Entrance)) {
					if (add) data.ConnectionMask |= dir;  //비트마스크로 계산하므로, OR 계산. 각 비트의 위치는 겹치지 않음...
					else data.ConnectionMask &= ~dir;
					MapBootstrapper.Grid[coord] = data;
				}
			}
		}
	}
}
