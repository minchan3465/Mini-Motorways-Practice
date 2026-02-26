using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000406 RID: 1030
	public class RemoveTrafficLightEdit : TileEdit
	{
		// Token: 0x170004EE RID: 1262
		// (get) Token: 0x0600192A RID: 6442 RVA: 0x0005A063 File Offset: 0x00058263
		// (set) Token: 0x0600192B RID: 6443 RVA: 0x0005A06B File Offset: 0x0005826B
		[Serialize(true, null)]
		public Vector2Int Coordinates { get; private set; }

		// Token: 0x0600192C RID: 6444 RVA: 0x0005A074 File Offset: 0x00058274
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield return tilemap.GetTile(this.Coordinates);
			yield break;
		}

		// Token: 0x0600192D RID: 6445 RVA: 0x0005A08B File Offset: 0x0005828B
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (tile.HasTrafficLight)
			{
				tile.HasTrafficLight = false;
				return true;
			}
			return false;
		}

		// Token: 0x0600192E RID: 6446 RVA: 0x0005A0A0 File Offset: 0x000582A0
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			Tile tile = tilemap.GetTile(this.Coordinates);
			if (tile != null && tile.HasTrafficLight)
			{
				upgradeDatabase.MothballUpgrade(UpgradeType.TrafficLight, 1);
				upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.TrafficLight, 1);
			}
			return true;
		}

		// Token: 0x0600192F RID: 6447 RVA: 0x0005A0D8 File Offset: 0x000582D8
		public static RemoveTrafficLightEdit Create(IScope scope, Vector2Int coordinates)
		{
			RemoveTrafficLightEdit removeTrafficLightEdit = scope.Get<RemoveTrafficLightEdit>();
			removeTrafficLightEdit.Coordinates = coordinates;
			return removeTrafficLightEdit;
		}

		// Token: 0x06001930 RID: 6448 RVA: 0x0005A0E7 File Offset: 0x000582E7
		public override void Reset()
		{
			base.Reset();
			this.Coordinates = Vector2Int.zero;
		}
	}
}
