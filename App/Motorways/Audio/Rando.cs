using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000679 RID: 1657
	public static class Rando
	{
		// Token: 0x06002DF0 RID: 11760 RVA: 0x000D5848 File Offset: 0x000D3A48
		public static float m(int seed = -1)
		{
			if (seed != -1)
			{
				return new D20(seed).Roll();
			}
			return Rando.d20.Roll();
		}

		// Token: 0x06002DF1 RID: 11761 RVA: 0x000D5864 File Offset: 0x000D3A64
		public static float Range(float min, float max, int seed = -1)
		{
			if (seed != -1)
			{
				return Mathf.Lerp(min, max, new D20(seed).Roll());
			}
			return Mathf.Lerp(min, max, Rando.d20.Roll());
		}

		// Token: 0x06002DF2 RID: 11762 RVA: 0x000D588E File Offset: 0x000D3A8E
		public static double Range(double min, double max, int seed = -1)
		{
			if (seed != -1)
			{
				return Maf.Lerp(min, max, new D20(seed).Rand.NextDouble());
			}
			return Maf.Lerp(min, max, Rando.d20.Rand.NextDouble());
		}

		// Token: 0x06002DF3 RID: 11763 RVA: 0x000D58C2 File Offset: 0x000D3AC2
		public static int Range(int min, int max, int seed = -1)
		{
			if (seed != -1)
			{
				return new D20(seed).Rand.Next(min, max);
			}
			return Rando.d20.Rand.Next(min, max);
		}

		// Token: 0x06002DF4 RID: 11764 RVA: 0x000D58EC File Offset: 0x000D3AEC
		public static T Pick<T>(List<T> list)
		{
			return list[Rando.d20.Rand.Next(list.Count)];
		}

		// Token: 0x06002DF5 RID: 11765 RVA: 0x000D5909 File Offset: 0x000D3B09
		public static T Pick<T>(params T[] options)
		{
			return options[Rando.d20.Rand.Next(options.Length)];
		}

		// Token: 0x06002DF6 RID: 11766 RVA: 0x000D5923 File Offset: 0x000D3B23
		public static T PickSeeded<T>(int seed, List<T> list)
		{
			return list[Rando.Range(0, list.Count, seed)];
		}

		// Token: 0x06002DF7 RID: 11767 RVA: 0x000D5938 File Offset: 0x000D3B38
		public static T PickSeeded<T>(int seed, params T[] options)
		{
			return options[Rando.Range(0, options.Length, seed)];
		}

		// Token: 0x06002DF8 RID: 11768 RVA: 0x000D594A File Offset: 0x000D3B4A
		public static int Index<T>(List<T> list, int seed = -1)
		{
			return Rando.Range(0, list.Count, seed);
		}

		// Token: 0x06002DF9 RID: 11769 RVA: 0x000D595C File Offset: 0x000D3B5C
		public static T EnumValue<T>(int truncateFromEnd = 0, int seed = -1)
		{
			D20 r = (seed == -1) ? Rando.d20 : new D20(seed);
			Array v = Enum.GetValues(typeof(T));
			return (T)((object)v.GetValue(r.Rand.Next(v.Length - truncateFromEnd)));
		}

		// Token: 0x06002DFA RID: 11770 RVA: 0x000D59AC File Offset: 0x000D3BAC
		public static void Repeat(int times, Action<int> action)
		{
			List<int> key_ii = Rando.Numbers(times, 0);
			for (int i = 0; i < times; i++)
			{
				action(key_ii[i]);
			}
		}

		// Token: 0x06002DFB RID: 11771 RVA: 0x000D59DA File Offset: 0x000D3BDA
		public static bool FlipCoin(float chance = 0.5f)
		{
			return Rando.d20.Luck(chance);
		}

		// Token: 0x06002DFC RID: 11772 RVA: 0x000D59E8 File Offset: 0x000D3BE8
		public static List<int> Numbers(int numbers, int lowestInt = 0)
		{
			List<int> i = new List<int>();
			for (int j = 0; j < numbers; j++)
			{
				i.Add(j + lowestInt);
			}
			return i.Shuffle(null, -1);
		}

		// Token: 0x06002DFD RID: 11773 RVA: 0x000D5A18 File Offset: 0x000D3C18
		public static float Random(this Vector2 v2, int seed = -1)
		{
			return Rando.Range(v2.x, v2.y, seed);
		}

		// Token: 0x06002DFE RID: 11774 RVA: 0x000D5A2C File Offset: 0x000D3C2C
		public static float Random(this Vector2Int v2, int seed = -1)
		{
			return (float)Rando.Range(v2.x, v2.y, seed);
		}

		// Token: 0x040027E3 RID: 10211
		private static D20 d20 = new D20(-1);
	}
}
