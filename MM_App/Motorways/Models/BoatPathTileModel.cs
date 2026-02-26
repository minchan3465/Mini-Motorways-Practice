using System;
using System.Runtime.CompilerServices;
using Factory;
using FixMath;
using JetBrains.Annotations;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004D3 RID: 1235
	public class BoatPathTileModel : Model<EmptyModelFrame, BoatPathTileModel.IObserver>, IDeserializedHandler
	{
		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x0600202D RID: 8237 RVA: 0x0007E709 File Offset: 0x0007C909
		// (set) Token: 0x0600202E RID: 8238 RVA: 0x0007E711 File Offset: 0x0007C911
		[Serialize(false, null)]
		public Fix64 Length { get; private set; }

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x0600202F RID: 8239 RVA: 0x0007E71A File Offset: 0x0007C91A
		// (set) Token: 0x06002030 RID: 8240 RVA: 0x0007E722 File Offset: 0x0007C922
		public BoatPathModel BoatPath
		{
			get
			{
				return this._boatPath;
			}
			set
			{
				this._boatPath = value;
			}
		}

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x06002031 RID: 8241 RVA: 0x0007E72B File Offset: 0x0007C92B
		public Vector2Int Coordinates
		{
			get
			{
				return this.TileModel.Coordinates;
			}
		}

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x06002032 RID: 8242 RVA: 0x0007E738 File Offset: 0x0007C938
		// (set) Token: 0x06002033 RID: 8243 RVA: 0x0007E740 File Offset: 0x0007C940
		[Serialize(true, null)]
		public TileModel TileModel { get; private set; }

		// Token: 0x06002034 RID: 8244 RVA: 0x0007E749 File Offset: 0x0007C949
		[CanBeNull]
		public BoatPathTileModel GetNextBoatPathModelInDirection(BoatModel.BoatDirection direction)
		{
			if (direction != BoatModel.BoatDirection.Forwards)
			{
				return this.PreviousBoatPathModel;
			}
			return this.NextBoatPathModel;
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06002035 RID: 8245 RVA: 0x0007E75C File Offset: 0x0007C95C
		[CanBeNull]
		public BoatPathTileModel PreviousBoatPathModel
		{
			get
			{
				TileDirection inputDirection = this.TileModel.Tile.BoatPathConnection.input;
				if (inputDirection == TileDirection.None)
				{
					return null;
				}
				TileModel adjacentTileModel = this.TileModel.GetAdjacentTileModelInDirection(inputDirection);
				if (adjacentTileModel == null)
				{
					return null;
				}
				return adjacentTileModel.BoatPathTileModel;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06002036 RID: 8246 RVA: 0x0007E7A0 File Offset: 0x0007C9A0
		[CanBeNull]
		public BoatPathTileModel NextBoatPathModel
		{
			get
			{
				TileDirection outputDirection = this.TileModel.Tile.BoatPathConnection.output;
				if (outputDirection == TileDirection.None)
				{
					return null;
				}
				TileModel adjacentTileModel = this.TileModel.GetAdjacentTileModelInDirection(outputDirection);
				if (adjacentTileModel == null)
				{
					return null;
				}
				return adjacentTileModel.BoatPathTileModel;
			}
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x0007E7E4 File Offset: 0x0007C9E4
		[return: TupleElementNames(new string[]
		{
			"destination",
			"distanceAlongDestination"
		})]
		public ValueTuple<BoatPathTileModel, Fix64> Traverse(Fix64 originDistance, Fix64 distanceToTraverse)
		{
			BoatPathTileModel cursor = this;
			Fix64 cursorPosition = originDistance;
			while (distanceToTraverse > Fix64.Zero)
			{
				Fix64 distanceLeftOnCursor = cursor.Length - cursorPosition;
				if (distanceLeftOnCursor > distanceToTraverse)
				{
					cursorPosition += distanceToTraverse;
					return new ValueTuple<BoatPathTileModel, Fix64>(cursor, cursorPosition);
				}
				BoatPathTileModel nextCursor = cursor.NextBoatPathModel;
				if (nextCursor == null)
				{
					return new ValueTuple<BoatPathTileModel, Fix64>(cursor, cursor.Length);
				}
				distanceToTraverse -= distanceLeftOnCursor;
				cursor = nextCursor;
				cursorPosition = Fix64.Zero;
			}
			Diagnostics.FailAssert("BoatPathTileModel.Traverse failed to complete its traversal. This should never happen!", Array.Empty<object>());
			return new ValueTuple<BoatPathTileModel, Fix64>(this, originDistance);
		}

		// Token: 0x06002038 RID: 8248 RVA: 0x0007E868 File Offset: 0x0007CA68
		public Fix64 DistanceTo(Fix64 originPosition, [NotNull] BoatPathTileModel targetBoatPath, Fix64 positionOnTargetBoatPath, BoatModel.BoatDirection direction)
		{
			if (this == targetBoatPath)
			{
				Fix64 distanceToTargetOnSameRail = positionOnTargetBoatPath - originPosition;
				if (distanceToTargetOnSameRail >= Fix64.Zero)
				{
					return distanceToTargetOnSameRail;
				}
				if (!this._boatPath.IsLoop)
				{
					return BoatPathTileModel.InvalidDistance;
				}
			}
			Fix64 distanceToTarget = this.Length - originPosition;
			for (BoatPathTileModel trackCursor = this.GetNextBoatPathModelInDirection(direction); trackCursor != null; trackCursor = trackCursor.GetNextBoatPathModelInDirection(direction))
			{
				if (trackCursor == targetBoatPath)
				{
					return distanceToTarget + positionOnTargetBoatPath;
				}
				if (trackCursor == this)
				{
					return BoatPathTileModel.InvalidDistance;
				}
				distanceToTarget += trackCursor.Length;
			}
			return BoatPathTileModel.InvalidDistance;
		}

		// Token: 0x06002039 RID: 8249 RVA: 0x0007E8F4 File Offset: 0x0007CAF4
		public CarparkModel GetFirstTerminal(Fix64 currentBoatTraversal, Fix64 boatCenterToBowDistance, BoatModel.BoatDirection boatDirection, out Fix64 distanceToTerminal)
		{
			BoatPathTileModel currentCursor = this;
			distanceToTerminal = default(Fix64);
			while (currentCursor.carpark == null)
			{
				currentCursor = currentCursor.NextBoatPathModel;
				if (currentCursor == null || currentCursor == this)
				{
					currentCursor = this;
					while (currentCursor.carpark == null)
					{
						currentCursor = currentCursor.PreviousBoatPathModel;
						if (currentCursor == null || currentCursor == this)
						{
							return null;
						}
					}
					ValueTuple<BoatPathTileModel, Fix64> traversal = this.Traverse(this.Length / Fix64Consts.Two, boatCenterToBowDistance);
					distanceToTerminal = this.DistanceTo(currentBoatTraversal, currentCursor, traversal.Item2, boatDirection);
					return currentCursor.carpark;
				}
			}
			ValueTuple<BoatPathTileModel, Fix64> traversal2 = this.Traverse(this.Length / Fix64Consts.Two, boatCenterToBowDistance);
			distanceToTerminal = this.DistanceTo(currentBoatTraversal, currentCursor, traversal2.Item2, boatDirection);
			return currentCursor.carpark;
		}

		// Token: 0x0600203A RID: 8250 RVA: 0x0007E9A4 File Offset: 0x0007CBA4
		public void Initialize(TileModel tileModel)
		{
			this.TileModel = tileModel;
			BoatPathTileDefinition definition = this._boatTileAtlas.GetDefinition(tileModel.Tile.BoatPathConnection);
			if (Diagnostics.Verify(definition != null))
			{
				this.Length = definition.path.Length;
			}
		}

		// Token: 0x0600203B RID: 8251 RVA: 0x0007E9EB File Offset: 0x0007CBEB
		public override void Reset()
		{
			base.Reset();
			this.Length = Fix64.Zero;
			this._boatPath = null;
			this.TileModel = null;
			this.carpark = null;
		}

		// Token: 0x0600203C RID: 8252 RVA: 0x0007EA13 File Offset: 0x0007CC13
		public override string ToString()
		{
			return string.Format("[BoatPathTileModel Coordinates={0} Length={1}]", this.Coordinates, this.Length);
		}

		// Token: 0x0600203D RID: 8253 RVA: 0x0007EA38 File Offset: 0x0007CC38
		public void OnDeserialized(IScope context)
		{
			BoatPathTileDefinition definition = this._boatTileAtlas.GetDefinition(this.TileModel.Tile.BoatPathConnection);
			if (Diagnostics.Verify(definition != null))
			{
				this.Length = definition.path.Length;
			}
		}

		// Token: 0x0600203E RID: 8254 RVA: 0x0007EA7D File Offset: 0x0007CC7D
		public BoatPathTileModel() : base(1)
		{
		}

		// Token: 0x04001AB9 RID: 6841
		public static readonly Fix64 InvalidDistance = -Fix64.One;

		// Token: 0x04001ABB RID: 6843
		private BoatPathModel _boatPath;

		// Token: 0x04001ABC RID: 6844
		public CarparkModel carpark;

		// Token: 0x04001ABE RID: 6846
		[Dependency]
		private BoatPathTileAtlas _boatTileAtlas;

		// Token: 0x020004D4 RID: 1236
		public interface IObserver
		{
		}
	}
}
