using System;
using Factory;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003E8 RID: 1000
	public readonly struct CornerAdjacencyReference : IComparable
	{
		// Token: 0x06001838 RID: 6200 RVA: 0x00056769 File Offset: 0x00054969
		public CornerAdjacencyReference(Vector2Int tileCoordinate, TileDirection cornerDirection)
		{
			this.tileCoordinate = tileCoordinate;
			this.cornerDirection = cornerDirection;
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x00056779 File Offset: 0x00054979
		public override bool Equals(object obj)
		{
			return obj is CornerAdjacencyReference && this.CompareTo(obj) == 0;
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x00056790 File Offset: 0x00054990
		public override int GetHashCode()
		{
			return this.tileCoordinate.GetHashCode() ^ this.cornerDirection.GetHashCode();
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x000567C4 File Offset: 0x000549C4
		public int CompareTo(object obj)
		{
			if (obj == null || !(obj is CornerAdjacencyReference))
			{
				return 1;
			}
			CornerAdjacencyReference otherCornerDefinition = (CornerAdjacencyReference)obj;
			if (otherCornerDefinition.tileCoordinate.x != this.tileCoordinate.x)
			{
				return this.tileCoordinate.x - otherCornerDefinition.tileCoordinate.x;
			}
			if (otherCornerDefinition.tileCoordinate.y != this.tileCoordinate.y)
			{
				return this.tileCoordinate.y - otherCornerDefinition.tileCoordinate.y;
			}
			if (this.cornerDirection != otherCornerDefinition.cornerDirection)
			{
				return this.cornerDirection - otherCornerDefinition.cornerDirection;
			}
			return 0;
		}

		// Token: 0x040014B5 RID: 5301
		public readonly Vector2Int tileCoordinate;

		// Token: 0x040014B6 RID: 5302
		public readonly TileDirection cornerDirection;

		// Token: 0x020003E9 RID: 1001
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x0600183C RID: 6204 RVA: 0x0005687C File Offset: 0x00054A7C
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is CornerAdjacencyReference)
				{
					Vector2Int tileCoordinates = ((CornerAdjacencyReference)obj).tileCoordinate;
					TileDirection cornerDirection = ((CornerAdjacencyReference)obj).cornerDirection;
					context.Writer.Write(tileCoordinates.x);
					context.Writer.Write(tileCoordinates.y);
					context.Writer.Write((int)cornerDirection);
					return true;
				}
				return false;
			}

			// Token: 0x0600183D RID: 6205 RVA: 0x000568DC File Offset: 0x00054ADC
			public override object Deserialize(object existingObj, ImportContext context)
			{
				Vector2Int tileCoordinate = new Vector2Int(context.Reader.ReadInt32(), context.Reader.ReadInt32());
				TileDirection cornerDirection = (TileDirection)context.Reader.ReadInt32();
				return new CornerAdjacencyReference(tileCoordinate, cornerDirection);
			}
		}
	}
}
