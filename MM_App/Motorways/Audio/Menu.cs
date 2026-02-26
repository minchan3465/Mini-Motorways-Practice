using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006A6 RID: 1702
	public class Menu : MusicData
	{
		// Token: 0x06002F09 RID: 12041 RVA: 0x000DB078 File Offset: 0x000D9278
		public override void Injections()
		{
			base.SetEchoDuratios(Liszt.From<float>(new float[]
			{
				1f
			}));
			int steps = Rando.Range(3, 6, -1);
			base.SetRhythms(Rando.Pick<List<Rhythm>>(new List<Rhythm>[]
			{
				Rhythm.Sine(steps, Rando.Pick<float>(new float[]
				{
					1f,
					1.25f,
					1.3333334f,
					1.5f,
					1.6666666f,
					1.75f,
					2f
				}), Rando.Pick<float>(new float[]
				{
					1f,
					2f
				}), 0.5f, 0f).Uniform(12).Scatter(-1),
				Rhythm.Frag(1f, -1).Uniform(steps).Spread(1f / (float)steps)
			}), MusicData.RhythmUpdateType.RandomParallel);
			base.SetQualities(QualityDatabase.Gather(new string[]
			{
				"Quartal",
				"Maj7",
				"Maj6",
				"7",
				"5sus2",
				"Sus",
				"Major Lower Tetra",
				"Sus2Maj7",
				"Major Pentatonic"
			}), QualityDatabase.Gather(new string[]
			{
				"Quartal",
				"Min7",
				"MinMaj6",
				"7",
				"5sus2",
				"Sus",
				"Minor Lower Tetra",
				"Sus2Min7",
				"Minor Pentatonic",
				"Dominant Penta 4"
			}), true);
			this.dur = this.Rhythms[0].Duration / (float)Rando.Pick<int>(new int[]
			{
				2,
				3
			});
			this.nbSteps = ((this.dur < 0.5f) ? Rando.Pick<int>(new int[]
			{
				2,
				3
			}) : Rando.Pick<int>(new int[]
			{
				3,
				4
			}));
			base.SetDrumSequencer(this.GenDrumRhythm(), true, true, true, true, 3f, -1f);
			this.DrumVolume = 0.5f;
			base.SetPortamento(new Param.Portamento(-75, 75, 0.0, 0.5), null);
			MusicData.MenuKey = this.StartingKey;
		}

		// Token: 0x06002F0A RID: 12042 RVA: 0x000DB29C File Offset: 0x000D949C
		private Rhythm GenDrumRhythm()
		{
			return new Rhythm(0f, new D20(-1).Frag(this.nbSteps, this.dur, 0.15f, -1f, -1f));
		}

		// Token: 0x06002F0B RID: 12043 RVA: 0x000DB2D0 File Offset: 0x000D94D0
		public override void OnHour()
		{
			DrumSequencer dS = Get.Loadout.DrumSequencer;
			if (Get.Hour % 6 == 0)
			{
				DrumSequencer.Part p = Rando.Pick<DrumSequencer.Part>(dS.Parts);
				p.Hits = Rando.Range(0, p.Steps, -1);
				p.Reroll();
			}
			if ((float)dS.Parts.Sum((DrumSequencer.Part x) => x.Hits) / (float)dS.Parts.Sum((DrumSequencer.Part x) => x.Steps) > 0.8f)
			{
				dS.Parts.ForEach(delegate(DrumSequencer.Part x)
				{
					x.Hits = Mathf.Max(0, x.Hits - 1);
				});
			}
		}

		// Token: 0x040028A3 RID: 10403
		private int nbSteps;

		// Token: 0x040028A4 RID: 10404
		private float dur;
	}
}
