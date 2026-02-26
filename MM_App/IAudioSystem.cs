using System;
using System.Collections.Generic;
using GAudio;
using Motorways;
using Motorways.Audio;

// Token: 0x02000073 RID: 115
public interface IAudioSystem
{
	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060000F6 RID: 246
	double DspTime { get; }

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060000F7 RID: 247
	double PulsePeriod { get; }

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x060000F8 RID: 248
	TimeScale ActivePulseTimeScale { get; }

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x060000F9 RID: 249
	// (set) Token: 0x060000FA RID: 250
	TimeScale ScheduledPulseTimeScale { get; set; }

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x060000FB RID: 251
	bool RequiresSync { get; }

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060000FC RID: 252
	AudioDatabase Database { get; }

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060000FD RID: 253
	bool RequiresVolumeControl { get; }

	// Token: 0x060000FE RID: 254
	void UpdateVolume(int option);

	// Token: 0x14000001 RID: 1
	// (add) Token: 0x060000FF RID: 255
	// (remove) Token: 0x06000100 RID: 256
	event Action<double, int, int> SignalPulse;

	// Token: 0x06000101 RID: 257
	bool Start(bool isAudioRunning);

	// Token: 0x06000102 RID: 258
	void Tick();

	// Token: 0x06000103 RID: 259
	void ScheduleEvent(AudioEvent audioEvent);

	// Token: 0x06000104 RID: 260
	AudioLoadout GetLoadout(string loadoutId);

	// Token: 0x06000105 RID: 261
	List<AudioEvent> GetEvents(double fromDspTime, int minId, AudioEventFilter filter, City city = null);

	// Token: 0x06000106 RID: 262
	AudioSample GetSample(IGATDataOwner sampleData);

	// Token: 0x06000107 RID: 263
	int AddAudioEventListener(AudioSystem.SignalAudioEventScheduled signal, AudioEventFilter filter);

	// Token: 0x06000108 RID: 264
	void RemoveAudioEventListener(int listenerId);
}
