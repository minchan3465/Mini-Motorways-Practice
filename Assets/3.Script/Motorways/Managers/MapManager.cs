using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Utils;

	public class MapManager : MonoBehaviour {
		public static MapManager Instance;

		public static Dictionary<Vector2Int, TileData> _grid { get; set; }
		private Dictionary<Vector2Int, RoadTile> _roadGraph = new Dictionary<Vector2Int, RoadTile>();
		public Dictionary<Vector2Int, RoadTile> RoadGraph => _roadGraph;

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);

			_grid = new Dictionary<Vector2Int, TileData>();
		}

		//--- 조회 ---
		public TileData GetTileData(Vector2Int coord) {
			_grid.TryGetValue(coord, out TileData tile);
			return tile;
		}

		public RoadTile GetRoadTile(Vector2Int coord) {
			_roadGraph.TryGetValue(coord, out RoadTile tile);
			return tile;
		}

		//---데이터 생성---
		public void RegisterTile(Vector2Int coord, TileData data) {
			if (!_grid.ContainsKey(coord)) _grid.Add(coord, data);
		}

		//---도로 연결 데이터 동기화---
		public void ConnectLanesOnMap(Lane lane) {
			if(!_roadGraph.TryGetValue(lane.StartNode, out RoadTile startTile)) {
				startTile = new RoadTile(lane.StartNode);
				UpdateTileVisualData(lane.StartNode, lane.EndNode, RoadState.Active);
			}
			startTile.AddLane(lane);
			UpdateTileVisualData(lane.StartNode, lane.EndNode, RoadState.Active);
		}
		public void DisconnectLanesOnMap(Lane lane) {
			if (_roadGraph.TryGetValue(lane.StartNode, out RoadTile startTile)) {
				startTile.RemoveLane(lane);
			}
			UpdateTileVisualData(lane.StartNode, lane.EndNode, RoadState.None);
		}

		private void UpdateTileVisualData(Vector2Int center, Vector2Int target, RoadState state) {
			if (_grid.TryGetValue(center, out TileData tile)) {
				TileDirection dir = TileUtils.GetDirection(center, target);
				tile.SetRoadState(dir, state);

				// TODO: 여기서 나중에 RoadVisualizer.UpdateMesh(coord) 등을 호출하면 됨
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
