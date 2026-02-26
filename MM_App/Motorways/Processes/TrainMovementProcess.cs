using System;
using Factory;
using Factory.Pools;
using FixMath;
using JetBrains.Annotations;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x0200049E RID: 1182
	public class TrainMovementProcess : IProcess, IReusable
	{
		// Token: 0x06001D38 RID: 7480 RVA: 0x00073660 File Offset: 0x00071860
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (TrainModel trainModel in simulation.GetModels<TrainModel>())
			{
				Fix64 currentSpeed = trainModel.CurrentFrame.speed;
				Fix64 stoppingDistance = trainModel.StoppingDistance;
				Fix64 distanceToTarget = trainModel.DistanceToTarget;
				Fix64 forwardTrainLength = this._constants.trainCenterToWheelDistance;
				Fix64 backwardTrainLength = this._constants.trainCenterToWheelDistance * (Fix64)5L + this._constants.trainCarriageSeparationDistance * (Fix64)2L;
				RailDirection direction = trainModel.CurrentFrame.direction;
				if (direction == RailDirection.Backwards)
				{
					Fix64 fix = backwardTrainLength;
					Fix64 fix2 = forwardTrainLength;
					forwardTrainLength = fix;
					backwardTrainLength = fix2;
				}
				RailTileModel targetTrack = null;
				Fix64 stoppingDistanceAlongTargetTrack = Fix64.Zero;
				CarparkModel targetStation = null;
				Fix64 crossingBlockingDistance = this._constants.trainCrossingSignalDistance + forwardTrainLength;
				Fix64 trainTargetLookaheadDistance = (trainModel.state == TrainModel.BehaviorState.Driving) ? (Fix64.Max(this._constants.trainStoppingDistanceFromBuffer, stoppingDistance * Fix64Consts.Two) + forwardTrainLength) : Fix64.Zero;
				Fix64 minTraversalDistance = Fix64.Max(crossingBlockingDistance, trainTargetLookaheadDistance);
				Fix64 distanceChecked = Fix64.Zero;
				RailTileModel trackCursor = trainModel.CurrentFrame.tile;
				Fix64 distanceAlongCursor = trainModel.CurrentFrame.distanceAlongTrack;
				RailTileModel blockedRailSectionEnd = null;
				while (trackCursor != null && distanceChecked < minTraversalDistance)
				{
					if (distanceChecked < crossingBlockingDistance)
					{
						blockedRailSectionEnd = trackCursor;
					}
					RailTileModel nextTrack = trackCursor.GetNextRailModelInDirection(direction);
					if (targetTrack == null && distanceChecked < trainTargetLookaheadDistance)
					{
						if (trackCursor.carpark != null && (nextTrack == null || trackCursor.carpark != nextTrack.carpark) && trainModel.distanceTraveledSinceLastStation > TilemapModel.TileWidth)
						{
							ValueTuple<RailTileModel, Fix64, Fix64> valueTuple = trackCursor.Traverse(trackCursor.Length / Fix64Consts.Two, forwardTrainLength, TileUtilities.GetOppositeDirection(direction));
							RailTileModel stationTargetTrack = valueTuple.Item1;
							Fix64 stoppingDistanceAlongStationTargetTrack = valueTuple.Item2;
							Fix64 distanceToStationTarget = trainModel.CurrentFrame.tile.DistanceTo(trainModel.CurrentFrame.distanceAlongTrack, stationTargetTrack, stoppingDistanceAlongTargetTrack, direction);
							if (distanceToStationTarget != RailTileModel.InvalidDistance && distanceToStationTarget > stoppingDistance)
							{
								bool isStationAheadOfTrain = true;
								if (stationTargetTrack.Line.IsLoop)
								{
									Fix64 backwardsDistanceToStationTarget = trainModel.CurrentFrame.tile.DistanceTo(trainModel.CurrentFrame.distanceAlongTrack, stationTargetTrack, stoppingDistanceAlongTargetTrack, TileUtilities.GetOppositeDirection(direction));
									isStationAheadOfTrain = (backwardsDistanceToStationTarget == RailTileModel.InvalidDistance || distanceToStationTarget < backwardsDistanceToStationTarget);
								}
								if (isStationAheadOfTrain)
								{
									targetStation = trackCursor.carpark;
									targetTrack = stationTargetTrack;
									stoppingDistanceAlongTargetTrack = stoppingDistanceAlongStationTargetTrack;
								}
							}
						}
						if (targetTrack == null && nextTrack == null)
						{
							ValueTuple<RailTileModel, Fix64, Fix64> valueTuple2 = trackCursor.Traverse((direction == RailDirection.Forwards) ? (trackCursor.Length - this._constants.trainStoppingDistanceFromBuffer) : this._constants.trainStoppingDistanceFromBuffer, forwardTrainLength, TileUtilities.GetOppositeDirection(direction));
							targetTrack = valueTuple2.Item1;
							stoppingDistanceAlongTargetTrack = valueTuple2.Item2;
						}
					}
					if (direction == RailDirection.Forwards)
					{
						distanceChecked += trackCursor.Length - distanceAlongCursor;
						distanceAlongCursor = Fix64.Zero;
					}
					else
					{
						distanceChecked += distanceAlongCursor;
						distanceAlongCursor = ((nextTrack != null) ? nextTrack.Length : Fix64.Zero);
					}
					trackCursor = nextTrack;
				}
				RailTileModel blockedRailSectionStart = trainModel.CurrentFrame.tile.Traverse(trainModel.CurrentFrame.distanceAlongTrack, backwardTrainLength, TileUtilities.GetOppositeDirection(direction)).Item1;
				if (direction == RailDirection.Backwards)
				{
					RailTileModel railTileModel = blockedRailSectionEnd;
					RailTileModel railTileModel2 = blockedRailSectionStart;
					blockedRailSectionStart = railTileModel;
					blockedRailSectionEnd = railTileModel2;
				}
				this.ConfigureSignals(blockedRailSectionStart, blockedRailSectionEnd);
				switch (trainModel.state)
				{
				case TrainModel.BehaviorState.Driving:
					if (targetTrack != null)
					{
						trainModel.state = TrainModel.BehaviorState.ApproachingDestination;
						trainModel.targetTrack = targetTrack;
						trainModel.targetStation = targetStation;
						trainModel.stoppingDistanceAlongTargetTrack = stoppingDistanceAlongTargetTrack;
					}
					break;
				case TrainModel.BehaviorState.ApproachingDestination:
					if (distanceToTarget <= stoppingDistance)
					{
						trainModel.state = TrainModel.BehaviorState.Stopping;
					}
					break;
				case TrainModel.BehaviorState.Stopped:
					trainModel.DelayBeforeStarting -= timestep;
					if (trainModel.DelayBeforeStarting <= Fix64.Zero)
					{
						trainModel.targetStation = null;
						trainModel.state = TrainModel.BehaviorState.Driving;
						trainModel.DelayBeforeStarting = Fix64.Zero;
					}
					break;
				}
				Fix64 minSpeed = Fix64.Zero;
				Fix64 acceleration;
				switch (trainModel.state)
				{
				case TrainModel.BehaviorState.Driving:
				case TrainModel.BehaviorState.ApproachingDestination:
					acceleration = this._constants.trainAcceleration;
					break;
				case TrainModel.BehaviorState.Stopping:
					acceleration = ((distanceToTarget >= Fix64.Zero) ? (-(currentSpeed * currentSpeed) / (Fix64Consts.Two * distanceToTarget)) : (-this._constants.trainDeceleration));
					minSpeed = this._constants.trainMinimumSpeedDuringDeceleration;
					break;
				case TrainModel.BehaviorState.Stopped:
					goto IL_46B;
				default:
					goto IL_46B;
				}
				IL_472:
				Fix64 newSpeed = trainModel.CurrentFrame.speed + acceleration * timestep;
				newSpeed = Fix64.Clamp(newSpeed, minSpeed, this._constants.trainSpeed);
				Fix64 distanceTraveled = newSpeed * timestep;
				bool reverseDirection = false;
				TrainModel.BehaviorState state = trainModel.state;
				if (state != TrainModel.BehaviorState.Stopping)
				{
					if (state == TrainModel.BehaviorState.Stopped)
					{
						newSpeed = Fix64.Zero;
					}
				}
				else if (distanceTraveled >= distanceToTarget || newSpeed <= Fix64.Zero)
				{
					newSpeed = Fix64.Zero;
					distanceTraveled = Fix64.Max(distanceToTarget, Fix64.Zero);
					trainModel.state = TrainModel.BehaviorState.Stopped;
					if (trainModel.targetStation != null)
					{
						trainModel.distanceTraveledSinceLastStation = Fix64.Zero;
						trainModel.DelayBeforeStarting = this._constants.trainStationWaitTime;
						trainModel.HasPendingDemand = true;
						Fix64 clearDistanceNeededAfterStation = forwardTrainLength + (forwardTrainLength + backwardTrainLength);
						if (trainModel.targetTrack.Traverse(trainModel.stoppingDistanceAlongTargetTrack, clearDistanceNeededAfterStation, direction).Item3 < clearDistanceNeededAfterStation)
						{
							reverseDirection = true;
						}
					}
					else
					{
						trainModel.DelayBeforeStarting = this._constants.trainStationWaitTime;
						reverseDirection = true;
					}
					trainModel.targetTrack = null;
					trainModel.stoppingDistanceAlongTargetTrack = Fix64.Zero;
				}
				RailTileModel newTile = trainModel.CurrentFrame.tile;
				Fix64 newDistanceAlongTrack = trainModel.CurrentFrame.distanceAlongTrack;
				if (direction == RailDirection.Forwards)
				{
					newDistanceAlongTrack += distanceTraveled;
					if (newDistanceAlongTrack > newTile.Length)
					{
						newDistanceAlongTrack -= newTile.Length;
						newTile = newTile.NextRailModel;
					}
				}
				else
				{
					newDistanceAlongTrack -= distanceTraveled;
					if (newDistanceAlongTrack < Fix64.Zero)
					{
						newTile = newTile.PreviousRailModel;
						newDistanceAlongTrack = newTile.Length + newDistanceAlongTrack;
					}
				}
				if (reverseDirection)
				{
					direction = ((direction == RailDirection.Forwards) ? RailDirection.Backwards : RailDirection.Forwards);
				}
				trainModel.distanceTraveledSinceLastStation += distanceTraveled;
				trainModel.NextFrame.direction = direction;
				trainModel.NextFrame.speed = newSpeed;
				trainModel.NextFrame.tile = newTile;
				trainModel.NextFrame.distanceAlongTrack = newDistanceAlongTrack;
				continue;
				IL_46B:
				acceleration = Fix64.Zero;
				goto IL_472;
			}
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x00073CEC File Offset: 0x00071EEC
		private void ConfigureSignals([NotNull] RailTileModel blockedSectionStart, [NotNull] RailTileModel blockedSectionEnd)
		{
			if (blockedSectionStart.Line.IsLoop)
			{
				TrainSignalState signalState = TrainSignalState.Closed;
				RailTileModel trackCursor = blockedSectionStart;
				do
				{
					RailTileModel nextTrack = trackCursor.NextRailModel;
					foreach (RoadChunkModel roadChunk in trackCursor.GetRoadChunksInDirection(RailDirection.Forwards))
					{
						if (roadChunk.TrainCrossingModel != null)
						{
							roadChunk.TrainCrossingModel.RequestSignalStateChange(signalState);
						}
					}
					if (nextTrack == blockedSectionEnd)
					{
						signalState = TrainSignalState.Open;
					}
					trackCursor = nextTrack;
					if (trackCursor == blockedSectionStart)
					{
						return;
					}
				}
				while (trackCursor != null);
				return;
			}
			RailTileModel trackCursor2 = blockedSectionStart.Line.StartTile;
			TrainSignalState signalState2 = TrainSignalState.Open;
			while (trackCursor2 != null)
			{
				RailTileModel nextTrack2 = trackCursor2.NextRailModel;
				if (signalState2 == TrainSignalState.Open && trackCursor2 == blockedSectionStart)
				{
					signalState2 = TrainSignalState.Closed;
				}
				trackCursor2.SignalState = signalState2;
				foreach (RoadChunkModel roadChunk2 in trackCursor2.GetRoadChunksInDirection(RailDirection.Forwards))
				{
					if (roadChunk2.TrainCrossingModel != null)
					{
						roadChunk2.TrainCrossingModel.RequestSignalStateChange(signalState2);
					}
				}
				if (signalState2 == TrainSignalState.Closed && trackCursor2 == blockedSectionEnd)
				{
					signalState2 = TrainSignalState.Open;
				}
				trackCursor2 = nextTrack2;
			}
		}

		// Token: 0x0400191A RID: 6426
		[Dependency]
		private TilemapModel _tilemapModel;

		// Token: 0x0400191B RID: 6427
		[Dependency]
		private SimulationConstantsData _constants;
	}
}
