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
		private RectInt _initialPlayableArea = new RectInt(-9, -5, 18, 10);
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


		//--- ��ȸ ---
		public TileData GetTileData(Vector2Int coord) {
			_grid.TryGetValue(coord, out TileData tile);
			return tile;
		}

		public CornerData GetCornerData(Vector2Int coord) {
			_cornerGrid.TryGetValue(coord, out CornerData corner);
			return corner;
		}

		//---������ ����---
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

		//---���� ���� ������ ����ȭ---
		public void ConnectLaneToMap(Lane lane) {
			//������ Ÿ���� ã�Ƽ�, �ش� ���⿡ ���ΰ� ����ٰ� �˸�.
			if(_grid.TryGetValue(lane.StartNode, out TileData tile)) {
				//���� ���.
				TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);
				tile.ConnectLane(dir, lane);
			}
		}
		public void DisconnectLaneFromMap(Lane lane) {
			if (_grid.TryGetValue(lane.StartNode, out TileData StartTile)) {
				TileDirection dir = TileUtils.GetDirection(lane.StartNode, lane.EndNode);

				//Ÿ�Ͽ��� ���� ���� ���� (TileData ���ο��� null ó�� + RoadState None ó��)
				StartTile.DisconnectLane(dir);
			}
		}

		//�� Ȯ��� ����Ұ�.
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
