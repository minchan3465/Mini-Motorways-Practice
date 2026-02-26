using System;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003E2 RID: 994
	public class AdjacentTileConnection
	{
		// Token: 0x06001814 RID: 6164 RVA: 0x00056149 File Offset: 0x00054349
		public AdjacentTileConnection(Vector2Int coordinates, TileDirection direction)
		{
			this._coordinates = coordinates;
			this._direction = direction;
		}

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06001815 RID: 6165 RVA: 0x0005615F File Offset: 0x0005435F
		public Vector2Int OriginCoordinates
		{
			get
			{
				return this._coordinates;
			}
		}

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06001816 RID: 6166 RVA: 0x00056167 File Offset: 0x00054367
		public TileDirection OriginDirection
		{
			get
			{
				return this._direction;
			}
		}

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06001817 RID: 6167 RVA: 0x0005616F File Offset: 0x0005436F
		public Vector2Int DestinationCoordinates
		{
			get
			{
				return TileUtilities.GetAdjacentCoordinates(this._coordinates, this._direction);
			}
		}

		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06001818 RID: 6168 RVA: 0x00056182 File Offset: 0x00054382
		public TileDirection DestinationDirection
		{
			get
			{
				return TileUtilities.GetOppositeDirection(this._direction);
			}
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x00056190 File Offset: 0x00054390
		public override bool Equals(object obj)
		{
			AdjacentTileConnection connection = obj as AdjacentTileConnection;
			return connection != null && this.Equals(connection);
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x000561B0 File Offset: 0x000543B0
		private bool Equals(AdjacentTileConnection obj)
		{
			return (this._coordinates == obj._coordinates && this._direction == obj._direction) || (this._coordinates == obj.DestinationCoordinates && this._direction == obj.DestinationDirection);
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x00056204 File Offset: 0x00054404
		public override int GetHashCode()
		{
			return this.OriginCoordinates.GetHashCode() ^ this.DestinationCoordinates.GetHashCode();
		}

		// Token: 0x040014A9 RID: 5289
		private readonly Vector2Int _coordinates;

		// Token: 0x040014AA RID: 5290
		private readonly TileDirection _direction;

		// Token: 0x020003E3 RID: 995
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x0600181C RID: 6172 RVA: 0x0005623C File Offset: 0x0005443C
			public override bool Serialize(object obj, ExportContext context)
			{
				AdjacentTileConnection node = obj as AdjacentTileConnection;
				if (node != null)
				{
					SerializerLibrary.GetSerializer<Vector2Int>().Serialize(node.OriginCoordinates, context);
					context.Writer.Write((byte)node.OriginDirection);
					return true;
				}
				return false;
			}

			// Token: 0x0600181D RID: 6173 RVA: 0x0005627F File Offset: 0x0005447F
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return new AdjacentTileConnection((Vector2Int)SerializerLibrary.GetSerializer<Vector2Int>().Deserialize(null, context), (TileDirection)context.Reader.ReadByte());
			}
		}
	}
}
