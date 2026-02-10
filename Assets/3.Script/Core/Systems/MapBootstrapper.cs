using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Systems {
	using Core.Data;

	public class MapBootstrapper : MonoBehaviour {
		[Header("Layers (Visual DB)")]
		[Tooltip("기본 지형 (건설 가능/불가 판단)")]
		[SerializeField] private Tilemap _terrainLayer;

		[Tooltip("집(Supply) 스폰 가중치 레이어 (투명도 활용)")]
		[SerializeField] private Tilemap _houseWeightLayer;

		[Tooltip("목적지(Demand) 스폰 가중치 레이어 (투명도 활용)")]
		[SerializeField] private Tilemap _destWeightLayer;


		//한가지 TileMap이 아닌, 이제 여러개의 레이어로 나눈 TileMap을 활용할 예정.
		//[Header("Source (Visual DB)")]
		//[SerializeField] private Tilemap _sourceTilemap;

		public static Dictionary<Vector2Int, CellData> Grid { get; private set; }
		public static BoundsInt MapBounds { get; private set; }

		private void Awake() {
			if (_terrainLayer == null) return;
			ExtractDataFromTilemap();
		}

		private void ExtractDataFromTilemap() {
			Grid = new Dictionary<Vector2Int, CellData>();  //Initialize

			_terrainLayer.CompressBounds();    //타일맵 원점 보정. (Tilemap은 0,0 좌표가 중앙일 수 있음)
			BoundsInt bounds = _terrainLayer.cellBounds;   //현재 맵의 크기 저장.

			//Debug.Log($"<color=cyan>[MapBootstrapper]</color> Starting Extraction... Bounds: {bounds}");

			//int processedCount = 0;

			// 2. 타일맵 순회 (Extraction Loop)
			//전에 테스트하면서 적었던건데 왜 날렸을까...
			//cellBounds = (타일이 있을 경우) 설치된 타일을 모두 감싸는 직사각형 크기
			//allPositionWithin = 그 직사각형의 크기 내부에 있는 모든 타일의 위치
			#region var 자료형에 관한 고찰
			/*
			var 자료형은 굉장한 많은 이야기가 있다.
			  장점
				1. 가독성이 올라감
				2. 남의 API 사용때 어떻게 받아올지 모를 때 사용함.
				3. 편함
				4. 값이 달라질 경우, 유용하게 받아올 수 있음.
			  단점
				1. 가독성이 나쁨 (타인이 볼때)
				2. 오류 도출가능성 있음

			어쨌든 장단점이 되게 명확한데, 적절히 쓰면 좋다로 생각.
			보통 foreach문에서 많이 쓴다고 말을 한다.
			개인적으로는 자료형을 바로 알아보고 싶어서 제대로된 변수명을 하는걸 좋아하지만.
			반대로 개인적으로 하니까 오히려 var로 짜도 괜찮지 않나 생각중이다.

			여러 의견들 중...
			+ 어짜피 마우스 올려보면 변수명 다 보이는데 상관없다.
			개인적인 의견
			+ 인정한다. 어짜피 코드 짜다보면 하단에 변수명을 적을때가 많다...
			+ 그리고 var이 안좋았다면 언젠가 사라졌겠지만, 계속 남아있는데는 이유가 다 있다고 생각.
			 */
			#endregion
			foreach (var pos in bounds.allPositionsWithin) {
				Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);
				Vector2Int gridCoord = new Vector2Int(pos.x, pos.y);

				CellData cell = new CellData(gridCoord);

				//지형 타일이 없다 = 맵 밖이거나 타일을 안설치함. (Obstacle로 처리할지 고민중)
				//근데 보통 타일맵에서 bounds 내부에 있는 타일이 안 칠해져있으면 null이라고 함.
				if(!_terrainLayer.HasTile(localPos)) {
					cell.Type = TileLogicType.Obstacle;	//또는 Restricted
				} else {
					//만약 지형 타일에 별도 로직(강, 산)이 있다면 여기서 TileBase 체크
					//예: TileBase tb = _terrainLayer.GetTile(localPos);
					//if (tb.name.Contains("Water")) cell.Type = TileLogicType.Obstacle;
				}

				Grid.Add(gridCoord, cell);
				////근데 타일 베이스에 있는 타일이 SmartTile, 즉 이미 타일 데이터가 있다면.
				//if (tileBase is SmartTile smartTile) {
				//	//데이터 파싱
				//	cell.Type = smartTile.logicType;
				//	cell.Weight = smartTile.spawnWeight;
				//}

				////Dictionary에 저장
				//if (!Grid.ContainsKey(cell.Coordinate)) {
				//	Grid.Add(gridCoord, cell);
				//	processedCount += 1;
				//}
			}

			//위에서 지형에 대한 정보를 다 해줬으니, 이제 스폰 위치에 대한 가중치를 계산합시다.
			ApplyWeightLayer(_houseWeightLayer, isHouse: true);
			ApplyWeightLayer(_destWeightLayer, isHouse: false);

			// 시각적 타일맵은 이제 필요 없으므로 렌더러를 끄거나, 배경으로만 사용
			//_sourceTilemap.GetComponent<TilemapRenderer>().enabled = false; 
			//TileMapRendererDisable(_terrainLayer);
			TileMapRendererDisable(_houseWeightLayer);
			TileMapRendererDisable(_destWeightLayer);
			//Debug.Log($"<color=green>[MapBootstrapper]</color> Extraction Complete! Processed {processedCount} SmartTiles.");
		}

		private void ApplyWeightLayer(Tilemap layer, bool isHouse) {
			if (layer == null) return;

			foreach(var pos in layer.cellBounds.allPositionsWithin) {
				//위의 지형과 동일하게, 설치된 타일들을 통하여 로직을 구현.
				Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);
				if (!layer.HasTile(localPos)) continue;
				Vector2Int gridCoord = new Vector2Int(pos.x, pos.y);

				if(Grid.TryGetValue(gridCoord, out CellData cell)) {
					float alpha = layer.GetColor(localPos).a;

					if (isHouse) cell.HouseWeight = alpha;
					else cell.DestinationWeight = alpha;
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

