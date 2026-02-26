using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Motorways.Audio
{
	// Token: 0x02000666 RID: 1638
	public class AudioMixbus
	{
		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06002D85 RID: 11653 RVA: 0x000D2643 File Offset: 0x000D0843
		public AudioMixer Mixer
		{
			get
			{
				return this._audioMixer;
			}
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06002D86 RID: 11654 RVA: 0x000D264C File Offset: 0x000D084C
		// (set) Token: 0x06002D87 RID: 11655 RVA: 0x000D266D File Offset: 0x000D086D
		public float Pitch
		{
			get
			{
				float p;
				this._audioMixer.GetFloat("Pitch", out p);
				return p;
			}
			set
			{
				this._audioMixer.SetFloat("Pitch", value);
				this._audioMixer.SetFloat("Volume", AudioMixbus.PitchToVolumeCompensation(value));
			}
		}

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x06002D88 RID: 11656 RVA: 0x000D2698 File Offset: 0x000D0898
		// (set) Token: 0x06002D89 RID: 11657 RVA: 0x000D26B9 File Offset: 0x000D08B9
		public float Volume
		{
			get
			{
				float v;
				this._audioMixer.GetFloat("MasterVolume", out v);
				return v;
			}
			set
			{
				this.InterpolateVolume(value, 2f);
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x06002D8A RID: 11658 RVA: 0x000D26C8 File Offset: 0x000D08C8
		// (set) Token: 0x06002D8B RID: 11659 RVA: 0x000D26E9 File Offset: 0x000D08E9
		public float EchoDelay
		{
			get
			{
				float d;
				this._audioMixer.GetFloat("EchoDelay", out d);
				return d;
			}
			set
			{
				this.InterpolateEchoDelay(value, 0.5f, 1, Twerp.CurveType.None);
			}
		}

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x06002D8C RID: 11660 RVA: 0x000D26FC File Offset: 0x000D08FC
		// (set) Token: 0x06002D8D RID: 11661 RVA: 0x000D271D File Offset: 0x000D091D
		public float EchoWet
		{
			get
			{
				float w;
				this._audioMixer.GetFloat("EchoWet", out w);
				return w;
			}
			set
			{
				this.InterpolateEchoWet(value, 4f, 1, Twerp.CurveType.None);
			}
		}

		// Token: 0x170007CF RID: 1999
		// (get) Token: 0x06002D8E RID: 11662 RVA: 0x000D2730 File Offset: 0x000D0930
		// (set) Token: 0x06002D8F RID: 11663 RVA: 0x000D2751 File Offset: 0x000D0951
		public float EchoDecay
		{
			get
			{
				float d;
				this._audioMixer.GetFloat("EchoDecay", out d);
				return d;
			}
			set
			{
				this.InterpolateEchoDecay(value, 0.5f, 1, Twerp.CurveType.None);
			}
		}

		// Token: 0x06002D90 RID: 11664 RVA: 0x000D2761 File Offset: 0x000D0961
		public AudioMixbus()
		{
			this._audioMixer = (Resources.Load("Audio/Master") as AudioMixer);
		}

		// Token: 0x06002D91 RID: 11665 RVA: 0x000D2780 File Offset: 0x000D0980
		public void InterpolateWetLevel(AudioMixbus.FilterType filterType, float targetWetLevel, float duration)
		{
			string param = ((filterType == AudioMixbus.FilterType.Highpass) ? "High" : "Low") + "passWet";
			float targetWetLevelDb = Mathf.Max(-79.99f, Mathf.Lerp(-80f, 0f, targetWetLevel));
			float currentWetLevelDb;
			this._audioMixer.GetFloat(param, out currentWetLevelDb);
			int pow = (currentWetLevelDb > targetWetLevelDb) ? 3 : -3;
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat(param, x);
			}, currentWetLevelDb, targetWetLevelDb, duration, pow, Twerp.CurveType.None, null));
		}

		// Token: 0x06002D92 RID: 11666 RVA: 0x000D2810 File Offset: 0x000D0A10
		public void InterpolateCutoffFreq(AudioMixbus.FilterType filterType, float targetFreq, float duration)
		{
			string param = ((filterType == AudioMixbus.FilterType.Highpass) ? "High" : "Low") + "passCutoff";
			float currentFreq;
			this._audioMixer.GetFloat(param, out currentFreq);
			int pow = (currentFreq > targetFreq) ? -3 : 3;
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat(param, x);
			}, currentFreq, targetFreq, duration, pow, Twerp.CurveType.None, null));
		}

		// Token: 0x06002D93 RID: 11667 RVA: 0x000D2888 File Offset: 0x000D0A88
		public void BoingPitchInPlace(float duration, float freq, float amp, float phase = 0f)
		{
			Action<bool> callback = delegate(bool b)
			{
				if (b)
				{
					this._audioMixer.SetFloat("Pitch", Settings.PITCH_ANCHOR);
				}
			};
			Twerp.StartCoroutine(Twerp.InterpolateFloatBoingInPlace(delegate(float x)
			{
				this._audioMixer.SetFloat("Pitch", x);
			}, Settings.PITCH_ANCHOR, duration, freq, amp, phase, callback));
		}

		// Token: 0x06002D94 RID: 11668 RVA: 0x000D28C4 File Offset: 0x000D0AC4
		public void InterpolatePitch(float targetPitch, float duration, int pow = 1, Twerp.CurveType curve = Twerp.CurveType.None)
		{
			float currentPitch = this.Pitch;
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat("Pitch", x);
			}, currentPitch, targetPitch, duration, pow, curve, null));
			float currentVol = AudioMixbus.PitchToVolumeCompensation(currentPitch);
			float targetVol = AudioMixbus.PitchToVolumeCompensation(targetPitch);
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat("Volume", x);
			}, currentVol, targetVol, duration, pow, curve, null));
		}

		// Token: 0x06002D95 RID: 11669 RVA: 0x000D2924 File Offset: 0x000D0B24
		public void InterpolateVolume(float targetVolume, float duration)
		{
			float currentVolume = this.Volume;
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat("MasterVolume", x);
			}, currentVolume, targetVolume, duration, 1, Twerp.CurveType.None, null));
		}

		// Token: 0x06002D96 RID: 11670 RVA: 0x000D2958 File Offset: 0x000D0B58
		public void InterpolateEchoDelay(float targetDelay, float duration, int pow = 1, Twerp.CurveType curve = Twerp.CurveType.None)
		{
			float currentDelay = this.EchoDelay;
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat("EchoDelay", x);
			}, currentDelay, targetDelay, duration, pow, curve, null));
		}

		// Token: 0x06002D97 RID: 11671 RVA: 0x000D298C File Offset: 0x000D0B8C
		public void InterpolateEchoDecay(float targetDecay, float duration, int pow = 1, Twerp.CurveType curve = Twerp.CurveType.None)
		{
			float currentDecay = this.EchoDecay;
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat("EchoDecay", x);
			}, currentDecay, targetDecay, duration, pow, curve, null));
		}

		// Token: 0x06002D98 RID: 11672 RVA: 0x000D29C0 File Offset: 0x000D0BC0
		public void InterpolateEchoWet(float targetWet, float duration, int pow = 1, Twerp.CurveType curve = Twerp.CurveType.None)
		{
			float currentWet = this.EchoWet;
			Twerp.StartCoroutine(Twerp.InterpolateFloat(delegate(float x)
			{
				this._audioMixer.SetFloat("EchoWet", x);
			}, currentWet, targetWet, duration, pow, curve, null));
		}

		// Token: 0x06002D99 RID: 11673 RVA: 0x000D29F2 File Offset: 0x000D0BF2
		private static float PitchToVolumeCompensation(float pitch)
		{
			return Maf.Map(pitch, 1f, 2f, 0f, Settings.PITCH_MIXBUS_ATTENUATION);
		}

		// Token: 0x0400279B RID: 10139
		private AudioMixer _audioMixer;

		// Token: 0x02000667 RID: 1639
		public enum FilterType
		{
			// Token: 0x0400279D RID: 10141
			Lowpass,
			// Token: 0x0400279E RID: 10142
			Highpass
		}
	}
}
