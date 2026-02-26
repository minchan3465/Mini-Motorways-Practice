using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006BB RID: 1723
	public class Vancouver : MusicData
	{
		// Token: 0x06002F82 RID: 12162 RVA: 0x000DD53C File Offset: 0x000DB73C
		public override void Injections()
		{
			base.SetQualities(Liszt.From<Quality>(new Quality[]
			{
				new Quality("Maj9", Liszt.From<int>(new int[]
				{
					2,
					2,
					3,
					4,
					1
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					24
				})).Chromatic(""),
				new Quality("Minor Hex", Liszt.From<int>(new int[]
				{
					2,
					1,
					2,
					2,
					3,
					2
				}), Liszt.From<int>(new int[]
				{
					0,
					12,
					24
				})).Chromatic("")
			}), null, false);
			base.SetVibrato(new Param.Vibrato(new Param.Data(10f, 20f), 12), new Param.Vibrato(new Param.Data(10f, 20f), 12));
			base.SetTremolo(new Param.LFO(new Param.Data(10f, 20f), new Param.Data(0f, 0.5f)), new Param.LFO(new Param.Data(10f, 20f), new Param.Data(0f, 0.5f)));
			this.firstRhythm = Rhythm.Frag(0.5f, -1);
			this.firstRhythm.Offset = Rando.m(-1);
			base.SetDrumSequencer(this.firstRhythm, false, false, false, true, 0f, 10f);
			base.SetRhythms(Liszt.From<Rhythm>(new Rhythm[]
			{
				this.firstRhythm
			}), MusicData.RhythmUpdateType.RandomParallel);
			base.SetVoiceLimits(0.25, 6, 0.25, 2);
			this.chord = new Persistent.Chord();
			this.commonTones = Get.MaxGroups - 1;
		}

		// Token: 0x06002F83 RID: 12163 RVA: 0x000DD6E8 File Offset: 0x000DB8E8
		public override void OnNewWeek()
		{
			if (Get.Week > 3)
			{
				this.commonTones = Get.MaxGroups - Rando.Range(1, 5, -1);
			}
			else if (Get.Week > 0)
			{
				this.commonTones = Get.MaxGroups - (Get.Week + 1);
			}
			if (Get.Week == 1)
			{
				this.Boom = true;
				this.Bap = true;
				this.Hat = true;
			}
			base.UpdateNoteWindow(this.commonTones, 1f, 0, 1f, true);
		}

		// Token: 0x06002F84 RID: 12164 RVA: 0x000DD763 File Offset: 0x000DB963
		private void UpdateDrums(bool boom, bool bap, bool hat)
		{
			if (Get.Week > 0 && Get.Day % 2 == 0)
			{
				base.UpdateDrumSequencer(this.firstRhythm, boom, bap, hat, false);
			}
		}

		// Token: 0x06002F85 RID: 12165 RVA: 0x000DD788 File Offset: 0x000DB988
		public override void OnHour()
		{
			if (Get.Hour == 23)
			{
				base.UpdateNoteWindow(this.commonTones, 1f, 0, 0.5f, true);
				Persistent.Chord chord = this.chord;
				double nextPulseTime = Clock.NextPulseTime;
				chord.Play(Mathf.Max(1, Get.AudibleGroups), this.firstRhythm.Steps[0] * 0.5f, false, 0, 0.5f, nextPulseTime);
			}
			if (Get.Hour == 5)
			{
				if (Get.Week % 4 == 1 || Get.Week % 4 == 3)
				{
					this.UpdateDrums(this.Boom, this.Bap, !this.Hat);
				}
				if (Get.Week > 2 && Rando.FlipCoin(0.5f))
				{
					this.chord.PlaySingleRandom(0, 0.5f, 0.33f);
				}
			}
			if (Get.Hour == 17)
			{
				if (Get.Week > 3)
				{
					this.chord.PlaySingleRandom(12, 0f, 0.33f);
				}
				if (Get.Week == 2 || Get.Week == 3)
				{
					this.UpdateDrums(this.Boom, !this.Bap, this.Hat);
					return;
				}
				if (Get.Week > 1 && Get.Day % 2 == 0)
				{
					this.firstRhythm = this.firstRhythm.InjectNoise(0.05f);
					this.firstRhythm = new Rhythm(this.firstRhythm.Offset, this.firstRhythm.Steps);
					this.UpdateDrums(Rando.FlipCoin(0.5f), Rando.FlipCoin(0.5f), Rando.FlipCoin(0.5f));
					foreach (DestinationGroup destinationGroup in Get.Loadout.DestinationGroups)
					{
						destinationGroup.Module.ChangePulse(this.firstRhythm);
					}
				}
			}
		}

		// Token: 0x06002F86 RID: 12166 RVA: 0x000022F5 File Offset: 0x000004F5
		public override void OnRhythmUpdate(int groupIndex)
		{
		}

		// Token: 0x06002F87 RID: 12167 RVA: 0x000DD96C File Offset: 0x000DBB6C
		public override void OnDay()
		{
			if (Get.Week % 4 == 0 || Get.Week % 4 == 3)
			{
				this.UpdateDrums(!this.Boom, this.Bap, this.Hat);
			}
			if (Get.Week > 2)
			{
				string note = Note.SCALE[this.CurrentScale.Key];
				AudioSample bass = this.Bass;
				if (bass != null)
				{
					bass.FadeOutAndStop(0.5);
				}
				AudioPlayer @default = AudioPlayer.Default;
				string sampleName = "bass_" + note;
				float pan = 0.5f;
				float gain = 0.2f;
				double nextPulseTime = Clock.NextPulseTime;
				this.Bass = @default.PlaySample(sampleName, pan, gain, Get.State.HasFlag(StateType.ModeNight) ? Rando.Pick<float>(new float[]
				{
					-0.5f,
					-1f
				}) : 1f, 0.5, nextPulseTime, false, null, false, false, 0f, false);
			}
		}

		// Token: 0x04002904 RID: 10500
		private int commonTones = 5;

		// Token: 0x04002905 RID: 10501
		private Rhythm firstRhythm;

		// Token: 0x04002906 RID: 10502
		private Persistent.Chord chord;
	}
}
