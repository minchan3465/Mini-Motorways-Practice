using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x0200065A RID: 1626
	public static class Tune
	{
		// Token: 0x06002D49 RID: 11593 RVA: 0x000D0F9D File Offset: 0x000CF19D
		public static int freqRatioToCents(float freqRatio)
		{
			return (int)Mathf.Round(1200f * Mathf.Log(freqRatio, 2f));
		}

		// Token: 0x06002D4A RID: 11594 RVA: 0x000D0FB6 File Offset: 0x000CF1B6
		public static float centsToFreqRatio(int cents)
		{
			return Mathf.Pow(2f, (float)cents / 1200f);
		}

		// Token: 0x0400276A RID: 10090
		public static readonly float[] JUST = new float[]
		{
			1f,
			1.066667f,
			1.125f,
			1.2f,
			1.25f,
			1.333333f,
			1.4f,
			1.5f,
			1.6f,
			1.666667f,
			1.777778f,
			1.875f
		};
	}
}
