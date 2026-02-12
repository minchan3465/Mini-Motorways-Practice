using System;
using System.Collections.Generic;
using GAudio;
using Motorways;
using Motorways.Audio;
using UnityEngine;

// Token: 0x02000074 RID: 116
public class NullAudioSystem : IAudioSystem
{
	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000109 RID: 265 RVA: 0x00004BAF File Offset: 0x00002DAF
	public double DspTime
	{
		get
		{
			return AudioSettings.dspTime;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x0600010A RID: 266 RVA: 0x00004BB6 File Offset: 0x00002DB6
	public double PulsePeriod
	{
		get
		{
			return 0.8333333333333334;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x0600010B RID: 267 RVA: 0x00004BC1 File Offset: 0x00002DC1
	public TimeScale ActivePulseTimeScale
	{
		get
		{
			return TimeScale.Single;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x0600010C RID: 268 RVA: 0x00004BC8 File Offset: 0x00002DC8
	// (set) Token: 0x0600010D RID: 269 RVA: 0x00004BD0 File Offset: 0x00002DD0
	public TimeScale ScheduledPulseTimeScale { get; set; }

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x0600010E RID: 270 RVA: 0x0000222C File Offset: 0x0000042C
	public bool RequiresSync
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x0600010F RID: 271 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public AudioDatabase Database
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000110 RID: 272 RVA: 0x000020AA File Offset: 0x000002AA
	public bool RequiresVolumeControl
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000111 RID: 273 RVA: 0x000022F5 File Offset: 0x000004F5
	public void UpdateVolume(int index)
	{
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x06000112 RID: 274 RVA: 0x00004BDC File Offset: 0x00002DDC
	// (remove) Token: 0x06000113 RID: 275 RVA: 0x00004C14 File Offset: 0x00002E14
	public event Action<double, int, int> SignalPulse;

	// Token: 0x06000114 RID: 276 RVA: 0x000020AA File Offset: 0x000002AA
	public bool Start(bool isAudioRunning)
	{
		return true;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Tick()
	{
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ScheduleEvent(AudioEvent audioEvent)
	{
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public AudioLoadout GetLoadout(string loadoutId)
	{
		return null;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x00004C49 File Offset: 0x00002E49
	public List<AudioEvent> GetEvents(double fromDspTime, int minId, AudioEventFilter filter, City city = null)
	{
		return new List<AudioEvent>();
	}

	// Token: 0x06000119 RID: 281 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public AudioSample GetSample(IGATDataOwner sampleData)
	{
		return null;
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00004C50 File Offset: 0x00002E50
	public int AddAudioEventListener(AudioSystem.SignalAudioEventScheduled signal, AudioEventFilter filter)
	{
		return -1;
	}

	// Token: 0x0600011B RID: 283 RVA: 0x000022F5 File Offset: 0x000004F5
	public void RemoveAudioEventListener(int listenerId)
	{
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00004C53 File Offset: 0x00002E53
	public NullAudioSystem()
	{
		AudioSystem.Hack_DontCallSetAudioSystem(this);
	}
}
