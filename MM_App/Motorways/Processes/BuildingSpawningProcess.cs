using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using Unity.Profiling;
using UnityEngine;

namespace Motorways.Processes
{
	// Token: 0x02000485 RID: 1157
	public class BuildingSpawningProcess : IProcess, IReusable
	{
		// Token: 0x06001CB1 RID: 7345 RVA: 0x0006B514 File Offset: 0x00069714
		private static List<BuildingPlacer.RailPlatform> GeneratePlatformPositions(Vector2Int destinationFootprint, TileDirection carparkSide)
		{
			int num = (carparkSide == TileDirection.North || carparkSide == TileDirection.South) ? 1 : 2;
			int stationTileCount = (num == 1) ? destinationFootprint.x : destinationFootprint.y;
			TileDirection stationDirection = (num == 1) ? TileDirection.East : TileDirection.South;
			Vector2Int platformStartingTile = default(Vector2Int);
			if (num == 1)
			{
				if (carparkSide == TileDirection.South)
				{
					platformStartingTile.y = destinationFootprint.y - 1;
				}
			}
			else
			{
				platformStartingTile.y = destinationFootprint.y - 1;
				if (carparkSide == TileDirection.West)
				{
					platformStartingTile.x = destinationFootprint.x - 1;
				}
			}
			List<BuildingPlacer.RailPlatform> platforms = new List<BuildingPlacer.RailPlatform>();
			TileDirection railConnectionDirection = TileUtilities.GetOppositeDirection(stationDirection);
			for (int carparkTileIndex = 0; carparkTileIndex < stationTileCount; carparkTileIndex++)
			{
				Vector2Int stationPosition = platformStartingTile + TileUtilities.GetAdjacencyOffsetForDirection(stationDirection) * carparkTileIndex;
				platforms.Add(new BuildingPlacer.RailPlatform
				{
					connection = new TileDirectionBitfield(railConnectionDirection),
					coordinatesOffset = stationPosition
				});
			}
			return platforms;
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x0006B5E4 File Offset: 0x000697E4
		private static List<Vector2Int> GenerateBoatTerminalTiles(Vector2Int destinationFootprint, TileDirection carparkSide)
		{
			List<Vector2Int> terminalTiles = new List<Vector2Int>();
			if (carparkSide == TileDirection.South)
			{
				for (int i = 0; i < destinationFootprint.x; i++)
				{
					Vector2Int topRowTile = new Vector2Int(i, destinationFootprint.y - 1);
					terminalTiles.Add(topRowTile);
				}
				return terminalTiles;
			}
			throw new ArgumentOutOfRangeException("carparkSide", carparkSide, null);
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x0006B63A File Offset: 0x0006983A
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			if (!this._city.Rules.HasDisabledAutomaticSpawn())
			{
				this.ScheduleHouses(simulation);
			}
			this.SpawnScheduledBuildings(simulation, deltaTime);
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x0006B660 File Offset: 0x00069860
		private void ScheduleHouses(ISimulation simulation)
		{
			if (this._city.Rules.DoesIgnorePlayableArea())
			{
				return;
			}
			if (!this._cityPlanModel.IsBuildingSpawningModeSet(CityPlanModel.BuildingSpawningMode.Houses))
			{
				return;
			}
			if (this._city.Rules.UsesPerCityHouseGraph)
			{
				this.ScheduleHousesFromCityHouseCurve(simulation);
				return;
			}
			if (this._demandModel.doesSupplyNeedRecalculation)
			{
				this._demandModel.RecalculateSupply();
			}
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				int groupIndex = destination.GroupIndex;
				if (!this._cityPlanModel.IsHouseScheduled(groupIndex) && !destination.IsSupplySufficient)
				{
					BuildingSpawningProcess.Log.Info("Houses for group {0} are unable meet demand at time {1}s.", new object[]
					{
						groupIndex,
						this._clock.ExpansionTime
					});
					this.ScheduleNewHouse(groupIndex, simulation);
				}
			}
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x0006B73C File Offset: 0x0006993C
		private void ScheduleHousesFromCityHouseCurve(ISimulation simulation)
		{
			int[] maximumDemandPerGroup = new int[MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS];
			int[] minimumHousesPerGroup = new int[MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS];
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if (destination.isActive)
				{
					maximumDemandPerGroup[destination.GroupIndex] += destination.MaximumDemandBeforeTimerStarts;
					minimumHousesPerGroup[destination.GroupIndex]++;
				}
			}
			bool scheduledHouse = false;
			for (int groupIndex = 0; groupIndex < minimumHousesPerGroup.Length; groupIndex++)
			{
				if (minimumHousesPerGroup[groupIndex] > 0)
				{
					minimumHousesPerGroup[groupIndex] += this._city.Rules.AdditionalHousesPerGroup;
				}
				if (this._cityPlanModel.IsHouseScheduled(groupIndex))
				{
					scheduledHouse = true;
				}
				else if (this._cityPlanModel.groupHouseCounts[groupIndex] < minimumHousesPerGroup[groupIndex])
				{
					scheduledHouse = true;
					this.ScheduleNewHouse(groupIndex, simulation);
				}
			}
			if (!scheduledHouse && simulation.GetModels<HouseModel>().Count < this._city.Definition.GetHousesAtDay(this._clock.ExpansionDay))
			{
				int biggestDifferenceInDemand = int.MinValue;
				int groupIndexToChoose = 0;
				for (int groupIndex2 = 0; groupIndex2 < maximumDemandPerGroup.Length; groupIndex2++)
				{
					int currentDifference = Mathf.CeilToInt((float)maximumDemandPerGroup[groupIndex2] / 2f) - this._cityPlanModel.groupHouseCounts[groupIndex2];
					if (currentDifference > biggestDifferenceInDemand && maximumDemandPerGroup[groupIndex2] > 0)
					{
						biggestDifferenceInDemand = currentDifference;
						groupIndexToChoose = groupIndex2;
					}
				}
				this.ScheduleNewHouse(groupIndexToChoose, simulation);
			}
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x0006B8AC File Offset: 0x00069AAC
		private void ScheduleNewHouse(int groupIndex, ISimulation simulation)
		{
			CityPlanModel.ScheduledBuilding newHouse = this._scope.Get<CityPlanModel.ScheduledBuilding>();
			newHouse.type = CityTileType.Supply;
			newHouse.groupIndex = groupIndex;
			newHouse.grouping = this.AssessGroupingStyleForHouse(groupIndex, simulation);
			newHouse.time = this._cityPlanModel.GetEarliestHouseSpawnTime(groupIndex, this._clock.ExpansionTime);
			this._cityPlanModel.ScheduleBuilding(newHouse);
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x0006B90C File Offset: 0x00069B0C
		private void SpawnScheduledBuildings(ISimulation simulation, Fix64 deltaTime)
		{
			bool scheduledBuildingsRequiresSort = false;
			if (this._cityPlanModel.SpawningMode != CityPlanModel.BuildingSpawningMode.All)
			{
				scheduledBuildingsRequiresSort = (this._cityPlanModel.IsBuildingSpawningModeSet(CityPlanModel.BuildingSpawningMode.Destinations) ^ this._cityPlanModel.IsBuildingSpawningModeSet(CityPlanModel.BuildingSpawningMode.Houses));
				Fix64 previousBuildingTime = Fix64.Zero;
				for (int buildingIndex = 0; buildingIndex < this._cityPlanModel.scheduledBuildings.Count; buildingIndex++)
				{
					CityPlanModel.ScheduledBuilding building = this._cityPlanModel.scheduledBuildings[buildingIndex];
					if (building.useFixedParameters)
					{
						scheduledBuildingsRequiresSort = true;
					}
					else if (this._cityPlanModel.SpawningMode == CityPlanModel.BuildingSpawningMode.None || (building.type == CityTileType.Demand && !this._cityPlanModel.IsBuildingSpawningModeSet(CityPlanModel.BuildingSpawningMode.Destinations)) || (building.type == CityTileType.Supply && !this._cityPlanModel.IsBuildingSpawningModeSet(CityPlanModel.BuildingSpawningMode.Houses)))
					{
						building.time += deltaTime;
						if (building.time > previousBuildingTime)
						{
							previousBuildingTime = building.time;
						}
					}
				}
			}
			if (this._city.Rules.FailedSpawnsIgnoreStoppedExpansionTime && !this._city.Rules.CanExpansionTimeContinue)
			{
				Fix64 previousBuildingTime2 = Fix64.Zero;
				for (int buildingIndex2 = 0; buildingIndex2 < this._cityPlanModel.scheduledBuildings.Count; buildingIndex2++)
				{
					CityPlanModel.ScheduledBuilding building2 = this._cityPlanModel.scheduledBuildings[buildingIndex2];
					if (building2.useFixedParameters)
					{
						scheduledBuildingsRequiresSort = true;
					}
					else
					{
						if (building2.spawnAttempts > 0)
						{
							building2.time -= deltaTime;
							if (!scheduledBuildingsRequiresSort && building2.time < previousBuildingTime2)
							{
								scheduledBuildingsRequiresSort = true;
							}
						}
						previousBuildingTime2 = building2.time;
					}
				}
			}
			if (scheduledBuildingsRequiresSort)
			{
				this._cityPlanModel.scheduledBuildings.Sort((CityPlanModel.ScheduledBuilding first, CityPlanModel.ScheduledBuilding second) => first.time.CompareTo(second.time));
			}
			Fix64 time = this._clock.ExpansionTime;
			while (this._cityPlanModel.scheduledBuildings.Count > 0 && this._cityPlanModel.scheduledBuildings[0].time <= time)
			{
				bool releaseScheduledBuilding = true;
				try
				{
					BuildingSpawningProcess.Log.Info("Spawning new buildings in city {0} at time {1}s. Simulation seed is {2}.", new object[]
					{
						this._city.Definition.name,
						this._clock.ExpansionTime,
						this._cityModel.pseudorandomGenerator
					});
					CityPlanModel.ScheduledBuilding building3 = this._cityPlanModel.scheduledBuildings[0];
					BuildingPlacer.WeightEvaluationLevel weightEvaluationLevel = this._behaviour.GetDefaultBuildingWeightEvaluationLevel(building3.type);
					if (weightEvaluationLevel != BuildingPlacer.WeightEvaluationLevel.IgnoreWeights)
					{
						if (building3.useFixedParameters)
						{
							weightEvaluationLevel = BuildingPlacer.WeightEvaluationLevel.IgnoreWeights;
						}
						else if (building3.spawnAttempts > this._constants.MaxFailedBuildingSpawnsBeforeIgnoringWeights)
						{
							if (building3.type == CityTileType.Demand)
							{
								weightEvaluationLevel = BuildingPlacer.WeightEvaluationLevel.AllowNonWeightedTiles;
							}
							else
							{
								weightEvaluationLevel = BuildingPlacer.WeightEvaluationLevel.AllowNonWeightedTiles;
							}
						}
						else
						{
							weightEvaluationLevel = BuildingPlacer.WeightEvaluationLevel.ExclusivelyUseWeightedTiles;
						}
					}
					if (building3.type == CityTileType.Supply)
					{
						this._placer.StartPlacing(TileContentType.House, building3.groupIndex, building3.grouping, weightEvaluationLevel, BuildingPlacer.WeightSource.Default);
						if (!building3.useFixedParameters)
						{
							this._placer.GeneratePlacements(BuildingSpawningProcess.HouseLayouts);
						}
						else
						{
							BuildingPlacer.Driveway driveway = new BuildingPlacer.Driveway
							{
								direction = building3.drivewayDirectionOverride
							};
							BuildingPlacer.Layout fixedLayout = new BuildingPlacer.Layout
							{
								footprint = BuildingSpawningProcess.HouseFootprint,
								driveways = new List<BuildingPlacer.Driveway>
								{
									driveway
								}
							};
							Diagnostics.Verify(this._placer.GenerateFixedPlacement(fixedLayout, building3.positionOverride), "Couldn't generate a valid fixed placement at {0}!", building3.positionOverride);
						}
						BuildingPlacer.Placement placement = this._placer.ChoosePlacement();
						if (placement != null)
						{
							BuildingSpawningProcess.Log.Info("Placed house of group {0} on tile {1} at time {2}s.", new object[]
							{
								building3.groupIndex,
								placement.coordinates,
								this._clock.ExpansionTime
							});
							Tile tile = this._tilemapModel.GetTile(placement.coordinates);
							if (tile != null && tile.HasTrafficLight)
							{
								TileEditResult trafficLightEditResult = this._tileEditor.ClearTileExplicit(this._tilemapModel, placement.coordinates, TileEditor.ClearTileOfType.TrafficLight, Tile.TileChangePermissions.Full);
								if (trafficLightEditResult.IsSuccessful && trafficLightEditResult.edit != null)
								{
									BuildingSpawningProcess.Log.Info(string.Format("Clearing traffic light at {0} underneath house.", placement.coordinates), Array.Empty<object>());
									this._upgradeDatabaseModel.ApplyEdit(trafficLightEditResult.edit, this._tilemapModel);
									trafficLightEditResult.edit.ApplyToTilemap(this._tilemapModel);
									trafficLightEditResult.edit.ApplyToSimulation(simulation);
								}
							}
							TileEditResult editResult = this._tileEditor.ClearTileExplicit(this._tilemapModel, placement.coordinates, TileEditor.ClearTileOfType.Roads, Tile.TileChangePermissions.Full);
							if (editResult.IsSuccessful && editResult.edit != null)
							{
								editResult.edit.ApplyToTilemap(this._tilemapModel);
								editResult.edit.ApplyToSimulation(simulation);
								this._upgradeDatabaseModel.ApplyEdit(editResult.edit, this._tilemapModel);
								this._laneUpdateProcess.Step(simulation, deltaTime);
								this._mothballedLanesProcess.Step(simulation, deltaTime);
							}
							HouseModel house = this._scope.Get<HouseModel>();
							house.Initialize(building3.groupIndex, this._tilemapModel.GetOrCreateTileModel(placement.coordinates), building3.tutorialIdentifier);
							simulation.AddModel(house);
							this._cityPlanModel.RecordNewHouse(house);
							this._demandModel.ApplyIncrementalSupplyFromHouse(house);
							Tile tile2 = house.tileModel.Tile;
							TileDirection drivewayDirection = placement.layout.driveways[0].direction;
							tile2.SetNodeState(new RoadTileNode(drivewayDirection, RoadType.Driveway, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
							this._tilemapModel.GetOrCreateTile(TileUtilities.GetAdjacentCoordinates(placement.coordinates, drivewayDirection)).SetNodeState(new RoadTileNode(TileUtilities.GetOppositeDirection(drivewayDirection), RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
						}
						else
						{
							BuildingSpawningProcess.Log.Info("Failed to place house of group {0} at time {1}s.", new object[]
							{
								building3.groupIndex,
								this._clock.ExpansionTime
							});
							building3.time += this._constants.FailedHouseSpawnCooldown;
							building3.spawnAttempts++;
							releaseScheduledBuilding = false;
							this._cityPlanModel.ScheduleBuilding(building3);
						}
					}
					else if (building3.grouping == GroupingStyle.Circle)
					{
						if (!this._behaviour.DoesBuildingStartUpgraded(building3.groupIndex) && !this.LevelUpDestinationBasedOnAge(simulation, building3.groupIndex, building3.demandMultiplier))
						{
							if (building3.spawnAttempts > this._constants.MaxFailedBuildingSpawnsBeforeIgnoringWeights)
							{
								if (building3.groupIndex == -1)
								{
									building3.groupIndex = this.ChooseRandomActiveGroupIndex(simulation);
								}
								building3.grouping = GroupingStyle.Normal;
								building3.spawnAttempts = 0;
								int failedCircles;
								if (!this._demandModel.failedDestinationUpgrades.TryGetValue(building3.groupIndex, out failedCircles))
								{
									this._demandModel.failedDestinationUpgrades.Add(building3.groupIndex, 1);
								}
								else
								{
									this._demandModel.failedDestinationUpgrades[building3.groupIndex] = failedCircles + 1;
								}
							}
							building3.time += this._constants.FailedDestinationRetryDelay;
							building3.spawnAttempts++;
							releaseScheduledBuilding = false;
							this._cityPlanModel.ScheduleBuilding(building3);
						}
					}
					else
					{
						if (building3.carparkPreference == CarparkPreference.NoPreference)
						{
							if (this._cityModel.pseudorandomGenerator.Fix64() <= this._cityPlanModel.DoubleDestinationProbability)
							{
								building3.carparkPreference = CarparkPreference.Double;
								this._cityPlanModel.ResetDoubleDestinationProbability();
							}
							else
							{
								building3.carparkPreference = CarparkPreference.Solo;
								this._cityPlanModel.IncreaseDoubleDestinationProbability();
							}
						}
						if (building3.carparkPreference != CarparkPreference.Station && building3.carparkPreference != CarparkPreference.JoinStation && building3.carparkPreference != CarparkPreference.ForceNewStation && building3.carparkPreference != CarparkPreference.ForceNewDouble && this._behaviour.ForceDoubleDestinations())
						{
							building3.carparkPreference = CarparkPreference.ForceDouble;
						}
						BuildingSpawningProcess.Log.Info("Attempting to place destination with carpark preference {0}.", new object[]
						{
							building3.carparkPreference
						});
						CarparkPreference carparkPreference2 = building3.carparkPreference;
						bool flag = carparkPreference2 == CarparkPreference.Double || carparkPreference2 == CarparkPreference.ForceDouble || carparkPreference2 == CarparkPreference.JoinDouble || carparkPreference2 == CarparkPreference.Station || carparkPreference2 == CarparkPreference.JoinStation || carparkPreference2 == CarparkPreference.BoatTerminal || carparkPreference2 == CarparkPreference.JoinBoatTerminal;
						carparkPreference2 = building3.carparkPreference;
						bool isStation = carparkPreference2 == CarparkPreference.Station || carparkPreference2 == CarparkPreference.JoinStation || carparkPreference2 == CarparkPreference.ForceNewStation;
						carparkPreference2 = building3.carparkPreference;
						bool isBoatTerminal = carparkPreference2 == CarparkPreference.BoatTerminal || carparkPreference2 == CarparkPreference.JoinBoatTerminal;
						CarparkModel carparkModelWithSpace;
						if (flag && this.HasDoubleCarparkWithVacantBuildingPosition(simulation, building3, isStation, isBoatTerminal, out carparkModelWithSpace) && (carparkModelWithSpace.destinations[0].GroupIndex != building3.groupIndex || building3.useFixedParameters))
						{
							BuildingSpawningProcess.Log.Info("Placed destination of group {0} into double carpark at {1} at time {2}s.", new object[]
							{
								building3.groupIndex,
								carparkModelWithSpace.TopLeftCarparkTileCoordinate,
								this._clock.ExpansionTime
							});
							DestinationModel.DestinationType destinationType = isStation ? DestinationModel.DestinationType.TrainStation : (isBoatTerminal ? DestinationModel.DestinationType.BoatTerminal : DestinationModel.DestinationType.Destination);
							this.AddBuildingToDoubleCarpark(simulation, building3, carparkModelWithSpace, destinationType);
						}
						else
						{
							BuildingPlacer.WeightSource weightSource = BuildingPlacer.WeightSource.Default;
							CarparkPreference carparkPreference = building3.carparkPreference;
							if (carparkPreference == CarparkPreference.Station || carparkPreference == CarparkPreference.JoinStation || carparkPreference == CarparkPreference.ForceNewStation)
							{
								weightSource = BuildingPlacer.WeightSource.Station;
							}
							else if (carparkPreference == CarparkPreference.BoatTerminal || carparkPreference == CarparkPreference.JoinBoatTerminal)
							{
								weightSource = BuildingPlacer.WeightSource.BoatTerminal;
							}
							this._placer.StartPlacing(TileContentType.Carpark, building3.groupIndex, building3.grouping, weightEvaluationLevel, weightSource);
							if (!building3.useFixedParameters)
							{
								carparkPreference2 = building3.carparkPreference;
								if (carparkPreference2 == CarparkPreference.Station || carparkPreference2 == CarparkPreference.JoinStation || carparkPreference2 == CarparkPreference.ForceNewStation)
								{
									this._placer.GeneratePlacements(BuildingSpawningProcess.RailwayStationLayouts);
								}
								else
								{
									carparkPreference2 = building3.carparkPreference;
									if (carparkPreference2 == CarparkPreference.BoatTerminal || carparkPreference2 == CarparkPreference.JoinBoatTerminal)
									{
										this._placer.GeneratePlacements(BuildingSpawningProcess.BoatTerminalLayouts);
									}
									else if (building3.PrefersDoubleCarpark)
									{
										this._placer.GeneratePlacements(BuildingSpawningProcess.DoubleCarparkLayouts);
									}
									else
									{
										this._placer.GeneratePlacements(BuildingSpawningProcess.SingleCarparkLayouts);
									}
								}
							}
							else
							{
								BuildingPlacer.Layout fixedLayout2 = this.GenerateFixedLayoutFromPlan(building3);
								this._placer.GenerateFixedPlacement(fixedLayout2, building3.positionOverride);
							}
							BuildingPlacer.Placement placement2 = this._placer.ChoosePlacement();
							bool successfullySpawnedDestination = false;
							if (placement2 != null)
							{
								BuildingSpawningProcess.Log.Info("Placed destination of group {0} at {1}, with footprint {2}, at time {3}s.", new object[]
								{
									building3.groupIndex,
									placement2.coordinates,
									placement2.layout.footprint,
									this._clock.ExpansionTime
								});
								this.SpawnDestination(placement2, building3, simulation, deltaTime);
								successfullySpawnedDestination = true;
							}
							else
							{
								BuildingSpawningProcess.Log.Info("Failed to place destination of group {0} at time {1}s.", new object[]
								{
									building3.groupIndex,
									this._clock.ExpansionTime
								});
							}
							if (!successfullySpawnedDestination && building3.PrefersDoubleCarpark && building3.spawnAttempts > this._constants.MaxFailedDoubleCarparkSpawnsBeforeConvertingToSingle)
							{
								bool increaseDoubleDestinationProbability = true;
								if (building3.carparkPreference == CarparkPreference.ForceDouble)
								{
									for (int otherScheduledBuildingIndex = 1; otherScheduledBuildingIndex < this._cityPlanModel.scheduledBuildings.Count; otherScheduledBuildingIndex++)
									{
										CityPlanModel.ScheduledBuilding otherBuilding = this._cityPlanModel.scheduledBuildings[otherScheduledBuildingIndex];
										if (otherBuilding != building3 && otherBuilding.groupIndex == building3.groupIndex && otherBuilding.type == CityTileType.Demand)
										{
											otherBuilding.carparkPreference = CarparkPreference.ForceDouble;
											increaseDoubleDestinationProbability = false;
											break;
										}
									}
								}
								if (increaseDoubleDestinationProbability)
								{
									this._cityPlanModel.IncreaseDoubleDestinationProbability();
								}
								building3.carparkPreference = CarparkPreference.Solo;
								building3.spawnAttempts = 1;
							}
							if (!successfullySpawnedDestination)
							{
								if (this._city.Rules.CanUpgradeDestinationsAfterFailedSpawns && building3.spawnAttempts >= this._constants.MaxFailedDestinationSpawnsBeforeConvertingToUpgrade)
								{
									building3.time += this._constants.FailedDestinationRetryDelay;
									building3.grouping = GroupingStyle.Circle;
									building3.spawnAttempts = 0;
									releaseScheduledBuilding = false;
									this._cityPlanModel.ScheduleBuilding(building3);
								}
								else
								{
									building3.time += this._constants.FailedDestinationRetryDelay;
									building3.spawnAttempts++;
									releaseScheduledBuilding = false;
									this._cityPlanModel.ScheduleBuilding(building3);
								}
							}
						}
					}
				}
				catch (Exception exception)
				{
					Diagnostics.FailAssert("{0} stacktrace: {1}", new object[]
					{
						exception,
						exception.StackTrace
					});
				}
				finally
				{
					if (releaseScheduledBuilding)
					{
						this._scope.Release(this._cityPlanModel.scheduledBuildings[0]);
					}
					this._cityPlanModel.scheduledBuildings.RemoveAt(0);
				}
			}
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x0006C5B0 File Offset: 0x0006A7B0
		private void SpawnDestination(BuildingPlacer.Placement placement, CityPlanModel.ScheduledBuilding building, ISimulation simulation, Fix64 deltaTime)
		{
			this.EnsureMinimumTimeForDestinationSpawnsAfterBuilding(this._clock.ExpansionTime + this._constants.MinimumTimeBetweenDestinationSpawns, building);
			IScope scope = simulation.Scope;
			Fix64 time = this._clock.ExpansionTime;
			CarparkEntrance carparkEntrances = (CarparkEntrance)0;
			foreach (BuildingPlacer.Driveway driveway in placement.layout.driveways)
			{
				if (driveway.direction == TileDirection.North || driveway.direction == TileDirection.West)
				{
					carparkEntrances |= CarparkEntrance.TopLeft;
				}
				else
				{
					carparkEntrances |= CarparkEntrance.BottomRight;
				}
			}
			bool removedRoad = false;
			for (int x = 0; x < placement.layout.footprint.x; x++)
			{
				for (int y = 0; y < placement.layout.footprint.y; y++)
				{
					Vector2Int position = new Vector2Int(x, y) + placement.coordinates;
					Tile tile = this._tilemapModel.GetTile(position);
					if (tile != null)
					{
						if (tile.HasTrafficLight)
						{
							TileEditResult trafficLightEditResult = this._tileEditor.ClearTileExplicit(this._tilemapModel, position, TileEditor.ClearTileOfType.TrafficLight, Tile.TileChangePermissions.Full);
							if (trafficLightEditResult.IsSuccessful && trafficLightEditResult.edit != null)
							{
								removedRoad = true;
								BuildingSpawningProcess.Log.Info(string.Format("Clearing traffic light at {0} underneath carpark.", position), Array.Empty<object>());
								this._upgradeDatabaseModel.ApplyEdit(trafficLightEditResult.edit, this._tilemapModel);
								trafficLightEditResult.edit.ApplyToTilemap(this._tilemapModel);
								trafficLightEditResult.edit.ApplyToSimulation(simulation);
							}
						}
						TileEditResult editResult = this._tileEditor.ClearTileExplicit(this._tilemapModel, position, TileEditor.ClearTileOfType.Roads, Tile.TileChangePermissions.Full);
						if (editResult.IsSuccessful && editResult.edit != null)
						{
							removedRoad = true;
							BuildingSpawningProcess.Log.Info(string.Format("Clearing Tile at {0} underneath carpark.", position), Array.Empty<object>());
							editResult.edit.ApplyToTilemap(this._tilemapModel);
							editResult.edit.ApplyToSimulation(simulation);
							this._upgradeDatabaseModel.ApplyEdit(editResult.edit, this._tilemapModel);
						}
					}
				}
			}
			if (removedRoad)
			{
				this._laneUpdateProcess.Step(simulation, deltaTime);
				this._mothballedLanesProcess.Step(simulation, deltaTime);
			}
			CarparkModel carpark = scope.Get<CarparkModel>();
			TileDirection drivewayDirection = placement.layout.driveways[0].direction;
			if (drivewayDirection != TileDirection.East)
			{
			}
			carpark.Initialize(carparkEntrances, building.carparkPreference, placement);
			simulation.AddModel(carpark);
			this._cityModel.OnCarparkAdded(carpark);
			Vector2Int destinationOffset = carpark.destinationOffsets[0];
			DestinationModel destination = scope.Get<DestinationModel>();
			CarparkPreference carparkPreference = building.carparkPreference;
			DestinationModel.DestinationType destinationType;
			if (carparkPreference == CarparkPreference.Station || carparkPreference == CarparkPreference.JoinStation || carparkPreference == CarparkPreference.ForceNewStation)
			{
				destinationType = DestinationModel.DestinationType.TrainStation;
			}
			else
			{
				carparkPreference = building.carparkPreference;
				if (carparkPreference == CarparkPreference.BoatTerminal || carparkPreference == CarparkPreference.JoinBoatTerminal)
				{
					destinationType = DestinationModel.DestinationType.BoatTerminal;
				}
				else
				{
					destinationType = DestinationModel.DestinationType.Destination;
				}
			}
			destination.Initialize(building.groupIndex, building.demandMultiplier, BuildingSpawningProcess.DestinationFootprint, placement.coordinates + destinationOffset, carpark, building.tutorialIdentifier, destinationType);
			if (building.initialUpgradeLevel != 0 || (this._behaviour.DoesBuildingStartUpgraded(building.groupIndex) && building.carparkPreference != CarparkPreference.Station && building.carparkPreference != CarparkPreference.JoinStation && building.carparkPreference != CarparkPreference.ForceNewStation))
			{
				destination.demandLevelUpTime = time;
			}
			simulation.AddModel(destination);
			this._cityPlanModel.RecordNewDestination(destination);
			this._demandModel.ApplyAbsoluteSupplyToDestination(destination);
			this._demandModel.CalculateSupplyScale(destination.GroupIndex);
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x0006C944 File Offset: 0x0006AB44
		private void EnsureMinimumTimeForDestinationSpawnsAfterBuilding(Fix64 minimumTime, CityPlanModel.ScheduledBuilding lastUnchangedBuilding)
		{
			bool shouldChangeBuildingTime = false;
			foreach (CityPlanModel.ScheduledBuilding building in this._cityPlanModel.scheduledBuildings)
			{
				if (shouldChangeBuildingTime)
				{
					if (!building.useFixedParameters && building.type == CityTileType.Demand && building.time < minimumTime)
					{
						building.time = minimumTime;
					}
				}
				else
				{
					shouldChangeBuildingTime = (building == lastUnchangedBuilding);
				}
			}
			this._cityPlanModel.scheduledBuildings.Sort((CityPlanModel.ScheduledBuilding first, CityPlanModel.ScheduledBuilding second) => first.time.CompareTo(second.time));
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x0006C9F8 File Offset: 0x0006ABF8
		private bool HasDoubleCarparkWithVacantBuildingPosition(ISimulation simulation, CityPlanModel.ScheduledBuilding requestedBuilding, bool requestStation, bool requestTerminal, out CarparkModel carparkModel)
		{
			carparkModel = null;
			foreach (CarparkModel otherCarpark in simulation.GetModels<CarparkModel>())
			{
				if (otherCarpark.SupportsTwoDestinations && otherCarpark.destinations.Count == 1)
				{
					if (requestedBuilding.useFixedParameters)
					{
						CarparkPreference carparkPreference = requestedBuilding.carparkPreference;
						if (carparkPreference == CarparkPreference.Double || carparkPreference == CarparkPreference.Station || carparkPreference == CarparkPreference.BoatTerminal)
						{
							if (carparkModel == null || Vector2Int.Distance(otherCarpark.TopLeftWorldCoordinate, requestedBuilding.positionOverride) < Vector2Int.Distance(carparkModel.TopLeftWorldCoordinate, requestedBuilding.positionOverride))
							{
								carparkModel = otherCarpark;
								continue;
							}
							continue;
						}
					}
					if (otherCarpark.destinations[0].IsTrainStation == requestStation && otherCarpark.destinations[0].IsBoatTerminal == requestTerminal)
					{
						carparkModel = otherCarpark;
						return true;
					}
				}
			}
			return carparkModel != null;
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x0006CACC File Offset: 0x0006ACCC
		public void AddBuildingToDoubleCarpark(ISimulation simulation, CityPlanModel.ScheduledBuilding building, CarparkModel carpark, DestinationModel.DestinationType destinationType)
		{
			IScope scope = simulation.Scope;
			Vector2Int destinationOffset = carpark.destinationOffsets[1];
			DestinationModel destination = scope.Get<DestinationModel>();
			destination.Initialize(building.groupIndex, building.demandMultiplier, BuildingSpawningProcess.DestinationFootprint, carpark.origin + destinationOffset, carpark, building.tutorialIdentifier, destinationType);
			if (this._behaviour.DoesBuildingStartUpgraded(building.groupIndex) && building.carparkPreference != CarparkPreference.Station && building.carparkPreference != CarparkPreference.JoinStation && building.carparkPreference != CarparkPreference.ForceNewStation)
			{
				destination.demandLevelUpTime = this._clock.ExpansionTime;
			}
			else if (this._behaviour.AllowSecondDestinationStartUpgraded && building.initialUpgradeLevel > 0)
			{
				destination.demandLevelUpTime = this._clock.ExpansionTime;
			}
			simulation.AddModel(destination);
			this._cityPlanModel.RecordNewDestination(destination);
			this._demandModel.ApplyAbsoluteSupplyToDestination(destination);
			this._demandModel.CalculateSupplyScale(destination.GroupIndex);
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x0006CBB8 File Offset: 0x0006ADB8
		private bool GetFirstSpawnTimeOfGroup(int groupIndex, ISimulation simulation, out Fix64 earliestTime)
		{
			earliestTime = Fix64.MaxValue;
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if (destination.GroupIndex == groupIndex && destination.ActivationTime < earliestTime)
				{
					earliestTime = destination.ActivationTime;
				}
			}
			return earliestTime != Fix64.MaxValue;
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x0006CC2C File Offset: 0x0006AE2C
		private GroupingStyle AssessGroupingStyleForHouse(int groupIndex, ISimulation simulation)
		{
			ScheduleGroup scheduleGroup = this._city.Definition.schedulePlanner.GetScheduleGroup(groupIndex);
			Fix64 curveTimeIndex = ClockModel.SecondsToFractionalDays(this._clock.ExpansionTime);
			Fix64 earliestTime;
			if (this.GetFirstSpawnTimeOfGroup(groupIndex, simulation, out earliestTime))
			{
				curveTimeIndex -= ClockModel.SecondsToFractionalDays(earliestTime);
			}
			float relativeCurveTime = (float)curveTimeIndex;
			GroupingStyle possibleGrouping = GroupingStyle.Near;
			if (this._cityPlanModel.suburbCount.ContainsKey(groupIndex))
			{
				int highestSpawnAttempts = this.GetHighestNumberOfSpawnAttempts(groupIndex);
				highestSpawnAttempts = Mathf.Max(highestSpawnAttempts - this._constants.MinimumSpawnAttemptsForSuburbMultiplier, 0);
				Fix64 suburbCountMultiplier = Fix64.Lerp(Fix64.One, this._constants.MaximumDelayedBuildingSuburbCountMultiplier, (Fix64)((long)highestSpawnAttempts) / (Fix64)((long)(this._constants.MaximumSpawnAttemptsForSuburbMultiplier - this._constants.MinimumSpawnAttemptsForSuburbMultiplier)));
				Fix64 numSuburbs = (Fix64)((long)this._cityPlanModel.suburbCount[groupIndex]);
				Fix64 minNumSuburbs = (Fix64)scheduleGroup.minimumNumSuburbs.Evaluate(relativeCurveTime) * suburbCountMultiplier;
				Fix64 maxNumSuburbs = (Fix64)scheduleGroup.maximumNumSuburbs.Evaluate(relativeCurveTime) * suburbCountMultiplier;
				if (numSuburbs < minNumSuburbs)
				{
					Fix64 newSuburbChance = Fix64.Pow((minNumSuburbs - numSuburbs) * this._constants.MinimumSuburbCountScale, this._constants.MinimumSuburbCountExponent);
					if (this._cityModel.pseudorandomGenerator.Fix64() < newSuburbChance)
					{
						possibleGrouping = GroupingStyle.Far;
					}
				}
				else if (numSuburbs < maxNumSuburbs)
				{
					Fix64 newSuburbChance2 = Fix64.Pow((maxNumSuburbs - numSuburbs) * this._constants.MaximumSuburbCountScale, this._constants.MaximumSuburbCountExponent);
					if (this._cityModel.pseudorandomGenerator.Fix64() < newSuburbChance2)
					{
						possibleGrouping = GroupingStyle.Far;
					}
				}
				if (possibleGrouping == GroupingStyle.Far)
				{
					Dictionary<int, int> suburbCount = this._cityPlanModel.suburbCount;
					int num = suburbCount[groupIndex];
					suburbCount[groupIndex] = num + 1;
				}
			}
			else
			{
				possibleGrouping = GroupingStyle.Normal;
				this._cityPlanModel.suburbCount.Add(groupIndex, 1);
			}
			return possibleGrouping;
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x0006CE30 File Offset: 0x0006B030
		private int GetHighestNumberOfSpawnAttempts(int groupIndex)
		{
			int highestSpawnAttempts = 0;
			foreach (CityPlanModel.ScheduledBuilding building in this._cityPlanModel.scheduledBuildings)
			{
				if (building.groupIndex == groupIndex && building.type == CityTileType.Demand)
				{
					highestSpawnAttempts = Math.Max(highestSpawnAttempts, building.spawnAttempts);
				}
			}
			return highestSpawnAttempts;
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x0006CEA4 File Offset: 0x0006B0A4
		private bool LevelUpDestinationBasedOnAge(ISimulation simulation, int groupIndex, Fix64 demandMultiplierOverride)
		{
			List<Tuple<DestinationModel, Fix64>> destinationsAndAges = new List<Tuple<DestinationModel, Fix64>>();
			Fix64 totalWeight = Fix64.Zero;
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if ((groupIndex == -1 || destination.GroupIndex == groupIndex) && !destination.IsUpgraded && !destination.IsScheduledToBeUpgraded && !destination.IsTrainStation)
				{
					bool hasTileInCircleBounds = false;
					for (int x = 0; x < destination.Carpark.footprint.x; x++)
					{
						for (int y = 0; y < destination.Carpark.footprint.y; y++)
						{
							if (this._behaviour.TileSupportsCircleDestinations(groupIndex, destination.Carpark.TopLeftWorldCoordinate + new Vector2Int(x, y)))
							{
								hasTileInCircleBounds = true;
								break;
							}
						}
						if (hasTileInCircleBounds)
						{
							break;
						}
					}
					if (hasTileInCircleBounds)
					{
						Fix64 age = this._clock.ExpansionTime - destination.ActivationTime;
						totalWeight += age;
						destinationsAndAges.Add(Tuple.Create<DestinationModel, Fix64>(destination, age));
					}
				}
			}
			if (destinationsAndAges.Count > 0)
			{
				Fix64 spin = this._cityModel.pseudorandomGenerator.Fix64(totalWeight);
				int indexToLevelUp = destinationsAndAges.Count - 1;
				for (int destinationIndex = 0; destinationIndex < destinationsAndAges.Count; destinationIndex++)
				{
					spin -= destinationsAndAges[destinationIndex].Item2;
					if (spin < Fix64.Zero)
					{
						indexToLevelUp = destinationIndex;
						break;
					}
				}
				Fix64 levelUpTime = Fix64.Max(destinationsAndAges[indexToLevelUp].Item1.ActivationTime + CityPlanModel.MinimumTimeBeforeBuildingDemandLevelsUp, this._clock.ExpansionTime);
				destinationsAndAges[indexToLevelUp].Item1.demandLevelUpTime = levelUpTime;
				destinationsAndAges[indexToLevelUp].Item1.demandMultiplier = demandMultiplierOverride;
				return true;
			}
			BuildingSpawningProcess.Log.Warn("Unable to level up existing destination at group {0}, at time {1}.", new object[]
			{
				groupIndex,
				this._clock.ExpansionTime
			});
			return false;
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x0006D0B8 File Offset: 0x0006B2B8
		private BuildingPlacer.Layout GenerateFixedLayoutFromPlan(CityPlanModel.ScheduledBuilding building)
		{
			BuildingPlacer.Layout fixedLayout;
			if (building.drivewayDirectionOverride == TileDirection.East)
			{
				fixedLayout = new BuildingPlacer.Layout
				{
					footprint = (building.PrefersDoubleCarpark ? BuildingSpawningProcess.HorizontalDoubleCarparkFootprint : BuildingSpawningProcess.HorizontalCarparkFootprint)
				};
				int drivewayY = (building.carparkSideOverride == TileDirection.North) ? (fixedLayout.footprint.y - 1) : 0;
				if ((building.entranceOverride & CarparkEntrance.TopLeft) != (CarparkEntrance)0)
				{
					fixedLayout.driveways.Add(new BuildingPlacer.Driveway
					{
						coordinatesOffset = new Vector2Int(0, drivewayY),
						direction = TileDirection.West
					});
				}
				if ((building.entranceOverride & CarparkEntrance.BottomRight) != (CarparkEntrance)0)
				{
					fixedLayout.driveways.Add(new BuildingPlacer.Driveway
					{
						coordinatesOffset = new Vector2Int(fixedLayout.footprint.x - 1, drivewayY),
						direction = TileDirection.East
					});
				}
				fixedLayout.carparkSide = ((building.carparkSideOverride == TileDirection.None) ? TileDirection.South : building.carparkSideOverride);
			}
			else
			{
				fixedLayout = new BuildingPlacer.Layout
				{
					footprint = (building.PrefersDoubleCarpark ? BuildingSpawningProcess.VerticalDoubleCarparkFootprint : BuildingSpawningProcess.VerticalCarparkFootprint)
				};
				int drivewayX = 0;
				if (building.carparkSideOverride == TileDirection.East)
				{
					drivewayX = fixedLayout.footprint.x - 1;
				}
				if ((building.entranceOverride & CarparkEntrance.TopLeft) != (CarparkEntrance)0)
				{
					fixedLayout.driveways.Add(new BuildingPlacer.Driveway
					{
						coordinatesOffset = new Vector2Int(drivewayX, fixedLayout.footprint.y - 1),
						direction = TileDirection.North
					});
				}
				if ((building.entranceOverride & CarparkEntrance.BottomRight) != (CarparkEntrance)0)
				{
					fixedLayout.driveways.Add(new BuildingPlacer.Driveway
					{
						coordinatesOffset = new Vector2Int(drivewayX, 0),
						direction = TileDirection.South
					});
				}
				fixedLayout.carparkSide = ((building.carparkSideOverride == TileDirection.None) ? TileDirection.West : building.carparkSideOverride);
			}
			CarparkPreference carparkPreference = building.carparkPreference;
			if (carparkPreference == CarparkPreference.Station || carparkPreference == CarparkPreference.JoinStation || carparkPreference == CarparkPreference.ForceNewStation)
			{
				fixedLayout.platforms = BuildingSpawningProcess.GeneratePlatformPositions(fixedLayout.footprint, fixedLayout.carparkSide);
			}
			return fixedLayout;
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x0006D274 File Offset: 0x0006B474
		private int ChooseRandomActiveGroupIndex(ISimulation simulation)
		{
			List<int> activeGroupIndexes = new List<int>();
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if (destination.isActive && !activeGroupIndexes.Contains(destination.GroupIndex))
				{
					activeGroupIndexes.Add(destination.GroupIndex);
				}
			}
			if (activeGroupIndexes.Count == 0)
			{
				return 0;
			}
			return activeGroupIndexes[this._cityModel.pseudorandomGenerator.Int(activeGroupIndexes.Count)];
		}

		// Token: 0x040018A8 RID: 6312
		[Dependency]
		private IScope _scope;

		// Token: 0x040018A9 RID: 6313
		[Dependency]
		private CityPlanModel _cityPlanModel;

		// Token: 0x040018AA RID: 6314
		[Dependency]
		private TilemapModel _tilemapModel;

		// Token: 0x040018AB RID: 6315
		[Dependency]
		private TileEditor _tileEditor;

		// Token: 0x040018AC RID: 6316
		[Dependency]
		private ClockModel _clock;

		// Token: 0x040018AD RID: 6317
		[Dependency]
		private City _city;

		// Token: 0x040018AE RID: 6318
		[Dependency]
		private CityModel _cityModel;

		// Token: 0x040018AF RID: 6319
		[Dependency]
		private BuildingPlacer _placer;

		// Token: 0x040018B0 RID: 6320
		[Dependency]
		private DemandModel _demandModel;

		// Token: 0x040018B1 RID: 6321
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x040018B2 RID: 6322
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x040018B3 RID: 6323
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabaseModel;

		// Token: 0x040018B4 RID: 6324
		[Dependency]
		private LaneUpdateProcess _laneUpdateProcess;

		// Token: 0x040018B5 RID: 6325
		[Dependency]
		private ReleaseMothballedLanesProcess _mothballedLanesProcess;

		// Token: 0x040018B6 RID: 6326
		public const int UpgradeAnyGroupIndex = -1;

		// Token: 0x040018B7 RID: 6327
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("BuildingSpawningProcess");

		// Token: 0x040018B8 RID: 6328
		public static readonly Vector2Int HouseFootprint = new Vector2Int(1, 1);

		// Token: 0x040018B9 RID: 6329
		private static readonly List<BuildingPlacer.Layout> HouseLayouts = new List<BuildingPlacer.Layout>
		{
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HouseFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.North
					}
				}
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HouseFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.East
					}
				}
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HouseFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.South
					}
				}
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HouseFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.West
					}
				}
			}
		};

		// Token: 0x040018BA RID: 6330
		public static readonly Vector2Int DestinationFootprint = new Vector2Int(2, 2);

		// Token: 0x040018BB RID: 6331
		private static readonly Vector2Int HorizontalCarparkFootprint = new Vector2Int(2, 3);

		// Token: 0x040018BC RID: 6332
		private static readonly Vector2Int VerticalCarparkFootprint = new Vector2Int(3, 2);

		// Token: 0x040018BD RID: 6333
		public static readonly Vector2Int VerticalDoubleCarparkFootprint = new Vector2Int(3, 4);

		// Token: 0x040018BE RID: 6334
		public static readonly Vector2Int HorizontalDoubleCarparkFootprint = new Vector2Int(4, 3);

		// Token: 0x040018BF RID: 6335
		public static readonly Vector2Int HorizontalDoubleCarparkBoatFootprint = new Vector2Int(4, 4);

		// Token: 0x040018C0 RID: 6336
		public static readonly List<BuildingPlacer.Layout> SingleCarparkLayouts = new List<BuildingPlacer.Layout>
		{
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HorizontalCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.West
					}
				},
				carparkSide = TileDirection.South
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HorizontalCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						coordinatesOffset = new Vector2Int(BuildingSpawningProcess.HorizontalCarparkFootprint.x - 1, 0),
						direction = TileDirection.East
					}
				},
				carparkSide = TileDirection.South
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.VerticalCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						coordinatesOffset = new Vector2Int(0, BuildingSpawningProcess.VerticalCarparkFootprint.y - 1),
						direction = TileDirection.North
					}
				},
				carparkSide = TileDirection.West
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.VerticalCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.South
					}
				},
				carparkSide = TileDirection.West
			}
		};

		// Token: 0x040018C1 RID: 6337
		public static readonly List<BuildingPlacer.Layout> DoubleCarparkLayouts = new List<BuildingPlacer.Layout>
		{
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.VerticalDoubleCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.South
					},
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.North,
						coordinatesOffset = new Vector2Int(0, BuildingSpawningProcess.VerticalDoubleCarparkFootprint.y - 1)
					}
				},
				carparkSide = TileDirection.West
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HorizontalDoubleCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.West
					},
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.East,
						coordinatesOffset = new Vector2Int(BuildingSpawningProcess.HorizontalDoubleCarparkFootprint.x - 1, 0)
					}
				},
				carparkSide = TileDirection.South
			}
		};

		// Token: 0x040018C2 RID: 6338
		public static readonly List<BuildingPlacer.Layout> RailwayStationLayouts = new List<BuildingPlacer.Layout>
		{
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.VerticalDoubleCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.South
					},
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.North,
						coordinatesOffset = new Vector2Int(0, BuildingSpawningProcess.VerticalDoubleCarparkFootprint.y - 1)
					}
				},
				platforms = BuildingSpawningProcess.GeneratePlatformPositions(BuildingSpawningProcess.VerticalDoubleCarparkFootprint, TileDirection.West),
				carparkSide = TileDirection.West
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HorizontalDoubleCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.West
					},
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.East,
						coordinatesOffset = new Vector2Int(BuildingSpawningProcess.HorizontalDoubleCarparkFootprint.x - 1, 0)
					}
				},
				platforms = BuildingSpawningProcess.GeneratePlatformPositions(BuildingSpawningProcess.HorizontalDoubleCarparkFootprint, TileDirection.South),
				carparkSide = TileDirection.South
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.VerticalDoubleCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.South,
						coordinatesOffset = new Vector2Int(BuildingSpawningProcess.VerticalDoubleCarparkFootprint.x - 1, 0)
					},
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.North,
						coordinatesOffset = new Vector2Int(BuildingSpawningProcess.VerticalDoubleCarparkFootprint.x - 1, BuildingSpawningProcess.VerticalDoubleCarparkFootprint.y - 1)
					}
				},
				platforms = BuildingSpawningProcess.GeneratePlatformPositions(BuildingSpawningProcess.VerticalDoubleCarparkFootprint, TileDirection.East),
				carparkSide = TileDirection.East
			},
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HorizontalDoubleCarparkFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.West,
						coordinatesOffset = new Vector2Int(0, BuildingSpawningProcess.HorizontalDoubleCarparkFootprint.y - 1)
					},
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.East,
						coordinatesOffset = new Vector2Int(BuildingSpawningProcess.HorizontalDoubleCarparkFootprint.x - 1, BuildingSpawningProcess.HorizontalDoubleCarparkFootprint.y - 1)
					}
				},
				platforms = BuildingSpawningProcess.GeneratePlatformPositions(BuildingSpawningProcess.HorizontalDoubleCarparkFootprint, TileDirection.North),
				carparkSide = TileDirection.North
			}
		};

		// Token: 0x040018C3 RID: 6339
		public static readonly List<BuildingPlacer.Layout> BoatTerminalLayouts = new List<BuildingPlacer.Layout>
		{
			new BuildingPlacer.Layout
			{
				footprint = BuildingSpawningProcess.HorizontalDoubleCarparkBoatFootprint,
				driveways = new List<BuildingPlacer.Driveway>
				{
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.West
					},
					new BuildingPlacer.Driveway
					{
						direction = TileDirection.East,
						coordinatesOffset = new Vector2Int(BuildingSpawningProcess.HorizontalDoubleCarparkBoatFootprint.x - 1, 0)
					}
				},
				platforms = BuildingSpawningProcess.GeneratePlatformPositions(BuildingSpawningProcess.HorizontalDoubleCarparkBoatFootprint, TileDirection.South),
				boatTerminalTiles = BuildingSpawningProcess.GenerateBoatTerminalTiles(BuildingSpawningProcess.HorizontalDoubleCarparkBoatFootprint, TileDirection.South),
				carparkSide = TileDirection.South
			}
		};

		// Token: 0x040018C4 RID: 6340
		private static readonly ProfilerMarker Profiler_Step = new ProfilerMarker(ProfilerCategory.Scripts, "BuildingSpawningProcess.Step()");

		// Token: 0x040018C5 RID: 6341
		private static readonly ProfilerMarker Profiler_ScheduleHouses = new ProfilerMarker(ProfilerCategory.Scripts, "BuildingSpawningProcess.ScheduleHouses()");

		// Token: 0x040018C6 RID: 6342
		private static readonly ProfilerMarker Profiler_SpawnScheduledBuildings = new ProfilerMarker(ProfilerCategory.Scripts, "BuildingSpawningProcess.SpawnScheduledBuildings()");

		// Token: 0x040018C7 RID: 6343
		private static readonly ProfilerMarker Profiler_SpawnHouse = new ProfilerMarker(ProfilerCategory.Scripts, "BuildingSpawningProcess.SpawnHouse()");

		// Token: 0x040018C8 RID: 6344
		private static readonly ProfilerMarker Profiler_SpawnDestination = new ProfilerMarker(ProfilerCategory.Scripts, "BuildingSpawningProcess.SpawnDestination()");
	}
}
