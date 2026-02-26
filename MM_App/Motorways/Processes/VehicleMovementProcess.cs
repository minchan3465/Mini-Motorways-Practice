using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using Unity.Profiling;

namespace Motorways.Processes
{
	// Token: 0x020004BE RID: 1214
	public class VehicleMovementProcess : IProcess, IReusable
	{
		// Token: 0x06001F9B RID: 8091 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001F9C RID: 8092 RVA: 0x0007B6B8 File Offset: 0x000798B8
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			bool useAverageLaneSpeed = this._constants.useAverageLaneSpeedRatherThanMin;
			Fix64 maxSpeed = VehicleMovementProcess.DefaultDesiredSpeed * this._constants.speedMultiplier;
			IntersectionDecisionDatabaseModel decisionDatabase = null;
			if (FeatureToggle.IsFeatureEnabled(Feature.RecordIntersectionDecisions))
			{
				decisionDatabase = simulation.GetModel<IntersectionDecisionDatabaseModel>();
			}
			foreach (VehicleModel vehicle3 in simulation.GetModels<VehicleModel>())
			{
				LaneModel currentLane = vehicle3.CurrentFrame.lane;
				RoadChunkModel currentRoadChunk = currentLane.roadChunk;
				LaneModel nextLane = null;
				RoadChunkModel nextRoadChunk = null;
				Fix64 distanceToNextRoadChunk = Fix64.MaxValue;
				VehicleModel.ObstacleType obstacleType = VehicleModel.ObstacleType.None;
				Fix64 distanceToObstacle = Fix64.MaxValue;
				Fix64 obstacleOffset = Fix64.Zero;
				Fix64 obstacleSpeed = Fix64.Zero;
				Fix64 obstacleAcceleration = Fix64.Zero;
				VehicleModel leadingVehicle = null;
				Fix64 distanceToLeadingVehicle = Fix64.MaxValue;
				LaneModel blockingLane = null;
				Fix64 distanceToBlockingLane = Fix64.MaxValue;
				bool isBlockedByHotswappingLane = false;
				VehicleModel blockingVehicle = null;
				RoadChunkModel uncontrolledFreeIntersection = null;
				Fix64 distanceToUncontrolledIntersection = Fix64.MaxValue;
				RoadChunkModel upcomingCrossing = null;
				Fix64 distanceToUpcomingCrossing = Fix64.MaxValue;
				bool isVehicleStopping = false;
				if (!this._constants.useAverageLaneSpeedRatherThanMinOnMotorways)
				{
					useAverageLaneSpeed = !vehicle3.CurrentFrame.lane.connection.IsMotorway;
				}
				if (!currentRoadChunk.CanTraversingVehicleContinue(vehicle3))
				{
					blockingLane = currentLane;
					distanceToBlockingLane = Fix64.Zero;
					obstacleType = VehicleModel.ObstacleType.BlockingIntersection;
				}
				else if (currentLane.TryGetNextVehicleAfter(vehicle3, out leadingVehicle, out distanceToLeadingVehicle) && leadingVehicle != null)
				{
					obstacleType = VehicleModel.ObstacleType.LeadingVehicle;
					distanceToObstacle = distanceToLeadingVehicle;
					obstacleSpeed = leadingVehicle.CurrentFrame.speed;
					obstacleAcceleration = leadingVehicle.CurrentFrame.acceleration;
					blockingVehicle = leadingVehicle;
				}
				Fix64 minLaneSpeedLimit = currentLane.SpeedLimit;
				Fix64 averageLaneSpeedLimit = minLaneSpeedLimit;
				int pathCount = vehicle3.path.Count;
				if (pathCount == 0)
				{
					Fix64 distanceToTarget = vehicle3.targetDistanceAlongLastLane - vehicle3.CurrentFrame.distanceAlongLane;
					if (vehicle3.behaviorState == VehicleModel.BehaviorState.RealigningDriveway || obstacleType == VehicleModel.ObstacleType.None || distanceToObstacle > distanceToTarget)
					{
						obstacleType = VehicleModel.ObstacleType.Target;
						distanceToObstacle = Fix64.Abs(distanceToTarget);
						obstacleOffset = VehicleMovementProcess.TargetStoppingOffset;
						obstacleSpeed = Fix64.Zero;
						obstacleAcceleration = Fix64.Zero;
					}
				}
				else
				{
					Fix64 distanceInspected = currentLane.Length - vehicle3.CurrentFrame.distanceAlongLane;
					Fix64 distanceAveraged = Fix64.Min(this._constants.LookaheadDistance, distanceInspected);
					averageLaneSpeedLimit *= distanceAveraged;
					nextLane = vehicle3.path[0];
					nextRoadChunk = nextLane.roadChunk;
					distanceToNextRoadChunk = distanceInspected;
					IntersectionEntryDecision decision = null;
					if (decisionDatabase != null && (nextRoadChunk.lanes.Count > 2 || nextRoadChunk.IsTrainCrossing))
					{
						decision = this._scope.Get<IntersectionEntryDecision>();
					}
					VehicleModel vehicleBlockingIntersection;
					if (!nextRoadChunk.CanInboundVehicleEnter(vehicle3, out vehicleBlockingIntersection, decision))
					{
						isVehicleStopping = true;
						if (obstacleType == VehicleModel.ObstacleType.None)
						{
							blockingLane = nextLane;
							distanceToBlockingLane = distanceInspected;
							obstacleType = VehicleModel.ObstacleType.BlockingIntersection;
							distanceToObstacle = distanceInspected;
							if (!nextRoadChunk.isTileCorner)
							{
								obstacleOffset += VehicleMovementProcess.IntersectionStoppingOffset;
							}
							blockingVehicle = vehicleBlockingIntersection;
							this._stuckVehicles.Add(vehicle3);
						}
					}
					if (decision != null)
					{
						decisionDatabase.AddDecision(decision);
					}
					bool hasCommittedToHotswappingLane = false;
					int pathIndex = 0;
					LaneModel nextLaneOnPath = nextLane;
					while (nextLaneOnPath != null && distanceInspected < this._constants.LookaheadDistance)
					{
						minLaneSpeedLimit = Fix64.Min(nextLaneOnPath.SpeedLimit, minLaneSpeedLimit);
						Fix64 laneDistanceToAverage = Fix64.Min(nextLaneOnPath.Length, this._constants.LookaheadDistance - distanceAveraged);
						averageLaneSpeedLimit += nextLaneOnPath.SpeedLimit * laneDistanceToAverage;
						distanceAveraged += laneDistanceToAverage;
						if (pathIndex < 2)
						{
							hasCommittedToHotswappingLane |= nextLaneOnPath.IsAboutToHotswap;
						}
						if (obstacleType == VehicleModel.ObstacleType.None)
						{
							RoadChunkModel nextRoadChunkOnPath = nextLaneOnPath.roadChunk;
							VehicleModel vehicleModel;
							if (pathIndex > 0 && !nextRoadChunkOnPath.CanInboundVehicleEnter(vehicle3, out vehicleModel, null))
							{
								blockingLane = nextLaneOnPath;
								distanceToBlockingLane = distanceInspected;
								obstacleType = VehicleModel.ObstacleType.BlockingIntersection;
								distanceToObstacle = distanceInspected;
								if (nextRoadChunkOnPath.IsTrainCrossing)
								{
									obstacleOffset -= this._constants.crossingStopDistance * VehicleMovementProcess.VehicleLength;
									if (nextRoadChunkOnPath.isTileCorner)
									{
										obstacleOffset -= VehicleMovementProcess.IntersectionStoppingOffset;
									}
								}
								else if (!nextRoadChunkOnPath.isTileCorner)
								{
									obstacleOffset += VehicleMovementProcess.IntersectionStoppingOffset;
								}
							}
							else
							{
								leadingVehicle = nextLaneOnPath.GetLastVehicle(true);
								if (leadingVehicle != null)
								{
									distanceToLeadingVehicle = distanceInspected + leadingVehicle.CurrentFrame.distanceAlongLane;
									obstacleType = VehicleModel.ObstacleType.LeadingVehicle;
									distanceToObstacle = distanceToLeadingVehicle;
									obstacleSpeed = leadingVehicle.CurrentFrame.speed;
									obstacleAcceleration = leadingVehicle.CurrentFrame.acceleration;
									blockingVehicle = leadingVehicle;
								}
								if (pathIndex == pathCount - 1)
								{
									Fix64 distanceToTarget2 = distanceInspected + vehicle3.targetDistanceAlongLastLane;
									if (obstacleType == VehicleModel.ObstacleType.None || distanceToObstacle > distanceToTarget2)
									{
										distanceToObstacle = distanceToTarget2;
										obstacleOffset = VehicleMovementProcess.TargetStoppingOffset;
										obstacleType = VehicleModel.ObstacleType.Target;
									}
								}
								if (obstacleType == VehicleModel.ObstacleType.None && uncontrolledFreeIntersection == null && distanceInspected > VehicleMovementProcess.MinimumDistanceToNonBlockingIntersection && !nextRoadChunkOnPath.IsControlled && nextRoadChunkOnPath.GetNumberOfRoadsInIntersectionForSlowingVehicles() >= this._constants.NumberOfRoadsAtIntersectionToSlowDownFor)
								{
									uncontrolledFreeIntersection = nextRoadChunkOnPath;
									distanceToUncontrolledIntersection = distanceInspected;
									if (!nextRoadChunkOnPath.isTileCorner)
									{
										distanceToUncontrolledIntersection += VehicleMovementProcess.IntersectionStoppingOffset;
									}
								}
								if (obstacleType == VehicleModel.ObstacleType.None && upcomingCrossing == null && nextRoadChunkOnPath.IsTrainCrossing && distanceInspected > VehicleMovementProcess.VehicleLength * this._constants.crossingSlowDistance)
								{
									upcomingCrossing = nextRoadChunkOnPath;
									distanceToUpcomingCrossing = distanceInspected;
								}
							}
						}
						distanceInspected += nextLaneOnPath.Length;
						pathIndex++;
						nextLaneOnPath = ((pathIndex < pathCount) ? vehicle3.path[pathIndex] : null);
					}
					if (pathCount > 2)
					{
						while (!hasCommittedToHotswappingLane && pathIndex < 2)
						{
							hasCommittedToHotswappingLane |= vehicle3.path[pathIndex].IsAboutToHotswap;
							pathIndex++;
						}
						if (!hasCommittedToHotswappingLane && vehicle3.path[2].IsAboutToHotswap)
						{
							RoadTileConnection currentLaneConnection = currentLane.connection;
							isBlockedByHotswappingLane = (currentLaneConnection.input.type != RoadType.Carpark && currentLaneConnection.output.type != RoadType.Carpark && currentLaneConnection.output.type != RoadType.ParkingSpace);
							isVehicleStopping = (isVehicleStopping || isBlockedByHotswappingLane);
						}
					}
					if (useAverageLaneSpeed)
					{
						averageLaneSpeedLimit /= distanceAveraged;
					}
				}
				Fix64 desiredSpeed = (useAverageLaneSpeed ? averageLaneSpeedLimit : minLaneSpeedLimit) * maxSpeed;
				Fix64 maxAcceleration;
				if (currentRoadChunk.IsRoundabout || (nextRoadChunk != null && nextRoadChunk.IsRoundabout))
				{
					maxAcceleration = this._constants.roundaboutAcceleration;
				}
				else if (currentRoadChunk.IsTrainCrossing || (nextRoadChunk != null && nextRoadChunk.IsTrainCrossing))
				{
					maxAcceleration = this._constants.maxAccelerationOnCrossings;
				}
				else
				{
					RoadChunkModel houseRoadChunk = vehicle3.house.DrivewayLane.roadChunk;
					bool flag = currentRoadChunk == houseRoadChunk | nextRoadChunk == houseRoadChunk | (currentRoadChunk.IsControlled || (nextRoadChunk != null && nextRoadChunk.IsControlled));
					RoadTileConnection currentConnection = currentLane.connection;
					maxAcceleration = ((flag | (currentConnection.input.type == RoadType.Motorway && currentConnection.output.type == RoadType.Motorway && vehicle3.CurrentFrame.lane.Length - vehicle3.CurrentFrame.distanceAlongLane < VehicleMovementProcess.VehicleLength)) ? this._constants.controlledIntersectionAcceleration : this._constants.maxAcceleration);
				}
				if (vehicle3.vehiclePushingInto != null)
				{
					if (obstacleType == VehicleModel.ObstacleType.LeadingVehicle && leadingVehicle == vehicle3.vehiclePushingInto)
					{
						if (obstacleSpeed > this._constants.minSpeedBeforePushingCycle)
						{
							this._vehiclesInBrokenPushingCycles.Add(vehicle3);
						}
						obstacleSpeed = Fix64.Max(obstacleSpeed, this._constants.minSpeedBeforePushingCycle);
						obstacleOffset = Fix64.Max(VehicleMovementProcess.VehicleLength + VehicleMovementProcess.MinimumGap * Fix64Consts.Two - distanceToObstacle, Fix64.Zero);
					}
					else
					{
						this._vehiclesInBrokenPushingCycles.Add(vehicle3);
					}
				}
				Fix64 acceleration = VehicleMovementProcess.CalculateAcceleration(distanceToObstacle + obstacleOffset, vehicle3.CurrentFrame.speed, obstacleSpeed, obstacleAcceleration, desiredSpeed, maxAcceleration, this._constants.accelerationExponent, this._constants.maxDeceleration, this._constants.decelerationExponent);
				if (obstacleType == VehicleModel.ObstacleType.BlockingIntersection && blockingLane.roadChunk.IsControlled && distanceToObstacle < VehicleMovementProcess.VehicleLength * Fix64Consts.Two)
				{
					acceleration *= Fix64Consts.Two;
				}
				if (isBlockedByHotswappingLane)
				{
					Fix64 blockingAcceleration = VehicleMovementProcess.CalculateAcceleration(distanceToNextRoadChunk + (nextRoadChunk.isTileCorner ? Fix64.Zero : VehicleMovementProcess.IntersectionStoppingOffset), vehicle3.CurrentFrame.speed, Fix64.Zero, Fix64.Zero, desiredSpeed, maxAcceleration, this._constants.accelerationExponent, this._constants.maxDeceleration, this._constants.decelerationExponent);
					acceleration = Fix64.Min(acceleration, blockingAcceleration);
					if (obstacleType != VehicleModel.ObstacleType.BlockingIntersection || blockingLane != currentLane)
					{
						obstacleType = VehicleModel.ObstacleType.HotswappingLane;
						distanceToBlockingLane = distanceToNextRoadChunk;
						blockingLane = nextLane;
					}
				}
				else if (uncontrolledFreeIntersection != null)
				{
					Fix64 desiredIntersectionSpeed = this._constants.targetSpeedTowardsIntersections * desiredSpeed;
					Fix64 intersectionAcceleration = VehicleMovementProcess.CalculateAcceleration(distanceToUncontrolledIntersection, vehicle3.CurrentFrame.speed, desiredIntersectionSpeed, Fix64.Zero, desiredIntersectionSpeed, maxAcceleration, this._constants.accelerationExponent, this._constants.maxDeceleration, this._constants.decelerationExponentTowardsIntersections);
					acceleration = Fix64.Min(acceleration, intersectionAcceleration);
				}
				else if (upcomingCrossing != null)
				{
					Fix64 desiredCrossingSpeed = this._constants.targetSpeedTowardsCrossings * desiredSpeed;
					Fix64 crossingAcceleration = VehicleMovementProcess.CalculateAcceleration(distanceToUpcomingCrossing, vehicle3.CurrentFrame.speed, desiredCrossingSpeed, Fix64.Zero, desiredCrossingSpeed, maxAcceleration, this._constants.accelerationExponent, this._constants.maxDeceleration, this._constants.decelerationExponentTowardsCrossings);
					acceleration = Fix64.Min(acceleration, crossingAcceleration);
				}
				VehicleModel.Frame nextFrame = vehicle3.NextFrame;
				nextFrame.nearestObstacle = obstacleType;
				nextFrame.leadingVehicle = leadingVehicle;
				nextFrame.distanceToLeadingVehicle = distanceToLeadingVehicle;
				nextFrame.blockingLane = blockingLane;
				nextFrame.distanceToBlockingLane = distanceToBlockingLane;
				nextFrame.acceleration = acceleration;
				Fix64 minSpeed = (distanceToObstacle > VehicleMovementProcess.ClearanceForMinimumSpeed) ? VehicleMovementProcess.MinimumSpeedWithClearance : Fix64.Zero;
				nextFrame.speed = Fix64.Max(vehicle3.CurrentFrame.speed + vehicle3.NextFrame.acceleration * deltaTime, minSpeed);
				vehicle3.blockingVehicle = blockingVehicle;
				if (obstacleType == VehicleModel.ObstacleType.LeadingVehicle && nextFrame.speed < this._constants.minSpeedBeforePushingCycle && distanceToLeadingVehicle < VehicleMovementProcess.MinimumGap)
				{
					this._stuckVehicles.Add(vehicle3);
				}
				bool isVehicleReversing = false;
				Fix64 distanceTravelled = (vehicle3.CurrentFrame.speed + vehicle3.NextFrame.speed) * Fix64Consts.OneHalf * deltaTime;
				if (vehicle3.behaviorState == VehicleModel.BehaviorState.RealigningDriveway && vehicle3.targetDistanceAlongLastLane < vehicle3.CurrentFrame.distanceAlongLane)
				{
					isVehicleReversing = true;
					distanceTravelled = -distanceTravelled;
				}
				vehicle3.NextFrame.distanceAlongLane = vehicle3.CurrentFrame.distanceAlongLane + distanceTravelled;
				vehicle3.NextFrame.lane = currentLane;
				if (vehicle3.NextFrame.distanceAlongLane > currentLane.Length)
				{
					if (isVehicleStopping)
					{
						vehicle3.NextFrame.distanceAlongLane = currentLane.Length;
					}
					else
					{
						this._vehiclesMovingLanes.Add(vehicle3);
					}
				}
				else if ((pathCount == 0 && !isVehicleReversing && vehicle3.NextFrame.distanceAlongLane > vehicle3.targetDistanceAlongLastLane) || (isVehicleReversing && vehicle3.NextFrame.distanceAlongLane < vehicle3.targetDistanceAlongLastLane))
				{
					vehicle3.NextFrame.distanceAlongLane = vehicle3.targetDistanceAlongLastLane;
					vehicle3.NextFrame.speed = Fix64.Zero;
				}
				vehicle3.NotifyBehaviorChange();
				useAverageLaneSpeed = this._constants.useAverageLaneSpeedRatherThanMin;
			}
			foreach (VehicleModel vehicle2 in this._vehiclesMovingLanes)
			{
				vehicle2.NextFrame.distanceAlongLane -= vehicle2.CurrentFrame.lane.Length;
				if (vehicle2.path.Count > 0)
				{
					this.MoveVehicleToNewLane(vehicle2, vehicle2.path[0]);
				}
				else
				{
					vehicle2.NextFrame.distanceAlongLane = vehicle2.CurrentFrame.lane.Length;
				}
				if (vehicle2.path.Count == 0)
				{
					bool isVehicleReversing2 = vehicle2.behaviorState == VehicleModel.BehaviorState.RealigningDriveway && vehicle2.targetDistanceAlongLastLane < vehicle2.CurrentFrame.distanceAlongLane;
					if ((!isVehicleReversing2 && vehicle2.NextFrame.distanceAlongLane > vehicle2.targetDistanceAlongLastLane) || (isVehicleReversing2 && vehicle2.NextFrame.distanceAlongLane < vehicle2.targetDistanceAlongLastLane))
					{
						vehicle2.NextFrame.distanceAlongLane = vehicle2.targetDistanceAlongLastLane;
						vehicle2.NextFrame.speed = Fix64.Zero;
					}
				}
			}
			if (this._vehiclesInBrokenPushingCycles.Count > 0)
			{
				foreach (VehicleModel pushedVehicle in this._vehiclesInBrokenPushingCycles)
				{
					if (pushedVehicle.vehiclePushingInto != null)
					{
						VehicleModel pushingVehicle = pushedVehicle;
						do
						{
							VehicleModel vehiclePushingInto = pushingVehicle.vehiclePushingInto;
							pushingVehicle.vehiclePushingInto = null;
							pushingVehicle = vehiclePushingInto;
						}
						while (pushingVehicle != pushedVehicle && pushingVehicle != null);
					}
				}
			}
			if (this._stuckVehicles.Count > 0)
			{
				int frame = simulation.Scope.Get<Clock>().FrameCount;
				List<VehicleModel> blockingChain = new List<VehicleModel>();
				foreach (VehicleModel stuckVehicle in this._stuckVehicles)
				{
					VehicleModel nextVehicleInBlockingChain = stuckVehicle;
					Predicate<VehicleModel> <>9__0;
					while (nextVehicleInBlockingChain != null)
					{
						if (nextVehicleInBlockingChain.frameBlockingChainLastChecked == frame)
						{
							if (blockingChain.Contains(nextVehicleInBlockingChain))
							{
								List<VehicleModel> list = blockingChain;
								Predicate<VehicleModel> match;
								if ((match = <>9__0) == null)
								{
									match = (<>9__0 = ((VehicleModel vehicle) => vehicle == nextVehicleInBlockingChain));
								}
								int loopIndex = list.FindIndex(match);
								if (loopIndex > 0)
								{
									blockingChain.RemoveRange(0, loopIndex);
								}
								this.BreakCycle(blockingChain);
								break;
							}
							break;
						}
						else
						{
							blockingChain.Add(nextVehicleInBlockingChain);
							nextVehicleInBlockingChain.frameBlockingChainLastChecked = frame;
							nextVehicleInBlockingChain = nextVehicleInBlockingChain.blockingVehicle;
						}
					}
					blockingChain.Clear();
				}
			}
			this._vehiclesMovingLanes.Clear();
			this._vehiclesInBrokenPushingCycles.Clear();
			this._stuckVehicles.Clear();
		}

		// Token: 0x06001F9D RID: 8093 RVA: 0x0007C518 File Offset: 0x0007A718
		private void MoveVehicleToNewLane(VehicleModel vehicle, LaneModel newLane)
		{
			vehicle.isShovingIntoNextIntersection = false;
			if (vehicle.path.Count > 0)
			{
				vehicle.path[0].roadChunk.RemoveInboundVehicle(vehicle, vehicle.path[0], false);
				vehicle.path.RemoveAt(0);
				vehicle.pathLength -= newLane.Length;
			}
			LaneModel oldLane = vehicle.CurrentFrame.lane;
			if (oldLane != null)
			{
				if (!Diagnostics.Verify(oldLane != newLane, "Cannot move vehicle to the lane it is already on."))
				{
					return;
				}
				oldLane.RemoveVehicle(vehicle);
			}
			vehicle.NextFrame.lane = newLane;
			VehicleModel.Frame currentFrame = vehicle.CurrentFrame;
			if (currentFrame.lane == null)
			{
				currentFrame.lane = newLane;
			}
			if (vehicle.path.Count > 3)
			{
				LaneModel nextLane = this._pathfinder.GetPathNextLane(vehicle.path[1].PathfindingEndNodeId, vehicle.path[vehicle.path.Count - 1].PathfindingStartNodeId);
				if (nextLane != null && nextLane != vehicle.path[2])
				{
					vehicle.RequestPathfind(VehicleModel.PathfindUrgency.AsSoonAsPossible);
					vehicle.RequestReturnPathfind(VehicleModel.PathfindUrgency.AsSoonAsPossible);
				}
			}
			switch (vehicle.behaviorState)
			{
			case VehicleModel.BehaviorState.WaitingForDestination:
			case VehicleModel.BehaviorState.RealigningDriveway:
				Diagnostics.FailAssert("Vehicle shouldn't be moving lanes in state {0}.", new object[]
				{
					vehicle.behaviorState
				});
				break;
			case VehicleModel.BehaviorState.DrivingToDestination:
				if (vehicle.path.Count < 2)
				{
					vehicle.behaviorState = VehicleModel.BehaviorState.ParkingAtDestination;
					vehicle.destination.Carpark.vehiclesEntering.Add(vehicle);
				}
				break;
			case VehicleModel.BehaviorState.ParkingAtDestination:
			case VehicleModel.BehaviorState.ParkedAtDestination:
				break;
			case VehicleModel.BehaviorState.DrivingHome:
				if ((vehicle.path.Count == 0 && newLane == vehicle.house.DrivewayLane) || (vehicle.path.Count == 1 && vehicle.path[0] == vehicle.house.DrivewayLane))
				{
					Fix64 laneTarget = vehicle.house.HasWaitingVehicle ? vehicle.house.GetLaneDistanceAtBackOfDriveway(vehicle.house.DrivewayLane) : vehicle.house.GetLaneDistanceAtFrontOfDriveway(vehicle.house.DrivewayLane);
					vehicle.targetDistanceAlongLastLane = laneTarget;
				}
				if (newLane.roadChunk == vehicle.house.DrivewayLane.roadChunk && vehicle.path.Count == 0)
				{
					if (newLane == vehicle.house.DrivewayLane)
					{
						vehicle.behaviorState = VehicleModel.BehaviorState.WaitingForDestination;
						vehicle.house.waitingVehicles.Add(vehicle);
					}
					else
					{
						vehicle.targetDistanceAlongLastLane = vehicle.house.GetLaneDistanceAtCenterOfDriveway(newLane);
						vehicle.behaviorState = VehicleModel.BehaviorState.RealigningDriveway;
						vehicle.house.realigningVehicles.Add(vehicle);
					}
					vehicle.destination = null;
					vehicle.OnArrivedAtHouse();
				}
				break;
			default:
				Diagnostics.FailAssert("Vehicle in unknown state {0}.", new object[]
				{
					vehicle.behaviorState
				});
				break;
			}
			newLane.AddVehicle(vehicle);
			vehicle.OnMovedToNewLane(newLane, oldLane);
		}

		// Token: 0x06001F9E RID: 8094 RVA: 0x0007C7F4 File Offset: 0x0007A9F4
		private void BreakCycle(List<VehicleModel> cycle)
		{
			bool canShove = false;
			bool canPush = true;
			foreach (VehicleModel vehicleModel in cycle)
			{
				VehicleModel.ObstacleType obstacle = vehicleModel.NextFrame.nearestObstacle;
				canShove |= (obstacle == VehicleModel.ObstacleType.BlockingIntersection);
				canPush &= (obstacle == VehicleModel.ObstacleType.LeadingVehicle);
			}
			if (canShove)
			{
				foreach (VehicleModel stalledVehicle in cycle)
				{
					if (stalledVehicle.NextFrame.nearestObstacle == VehicleModel.ObstacleType.BlockingIntersection)
					{
						stalledVehicle.isShovingIntoNextIntersection = true;
					}
				}
				return;
			}
			if (canPush)
			{
				foreach (VehicleModel vehicleModel2 in cycle)
				{
					vehicleModel2.vehiclePushingInto = vehicleModel2.NextFrame.leadingVehicle;
				}
				return;
			}
			Diagnostics.Log.Channel log = VehicleMovementProcess.Log;
			string message = "Unable to shove or push vehicle cycle!\n{0}";
			object[] array = new object[1];
			array[0] = string.Join("\n", from vehicle in cycle
			select string.Format("[Vehicle Id={0}, Obstacle={1}]", vehicle.id, vehicle.NextFrame.nearestObstacle));
			log.Warn(message, array);
		}

		// Token: 0x06001F9F RID: 8095 RVA: 0x0007C93C File Offset: 0x0007AB3C
		public static Fix64 CalculateAcceleration(Fix64 currentGap, Fix64 currentSpeed, Fix64 leadingSpeed, Fix64 leadingAcceleration, Fix64 desiredSpeed, Fix64 maximumAcceleration, Fix64 accelerationExponent, Fix64 maximumDeceleration, Fix64 decelerationExponent)
		{
			return Fix64.FromRaw(VehicleMovementProcess.NativeCalculateAcceleration(currentGap.RawValue, currentSpeed.RawValue, leadingSpeed.RawValue, leadingAcceleration.RawValue, desiredSpeed.RawValue, maximumAcceleration.RawValue, accelerationExponent.RawValue, maximumDeceleration.RawValue, decelerationExponent.RawValue));
		}

		// Token: 0x06001FA0 RID: 8096 RVA: 0x0007C994 File Offset: 0x0007AB94
		public static Fix64 ReferenceCalculateAcceleration(Fix64 currentGap, Fix64 currentSpeed, Fix64 leadingSpeed, Fix64 leadingAcceleration, Fix64 desiredSpeed, Fix64 maximumAcceleration, Fix64 accelerationExponent, Fix64 maximumDeceleration, Fix64 decelerationExponent)
		{
			Fix64 gap = Fix64.Max(currentGap - VehicleMovementProcess.VehicleLength, VehicleMovementProcess.MinimumGap);
			Fix64 deltaSpeed = currentSpeed - leadingSpeed;
			Fix64 freeAcceleration = (currentSpeed <= desiredSpeed) ? (maximumAcceleration * (Fix64.One - Fix64.Pow(currentSpeed / desiredSpeed, accelerationExponent))) : (-maximumDeceleration * (Fix64.One - Fix64.Pow(desiredSpeed / currentSpeed, maximumAcceleration * decelerationExponent / maximumDeceleration)));
			Fix64 z = (VehicleMovementProcess.MinimumGap + Fix64.Max(Fix64.Zero, currentSpeed * VehicleMovementProcess.DesiredTimeGap + Fix64Consts.OneHalf * currentSpeed * deltaSpeed / Fix64.Sqrt(maximumAcceleration * maximumDeceleration))) / gap;
			Fix64 interactionAcceleration = maximumAcceleration * (Fix64.One - z * z);
			Fix64 accelerationIIDM;
			if (currentSpeed <= desiredSpeed)
			{
				accelerationIIDM = ((z >= Fix64.One) ? interactionAcceleration : (freeAcceleration * (Fix64.One - Fix64.Pow(z, Fix64Consts.Two * maximumAcceleration / Fix64.Max(freeAcceleration, VehicleMovementProcess.OneEMinus3)))));
			}
			else
			{
				accelerationIIDM = ((z >= Fix64.One) ? (freeAcceleration + interactionAcceleration) : freeAcceleration);
			}
			Fix64 omega = (currentSpeed - leadingSpeed >= Fix64.Zero) ? Fix64.One : Fix64.Zero;
			Fix64 x = Fix64Consts.Two * gap * leadingAcceleration;
			if (x == Fix64.Zero)
			{
				x = VehicleMovementProcess.OneEMinus5;
			}
			Fix64 accelerationCAH = (leadingSpeed * deltaSpeed < -x) ? (currentSpeed * currentSpeed * leadingAcceleration / (leadingSpeed * leadingSpeed - x)) : (leadingAcceleration - deltaSpeed * deltaSpeed * omega / (Fix64Consts.Two * gap));
			accelerationCAH = Fix64.Min(accelerationCAH, maximumAcceleration);
			Fix64 accelerationACC = (accelerationIIDM >= accelerationCAH) ? accelerationIIDM : ((Fix64.One - VehicleMovementProcess.CoolnessFactor) * accelerationIIDM + VehicleMovementProcess.CoolnessFactor * (accelerationCAH + maximumDeceleration * Fix64.Tanh((accelerationIIDM - accelerationCAH) / maximumDeceleration)));
			if (!(desiredSpeed < VehicleMovementProcess.OneEMinus5))
			{
				return Fix64.Max(-maximumDeceleration, accelerationACC);
			}
			return Fix64.Zero;
		}

		// Token: 0x06001FA1 RID: 8097
		[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl, EntryPoint = "CalculateAcceleration")]
		private static extern long NativeCalculateAcceleration(long currentGap, long currentSpeed, long leadingSpeed, long leadingAcceleration, long desiredSpeed, long maximumAcceleration, long accelerationExponent, long maximumDeceleration, long decelerationExponent);

		// Token: 0x04001A43 RID: 6723
		[Serialize(false, null)]
		private readonly List<VehicleModel> _vehiclesMovingLanes = new List<VehicleModel>();

		// Token: 0x04001A44 RID: 6724
		[Serialize(false, null)]
		private readonly List<VehicleModel> _vehiclesInBrokenPushingCycles = new List<VehicleModel>();

		// Token: 0x04001A45 RID: 6725
		[Serialize(false, null)]
		private readonly List<VehicleModel> _stuckVehicles = new List<VehicleModel>();

		// Token: 0x04001A46 RID: 6726
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("VehicleMovementProcess");

		// Token: 0x04001A47 RID: 6727
		[Dependency]
		private IScope _scope;

		// Token: 0x04001A48 RID: 6728
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001A49 RID: 6729
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x04001A4A RID: 6730
		public static readonly Fix64 DefaultDesiredSpeed = Fix64.FromRaw(12884901888L);

		// Token: 0x04001A4B RID: 6731
		public static readonly Fix64 DesiredTimeGap = Fix64.FromRaw(4294967296L);

		// Token: 0x04001A4C RID: 6732
		public static readonly Fix64 MinimumGap = Fix64.FromRaw(429496729L);

		// Token: 0x04001A4D RID: 6733
		public static readonly Fix64 VehicleLength = Fix64.FromRaw((long)((ulong)-1503238554));

		// Token: 0x04001A4E RID: 6734
		public static readonly Fix64 MaximumDeceleration = Fix64.FromRaw(6442450944L);

		// Token: 0x04001A4F RID: 6735
		private static readonly Fix64 CoolnessFactor = Fix64.FromRaw(858993459L);

		// Token: 0x04001A50 RID: 6736
		private static readonly Fix64 AccelerationExponent = Fix64.FromRaw(17179869184L);

		// Token: 0x04001A51 RID: 6737
		private static readonly Fix64 OneEMinus3 = Fix64.FromRaw(429496L);

		// Token: 0x04001A52 RID: 6738
		private static readonly Fix64 OneEMinus5 = Fix64.FromRaw(4294L);

		// Token: 0x04001A53 RID: 6739
		private static readonly Fix64 MinimumSpeedWithClearance = (Fix64)1E-08;

		// Token: 0x04001A54 RID: 6740
		private static readonly Fix64 ClearanceForMinimumSpeed = VehicleMovementProcess.MinimumGap + VehicleMovementProcess.VehicleLength * Fix64Consts.Two;

		// Token: 0x04001A55 RID: 6741
		private static readonly Fix64 MinimumDistanceToNonBlockingIntersection = VehicleMovementProcess.VehicleLength * Fix64Consts.Two;

		// Token: 0x04001A56 RID: 6742
		private static readonly Fix64 IntersectionStoppingOffset = VehicleMovementProcess.VehicleLength - VehicleMovementProcess.MinimumGap * Fix64Consts.OneHalf;

		// Token: 0x04001A57 RID: 6743
		private static readonly Fix64 TargetStoppingOffset = VehicleMovementProcess.VehicleLength + VehicleMovementProcess.MinimumGap;

		// Token: 0x04001A58 RID: 6744
		private static readonly ProfilerMarker Profiler_Step = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step");

		// Token: 0x04001A59 RID: 6745
		private static readonly ProfilerMarker Profiler_StepCalculateSpeed = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step.CalculateSpeed");

		// Token: 0x04001A5A RID: 6746
		private static readonly ProfilerMarker Profiler_StepCalculateSpeedCurrentLane = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step.CalculateSpeed.CurrentLane");

		// Token: 0x04001A5B RID: 6747
		private static readonly ProfilerMarker Profiler_StepCalculateSpeedNextLane = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step.CalculateSpeed.NextLane");

		// Token: 0x04001A5C RID: 6748
		private static readonly ProfilerMarker Profiler_StepCalculateSpeedCheckHotswapBlocking = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step.CalculateSpeed.CheckHotswapBlocking");

		// Token: 0x04001A5D RID: 6749
		private static readonly ProfilerMarker Profiler_StepCalculateSpeedMaxAcceleration = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step.CalculateSpeed.MaxAcceleration");

		// Token: 0x04001A5E RID: 6750
		private static readonly ProfilerMarker Profiler_StepCalculateSpeedUpdateDistance = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step.CalculateSpeed.UpdateDistance");

		// Token: 0x04001A5F RID: 6751
		private static readonly ProfilerMarker Profiler_StepMoveLane = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.Step.MoveLane");

		// Token: 0x04001A60 RID: 6752
		private static readonly ProfilerMarker Profiler_CalculateAcceleration = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.CalculateAcceleration");

		// Token: 0x04001A61 RID: 6753
		private static readonly ProfilerMarker Profiler_MoveToNewLane = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.MoveToNewLane");

		// Token: 0x04001A62 RID: 6754
		private static readonly ProfilerMarker Profiler_ClearPushingCycles = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.ClearPushingCycles");

		// Token: 0x04001A63 RID: 6755
		private static readonly ProfilerMarker Profiler_CheckCycles = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.CheckCycles");

		// Token: 0x04001A64 RID: 6756
		private static readonly ProfilerMarker Profiler_BreakCycle = new ProfilerMarker(ProfilerUtility.CategoryProcess, "VehicleMovementProcess.BreakCycle");
	}
}
