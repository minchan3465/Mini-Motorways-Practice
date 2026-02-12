using System;
using System.Collections.Generic;
using Factory;
using Motorways.Models;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003F9 RID: 1017
	public class ClearTileEdit : TileEdit
	{
		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x060018C3 RID: 6339 RVA: 0x00058303 File Offset: 0x00056503
		// (set) Token: 0x060018C4 RID: 6340 RVA: 0x0005830B File Offset: 0x0005650B
		[Serialize(true, null)]
		public Vector2Int Coordinates { get; private set; }

		// Token: 0x060018C5 RID: 6341 RVA: 0x00058314 File Offset: 0x00056514
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			int num;
			for (int x = -1; x <= 1; x = num)
			{
				for (int y = -1; y <= 1; y = num)
				{
					Tile adjacentTile = tilemap.GetTile(this.Coordinates + new Vector2Int(x, y));
					if (adjacentTile != null && adjacentTile.ContentType != TileContentType.House && adjacentTile.ContentType != TileContentType.Destination && adjacentTile.ContentType != TileContentType.Carpark)
					{
						yield return adjacentTile;
					}
					num = y + 1;
				}
				num = x + 1;
			}
			yield break;
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x0005832C File Offset: 0x0005652C
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (tile.Coordinates.Equals(this.Coordinates))
			{
				bool mothballedAllNodes = true;
				foreach (TileDirection activeDirection in this._roadDirectionsToMothball)
				{
					mothballedAllNodes = (Diagnostics.Verify(tile.SetNodeState(new RoadTileNode(activeDirection, RoadType.TwoLane, -1), RoadState.Mothballed, this._changePermissions), "Failed to mothball two-lane road from {0} headed {1}.", tile, activeDirection) && mothballedAllNodes);
				}
				return mothballedAllNodes;
			}
			TileDirection connectionDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(tile.Coordinates, this.Coordinates);
			if (Diagnostics.Verify(connectionDirection != TileDirection.None, "ClearTileEdit applied between non-adjacent tiles {0} and {1}.", tile.Coordinates, this.Coordinates))
			{
				if ((tile.GetTwoLaneRoadStateInDirection(connectionDirection) & RoadState.ActiveOrPending) != RoadState.None)
				{
					tile.SetNodeState(new RoadTileNode(connectionDirection, RoadType.TwoLane, -1), RoadState.Mothballed, this._changePermissions);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x00058400 File Offset: 0x00056600
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			if (this._concreteToMothball > 0)
			{
				upgradeDatabase.MothballUpgrade(UpgradeType.Concrete, this._concreteToMothball);
				if (this._concreteToRelease > 0)
				{
					upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Concrete, this._concreteToRelease);
				}
			}
			return true;
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x00058434 File Offset: 0x00056634
		private static TileDirectionBitfield GetRoadDirectionsToMothball(Tile tileToClear, ITilemap tilemap, GameBehaviourModel behaviour, Tile.TileChangePermissions changePermissions, out int mothballConcreteCount, out int releaseConcreteCount)
		{
			TileDirectionBitfield roadDirectionsToMothball = default(TileDirectionBitfield);
			mothballConcreteCount = 0;
			releaseConcreteCount = 0;
			if (tileToClear.ContentType == TileContentType.House || tileToClear.ContentType == TileContentType.Destination || tileToClear.ContentType == TileContentType.Carpark)
			{
				return roadDirectionsToMothball;
			}
			if (tileToClear.IsCenterOfRoundabout && tileToClear.IsRoundaboutPermanent)
			{
				foreach (TileDirection direction in TileUtilities.Directions)
				{
					Tile neighbourTile = tilemap.GetTile(TileUtilities.GetAdjacentCoordinates(tileToClear.Coordinates, direction));
					TileDirection oppositeDirection = TileUtilities.GetOppositeDirection(direction);
					if (neighbourTile != null && neighbourTile.ContentType != TileContentType.House && neighbourTile.ContentType != TileContentType.Destination && neighbourTile.ContentType != TileContentType.Carpark && neighbourTile.CanSetNodeState(new RoadTileNode(oppositeDirection, RoadType.TwoLane, -1), RoadState.Mothballed, changePermissions) && neighbourTile.HasTwoLaneRoadInDirection(oppositeDirection, RoadState.ActiveOrPending))
					{
						int concreteCountForDirection = behaviour.GetConcreteCostForConnection(neighbourTile.Coordinates, neighbourTile.ContentType, tileToClear.Coordinates, (tileToClear != null) ? tileToClear.ContentType : TileContentType.None);
						mothballConcreteCount += concreteCountForDirection;
						if (tileToClear.HasTwoLaneRoadInDirection(oppositeDirection, RoadState.Pending))
						{
							releaseConcreteCount += concreteCountForDirection;
						}
					}
				}
				return roadDirectionsToMothball;
			}
			foreach (TileDirection activeDirection in tileToClear.GetTwoLaneRoads(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore))
			{
				Vector2Int adjacentTileCoordinates = TileUtilities.GetAdjacentCoordinates(tileToClear.Coordinates, activeDirection);
				Tile adjacentTile = tileToClear.Tilemap.GetTile(adjacentTileCoordinates);
				if ((adjacentTile == null || (adjacentTile.ContentType != TileContentType.House && adjacentTile.ContentType != TileContentType.Destination && adjacentTile.ContentType != TileContentType.Carpark)) && tileToClear.CanSetNodeState(new RoadTileNode(activeDirection, RoadType.TwoLane, -1), RoadState.Mothballed, changePermissions))
				{
					int concreteCountForDirection2 = behaviour.GetConcreteCostForConnection(tileToClear.Coordinates, tileToClear.ContentType, adjacentTileCoordinates, (adjacentTile != null) ? adjacentTile.ContentType : TileContentType.None);
					mothballConcreteCount += concreteCountForDirection2;
					if (tileToClear.HasTwoLaneRoadInDirection(activeDirection, RoadState.Pending))
					{
						releaseConcreteCount += concreteCountForDirection2;
					}
					roadDirectionsToMothball[activeDirection] = true;
				}
			}
			return roadDirectionsToMothball;
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x0005861C File Offset: 0x0005681C
		public override void Reset()
		{
			base.Reset();
			this.Coordinates = default(Vector2Int);
			this._roadDirectionsToMothball = TileDirectionBitfield.None;
			this._concreteToMothball = 0;
			this._concreteToRelease = 0;
			this._changePermissions = Tile.TileChangePermissions.Full;
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x00058660 File Offset: 0x00056860
		public static ClearTileEdit Create(IScope scope, Vector2Int coordinates, ITilemap tilemap, Tile.TileChangePermissions changePermissions = Tile.TileChangePermissions.Full)
		{
			Tile tileToClear = tilemap.GetTile(coordinates);
			ClearTileEdit newEdit = scope.Get<ClearTileEdit>();
			newEdit.Coordinates = coordinates;
			newEdit._changePermissions = changePermissions;
			newEdit._roadDirectionsToMothball = ClearTileEdit.GetRoadDirectionsToMothball(tileToClear, tilemap, scope.Get<GameBehaviourModel>(), changePermissions, out newEdit._concreteToMothball, out newEdit._concreteToRelease);
			return newEdit;
		}

		// Token: 0x04001508 RID: 5384
		private TileDirectionBitfield _roadDirectionsToMothball;

		// Token: 0x04001509 RID: 5385
		private int _concreteToMothball;

		// Token: 0x0400150A RID: 5386
		private int _concreteToRelease;

		// Token: 0x0400150B RID: 5387
		private Tile.TileChangePermissions _changePermissions;
	}
}
