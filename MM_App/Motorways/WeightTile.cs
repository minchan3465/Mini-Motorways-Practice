using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x02000396 RID: 918
	[CreateAssetMenu(fileName = "New Weight Tile", menuName = "Tiles/Spawn Weight Tile")]
	public class WeightTile : TileBase
	{
		// Token: 0x060015F0 RID: 5616 RVA: 0x0004B574 File Offset: 0x00049774
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			base.GetTileData(position, tilemap, ref tileData);
			tileData.flags = TileFlags.LockColor;
			tileData.sprite = this.sprite;
			Color c = this.color;
			if (this.overrideWeightColor)
			{
				c.a = this.tileWeight;
			}
			tileData.color = c;
		}

		// Token: 0x040012AD RID: 4781
		public float tileWeight;

		// Token: 0x040012AE RID: 4782
		public Sprite sprite;

		// Token: 0x040012AF RID: 4783
		public Color color;

		// Token: 0x040012B0 RID: 4784
		public bool isCircle;

		// Token: 0x040012B1 RID: 4785
		public bool overrideWeightColor = true;
	}
}
