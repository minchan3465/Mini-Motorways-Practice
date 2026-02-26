using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Factory;
using FixMath;
using JetBrains.Annotations;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004F3 RID: 1267
	public class RailTileModel : Model<EmptyModelFrame, RailTileModel.IObserver>, IDeserializedHandler
	{
		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06002167 RID: 8551 RVA: 0x00085631 File Offset: 0x00083831
		// (set) Token: 0x06002168 RID: 8552 RVA: 0x00085639 File Offset: 0x00083839
		[Serialize(false, null)]
		public TrainSignalState SignalState { get; set; } = TrainSignalState.Open;

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06002169 RID: 8553 RVA: 0x00085642 File Offset: 0x00083842
		// (set) Token: 0x0600216A RID: 8554 RVA: 0x0008564A File Offset: 0x0008384A
		[Serialize(false, null)]
		public Fix64 Length { get; private set; }

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x0600216B RID: 8555 RVA: 0x00085653 File Offset: 0x00083853
		// (set) Token: 0x0600216C RID: 8556 RVA: 0x0008565B File Offset: 0x0008385B
		public TrainLineModel Line
		{
			get
			{
				return this._line;
			}
			set
			{
				this._line = value;
			}
		}

		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x0600216D RID: 8557 RVA: 0x00085664 File Offset: 0x00083864
		public Vector2Int Coordinates
		{
			get
			{
				return this.TileModel.Coordinates;
			}
		}

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x0600216E RID: 8558 RVA: 0x00085671 File Offset: 0x00083871
		// (set) Token: 0x0600216F RID: 8559 RVA: 0x00085679 File Offset: 0x00083879
		[Serialize(true, null)]
		public TileModel TileModel { get; private set; }

		// Token: 0x06002170 RID: 8560 RVA: 0x00085682 File Offset: 0x00083882
		[CanBeNull]
		public RailTileModel GetNextRailModelInDirection(RailDirection direction)
		{
			if (direction != RailDirection.Forwards)
			{
				return this.PreviousRailModel;
			}
			return this.NextRailModel;
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x00085694 File Offset: 0x00083894
		[CanBeNull]
		public RailTileModel GetPreviousRailModelInDirection(RailDirection direction)
		{
			if (direction != RailDirection.Forwards)
			{
				return this.NextRailModel;
			}
			return this.PreviousRailModel;
		}

		// Token: 0x170005FC RID: 1532
		// (get) Token: 0x06002172 RID: 8562 RVA: 0x000856A8 File Offset: 0x000838A8
		[CanBeNull]
		public RailTileModel PreviousRailModel
		{
			get
			{
				TileDirection inputDirection = this.TileModel.Tile.RailConnection.input;
				if (inputDirection == TileDirection.None)
				{
					return null;
				}
				TileModel adjacentTileModel = this.TileModel.GetAdjacentTileModelInDirection(inputDirection);
				if (adjacentTileModel == null)
				{
					return null;
				}
				return adjacentTileModel.RailTileModel;
			}
		}

		// Token: 0x170005FD RID: 1533
		// (get) Token: 0x06002173 RID: 8563 RVA: 0x000856EC File Offset: 0x000838EC
		[CanBeNull]
		public RailTileModel NextRailModel
		{
			get
			{
				TileDirection outputDirection = this.TileModel.Tile.RailConnection.output;
				if (outputDirection == TileDirection.None)
				{
					return null;
				}
				TileModel adjacentTileModel = this.TileModel.GetAdjacentTileModelInDirection(outputDirection);
				if (adjacentTileModel == null)
				{
					return null;
				}
				return adjacentTileModel.RailTileModel;
			}
		}

		// Token: 0x06002174 RID: 8564 RVA: 0x00085730 File Offset: 0x00083930
		[return: TupleElementNames(new string[]
		{
			"destination",
			"distanceAlongDestination",
			"totalDistanceTraversed"
		})]
		public ValueTuple<RailTileModel, Fix64, Fix64> Traverse(Fix64 originDistance, Fix64 distanceToTraverse, RailDirection traversalDirection)
		{
			Fix64 distanceTraversed = Fix64.Zero;
			RailTileModel cursor = this;
			Fix64 cursorPosition = originDistance;
			while (distanceToTraverse > Fix64.Zero)
			{
				if (traversalDirection == RailDirection.Forwards)
				{
					Fix64 distanceLeftOnCursor = cursor.Length - cursorPosition;
					if (distanceLeftOnCursor > distanceToTraverse)
					{
						distanceTraversed += distanceToTraverse;
						cursorPosition += distanceToTraverse;
						return new ValueTuple<RailTileModel, Fix64, Fix64>(cursor, cursorPosition, distanceTraversed);
					}
					RailTileModel nextCursor = cursor.NextRailModel;
					if (nextCursor == null)
					{
						return new ValueTuple<RailTileModel, Fix64, Fix64>(cursor, cursor.Length, distanceTraversed + distanceLeftOnCursor);
					}
					distanceToTraverse -= distanceLeftOnCursor;
					distanceTraversed += distanceLeftOnCursor;
					cursor = nextCursor;
					cursorPosition = Fix64.Zero;
				}
				else
				{
					Fix64 distanceLeftOnCursor2 = cursorPosition;
					if (distanceLeftOnCursor2 > distanceToTraverse)
					{
						distanceTraversed += distanceToTraverse;
						cursorPosition -= distanceToTraverse;
						return new ValueTuple<RailTileModel, Fix64, Fix64>(cursor, cursorPosition, distanceTraversed);
					}
					RailTileModel nextCursor2 = cursor.PreviousRailModel;
					if (nextCursor2 == null)
					{
						return new ValueTuple<RailTileModel, Fix64, Fix64>(cursor, Fix64.Zero, distanceTraversed + distanceLeftOnCursor2);
					}
					distanceToTraverse -= distanceLeftOnCursor2;
					distanceTraversed += distanceLeftOnCursor2;
					cursor = nextCursor2;
					cursorPosition = cursor.Length;
				}
			}
			Diagnostics.FailAssert("RailTileModel.Traverse failed to complete its traversal. This should never happen!", Array.Empty<object>());
			return new ValueTuple<RailTileModel, Fix64, Fix64>(this, originDistance, Fix64.Zero);
		}

		// Token: 0x06002175 RID: 8565 RVA: 0x00085848 File Offset: 0x00083A48
		public Fix64 DistanceTo(Fix64 originPosition, [NotNull] RailTileModel targetRail, Fix64 positionOnTargetRail, RailDirection direction)
		{
			if (this == targetRail)
			{
				Fix64 distanceToTargetOnSameRail = positionOnTargetRail - originPosition;
				if (direction == RailDirection.Backwards)
				{
					distanceToTargetOnSameRail = -distanceToTargetOnSameRail;
				}
				if (distanceToTargetOnSameRail >= Fix64.Zero)
				{
					return distanceToTargetOnSameRail;
				}
				if (!this._line.IsLoop)
				{
					return RailTileModel.InvalidDistance;
				}
			}
			Fix64 distanceToTarget = (direction == RailDirection.Forwards) ? (this.Length - originPosition) : originPosition;
			for (RailTileModel trackCursor = this.GetNextRailModelInDirection(direction); trackCursor != null; trackCursor = trackCursor.GetNextRailModelInDirection(direction))
			{
				if (trackCursor == targetRail)
				{
					return distanceToTarget + ((direction == RailDirection.Forwards) ? positionOnTargetRail : (targetRail.Length - positionOnTargetRail));
				}
				if (trackCursor == this)
				{
					return RailTileModel.InvalidDistance;
				}
				distanceToTarget += trackCursor.Length;
			}
			return RailTileModel.InvalidDistance;
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x000858F8 File Offset: 0x00083AF8
		public IEnumerable<RoadChunkModel> GetRoadChunksInDirection(RailDirection direction)
		{
			yield return this.TileModel.roadChunk;
			RailTileConnection railConnection = this.TileModel.Tile.RailConnection;
			TileDirection nextTileDirection = (direction == RailDirection.Forwards) ? railConnection.output : railConnection.input;
			if (TileUtilities.IsDirectionDiagonal(nextTileDirection))
			{
				TileCornerModel cornerModel = this.TileModel.GetAdjacentTileCornerModelInDirection(nextTileDirection);
				if (cornerModel != null)
				{
					yield return cornerModel.roadChunk;
				}
			}
			yield break;
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x00085910 File Offset: 0x00083B10
		public void Initialize(TileModel tileModel)
		{
			this.TileModel = tileModel;
			RailTileDefinition definition = this._railTileAtlas.GetDefinition(tileModel.Tile.RailConnection);
			if (Diagnostics.Verify(definition != null))
			{
				this.Length = definition.path.Length;
			}
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x00085957 File Offset: 0x00083B57
		public void SetTrainStation(DestinationModel trainStation)
		{
			Diagnostics.Log.Info("RailTileModel", "Adding station {0} to rail {1}", new object[]
			{
				trainStation,
				this
			});
			this._attachedTrainStation = trainStation;
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x0008597D File Offset: 0x00083B7D
		public void RemoveTrainStation()
		{
			Diagnostics.Log.Info("RailTileModel", "Removing station {0} from rail {1}", new object[]
			{
				this._attachedTrainStation,
				this
			});
			this._attachedTrainStation = null;
		}

		// Token: 0x170005FE RID: 1534
		// (get) Token: 0x0600217A RID: 8570 RVA: 0x000859A8 File Offset: 0x00083BA8
		public DestinationModel Station
		{
			get
			{
				return this._attachedTrainStation;
			}
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x000859B0 File Offset: 0x00083BB0
		public override void Reset()
		{
			base.Reset();
			this.SignalState = TrainSignalState.Open;
			this.Length = Fix64.Zero;
			this._line = null;
			this.TileModel = null;
			this._attachedTrainStation = null;
			this.carpark = null;
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x000859E6 File Offset: 0x00083BE6
		public override string ToString()
		{
			return string.Format("[RailTileModel Coordinates={0} Length={1}]", this.Coordinates, this.Length);
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x00085A08 File Offset: 0x00083C08
		public void OnDeserialized(IScope context)
		{
			RailTileDefinition definition = this._railTileAtlas.GetDefinition(this.TileModel.Tile.RailConnection);
			if (Diagnostics.Verify(definition != null))
			{
				this.Length = definition.path.Length;
			}
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x00085A4D File Offset: 0x00083C4D
		public RailTileModel() : base(1)
		{
		}

		// Token: 0x04001B8E RID: 7054
		public static readonly Fix64 InvalidDistance = -Fix64.One;

		// Token: 0x04001B91 RID: 7057
		private TrainLineModel _line;

		// Token: 0x04001B92 RID: 7058
		private DestinationModel _attachedTrainStation;

		// Token: 0x04001B93 RID: 7059
		public CarparkModel carpark;

		// Token: 0x04001B95 RID: 7061
		[Dependency]
		private RailTileAtlas _railTileAtlas;

		// Token: 0x020004F4 RID: 1268
		public interface IObserver
		{
		}
	}
}
