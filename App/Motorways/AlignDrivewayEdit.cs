using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003F7 RID: 1015
	public class AlignDrivewayEdit : TileEdit
	{
		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x060018AA RID: 6314 RVA: 0x00057FC7 File Offset: 0x000561C7
		// (set) Token: 0x060018AB RID: 6315 RVA: 0x00057FCF File Offset: 0x000561CF
		[Serialize(true, null)]
		public Vector2Int HouseCoordinates { get; private set; }

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x060018AC RID: 6316 RVA: 0x00057FD8 File Offset: 0x000561D8
		// (set) Token: 0x060018AD RID: 6317 RVA: 0x00057FE0 File Offset: 0x000561E0
		[Serialize(true, null)]
		public TileDirection PreviousDrivewayDirection { get; private set; }

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x060018AE RID: 6318 RVA: 0x00057FE9 File Offset: 0x000561E9
		// (set) Token: 0x060018AF RID: 6319 RVA: 0x00057FF1 File Offset: 0x000561F1
		[Serialize(true, null)]
		public TileDirection NewDrivewayDirection { get; private set; }

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x060018B0 RID: 6320 RVA: 0x00057FFA File Offset: 0x000561FA
		public Vector2Int PreviousDestinationCoordinates
		{
			get
			{
				return TileUtilities.GetAdjacentCoordinates(this.HouseCoordinates, this.PreviousDrivewayDirection);
			}
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x060018B1 RID: 6321 RVA: 0x0005800D File Offset: 0x0005620D
		public TileDirection PreviousDestinationToHouseDirection
		{
			get
			{
				return TileUtilities.GetOppositeDirection(this.PreviousDrivewayDirection);
			}
		}

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x060018B2 RID: 6322 RVA: 0x0005801A File Offset: 0x0005621A
		public Vector2Int NewDestinationCoordinates
		{
			get
			{
				return TileUtilities.GetAdjacentCoordinates(this.HouseCoordinates, this.NewDrivewayDirection);
			}
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x060018B3 RID: 6323 RVA: 0x0005802D File Offset: 0x0005622D
		public TileDirection NewDestinationToHouseDirection
		{
			get
			{
				return TileUtilities.GetOppositeDirection(this.NewDrivewayDirection);
			}
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x0005803A File Offset: 0x0005623A
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield return tilemap.GetOrCreateTile(this.HouseCoordinates);
			yield return tilemap.GetOrCreateTile(this.NewDestinationCoordinates);
			if (this.PreviousDrivewayDirection != TileDirection.None)
			{
				Tile previousDrivewayTile = tilemap.GetTile(this.PreviousDestinationCoordinates);
				if (previousDrivewayTile != null)
				{
					yield return previousDrivewayTile;
				}
			}
			yield break;
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x00058054 File Offset: 0x00056254
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (tile.Coordinates == this.HouseCoordinates)
			{
				if (this.PreviousDrivewayDirection != TileDirection.None)
				{
					tile.SetNodeState(new RoadTileNode(this.PreviousDrivewayDirection, RoadType.Driveway, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
				}
				if (this.NewDrivewayDirection != TileDirection.None)
				{
					tile.SetNodeState(new RoadTileNode(this.NewDrivewayDirection, RoadType.Driveway, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
				}
			}
			else if (tile.Coordinates == this.NewDestinationCoordinates)
			{
				if (Roundabout.IsTileCenterOfRoundabout(tile, RoadState.VisiblyActive))
				{
					return true;
				}
				tile.SetNodeState(new RoadTileNode(this.NewDestinationToHouseDirection, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
			}
			else if (this.PreviousDrivewayDirection != TileDirection.None && tile.Coordinates == this.PreviousDestinationCoordinates)
			{
				tile.SetNodeState(new RoadTileNode(this.PreviousDestinationToHouseDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
			}
			return true;
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			return true;
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x00058124 File Offset: 0x00056324
		public override void Reset()
		{
			base.Reset();
			this.HouseCoordinates = default(Vector2Int);
			this.PreviousDrivewayDirection = TileDirection.North;
			this.NewDrivewayDirection = TileDirection.North;
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x00058154 File Offset: 0x00056354
		public override string ToString()
		{
			return string.Format("[AlignDrivewayEdit HouseCoordinates={0}, PreviousDrivewayDirection={1}, NewDrivewayDirection={2}]", this.HouseCoordinates, this.PreviousDrivewayDirection, this.NewDrivewayDirection);
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x00058184 File Offset: 0x00056384
		public static AlignDrivewayEdit Create(IScope scope, ITilemap tilemap, Vector2Int originCoordinates, TileDirection direction)
		{
			Tile originTile = tilemap.GetTile(originCoordinates);
			TileDirection oldDrivewayDirection = originTile.DrivewayDirection;
			if (direction != oldDrivewayDirection)
			{
				AlignDrivewayEdit alignDrivewayEdit = scope.Get<AlignDrivewayEdit>();
				alignDrivewayEdit.HouseCoordinates = originTile.Coordinates;
				alignDrivewayEdit.PreviousDrivewayDirection = oldDrivewayDirection;
				alignDrivewayEdit.NewDrivewayDirection = direction;
				return alignDrivewayEdit;
			}
			return null;
		}
	}
}
