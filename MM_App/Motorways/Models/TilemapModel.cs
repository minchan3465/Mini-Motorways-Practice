using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using JetBrains.Annotations;
using Server;
using Unity.Profiling;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x02000500 RID: 1280
	public class TilemapModel : Model<EmptyModelFrame, IEmptyModelObserver>, ITilemap, TileModel.IObserver, IDeserializedHandler
	{
		// Token: 0x060021F2 RID: 8690 RVA: 0x00088CD2 File Offset: 0x00086ED2
		[CanBeNull]
		public Motorway GetMotorway(int id)
		{
			return this.GetMotorwayModel(id);
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x00088CDC File Offset: 0x00086EDC
		public Motorway CreateMotorway(int id, int number, int replacedMotorwayId)
		{
			if (Diagnostics.Verify(!this._motorways.ContainsKey(id), "Motorway model already created."))
			{
				MotorwayModel motorway = this._scope.Get<MotorwayModel>();
				motorway.Initialize(this, id, number, RoadState.None);
				this._motorways.Add(id, motorway);
				this._simulation.AddModel(motorway);
				return motorway;
			}
			return null;
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x00088D38 File Offset: 0x00086F38
		[CanBeNull]
		public MotorwayModel GetMotorwayModel(int id)
		{
			MotorwayModel motorway;
			if (this._motorways.TryGetValue(id, out motorway))
			{
				return motorway;
			}
			return null;
		}

		// Token: 0x060021F5 RID: 8693 RVA: 0x00088D58 File Offset: 0x00086F58
		public bool RemoveMotorwayModel(MotorwayModel motorwayModel)
		{
			if (this.GetMotorwayModel(motorwayModel.Id) == motorwayModel)
			{
				this._motorways.Remove(motorwayModel.Id);
				return true;
			}
			return false;
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x00088D7E File Offset: 0x00086F7E
		[CanBeNull]
		public Tile GetTile(Vector2Int coordinates)
		{
			TileModel tileModel = this.GetTileModel(coordinates);
			if (tileModel == null)
			{
				return null;
			}
			return tileModel.Tile;
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x00088D92 File Offset: 0x00086F92
		[NotNull]
		public Tile GetOrCreateTile(Vector2Int coordinates)
		{
			TileModel orCreateTileModel = this.GetOrCreateTileModel(coordinates);
			if (orCreateTileModel == null)
			{
				return null;
			}
			return orCreateTileModel.Tile;
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x00088DA8 File Offset: 0x00086FA8
		[CanBeNull]
		public TileModel GetTileModel(Vector2Int coordinates)
		{
			TileModel tile;
			if (this._tiles.TryGetValue(coordinates, out tile))
			{
				return tile;
			}
			return null;
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x00088DC8 File Offset: 0x00086FC8
		[NotNull]
		public TileModel GetOrCreateTileModel(Vector2Int coordinates)
		{
			TileModel tile = this.GetTileModel(coordinates);
			if (tile != null)
			{
				return tile;
			}
			tile = this._scope.Get<TileModel>();
			tile.Initialize(coordinates);
			tile.Subscribe(this);
			this._tiles[coordinates] = tile;
			this._simulation.AddModel(tile);
			return tile;
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x00088E18 File Offset: 0x00087018
		[CanBeNull]
		public TileCornerModel GetTileCornerModel(CornerAdjacencyReference cornerDefinition)
		{
			TileCornerModel tileCorner;
			if (this._tileCorners.TryGetValue(cornerDefinition, out tileCorner))
			{
				return tileCorner;
			}
			return null;
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x00088E38 File Offset: 0x00087038
		[NotNull]
		public TileCornerModel GetOrCreateTileCornerModel(CornerAdjacencyReference cornerDefinition)
		{
			TileCornerModel tileCorner = this.GetTileCornerModel(cornerDefinition);
			if (tileCorner != null)
			{
				return tileCorner;
			}
			Vector2Int position = cornerDefinition.tileCoordinate;
			Vector2Int direction = TileUtilities.DirectionToTileAdjacencyOffset[(int)cornerDefinition.cornerDirection];
			List<CornerAdjacencyReference> cornerAdjacencyReferences = new List<CornerAdjacencyReference>();
			cornerAdjacencyReferences.Add(cornerDefinition);
			TileDirection adjacentDirectionA = TileUtilities.GetClosestDirection(new Vector2Fixed((float)(-(float)direction.x), (float)direction.y));
			CornerAdjacencyReference adjacentCornerA = new CornerAdjacencyReference(position + new Vector2Int(direction.x, 0), adjacentDirectionA);
			cornerAdjacencyReferences.Add(adjacentCornerA);
			TileDirection adjacentDirectionB = TileUtilities.GetClosestDirection(new Vector2Fixed((float)direction.x, (float)(-(float)direction.y)));
			CornerAdjacencyReference adjacentCornerB = new CornerAdjacencyReference(position + new Vector2Int(0, direction.y), adjacentDirectionB);
			cornerAdjacencyReferences.Add(adjacentCornerB);
			TileDirection oppositeDirection = TileUtilities.GetClosestDirection(new Vector2Fixed((float)(-(float)direction.x), (float)(-(float)direction.y)));
			CornerAdjacencyReference oppositeCorner = new CornerAdjacencyReference(position + new Vector2Int(direction.x, direction.y), oppositeDirection);
			cornerAdjacencyReferences.Add(oppositeCorner);
			tileCorner = this._scope.Get<TileCornerModel>();
			tileCorner.Initialize(TilemapModel.GetWorldPositionForCoordinates(position) + new Vector2Fixed(direction) * TilemapModel.HalfTileWidth, cornerAdjacencyReferences);
			this._tileCorners[cornerDefinition] = tileCorner;
			this._tileCorners[adjacentCornerA] = tileCorner;
			this._tileCorners[adjacentCornerB] = tileCorner;
			this._tileCorners[oppositeCorner] = tileCorner;
			this._simulation.AddModel(tileCorner);
			return tileCorner;
		}

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x060021FC RID: 8700 RVA: 0x00088FB4 File Offset: 0x000871B4
		public IList<LaneModel> TemporaryLanes
		{
			get
			{
				return this._temporaryLanes;
			}
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x060021FD RID: 8701 RVA: 0x00088FBC File Offset: 0x000871BC
		public IEnumerable<TileModel> ChangedTiles
		{
			get
			{
				return this._changedTiles;
			}
		}

		// Token: 0x060021FE RID: 8702 RVA: 0x00088FC4 File Offset: 0x000871C4
		public void ClearChangedTiles()
		{
			this._changedTiles.Clear();
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x00088FD4 File Offset: 0x000871D4
		public void ActivateUnblockedPendingLanes()
		{
			CityDefinition cityDefinition = this._city.Definition;
			this._arePendingTileConnectionsLocked = true;
			bool rebuildBridges = false;
			bool rebuildTunnels = false;
			this._potentialNewPassages.Clear();
			this._blockedTileConnections.Clear();
			foreach (AdjacentTileConnection pendingTileConnection in this._pendingTileConnections)
			{
				Tile originTile = this.GetTile(pendingTileConnection.OriginCoordinates);
				RoadTileNode originNode = new RoadTileNode(pendingTileConnection.OriginDirection, RoadType.TwoLane, -1);
				Tile destinationTile = this.GetTile(pendingTileConnection.DestinationCoordinates);
				RoadTileNode destinationNode = new RoadTileNode(pendingTileConnection.DestinationDirection, RoadType.TwoLane, -1);
				bool didActivateConnection = true;
				if ((originTile.CanSetNodeState(originNode, RoadState.Active, Tile.TileChangePermissions.Full) && destinationTile.CanSetNodeState(destinationNode, RoadState.Active, Tile.TileChangePermissions.Full)) || (originTile.CanSetNodeState(originNode, RoadState.Active, Tile.TileChangePermissions.Full) && destinationTile.StateOfRoadInDirection(destinationNode.direction) == RoadState.Active) || (originTile.StateOfRoadInDirection(originNode.direction) == RoadState.Active && destinationTile.CanSetNodeState(destinationNode, RoadState.Active, Tile.TileChangePermissions.Full)))
				{
					if (!originTile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore)[originNode.direction])
					{
						originTile.SetNodeState(originNode, RoadState.Active, Tile.TileChangePermissions.Full);
					}
					if (!destinationTile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore)[destinationNode.direction])
					{
						destinationTile.SetNodeState(destinationNode, RoadState.Active, Tile.TileChangePermissions.Full);
					}
				}
				else if (Roundabout.IsTileCenterOfRoundabout(originTile, RoadState.VisiblyActive) && destinationTile.CanSetNodeState(destinationNode, RoadState.Active, Tile.TileChangePermissions.Full))
				{
					destinationTile.SetNodeState(destinationNode, RoadState.Active, Tile.TileChangePermissions.Full);
				}
				else if (Roundabout.IsTileCenterOfRoundabout(destinationTile, RoadState.VisiblyActive) && originTile.CanSetNodeState(originNode, RoadState.Active, Tile.TileChangePermissions.Full))
				{
					originTile.SetNodeState(originNode, RoadState.Active, Tile.TileChangePermissions.Full);
				}
				else
				{
					didActivateConnection = false;
				}
				if (didActivateConnection)
				{
					bool isOriginOverWater = cityDefinition.TileIsOverWater(pendingTileConnection.OriginCoordinates);
					bool isDestinationOverWater = cityDefinition.TileIsOverWater(pendingTileConnection.DestinationCoordinates);
					bool isOriginUnderMountain = cityDefinition.TileIsUnderAMountain(pendingTileConnection.OriginCoordinates);
					bool isDestinationUnderMountain = cityDefinition.TileIsUnderAMountain(pendingTileConnection.DestinationCoordinates);
					if (isOriginOverWater || isDestinationOverWater)
					{
						rebuildBridges = true;
						if (isOriginOverWater ^ isDestinationOverWater)
						{
							this._potentialNewPassages.Add(pendingTileConnection);
						}
					}
					else if (isOriginUnderMountain || isDestinationUnderMountain)
					{
						rebuildTunnels = true;
						if (isOriginUnderMountain ^ isDestinationUnderMountain)
						{
							this._potentialNewPassages.Add(pendingTileConnection);
						}
					}
				}
				else if (originTile.GetTwoLaneRoads(RoadState.Pending, Tile.MotorwayInclusion.Ignore)[originNode.direction] || destinationTile.GetTwoLaneRoads(RoadState.Pending, Tile.MotorwayInclusion.Ignore)[destinationNode.direction])
				{
					this._blockedTileConnections.Add(pendingTileConnection);
				}
			}
			this._pendingTileConnections.Clear();
			if (this._blockedTileConnections.Count > 0)
			{
				HashSet<AdjacentTileConnection> blockedTileConnections = this._blockedTileConnections;
				HashSet<AdjacentTileConnection> pendingTileConnections = this._pendingTileConnections;
				this._pendingTileConnections = blockedTileConnections;
				this._blockedTileConnections = pendingTileConnections;
			}
			if (rebuildBridges || rebuildTunnels)
			{
				ModelList<PassageModel> passages = this._simulation.GetModels<PassageModel>();
				for (int passageIndex = 0; passageIndex < passages.Count; passageIndex++)
				{
					PassageModel passageModel = passages[passageIndex];
					Passage passage = passageModel.Passage;
					if (!passage.IsComplete && (rebuildBridges || passage.UpgradeType != UpgradeType.Bridge) && (rebuildTunnels || passage.UpgradeType != UpgradeType.Tunnel))
					{
						bool foundMergedPassage = false;
						for (int previousPassageIndex = 0; previousPassageIndex < passageIndex; previousPassageIndex++)
						{
							if (passages[previousPassageIndex].Passage.OverlapsPassage(passage))
							{
								this._simulation.RemoveModel(passageModel);
								foundMergedPassage = true;
								break;
							}
						}
						if (foundMergedPassage)
						{
							break;
						}
						passageModel.ExtendOverActiveConnections();
					}
				}
				foreach (AdjacentTileConnection potentialNewPassage in this._potentialNewPassages)
				{
					UpgradeType passageType;
					Vector2Int landCoordinates;
					Vector2Int crossingCoordinates;
					if (cityDefinition.TileIsOverWater(potentialNewPassage.OriginCoordinates))
					{
						passageType = UpgradeType.Bridge;
						landCoordinates = potentialNewPassage.DestinationCoordinates;
						crossingCoordinates = potentialNewPassage.OriginCoordinates;
					}
					else if (cityDefinition.TileIsOverWater(potentialNewPassage.DestinationCoordinates))
					{
						passageType = UpgradeType.Bridge;
						landCoordinates = potentialNewPassage.OriginCoordinates;
						crossingCoordinates = potentialNewPassage.DestinationCoordinates;
					}
					else if (cityDefinition.TileIsUnderAMountain(potentialNewPassage.OriginCoordinates))
					{
						passageType = UpgradeType.Tunnel;
						landCoordinates = potentialNewPassage.DestinationCoordinates;
						crossingCoordinates = potentialNewPassage.OriginCoordinates;
					}
					else
					{
						if (!cityDefinition.TileIsUnderAMountain(potentialNewPassage.DestinationCoordinates))
						{
							Diagnostics.FailAssert("{0} -> {1} was added as a potential new passage, but neither end is over an obstruction.", new object[]
							{
								potentialNewPassage.OriginCoordinates,
								potentialNewPassage.DestinationCoordinates
							});
							continue;
						}
						passageType = UpgradeType.Tunnel;
						landCoordinates = potentialNewPassage.OriginCoordinates;
						crossingCoordinates = potentialNewPassage.DestinationCoordinates;
					}
					bool foundExistingPassage = false;
					foreach (PassageModel passageModel2 in this._simulation.GetModels<PassageModel>())
					{
						Passage passage2 = passageModel2.Passage;
						if (passageType == passage2.UpgradeType && passage2.StartsWithConnection(landCoordinates, crossingCoordinates))
						{
							foundExistingPassage = true;
							break;
						}
					}
					if (!foundExistingPassage)
					{
						PassageModel newPassageModel = this._scope.Get<PassageModel>();
						newPassageModel.Initialize(passageType, landCoordinates, crossingCoordinates);
						this._simulation.AddModel(newPassageModel);
					}
				}
				this._potentialNewPassages.Clear();
			}
			this._arePendingTileConnectionsLocked = false;
		}

		// Token: 0x17000614 RID: 1556
		// (get) Token: 0x06002200 RID: 8704 RVA: 0x000894EC File Offset: 0x000876EC
		public IEnumerable<AdjacentTileConnection> MothballedTileConnections
		{
			get
			{
				return this._mothballedTileConnections;
			}
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x000894F4 File Offset: 0x000876F4
		public void ReserveTile(Vector2Int coordinates)
		{
			this._reservedTiles.Add(coordinates);
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x00089503 File Offset: 0x00087703
		public void UnreserveTile(Vector2Int coordinates)
		{
			this._reservedTiles.Remove(coordinates);
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x00089512 File Offset: 0x00087712
		public bool IsTileReserved(Vector2Int coordinates)
		{
			return this._reservedTiles.Contains(coordinates);
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x00089520 File Offset: 0x00087720
		public IEnumerable<Vector2Int> GetAllTileCoordinates()
		{
			return this._tiles.Keys;
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x0008952D File Offset: 0x0008772D
		public void ClearTileReservations()
		{
			this._reservedTiles.Clear();
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x0008953A File Offset: 0x0008773A
		public static Vector2Fixed GetWorldPositionForCoordinates(Vector2Int coordinates)
		{
			return new Vector2Fixed((Fix64)((long)coordinates.x) * TilemapModel.TileWidth, (Fix64)((long)coordinates.y) * TilemapModel.TileWidth);
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x00089570 File Offset: 0x00087770
		public override void Reset()
		{
			base.Reset();
			this._tiles.Clear();
			this._reservedTiles.Clear();
			this._tileCorners.Clear();
			this._motorways.Clear();
			this._pendingTileConnections.Clear();
			this._arePendingTileConnectionsLocked = false;
			this._changedTiles.Clear();
			this._mothballedTileConnections.Clear();
			this._pendingTileConnections.Clear();
			this._blockedTileConnections.Clear();
			this._potentialNewPassages.Clear();
			this._temporaryLanes.Clear();
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x00089604 File Offset: 0x00087804
		public void OnDeserialized(IScope context)
		{
			foreach (TileModel tileModel in this._tiles.Values)
			{
				tileModel.Subscribe(this);
				this.CachePendingTileConnections(tileModel.Tile);
				this.CacheMothballedTileConnections(tileModel.Tile);
			}
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x00089674 File Offset: 0x00087874
		public void OnTileModelChanged(TileModel model)
		{
			this._changedTiles.Add(model);
			if (!this._arePendingTileConnectionsLocked)
			{
				this.CachePendingTileConnections(model.Tile);
			}
			this.CacheMothballedTileConnections(model.Tile);
		}

		// Token: 0x0600220A RID: 8714 RVA: 0x000896A4 File Offset: 0x000878A4
		private void CachePendingTileConnections(Tile tile)
		{
			foreach (TileDirection pendingNodeDirection in tile.GetTwoLaneRoads(RoadState.Pending, Tile.MotorwayInclusion.Ignore))
			{
				this._pendingTileConnections.Add(new AdjacentTileConnection(tile.Coordinates, pendingNodeDirection));
			}
		}

		// Token: 0x0600220B RID: 8715 RVA: 0x000896F0 File Offset: 0x000878F0
		private void CacheMothballedTileConnections(Tile tile)
		{
			if (this._mothballedTileConnections.Count > 0)
			{
				this._mothballedTileConnections.RemoveWhere(delegate(AdjacentTileConnection connection)
				{
					if (connection.OriginCoordinates == tile.Coordinates)
					{
						if (!tile.HasTwoLaneRoadInDirection(connection.OriginDirection, RoadState.Mothballed))
						{
							Tile tile2 = this.GetTile(connection.DestinationCoordinates);
							return tile2 == null || !tile2.HasTwoLaneRoadInDirection(connection.DestinationDirection, RoadState.Mothballed);
						}
						return false;
					}
					else
					{
						if (!(connection.DestinationCoordinates == tile.Coordinates))
						{
							return false;
						}
						if (!tile.HasTwoLaneRoadInDirection(connection.DestinationDirection, RoadState.Mothballed))
						{
							Tile tile3 = this.GetTile(connection.OriginCoordinates);
							return tile3 == null || !tile3.HasTwoLaneRoadInDirection(connection.OriginDirection, RoadState.Mothballed);
						}
						return false;
					}
				});
			}
			TileDirectionBitfield mothballedDirections = tile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore);
			if (mothballedDirections.Count > 0)
			{
				CityDefinition cityDefinition = this._city.Definition;
				if (cityDefinition.TileIsOverWater(tile.Coordinates) || cityDefinition.TileIsUnderAMountain(tile.Coordinates))
				{
					return;
				}
				foreach (TileDirection mothballedNodeDirection in mothballedDirections)
				{
					Vector2Int destinationCoordinates = TileUtilities.GetAdjacencyOffsetForDirection(mothballedNodeDirection) + tile.Coordinates;
					if (!cityDefinition.TileIsOverWater(destinationCoordinates) && !cityDefinition.TileIsUnderAMountain(destinationCoordinates))
					{
						this._mothballedTileConnections.Add(new AdjacentTileConnection(tile.Coordinates, mothballedNodeDirection));
					}
				}
			}
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x000897F0 File Offset: 0x000879F0
		public TilemapModel() : base(1)
		{
		}

		// Token: 0x04001BDB RID: 7131
		public static readonly Fix64 TileWidth = Fix64Consts.Two;

		// Token: 0x04001BDC RID: 7132
		public static readonly Fix64 HalfTileWidth = TilemapModel.TileWidth * Fix64Consts.OneHalf;

		// Token: 0x04001BDD RID: 7133
		[Dependency]
		private Scope _scope;

		// Token: 0x04001BDE RID: 7134
		[Dependency]
		private Simulation _simulation;

		// Token: 0x04001BDF RID: 7135
		[Dependency]
		private City _city;

		// Token: 0x04001BE0 RID: 7136
		private readonly Dictionary<Vector2Int, TileModel> _tiles = new Dictionary<Vector2Int, TileModel>();

		// Token: 0x04001BE1 RID: 7137
		[Serialize(false, null)]
		private readonly HashSet<Vector2Int> _reservedTiles = new HashSet<Vector2Int>();

		// Token: 0x04001BE2 RID: 7138
		private readonly Dictionary<CornerAdjacencyReference, TileCornerModel> _tileCorners = new Dictionary<CornerAdjacencyReference, TileCornerModel>();

		// Token: 0x04001BE3 RID: 7139
		[Serialize(false, null)]
		private readonly HashSet<TileModel> _changedTiles = new HashSet<TileModel>();

		// Token: 0x04001BE4 RID: 7140
		[Serialize(false, null)]
		private readonly HashSet<AdjacentTileConnection> _mothballedTileConnections = new HashSet<AdjacentTileConnection>();

		// Token: 0x04001BE5 RID: 7141
		[Serialize(false, null)]
		private HashSet<AdjacentTileConnection> _pendingTileConnections = new HashSet<AdjacentTileConnection>();

		// Token: 0x04001BE6 RID: 7142
		[Serialize(false, null)]
		private HashSet<AdjacentTileConnection> _blockedTileConnections = new HashSet<AdjacentTileConnection>();

		// Token: 0x04001BE7 RID: 7143
		[Serialize(false, null)]
		private bool _arePendingTileConnectionsLocked;

		// Token: 0x04001BE8 RID: 7144
		private readonly List<LaneModel> _temporaryLanes = new List<LaneModel>();

		// Token: 0x04001BE9 RID: 7145
		[Serialize(false, null)]
		private readonly List<AdjacentTileConnection> _potentialNewPassages = new List<AdjacentTileConnection>();

		// Token: 0x04001BEA RID: 7146
		private readonly Dictionary<int, MotorwayModel> _motorways = new Dictionary<int, MotorwayModel>();

		// Token: 0x04001BEB RID: 7147
		private static readonly ProfilerMarker Profiler_ActivateUnblockedPendingLanes = new ProfilerMarker(ProfilerCategory.Scripts, "TilemapModel.ActivateUnblockedPendingLanes()");

		// Token: 0x04001BEC RID: 7148
		private static readonly ProfilerMarker Profiler_CachePendingTileConnections = new ProfilerMarker(ProfilerCategory.Scripts, "TilemapModel.CachePendingTileConnections()");

		// Token: 0x04001BED RID: 7149
		private static readonly ProfilerMarker Profiler_CacheMothballedTileConnections = new ProfilerMarker(ProfilerCategory.Scripts, "TilemapModel.CacheMothballedTileConnections()");
	}
}
