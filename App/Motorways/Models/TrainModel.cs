using System;
using Factory;
using FixMath;
using Server;

namespace Motorways.Models
{
	// Token: 0x0200050C RID: 1292
	public class TrainModel : Model<TrainModel.Frame, TrainModel.IObserver>
	{
		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x0600224E RID: 8782 RVA: 0x0008A840 File Offset: 0x00088A40
		public Fix64 DistanceToTarget
		{
			get
			{
				if (this.targetTrack == null)
				{
					return RailTileModel.InvalidDistance;
				}
				Fix64 distanceToTarget = base.CurrentFrame.tile.DistanceTo(base.CurrentFrame.distanceAlongTrack, this.targetTrack, this.stoppingDistanceAlongTargetTrack, base.CurrentFrame.direction);
				if (distanceToTarget == RailTileModel.InvalidDistance && base.CurrentFrame.tile == this.targetTrack)
				{
					distanceToTarget = Fix64.Zero;
				}
				return distanceToTarget;
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x0600224F RID: 8783 RVA: 0x0008A8B5 File Offset: 0x00088AB5
		public Fix64 StoppingDistance
		{
			get
			{
				return this.GetBrakingDistance(Fix64.Zero);
			}
		}

		// Token: 0x06002250 RID: 8784 RVA: 0x0008A8C4 File Offset: 0x00088AC4
		public Fix64 GetBrakingDistance(Fix64 targetSpeed)
		{
			Fix64 currentSpeed = base.CurrentFrame.speed;
			if (currentSpeed <= targetSpeed)
			{
				return Fix64.Zero;
			}
			return (targetSpeed * targetSpeed - currentSpeed * currentSpeed) / (Fix64Consts.Two * -this._simulationConstants.trainDeceleration);
		}

		// Token: 0x06002251 RID: 8785 RVA: 0x0008A91E File Offset: 0x00088B1E
		public override void Reset()
		{
			base.Reset();
			this.state = TrainModel.BehaviorState.Stopped;
			this.targetTrack = null;
			this.targetStation = null;
			this.stoppingDistanceAlongTargetTrack = Fix64.Zero;
			this.distanceTraveledSinceLastStation = Fix64.Zero;
			this.DelayBeforeStarting = Fix64.Zero;
		}

		// Token: 0x06002252 RID: 8786 RVA: 0x0008A95C File Offset: 0x00088B5C
		public TrainModel() : base(1)
		{
		}

		// Token: 0x04001C17 RID: 7191
		public TrainModel.BehaviorState state = TrainModel.BehaviorState.Stopped;

		// Token: 0x04001C18 RID: 7192
		public RailTileModel targetTrack;

		// Token: 0x04001C19 RID: 7193
		public CarparkModel targetStation;

		// Token: 0x04001C1A RID: 7194
		public Fix64 stoppingDistanceAlongTargetTrack = Fix64.Zero;

		// Token: 0x04001C1B RID: 7195
		public Fix64 distanceTraveledSinceLastStation = Fix64.Zero;

		// Token: 0x04001C1C RID: 7196
		public bool HasPendingDemand;

		// Token: 0x04001C1D RID: 7197
		[Dependency]
		private SimulationConstantsData _simulationConstants;

		// Token: 0x04001C1E RID: 7198
		public Fix64 DelayBeforeStarting = Fix64.Zero;

		// Token: 0x0200050D RID: 1293
		public enum BehaviorState
		{
			// Token: 0x04001C20 RID: 7200
			Driving,
			// Token: 0x04001C21 RID: 7201
			ApproachingDestination,
			// Token: 0x04001C22 RID: 7202
			Stopping,
			// Token: 0x04001C23 RID: 7203
			Stopped
		}

		// Token: 0x0200050E RID: 1294
		public class Frame : IFrame
		{
			// Token: 0x06002253 RID: 8787 RVA: 0x0008A98D File Offset: 0x00088B8D
			public void Reset()
			{
				this.tile = null;
				this.distanceAlongTrack = Fix64.Zero;
				this.speed = Fix64.Zero;
				this.direction = RailDirection.Forwards;
			}

			// Token: 0x06002254 RID: 8788 RVA: 0x0008A9B3 File Offset: 0x00088BB3
			public bool CloneInto(IFrame cloneFrame, IScope scope)
			{
				TrainModel.Frame frame = (TrainModel.Frame)cloneFrame;
				frame.tile = this.tile;
				frame.distanceAlongTrack = this.distanceAlongTrack;
				frame.speed = this.speed;
				frame.direction = this.direction;
				return true;
			}

			// Token: 0x04001C24 RID: 7204
			public RailTileModel tile;

			// Token: 0x04001C25 RID: 7205
			public Fix64 distanceAlongTrack;

			// Token: 0x04001C26 RID: 7206
			public Fix64 speed;

			// Token: 0x04001C27 RID: 7207
			public RailDirection direction;
		}

		// Token: 0x0200050F RID: 1295
		public interface IObserver
		{
		}
	}
}
