using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x02000379 RID: 889
	public class CityTilemap : MonoBehaviour, IFeatureSwapObserver
	{
		// Token: 0x06001583 RID: 5507 RVA: 0x00049F24 File Offset: 0x00048124
		private void Awake()
		{
			CityTilemap._roadSortingLayerId = SortingLayer.NameToID("Road");
			CityTilemap._roadOutlineSortingLayerId = SortingLayer.NameToID("RoadOutline");
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x00049F44 File Offset: 0x00048144
		public Color TilemapColorFor(CityTileType type, int groupIndex)
		{
			foreach (CityTileTypeDefinition typeDef in this.cityTileDefinitions)
			{
				if (typeDef.type == type && typeDef.groupIndex == groupIndex)
				{
					return typeDef.tiles.color;
				}
			}
			return Color.black;
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x00049FB8 File Offset: 0x000481B8
		public void Compile(RectInt tilemapDimensions)
		{
			this.CompileTileData(tilemapDimensions);
			this._tileWeightData = new CitySpawningLayerData(this.cityTileDefinitions, this.stationDemandTilemap, this.boatTerminalTilemap);
			this.HideTilemaps(false);
		}

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06001586 RID: 5510 RVA: 0x00049FE5 File Offset: 0x000481E5
		public CitySpawningLayerData TileWeightData
		{
			get
			{
				return this._tileWeightData;
			}
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x00049FF0 File Offset: 0x000481F0
		private void HideTilemaps(bool ignoreEditorPrefs = false)
		{
			foreach (CityTileTypeDefinition cityTileTypeDefinition in this.cityTileDefinitions)
			{
				cityTileTypeDefinition.tiles.GetComponent<Renderer>().enabled = false;
			}
			this.spawnDensityTilemap.GetComponent<Renderer>().enabled = false;
			this.bridgeableTilemap.GetComponent<Renderer>().enabled = false;
			this.mountainTilemap.GetComponent<Renderer>().enabled = false;
			this.unbuildableTilemap.GetComponent<Renderer>().enabled = false;
			this.unzoneableTilemap.GetComponent<Renderer>().enabled = false;
			this.treeTilemap.GetComponent<Renderer>().enabled = false;
			if (this.bonusTreeTilemap != null)
			{
				this.bonusTreeTilemap.GetComponent<Renderer>().enabled = false;
			}
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x0004A0D0 File Offset: 0x000482D0
		public DensityGroup DensityForPosition(Vector3Int position)
		{
			DensityTile tile = this.spawnDensityTilemap.GetTile(position) as DensityTile;
			if (tile != null)
			{
				return tile.group;
			}
			return DensityGroup.High;
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x0004A100 File Offset: 0x00048300
		public bool TileSupportsCircleDestinations(int groupIndex, Vector3Int position)
		{
			foreach (CityTileTypeDefinition cityTilemap in this.cityTileDefinitions)
			{
				if (cityTilemap.type == CityTileType.Demand && (cityTilemap.groupIndex == groupIndex || groupIndex == -1))
				{
					WeightTile tile = cityTilemap.tiles.GetTile(position) as WeightTile;
					if (tile != null)
					{
						return tile.isCircle;
					}
					if (groupIndex != -1)
					{
						return false;
					}
				}
			}
			return false;
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x0004A190 File Offset: 0x00048390
		public static int LayerIdFor(CityTileType type, int groupIndex)
		{
			return (int)(type * (CityTileType)100 + groupIndex + 1);
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x000022F5 File Offset: 0x000004F5
		public void OnChosen()
		{
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x0004A19A File Offset: 0x0004839A
		public void OnNotChosen()
		{
			this.HideTilemaps(true);
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x0004A1A3 File Offset: 0x000483A3
		public bool TileIsBuildable(Vector2Int tileCoordinates)
		{
			return this._buildableTileData[tileCoordinates];
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0004A1B1 File Offset: 0x000483B1
		public bool TileIsZoneable(Vector2Int tileCoordinates)
		{
			return this._zoneableTileData[tileCoordinates];
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x0004A1BF File Offset: 0x000483BF
		public bool TileIsOverWater(Vector2Int tileCoordinates)
		{
			return this._waterTileData[tileCoordinates];
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x0004A1CD File Offset: 0x000483CD
		public bool TileIsUnderAMountain(Vector2Int tileCoordinates)
		{
			return this._mountainTileData[tileCoordinates];
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x0004A1DB File Offset: 0x000483DB
		public bool TileIsOverRail(Vector2Int tileCoordinates)
		{
			return this._railTileData[tileCoordinates];
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x0004A1E9 File Offset: 0x000483E9
		public bool TileIsOverBoatPath(Vector2Int tileCoordinates)
		{
			return this._boatPathTileData[tileCoordinates];
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x0004A1F7 File Offset: 0x000483F7
		public bool TileIsDriveable(Vector2Int tileCoordinates)
		{
			return this._driveableTileData[tileCoordinates];
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0004A208 File Offset: 0x00048408
		private void CompileTileData(RectInt tilemapDimensions)
		{
			this._waterTileData = TileMatrixBool.CreateUnscoped(tilemapDimensions, false);
			this._waterTileData.FillFromTilemap(this.bridgeableTilemap, (TileBase tile) => tile != null);
			this._mountainTileData = TileMatrixBool.CreateUnscoped(tilemapDimensions, false);
			this._mountainTileData.FillFromTilemap(this.mountainTilemap, (TileBase tile) => tile != null);
			this._railTileData = TileMatrixBool.CreateUnscoped(tilemapDimensions, false);
			this._railTileData.FillFromTilemap(this.railTilemap, (TileBase tile) => tile != null);
			this._boatPathTileData = TileMatrixBool.CreateUnscoped(tilemapDimensions, false);
			this._boatPathTileData.FillFromTilemap(this.boatPathTilemap, (TileBase tile) => tile != null);
			this._buildableTileData = TileMatrixBool.CreateUnscoped(tilemapDimensions, true);
			this._buildableTileData.FillFromTilemap(this.unbuildableTilemap, (TileBase tile) => tile == null);
			this._zoneableTileData = TileMatrixBool.CreateUnscoped(tilemapDimensions, true);
			this._zoneableTileData.FillFromTilemap(this.unzoneableTilemap, (TileBase tile) => tile == null);
			this._driveableTileData = TileMatrixBool.CreateUnscoped(tilemapDimensions, true);
			this._driveableTileData.FillFromCoordinates((Vector2Int tileCoordinates) => this._buildableTileData[tileCoordinates] && !this._waterTileData[tileCoordinates] && !this._mountainTileData[tileCoordinates]);
		}

		// Token: 0x04001229 RID: 4649
		public List<CityTileTypeDefinition> cityTileDefinitions;

		// Token: 0x0400122A RID: 4650
		public Tilemap stationDemandTilemap;

		// Token: 0x0400122B RID: 4651
		[FormerlySerializedAs("ferryTerminalTilemap")]
		public Tilemap boatTerminalTilemap;

		// Token: 0x0400122C RID: 4652
		public Tilemap spawnDensityTilemap;

		// Token: 0x0400122D RID: 4653
		public Tilemap bridgeableTilemap;

		// Token: 0x0400122E RID: 4654
		public Tilemap mountainTilemap;

		// Token: 0x0400122F RID: 4655
		public Tilemap railTilemap;

		// Token: 0x04001230 RID: 4656
		public Tilemap boatPathTilemap;

		// Token: 0x04001231 RID: 4657
		public Tilemap unbuildableTilemap;

		// Token: 0x04001232 RID: 4658
		public Tilemap unzoneableTilemap;

		// Token: 0x04001233 RID: 4659
		public Tilemap treeTilemap;

		// Token: 0x04001234 RID: 4660
		public Tilemap bonusTreeTilemap;

		// Token: 0x04001235 RID: 4661
		private TileMatrixBool _waterTileData;

		// Token: 0x04001236 RID: 4662
		private TileMatrixBool _bridgeTileData;

		// Token: 0x04001237 RID: 4663
		private TileMatrixBool _mountainTileData;

		// Token: 0x04001238 RID: 4664
		private TileMatrixBool _railTileData;

		// Token: 0x04001239 RID: 4665
		private TileMatrixBool _boatPathTileData;

		// Token: 0x0400123A RID: 4666
		private TileMatrixBool _buildableTileData;

		// Token: 0x0400123B RID: 4667
		private TileMatrixBool _zoneableTileData;

		// Token: 0x0400123C RID: 4668
		private TileMatrixBool _driveableTileData;

		// Token: 0x0400123D RID: 4669
		private CitySpawningLayerData _tileWeightData;

		// Token: 0x0400123E RID: 4670
		public const string RoadSortingLayerName = "Road";

		// Token: 0x0400123F RID: 4671
		private static int _roadSortingLayerId;

		// Token: 0x04001240 RID: 4672
		public const string RoadOutlineSortingLayerName = "RoadOutline";

		// Token: 0x04001241 RID: 4673
		private static int _roadOutlineSortingLayerId;

		// Token: 0x04001242 RID: 4674
		private const int InvalidLinearCoordinate = -1;
	}
}
