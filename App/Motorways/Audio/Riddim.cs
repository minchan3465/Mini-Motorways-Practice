using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006E4 RID: 1764
	public static class Riddim
	{
		// Token: 0x06003064 RID: 12388 RVA: 0x000E395C File Offset: 0x000E1B5C
		public static List<Rhythm> And(this List<Rhythm> list, List<Rhythm> rhythms)
		{
			return list.Concat(rhythms).ToList<Rhythm>();
		}

		// Token: 0x06003065 RID: 12389 RVA: 0x000E396C File Offset: 0x000E1B6C
		public static Rhythm Crop(this Rhythm r, float duration)
		{
			float lengthToCrop = r.Duration - duration;
			if (Mathf.Approximately(lengthToCrop, 0f))
			{
				return r;
			}
			List<float> steps = r.Steps.ToList<float>();
			if (lengthToCrop <= 0f)
			{
				List<float> list = steps;
				int index = steps.Count - 1;
				list[index] += Mathf.Abs(lengthToCrop);
			}
			else
			{
				while (lengthToCrop > 0f)
				{
					float removing = Mathf.Min(lengthToCrop, steps[steps.Count - 1]);
					List<float> list = steps;
					int index = steps.Count - 1;
					list[index] -= removing;
					if (Mathf.Approximately(steps[steps.Count - 1], 0f))
					{
						steps.RemoveAt(steps.Count - 1);
					}
					lengthToCrop -= removing;
				}
			}
			r = new Rhythm(r.Offset, steps.ToArray());
			return r;
		}

		// Token: 0x06003066 RID: 12390 RVA: 0x000E3A48 File Offset: 0x000E1C48
		public static List<Rhythm> Crop(this List<Rhythm> list, float duration)
		{
			return list.Edit((Rhythm x) => x.Crop(duration));
		}

		// Token: 0x06003067 RID: 12391 RVA: 0x000E3A74 File Offset: 0x000E1C74
		public static Rhythm ToDuration(this Rhythm r, float duration)
		{
			if (Mathf.Approximately(r.Duration, duration))
			{
				return r;
			}
			List<float> steps = r.Steps.ToList<float>();
			float stretchFactor = duration / r.Duration;
			steps.Edit((float x) => x *= stretchFactor);
			return new Rhythm(r.Offset, steps.ToArray());
		}

		// Token: 0x06003068 RID: 12392 RVA: 0x000E3AD8 File Offset: 0x000E1CD8
		public static List<Rhythm> ToDuration(this List<Rhythm> list, float duration)
		{
			return list.Edit((Rhythm x) => x.ToDuration(duration));
		}

		// Token: 0x06003069 RID: 12393 RVA: 0x000E3B04 File Offset: 0x000E1D04
		public static Rhythm Scale(this Rhythm r, float factor, bool scaleOffset = false)
		{
			if (Mathf.Approximately(factor, 1f))
			{
				return r;
			}
			List<float> steps = r.Steps.ToList<float>();
			steps.Edit((float x) => x *= factor);
			return new Rhythm(scaleOffset ? (r.Offset * factor) : r.Offset, steps.ToArray());
		}

		// Token: 0x0600306A RID: 12394 RVA: 0x000E3B74 File Offset: 0x000E1D74
		public static List<Rhythm> Scale(this List<Rhythm> list, float factor)
		{
			return list.Edit((Rhythm x) => x.Scale(factor, false));
		}

		// Token: 0x0600306B RID: 12395 RVA: 0x000E3BA0 File Offset: 0x000E1DA0
		public static Rhythm Backwards(this Rhythm r)
		{
			List<float> steps = r.Steps.ToList<float>();
			steps.Reverse();
			return new Rhythm(1f - r.Offset, steps.ToArray());
		}

		// Token: 0x0600306C RID: 12396 RVA: 0x000E3BD6 File Offset: 0x000E1DD6
		public static List<Rhythm> Backwards(this List<Rhythm> list)
		{
			return list.Edit((Rhythm x) => x.Backwards());
		}

		// Token: 0x0600306D RID: 12397 RVA: 0x000E3BFD File Offset: 0x000E1DFD
		public static Rhythm Palindrome(this Rhythm r)
		{
			return new Rhythm(r.Offset, r.Steps.ToList<float>().Palindrome<float>().ToArray());
		}

		// Token: 0x0600306E RID: 12398 RVA: 0x000E3C1F File Offset: 0x000E1E1F
		public static List<Rhythm> Palindrome(this List<Rhythm> list)
		{
			return list.Edit((Rhythm x) => x.Palindrome());
		}

		// Token: 0x0600306F RID: 12399 RVA: 0x000E3C48 File Offset: 0x000E1E48
		public static List<Rhythm> Uniform(this Rhythm r, int size = 12)
		{
			return Liszt.Make<Rhythm>(size, () => r);
		}

		// Token: 0x06003070 RID: 12400 RVA: 0x000E3C74 File Offset: 0x000E1E74
		public static List<Rhythm> Scatter(this List<Rhythm> list, int seed = -1)
		{
			D20 d20 = new D20(seed);
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = new Rhythm(d20.Roll(), list[i].Steps);
			}
			return list;
		}

		// Token: 0x06003071 RID: 12401 RVA: 0x000E3CB8 File Offset: 0x000E1EB8
		public static List<Rhythm> Spread(this List<Rhythm> list, float delta = 0.0625f)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = new Rhythm(Maf.FloorMod((float)i * delta, 1f), list[i].Steps);
			}
			return list;
		}

		// Token: 0x06003072 RID: 12402 RVA: 0x000E3D00 File Offset: 0x000E1F00
		public static List<Rhythm> Phase(this List<Rhythm> list, float phase = 0.0625f)
		{
			for (int i = 0; i < list.Count; i++)
			{
				float[] steps = list[i].Steps.ToArray<float>();
				for (int s = 0; s < steps.Length; s++)
				{
					steps[s] += (float)i * phase;
				}
				list[i] = new Rhythm(list[i].Offset, steps);
			}
			return list;
		}

		// Token: 0x06003073 RID: 12403 RVA: 0x000E3D67 File Offset: 0x000E1F67
		public static Rhythm Steppiest(this List<Rhythm> rhythms)
		{
			return rhythms.Aggregate(delegate(Rhythm max, Rhythm r)
			{
				if (max != null)
				{
					int? num;
					if (r == null)
					{
						num = null;
					}
					else
					{
						float[] steps = r.Steps;
						num = ((steps != null) ? new int?(steps.Count<float>()) : null);
					}
					int? num2 = num;
					int? num3;
					if (max == null)
					{
						num3 = null;
					}
					else
					{
						float[] steps2 = max.Steps;
						num3 = ((steps2 != null) ? new int?(steps2.Count<float>()) : null);
					}
					int? num4 = num3;
					if (!(num2.GetValueOrDefault() > num4.GetValueOrDefault() & (num2 != null & num4 != null)))
					{
						return max;
					}
				}
				return r;
			});
		}

		// Token: 0x06003074 RID: 12404 RVA: 0x000E3D8E File Offset: 0x000E1F8E
		public static Rhythm Shortest(this List<Rhythm> rhythms)
		{
			return rhythms.Aggregate(delegate(Rhythm min, Rhythm r)
			{
				if (min != null && r.Duration >= min.Duration)
				{
					return min;
				}
				return r;
			});
		}
	}
}
