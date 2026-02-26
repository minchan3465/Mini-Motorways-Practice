using System;
using System.Collections.Generic;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006D6 RID: 1750
	public class Playback
	{
		// Token: 0x0600300C RID: 12300 RVA: 0x000E1848 File Offset: 0x000DFA48
		public Playback(AudioEventFilter filter, float gain = 1f)
		{
			this.filter = filter;
			this.gain = gain;
		}

		// Token: 0x0600300D RID: 12301 RVA: 0x000E18A0 File Offset: 0x000DFAA0
		public Playback(AudioEventFilter filter, string[] samples, float gain = 1f) : this(filter)
		{
			this.samples = samples;
			this.gain = gain;
		}

		// Token: 0x0600300E RID: 12302 RVA: 0x000E18B8 File Offset: 0x000DFAB8
		public Playback(AudioEventFilter filter)
		{
			this.filter = filter;
		}

		// Token: 0x0600300F RID: 12303 RVA: 0x000E1909 File Offset: 0x000DFB09
		public Playback()
		{
		}

		// Token: 0x06003010 RID: 12304 RVA: 0x000E1948 File Offset: 0x000DFB48
		public void OnGATPulse(IGATPulseInfo pulse, double lastPulseTime)
		{
			this.pulse = pulse;
			this.master = pulse.PulseSender.MasterPulseInfo;
			this.time = pulse.PulseDspTime;
			this.lastPulseTime = lastPulseTime;
			this.pseudoStep = this.master.StepIndex * pulse.NbOfSteps + pulse.StepIndex;
			if (!this.hasLoggedLagWarning && this.time < AudioSettings.dspTime + GATInfo.AudioBufferDuration)
			{
				Dbug.Log.Warn("Scheduled PulseDspTime ({0:0.##}) has lagged behind the current DSP time ({1:0.##}) plus the buffer duration ({2:0.##}) in Playback {3}. ({0:0.##} < {4:0.##}))", new object[]
				{
					this.time,
					AudioSettings.dspTime,
					GATInfo.AudioBufferDuration,
					this,
					AudioSettings.dspTime + GATInfo.AudioBufferDuration
				});
				this.hasLoggedLagWarning = true;
				return;
			}
			this.OnPulse();
		}

		// Token: 0x06003011 RID: 12305 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnBeginPulse()
		{
		}

		// Token: 0x06003012 RID: 12306 RVA: 0x000E1A1C File Offset: 0x000DFC1C
		public void Activate(AudioEnvironment environment)
		{
			this.Environment = environment;
			this.EventListener.Start(new Action(this.AddEventListeners));
			this.hasLoggedLagWarning = false;
		}

		// Token: 0x06003013 RID: 12307 RVA: 0x000E1A44 File Offset: 0x000DFC44
		public void Deactivate()
		{
			this.EventListener.Stop();
			this.Environment = null;
		}

		// Token: 0x06003014 RID: 12308 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void AddEventListeners()
		{
		}

		// Token: 0x06003015 RID: 12309 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnActivate()
		{
		}

		// Token: 0x06003016 RID: 12310 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnDeactivate()
		{
		}

		// Token: 0x06003017 RID: 12311 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void Update()
		{
		}

		// Token: 0x06003018 RID: 12312 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnPulse()
		{
		}

		// Token: 0x06003019 RID: 12313 RVA: 0x000E1A58 File Offset: 0x000DFC58
		protected bool GetEvents(int limit = 0)
		{
			List<AudioEvent> newEvents = AudioSystem.Instance.GetEvents(this.lastPulseTime, this.lastEventId, this.filter, (this.Environment == null) ? null : this.Environment.City);
			if (newEvents != null && newEvents.Count != 0)
			{
				if (newEvents.Count > limit && limit != 0)
				{
					newEvents.RemoveRange(limit, newEvents.Count - limit);
				}
				this.audioEvents.AddRange(newEvents);
				this.lastEventId = newEvents[newEvents.Count - 1].Id + 1;
			}
			return this.audioEvents.Count != 0;
		}

		// Token: 0x04002978 RID: 10616
		protected AudioEnvironment Environment;

		// Token: 0x04002979 RID: 10617
		protected double lastPulseTime;

		// Token: 0x0400297A RID: 10618
		protected IGATPulseInfo pulse;

		// Token: 0x0400297B RID: 10619
		protected IGATPulseInfo master;

		// Token: 0x0400297C RID: 10620
		public int pseudoStep;

		// Token: 0x0400297D RID: 10621
		public double time;

		// Token: 0x0400297E RID: 10622
		protected List<AudioEvent> audioEvents = new List<AudioEvent>();

		// Token: 0x0400297F RID: 10623
		protected int lastEventId;

		// Token: 0x04002980 RID: 10624
		protected AudioEventFilter filter;

		// Token: 0x04002981 RID: 10625
		public PulsedAudioModule Module;

		// Token: 0x04002982 RID: 10626
		public AudioEventListener EventListener = new AudioEventListener();

		// Token: 0x04002983 RID: 10627
		protected float gain = 1f;

		// Token: 0x04002984 RID: 10628
		protected float pan = -1f;

		// Token: 0x04002985 RID: 10629
		protected float pitch = 1f;

		// Token: 0x04002986 RID: 10630
		protected string[] samples;

		// Token: 0x04002987 RID: 10631
		private bool hasLoggedLagWarning;
	}
}
