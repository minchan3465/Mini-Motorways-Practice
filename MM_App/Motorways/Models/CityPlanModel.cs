using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Processes;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004DD RID: 1245
	public class CityPlanModel : IModel, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06002070 RID: 8304 RVA: 0x00080922 File Offset: 0x0007EB22
		// (set) Token: 0x06002071 RID: 8305 RVA: 0x0008092A File Offset: 0x0007EB2A
		[Serialize(true, null)]
		public Fix64 DoubleDestinationProbability { get; private set; } = CityPlanModel.BaselineDoubleDestinationProbability;

		// Token: 0x06002072 RID: 8306 RVA: 0x00080933 File Offset: 0x0007EB33
		public bool IsBuildingSpawningModeSet(CityPlanModel.BuildingSpawningMode mode)
		{
			return (this.SpawningMode & mode) == mode;
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x00080940 File Offset: 0x0007EB40
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (CityPlanModel.ScheduledBuilding scheduledBuilding in this.scheduledBuildings)
			{
				scope.Release(scheduledBuilding);
			}
			this.scheduledBuildings.Clear();
			foreach (TileMatrixInt dataGrid in this._nearbyHouseCountOfGroup.Values)
			{
				scope.Release(dataGrid);
			}
			this._nearbyHouseCountOfGroup.Clear();
			foreach (TileMatrixInt dataGrid2 in this._distanceToNearestHouseOfGroup.Values)
			{
				scope.Release(dataGrid2);
			}
			this._distanceToNearestHouseOfGroup.Clear();
			foreach (TileMatrixInt dataGrid3 in this._distanceToNearestDestinationOfGroup.Values)
			{
				scope.Release(dataGrid3);
			}
			this._distanceToNearestDestinationOfGroup.Clear();
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x00080A9C File Offset: 0x0007EC9C
		public void Reset()
		{
			this.scheduledBuildings.Clear();
			this.latestHouseSpawnTime.Clear();
			Array.Clear(this.groupHouseCounts, 0, this.groupHouseCounts.Length);
			this.suburbCount.Clear();
			this.SpawningMode = CityPlanModel.BuildingSpawningMode.All;
			this.DoubleDestinationProbability = CityPlanModel.BaselineDoubleDestinationProbability;
			this.destinationLanes.Clear();
			this._nearbyHouseCountOfGroup.Clear();
			this._distanceToNearestHouseOfGroup.Clear();
			this._distanceToNearestDestinationOfGroup.Clear();
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x00080B1C File Offset: 0x0007ED1C
		public bool IsHouseScheduled(int groupIndex)
		{
			foreach (CityPlanModel.ScheduledBuilding building in this.scheduledBuildings)
			{
				if (building.type == CityTileType.Supply && building.groupIndex == groupIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x00080B84 File Offset: 0x0007ED84
		public Fix64 GetEarliestHouseSpawnTime(int groupIndex, Fix64 earliestTime)
		{
			Fix64 latestSpawnTime;
			if (this.latestHouseSpawnTime.TryGetValue(groupIndex, out latestSpawnTime))
			{
				return Fix64.Max(earliestTime, latestSpawnTime + this._constants.DelayBetweenSameGroupHouseSpawns);
			}
			return earliestTime;
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x00080BBC File Offset: 0x0007EDBC
		public int GetNearbyHouseCountOfGroup(Vector2Int tileCoordinates, int groupIndex)
		{
			TileMatrixInt houseCountMatrix;
			if (this._nearbyHouseCountOfGroup.TryGetValue(groupIndex, out houseCountMatrix))
			{
				return houseCountMatrix[tileCoordinates];
			}
			return 0;
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x00080BE4 File Offset: 0x0007EDE4
		public TileMatrixInt GetHouseDistanceMatrixForGroup(int groupIndex)
		{
			TileMatrixInt houseDistanceMatrix;
			if (this._distanceToNearestHouseOfGroup.TryGetValue(groupIndex, out houseDistanceMatrix))
			{
				return houseDistanceMatrix;
			}
			return null;
		}

		// Token: 0x06002079 RID: 8313 RVA: 0x00080C04 File Offset: 0x0007EE04
		public TileMatrixInt GetHouseCountMatrixForGroup(int groupIndex)
		{
			TileMatrixInt houseCountMatrix;
			if (this._nearbyHouseCountOfGroup.TryGetValue(groupIndex, out houseCountMatrix))
			{
				return houseCountMatrix;
			}
			return null;
		}

		// Token: 0x0600207A RID: 8314 RVA: 0x00080C24 File Offset: 0x0007EE24
		public TileMatrixInt GetDestinationMatrixForGroup(int groupIndex)
		{
			TileMatrixInt destinationDistanceMatrix;
			if (this._distanceToNearestDestinationOfGroup.TryGetValue(groupIndex, out destinationDistanceMatrix))
			{
				return destinationDistanceMatrix;
			}
			return null;
		}

		// Token: 0x0600207B RID: 8315 RVA: 0x00080C44 File Offset: 0x0007EE44
		public int GetDistanceToNearestSupplyOfGroup(Vector2Int tileCoordinates, int groupIndex)
		{
			TileMatrixInt distanceMatrix;
			if (this._distanceToNearestHouseOfGroup.TryGetValue(groupIndex, out distanceMatrix))
			{
				return distanceMatrix[tileCoordinates];
			}
			return int.MaxValue;
		}

		// Token: 0x0600207C RID: 8316 RVA: 0x00080C70 File Offset: 0x0007EE70
		public int GetDistanceToNearestSupplyNotOfGroup(Vector2Int tileCoordinates, int groupIndex)
		{
			int shortestDistance = int.MaxValue;
			foreach (KeyValuePair<int, TileMatrixInt> groupToDistanceMatrix in this._distanceToNearestHouseOfGroup)
			{
				if (groupToDistanceMatrix.Key != groupIndex)
				{
					int shortestDistanceToSupplyOfGroup = groupToDistanceMatrix.Value[tileCoordinates];
					if (shortestDistance > shortestDistanceToSupplyOfGroup)
					{
						shortestDistance = shortestDistanceToSupplyOfGroup;
					}
				}
			}
			return shortestDistance;
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x00080CE4 File Offset: 0x0007EEE4
		public int GetDistanceToNearestDemand(Vector2Int tileCoordinates)
		{
			int shortestDistance = int.MaxValue;
			foreach (TileMatrixInt tileMatrixInt in this._distanceToNearestDestinationOfGroup.Values)
			{
				int shortestDistanceToSupplyOfGroup = tileMatrixInt[tileCoordinates];
				if (shortestDistance > shortestDistanceToSupplyOfGroup)
				{
					shortestDistance = shortestDistanceToSupplyOfGroup;
				}
			}
			return shortestDistance;
		}

		// Token: 0x0600207E RID: 8318 RVA: 0x00080D48 File Offset: 0x0007EF48
		public int GetDistanceToNearestDemandOfGroup(Vector2Int tileCoordinates, int groupIndex)
		{
			TileMatrixInt distanceMatrix;
			if (this._distanceToNearestDestinationOfGroup.TryGetValue(groupIndex, out distanceMatrix))
			{
				return distanceMatrix[tileCoordinates];
			}
			return int.MaxValue;
		}

		// Token: 0x0600207F RID: 8319 RVA: 0x00080D74 File Offset: 0x0007EF74
		public void RecordNewHouse(HouseModel model)
		{
			List<Vector2Int> startCoordinates = new List<Vector2Int>
			{
				model.tileModel.Coordinates
			};
			int groupIndex = model.GroupIndex;
			TileMatrixInt distanceMatrix;
			if (!this._distanceToNearestHouseOfGroup.TryGetValue(groupIndex, out distanceMatrix))
			{
				distanceMatrix = TileMatrixInt.Create(this._scope, this._city.Definition.PlayableArea, int.MaxValue);
				this._distanceToNearestHouseOfGroup.Add(groupIndex, distanceMatrix);
			}
			distanceMatrix.FloodFill(startCoordinates, 0, (int color) => color + 1, new TileMatrix<int>.CanFloodFillEnterTile(this.CanFloodFillEnterCell));
			TileMatrixInt houseCountMatrix;
			if (!this._nearbyHouseCountOfGroup.TryGetValue(groupIndex, out houseCountMatrix))
			{
				houseCountMatrix = TileMatrixInt.Create(this._scope, this._city.Definition.PlayableArea, 0);
				this._nearbyHouseCountOfGroup.Add(groupIndex, houseCountMatrix);
			}
			Vector2Int circleOrigin = model.tileModel.Coordinates;
			int minX = Mathf.Max(circleOrigin.x - 5, houseCountMatrix.Dimensions.xMin);
			int num = Mathf.Max(circleOrigin.y - 5, houseCountMatrix.Dimensions.yMin);
			int maxX = Mathf.Min(circleOrigin.x + 5, houseCountMatrix.Dimensions.xMax - 1);
			int maxY = Mathf.Min(circleOrigin.y + 5, houseCountMatrix.Dimensions.yMax - 1);
			for (int circleY = num; circleY <= maxY; circleY++)
			{
				for (int circleX = minX; circleX <= maxX; circleX++)
				{
					Vector2Int coordinates = new Vector2Int(circleX, circleY);
					if ((coordinates - circleOrigin).sqrMagnitude <= 25)
					{
						int houseCount = houseCountMatrix[coordinates];
						houseCountMatrix[coordinates] = houseCount + 1;
					}
				}
			}
			this.groupHouseCounts[model.GroupIndex]++;
			this.UpdateLatestHouseSpawnTime(model.GroupIndex, this._clock.ExpansionTime);
		}

		// Token: 0x06002080 RID: 8320 RVA: 0x00080F5C File Offset: 0x0007F15C
		public void RecordNewDestination(DestinationModel model)
		{
			List<Vector2Int> startCoordinates = new List<Vector2Int>();
			foreach (TileModel destinationTile in model.TileModels)
			{
				startCoordinates.Add(destinationTile.Coordinates);
			}
			int groupIndex = model.GroupIndex;
			TileMatrixInt distanceMatrix;
			if (!this._distanceToNearestDestinationOfGroup.TryGetValue(groupIndex, out distanceMatrix))
			{
				distanceMatrix = TileMatrixInt.Create(this._scope, this._city.Definition.PlayableArea, int.MaxValue);
				this._distanceToNearestDestinationOfGroup.Add(groupIndex, distanceMatrix);
			}
			distanceMatrix.FloodFill(startCoordinates, 0, (int color) => color + 1, new TileMatrix<int>.CanFloodFillEnterTile(this.CanFloodFillEnterCell));
		}

		// Token: 0x06002081 RID: 8321 RVA: 0x00081034 File Offset: 0x0007F234
		public void ResetDoubleDestinationProbability()
		{
			this.DoubleDestinationProbability = CityPlanModel.BaselineDoubleDestinationProbability;
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x00081041 File Offset: 0x0007F241
		public void IncreaseDoubleDestinationProbability()
		{
			this.DoubleDestinationProbability += CityPlanModel.DoubleDestinationProbabilityIncrease;
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x0008105C File Offset: 0x0007F25C
		public void ScheduleBuilding(CityPlanModel.ScheduledBuilding building)
		{
			int insertIndex = this.scheduledBuildings.Count;
			while (insertIndex > 0 && this.scheduledBuildings[insertIndex - 1].time > building.time)
			{
				insertIndex--;
			}
			this.scheduledBuildings.Insert(insertIndex, building);
			if (building.type == CityTileType.Supply)
			{
				this.UpdateLatestHouseSpawnTime(building.groupIndex, building.time);
			}
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x000810C8 File Offset: 0x0007F2C8
		private void UpdateLatestHouseSpawnTime(int groupIndex, Fix64 newSpawnTime)
		{
			Fix64 latestSpawnTime;
			if (this.latestHouseSpawnTime.TryGetValue(groupIndex, out latestSpawnTime))
			{
				if (newSpawnTime > latestSpawnTime)
				{
					this.latestHouseSpawnTime[groupIndex] = newSpawnTime;
					return;
				}
			}
			else
			{
				this.latestHouseSpawnTime[groupIndex] = newSpawnTime;
			}
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x0008110C File Offset: 0x0007F30C
		private bool CanFloodFillEnterCell(Vector2Int coordinate, int stepCount, int targetDistance, int replacementDistance)
		{
			return replacementDistance < targetDistance && !this._city.Definition.TileIsOverWater(coordinate) && !this._city.Definition.TileIsUnderAMountain(coordinate) && !this._city.Definition.TileIsOverRail(coordinate);
		}

		// Token: 0x06002086 RID: 8326 RVA: 0x0008115A File Offset: 0x0007F35A
		private int ReplaceFloodFillColor(int data, int color)
		{
			return color;
		}

		// Token: 0x04001AEC RID: 6892
		public readonly List<CityPlanModel.ScheduledBuilding> scheduledBuildings = new List<CityPlanModel.ScheduledBuilding>();

		// Token: 0x04001AED RID: 6893
		public readonly Dictionary<int, Fix64> latestHouseSpawnTime = new Dictionary<int, Fix64>();

		// Token: 0x04001AEE RID: 6894
		public readonly int[] groupHouseCounts = new int[MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS];

		// Token: 0x04001AEF RID: 6895
		public readonly Dictionary<int, int> suburbCount = new Dictionary<int, int>();

		// Token: 0x04001AF0 RID: 6896
		private readonly Dictionary<int, TileMatrixInt> _nearbyHouseCountOfGroup = new Dictionary<int, TileMatrixInt>();

		// Token: 0x04001AF1 RID: 6897
		private readonly Dictionary<int, TileMatrixInt> _distanceToNearestHouseOfGroup = new Dictionary<int, TileMatrixInt>();

		// Token: 0x04001AF2 RID: 6898
		private readonly Dictionary<int, TileMatrixInt> _distanceToNearestDestinationOfGroup = new Dictionary<int, TileMatrixInt>();

		// Token: 0x04001AF3 RID: 6899
		public readonly List<LaneModel> destinationLanes = new List<LaneModel>();

		// Token: 0x04001AF4 RID: 6900
		private const int InfiniteDistance = 2147483647;

		// Token: 0x04001AF5 RID: 6901
		[Dependency]
		private IScope _scope;

		// Token: 0x04001AF6 RID: 6902
		[Dependency]
		private City _city;

		// Token: 0x04001AF7 RID: 6903
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001AF8 RID: 6904
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001AF9 RID: 6905
		public CityPlanModel.BuildingSpawningMode SpawningMode = CityPlanModel.BuildingSpawningMode.All;

		// Token: 0x04001AFA RID: 6906
		public static readonly Fix64 MinimumTimeBeforeBuildingDemandLevelsUp = (Fix64)10L;

		// Token: 0x04001AFB RID: 6907
		private static readonly Fix64 BaselineDoubleDestinationProbability = (Fix64)0.3;

		// Token: 0x04001AFC RID: 6908
		private static readonly Fix64 DoubleDestinationProbabilityIncrease = (Fix64)0.05;

		// Token: 0x020004DE RID: 1246
		[Flags]
		public enum BuildingSpawningMode
		{
			// Token: 0x04001AFF RID: 6911
			None = 0,
			// Token: 0x04001B00 RID: 6912
			Houses = 1,
			// Token: 0x04001B01 RID: 6913
			Destinations = 2,
			// Token: 0x04001B02 RID: 6914
			All = 3
		}

		// Token: 0x020004DF RID: 1247
		[Factory.Serializable(1)]
		public class ScheduledBuilding : IReusable
		{
			// Token: 0x170005BD RID: 1469
			// (get) Token: 0x06002089 RID: 8329 RVA: 0x00081218 File Offset: 0x0007F418
			public bool PrefersDoubleCarpark
			{
				get
				{
					CarparkPreference carparkPreference = this.carparkPreference;
					return carparkPreference == CarparkPreference.Double || carparkPreference == CarparkPreference.ForceDouble || carparkPreference == CarparkPreference.ForceNewDouble || carparkPreference == CarparkPreference.Station || carparkPreference == CarparkPreference.ForceNewStation || carparkPreference == CarparkPreference.BoatTerminal;
				}
			}

			// Token: 0x0600208A RID: 8330 RVA: 0x00081250 File Offset: 0x0007F450
			public void Reset()
			{
				this.time = Fix64Consts.Zero;
				this.spawnAttempts = 0;
				this.type = CityTileType.Demand;
				this.groupIndex = 0;
				this.carparkPreference = CarparkPreference.NoPreference;
				this.grouping = GroupingStyle.Normal;
				this.demandMultiplier = Fix64Consts.Zero;
				this.initialUpgradeLevel = 0;
				this.useFixedParameters = false;
				this.positionOverride = Vector2Int.zero;
				this.entranceOverride = CarparkEntrance.TopLeft;
				this.drivewayDirectionOverride = TileDirection.North;
				this.carparkSideOverride = TileDirection.None;
				this.tutorialIdentifier = TutorialIdentifier.None;
			}

			// Token: 0x04001B03 RID: 6915
			public Fix64 time;

			// Token: 0x04001B04 RID: 6916
			public int spawnAttempts;

			// Token: 0x04001B05 RID: 6917
			public CityTileType type;

			// Token: 0x04001B06 RID: 6918
			public int groupIndex;

			// Token: 0x04001B07 RID: 6919
			public CarparkPreference carparkPreference;

			// Token: 0x04001B08 RID: 6920
			public GroupingStyle grouping;

			// Token: 0x04001B09 RID: 6921
			public Fix64 demandMultiplier;

			// Token: 0x04001B0A RID: 6922
			public int initialUpgradeLevel;

			// Token: 0x04001B0B RID: 6923
			public bool useFixedParameters;

			// Token: 0x04001B0C RID: 6924
			public Vector2Int positionOverride;

			// Token: 0x04001B0D RID: 6925
			public CarparkEntrance entranceOverride;

			// Token: 0x04001B0E RID: 6926
			public TileDirection drivewayDirectionOverride;

			// Token: 0x04001B0F RID: 6927
			public TileDirection carparkSideOverride;

			// Token: 0x04001B10 RID: 6928
			public TutorialIdentifier tutorialIdentifier;
		}
	}
}
