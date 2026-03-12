using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Models;
	using Utils;

	public class RoadNetworkManager : MonoBehaviour {
		public static RoadNetworkManager Instance;

		public List<Lane> AllLanes { get; private set; } = new List<Lane>();
		private List<Lane> _mothballedLanes = new List<Lane>();
		public List<Lane> MothballedLanes => _mothballedLanes; //[추가] 외부에서 Mothballed 도로에 빠르게 접근하기 위해 노출
		private HashSet<Lane> _systemLanes = new HashSet<Lane>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		private void Update() {
			ProcessMothballedLanes();
			
			//건설 중(드래그)일 때는 완성되지 않은 다리를 붕괴시키지 않습니다.
			bool isBuilding = false;
			if (Actions.InteractionController.Instance != null && Actions.InteractionController.Instance.IsBuildingRoad) {
				isBuilding = true;
			}

			if (!isBuilding) {
				ValidateAllBridges(); //매 프레임 다리 유효성을 검사하여, 한 칸이라도 끊기면 즉시 전체 붕괴 처리
			}
		}

		private void LateUpdate() {
			if (CityModel.ChangedNodes.Count > 0) {
				TilemapView.Instance.UpdateTiles(CityModel.ChangedNodes);
				CityModel.ChangedNodes.Clear();
			}
		}

		public void TryBuildRoad(Vector2Int from, Vector2Int to) {
			if (Vector2Int.Distance(from, to) > 1.5f) return;

			Lane existingLane = GetLane(from, to);
			if (existingLane != null) {
				if (existingLane.State == RoadState.Mothballed) {
					//자원이 충분할 때만 복구
					if (CanRestoreMothballedLane(existingLane)) {
						RestoreMothballedLane(existingLane);
						Lane opposite = GetLane(to, from);
						if (opposite != null) RestoreMothballedLane(opposite);
					}
				}
				return;
			}

			bool isFromWater = false;
			bool isToWater = false;
			bool fromHasRoads = false;
			bool toHasRoads = false;

			if (MapManager.Instance._grid.TryGetValue(from, out TileData fTile)) {
				isFromWater = (fTile.type == TileLogicType.Water);
				fromHasRoads = fTile.HasAnyRoad;
				if (isFromWater && GetActiveConnectionCount(fTile) >= 2) return; 
			}
			if (MapManager.Instance._grid.TryGetValue(to, out TileData tTile)) {
				isToWater = (tTile.type == TileLogicType.Water);
				toHasRoads = tTile.HasAnyRoad;
				if (isToWater && GetActiveConnectionCount(tTile) >= 2) return;
			}

			//다리 규칙 1: 물에서 아무것도 없이 허공 시작 불가
			if (isFromWater && !fromHasRoads) return;

			bool isBridge = isFromWater || isToWater;
			bool isBridgeHead = false;

			//기본적으로 모든 도로는 타일 1칸당 1개의 도로(Road) 자원을 요구합니다.
			if (!ResourceManager.Instance.HasResource(ItemType.Road)) return;

			if (isBridge) {
				//다리 규칙 2: 육지 -> 빈 물 타일로 들어갈 때 다리(Bridge) 자원이 '추가로' 필요합니다.
				if (!isFromWater && isToWater && !toHasRoads) {
					if (!ResourceManager.Instance.HasResource(ItemType.Bridge)) return;
					
					//다리 자원 소모
					ResourceManager.Instance.TryConsumeResource(ItemType.Bridge, 1);
					isBridgeHead = true;
				}
			}

			//도로 자원 무조건 1개 소모 (물 위를 지나더라도 아스팔트는 깔아야 함)
			ResourceManager.Instance.TryConsumeResource(ItemType.Road, 1);

			CreateLane(from, to, isBridge, isBridgeHead);
			CreateLane(to, from, isBridge, isBridgeHead);
		}

		private int GetActiveConnectionCount(TileData tile) {
			int count = 0;
			for (int i = 0; i < 8; i++) {
				if (tile.RoadStates[i] == RoadState.Active || tile.RoadStates[i] == RoadState.Pending) count++;
			}
			return count;
		}

		public void ValidateAllBridges() {
			HashSet<Vector2Int> waterRoadTiles = new HashSet<Vector2Int>();
			foreach (var lane in AllLanes) {
				if (lane.IsBridge && lane.State != RoadState.Mothballed) {
					if (MapManager.Instance._grid.TryGetValue(lane.StartNode, out TileData sTile) && sTile.type == TileLogicType.Water) waterRoadTiles.Add(lane.StartNode);
					if (MapManager.Instance._grid.TryGetValue(lane.EndNode, out TileData eTile) && eTile.type == TileLogicType.Water) waterRoadTiles.Add(lane.EndNode);
				}
			}

			HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

			foreach (var tile in waterRoadTiles) {
				if (visited.Contains(tile)) continue;

				List<Vector2Int> currentPassage = new List<Vector2Int>();
				Queue<Vector2Int> queue = new Queue<Vector2Int>();
				queue.Enqueue(tile);
				visited.Add(tile);

				int landConnections = 0;

				while (queue.Count > 0) {
					Vector2Int current = queue.Dequeue();
					currentPassage.Add(current);

					TileData currentData = MapManager.Instance._grid[current];
					for (int i = 0; i < 8; i++) {
						if (currentData.RoadStates[i] == RoadState.Active || currentData.RoadStates[i] == RoadState.Pending) {
							Vector2Int neighbor = current + TileUtils.GetDirectionVector((TileDirection)(1 << i));
							if (MapManager.Instance._grid.TryGetValue(neighbor, out TileData neighborData)) {
								if (neighborData.type == TileLogicType.Water) {
									if (!visited.Contains(neighbor)) {
										visited.Add(neighbor);
										queue.Enqueue(neighbor);
									}
								} else {
									landConnections++;
								}
							}
						}
					}
				}

				//다리 규칙 3: 육지 연결이 2곳 미만인 불완전한 다리는 즉시 삭제 대기(붕괴) 처리
				if (landConnections < 2) {
					foreach (var p in currentPassage) {
						TryRemoveRoad(p);
					}
				}
			}
		}

		public void BuildSystemRoad(Vector2Int from, Vector2Int to, out Lane outLane, out Lane inLane) {
			//기존에 동일한 구간의 도로가 있는지 먼저 확인합니다. (회전 시 중복 생성 방지)
			outLane = GetLane(from, to);
			inLane = GetLane(to, from);

			if (outLane != null && inLane != null) {
				//이미 있다면 활성화 상태로 돌리고 리턴
				if (outLane.State == RoadState.Mothballed) RestoreMothballedLane(outLane);
				if (inLane.State == RoadState.Mothballed) RestoreMothballedLane(inLane);
				
				_systemLanes.Add(outLane);
				_systemLanes.Add(inLane);
				return;
			}

			Vector2? controlPoint = CalculateControlPoint(from, to);
			outLane = new Lane(from, to, controlPoint, false, false);
			inLane = new Lane(to, from, controlPoint, false, false);

			AllLanes.Add(outLane);
			AllLanes.Add(inLane);
			_systemLanes.Add(outLane);
			_systemLanes.Add(inLane);

			MapManager.Instance.ConnectLaneToMap(outLane);
			MapManager.Instance.ConnectLaneToMap(inLane);

			SyncVisualsBetweenNodes(from, to);
		}

		public void TryRemoveRoad(Vector2Int targetTile) {
			if (MapManager.Instance._grid.TryGetValue(targetTile, out TileData tile)) {
				if (tile.Building != null) return; 
			}

			List<Lane> connectedLanes = AllLanes.FindAll(lane => lane.StartNode == targetTile || lane.EndNode == targetTile);
			if (connectedLanes.Count == 0) return;

			foreach (Lane lane in connectedLanes) {
				if (_systemLanes.Contains(lane)) continue;
				SetLaneToMothballed(lane);
			}
		}

		public void MothballSystemRoad(Lane outLane, Lane inLane) {
			if (outLane != null) {
				SetLaneToMothballed(outLane);
				MapManager.Instance.DisconnectLaneFromMap(outLane);
			}

			if (inLane != null) {
				SetLaneToMothballed(inLane);
				MapManager.Instance.DisconnectLaneFromMap(inLane);
			}
		}

		private void CreateLane(Vector2Int start, Vector2Int end, bool isBridge = false, bool isBridgeHead = false) {
			Vector2? controlPoint = CalculateControlPoint(start, end);
			Lane newLane = new Lane(start, end, controlPoint, isBridge, isBridgeHead);
			AllLanes.Add(newLane);

			MapManager.Instance.ConnectLaneToMap(newLane);
			SyncVisualsBetweenNodes(start, end);

			CityModel.LatestLaneChangeFrame = Time.frameCount;

			SoundManager.Instance.PlaySFX(SoundEffect.RoadBuild);
		}

		private Vector2? CalculateControlPoint(Vector2Int start, Vector2Int end) {
			int dx = end.x - start.x;
			int dy = end.y - start.y;

			if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1) {
				Vector2 pStart = new Vector2(start.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, start.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
				Vector2 pEnd = new Vector2(end.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, end.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
				
				Vector2 cornerPoint;
				if (dx == 1 && dy == 1) cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE);
				else if (dx == -1 && dy == -1) cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE);
				else if (dx == 1 && dy == -1) cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE);
				else cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE);

				return cornerPoint;
			}
			
			return null;
		}

		private void SetLaneToMothballed(Lane lane) {
			if (lane.State == RoadState.Mothballed) return;

			lane.State = RoadState.Mothballed;
			_mothballedLanes.Add(lane);

			SyncVisualsBetweenNodes(lane.StartNode, lane.EndNode);
			CityModel.LatestLaneChangeFrame = Time.frameCount;

			//집 방향 회전은 삭제 소리가 없어야함.
			if(!_systemLanes.Contains(lane)) SoundManager.Instance.PlaySFX(SoundEffect.RoadRemove);
		}

		private bool CanRestoreMothballedLane(Lane lane) {
			//지워지기 전(Mothballed) 상태에서 다시 살리는 것은 자원을 소모하지 않음 (환원된 적이 없으므로)
			return true;
		}

		private void RestoreMothballedLane(Lane lane) {
			if (lane.State == RoadState.Mothballed) {
				lane.State = RoadState.Active;
				_mothballedLanes.Remove(lane);

				//끊겼던 도로 정보를 다시 맵 그리드 데이터에 연결합니다.
				MapManager.Instance.ConnectLaneToMap(lane);

				SyncVisualsBetweenNodes(lane.StartNode, lane.EndNode);
				CityModel.LatestLaneChangeFrame = Time.frameCount;
			}
		}

		private void ProcessMothballedLanes() {
			if (_mothballedLanes.Count == 0) return;

			for (int i = _mothballedLanes.Count - 1; i >= 0; i--) {
				Lane lane = _mothballedLanes[i];

				if (lane.CanRelease()) {
					FinalizeLaneRemoval(lane);
					_mothballedLanes.RemoveAt(i);
				}
			}
		}

		private void FinalizeLaneRemoval(Lane lane) {
			bool wasPlayerBuilt = AllLanes.Remove(lane);
			bool isSystem = _systemLanes.Remove(lane);

			MapManager.Instance.DisconnectLaneFromMap(lane);
			SyncVisualsBetweenNodes(lane.StartNode, lane.EndNode);
			
			//차량이 모두 빠져나가고 도로가 완전히 삭제될 때 자원을 돌려줍니다.
			if (wasPlayerBuilt && !isSystem) {
				bool isCanonical = (lane.StartNode.x < lane.EndNode.x) ||
								   (lane.StartNode.x == lane.EndNode.x && lane.StartNode.y < lane.EndNode.y);

				if (isCanonical) {
					ResourceManager.Instance.AddResource(ItemType.Road, 1);
					if (lane.IsBridgeHead) {
						ResourceManager.Instance.AddResource(ItemType.Bridge, 1);
					}
				}
			}
		}

		public Lane GetLane(Vector2Int start, Vector2Int end) {
			return AllLanes.Find(l => l.StartNode == start && l.EndNode == end);
		}

		private void SyncVisualsBetweenNodes(Vector2Int a, Vector2Int b) {
			Lane ab = GetLane(a, b);
			Lane ba = GetLane(b, a);

			RoadState combined = RoadState.None;

			bool hasActive = (ab != null && ab.State == RoadState.Active) || (ba != null && ba.State == RoadState.Active);
			bool hasMothballed = (ab != null && ab.State == RoadState.Mothballed) || (ba != null && ba.State == RoadState.Mothballed);

			if (hasActive) {
				combined = RoadState.Active;
			} else if (hasMothballed) {
				combined = RoadState.Mothballed;
			}

			TileDirection dirAToB = TileUtils.GetDirection(a, b);
			TileDirection dirBToA = TileUtils.GetOppositeDirection(dirAToB);

			if (MapManager.Instance._grid.TryGetValue(a, out TileData tileA)) {
				tileA.SetRoadState(dirAToB, combined);
			}
			if (MapManager.Instance._grid.TryGetValue(b, out TileData tileB)) {
				tileB.SetRoadState(dirBToA, combined);
			}

			UpdateCornerStateLogic(a, b, combined);

			CityModel.ChangedNodes.Add(a);
			CityModel.ChangedNodes.Add(b);
		}

		private void UpdateCornerStateLogic(Vector2Int start, Vector2Int end, RoadState state) {
			int dx = end.x - start.x;
			int dy = end.y - start.y;
			if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1) {
				Vector2Int cornerCoord;
				CornerDiagonalType diagonalType;
				if (dx == 1 && dy == 1) {
					cornerCoord = start;
					diagonalType = CornerDiagonalType.SW_to_NE;
				} else if (dx == -1 && dy == -1) {
					cornerCoord = end;
					diagonalType = CornerDiagonalType.SW_to_NE;
				} else if (dx == 1 && dy == -1) {
					cornerCoord = new Vector2Int(start.x, end.y);
					diagonalType = CornerDiagonalType.NW_to_SE;
				} else {
					cornerCoord = new Vector2Int(end.x, start.y);
					diagonalType = CornerDiagonalType.NW_to_SE;
				}

				CornerData corner = MapManager.Instance.GetOrCreateCorner(cornerCoord);
				if (corner != null) {
					if (state == RoadState.None) {
						if (GetLane(start, end) == null && GetLane(end, start) == null) {
							corner.RemoveDiagonal(diagonalType);
						}
					} else {
						corner.AddDiagonal(diagonalType, state);
					}
					CityModel.ChangedNodes.Add(cornerCoord);
				}
			}
		}
	}
}
