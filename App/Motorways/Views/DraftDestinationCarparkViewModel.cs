using System;
using FixMath;
using Motorways.Models;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005C0 RID: 1472
	public class DraftDestinationCarparkViewModel
	{
		// Token: 0x060028FE RID: 10494 RVA: 0x000B0366 File Offset: 0x000AE566
		public void InitializeNew(bool isDoubleIn, int groupIndex)
		{
			this.isDouble = isDoubleIn;
			this.isTrainStation = false;
			this.activeBuilding = this.building1;
			this.building1.groupIndex = groupIndex;
			this.carparkSide = TileDirection.West;
			this.buildingLayout = BuildingLayout.BuildingToSide;
		}

		// Token: 0x060028FF RID: 10495 RVA: 0x000B039C File Offset: 0x000AE59C
		public void InitializeExisting(DestinationModel destinationModel)
		{
			CarparkModel carparkModel = destinationModel.Carpark;
			if (!Diagnostics.Verify(carparkModel.destinations.Count > 0, "Carpark should have at least one destination"))
			{
				return;
			}
			this.isTrainStation = (carparkModel.ActiveDestinationCount > 0 && carparkModel.destinations[0].IsTrainStation);
			this.isBoatTerminal = carparkModel.supportsBoats;
			this.isDouble = carparkModel.SupportsTwoDestinations;
			this.isTrainStation = destinationModel.IsTrainStation;
			this.carparkSide = carparkModel.carparkSide;
			this.buildingLayout = ((carparkModel.Alignment == TileAlignment.Horizontal) ? BuildingLayout.BuildingAbove : BuildingLayout.BuildingToSide);
			this.bottomLeft = carparkModel.origin + (this.isDouble ? ((this.buildingLayout == BuildingLayout.BuildingAbove) ? (2 * Vector2Int.down) : Vector2Int.zero) : Vector2Int.up);
			if (this.buildingLayout == BuildingLayout.BuildingAbove)
			{
				this.bottomLeft += new Vector2Int(-1, 1);
			}
			string error = "";
			if (!this.SetCoordinateData(ref error))
			{
				Diagnostics.Log.Error("DraftDestinationCarparkViewModel", "Error setting coordinate data for view model: {0}", new object[]
				{
					error
				});
			}
			if (!this.isDouble)
			{
				if (carparkModel.entranceAtBottomRight)
				{
					this.singleDestinationAboveDrivewayDirections = Motorways.Models.DrivewayDirection.East;
					this.singleDestinationToSideDrivewayDirections = Motorways.Models.DrivewayDirection.South;
				}
				else
				{
					this.singleDestinationAboveDrivewayDirections = Motorways.Models.DrivewayDirection.West;
					this.singleDestinationToSideDrivewayDirections = Motorways.Models.DrivewayDirection.North;
				}
			}
			this.hasSecondDestination = (carparkModel.ActiveDestinationCount > 1);
			this.activeBuilding = this.building1;
			for (int buildingIndex = 0; buildingIndex < carparkModel.destinations.Count; buildingIndex++)
			{
				DestinationModel buildingModel = carparkModel.destinations[buildingIndex];
				DraftDestinationBuildingViewModel building = (buildingIndex == 0) ? this.building1 : this.building2;
				building.groupIndex = buildingModel.GroupIndex;
				building.upgradeLevel = (buildingModel.IsUpgraded ? 1 : 0);
				if (destinationModel == buildingModel)
				{
					this.activeBuilding = building;
				}
			}
		}

		// Token: 0x06002900 RID: 10496 RVA: 0x000B0560 File Offset: 0x000AE760
		public void RemoveBuilding(DraftDestinationBuildingViewModel building)
		{
			building.Reset();
			this.activeBuilding = null;
			this.hasSecondDestination = false;
			if (building == this.building1)
			{
				this.building1.groupIndex = this.building2.groupIndex;
				this.building1.upgradeLevel = this.building2.upgradeLevel;
				this.building2.Reset();
				this.activeBuilding = this.building1;
			}
		}

		// Token: 0x06002901 RID: 10497 RVA: 0x000B05D0 File Offset: 0x000AE7D0
		public void Reset()
		{
			this.isDouble = false;
			this.isTrainStation = false;
			this.isBoatTerminal = false;
			this.carparkSide = TileDirection.None;
			this.buildingLayout = BuildingLayout.BuildingAbove;
			this.bottomLeft = new Vector2Int(-1, -1);
			this.hasSecondDestination = false;
			this.singleDestinationAboveDrivewayDirections = Motorways.Models.DrivewayDirection.West;
			this.singleDestinationToSideDrivewayDirections = Motorways.Models.DrivewayDirection.North;
			this.building1.Reset();
			this.building2.Reset();
		}

		// Token: 0x06002902 RID: 10498 RVA: 0x000B0638 File Offset: 0x000AE838
		public Vector3 GetLocalPositionBuilding1()
		{
			return this.GetLocalPositionBuilding(this.building1);
		}

		// Token: 0x06002903 RID: 10499 RVA: 0x000B0646 File Offset: 0x000AE846
		public Vector3 GetLocalPositionBuilding2()
		{
			return this.GetLocalPositionBuilding(this.building2);
		}

		// Token: 0x06002904 RID: 10500 RVA: 0x000B0654 File Offset: 0x000AE854
		private Vector3 GetLocalPositionBuilding(DraftDestinationBuildingViewModel building)
		{
			if (!this.isDouble || (building == this.activeBuilding && !this.isTrainStation))
			{
				return Vector3.zero;
			}
			Vector3 localPositionForBuilding = Vector3.zero;
			if (building == this.building1 && this.activeBuilding == this.building2)
			{
				localPositionForBuilding = 4f * ((this.buildingLayout == BuildingLayout.BuildingAbove) ? Vector3.left : Vector3.up);
			}
			else if (building == this.building2 && this.activeBuilding == this.building1)
			{
				localPositionForBuilding = 4f * ((this.buildingLayout == BuildingLayout.BuildingAbove) ? Vector3.right : Vector3.down);
			}
			if (this.isTrainStation && this.carparkSide == TileDirection.West)
			{
				localPositionForBuilding += 1.5f * Vector3.left;
			}
			else if (this.isTrainStation && this.carparkSide == TileDirection.North)
			{
				localPositionForBuilding += 1.5f * Vector3.up;
			}
			return localPositionForBuilding;
		}

		// Token: 0x06002905 RID: 10501 RVA: 0x000B0744 File Offset: 0x000AE944
		public Vector3 GetWorldPositionBuilding1()
		{
			Vector2Int minBuildingCoordinate = this.bottomLeft + (this.isDouble ? new Vector2Int(1, 2) : new Vector2Int(1, -1));
			if (this.isTrainStation && this.carparkSide == TileDirection.North)
			{
				minBuildingCoordinate += Vector2Int.down;
			}
			else if (this.isTrainStation && this.carparkSide == TileDirection.East)
			{
				minBuildingCoordinate += Vector2Int.left;
			}
			Vector2Int maxBuildingCoordinate = minBuildingCoordinate + Vector2Int.one;
			return 0.5f * (Vector3)(TilemapModel.GetWorldPositionForCoordinates(minBuildingCoordinate) + TilemapModel.GetWorldPositionForCoordinates(maxBuildingCoordinate));
		}

		// Token: 0x06002906 RID: 10502 RVA: 0x000B07DC File Offset: 0x000AE9DC
		public Vector3 GetWorldPositionBuilding2()
		{
			Vector2Int minBuildingCoordinate = this.bottomLeft + (this.isDouble ? new Vector2Int(1, 2) : new Vector2Int(1, -1)) + 2 * ((this.buildingLayout == BuildingLayout.BuildingAbove) ? Vector2Int.right : Vector2Int.down);
			if (this.isTrainStation && this.carparkSide == TileDirection.North)
			{
				minBuildingCoordinate += Vector2Int.down;
			}
			else if (this.isTrainStation && this.carparkSide == TileDirection.East)
			{
				minBuildingCoordinate += Vector2Int.left;
			}
			Vector2Int maxBuildingCoordinate = minBuildingCoordinate + Vector2Int.one;
			return 0.5f * (Vector3)(TilemapModel.GetWorldPositionForCoordinates(minBuildingCoordinate) + TilemapModel.GetWorldPositionForCoordinates(maxBuildingCoordinate));
		}

		// Token: 0x06002907 RID: 10503 RVA: 0x000B0893 File Offset: 0x000AEA93
		public Vector3 GetWorldPositionForActiveBuilding()
		{
			if (this.activeBuilding == this.building1)
			{
				return this.GetWorldPositionBuilding1();
			}
			return this.GetWorldPositionBuilding2();
		}

		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x06002908 RID: 10504 RVA: 0x000B08B0 File Offset: 0x000AEAB0
		public DrivewayDirection DesiredDirection
		{
			get
			{
				if (this.isDouble)
				{
					return Motorways.Models.DrivewayDirection.Both;
				}
				if (this.buildingLayout == BuildingLayout.BuildingAbove)
				{
					return this.singleDestinationAboveDrivewayDirections;
				}
				return this.singleDestinationToSideDrivewayDirections;
			}
		}

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06002909 RID: 10505 RVA: 0x000B08D4 File Offset: 0x000AEAD4
		public CarparkEntrance CarparkEntrance
		{
			get
			{
				CarparkEntrance carparkEntrance = CarparkEntrance.TopLeft;
				switch (this.DesiredDirection)
				{
				case Motorways.Models.DrivewayDirection.West:
					carparkEntrance = CarparkEntrance.TopLeft;
					break;
				case Motorways.Models.DrivewayDirection.East:
					carparkEntrance = CarparkEntrance.BottomRight;
					break;
				case Motorways.Models.DrivewayDirection.North:
					carparkEntrance = CarparkEntrance.TopLeft;
					break;
				case Motorways.Models.DrivewayDirection.South:
					carparkEntrance = CarparkEntrance.BottomRight;
					break;
				case Motorways.Models.DrivewayDirection.Both:
					carparkEntrance = CarparkEntrance.TopLeftAndBottomRight;
					break;
				}
				return carparkEntrance;
			}
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x0600290A RID: 10506 RVA: 0x000B0919 File Offset: 0x000AEB19
		public CarparkPreference CarparkPref
		{
			get
			{
				if (this.isBoatTerminal)
				{
					return CarparkPreference.BoatTerminal;
				}
				if (this.isTrainStation)
				{
					return CarparkPreference.ForceNewStation;
				}
				if (!this.isDouble)
				{
					return CarparkPreference.Solo;
				}
				return CarparkPreference.ForceNewDouble;
			}
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x0600290B RID: 10507 RVA: 0x000B093B File Offset: 0x000AEB3B
		public TileDirection DrivewayDirection
		{
			get
			{
				if (this.buildingLayout != BuildingLayout.BuildingAbove)
				{
					return TileDirection.South;
				}
				return TileDirection.East;
			}
		}

		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x0600290C RID: 10508 RVA: 0x000B0948 File Offset: 0x000AEB48
		public Vector2Int PositionOverride
		{
			get
			{
				return this.bottomLeft + (this.isDouble ? ((this.buildingLayout == BuildingLayout.BuildingAbove) ? Vector2Int.one : Vector2Int.zero) : ((this.buildingLayout == BuildingLayout.BuildingAbove) ? (Vector2Int.one + 3 * Vector2Int.down) : Vector2Int.down));
			}
		}

		// Token: 0x0600290D RID: 10509 RVA: 0x000B09A4 File Offset: 0x000AEBA4
		public bool SetCoordinateData(ref string errorMessage)
		{
			bool setSuccessfully = true;
			this.minCoordinates = this.bottomLeft + (this.isDouble ? ((this.buildingLayout == BuildingLayout.BuildingAbove) ? Vector2Int.one : Vector2Int.zero) : ((this.buildingLayout == BuildingLayout.BuildingAbove) ? (Vector2Int.right + 2 * Vector2Int.down) : Vector2Int.down));
			this.maxCoordinates = this.minCoordinates + (this.isDouble ? ((this.buildingLayout == BuildingLayout.BuildingAbove) ? new Vector2Int(3, 2) : new Vector2Int(2, 3)) : ((this.buildingLayout == BuildingLayout.BuildingAbove) ? new Vector2Int(1, 2) : new Vector2Int(2, 1)));
			this.carparkCoordinates = this.minCoordinates + (this.isDouble ? ((this.buildingLayout == BuildingLayout.BuildingAbove) ? new Vector2Int(-2, 0) : Vector2Int.zero) : ((this.buildingLayout == BuildingLayout.BuildingAbove) ? Vector2Int.zero : new Vector2Int(0, 1)));
			BuildingLayout buildingLayout = this.buildingLayout;
			if (buildingLayout != BuildingLayout.BuildingAbove)
			{
				if (buildingLayout != BuildingLayout.BuildingToSide)
				{
					setSuccessfully = false;
					errorMessage = "Unhandled building layout " + this.buildingLayout.ToString();
					this.drivewayCoordinates = this.carparkCoordinates + new Vector2Int(0, 1);
				}
				else if (this.isDouble)
				{
					this.drivewayCoordinates = this.carparkCoordinates + Vector2Int.down;
					this.secondDrivewayCoordinates = this.carparkCoordinates + 4 * Vector2Int.up;
				}
				else
				{
					switch (this.singleDestinationToSideDrivewayDirections)
					{
					case Motorways.Models.DrivewayDirection.North:
						this.drivewayCoordinates = this.carparkCoordinates + Vector2Int.up;
						return setSuccessfully;
					case Motorways.Models.DrivewayDirection.South:
						this.drivewayCoordinates = this.carparkCoordinates + 2 * Vector2Int.down;
						return setSuccessfully;
					}
					setSuccessfully = false;
					errorMessage = "Invalid driveway direction " + this.singleDestinationToSideDrivewayDirections.ToString() + " for building layout " + this.buildingLayout.ToString();
					this.drivewayCoordinates = this.carparkCoordinates + new Vector2Int(0, 1);
				}
			}
			else if (this.isDouble)
			{
				this.drivewayCoordinates = this.minCoordinates + Vector2Int.left;
				this.secondDrivewayCoordinates = this.minCoordinates + 4 * Vector2Int.right;
			}
			else
			{
				switch (this.singleDestinationAboveDrivewayDirections)
				{
				case Motorways.Models.DrivewayDirection.West:
					this.drivewayCoordinates = this.carparkCoordinates + Vector2Int.left;
					return setSuccessfully;
				case Motorways.Models.DrivewayDirection.East:
					this.drivewayCoordinates = this.carparkCoordinates + 2 * Vector2Int.right;
					return setSuccessfully;
				}
				setSuccessfully = false;
				errorMessage = "Invalid driveway direction " + this.singleDestinationAboveDrivewayDirections.ToString() + " for building layout " + this.buildingLayout.ToString();
				this.drivewayCoordinates = this.carparkCoordinates + new Vector2Int(0, 1);
			}
			return setSuccessfully;
		}

		// Token: 0x0600290E RID: 10510 RVA: 0x000B0CC4 File Offset: 0x000AEEC4
		public void BuildScheduled(DraftDestinationBuildingViewModel building, ref CityPlanModel.ScheduledBuilding scheduled)
		{
			scheduled.type = CityTileType.Demand;
			scheduled.carparkPreference = this.CarparkPref;
			scheduled.useFixedParameters = true;
			scheduled.positionOverride = this.PositionOverride;
			scheduled.drivewayDirectionOverride = this.DrivewayDirection;
			scheduled.entranceOverride = this.CarparkEntrance;
			scheduled.time = Fix64.Zero;
			scheduled.demandMultiplier = Fix64.One;
			scheduled.carparkSideOverride = this.carparkSide;
			scheduled.groupIndex = building.groupIndex;
			scheduled.initialUpgradeLevel = ((this.CarparkPref != CarparkPreference.ForceNewStation) ? building.upgradeLevel : 0);
			if (building == this.building2)
			{
				Vector2Int topLeft = this.PositionOverride + ((this.buildingLayout == BuildingLayout.BuildingAbove) ? new Vector2Int(0, 2) : new Vector2Int(0, 3));
				scheduled.positionOverride = topLeft;
				scheduled.carparkPreference = (this.isBoatTerminal ? CarparkPreference.JoinBoatTerminal : (this.isTrainStation ? CarparkPreference.Station : CarparkPreference.Double));
			}
		}

		// Token: 0x040022BF RID: 8895
		public bool isDouble;

		// Token: 0x040022C0 RID: 8896
		public bool hasSecondDestination;

		// Token: 0x040022C1 RID: 8897
		public TileDirection carparkSide = TileDirection.None;

		// Token: 0x040022C2 RID: 8898
		public BuildingLayout buildingLayout = BuildingLayout.BuildingToSide;

		// Token: 0x040022C3 RID: 8899
		public Vector2Int bottomLeft = new Vector2Int(-1, -1);

		// Token: 0x040022C4 RID: 8900
		public DrivewayDirection singleDestinationAboveDrivewayDirections;

		// Token: 0x040022C5 RID: 8901
		public DrivewayDirection singleDestinationToSideDrivewayDirections = Motorways.Models.DrivewayDirection.North;

		// Token: 0x040022C6 RID: 8902
		public readonly DraftDestinationBuildingViewModel building1 = new DraftDestinationBuildingViewModel();

		// Token: 0x040022C7 RID: 8903
		public readonly DraftDestinationBuildingViewModel building2 = new DraftDestinationBuildingViewModel();

		// Token: 0x040022C8 RID: 8904
		public DraftDestinationBuildingViewModel activeBuilding;

		// Token: 0x040022C9 RID: 8905
		public bool isTrainStation;

		// Token: 0x040022CA RID: 8906
		public bool isBoatTerminal;

		// Token: 0x040022CB RID: 8907
		public Vector2Int maxCoordinates;

		// Token: 0x040022CC RID: 8908
		public Vector2Int minCoordinates;

		// Token: 0x040022CD RID: 8909
		public Vector2Int carparkCoordinates;

		// Token: 0x040022CE RID: 8910
		public Vector2Int drivewayCoordinates;

		// Token: 0x040022CF RID: 8911
		public Vector2Int secondDrivewayCoordinates;
	}
}
