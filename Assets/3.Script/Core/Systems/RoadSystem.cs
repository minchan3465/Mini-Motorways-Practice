using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	using Core.Data;

	public class RoadSystem : MonoBehaviour {
		public static RoadSystem Instance = null;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		// 두 지점을 도로로 연결하고 비트마스크를 갱신.
		public void ConnectRoads(Vector2Int from, Vector2Int to) {
			//sqrMagnitude가 distance보다 훨~씬 빠름.
			if (Vector2Int.Distance(from, to) > 1.5f) return;
			//if ((from - to).sqrMagnitude > 2.25f) return;
			if (!IsRoadBuildable(to)) return; //건설이 가능한지?
			CreateRoadNode(to); //목적지에 데이터가 없으면 도로 노드 생성
			Vector2Int dirVec = to - from;
			RoadDirection dirToTarget = GetDirectionFromVector(dirVec);
			RoadDirection dirToOrigin = GetDirectionFromVector(-dirVec);

			//위의 마스크 계산간거 적용
			UpdateConnectionMask(from, dirToTarget, add: true);
			UpdateConnectionMask(to, dirToOrigin, add: true);
		}

		public void CreateRoadNode(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData existing)) {
				if (existing.Type.Equals(TileLogicType.Empty)) {
					existing.Type = TileLogicType.Road;
					MapBootstrapper.Grid[coord] = existing;
					RoadVisualizer.Instance.UpdateRoadVisual(coord, MapBootstrapper.Grid[coord]);
				}
			} else {
				CellData newRoad = new CellData {
					Coordinate = coord,
					Type = TileLogicType.Road,
					ConnectionMask = RoadDirection.None
				};
				MapBootstrapper.Grid.Add(coord, newRoad);
				RoadVisualizer.Instance.UpdateRoadVisual(coord, MapBootstrapper.Grid[coord]);
			}
		}

		public void RemoveRoad(Vector2Int coord) {
			if (MapBootstrapper.Grid.TryGetValue(coord, out CellData target)) {
				if (target.Type != TileLogicType.Road) return;
				target.Type = TileLogicType.Empty;
				RoadDirection oldMask = target.ConnectionMask;
				target.ConnectionMask = RoadDirection.None;
				MapBootstrapper.Grid[coord] = target;

				UpdateNeighborsOnRemove(coord, oldMask);
				RoadVisualizer.Instance.RemoveRoadVisual(coord);
			}
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
				RoadDirection inverseDir = GetDirectionFromVector(-offset);
				UpdateConnectionMask(neighborPos, inverseDir, add: false);

				CleanupifIsolated(neighborPos);
			}
		}

		private void UpdateConnectionMask(Vector2Int coord, RoadDirection dir, bool add) {
			if(MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				if(data.Type.Equals(TileLogicType.Road)) {
					if (add) data.ConnectionMask |= dir;  //비트마스크로 계산하므로, OR 계산. 각 비트의 위치는 겹치지 않음...
					else data.ConnectionMask &= ~dir;

					MapBootstrapper.Grid[coord] = data;

					RoadVisualizer.Instance.UpdateRoadVisual(coord, data);
				}
			}
		}


		public bool IsRoadBuildable(Vector2Int coord) {
			if(MapBootstrapper.Grid.TryGetValue(coord, out CellData data)) {
				//도로거나 빈 땅여야 합니다.
				return data.Type.Equals(TileLogicType.Road) || data.Type.Equals(TileLogicType.Empty);
			}
			return true;
		}

		//비트계산 실적용
		private RoadDirection GetDirectionFromVector(Vector2Int dir) {
			if (dir.x == 0 && dir.y == 1) return RoadDirection.North;
			if (dir.x == 0 && dir.y == -1) return RoadDirection.South;
			if (dir.x == 1 && dir.y == 0) return RoadDirection.East;
			if (dir.x == -1 && dir.y == 0) return RoadDirection.West;

			if (dir.x == 1 && dir.y == 1) return RoadDirection.NorthEast;
			if (dir.x == 1 && dir.y == -1) return RoadDirection.SouthEast;
			if (dir.x == -1 && dir.y == -1) return RoadDirection.SouthWest;
			if (dir.x == -1 && dir.y == 1) return RoadDirection.NorthWest;

			return RoadDirection.None;
		}
	}
}
