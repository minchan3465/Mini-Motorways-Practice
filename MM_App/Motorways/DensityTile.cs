using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x0200037D RID: 893
	[CreateAssetMenu(fileName = "New Density Tile", menuName = "Tiles/Spawn Density Tile")]
	public class DensityTile : TileBase
	{
		// Token: 0x060015A3 RID: 5539 RVA: 0x0004A580 File Offset: 0x00048780
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			base.GetTileData(position, tilemap, ref tileData);
			tileData.flags = TileFlags.LockColor;
			tileData.sprite = this.sprite;
			tileData.color = this.color;
		}

		// Token: 0x0400124E RID: 4686
		public Sprite sprite;

		// Token: 0x0400124F RID: 4687
		public Color color;

		// Token: 0x04001250 RID: 4688
		public DensityGroup group;
	}
}
