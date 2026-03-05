using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways.Managers {

	public class MapBootstrapper : MonoBehaviour {
		[Header("Layers (Visual DB)")]
		[Tooltip("기본 지형 (건설 가능/불가 판정)")]
		[SerializeField] private Tilemap _terrainLayer;

		[Tooltip("집(Supply) 생성 가중치 레이어 (알파값 활용)")]
		[SerializeField] private Tilemap _houseWeightLayer;

		[Tooltip("목적지(Demand) 생성 가중치 레이어 (알파값 활용)")]
		[SerializeField] private Tilemap _destWeightLayer;

		//TODO : 건물 데이터를 배치할 레이어도 추가해야 함.

		private void Start() {
			if (_terrainLayer == null) return;
			ExtractDataFromTilemap();
		}

		private void ExtractDataFromTilemap() {
			_terrainLayer.CompressBounds();    // 타일맵 범위 압축 (0,0 기준 정렬)
			BoundsInt bounds = _terrainLayer.cellBounds;   // 실제 타일맵 크기 획득

			// 2. 타일맵 순회 (Extraction Loop)
			foreach (var pos in bounds.allPositionsWithin) {
				Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);
				Vector2Int gridCoord = new Vector2Int(pos.x, pos.y);

				TileData tile = new TileData(gridCoord);

				// 지형 데이터 설정
				if (_terrainLayer.HasTile(localPos)) {
					tile.type = TileLogicType.Empty;
				} else {
					tile.type = TileLogicType.None;
				}

				MapManager.Instance._grid.Add(gridCoord, tile);
			}

			// 가중치 데이터 적용
			ApplyWeightLayer(_houseWeightLayer, isHouse: true);
			ApplyWeightLayer(_destWeightLayer, isHouse: false);

			// 시각용 타일맵 비활성화 (로직 데이터만 추출 후 가림)
			TileMapRendererDisable(_houseWeightLayer);
			TileMapRendererDisable(_destWeightLayer);
		}

		private void ApplyWeightLayer(Tilemap layer, bool isHouse) {
			if (layer == null) return;

			foreach (var pos in layer.cellBounds.allPositionsWithin) {
				Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);
				if (!layer.HasTile(localPos)) continue;
				Vector2Int gridCoord = new Vector2Int(pos.x, pos.y);

				if (MapManager.Instance._grid.TryGetValue(gridCoord, out TileData cell)) {
					float alpha = layer.GetColor(localPos).a;

					if (isHouse) cell.WeightHouseSpawn = alpha;
					else cell.WeightDestinationSpawn = alpha;
				}
			}
		}

		private void TileMapRendererDisable(Tilemap tilemap) {
			if (tilemap.TryGetComponent(out TilemapRenderer tilemapRenderer)) {
				tilemapRenderer.enabled = false;
			}
		}
	}
}
