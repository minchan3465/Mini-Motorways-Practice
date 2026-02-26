using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x02000483 RID: 1155
	public class BoatMovementProcess : IProcess, IReusable
	{
		// Token: 0x06001CAB RID: 7339 RVA: 0x0006AE78 File Offset: 0x00069078
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			foreach (BoatModel boatModel in simulation.GetModels<BoatModel>())
			{
				Fix64 currentSpeed = boatModel.CurrentFrame.speed;
				Fix64 stoppingDistance = boatModel.StoppingDistance;
				Fix64 distanceToTarget = boatModel.DistanceToTarget;
				Fix64 forwardBoatLength = this._constants.boatCenterToBowDistance;
				BoatPathTileModel targetBoatPath = null;
				Fix64 stoppingDistanceAlongTargetPath = Fix64.Zero;
				CarparkModel targetTerminal = null;
				Fix64 boatTargetLookaheadDistance = (boatModel.state == BoatModel.BehaviorState.Sailing) ? Fix64.Max(this._constants.boatStoppingDistanceFromBuffer, stoppingDistance * Fix64Consts.Two) : Fix64.Zero;
				Fix64 minTraversalDistance = boatTargetLookaheadDistance;
				Fix64 distanceChecked = Fix64.Zero;
				BoatPathTileModel pathCursor = boatModel.CurrentFrame.tile;
				Fix64 distanceAlongCursor = boatModel.CurrentFrame.DistanceAlongPathSegment;
				BoatModel.BoatDirection boatDirection = boatModel.CurrentFrame.direction;
				while (pathCursor != null && distanceChecked < minTraversalDistance)
				{
					BoatPathTileModel nextPathSegment = pathCursor.GetNextBoatPathModelInDirection(boatDirection);
					if (targetBoatPath == null && distanceChecked < boatTargetLookaheadDistance)
					{
						if (pathCursor.carpark != null)
						{
							ValueTuple<BoatPathTileModel, Fix64> valueTuple = pathCursor.Traverse(pathCursor.Length / Fix64Consts.Two, forwardBoatLength);
							BoatPathTileModel boatTargetPath = valueTuple.Item1;
							Fix64 stoppingDistanceAlongTerminalTargetPath = valueTuple.Item2;
							Fix64 distanceToTerminalTarget = boatModel.CurrentFrame.tile.DistanceTo(boatModel.CurrentFrame.DistanceAlongPathSegment, boatTargetPath, stoppingDistanceAlongTargetPath, boatModel.CurrentFrame.direction);
							if (distanceToTerminalTarget != BoatPathTileModel.InvalidDistance && distanceToTerminalTarget > stoppingDistance)
							{
								bool isTerminalAheadOfBoat = true;
								if (boatTargetPath.BoatPath.IsLoop)
								{
									Fix64 backwardsDistanceToTerminalTarget = boatModel.CurrentFrame.tile.DistanceTo(boatModel.CurrentFrame.DistanceAlongPathSegment, boatTargetPath, stoppingDistanceAlongTargetPath, boatModel.CurrentFrame.direction);
									isTerminalAheadOfBoat = (backwardsDistanceToTerminalTarget == BoatPathTileModel.InvalidDistance || distanceToTerminalTarget < backwardsDistanceToTerminalTarget);
								}
								if (isTerminalAheadOfBoat)
								{
									targetTerminal = pathCursor.carpark;
									targetBoatPath = boatTargetPath;
									stoppingDistanceAlongTargetPath = stoppingDistanceAlongTerminalTargetPath;
								}
							}
						}
						if (targetBoatPath == null && nextPathSegment == null)
						{
							ValueTuple<BoatPathTileModel, Fix64> valueTuple2 = pathCursor.Traverse(pathCursor.Length - this._constants.boatStoppingDistanceFromBuffer, forwardBoatLength);
							targetBoatPath = valueTuple2.Item1;
							stoppingDistanceAlongTargetPath = valueTuple2.Item2;
						}
					}
					distanceChecked += pathCursor.Length - distanceAlongCursor;
					distanceAlongCursor = Fix64.Zero;
					pathCursor = nextPathSegment;
				}
				switch (boatModel.state)
				{
				case BoatModel.BehaviorState.Sailing:
					if (targetBoatPath != null)
					{
						boatModel.state = BoatModel.BehaviorState.ApproachingTerminal;
						boatModel.targetBoatPath = targetBoatPath;
						boatModel.SetTargetTerminal(targetTerminal);
						boatModel.stoppingDistanceAlongTargetPathSegment = stoppingDistanceAlongTargetPath;
					}
					break;
				case BoatModel.BehaviorState.ApproachingTerminal:
					if (distanceToTarget <= stoppingDistance)
					{
						boatModel.state = BoatModel.BehaviorState.Stopping;
					}
					break;
				case BoatModel.BehaviorState.Stopped:
					boatModel.DelayBeforeStarting -= timestep;
					if (boatModel.DelayBeforeStarting <= Fix64.Zero)
					{
						if (boatModel.GetTargetTerminal() != null)
						{
							boatModel.SetTargetTerminal(null);
							boatModel.state = BoatModel.BehaviorState.Undocking;
						}
						else
						{
							boatModel.state = BoatModel.BehaviorState.Sailing;
						}
						boatModel.DelayBeforeStarting = Fix64.Zero;
					}
					break;
				}
				Fix64 minSpeed = Fix64.Zero;
				Fix64 acceleration;
				switch (boatModel.state)
				{
				case BoatModel.BehaviorState.Sailing:
				case BoatModel.BehaviorState.ApproachingTerminal:
					acceleration = this._constants.boatAcceleration;
					break;
				case BoatModel.BehaviorState.Stopping:
					acceleration = ((distanceToTarget >= Fix64.Zero) ? (-(currentSpeed * currentSpeed) / (Fix64Consts.Two * distanceToTarget)) : (-this._constants.boatDeceleration));
					minSpeed = this._constants.boatMinimumSpeedDuringDeceleration;
					break;
				case BoatModel.BehaviorState.Stopped:
					goto IL_36F;
				case BoatModel.BehaviorState.Undocking:
					acceleration = this._constants.boatUndockingAcceleration;
					break;
				default:
					goto IL_36F;
				}
				IL_376:
				Fix64 newSpeed = boatModel.CurrentFrame.speed + acceleration * timestep;
				newSpeed = Fix64.Clamp(newSpeed, minSpeed, this._constants.boatSpeed);
				Fix64 distanceTraveled = newSpeed * timestep;
				bool reverseDirection = false;
				switch (boatModel.state)
				{
				case BoatModel.BehaviorState.Stopping:
					if (distanceTraveled >= distanceToTarget || newSpeed <= Fix64.Zero)
					{
						newSpeed = Fix64.Zero;
						distanceTraveled = boatModel.CurrentFrame.tile.Length - boatModel.CurrentFrame.DistanceAlongPathSegment;
						boatModel.state = BoatModel.BehaviorState.Stopped;
						if (boatModel.GetTargetTerminal() != null)
						{
							boatModel.DelayBeforeStarting = this._constants.boatTerminalWaitTime;
							boatModel.HasPendingDemand = true;
							reverseDirection = true;
						}
						else
						{
							boatModel.DelayBeforeStarting = this._constants.boatTerminalWaitTime;
							reverseDirection = true;
						}
						boatModel.stoppingDistanceAlongTargetPathSegment = Fix64.Zero;
						boatModel.distanceTraveledSinceLastTarget = Fix64.Zero;
					}
					break;
				case BoatModel.BehaviorState.Stopped:
					newSpeed = Fix64.Zero;
					break;
				case BoatModel.BehaviorState.Undocking:
					if (!Diagnostics.Verify(this._constants.boatUndockingSpeedThreshold < this._constants.boatSpeed, "undocking speed threshold must be less than boat speed!"))
					{
						this._constants.boatUndockingSpeedThreshold = this._constants.boatSpeed;
					}
					if (newSpeed >= this._constants.boatUndockingSpeedThreshold)
					{
						boatModel.state = BoatModel.BehaviorState.Sailing;
					}
					break;
				}
				BoatPathTileModel newTile = boatModel.CurrentFrame.tile;
				Fix64 newDistanceAlongPath = boatModel.CurrentFrame.DistanceAlongPathSegment;
				newDistanceAlongPath += distanceTraveled;
				if (newDistanceAlongPath > newTile.Length)
				{
					newDistanceAlongPath -= newTile.Length;
					newTile = newTile.GetNextBoatPathModelInDirection(boatModel.CurrentFrame.direction);
				}
				if (reverseDirection)
				{
					boatDirection = ((boatDirection == BoatModel.BoatDirection.Forwards) ? BoatModel.BoatDirection.Backwards : BoatModel.BoatDirection.Forwards);
				}
				boatModel.distanceTraveledSinceLastTarget += distanceTraveled;
				boatModel.NextFrame.speed = newSpeed;
				boatModel.NextFrame.tile = newTile;
				boatModel.NextFrame.direction = boatDirection;
				boatModel.NextFrame.DistanceAlongPathSegment = newDistanceAlongPath;
				continue;
				IL_36F:
				acceleration = Fix64.Zero;
				goto IL_376;
			}
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x040018A4 RID: 6308
		[Dependency]
		private TilemapModel _tilemapModel;

		// Token: 0x040018A5 RID: 6309
		[Dependency]
		private SimulationConstantsData _constants;
	}
}
