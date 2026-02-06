using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;

namespace Core.EditorTools {
	[CreateAssetMenu(fileName = "New SmartTile Brush", menuName = "Brushes/SmartTile Brush")]
	[CustomGridBrush(false, true, false, "SmartTile Brush")]
	public class SmartTileBrush : GridBrush {
		[Header("단계별 타일 (약함 -> 강함 순서)")]
		[Tooltip("인덱스 0: 가장 연한 타일 ~ 인덱스 3: 가장 진한 타일")]
		public TileBase[] densityTiles = new TileBase[4];

		[Header("브러시 크기")]
		[Range(1, 10)] public int radius = 3;

		[Range(1, 3)] public int intensity = 1;

		public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
			if (brushTarget.TryGetComponent(out Tilemap tilemap)) {
				// 타일 배열이 비어있으면 실행 안 함
				if (densityTiles == null || densityTiles.Length == 0) return;

				for (int x = -radius; x <= radius; x++) {
					for (int y = -radius; y <= radius; y++) {
						Vector3Int offset = new Vector3Int(x, y, 0);
						Vector3Int targetPos = position + offset;

						if (offset.magnitude > radius) continue;

						// 1. 브러시가 칠하려는 단계 (거리 비례)
						// (가장 외곽도 최소 1의 가중치를 가짐)
						int brushLevel = CalculateIndex(offset.magnitude, radius, densityTiles.Length);

						// 2. 현재 바닥에 깔린 타일의 단계 가져오기
						TileBase currentTile = tilemap.GetTile(targetPos);
						int currentLevel = GetTileIndex(currentTile); // 타일이 없으면 -1 반환

						// 3. 단계 합치기 (Additive)
						// 기존 타일이 없으면(-1) 0으로 취급하고 계산
						int existingWeight = (currentLevel == -1) ? 0 : (currentLevel + 1);
						int brushWeight = brushLevel + 1; // 인덱스 0을 가중치 1로 변환

						int finalWeight = existingWeight + brushWeight;

						// 최대 단계(4)를 넘지 않도록 제한
						int finalIndex = Mathf.Clamp(finalWeight, 1, densityTiles.Length) - 1;

						// 계산된 최종 타일로 교체
						tilemap.SetTile(targetPos, densityTiles[finalIndex]);
					}
				}
			}
		}

		public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position) {
			if (brushTarget.TryGetComponent(out Tilemap tilemap)) {
				for (int x = -radius; x <= radius; x++) {
					for (int y = -radius; y <= radius; y++) {
						Vector3Int offset = new Vector3Int(x, y, 0);
						if (offset.magnitude > radius) continue;

						tilemap.SetTile(position + offset, null);
					}
				}
			}
		}

		// 거리 비례 인덱스 계산 함수
		private int CalculateIndex(float distance, float maxRadius, int maxLevels) {
			// 거리 비율 (0: 중심, 1: 외곽)
			float ratio = 1.0f - (distance / (maxRadius + 0.5f));

			// 비율을 배열 인덱스로 변환 (0 ~ 3)
			int index = Mathf.FloorToInt(ratio * maxLevels);
			return Mathf.Clamp(index, 0, maxLevels - 1);
		}

		// 현재 타일이 몇 단계인지 확인하는 함수 (덮어쓰기 방지용)
		private int GetTileIndex(TileBase tile) {
			for (int i = 0; i < densityTiles.Length; i++) {
				if (densityTiles[i] == tile) return i;
			}
			return -1; // 내 타일이 아님
		}
	}
}