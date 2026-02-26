using System;
using System.Collections.Generic;
using System.Diagnostics;
using Factory;
using Factory.Pools;
using FixMath;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004E3 RID: 1251
	public class DemandModel : IModel, IReusable
	{
		// Token: 0x060020A8 RID: 8360 RVA: 0x00081544 File Offset: 0x0007F744
		public void ApplyIncrementalSupplyFromHouse(HouseModel newHouse)
		{
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				if (destination.GroupIndex == newHouse.GroupIndex)
				{
					destination.contributedSupply += this.CalculateSupplyContributionFromHouseToDestination(newHouse, destination);
				}
			}
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x000815A0 File Offset: 0x0007F7A0
		public void ApplyAbsoluteSupplyToDestination(DestinationModel destination)
		{
			int groupIndex = destination.GroupIndex;
			Fix64 contributedSupply = Fix64.Zero;
			foreach (HouseModel house in this._simulation.GetModels<HouseModel>())
			{
				if (house.GroupIndex == groupIndex)
				{
					contributedSupply += this.CalculateSupplyContributionFromHouseToDestination(house, destination);
				}
			}
			destination.contributedSupply = contributedSupply;
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x00081604 File Offset: 0x0007F804
		public void CalculateSupplyScale(int groupIndex)
		{
			int destinationsInGroup = 0;
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				if (destination.isActive && destination.GroupIndex == groupIndex && !destination.IsTrainStation)
				{
					destinationsInGroup++;
				}
			}
			this._supplyScales[groupIndex] = this._constants.EvaluateDestinationCountHouseValueMultiplier(destinationsInGroup);
		}

		// Token: 0x060020AB RID: 8363 RVA: 0x00081670 File Offset: 0x0007F870
		public Fix64 GetSupplyScale(int groupIndex)
		{
			Fix64 supplyScale;
			if (this._supplyScales.TryGetValue(groupIndex, out supplyScale))
			{
				return supplyScale;
			}
			this.CalculateSupplyScale(groupIndex);
			return this._supplyScales[groupIndex];
		}

		// Token: 0x060020AC RID: 8364 RVA: 0x000816A4 File Offset: 0x0007F8A4
		public void RecalculateSupply()
		{
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				this.ApplyAbsoluteSupplyToDestination(destination);
			}
			this._supplyScales.Clear();
			this.doesSupplyNeedRecalculation = false;
		}

		// Token: 0x060020AD RID: 8365 RVA: 0x000816F4 File Offset: 0x0007F8F4
		public Fix64 GetGroupDemandOscillationOffset(int groupIndex)
		{
			Fix64 offset;
			if (!this._demandOscillationOffsets.TryGetValue(groupIndex, out offset))
			{
				CitySchedulePlanner planner = this._city.Definition.schedulePlanner;
				if (Diagnostics.Verify(planner.demandOscillationData != null && planner.demandOscillationData.Count > 0, "We have no demand oscillation for this city {0}! Defaulting to no offset", this._city.Definition.name))
				{
					if (Diagnostics.Verify(groupIndex >= 0 && groupIndex < planner.demandOscillationData.Count, "Group index {0} out of range for demand oscillation data (count {1}) for this city {2}! Defaulting to no offset", groupIndex, planner.demandOscillationData.Count, this._city.Definition.name))
					{
						GroupDemandOscillation demandOscillation = planner.demandOscillationData[groupIndex];
						offset = this._cityModel.pseudorandomGenerator.Fix64((Fix64)((long)demandOscillation.periodInDays));
					}
					else
					{
						offset = Fix64.Zero;
					}
				}
				else
				{
					offset = Fix64.Zero;
				}
				this._demandOscillationOffsets.Add(groupIndex, offset);
			}
			return offset;
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x000817EC File Offset: 0x0007F9EC
		[Conditional("UNITY_EDITOR")]
		public void OnPinAllocated(int groupIndex)
		{
			List<Fix64> times;
			if (this.allocatedPinsInLastWeek.TryGetValue(groupIndex, out times))
			{
				times.Add(this._clock.Time);
				return;
			}
			times = new List<Fix64>
			{
				this._clock.Time
			};
			this.allocatedPinsInLastWeek.Add(groupIndex, times);
		}

		// Token: 0x060020AF RID: 8367 RVA: 0x00081840 File Offset: 0x0007FA40
		[Conditional("UNITY_EDITOR")]
		public void OnPinReallocated(int receivingDestinationGroupIndex, int originalDestinationGroupIndex)
		{
			Dictionary<int, List<Fix64>> groupToUse;
			if (receivingDestinationGroupIndex == originalDestinationGroupIndex)
			{
				groupToUse = this.reallocatedPinsInLastWeek;
			}
			else
			{
				groupToUse = this.reallocatedToOtherGroupsPinsInLastWeek;
			}
			List<Fix64> times;
			if (groupToUse.TryGetValue(originalDestinationGroupIndex, out times))
			{
				times.Add(this._clock.Time);
				return;
			}
			times = new List<Fix64>
			{
				this._clock.Time
			};
			groupToUse.Add(originalDestinationGroupIndex, times);
		}

		// Token: 0x060020B0 RID: 8368 RVA: 0x000818A0 File Offset: 0x0007FAA0
		[Conditional("UNITY_EDITOR")]
		public void OnPinDiscarded(int groupIndex)
		{
			List<Fix64> times;
			if (this.discardedPinsInLastWeek.TryGetValue(groupIndex, out times))
			{
				times.Add(this._clock.Time);
				return;
			}
			times = new List<Fix64>
			{
				this._clock.Time
			};
			this.discardedPinsInLastWeek.Add(groupIndex, times);
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x000818F3 File Offset: 0x0007FAF3
		[Conditional("UNITY_EDITOR")]
		public void ClearOldestPinTrackingEntries()
		{
			this._clock.Time - (Fix64)140.0;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x00081914 File Offset: 0x0007FB14
		[Conditional("UNITY_EDITOR")]
		private void ClearPinsOlderThan(Fix64 oldestTime, Dictionary<int, List<Fix64>> group)
		{
			foreach (int groupIndex in group.Keys)
			{
				while (group[groupIndex].Count > 0 && group[groupIndex][0] < oldestTime)
				{
					group[groupIndex].RemoveAt(0);
				}
			}
		}

		// Token: 0x060020B3 RID: 8371 RVA: 0x00081994 File Offset: 0x0007FB94
		public Fix64 CalculateSupplyContributionFromHouseToDestination(HouseModel house, DestinationModel destination)
		{
			Vector2Int toDestination = (destination.Carpark.entranceAtTopLeft ? destination.Carpark.TopLeftDrivewayTileCoordinates : destination.Carpark.BottomRightDrivewayTileCoordinates) - house.tileModel.Coordinates;
			int angledComponent = Mathf.Min(Mathf.Abs(toDestination.x), Mathf.Abs(toDestination.y));
			int straightComponent = Mathf.Max(Mathf.Abs(toDestination.x), Mathf.Abs(toDestination.y)) - angledComponent;
			Fix64 distance = (Fix64)((long)angledComponent) * Fix64Consts.SqrtTwo + (Fix64)((long)straightComponent);
			return this._constants.EvaluateHouseContributionFromDistance(distance);
		}

		// Token: 0x060020B4 RID: 8372 RVA: 0x00081A40 File Offset: 0x0007FC40
		public void Reset()
		{
			this.spawnScale = Fix64.Zero;
			this.doesSupplyNeedRecalculation = false;
			this._supplyScales.Clear();
			this._demandOscillationOffsets.Clear();
			this.extraDemand.Clear();
			this.failedDestinationUpgrades.Clear();
			this.allocatedPinsInLastWeek.Clear();
			this.reallocatedPinsInLastWeek.Clear();
			this.reallocatedToOtherGroupsPinsInLastWeek.Clear();
			this.discardedPinsInLastWeek.Clear();
		}

		// Token: 0x060020B5 RID: 8373 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Inspect()
		{
		}

		// Token: 0x04001B1A RID: 6938
		public Fix64 spawnScale;

		// Token: 0x04001B1B RID: 6939
		public bool doesSupplyNeedRecalculation;

		// Token: 0x04001B1C RID: 6940
		private readonly Dictionary<int, Fix64> _supplyScales = new Dictionary<int, Fix64>();

		// Token: 0x04001B1D RID: 6941
		private readonly Dictionary<int, Fix64> _demandOscillationOffsets = new Dictionary<int, Fix64>();

		// Token: 0x04001B1E RID: 6942
		public readonly Dictionary<int, Fix64> extraDemand = new Dictionary<int, Fix64>();

		// Token: 0x04001B1F RID: 6943
		[Serialize(false, null)]
		public readonly Dictionary<int, int> failedDestinationUpgrades = new Dictionary<int, int>();

		// Token: 0x04001B20 RID: 6944
		[Serialize(false, null)]
		public readonly Dictionary<int, List<Fix64>> allocatedPinsInLastWeek = new Dictionary<int, List<Fix64>>();

		// Token: 0x04001B21 RID: 6945
		[Serialize(false, null)]
		public readonly Dictionary<int, List<Fix64>> reallocatedPinsInLastWeek = new Dictionary<int, List<Fix64>>();

		// Token: 0x04001B22 RID: 6946
		[Serialize(false, null)]
		public readonly Dictionary<int, List<Fix64>> reallocatedToOtherGroupsPinsInLastWeek = new Dictionary<int, List<Fix64>>();

		// Token: 0x04001B23 RID: 6947
		[Serialize(false, null)]
		public readonly Dictionary<int, List<Fix64>> discardedPinsInLastWeek = new Dictionary<int, List<Fix64>>();

		// Token: 0x04001B24 RID: 6948
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001B25 RID: 6949
		[Dependency]
		private City _city;

		// Token: 0x04001B26 RID: 6950
		[Dependency]
		private CityModel _cityModel;

		// Token: 0x04001B27 RID: 6951
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001B28 RID: 6952
		[Dependency]
		private SimulationConstantsData _constants;
	}
}
