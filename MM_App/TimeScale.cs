using System;
using UnityEngine;

// Token: 0x0200024F RID: 591
public class TimeScale
{
	// Token: 0x06000E1C RID: 3612 RVA: 0x0002FA61 File Offset: 0x0002DC61
	public TimeScale(float scale)
	{
		this._scale = scale;
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06000E1D RID: 3613 RVA: 0x0002FA70 File Offset: 0x0002DC70
	public float Scale
	{
		get
		{
			return this._scale;
		}
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x0002FA78 File Offset: 0x0002DC78
	public float ScaleTime(float time)
	{
		return time * this._scale;
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x0002FA84 File Offset: 0x0002DC84
	public static TimeScale FromScale(float scale)
	{
		if (Mathf.Approximately(scale, TimeScale.SingleSlow.Scale))
		{
			return TimeScale.SingleSlow;
		}
		if (Mathf.Approximately(scale, TimeScale.Double.Scale))
		{
			return TimeScale.Double;
		}
		if (Mathf.Approximately(scale, TimeScale.DoubleSlow.Scale))
		{
			return TimeScale.DoubleSlow;
		}
		return TimeScale.Single;
	}

	// Token: 0x04000843 RID: 2115
	private readonly float _scale;

	// Token: 0x04000844 RID: 2116
	public static readonly TimeScale Single = new TimeScale(1f);

	// Token: 0x04000845 RID: 2117
	public static readonly TimeScale SingleSlow = new TimeScale(0.75f);

	// Token: 0x04000846 RID: 2118
	public static readonly TimeScale Double = new TimeScale(2f);

	// Token: 0x04000847 RID: 2119
	public static readonly TimeScale DoubleSlow = new TimeScale(1.5f);

	// Token: 0x04000848 RID: 2120
	public static readonly TimeScale ExtraFast = new TimeScale(3.5f);
}
