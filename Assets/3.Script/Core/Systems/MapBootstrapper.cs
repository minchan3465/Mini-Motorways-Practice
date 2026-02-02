using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core.Systems {
	using Core.Data;

	public class MapBootstrapper : MonoBehaviour {
		[Header("Source (Visual DB)")]
		[SerializeField] private Tilemap _sourceTilemap;

		public static Dictionary<Vector2Int, CellData> Grid { get; private set; }
		public static BoundsInt MapBounds { get; private set; }

		private void Awake() {
			if (_sourceTilemap == null) return;
			ExtractDataFromTilemap();
		}

		private void ExtractDataFromTilemap() {
			//데이터 배열 초기화
			Grid = new Dictionary<Vector2Int, CellData>();

			_sourceTilemap.CompressBounds();    //타일맵 원점 보정. (Tilemap은 0,0 좌표가 중앙일 수 있음)
			BoundsInt bounds = _sourceTilemap.cellBounds;   //현재 맵의 크기 저장.

			//Debug.Log($"<color=cyan>[MapBootstrapper]</color> Starting Extraction... Bounds: {bounds}");

			int processedCount = 0;

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
			foreach (var pos in _sourceTilemap.cellBounds.allPositionsWithin) {
				Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);

				if (!_sourceTilemap.HasTile(localPos)) continue;

				//해당 위치 타일 가져오기.
				//+ 좌표 2차원화
				TileBase tileBase = _sourceTilemap.GetTile(localPos);

				Vector2Int gridCoord = new Vector2Int(pos.x, pos.y);

				//일단 타일 데이터 생성
				CellData cell = new CellData {
					Coordinate = gridCoord,
					Type = TileLogicType.Empty,
					Weight = 0f,
					ConnectionMask = 0
				};

				//근데 타일 베이스에 있는 타일이 SmartTile, 즉 이미 타일 데이터가 있다면.
				if (tileBase is SmartTile smartTile) {
					//데이터 파싱
					cell.Type = smartTile.logicType;
					cell.Weight = smartTile.spawnWeight;
				}

				//Dictionary에 저장
				if (!Grid.ContainsKey(cell.Coordinate)) {
					Grid.Add(gridCoord, cell);
					processedCount++;
				}

			}

			// 시각적 타일맵은 이제 필요 없으므로 렌더러를 끄거나, 배경으로만 사용
			// _sourceTilemap.GetComponent<TilemapRenderer>().enabled = false; 

			//Debug.Log($"<color=green>[MapBootstrapper]</color> Extraction Complete! Processed {processedCount} SmartTiles.");
		}

		private void OnDrawGizmos() {
			if (Grid == null) return;

			foreach(var kvp in Grid) {
				CellData cell = kvp.Value;
				if (cell.Type != TileLogicType.Empty) {
					switch (cell.Type) {
						case TileLogicType.Obstacle:
							Gizmos.color = Color.red;
							break;
						case TileLogicType.Road:
							Gizmos.color = Color.gray;
							break;
						case TileLogicType.Supply:
							Gizmos.color = Color.blue;
							break;
						case TileLogicType.Demand:
							Gizmos.color = Color.yellow;
							break;
						case TileLogicType.Restricted:
							break;
					}
					
					// 시각적 확인을 위해 위치를 다시 월드로 변환해서 그리기
					Vector3 center = new Vector3(cell.Coordinate.x + 0.5f, 0, cell.Coordinate.y + 0.5f);
					Gizmos.DrawWireCube(center, Vector3.one * 0.9f);

					if(cell.Type.Equals(TileLogicType.Road) && cell.ConnectionMask != RoadDirection.None) {
						Gizmos.color = Color.magenta;
						DrawConnections(center, cell.ConnectionMask);
					}

				}
			}
		}

		// 비트마스크를 풀어서 선으로 그리는 헬퍼 함수
		private void DrawConnections(Vector3 center, RoadDirection mask) {
			// 각 비트가 켜져 있는지 확인하고 선 긋기
			if (mask.HasFlag(RoadDirection.North)) Gizmos.DrawLine(center, center + new Vector3(0, 0, 0.5f));
			if (mask.HasFlag(RoadDirection.South)) Gizmos.DrawLine(center, center + new Vector3(0, 0, -0.5f));
			if (mask.HasFlag(RoadDirection.East)) Gizmos.DrawLine(center, center + new Vector3(0.5f, 0, 0));
			if (mask.HasFlag(RoadDirection.West)) Gizmos.DrawLine(center, center + new Vector3(-0.5f, 0, 0));

			// 대각선
			if (mask.HasFlag(RoadDirection.NorthEast)) Gizmos.DrawLine(center, center + new Vector3(0.5f, 0, 0.5f));
			if (mask.HasFlag(RoadDirection.SouthEast)) Gizmos.DrawLine(center, center + new Vector3(0.5f, 0, -0.5f));
			if (mask.HasFlag(RoadDirection.SouthWest)) Gizmos.DrawLine(center, center + new Vector3(-0.5f, 0, -0.5f));
			if (mask.HasFlag(RoadDirection.NorthWest)) Gizmos.DrawLine(center, center + new Vector3(-0.5f, 0, 0.5f));
		}
	}
}

