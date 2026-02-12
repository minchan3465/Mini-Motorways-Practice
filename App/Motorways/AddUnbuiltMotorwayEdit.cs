using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003F5 RID: 1013
	public class AddUnbuiltMotorwayEdit : TileEdit
	{
		// Token: 0x0600189C RID: 6300 RVA: 0x00057DF3 File Offset: 0x00055FF3
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield return tilemap.GetOrCreateTile(this._originCoordinates);
			yield break;
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x00057E0C File Offset: 0x0005600C
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (base.CanApplyToSimulation && Diagnostics.Verify(TileEditor.TileSupportsUnbuiltMotorway(tile, this._motorwayId), "Can't apply a new, unbuilt motorway to this tile (tile id is {0}, expected {1}", tile.UnbuiltMotorwayId, this._motorwayId))
			{
				TileEdit.Log.Info("Applying unbuilt motorway {0} to tile", new object[]
				{
					this._motorwayId
				});
				tile.UnbuiltMotorwayId = this._motorwayId;
				tile.UnbuiltMotorwayNumber = this._motorwayNumber;
				return true;
			}
			return false;
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x00057E8D File Offset: 0x0005608D
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			TileEdit.Log.Info("Consuming a motorway from UnbuildMotorwayEdit", Array.Empty<object>());
			return upgradeDatabase.ConsumeUpgrade(UpgradeType.Motorway, 1);
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x00057EAB File Offset: 0x000560AB
		public override void Reset()
		{
			this._originCoordinates = default(Vector2Int);
			this._motorwayId = -1;
			this._motorwayNumber = 0;
			base.Reset();
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x00057ECD File Offset: 0x000560CD
		public static AddUnbuiltMotorwayEdit Create(IScope scope, Vector2Int originCoordinates, int motorwayId, int motorwayNumber)
		{
			AddUnbuiltMotorwayEdit addUnbuiltMotorwayEdit = scope.Get<AddUnbuiltMotorwayEdit>();
			addUnbuiltMotorwayEdit._originCoordinates = originCoordinates;
			addUnbuiltMotorwayEdit._motorwayId = motorwayId;
			addUnbuiltMotorwayEdit._motorwayNumber = motorwayNumber;
			return addUnbuiltMotorwayEdit;
		}

		// Token: 0x040014F6 RID: 5366
		private Vector2Int _originCoordinates;

		// Token: 0x040014F7 RID: 5367
		private int _motorwayId = -1;

		// Token: 0x040014F8 RID: 5368
		private int _motorwayNumber;
	}
}
