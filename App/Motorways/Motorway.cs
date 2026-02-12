using System;
using Factory;
using Factory.Pools;
using FixMath;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000414 RID: 1044
	public class Motorway : IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x0600199C RID: 6556 RVA: 0x0005C147 File Offset: 0x0005A347
		// (set) Token: 0x0600199D RID: 6557 RVA: 0x0005C14F File Offset: 0x0005A34F
		public Fix64 PermanenceProgress
		{
			get
			{
				return this._permanenceProgress;
			}
			protected set
			{
				if (this._permanenceProgress != value)
				{
					this._permanenceProgress = value;
					if (this._permanenceProgress > Fix64.One)
					{
						this._permanenceProgress = Fix64.One;
					}
					this.NotifyMotorwayChanged(Motorway.ChangeFlags.Permanence);
				}
			}
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x0005C18B File Offset: 0x0005A38B
		public void SetPermanence(Fix64 permanence)
		{
			this.PermanenceProgress = permanence;
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x0005C194 File Offset: 0x0005A394
		public virtual bool Initialize(ITilemap tilemap, int id, int number, RoadState roadState = RoadState.None)
		{
			if (!Diagnostics.Verify(this._id == -1, "Motorway does not have an uninitialised id."))
			{
				return false;
			}
			if (!Diagnostics.Verify(number != 0, "Motorway must have valid number when initialized"))
			{
				return false;
			}
			this._tilemap = tilemap;
			this._id = id;
			this._number = number;
			this._state = roadState;
			this._startCoordinates = new Vector2Int(0, 0);
			this._startDirection = TileDirection.None;
			this._endCoordinates = new Vector2Int(0, 0);
			this._endDirection = TileDirection.None;
			this._concreteCost = 0;
			this._concreteGivenToReplacement = 0;
			return true;
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x0005C220 File Offset: 0x0005A420
		public virtual void Reset()
		{
			this._tilemap = null;
			this._id = -1;
			this._number = 0;
			this._state = RoadState.None;
			this._startCoordinates = default(Vector2Int);
			this._startDirection = TileDirection.North;
			this._endCoordinates = default(Vector2Int);
			this._endDirection = TileDirection.North;
			this._concreteCost = 0;
			this._concreteGivenToReplacement = 0;
			this._permanenceProgress = Fix64.Zero;
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x0005C288 File Offset: 0x0005A488
		public virtual void OnReleasedFromScope(IScope scope)
		{
			this._observers.UnsubscribeAll();
		}

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x060019A2 RID: 6562 RVA: 0x0005C295 File Offset: 0x0005A495
		public ITilemap Tilemap
		{
			get
			{
				return this._tilemap;
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x060019A3 RID: 6563 RVA: 0x0005C29D File Offset: 0x0005A49D
		// (set) Token: 0x060019A4 RID: 6564 RVA: 0x0005C2A5 File Offset: 0x0005A4A5
		public int Id
		{
			get
			{
				return this._id;
			}
			private set
			{
				this._id = value;
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x060019A5 RID: 6565 RVA: 0x0005C2AE File Offset: 0x0005A4AE
		public int Number
		{
			get
			{
				return this._number;
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x060019A6 RID: 6566 RVA: 0x0005C2B6 File Offset: 0x0005A4B6
		public RoadState State
		{
			get
			{
				return this._state;
			}
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x0005C2C0 File Offset: 0x0005A4C0
		public bool CanSetState(RoadState newState)
		{
			if (newState <= RoadState.Planned)
			{
				if (newState != RoadState.None)
				{
					if (newState == RoadState.Planned)
					{
						if (this._state != RoadState.None)
						{
							return false;
						}
					}
				}
				else if (this._state != RoadState.Mothballed && this._state != RoadState.Planned)
				{
					return false;
				}
			}
			else if (newState != RoadState.Active)
			{
				if (newState == RoadState.Mothballed)
				{
					if ((this._state & (RoadState.Planned | RoadState.Active)) == RoadState.None)
					{
						return false;
					}
				}
			}
			else if (this._state != RoadState.Planned && this._state != RoadState.Mothballed)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x0005C328 File Offset: 0x0005A528
		public bool SetState(RoadState newState)
		{
			if (!this.CanSetState(newState))
			{
				return false;
			}
			if (this._state == RoadState.Planned && newState == RoadState.Mothballed)
			{
				this._state = RoadState.None;
			}
			else
			{
				if (newState == RoadState.Planned && this._state == RoadState.Mothballed)
				{
					this._permanenceProgress = Fix64.Zero;
				}
				this._state = newState;
			}
			this.NotifyMotorwayChanged(Motorway.ChangeFlags.State);
			return true;
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x060019A9 RID: 6569 RVA: 0x0005C37F File Offset: 0x0005A57F
		// (set) Token: 0x060019AA RID: 6570 RVA: 0x0005C387 File Offset: 0x0005A587
		public Vector2Int StartCoordinates
		{
			get
			{
				return this._startCoordinates;
			}
			set
			{
				if (this._startCoordinates != value)
				{
					this._startCoordinates = value;
					this.NotifyMotorwayChanged(Motorway.ChangeFlags.StartTile);
				}
			}
		}

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x060019AB RID: 6571 RVA: 0x0005C3A5 File Offset: 0x0005A5A5
		// (set) Token: 0x060019AC RID: 6572 RVA: 0x0005C3AD File Offset: 0x0005A5AD
		public TileDirection StartDirection
		{
			get
			{
				return this._startDirection;
			}
			set
			{
				if (this._startDirection != value)
				{
					this._startDirection = value;
					this.NotifyMotorwayChanged(Motorway.ChangeFlags.StartTile);
				}
			}
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x060019AD RID: 6573 RVA: 0x0005C3C6 File Offset: 0x0005A5C6
		// (set) Token: 0x060019AE RID: 6574 RVA: 0x0005C3CE File Offset: 0x0005A5CE
		public Vector2Int EndCoordinates
		{
			get
			{
				return this._endCoordinates;
			}
			set
			{
				if (this._endCoordinates != value)
				{
					this._endCoordinates = value;
					this.NotifyMotorwayChanged(Motorway.ChangeFlags.EndTile);
				}
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x060019AF RID: 6575 RVA: 0x0005C3EC File Offset: 0x0005A5EC
		// (set) Token: 0x060019B0 RID: 6576 RVA: 0x0005C3F4 File Offset: 0x0005A5F4
		public TileDirection EndDirection
		{
			get
			{
				return this._endDirection;
			}
			set
			{
				if (this._endDirection != value)
				{
					this._endDirection = value;
					this.NotifyMotorwayChanged(Motorway.ChangeFlags.EndTile);
				}
			}
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x060019B1 RID: 6577 RVA: 0x0005C40D File Offset: 0x0005A60D
		// (set) Token: 0x060019B2 RID: 6578 RVA: 0x0005C415 File Offset: 0x0005A615
		public int ConcreteCost
		{
			get
			{
				return this._concreteCost;
			}
			set
			{
				if (this._concreteCost != value)
				{
					this._concreteCost = value;
					this.NotifyMotorwayChanged(Motorway.ChangeFlags.Concrete);
				}
			}
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x060019B3 RID: 6579 RVA: 0x0005C42F File Offset: 0x0005A62F
		// (set) Token: 0x060019B4 RID: 6580 RVA: 0x0005C437 File Offset: 0x0005A637
		public int ConcreteGivenToReplacement
		{
			get
			{
				return this._concreteGivenToReplacement;
			}
			set
			{
				if (this._concreteGivenToReplacement != value)
				{
					this._concreteGivenToReplacement = value;
					this.NotifyMotorwayChanged(Motorway.ChangeFlags.Concrete);
				}
			}
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x0005C451 File Offset: 0x0005A651
		public bool IsPermanent
		{
			get
			{
				return this._permanenceProgress >= Fix64.One;
			}
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0005C464 File Offset: 0x0005A664
		public void CloneInto(Motorway cloneMotorway)
		{
			Motorway.ChangeFlags cloneChanges = (Motorway.ChangeFlags)0;
			if (cloneMotorway._state != this._state)
			{
				cloneMotorway._state = this._state;
				cloneChanges |= Motorway.ChangeFlags.State;
			}
			if (cloneMotorway._number != this._number)
			{
				cloneMotorway._number = this._number;
				cloneChanges |= Motorway.ChangeFlags.Number;
			}
			if (cloneMotorway._startCoordinates != this._startCoordinates)
			{
				cloneMotorway._startCoordinates = this._startCoordinates;
				cloneChanges |= Motorway.ChangeFlags.StartTile;
			}
			if (cloneMotorway._startDirection != this._startDirection)
			{
				cloneMotorway._startDirection = this._startDirection;
				cloneChanges |= Motorway.ChangeFlags.StartTile;
			}
			if (cloneMotorway._endCoordinates != this._endCoordinates)
			{
				cloneMotorway._endCoordinates = this._endCoordinates;
				cloneChanges |= Motorway.ChangeFlags.EndTile;
			}
			if (cloneMotorway._endDirection != this._endDirection)
			{
				cloneMotorway._endDirection = this._endDirection;
				cloneChanges |= Motorway.ChangeFlags.EndTile;
			}
			if (cloneMotorway._concreteCost != this._concreteCost)
			{
				cloneMotorway._concreteCost = this._concreteCost;
				cloneChanges |= Motorway.ChangeFlags.Concrete;
			}
			if (cloneMotorway._concreteGivenToReplacement != this._concreteGivenToReplacement)
			{
				cloneMotorway._concreteGivenToReplacement = this._concreteGivenToReplacement;
				cloneChanges |= Motorway.ChangeFlags.Concrete;
			}
			if (cloneMotorway._permanenceProgress != this._permanenceProgress)
			{
				cloneMotorway._permanenceProgress = this._permanenceProgress;
				cloneChanges |= Motorway.ChangeFlags.Permanence;
			}
			if (cloneChanges != (Motorway.ChangeFlags)0)
			{
				cloneMotorway.NotifyMotorwayChanged(cloneChanges);
			}
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0005C59D File Offset: 0x0005A79D
		public void Clear()
		{
			this._state = RoadState.None;
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x0005C5A6 File Offset: 0x0005A7A6
		public void Subscribe(Motorway.IObserver observer)
		{
			this._observers.Subscribe(observer);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0005C5B4 File Offset: 0x0005A7B4
		public bool Unsubscribe(Motorway.IObserver observer)
		{
			return this._observers.Unsubscribe(observer);
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x0005C5C4 File Offset: 0x0005A7C4
		private void NotifyMotorwayChanged(Motorway.ChangeFlags changes)
		{
			foreach (Motorway.IObserver observer in this._observers)
			{
				observer.OnMotorwayChanged(this, changes);
			}
		}

		// Token: 0x0400159E RID: 5534
		public const int InvalidMotorwayId = -1;

		// Token: 0x0400159F RID: 5535
		public const int InvalidMotorwayNumber = 0;

		// Token: 0x040015A0 RID: 5536
		private ITilemap _tilemap;

		// Token: 0x040015A1 RID: 5537
		private int _id = -1;

		// Token: 0x040015A2 RID: 5538
		private int _number;

		// Token: 0x040015A3 RID: 5539
		private RoadState _state;

		// Token: 0x040015A4 RID: 5540
		private Vector2Int _startCoordinates;

		// Token: 0x040015A5 RID: 5541
		private TileDirection _startDirection;

		// Token: 0x040015A6 RID: 5542
		private Vector2Int _endCoordinates;

		// Token: 0x040015A7 RID: 5543
		private TileDirection _endDirection;

		// Token: 0x040015A8 RID: 5544
		private int _concreteCost;

		// Token: 0x040015A9 RID: 5545
		private int _concreteGivenToReplacement;

		// Token: 0x040015AA RID: 5546
		private Fix64 _permanenceProgress = Fix64.Zero;

		// Token: 0x040015AB RID: 5547
		[Serialize(false, null)]
		private ObserverList<Motorway.IObserver> _observers = new ObserverList<Motorway.IObserver>(1);

		// Token: 0x02000415 RID: 1045
		[Flags]
		public enum ChangeFlags
		{
			// Token: 0x040015AD RID: 5549
			Number = 1,
			// Token: 0x040015AE RID: 5550
			State = 2,
			// Token: 0x040015AF RID: 5551
			StartTile = 4,
			// Token: 0x040015B0 RID: 5552
			EndTile = 8,
			// Token: 0x040015B1 RID: 5553
			Concrete = 16,
			// Token: 0x040015B2 RID: 5554
			Permanence = 32
		}

		// Token: 0x02000416 RID: 1046
		public interface IObserver
		{
			// Token: 0x060019BC RID: 6588
			void OnMotorwayChanged(Motorway motorway, Motorway.ChangeFlags changes);
		}
	}
}
