using System;
using System.Collections.Generic;
using GAudio;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x020006C8 RID: 1736
	public class DestinationGroup : Playback
	{
		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x06002FC1 RID: 12225 RVA: 0x000DF0B9 File Offset: 0x000DD2B9
		public MusicData.NoteSequenceType SequenceStyleActual
		{
			get
			{
				return this._seqStyle;
			}
		}

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x06002FC2 RID: 12226 RVA: 0x000DF0C1 File Offset: 0x000DD2C1
		private float VibratoFrequencyLive
		{
			get
			{
				return Mathf.Lerp(this._vibrFreq, this._vibrFreqZ, Get.ZoomOutProgress);
			}
		}

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x06002FC3 RID: 12227 RVA: 0x000DF0D9 File Offset: 0x000DD2D9
		private float VibratoAmplitudeLive
		{
			get
			{
				return Mathf.Lerp(this._vibrAmp, this._vibrAmpZ, Get.ZoomOutProgress);
			}
		}

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x06002FC4 RID: 12228 RVA: 0x000DF0F1 File Offset: 0x000DD2F1
		private float TremoloFrequencyLive
		{
			get
			{
				return Mathf.Lerp(this._tremFreq, this._tremFreqZ, Get.ZoomOutProgress);
			}
		}

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x06002FC5 RID: 12229 RVA: 0x000DF109 File Offset: 0x000DD309
		private float TremoloAmplitudeLive
		{
			get
			{
				return Mathf.Lerp(this._tremAmp, this._tremAmpZ, Get.ZoomOutProgress);
			}
		}

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x06002FC6 RID: 12230 RVA: 0x000DF121 File Offset: 0x000DD321
		private int HocketCount
		{
			get
			{
				return Mathf.Min(this.Notes.Count, this._loopPoint);
			}
		}

		// Token: 0x06002FC7 RID: 12231 RVA: 0x000DF13C File Offset: 0x000DD33C
		private void RefreshViews()
		{
			this._views.Clear();
			if (this.Index < 0)
			{
				return;
			}
			AudioEnvironment environment = this.Environment;
			int? num;
			if (environment == null)
			{
				num = null;
			}
			else
			{
				List<List<HouseView>> houses = environment.Houses;
				num = ((houses != null) ? new int?(houses.Count) : null);
			}
			int? num2 = num;
			int index = this.Index;
			if (num2.GetValueOrDefault() > index & num2 != null)
			{
				this._views.AddRange(this.Environment.Houses[this.Index]);
			}
			AudioEnvironment environment2 = this.Environment;
			int? num3;
			if (environment2 == null)
			{
				num3 = null;
			}
			else
			{
				List<List<DestinationView>> destinations2 = environment2.Destinations;
				num3 = ((destinations2 != null) ? new int?(destinations2.Count) : null);
			}
			num2 = num3;
			index = this.Index;
			if (num2.GetValueOrDefault() > index & num2 != null)
			{
				List<DestinationView> destinations = this.Environment.Destinations[this.Index];
				for (int destinationIndex = 0; destinationIndex < destinations.Count; destinationIndex++)
				{
					DestinationView destination = destinations[destinationIndex];
					if (destination.NetworkConnectivity == NetworkConnectivity.Connected)
					{
						this._views.Add(destination);
					}
				}
			}
		}

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x06002FC8 RID: 12232 RVA: 0x000DF26A File Offset: 0x000DD46A
		public int ViewsCount
		{
			get
			{
				this.RefreshViews();
				return this._views.Count;
			}
		}

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x06002FC9 RID: 12233 RVA: 0x000DF280 File Offset: 0x000DD480
		public int ConnectedHouseCount
		{
			get
			{
				if (this.Index < 0)
				{
					return 0;
				}
				AudioEnvironment environment = this.Environment;
				int? num;
				if (environment == null)
				{
					num = null;
				}
				else
				{
					List<List<HouseView>> houses = environment.Houses;
					num = ((houses != null) ? new int?(houses.Count) : null);
				}
				int? num2 = num;
				int index = this.Index;
				if (num2.GetValueOrDefault() > index & num2 != null)
				{
					return this.Environment.Houses[this.Index].Count;
				}
				return 0;
			}
		}

		// Token: 0x06002FCA RID: 12234 RVA: 0x000DF304 File Offset: 0x000DD504
		public DestinationGroup(AudioEventFilter filter) : base(filter)
		{
			this.Index = filter.GroupIndex;
			this._prefix = Get.Loadout.MusicData.GroupPrefices[this.Index] + "_";
			this.SetLFOData();
		}

		// Token: 0x06002FCB RID: 12235 RVA: 0x000DF3C0 File Offset: 0x000DD5C0
		private void SetLFOData()
		{
			MusicData md = Get.Loadout.MusicData;
			this._tremFreq = (this._tremFreqZ = md.Tremolo.Freq.Range.Random(-1));
			this._tremAmp = (this._tremAmpZ = md.Tremolo.Amp.Range.Random(-1));
			this._vibrFreq = (this._vibrFreqZ = md.Vibrato.Freq.Range.Random(-1));
			this._vibrAmp = (this._vibrAmpZ = md.Vibrato.Amp.Range.Random(-1));
			if (md.TremoloZ != null)
			{
				this._tremFreqZ = md.TremoloZ.Freq.Range.Random(-1);
				this._tremAmpZ = md.TremoloZ.Amp.Range.Random(-1);
			}
			if (md.VibratoZ != null)
			{
				this._vibrFreqZ = md.VibratoZ.Freq.Range.Random(-1);
				this._vibrAmpZ = md.VibratoZ.Amp.Range.Random(-1);
			}
		}

		// Token: 0x06002FCC RID: 12236 RVA: 0x000DF4EC File Offset: 0x000DD6EC
		private void StopAndRemoveIdleLoopAt(int i)
		{
			double fadeTime = 3.5 * ((Get.Pulse.Scale == TimeScale.Double) ? 0.5 : 1.0);
			this.IdleLoops[i].FadeOutAndStop(fadeTime);
			DestinationGroup.CityIdleLoops.Remove(this.IdleLoops[i]);
			this.IdleLoops.RemoveAt(i);
		}

		// Token: 0x06002FCD RID: 12237 RVA: 0x000DF55C File Offset: 0x000DD75C
		private void ManageIdleLoops(List<string> noteNames, bool isMenu)
		{
			for (int i = this.IdleLoops.Count - 1; i >= 0; i--)
			{
				if (this.ConnectedHouseCount == 0 || !DestinationGroup.ContainsAnyNoteNames(this.IdleLoops[i].Name, noteNames))
				{
					this.StopAndRemoveIdleLoopAt(i);
				}
			}
			if (this.ConnectedHouseCount == 0)
			{
				return;
			}
			List<string> availableNoteNames = new List<string>(noteNames.Count);
			using (List<string>.Enumerator enumerator = noteNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string note = enumerator.Current;
					if (this.IdleLoops.TrueForAll((AudioSample idleLoop) => !idleLoop.Name.Contains(note)))
					{
						availableNoteNames.Add(note);
					}
				}
			}
			for (int j = 0; j < availableNoteNames.Count; j++)
			{
				float _pan;
				IGATDynamicMixInfo _mix;
				if (isMenu)
				{
					_pan = Rando.m(-1);
					string note2 = availableNoteNames[j];
					_mix = new DestinationGroup.IdleLoopMixMenu(new FX.Modulator.Vibrato((double)this.VibratoFrequencyLive, (double)this.VibratoAmplitudeLive, (double)Get.Loadout.MusicData.SamplePitchSign(), (double)UnityEngine.Random.value), new FX.Modulator.Tremolo((double)(this.TremoloFrequencyLive * 0.25f), this.TremoloAmplitudeLive * 2f, (double)UnityEngine.Random.value), note2);
				}
				else
				{
					_pan = ((j < this._disconnectedViews.Count - 1) ? this._disconnectedViews[j].Pan.x : Rando.m(-1));
					_mix = new DestinationGroup.IdleLoopMix(this._v, new FX.Modulator.Vibrato((double)this.VibratoFrequencyLive, (double)this.VibratoAmplitudeLive, (double)Get.Loadout.MusicData.SamplePitchSign(), (double)UnityEngine.Random.value), new FX.Modulator.Tremolo((double)(this.TremoloFrequencyLive * 0.25f), this.TremoloAmplitudeLive * 2f, (double)UnityEngine.Random.value), "C2");
				}
				AudioPlayer @default = AudioPlayer.Default;
				string sampleName = "LineCreated_" + availableNoteNames[j];
				double time = this.time;
				float pitch = Get.Loadout.MusicData.SamplePitchSign();
				AudioSample a = @default.PlaySample(sampleName, _pan, 1f, pitch, 2.0, time, true, _mix, false, false, 0f, false);
				this.IdleLoops.AddVoice(a);
				DestinationGroup.CityIdleLoops.AddVoice(a);
			}
		}

		// Token: 0x06002FCE RID: 12238 RVA: 0x000DF7B8 File Offset: 0x000DD9B8
		private static bool ContainsAnyNoteNames(string name, List<string> noteNames)
		{
			foreach (string note in noteNames)
			{
				if (name.Contains(note))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002FCF RID: 12239 RVA: 0x000DF810 File Offset: 0x000DDA10
		public override void OnActivate()
		{
			this.UpdateLoopPoint();
			DestinationGroup.DivvyUpNoteWindow();
			this.LatchToOffsetAndStartPulsing();
		}

		// Token: 0x06002FD0 RID: 12240 RVA: 0x000DF824 File Offset: 0x000DDA24
		public override void OnDeactivate()
		{
			List<AudioSample> cityIdleLoops = DestinationGroup.CityIdleLoops;
			if (cityIdleLoops != null)
			{
				cityIdleLoops.ForEach(delegate(AudioSample x)
				{
					if (x != null)
					{
						x.FadeOutAndStop(3.0);
					}
				});
			}
			List<AudioSample> cityHocketTones = DestinationGroup.CityHocketTones;
			if (cityHocketTones != null)
			{
				cityHocketTones.ForEach(delegate(AudioSample x)
				{
					if (x != null)
					{
						x.FadeOutAndStop(3.0);
					}
				});
			}
			DestinationGroup.CityIdleLoops.Clear();
			DestinationGroup.CityHocketTones.Clear();
			this.HocketTones.Clear();
			this.IdleLoops.Clear();
		}

		// Token: 0x06002FD1 RID: 12241 RVA: 0x000DF8B9 File Offset: 0x000DDAB9
		public override void Update()
		{
			this.IdleLoops.ForEach(delegate(AudioSample x)
			{
				if (x != null)
				{
					IGATDynamicMixInfo dynamicMix = x.DynamicMix;
					if (dynamicMix == null)
					{
						return;
					}
					dynamicMix.OnGameTick();
				}
			});
			this.UpdateLoopPoint();
		}

		// Token: 0x06002FD2 RID: 12242 RVA: 0x000DF8EC File Offset: 0x000DDAEC
		protected override void OnPulse()
		{
			this.RefreshViews();
			this._disconnectedViews.Clear();
			if (this.Index < 0 || this.Index >= this.Environment.Disconnecteds.Count)
			{
				Diagnostics.FailAssert("Index {0} is OutOfRange of Environment.Disconnecteds Count: {1}", new object[]
				{
					this.Index,
					this.Environment.Disconnecteds.Count
				});
				return;
			}
			this._disconnectedViews.AddRange(this.Environment.Disconnecteds[this.Index]);
			if (this.Notes.Count == 0 || this._views.Count == 0)
			{
				return;
			}
			MusicData musicData = Get.Loadout.MusicData;
			if (Get.Loadout.Id != "menu")
			{
				this.ManageIdleLoops(this.Notes, false);
			}
			else if (this.Index == 0)
			{
				this.ManageIdleLoops(Get.Loadout.MusicData.NoteWindow, true);
			}
			if (this.HocketCount == 0)
			{
				return;
			}
			this._v = this._views[this._dest_i % this._views.Count];
			int ii = this._dest_i;
			while (this._v is DestinationView && ((DestinationView)this._v).PinCount == 0)
			{
				this._v = this._views[this._dest_i % this._views.Count];
				if (this._dest_i - ii >= this._views.Count)
				{
					break;
				}
				this._dest_i++;
			}
			this._dest_i++;
			this.Note_i = Maf.FloorMod(this._step, this.HocketCount);
			if (this.Note_i < 0 || this.Note_i > this.Notes.Count - 1)
			{
				Diagnostics.FailAssert("Note_i index OutOfBounds. Note_i: {0} - Notes.Count {1}. Clamping Note_i to within bounds.", new object[]
				{
					this.Note_i,
					this.Notes.Count
				});
				this.Note_i = Mathf.Clamp(this.Note_i, 0, this.Notes.Count - 1);
			}
			if (Diagnostics.Verify(this.Index < musicData.NoteSequenceStyles.Count, "Index OutOfBounds. Does the map have more Indexes than defined NotSequenceStyles?"))
			{
				this._seqStyle = musicData.NoteSequenceStyles[this.Index];
			}
			if (this._seqStyle == MusicData.NoteSequenceType.AutoReroll)
			{
				this._seqStyle = Rando.EnumValue<MusicData.NoteSequenceType>(1, this.Index + musicData.NotePointer);
			}
			switch (this._seqStyle)
			{
			case MusicData.NoteSequenceType.Backward:
				this._step--;
				goto IL_357;
			case MusicData.NoteSequenceType.PingPong:
				if (!this._retrograde && this.Note_i == this.HocketCount - 1)
				{
					this._retrograde = true;
				}
				else if (this._retrograde && this.Note_i == 0)
				{
					this._retrograde = false;
				}
				this._step += (this._retrograde ? -1 : 1);
				goto IL_357;
			case MusicData.NoteSequenceType.Seeded:
			{
				List<string> seedNotes = new List<string>(this.Notes);
				seedNotes.Shuffle(null, musicData.NotePointer);
				this.Note_i = seedNotes.FindIndex((string x) => x == this.Notes[this.Note_i]);
				break;
			}
			case MusicData.NoteSequenceType.Chaotic:
				this._step = UnityEngine.Random.Range(0, 100);
				break;
			}
			this._step++;
			IL_357:
			this._v_gain = ((this._v is DestinationView) ? Twerp.Ease.Out(Mathf.Lerp(0.17f, 0.4f, Mathf.Clamp01((float)((DestinationView)this._v).PinCount / (float)this._maxDemand)), 2) : 0.17f);
			this._v_gain *= Note.GainFactor(this.Notes[this.Note_i]);
			if (Get.Loadout.Id == "menu")
			{
				this._v_gain = Mathf.Min(this._v_gain, 0.060000002f);
			}
			int polyphony = Mathf.Min(musicData.LocalPolyphony, this.Notes.Count);
			this.HocketTones.Limit(musicData.LocalFadeOut, polyphony);
			polyphony = Mathf.Min(musicData.GlobalPolyphony, this.Notes.Count);
			DestinationGroup.CityHocketTones.Limit((musicData.SamplePitchSign() < 0f) ? 2.0 : musicData.GlobalFadeOut, (musicData.SamplePitchSign() < 0f) ? 4 : polyphony);
			float atten = (Get.Loadout.Id == "menu") ? this._v.GetAttenuation(false, 33f) : this._v.Attenuation;
			float pSign = musicData.SamplePitchSign();
			bool flag = musicData.PortamentoZ != null;
			float sPitch = flag ? Vector2.Lerp(musicData.Portamento.StartingPitch.Range, musicData.PortamentoZ.StartingPitch.Range, Get.ZoomOutProgress).Random(-1) : musicData.Portamento.StartingPitch.Range.Random(-1);
			float startPitch = pSign * sPitch;
			float pTime = flag ? Vector2.Lerp(musicData.Portamento.Time.Range, musicData.PortamentoZ.Time.Range, Get.ZoomOutProgress).Random(-1) : musicData.Portamento.Time.Range.Random(-1);
			pTime *= ((Get.Pulse.Scale == TimeScale.Double) ? 0.5f : 1f);
			AudioSample s = AudioPlayer.Default.PlaySample(this._prefix + this.Notes[this.Note_i], this._v.Pan.x, atten * this._v_gain, pSign, musicData.FadeInTime(), this.time, false, new FX.Modulator(new FX.Modulator.Portamento((double)startPitch, (double)pSign, (double)pTime), new FX.Modulator.Vibrato((double)this.VibratoFrequencyLive, (double)this.VibratoAmplitudeLive, (double)pSign, (double)UnityEngine.Random.value), new FX.Modulator.Tremolo((double)this.TremoloFrequencyLive, this.TremoloAmplitudeLive, (double)UnityEngine.Random.value)), false, false, 0f, false);
			this.HocketTones.AddVoice(s);
			DestinationGroup.CityHocketTones.AddVoice(s);
			while (this.IdleLoops.Count > this.Notes.Count - this.HocketCount)
			{
				this.StopAndRemoveIdleLoopAt(this.IdleLoops.Count - 1);
			}
		}

		// Token: 0x06002FD3 RID: 12243 RVA: 0x000DFF54 File Offset: 0x000DE154
		public override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnEvents), AudioEventType.HouseSpawned | AudioEventType.DestinationActivated | AudioEventType.DestinationDemanded | AudioEventType.VehicleFulfillsDemand | AudioEventType.DestinationConnectedToNetwork | AudioEventType.HouseConnectedToNetwork | AudioEventType.DestinationMutated, this.Index);
			this.EventListener.Add(new Action<AudioEvent>(this.OnAudioMinimized), AudioEventType.AudioMinimized, -1);
		}

		// Token: 0x06002FD4 RID: 12244 RVA: 0x000DFFA8 File Offset: 0x000DE1A8
		private void OnAudioMinimized(AudioEvent e)
		{
			this.OnDeactivate();
		}

		// Token: 0x06002FD5 RID: 12245 RVA: 0x000DFFB0 File Offset: 0x000DE1B0
		public void OnEvents(AudioEvent e)
		{
			DestinationGroup.<>c__DisplayClass59_0 CS$<>8__locals1 = new DestinationGroup.<>c__DisplayClass59_0();
			CS$<>8__locals1.e = e;
			CS$<>8__locals1.<>4__this = this;
			DestinationView d = CS$<>8__locals1.e.Destination;
			CS$<>8__locals1.window = Get.Loadout.MusicData.NoteWindow;
			AudioEventType type = CS$<>8__locals1.e.Type;
			if (type <= AudioEventType.VehicleFulfillsDemand)
			{
				if (type != AudioEventType.DestinationActivated)
				{
					if (type == AudioEventType.DestinationDemanded)
					{
						string noteName = (this.Notes.Count < 1) ? Get.Loadout.MusicData.NoteWindow.SafeGet(-1) : this.Notes.SafeGet(this.Note_i);
						bool isImportant = FeatureToggle.IsFeatureEnabled(Feature.SmallPinSFXWithMinimalSoundscape);
						AudioPlayer @default = AudioPlayer.Default;
						string sampleName = "StationAdded_" + noteName;
						float x = d.Pan.x;
						double dspTime = Get.Pulse.HybridTime(this.Module);
						@default.PlaySample(sampleName, x, Note.GainFactor(noteName) * d.Attenuation * Twerp.Ease.Out(Mathf.Lerp(Settings.Gain.DESTINATION_DEMANDED.x, Settings.Gain.DESTINATION_DEMANDED.y, (float)d.PinCount / (float)this._maxDemand), 3), 2f, 0.0, dspTime, false, null, false, false, 0f, isImportant);
						return;
					}
					if (type != AudioEventType.VehicleFulfillsDemand)
					{
						return;
					}
					AudioPlayer.UI.PlaySample("PinFulfilled-01", d.Pan.x, d.Attenuation * Twerp.Ease.Out(Mathf.Lerp(0.05f, 0.2f, (float)d.PinCount / (float)this._maxDemand), 2), 1.33f, 0.0, AudioPlayer.EarliestSchedulableTime + 0.25, false, null, false, false, 0f, false);
					return;
				}
				else if (!(Get.Loadout.MusicData is Menu))
				{
					Get.Loadout.MusicData.OnDestinationActivated(this.Index);
					AudioPlayer.UI.PlaySample("StationSpawn_" + Get.Loadout.MusicData.Timbres[d.groupIndex], d.Pan.x, d.GetAttenuation(false, 25f) * 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
					Get.Loadout.MusicData.UpdateNoteWindow(Get.MaxGroups - 2, 1f, 0, 0f, false);
					AudioSample bass = Get.Loadout.MusicData.Bass;
					if (bass != null)
					{
						bass.FadeOutAndStop(0.5);
					}
					Get.Loadout.MusicData.Bass = AudioPlayer.Default.PlaySample("bass_" + Note.SCALE[Get.Loadout.MusicData.CurrentScale.Key], CS$<>8__locals1.e.Destination.Pan.x, CS$<>8__locals1.e.Destination.Attenuation * 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
					Maf.Repeat(CS$<>8__locals1.window.Count, delegate(int i)
					{
						AudioPlayer.Default.PlaySample("chordTone_" + CS$<>8__locals1.window[i], CS$<>8__locals1.e.Destination.Pan.x, Note.GainFactor(CS$<>8__locals1.window[i]) * CS$<>8__locals1.e.Destination.Attenuation * (CS$<>8__locals1.<>4__this.Notes.Contains(CS$<>8__locals1.window[i]) ? 0.33f : 0.15f), 1f, 0.0, AudioPlayer.EarliestSchedulableTime + (double)i * 0.25 / (double)CS$<>8__locals1.window.Count, false, null, false, false, 0f, false);
					}, Rando.FlipCoin(0.5f));
					return;
				}
			}
			else if (type != AudioEventType.DestinationConnectedToNetwork)
			{
				if (type != AudioEventType.HouseConnectedToNetwork)
				{
					if (type == AudioEventType.DestinationMutated)
					{
						AudioPlayer.UI.PlaySample("interchange_placed", d.Pan.x, d.GetAttenuation(false, 25f) * 0.7f, 0.75f, 0.0, -1.0, false, null, false, false, 0f, true);
						double delay = 0.6;
						double delayedTriggerTime = AudioPlayer.EarliestSchedulableTime + delay;
						AudioPlayer.UI.PlaySample("StationSpawn_" + Get.Loadout.MusicData.Timbres[d.groupIndex], d.Pan.x, d.GetAttenuation(false, 25f) * 1f, 1f, 0.0, delayedTriggerTime, false, null, false, false, 0f, true);
						Get.Loadout.MusicData.UpdateNoteWindow(Get.MaxGroups - 2, 1f, 0, 0f, false);
						AudioSample bass2 = Get.Loadout.MusicData.Bass;
						if (bass2 != null)
						{
							bass2.FadeOutAndStop(0.5);
						}
						Get.Loadout.MusicData.Bass = AudioPlayer.Default.PlaySample("bass_" + Note.SCALE[Get.Loadout.MusicData.CurrentScale.Key], CS$<>8__locals1.e.Destination.Pan.x, CS$<>8__locals1.e.Destination.Attenuation * 1f, 1f, 0.0, delayedTriggerTime, false, null, false, false, 0f, true);
						int repeats = 3;
						Maf.Repeat(repeats, delegate(int i)
						{
							AudioPlayer.Default.PlaySample("chordTone_" + Note.Transpose(12, CS$<>8__locals1.window[i % CS$<>8__locals1.window.Count]), CS$<>8__locals1.e.Destination.Pan.x, Note.GainFactor(CS$<>8__locals1.window[i]) * CS$<>8__locals1.e.Destination.Attenuation * (CS$<>8__locals1.<>4__this.Notes.Contains(CS$<>8__locals1.window[i]) ? 0.33f : 0.15f), 1f, 0.0, delayedTriggerTime + (double)i * 1.25 / (double)repeats, false, null, false, false, 0f, true);
						}, Rando.FlipCoin(0.5f));
						return;
					}
				}
				else if (CS$<>8__locals1.e.Condition)
				{
					this.SetLFOData();
					Get.Loadout.MusicData.OnHouseConnected(CS$<>8__locals1.e.GroupIndex);
				}
			}
			else if (CS$<>8__locals1.e.Condition)
			{
				Get.Loadout.MusicData.OnRhythmUpdate(CS$<>8__locals1.e.GroupIndex);
				Get.Loadout.MusicData.OnDestinationConnected(CS$<>8__locals1.e.GroupIndex);
				return;
			}
		}

		// Token: 0x06002FD6 RID: 12246 RVA: 0x000E05A6 File Offset: 0x000DE7A6
		private void LatchToOffsetAndStartPulsing()
		{
			if (!this._doOnce)
			{
				((SubPulseModule)this.Module.Pulse).PrepOffset(false);
				this._doOnce = true;
			}
		}

		// Token: 0x06002FD7 RID: 12247 RVA: 0x000E05D0 File Offset: 0x000DE7D0
		private void UpdateLoopPoint()
		{
			this._loopPoint = Mathf.Max(0, this.Environment.GetPinCount(this.Index) - this.Environment.GetDisconnectedCount(this.Index));
			if (this._loopPoint > 0 && this._loopPointPrev > 0 && this._loopPoint != this._loopPointPrev)
			{
				this._step += Maf.FloorMod(this._step + (this._retrograde ? 1 : -1), this._loopPointPrev) - Maf.FloorMod(this._step + (this._retrograde ? 1 : -1), this._loopPoint);
			}
			this._loopPointPrev = this._loopPoint;
		}

		// Token: 0x06002FD8 RID: 12248 RVA: 0x000E0684 File Offset: 0x000DE884
		public static void DivvyUpNoteWindow()
		{
			if (Get.Loadout.DestinationGroups.Count < 1)
			{
				Dbug.Log.Info("Note Divvy : No Groups !", Array.Empty<object>());
				return;
			}
			if (Get.Loadout.DestinationGroups.Count < 2)
			{
				Dbug.Log.Info("Note Divvy : Only One Group, Giving Them All The Notes !", Array.Empty<object>());
				Get.Loadout.DestinationGroups[0].Notes.Clear();
				Get.Loadout.DestinationGroups[0].Notes.AddRange(Get.Loadout.MusicData.NoteWindow);
				return;
			}
			int seed = Get.MaxGroups;
			int audibleGroups = AudioEnvironment.Instance.GetAudibleGroups();
			List<string> availableNotes = new List<string>(Get.Loadout.MusicData.NoteWindow);
			int maxNotesPerGroup = availableNotes.Count / Mathf.Max(1, audibleGroups);
			Dbug.Log.Info("Note Divvy: Destination Groups: {0}, Notes to Distribute: {1}", new object[]
			{
				audibleGroups,
				availableNotes.Count
			});
			foreach (DestinationGroup d in Get.Loadout.DestinationGroups)
			{
				d.Notes.Clear();
				if (d.ViewsCount == 0)
				{
					Dbug.Log.Info(string.Format("Note Divvy: Group {0} is empty. Continuing...", d.Index), Array.Empty<object>());
				}
				else
				{
					int notesToTake = Mathf.Max(1, Mathf.Min(maxNotesPerGroup, d.ViewsCount));
					Dbug.Log.Info(string.Format("Note Divvy: Loop Point is {0}, notesToTake is {1}, availableNotes is {2}", d.ViewsCount, notesToTake, availableNotes.Count), Array.Empty<object>());
					for (int p = 0; p < notesToTake; p++)
					{
						if (availableNotes.Count == 0)
						{
							Dbug.Log.Warn("Note Divvy: Ran out of available notes!", Array.Empty<object>());
							break;
						}
						int randomI = Rando.Index<string>(availableNotes, seed);
						d.Notes.Add(availableNotes[randomI]);
						availableNotes.RemoveAt(randomI);
					}
					Dbug.Log.Info(string.Format("Note Divvy: Step 1. Divvy Proportionally : Destination Group {0} Gets {1} Notes. Available Notes Left: {2}", d.Index, d.Notes.Count, availableNotes.Count), Array.Empty<object>());
				}
			}
			foreach (DestinationGroup d2 in Get.Loadout.DestinationGroups)
			{
				if (d2.ViewsCount != 0 && d2.Notes.Count < d2.ViewsCount)
				{
					if (availableNotes.Count == 0)
					{
						goto IL_3E3;
					}
					int random_i = Rando.Index<string>(availableNotes, seed);
					d2.Notes.Add(availableNotes[random_i]);
					availableNotes.RemoveAt(random_i);
					Dbug.Log.Info("Note Divvy: Step 2. Divvy Remaining Notes: Destination {0} has less notes than Views. Adding a Note. Available Notes Left: {1}", new object[]
					{
						d2.Index,
						availableNotes.Count
					});
				}
			}
			if (availableNotes.Count > 0)
			{
				DestinationGroup gMax = Get.Loadout.GetDestinationGroup(0);
				int maxViews = 0;
				foreach (DestinationGroup g in Get.Loadout.DestinationGroups)
				{
					if (g.ViewsCount > maxViews)
					{
						gMax = g;
						maxViews = g.ViewsCount;
					}
				}
				int pitchesLeft = availableNotes.Count;
				Dbug.Log.Info("Note Divvy: Step 3. Lingering Notes Left. Giving all {0} remaining notes to Destination Group {1}", new object[]
				{
					availableNotes.Count,
					gMax.Index
				});
				for (int i = 0; i < pitchesLeft; i++)
				{
					int random_i2 = Rando.Index<string>(availableNotes, seed);
					gMax.Notes.Add(availableNotes[random_i2]);
					availableNotes.RemoveAt(random_i2);
				}
			}
			IL_3E3:
			Diagnostics.Verify(availableNotes.Count == 0, "Audio | Note Divvy: {0} Notes Failed to be Distributed.", availableNotes.Count);
		}

		// Token: 0x0400292F RID: 10543
		public int Index;

		// Token: 0x04002930 RID: 10544
		public readonly List<string> Notes = new List<string>();

		// Token: 0x04002931 RID: 10545
		public List<AudioSample> IdleLoops = new List<AudioSample>();

		// Token: 0x04002932 RID: 10546
		public int Note_i;

		// Token: 0x04002933 RID: 10547
		public readonly int Seed = Rando.Range(0, 10000, -1);

		// Token: 0x04002934 RID: 10548
		public static List<AudioSample> CityIdleLoops = new List<AudioSample>();

		// Token: 0x04002935 RID: 10549
		public static List<AudioSample> CityHocketTones = new List<AudioSample>();

		// Token: 0x04002936 RID: 10550
		private readonly List<IAudioView> _views = new List<IAudioView>();

		// Token: 0x04002937 RID: 10551
		private float _tremFreq;

		// Token: 0x04002938 RID: 10552
		private float _tremFreqZ;

		// Token: 0x04002939 RID: 10553
		private float _tremAmp;

		// Token: 0x0400293A RID: 10554
		private float _tremAmpZ;

		// Token: 0x0400293B RID: 10555
		private float _vibrFreq;

		// Token: 0x0400293C RID: 10556
		private float _vibrFreqZ;

		// Token: 0x0400293D RID: 10557
		private float _vibrAmp;

		// Token: 0x0400293E RID: 10558
		private float _vibrAmpZ;

		// Token: 0x0400293F RID: 10559
		private int _maxDemand = 10;

		// Token: 0x04002940 RID: 10560
		private int _loopPoint = 1;

		// Token: 0x04002941 RID: 10561
		private int _loopPointPrev = 1;

		// Token: 0x04002942 RID: 10562
		private float _v_gain = 0.17f;

		// Token: 0x04002943 RID: 10563
		private string _prefix;

		// Token: 0x04002944 RID: 10564
		private int _step;

		// Token: 0x04002945 RID: 10565
		private int _dest_i;

		// Token: 0x04002946 RID: 10566
		private MusicData.NoteSequenceType _seqStyle;

		// Token: 0x04002947 RID: 10567
		private bool _retrograde;

		// Token: 0x04002948 RID: 10568
		private bool _doOnce;

		// Token: 0x04002949 RID: 10569
		private IAudioView _v;

		// Token: 0x0400294A RID: 10570
		private readonly List<IAudioView> _disconnectedViews = new List<IAudioView>();

		// Token: 0x0400294B RID: 10571
		private List<AudioSample> HocketTones = new List<AudioSample>();

		// Token: 0x020006C9 RID: 1737
		private class IdleLoopMix : FX.Modulator
		{
			// Token: 0x06002FDB RID: 12251 RVA: 0x000E0B0F File Offset: 0x000DED0F
			public IdleLoopMix(IAudioView view, FX.Modulator.Vibrato vibrato, FX.Modulator.Tremolo tremolo, string note = "C2") : base(null, vibrato, tremolo)
			{
				this.View = view;
				this.note = note;
			}

			// Token: 0x06002FDC RID: 12252 RVA: 0x000E0B29 File Offset: 0x000DED29
			public override void OnGameTick()
			{
				IAudioView view = this.View;
				this.pan = ((view != null) ? view.Pan.x : 0.5f);
			}

			// Token: 0x1700080A RID: 2058
			// (get) Token: 0x06002FDD RID: 12253 RVA: 0x000E0B4C File Offset: 0x000DED4C
			public override float Pan
			{
				get
				{
					return this.pan;
				}
			}

			// Token: 0x1700080B RID: 2059
			// (get) Token: 0x06002FDE RID: 12254 RVA: 0x000E0B54 File Offset: 0x000DED54
			public override float Gain
			{
				get
				{
					return base.Gain * 0.125f * Note.GainFactor(this.note);
				}
			}

			// Token: 0x0400294C RID: 10572
			public IAudioView View;

			// Token: 0x0400294D RID: 10573
			private string note;

			// Token: 0x0400294E RID: 10574
			private float pan;
		}

		// Token: 0x020006CA RID: 1738
		private class IdleLoopMixMenu : FX.Modulator
		{
			// Token: 0x06002FDF RID: 12255 RVA: 0x000E0B6E File Offset: 0x000DED6E
			public IdleLoopMixMenu(FX.Modulator.Vibrato vibrato, FX.Modulator.Tremolo tremolo, string note = "C2") : base(null, vibrato, tremolo)
			{
				this.note = note;
			}

			// Token: 0x06002FE0 RID: 12256 RVA: 0x000E0B9A File Offset: 0x000DED9A
			public override void OnGameTick()
			{
				this.attenuation = Get.Camera.GameCamera.GetAttenuationFromWorld(this.center, false, 500f);
			}

			// Token: 0x1700080C RID: 2060
			// (get) Token: 0x06002FE1 RID: 12257 RVA: 0x000E0BBD File Offset: 0x000DEDBD
			public override float Gain
			{
				get
				{
					return base.Gain * 0.09375f * Note.GainFactor(this.note) * this.attenuation;
				}
			}

			// Token: 0x0400294F RID: 10575
			private float attenuation;

			// Token: 0x04002950 RID: 10576
			private string note;

			// Token: 0x04002951 RID: 10577
			private Vector3 center = new Vector3(-259f, 100.5f, 30f);
		}
	}
}
