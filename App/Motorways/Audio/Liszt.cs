using System;
using System.Collections.Generic;
using System.Linq;

namespace Motorways.Audio
{
	// Token: 0x02000650 RID: 1616
	public static class Liszt
	{
		// Token: 0x06002D18 RID: 11544 RVA: 0x000D0748 File Offset: 0x000CE948
		public static T SafeGet<T>(this List<T> list, int pointer)
		{
			if (!Diagnostics.Verify(list.Count > 0))
			{
				return default(T);
			}
			return list[Maf.FloorMod(pointer, list.Count)];
		}

		// Token: 0x06002D19 RID: 11545 RVA: 0x000D0781 File Offset: 0x000CE981
		public static int SafeIndex<T>(this List<T> list, int pointer)
		{
			if (!Diagnostics.Verify(list.Count > 0))
			{
				return -1;
			}
			return Maf.FloorMod(pointer, list.Count);
		}

		// Token: 0x06002D1A RID: 11546 RVA: 0x000D07A4 File Offset: 0x000CE9A4
		public static List<T> Make<T>(int size, Func<T> func)
		{
			return (from x in Enumerable.Range(0, size)
			select func()).ToList<T>();
		}

		// Token: 0x06002D1B RID: 11547 RVA: 0x000D07DC File Offset: 0x000CE9DC
		public static List<T> Make<T>(int size, Func<int, T> func)
		{
			return Enumerable.Range(0, size).Select((int x, int index) => func(index)).ToList<T>();
		}

		// Token: 0x06002D1C RID: 11548 RVA: 0x000D0814 File Offset: 0x000CEA14
		public static List<T> From<T>(params T[] options)
		{
			List<T> list = new List<T>();
			for (int i = 0; i < options.Length; i++)
			{
				list.Add(options[i]);
			}
			return list;
		}

		// Token: 0x06002D1D RID: 11549 RVA: 0x000D0844 File Offset: 0x000CEA44
		public static List<T> Edit<T>(this List<T> list, Func<T, int, T> func)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = func(list[i], i);
			}
			return list;
		}

		// Token: 0x06002D1E RID: 11550 RVA: 0x000D0878 File Offset: 0x000CEA78
		public static List<T> Edit<T>(this List<T> list, Func<T, T> func)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = func(list[i]);
			}
			return list;
		}

		// Token: 0x06002D1F RID: 11551 RVA: 0x000D08AC File Offset: 0x000CEAAC
		public static List<T> Flatten<T>(params List<T>[] options)
		{
			List<T> list = new List<T>();
			for (int i = 0; i < options.Length; i++)
			{
				list = list.Concat(options[i]).ToList<T>();
			}
			return list;
		}

		// Token: 0x06002D20 RID: 11552 RVA: 0x000D08DD File Offset: 0x000CEADD
		public static T Pick<T>(this List<T> list, int seed = -1)
		{
			return list[Rando.Index<T>(list, seed)];
		}

		// Token: 0x06002D21 RID: 11553 RVA: 0x000D08EC File Offset: 0x000CEAEC
		public static List<T> Shuffle<T>(this List<T> list, D20 d20 = null, int seed = -1)
		{
			D20 r = d20 ?? new D20(seed);
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int j = r.Rand.Next(i + 1);
				T value = list[j];
				list[j] = list[i];
				list[i] = value;
			}
			return list;
		}

		// Token: 0x06002D22 RID: 11554 RVA: 0x000D0944 File Offset: 0x000CEB44
		public static List<T> Palindrome<T>(this List<T> list)
		{
			List<T> flip = list.ToList<T>();
			flip.Reverse();
			flip.RemoveAt(0);
			return list.Concat(flip).ToList<T>();
		}

		// Token: 0x06002D23 RID: 11555 RVA: 0x000D0974 File Offset: 0x000CEB74
		public static List<T> Rotate<T>(this List<T> list, int delta)
		{
			if (delta == 0 || list.Count == 0)
			{
				return list;
			}
			int start_i = Maf.FloorMod(delta, list.Count);
			List<T> i = list.ToList<T>();
			IEnumerable<T> range = i.GetRange(start_i, list.Count - start_i);
			i.RemoveRange(start_i, i.Count - start_i);
			return range.Concat(i).ToList<T>();
		}

		// Token: 0x06002D24 RID: 11556 RVA: 0x000D09CB File Offset: 0x000CEBCB
		public static List<T> Whittle<T>(this List<T> list, int newCount, int seed = -1)
		{
			if (newCount >= list.Count)
			{
				return list;
			}
			while (list.Count > newCount)
			{
				list.RemoveAt(Rando.Index<T>(list, seed));
			}
			return list;
		}
	}
}
