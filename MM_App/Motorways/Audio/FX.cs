using System;
using System.Linq;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000649 RID: 1609
	public static class FX
	{
		// Token: 0x06002D05 RID: 11525 RVA: 0x000D0190 File Offset: 0x000CE390
		public static void UpdateEcho()
		{
			Get.Mixbus.EchoDecay = Settings.ECHO_DECAY_RANGE.Random(-1) * ((Get.Pulse.Scale == TimeScale.DoubleSlow) ? 0.5f : 1f);
			Get.Mixbus.EchoDelay = 1000f * Get.Loadout.MusicData.EchoDuration();
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x000D01EC File Offset: 0x000CE3EC
		public static void ToggleEcho(bool on)
		{
			if (FX.IsEchoing == on)
			{
				return;
			}
			if (on)
			{
				Get.Mixbus.EchoWet = Settings.ECHO_WET_RANGE.Random(-1) * ((Get.Pulse.Scale == TimeScale.DoubleSlow) ? 0.5f : 1f);
				FX.UpdateEcho();
				FX.IsEchoing = true;
				return;
			}
			Get.Mixbus.EchoWet = 0f;
			Get.Mixbus.EchoDecay = 0.75f;
			FX.IsEchoing = false;
		}

		// Token: 0x06002D07 RID: 11527 RVA: 0x000D0264 File Offset: 0x000CE464
		public static void ToggleNightMode(bool on, bool init = false)
		{
			if (Get.Loadout == null)
			{
				return;
			}
			FX.ToggleEcho(on);
			AudioLoadout loadout = Get.Loadout;
			bool flag;
			if (loadout == null)
			{
				flag = (null != null);
			}
			else
			{
				MusicData musicData = loadout.MusicData;
				flag = (((musicData != null) ? musicData.NightQualities : null) != null);
			}
			if (flag)
			{
				Get.Loadout.MusicData.CurrentQualities = (on ? Get.Loadout.MusicData.NightQualities.ToList<Quality>() : Get.Loadout.MusicData.DayQualities.ToList<Quality>());
			}
			if (init && on)
			{
				Get.Mixbus.Pitch = Settings.PITCH_NIGHT;
				Settings.PITCH_ANCHOR = Settings.PITCH_NIGHT;
				return;
			}
			AudioLoadout loadout2 = Get.Loadout;
			if (loadout2 != null)
			{
				MusicData musicData2 = loadout2.MusicData;
				if (musicData2 != null)
				{
					AudioSample bass = musicData2.Bass;
					if (bass != null)
					{
						bass.FadeOutAndStop(0.5);
					}
				}
			}
			float transitionTime = AudioEnvironment.Game.Theme.TransitionDuration * 2f;
			float p;
			if (Get.State.HasFlag(StateType.GamePaused))
			{
				p = (on ? (Settings.PITCH_NIGHT * Settings.PITCH_PAUSE) : Settings.PITCH_PAUSE);
			}
			else
			{
				p = (on ? Settings.PITCH_NIGHT : 1f);
			}
			Settings.PITCH_ANCHOR = p;
			Get.Mixbus.InterpolatePitch(p, transitionTime, 1, Twerp.CurveType.Boing);
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x000D0394 File Offset: 0x000CE594
		public static void TogglePauseModePitch(bool on)
		{
			float p;
			if (Get.State.HasFlag(StateType.ModeNight))
			{
				p = (on ? (Settings.PITCH_NIGHT * Settings.PITCH_PAUSE) : Settings.PITCH_NIGHT);
			}
			else
			{
				p = (on ? Settings.PITCH_PAUSE : 1f);
			}
			Settings.PITCH_ANCHOR = p;
			Get.Mixbus.InterpolatePitch(p, 2f, 1, Twerp.CurveType.Boing);
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x000D03FC File Offset: 0x000CE5FC
		public static float SineLFO(float freq)
		{
			return Mathf.Sin(Time.time * freq) * 0.5f + 0.5f;
		}

		// Token: 0x04002750 RID: 10064
		public static bool IsEchoing;

		// Token: 0x0200064A RID: 1610
		public class Modulator : GATDynamicMixInfo
		{
			// Token: 0x06002D0A RID: 11530 RVA: 0x000D0416 File Offset: 0x000CE616
			public Modulator(FX.Modulator.Portamento portamento = null, FX.Modulator.Vibrato vibrato = null, FX.Modulator.Tremolo tremolo = null)
			{
				this.Port = portamento;
				this.Vibr = vibrato;
				this.Trem = tremolo;
			}

			// Token: 0x06002D0B RID: 11531 RVA: 0x000D0433 File Offset: 0x000CE633
			public override void Update(double deltaDspTime)
			{
				FX.Modulator.Vibrato vibr = this.Vibr;
				if (vibr != null)
				{
					vibr.Update(this._timeThrough);
				}
				FX.Modulator.Tremolo trem = this.Trem;
				if (trem != null)
				{
					trem.Update(this._timeThrough);
				}
				this._timeThrough += deltaDspTime;
			}

			// Token: 0x170007BF RID: 1983
			// (get) Token: 0x06002D0C RID: 11532 RVA: 0x000D0471 File Offset: 0x000CE671
			public override float Gain
			{
				get
				{
					FX.Modulator.Tremolo trem = this.Trem;
					if (trem == null)
					{
						return 1f;
					}
					return trem.Value(this._timeThrough);
				}
			}

			// Token: 0x170007C0 RID: 1984
			// (get) Token: 0x06002D0D RID: 11533 RVA: 0x000D0490 File Offset: 0x000CE690
			public override double Pitch
			{
				get
				{
					if (this.Port == null && this.Vibr == null)
					{
						return base.Pitch;
					}
					FX.Modulator.Portamento port = this.Port;
					double pitchPole = (port != null) ? port.Value(this._timeThrough) : this.Vibr.PitchPole;
					double num = pitchPole;
					double num2 = pitchPole;
					FX.Modulator.Vibrato vibr = this.Vibr;
					double a = num + num2 * ((vibr != null) ? (-vibr.Amplitude) : 0.0);
					double num3 = pitchPole;
					double num4 = pitchPole;
					FX.Modulator.Vibrato vibr2 = this.Vibr;
					double b = num3 + num4 * ((vibr2 != null) ? vibr2.Amplitude : 0.0);
					FX.Modulator.Vibrato vibr3 = this.Vibr;
					return Maf.Lerp(a, b, (vibr3 != null) ? vibr3.Alpha(this._timeThrough) : 1.0);
				}
			}

			// Token: 0x04002751 RID: 10065
			private double _timeThrough;

			// Token: 0x04002752 RID: 10066
			public FX.Modulator.Portamento Port;

			// Token: 0x04002753 RID: 10067
			public FX.Modulator.Vibrato Vibr;

			// Token: 0x04002754 RID: 10068
			public FX.Modulator.Tremolo Trem;

			// Token: 0x0200064B RID: 1611
			public class Portamento
			{
				// Token: 0x06002D0E RID: 11534 RVA: 0x000D0539 File Offset: 0x000CE739
				public Portamento(double startPitch, double endPitch, double duration)
				{
					this.StartPitch = startPitch;
					this.EndPitch = endPitch;
					this.Duration = duration;
				}

				// Token: 0x06002D0F RID: 11535 RVA: 0x000D0558 File Offset: 0x000CE758
				public double Value(double timeThrough)
				{
					double alpha = (this.Duration < 0.001) ? 1.0 : Maf.Clamp(timeThrough / this.Duration, 0.0, 1.0);
					return this.StartPitch * (1.0 - alpha) + this.EndPitch * alpha;
				}

				// Token: 0x04002755 RID: 10069
				public double StartPitch;

				// Token: 0x04002756 RID: 10070
				public double EndPitch;

				// Token: 0x04002757 RID: 10071
				public double Duration;
			}

			// Token: 0x0200064C RID: 1612
			public class LFO
			{
				// Token: 0x06002D10 RID: 11536 RVA: 0x000D05BC File Offset: 0x000CE7BC
				public LFO(double freq, double amp, double phaseAlpha = 0.0)
				{
					this.FrequencyAtStart = freq;
					this.Frequency = freq;
					this.AmplitudeAtStart = amp;
					this.Amplitude = amp;
					this.Phase = Maf.Lerp(0.0, 6.283185307179586, phaseAlpha);
				}

				// Token: 0x06002D11 RID: 11537 RVA: 0x000D0610 File Offset: 0x000CE810
				public void Update(double timeThrough)
				{
					double curr = (timeThrough * this.Frequency + this.Phase) % 6.283185307179586;
					double next = timeThrough * this.Frequency % 6.283185307179586;
					this.Phase = curr - next;
				}

				// Token: 0x04002758 RID: 10072
				public double Frequency;

				// Token: 0x04002759 RID: 10073
				public double Amplitude;

				// Token: 0x0400275A RID: 10074
				public double FrequencyAtStart;

				// Token: 0x0400275B RID: 10075
				public double AmplitudeAtStart;

				// Token: 0x0400275C RID: 10076
				public double Phase;
			}

			// Token: 0x0200064D RID: 1613
			public class Vibrato : FX.Modulator.LFO
			{
				// Token: 0x06002D12 RID: 11538 RVA: 0x000D0653 File Offset: 0x000CE853
				public Vibrato(double freq, double amp, double pitchPole, double phaseAlpha) : base(freq, amp, phaseAlpha)
				{
					this.PitchPole = pitchPole;
				}

				// Token: 0x06002D13 RID: 11539 RVA: 0x000D0666 File Offset: 0x000CE866
				public double Alpha(double timeThrough)
				{
					return Math.Sin(timeThrough * this.Frequency + this.Phase) * 0.5 + 0.5;
				}

				// Token: 0x06002D14 RID: 11540 RVA: 0x000D0690 File Offset: 0x000CE890
				public double Value(double timeThrough)
				{
					return (this.PitchPole + this.PitchPole * -this.Amplitude) * (1.0 - this.Alpha(timeThrough)) + (this.PitchPole + this.PitchPole * this.Amplitude) * this.Alpha(timeThrough);
				}

				// Token: 0x0400275D RID: 10077
				public double PitchPole;
			}

			// Token: 0x0200064E RID: 1614
			public class Tremolo : FX.Modulator.LFO
			{
				// Token: 0x06002D15 RID: 11541 RVA: 0x000D06E1 File Offset: 0x000CE8E1
				public Tremolo(double freq, float amp, double phaseAlpha = 0.0) : base(freq, (double)amp, phaseAlpha)
				{
				}

				// Token: 0x06002D16 RID: 11542 RVA: 0x000D06F0 File Offset: 0x000CE8F0
				public float Value(double timeThrough)
				{
					float alpha = (float)(Math.Sin(timeThrough * (6.283185307179586 * this.Frequency) + this.Phase) * 0.5) + 0.5f;
					return 1f - (float)this.Amplitude * alpha;
				}
			}
		}
	}
}
