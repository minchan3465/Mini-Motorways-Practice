using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000680 RID: 1664
	public static class Param
	{
		// Token: 0x06002E32 RID: 11826 RVA: 0x000D6380 File Offset: 0x000D4580
		public static Param.Group Gain(float gMin, float gMax = -1f)
		{
			return new Param.Group(new Param.Data(gMin, gMax), null);
		}

		// Token: 0x06002E33 RID: 11827 RVA: 0x000D638F File Offset: 0x000D458F
		public static Param.Group Gain(this Param.Group sp, float gMin, float gMax = -1f)
		{
			sp.Gain = new Param.Data(gMin, gMax);
			return sp;
		}

		// Token: 0x06002E34 RID: 11828 RVA: 0x000D639F File Offset: 0x000D459F
		public static Param.Group Pitch(float pMin, float pMax = -1f)
		{
			return new Param.Group(null, new Param.Data(pMin, pMax));
		}

		// Token: 0x06002E35 RID: 11829 RVA: 0x000D63AE File Offset: 0x000D45AE
		public static Param.Group Pitch(this Param.Group sp, float pMin, float pMax = -1f)
		{
			sp.Pitch = new Param.Data(pMin, pMax);
			return sp;
		}

		// Token: 0x02000681 RID: 1665
		public class Data
		{
			// Token: 0x06002E36 RID: 11830 RVA: 0x000D63BE File Offset: 0x000D45BE
			public Data(float value, float valueMax = -1f)
			{
				this.Value = value;
				this.Range = new Vector2(value, (valueMax < 0f) ? value : valueMax);
			}

			// Token: 0x06002E37 RID: 11831 RVA: 0x000D63E5 File Offset: 0x000D45E5
			public override string ToString()
			{
				return this.Range.ToString();
			}

			// Token: 0x0400282E RID: 10286
			public float Value;

			// Token: 0x0400282F RID: 10287
			public Vector2 Range;
		}

		// Token: 0x02000682 RID: 1666
		public class LFO
		{
			// Token: 0x06002E38 RID: 11832 RVA: 0x000D63F8 File Offset: 0x000D45F8
			public LFO(Param.Data freq, Param.Data amp)
			{
				this.Freq = freq;
				this.Amp = amp;
			}

			// Token: 0x04002830 RID: 10288
			public Param.Data Freq;

			// Token: 0x04002831 RID: 10289
			public Param.Data Amp;
		}

		// Token: 0x02000683 RID: 1667
		public class Vibrato : Param.LFO
		{
			// Token: 0x06002E39 RID: 11833 RVA: 0x000D640E File Offset: 0x000D460E
			public Vibrato(Param.Data freq, int strengthInCents) : base(freq, new Param.Data(0f, Tune.centsToFreqRatio(strengthInCents) - 1f))
			{
			}
		}

		// Token: 0x02000684 RID: 1668
		public class Portamento
		{
			// Token: 0x06002E3A RID: 11834 RVA: 0x000D642D File Offset: 0x000D462D
			public Portamento(int startingPitchDeltaMinCents = 0, int startingPitchDeltaMaxCents = 0, double timeMin = 0.0, double timeMax = 0.0)
			{
				this.StartingPitch = new Param.Data(Tune.centsToFreqRatio(startingPitchDeltaMinCents), Tune.centsToFreqRatio(startingPitchDeltaMaxCents));
				this.Time = new Param.Data((float)timeMin, (float)timeMax);
			}

			// Token: 0x04002832 RID: 10290
			public Param.Data StartingPitch;

			// Token: 0x04002833 RID: 10291
			public Param.Data Time;
		}

		// Token: 0x02000685 RID: 1669
		public class Group
		{
			// Token: 0x06002E3B RID: 11835 RVA: 0x000D645C File Offset: 0x000D465C
			public Group(Param.Data gain = null, Param.Data pitch = null)
			{
				this.Gain = (gain ?? new Param.Data(1f, -1f));
				this.Pitch = (pitch ?? new Param.Data(1f, -1f));
			}

			// Token: 0x06002E3C RID: 11836 RVA: 0x000D6498 File Offset: 0x000D4698
			public static Param.Group Make(float gMin, float gMax, float pMin, float pMax)
			{
				return new Param.Group(new Param.Data(gMin, gMax), new Param.Data(pMin, pMax));
			}

			// Token: 0x06002E3D RID: 11837 RVA: 0x000D64B0 File Offset: 0x000D46B0
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"Gain: [",
					this.Gain.ToString(),
					"], Pitch: [",
					this.Pitch.ToString(),
					"]"
				});
			}

			// Token: 0x04002834 RID: 10292
			public Param.Data Pitch;

			// Token: 0x04002835 RID: 10293
			public Param.Data Gain;
		}
	}
}
