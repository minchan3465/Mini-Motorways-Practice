using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways;
using Motorways.Models;
using Server;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class MotorwaysDevToolCommand : BaseInGameDevToolCommand<MotorwaysDevToolCommand>
{
	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060001E7 RID: 487 RVA: 0x00006580 File Offset: 0x00004780
	// (set) Token: 0x060001E8 RID: 488 RVA: 0x00006588 File Offset: 0x00004788
	[Dependency]
	public IScope Scope { get; protected set; }

	// Token: 0x060001E9 RID: 489 RVA: 0x00006591 File Offset: 0x00004791
	public void SpawnHouse(TileDirection drivewayDirection, int groupIndex)
	{
		this.SpawnHouse(drivewayDirection, groupIndex, this._clock.ExpansionTime);
	}

	// Token: 0x060001EA RID: 490 RVA: 0x000065A8 File Offset: 0x000047A8
	private void SpawnHouse(TileDirection drivewayDirection, int groupIndex, Fix64 spawnTime)
	{
		if (groupIndex < 0)
		{
			List<int> validIndexes = new List<int>();
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				if (!validIndexes.Contains(destination.GroupIndex))
				{
					validIndexes.Add(destination.GroupIndex);
				}
			}
			if (validIndexes.Count > 0)
			{
				groupIndex = validIndexes[this._cityModel.pseudorandomGenerator.Int(validIndexes.Count)];
			}
			else
			{
				groupIndex = 0;
			}
		}
		if (drivewayDirection == TileDirection.None)
		{
			drivewayDirection = TileUtilities.NonDiagonalDirections[this._cityModel.pseudorandomGenerator.Int(TileUtilities.NonDiagonalDirections.Length)];
		}
		CityPlanModel.ScheduledBuilding newHouse = this.Scope.Get<CityPlanModel.ScheduledBuilding>();
		newHouse.type = CityTileType.Supply;
		newHouse.groupIndex = groupIndex;
		newHouse.useFixedParameters = true;
		newHouse.positionOverride = this.cursorTilePosition;
		newHouse.drivewayDirectionOverride = drivewayDirection;
		newHouse.time = spawnTime;
		this._cityPlanModel.ScheduleBuilding(newHouse);
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000669C File Offset: 0x0000489C
	public void SpawnDestinationAtCursorPosition(CarparkEntrance carparkEntrance, CarparkPreference carparkPreference, TileDirection drivewayDirection, TileDirection carparkSide, int groupIndex, bool upgrade, int secondGroupIndex = -1, bool secondUpgrade = false)
	{
		this.SpawnDestination(this.cursorTilePosition, carparkEntrance, carparkPreference, drivewayDirection, groupIndex, this._clock.ExpansionTime, upgrade, secondGroupIndex, secondUpgrade, carparkSide);
	}

	// Token: 0x060001EC RID: 492 RVA: 0x000066D0 File Offset: 0x000048D0
	private void SpawnDestination(Vector2Int coordinate, CarparkEntrance carparkEntrance, CarparkPreference carparkPreference, TileDirection drivewayDirection, int groupIndex, Fix64 spawnTime, bool upgrade, int secondGroupIndex, bool secondUpgrade)
	{
		TileAlignment carparkAlignment;
		if (drivewayDirection == TileDirection.East || drivewayDirection == TileDirection.West)
		{
			carparkAlignment = TileAlignment.Horizontal;
		}
		else if (drivewayDirection == TileDirection.North || drivewayDirection == TileDirection.South)
		{
			carparkAlignment = TileAlignment.Vertical;
		}
		else
		{
			carparkAlignment = TileAlignment.None;
		}
		TileDirection carparkSide;
		if (carparkAlignment == TileAlignment.None)
		{
			carparkSide = TileDirection.None;
		}
		else if (carparkAlignment == TileAlignment.Horizontal)
		{
			carparkSide = TileDirection.South;
		}
		else
		{
			carparkSide = TileDirection.West;
		}
		this.SpawnDestination(coordinate, carparkEntrance, carparkPreference, drivewayDirection, groupIndex, spawnTime, upgrade, secondGroupIndex, secondUpgrade, carparkSide);
	}

	// Token: 0x060001ED RID: 493 RVA: 0x00006724 File Offset: 0x00004924
	private void SpawnDestination(Vector2Int coordinate, CarparkEntrance carparkEntrance, CarparkPreference carparkPreference, TileDirection drivewayDirection, int groupIndex, Fix64 spawnTime, bool upgrade, int secondGroupIndex, bool secondUpgrade, TileDirection carparkSide)
	{
		CityPlanModel.ScheduledBuilding firstDestination = this.Scope.Get<CityPlanModel.ScheduledBuilding>();
		firstDestination.type = CityTileType.Demand;
		firstDestination.groupIndex = groupIndex;
		firstDestination.initialUpgradeLevel = (upgrade ? 1 : 0);
		firstDestination.carparkPreference = carparkPreference;
		firstDestination.useFixedParameters = true;
		firstDestination.positionOverride = coordinate;
		firstDestination.drivewayDirectionOverride = drivewayDirection;
		firstDestination.entranceOverride = carparkEntrance;
		firstDestination.time = spawnTime;
		firstDestination.demandMultiplier = Fix64.One;
		firstDestination.carparkSideOverride = carparkSide;
		this._cityPlanModel.ScheduleBuilding(firstDestination);
		if (secondGroupIndex != -1)
		{
			Vector2Int secondDestinationOffset = (drivewayDirection == TileDirection.East) ? CarparkModel.GenerateDestinationPositions(2, TileDirection.South)[1] : CarparkModel.GenerateDestinationPositions(2, TileDirection.West)[1];
			CityPlanModel.ScheduledBuilding secondDestination = this.Scope.Get<CityPlanModel.ScheduledBuilding>();
			secondDestination.type = CityTileType.Demand;
			secondDestination.groupIndex = secondGroupIndex;
			secondDestination.initialUpgradeLevel = (secondUpgrade ? 1 : 0);
			secondDestination.carparkPreference = ((carparkPreference == CarparkPreference.Station || carparkPreference == CarparkPreference.JoinStation || carparkPreference == CarparkPreference.ForceNewStation) ? carparkPreference : CarparkPreference.Double);
			secondDestination.useFixedParameters = true;
			secondDestination.positionOverride = coordinate + secondDestinationOffset;
			secondDestination.drivewayDirectionOverride = drivewayDirection;
			secondDestination.entranceOverride = carparkEntrance;
			secondDestination.time = spawnTime;
			secondDestination.demandMultiplier = Fix64.One;
			secondDestination.carparkSideOverride = carparkSide;
			this._cityPlanModel.ScheduleBuilding(secondDestination);
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00006858 File Offset: 0x00004A58
	public void RemoveAnyBuilding()
	{
		this.RemoveAnyBuildingAtTileCoordinate(this.cursorTilePosition);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x00006868 File Offset: 0x00004A68
	public void RemoveAnyBuildingAtTileCoordinate(Vector2Int tileCoordinate)
	{
		Tile tile = this._simulation.GetModel<TilemapModel>().GetTile(tileCoordinate);
		this.RemoveSpecificBuildingOnTile(tile, TileContentType.House);
		this.RemoveSpecificBuildingOnTile(tile, TileContentType.Destination);
		this.RemoveSpecificBuildingOnTile(tile, TileContentType.Carpark);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x000068A0 File Offset: 0x00004AA0
	public void RemoveSpecificBuildingAtTileCoordinate(Vector2Int tileCoordinate, TileContentType explicitType)
	{
		Tile tile = this._simulation.GetModel<TilemapModel>().GetTile(tileCoordinate);
		this.RemoveSpecificBuildingOnTile(tile, explicitType);
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x000068C8 File Offset: 0x00004AC8
	public void RemoveSpecificBuildingOnTile(Tile tile, TileContentType explicitType)
	{
		if (tile != null)
		{
			if (explicitType == TileContentType.House && tile.ContentType == TileContentType.House)
			{
				(tile.ContentModel as HouseModel).Remove();
				return;
			}
			if (explicitType == TileContentType.Destination && tile.ContentType == TileContentType.Destination)
			{
				DestinationModel destinationModel = tile.ContentModel as DestinationModel;
				if (destinationModel == null)
				{
					return;
				}
				destinationModel.Carpark.Remove();
				return;
			}
			else if (explicitType == TileContentType.Carpark && tile.ContentType == TileContentType.Carpark)
			{
				CarparkModel carparkModel = tile.ContentModel as CarparkModel;
				if (carparkModel == null)
				{
					return;
				}
				carparkModel.Remove();
			}
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x00006940 File Offset: 0x00004B40
	public void ChangeGroupIndex(int groupIndex)
	{
		Tile focussedTile = this._simulation.GetModel<TilemapModel>().GetTile(this.cursorTilePosition);
		if (focussedTile != null)
		{
			if (focussedTile.ContentType == TileContentType.House)
			{
				Fix64 spawnTime = this._clock.ExpansionTime;
				TileDirection drivewayDir = (focussedTile.ContentModel as HouseModel).DrivewayLane.connection.output.direction;
				this.RemoveSpecificBuildingOnTile(focussedTile, TileContentType.House);
				this.SpawnHouse(drivewayDir, groupIndex, spawnTime);
				return;
			}
			if (focussedTile.ContentType == TileContentType.Destination)
			{
				Fix64 spawnTime2 = this._clock.ExpansionTime;
				DestinationModel destinationModel = focussedTile.ContentModel as DestinationModel;
				Vector2Int spawnCoordinate;
				if (destinationModel.Carpark.Alignment == TileAlignment.Vertical)
				{
					spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates - new Vector2Int(1, 0);
				}
				else
				{
					spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates;
				}
				CarparkEntrance entrance;
				if (destinationModel.Carpark.entranceAtTopLeft && destinationModel.Carpark.entranceAtBottomRight)
				{
					entrance = CarparkEntrance.TopLeftAndBottomRight;
				}
				else if (destinationModel.Carpark.entranceAtTopLeft)
				{
					entrance = CarparkEntrance.TopLeft;
				}
				else
				{
					entrance = CarparkEntrance.BottomRight;
				}
				CarparkPreference carparkPreference = destinationModel.Carpark.SupportsTwoDestinations ? CarparkPreference.Double : CarparkPreference.Solo;
				int otherGroupIndex = -1;
				bool otherUpgraded = false;
				bool isTargetDestinationInIndexZero = destinationModel.Carpark.destinations[0] == destinationModel;
				if (destinationModel.Carpark.SupportsTwoDestinations && destinationModel.Carpark.destinations.Count == 2)
				{
					DestinationModel destinationModel2 = destinationModel.Carpark.destinations[isTargetDestinationInIndexZero ? 1 : 0];
					otherGroupIndex = destinationModel2.GroupIndex;
					otherUpgraded = destinationModel2.IsUpgraded;
				}
				this.RemoveSpecificBuildingOnTile(focussedTile, TileContentType.Destination);
				TileDirection direction = (destinationModel.Carpark.Alignment == TileAlignment.Horizontal) ? TileDirection.East : TileDirection.South;
				if (isTargetDestinationInIndexZero)
				{
					this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, groupIndex, spawnTime2, destinationModel.IsUpgraded, otherGroupIndex, otherUpgraded);
					return;
				}
				this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, otherGroupIndex, spawnTime2, otherUpgraded, groupIndex, destinationModel.IsUpgraded);
			}
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x00006B48 File Offset: 0x00004D48
	public void RotateBuilding()
	{
		Tile focussedTile = this._simulation.GetModel<TilemapModel>().GetTile(this.cursorTilePosition);
		if (focussedTile != null)
		{
			if (focussedTile.ContentType == TileContentType.House)
			{
				Fix64 spawnTime = this._clock.ExpansionTime;
				HouseModel houseModel = focussedTile.ContentModel as HouseModel;
				TileDirection direction2 = houseModel.DrivewayLane.connection.output.direction;
				houseModel.Remove();
				TileDirection newDirection = TileUtilities.GetRotatedDirection(direction2, RoadTileRotation.QuarterTurn);
				this.SpawnHouse(newDirection, houseModel.GroupIndex, spawnTime);
				return;
			}
			if (focussedTile.ContentType == TileContentType.Destination || focussedTile.ContentType == TileContentType.Carpark)
			{
				Fix64 spawnTime2 = this._clock.ExpansionTime;
				DestinationModel destinationModel;
				if (focussedTile.ContentType == TileContentType.Carpark)
				{
					destinationModel = (focussedTile.ContentModel as CarparkModel).destinations[0];
				}
				else
				{
					destinationModel = (focussedTile.ContentModel as DestinationModel);
				}
				TileDirection direction;
				Vector2Int spawnCoordinate;
				if (destinationModel.Carpark.Alignment == TileAlignment.Vertical)
				{
					direction = TileDirection.East;
					spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates - new Vector2Int(0, 1);
				}
				else
				{
					direction = TileDirection.South;
					spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates - new Vector2Int(1, 0);
				}
				CarparkEntrance entrance;
				if (destinationModel.Carpark.entranceAtTopLeft && destinationModel.Carpark.entranceAtBottomRight)
				{
					entrance = CarparkEntrance.TopLeftAndBottomRight;
				}
				else if (destinationModel.Carpark.entranceAtTopLeft)
				{
					entrance = CarparkEntrance.TopLeft;
				}
				else
				{
					entrance = CarparkEntrance.BottomRight;
				}
				CarparkPreference carparkPreference = destinationModel.Carpark.SupportsTwoDestinations ? CarparkPreference.Double : CarparkPreference.Solo;
				int otherGroupIndex = -1;
				bool otherUpgraded = false;
				bool isTargetDestinationInIndexZero = destinationModel.Carpark.destinations[0] == destinationModel;
				if (destinationModel.Carpark.SupportsTwoDestinations && destinationModel.Carpark.destinations.Count == 2)
				{
					DestinationModel destinationModel2 = destinationModel.Carpark.destinations[isTargetDestinationInIndexZero ? 1 : 0];
					otherGroupIndex = destinationModel2.GroupIndex;
					otherUpgraded = destinationModel2.IsUpgraded;
				}
				this.RemoveAnyBuilding();
				if (isTargetDestinationInIndexZero)
				{
					this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, destinationModel.GroupIndex, spawnTime2, destinationModel.IsUpgraded, otherGroupIndex, otherUpgraded);
					return;
				}
				this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, otherGroupIndex, spawnTime2, otherUpgraded, destinationModel.GroupIndex, destinationModel.IsUpgraded);
			}
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x00006D90 File Offset: 0x00004F90
	public void FlipDestination()
	{
		Tile focussedTile = this._simulation.GetModel<TilemapModel>().GetTile(this.cursorTilePosition);
		if (focussedTile != null && (focussedTile.ContentType == TileContentType.Destination || focussedTile.ContentType == TileContentType.Carpark))
		{
			DestinationModel destinationModel;
			if (focussedTile.ContentType == TileContentType.Carpark)
			{
				destinationModel = (focussedTile.ContentModel as CarparkModel).destinations[0];
			}
			else
			{
				destinationModel = (focussedTile.ContentModel as DestinationModel);
			}
			Vector2Int spawnCoordinate;
			if (destinationModel.Carpark.Alignment == TileAlignment.Vertical)
			{
				spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates - new Vector2Int(1, 0);
			}
			else
			{
				spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates - new Vector2Int(0, 1);
			}
			if (destinationModel.Carpark.entranceAtTopLeft ^ destinationModel.Carpark.entranceAtBottomRight)
			{
				CarparkEntrance entrance;
				if (destinationModel.Carpark.entranceAtTopLeft)
				{
					entrance = CarparkEntrance.BottomRight;
				}
				else
				{
					entrance = CarparkEntrance.TopLeft;
				}
				int groupIndex = destinationModel.GroupIndex;
				CarparkPreference carparkPreference = destinationModel.Carpark.SupportsTwoDestinations ? CarparkPreference.Double : CarparkPreference.Solo;
				int otherGroupIndex = -1;
				bool otherUpgraded = false;
				bool isTargetDestinationInIndexZero = destinationModel.Carpark.destinations[0] == destinationModel;
				if (destinationModel.Carpark.SupportsTwoDestinations && destinationModel.Carpark.destinations.Count == 2)
				{
					DestinationModel destinationModel2 = destinationModel.Carpark.destinations[isTargetDestinationInIndexZero ? 1 : 0];
					otherGroupIndex = destinationModel2.GroupIndex;
					otherUpgraded = destinationModel2.IsUpgraded;
				}
				this.RemoveAnyBuilding();
				Fix64 spawnTime = this._clock.ExpansionTime;
				TileDirection direction = (destinationModel.Carpark.Alignment == TileAlignment.Horizontal) ? TileDirection.East : TileDirection.South;
				if (isTargetDestinationInIndexZero)
				{
					this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, destinationModel.GroupIndex, spawnTime, destinationModel.IsUpgraded, otherGroupIndex, otherUpgraded);
					return;
				}
				this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, otherGroupIndex, spawnTime, otherUpgraded, destinationModel.GroupIndex, destinationModel.IsUpgraded);
			}
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x00006F7D File Offset: 0x0000517D
	public void UpgradeDestination()
	{
		this.SetDestinationUpgraded(true);
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x00006F86 File Offset: 0x00005186
	public void DowngradeDestinations()
	{
		this.SetDestinationUpgraded(false);
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x00006F90 File Offset: 0x00005190
	private void SetDestinationUpgraded(bool isUpgraded)
	{
		Tile focussedTile = this._simulation.GetModel<TilemapModel>().GetTile(this.cursorTilePosition);
		if (focussedTile != null && focussedTile.ContentType == TileContentType.Destination)
		{
			DestinationModel destinationModel = focussedTile.ContentModel as DestinationModel;
			if (destinationModel.IsUpgraded != isUpgraded)
			{
				if (!isUpgraded)
				{
					Vector2Int spawnCoordinate;
					if (destinationModel.Carpark.Alignment == TileAlignment.Vertical)
					{
						spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates - new Vector2Int(1, 0);
					}
					else
					{
						spawnCoordinate = destinationModel.Carpark.destinations[0].TileModels[0].Coordinates;
					}
					CarparkEntrance entrance;
					if (destinationModel.Carpark.entranceAtTopLeft && destinationModel.Carpark.entranceAtBottomRight)
					{
						entrance = CarparkEntrance.TopLeftAndBottomRight;
					}
					else if (destinationModel.Carpark.entranceAtTopLeft)
					{
						entrance = CarparkEntrance.TopLeft;
					}
					else
					{
						entrance = CarparkEntrance.BottomRight;
					}
					CarparkPreference carparkPreference = destinationModel.Carpark.SupportsTwoDestinations ? CarparkPreference.Double : CarparkPreference.Solo;
					int otherGroupIndex = -1;
					bool otherUpgraded = false;
					bool isTargetDestinationInIndexZero = destinationModel.Carpark.destinations[0] == destinationModel;
					if (destinationModel.Carpark.SupportsTwoDestinations && destinationModel.Carpark.destinations.Count == 2)
					{
						DestinationModel destinationModel2 = destinationModel.Carpark.destinations[isTargetDestinationInIndexZero ? 1 : 0];
						otherGroupIndex = destinationModel2.GroupIndex;
						otherUpgraded = destinationModel2.IsUpgraded;
					}
					this.RemoveAnyBuilding();
					Fix64 spawnTime = this._clock.ExpansionTime;
					TileDirection direction = (destinationModel.Carpark.Alignment == TileAlignment.Horizontal) ? TileDirection.East : TileDirection.South;
					if (isTargetDestinationInIndexZero)
					{
						this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, destinationModel.GroupIndex, spawnTime, false, otherGroupIndex, otherUpgraded);
						return;
					}
					this.SpawnDestination(spawnCoordinate, entrance, carparkPreference, direction, otherGroupIndex, spawnTime, otherUpgraded, destinationModel.GroupIndex, false);
					return;
				}
				else
				{
					destinationModel.demandLevelUpTime = this._clock.ExpansionTime;
				}
			}
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000715B File Offset: 0x0000535B
	public void SetSpawningMode(CityPlanModel.BuildingSpawningMode mode)
	{
		this._cityPlanModel.SpawningMode = mode;
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x00007169 File Offset: 0x00005369
	public CityPlanModel.BuildingSpawningMode GetSpawningMode()
	{
		return this._cityPlanModel.SpawningMode;
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00007176 File Offset: 0x00005376
	public void SetClockPaused(bool paused)
	{
		this._clock.isPaused = paused;
	}

	// Token: 0x060001FB RID: 507 RVA: 0x00007184 File Offset: 0x00005384
	public void ChangePeepCount(int deltaPeepCount, DestinationModel destination)
	{
		if (deltaPeepCount > 0)
		{
			int demand = Mathf.Min(destination.TotalDemand + deltaPeepCount, this.Scope.Get<City>().Rules.GetMaximumDemandForDestination(destination));
			while (demand > destination.TotalDemand)
			{
				destination.unassignedDemand.Add(destination.GroupIndex);
			}
			return;
		}
		int demand2 = Mathf.Max(destination.unassignedDemand.Count + deltaPeepCount, 0);
		while (demand2 < destination.unassignedDemand.Count)
		{
			destination.unassignedDemand.RemoveAt(destination.unassignedDemand.Count - 1);
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x00007214 File Offset: 0x00005414
	public void ChangePeepCount(int deltaPeepCount, int targetGroupIndex = -1)
	{
		foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
		{
			if (destination.isActive && (destination.GroupIndex == targetGroupIndex || targetGroupIndex == -1))
			{
				this.ChangePeepCount(deltaPeepCount, destination);
			}
		}
	}

	// Token: 0x060001FD RID: 509 RVA: 0x00007268 File Offset: 0x00005468
	public void SetPinCountOnDestination(int pinCount)
	{
		Tile focussedTile = this._simulation.GetModel<TilemapModel>().GetTile(this.cursorTilePosition);
		if (focussedTile != null)
		{
			IModel contentModel = focussedTile.ContentModel;
			CarparkModel carparkModel = contentModel as CarparkModel;
			DestinationModel destination;
			if (carparkModel == null)
			{
				DestinationModel destinationModel = contentModel as DestinationModel;
				if (destinationModel == null)
				{
					Diagnostics.FailAssert("Can't find destination from {0}.", new object[]
					{
						focussedTile.ContentModel
					});
					return;
				}
				destination = destinationModel;
			}
			else
			{
				destination = carparkModel.destinations[0];
			}
			int deltaPins = pinCount - destination.unassignedDemand.Count;
			this.ChangePeepCount(deltaPins, destination);
		}
	}

	// Token: 0x060001FE RID: 510 RVA: 0x000072F8 File Offset: 0x000054F8
	public void SetGroupIndex(int groupIndex)
	{
		Tile focussedTile = this._simulation.GetModel<TilemapModel>().GetTile(this.cursorTilePosition);
		if (focussedTile != null)
		{
			IModel contentModel = focussedTile.ContentModel;
			HouseModel houseModel = contentModel as HouseModel;
			if (houseModel != null)
			{
				houseModel.GroupIndex = groupIndex;
				return;
			}
			DestinationModel destinationModel = contentModel as DestinationModel;
			if (destinationModel != null)
			{
				destinationModel.GroupIndex = groupIndex;
				return;
			}
			Diagnostics.FailAssert("Can't set group index on {0}.", new object[]
			{
				focussedTile.ContentModel
			});
		}
	}

	// Token: 0x040000BD RID: 189
	[Dependency]
	protected CityPlanModel _cityPlanModel;

	// Token: 0x040000BE RID: 190
	[Dependency]
	protected CityModel _cityModel;

	// Token: 0x040000BF RID: 191
	[Dependency]
	protected ClockModel _clock;

	// Token: 0x040000C0 RID: 192
	[Dependency]
	protected ISimulation _simulation;
}
