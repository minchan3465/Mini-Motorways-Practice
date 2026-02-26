using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000697 RID: 1687
	public class Cairns : MusicData
	{
		// Token: 0x06002ECD RID: 11981 RVA: 0x000D9864 File Offset: 0x000D7A64
		public override void Injections()
		{
			base.SetEchoDuratios(Liszt.From<float>(new float[]
			{
				0.75f
			}));
			base.SetKeyDeltas(Liszt.From<int>(new int[]
			{
				5,
				7
			}), 20);
			base.SetPortamento(new Param.Portamento(-75, 75, 0.0, 0.5), null);
			base.SetFadeInProgression(1.0, 0.0, false);
			base.SetVoiceLimits(0.05, 3, 0.0, 5);
			base.SetVibrato(this.Vibrato, new Param.Vibrato(new Param.Data(10f, 20f), 20));
			base.SetWeekendChances(0f, 0f);
			this.rhythms = Liszt.From<Rhythm>(new Rhythm[]
			{
				new Rhythm(0f, new float[]
				{
					0.75f,
					0.75f,
					0.5f
				}),
				new Rhythm(0f, new float[]
				{
					0.5f,
					1f,
					0.5f,
					1f,
					0.5f,
					0.5f
				}),
				new Rhythm(0f, new float[]
				{
					1f,
					1f,
					1.5f,
					0.5f
				})
			});
			for (int i = 0; i < 3; i++)
			{
				this.rhythms.Add(this.rhythms[i].Scale(2f, true));
				this.rhythms.Add(this.rhythms[i].Scale(3f, true));
			}
			base.SetRhythms(this.rhythms, MusicData.RhythmUpdateType.RandomParallel);
			base.SetNoteSequenceStyles(Liszt.Make<MusicData.NoteSequenceType>(6, () => MusicData.NoteSequenceType.Seeded));
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				new Quality("Mixolydian", Liszt.From<int>(new int[]
				{
					2,
					2,
					1,
					2,
					2,
					1,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					17
				})).Chromatic("")
			}), null, true);
			base.SetDrumSequencer(this.eighthNote, false, false, false, true, 0f, 0f);
			this.chord = new Persistent.Chord();
		}

		// Token: 0x06002ECE RID: 11982 RVA: 0x000D9A99 File Offset: 0x000D7C99
		public override void OnNewWeek()
		{
			if (Get.Week == 1)
			{
				base.UpdateDrumSequencer(this.eighthNote, true, false, true, false);
			}
		}

		// Token: 0x06002ECF RID: 11983 RVA: 0x000D9AB4 File Offset: 0x000D7CB4
		public override void OnDay()
		{
			if (Get.Day == 5)
			{
				base.UpdateNoteWindow(Get.MaxGroups - 1, 1f, 0, 0.5f, true);
				Persistent.Chord chord = this.chord;
				double nextPulseTime = Clock.NextPulseTime;
				chord.Play(Mathf.Max(1, Get.AudibleGroups), this.eighthNote.Steps[0] * 0.5f, false, 0, 0.5f, nextPulseTime);
			}
		}

		// Token: 0x0400288A RID: 10378
		private List<Rhythm> rhythms;

		// Token: 0x0400288B RID: 10379
		private Persistent.Chord chord;

		// Token: 0x0400288C RID: 10380
		private Rhythm eighthNote = new Rhythm(0f, new float[]
		{
			0.5f
		});
	}
}
