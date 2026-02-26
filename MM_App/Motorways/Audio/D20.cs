using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000677 RID: 1655
	public class D20
	{
		// Token: 0x06002DE4 RID: 11748 RVA: 0x000D5670 File Offset: 0x000D3870
		public D20(int seed = -1)
		{
			this.Rand = ((seed == -1) ? new System.Random() : new System.Random(seed));
			this.Seed = seed;
		}

		// Token: 0x06002DE5 RID: 11749 RVA: 0x000D5696 File Offset: 0x000D3896
		public float Roll()
		{
			return (float)this.Rand.NextDouble();
		}

		// Token: 0x06002DE6 RID: 11750 RVA: 0x000D56A4 File Offset: 0x000D38A4
		public float Range(float min, float max)
		{
			return Mathf.Lerp(min, max, (float)this.Rand.NextDouble());
		}

		// Token: 0x06002DE7 RID: 11751 RVA: 0x000D56B9 File Offset: 0x000D38B9
		public int Range(int min, int max)
		{
			return this.Rand.Next(min, max + 1);
		}

		// Token: 0x06002DE8 RID: 11752 RVA: 0x000D56CA File Offset: 0x000D38CA
		public T Pick<T>(List<T> list)
		{
			return list[this.Rand.Next(list.Count)];
		}

		// Token: 0x06002DE9 RID: 11753 RVA: 0x000D56E3 File Offset: 0x000D38E3
		public T Pick<T>(params T[] options)
		{
			return options[this.Rand.Next(options.Length)];
		}

		// Token: 0x06002DEA RID: 11754 RVA: 0x000D56F9 File Offset: 0x000D38F9
		public int Index<T>(List<T> list)
		{
			return this.Range(0, list.Count - 1);
		}

		// Token: 0x06002DEB RID: 11755 RVA: 0x000D570C File Offset: 0x000D390C
		public T EnumValue<T>(int truncateFromEnd = 0)
		{
			Array v = Enum.GetValues(typeof(T));
			return (T)((object)v.GetValue(this.Rand.Next(v.Length - truncateFromEnd)));
		}

		// Token: 0x06002DEC RID: 11756 RVA: 0x000D5748 File Offset: 0x000D3948
		public float[] Frag(int nbSteps, float duration, float noise = 1f, float minFrag = -1f, float maxFrag = -1f)
		{
			float uniformStep = duration / (float)nbSteps;
			if (minFrag < 0f)
			{
				minFrag = uniformStep * 0.5f;
			}
			if (maxFrag < 0f)
			{
				maxFrag = duration * 0.75f;
			}
			float[] noisyFrags = new float[nbSteps];
			for (int k = 0; k < noisyFrags.Length; k++)
			{
				noisyFrags[k] = this.Range(duration * minFrag, duration * maxFrag);
			}
			float sum = noisyFrags.Sum();
			noisyFrags = noisyFrags.Select((float x, int i) => x / sum * duration).ToArray<float>();
			noisyFrags[noisyFrags.Length - 1] -= noisyFrags.Sum() - duration;
			for (int j = 0; j < noisyFrags.Length; j++)
			{
				noisyFrags[j] = Mathf.Lerp(uniformStep, noisyFrags[j], noise);
			}
			return noisyFrags;
		}

		// Token: 0x06002DED RID: 11757 RVA: 0x000D5827 File Offset: 0x000D3A27
		public bool Luck(float chance = 0.05f)
		{
			return this.Roll() < Mathf.Clamp01(chance);
		}

		// Token: 0x040027DF RID: 10207
		public int Seed;

		// Token: 0x040027E0 RID: 10208
		public System.Random Rand;
	}
}
