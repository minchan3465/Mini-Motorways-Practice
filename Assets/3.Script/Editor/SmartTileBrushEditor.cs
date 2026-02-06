using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;
using Core.EditorTools;

namespace Core.EditorTools {
	[CustomEditor(typeof(SmartTileBrush))]
	public class SmartTileBrushEditor : GridBrushEditor {
		private new SmartTileBrush brush { get { return target as SmartTileBrush; } }
		private Tilemap currentTilemap;

		public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing) {
			if (brushTarget.TryGetComponent(out Tilemap tilemap)) {
				currentTilemap = tilemap;

				// 잔상 제거
				tilemap.ClearAllEditorPreviewTiles();

				if (executing) {
					base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
				} else if (tool == GridBrushBase.Tool.Paint) {
					DrawAdditivePreview(tilemap, position.position);
					SceneView.RepaintAll();
				}
			}
		}

		private void DrawAdditivePreview(Tilemap tilemap, Vector3Int centerPos) {
			if (brush.densityTiles == null || brush.densityTiles.Length == 0) return;

			for (int x = -brush.radius; x <= brush.radius; x++) {
				for (int y = -brush.radius; y <= brush.radius; y++) {
					Vector3Int offset = new Vector3Int(x, y, 0);
					Vector3Int targetPos = centerPos + offset;

					if (offset.magnitude > brush.radius) continue;

					// 1. 브러시 단계 계산
					int brushLevel = CalculateIndex(offset.magnitude, brush.radius, brush.densityTiles.Length);

					// 2. 현재 타일맵의 상태 확인
					TileBase currentTile = tilemap.GetTile(targetPos);
					int currentLevel = GetTileIndex(currentTile);

					// 3. 미리보기용 합산 로직
					int existingWeight = (currentLevel == -1) ? 0 : (currentLevel + 1);
					int brushWeight = brushLevel + 1;

					int finalWeight = existingWeight + brushWeight;
					int finalIndex = Mathf.Clamp(finalWeight, 1, brush.densityTiles.Length) - 1;

					// 4. 최종 결과물이 될 타일을 미리보기로 표시
					tilemap.SetEditorPreviewTile(targetPos, brush.densityTiles[finalIndex]);
				}
			}
		}

		// 로직 스크립트의 계산식과 동일하게 유지 (복사해서 사용)
		private int CalculateIndex(float distance, float maxRadius, int maxLevels) {
			float ratio = 1.0f - (distance / (maxRadius + 0.5f));
			int index = Mathf.FloorToInt(ratio * maxLevels);
			return Mathf.Clamp(index, 0, maxLevels - 1);
		}

		private int GetTileIndex(TileBase tile) {
			if (tile == null) return -1;
			for (int i = 0; i < brush.densityTiles.Length; i++) {
				if (brush.densityTiles[i] == tile) return i;
			}
			return -1;
		}

		public override void OnMouseLeave() {
			ClearPreview();
			base.OnMouseLeave();
		}

		protected override void OnDisable() {
			ClearPreview();
			base.OnDisable();
		}

		public new void ClearPreview() {
			if (currentTilemap != null) {
				currentTilemap.ClearAllEditorPreviewTiles();
				currentTilemap = null;
			}
		}
	}
}