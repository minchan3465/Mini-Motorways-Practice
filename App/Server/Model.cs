using System;
using Factory;
using Factory.Pools;

namespace Server
{
	// Token: 0x0200028F RID: 655
	public abstract class Model<TFrame, TObserver> : IModel, IReusable, IReleasedFromScopeHandler where TFrame : IFrame, new()
	{
		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001014 RID: 4116 RVA: 0x00035FE3 File Offset: 0x000341E3
		// (set) Token: 0x06001015 RID: 4117 RVA: 0x00035FEB File Offset: 0x000341EB
		[Dependency]
		public Clock Clock { get; protected set; }

		// Token: 0x06001016 RID: 4118 RVA: 0x00035FF4 File Offset: 0x000341F4
		public void Subscribe(TObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x00036002 File Offset: 0x00034202
		public bool Unsubscribe(TObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x00036010 File Offset: 0x00034210
		protected Model(int observerCapacity = 1)
		{
			this._observers = ((observerCapacity > 0) ? new ObserverList<TObserver>(observerCapacity) : null);
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x06001019 RID: 4121 RVA: 0x0003604F File Offset: 0x0003424F
		protected ObserverList<TObserver> Observers
		{
			get
			{
				return this._observers;
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600101A RID: 4122 RVA: 0x00036057 File Offset: 0x00034257
		public TFrame CurrentFrame
		{
			get
			{
				return this._frames[this.Clock.ModelFrameIndex];
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x0600101B RID: 4123 RVA: 0x0003606F File Offset: 0x0003426F
		public TFrame NextFrame
		{
			get
			{
				return this._frames[1 - this.Clock.ModelFrameIndex];
			}
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x00036089 File Offset: 0x00034289
		public virtual void OnReleasedFromScope(IScope scope)
		{
			this._observers.UnsubscribeAll();
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x00036096 File Offset: 0x00034296
		public virtual void Reset()
		{
			this._frames[0].Reset();
			this._frames[1].Reset();
		}

		// Token: 0x04000E45 RID: 3653
		[Serialize(true, typeof(ModelFrameSerializer))]
		private readonly TFrame[] _frames = new TFrame[]
		{
			Activator.CreateInstance<TFrame>(),
			Activator.CreateInstance<TFrame>()
		};

		// Token: 0x04000E46 RID: 3654
		[Serialize(false, null)]
		private readonly ObserverList<TObserver> _observers;
	}
}
