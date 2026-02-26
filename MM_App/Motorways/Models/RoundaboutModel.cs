using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004FA RID: 1274
	public class RoundaboutModel : IModel, IReusable, Tile.IObserver, IReleasedFromScopeHandler, IDeserializedHandler
	{
		// Token: 0x17000609 RID: 1545
		// (get) Token: 0x060021C6 RID: 8646 RVA: 0x00087E57 File Offset: 0x00086057
		public IEnumerable<AdjacentTileConnection> ReplacedConnections
		{
			get
			{
				return this._replacedConnections;
			}
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x00087E60 File Offset: 0x00086060
		public void Initialize(Vector2Int originCoordinates, List<AdjacentTileConnection> replacedConnections)
		{
			this._originCoordinates = originCoordinates;
			this._replacedConnections.AddRange(replacedConnections);
			this._centerTileModel = this._tilemap.GetOrCreateTileModel(this._originCoordinates + Roundabout.GetCenterOffset());
			this._centerTileModel.Tile.Subscribe(this);
			Vector2Int referenceOffset = Roundabout.GetCoordinatesOffsets()[0];
			this._referenceConnection = Roundabout.GetConnectionForCoordinatesOffset(referenceOffset);
			this._referenceTile = this._tilemap.GetOrCreateTile(this._originCoordinates + referenceOffset);
			this._referenceTile.Subscribe(this);
			this._lastKnownState = this.State;
		}

		// Token: 0x1700060A RID: 1546
		// (get) Token: 0x060021C8 RID: 8648 RVA: 0x00087EFF File Offset: 0x000860FF
		public Vector2Int OriginCoordinates
		{
			get
			{
				return this._originCoordinates;
			}
		}

		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x060021C9 RID: 8649 RVA: 0x00087F07 File Offset: 0x00086107
		public TileModel CenterTileModel
		{
			get
			{
				return this._centerTileModel;
			}
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x060021CA RID: 8650 RVA: 0x00087F0F File Offset: 0x0008610F
		public Vector2Int CenterCoordinates
		{
			get
			{
				return this._centerTileModel.Coordinates;
			}
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x060021CB RID: 8651 RVA: 0x00087F1C File Offset: 0x0008611C
		public RoadState State
		{
			get
			{
				return this._referenceTile.GetRoundaboutState(this._referenceConnection);
			}
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x00087F30 File Offset: 0x00086130
		public bool Activate()
		{
			if (this.CanActivate)
			{
				foreach (Tile tile in Roundabout.GetTilesInRoundabout(this._referenceTile, RoadState.Planned))
				{
					RoadTileConnection plannedRoundaboutConnection = tile.GetRoundaboutConnection(RoadState.Planned);
					tile.SetRoundaboutState(plannedRoundaboutConnection, RoadState.Active);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x00087F98 File Offset: 0x00086198
		public void Reset()
		{
			this._originCoordinates = default(Vector2Int);
			this._centerTileModel = null;
			this._referenceTile = null;
			this._referenceConnection = default(RoadTileConnection);
			this._lastKnownState = RoadState.None;
			this._replacedConnections.Clear();
		}

		// Token: 0x060021CE RID: 8654 RVA: 0x00087FD4 File Offset: 0x000861D4
		public void OnReleasedFromScope(IScope scope)
		{
			Tile referenceTile = this._referenceTile;
			if (referenceTile != null)
			{
				referenceTile.Unsubscribe(this);
			}
			this._referenceTile = null;
			TileModel centerTileModel = this._centerTileModel;
			if (centerTileModel != null)
			{
				Tile tile = centerTileModel.Tile;
				if (tile != null)
				{
					tile.Unsubscribe(this);
				}
			}
			this._centerTileModel = null;
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x00088020 File Offset: 0x00086220
		public void OnDeserialized(IScope context)
		{
			if (Diagnostics.Verify(this._referenceTile != null))
			{
				this._referenceTile.Subscribe(this);
			}
			if (Diagnostics.Verify(this._centerTileModel != null))
			{
				Tile tile = this._centerTileModel.Tile;
				if (tile == null)
				{
					return;
				}
				tile.Subscribe(this);
			}
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x0008806F File Offset: 0x0008626F
		public void ClearReplacedConnections()
		{
			this._replacedConnections.Clear();
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x0008807C File Offset: 0x0008627C
		public void OnTileChanged(Tile changedTile)
		{
			if (this._centerTileModel == null || this._referenceTile == null)
			{
				Diagnostics.FailAssert(string.Format("We somehow subscribed to a tile at {0} without initializing the roundabout!", changedTile.Coordinates), Array.Empty<object>());
				return;
			}
			if (changedTile == this._centerTileModel.Tile)
			{
				if (changedTile.IsCenterOfRoundabout && changedTile.IsRoundaboutPermanent)
				{
					this._centerTileModel.Tile.Unsubscribe(this);
					this.RestoreConcreteFromStoredReplacedConnections(RoundaboutModel.ConcreteRestoreType.Release);
				}
				return;
			}
			RoadState newState = this.State;
			if (newState != this._lastKnownState)
			{
				if (newState <= RoadState.Planned)
				{
					if (newState != RoadState.None)
					{
						if (newState != RoadState.Planned)
						{
						}
					}
					else
					{
						if (this._lastKnownState == RoadState.Planned)
						{
							foreach (Tile roundaboutTile in Roundabout.GetTilesInRoundabout(this._referenceTile, this._referenceConnection))
							{
								TileModel roundaboutTileModel = this._tilemap.GetTileModel(roundaboutTile.Coordinates);
								if (Diagnostics.Verify(roundaboutTileModel != null))
								{
									RoundaboutModel.ClearHotswappingLanes(roundaboutTileModel);
								}
							}
							RoundaboutModel.ClearHotswappingLanes(this._centerTileModel);
						}
						this._centerTileModel.Tile.ResetRoundaboutPermanence();
						this._simulation.RemoveModel(this);
					}
				}
				else if (newState != RoadState.Active)
				{
					if (newState != RoadState.Mothballed)
					{
					}
				}
				else if (this._lastKnownState == RoadState.Mothballed)
				{
					foreach (Tile roundaboutTile2 in Roundabout.GetTilesInRoundabout(this._referenceTile, this._referenceConnection))
					{
						TileModel roundaboutTileModel2 = this._tilemap.GetTileModel(roundaboutTile2.Coordinates);
						if (Diagnostics.Verify(roundaboutTileModel2 != null, "A roundabout had a non-existent tile."))
						{
							foreach (LaneModel laneModel in roundaboutTileModel2.roadChunk.lanes)
							{
								if (laneModel.connection.input.type == RoadType.Roundabout || laneModel.connection.output.type == RoadType.Roundabout)
								{
									laneModel.IsAboutToHotswap = false;
									if (laneModel.connection.IsRoundabout)
									{
										foreach (LaneModel laneModel2 in laneModel.InboundLanes)
										{
											laneModel2.IsAboutToHotswap = false;
										}
										foreach (LaneModel outboundLane in laneModel.OutboundLanes)
										{
											if (!outboundLane.connection.IsRoundabout)
											{
												outboundLane.IsAboutToHotswap = false;
											}
										}
									}
								}
							}
						}
					}
				}
				this._lastKnownState = newState;
			}
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x000883A8 File Offset: 0x000865A8
		public void RestoreConcreteFromStoredReplacedConnections(RoundaboutModel.ConcreteRestoreType restoreType)
		{
			GameBehaviourModel gameBehaviourModel = this._simulation.Scope.Get<GameBehaviourModel>();
			ITilemap tilemap = this._simulation.Scope.Get<TilemapModel>();
			int concreteToRelease = 0;
			foreach (AdjacentTileConnection removedPermanentConnection in this._replacedConnections)
			{
				if ((!(removedPermanentConnection.OriginCoordinates == this._originCoordinates) && !(removedPermanentConnection.DestinationCoordinates == this._originCoordinates)) || !TileUtilities.IsDirectionDiagonal(removedPermanentConnection.DestinationDirection))
				{
					concreteToRelease += gameBehaviourModel.GetConcreteCostForConnection(tilemap.GetTile(removedPermanentConnection.OriginCoordinates), tilemap.GetTile(removedPermanentConnection.DestinationCoordinates));
				}
			}
			if (restoreType == RoundaboutModel.ConcreteRestoreType.Release)
			{
				this._simulation.GetModel<UpgradeDatabaseModel>().ReleaseMothballedUpgrade(UpgradeType.Concrete, concreteToRelease);
				return;
			}
			this._simulation.GetModel<UpgradeDatabaseModel>().UnmothballUpgrade(UpgradeType.Concrete, concreteToRelease);
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x060021D3 RID: 8659 RVA: 0x000884A0 File Offset: 0x000866A0
		private bool CanActivate
		{
			get
			{
				using (IEnumerator<Tile> enumerator = Roundabout.GetTilesInRoundabout(this._referenceTile, RoadState.Planned).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsPlannedRoundaboutBlocked)
						{
							return false;
						}
					}
				}
				return !this._centerTileModel.Tile.IsPlannedRoundaboutBlocked;
			}
		}

		// Token: 0x060021D4 RID: 8660 RVA: 0x0008850C File Offset: 0x0008670C
		private static void ClearHotswappingLanes(TileModel tile)
		{
			foreach (LaneModel lane in tile.roadChunk.lanes)
			{
				if (!lane.connection.IsRoundabout)
				{
					lane.IsAboutToHotswap = false;
					foreach (LaneModel inboundLane in lane.InboundLanes)
					{
						if (inboundLane.connection.input.type != RoadType.Roundabout && inboundLane.connection.output.type != RoadType.Roundabout && TileUtilities.IsDirectionDiagonal(inboundLane.connection.output.direction))
						{
							inboundLane.IsAboutToHotswap = false;
						}
					}
					foreach (LaneModel outboundLane in lane.OutboundLanes)
					{
						if (outboundLane.connection.input.type != RoadType.Roundabout && outboundLane.connection.output.type != RoadType.Roundabout && TileUtilities.IsDirectionDiagonal(outboundLane.connection.output.direction))
						{
							outboundLane.IsAboutToHotswap = false;
						}
					}
				}
			}
		}

		// Token: 0x04001BC1 RID: 7105
		private Vector2Int _originCoordinates;

		// Token: 0x04001BC2 RID: 7106
		private TileModel _centerTileModel;

		// Token: 0x04001BC3 RID: 7107
		private Tile _referenceTile;

		// Token: 0x04001BC4 RID: 7108
		private RoadTileConnection _referenceConnection;

		// Token: 0x04001BC5 RID: 7109
		private RoadState _lastKnownState;

		// Token: 0x04001BC6 RID: 7110
		private readonly List<AdjacentTileConnection> _replacedConnections = new List<AdjacentTileConnection>();

		// Token: 0x04001BC7 RID: 7111
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x04001BC8 RID: 7112
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x020004FB RID: 1275
		public enum ConcreteRestoreType
		{
			// Token: 0x04001BCA RID: 7114
			Unmothball,
			// Token: 0x04001BCB RID: 7115
			Release
		}
	}
}
