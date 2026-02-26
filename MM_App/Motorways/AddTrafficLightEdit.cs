using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003F3 RID: 1011
	public class AddTrafficLightEdit : TileEdit
	{
		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x0600188C RID: 6284 RVA: 0x00057CAB File Offset: 0x00055EAB
		// (set) Token: 0x0600188D RID: 6285 RVA: 0x00057CB3 File Offset: 0x00055EB3
		[Serialize(true, null)]
		public Vector2Int OriginCoordinates { get; private set; }

		// Token: 0x0600188E RID: 6286 RVA: 0x00057CBC File Offset: 0x00055EBC
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			yield return tilemap.GetOrCreateTile(this.OriginCoordinates);
			yield break;
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x00057CD3 File Offset: 0x00055ED3
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (base.CanApplyToSimulation && Diagnostics.Verify(TileEditor.TileSupportsTrafficLight(tile), "Can't apply a traffic light to this tile"))
			{
				tile.HasTrafficLight = true;
				return true;
			}
			return false;
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x00057CF9 File Offset: 0x00055EF9
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			return upgradeDatabase.ConsumeUpgrade(UpgradeType.TrafficLight, 1);
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x00057D03 File Offset: 0x00055F03
		public static AddTrafficLightEdit Create(IScope scope, Vector2Int originCoordinates)
		{
			AddTrafficLightEdit addTrafficLightEdit = scope.Get<AddTrafficLightEdit>();
			addTrafficLightEdit.OriginCoordinates = originCoordinates;
			return addTrafficLightEdit;
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x00057D12 File Offset: 0x00055F12
		public override void Reset()
		{
			base.Reset();
			this.OriginCoordinates = Vector2Int.zero;
		}
	}
}
