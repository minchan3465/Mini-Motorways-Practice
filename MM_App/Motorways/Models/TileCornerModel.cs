using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004FF RID: 1279
	public class TileCornerModel : Model<EmptyModelFrame, IEmptyModelObserver>
	{
		// Token: 0x060021EA RID: 8682 RVA: 0x000889BC File Offset: 0x00086BBC
		public void Initialize(Vector2Fixed worldPosition, List<CornerAdjacencyReference> adjacencyReferences)
		{
			this.roadChunk = this._scope.Get<RoadChunkModel>();
			this.roadChunk.isTileCorner = true;
			this._simulation.AddModel(this.roadChunk);
			this._worldPosition = worldPosition;
			this._adjacencyReferences.AddRange(adjacencyReferences);
		}

		// Token: 0x060021EB RID: 8683 RVA: 0x00088A0C File Offset: 0x00086C0C
		public LaneModel AddLane(RoadTileConnection connection, RoadTileDefinition tileDefinition, RoadState initialState)
		{
			LaneModel newLane = this.roadChunk.AddLane(connection, tileDefinition, initialState, this._worldPosition, false);
			RoadChunkModel adjacentRoadChunkModelInDirection = this.GetAdjacentRoadChunkModelInDirection(connection.input.direction);
			if (adjacentRoadChunkModelInDirection != null)
			{
				adjacentRoadChunkModelInDirection.ConnectOutboundLane(newLane);
			}
			RoadChunkModel adjacentRoadChunkModelInDirection2 = this.GetAdjacentRoadChunkModelInDirection(connection.output.direction);
			if (adjacentRoadChunkModelInDirection2 != null)
			{
				adjacentRoadChunkModelInDirection2.ConnectInboundLane(newLane);
			}
			return newLane;
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x00088A6C File Offset: 0x00086C6C
		public RoadTileSignature CreateTileSignature()
		{
			RoadTileSignature cornerSignature = this._scope.Get<RoadTileSignature>();
			TileDirection roundaboutInputDirection = TileDirection.None;
			foreach (CornerAdjacencyReference adjacencyReference in this._adjacencyReferences)
			{
				Tile tile = this._tilemap.GetTile(adjacencyReference.tileCoordinate);
				if (tile != null && tile.HasRoundabout(RoadState.Active))
				{
					RoadTileConnection roundaboutConnection = tile.GetRoundaboutConnection(RoadState.Active);
					if (roundaboutConnection.output.direction == adjacencyReference.cornerDirection)
					{
						roundaboutInputDirection = TileUtilities.GetOppositeDirection(roundaboutConnection.output.direction);
					}
				}
			}
			if (roundaboutInputDirection != TileDirection.None)
			{
				cornerSignature.AddConnection(new RoadTileConnection(new RoadTileNode(roundaboutInputDirection, RoadType.Roundabout, -1), new RoadTileNode(TileUtilities.GetOppositeDirection(roundaboutInputDirection), RoadType.Roundabout, -1)));
			}
			foreach (CornerAdjacencyReference adjacencyReference2 in this._adjacencyReferences)
			{
				Tile tile2 = this._tilemap.GetTile(adjacencyReference2.tileCoordinate);
				if (tile2 != null && tile2.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore)[adjacencyReference2.cornerDirection] && (roundaboutInputDirection == TileDirection.None || adjacencyReference2.cornerDirection != TileUtilities.GetRotatedDirection(roundaboutInputDirection, -2)))
				{
					cornerSignature.AddNode(new RoadTileNode(TileUtilities.GetOppositeDirection(adjacencyReference2.cornerDirection), RoadType.TwoLane, -1));
				}
			}
			return cornerSignature;
		}

		// Token: 0x060021ED RID: 8685 RVA: 0x00088BDC File Offset: 0x00086DDC
		private TileModel GetAdjacentTileModelInDirection(TileDirection direction)
		{
			if (!TileUtilities.IsDirectionDiagonal(direction))
			{
				return null;
			}
			foreach (CornerAdjacencyReference cornerDefinition in this._adjacencyReferences)
			{
				if (TileUtilities.GetOppositeDirection(cornerDefinition.cornerDirection) == direction)
				{
					return this._tilemap.GetTileModel(cornerDefinition.tileCoordinate);
				}
			}
			return null;
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x00088C58 File Offset: 0x00086E58
		private RoadChunkModel GetAdjacentRoadChunkModelInDirection(TileDirection direction)
		{
			TileModel adjacentTileModelInDirection = this.GetAdjacentTileModelInDirection(direction);
			if (adjacentTileModelInDirection == null)
			{
				return null;
			}
			return adjacentTileModelInDirection.roadChunk;
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x00088C6C File Offset: 0x00086E6C
		public override void Reset()
		{
			base.Reset();
			this._tilemap = null;
			this._adjacencyReferences.Clear();
			this.roadChunk = null;
			this._worldPosition = default(Vector2Fixed);
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x00088C99 File Offset: 0x00086E99
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			if (this.roadChunk != null)
			{
				scope.Release(this.roadChunk);
				this.roadChunk = null;
			}
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x00088CBE File Offset: 0x00086EBE
		public TileCornerModel() : base(1)
		{
		}

		// Token: 0x04001BD5 RID: 7125
		[Dependency]
		private IScope _scope;

		// Token: 0x04001BD6 RID: 7126
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001BD7 RID: 7127
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001BD8 RID: 7128
		private List<CornerAdjacencyReference> _adjacencyReferences = new List<CornerAdjacencyReference>();

		// Token: 0x04001BD9 RID: 7129
		private Vector2Fixed _worldPosition;

		// Token: 0x04001BDA RID: 7130
		public RoadChunkModel roadChunk;
	}
}
