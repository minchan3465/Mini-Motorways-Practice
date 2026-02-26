using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x02000390 RID: 912
	[CreateAssetMenu(fileName = "New Tree Tile", menuName = "Tiles/Spawn Tree Tile")]
	public class TreeTile : TileBase
	{
		// Token: 0x060015E8 RID: 5608 RVA: 0x0004B3DF File Offset: 0x000495DF
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			base.GetTileData(position, tilemap, ref tileData);
			tileData.flags = TileFlags.LockColor;
			tileData.sprite = this.sprite;
		}

		// Token: 0x0400129A RID: 4762
		public int prefabIndex;

		// Token: 0x0400129B RID: 4763
		public Sprite sprite;
	}
}
