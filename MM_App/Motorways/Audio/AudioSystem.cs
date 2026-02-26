using System;
using System.Collections.Generic;
using Factory;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000686 RID: 1670
	public class AudioSystem : IAudioSystem, IGATPulseClient
	{
		// Token: 0x1400004A RID: 74
		// (add) Token: 0x06002E3E RID: 11838 RVA: 0x000D64FC File Offset: 0x000D46FC
		// (remove) Token: 0x06002E3F RID: 11839 RVA: 0x000D6534 File Offset: 0x000D4734
		public event Action<double, int, int> SignalPulse;

		// Token: 0x06002E40 RID: 11840 RVA: 0x000D656C File Offset: 0x000D476C
		public bool Start(bool isAudioRunning)
		{
			AudioSystem.Log.IsMuted = true;
			this.m_isRunning = true;
			this._player.DataChanged += this.OnSaveDataChanged;
			AudioSystem.Log.Info("AudioSystem: Starting. Sample rate: {0} kHz.", new object[]
			{
				AudioSettings.outputSampleRate
			});
			this.audioDatabase = new AudioDatabase();
			AudioSystem.Mixbus = new AudioMixbus();
			AudioPlayer.Default = new AudioPlayer("Default");
			AudioPlayer.UI = new AudioPlayer("UI");
			if (isAudioRunning && !this.audioDatabase.LoadBanks())
			{
				AudioSystem.Log.Warn("AudioSystem: Failed to load sample banks, disabling audio.", Array.Empty<object>());
				isAudioRunning = false;
			}
			if (isAudioRunning)
			{
				this.audioDatabase.LoadLoadouts();
				this.masterPulse = this.audioDatabase.MasterPulse;
				if (this.masterPulse != null)
				{
					this.defaultPulsePeriod = this.masterPulse.Period;
					MasterPulseModule masterPulseModule = this.masterPulse;
					masterPulseModule.onWillPulse = (PulseModule.OnPulseHandler)Delegate.Combine(masterPulseModule.onWillPulse, new PulseModule.OnPulseHandler(this.OnWillPulse));
					this.masterPulse.SubscribeToPulse(this);
					this.pulseLatency = (float)GATManager.UniqueInstance.PulseLatency * 1.5f;
				}
				else
				{
					AudioSystem.Log.Warn("AudioSystem: Failed to subscribe to master pulse.", Array.Empty<object>());
				}
			}
			else
			{
				this.pulseLatency = 0.15f;
			}
			return true;
		}

		// Token: 0x06002E41 RID: 11841 RVA: 0x000D66CE File Offset: 0x000D48CE
		public void Stop()
		{
			AudioPlayer @default = AudioPlayer.Default;
			if (@default != null)
			{
				@default.GAT.Stop();
			}
			AudioPlayer ui = AudioPlayer.UI;
			if (ui == null)
			{
				return;
			}
			ui.GAT.Stop();
		}

		// Token: 0x06002E42 RID: 11842 RVA: 0x000D66FC File Offset: 0x000D48FC
		public void Tick()
		{
			if (this.masterPulse != null)
			{
				if (Math.Abs(this.lastDspTime - this.DspTime) < 1E-05)
				{
					this.pausedDspFrameCount++;
					if (this.pausedDspFrameCount >= 90)
					{
						AudioSystem.Log.Warn("AudioSystem: Audio thread halted, faking DSP clock from now on.", Array.Empty<object>());
						MasterPulseModule masterPulseModule = this.masterPulse;
						masterPulseModule.onWillPulse = (PulseModule.OnPulseHandler)Delegate.Remove(masterPulseModule.onWillPulse, new PulseModule.OnPulseHandler(this.OnWillPulse));
						this.masterPulse.UnsubscribeToPulse(this);
						this.masterPulse = null;
						this.fakeDspTime = this.lastDspTime;
					}
				}
				else
				{
					this.pausedDspFrameCount = 0;
				}
				this.lastDspTime = this.DspTime;
			}
			else
			{
				this.fakeDspTime += (double)Time.deltaTime;
				while (this.fakeDspTime >= this.nextFakePulse - 0.10000000149011612)
				{
					Action<double, int, int> signalPulse = this.SignalPulse;
					if (signalPulse != null)
					{
						signalPulse(this.nextFakePulse, this.fakePulseCount % 12, this.fakePulseCount);
					}
					this.nextFakePulse += 0.8333333333333334 / (double)this.pulseTimeScale.Scale;
					this.fakePulseCount++;
				}
			}
			double expiryTime = this.DspTime - 1.0;
			int expiredEventCount = 0;
			while (expiredEventCount < this.events.Count && this.events[expiredEventCount].DspTime < expiryTime)
			{
				expiredEventCount++;
			}
			if (expiredEventCount > 0)
			{
				this.events.RemoveRange(0, expiredEventCount);
			}
			int sampleIndex = 0;
			while (sampleIndex < this.PlayingSamples.Count)
			{
				AudioSample sample = this.PlayingSamples[sampleIndex];
				if (sample.CanRecycle)
				{
					sample.Recycle();
					this.PlayingSamples.RemoveAt(sampleIndex);
				}
				else
				{
					sampleIndex++;
				}
			}
		}

		// Token: 0x06002E43 RID: 11843 RVA: 0x000D68D4 File Offset: 0x000D4AD4
		private void OnSaveDataChanged()
		{
			this.UpdateVolume((this._player.Soundscape == 0) ? 0 : this._player.VolumeSetting);
			if (this._player.Soundscape == 1)
			{
				if ((Get.State & StateType.Minimal) == StateType.None)
				{
					Get.State |= StateType.Minimal;
					this.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.AudioMinimized, 0.5f, -1f, true, null));
					return;
				}
			}
			else
			{
				Get.State &= ~StateType.Minimal;
			}
		}

		// Token: 0x06002E44 RID: 11844 RVA: 0x000D6970 File Offset: 0x000D4B70
		public void UpdateVolume(int index)
		{
			switch (index)
			{
			case 0:
				AudioSystem.Mixbus.Volume = -80f;
				return;
			case 1:
				AudioSystem.Mixbus.Volume = -20f;
				return;
			case 2:
				AudioSystem.Mixbus.Volume = -10f;
				return;
			case 3:
				AudioSystem.Mixbus.Volume = 0f;
				return;
			default:
				AudioSystem.Mixbus.Volume = 0f;
				return;
			}
		}

		// Token: 0x06002E45 RID: 11845 RVA: 0x000D69E4 File Offset: 0x000D4BE4
		public AudioSample GetSample(IGATDataOwner sampleData)
		{
			AudioSample sample;
			if (this.freeSamples.Count > 0)
			{
				sample = this.freeSamples[this.freeSamples.Count - 1];
				this.freeSamples.RemoveAt(this.freeSamples.Count - 1);
			}
			else
			{
				sample = new AudioSample();
			}
			sample.Initialise(sampleData);
			this.PlayingSamples.Add(sample);
			return sample;
		}

		// Token: 0x06002E46 RID: 11846 RVA: 0x000D6A50 File Offset: 0x000D4C50
		public void ScheduleEvent(AudioEvent audioEvent)
		{
			if (audioEvent == null)
			{
				return;
			}
			this.events.Add(audioEvent);
			try
			{
				int audioEventListenerCount = this.audioEventListeners.Count;
				for (int i = 0; i < audioEventListenerCount; i++)
				{
					AudioSystem.AudioEventListener audioEventListener = this.audioEventListeners[i];
					if (audioEventListener.filter.IsEventFiltered(audioEvent))
					{
						audioEventListener.signal(audioEvent);
					}
				}
			}
			catch (Exception exc)
			{
				AudioSystem.Log.Error("Hit exception {0} while signalling audio event {1} to listeners.", new object[]
				{
					exc,
					audioEvent
				});
			}
		}

		// Token: 0x06002E47 RID: 11847 RVA: 0x000D6AE0 File Offset: 0x000D4CE0
		public int AddAudioEventListener(AudioSystem.SignalAudioEventScheduled signal, AudioEventFilter filter)
		{
			this.audioEventListeners.Add(new AudioSystem.AudioEventListener(signal, filter));
			return this.audioEventListeners[this.audioEventListeners.Count - 1].id;
		}

		// Token: 0x06002E48 RID: 11848 RVA: 0x000D6B14 File Offset: 0x000D4D14
		public void RemoveAudioEventListener(int listenerId)
		{
			for (int listenerIndex = 0; listenerIndex < this.audioEventListeners.Count; listenerIndex++)
			{
				if (this.audioEventListeners[listenerIndex].id == listenerId)
				{
					this.audioEventListeners.RemoveAt(listenerIndex);
					return;
				}
			}
		}

		// Token: 0x170007E0 RID: 2016
		// (get) Token: 0x06002E49 RID: 11849 RVA: 0x000D6B58 File Offset: 0x000D4D58
		public float PulseLatency
		{
			get
			{
				return this.pulseLatency;
			}
		}

		// Token: 0x06002E4A RID: 11850 RVA: 0x000D6B60 File Offset: 0x000D4D60
		public List<AudioEvent> GetEvents(double fromDspTime, int minId, AudioEventFilter filter, City city = null)
		{
			this.queriedEvents.Clear();
			int eventIndex;
			for (eventIndex = 0; eventIndex < this.events.Count; eventIndex++)
			{
				if (this.events[eventIndex].Id >= minId)
				{
					break;
				}
			}
			while (eventIndex < this.events.Count)
			{
				if (filter.IsEventFiltered(this.events[eventIndex]))
				{
					City eventCity = this.events[eventIndex].City;
					if (eventCity == null || city == null || eventCity == city)
					{
						this.queriedEvents.Add(this.events[eventIndex]);
					}
				}
				eventIndex++;
			}
			return this.queriedEvents;
		}

		// Token: 0x170007E1 RID: 2017
		// (get) Token: 0x06002E4B RID: 11851 RVA: 0x000D6C06 File Offset: 0x000D4E06
		// (set) Token: 0x06002E4C RID: 11852 RVA: 0x000D6C0E File Offset: 0x000D4E0E
		public TimeScale ScheduledPulseTimeScale
		{
			get
			{
				return this.pulseTimeScale;
			}
			set
			{
				if (this.pulseTimeScale == value)
				{
					return;
				}
				this.pulseTimeScale = value;
				if (this.masterPulse != null)
				{
					this.masterPulse.NewPeriod = this.defaultPulsePeriod / (double)this.pulseTimeScale.Scale;
				}
			}
		}

		// Token: 0x170007E2 RID: 2018
		// (get) Token: 0x06002E4D RID: 11853 RVA: 0x000D6C4D File Offset: 0x000D4E4D
		public TimeScale ActivePulseTimeScale
		{
			get
			{
				if (this.masterPulse == null)
				{
					return this.pulseTimeScale;
				}
				return TimeScale.FromScale((float)(this.defaultPulsePeriod / this.masterPulse.Period));
			}
		}

		// Token: 0x170007E3 RID: 2019
		// (get) Token: 0x06002E4E RID: 11854 RVA: 0x000D6C7C File Offset: 0x000D4E7C
		public int SampleCount
		{
			get
			{
				return this.PlayingSamples.Count;
			}
		}

		// Token: 0x170007E4 RID: 2020
		// (get) Token: 0x06002E4F RID: 11855 RVA: 0x000D6C89 File Offset: 0x000D4E89
		public double PulsePeriod
		{
			get
			{
				if (this.masterPulse == null)
				{
					return 0.8333333333333334 / (double)this.pulseTimeScale.Scale;
				}
				return this.masterPulse.Period;
			}
		}

		// Token: 0x170007E5 RID: 2021
		// (get) Token: 0x06002E50 RID: 11856 RVA: 0x000020AA File Offset: 0x000002AA
		public bool RequiresSync
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002E51 RID: 11857 RVA: 0x000D6CBB File Offset: 0x000D4EBB
		public void OnWillPulse(IGATPulseInfo pulseInfo)
		{
			this.ScheduleEvent(AudioEvent.CreateEvent(pulseInfo.PulseDspTime, AudioEventType.Pulse, 0.5f, -1f, true, null));
		}

		// Token: 0x06002E52 RID: 11858 RVA: 0x000D6CE4 File Offset: 0x000D4EE4
		public void OnPulse(IGATPulseInfo pulseInfo)
		{
			if (pulseInfo.StepIndex == 0)
			{
				this.pulseLoopCount++;
			}
			Action<double, int, int> signalPulse = this.SignalPulse;
			if (signalPulse != null)
			{
				signalPulse(pulseInfo.PulseDspTime, pulseInfo.StepIndex, this.pulseLoopCount * this.Database.MasterPulse.Steps.Length + pulseInfo.StepIndex);
			}
			this.fakePulseCount = pulseInfo.StepIndex;
			this.nextFakePulse = pulseInfo.PulseDspTime + 0.8333333333333334;
		}

		// Token: 0x06002E53 RID: 11859 RVA: 0x000022F5 File Offset: 0x000004F5
		public void PulseStepsDidChange(bool[] newSteps)
		{
		}

		// Token: 0x170007E6 RID: 2022
		// (get) Token: 0x06002E54 RID: 11860 RVA: 0x000D6D66 File Offset: 0x000D4F66
		public double DspTime
		{
			get
			{
				if (!(this.masterPulse != null))
				{
					return this.fakeDspTime;
				}
				return AudioSettings.dspTime;
			}
		}

		// Token: 0x170007E7 RID: 2023
		// (get) Token: 0x06002E55 RID: 11861 RVA: 0x000D6D82 File Offset: 0x000D4F82
		public AudioDatabase Database
		{
			get
			{
				return this.audioDatabase;
			}
		}

		// Token: 0x170007E8 RID: 2024
		// (get) Token: 0x06002E56 RID: 11862 RVA: 0x000020AA File Offset: 0x000002AA
		public virtual bool RequiresVolumeControl
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06002E57 RID: 11863 RVA: 0x000D6D8A File Offset: 0x000D4F8A
		public AudioLoadout GetLoadout(string loadoutId)
		{
			if (this.audioDatabase == null)
			{
				return null;
			}
			return this.audioDatabase.GetLoadout(loadoutId);
		}

		// Token: 0x170007E9 RID: 2025
		// (get) Token: 0x06002E58 RID: 11864 RVA: 0x000D6DA2 File Offset: 0x000D4FA2
		public bool IsRunning
		{
			get
			{
				return this.m_isRunning;
			}
		}

		// Token: 0x170007EA RID: 2026
		// (get) Token: 0x06002E59 RID: 11865 RVA: 0x000D6DAA File Offset: 0x000D4FAA
		public static IAudioSystem Instance
		{
			get
			{
				return AudioSystem.instance;
			}
		}

		// Token: 0x06002E5A RID: 11866 RVA: 0x000D6DB4 File Offset: 0x000D4FB4
		public AudioSystem()
		{
			if (Diagnostics.Verify(AudioSystem.instance == null))
			{
				AudioSystem.instance = this;
			}
		}

		// Token: 0x06002E5B RID: 11867 RVA: 0x000D6E37 File Offset: 0x000D5037
		public static void Hack_DontCallSetAudioSystem(NullAudioSystem nullAudioSystem)
		{
			AudioSystem.instance = nullAudioSystem;
		}

		// Token: 0x04002836 RID: 10294
		private AudioDatabase audioDatabase;

		// Token: 0x04002837 RID: 10295
		private bool m_isRunning;

		// Token: 0x04002838 RID: 10296
		private MasterPulseModule masterPulse;

		// Token: 0x04002839 RID: 10297
		private double defaultPulsePeriod;

		// Token: 0x0400283A RID: 10298
		private int pulseLoopCount;

		// Token: 0x0400283B RID: 10299
		private TimeScale pulseTimeScale = TimeScale.Single;

		// Token: 0x0400283C RID: 10300
		private List<AudioEvent> queriedEvents = new List<AudioEvent>();

		// Token: 0x0400283D RID: 10301
		private List<AudioEvent> events = new List<AudioEvent>();

		// Token: 0x0400283E RID: 10302
		public List<AudioSample> PlayingSamples = new List<AudioSample>(200);

		// Token: 0x0400283F RID: 10303
		private List<AudioSample> freeSamples = new List<AudioSample>(200);

		// Token: 0x04002840 RID: 10304
		private float pulseLatency;

		// Token: 0x04002841 RID: 10305
		private double lastDspTime;

		// Token: 0x04002842 RID: 10306
		private int pausedDspFrameCount;

		// Token: 0x04002843 RID: 10307
		private double nextFakePulse = 0.8333333333333334;

		// Token: 0x04002844 RID: 10308
		private double fakeDspTime;

		// Token: 0x04002845 RID: 10309
		private int fakePulseCount;

		// Token: 0x04002846 RID: 10310
		private const double FAKE_PULSE_LATENCY = 0.10000000149011612;

		// Token: 0x04002847 RID: 10311
		private const double FAKE_PULSE_PERIOD = 0.8333333333333334;

		// Token: 0x04002848 RID: 10312
		private const int SkippedDspFrameThreshold = 90;

		// Token: 0x04002849 RID: 10313
		public static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Audio");

		// Token: 0x0400284A RID: 10314
		public static AudioMixbus Mixbus;

		// Token: 0x0400284C RID: 10316
		private List<AudioSystem.AudioEventListener> audioEventListeners = new List<AudioSystem.AudioEventListener>();

		// Token: 0x0400284D RID: 10317
		private static IAudioSystem instance;

		// Token: 0x0400284E RID: 10318
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x02000687 RID: 1671
		// (Invoke) Token: 0x06002E5E RID: 11870
		public delegate void SignalAudioEventScheduled(AudioEvent scheduledAudioEvent);

		// Token: 0x02000688 RID: 1672
		private struct AudioEventListener
		{
			// Token: 0x06002E61 RID: 11873 RVA: 0x000D6E50 File Offset: 0x000D5050
			public AudioEventListener(AudioSystem.SignalAudioEventScheduled signal, AudioEventFilter filter)
			{
				this.id = AudioSystem.AudioEventListener.nextId++;
				this.signal = signal;
				this.filter = filter;
			}

			// Token: 0x0400284F RID: 10319
			public int id;

			// Token: 0x04002850 RID: 10320
			public AudioSystem.SignalAudioEventScheduled signal;

			// Token: 0x04002851 RID: 10321
			public AudioEventFilter filter;

			// Token: 0x04002852 RID: 10322
			private static int nextId = 1;
		}
	}
}
