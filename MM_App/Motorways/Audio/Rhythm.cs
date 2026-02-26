using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006DE RID: 1758
	public class Rhythm
	{
		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x06003048 RID: 12360 RVA: 0x000E3166 File Offset: 0x000E1366
		// (set) Token: 0x06003049 RID: 12361 RVA: 0x000E316E File Offset: 0x000E136E
		public float Duration { get; private set; }

		// Token: 0x0600304A RID: 12362 RVA: 0x000E3178 File Offset: 0x000E1378
		public Rhythm(float offsetRatio, params float[] steps)
		{
			this.Steps = steps;
			this.Offset = offsetRatio;
			this.Id = "HyperPulse: o." + this.Offset.ToString() + ".s." + string.Join<float>(", ", this.Steps);
			this.Duration = steps.Sum();
		}

		// Token: 0x0600304B RID: 12363 RVA: 0x000E31D8 File Offset: 0x000E13D8
		public Rhythm InjectNoise(float noise = 0f)
		{
			float[] noisySteps = new D20(-1).Frag(this.Steps.Length, this.Duration, 1f, -1f, -1f);
			for (int i = 0; i < this.Steps.Length; i++)
			{
				this.Steps[i] = Mathf.Lerp(this.Steps[i], noisySteps[i], noise);
			}
			return this;
		}

		// Token: 0x0600304C RID: 12364 RVA: 0x000E323B File Offset: 0x000E143B
		public static List<float> FragRatios(int steps)
		{
			float[] array = new float[]
			{
				0f,
				1.25f,
				1.3333334f,
				1.5f,
				1.6666666f,
				1.75f,
				2f
			};
			array[0] = ((steps == 5) ? 2f : 1f);
			return Liszt.From<float>(array);
		}

		// Token: 0x0600304D RID: 12365 RVA: 0x000E3268 File Offset: 0x000E1468
		public static Rhythm Frag(float noise = 1f, int seed = -1)
		{
			D20 d20 = new D20(seed);
			int steps = d20.Pick<int>(new int[]
			{
				3,
				6
			});
			return new Rhythm(0f, d20.Frag(steps, (steps == 6) ? 2f : d20.Pick<float>(Rhythm.FragRatios(steps)), noise, -1f, -1f));
		}

		// Token: 0x0600304E RID: 12366 RVA: 0x000E32C4 File Offset: 0x000E14C4
		public static List<Rhythm> Frags(int seed = -1)
		{
			D20 d20 = new D20(seed);
			return Liszt.Make<Rhythm>(12, delegate(int i)
			{
				int steps = d20.Pick<int>(new int[]
				{
					3,
					6
				});
				return new Rhythm(0f, d20.Frag(steps, (steps == 6) ? 2f : d20.Pick<float>(Rhythm.FragRatios(steps)), 1f, -1f, -1f));
			});
		}

		// Token: 0x0600304F RID: 12367 RVA: 0x000E32F8 File Offset: 0x000E14F8
		public static Rhythm Sine(int steps, float duration, float freq, float strength = 0.5f, float offsetRatio = 0f)
		{
			strength = Mathf.Clamp01(strength);
			List<float> stps = new List<float>();
			for (int i = 0; i < steps; i++)
			{
				float alpha = Mathf.Sin((float)i / (float)(steps - 1) * freq * 2f * 3.1415927f) * 0.5f + 0.5f;
				float stepDur = Mathf.Approximately(strength, 0f) ? 1f : Mathf.Lerp(1f - strength, 1f + strength, alpha);
				stps.Add(stepDur);
			}
			return new Rhythm(offsetRatio, stps.ToArray()).ToDuration(duration);
		}

		// Token: 0x06003050 RID: 12368 RVA: 0x000E3389 File Offset: 0x000E1589
		public static List<Rhythm> AllPlets(int seed = -1)
		{
			return Rhythm.Duplet.All(-1).And(Rhythm.Triplet.All(-1)).And(Rhythm.Quintuplet.All(-1));
		}

		// Token: 0x06003051 RID: 12369 RVA: 0x000E33B6 File Offset: 0x000E15B6
		public static List<Rhythm> AllPulses(int seed = -1)
		{
			return Rhythm.Duplet.Pulses(-1).And(Rhythm.Triplet.Pulses(-1)).And(Rhythm.Quintuplet.Pulses(-1));
		}

		// Token: 0x06003052 RID: 12370 RVA: 0x000E33E3 File Offset: 0x000E15E3
		public static List<Rhythm> AllPatterns(int seed = -1)
		{
			return Rhythm.Duplet.Patterns(-1).And(Rhythm.Triplet.Patterns(-1)).And(Rhythm.Quintuplet.Patterns(-1));
		}

		// Token: 0x06003053 RID: 12371 RVA: 0x000E3410 File Offset: 0x000E1610
		public override string ToString()
		{
			return this.Id;
		}

		// Token: 0x040029AC RID: 10668
		public const int DEFAULT_SIZE = 12;

		// Token: 0x040029AD RID: 10669
		public float[] Steps;

		// Token: 0x040029AE RID: 10670
		public float Offset;

		// Token: 0x040029AF RID: 10671
		public string Id;

		// Token: 0x040029B1 RID: 10673
		public static List<Rhythm> Claves = Liszt.From<Rhythm>(new Rhythm[]
		{
			new Rhythm(0f, new float[]
			{
				0.5f,
				0.75f,
				0.5f,
				1f,
				0.5f,
				0.75f
			}),
			new Rhythm(0f, new float[]
			{
				0.75f,
				0.75f,
				1f,
				0.5f,
				1f
			}),
			new Rhythm(0f, new float[]
			{
				0.75f,
				0.25f,
				0.75f,
				0.25f,
				0.5f,
				0.5f,
				0.75f,
				0.25f
			}),
			new Rhythm(0f, new float[]
			{
				0.25f,
				0.5f,
				0.25f,
				0.5f,
				0.5f,
				0.5f,
				0.5f,
				0.5f,
				0.5f
			}),
			new Rhythm(0f, new float[]
			{
				0.75f,
				0.75f,
				0.75f,
				0.75f,
				1f
			}),
			new Rhythm(0f, new float[]
			{
				0.75f,
				0.75f,
				1f,
				0.75f,
				0.75f
			}),
			new Rhythm(0f, new float[]
			{
				0.25f,
				0.5f,
				0.5f,
				0.5f,
				0.25f,
				0.5f,
				0.5f,
				0.5f,
				0.5f
			}),
			new Rhythm(0.25f, new float[]
			{
				0.5f,
				0.5f,
				0.5f,
				0.25f,
				0.5f,
				0.5f,
				0.5f,
				0.75f
			}),
			new Rhythm(0.25f, new float[]
			{
				0.5f,
				0.75f,
				1f,
				0.75f,
				0.5f,
				0.5f
			}),
			new Rhythm(0f, new float[]
			{
				0.5f,
				0.75f,
				0.5f,
				0.5f,
				0.75f,
				0.5f,
				0.5f
			}),
			new Rhythm(0f, new float[]
			{
				0.25f,
				0.5f,
				0.25f,
				0.5f,
				0.5f,
				0.5f,
				0.5f,
				0.25f,
				0.5f,
				0.25f
			}),
			new Rhythm(0f, new float[]
			{
				0.5f,
				0.5f,
				0.5f,
				0.5f,
				0.25f,
				0.5f,
				0.25f,
				0.5f,
				0.5f
			}),
			new Rhythm(0f, new float[]
			{
				0.25f,
				0.5f,
				0.5f,
				0.25f,
				0.5f,
				0.5f,
				0.5f,
				0.5f,
				0.5f
			})
		});

		// Token: 0x040029B2 RID: 10674
		public static Rhythm.PletDef Duplet = new Rhythm.PletDef(Liszt.From<float>(new float[]
		{
			0f,
			0.25f,
			0.5f,
			0.75f
		}), Liszt.From<float>(new float[]
		{
			0.25f,
			0.5f,
			0.75f,
			1f,
			1.25f,
			1.5f,
			1.75f,
			2f
		}), Liszt.From<float>(new float[]
		{
			0.125f,
			0.375f
		}));

		// Token: 0x040029B3 RID: 10675
		public static Rhythm.PletDef Triplet = new Rhythm.PletDef(Liszt.From<float>(new float[]
		{
			0f,
			0.33333334f,
			0.6666667f
		}), Liszt.From<float>(new float[]
		{
			0.33333334f,
			0.6666667f,
			0.5f,
			1f,
			1.3333334f,
			1.6666666f,
			2f
		}), Liszt.From<float>(new float[]
		{
			0.16666667f
		}));

		// Token: 0x040029B4 RID: 10676
		public static Rhythm.PletDef Quintuplet = new Rhythm.PletDef(Liszt.From<float>(new float[]
		{
			0f,
			0.2f,
			0.4f,
			0.6f,
			0.6f
		}), Liszt.From<float>(new float[]
		{
			0.2f,
			0.4f,
			0.6f,
			0.8f,
			1f,
			1.2f,
			1.4f,
			1.6f,
			1.8f,
			2f
		}), null);

		// Token: 0x020006DF RID: 1759
		public class PletDef
		{
			// Token: 0x06003055 RID: 12373 RVA: 0x000E369C File Offset: 0x000E189C
			public PletDef(List<float> offsets, List<float> ratios, List<float> subRatios = null)
			{
				this.Offsets = offsets;
				this.Ratios = ratios;
				this.SubRatios = subRatios;
			}

			// Token: 0x06003056 RID: 12374 RVA: 0x000E36BC File Offset: 0x000E18BC
			public Rhythm Pulse(int seed = -1)
			{
				D20 d20 = new D20(seed);
				return new Rhythm(d20.Pick<float>(this.Offsets), new float[]
				{
					d20.Pick<float>(this.Ratios)
				});
			}

			// Token: 0x06003057 RID: 12375 RVA: 0x000E36F8 File Offset: 0x000E18F8
			public List<Rhythm> Pulses(int seed = -1)
			{
				D20 d20 = new D20(seed);
				return Liszt.Make<Rhythm>(12, (int r_i) => new Rhythm(d20.Pick<float>(this.Offsets), new float[]
				{
					d20.Pick<float>(this.Ratios)
				}));
			}

			// Token: 0x06003058 RID: 12376 RVA: 0x000E3731 File Offset: 0x000E1931
			public List<Rhythm> All(int seed = -1)
			{
				return this.Pulses(seed).And(this.Patterns(seed));
			}

			// Token: 0x06003059 RID: 12377 RVA: 0x000E3748 File Offset: 0x000E1948
			public Rhythm Pattern(int seed = -1)
			{
				D20 d20 = new D20(seed);
				List<float> ratios = (this.SubRatios == null) ? this.Ratios : this.Ratios.Concat(this.SubRatios).ToList<float>();
				return new Rhythm(d20.Pick<float>(this.Offsets), Liszt.Make<float>(d20.Range(3, 6), () => d20.Pick<float>(ratios)).ToArray());
			}

			// Token: 0x0600305A RID: 12378 RVA: 0x000E37CC File Offset: 0x000E19CC
			public List<Rhythm> Patterns(int seed = -1)
			{
				D20 d20 = new D20(seed);
				List<float> ratios = (this.SubRatios == null) ? this.Ratios : this.Ratios.Concat(this.SubRatios).ToList<float>();
				Func<float> <>9__1;
				return Liszt.Make<Rhythm>(12, delegate(int r_i)
				{
					float offsetRatio = d20.Pick<float>(this.Offsets);
					int size = d20.Range(3, 6);
					Func<float> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (() => d20.Pick<float>(ratios)));
					}
					return new Rhythm(offsetRatio, Liszt.Make<float>(size, func).ToArray());
				});
			}

			// Token: 0x040029B5 RID: 10677
			public List<float> Offsets;

			// Token: 0x040029B6 RID: 10678
			public List<float> Ratios;

			// Token: 0x040029B7 RID: 10679
			public List<float> SubRatios;
		}
	}
}
