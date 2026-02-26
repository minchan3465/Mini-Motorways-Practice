using System;
using System.Collections.Generic;
using Factory;

namespace Motorways
{
	// Token: 0x020003FC RID: 1020
	public class MothballMotorwayEdit : TileEdit
	{
		// Token: 0x060018D5 RID: 6357 RVA: 0x000587FB File Offset: 0x000569FB
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			Motorway motorway = tilemap.GetMotorway(this._motorwayId);
			yield return tilemap.GetTile(motorway.StartCoordinates);
			yield return tilemap.GetTile(motorway.EndCoordinates);
			yield break;
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x00058814 File Offset: 0x00056A14
		public override bool ApplyToAffectedTile(Tile tile)
		{
			Motorway mothballedMotorway = tile.Tilemap.GetMotorway(this._motorwayId);
			if (tile.Coordinates == mothballedMotorway.StartCoordinates)
			{
				return tile.SetNodeState(new RoadTileNode(mothballedMotorway.StartDirection, RoadType.Motorway, this._motorwayId), RoadState.Mothballed, Tile.TileChangePermissions.Full);
			}
			return tile.Coordinates == mothballedMotorway.EndCoordinates && tile.SetNodeState(new RoadTileNode(mothballedMotorway.EndDirection, RoadType.Motorway, this._motorwayId), RoadState.Mothballed, Tile.TileChangePermissions.Full);
		}

		// Token: 0x060018D7 RID: 6359 RVA: 0x00058892 File Offset: 0x00056A92
		public override IEnumerable<Motorway> GetAffectedMotorways(ITilemap tilemap)
		{
			Motorway motorway = tilemap.GetMotorway(this._motorwayId);
			if (Diagnostics.Verify(motorway != null, "Unable to find motorway from ID {0}", this._motorwayId))
			{
				yield return motorway;
			}
			yield break;
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x000588A9 File Offset: 0x00056AA9
		public override bool ApplyToAffectedMotorway(Motorway motorway)
		{
			if (motorway.Id == this._motorwayId)
			{
				motorway.SetState(RoadState.Mothballed);
				return true;
			}
			return false;
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			return true;
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x000588C5 File Offset: 0x00056AC5
		public override void Reset()
		{
			base.Reset();
			this._motorwayId = 0;
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x000588D4 File Offset: 0x00056AD4
		public static MothballMotorwayEdit Create(IScope scope, int motorwayId)
		{
			MothballMotorwayEdit mothballMotorwayEdit = scope.Get<MothballMotorwayEdit>();
			mothballMotorwayEdit._motorwayId = motorwayId;
			return mothballMotorwayEdit;
		}

		// Token: 0x04001519 RID: 5401
		private int _motorwayId;
	}
}
