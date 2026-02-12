using System;
using System.Collections.Generic;
using FixMath;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x02000366 RID: 870
	[RequireComponent(typeof(CitySchedulePlanner))]
	public class CityDefinition : MonoBehaviour
	{
		// Token: 0x0600154B RID: 5451 RVA: 0x000490EB File Offset: 0x000472EB
		public int GetHousesAtDay(int day)
		{
			return (int)this._housesAtDay.Evaluate((float)day);
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x0600154C RID: 5452 RVA: 0x000490FB File Offset: 0x000472FB
		// (set) Token: 0x0600154D RID: 5453 RVA: 0x00049103 File Offset: 0x00047303
		public CityTilemapMeshGenerator CityTilemapMeshGenerator { get; private set; }

		// Token: 0x0600154E RID: 5454 RVA: 0x0004910C File Offset: 0x0004730C
		protected virtual void Awake()
		{
			if (Application.isPlaying)
			{
				this.CityTilemapMeshGenerator = base.GetComponent<CityTilemapMeshGenerator>();
			}
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x00049124 File Offset: 0x00047324
		public bool UsesUpgradeType(UpgradeType type)
		{
			if (this._availableUpgrades.Count == 0)
			{
				foreach (UpgradePackageDefinition package in this.upgradeDefinitions.startingPackages)
				{
					if (!this._availableUpgrades.Contains(package.type))
					{
						this._availableUpgrades.Add(package.type);
					}
				}
				foreach (WeeklyUpgradeDefinition upgradeDefinition in this.upgradeDefinitions.weeklyChoicePackages)
				{
					if (!this._availableUpgrades.Contains(upgradeDefinition.package.type))
					{
						this._availableUpgrades.Add(upgradeDefinition.package.type);
					}
				}
			}
			return this._availableUpgrades.Contains(type);
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x000491E4 File Offset: 0x000473E4
		public Fix64 GetEfficiencyMilestone(int index, int increaseAfterPrecalculatedIntervals)
		{
			int length = this._efficiencyMilestoneIntervals.Length;
			if (index < length)
			{
				return (Fix64)((long)this._efficiencyMilestoneIntervals[index]);
			}
			return (Fix64)((long)(this._efficiencyMilestoneIntervals[length - 1] + (index - length + 1) * increaseAfterPrecalculatedIntervals));
		}

		// Token: 0x06001551 RID: 5457 RVA: 0x00049228 File Offset: 0x00047428
		public void CompileTilemap()
		{
			if (this._isCompiled)
			{
				return;
			}
			this._isCompiled = true;
			Fix64 x = City.PlayableRatio * this.cameraZoom.endSize;
			Fix64 rawHeight = this.cameraZoom.endSize;
			int minX = Fix64.CeilToInt(-x * Fix64Consts.OneHalf);
			int minY = Fix64.CeilToInt(-rawHeight * Fix64Consts.OneHalf);
			int maxX = Fix64.FloorToInt(x * Fix64Consts.OneHalf) + 1;
			int maxY = Fix64.FloorToInt(rawHeight * Fix64Consts.OneHalf) + 1;
			this._playableArea = new RectInt
			{
				x = minX,
				y = minY,
				width = maxX - minX,
				height = maxY - minY
			};
			CityTilemap tilemap = (CityTilemap)this._tilemap;
			if (tilemap != null)
			{
				tilemap.Compile(this._playableArea);
			}
		}

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06001552 RID: 5458 RVA: 0x00049311 File Offset: 0x00047511
		public virtual CitySpawningLayerData TileWeightData
		{
			get
			{
				return ((CityTilemap)this._tilemap).TileWeightData;
			}
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x00049323 File Offset: 0x00047523
		public virtual DensityGroup DensityForPosition(Vector2Int position)
		{
			return ((CityTilemap)this._tilemap).DensityForPosition((Vector3Int)position);
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x0004933B File Offset: 0x0004753B
		public virtual bool TileSupportsCircleDestinations(int groupIndex, Vector2Int position)
		{
			return ((CityTilemap)this._tilemap).TileSupportsCircleDestinations(groupIndex, (Vector3Int)position);
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06001555 RID: 5461 RVA: 0x00049354 File Offset: 0x00047554
		public RectInt PlayableArea
		{
			get
			{
				return this._playableArea;
			}
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x0004935C File Offset: 0x0004755C
		public virtual IEnumerable<Tuple<Vector2Int, int>> GetTreeData(bool includeBonusTrees)
		{
			CityTilemap tilemap = (CityTilemap)this._tilemap;
			if (tilemap != null)
			{
				if (tilemap.treeTilemap != null)
				{
					foreach (Vector3Int position in tilemap.treeTilemap.cellBounds.allPositionsWithin)
					{
						TileBase tile = tilemap.treeTilemap.GetTile(position);
						if (tile is TreeTile)
						{
							TreeTile treeTile = tile as TreeTile;
							yield return new Tuple<Vector2Int, int>((Vector2Int)position, treeTile.prefabIndex);
						}
					}
					BoundsInt.PositionEnumerator positionEnumerator = default(BoundsInt.PositionEnumerator);
				}
				if (tilemap.bonusTreeTilemap != null && includeBonusTrees)
				{
					foreach (Vector3Int position2 in tilemap.bonusTreeTilemap.cellBounds.allPositionsWithin)
					{
						TileBase tile2 = tilemap.bonusTreeTilemap.GetTile(position2);
						if (tile2 is TreeTile)
						{
							TreeTile treeTile2 = tile2 as TreeTile;
							yield return new Tuple<Vector2Int, int>((Vector2Int)position2, treeTile2.prefabIndex);
						}
					}
					BoundsInt.PositionEnumerator positionEnumerator = default(BoundsInt.PositionEnumerator);
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x06001557 RID: 5463 RVA: 0x00049373 File Offset: 0x00047573
		public virtual bool TileIsBuildable(Vector2Int tileCoordinates)
		{
			return ((CityTilemap)this._tilemap).TileIsBuildable(tileCoordinates);
		}

		// Token: 0x06001558 RID: 5464 RVA: 0x00049386 File Offset: 0x00047586
		public virtual bool TileIsZoneable(Vector2Int tileCoordinates)
		{
			return ((CityTilemap)this._tilemap).TileIsZoneable(tileCoordinates);
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x00049399 File Offset: 0x00047599
		public virtual bool TileIsOverWater(Vector2Int tileCoordinates)
		{
			return ((CityTilemap)this._tilemap).TileIsOverWater(tileCoordinates);
		}

		// Token: 0x0600155A RID: 5466 RVA: 0x000493AC File Offset: 0x000475AC
		public virtual bool TileIsUnderAMountain(Vector2Int tileCoordinates)
		{
			return ((CityTilemap)this._tilemap).TileIsUnderAMountain(tileCoordinates);
		}

		// Token: 0x0600155B RID: 5467 RVA: 0x000493BF File Offset: 0x000475BF
		public virtual bool TileIsOverRail(Vector2Int tileCoordinates)
		{
			return ((CityTilemap)this._tilemap).TileIsOverRail(tileCoordinates);
		}

		// Token: 0x0600155C RID: 5468 RVA: 0x000493D2 File Offset: 0x000475D2
		public virtual bool TileIsDriveable(Vector2Int tileCoordinates)
		{
			return ((CityTilemap)this._tilemap).TileIsDriveable(tileCoordinates);
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x000493E8 File Offset: 0x000475E8
		public Vector3Fixed GenerateCityStartOffset(PseudorandomGenerator pseudorandomGenerator)
		{
			if (this.startingOffsets.Count == 0)
			{
				return Vector3Fixed.zero;
			}
			int startingOffsetIndex = pseudorandomGenerator.Int(this.startingOffsets.Count);
			CityStartOffsetDefinition startingOffsetDefinition = this.startingOffsets.offsets[startingOffsetIndex];
			Fix64 variance = startingOffsetDefinition.variance;
			Fix64 xVariance = variance * (pseudorandomGenerator.Fix64(Fix64Consts.Two) - Fix64Consts.One);
			Fix64 yVariance = variance * (pseudorandomGenerator.Fix64(Fix64Consts.Two) - Fix64Consts.One);
			return new Vector3Fixed(startingOffsetDefinition.fixedPosition.x + xVariance, startingOffsetDefinition.fixedPosition.y + yVariance, startingOffsetDefinition.fixedPosition.z);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x0004949B File Offset: 0x0004769B
		public TrainNetworkDefinition GetTrainNetworkDefinition()
		{
			return TrainNetworkDefinition.CreateFromRailTileCoordinates(this.CompileRailTileCoordinates());
		}

		// Token: 0x0600155F RID: 5471 RVA: 0x000494A8 File Offset: 0x000476A8
		protected virtual Dictionary<Vector2Int, RailType> CompileRailTileCoordinates()
		{
			Dictionary<Vector2Int, RailType> railTileCoordinates = new Dictionary<Vector2Int, RailType>();
			CityTilemap cityTilemap = (CityTilemap)this._tilemap;
			Tilemap tilemap = (cityTilemap != null) ? cityTilemap.railTilemap : null;
			if (tilemap != null)
			{
				foreach (Vector3Int tilePosition in tilemap.cellBounds.allPositionsWithin)
				{
					TileBase tileBase = tilemap.GetTile(tilePosition);
					if (tileBase != null)
					{
						RailType railType = RailType.Normal;
						WeightTile weightTile = tileBase as WeightTile;
						if (weightTile != null && weightTile.sprite.name == "blank_s")
						{
							railType = RailType.TrainOrigin;
						}
						railTileCoordinates.Add((Vector2Int)tilePosition, railType);
					}
				}
			}
			return railTileCoordinates;
		}

		// Token: 0x06001560 RID: 5472 RVA: 0x0004957C File Offset: 0x0004777C
		public BoatNetworkDefinition GetBoatPathNetworkDefinition()
		{
			return BoatNetworkDefinition.CreateFromBoatPathTileCoordinates(this.CompileBoatPathTileCoordinates());
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x0004958C File Offset: 0x0004778C
		protected virtual Dictionary<Vector2Int, BoatPathType> CompileBoatPathTileCoordinates()
		{
			Dictionary<Vector2Int, BoatPathType> boatPathTileCoordinates = new Dictionary<Vector2Int, BoatPathType>();
			CityTilemap cityTilemap = (CityTilemap)this._tilemap;
			Tilemap tilemap = (cityTilemap != null) ? cityTilemap.boatPathTilemap : null;
			if (tilemap != null)
			{
				foreach (Vector3Int tilePosition in tilemap.cellBounds.allPositionsWithin)
				{
					TileBase tileBase = tilemap.GetTile(tilePosition);
					if (tileBase != null)
					{
						BoatPathType boatPathType = BoatPathType.Normal;
						WeightTile weightTile = tileBase as WeightTile;
						if (weightTile != null && weightTile.sprite.name == "blank_s")
						{
							boatPathType = BoatPathType.BoatOrigin;
						}
						boatPathTileCoordinates.Add((Vector2Int)tilePosition, boatPathType);
					}
				}
			}
			return boatPathTileCoordinates;
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x00049660 File Offset: 0x00047860
		private void OnValidate()
		{
			if (this.upgradeDefinitions.weeklyChoicePackages != null)
			{
				foreach (WeeklyUpgradeDefinition weeklyUpgrade in this.upgradeDefinitions.weeklyChoicePackages)
				{
					if (weeklyUpgrade.expectedUpgradeTimeline != null && weeklyUpgrade.expectedUpgradeTimeline.Count > 1)
					{
						weeklyUpgrade.expectedUpgradeTimeline.Sort((ExpectedUpgradeTimeline x, ExpectedUpgradeTimeline y) => x.week.CompareTo(y.week));
					}
				}
			}
		}

		// Token: 0x040011CD RID: 4557
		public TrafficSide trafficSide;

		// Token: 0x040011CE RID: 4558
		[HideInInspector]
		public ZoomParameters cameraZoom = new ZoomParameters();

		// Token: 0x040011CF RID: 4559
		[SerializeField]
		private SwapByFeature_CityTilemap _tilemap = new SwapByFeature_CityTilemap();

		// Token: 0x040011D0 RID: 4560
		public CitySchedulePlanner schedulePlanner;

		// Token: 0x040011D1 RID: 4561
		public UpgradeCycleDefinition upgradeDefinitions;

		// Token: 0x040011D2 RID: 4562
		public CityStartOffsets startingOffsets;

		// Token: 0x040011D3 RID: 4563
		public SpawnRampParameters spawnRamp;

		// Token: 0x040011D4 RID: 4564
		[SerializeField]
		[HideInInspector]
		[Tooltip("If the game mode (endless) uses this curve, it has a minimum house count at a particular day.")]
		private AnimationCurve _housesAtDay = AnimationCurve.Linear(0f, 0f, 400f, 100f);

		// Token: 0x040011D5 RID: 4565
		[SerializeField]
		[HideInInspector]
		private int[] _efficiencyMilestoneIntervals = new int[]
		{
			10,
			60,
			100,
			140,
			180,
			220,
			260,
			300,
			340,
			380,
			420
		};

		// Token: 0x040011D6 RID: 4566
		[SerializeField]
		private float _expectedPointAttrition = 0.65f;

		// Token: 0x040011D7 RID: 4567
		[SerializeField]
		private float _expectedTrafficPerHouse = 0.2f;

		// Token: 0x040011D8 RID: 4568
		public TextAsset audioLoadout;

		// Token: 0x040011D9 RID: 4569
		public GameObject[] bonusTreeGrassObjects;

		// Token: 0x040011DA RID: 4570
		private bool _isCompiled;

		// Token: 0x040011DB RID: 4571
		private RectInt _playableArea;

		// Token: 0x040011DC RID: 4572
		public CityDefinition.DestinationVisualVariantType destinationVisualVariantType;

		// Token: 0x040011DD RID: 4573
		private readonly HashSet<UpgradeType> _availableUpgrades = new HashSet<UpgradeType>();

		// Token: 0x02000367 RID: 871
		public enum DestinationVisualVariantType
		{
			// Token: 0x040011E0 RID: 4576
			Standard,
			// Token: 0x040011E1 RID: 4577
			SpireShadows
		}
	}
}
