using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Managers {
	using Views;

	public class TilemapView : MonoBehaviour {
		public static TilemapView Instance;

		[Header("Assets")]
		public RoadTileAtlas roadAtlas;
		public Material roadMaterial;
		public Material outlineMaterial;
		public Material mothballedMaterial;
		public Material bridgeOutlineMaterial; //다리 전용 아웃라인 매테리얼

		private Dictionary<Vector2Int, TileView> _tileViews = new Dictionary<Vector2Int, TileView>();

		private void Awake() {
			if (Instance == null) Instance = this;
			else Destroy(gameObject);
		}

		public void UpdateTiles(HashSet<Vector2Int> changedNodes) {
			foreach (Vector2Int coord in changedNodes) {
				TileView view = GetOrCreateTileView(coord);
				TileData data = MapManager.Instance.GetTileData(coord);
				view.Refresh(data);
			}
		}

		private TileView GetOrCreateTileView(Vector2Int coord) {
			if (_tileViews.TryGetValue(coord, out TileView existingView)) {
				return existingView;
			}

			GameObject tileObj = new GameObject($"Tile_{coord.x}_{coord.y}");
			tileObj.transform.SetParent(this.transform);

			TileView newView = tileObj.AddComponent<TileView>();
			newView.Initialize(coord, roadAtlas, roadMaterial, outlineMaterial, mothballedMaterial, bridgeOutlineMaterial);

			_tileViews.Add(coord, newView);
			return newView;
		}
	}
}
