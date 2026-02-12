using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000363 RID: 867
	public class BoatPathLineDefinition
	{
		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x0600153C RID: 5436 RVA: 0x00048B1B File Offset: 0x00046D1B
		public int TileCount
		{
			get
			{
				return this._boatPathTilePositions.Count;
			}
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x00048B28 File Offset: 0x00046D28
		public Vector2Int GetBoatPathTileCoordinates(int tileIndex)
		{
			return this._boatPathTilePositions[tileIndex];
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x00048B36 File Offset: 0x00046D36
		public BoatPathType GetBoatPathTileType(int tileIndex)
		{
			if (!this._boatSpawnLocations.Contains(this._boatPathTilePositions[tileIndex]))
			{
				return BoatPathType.Normal;
			}
			return BoatPathType.BoatOrigin;
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x00048B54 File Offset: 0x00046D54
		public void AddBoatPath(Vector2Int boatPathTilePosition, BoatPathType boatPathType)
		{
			this._boatPathTilePositions.Add(boatPathTilePosition);
			if (boatPathType == BoatPathType.BoatOrigin)
			{
				this._boatSpawnLocations.Add(boatPathTilePosition);
			}
		}

		// Token: 0x040011C4 RID: 4548
		public bool isLoop;

		// Token: 0x040011C5 RID: 4549
		public bool isValid = true;

		// Token: 0x040011C6 RID: 4550
		private readonly List<Vector2Int> _boatPathTilePositions = new List<Vector2Int>();

		// Token: 0x040011C7 RID: 4551
		private readonly List<Vector2Int> _boatSpawnLocations = new List<Vector2Int>();
	}
}
