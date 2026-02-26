using System;
using System.Collections.Generic;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006C5 RID: 1733
	public class DemandTimer : Playback
	{
		// Token: 0x06002FB8 RID: 12216 RVA: 0x000DEA3C File Offset: 0x000DCC3C
		public DemandTimer(AudioEventFilter filter) : base(filter)
		{
		}

		// Token: 0x06002FB9 RID: 12217 RVA: 0x000DEAA0 File Offset: 0x000DCCA0
		protected override void OnPulse()
		{
			this.pulseCount++;
			if (base.GetEvents(0))
			{
				foreach (AudioEvent aE in this.audioEvents)
				{
					this.timerCount += (aE.Condition ? 1 : -1);
				}
				this.audioEvents.Clear();
			}
			if (this.timerCount < 1 || Get.State.HasAny(new StateType[]
			{
				StateType.GameOver,
				StateType.GamePaused,
				StateType.MenuPause,
				StateType.MenuUpgrades,
				StateType.MenuPhoto
			}))
			{
				this.pulseCount = 0;
				return;
			}
			this.maxOvercrowdingTime = (float)Get.GameConstants.MaxOvercrowdTime;
			this.longestOvercrowdingTime = 0f;
			this.pings.Clear();
			foreach (List<DestinationView> list in this.Environment.Destinations)
			{
				foreach (DestinationView d in list)
				{
					float ocTime = (float)d.Model.CurrentFrame.OvercrowdingTime;
					if (ocTime > 0f)
					{
						this.pings.Add(new DemandTimer.Ping(d.Pan.x, d.groupIndex, Maf.Map(ocTime / this.maxOvercrowdingTime, this.DANGER_START, 1f, 0f, 1f)));
						if (ocTime > this.longestOvercrowdingTime)
						{
							this.longestOvercrowdingTime = Mathf.Max(this.longestOvercrowdingTime, ocTime);
						}
					}
					this.maxOvercrowdingTime = Mathf.Max(this.maxOvercrowdingTime, d.MaxOvercrowdingTime);
				}
			}
			if (this.pings.Count == 0)
			{
				return;
			}
			int culprit_i = this.pulseCount % this.pings.Count;
			float pan = this.pings[culprit_i].Pan;
			float danger = Maf.Map(this.longestOvercrowdingTime / this.maxOvercrowdingTime, this.DANGER_START, 1f, 0f, 1f);
			float gain_a = Maf.VolCurve(Mathf.Lerp(0f, 1f, danger));
			float pitch = (this.pings.Count < 2) ? 0.75f : Maf.Map((float)(this.pulseCount % this.pings.Count), 0f, (float)(this.pings.Count - 1), 0.75f, 1.5f);
			pitch = Mathf.Lerp(pitch, 3f, danger);
			AudioPlayer.UI.PlaySample("DangerTick", pan, gain_a * this.TICK_GAINS[this.pulse.StepIndex % 2], pitch, 0.0, this.time, false, null, false, false, 0f, false);
			pitch = this.PITCHES[(int)Mathf.Lerp(0f, 1.999f, danger)] + Mathf.Lerp(0f, 0.225f, danger * danger);
			AudioPlayer.Default.PlaySample("LineCreated_" + Get.Loadout.MusicData.NoteWindow.SafeGet(culprit_i), pan, Mathf.Lerp(0f, 0.3f, this.pings.SafeGet(this.pulseCount).Danger) * this.TICK_GAINS[this.pulse.StepIndex % 2], pitch, 0.0, this.time, false, null, false, false, 0f, false);
		}

		// Token: 0x06002FBA RID: 12218 RVA: 0x000DEE60 File Offset: 0x000DD060
		public override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnRippleAlert), AudioEventType.RippleAlert, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnDestinationOvercrowding), AudioEventType.DestinationOvercrowding, -1);
		}

		// Token: 0x06002FBB RID: 12219 RVA: 0x000DEEB0 File Offset: 0x000DD0B0
		private void OnDestinationOvercrowding(AudioEvent e)
		{
			if (Get.Loadout.Id != "menu")
			{
				if (e.Condition)
				{
					AudioPlayer.UI.PlaySample("PopUp-" + Rando.Pick<string>(new string[]
					{
						"01",
						"02",
						"03"
					}), e.Pan, 0.8f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
					return;
				}
				AudioPlayer.UI.PlaySample("PinFulfilled-01", e.Pan, 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
			}
		}

		// Token: 0x06002FBC RID: 12220 RVA: 0x000DEF80 File Offset: 0x000DD180
		private void OnRippleAlert(AudioEvent e)
		{
			float g = (float)e.Destination.Model.CurrentFrame.OvercrowdingTime / this.maxOvercrowdingTime;
			AudioPlayer.UI.PlaySample("ui_stationWarning", e.Pan, g * 0.35f, Rando.Range(0.9f, 1.1f, -1), 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x04002924 RID: 10532
		private int timerCount;

		// Token: 0x04002925 RID: 10533
		private int pulseCount;

		// Token: 0x04002926 RID: 10534
		private List<DemandTimer.Ping> pings = new List<DemandTimer.Ping>();

		// Token: 0x04002927 RID: 10535
		private float maxOvercrowdingTime;

		// Token: 0x04002928 RID: 10536
		private float longestOvercrowdingTime;

		// Token: 0x04002929 RID: 10537
		private readonly float[] TICK_GAINS = new float[]
		{
			0.5f,
			0.083f
		};

		// Token: 0x0400292A RID: 10538
		private readonly float DANGER_START = 0.5f;

		// Token: 0x0400292B RID: 10539
		private readonly float[] PITCHES = new float[]
		{
			2f,
			4f
		};

		// Token: 0x020006C6 RID: 1734
		private struct Ping
		{
			// Token: 0x06002FBD RID: 12221 RVA: 0x000DEFF9 File Offset: 0x000DD1F9
			public Ping(float pan, int groupIndex, float danger)
			{
				this = default(DemandTimer.Ping);
				this.Pan = pan;
				this.GroupIndex = groupIndex;
				this.Danger = danger;
			}

			// Token: 0x0400292C RID: 10540
			public float Pan;

			// Token: 0x0400292D RID: 10541
			public int GroupIndex;

			// Token: 0x0400292E RID: 10542
			public float Danger;
		}
	}
}
