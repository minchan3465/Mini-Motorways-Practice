using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Utils;

	public class MapManager : MonoBehaviour {
		public static MapManager Instance;

		public Dictionary<Vector2Int, TileData> _grid =  new Dictionary<Vector2Int, TileData>();
		// [추가됨] 코너 데이터를 독립적으로 관리하는 딕셔너리
		public Dictionary<Vector2Int, CornerData> _cornerGrid = new Dictionary<Vector2Int, CornerData>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
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

		//---데이터 생성---
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

		//---도로 연결 데이터 동기화---
		public void ConnectLaneToMap(Lane lane) {
			//시작점 타일을 찾아서, 해당 방향에 도로가 생겼다고 알림.
			if(_grid.TryGetValue(lane.StartNode, out TileData tile)) {
				//방향 계산.
				TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);
				tile.ConnectLane(dir, lane);
			}
		}
		public void DisconnectLaneFromMap(Lane lane) {
			if (_grid.TryGetValue(lane.StartNode, out TileData StartTile)) {
				TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);

				// 타일에게 연결 해제 지시 (TileData 내부에서 null 처리 + RoadState None 처리)
				StartTile.DisconnectLane(dir);
			}
		}

		//맵 확장시 사용할거.
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
