using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x02000502 RID: 1282
	public class TileModel : Model<EmptyModelFrame, TileModel.IObserver>, Tile.IObserver, IDeserializedHandler
	{
		// Token: 0x06002210 RID: 8720 RVA: 0x0008999C File Offset: 0x00087B9C
		public void Initialize(Vector2Int coordinates)
		{
			this.roadChunk = this._scope.Get<RoadChunkModel>();
			this._simulation.AddModel(this.roadChunk);
			this._tile = this._scope.Get<Tile>();
			this._tile.Initialize(this._tilemap, coordinates, TileContentType.None);
			this._tile.Subscribe(this);
			base.Subscribe(this._behaviour);
			this._worldPosition = TilemapModel.GetWorldPositionForCoordinates(coordinates);
		}

		// Token: 0x17000615 RID: 1557
		// (get) Token: 0x06002211 RID: 8721 RVA: 0x00089A14 File Offset: 0x00087C14
		public Vector2Int Coordinates
		{
			get
			{
				return this._tile.Coordinates;
			}
		}

		// Token: 0x17000616 RID: 1558
		// (get) Token: 0x06002212 RID: 8722 RVA: 0x00089A21 File Offset: 0x00087C21
		public Tile Tile
		{
			get
			{
				return this._tile;
			}
		}

		// Token: 0x17000617 RID: 1559
		// (get) Token: 0x06002213 RID: 8723 RVA: 0x00089A29 File Offset: 0x00087C29
		public RailTileModel RailTileModel
		{
			get
			{
				return this._railTileModel;
			}
		}

		// Token: 0x17000618 RID: 1560
		// (get) Token: 0x06002214 RID: 8724 RVA: 0x00089A31 File Offset: 0x00087C31
		public BoatPathTileModel BoatPathTileModel
		{
			get
			{
				return this._boatPathTileModel;
			}
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x00089A3C File Offset: 0x00087C3C
		public void OnTileChanged(Tile changedTile)
		{
			foreach (TileModel.IObserver observer in base.Observers)
			{
				observer.OnTileModelChanged(this);
			}
			if (changedTile.HasRailConnection)
			{
				if (this._railTileModel == null)
				{
					this._railTileModel = this._scope.Get<RailTileModel>();
					this._railTileModel.Initialize(this);
					this._simulation.AddModel(this._railTileModel);
				}
			}
			else
			{
				RailTileModel railTileModel = this._railTileModel;
			}
			if (changedTile.HasBoatPathConnection)
			{
				if (this._boatPathTileModel == null)
				{
					this._boatPathTileModel = this._scope.Get<BoatPathTileModel>();
					this._boatPathTileModel.Initialize(this);
					this._simulation.AddModel(this._boatPathTileModel);
				}
			}
			else
			{
				BoatPathTileModel boatPathTileModel = this._boatPathTileModel;
			}
			if (changedTile.HasTrafficLight)
			{
				if (this.roadChunk.TrafficLight == null)
				{
					this.roadChunk.TrafficLight = this._simulation.Scope.Get<TrafficLightModel>();
					this.roadChunk.TrafficLight.Initialize(this.roadChunk);
					this._simulation.AddModel(this.roadChunk.TrafficLight);
					return;
				}
			}
			else if (this.roadChunk.TrafficLight != null)
			{
				this._simulation.RemoveModel(this.roadChunk.TrafficLight);
				this.roadChunk.TrafficLight = null;
			}
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x00089B8B File Offset: 0x00087D8B
		public void RemoveTrainCrossing()
		{
			if (this.roadChunk.TrainCrossingModel != null)
			{
				this._simulation.RemoveModel(this.roadChunk.TrainCrossingModel);
				this.roadChunk.TrainCrossingModel = null;
			}
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x00089BC0 File Offset: 0x00087DC0
		public LaneModel AddLane(RoadTileConnection connection, RoadTileDefinition tileDefinition, RoadState initialState, bool isEndpointLane)
		{
			LaneModel newLane = this.roadChunk.AddLane(connection, tileDefinition, initialState, this.WorldPosition, isEndpointLane);
			if (connection.input.type != RoadType.Motorway)
			{
				RoadChunkModel adjacentRoadChunkModelInDirection = this.GetAdjacentRoadChunkModelInDirection(connection.input.direction);
				if (adjacentRoadChunkModelInDirection != null)
				{
					adjacentRoadChunkModelInDirection.ConnectOutboundLane(newLane);
				}
			}
			if (connection.output.type != RoadType.Motorway)
			{
				RoadChunkModel adjacentRoadChunkModelInDirection2 = this.GetAdjacentRoadChunkModelInDirection(connection.output.direction);
				if (adjacentRoadChunkModelInDirection2 != null)
				{
					adjacentRoadChunkModelInDirection2.ConnectInboundLane(newLane);
				}
			}
			return newLane;
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x00089C3C File Offset: 0x00087E3C
		public TileModel GetAdjacentTileModelInDirection(TileDirection directionToCheck)
		{
			Vector2Int adjacentCoordinates = TileUtilities.GetAdjacentCoordinates(this.Coordinates, directionToCheck);
			return this._tilemap.GetTileModel(adjacentCoordinates);
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x00089C64 File Offset: 0x00087E64
		public TileCornerModel GetAdjacentTileCornerModelInDirection(TileDirection directionToCheck)
		{
			CornerAdjacencyReference cornerAdjacencyReference = new CornerAdjacencyReference(this.Coordinates, directionToCheck);
			return this._tilemap.GetTileCornerModel(cornerAdjacencyReference);
		}

		// Token: 0x17000619 RID: 1561
		// (get) Token: 0x0600221A RID: 8730 RVA: 0x00089C8B File Offset: 0x00087E8B
		public Vector2Fixed WorldPosition
		{
			get
			{
				return this._worldPosition;
			}
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x00089C93 File Offset: 0x00087E93
		public override void Reset()
		{
			base.Reset();
			this._worldPosition = Vector2Fixed.zero;
			this._railTileModel = null;
			this._boatPathTileModel = null;
			this.roadChunk = null;
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x00089CBC File Offset: 0x00087EBC
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			if (this._tile != null)
			{
				scope.Release(this._tile);
				this._tile = null;
			}
			if (this.roadChunk != null)
			{
				scope.Release(this.roadChunk);
				this.roadChunk = null;
			}
			if (this._behaviour != null)
			{
				base.Unsubscribe(this._behaviour);
			}
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x00089D1D File Offset: 0x00087F1D
		public void OnDeserialized(IScope context)
		{
			if (Diagnostics.Verify(this._tile != null))
			{
				this._tile.Subscribe(this);
			}
			if (this._behaviour != null)
			{
				base.Subscribe(this._behaviour);
			}
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x00089D4F File Offset: 0x00087F4F
		public override string ToString()
		{
			if (this._tile == null)
			{
				return "[TileModel]";
			}
			return string.Format("[TileModel Coordinates={0}]", this._tile.Coordinates);
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x00089D7C File Offset: 0x00087F7C
		public bool AreAllLanesInDirectionUnused(TileDirection direction, RoadState state = RoadState.Active)
		{
			using (List<LaneModel>.Enumerator enumerator = this.roadChunk.GetLanesConnectedToDirection(state, direction).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.hasBeenUsed)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002220 RID: 8736 RVA: 0x00089DDC File Offset: 0x00087FDC
		private RoadChunkModel GetAdjacentRoadChunkModelInDirection(TileDirection directionToCheck)
		{
			if (TileUtilities.IsDirectionDiagonal(directionToCheck))
			{
				TileCornerModel adjacentTileCornerModelInDirection = this.GetAdjacentTileCornerModelInDirection(directionToCheck);
				if (adjacentTileCornerModelInDirection == null)
				{
					return null;
				}
				return adjacentTileCornerModelInDirection.roadChunk;
			}
			else
			{
				TileModel adjacentTileModelInDirection = this.GetAdjacentTileModelInDirection(directionToCheck);
				if (adjacentTileModelInDirection == null)
				{
					return null;
				}
				return adjacentTileModelInDirection.roadChunk;
			}
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x00089E0B File Offset: 0x0008800B
		public TileModel() : base(1)
		{
		}

		// Token: 0x04001BF0 RID: 7152
		[Dependency]
		private IScope _scope;

		// Token: 0x04001BF1 RID: 7153
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001BF2 RID: 7154
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001BF3 RID: 7155
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04001BF4 RID: 7156
		private Tile _tile;

		// Token: 0x04001BF5 RID: 7157
		private Vector2Fixed _worldPosition;

		// Token: 0x04001BF6 RID: 7158
		public RoadChunkModel roadChunk;

		// Token: 0x04001BF7 RID: 7159
		private RailTileModel _railTileModel;

		// Token: 0x04001BF8 RID: 7160
		private BoatPathTileModel _boatPathTileModel;

		// Token: 0x02000503 RID: 1283
		public interface IObserver
		{
			// Token: 0x06002222 RID: 8738
			void OnTileModelChanged(TileModel model);
		}
	}
}
