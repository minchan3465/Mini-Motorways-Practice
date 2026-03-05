using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Models;
	using Utils;

	//도로 네트워크의 논리적 연결(Lane)과 시각적 상태(TileData)를 동기화하고 관리하는 핵심 매니저입니다.
	public class RoadNetworkManager : MonoBehaviour {
		public static RoadNetworkManager Instance;

		//전체 도로 차선 리스트
		public List<Lane> AllLanes { get; private set; } = new List<Lane>();
		//현재 삭제 대기 중(Mothballed)인 차선들
		private List<Lane> _mothballedLanes = new List<Lane>();
		//시스템(건물 등)에 의해 생성된 고정 도로
		private HashSet<Lane> _systemLanes = new HashSet<Lane>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}
		private void Update() {
			//매 프레임 삭제 대기 중인 도로가 비었는지 확인
			ProcessMothballedLanes();

			//변경된 타일이 있다면 시각적 갱신 요청
			if (CityModel.ChangedNodes.Count > 0) {
				TilemapView.Instance.UpdateTiles(CityModel.ChangedNodes);
				CityModel.ChangedNodes.Clear();
			}
		}


		//두 지점 사이에 도로 건설을 시도합니다.
		public void TryBuildRoad(Vector2Int from, Vector2Int to) {
			if (Vector2Int.Distance(from, to) > 1.5f) return;

			Lane existingLane = GetLane(from, to);
			if (existingLane != null) {
				//이미 삭제 대기 중인 도로가 있다면 다시 복구
				if (existingLane.State == RoadState.Mothballed) {
					RestoreMothballedLane(existingLane);
					Lane opposite = GetLane(to, from);
					if (opposite != null) RestoreMothballedLane(opposite);
				}
				return;
			}

			//자원 소비 후 양방향 차선 생성
			if (!ResourceManager.Instance.TryConsumeResource(ItemType.Road, 1)) return;

			CreateLane(from, to);
			CreateLane(to, from);
		}

		//시스템용 도로(삭제 불가)를 건설합니다.
		public void BuildSystemRoad(Vector2Int from, Vector2Int to, out Lane outLane, out Lane inLane) {
			Vector2? controlPoint = CalculateControlPoint(from, to);
			outLane = new Lane(from, to, controlPoint);
			inLane = new Lane(to, from, controlPoint);

			AllLanes.Add(outLane);
			AllLanes.Add(inLane);
			_systemLanes.Add(outLane);
			_systemLanes.Add(inLane);

			MapManager.Instance.ConnectLaneToMap(outLane);
			MapManager.Instance.ConnectLaneToMap(inLane);

			SyncVisualsBetweenNodes(from, to);
		}


		//특정 타일의 모든 도로를 삭제 대기(Mothballed) 상태로 전환합니다.
		public void TryRemoveRoad(Vector2Int targetTile) {
			if (MapManager.Instance._grid.TryGetValue(targetTile, out TileData tile)) {
				if (tile.Building != null) return; //건물이 있는 타일은 삭제 불가
			}

			List<Lane> connectedLanes = AllLanes.FindAll(lane => lane.StartNode == targetTile || lane.EndNode == targetTile);
			if (connectedLanes.Count == 0) return;

			foreach (Lane lane in connectedLanes) {
				if (_systemLanes.Contains(lane)) continue;
				SetLaneToMothballed(lane);
			}
		}

		//시스템용 도로를 삭제 대기 상태로 전환합니다. (주로 건물이 파괴될 때 사용)
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


		//---내부---
		private void CreateLane(Vector2Int start, Vector2Int end) {
			//원작 방식: 모든 차선은 기본적으로 직선이지만, 
			//타일 내에서 곡선이 필요한 경우(예: 주차장 진입로 등)를 위해 제어점을 가질 수 있는 구조입니다.
			Vector2? controlPoint = CalculateControlPoint(start, end);
			Lane newLane = new Lane(start, end, controlPoint);
			AllLanes.Add(newLane);

			MapManager.Instance.ConnectLaneToMap(newLane);
			SyncVisualsBetweenNodes(start, end);

			CityModel.LatestLaneChangeFrame = Time.frameCount;
		}

		//코너링을 위한 제어점 계산 헬퍼 함수
		private Vector2? CalculateControlPoint(Vector2Int start, Vector2Int end) {
			int dx = end.x - start.x;
			int dy = end.y - start.y;

			//대각선 이동일 경우 곡선이 필요함
			if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1) {
				//두 가지 가능한 모서리 중 하나를 제어점으로 선택해야 합니다.
				//미니 모터웨이의 타일 연결 방식(CornerData)을 고려할 때,
				//타일 경계의 교차점을 제어점으로 삼아 부드러운 곡선을 만듭니다.
				//(start.x, end.y) 또는 (end.x, start.y) 중 하나가 제어점이 됩니다.
				//간단하게 한쪽 직각 모서리를 제어점으로 반환합니다.
				
				//시각적으로 도로가 어떤 모서리를 도는지 확인하기 위해 
				//임의로 한 쪽을 제어점으로 삼습니다. (필요 시 기존 맵 데이터 참조)
				//좀 더 완벽한 둥근 코너를 위해 타일 중앙 + 모서리 오프셋을 사용.
				Vector2 pStart = new Vector2(start.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, start.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
				Vector2 pEnd = new Vector2(end.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, end.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
				
				//교차점(모서리)을 제어점으로 설정
				Vector2 cornerPoint;
				if (dx == 1 && dy == 1) cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE);
				else if (dx == -1 && dy == -1) cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE);
				else if (dx == 1 && dy == -1) cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE);
				else cornerPoint = new Vector2(start.x * MapSettings.TILE_SIZE, start.y * MapSettings.TILE_SIZE + MapSettings.TILE_SIZE);

				return cornerPoint;
			}
			
			//직선일 경우 제어점 없음
			return null;
		}

		//도로를 삭제 대기 상태로 변경하고 리스트에 등록합니다.
		private void SetLaneToMothballed(Lane lane) {
			if (lane.State == RoadState.Mothballed) return;

			lane.State = RoadState.Mothballed;
			_mothballedLanes.Add(lane);

			SyncVisualsBetweenNodes(lane.StartNode, lane.EndNode);
			CityModel.LatestLaneChangeFrame = Time.frameCount;
		}

		//삭제 대기 중인 도로를 다시 활성화합니다.
		private void RestoreMothballedLane(Lane lane) {
			if (lane.State == RoadState.Mothballed) {
				lane.State = RoadState.Active;
				_mothballedLanes.Remove(lane);

				SyncVisualsBetweenNodes(lane.StartNode, lane.EndNode);
				CityModel.LatestLaneChangeFrame = Time.frameCount;
			}
		}

		//차량이 모두 빠져나간 Mothballed 도로를 실제로 메모리에서 제거합니다.
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

		//---심층---
		//도로의 모든 물리적 데이터를 삭제하고 자원을 환원합니다.
		private void FinalizeLaneRemoval(Lane lane) {
			bool wasPlayerBuilt = AllLanes.Remove(lane);
			bool isSystem = _systemLanes.Remove(lane);

			MapManager.Instance.DisconnectLaneFromMap(lane);
			SyncVisualsBetweenNodes(lane.StartNode, lane.EndNode);

			if (wasPlayerBuilt && !isSystem) {
				//중복 환원을 방지하기 위해 한쪽 방향 기준으로만 자원 환원
				bool isCanonical = (lane.StartNode.x < lane.EndNode.x) ||
								   (lane.StartNode.x == lane.EndNode.x && lane.StartNode.y < lane.EndNode.y);

				if (isCanonical) ResourceManager.Instance.AddResource(ItemType.Road, 1);
			}
		}

		//---헬퍼---
		public Lane GetLane(Vector2Int start, Vector2Int end) {
			return AllLanes.Find(l => l.StartNode == start && l.EndNode == end);
		}

		//---시각화---
		//두 노드 사이의 차선 상태를 종합하여 타일의 시각적 상태(RoadState)를 결정합니다.
		private void SyncVisualsBetweenNodes(Vector2Int a, Vector2Int b) {
			Lane ab = GetLane(a, b);
			Lane ba = GetLane(b, a);

			RoadState combined = RoadState.None;

			//[수정된 로직]
			//한쪽 방향이라도 Active 상태라면 시각적으로는 'Active' 도로로 표시합니다.
			//이는 한쪽 차선이 먼저 삭제되거나(Finalized), 복구(Restore) 중인 상황에서도 도로가 끊겨 보이지 않게 합니다.
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

		//대각선 연결(Corner)의 상태를 관리하고 시각적 동기화를 수행합니다.
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
						//양쪽 방향 차선이 모두 없을 때만 데이터 삭제
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
