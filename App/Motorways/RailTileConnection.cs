using System;
using Factory;

namespace Motorways
{
	// Token: 0x02000419 RID: 1049
	public struct RailTileConnection : IComparable
	{
		// Token: 0x060019DD RID: 6621 RVA: 0x0005D175 File Offset: 0x0005B375
		public RailTileConnection(TileDirection inputDirection, TileDirection outputDirection)
		{
			this.input = inputDirection;
			this.output = outputDirection;
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x0005D185 File Offset: 0x0005B385
		public RailTileConnection GetRotatedConnection(RoadTileRotation rotation)
		{
			return new RailTileConnection(TileUtilities.GetRotatedDirection(this.input, rotation), TileUtilities.GetRotatedDirection(this.output, rotation));
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x0005D1A4 File Offset: 0x0005B3A4
		public TileDirection GetOtherDirection(TileDirection direction)
		{
			if (this.input == direction)
			{
				return this.output;
			}
			if (this.output == direction)
			{
				return this.input;
			}
			return TileDirection.None;
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x060019E0 RID: 6624 RVA: 0x0005D1C7 File Offset: 0x0005B3C7
		public bool IsDeadEnd
		{
			get
			{
				return (this.input != this.output && this.input == TileDirection.None) || this.output == TileDirection.None;
			}
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x0005D1EB File Offset: 0x0005B3EB
		public static bool operator ==(RailTileConnection lhs, RailTileConnection rhs)
		{
			return lhs.Equals(rhs);
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x0005D1F5 File Offset: 0x0005B3F5
		public static bool operator !=(RailTileConnection lhs, RailTileConnection rhs)
		{
			return !lhs.Equals(rhs);
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x0005D204 File Offset: 0x0005B404
		public override bool Equals(object obj)
		{
			if (obj is RailTileConnection)
			{
				RailTileConnection railTileConnection = (RailTileConnection)obj;
				return this.Equals(railTileConnection);
			}
			return false;
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x0005D229 File Offset: 0x0005B429
		public bool Equals(RailTileConnection otherConnection)
		{
			return this.input == otherConnection.input && this.output == otherConnection.output;
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x0005D24C File Offset: 0x0005B44C
		public int CompareTo(object obj)
		{
			if (obj is RailTileConnection)
			{
				RailTileConnection connection = (RailTileConnection)obj;
				return this.CompareTo(connection);
			}
			return 1;
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x0005D274 File Offset: 0x0005B474
		public int CompareTo(RailTileConnection otherConnection)
		{
			int comparison = this.input - otherConnection.input;
			if (comparison != 0)
			{
				return comparison;
			}
			comparison = this.output - otherConnection.output;
			if (comparison != 0)
			{
				return comparison;
			}
			return 0;
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x0005D2A8 File Offset: 0x0005B4A8
		public override int GetHashCode()
		{
			return (int)(this.input + 100 + (int)(this.output + 1));
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x0005D2BC File Offset: 0x0005B4BC
		public override string ToString()
		{
			return string.Format("{0} to {1}", this.input, this.output);
		}

		// Token: 0x040015BD RID: 5565
		public readonly TileDirection input;

		// Token: 0x040015BE RID: 5566
		public readonly TileDirection output;

		// Token: 0x040015BF RID: 5567
		public static readonly RailTileConnection InvalidConnection = new RailTileConnection(TileDirection.None, TileDirection.None);

		// Token: 0x0200041A RID: 1050
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x060019EA RID: 6634 RVA: 0x0005D2EC File Offset: 0x0005B4EC
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is RailTileConnection)
				{
					RailTileConnection connection = (RailTileConnection)obj;
					context.Writer.Write((byte)connection.input);
					context.Writer.Write((byte)connection.output);
					return true;
				}
				return false;
			}

			// Token: 0x060019EB RID: 6635 RVA: 0x0005D330 File Offset: 0x0005B530
			public override object Deserialize(object existingObj, ImportContext context)
			{
				TileDirection inputDirection = TileUtilities.DeserializeDirection(context.Reader.ReadByte());
				TileDirection output = TileUtilities.DeserializeDirection(context.Reader.ReadByte());
				return new RailTileConnection(inputDirection, output);
			}
		}
	}
}
