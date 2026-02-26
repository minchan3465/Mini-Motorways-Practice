using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200038E RID: 910
	public class TrainLineDefinition
	{
		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x060015DE RID: 5598 RVA: 0x0004AE9C File Offset: 0x0004909C
		public int TileCount
		{
			get
			{
				return this._trackTilePositions.Count;
			}
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x0004AEA9 File Offset: 0x000490A9
		public Vector2Int GetRailTileCoordinates(int tileIndex)
		{
			return this._trackTilePositions[tileIndex];
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x0004AEB7 File Offset: 0x000490B7
		public RailType GetRailTileType(int tileIndex)
		{
			if (!this._trainSpawnLocations.Contains(this._trackTilePositions[tileIndex]))
			{
				return RailType.Normal;
			}
			return RailType.TrainOrigin;
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x0004AED5 File Offset: 0x000490D5
		public void AddTrack(Vector2Int trackTilePosition, RailType trackType)
		{
			this._trackTilePositions.Add(trackTilePosition);
			if (trackType == RailType.TrainOrigin)
			{
				this._trainSpawnLocations.Add(trackTilePosition);
			}
		}

		// Token: 0x04001295 RID: 4757
		public bool isLoop;

		// Token: 0x04001296 RID: 4758
		public bool isValid = true;

		// Token: 0x04001297 RID: 4759
		private readonly List<Vector2Int> _trackTilePositions = new List<Vector2Int>();

		// Token: 0x04001298 RID: 4760
		private readonly List<Vector2Int> _trainSpawnLocations = new List<Vector2Int>();
	}
}
