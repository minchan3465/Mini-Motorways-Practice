using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003EF RID: 1007
	public class AddRoadLineEdit : TileEdit
	{
		// Token: 0x0600186D RID: 6253 RVA: 0x00057383 File Offset: 0x00055583
		public override IEnumerable<Tile> GetAffectedTiles(ITilemap tilemap)
		{
			Vector2Int directionOffset = TileUtilities.GetAdjacencyOffsetForDirection(this._direction);
			int num;
			for (int tileIndex = 0; tileIndex <= this._length; tileIndex = num)
			{
				yield return tilemap.GetOrCreateTile(this._originCoordinates + directionOffset * tileIndex);
				num = tileIndex + 1;
			}
			yield break;
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0005739C File Offset: 0x0005559C
		public override bool ApplyToAffectedTile(Tile tile)
		{
			if (tile.Coordinates != this._destinationCoordinates)
			{
				tile.SetNodeState(new RoadTileNode(this._direction, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
			}
			if (tile.Coordinates != this._originCoordinates)
			{
				tile.SetNodeState(new RoadTileNode(TileUtilities.GetOppositeDirection(this._direction), RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
			}
			return true;
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ApplyToUpgradeDatabase(UpgradeDatabase upgradeDatabase, ITilemap tilemap)
		{
			return true;
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x00057401 File Offset: 0x00055601
		public override void Reset()
		{
			base.Reset();
			this._originCoordinates = default(Vector2Int);
			this._destinationCoordinates = default(Vector2Int);
			this._direction = TileDirection.North;
			this._length = 0;
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x0005742F File Offset: 0x0005562F
		public static AddRoadLineEdit Create(IScope scope, Vector2Int originCoordinates, TileDirection direction, int length)
		{
			AddRoadLineEdit addRoadLineEdit = scope.Get<AddRoadLineEdit>();
			addRoadLineEdit._originCoordinates = originCoordinates;
			addRoadLineEdit._destinationCoordinates = originCoordinates + TileUtilities.GetAdjacencyOffsetForDirection(direction) * length;
			addRoadLineEdit._direction = direction;
			addRoadLineEdit._length = length;
			return addRoadLineEdit;
		}

		// Token: 0x040014D6 RID: 5334
		private Vector2Int _originCoordinates;

		// Token: 0x040014D7 RID: 5335
		private Vector2Int _destinationCoordinates;

		// Token: 0x040014D8 RID: 5336
		private TileDirection _direction;

		// Token: 0x040014D9 RID: 5337
		private int _length;
	}
}
