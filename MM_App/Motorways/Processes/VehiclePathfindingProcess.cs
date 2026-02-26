using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x020004C1 RID: 1217
	public class VehiclePathfindingProcess : IProcess, IReusable
	{
		// Token: 0x06001FA9 RID: 8105 RVA: 0x0007CEC1 File Offset: 0x0007B0C1
		public void Reset()
		{
			this._vehiclesToPathfind.Clear();
			VehiclePathfindingProcess.newPath.Clear();
			VehiclePathfindingProcess.newReturnPath.Clear();
			VehiclePathfindingProcess.enumerablePathHolder = null;
		}

		// Token: 0x06001FAA RID: 8106 RVA: 0x0007CEE8 File Offset: 0x0007B0E8
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			int latestLaneChangeFrame = simulation.GetModel<CityModel>().latestLaneChangeFrame;
			foreach (VehicleModel vehicle in simulation.GetModels<VehicleModel>())
			{
				if (vehicle.repathUrgency != VehicleModel.PathfindUrgency.NotRequired || vehicle.returnRepathUrgency != VehicleModel.PathfindUrgency.NotRequired)
				{
					if (vehicle.behaviorState == VehicleModel.BehaviorState.DrivingHome)
					{
						vehicle.returnRepathUrgency = VehicleModel.PathfindUrgency.NotRequired;
						if (vehicle.LastCommittedLane.roadChunk == vehicle.house.DrivewayLane.roadChunk)
						{
							vehicle.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
							continue;
						}
					}
					if (vehicle.behaviorState == VehicleModel.BehaviorState.ParkingAtDestination || vehicle.behaviorState == VehicleModel.BehaviorState.ParkedAtDestination)
					{
						vehicle.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
					}
					if (vehicle.behaviorState == VehicleModel.BehaviorState.RealigningDriveway)
					{
						vehicle.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
						vehicle.returnRepathUrgency = VehicleModel.PathfindUrgency.NotRequired;
					}
					else
					{
						VehicleModel.PathfindUrgency urgency = (vehicle.repathUrgency > vehicle.returnRepathUrgency) ? vehicle.repathUrgency : vehicle.returnRepathUrgency;
						if (urgency != VehicleModel.PathfindUrgency.NotRequired && vehicle.latestAttemptedPathfindFrame < latestLaneChangeFrame)
						{
							bool flag = urgency == VehicleModel.PathfindUrgency.AsSoonAsPossible;
							int vehiclePriorityIndex = 0;
							if (!flag)
							{
								while (vehiclePriorityIndex < this._vehiclesToPathfind.Count && this._vehiclesToPathfind[vehiclePriorityIndex].latestAttemptedPathfindFrame <= vehicle.latestAttemptedPathfindFrame)
								{
									vehiclePriorityIndex++;
								}
							}
							this._vehiclesToPathfind.Insert(vehiclePriorityIndex, vehicle);
						}
					}
				}
			}
			foreach (VehicleModel vehicleToPathfind in this._vehiclesToPathfind)
			{
				vehicleToPathfind.latestAttemptedPathfindFrame = this._clock.FrameCount;
				bool foundPath = false;
				bool foundReturnPath = false;
				VehiclePathfindingProcess.newPath.Clear();
				VehiclePathfindingProcess.newReturnPath.Clear();
				Fix64 distanceAlongTargetLane = -Fix64.One;
				if (vehicleToPathfind.behaviorState == VehicleModel.BehaviorState.DrivingToDestination)
				{
					bool tryReturnPath = vehicleToPathfind.returnRepathUrgency > VehicleModel.PathfindUrgency.NotRequired;
					if (vehicleToPathfind.repathUrgency != VehicleModel.PathfindUrgency.NotRequired)
					{
						VehiclePathfindingProcess.enumerablePathHolder = this._pathfinder.CreatePath(vehicleToPathfind.LastCommittedLane, vehicleToPathfind.destination.Carpark.entranceLanes, true);
						if (Diagnostics.Verify(VehiclePathfindingProcess.enumerablePathHolder != null, "Vehicle {0} could not find a path to the destination it is already driving towards on simulation frame {1}.", vehicleToPathfind.id, this._clock.FrameCount))
						{
							foundPath = true;
							VehiclePathfindingProcess.newPath.AddRange(VehiclePathfindingProcess.enumerablePathHolder);
						}
						else
						{
							tryReturnPath = false;
						}
					}
					if (tryReturnPath)
					{
						VehiclePathfindingProcess.enumerablePathHolder = this._pathfinder.CreatePath(vehicleToPathfind.destination.Carpark.entranceLanes[0], vehicleToPathfind.house.DrivewayLane, true);
						if (VehiclePathfindingProcess.enumerablePathHolder == null)
						{
							foreach (TileDirection mothballedHomeLaneDirection in vehicleToPathfind.house.tileModel.Tile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore))
							{
								LaneModel alternateLaneToPathTo = vehicleToPathfind.house.tileModel.roadChunk.GetLanesEnteringFromDirection(mothballedHomeLaneDirection)[0];
								VehiclePathfindingProcess.enumerablePathHolder = this._pathfinder.CreatePath(vehicleToPathfind.destination.Carpark.entranceLanes[0], alternateLaneToPathTo, true);
								if (VehiclePathfindingProcess.enumerablePathHolder != null)
								{
									break;
								}
							}
						}
						if (Diagnostics.Verify(VehiclePathfindingProcess.enumerablePathHolder != null, "Vehicle {0} could not find a return path back to its house on simulation frame {1}.", vehicleToPathfind.id, this._clock.FrameCount))
						{
							foundReturnPath = true;
							VehiclePathfindingProcess.newReturnPath.AddRange(VehiclePathfindingProcess.enumerablePathHolder);
						}
					}
				}
				else if (vehicleToPathfind.behaviorState == VehicleModel.BehaviorState.DrivingHome)
				{
					VehiclePathfindingProcess.enumerablePathHolder = this._pathfinder.CreatePath(vehicleToPathfind.LastCommittedLane, vehicleToPathfind.house.DrivewayLane.roadChunk.lanes, true);
					if (Diagnostics.Verify(VehiclePathfindingProcess.enumerablePathHolder != null, "Vehicle {0} could not find a path home on simulation frame {1}.", vehicleToPathfind.id, this._clock.FrameCount))
					{
						foundPath = true;
						VehiclePathfindingProcess.newPath.AddRange(VehiclePathfindingProcess.enumerablePathHolder);
						if (VehiclePathfindingProcess.newPath.Count > 0)
						{
							LaneModel drivewayLane = VehiclePathfindingProcess.newPath[VehiclePathfindingProcess.newPath.Count - 1];
							distanceAlongTargetLane = vehicleToPathfind.house.GetLaneDistanceAtFrontOfDriveway(drivewayLane);
						}
					}
				}
				else if (vehicleToPathfind.behaviorState == VehicleModel.BehaviorState.ParkedAtDestination || vehicleToPathfind.behaviorState == VehicleModel.BehaviorState.ParkingAtDestination)
				{
					bool flag2;
					if (vehicleToPathfind == null)
					{
						flag2 = (null != null);
					}
					else
					{
						DestinationModel destination = vehicleToPathfind.destination;
						flag2 = (((destination != null) ? destination.Carpark : null) != null);
					}
					LaneModel laneToPathfindFrom = flag2 ? vehicleToPathfind.destination.Carpark.entranceLanes[0] : vehicleToPathfind.LastCommittedLane;
					VehiclePathfindingProcess.enumerablePathHolder = this._pathfinder.CreatePath(laneToPathfindFrom, vehicleToPathfind.house.DrivewayLane, true);
					if (VehiclePathfindingProcess.enumerablePathHolder == null)
					{
						foreach (TileDirection mothballedHomeLaneDirection2 in vehicleToPathfind.house.tileModel.Tile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore))
						{
							List<LaneModel> drivewayLanes = vehicleToPathfind.house.tileModel.roadChunk.GetLanesEnteringFromDirection(mothballedHomeLaneDirection2);
							if (Diagnostics.Verify(drivewayLanes.Count > 0, "A house tile has no mothballed lanes in a direction that it says it does. Lies!"))
							{
								LaneModel alternateLaneToPathTo2 = drivewayLanes[0];
								VehiclePathfindingProcess.enumerablePathHolder = this._pathfinder.CreatePath(laneToPathfindFrom, alternateLaneToPathTo2, true);
								if (VehiclePathfindingProcess.enumerablePathHolder != null)
								{
									break;
								}
							}
						}
					}
					if (Diagnostics.Verify(VehiclePathfindingProcess.enumerablePathHolder != null, "Vehicle {0} could not find a path back to the house from the destination on simulation frame {1}.", vehicleToPathfind.id, this._clock.FrameCount))
					{
						foundReturnPath = true;
						VehiclePathfindingProcess.newReturnPath.AddRange(VehiclePathfindingProcess.enumerablePathHolder);
					}
				}
				else
				{
					Diagnostics.FailAssert("Vehicle {0} with behavior state {1} is requesting a pathfind.", new object[]
					{
						vehicleToPathfind.id,
						vehicleToPathfind.behaviorState
					});
					vehicleToPathfind.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
					vehicleToPathfind.returnRepathUrgency = VehicleModel.PathfindUrgency.NotRequired;
				}
				if (foundPath)
				{
					vehicleToPathfind.latestAttemptedPathfindFrame = 0;
					vehicleToPathfind.repathUrgency = VehicleModel.PathfindUrgency.NotRequired;
					if (VehiclePathfindingProcess.newPath.Count > 0)
					{
						vehicleToPathfind.AssignPath(VehiclePathfindingProcess.newPath, distanceAlongTargetLane);
					}
				}
				if (foundReturnPath)
				{
					vehicleToPathfind.latestAttemptedPathfindFrame = 0;
					vehicleToPathfind.returnRepathUrgency = VehicleModel.PathfindUrgency.NotRequired;
					if (VehiclePathfindingProcess.newReturnPath.Count > 0)
					{
						vehicleToPathfind.AssignReturnPath(VehiclePathfindingProcess.newReturnPath);
					}
				}
				VehiclePathfindingProcess.newPath.Clear();
				VehiclePathfindingProcess.newReturnPath.Clear();
				VehiclePathfindingProcess.enumerablePathHolder = null;
			}
			this._vehiclesToPathfind.Clear();
		}

		// Token: 0x04001A69 RID: 6761
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x04001A6A RID: 6762
		[Dependency]
		private Clock _clock;

		// Token: 0x04001A6B RID: 6763
		[Serialize(false, null)]
		private List<VehicleModel> _vehiclesToPathfind = new List<VehicleModel>();

		// Token: 0x04001A6C RID: 6764
		private static readonly List<LaneModel> newPath = new List<LaneModel>();

		// Token: 0x04001A6D RID: 6765
		private static readonly List<LaneModel> newReturnPath = new List<LaneModel>();

		// Token: 0x04001A6E RID: 6766
		private static IEnumerable<LaneModel> enumerablePathHolder;

		// Token: 0x04001A6F RID: 6767
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("VehiclePathfindingProcess");
	}
}
