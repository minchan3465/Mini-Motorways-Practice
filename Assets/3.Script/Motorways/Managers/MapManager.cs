using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Utils;

	public class MapManager : MonoBehaviour {
		public static MapManager Instance;

		public Dictionary<Vector2Int, TileData> _grid =  new Dictionary<Vector2Int, TileData>();
		//현재 플레이 가능한 맵 범위
		[SerializeField] 
		private RectInt _initialPlayableArea = new RectInt(-9, -6, 20, 12);
		public RectInt PlayableArea { get; private set; }

		//코너 데이터를 관리하는 딕셔너리
		public Dictionary<Vector2Int, CornerData> _cornerGrid = new Dictionary<Vector2Int, CornerData>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			PlayableArea = _initialPlayableArea;
		}

		//--- 범위 체크 ---
		public bool IsInPlayableArea(Vector2Int coord) {
			return PlayableArea.Contains(coord);
		}

		//--- 범위 확장 ---
		public void ExpandPlayableArea(int amount) {
			PlayableArea = new RectInt(
				PlayableArea.x - amount,
				PlayableArea.y - amount,
				PlayableArea.width + (amount * 2),
				PlayableArea.height + (amount * 2)
			);
			Debug.Log($"Map Expanded: {PlayableArea}");
		}


		//--- 조회 ---
		public TileData GetTileData(Vector2Int coord) {
			_grid.TryGetValue(coord, out TileData tile);
			return tile;
		}

		public CornerData GetCornerData(Vector2Int coord) {
			_cornerGrid.TryGetValue(coord, out CornerData corner);
			return corner;
		}

		//--- 타일 등록 ---
		public void RegisterTile(Vector2Int coord, TileData data) {
			if (!_grid.ContainsKey(coord)) _grid.Add(coord, data);
		}

		public CornerData GetOrCreateCorner(Vector2Int coord) {
			if (_cornerGrid.TryGetValue(coord, out CornerData corner)) {
				return corner;
			}
			CornerData newCorner = new CornerData(coord);
			_cornerGrid.Add(coord, newCorner);
			return newCorner;
		}

		//--- 차선 데이터 연결 ---
		public void ConnectLaneToMap(Lane lane) {
			// 시작 지점 타일을 찾아서, 해당 방향에 차선이 연결되었다고 알림.
			if(_grid.TryGetValue(lane.StartNode, out TileData tile)) {
				// 방향 계산.
				TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);
				tile.ConnectLane(dir, lane);
			}
		}
		public void DisconnectLaneFromMap(Lane lane) {
			if (_grid.TryGetValue(lane.StartNode, out TileData StartTile)) {
				TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);

				// 타일에서 차선 연결 해제 (TileData 내부에서 null 처리 + RoadState None 처리)
				StartTile.DisconnectLane(dir);
			}
		}

		// 맵 확장 시 사용 가능.
		//public TileData GetOrCreateTile(Vector2Int coord) {
		//	if(_grid.TryGetValue(coord, out TileData tile)) {
		//		return tile;
		//	}

		//	TileData newTile = new TileData(coord);
		//	_grid.Add(coord, newTile);
		//	return newTile;
		//}
	}
}
