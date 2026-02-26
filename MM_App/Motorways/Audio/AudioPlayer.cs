using System;
using System.Collections.Generic;
using GAudio;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x02000676 RID: 1654
	public class AudioPlayer
	{
		// Token: 0x06002DDC RID: 11740 RVA: 0x000D5170 File Offset: 0x000D3370
		public AudioPlayer(string name)
		{
			AudioSource aS;
			if (name == "Default")
			{
				this.GAT = GATManager.DefaultPlayer;
				this.GAT.DeleteTrack(GATManager.DefaultPlayer.GetTrack(0));
				aS = this.GAT.gameObject.GetComponent<AudioSource>();
			}
			else
			{
				this.GAT = new GameObject().AddComponent<GATPlayer>();
				this.GAT.transform.parent = GATManager.UniqueInstance.transform;
				this.GAT.name = name + " Player";
				aS = this.GAT.gameObject.GetComponent<AudioSource>();
			}
			this.GAT.Clip = false;
			aS.bypassEffects = true;
			aS.bypassListenerEffects = true;
			aS.bypassReverbZones = true;
			aS.outputAudioMixerGroup = Get.Mixbus.Mixer.FindMatchingGroups(name)[0];
		}

		// Token: 0x170007D0 RID: 2000
		// (get) Token: 0x06002DDD RID: 11741 RVA: 0x000D524E File Offset: 0x000D344E
		public static double EarliestSchedulableTime
		{
			get
			{
				return AudioSystem.Instance.DspTime + 2.0 * GATInfo.AudioBufferDuration;
			}
		}

		// Token: 0x06002DDE RID: 11742 RVA: 0x000D526C File Offset: 0x000D346C
		public void PlayChord(string samplePrefix, List<string> notes, double dspTime = -1.0, float arpeggioRate = 0.05f, float gain = 1f, float gainEnd = -1f, float minPan = 0f, float maxPan = 1f, float fadeTimeStart = 0f, float fadeTimeEnd = 0f, int count = -1, bool downwards = false)
		{
			if (notes == null)
			{
				Dbug.Log.Info("PlayChord(): Notes list is null.", Array.Empty<object>());
				return;
			}
			if (gainEnd < 0f)
			{
				gainEnd = gain;
			}
			if (dspTime < 0.0)
			{
				dspTime = AudioPlayer.EarliestSchedulableTime + (double)arpeggioRate;
			}
			count = ((count < 1) ? notes.Count : Mathf.Min(count, notes.Count));
			for (int i = 0; i < count; i++)
			{
				int ii = downwards ? (count - 1 - i) : i;
				string sampleName = samplePrefix + "_" + notes[ii];
				float pan = UnityEngine.Random.Range(minPan, maxPan);
				double dspTime2 = dspTime + (double)(arpeggioRate * (float)i);
				this.PlaySample(sampleName, pan, Mathf.Lerp(gain, gainEnd, (float)((i == 0) ? 0 : (i / notes.Count - 1))) * Note.GainFactor(notes[ii]), 1f, (double)Mathf.Lerp(fadeTimeStart, fadeTimeEnd, (float)((i == 0) ? 0 : (i / notes.Count - 1))), dspTime2, false, null, false, false, 0f, false);
			}
		}

		// Token: 0x06002DDF RID: 11743 RVA: 0x000D5370 File Offset: 0x000D3570
		public AudioSample PrepSample(string sampleName, float pan = -1f, float pitch = 1f, double fadeTime = 0.0, IGATDynamicMixInfo mix = null, bool loop = false, bool randomStart = false, float startPosition = 0f, bool isImportant = false)
		{
			AudioSample sample = this.GetSample(sampleName);
			if (sample == null)
			{
				return null;
			}
			sample.Player = this.GAT;
			sample.Pitch = pitch;
			sample.Name = sampleName;
			sample.FadesIn = (fadeTime > 0.0);
			sample.FadeInDuration = fadeTime;
			sample.IsLooping = loop;
			sample.IsImportant = isImportant;
			sample.FixedPan = Mathf.Clamp01(pan);
			if (randomStart)
			{
				sample.SetStartPosition(UnityEngine.Random.value * sample.Duration);
			}
			else if (startPosition > 0f)
			{
				sample.SetStartPosition(startPosition * sample.Duration);
			}
			if (mix != null)
			{
				sample.DynamicMix = mix;
			}
			return sample;
		}

		// Token: 0x06002DE0 RID: 11744 RVA: 0x000D5418 File Offset: 0x000D3618
		public AudioSample PlaySample(string sampleName, float pan = 0.5f, float gain = 1f, float pitch = 1f, double fadeTime = 0.0, double dspTime = -1.0, bool loop = false, IGATDynamicMixInfo mix = null, bool stereo = false, bool randomStart = false, float startPosition = 0f, bool isImportant = false)
		{
			if (stereo)
			{
				this.PlaySample(sampleName + "_0", Mathf.Clamp01(pan - 0.5f), gain, pitch, fadeTime, dspTime, loop, mix, false, randomStart, startPosition, isImportant);
				this.PlaySample(sampleName + "_1", Mathf.Clamp01(pan + 0.5f), gain, pitch, fadeTime, dspTime, loop, mix, false, randomStart, startPosition, isImportant);
				return null;
			}
			AudioSample sample = this.PrepSample(sampleName, pan, pitch, fadeTime, mix, loop, randomStart, startPosition, isImportant);
			if (sample == null)
			{
				return null;
			}
			return this.Play(sample, dspTime, gain);
		}

		// Token: 0x06002DE1 RID: 11745 RVA: 0x000D54B4 File Offset: 0x000D36B4
		public AudioSample PlayDurational(string sampleName, float gain = 1f, float pan = 0.5f, double dspTime = -1.0, float length = 1f, float attack = 0f, float decay = 0f, float pitch = 1f, bool stereo = false, IGATDynamicMixInfo mix = null, bool randomStart = false, bool isImportant = false)
		{
			if (stereo)
			{
				this.PlayDurational(sampleName + "_0", gain, Mathf.Clamp01(pan - 0.5f), dspTime, length, attack, decay, pitch, false, mix, randomStart, isImportant);
				this.PlayDurational(sampleName + "_1", gain, Mathf.Clamp01(pan + 0.5f), dspTime, length, attack, decay, pitch, false, mix, randomStart, isImportant);
				return null;
			}
			AudioSample sample = this.PrepSample(sampleName, pan, pitch, (double)attack, mix, true, randomStart, 0f, isImportant);
			if (sample == null)
			{
				return null;
			}
			AudioSample result = this.Play(sample, dspTime, gain);
			if (attack + decay > length)
			{
				attack = Maf.Map(attack, 0f, attack + decay, 0f, length);
				decay = Maf.Map(decay, 0f, attack + decay, 0f, length);
			}
			double decayStartTime = ((dspTime < 0.0) ? AudioSystem.Instance.DspTime : dspTime) + (double)length - (double)decay;
			sample.GATRealTimeSample.ScheduleFadeOut(decayStartTime, (double)decay);
			return result;
		}

		// Token: 0x06002DE2 RID: 11746 RVA: 0x000D55C0 File Offset: 0x000D37C0
		private AudioSample Play(AudioSample sample, double dspTime, float gain)
		{
			gain = Mathf.Clamp01(gain);
			if (dspTime < 0.0)
			{
				sample.PlayPanned(gain);
			}
			else
			{
				sample.PlayScheduled(dspTime, gain);
			}
			return sample;
		}

		// Token: 0x06002DE3 RID: 11747 RVA: 0x000D55E8 File Offset: 0x000D37E8
		private AudioSample GetSample(string sampleName)
		{
			AudioSample result;
			try
			{
				result = AudioSystem.Instance.GetSample(AudioSystem.Instance.Database.GetSampleData(sampleName).GATData);
			}
			catch (KeyNotFoundException)
			{
				AudioSystem.Log.Warn("The sample '{0}' has a name that cannot be found. Is the sample stereo instead of mono?", new object[]
				{
					sampleName
				});
				result = null;
			}
			catch (NullReferenceException)
			{
				AudioSystem.Log.Warn("The sample '{0}' is null.", new object[]
				{
					sampleName
				});
				result = null;
			}
			return result;
		}

		// Token: 0x040027DC RID: 10204
		public static AudioPlayer UI;

		// Token: 0x040027DD RID: 10205
		public static AudioPlayer Default;

		// Token: 0x040027DE RID: 10206
		public GATPlayer GAT;
	}
}
