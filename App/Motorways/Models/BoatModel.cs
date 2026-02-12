using System;
using Factory;
using FixMath;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004CC RID: 1228
	public class BoatModel : Model<BoatModel.Frame, BoatModel.IObserver>
	{
		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x06002017 RID: 8215 RVA: 0x0007E410 File Offset: 0x0007C610
		public Fix64 DistanceToTarget
		{
			get
			{
				if (this.targetBoatPath == null)
				{
					return BoatPathTileModel.InvalidDistance;
				}
				Fix64 distanceToTarget = base.CurrentFrame.tile.DistanceTo(base.CurrentFrame.DistanceAlongPathSegment, this.targetBoatPath, this.stoppingDistanceAlongTargetPathSegment, base.CurrentFrame.direction);
				if (distanceToTarget == BoatPathTileModel.InvalidDistance && base.CurrentFrame.tile == this.targetBoatPath)
				{
					distanceToTarget = Fix64.Zero;
				}
				return distanceToTarget;
			}
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x06002018 RID: 8216 RVA: 0x0007E485 File Offset: 0x0007C685
		public Fix64 StoppingDistance
		{
			get
			{
				return this.GetBrakingDistance(Fix64.Zero);
			}
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x0007E494 File Offset: 0x0007C694
		public Fix64 GetBrakingDistance(Fix64 targetSpeed)
		{
			Fix64 currentSpeed = base.CurrentFrame.speed;
			if (currentSpeed <= targetSpeed)
			{
				return Fix64.Zero;
			}
			return (targetSpeed * targetSpeed - currentSpeed * currentSpeed) / (Fix64Consts.Two * -this._simulationConstants.trainDeceleration);
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x0007E4F0 File Offset: 0x0007C6F0
		public void SetTargetTerminal(CarparkModel carparkModel)
		{
			this._targetTerminal = carparkModel;
			foreach (BoatModel.IObserver observer in base.Observers)
			{
				observer.OnTargetTerminalSet(carparkModel);
			}
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x0007E528 File Offset: 0x0007C728
		public CarparkModel GetTargetTerminal()
		{
			return this._targetTerminal;
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x0007E530 File Offset: 0x0007C730
		public override void Reset()
		{
			base.Reset();
			this.state = BoatModel.BehaviorState.Stopped;
			this.targetBoatPath = null;
			this._targetTerminal = null;
			this.stoppingDistanceAlongTargetPathSegment = Fix64.Zero;
			this.distanceTraveledSinceLastTarget = Fix64.Zero;
			this.DelayBeforeStarting = Fix64.Zero;
		}

		// Token: 0x0600201D RID: 8221 RVA: 0x0007E56E File Offset: 0x0007C76E
		public BoatModel() : base(1)
		{
		}

		// Token: 0x04001A9C RID: 6812
		public BoatModel.BehaviorState state;

		// Token: 0x04001A9D RID: 6813
		public BoatPathTileModel targetBoatPath;

		// Token: 0x04001A9E RID: 6814
		public Fix64 stoppingDistanceAlongTargetPathSegment = Fix64.Zero;

		// Token: 0x04001A9F RID: 6815
		public Fix64 distanceTraveledSinceLastTarget = Fix64.Zero;

		// Token: 0x04001AA0 RID: 6816
		public bool HasPendingDemand;

		// Token: 0x04001AA1 RID: 6817
		[Dependency]
		private SimulationConstantsData _simulationConstants;

		// Token: 0x04001AA2 RID: 6818
		public Fix64 DelayBeforeStarting = Fix64.Zero;

		// Token: 0x04001AA3 RID: 6819
		private CarparkModel _targetTerminal;

		// Token: 0x020004CD RID: 1229
		public enum BoatDirection
		{
			// Token: 0x04001AA5 RID: 6821
			Forwards,
			// Token: 0x04001AA6 RID: 6822
			Backwards
		}

		// Token: 0x020004CE RID: 1230
		public enum BehaviorState
		{
			// Token: 0x04001AA8 RID: 6824
			Sailing,
			// Token: 0x04001AA9 RID: 6825
			ApproachingTerminal,
			// Token: 0x04001AAA RID: 6826
			Stopping,
			// Token: 0x04001AAB RID: 6827
			Stopped,
			// Token: 0x04001AAC RID: 6828
			Undocking,
			// Token: 0x04001AAD RID: 6829
			TurningAtTerminal,
			// Token: 0x04001AAE RID: 6830
			TurningAtEndOfLine
		}

		// Token: 0x020004CF RID: 1231
		public class Frame : IFrame
		{
			// Token: 0x0600201E RID: 8222 RVA: 0x0007E598 File Offset: 0x0007C798
			public void Reset()
			{
				this.tile = null;
				this.DistanceAlongPathSegment = Fix64.Zero;
				this.speed = Fix64.Zero;
			}

			// Token: 0x0600201F RID: 8223 RVA: 0x0007E5B7 File Offset: 0x0007C7B7
			public bool CloneInto(IFrame cloneFrame, IScope scope)
			{
				BoatModel.Frame frame = (BoatModel.Frame)cloneFrame;
				frame.tile = this.tile;
				frame.DistanceAlongPathSegment = this.DistanceAlongPathSegment;
				frame.speed = this.speed;
				frame.direction = this.direction;
				return true;
			}

			// Token: 0x04001AAF RID: 6831
			public BoatPathTileModel tile;

			// Token: 0x04001AB0 RID: 6832
			public Fix64 DistanceAlongPathSegment;

			// Token: 0x04001AB1 RID: 6833
			public Fix64 speed;

			// Token: 0x04001AB2 RID: 6834
			public BoatModel.BoatDirection direction;
		}

		// Token: 0x020004D0 RID: 1232
		public interface IObserver
		{
			// Token: 0x06002021 RID: 8225
			void OnTargetTerminalSet(CarparkModel targetTerminal);
		}
	}
}
