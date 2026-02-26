using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000493 RID: 1171
	public class ParkVehiclesProcess : IProcess, IReusable
	{
		// Token: 0x06001D00 RID: 7424 RVA: 0x0007012D File Offset: 0x0006E32D
		public void Reset()
		{
			this._freeParkingSpaces.Clear();
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x0007013C File Offset: 0x0006E33C
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			foreach (CarparkModel carparkModel in simulation.GetModels<CarparkModel>())
			{
				CarparkModel.ParkingSpace longestParkedSpace = null;
				foreach (CarparkModel.ParkingSpace parkingSpace in carparkModel.spaces)
				{
					if (parkingSpace.vehicle == null)
					{
						this._freeParkingSpaces.Add(parkingSpace);
					}
					else if (parkingSpace.timeVehicleParked >= Fix64.Zero)
					{
						if (parkingSpace.vehicle.behaviorState == VehicleModel.BehaviorState.DrivingHome)
						{
							if (parkingSpace.vehicle.CurrentFrame.lane.roadChunk != parkingSpace.parkRoadChunk)
							{
								parkingSpace.vehicle.OnDepartedDestination();
								parkingSpace.vehicle = null;
								parkingSpace.timeVehicleParked = -Fix64.One;
								this._freeParkingSpaces.Add(parkingSpace);
							}
						}
						else
						{
							parkingSpace.timeVehicleParked += deltaTime;
							if (parkingSpace.timeVehicleParked > ParkVehiclesProcess.VehicleParkingTime)
							{
								parkingSpace.vehicle.behaviorState = VehicleModel.BehaviorState.DrivingHome;
								parkingSpace.vehicle.RequestPathfind(VehicleModel.PathfindUrgency.WhenPossible);
							}
							else if (longestParkedSpace == null || longestParkedSpace.timeVehicleParked < parkingSpace.timeVehicleParked)
							{
								longestParkedSpace = parkingSpace;
							}
						}
					}
					else if (parkingSpace.vehicle.CurrentFrame.lane.roadChunk == parkingSpace.parkRoadChunk)
					{
						parkingSpace.timeVehicleParked = Fix64.Zero;
						parkingSpace.vehicle.behaviorState = VehicleModel.BehaviorState.ParkedAtDestination;
						parkingSpace.vehicle.OnArrivedAtDestination();
					}
					else if (!Diagnostics.Verify(parkingSpace.vehicle.path.Count > 0 || parkingSpace.vehicle.NextFrame.lane.roadChunk == parkingSpace.parkRoadChunk, "A parking vehicle does not have a path! It will be directed into its parking space again, or dispatched home immediately."))
					{
						VehicleModel stalledVehicle = parkingSpace.vehicle;
						if (stalledVehicle.LastCommittedLane.OutboundLanes.Count == 1 && stalledVehicle.LastCommittedLane.OutboundLanes[0].roadChunk == parkingSpace.parkRoadChunk)
						{
							LaneModel parkingSpaceLane = stalledVehicle.LastCommittedLane.OutboundLanes[0];
							stalledVehicle.AssignPath(new List<LaneModel>
							{
								parkingSpaceLane
							}, parkingSpaceLane.Length / Fix64Consts.Two);
						}
						else
						{
							stalledVehicle.behaviorState = VehicleModel.BehaviorState.DrivingHome;
							stalledVehicle.RequestPathfind(VehicleModel.PathfindUrgency.WhenPossible);
							stalledVehicle.OnArrivedAtDestination();
							stalledVehicle.OnDepartedDestination();
							parkingSpace.vehicle = null;
							parkingSpace.timeVehicleParked = -Fix64.One;
							this._freeParkingSpaces.Add(parkingSpace);
						}
					}
				}
				if (this._freeParkingSpaces.Count == 0 && longestParkedSpace != null)
				{
					bool releaseLongestParkedVehicle = false;
					using (List<DestinationModel>.Enumerator enumerator3 = carparkModel.destinations.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.waitingDemand.Count > 0)
							{
								releaseLongestParkedVehicle = true;
								break;
							}
						}
					}
					if (releaseLongestParkedVehicle)
					{
						longestParkedSpace.vehicle.behaviorState = VehicleModel.BehaviorState.DrivingHome;
						longestParkedSpace.vehicle.RequestPathfind(VehicleModel.PathfindUrgency.WhenPossible);
					}
				}
				int vehicleDrivingThroughIndex = 0;
				while (vehicleDrivingThroughIndex < carparkModel.vehiclesDrivingThrough.Count)
				{
					VehicleModel vehicleDrivingThrough = carparkModel.vehiclesDrivingThrough[vehicleDrivingThroughIndex];
					if (vehicleDrivingThrough.CurrentFrame.lane.connection.input.type == RoadType.Carpark)
					{
						vehicleDrivingThrough.OnArrivedAtDestination();
						vehicleDrivingThrough.OnDepartedDestination();
						carparkModel.vehiclesDrivingThrough.RemoveAt(vehicleDrivingThroughIndex);
					}
					else
					{
						vehicleDrivingThroughIndex++;
					}
				}
				foreach (VehicleModel enteringVehicle in carparkModel.vehiclesEntering)
				{
					ParkVehiclesProcess.Log.Info("Attempting to park vehicle {0}.", new object[]
					{
						enteringVehicle.id
					});
					bool foundParkingSpaceForVehicle = false;
					if (this._freeParkingSpaces.Count > 0)
					{
						int freeParkingSpaceIndex = (this._freeParkingSpaces.Count == 1) ? 0 : this._cityModel.pseudorandomGenerator.Int(this._freeParkingSpaces.Count);
						CarparkModel.ParkingSpace freeParkingSpace = this._freeParkingSpaces[freeParkingSpaceIndex];
						List<LaneModel> path = new List<LaneModel>();
						LaneModel laneCursor = enteringVehicle.LastCommittedLane;
						if (!Diagnostics.Verify(carparkModel.entranceLanes.Contains(laneCursor), "Somehow the carpark {0} was asked to park {1}, which has not committed to drive to the carpark's entrance lane. It is being dispatched home instead.", carparkModel, enteringVehicle))
						{
							laneCursor = null;
						}
						while (laneCursor != null)
						{
							LaneModel laneIntoParkingSpace = null;
							LaneModel laneThroughCarpark = null;
							foreach (LaneModel outboundLane in laneCursor.OutboundLanes)
							{
								if (outboundLane.connection.output.type == RoadType.Carpark)
								{
									laneThroughCarpark = outboundLane;
								}
								else if (outboundLane.connection.output.type == RoadType.ParkingSpace && (outboundLane.roadChunk == freeParkingSpace.innerRoadChunk || outboundLane.roadChunk == freeParkingSpace.outerRoadChunk))
								{
									laneIntoParkingSpace = outboundLane;
								}
							}
							if (laneIntoParkingSpace != null)
							{
								if (Diagnostics.Verify(laneIntoParkingSpace.OutboundLanes.Count == 1 && laneIntoParkingSpace.OutboundLanes[0].connection.output.type == RoadType.ParkingSpace, "Vehicle {0} thought it found a path to a parking space within {1}, but the final lane {2} did not lead to a parking space. It will be dispatched home instead.", enteringVehicle, carparkModel, laneIntoParkingSpace))
								{
									foundParkingSpaceForVehicle = true;
									path.Add(laneIntoParkingSpace);
									path.Add(laneIntoParkingSpace.OutboundLanes[0]);
									break;
								}
								break;
							}
							else
							{
								if (!Diagnostics.Verify(laneThroughCarpark != null, "Failed to find a route to a parking space in {0} for vehicle {1}. It will be dispatched home.", carparkModel, enteringVehicle))
								{
									break;
								}
								path.Add(laneThroughCarpark);
								laneCursor = laneThroughCarpark;
							}
						}
						if (foundParkingSpaceForVehicle)
						{
							ParkVehiclesProcess.Log.Info("Vehicle {0} is parking at space {1}.", new object[]
							{
								enteringVehicle.id,
								freeParkingSpaceIndex
							});
							this._freeParkingSpaces.RemoveAt(freeParkingSpaceIndex);
							freeParkingSpace.vehicle = enteringVehicle;
							freeParkingSpace.timeVehicleParked = -Fix64.One;
							enteringVehicle.AssignPath(path, path[path.Count - 1].Length / Fix64Consts.Two);
						}
					}
					if (!foundParkingSpaceForVehicle)
					{
						ParkVehiclesProcess.Log.Info("Vehicle {0} cannot find a free space so is driving through the carpark.", new object[]
						{
							enteringVehicle.id
						});
						carparkModel.vehiclesDrivingThrough.Add(enteringVehicle);
						enteringVehicle.behaviorState = VehicleModel.BehaviorState.DrivingHome;
						enteringVehicle.RequestPathfind(VehicleModel.PathfindUrgency.WhenPossible);
					}
					enteringVehicle.OnEnteredCarpark();
				}
				carparkModel.vehiclesEntering.Clear();
				this._freeParkingSpaces.Clear();
			}
		}

		// Token: 0x040018F0 RID: 6384
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ParkVehiclesProcess");

		// Token: 0x040018F1 RID: 6385
		[Serialize(false, null)]
		private List<CarparkModel.ParkingSpace> _freeParkingSpaces = new List<CarparkModel.ParkingSpace>();

		// Token: 0x040018F2 RID: 6386
		[Dependency]
		private CityModel _cityModel;

		// Token: 0x040018F3 RID: 6387
		private static Fix64 VehicleParkingTime = (Fix64)3L;
	}
}
