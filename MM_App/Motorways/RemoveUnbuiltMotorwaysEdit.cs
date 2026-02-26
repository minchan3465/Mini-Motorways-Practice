using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000408 RID: 1032
	public class RemoveUnbuiltMotorwaysEdit : TileEdit
	{
		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x0600193A RID: 6458 RVA: 0x0005A1C7 File Offset: 0x000583C7
		// (set) Token: 0x0600193B RID: 6459 RVA: 0x0005A1CF File Offset: 0x000583CF
		[Serialize(true, null)]
		public Vector2Int Coordinates { get; private set; }

		// Token: 0x0600193C RID: 6460 RVA: 0x0005A1D8 File Offset: 0x000583D8
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			Tile tile = tilemap.GetTile(this.Coordinates);
			if (tile != null)
			{
				yield return tile;
			}
			yield break;
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x0005A1F0 File Offset: 0x000583F0
		public override bool ApplyToAffectedTile(Tile tile)
		{
			Tile.Log.Info("Applying RemoveMotorwayEdit from tile {0}, to tile {1}.", new object[]
			{
				this.Coordinates,
				tile.Coordinates
			});
			if (!tile.Coordinates.Equals(this.Coordinates))
			{
				return false;
			}
			if (tile.UnbuiltMotorwayId != -1)
			{
				tile.UnbuiltMotorwayId = -1;
				tile.UnbuiltMotorwayNumber = 0;
				Tile.Log.Info("Removed unbuilt motorway.", Array.Empty<object>());
				return true;
			}
			return true;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x0005A274 File Offset: 0x00058474
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			upgradeDatabase.MothballUpgrade(UpgradeType.Motorway, 1);
			upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Motorway, 1);
			return true;
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x0005A289 File Offset: 0x00058489
		public static RemoveUnbuiltMotorwaysEdit Create(IScope scope, Vector2Int coordinates)
		{
			RemoveUnbuiltMotorwaysEdit removeUnbuiltMotorwaysEdit = scope.Get<RemoveUnbuiltMotorwaysEdit>();
			removeUnbuiltMotorwaysEdit.Coordinates = coordinates;
			return removeUnbuiltMotorwaysEdit;
		}
	}
}
