using System;
using System.Collections.Generic;
using Factory;

namespace Motorways
{
	// Token: 0x0200043D RID: 1085
	public struct TileDirectionBitfield : IEquatable<TileDirectionBitfield>
	{
		// Token: 0x06001AE4 RID: 6884 RVA: 0x00062B62 File Offset: 0x00060D62
		public TileDirectionBitfield(TileDirection direction)
		{
			this._bitfield = 0;
			this[direction] = true;
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x00062B74 File Offset: 0x00060D74
		public TileDirectionBitfield(IEnumerable<TileDirection> directions)
		{
			this._bitfield = 0;
			foreach (TileDirection direction in directions)
			{
				this[direction] = true;
			}
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x00062BC4 File Offset: 0x00060DC4
		public TileDirectionBitfield(int bitfield)
		{
			this._bitfield = bitfield;
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x00062BCD File Offset: 0x00060DCD
		public TileDirectionBitfield.Enumerator GetEnumerator()
		{
			return new TileDirectionBitfield.Enumerator(this._bitfield);
		}

		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06001AE8 RID: 6888 RVA: 0x00062BDC File Offset: 0x00060DDC
		public int Count
		{
			get
			{
				int count = 0;
				for (int directionIndex = 0; directionIndex < 8; directionIndex++)
				{
					if ((this._bitfield & 1 << directionIndex) != 0)
					{
						count++;
					}
				}
				return count;
			}
		}

		// Token: 0x17000537 RID: 1335
		public TileDirection this[int index]
		{
			get
			{
				int toFindCount = index + 1;
				for (int directionIndex = 0; directionIndex < 8; directionIndex++)
				{
					if ((this._bitfield & 1 << directionIndex) != 0)
					{
						toFindCount--;
					}
					if (toFindCount == 0)
					{
						return (TileDirection)directionIndex;
					}
				}
				return TileDirection.None;
			}
		}

		// Token: 0x17000538 RID: 1336
		public bool this[TileDirection direction]
		{
			get
			{
				return (this._bitfield & 1 << (int)direction) != 0;
			}
			set
			{
				if (value)
				{
					this._bitfield |= 1 << (int)direction;
					return;
				}
				this._bitfield &= ~(1 << (int)direction);
			}
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x00062C81 File Offset: 0x00060E81
		public void Clear()
		{
			this._bitfield = 0;
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x00062C8A File Offset: 0x00060E8A
		public bool Equals(IEnumerable<TileDirection> directions)
		{
			return this.Equals(new TileDirectionBitfield(directions));
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x00062C98 File Offset: 0x00060E98
		public override string ToString()
		{
			List<string> directionStrings = new List<string>();
			foreach (TileDirection direction in this)
			{
				directionStrings.Add(direction.ToString());
			}
			return string.Format("TileDirectionBitfield[{0}]", string.Join(", ", directionStrings));
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x06001AEF RID: 6895 RVA: 0x00062CED File Offset: 0x00060EED
		public int Bits
		{
			get
			{
				return this._bitfield;
			}
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x00062CF5 File Offset: 0x00060EF5
		public static TileDirectionBitfield operator ~(TileDirectionBitfield bitfield)
		{
			return new TileDirectionBitfield(~bitfield._bitfield);
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x00062D03 File Offset: 0x00060F03
		public bool Equals(TileDirectionBitfield other)
		{
			return this._bitfield == other._bitfield;
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x00062D14 File Offset: 0x00060F14
		public override bool Equals(object obj)
		{
			if (obj is TileDirectionBitfield)
			{
				TileDirectionBitfield other = (TileDirectionBitfield)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x00062CED File Offset: 0x00060EED
		public override int GetHashCode()
		{
			return this._bitfield;
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x00062D39 File Offset: 0x00060F39
		public static bool operator ==(TileDirectionBitfield left, TileDirectionBitfield right)
		{
			return left.Equals(right);
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x00062D43 File Offset: 0x00060F43
		public static bool operator !=(TileDirectionBitfield left, TileDirectionBitfield right)
		{
			return !left.Equals(right);
		}

		// Token: 0x04001669 RID: 5737
		public static readonly TileDirectionBitfield All = new TileDirectionBitfield(255);

		// Token: 0x0400166A RID: 5738
		public static readonly TileDirectionBitfield None = new TileDirectionBitfield(0);

		// Token: 0x0400166B RID: 5739
		private int _bitfield;

		// Token: 0x0200043E RID: 1086
		public struct Enumerator
		{
			// Token: 0x06001AF7 RID: 6903 RVA: 0x00062D6C File Offset: 0x00060F6C
			public Enumerator(int bitfield)
			{
				this._bitfield = bitfield;
				this._currentDirection = -1;
			}

			// Token: 0x1700053A RID: 1338
			// (get) Token: 0x06001AF8 RID: 6904 RVA: 0x00062D7C File Offset: 0x00060F7C
			public TileDirection Current
			{
				get
				{
					return (TileDirection)this._currentDirection;
				}
			}

			// Token: 0x06001AF9 RID: 6905 RVA: 0x00062D84 File Offset: 0x00060F84
			public bool MoveNext()
			{
				do
				{
					this._currentDirection++;
				}
				while (this._currentDirection < 8 && (this._bitfield & 1 << this._currentDirection) == 0);
				return this._currentDirection < 8;
			}

			// Token: 0x0400166C RID: 5740
			private readonly int _bitfield;

			// Token: 0x0400166D RID: 5741
			private int _currentDirection;
		}

		// Token: 0x0200043F RID: 1087
		public class Serializer : PrimitiveSerializer
		{
			// Token: 0x06001AFA RID: 6906 RVA: 0x00062DBC File Offset: 0x00060FBC
			public override bool Serialize(object obj, ExportContext context)
			{
				if (obj is TileDirectionBitfield)
				{
					int bitfield = ((TileDirectionBitfield)obj)._bitfield;
					context.Writer.Write(bitfield);
					return true;
				}
				return false;
			}

			// Token: 0x06001AFB RID: 6907 RVA: 0x00062DEC File Offset: 0x00060FEC
			public override object Deserialize(object existingObj, ImportContext context)
			{
				return new TileDirectionBitfield(context.Reader.ReadInt32());
			}
		}
	}
}
