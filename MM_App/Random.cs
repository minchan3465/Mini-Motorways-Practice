using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x0200026A RID: 618
public static class Random
{
	// Token: 0x06000EB2 RID: 3762 RVA: 0x00031BA5 File Offset: 0x0002FDA5
	public static void SetSimulationSeed(uint seed, IScope scope)
	{
		global::Random.Log.Info("Set simulation seed {0}.", new object[]
		{
			seed
		});
		if (global::Random._simulationSeedGenerator == null)
		{
			global::Random._simulationSeedGenerator = scope.Get<PseudorandomGenerator>();
		}
		global::Random._simulationSeedGenerator.Seed = (ulong)seed;
	}

	// Token: 0x06000EB3 RID: 3763 RVA: 0x00031BE3 File Offset: 0x0002FDE3
	public static float Float()
	{
		return (float)global::Random.NextDouble();
	}

	// Token: 0x06000EB4 RID: 3764 RVA: 0x00031BEB File Offset: 0x0002FDEB
	public static float Float(int seed)
	{
		return (float)new System.Random(seed).NextDouble();
	}

	// Token: 0x06000EB5 RID: 3765 RVA: 0x00031BF9 File Offset: 0x0002FDF9
	public static double Double()
	{
		return global::Random.NextDouble();
	}

	// Token: 0x06000EB6 RID: 3766 RVA: 0x00031C00 File Offset: 0x0002FE00
	public static float Float(float max)
	{
		return (float)global::Random.NextDouble() * max;
	}

	// Token: 0x06000EB7 RID: 3767 RVA: 0x00031C0A File Offset: 0x0002FE0A
	public static int Int()
	{
		return global::Random.NextInt();
	}

	// Token: 0x06000EB8 RID: 3768 RVA: 0x00031C11 File Offset: 0x0002FE11
	public static int Int(int max)
	{
		if (max == 0)
		{
			return 0;
		}
		return global::Random.NextInt() % max;
	}

	// Token: 0x06000EB9 RID: 3769 RVA: 0x00031C1F File Offset: 0x0002FE1F
	public static float Range(float low, float high)
	{
		return low + (high - low) * (float)global::Random.NextDouble();
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x00031C30 File Offset: 0x0002FE30
	public static int Range(int low, int high)
	{
		int delta = high - low;
		if (delta == 0)
		{
			return 0;
		}
		return low + global::Random.NextInt() % delta;
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x00031C4F File Offset: 0x0002FE4F
	public static bool Bool()
	{
		return global::Random.NextDouble() < 0.5;
	}

	// Token: 0x06000EBC RID: 3772 RVA: 0x00031C61 File Offset: 0x0002FE61
	public static object Select(params object[] objects)
	{
		return objects[global::Random.NextInt() % objects.Length];
	}

	// Token: 0x06000EBD RID: 3773 RVA: 0x00031C70 File Offset: 0x0002FE70
	public static T AnyItem<T>(T[] items)
	{
		if (items.Length == 0)
		{
			return default(T);
		}
		return items[global::Random.NextInt() % items.Length];
	}

	// Token: 0x06000EBE RID: 3774 RVA: 0x00031C9C File Offset: 0x0002FE9C
	public static T AnyItem<T>(List<T> items)
	{
		if (items.Count == 0)
		{
			return default(T);
		}
		return items[global::Random.NextInt() % items.Count];
	}

	// Token: 0x06000EBF RID: 3775 RVA: 0x00031CD0 File Offset: 0x0002FED0
	public static Vector2 Vector2Normalized()
	{
		return new Vector2(global::Random.Range(-1f, 1f), global::Random.Range(-1f, 1f)).normalized;
	}

	// Token: 0x06000EC0 RID: 3776 RVA: 0x00031D08 File Offset: 0x0002FF08
	public static Vector3 Vector3Normalized()
	{
		return new Vector3(global::Random.Range(-1f, 1f), global::Random.Range(-1f, 1f), global::Random.Range(-1f, 1f)).normalized;
	}

	// Token: 0x06000EC1 RID: 3777 RVA: 0x00031D4F File Offset: 0x0002FF4F
	public static void ShuffleList<T>(List<T> list)
	{
		list.Shuffle<T>();
	}

	// Token: 0x06000EC2 RID: 3778 RVA: 0x00031D58 File Offset: 0x0002FF58
	public static void Shuffle<T>(this List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			i--;
			int j = global::Random.Int(i + 1);
			T value = list[j];
			list[j] = list[i];
			list[i] = value;
		}
	}

	// Token: 0x06000EC3 RID: 3779 RVA: 0x00031DA0 File Offset: 0x0002FFA0
	public static void Shuffle<T>(this List<T> list, PseudorandomGenerator rand)
	{
		int i = list.Count;
		while (i > 1)
		{
			i--;
			int j = rand.Int(i + 1);
			T value = list[j];
			list[j] = list[i];
			list[i] = value;
		}
	}

	// Token: 0x06000EC4 RID: 3780 RVA: 0x00031DE6 File Offset: 0x0002FFE6
	public static uint NextSimulationSeed()
	{
		if (global::Random._simulationSeedGenerator == null)
		{
			return 0U;
		}
		return (uint)(global::Random._simulationSeedGenerator.Int() + 1);
	}

	// Token: 0x06000EC5 RID: 3781 RVA: 0x00031DFD File Offset: 0x0002FFFD
	private static int NextInt()
	{
		return global::Random._randomSource.Next();
	}

	// Token: 0x06000EC6 RID: 3782 RVA: 0x00031E09 File Offset: 0x00030009
	private static double NextDouble()
	{
		return global::Random._randomSource.NextDouble();
	}

	// Token: 0x06000EC7 RID: 3783 RVA: 0x00031E15 File Offset: 0x00030015
	private static int RandomComparison<T>(T a, T b)
	{
		if (global::Random.NextInt() % 2 == 0)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x040008AD RID: 2221
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Random");

	// Token: 0x040008AE RID: 2222
	private static System.Random _randomSource = new System.Random(Environment.TickCount);

	// Token: 0x040008AF RID: 2223
	private static PseudorandomGenerator _simulationSeedGenerator = null;
}
