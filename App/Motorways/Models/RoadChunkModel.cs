using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using JetBrains.Annotations;
using Motorways.Processes;
using Server;
using Unity.Profiling;

namespace Motorways.Models
{
	// Token: 0x020004F6 RID: 1270
	public class RoadChunkModel : Model<EmptyModelFrame, IEmptyModelObserver>, IDeserializedHandler
	{
		// Token: 0x17000601 RID: 1537
		// (get) Token: 0x06002188 RID: 8584 RVA: 0x00085B9F File Offset: 0x00083D9F
		// (set) Token: 0x06002189 RID: 8585 RVA: 0x00085BA7 File Offset: 0x00083DA7
		[Serialize(true, null)]
		public TrainCrossingModel TrainCrossingModel { get; set; }

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x0600218A RID: 8586 RVA: 0x00085BB0 File Offset: 0x00083DB0
		// (set) Token: 0x0600218B RID: 8587 RVA: 0x00085BB8 File Offset: 0x00083DB8
		public TrafficLightModel TrafficLight
		{
			get
			{
				return this._trafficLightModel;
			}
			set
			{
				this._trafficLightModel = value;
				foreach (LaneModel laneModel in this.lanes)
				{
					foreach (LaneModel laneModel2 in laneModel.InboundLanes)
					{
						laneModel2.RecalculateSpeedLimit();
					}
				}
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x0600218C RID: 8588 RVA: 0x00085C48 File Offset: 0x00083E48
		public bool IsControlled
		{
			get
			{
				return this._trafficLightModel != null || (this.TrainCrossingModel == null && this.IsRoundabout);
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x0600218D RID: 8589 RVA: 0x00085C64 File Offset: 0x00083E64
		public bool IsRoundabout
		{
			get
			{
				using (List<LaneModel>.Enumerator enumerator = this.lanes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.connection.IsRoundabout)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x0600218E RID: 8590 RVA: 0x00085CC4 File Offset: 0x00083EC4
		public bool IsTrainCrossing
		{
			get
			{
				return this.TrainCrossingModel != null;
			}
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x00085CD0 File Offset: 0x00083ED0
		static RoadChunkModel()
		{
			RoadChunkModel.Log.IsMuted = true;
		}

		// Token: 0x06002190 RID: 8592 RVA: 0x00085D88 File Offset: 0x00083F88
		public override void Reset()
		{
			base.Reset();
			this.lanes.Clear();
			this.TrafficLight = null;
			this._laneSpeedLimitScale = Fix64Consts.One;
			this.isTileCorner = false;
			this.traversingVehicles.Clear();
			this._outboundDirections = default(TileDirectionBitfield);
			this.TrainCrossingModel = null;
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x00085DE0 File Offset: 0x00083FE0
		public bool AddInboundVehicle(VehicleModel vehicle, LaneModel chosenLane, int offset, bool returningInboundVehicle = false)
		{
			if (!Diagnostics.Verify(this.lanes.Contains(chosenLane), "Can't add a vehicle inbound to a lane arbitrated by a different road chunk."))
			{
				return false;
			}
			RoadChunkModel.InboundVehicle newInboundVehicle = this._scope.Get<RoadChunkModel.InboundVehicle>();
			newInboundVehicle.vehicle = vehicle;
			newInboundVehicle.chosenLane = chosenLane;
			newInboundVehicle.timestamp = this._clock.Time + (Fix64)((long)offset);
			if (!returningInboundVehicle)
			{
				this.inboundVehicles.Add(newInboundVehicle);
			}
			else
			{
				this.returningInboundVehicles.Add(newInboundVehicle);
			}
			chosenLane.hasBeenUsed = true;
			return true;
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x00085E64 File Offset: 0x00084064
		public bool RemoveInboundVehicle(VehicleModel vehicle, LaneModel lane, bool returningVehicle = false)
		{
			List<RoadChunkModel.InboundVehicle> inbounds = returningVehicle ? this.returningInboundVehicles : this.inboundVehicles;
			for (int inboundVehicleIndex = 0; inboundVehicleIndex < inbounds.Count; inboundVehicleIndex++)
			{
				RoadChunkModel.InboundVehicle inboundVehicle = inbounds[inboundVehicleIndex];
				if (inboundVehicle.vehicle == vehicle && inboundVehicle.chosenLane == lane)
				{
					this._scope.Release(inboundVehicle);
					inbounds.RemoveAt(inboundVehicleIndex);
					return true;
				}
			}
			Diagnostics.FailAssert("Failed to find inbound vehicle {0} for lane {1}!", new object[]
			{
				vehicle,
				lane
			});
			return false;
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x00085EE0 File Offset: 0x000840E0
		public void SortInboundVehicles()
		{
			this.traversingVehicles.Clear();
			if (this.inboundVehicles.Count == 0 || (this.lanes.Count <= 2 && this.TrainCrossingModel == null))
			{
				return;
			}
			foreach (LaneModel laneModel in this.lanes)
			{
				foreach (VehicleModel traversingVehicle in laneModel.Vehicles)
				{
					this.traversingVehicles.Add(traversingVehicle);
				}
			}
			int lowPriorityIndex = -1;
			int inboundVehicleCount = this.inboundVehicles.Count;
			for (int inboundIndex = 0; inboundIndex < inboundVehicleCount; inboundIndex++)
			{
				RoadChunkModel.InboundVehicle inboundVehicle = this.inboundVehicles[inboundIndex];
				bool flag = inboundVehicle.vehicle.IsCommittedToLane(inboundVehicle.chosenLane);
				if (flag && inboundVehicle.committedTimestamp < Fix64.Zero)
				{
					inboundVehicle.committedTimestamp = this._clock.Time;
				}
				VehicleModel vehicleModel;
				bool flag2 = (flag && !this.IsDirectionBlockedByTrafficLight(inboundVehicle.chosenLane.connection.input.direction, inboundVehicle.chosenLane.connection.output.direction) && !this.WouldInboundVehicleCollideWithTraversingVehicle(inboundVehicle, out vehicleModel, null)) || inboundVehicle.IsShoving;
				if (!flag2 && lowPriorityIndex < 0)
				{
					lowPriorityIndex = inboundIndex;
				}
				if (flag2 && lowPriorityIndex >= 0)
				{
					RoadChunkModel.InboundVehicle swapper = this.inboundVehicles[lowPriorityIndex];
					this.inboundVehicles[lowPriorityIndex] = this.inboundVehicles[inboundIndex];
					this.inboundVehicles[inboundIndex] = swapper;
					lowPriorityIndex++;
				}
			}
			LaneModel roundaboutLane = null;
			foreach (LaneModel lane in this.lanes)
			{
				if (lane.connection.IsRoundabout)
				{
					roundaboutLane = lane;
					break;
				}
			}
			RoadChunkModel.InboundVehicleDistanceComparer.roundaboutLane = roundaboutLane;
			if (lowPriorityIndex > 1)
			{
				this.inboundVehicles.Sort(0, lowPriorityIndex, RoadChunkModel.inboundVehicleDistanceComparer);
				return;
			}
			if (lowPriorityIndex < 0)
			{
				this.inboundVehicles.Sort(RoadChunkModel.inboundVehicleDistanceComparer);
			}
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x00086138 File Offset: 0x00084338
		public bool CanInboundVehicleEnter([NotNull] VehicleModel vehicle, out VehicleModel blockingVehicle, [CanBeNull] IntersectionEntryDecision decision = null)
		{
			blockingVehicle = null;
			RoadChunkModel.InboundVehicle inboundForVehicle = this.InboundVehicleForVehicle(vehicle);
			if (!Diagnostics.Verify(inboundForVehicle != null, "Can't find InboundVehicle for {0}.", vehicle))
			{
				return false;
			}
			if (decision != null)
			{
				decision.Initialize(inboundForVehicle);
			}
			LaneModel inboundChosenLane = inboundForVehicle.chosenLane;
			if (!Diagnostics.Verify(inboundChosenLane != null, "InboundVehicle for {0} does not have an assigned lane.", vehicle))
			{
				if (decision != null)
				{
					decision.SetVerdict(IntersectionEntryVerdict.NoReservedLane);
				}
				return false;
			}
			if (this.TrainCrossingModel != null)
			{
				TrainCrossingModel trainCrossingModel = vehicle.CurrentFrame.lane.roadChunk.TrainCrossingModel;
				if (trainCrossingModel == null || trainCrossingModel.SignalState != TrainSignalState.Closed)
				{
					if (this.TrainCrossingModel.SignalState == TrainSignalState.Closed)
					{
						Fix64 distanceToChunk = Fix64.Zero;
						distanceToChunk += vehicle.CurrentFrame.lane.Length - vehicle.CurrentFrame.distanceAlongLane;
						foreach (LaneModel pathLane in vehicle.path)
						{
							if (pathLane.roadChunk == this)
							{
								break;
							}
							distanceToChunk += pathLane.Length;
						}
						if (!this.isTileCorner || distanceToChunk > VehicleMovementProcess.VehicleLength)
						{
							if (decision != null)
							{
								decision.SetVerdict(IntersectionEntryVerdict.BlockedByUnsafeCrossing);
							}
							return false;
						}
						RoadChunkModel.Log.Info("Not stopping vehicle {0} at train crossing on chunk ? because distance to the crossing chunk is only {1} - less than vehicle length: {2}", new object[]
						{
							vehicle.id,
							distanceToChunk,
							VehicleMovementProcess.VehicleLength
						});
					}
					else if (vehicle.CurrentFrame.lane.state != RoadState.Mothballed)
					{
						foreach (LaneModel upcomingLane in vehicle.path)
						{
							foreach (VehicleModel traversingVehicle in upcomingLane.roadChunk.traversingVehicles)
							{
								if (traversingVehicle.CurrentFrame.lane.connection.IntersectsOtherConnection(upcomingLane.connection, false, false, false) && traversingVehicle.blockingVehicle != null)
								{
									Fix64 distanceToBlockingVehicle = Fix64.Zero;
									if (traversingVehicle.CurrentFrame.lane == traversingVehicle.blockingVehicle.CurrentFrame.lane)
									{
										distanceToBlockingVehicle += traversingVehicle.blockingVehicle.CurrentFrame.distanceAlongLane - traversingVehicle.CurrentFrame.distanceAlongLane;
									}
									else
									{
										distanceToBlockingVehicle += traversingVehicle.CurrentFrame.lane.Length - traversingVehicle.CurrentFrame.distanceAlongLane;
										foreach (LaneModel laneModel in traversingVehicle.path)
										{
											if (laneModel == traversingVehicle.blockingVehicle.CurrentFrame.lane)
											{
												distanceToBlockingVehicle += traversingVehicle.blockingVehicle.CurrentFrame.distanceAlongLane;
												break;
											}
											distanceToBlockingVehicle += laneModel.Length;
										}
									}
									if (distanceToBlockingVehicle < (Fix64)3L * VehicleMovementProcess.VehicleLength || traversingVehicle.CurrentFrame.speed < RoadChunkModel.CarStoppedSpeedThreshold)
									{
										RoadChunkModel.Log.Info("Vehicle {0} blocked from entering train crossing because vehicle {1} is currently on or just after the crossing and is blocked by vehicle {2} which is only {3} in front of it", new object[]
										{
											vehicle.id,
											traversingVehicle.id,
											traversingVehicle.blockingVehicle.id,
											distanceToBlockingVehicle
										});
										if (decision != null)
										{
											decision.SetVerdict(IntersectionEntryVerdict.BlockedByCongestedCrossing);
										}
										return false;
									}
								}
							}
							if (upcomingLane.roadChunk.TrainCrossingModel == null)
							{
								break;
							}
						}
					}
				}
			}
			if (this.lanes.Count <= 2)
			{
				if (decision != null)
				{
					decision.SetVerdict(IntersectionEntryVerdict.NoIntersectingLanes);
				}
				return true;
			}
			if (this.IsDirectionBlockedByTrafficLight(inboundChosenLane.connection.input.direction, inboundChosenLane.connection.output.direction))
			{
				if (decision != null)
				{
					decision.SetVerdict(IntersectionEntryVerdict.BlockedByTrafficLight);
				}
				return false;
			}
			if (vehicle.isShovingIntoNextIntersection && vehicle.path.Count > 0 && vehicle.path[0] == inboundChosenLane)
			{
				if (decision != null)
				{
					decision.SetVerdict(IntersectionEntryVerdict.Shoved);
				}
				return true;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.MaximumWaitTimeAtIntersections) && this._clock.Time - inboundForVehicle.committedTimestamp > this._constants.MaximumTimeToWaitAtIntersection)
			{
				if (decision != null)
				{
					decision.SetVerdict(IntersectionEntryVerdict.ExceededMaximumWaitTime);
				}
				return true;
			}
			if (this.WouldInboundVehicleCollideWithTraversingVehicle(inboundForVehicle, out blockingVehicle, decision))
			{
				if (decision != null)
				{
					decision.SetVerdict(IntersectionEntryVerdict.BlockedByTraversingVehicle);
				}
				return false;
			}
			foreach (RoadChunkModel.InboundVehicle inboundVehicle in this.inboundVehicles)
			{
				if (inboundVehicle.vehicle == vehicle)
				{
					if (decision != null)
					{
						decision.SetVerdict(IntersectionEntryVerdict.NoBlockingVehicles);
					}
					return true;
				}
				bool allowCrossingInFrontOfOther = inboundVehicle.vehicle.CurrentFrame.nearestObstacle == VehicleModel.ObstacleType.HotswappingLane || inboundVehicle.vehicle.CurrentFrame.speed < RoadChunkModel.CarStoppedSpeedThreshold;
				if (inboundForVehicle.chosenLane.connection.IntersectsOtherConnection(inboundVehicle.chosenLane.connection, false, true, allowCrossingInFrontOfOther))
				{
					blockingVehicle = inboundVehicle.vehicle;
					if (decision != null)
					{
						decision.SetInboundVehicleInfluence(inboundVehicle, IntersectionEntryVehicleInfluence.ReservedIntersectingLane);
					}
					if (decision != null)
					{
						decision.SetVerdict(IntersectionEntryVerdict.BlockedByInboundVehicle);
					}
					return false;
				}
				if (decision != null)
				{
					IntersectionEntryVehicleInfluence inboundVehicleInfluence = IntersectionEntryVehicleInfluence.ReservedNonIntersectingLane;
					if (allowCrossingInFrontOfOther && inboundForVehicle.chosenLane.connection.IntersectsOtherConnection(inboundVehicle.chosenLane.connection, false, true, false))
					{
						inboundVehicleInfluence = ((inboundVehicle.vehicle.CurrentFrame.nearestObstacle == VehicleModel.ObstacleType.HotswappingLane) ? IntersectionEntryVehicleInfluence.BlockedByHotswap : IntersectionEntryVehicleInfluence.Stopped);
					}
					decision.SetInboundVehicleInfluence(inboundVehicle, inboundVehicleInfluence);
				}
			}
			return this.inboundVehicles[0].vehicle == vehicle;
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x00086788 File Offset: 0x00084988
		public bool WouldInboundVehicleCollideWithTraversingVehicle(RoadChunkModel.InboundVehicle inboundVehicle, out VehicleModel collidingVehicle, IntersectionEntryDecision decision = null)
		{
			bool canIgnoreMostCollisions = this._constants.greenLightsIgnoreCollisions && this.IsDirectionGreenFromTrafficLight(inboundVehicle.chosenLane.connection.input.direction);
			bool hasCalculatedIfInboundVehicleHasSpace = false;
			bool inboundVehicleHasSpace = false;
			foreach (VehicleModel traversingVehicle in this.traversingVehicles)
			{
				if (traversingVehicle.id == inboundVehicle.vehicle.id)
				{
					if (decision != null)
					{
						decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.Self);
					}
				}
				else if (traversingVehicle.path == null || traversingVehicle.path.Count == 0)
				{
					if (decision != null)
					{
						decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.Parked);
					}
				}
				else if (this._constants.TreatStraightRoundaboutEntrancesAsNotRoundabouts && this.HasVehicleNotYetEnteredRoundabout(traversingVehicle) && inboundVehicle.chosenLane != traversingVehicle.CurrentFrame.lane)
				{
					if (decision != null)
					{
						decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.SeparateRoundaboutEntrance);
					}
				}
				else
				{
					if (this.SharesExitToIntersection(traversingVehicle.CurrentFrame.lane, inboundVehicle.chosenLane))
					{
						if (!hasCalculatedIfInboundVehicleHasSpace)
						{
							inboundVehicleHasSpace = this.DoesVehicleHaveSpace(inboundVehicle.vehicle);
							hasCalculatedIfInboundVehicleHasSpace = true;
						}
						if (!inboundVehicleHasSpace)
						{
							collidingVehicle = traversingVehicle;
							if (decision != null)
							{
								decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.SameExitNoSpace);
							}
							return true;
						}
					}
					else if (traversingVehicle.CurrentFrame.distanceAlongLane > traversingVehicle.CurrentFrame.lane.Length - VehicleMovementProcess.VehicleLength)
					{
						if (decision != null)
						{
							decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.AlmostThroughLane);
							continue;
						}
						continue;
					}
					if (!canIgnoreMostCollisions && traversingVehicle.CurrentFrame.lane != inboundVehicle.chosenLane && traversingVehicle.CurrentFrame.lane.connection.IntersectsOtherConnection(inboundVehicle.chosenLane.connection, false, true, false))
					{
						collidingVehicle = traversingVehicle;
						if (decision != null)
						{
							decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.OnIntersectingLane);
						}
						return true;
					}
					if (decision != null)
					{
						if (canIgnoreMostCollisions && traversingVehicle.CurrentFrame.lane != inboundVehicle.chosenLane && traversingVehicle.CurrentFrame.lane.connection.IntersectsOtherConnection(inboundVehicle.chosenLane.connection, false, true, false))
						{
							decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.OnIgnoredIntersectingLane);
						}
						else
						{
							decision.SetTraversingVehicleInfluence(traversingVehicle, IntersectionEntryVehicleInfluence.None);
						}
					}
				}
			}
			collidingVehicle = null;
			return false;
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x000869E0 File Offset: 0x00084BE0
		public bool CanTraversingVehicleContinue(VehicleModel vehicle)
		{
			return !this._constants.TreatStraightRoundaboutEntrancesAsNotRoundabouts || !this.IsRoundabout || this.inboundVehicles.Count == 0 || !Diagnostics.Verify(this.lanes.Contains(vehicle.CurrentFrame.lane), "This vehicle isn't actually in this road chunk!") || !this.HasVehicleNotYetEnteredRoundabout(vehicle) || !this.inboundVehicles[0].chosenLane.connection.IsRoundabout || !this.inboundVehicles[0].chosenLane.InboundLanes.Contains(this.inboundVehicles[0].vehicle.CurrentFrame.lane);
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x00086A98 File Offset: 0x00084C98
		private bool HasVehicleNotYetEnteredRoundabout(VehicleModel vehicle)
		{
			return this.IsRoundabout && !TileUtilities.IsDirectionDiagonal(vehicle.CurrentFrame.lane.connection.input.direction) && vehicle.CurrentFrame.lane.connection.input.type != RoadType.Roundabout && vehicle.CurrentFrame.lane.connection.output.type == RoadType.Roundabout && vehicle.CurrentFrame.distanceAlongLane < vehicle.CurrentFrame.lane.Length * this._constants.PercentageOfStraightLanesIntoRoundaboutsToCountOutside;
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x00086B40 File Offset: 0x00084D40
		private bool DoesVehicleHaveSpace(VehicleModel inputVehicle)
		{
			VehicleModel currentVehicle = inputVehicle;
			Fix64 cumulativeSpace = Fix64.Zero;
			int vehicleCount = 0;
			while (vehicleCount < 5 && currentVehicle != null)
			{
				VehicleModel.Frame currentFrame = currentVehicle.CurrentFrame;
				switch (currentFrame.nearestObstacle)
				{
				case VehicleModel.ObstacleType.None:
					return true;
				case VehicleModel.ObstacleType.Target:
					cumulativeSpace += currentVehicle.targetDistanceAlongLastLane - currentFrame.distanceAlongLane - VehicleMovementProcess.VehicleLength;
					currentVehicle = null;
					break;
				case VehicleModel.ObstacleType.LeadingVehicle:
				{
					VehicleModel leadingVehicle = currentFrame.leadingVehicle;
					Fix64 distanceToLeadingVehicle = currentFrame.distanceToLeadingVehicle;
					LaneModel leadingVehicleLane = leadingVehicle.CurrentFrame.lane;
					if (leadingVehicleLane.roadChunk == this)
					{
						distanceToLeadingVehicle -= leadingVehicleLane.Length - VehicleMovementProcess.VehicleLength;
					}
					cumulativeSpace += Fix64.Max(distanceToLeadingVehicle - VehicleMovementProcess.VehicleLength, Fix64.Zero);
					currentVehicle = leadingVehicle;
					break;
				}
				case VehicleModel.ObstacleType.BlockingIntersection:
				case VehicleModel.ObstacleType.HotswappingLane:
					cumulativeSpace += currentFrame.distanceToBlockingLane;
					currentVehicle = null;
					break;
				}
				if (cumulativeSpace > VehicleMovementProcess.VehicleLength * Fix64Consts.Two)
				{
					return true;
				}
				vehicleCount++;
			}
			return false;
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x00086C49 File Offset: 0x00084E49
		private bool SharesExitToIntersection(LaneModel laneA, LaneModel laneB)
		{
			return laneA.connection.output.direction == laneB.connection.output.direction;
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x00086C70 File Offset: 0x00084E70
		private RoadChunkModel.InboundVehicle InboundVehicleForVehicle(VehicleModel vehicle)
		{
			RoadChunkModel.InboundVehicle earliestVehicle = null;
			foreach (RoadChunkModel.InboundVehicle inboundVehicle in this.inboundVehicles)
			{
				if (inboundVehicle.vehicle == vehicle && (earliestVehicle == null || earliestVehicle.timestamp > inboundVehicle.timestamp))
				{
					earliestVehicle = inboundVehicle;
				}
			}
			return earliestVehicle;
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x00086CE0 File Offset: 0x00084EE0
		public bool DoesLaneHaveAnyInboundVehicles(LaneModel laneModel)
		{
			using (List<RoadChunkModel.InboundVehicle>.Enumerator enumerator = this.inboundVehicles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.chosenLane == laneModel)
					{
						return true;
					}
				}
			}
			using (List<RoadChunkModel.InboundVehicle>.Enumerator enumerator = this.returningInboundVehicles.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.chosenLane == laneModel)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x00086D80 File Offset: 0x00084F80
		public bool DoesLaneHaveAnyCommittedVehicles(LaneModel laneModel)
		{
			foreach (RoadChunkModel.InboundVehicle inboundVehicle in this.inboundVehicles)
			{
				if (inboundVehicle.chosenLane == laneModel)
				{
					return inboundVehicle.vehicle.IsCommittedToLane(laneModel);
				}
			}
			return false;
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x00086DE8 File Offset: 0x00084FE8
		public bool HasLaneForConnection(RoadTileConnection connection)
		{
			using (List<LaneModel>.Enumerator enumerator = this.lanes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.connection.Equals(connection))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x00086E48 File Offset: 0x00085048
		public LaneModel AddLane(RoadTileConnection connection, RoadTileDefinition definition, RoadState initialState, Vector2Fixed position, bool isEndpointLane)
		{
			RoadChunkModel.Log.Info("Adding {0} lane for connection {1} at position {2}.", new object[]
			{
				initialState,
				connection,
				position
			});
			foreach (LaneModel lane in this.lanes)
			{
				if (lane.connection.Equals(connection) && !Diagnostics.Verify(!lane.connection.Equals(connection), "Please don't add a new {0} lane for with the same connection as {1}.", initialState, lane))
				{
					return lane;
				}
			}
			LaneModel newLane = this._scope.Get<LaneModel>();
			newLane.Initialize(this, definition, connection, position, isEndpointLane);
			newLane.state = initialState;
			this.AddLaneModel(newLane);
			return newLane;
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x00086F24 File Offset: 0x00085124
		public LaneModel AddBespokeLane(RoadTileConnection connection, List<Vector2Fixed> path, RoadState initialState, bool isCarparkLane = false, bool isEndpointLane = false)
		{
			LaneModel newLane = this._scope.Get<LaneModel>();
			newLane.Initialize(this, connection, path, isEndpointLane, isCarparkLane);
			newLane.state = initialState;
			this.AddLaneModel(newLane);
			return newLane;
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x00086F5C File Offset: 0x0008515C
		public bool RemoveLane(LaneModel lane)
		{
			if (!Diagnostics.Verify(this.lanes.Contains(lane), "Unable to remove lane from a road chunk it is not part of."))
			{
				return false;
			}
			if (lane.isTemporary)
			{
				this._tilemap.TemporaryLanes.Remove(lane);
			}
			else if (this.TrainCrossingModel != null)
			{
				this._simulation.RemoveModel(this.TrainCrossingModel);
				this.TrainCrossingModel = null;
			}
			lane.RemoveInboundAndOutboundLanes();
			this.lanes.Remove(lane);
			this._simulation.RemoveModel(lane);
			TrafficLightModel trafficLight = this.TrafficLight;
			if (trafficLight != null)
			{
				trafficLight.OnLanesChanged();
			}
			return true;
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x00086FF4 File Offset: 0x000851F4
		public void RemoveAllLanes()
		{
			for (int laneIndex = this.lanes.Count - 1; laneIndex >= 0; laneIndex--)
			{
				LaneModel lane = this.lanes[laneIndex];
				this.RemoveLane(lane);
			}
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x00087030 File Offset: 0x00085230
		public void ConnectInboundLane(LaneModel inboundLane)
		{
			foreach (LaneModel outboundLane in this.GetLanesEnteringFromDirection(TileUtilities.GetOppositeDirection(inboundLane.connection.output.direction)))
			{
				outboundLane.AddInboundLane(inboundLane);
				inboundLane.AddOutboundLane(outboundLane);
			}
		}

		// Token: 0x060021A3 RID: 8611 RVA: 0x000870A0 File Offset: 0x000852A0
		public void ConnectOutboundLane(LaneModel outboundLane)
		{
			foreach (LaneModel inboundLane in this.GetLanesExitingInDirection(TileUtilities.GetOppositeDirection(outboundLane.connection.input.direction)))
			{
				inboundLane.AddOutboundLane(outboundLane);
				outboundLane.AddInboundLane(inboundLane);
			}
		}

		// Token: 0x060021A4 RID: 8612 RVA: 0x00087110 File Offset: 0x00085310
		public TileDirectionBitfield GetInboundDirections()
		{
			List<TileDirection> directions = new List<TileDirection>();
			foreach (LaneModel lane in this.lanes)
			{
				directions.Add(lane.connection.input.direction);
			}
			return new TileDirectionBitfield(directions);
		}

		// Token: 0x060021A5 RID: 8613 RVA: 0x00087180 File Offset: 0x00085380
		public List<LaneModel> GetLanesConnectedToDirection(RoadState states, TileDirection direction)
		{
			TileDirectionBitfield directions = default(TileDirectionBitfield);
			directions[direction] = true;
			return this.GetLanesConnectedToDirections(states, directions);
		}

		// Token: 0x060021A6 RID: 8614 RVA: 0x000871A8 File Offset: 0x000853A8
		public List<LaneModel> GetLanesConnectedToDirections(RoadState states, TileDirectionBitfield directions)
		{
			List<LaneModel> filteredLanes = new List<LaneModel>();
			foreach (LaneModel lane in this.lanes)
			{
				if ((lane.state & states) == lane.state && (directions[lane.connection.input.direction] || directions[lane.connection.output.direction]))
				{
					filteredLanes.Add(lane);
				}
			}
			return filteredLanes;
		}

		// Token: 0x060021A7 RID: 8615 RVA: 0x00087244 File Offset: 0x00085444
		public List<LaneModel> GetLanesEnteringFromDirection(TileDirection direction)
		{
			List<LaneModel> enteringLanes = new List<LaneModel>();
			foreach (LaneModel lane in this.lanes)
			{
				if (lane.connection.input.direction == direction)
				{
					enteringLanes.Add(lane);
				}
			}
			return enteringLanes;
		}

		// Token: 0x060021A8 RID: 8616 RVA: 0x000872B4 File Offset: 0x000854B4
		public List<LaneModel> GetLanesExitingInDirection(TileDirection direction)
		{
			List<LaneModel> exitingLanes = new List<LaneModel>();
			foreach (LaneModel lane in this.lanes)
			{
				if (lane.connection.output.direction == direction)
				{
					exitingLanes.Add(lane);
				}
			}
			return exitingLanes;
		}

		// Token: 0x060021A9 RID: 8617 RVA: 0x00087324 File Offset: 0x00085524
		public int GetNumberOfRoadsInIntersectionForSlowingVehicles()
		{
			TileDirectionBitfield directionsWithValidRoads = default(TileDirectionBitfield);
			int validRoadCount = 0;
			foreach (LaneModel lane in this.lanes)
			{
				TileDirection outboundDirection = lane.connection.output.direction;
				if (!directionsWithValidRoads[outboundDirection] && (lane.state != RoadState.Mothballed || !lane.connection.IsUTurn))
				{
					if (this._constants.IgnoreHousesForIntersectionSlowDown)
					{
						bool directionConnectsToHouse = false;
						if (TileUtilities.IsDirectionDiagonal(outboundDirection))
						{
							using (List<LaneModel>.Enumerator enumerator2 = lane.OutboundLanes.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									LaneModel tileCornerLane = enumerator2.Current;
									if (tileCornerLane.connection.output.direction == outboundDirection && tileCornerLane.OutboundLanes.Count == 1 && tileCornerLane.OutboundLanes[0].connection.IsUTurn && tileCornerLane.OutboundLanes[0].connection.input.type == RoadType.Driveway)
									{
										directionConnectsToHouse = true;
										break;
									}
								}
								goto IL_157;
							}
							goto IL_10E;
						}
						goto IL_10E;
						IL_157:
						if (!directionConnectsToHouse)
						{
							goto IL_15B;
						}
						continue;
						IL_10E:
						directionConnectsToHouse = (lane.OutboundLanes.Count == 1 && lane.OutboundLanes[0].connection.IsUTurn && lane.OutboundLanes[0].connection.input.type == RoadType.Driveway);
						goto IL_157;
					}
					IL_15B:
					if ((!this._constants.IgnoreDestinationsForIntersectionSlowDown || !Diagnostics.Verify(lane.OutboundLanes.Count > 0) || lane.OutboundLanes[0].connection.output.type != RoadType.Carpark) && lane.connection.output.type != RoadType.Roundabout && lane.connection.input.type != RoadType.Roundabout)
					{
						directionsWithValidRoads[outboundDirection] = true;
						validRoadCount++;
					}
				}
			}
			return validRoadCount;
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x00087554 File Offset: 0x00085754
		public IEnumerable<RoadChunkModel.InboundVehicle> InboundVehiclesEnteringFromDirection(TileDirection direction, Fix64 withinDistance)
		{
			foreach (RoadChunkModel.InboundVehicle inboundVehicle in this.inboundVehicles)
			{
				if (inboundVehicle.vehicle.IsCommittedToLane(inboundVehicle.chosenLane) && inboundVehicle.chosenLane.connection.input.direction == direction)
				{
					if (withinDistance <= Fix64.Zero)
					{
						yield return inboundVehicle;
					}
					else if (inboundVehicle.vehicle.DistanceToLane(inboundVehicle.chosenLane) < withinDistance)
					{
						yield return inboundVehicle;
					}
				}
			}
			List<RoadChunkModel.InboundVehicle>.Enumerator enumerator = default(List<RoadChunkModel.InboundVehicle>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x00087574 File Offset: 0x00085774
		public int NumberOfCarsEnteringFromDirection(TileDirection direction, bool ignoreBlockedVehicles, Fix64 withinDistance)
		{
			int count = 0;
			foreach (RoadChunkModel.InboundVehicle inboundVehicle in this.inboundVehicles)
			{
				if (inboundVehicle.vehicle.IsCommittedToLane(inboundVehicle.chosenLane))
				{
					if (ignoreBlockedVehicles)
					{
						if (inboundVehicle.vehicle.CurrentFrame.nearestObstacle == VehicleModel.ObstacleType.HotswappingLane)
						{
							break;
						}
						LaneModel blockingLane = inboundVehicle.vehicle.CurrentFrame.blockingLane;
						if (((blockingLane != null) ? blockingLane.roadChunk : null) == this)
						{
							break;
						}
					}
					if (inboundVehicle.chosenLane.connection.input.direction == direction)
					{
						if (withinDistance <= Fix64.Zero)
						{
							count++;
						}
						else if (inboundVehicle.vehicle.DistanceToLane(inboundVehicle.chosenLane) < withinDistance)
						{
							count++;
						}
					}
				}
			}
			return count;
		}

		// Token: 0x060021AC RID: 8620 RVA: 0x0008765C File Offset: 0x0008585C
		public void SetLaneSpeedLimitScale(Fix64 newSpeedScale)
		{
			this._laneSpeedLimitScale = newSpeedScale;
			foreach (LaneModel laneModel in this.lanes)
			{
				laneModel.SetSpeedLimitScale(this._laneSpeedLimitScale);
			}
		}

		// Token: 0x060021AD RID: 8621 RVA: 0x000876BC File Offset: 0x000858BC
		public void SetSpeedLimitScaleOnDirections(TileDirectionBitfield directions, Fix64 newSpeedScale, bool resetOtherDirections)
		{
			foreach (LaneModel lane in this.lanes)
			{
				if (directions[lane.connection.output.direction])
				{
					lane.SetSpeedLimitScale(newSpeedScale);
				}
				else if (resetOtherDirections)
				{
					lane.SetSpeedLimitScale(Fix64.One);
				}
			}
		}

		// Token: 0x060021AE RID: 8622 RVA: 0x00087738 File Offset: 0x00085938
		public void UpdateLaneCosts()
		{
			foreach (LaneModel lane in this.lanes)
			{
				if (lane.PathfindingStartNodeId != -1 && lane.PathfindingEndNodeId != -1)
				{
					lane.UpdateLaneCost(lane.PathfindingCost);
				}
			}
		}

		// Token: 0x060021AF RID: 8623 RVA: 0x000877A4 File Offset: 0x000859A4
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			foreach (RoadChunkModel.InboundVehicle inboundVehicle in this.inboundVehicles)
			{
				scope.Release(inboundVehicle);
			}
			this.inboundVehicles.Clear();
			foreach (RoadChunkModel.InboundVehicle returningInboundVehicle in this.returningInboundVehicles)
			{
				scope.Release(returningInboundVehicle);
			}
			this.returningInboundVehicles.Clear();
		}

		// Token: 0x060021B0 RID: 8624 RVA: 0x00087858 File Offset: 0x00085A58
		public void OnDeserialized(IScope context)
		{
			foreach (LaneModel laneModel in this.lanes)
			{
				laneModel.roadChunk = this;
				laneModel.SetSpeedLimitScale(this._laneSpeedLimitScale);
			}
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x000878B8 File Offset: 0x00085AB8
		private void AddLaneModel(LaneModel newLane)
		{
			newLane.SetSpeedLimitScale(this._laneSpeedLimitScale);
			this.lanes.Add(newLane);
			this._simulation.AddModel(newLane);
			if (this.TrafficLight != null)
			{
				this.TrafficLight.OnLanesChanged();
			}
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x000878F4 File Offset: 0x00085AF4
		public TileDirectionBitfield GetOutboundDirections()
		{
			if (this._outboundDirections.Count != this.lanes.Count)
			{
				List<TileDirection> directions = new List<TileDirection>();
				foreach (LaneModel lane in this.lanes)
				{
					directions.Add(lane.connection.output.direction);
				}
				this._outboundDirections = new TileDirectionBitfield(directions);
			}
			return this._outboundDirections;
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x00087988 File Offset: 0x00085B88
		private bool IsDirectionGreenFromTrafficLight(TileDirection direction)
		{
			return this.TrafficLight != null && !this._trafficLightModel.BlockedLanes[direction];
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x000879B8 File Offset: 0x00085BB8
		public bool ConnectionCrossesLane(TileDirection directionEnteringIntersection, TileDirection directionExitingIntersection)
		{
			if (this._constants.americanRedLightRules)
			{
				TileDirectionBitfield currentDirections = this.GetOutboundDirections();
				for (int rotationIndex = 1; rotationIndex <= 3; rotationIndex++)
				{
					TileDirection outDirection = TileUtilities.GetRotatedDirection(directionEnteringIntersection, -rotationIndex);
					if (currentDirections[outDirection])
					{
						return outDirection != directionExitingIntersection;
					}
				}
				return true;
			}
			return true;
		}

		// Token: 0x060021B5 RID: 8629 RVA: 0x00087A04 File Offset: 0x00085C04
		private bool IsDirectionBlockedByTrafficLight(TileDirection directionEnteringIntersection, TileDirection directionExitingIntersection)
		{
			return this.TrafficLight != null && this._trafficLightModel.BlockedLanes[directionEnteringIntersection] && this.ConnectionCrossesLane(directionEnteringIntersection, directionExitingIntersection);
		}

		// Token: 0x060021B6 RID: 8630 RVA: 0x00087A3B File Offset: 0x00085C3B
		public RoadChunkModel() : base(1)
		{
		}

		// Token: 0x04001B9C RID: 7068
		public readonly List<LaneModel> lanes = new List<LaneModel>();

		// Token: 0x04001B9D RID: 7069
		private Fix64 _laneSpeedLimitScale = Fix64Consts.One;

		// Token: 0x04001B9E RID: 7070
		private static readonly Fix64 CarStoppedSpeedThreshold = (Fix64)0.001f;

		// Token: 0x04001B9F RID: 7071
		private static readonly RoadChunkModel.InboundVehicleDistanceComparer inboundVehicleDistanceComparer = new RoadChunkModel.InboundVehicleDistanceComparer();

		// Token: 0x04001BA0 RID: 7072
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001BA1 RID: 7073
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001BA2 RID: 7074
		public bool isTileCorner;

		// Token: 0x04001BA4 RID: 7076
		private TrafficLightModel _trafficLightModel;

		// Token: 0x04001BA5 RID: 7077
		public readonly List<RoadChunkModel.InboundVehicle> inboundVehicles = new List<RoadChunkModel.InboundVehicle>();

		// Token: 0x04001BA6 RID: 7078
		public readonly List<RoadChunkModel.InboundVehicle> returningInboundVehicles = new List<RoadChunkModel.InboundVehicle>();

		// Token: 0x04001BA7 RID: 7079
		[Serialize(false, null)]
		public readonly List<VehicleModel> traversingVehicles = new List<VehicleModel>();

		// Token: 0x04001BA8 RID: 7080
		private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("RoadChunkModel");

		// Token: 0x04001BA9 RID: 7081
		[Dependency]
		private IScope _scope;

		// Token: 0x04001BAA RID: 7082
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001BAB RID: 7083
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001BAC RID: 7084
		private TileDirectionBitfield _outboundDirections;

		// Token: 0x04001BAD RID: 7085
		private static readonly ProfilerMarker Profiler_GetNumberOfRoadsInIntersectionForSlowingVehicles = new ProfilerMarker(ProfilerUtility.CategoryModel, "RoadChunkModel.GetNumberOfRoadsInIntersectionForSlowingVehicles");

		// Token: 0x04001BAE RID: 7086
		private static readonly ProfilerMarker Profiler_SortInboundVehicles = new ProfilerMarker(ProfilerUtility.CategoryModel, "RoadChunkModel.SortInboundVehicles");

		// Token: 0x04001BAF RID: 7087
		private static readonly ProfilerMarker Profiler_SortInboundVehiclesSorting = new ProfilerMarker(ProfilerUtility.CategoryModel, "RoadChunkModel.SortInboundVehicles.Sorting");

		// Token: 0x04001BB0 RID: 7088
		private static readonly ProfilerMarker Profiler_CanInboundVehicleEnter = new ProfilerMarker(ProfilerUtility.CategoryModel, "RoadChunkModel.CanInboundVehicleEnter");

		// Token: 0x04001BB1 RID: 7089
		private static readonly ProfilerMarker Profiler_InboundVehicleCollidesWithTraversingVehicle = new ProfilerMarker(ProfilerUtility.CategoryModel, "RoadChunkModel.InboundVehicleCollidesWithTraversingVehicle");

		// Token: 0x04001BB2 RID: 7090
		private static readonly ProfilerMarker Profiler_VehicleHasSpace = new ProfilerMarker(ProfilerUtility.CategoryModel, "RoadChunkModel.VehicleHasSpace");

		// Token: 0x020004F7 RID: 1271
		[Factory.Serializable(1)]
		public class InboundVehicle : IReusable
		{
			// Token: 0x17000606 RID: 1542
			// (get) Token: 0x060021B7 RID: 8631 RVA: 0x00087A7B File Offset: 0x00085C7B
			public bool IsShoving
			{
				get
				{
					return this.vehicle.isShovingIntoNextIntersection && this.vehicle.path.Count > 0 && this.vehicle.path[0] == this.chosenLane;
				}
			}

			// Token: 0x060021B8 RID: 8632 RVA: 0x00087AB8 File Offset: 0x00085CB8
			public void Reset()
			{
				this.vehicle = null;
				this.chosenLane = null;
				this.timestamp = Fix64.Zero;
				this.committedTimestamp = -Fix64.One;
			}

			// Token: 0x04001BB3 RID: 7091
			public VehicleModel vehicle;

			// Token: 0x04001BB4 RID: 7092
			public LaneModel chosenLane;

			// Token: 0x04001BB5 RID: 7093
			public Fix64 timestamp;

			// Token: 0x04001BB6 RID: 7094
			public Fix64 committedTimestamp = -Fix64.One;
		}

		// Token: 0x020004F8 RID: 1272
		private class InboundVehicleDistanceComparer : IComparer<RoadChunkModel.InboundVehicle>
		{
			// Token: 0x060021BA RID: 8634 RVA: 0x00087AFC File Offset: 0x00085CFC
			public int Compare(RoadChunkModel.InboundVehicle x, RoadChunkModel.InboundVehicle y)
			{
				if (x.IsShoving ^ y.IsShoving)
				{
					if (!x.IsShoving)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					if (RoadChunkModel.InboundVehicleDistanceComparer.roundaboutLane != null)
					{
						bool isXOnRoundabout = this.IsVehicleOnRoundabout(x.vehicle, RoadChunkModel.InboundVehicleDistanceComparer.roundaboutLane);
						if (isXOnRoundabout != this.IsVehicleOnRoundabout(y.vehicle, RoadChunkModel.InboundVehicleDistanceComparer.roundaboutLane))
						{
							if (!isXOnRoundabout)
							{
								return 1;
							}
							return -1;
						}
					}
					Fix64 xDistance = x.vehicle.DistanceToLane(x.chosenLane);
					Fix64 yDistance = y.vehicle.DistanceToLane(y.chosenLane);
					int result = xDistance.CompareTo(yDistance);
					if (result == 0)
					{
						return x.timestamp.CompareTo(y.timestamp);
					}
					return result;
				}
			}

			// Token: 0x060021BB RID: 8635 RVA: 0x00087B9C File Offset: 0x00085D9C
			private bool IsVehicleOnRoundabout(VehicleModel vehicle, LaneModel roundaboutLane)
			{
				LaneModel vehicleLane = vehicle.CurrentFrame.lane;
				LaneModel firstLane = roundaboutLane;
				while (vehicleLane != roundaboutLane && !vehicleLane.OutboundLanes.Contains(roundaboutLane))
				{
					bool foundNextLane = false;
					foreach (LaneModel outboundLane in roundaboutLane.OutboundLanes)
					{
						if (outboundLane.connection.IsRoundabout)
						{
							roundaboutLane = outboundLane;
							foundNextLane = true;
							break;
						}
					}
					if (!foundNextLane)
					{
						return false;
					}
					if (roundaboutLane == firstLane)
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x04001BB7 RID: 7095
			public static LaneModel roundaboutLane;
		}
	}
}
