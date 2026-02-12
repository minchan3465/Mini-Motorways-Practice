using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x020003A1 RID: 929
	public class SubdividableTilemap
	{
		// Token: 0x06001615 RID: 5653 RVA: 0x0004BE85 File Offset: 0x0004A085
		public SubdividableTilemap(Tilemap targetTilemap, int subdivisionSteps)
		{
			this._targetTilemap = targetTilemap;
			this._subdivisionSteps = subdivisionSteps;
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x0004BE9B File Offset: 0x0004A09B
		private Vector3Int TransformPosition(Vector3Int position)
		{
			return Vector3Int.FloorToInt(position / (float)this._subdivisionSteps);
		}

		// Token: 0x06001617 RID: 5655 RVA: 0x0004BEB4 File Offset: 0x0004A0B4
		public bool Contains(Vector3Int position)
		{
			return this._targetTilemap.cellBounds.Contains(this.TransformPosition(position));
		}

		// Token: 0x06001618 RID: 5656 RVA: 0x0004BEDB File Offset: 0x0004A0DB
		public bool HasTile(Vector3Int position)
		{
			return this._targetTilemap.HasTile(this.TransformPosition(position));
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001619 RID: 5657 RVA: 0x0004BEF0 File Offset: 0x0004A0F0
		public BoundsInt.PositionEnumerator AllPositionsWithin
		{
			get
			{
				BoundsInt cellBounds = this._targetTilemap.cellBounds;
				cellBounds.max += new Vector3Int(cellBounds.max.x * (this._subdivisionSteps - 1), cellBounds.max.y * (this._subdivisionSteps - 1), 0);
				cellBounds.min += new Vector3Int(cellBounds.min.x * (this._subdivisionSteps - 1), cellBounds.min.y * (this._subdivisionSteps - 1), 0);
				return cellBounds.allPositionsWithin;
			}
		}

		// Token: 0x040012E0 RID: 4832
		private readonly Tilemap _targetTilemap;

		// Token: 0x040012E1 RID: 4833
		private readonly int _subdivisionSteps;
	}
}
