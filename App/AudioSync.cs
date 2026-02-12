using System;

// Token: 0x0200024D RID: 589
public class AudioSync
{
	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06000E0A RID: 3594 RVA: 0x0002F7B2 File Offset: 0x0002D9B2
	// (set) Token: 0x06000E0B RID: 3595 RVA: 0x0002F7BA File Offset: 0x0002D9BA
	public AudioSyncState State { get; private set; }

	// Token: 0x06000E0C RID: 3596 RVA: 0x0002F7C3 File Offset: 0x0002D9C3
	public void StartClock()
	{
		this.State = AudioSyncState.WaitForFirstPulse;
		this._nextSyncState = AudioSyncState.StartClock;
		this._isSyncForced = false;
		this._lastPulseTime = -1.0;
		this._lastPulsePeriod = -1.0;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0002F7F8 File Offset: 0x0002D9F8
	public void ResumeClock(float gamePulseProgress)
	{
		if (this.State != AudioSyncState.WaitForFirstPulse)
		{
			this.State = AudioSyncState.ResumeClock;
		}
		else
		{
			this._nextSyncState = AudioSyncState.ResumeClock;
		}
		this._gamePulseProgress = gamePulseProgress;
		this._isSyncForced = false;
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0002F820 File Offset: 0x0002DA20
	public void SyncTimeInterval(TimeInterval time, double nextPulseTime, IAudioSystem audioSystem)
	{
		if (!audioSystem.RequiresSync)
		{
			return;
		}
		float deltaTime = time.UnsyncedDelta;
		double dspTime = audioSystem.DspTime;
		double pulsePeriod = audioSystem.PulsePeriod;
		if (this.State == AudioSyncState.WaitForFirstPulse || this.State == AudioSyncState.StartClock)
		{
			if (nextPulseTime >= 0.0)
			{
				if (dspTime >= nextPulseTime)
				{
					if (this.State == AudioSyncState.WaitForFirstPulse)
					{
						deltaTime = 0f;
						this.State = this._nextSyncState;
					}
					else
					{
						deltaTime = (float)(dspTime - nextPulseTime);
						this.State = AudioSyncState.Synced;
						nextPulseTime = -1.0;
					}
				}
				else
				{
					deltaTime = 0f;
				}
			}
			else
			{
				deltaTime = 0f;
			}
		}
		if (this.State == AudioSyncState.ResumeClock && this._lastPulseTime >= 0.0)
		{
			float audioPulseProgress = (float)((dspTime - this._lastPulseTime) / this._lastPulsePeriod);
			if (this._gamePulseProgress > audioPulseProgress)
			{
				deltaTime = 0f;
				this._isSyncForced = true;
			}
			else
			{
				float pulseDelta = (audioPulseProgress - this._gamePulseProgress) * (float)pulsePeriod;
				if (this._isSyncForced || pulseDelta <= deltaTime * 2f)
				{
					deltaTime = pulseDelta;
					time.IsPaused = false;
					this.State = AudioSyncState.Scale;
					this._isSyncForced = false;
				}
				else
				{
					deltaTime = 0f;
				}
			}
		}
		else if (this.State == AudioSyncState.Scale)
		{
			TimeScale pulseTimeScale = audioSystem.ActivePulseTimeScale;
			if (pulseTimeScale != time.Scale)
			{
				deltaTime /= time.Scale.Scale;
				deltaTime *= pulseTimeScale.Scale;
			}
			else
			{
				this.State = AudioSyncState.Synced;
			}
		}
		if (nextPulseTime >= 0.0 && dspTime >= nextPulseTime)
		{
			if (this.State == AudioSyncState.ResumeClock)
			{
				this._isSyncForced = true;
			}
			this._lastPulseTime = nextPulseTime;
			this._lastPulsePeriod = pulsePeriod;
			nextPulseTime = -1.0;
		}
		time.Delta = deltaTime;
	}

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06000E0F RID: 3599 RVA: 0x0002F9B0 File Offset: 0x0002DBB0
	public bool IsSynced
	{
		get
		{
			return this.State == AudioSyncState.Synced;
		}
	}

	// Token: 0x0400083A RID: 2106
	private AudioSyncState _nextSyncState;

	// Token: 0x0400083B RID: 2107
	private bool _isSyncForced;

	// Token: 0x0400083C RID: 2108
	private double _lastPulseTime = -1.0;

	// Token: 0x0400083D RID: 2109
	private double _lastPulsePeriod = -1.0;

	// Token: 0x0400083E RID: 2110
	private float _gamePulseProgress;
}
