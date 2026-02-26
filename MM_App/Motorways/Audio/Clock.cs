using System;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006C4 RID: 1732
	public class Clock : Playback
	{
		// Token: 0x06002FB0 RID: 12208 RVA: 0x000DE6E5 File Offset: 0x000DC8E5
		public Clock(AudioEventFilter filter, string scenario = "") : base(filter)
		{
			this.scenario = scenario;
		}

		// Token: 0x06002FB1 RID: 12209 RVA: 0x000DE708 File Offset: 0x000DC908
		protected override void OnPulse()
		{
			Clock.NextPulseTime = this.Module.NextPulseTime;
			if (Get.Game.Simulation.IsPaused)
			{
				this.playCount = 0;
				return;
			}
			if (base.GetEvents(0))
			{
				this.playCount = 0;
				this.audioEvents.Clear();
			}
			this._hour = Get.Clock.Hour % 24;
			this._day = Get.Clock.Day % 7;
			if (Get.City.Rules.ScoringMode == ScoringMode.Trips && this._day == 6 && this._hour == 23)
			{
				Persistent.UpgradeChord(this.Module.NextPulseTime);
				this.playCountLimit = 8;
				this.playCount = 8;
			}
			this.Play();
		}

		// Token: 0x06002FB2 RID: 12210 RVA: 0x000DE7C8 File Offset: 0x000DC9C8
		private void Play()
		{
			Clock.GainFactor = 1f;
			if (this.scenario == "Clock")
			{
				if (Get.City.Rules.ScoringMode == ScoringMode.Trips && this._day == 6 && this._hour > 14 && this._hour < 24)
				{
					Clock.GainFactor = 0.5f * Mathf.Pow(((float)this._hour - 14f) / 9f, 1.5f);
				}
				else
				{
					if (this.playCount == this.playCountLimit)
					{
						Clock.GainFactor = 0f;
						return;
					}
					this.playCount++;
					Clock.GainFactor = Mathf.Pow(Clock.GainFactor / (float)this.playCount, 1.5f);
				}
			}
			if (this.scenario == "Click")
			{
				Clock.GainFactor = 0f;
				AudioPlayer.UI.PlaySample("metronome_0", 0.5f, 0.5f, 1f, 0.0, this.time, false, null, false, false, 0f, false);
				return;
			}
			AudioPlayer.UI.PlaySample("metronome_0", 0.75f, Clock.GainFactor * 0.5f, 1f, 0.0, this.time, false, null, false, false, 0f, false);
		}

		// Token: 0x06002FB3 RID: 12211 RVA: 0x000DE924 File Offset: 0x000DCB24
		public override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnPressPlay), UIEventType.Click, UIAudioProfile.Play);
			this.EventListener.Add(new Action<AudioEvent>(this.OnPressFF), UIEventType.Click, UIAudioProfile.FastForward);
			this.EventListener.Add(new Action<AudioEvent>(this.OnClockToggle), UIEventType.Click, UIAudioProfile.Clock);
			this.EventListener.Add(new Action<AudioEvent>(this.OnClockStart), AudioEventType.ClockStart, -1);
		}

		// Token: 0x06002FB4 RID: 12212 RVA: 0x000DE9A2 File Offset: 0x000DCBA2
		private void OnClockStart(AudioEvent e)
		{
			this.playCount = 0;
		}

		// Token: 0x06002FB5 RID: 12213 RVA: 0x000DE9AC File Offset: 0x000DCBAC
		private void OnClockToggle(AudioEvent e)
		{
			string sampleName = e.Condition ? "clock-show-controls" : "clock-hide-controls";
			double delay = e.Condition ? 0.1 : 0.0;
			this.playCount = (e.Condition ? 0 : this.playCountLimit);
			AudioPlayer.UI.PlaySample(sampleName, 0.75f, 0.5f, 1f, 0.0, AudioSystem.Instance.DspTime + delay, false, null, false, false, 0f, false);
		}

		// Token: 0x06002FB6 RID: 12214 RVA: 0x000DE9A2 File Offset: 0x000DCBA2
		private void OnPressPlay(AudioEvent e)
		{
			this.playCount = 0;
		}

		// Token: 0x06002FB7 RID: 12215 RVA: 0x000DE9A2 File Offset: 0x000DCBA2
		private void OnPressFF(AudioEvent e)
		{
			this.playCount = 0;
		}

		// Token: 0x0400291D RID: 10525
		private string scenario;

		// Token: 0x0400291E RID: 10526
		private int playCountLimit = 9;

		// Token: 0x0400291F RID: 10527
		private int playCount = 9;

		// Token: 0x04002920 RID: 10528
		private int _hour;

		// Token: 0x04002921 RID: 10529
		private int _day;

		// Token: 0x04002922 RID: 10530
		public static float GainFactor;

		// Token: 0x04002923 RID: 10531
		public static double NextPulseTime;
	}
}
