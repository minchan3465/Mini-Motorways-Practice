using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006BE RID: 1726
	public class Wellington : MusicData
	{
		// Token: 0x06002F90 RID: 12176 RVA: 0x000DDC1C File Offset: 0x000DBE1C
		private Rhythm GenDrumRhythm()
		{
			return new Rhythm(0f, Liszt.Make<float>(Rando.Range(4, 8, -1), (int x) => Rando.Pick<float>(new float[]
			{
				0.25f,
				0.5f,
				0.75f
			})).ToArray()).Crop(2f);
		}

		// Token: 0x06002F91 RID: 12177 RVA: 0x000DDC70 File Offset: 0x000DBE70
		public override void Injections()
		{
			this.drumVolume = this.DrumVolume;
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				new Quality("Wellington 1", Liszt.From<int>(new int[]
				{
					2,
					7,
					3
				}), Liszt.From<int>(new int[]
				{
					0,
					24
				})),
				new Quality("Wellington 2", Liszt.From<int>(new int[]
				{
					2,
					2,
					7,
					1
				}), Liszt.From<int>(new int[]
				{
					0,
					24
				})),
				new Quality("Wellington 3", Liszt.From<int>(new int[]
				{
					3,
					2,
					7
				}), Liszt.From<int>(new int[]
				{
					0,
					24
				})),
				new Quality("Wellington 4", Liszt.From<int>(new int[]
				{
					2,
					3,
					2,
					5
				}), Liszt.From<int>(new int[]
				{
					0,
					24
				})),
				new Quality("Wellington 5", Liszt.From<int>(new int[]
				{
					2,
					5,
					2,
					3
				}), Liszt.From<int>(new int[]
				{
					0,
					24
				})),
				new Quality("Wellington 6", Liszt.From<int>(new int[]
				{
					5,
					3,
					2,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					24
				})),
				new Quality("Wellington 7", Liszt.From<int>(new int[]
				{
					7,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					24
				}))
			}).Chromatic(""), null, true);
			base.SetKeyDeltas(Liszt.From<int>(new int[1]), MusicData.MenuKey);
			base.SetWeekendChances(0f, 0f);
			this.BaseRhythm = Rhythm.Duplet.Pulse(-1);
			this.BaseRhythms = this.BaseRhythm.Uniform(12);
			base.SetRhythms(this.BaseRhythms, MusicData.RhythmUpdateType.LinearUniform);
			base.SetDrumSequencer(this.GenDrumRhythm(), true, true, true, true, -1f, -1f);
			this.DrumVolume = 0f;
			base.SetPortamento(new Param.Portamento(-300, 0, 0.0, 0.5), null);
		}

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x06002F92 RID: 12178 RVA: 0x000DDEAB File Offset: 0x000DC0AB
		private DrumSequencer DS
		{
			get
			{
				return Get.Loadout.DrumSequencer;
			}
		}

		// Token: 0x06002F93 RID: 12179 RVA: 0x000DDEB8 File Offset: 0x000DC0B8
		public override void OnDawn()
		{
			this.DS.Hat.Hits = Rando.Range((int)((double)this.DS.Hat.Steps * 0.75), this.DS.Hat.Steps, -1);
			this.DS.Bap.Hits = Rando.Range((int)((double)this.DS.Bap.Steps * 0.75), this.DS.Bap.Steps, -1);
			this.DS.Boom.Hits = Rando.Range(0, this.DS.Boom.Steps, -1);
		}

		// Token: 0x06002F94 RID: 12180 RVA: 0x000DDF70 File Offset: 0x000DC170
		public override void OnDusk()
		{
			this.DS.Hat.Hits = Rando.Range(0, this.DS.Hat.Steps / 2, -1);
			this.DS.Bap.Hits = Rando.Range(0, this.DS.Bap.Steps / 2, -1);
			this.DS.Boom.Hits = Rando.Range(this.DS.Boom.Hits / 2, this.DS.Boom.Hits, -1);
			this.DS.Parts.ForEach(delegate(DrumSequencer.Part x)
			{
				x.Reroll();
			});
		}

		// Token: 0x06002F95 RID: 12181 RVA: 0x000DE038 File Offset: 0x000DC238
		public override void OnHour()
		{
			if (Get.Week == 0 && Get.Day == 6 && Get.Hour > 14 && Get.Hour < 24)
			{
				this.DrumVolume = this.drumVolume * Mathf.Pow(((float)Get.Hour - 14f) / 9f, 1.5f);
			}
		}

		// Token: 0x06002F96 RID: 12182 RVA: 0x000DE08F File Offset: 0x000DC28F
		public override void OnNewWeek()
		{
			base.OnNewWeek();
			if (Get.Week % 2 == 1)
			{
				base.UpdateDrumSequencer(this.GenDrumRhythm(), true, true, true, false);
			}
		}

		// Token: 0x06002F97 RID: 12183 RVA: 0x000DE0B4 File Offset: 0x000DC2B4
		public override void OnConnection()
		{
			this.spreadDelta += this.spreadDeltaInc;
			List<Rhythm> newRhythms = this.BaseRhythms.ToList<Rhythm>().Spread(this.spreadDelta);
			for (int i = 0; i < Get.Loadout.DestinationGroups.Count; i++)
			{
				Get.Loadout.DestinationGroups[i].Module.ChangePulse(newRhythms[i]);
			}
		}

		// Token: 0x06002F98 RID: 12184 RVA: 0x000DE126 File Offset: 0x000DC326
		public override float SamplePitchSign()
		{
			if (!Get.State.HasFlag(StateType.ModeNight))
			{
				return base.SamplePitchSign();
			}
			return -1f;
		}

		// Token: 0x0400290B RID: 10507
		private Rhythm BaseRhythm;

		// Token: 0x0400290C RID: 10508
		private List<Rhythm> BaseRhythms;

		// Token: 0x0400290D RID: 10509
		private float spreadDeltaInc = 0.05f;

		// Token: 0x0400290E RID: 10510
		private float spreadDelta;

		// Token: 0x0400290F RID: 10511
		private float drumVolume;
	}
}
