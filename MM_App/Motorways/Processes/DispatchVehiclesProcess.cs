using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x0200048A RID: 1162
	public class DispatchVehiclesProcess : IProcess, IReusable
	{
		// Token: 0x06001CD6 RID: 7382 RVA: 0x0006DCE8 File Offset: 0x0006BEE8
		public void Reset()
		{
			this.sortedDestinationsWithDemand.Clear();
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x0006DCF8 File Offset: 0x0006BEF8
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			if (simulation.IsPaused)
			{
				return;
			}
			foreach (DestinationModel destination in simulation.GetModels<DestinationModel>())
			{
				if (destination.unassignedDemand.Count > 0)
				{
					if (this.sortedDestinationsWithDemand.Count == 0)
					{
						this.sortedDestinationsWithDemand.Add(destination);
					}
					else
					{
						bool addedDestination = false;
						for (int otherDestinationIndex = 0; otherDestinationIndex < this.sortedDestinationsWithDemand.Count; otherDestinationIndex++)
						{
							if (this.sortedDestinationsWithDemand[otherDestinationIndex].unassignedDemand.Count < destination.unassignedDemand.Count)
							{
								this.sortedDestinationsWithDemand.Insert(otherDestinationIndex, destination);
								addedDestination = true;
								break;
							}
						}
						if (!addedDestination)
						{
							this.sortedDestinationsWithDemand.Add(destination);
						}
					}
				}
			}
			for (int otherDestinationIndex2 = 0; otherDestinationIndex2 < this.sortedDestinationsWithDemand.Count; otherDestinationIndex2++)
			{
				DestinationModel destination2 = this.sortedDestinationsWithDemand[otherDestinationIndex2];
				if (otherDestinationIndex2 == this.sortedDestinationsWithDemand.Count - 1)
				{
					for (int unassignedDemandCount = 0; unassignedDemandCount < destination2.unassignedDemand.Count; unassignedDemandCount++)
					{
						int demandedGroupIndex = destination2.unassignedDemand[0];
						if (!this.DispatchVehicleToDestination(simulation, demandedGroupIndex, destination2))
						{
							break;
						}
						destination2.unassignedDemand.RemoveAt(0);
						destination2.waitingDemand.Add(demandedGroupIndex);
					}
				}
				else
				{
					int carsToAssign = Math.Min(Math.Max(destination2.unassignedDemand.Count / 2, 2), destination2.unassignedDemand.Count);
					for (int unassignedDemandCount2 = 0; unassignedDemandCount2 < carsToAssign; unassignedDemandCount2++)
					{
						int demandedGroupIndex2 = destination2.unassignedDemand[0];
						if (!this.DispatchVehicleToDestination(simulation, demandedGroupIndex2, destination2))
						{
							break;
						}
						destination2.unassignedDemand.RemoveAt(0);
						destination2.waitingDemand.Add(demandedGroupIndex2);
					}
				}
			}
			this.sortedDestinationsWithDemand.Clear();
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x0006DED4 File Offset: 0x0006C0D4
		private bool DispatchVehicleToDestination(ISimulation simulation, int groupIndex, DestinationModel destination)
		{
			int shortestPathCost = int.MaxValue;
			HouseModel nearestHouse = null;
			foreach (HouseModel house in simulation.GetModels<HouseModel>())
			{
				if (house.GroupIndex == groupIndex && house.HasWaitingVehicle)
				{
					int pathCost = this._pathfinder.GetMinPathCost(house.FirstWaitingVehicle.CurrentFrame.lane, destination.Carpark.entranceLanes, false);
					if (pathCost != -1 && pathCost < shortestPathCost)
					{
						shortestPathCost = pathCost;
						nearestHouse = house;
					}
				}
			}
			if (nearestHouse != null)
			{
				VehicleModel vehicle = nearestHouse.FirstWaitingVehicle;
				List<LaneModel> path = this._pathfinder.CreatePath(vehicle.CurrentFrame.lane, destination.Carpark.entranceLanes, false);
				if (Diagnostics.Verify(path != null && path.Count > 0))
				{
					if (FeatureToggle.IsFeatureEnabled(Feature.ValidateSimulationDeterminism))
					{
						SnapshotModel snapshotModel = simulation.GetModel<SnapshotModel>();
						if (snapshotModel != null)
						{
							VehicleDispatchRecord vehicleDispatchRecord = simulation.Scope.Get<VehicleDispatchRecord>();
							vehicleDispatchRecord.HouseCoordinates = vehicle.house.tileModel.Coordinates;
							vehicleDispatchRecord.DestinationCoordinates = destination.TileModels[0].Coordinates;
							vehicleDispatchRecord.SimulationFrame = this._clock.FrameCount;
							snapshotModel.vehicleDispatches.Add(vehicleDispatchRecord);
						}
					}
					vehicle.behaviorState = VehicleModel.BehaviorState.DrivingToDestination;
					vehicle.destination = destination;
					vehicle.AssignPath(path, -Fix64.One);
					vehicle.pathLengthAtStartOfJourney = vehicle.pathLength;
					path = this._pathfinder.CreatePath(destination.Carpark.entranceLanes[0], nearestHouse.DrivewayLane, false);
					if (Diagnostics.Verify(path != null, "House at {0} has a path to a destination, but no path could be found back on simulation frame {1}.", nearestHouse.tileModel.Coordinates, this._clock.FrameCount))
					{
						vehicle.AssignReturnPath(path);
					}
					vehicle.house.waitingVehicles.Remove(vehicle);
					vehicle.OnDepartedHouse();
					if (vehicle.house.HasWaitingVehicle)
					{
						vehicle.house.waitingVehicles[0].targetDistanceAlongLastLane = vehicle.house.GetLaneDistanceAtFrontOfDriveway(vehicle.house.DrivewayLane);
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x040018D1 RID: 6353
		private readonly List<DestinationModel> sortedDestinationsWithDemand = new List<DestinationModel>();

		// Token: 0x040018D2 RID: 6354
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x040018D3 RID: 6355
		[Dependency]
		private Clock _clock;
	}
}
