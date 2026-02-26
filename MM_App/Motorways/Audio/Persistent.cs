using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Audio
{
	// Token: 0x0200068A RID: 1674
	public class Persistent : ImmediateAudioModule
	{
		// Token: 0x06002E67 RID: 11879 RVA: 0x000D6E93 File Offset: 0x000D5093
		protected override void OnActivate()
		{
			FX.TogglePauseModePitch(false);
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x000D6E9C File Offset: 0x000D509C
		public override void UpdateModule()
		{
			if (Get.Loadout.MusicData.NoteWindow != null && !this.firedStartupChord && !Get.State.HasFlag(StateType.SkippingMenu))
			{
				this.StartupChord();
				this.firedStartupChord = true;
			}
		}

		// Token: 0x06002E69 RID: 11881 RVA: 0x000D6EEA File Offset: 0x000D50EA
		private void StartupChord()
		{
			Maf.Repeat(Get.Loadout.MusicData.NoteWindow.Count, delegate(int i)
			{
				AudioPlayer @default = AudioPlayer.Default;
				string sampleName = "LineCreated_" + Get.Loadout.MusicData.NoteWindow[i];
				float pan = UnityEngine.Random.Range(0f, 1f);
				@default.PlayDurational(sampleName, 0.55f * Note.GainFactor(Get.Loadout.MusicData.NoteWindow[i]), pan, -1.0, UnityEngine.Random.Range(3f, 5f), UnityEngine.Random.Range(1f, 3f), UnityEngine.Random.Range(2f, 4f), 1f, false, new FX.Modulator(new FX.Modulator.Portamento(0.9375, 1.0, (double)UnityEngine.Random.Range(0.1f, 0.33f)), Rando.Pick<FX.Modulator.Vibrato>(new FX.Modulator.Vibrato[]
				{
					null,
					new FX.Modulator.Vibrato(Rando.Range(10.0, 20.0, -1), Rando.Range(0.0, 0.01, -1), 1.0, (double)UnityEngine.Random.value)
				}), null), false, false);
			}, false);
		}

		// Token: 0x06002E6A RID: 11882 RVA: 0x000D6F28 File Offset: 0x000D5128
		protected override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnDrawModeToggle), AudioEventType.DrawMode, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnNightModeToggle), AudioEventType.NightMode, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnTransition), UIEventType.Transition, UIAudioProfile.None);
			this.EventListener.Add(new Action<AudioEvent>(this.OnLateGame), AudioEventType.LateGame, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnInGamePause), UIEventType.Click, UIAudioProfile.Pause | UIAudioProfile.Play | UIAudioProfile.FastForward);
			this.EventListener.Add(new Action<AudioEvent>(this.OnTheHour), AudioEventType.Pulse, -1);
		}

		// Token: 0x06002E6B RID: 11883 RVA: 0x000D6FF4 File Offset: 0x000D51F4
		private void OnTheHour(AudioEvent e)
		{
			if (AudioEnvironment.Instance == null)
			{
				return;
			}
			int connections = Get.ConnectedViewCount();
			MusicData mD = Get.Loadout.MusicData;
			if (!(mD is Menu) && connections > Persistent.Connections)
			{
				MusicData musicData = mD;
				int notePointer = musicData.NotePointer;
				musicData.NotePointer = notePointer + 1;
				mD.OnConnection();
			}
			Persistent.Connections = connections;
			if (!Get.State.HasFlag(StateType.MenuUpgrades) && Get.Clock.Hour > 6)
			{
				mD.OnHour();
				if (Get.Hour == 0)
				{
					mD.OnDay();
				}
				if (Get.Hour == 6)
				{
					mD.OnDawn();
					return;
				}
				if (Get.Hour == 19)
				{
					mD.OnDusk();
				}
			}
		}

		// Token: 0x06002E6C RID: 11884 RVA: 0x000D709F File Offset: 0x000D529F
		private void OnLateGame(AudioEvent e)
		{
			this.DrumSequencer(true);
		}

		// Token: 0x06002E6D RID: 11885 RVA: 0x000D70A8 File Offset: 0x000D52A8
		private void OnInGamePause(AudioEvent e)
		{
			UIAudioProfile uiaudioProfile = e.UIAudioProfile;
			if (uiaudioProfile == UIAudioProfile.Pause)
			{
				if (!Get.State.HasFlag(StateType.GamePaused))
				{
					AudioPlayer.UI.PlaySample("ui_clockSlow", 0.75f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
				}
				if (Get.Pulse.Scale == TimeScale.Single)
				{
					Get.Pulse.Scale = TimeScale.SingleSlow;
				}
				else
				{
					Get.Pulse.Scale = TimeScale.DoubleSlow;
				}
				Get.State |= StateType.GamePaused;
				Get.State &= ~StateType.GameActive;
				FX.TogglePauseModePitch(true);
				this.DrumSequencer(true);
				return;
			}
			if (uiaudioProfile == UIAudioProfile.Play)
			{
				if (Get.Pulse.Scale != TimeScale.Single)
				{
					AudioPlayer.UI.PlaySample((Get.Pulse.Scale == TimeScale.Double) ? "ui_clockSlow" : "ui_clockFast", 0.75f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
				}
				Get.Pulse.Scale = TimeScale.Single;
				Get.State |= StateType.GameActive;
				Get.State &= ~StateType.GamePaused;
				FX.TogglePauseModePitch(false);
				this.DrumSequencer(false);
				return;
			}
			if (uiaudioProfile != UIAudioProfile.FastForward)
			{
				return;
			}
			if (Get.Pulse.Scale != TimeScale.Double)
			{
				AudioPlayer.UI.PlaySample("ui_clockFast", 0.75f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
			}
			Get.Pulse.Scale = TimeScale.Double;
			Get.State |= StateType.GameActive;
			Get.State &= ~StateType.GamePaused;
			FX.TogglePauseModePitch(false);
			this.DrumSequencer(false);
		}

		// Token: 0x06002E6E RID: 11886 RVA: 0x000D7278 File Offset: 0x000D5478
		private void OnDrawModeToggle(AudioEvent e)
		{
			float duration = e.Duration;
			if (!e.Condition)
			{
				Get.State |= StateType.ModeDelete;
				Get.State &= ~StateType.ModeEdit;
				Get.Mixbus.InterpolateCutoffFreq(AudioMixbus.FilterType.Lowpass, 750f, 3f * duration);
				Get.Mixbus.InterpolateCutoffFreq(AudioMixbus.FilterType.Highpass, 100f, 3f * duration);
				return;
			}
			Get.State |= StateType.ModeEdit;
			Get.State &= ~StateType.ModeDelete;
			Get.Mixbus.InterpolateCutoffFreq(AudioMixbus.FilterType.Lowpass, 22000f, 3f * duration);
			Get.Mixbus.InterpolateCutoffFreq(AudioMixbus.FilterType.Highpass, 10f, 3f * duration);
		}

		// Token: 0x06002E6F RID: 11887 RVA: 0x000D732E File Offset: 0x000D552E
		private void OnNightModeToggle(AudioEvent e)
		{
			FX.ToggleNightMode(Get.State.HasFlag(StateType.ModeNight), false);
		}

		// Token: 0x06002E70 RID: 11888 RVA: 0x000D7350 File Offset: 0x000D5550
		private void OnTransition(AudioEvent e)
		{
			if (e.Screen == ScreenStack.MotorwaysScreen.Startup || e.PreviousScreen == ScreenStack.MotorwaysScreen.Photo)
			{
				return;
			}
			if (e.Screen == ScreenStack.MotorwaysScreen.CinematicMode || e.PreviousScreen == ScreenStack.MotorwaysScreen.CinematicMode)
			{
				return;
			}
			if (Get.State.HasFlag(StateType.MenuMain) && !Get.State.HasFlag(StateType.ModeNight))
			{
				FX.ToggleEcho(false);
			}
			bool enteringPauseFromGame = e.PreviousScreen == ScreenStack.MotorwaysScreen.InGame && e.Screen == ScreenStack.MotorwaysScreen.Pause;
			float d = (e.Screen == ScreenStack.MotorwaysScreen.MapSelect) ? 0.5f : (e.Duration * 0.7f);
			float a = (e.Screen == ScreenStack.MotorwaysScreen.MapSelect) ? d : (d * 0.3f);
			float g = (e.Screen == ScreenStack.MotorwaysScreen.MapSelect) ? 0.015000001f : 0.075f;
			float p = (e.Screen == ScreenStack.MotorwaysScreen.MapSelect) ? 2f : 1f;
			if (e.Condition && !Get.State.HasFlag(StateType.SkippingMenu) && !enteringPauseFromGame && d > 0f)
			{
				AudioPlayer.UI.PlayDurational("ui_transition", g, 0.5f, -1.0, d, a, d, p, true, null, true, false);
			}
			switch (e.Screen)
			{
			case ScreenStack.MotorwaysScreen.InGame:
				if (e.PreviousScreen == ScreenStack.MotorwaysScreen.Upgrade)
				{
					Get.Loadout.MusicData.OnRhythmUpdate(0);
					Get.Loadout.MusicData.OnNewWeek();
					MusicData musicData = Get.Loadout.MusicData;
					int commonTones = Get.MaxGroups - Get.AudibleGroups;
					int transposeBy = Rando.Pick<int>(Get.Loadout.MusicData.WeekendTranspositions);
					musicData.UpdateNoteWindow(commonTones, Get.Loadout.MusicData.WeekendQualityChangeChance, transposeBy, Get.Loadout.MusicData.WeekendKeyChangeChance, false);
					DestinationGroup.CityHocketTones.Limit(0.0, 0);
					DestinationGroup.CityIdleLoops.Limit(0.0, 0);
				}
				if (!Get.State.HasFlag(StateType.SkippingMenu))
				{
					this.PlayScreenChangeChord(false);
				}
				else
				{
					Get.State &= ~StateType.SkippingMenu;
				}
				this.LPFSweep(false);
				return;
			case ScreenStack.MotorwaysScreen.Pause:
				this.LPFSweep(true);
				return;
			case ScreenStack.MotorwaysScreen.GameOver:
				this.PlayScreenChangeChord(true);
				this.LPFSweep(false);
				this.DrumSequencer(false);
				return;
			case ScreenStack.MotorwaysScreen.Upgrade:
				if (Get.City.Rules.ScoringMode == ScoringMode.EfficiencyMilestones)
				{
					Persistent.UpgradeChord(-1.0);
				}
				this.LPFSweep(true);
				return;
			default:
				this.LPFSweep(false);
				Get.Mixbus.InterpolateCutoffFreq(AudioMixbus.FilterType.Highpass, 10f, 2f);
				return;
			}
		}

		// Token: 0x06002E71 RID: 11889 RVA: 0x000D75F1 File Offset: 0x000D57F1
		private void LPFSweep(bool on)
		{
			Get.Mixbus.InterpolateCutoffFreq(AudioMixbus.FilterType.Lowpass, on ? 900f : 22000f, on ? 1.5f : 2f);
		}

		// Token: 0x06002E72 RID: 11890 RVA: 0x000D761C File Offset: 0x000D581C
		private void DrumSequencer(bool play)
		{
			Get.Loadout.DrumSequencer.PauseMode = play;
			FX.ToggleEcho(play);
		}

		// Token: 0x06002E73 RID: 11891 RVA: 0x000D7634 File Offset: 0x000D5834
		public static void UpgradeChord(double dspTime = -1.0)
		{
			AudioPlayer @default = AudioPlayer.Default;
			string samplePrefix = "chordTone";
			List<string> noteWindow = Get.Loadout.MusicData.NoteWindow;
			float x = Settings.Gain.CHORD_WEEKOVER.x;
			float y = Settings.Gain.CHORD_WEEKOVER.y;
			@default.PlayChord(samplePrefix, noteWindow, dspTime, Get.Loadout.MusicData.ChordSpread(), x, y, 0f, 1f, 0f, 0.1f, Get.Loadout.MusicData.ChordSize(), Rando.FlipCoin(0.5f));
		}

		// Token: 0x06002E74 RID: 11892 RVA: 0x000D76B4 File Offset: 0x000D58B4
		private void PlayScreenChangeChord(bool isImportant = false)
		{
			AudioSample bass = Get.Loadout.MusicData.Bass;
			if (bass != null)
			{
				bass.FadeOutAndStop(0.5);
			}
			Get.Loadout.MusicData.Bass = AudioPlayer.Default.PlaySample("bass_" + Note.SCALE[Get.Loadout.MusicData.CurrentScale.Key], 0.5f, 0.5f, 1f, 0.5, -1.0, false, null, false, false, 0f, isImportant);
			new Persistent.Chord().Play(0, -1f, isImportant, 12, 1f, -1.0);
		}

		// Token: 0x04002857 RID: 10327
		private bool firedStartupChord;

		// Token: 0x04002858 RID: 10328
		public static int Connections;

		// Token: 0x0200068B RID: 1675
		public class Chord
		{
			// Token: 0x06002E76 RID: 11894 RVA: 0x000D777C File Offset: 0x000D597C
			public void Play(int polyphony = 0, float spread = -1f, bool isImportant = false, int transpose = 0, float gainAdjust = 1f, double dspStartTime = -1.0)
			{
				List<string> notes = Get.Loadout.MusicData.NoteWindow;
				if (transpose != 0)
				{
					notes = Note.Transpose(transpose, notes);
				}
				if (polyphony == 0)
				{
					polyphony = Get.Loadout.MusicData.ChordSize();
				}
				if (spread < 0f)
				{
					spread = Get.Loadout.MusicData.ChordSpread();
				}
				if (dspStartTime < 0.0)
				{
					dspStartTime = AudioPlayer.EarliestSchedulableTime;
				}
				Maf.Repeat(polyphony, delegate(int i)
				{
					AudioPlayer @default = AudioPlayer.Default;
					string sampleName = this.prefix + notes[i];
					float pan = Rando.m(-1);
					double dspTime = dspStartTime + (double)(spread * (float)i);
					@default.PlaySample(sampleName, pan, gainAdjust * 0.275f * Note.GainFactor(notes[i]), 1f, (double)Mathf.Lerp(0f, 0.5f, (float)((i == 0) ? 0 : (i / (notes.Count - 1)))), dspTime, false, new FX.Modulator(new FX.Modulator.Portamento(0.9375, 1.0, Rando.Range(0.1, 0.33, -1)), Rando.Pick<FX.Modulator.Vibrato>(new FX.Modulator.Vibrato[]
					{
						null,
						new FX.Modulator.Vibrato(Rando.Range(10.0, 20.0, -1), Rando.Range(0.0, 0.01, -1), 1.0, (double)UnityEngine.Random.value)
					}), new FX.Modulator.Tremolo(Rando.Range(0.25, 20.0, -1), UnityEngine.Random.Range(0f, 0.5f), (double)UnityEngine.Random.value)), false, false, 0f, isImportant);
				}, Rando.FlipCoin(0.5f));
			}

			// Token: 0x06002E77 RID: 11895 RVA: 0x000D7850 File Offset: 0x000D5A50
			public void PlaySingleRandom(int transpose = 0, float fadeTime = 0f, float gainAdjust = 1f)
			{
				string note = Note.Transpose(transpose, Rando.Pick<string>(Get.Loadout.MusicData.NoteWindow));
				AudioPlayer @default = AudioPlayer.Default;
				string sampleName = this.prefix + note;
				float pan = Rando.m(-1);
				double nextPulseTime = Clock.NextPulseTime;
				@default.PlaySample(sampleName, pan, gainAdjust * 0.275f * Note.GainFactor(note), 1f, (double)fadeTime, nextPulseTime, false, new FX.Modulator(new FX.Modulator.Portamento(0.9375, 1.0, Rando.Range(0.1, 0.33, -1)), Rando.Pick<FX.Modulator.Vibrato>(new FX.Modulator.Vibrato[]
				{
					null,
					new FX.Modulator.Vibrato(Rando.Range(10.0, 20.0, -1), Rando.Range(0.0, 0.01, -1), 1.0, (double)UnityEngine.Random.value)
				}), new FX.Modulator.Tremolo(Rando.Range(0.25, 20.0, -1), UnityEngine.Random.Range(0f, 0.5f), (double)UnityEngine.Random.value)), false, false, 0f, false);
			}

			// Token: 0x04002859 RID: 10329
			private string prefix = "chordTone_";
		}
	}
}
