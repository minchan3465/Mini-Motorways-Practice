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
	// Token: 0x0200048D RID: 1165
	public class GenerateDemandProcess : IProcess, IReusable
	{
		// Token: 0x06001CE1 RID: 7393 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x0006E3F3 File Offset: 0x0006C5F3
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			this.BalanceDemandFromFailedSpawns(simulation);
			this.CalculateSpawnRamp();
			if (this._city.Rules.GetDemandGenerationStyle == GenerateDemandProcess.DemandGenerationStyle.Timer)
			{
				this.GenerateTimerDemand(simulation, timestep);
			}
			else
			{
				this.GeneratePermanentBalancedDemand(simulation);
			}
			this.GenerateStationDemand(simulation);
			this.GenerateBoatTerminalDemand(simulation);
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x0006E434 File Offset: 0x0006C634
		private void BalanceDemandFromFailedSpawns(ISimulation simulation)
		{
			this._demandModel.extraDemand.Clear();
			Dictionary<int, Fix64> pendingDemandPerGroup = new Dictionary<int, Fix64>();
			Fix64 demandMultiplier = this.DemandMultiplierForDelayedBuildings(this._cityPlanModel);
			foreach (CityPlanModel.ScheduledBuilding buildingPlan in this._cityPlanModel.scheduledBuildings)
			{
				if (buildingPlan.spawnAttempts > 0 && buildingPlan.type == CityTileType.Demand)
				{
					if (!pendingDemandPerGroup.ContainsKey(buildingPlan.groupIndex))
					{
						pendingDemandPerGroup[buildingPlan.groupIndex] = Fix64.Zero;
					}
					Fix64 amountToAdd = buildingPlan.demandMultiplier;
					if (buildingPlan.spawnAttempts > this._constants.MaxFailedBuildingSpawnsBeforeIgnoringWeights)
					{
						amountToAdd *= demandMultiplier;
					}
					Dictionary<int, Fix64> dictionary = pendingDemandPerGroup;
					int key = buildingPlan.groupIndex;
					dictionary[key] += amountToAdd;
				}
			}
			Fix64 totalDemand = Fix64.Zero;
			Dictionary<int, Fix64> activeDemandPerGroup = new Dictionary<int, Fix64>();
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if (destination.isActive)
				{
					if (!activeDemandPerGroup.ContainsKey(destination.GroupIndex))
					{
						activeDemandPerGroup[destination.GroupIndex] = Fix64.Zero;
					}
					Fix64 destinationDemand = this._behaviour.GetDemandMultiplierForBuilding(destination);
					Dictionary<int, Fix64> dictionary = activeDemandPerGroup;
					int key = destination.GroupIndex;
					dictionary[key] += destinationDemand;
					totalDemand += destinationDemand;
				}
			}
			foreach (KeyValuePair<int, Fix64> pendingGroupDemand in pendingDemandPerGroup)
			{
				int groupId = pendingGroupDemand.Key;
				Fix64 extraDemand = pendingGroupDemand.Value;
				if (activeDemandPerGroup.ContainsKey(groupId))
				{
					if (!this._demandModel.extraDemand.ContainsKey(groupId))
					{
						this._demandModel.extraDemand[groupId] = Fix64.Zero;
					}
					Fix64 extraDemandForThisGroup = Fix64.Min(activeDemandPerGroup[groupId], extraDemand);
					Dictionary<int, Fix64> dictionary = this._demandModel.extraDemand;
					int key = groupId;
					dictionary[key] += extraDemandForThisGroup / activeDemandPerGroup[groupId];
					extraDemand -= extraDemandForThisGroup;
				}
				if (extraDemand > Fix64.Zero && totalDemand > Fix64.Zero)
				{
					Fix64 extraDemandMultiplier = extraDemand / totalDemand;
					foreach (int activeGroupId in activeDemandPerGroup.Keys)
					{
						if (activeGroupId != groupId)
						{
							if (!this._demandModel.extraDemand.ContainsKey(activeGroupId))
							{
								this._demandModel.extraDemand[activeGroupId] = Fix64.Zero;
							}
							Dictionary<int, Fix64> dictionary = this._demandModel.extraDemand;
							int key = activeGroupId;
							dictionary[key] += extraDemandMultiplier;
						}
					}
				}
			}
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x0006E788 File Offset: 0x0006C988
		private void CalculateSpawnRamp()
		{
			this._demandModel.spawnScale = Fix64Consts.One;
			int day = this._clock.ExpansionDay;
			if (this._city.Definition.spawnRamp.startDay > 0)
			{
				int daysPastSpawnRamp = day - this._city.Definition.spawnRamp.startDay;
				if (daysPastSpawnRamp > 0)
				{
					this._demandModel.spawnScale += (Fix64)((long)daysPastSpawnRamp) * this._city.Definition.spawnRamp.dailyIncrement * this._city.Rules.SpawnRampMultiplier;
				}
			}
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x0006E834 File Offset: 0x0006CA34
		private void GenerateTimerDemand(ISimulation simulation, Fix64 timestep)
		{
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if (destination.isActive && !destination.IsTrainStation && !destination.IsBoatTerminal)
				{
					if (destination.demandTimer >= Fix64.Zero)
					{
						Fix64 demandTick = timestep * this._demandModel.spawnScale;
						if (this._demandModel.extraDemand.ContainsKey(destination.GroupIndex))
						{
							demandTick *= Fix64.One + this._demandModel.extraDemand[destination.GroupIndex];
						}
						destination.demandTimer -= demandTick;
						if (destination.demandTimer <= Fix64.Zero)
						{
							if (this.CanAddDemandToDestination(destination))
							{
								this.AddDemandToDestination(destination);
							}
							else if (this._city.Rules.AllowDemandRelocation)
							{
								DestinationModel destinationToReceiveDemand = this.GetDestinationToReallocatePinTo(destination, simulation);
								if (destinationToReceiveDemand != null)
								{
									this.AddDemandToDestination(destinationToReceiveDemand);
								}
							}
							destination.demandTimer = this.IntervalForDestination(destination);
						}
					}
					else
					{
						destination.demandTimer = this.IntervalForDestination(destination);
					}
				}
			}
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x0006E970 File Offset: 0x0006CB70
		private void GenerateStationDemand(ISimulation simulation)
		{
			foreach (TrainModel train in simulation.GetModels<TrainModel>())
			{
				if (train.HasPendingDemand && Diagnostics.Verify(train.targetStation != null))
				{
					List<DestinationModel> trainStations = new List<DestinationModel>();
					foreach (DestinationModel destination in train.targetStation.destinations)
					{
						if (Diagnostics.Verify(destination.IsTrainStation))
						{
							trainStations.Add(destination);
						}
					}
					Fix64 totalHouses = Fix64.Zero;
					foreach (HouseModel house in simulation.GetModels<HouseModel>())
					{
						foreach (DestinationModel trainStation in trainStations)
						{
							if (house.GroupIndex == trainStation.GroupIndex)
							{
								totalHouses += Fix64.One;
							}
						}
					}
					Fix64 totalHousesScaled = totalHouses * this._constants.demandPerHouse;
					Fix64 inverseDemandOscillationTotal = Fix64.Zero;
					Fix64[] perStationInverseDemandOscillation = new Fix64[trainStations.Count];
					for (int trainStationIndex = 0; trainStationIndex < trainStations.Count; trainStationIndex++)
					{
						int groupIndex = trainStations[trainStationIndex].GroupIndex;
						Fix64 timeInDaysWithOffset = this._clock.FractionalDays + this._demandModel.GetGroupDemandOscillationOffset(groupIndex);
						Fix64 oscillationValue = this._city.Definition.schedulePlanner.GetOscillationForDemand(groupIndex, timeInDaysWithOffset);
						Fix64 inverseDemandOscillation = Fix64.One / oscillationValue;
						perStationInverseDemandOscillation[trainStationIndex] = inverseDemandOscillation;
						inverseDemandOscillationTotal += inverseDemandOscillation;
					}
					for (int trainStationIndex2 = 0; trainStationIndex2 < trainStations.Count; trainStationIndex2++)
					{
						Fix64 demandToGive = perStationInverseDemandOscillation[trainStationIndex2] / inverseDemandOscillationTotal * totalHousesScaled;
						demandToGive = Fix64.Clamp(Fix64.Round(demandToGive), (Fix64)((long)this._constants.minDemandFromTrain), (Fix64)((long)this._constants.maxDemandFromTrain));
						this.AddDemandToStationOrTerminal(trainStations[trainStationIndex2], (int)((long)demandToGive));
					}
					train.HasPendingDemand = false;
				}
			}
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x0006EBCC File Offset: 0x0006CDCC
		private void GenerateBoatTerminalDemand(ISimulation simulation)
		{
			foreach (BoatModel boatModel in simulation.GetModels<BoatModel>())
			{
				if (boatModel.HasPendingDemand && Diagnostics.Verify(boatModel.GetTargetTerminal() != null))
				{
					List<DestinationModel> boatTerminals = new List<DestinationModel>();
					foreach (DestinationModel destination in boatModel.GetTargetTerminal().destinations)
					{
						if (Diagnostics.Verify(destination.IsBoatTerminal))
						{
							boatTerminals.Add(destination);
						}
					}
					Fix64 totalHouses = Fix64.Zero;
					foreach (HouseModel house in simulation.GetModels<HouseModel>())
					{
						foreach (DestinationModel boatTerminal in boatTerminals)
						{
							if (house.GroupIndex == boatTerminal.GroupIndex)
							{
								totalHouses += Fix64.One;
							}
						}
					}
					Fix64 totalHousesScaled = totalHouses * this._constants.demandPerHouse;
					Fix64 inverseDemandOscillationTotal = Fix64.Zero;
					Fix64[] perTerminalInverseDemandOscillation = new Fix64[boatTerminals.Count];
					for (int boatTerminalIndex = 0; boatTerminalIndex < boatTerminals.Count; boatTerminalIndex++)
					{
						int groupIndex = boatTerminals[boatTerminalIndex].GroupIndex;
						Fix64 timeInDaysWithOffset = this._clock.FractionalDays + this._demandModel.GetGroupDemandOscillationOffset(groupIndex);
						Fix64 oscillationValue = this._city.Definition.schedulePlanner.GetOscillationForDemand(groupIndex, timeInDaysWithOffset);
						Fix64 inverseDemandOscillation = Fix64.One / oscillationValue;
						perTerminalInverseDemandOscillation[boatTerminalIndex] = inverseDemandOscillation;
						inverseDemandOscillationTotal += inverseDemandOscillation;
					}
					for (int boatTerminalIndex2 = 0; boatTerminalIndex2 < boatTerminals.Count; boatTerminalIndex2++)
					{
						Fix64 demandToGive = perTerminalInverseDemandOscillation[boatTerminalIndex2] / inverseDemandOscillationTotal * totalHousesScaled;
						demandToGive = Fix64.Clamp(Fix64.Round(demandToGive), (Fix64)((long)this._constants.minDemandFromTrain), (Fix64)((long)this._constants.maxDemandFromTrain));
						demandToGive *= this._behaviour.GetDemandMultiplierForBuilding(boatTerminals[boatTerminalIndex2]);
						this.AddDemandToStationOrTerminal(boatTerminals[boatTerminalIndex2], (int)((long)demandToGive));
					}
					boatModel.HasPendingDemand = false;
				}
			}
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x0006EE4C File Offset: 0x0006D04C
		private void GeneratePermanentBalancedDemand(ISimulation simulation)
		{
			Array.Clear(this._allocatedColorGroups, 0, this._allocatedColorGroups.Length);
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if (destination.isActive && !destination.IsTrainStation && !this._allocatedColorGroups[destination.GroupIndex])
				{
					this._allocatedColorGroups[destination.GroupIndex] = true;
					List<DestinationModel> allDestinationsOfGroup = new List<DestinationModel>
					{
						destination
					};
					int totalGroupDemand = destination.TotalDemand;
					foreach (DestinationModel otherDestination in simulation.GetModels<DestinationModel>())
					{
						if (!otherDestination.IsTrainStation && !allDestinationsOfGroup.Contains(otherDestination) && destination.GroupIndex == otherDestination.GroupIndex)
						{
							allDestinationsOfGroup.Add(otherDestination);
							totalGroupDemand += otherDestination.TotalDemand;
						}
					}
					int totalCarsAvailableOrInUse = 0;
					foreach (VehicleModel car in simulation.GetModels<VehicleModel>())
					{
						if (car.house.GroupIndex == destination.GroupIndex && (car.IsAvailableAtHouse || car.IsDrivingToDestination))
						{
							totalCarsAvailableOrInUse++;
						}
					}
					int demandToAdd = totalCarsAvailableOrInUse - totalGroupDemand;
					int offset = 0;
					allDestinationsOfGroup.Sort((DestinationModel x, DestinationModel y) => x.TotalDemand - y.TotalDemand);
					int demandIndex = 0;
					while (demandIndex < demandToAdd && offset < 30)
					{
						int offsetDestinationIndex = (demandIndex + offset) % allDestinationsOfGroup.Count;
						if (this.CanAddDemandToDestination(allDestinationsOfGroup[offsetDestinationIndex]))
						{
							this.AddDemandToDestination(allDestinationsOfGroup[offsetDestinationIndex]);
						}
						else
						{
							demandIndex--;
							offset++;
						}
						demandIndex++;
					}
				}
			}
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x0006F010 File Offset: 0x0006D210
		private DestinationModel GetDestinationToReallocatePinTo(DestinationModel maxedDestination, ISimulation simulation)
		{
			DestinationModel nearestValidDestinationOfSameGroup = null;
			DestinationModel nearestValidDestinationOfOtherGroup = null;
			foreach (DestinationModel otherDestination in simulation.GetModels<DestinationModel>())
			{
				if (otherDestination.isActive && this.CanAddDemandToDestination(otherDestination))
				{
					if (otherDestination.GroupIndex == maxedDestination.GroupIndex)
					{
						if (nearestValidDestinationOfSameGroup == null || Vector2.SqrMagnitude(otherDestination.Carpark.TopLeftCarparkTileCoordinate - maxedDestination.Carpark.TopLeftCarparkTileCoordinate) < Vector2.SqrMagnitude(nearestValidDestinationOfSameGroup.Carpark.TopLeftCarparkTileCoordinate - maxedDestination.Carpark.TopLeftCarparkTileCoordinate))
						{
							nearestValidDestinationOfSameGroup = otherDestination;
						}
					}
					else if (nearestValidDestinationOfOtherGroup == null || Vector2.SqrMagnitude(otherDestination.Carpark.TopLeftCarparkTileCoordinate - maxedDestination.Carpark.TopLeftCarparkTileCoordinate) < Vector2.SqrMagnitude(nearestValidDestinationOfOtherGroup.Carpark.TopLeftCarparkTileCoordinate - maxedDestination.Carpark.TopLeftCarparkTileCoordinate))
					{
						nearestValidDestinationOfOtherGroup = otherDestination;
					}
				}
			}
			return nearestValidDestinationOfSameGroup ?? nearestValidDestinationOfOtherGroup;
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x0006F120 File Offset: 0x0006D320
		private bool CanAddDemandToDestination(DestinationModel destination)
		{
			return destination.TotalDemand < this._city.Rules.GetMaximumDemandForDestination(destination);
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x0006F13B File Offset: 0x0006D33B
		private void AddDemandToDestination(DestinationModel destination)
		{
			if (Diagnostics.Verify(destination.GroupIndex >= 0))
			{
				destination.unassignedDemand.Add(destination.GroupIndex);
			}
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x0006F164 File Offset: 0x0006D364
		private void AddDemandToStationOrTerminal(DestinationModel destination, int amountOfDemand)
		{
			if (Diagnostics.Verify(destination.GroupIndex >= 0))
			{
				int maximumAllowedDemand = this._city.Rules.GetMaximumDemandForDestination(destination);
				int demandIndex = 0;
				while (demandIndex < amountOfDemand && destination.TotalDemand < maximumAllowedDemand)
				{
					destination.unassignedDemand.Add(destination.GroupIndex);
					demandIndex++;
				}
			}
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x0006F1BC File Offset: 0x0006D3BC
		private Fix64 DemandMultiplierForDelayedBuildings(CityPlanModel cityPlan)
		{
			int numberOfLongFailedBuildings = 0;
			using (List<CityPlanModel.ScheduledBuilding>.Enumerator enumerator = cityPlan.scheduledBuildings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.spawnAttempts > this._constants.MaxFailedBuildingSpawnsBeforeIgnoringWeights)
					{
						numberOfLongFailedBuildings++;
					}
				}
			}
			if (numberOfLongFailedBuildings > 0)
			{
				return Fix64.One + Fix64.Log2((Fix64)((long)numberOfLongFailedBuildings));
			}
			return Fix64.One;
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x0006F240 File Offset: 0x0006D440
		private Fix64 IntervalForDestination(DestinationModel model)
		{
			Fix64 dayLength = (Fix64)20.0;
			if (this._clock.Time < Fix64.One)
			{
				return (Fix64)3L;
			}
			Fix64 timeInDaysWithOffset = this._clock.FractionalDays + this._demandModel.GetGroupDemandOscillationOffset(model.GroupIndex);
			Fix64 oscillationValue = this._city.Definition.schedulePlanner.GetOscillationForDemand(model.GroupIndex, timeInDaysWithOffset);
			return dayLength / (this._constants.AverageCarsPerDay * this._behaviour.GetDemandMultiplierForBuilding(model) * oscillationValue);
		}

		// Token: 0x040018DA RID: 6362
		[Dependency]
		private City _city;

		// Token: 0x040018DB RID: 6363
		[Dependency]
		private CityPlanModel _cityPlanModel;

		// Token: 0x040018DC RID: 6364
		[Dependency]
		private ClockModel _clock;

		// Token: 0x040018DD RID: 6365
		[Dependency]
		private DemandModel _demandModel;

		// Token: 0x040018DE RID: 6366
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x040018DF RID: 6367
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x040018E0 RID: 6368
		private readonly bool[] _allocatedColorGroups = new bool[6];

		// Token: 0x040018E1 RID: 6369
		private static readonly ProfilerMarker Profiler_Step = new ProfilerMarker(ProfilerUtility.CategoryProcess, "GenerateDemandProcess.Step");

		// Token: 0x0200048E RID: 1166
		public enum DemandGenerationStyle
		{
			// Token: 0x040018E3 RID: 6371
			Timer,
			// Token: 0x040018E4 RID: 6372
			PermanentBalanced
		}
	}
}
