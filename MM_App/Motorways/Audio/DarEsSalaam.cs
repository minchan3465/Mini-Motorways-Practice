using System;
using System.Collections.Generic;

namespace Motorways.Audio
{
	// Token: 0x0200069D RID: 1693
	public class DarEsSalaam : MusicData
	{
		// Token: 0x06002EE4 RID: 12004 RVA: 0x000DA42C File Offset: 0x000D862C
		public override void Injections()
		{
			base.SetRhythms(Rhythm.Quintuplet.Patterns(-1).Edit((Rhythm x, int i) => x.InjectNoise(0.1f)), MusicData.RhythmUpdateType.RandomParallel);
			float d = Math.Max(this.Rhythms.Shortest().Duration, 2f);
			this.initDrumRhythm = new Rhythm(0f, new D20(-1).Frag(10, d, 0f, -1f, -1f));
			base.SetDrumSequencer(this.initDrumRhythm, false, false, true, true, -1f, -1f);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => MusicData.NoteSequenceType.PingPong));
			base.SetVoiceLimits(0.05, 1, 0.0, 5);
			base.SetQualities(Liszt.Flatten<Quality>(new List<Quality>[]
			{
				Liszt.From<Quality>(new Quality[]
				{
					QualityDatabase.MAJOR_TETRA.Chromodal(Array.Empty<string>())
				}),
				QualityDatabase.Gather(new string[]
				{
					"SUHMM Mixolydian",
					"SUHMM Lydian"
				})
			}), null, true);
		}

		// Token: 0x06002EE5 RID: 12005 RVA: 0x000DA568 File Offset: 0x000D8768
		public override void OnConnection()
		{
			if (Persistent.Connections < 5)
			{
				return;
			}
			DrumSequencer dS = Get.Loadout.DrumSequencer;
			switch (Rando.Pick<int>(new int[]
			{
				1,
				2,
				3
			}))
			{
			case 1:
				dS.Boom.Hits = Rando.Range(0, dS.Boom.Steps, -1);
				dS.Boom.Reroll();
				break;
			case 2:
				dS.Bap.Hits = Rando.Range(0, dS.Bap.Steps, -1);
				dS.Bap.Reroll();
				break;
			case 3:
				dS.Hat.Hits = Rando.Range(0, dS.Hat.Steps, -1);
				dS.Hat.Reroll();
				break;
			}
			base.UpdateDrumSequencer(this.DrumSequencerRhythm, (this.connCount % 6 == 1 || this.connCount % 6 == 5) ? (!this.Boom) : this.Boom, (this.connCount % 6 == 2 || this.connCount % 6 == 4) ? (!this.Bap) : this.Bap, true, false);
			this.connCount++;
		}

		// Token: 0x06002EE6 RID: 12006 RVA: 0x000DA69C File Offset: 0x000D889C
		public override void OnDay()
		{
			base.UpdateDrumSequencer(this.initDrumRhythm.InjectNoise(Get.WeekProgress * 0.1f), this.Boom, this.Bap, this.Hat, false);
		}

		// Token: 0x06002EE7 RID: 12007 RVA: 0x000DA6D0 File Offset: 0x000D88D0
		public override void OnHour()
		{
			if (Get.Clock.Hour < 6 || Get.AudibleGroups < 1)
			{
				return;
			}
			if (Get.Hour % this.bassFreq == 1)
			{
				this.bassFreq = Rando.Pick<int>(new int[]
				{
					3,
					6,
					9,
					12,
					15,
					18,
					21,
					24
				});
				string note = Note.SCALE[this.CurrentScale.Key];
				if (Get.Week % 2 == 0)
				{
					note = Rando.Pick<string>(base.NoteWindow);
					note = note.Substring(0, note.Length - 1);
				}
				AudioSample bass = this.Bass;
				if (bass != null)
				{
					bass.FadeOutAndStop(0.5);
				}
				this.Bass = AudioPlayer.Default.PlaySample("bass_" + note, 0.5f, 0.4f, Get.State.HasFlag(StateType.ModeNight) ? Rando.Pick<float>(new float[]
				{
					-0.5f,
					-1f
				}) : 1f, 0.5, -1.0, false, null, false, false, 0f, false);
			}
		}

		// Token: 0x04002896 RID: 10390
		private Rhythm initDrumRhythm;

		// Token: 0x04002897 RID: 10391
		private int connCount;

		// Token: 0x04002898 RID: 10392
		private int bassFreq = 6;
	}
}
