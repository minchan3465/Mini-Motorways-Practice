using System;
using Factory;
using Factory.Pools;
using FixMath;

// Token: 0x02000269 RID: 617
[Factory.Serializable(1)]
public class PseudorandomGenerator : IReusable
{
	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06000EA4 RID: 3748 RVA: 0x00031A6F File Offset: 0x0002FC6F
	// (set) Token: 0x06000EA5 RID: 3749 RVA: 0x00031A77 File Offset: 0x0002FC77
	public ulong Seed
	{
		get
		{
			return this._x;
		}
		set
		{
			this._x = value;
			this._w = value;
		}
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x00031A87 File Offset: 0x0002FC87
	public void Reset()
	{
		this._x = 0UL;
		this._w = 0UL;
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x00031A99 File Offset: 0x0002FC99
	public int Int()
	{
		return this.NextInt();
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x00031AA1 File Offset: 0x0002FCA1
	public int Int(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextInt() % (ulong)((long)max));
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x00031AB4 File Offset: 0x0002FCB4
	public ulong ULong()
	{
		return this.NextULong();
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x00031ABC File Offset: 0x0002FCBC
	public bool Bool()
	{
		return (this.NextInt() & 1) == 1;
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x00031ACC File Offset: 0x0002FCCC
	public Fix64 Fix64()
	{
		long frac = (long)this.NextInt();
		frac <<= 1;
		if (frac < 0L)
		{
			frac = -frac + 1L;
		}
		return FixMath.Fix64.FromRaw(frac);
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x00031AF6 File Offset: 0x0002FCF6
	public Fix64 Fix64(Fix64 max)
	{
		return this.Fix64() * max;
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x00031B04 File Offset: 0x0002FD04
	public override string ToString()
	{
		return string.Format("PseudorandomGenerator[x={0}, w={1}]", this._x, this._w);
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x00031B28 File Offset: 0x0002FD28
	private ulong NextULong()
	{
		this._x *= this._x;
		this._w += PseudorandomGenerator._s;
		this._x += this._w;
		this._x = (this._x >> 32 | this._x << 32);
		return this._x;
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x00031B8C File Offset: 0x0002FD8C
	private int NextInt()
	{
		return (int)this.NextULong();
	}

	// Token: 0x040008AA RID: 2218
	private ulong _x;

	// Token: 0x040008AB RID: 2219
	private ulong _w;

	// Token: 0x040008AC RID: 2220
	private static ulong _s = 13091206342165455529UL;
}
