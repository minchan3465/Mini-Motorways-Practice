using System;
using Factory;

namespace Motorways
{
	// Token: 0x020003E5 RID: 997
	public struct BoatPathTileConnection : IComparable
	{
		// Token: 0x06001824 RID: 6180 RVA: 0x00056505 File Offset: 0x00054705
		public BoatPathTileConnection(TileDirection inputDirection, TileDirection outputDirection)
		{
			this.input = inputDirection;
			this.output = outputDirection;
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x00056515 File Offset: 0x00054715
		public BoatPathTileConnection GetRotatedConnection(RoadTileRotation rotation)
		{
			return new BoatPathTileConnection(TileUtilities.GetRotatedDirection(this.input, rotation), TileUtilities.GetRotatedDirection(this.output, rotation));
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x00056534 File Offset: 0x00054734
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

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06001827 RID: 6183 RVA: 0x00056557 File Offset: 0x00054757
		public bool IsDeadEnd
		{
			get
			{
				return (this.input != this.output && this.input == TileDirection.None) || this.output == TileDirection.None;
			}
		}

		// Token: 0x06001828 RID: 6184 RVA: 0x0005657B File Offset: 0x0005477B
		public static bool operator ==(BoatPathTileConnection lhs, BoatPathTileConnection rhs)
		{
			return lhs.Equals(rhs);
		}

		// Token: 0x06001829 RID: 6185 RVA: 0x00056585 File Offset: 0x00054785
		public static bool operator !=(BoatPathTileConnection lhs, BoatPathTileConnection rhs)
		{
			return !lhs.Equals(rhs);
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x00056594 File Offset: 0x00054794
		public override bool Equals(object obj)
		{
			if (obj is BoatPathTileConnection)
			{
				BoatPathTileConnection BoatPathTileConnection = (BoatPathTileConnection)obj;
				return this.Equals(BoatPathTileConnection);
			}
			return false;
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x000565B9 File Offset: 0x000547B9
		public bool Equals(BoatPathTileConnection otherConnection)
		{
			return this.input == otherConnection.input && this.output == otherConnection.output;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x000565DC File Offset: 0x000547DC
		public int CompareTo(object obj)
		{
			if (obj is BoatPathTileConnection)
			{
				BoatPathTileConnection connection = (BoatPathTileConnection)obj;
				return this.CompareTo(connection);
			}
			return 1;
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x00056604 File Offset: 0x00054804
		public int CompareTo(BoatPathTileConnection otherConnection)
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

		// Token: 0x0600182E RID: 6190 RVA: 0x00056638 File Offset: 0x00054838
		public override int GetHashCode()
		{
			return (int)(this.input + 100 + (int)(this.output + 1));
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x0005664C File Offset: 0x0005484C
		public override string ToString()
		{
			return string.Format("{0} to {1}", this.input, this.output);
		}

		// Token: 0x040014AF RID: 5295
		public readonly TileDirection input;

		// Token: 0x040014B0 RID: 5296
		public readonly TileDirection output;

		// Token: 0x040014B1 RID: 5297
		public static readonly BoatPathTileConnection InvalidConnection = new BoatPathTileConnection(TileDirection.None, TileDirection.None);

		// Token: 0x020003E6 RID: 998
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x06001831 RID: 6193 RVA: 0x0005667C File Offset: 0x0005487C
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is BoatPathTileConnection)
				{
					BoatPathTileConnection connection = (BoatPathTileConnection)obj;
					context.Writer.Write((byte)connection.input);
					context.Writer.Write((byte)connection.output);
					return true;
				}
				return false;
			}

			// Token: 0x06001832 RID: 6194 RVA: 0x000566C0 File Offset: 0x000548C0
			public override object Deserialize(object existingObj, ImportContext context)
			{
				TileDirection inputDirection = TileUtilities.DeserializeDirection(context.Reader.ReadByte());
				TileDirection output = TileUtilities.DeserializeDirection(context.Reader.ReadByte());
				return new BoatPathTileConnection(inputDirection, output);
			}
		}
	}
}
