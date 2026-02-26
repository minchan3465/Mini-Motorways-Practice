using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006CF RID: 1743
	public class DrumSequencer : Playback
	{
		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x06002FED RID: 12269 RVA: 0x000E0DED File Offset: 0x000DEFED
		// (set) Token: 0x06002FEE RID: 12270 RVA: 0x000E0DF5 File Offset: 0x000DEFF5
		public bool PauseMode
		{
			get
			{
				return this.pauseMode;
			}
			set
			{
				if (value == this.pauseMode)
				{
					return;
				}
				this.pauseMode = value;
			}
		}

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x06002FEF RID: 12271 RVA: 0x000E0E08 File Offset: 0x000DF008
		// (set) Token: 0x06002FF0 RID: 12272 RVA: 0x000E0E10 File Offset: 0x000DF010
		public bool Play
		{
			get
			{
				return this.play;
			}
			set
			{
				if (value == this.play)
				{
					return;
				}
				this.play = value;
				this._pulseCount = 0;
				if (this.play)
				{
					this.Init();
				}
			}
		}

		// Token: 0x06002FF1 RID: 12273 RVA: 0x000E0E38 File Offset: 0x000DF038
		public override void OnDeactivate()
		{
			this.Play = false;
		}

		// Token: 0x06002FF2 RID: 12274 RVA: 0x000E0E41 File Offset: 0x000DF041
		public override void OnBeginPulse()
		{
			this.ChangePulse(Get.Loadout.MusicData.DrumSequencerRhythm);
		}

		// Token: 0x06002FF3 RID: 12275 RVA: 0x000E0E58 File Offset: 0x000DF058
		public void ChangePulse(Rhythm newRhythm)
		{
			this.Module.ChangePulse(newRhythm);
			this.Init();
		}

		// Token: 0x06002FF4 RID: 12276 RVA: 0x000E0E6C File Offset: 0x000DF06C
		public static float GetPanLFO(double offset = 0.0)
		{
			offset *= 48.0;
			double dProg = (offset + AudioSystem.Instance.DspTime) % (double)Get.Pulse.Duratio(48f) / (double)Get.Pulse.Duratio(24f);
			return (float)((dProg > 1.0) ? (2.0 - dProg) : dProg);
		}

		// Token: 0x06002FF5 RID: 12277 RVA: 0x000E0EC8 File Offset: 0x000DF0C8
		protected override void OnPulse()
		{
			if ((!this.Play && !this.PauseMode) || Get.State.HasAny(new StateType[]
			{
				StateType.GameOver
			}) || (Get.Loadout.Id != "menu" && Get.AudibleGroups < 1))
			{
				return;
			}
			MusicData mD = Get.Loadout.MusicData;
			if (this.prevScale != Get.Pulse.Scale && Get.Pulse.Scale != TimeScale.DoubleSlow && Get.Pulse.Scale != TimeScale.SingleSlow)
			{
				this.Module.ChangePulse(mD.DrumSequencerRhythm.Scale(Get.Pulse.Scale.Scale, false));
			}
			this.prevScale = Get.Pulse.Scale;
			this._pulseCount++;
			this.VolumeActual = mD.DrumVolume;
			float timer = (float)(this._pulseCount / this.Module.Rhythm.Steps.Length) * this.Module.Rhythm.Duration;
			if (((mD.DrumDelayDuration == 0f) ? 1f : (timer / mD.DrumDelayDuration)) < 1f)
			{
				return;
			}
			float attackProgress = (mD.DrumAttackDuration == 0f) ? 1f : ((timer - mD.DrumDelayDuration) / mD.DrumAttackDuration);
			if (attackProgress < 1f)
			{
				this.VolumeActual = Mathf.Lerp(0f, mD.DrumVolume, Maf.VolCurve(attackProgress));
			}
			this.Boom.Play(mD.Boom);
			this.Bap.Play(mD.Bap);
			this.Hat.Play(mD.Hat);
			if (mD != null)
			{
				mD.OnDrumPulse();
			}
			this.sequence_i++;
		}

		// Token: 0x06002FF6 RID: 12278 RVA: 0x000E1078 File Offset: 0x000DF278
		private void Init()
		{
			this.Play = true;
			List<int> stepOptions = Liszt.From<int>(new int[]
			{
				8,
				12,
				16,
				24,
				32,
				40
			});
			int boomSteps = Rando.Pick<int>(stepOptions);
			stepOptions.Remove(boomSteps);
			int bapSteps = Rando.Pick<int>(stepOptions);
			int hatSteps = Rando.Pick<int>(stepOptions);
			this.Boom = new DrumSequencer.Part(boomSteps, UnityEngine.Random.Range(2, boomSteps - 1), Param.Group.Make(0.2f, 1f, 0.75f, 1.25f), "perc_kick", 0f);
			this.Bap = new DrumSequencer.Part(Rando.Pick<int>(stepOptions), UnityEngine.Random.Range(1, bapSteps / 2), Param.Group.Make(0.2f, 0.5f, 2f, 6f), "perc_kick", 0f);
			this.Hat = new DrumSequencer.Part(Rando.Pick<int>(stepOptions), UnityEngine.Random.Range(hatSteps / 2, hatSteps), Param.Group.Make(0f, 0.4f, 1f, 4f), "PeepAppears_TRIANGLE", 0f);
			this.Parts = Liszt.From<DrumSequencer.Part>(new DrumSequencer.Part[]
			{
				this.Boom,
				this.Bap,
				this.Hat
			});
		}

		// Token: 0x0400295D RID: 10589
		private int sequence_i;

		// Token: 0x0400295E RID: 10590
		private bool pauseMode;

		// Token: 0x0400295F RID: 10591
		private bool play;

		// Token: 0x04002960 RID: 10592
		private int _pulseCount;

		// Token: 0x04002961 RID: 10593
		private float VolumeActual;

		// Token: 0x04002962 RID: 10594
		public DrumSequencer.Part Boom;

		// Token: 0x04002963 RID: 10595
		public DrumSequencer.Part Bap;

		// Token: 0x04002964 RID: 10596
		public DrumSequencer.Part Hat;

		// Token: 0x04002965 RID: 10597
		public List<DrumSequencer.Part> Parts;

		// Token: 0x04002966 RID: 10598
		private TimeScale prevScale;

		// Token: 0x020006D0 RID: 1744
		public struct Part
		{
			// Token: 0x06002FF8 RID: 12280 RVA: 0x000E11B4 File Offset: 0x000DF3B4
			public Part(int steps, int hits, Param.Group group, string sampleName = "perc_kick", float pseudoUpbeatChance = 0f)
			{
				this.SampleName = sampleName;
				this.PseudoUpbeatChance = pseudoUpbeatChance;
				this.Parameters = group;
				this.Steps = steps;
				this.Hits = hits;
				this.StartOnTrue = Rando.FlipCoin(0.5f);
				this.Reverse = Rando.FlipCoin(0.5f);
				this.Sequence = Maf.Bjorklund(this.Hits, this.Steps, this.StartOnTrue, this.Reverse);
				this.PanOffset = Rando.m(-1);
				this.Do = (this.On = false);
				this.Randoms = new List<List<float>>();
				for (int i = 0; i < 3; i++)
				{
					this.Randoms.Add(Liszt.Make<float>(this.Sequence.Count, () => Rando.m(-1)));
				}
			}

			// Token: 0x06002FF9 RID: 12281 RVA: 0x000E1296 File Offset: 0x000DF496
			public void Reroll()
			{
				this.Sequence = Maf.Bjorklund(this.Hits, this.Steps, this.StartOnTrue, this.Reverse);
			}

			// Token: 0x06002FFA RID: 12282 RVA: 0x000E12BB File Offset: 0x000DF4BB
			public void Toggle(float chance = 1f)
			{
				this.On = (Rando.FlipCoin(chance) ? (!this.On) : this.On);
			}

			// Token: 0x06002FFB RID: 12283 RVA: 0x000E12DC File Offset: 0x000DF4DC
			public void Play(bool on)
			{
				this.On = on;
				MusicData musicData = Get.Loadout.MusicData;
				DrumSequencer dS = Get.Loadout.DrumSequencer;
				if (musicData.UseEuclideanDrumGates)
				{
					this.Do = (this.On && this.Sequence.SafeGet(dS.sequence_i));
				}
				else
				{
					this.Do = this.On;
				}
				if (this.Do)
				{
					double t = dS.time;
					if (this.Randoms[2].SafeGet(dS.sequence_i) < this.PseudoUpbeatChance)
					{
						t += (dS.Module.NextPulseTime - dS.time) / 2.0;
					}
					AudioPlayer.Default.PlaySample(this.SampleName, DrumSequencer.GetPanLFO((double)this.PanOffset), dS.VolumeActual * Mathf.Lerp(this.Parameters.Gain.Range.x, this.Parameters.Gain.Range.y, this.Randoms[0].SafeGet(dS.sequence_i)), Mathf.Lerp(this.Parameters.Pitch.Range.x, this.Parameters.Pitch.Range.y, this.Randoms[1].SafeGet(dS.sequence_i)), 0.0, t, false, null, false, false, 0f, false);
				}
			}

			// Token: 0x04002967 RID: 10599
			public int Steps;

			// Token: 0x04002968 RID: 10600
			public int Hits;

			// Token: 0x04002969 RID: 10601
			public bool StartOnTrue;

			// Token: 0x0400296A RID: 10602
			public bool Reverse;

			// Token: 0x0400296B RID: 10603
			public List<bool> Sequence;

			// Token: 0x0400296C RID: 10604
			public float PanOffset;

			// Token: 0x0400296D RID: 10605
			public string SampleName;

			// Token: 0x0400296E RID: 10606
			public float PseudoUpbeatChance;

			// Token: 0x0400296F RID: 10607
			public Param.Group Parameters;

			// Token: 0x04002970 RID: 10608
			public bool Do;

			// Token: 0x04002971 RID: 10609
			public List<List<float>> Randoms;

			// Token: 0x04002972 RID: 10610
			public bool On;
		}
	}
}
