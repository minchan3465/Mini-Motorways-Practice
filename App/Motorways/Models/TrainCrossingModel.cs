using System;
using System.Runtime.CompilerServices;
using Factory;
using FixMath;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x02000507 RID: 1287
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1,
		1
	})]
	public class TrainCrossingModel : Model<EmptyModelFrame, TrainCrossingModel.IObserver>
	{
		// Token: 0x1700061C RID: 1564
		// (get) Token: 0x06002236 RID: 8758 RVA: 0x0008A5BD File Offset: 0x000887BD
		[Nullable(2)]
		public Tile Tile
		{
			[NullableContext(2)]
			get
			{
				return this._tile;
			}
		}

		// Token: 0x1700061D RID: 1565
		// (get) Token: 0x06002237 RID: 8759 RVA: 0x0008A5C5 File Offset: 0x000887C5
		public RoadChunkModel RoadChunkModel
		{
			get
			{
				return this._roadChunkModel;
			}
		}

		// Token: 0x1700061E RID: 1566
		// (get) Token: 0x06002238 RID: 8760 RVA: 0x0008A5CD File Offset: 0x000887CD
		// (set) Token: 0x06002239 RID: 8761 RVA: 0x0008A5D5 File Offset: 0x000887D5
		public Vector2Int CrossingDirection { get; private set; }

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x0600223A RID: 8762 RVA: 0x0008A5DE File Offset: 0x000887DE
		// (set) Token: 0x0600223B RID: 8763 RVA: 0x0008A5E8 File Offset: 0x000887E8
		public TrainSignalState SignalState
		{
			get
			{
				return this._signalState;
			}
			private set
			{
				if (value != this._signalState)
				{
					this._signalState = value;
					foreach (TrainCrossingModel.IObserver observer in base.Observers)
					{
						observer.OnSignalChanged(value);
					}
				}
			}
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x0008A629 File Offset: 0x00088829
		public void Initialize(Tile tile, RoadChunkModel roadChunkModel, Vector2Int trainCrossingDirection)
		{
			this._tile = tile;
			this._roadChunkModel = roadChunkModel;
			this.CrossingDirection = trainCrossingDirection;
			this._signalState = TrainSignalState.Open;
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x0008A647 File Offset: 0x00088847
		public override void Reset()
		{
			base.Reset();
			this.CrossingDirection = Vector2Int.zero;
			this._signalState = TrainSignalState.Open;
			this._tile = null;
			this._signalOpenRequested = false;
			this._signalOpenRequestTime = default(Fix64);
			this._roadChunkModel = null;
		}

		// Token: 0x0600223E RID: 8766 RVA: 0x0008A682 File Offset: 0x00088882
		public void RequestSignalStateChange(TrainSignalState targetSignalState)
		{
			if (targetSignalState != this._signalState && !this._signalOpenRequested)
			{
				if (targetSignalState == TrainSignalState.Closed)
				{
					this.SignalState = TrainSignalState.Closed;
					return;
				}
				this._signalOpenRequested = true;
				this._signalOpenRequestTime = base.Clock.Time;
			}
		}

		// Token: 0x0600223F RID: 8767 RVA: 0x0008A6B8 File Offset: 0x000888B8
		public bool HasPendingSignalOpenRequestTimeElapsed()
		{
			return this._signalOpenRequested && base.Clock.Time > this._signalOpenRequestTime + this._constants.crossingWaitTime;
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x0008A6EA File Offset: 0x000888EA
		public void CommitPendingSignalOpenRequest()
		{
			if (!this._signalOpenRequested)
			{
				Diagnostics.FailAssert("No signal state change request pending!", Array.Empty<object>());
				return;
			}
			this._signalOpenRequested = false;
			this.SignalState = TrainSignalState.Open;
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x0008A712 File Offset: 0x00088912
		public TrainCrossingModel() : base(1)
		{
		}

		// Token: 0x04001C07 RID: 7175
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001C08 RID: 7176
		[Nullable(2)]
		private Tile _tile;

		// Token: 0x04001C09 RID: 7177
		private RoadChunkModel _roadChunkModel;

		// Token: 0x04001C0B RID: 7179
		private TrainSignalState _signalState = TrainSignalState.Open;

		// Token: 0x04001C0C RID: 7180
		private bool _signalOpenRequested;

		// Token: 0x04001C0D RID: 7181
		private Fix64 _signalOpenRequestTime;

		// Token: 0x02000508 RID: 1288
		public interface IObserver
		{
			// Token: 0x06002242 RID: 8770
			void OnSignalChanged(TrainSignalState trainSignalState);
		}
	}
}
