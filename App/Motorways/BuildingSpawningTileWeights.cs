using System;
using System.Collections.Generic;
using FixMath;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x0200037B RID: 891
	public class BuildingSpawningTileWeights
	{
		// Token: 0x0600159F RID: 5535 RVA: 0x0004A3F4 File Offset: 0x000485F4
		public BuildingSpawningTileWeights(Dictionary<Vector3Int, Fix64> weights)
		{
			this.weights = weights;
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x0004A404 File Offset: 0x00048604
		public BuildingSpawningTileWeights(Tilemap tilemap)
		{
			this.weights = new Dictionary<Vector3Int, Fix64>();
			foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
			{
				if (tilemap.GetTile(position) != null)
				{
					WeightTile weightTile = tilemap.GetTile(position) as WeightTile;
					if (weightTile != null)
					{
						this.weights[position] = (Fix64)weightTile.tileWeight;
					}
					else
					{
						this.weights[position] = Fix64.One;
					}
				}
			}
		}

		// Token: 0x0400124A RID: 4682
		public Dictionary<Vector3Int, Fix64> weights;
	}
}
