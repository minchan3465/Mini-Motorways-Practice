using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000658 RID: 1624
	public static class Maf
	{
		// Token: 0x06002D37 RID: 11575 RVA: 0x000D0B6C File Offset: 0x000CED6C
		public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			smoothTime = Mathf.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float num3 = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			float num4 = current - target;
			float num5 = target;
			float num6 = maxSpeed * smoothTime;
			num4 = Mathf.Clamp(num4, -num6, num6);
			target = current - num4;
			float num7 = (currentVelocity + num * num4) * deltaTime;
			currentVelocity = (currentVelocity - num * num7) * num3;
			float num8 = target + (num4 + num7) * num3;
			if (num5 - current > 0f == num8 > num5)
			{
				num8 = num5;
				currentVelocity = (num8 - num5) / deltaTime;
			}
			return num8;
		}

		// Token: 0x06002D38 RID: 11576 RVA: 0x000D0C18 File Offset: 0x000CEE18
		public static double Clamp(double value, double min, double max)
		{
			return Math.Max(min, Math.Min(value, max));
		}

		// Token: 0x06002D39 RID: 11577 RVA: 0x000D0C27 File Offset: 0x000CEE27
		public static float Deviate(float val, float percent)
		{
			return val + val * percent;
		}

		// Token: 0x06002D3A RID: 11578 RVA: 0x000D0C2E File Offset: 0x000CEE2E
		public static float MoveTowards(float current, float target, float maxDelta)
		{
			if (Mathf.Abs(target - current) <= maxDelta)
			{
				return target;
			}
			return current + Mathf.Sign(target - current) * maxDelta;
		}

		// Token: 0x06002D3B RID: 11579 RVA: 0x000D0C4C File Offset: 0x000CEE4C
		public static void Repeat(int times, Action<int> action, bool countDown = false)
		{
			if (countDown)
			{
				for (int i = times - 1; i >= 0; i--)
				{
					action(i);
				}
				return;
			}
			for (int j = 0; j < times; j++)
			{
				action(j);
			}
		}

		// Token: 0x06002D3C RID: 11580 RVA: 0x000D0C88 File Offset: 0x000CEE88
		public static void Repeat(int times, Action action)
		{
			for (int i = 0; i < times; i++)
			{
				action();
			}
		}

		// Token: 0x06002D3D RID: 11581 RVA: 0x000D0CA7 File Offset: 0x000CEEA7
		public static float Reflect(float x, float ceil)
		{
			if (x <= ceil)
			{
				return x;
			}
			return 1f - ceil - (x - ceil);
		}

		// Token: 0x06002D3E RID: 11582 RVA: 0x0004540B File Offset: 0x0004360B
		public static int FloorMod(int x, int m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x06002D3F RID: 11583 RVA: 0x0004540B File Offset: 0x0004360B
		public static float FloorMod(float x, float m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x06002D40 RID: 11584 RVA: 0x000D0CBC File Offset: 0x000CEEBC
		public static float Normalize(float f, float a, float b, bool clamp = true)
		{
			float x = (f - a) / (b - a);
			if (clamp)
			{
				x = Mathf.Clamp(x, 0f, 1f);
			}
			return x;
		}

		// Token: 0x06002D41 RID: 11585 RVA: 0x000D0CE6 File Offset: 0x000CEEE6
		public static float Map(float f, float fromA, float fromB, float toA, float toB)
		{
			return Mathf.Lerp(toA, toB, Maf.Normalize(f, fromA, fromB, true));
		}

		// Token: 0x06002D42 RID: 11586 RVA: 0x000D0CFC File Offset: 0x000CEEFC
		public static int[] ToPalindrome(int[] array)
		{
			if (array.Length < 3)
			{
				return array;
			}
			int count = array.Length;
			int[] newArray = new int[count * 2 - 2];
			array.CopyTo(newArray, 0);
			for (int i = 0; i < newArray.Length - count; i++)
			{
				newArray[i + count] = array[count - i - 2];
			}
			return newArray;
		}

		// Token: 0x06002D43 RID: 11587 RVA: 0x000D0D45 File Offset: 0x000CEF45
		public static double Lerp(double a, double b, double norm)
		{
			return a * (1.0 - norm) + b * norm;
		}

		// Token: 0x06002D44 RID: 11588 RVA: 0x000D0D58 File Offset: 0x000CEF58
		public static float VolCurve(float f)
		{
			float a = 31.622776f;
			return (Mathf.Pow(a, f) - 1f) / (a - 1f);
		}

		// Token: 0x06002D45 RID: 11589 RVA: 0x000D0D80 File Offset: 0x000CEF80
		public static List<bool> Bjorklund(int hits, int steps, bool startOnTrue = false, bool reverse = false)
		{
			List<bool> pattern = new List<bool>();
			if (steps == 0)
			{
				return pattern;
			}
			if (hits == 0)
			{
				for (int i = 0; i < steps; i++)
				{
					pattern.Add(false);
				}
				return pattern;
			}
			if (hits >= steps)
			{
				for (int i = 0; i < steps; i++)
				{
					pattern.Add(true);
				}
				return pattern;
			}
			List<int> counts = new List<int>();
			List<int> remainders = new List<int>
			{
				hits
			};
			int divisor = steps - hits;
			int level = 0;
			do
			{
				counts.Add((int)Mathf.Floor((float)(divisor / remainders[level])));
				remainders.Add(divisor % remainders[level]);
				divisor = remainders[level];
				level++;
			}
			while (remainders[level] > 1);
			counts.Add(divisor);
			int r = 0;
			Action<int> build = null;
			build = delegate(int lvl)
			{
				int r = r;
				r++;
				if (lvl > -1)
				{
					for (int j = 0; j < counts[lvl]; j++)
					{
						build(lvl - 1);
					}
					if (remainders[lvl] != 0)
					{
						build(lvl - 2);
						return;
					}
				}
				else
				{
					if (lvl == -1)
					{
						pattern.Add(false);
						return;
					}
					if (lvl == -2)
					{
						pattern.Add(true);
					}
				}
			};
			build(level);
			if (startOnTrue)
			{
				while (!pattern[0])
				{
					bool end = pattern[pattern.Count - 1];
					pattern.RemoveAt(pattern.Count - 1);
					pattern.Insert(0, end);
				}
			}
			if (reverse)
			{
				pattern.Reverse();
			}
			return pattern;
		}

		// Token: 0x06002D46 RID: 11590 RVA: 0x000D0F08 File Offset: 0x000CF108
		public static bool IsWithin(float x, int min, int max)
		{
			return x >= (float)min && x <= (float)max;
		}
	}
}
