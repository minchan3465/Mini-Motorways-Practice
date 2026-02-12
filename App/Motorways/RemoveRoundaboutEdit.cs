using System;
using System.Collections.Generic;
using Factory;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000404 RID: 1028
	public class RemoveRoundaboutEdit : TileEdit
	{
		// Token: 0x170004EB RID: 1259
		// (get) Token: 0x06001917 RID: 6423 RVA: 0x0005976B File Offset: 0x0005796B
		// (set) Token: 0x06001918 RID: 6424 RVA: 0x00059773 File Offset: 0x00057973
		[Serialize(true, null)]
		public Vector2Int Coordinates { get; private set; }

		// Token: 0x06001919 RID: 6425 RVA: 0x0005977C File Offset: 0x0005797C
		public override void Reset()
		{
			base.Reset();
			this.Coordinates = default(Vector2Int);
			this._tileCoordinatesToMothball.Clear();
			this._centerCoordinates = default(Vector2Int);
			this._centerTileNodeDirectionsToRebuild = default(TileDirectionBitfield);
			this._centreTileNodeDirectionsToMothball = default(TileDirectionBitfield);
			this._isRemovingPlannedRoundabout = false;
			this._connectionsToRestore.Clear();
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x000597DF File Offset: 0x000579DF
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			Tile tile = tilemap.GetTile(this.Coordinates);
			if (tile != null)
			{
				yield return tilemap.GetOrCreateTile(this._centerCoordinates);
				foreach (Tile roundaboutTile in Roundabout.GetTilesInRoundabout(tile, RoadState.Planned | RoadState.Active))
				{
					yield return roundaboutTile;
				}
				IEnumerator<Tile> enumerator = null;
				foreach (Vector2Int roadTileCoordinates in this._tileCoordinatesToMothball.Keys)
				{
					yield return tilemap.GetOrCreateTile(roadTileCoordinates);
				}
				Dictionary<Vector2Int, TileDirection>.KeyCollection.Enumerator enumerator2 = default(Dictionary<Vector2Int, TileDirection>.KeyCollection.Enumerator);
				foreach (TileDirection diagonalDirection in TileUtilities.DiagonalDirections)
				{
					yield return tilemap.GetOrCreateTile(TileUtilities.GetAdjacentCoordinates(this._centerCoordinates, diagonalDirection));
				}
				TileDirection[] array = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x000597F8 File Offset: 0x000579F8
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (!Diagnostics.Verify(tile != null, "Somehow was passed a null tile!"))
			{
				return false;
			}
			if (tile.Coordinates == this._centerCoordinates)
			{
				bool centerTileSuccess = true;
				foreach (TileDirection centerTileNodeDirection in this._centerTileNodeDirectionsToRebuild)
				{
					centerTileSuccess = (tile.SetNodeState(new RoadTileNode(centerTileNodeDirection, RoadType.TwoLane, -1), this._centreTileNodeDirectionsToMothball[centerTileNodeDirection] ? RoadState.Mothballed : RoadState.Pending, Tile.TileChangePermissions.Full) && centerTileSuccess);
				}
				tile.IsCenterOfRoundabout = false;
				return centerTileSuccess & this.RestoreAnyConnectionsForTile(tile);
			}
			if (this._tileCoordinatesToMothball.ContainsKey(tile.Coordinates))
			{
				TileDirection directionToMothball = this._tileCoordinatesToMothball[tile.Coordinates];
				return (!tile.HasTwoLaneRoadInDirection(directionToMothball, RoadState.Planned | RoadState.Active) || tile.SetNodeState(new RoadTileNode(directionToMothball, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full)) & this.RestoreAnyConnectionsForTile(tile);
			}
			RoadTileConnection roundaboutConnection = tile.GetRoundaboutConnection(RoadState.Planned | RoadState.Active);
			bool isDiagonalConnectionFromCenter = TileUtilities.IsDirectionDiagonal(TileUtilities.GetDirectionBetweenAdjacentCoordinates(tile.Coordinates, this._centerCoordinates));
			return (!tile.HasRoundabout(RoadState.VisiblyActive) || isDiagonalConnectionFromCenter || tile.SetRoundaboutState(roundaboutConnection.input.direction, roundaboutConnection.output.direction, RoadState.Mothballed)) & this.RestoreAnyConnectionsForTile(tile);
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x00059930 File Offset: 0x00057B30
		private bool RestoreAnyConnectionsForTile(Tile tile)
		{
			bool success = true;
			foreach (AdjacentTileConnection connection in this._connectionsToRestore)
			{
				if (connection.DestinationCoordinates == tile.Coordinates)
				{
					RoadTileNode node = new RoadTileNode(connection.DestinationDirection, RoadType.TwoLane, -1);
					if (tile.CanSetNodeState(node, RoadState.Pending, Tile.TileChangePermissions.Full))
					{
						success &= tile.SetNodeState(node, RoadState.Pending, Tile.TileChangePermissions.Full);
						tile.SetNodePermanence(connection.DestinationDirection, true);
					}
				}
				else if (connection.OriginCoordinates == tile.Coordinates)
				{
					RoadTileNode node2 = new RoadTileNode(connection.OriginDirection, RoadType.TwoLane, -1);
					if (tile.CanSetNodeState(node2, RoadState.Pending, Tile.TileChangePermissions.Full))
					{
						success &= tile.SetNodeState(node2, RoadState.Pending, Tile.TileChangePermissions.Full);
						tile.SetNodePermanence(connection.OriginDirection, true);
					}
				}
			}
			return success;
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x00059A18 File Offset: 0x00057C18
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			bool success = upgradeDatabase.MothballUpgrade(UpgradeType.Roundabout, 1);
			if (this._isRemovingPlannedRoundabout)
			{
				success = (upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Roundabout, 1) && success);
			}
			if (this._tileCoordinatesToMothball.Keys.Count > 0)
			{
				int amountOfConcreteToMothball = 0;
				foreach (KeyValuePair<Vector2Int, TileDirection> coordinate in this._tileCoordinatesToMothball)
				{
					amountOfConcreteToMothball += this._behaviour.GetConcreteCostForConnection(tilemap, coordinate.Key, TileUtilities.GetAdjacentCoordinates(coordinate.Key, coordinate.Value));
				}
				success = (upgradeDatabase.MothballUpgrade(UpgradeType.Concrete, amountOfConcreteToMothball) && success);
			}
			if (this._connectionsToRestore.Count > 0)
			{
				int amountOfConcreteToUnmothball = 0;
				foreach (AdjacentTileConnection connection in this._connectionsToRestore)
				{
					amountOfConcreteToUnmothball += this._behaviour.GetConcreteCostForConnection(tilemap, connection.OriginCoordinates, connection.DestinationCoordinates);
				}
				success = (upgradeDatabase.UnmothballUpgrade(UpgradeType.Concrete, amountOfConcreteToUnmothball) && success);
			}
			return success;
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x00059B44 File Offset: 0x00057D44
		public static RemoveRoundaboutEdit Create(IScope scope, Vector2Int coordinates, ITilemap tilemap, CityDefinition cityDefinition)
		{
			RemoveRoundaboutEdit newEdit = scope.Get<RemoveRoundaboutEdit>();
			newEdit.Coordinates = coordinates;
			Tile focusTile = tilemap.GetTile(coordinates);
			if (Diagnostics.Verify(focusTile != null))
			{
				RoadTileConnection roundaboutConnection = focusTile.GetRoundaboutConnection(RoadState.VisiblyActive);
				Vector2Int focusOffset = Roundabout.IsTileCenterOfRoundabout(focusTile, RoadState.VisiblyActive) ? Roundabout.GetCenterOffset() : Roundabout.GetCoordinatesOffsetForConnection(roundaboutConnection);
				newEdit._centerCoordinates = coordinates - focusOffset;
				foreach (TileDirection diagonalDirection in TileUtilities.DiagonalDirections)
				{
					Vector2Int adjacentCoordinate = TileUtilities.GetAdjacentCoordinates(newEdit._centerCoordinates, diagonalDirection);
					Tile adjacentTile = tilemap.GetTile(adjacentCoordinate);
					if (adjacentTile != null)
					{
						TileDirection incomingRoadDirection = TileUtilities.GetOppositeDirection(diagonalDirection);
						RoadState incomingRoadState = adjacentTile.GetTwoLaneRoadStateInDirection(incomingRoadDirection);
						if (incomingRoadState != RoadState.None)
						{
							if (adjacentTile.ContentType != TileContentType.House && !cityDefinition.TileIsOverWater(adjacentCoordinate) && !cityDefinition.TileIsUnderAMountain(adjacentCoordinate))
							{
								if (incomingRoadState == RoadState.Active || incomingRoadState == RoadState.Pending)
								{
									newEdit._tileCoordinatesToMothball[adjacentTile.Coordinates] = incomingRoadDirection;
								}
							}
							else
							{
								newEdit._centerTileNodeDirectionsToRebuild[diagonalDirection] = true;
								newEdit._centreTileNodeDirectionsToMothball[diagonalDirection] = (incomingRoadState == RoadState.Mothballed);
							}
						}
					}
				}
			}
			if (tilemap.GetTile(newEdit.Coordinates).IsCenterOfRoundabout)
			{
				newEdit._isRemovingPlannedRoundabout = tilemap.GetTile(TileUtilities.GetAdjacentCoordinates(newEdit.Coordinates, TileDirection.North)).HasRoundabout(RoadState.Planned);
			}
			else
			{
				newEdit._isRemovingPlannedRoundabout = tilemap.GetTile(newEdit.Coordinates).HasRoundabout(RoadState.Planned);
			}
			ISimulation simulation = scope.Get<ISimulation>();
			RoundaboutModel existingRoundabout = null;
			foreach (RoundaboutModel roundabout in simulation.GetModels<RoundaboutModel>())
			{
				if (roundabout.OriginCoordinates == newEdit._centerCoordinates)
				{
					existingRoundabout = roundabout;
					break;
				}
			}
			if (Diagnostics.Verify(existingRoundabout != null, string.Format("We have no roundabout model at {0}.", newEdit._centerCoordinates)))
			{
				newEdit._connectionsToRestore.AddRange(existingRoundabout.ReplacedConnections);
			}
			return newEdit;
		}

		// Token: 0x04001546 RID: 5446
		[Serialize(true, null)]
		private readonly Dictionary<Vector2Int, TileDirection> _tileCoordinatesToMothball = new Dictionary<Vector2Int, TileDirection>();

		// Token: 0x04001547 RID: 5447
		[Serialize(true, null)]
		private Vector2Int _centerCoordinates;

		// Token: 0x04001548 RID: 5448
		[Serialize(true, null)]
		private TileDirectionBitfield _centerTileNodeDirectionsToRebuild;

		// Token: 0x04001549 RID: 5449
		[Serialize(true, null)]
		private TileDirectionBitfield _centreTileNodeDirectionsToMothball;

		// Token: 0x0400154A RID: 5450
		[Serialize(true, null)]
		private readonly List<AdjacentTileConnection> _connectionsToRestore = new List<AdjacentTileConnection>();

		// Token: 0x0400154B RID: 5451
		private bool _isRemovingPlannedRoundabout;
	}
}
