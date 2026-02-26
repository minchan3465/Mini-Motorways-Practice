using System;
using System.Collections;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x0200065B RID: 1627
	public static class Twerp
	{
		// Token: 0x06002D4C RID: 11596 RVA: 0x000D0FE3 File Offset: 0x000CF1E3
		public static Coroutine StartCoroutine(IEnumerator routine)
		{
			return GATManager.UniqueInstance.StartCoroutine(routine);
		}

		// Token: 0x06002D4D RID: 11597 RVA: 0x000D0FF0 File Offset: 0x000CF1F0
		public static IEnumerator InterpolateFloatBoingInPlace(Action<float> val, float from, float duration, float freq, float amp, float phase = 0f, Action<bool> callback = null)
		{
			float elapsedTime = 0f;
			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float x = elapsedTime / duration;
				x = Twerp.BoingInPlace(x, freq, amp, phase);
				val(from + x * from);
				yield return new WaitForEndOfFrame();
			}
			if (callback != null)
			{
				callback(true);
			}
			yield break;
		}

		// Token: 0x06002D4E RID: 11598 RVA: 0x000D102C File Offset: 0x000CF22C
		public static IEnumerator InterpolateFloat(Action<float> val, float from, float to, float duration, int pow = 1, Twerp.CurveType curve = Twerp.CurveType.None, Action<bool> callback = null)
		{
			if (Mathf.Approximately(from, to))
			{
				yield break;
			}
			float elapsedTime = 0f;
			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float alpha = elapsedTime / duration;
				float x = alpha;
				if (curve == Twerp.CurveType.None)
				{
					if (pow > 1)
					{
						curve = Twerp.CurveType.EaseIn;
					}
					else if (pow < -1)
					{
						curve = Twerp.CurveType.EaseOut;
					}
				}
				pow = Mathf.Abs(pow);
				switch (curve)
				{
				case Twerp.CurveType.EaseIn:
					x = Twerp.Ease.In(alpha, pow);
					break;
				case Twerp.CurveType.EaseOut:
					x = Twerp.Ease.Out(alpha, pow);
					break;
				case Twerp.CurveType.EaseInOut:
					x = Twerp.Ease.InOut(alpha, pow);
					break;
				case Twerp.CurveType.Boing:
					x = Twerp.Boing(alpha);
					break;
				case Twerp.CurveType.Bounce:
					x = Twerp.Bounce.In(alpha);
					break;
				case Twerp.CurveType.ElasticIn:
					x = Twerp.Elastic.In(alpha);
					break;
				case Twerp.CurveType.ElasticOut:
					x = Twerp.Elastic.Out(alpha);
					break;
				case Twerp.CurveType.ElasticInOut:
					x = Twerp.Elastic.InOut(alpha);
					break;
				case Twerp.CurveType.Volume:
					x = ((from < to) ? Maf.VolCurve(alpha) : (1f - Maf.VolCurve(1f - alpha)));
					break;
				}
				val(Mathf.Lerp(from, to, x));
				yield return new WaitForEndOfFrame();
			}
			if (callback != null)
			{
				callback(true);
			}
			yield break;
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x000D1068 File Offset: 0x000CF268
		public static float Boing(float x)
		{
			x = Mathf.Clamp01(x);
			x = (Mathf.Sin(x * 3.1415927f * (0.2f + 2.5f * x * x * x)) * Mathf.Pow(1f - x, 2.2f) + x) * (1f + 1.2f * (1f - x));
			return x;
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x000D10C6 File Offset: 0x000CF2C6
		public static float BoingInPlace(float x, float freq, float amp, float phase = 0f)
		{
			return (1f - x) * amp * Mathf.Sin(x * (freq * (1f - x)) * 2f * 3.1415927f + Mathf.Lerp(0f, 6.2831855f, phase));
		}

		// Token: 0x0200065C RID: 1628
		public enum CurveType
		{
			// Token: 0x0400276C RID: 10092
			None,
			// Token: 0x0400276D RID: 10093
			EaseIn,
			// Token: 0x0400276E RID: 10094
			EaseOut,
			// Token: 0x0400276F RID: 10095
			EaseInOut,
			// Token: 0x04002770 RID: 10096
			Boing,
			// Token: 0x04002771 RID: 10097
			Bounce,
			// Token: 0x04002772 RID: 10098
			ElasticIn,
			// Token: 0x04002773 RID: 10099
			ElasticOut,
			// Token: 0x04002774 RID: 10100
			ElasticInOut,
			// Token: 0x04002775 RID: 10101
			Volume
		}

		// Token: 0x0200065D RID: 1629
		public static class Ease
		{
			// Token: 0x06002D51 RID: 11601 RVA: 0x000D10FF File Offset: 0x000CF2FF
			public static float In(float x, int pow)
			{
				while (pow > 1)
				{
					x *= x;
					pow--;
				}
				return x;
			}

			// Token: 0x06002D52 RID: 11602 RVA: 0x000D1112 File Offset: 0x000CF312
			public static float Out(float x, int pow)
			{
				return 1f - Twerp.Ease.In(1f - x, pow);
			}

			// Token: 0x06002D53 RID: 11603 RVA: 0x000D1127 File Offset: 0x000CF327
			public static float InOut(float x, int pow)
			{
				if ((x *= 2f) < 1f)
				{
					return 0.5f * Twerp.Ease.In(x, pow);
				}
				return 0.5f * Twerp.Ease.Out(x, pow);
			}
		}

		// Token: 0x0200065E RID: 1630
		public static class Elastic
		{
			// Token: 0x06002D54 RID: 11604 RVA: 0x000D1158 File Offset: 0x000CF358
			public static float In(float x)
			{
				if (Mathf.Approximately(x, 0f))
				{
					return 0f;
				}
				if (Mathf.Approximately(x, 1f))
				{
					return 1f;
				}
				return -Mathf.Pow(2f, 10f * (x - 1f)) * Mathf.Sin((x - 1.1f) * 5f * 3.1415927f);
			}

			// Token: 0x06002D55 RID: 11605 RVA: 0x000D11BC File Offset: 0x000CF3BC
			public static float Out(float x)
			{
				if (Mathf.Approximately(x, 0f))
				{
					return 0f;
				}
				if (Mathf.Approximately(x, 1f))
				{
					return 1f;
				}
				return Mathf.Pow(2f, -10f * x) * Mathf.Sin((x - 0.1f) * 5f * 3.1415927f) + 1f;
			}

			// Token: 0x06002D56 RID: 11606 RVA: 0x000D1220 File Offset: 0x000CF420
			public static float InOut(float x)
			{
				if (Mathf.Approximately(x, 0f))
				{
					return 0f;
				}
				if (Mathf.Approximately(x, 1f))
				{
					return 1f;
				}
				x *= 2f;
				if (x < 1f)
				{
					return -0.5f * Mathf.Pow(2f, 10f * (x - 1f)) * Mathf.Sin((x - 1.1f) * 5f * 3.1415927f);
				}
				return 0.5f * Mathf.Pow(2f, -10f * (x - 1f)) * Mathf.Sin((x - 1.1f) * 5f * 3.1415927f) + 1f;
			}
		}

		// Token: 0x0200065F RID: 1631
		public static class Bounce
		{
			// Token: 0x06002D57 RID: 11607 RVA: 0x000D12D7 File Offset: 0x000CF4D7
			public static float In(float x)
			{
				return 1f - Twerp.Bounce.Out(1f - x);
			}

			// Token: 0x06002D58 RID: 11608 RVA: 0x000D12EB File Offset: 0x000CF4EB
			public static float In2(float x)
			{
				return Mathf.Abs(x - Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x)));
			}

			// Token: 0x06002D59 RID: 11609 RVA: 0x000D131C File Offset: 0x000CF51C
			public static float Out(float x)
			{
				if (x < 0.36363637f)
				{
					return 121f * x * x / 16f;
				}
				if (x < 0.72727275f)
				{
					return 9.075f * x * x - 9.9f * x + 3.4f;
				}
				if (x < 0.9f)
				{
					return 12.066482f * x * x - 19.635458f * x + 8.898061f;
				}
				return 10.8f * x * x - 20.52f * x + 10.72f;
			}

			// Token: 0x06002D5A RID: 11610 RVA: 0x000D1398 File Offset: 0x000CF598
			public static float InOut(float x)
			{
				if (x < 0.5f)
				{
					return Twerp.Bounce.In(x * 2f) * 0.5f;
				}
				return Twerp.Bounce.Out(x * 2f - 1f) * 0.5f + 0.5f;
			}
		}
	}
}
