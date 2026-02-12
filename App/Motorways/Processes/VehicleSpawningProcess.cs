using System;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x020004C2 RID: 1218
	public class VehicleSpawningProcess : IProcess, IReusable
	{
		// Token: 0x06001FAD RID: 8109 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001FAE RID: 8110 RVA: 0x0007D554 File Offset: 0x0007B754
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			foreach (HouseModel house in simulation.GetModels<HouseModel>())
			{
				if (house.ownedVehicles.Count == 0)
				{
					LaneModel drivewayLane = house.DrivewayLane;
					if (drivewayLane != null)
					{
						for (int carSpawnIndex = 0; carSpawnIndex < 2; carSpawnIndex++)
						{
							VehicleModel vehicle = simulation.Scope.Get<VehicleModel>();
							vehicle.MoveToLane(drivewayLane, (carSpawnIndex == 0) ? house.GetLaneDistanceAtFrontOfDriveway(drivewayLane) : house.GetLaneDistanceAtBackOfDriveway(drivewayLane));
							vehicle.house = house;
							vehicle.behaviorState = VehicleModel.BehaviorState.WaitingForDestination;
							vehicle.lastNotifiedBehaviorState = VehicleModel.BehaviorState.WaitingForDestination;
							house.ownedVehicles.Add(vehicle);
							house.waitingVehicles.Add(vehicle);
							simulation.AddModel(vehicle);
						}
					}
				}
				else if (house.waitingVehicles.Count > 0)
				{
					int waitingVehicleIndex = 0;
					while (waitingVehicleIndex < house.waitingVehicles.Count)
					{
						VehicleModel waitingVehicle = house.waitingVehicles[waitingVehicleIndex];
						if (waitingVehicle.CurrentFrame.lane.state != RoadState.Mothballed)
						{
							waitingVehicleIndex++;
						}
						else if (Fix64.Abs(waitingVehicle.targetDistanceAlongLastLane - waitingVehicle.CurrentFrame.distanceAlongLane) < VehicleSpawningProcess.VehicleDistanceToTargetThreshold)
						{
							bool flag = waitingVehicle.targetDistanceAlongLastLane > house.GetLaneDistanceAtCenterOfDriveway(waitingVehicle.CurrentFrame.lane);
							LaneModel newDrivewayLane = house.DrivewayLane;
							Fix64 distanceAlongDriveway = flag ? house.GetLaneDistanceAtFrontOfDriveway(newDrivewayLane) : house.GetLaneDistanceAtBackOfDriveway(newDrivewayLane);
							waitingVehicle.MoveToLane(newDrivewayLane, distanceAlongDriveway);
							waitingVehicleIndex++;
						}
						else
						{
							waitingVehicle.behaviorState = VehicleModel.BehaviorState.RealigningDriveway;
							waitingVehicle.targetDistanceAlongLastLane = house.GetLaneDistanceAtCenterOfDriveway(waitingVehicle.CurrentFrame.lane);
							house.waitingVehicles.RemoveAt(waitingVehicleIndex);
							house.realigningVehicles.Add(waitingVehicle);
						}
					}
				}
				if (house.realigningVehicles.Count > 0)
				{
					int realigningVehicleIndex = 0;
					while (realigningVehicleIndex < house.realigningVehicles.Count)
					{
						VehicleModel realigningVehicle = house.realigningVehicles[realigningVehicleIndex];
						bool didVehicleRealign = false;
						if (realigningVehicle.CurrentFrame.lane.state == RoadState.Mothballed)
						{
							if (Fix64.Abs(realigningVehicle.targetDistanceAlongLastLane - realigningVehicle.CurrentFrame.distanceAlongLane) < VehicleSpawningProcess.VehicleDistanceToHouseThreshold)
							{
								realigningVehicle.MoveToLane(house.DrivewayLane, house.GetLaneDistanceAtCenterOfDriveway(house.DrivewayLane));
								if (house.HasWaitingVehicle)
								{
									realigningVehicle.targetDistanceAlongLastLane = house.GetLaneDistanceAtBackOfDriveway(house.DrivewayLane);
								}
								else
								{
									realigningVehicle.targetDistanceAlongLastLane = house.GetLaneDistanceAtFrontOfDriveway(house.DrivewayLane);
									didVehicleRealign = true;
								}
							}
						}
						else
						{
							VehicleModel firstVehicleOnDriveway = house.DrivewayLane.FirstVehicleFromHouse(house);
							if (!house.HasWaitingVehicle && (firstVehicleOnDriveway == realigningVehicle || firstVehicleOnDriveway.behaviorState != VehicleModel.BehaviorState.RealigningDriveway))
							{
								realigningVehicle.targetDistanceAlongLastLane = house.GetLaneDistanceAtFrontOfDriveway(house.DrivewayLane);
								didVehicleRealign = true;
							}
							else
							{
								realigningVehicle.targetDistanceAlongLastLane = house.GetLaneDistanceAtBackOfDriveway(house.DrivewayLane);
								if (Fix64.Abs(realigningVehicle.targetDistanceAlongLastLane - realigningVehicle.CurrentFrame.distanceAlongLane) < VehicleSpawningProcess.VehicleDistanceToTargetThreshold)
								{
									realigningVehicle.targetDistanceAlongLastLane = realigningVehicle.CurrentFrame.distanceAlongLane;
									didVehicleRealign = true;
								}
							}
						}
						if (didVehicleRealign)
						{
							realigningVehicle.behaviorState = VehicleModel.BehaviorState.WaitingForDestination;
							house.waitingVehicles.Add(realigningVehicle);
							house.realigningVehicles.RemoveAt(realigningVehicleIndex);
						}
						else
						{
							realigningVehicleIndex++;
						}
					}
				}
			}
		}

		// Token: 0x04001A70 RID: 6768
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("VehicleSpawningProcess");

		// Token: 0x04001A71 RID: 6769
		private static readonly Fix64 VehicleDistanceToTargetThreshold = Fix64.FromRaw(214748368L);

		// Token: 0x04001A72 RID: 6770
		private static readonly Fix64 VehicleDistanceToHouseThreshold = Fix64.FromRaw(429496736L);
	}
}
