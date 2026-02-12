using System;
using GAudio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Motorways.Audio
{
	// Token: 0x0200068E RID: 1678
	public class SFX : ImmediateAudioModule
	{
		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x06002E7E RID: 11902 RVA: 0x000D7C15 File Offset: 0x000D5E15
		public static float MouseSpeed
		{
			get
			{
				return Maf.Normalize(SFX.mouseSpeed, 0f, 100f, true);
			}
		}

		// Token: 0x06002E7F RID: 11903 RVA: 0x000022F5 File Offset: 0x000004F5
		protected override void OnActivate()
		{
		}

		// Token: 0x06002E80 RID: 11904 RVA: 0x000D7C2C File Offset: 0x000D5E2C
		public override void UpdateModule()
		{
			SFX.mousePos = Input.mousePosition;
			SFX.mouseSpeed = (SFX.mousePos - SFX.mousePosPrev).magnitude;
			SFX.mousePosPrev = SFX.mousePos;
		}

		// Token: 0x06002E81 RID: 11905 RVA: 0x000D7C70 File Offset: 0x000D5E70
		protected override void AddEventListeners()
		{
			this.EventListener.Add(new Action<AudioEvent>(this.OnHover), UIEventType.MouseOver, UIAudioProfile.None);
			this.EventListener.Add(new Action<AudioEvent>(this.OnClick), UIEventType.Click, UIAudioProfile.None);
			this.EventListener.Add(new Action<AudioEvent>(this.OnCheckbox), UIEventType.CheckboxChecked | UIEventType.CheckboxUnchecked, UIAudioProfile.None);
			this.EventListener.Add(new Action<AudioEvent>(this.OnDrawModeToggle), UIEventType.Click, UIAudioProfile.DrawModeToggle);
			this.EventListener.Add(new Action<AudioEvent>(this.OnUpgrade), AudioEventType.UpgradeDragged | AudioEventType.UpgradeReleased | AudioEventType.UpgradeOver | AudioEventType.UpgradeDragSnap, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnTextMessageShown), AudioEventType.TextMessageShown, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnFocusZoom), UIEventType.FocusZoomIn | UIEventType.FocusZoomOut, UIAudioProfile.None);
			this.EventListener.Add(new Action<AudioEvent>(this.OnUpgradePlaced), AudioEventType.UpgradePlaced, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnElectiveUpgradeAvailable), AudioEventType.ElectiveUpgradeAvailable, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnElectiveUpgradePulse), AudioEventType.ElectiveUpgradePulse, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnCreativeModeEditPanelButtonAppears), AudioEventType.CreativeModeEditPanelButtonAppears, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnLogoPin), AudioEventType.LogoPinAppear | AudioEventType.LogoPinDisappear, -1);
			this.EventListener.Add(new Action<AudioEvent>(this.OnMapUnlock), AudioEventType.UnlockMap, -1);
		}

		// Token: 0x06002E82 RID: 11906 RVA: 0x000D7E08 File Offset: 0x000D6008
		private void OnCreativeModeEditPanelButtonAppears(AudioEvent e)
		{
			string note = Get.Loadout.MusicData.NoteWindow.SafeGet(this.hoverCounter);
			note = Note.Transpose(24, note);
			float i = UnityEngine.Random.Range(0.1f, 0.5f);
			AudioPlayer.Default.PlayDurational("Boop_3_" + note, Note.GainFactor(note) * Mathf.Lerp(Settings.Gain.UI_CHECKBOX_HOVER.x, Settings.Gain.UI_CHECKBOX_HOVER.y, SFX.MouseSpeed), 0.5f, -1.0, i, 0f, i, 1f, false, new FX.Modulator(new FX.Modulator.Portamento((double)Rando.Range(0.5f, 1f, -1), 1.0, (double)Rando.Range(0f, 0.1f, -1)), null, null), false, false);
			this.hoverCounter++;
		}

		// Token: 0x06002E83 RID: 11907 RVA: 0x000D7EE8 File Offset: 0x000D60E8
		private void OnElectiveUpgradeAvailable(AudioEvent e)
		{
			AudioPlayer.UI.PlaySample("elective-upgrade.available", 0.66f, 0.33f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E84 RID: 11908 RVA: 0x000D7F30 File Offset: 0x000D6130
		private void OnElectiveUpgradePulse(AudioEvent e)
		{
			AudioPlayer.UI.PlaySample("elective-upgrade.attract-mode", 0.66f, 0.225f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E85 RID: 11909 RVA: 0x000D7F78 File Offset: 0x000D6178
		private void OnMapUnlock(AudioEvent e)
		{
			AudioPlayer.UI.PlaySample("interchange_placed", 0.5f, 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E86 RID: 11910 RVA: 0x000D7FC0 File Offset: 0x000D61C0
		private void OnLogoPin(AudioEvent e)
		{
			if (e.Type == AudioEventType.LogoPinAppear)
			{
				AudioPlayer.UI.PlaySample("PinAppears-01", 0.5f, 0.1f, 1.33f, 0.0, -1.0, false, null, false, false, 0f, false);
				return;
			}
			AudioPlayer.UI.PlaySample("PinFulfilled-01", 0.5f, 0.1f, 1.33f, 0.0, -1.0, false, null, false, false, 0f, false);
		}

		// Token: 0x06002E87 RID: 11911 RVA: 0x000D8054 File Offset: 0x000D6254
		private void OnUpgradePlaced(AudioEvent e)
		{
			if (Get.Game.Scope.Get<ScreenStack>().AreAnyScreensTransitioning)
			{
				return;
			}
			AudioPlayer.UI.PlaySample("UpgradeReleased", e.Pan, 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E88 RID: 11912 RVA: 0x000D80B4 File Offset: 0x000D62B4
		private void OnFocusZoom(AudioEvent e)
		{
			float duration = 1f;
			float freq = 0.5f;
			float amp = Settings.PITCH_BOING_IN_PLACE.Random(-1);
			UIEventType uieventType = e.UIEventType;
			if (uieventType == UIEventType.FocusZoomIn)
			{
				AudioPlayer.UI.PlaySample("FocusZoomIn", 0.5f, 0.25f, 0.75f, 0.0, -1.0, false, null, false, false, 0f, true);
				Get.Mixbus.BoingPitchInPlace(duration, freq, amp, 0f);
				return;
			}
			if (uieventType != UIEventType.FocusZoomOut)
			{
				return;
			}
			AudioPlayer.UI.PlaySample("FocusZoomOut", 0.5f, 0.25f, 0.75f, 0.0, -1.0, false, null, false, false, 0f, true);
			Get.Mixbus.BoingPitchInPlace(duration, freq, amp, 0.5f);
		}

		// Token: 0x06002E89 RID: 11913 RVA: 0x000D818C File Offset: 0x000D638C
		private void OnTextMessageShown(AudioEvent e)
		{
			string s = e.Condition ? "FocusZoomIn" : "FocusZoomOut";
			double t = e.Condition ? 0.0 : 0.4;
			AudioPlayer ui = AudioPlayer.UI;
			string sampleName = s;
			float pan = 0.5f;
			float gain = 0.25f;
			double dspTime = AudioPlayer.EarliestSchedulableTime + t;
			ui.PlaySample(sampleName, pan, gain, UnityEngine.Random.Range(1.75f, 2.25f), 0.0, dspTime, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E8A RID: 11914 RVA: 0x000D820C File Offset: 0x000D640C
		private void OnUpgrade(AudioEvent e)
		{
			AudioEventType type = e.Type;
			if (type <= AudioEventType.UpgradeReleased)
			{
				if (type == AudioEventType.UpgradeDragged)
				{
					AudioPlayer.UI.PlaySample("ui_lineOpens", 0.5f, Settings.UPGRADE_GRAB.Gain.Value, Settings.UPGRADE_GRAB.Pitch.Value, 0.0, -1.0, false, null, false, false, 0f, true);
					return;
				}
				if (type != AudioEventType.UpgradeReleased)
				{
					return;
				}
				if (e.UpgradeType == UpgradeType.Motorway)
				{
					AudioPlayer.UI.PlaySample("DrawRoad", 0.5f, Settings.BUILD_ROAD.Gain.Range.Random(-1), Settings.BUILD_ROAD.Pitch.Range.Random(-1), 0.0, -1.0, false, null, false, false, 0f, true);
					return;
				}
				AudioPlayer.UI.PlaySample("ui_lineOpens", 0.5f, Settings.UPGRADE_RELEASE.Gain.Value, Settings.UPGRADE_RELEASE.Pitch.Value, 0.0, -1.0, false, null, false, false, 0f, true);
				return;
			}
			else
			{
				if (type != AudioEventType.UpgradeOver && type != AudioEventType.UpgradeDragSnap)
				{
					return;
				}
				double dur = AudioSystem.Instance.Database.MasterPulse.PulseInfo.PulseDuration;
				double lastPulse = AudioSystem.Instance.Database.MasterPulse.PulseInfo.PulseDspTime - dur;
				double subDivision = dur / 12.0;
				double subsSincePulse = Math.Ceiling((AudioPlayer.EarliestSchedulableTime - lastPulse) / subDivision);
				double nextSubdivision = lastPulse + subsSincePulse * subDivision;
				float a = Maf.Normalize(SFX.mouseSpeed, 0f, 100f, true);
				AudioPlayer ui = AudioPlayer.UI;
				string sampleName = "sineFX_35";
				float pan = 0.5f;
				double dspTime = nextSubdivision;
				ui.PlaySample(sampleName, pan, Mathf.Lerp(0.05f, 0.5f, Maf.VolCurve(a)), Mathf.Lerp(1f, 2f, a), 0.0, dspTime, false, null, false, false, 0f, true);
				return;
			}
		}

		// Token: 0x06002E8B RID: 11915 RVA: 0x000D8428 File Offset: 0x000D6628
		private void OnDrawModeToggle(AudioEvent e)
		{
			bool deleteMode = !e.Condition;
			AudioPlayer.UI.PlaySample("panel_" + (deleteMode ? "lock" : "unlock"), 0.5f, 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E8C RID: 11916 RVA: 0x000D8490 File Offset: 0x000D6690
		private void OnClick(AudioEvent e)
		{
			if (e.UIAudioProfile.HasAny(new UIAudioProfile[]
			{
				UIAudioProfile.Pause,
				UIAudioProfile.Play,
				UIAudioProfile.FastForward
			}) && !e.Condition)
			{
				return;
			}
			float gain = 1f;
			UIAudioProfile uiaudioProfile = e.UIAudioProfile;
			if (uiaudioProfile <= UIAudioProfile.Upgrade)
			{
				if (uiaudioProfile <= UIAudioProfile.Back)
				{
					if (uiaudioProfile == UIAudioProfile.None)
					{
						Dbug.Log.Warn("Audio Event {0} has UIAudioProfile.None", new object[]
						{
							e
						});
						return;
					}
					if (uiaudioProfile == UIAudioProfile.Back)
					{
						AudioPlayer.UI.PlaySample("ui_lineOpens", 0.5f, 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
						return;
					}
				}
				else
				{
					if (uiaudioProfile == UIAudioProfile.Map)
					{
						FX.Modulator.Tremolo trem = new FX.Modulator.Tremolo(Rando.Range(10.0, 20.0, -1), UnityEngine.Random.Range(0f, 0.5f), 0.0);
						AudioPlayer @default = AudioPlayer.Default;
						string sampleName = "PeepEmbarks_" + Get.Loadout.MusicData.NoteWindow.SafeGet(this.mapClickCounter);
						float pan = 0.5f;
						float gain2 = 0.25f;
						float pitch = 3f;
						double fadeTime = (double)UnityEngine.Random.Range(0f, 0.5f);
						double dspTime = -1.0;
						bool loop = false;
						FX.Modulator[] array = new FX.Modulator[2];
						array[0] = new FX.Modulator(null, null, trem);
						int num = 1;
						FX.Modulator.Tremolo tremolo = trem;
						array[num] = new FX.Modulator(new FX.Modulator.Portamento((double)(4f + Mathf.Sign(SFX.PointerTargetDelta) * UnityEngine.Random.value), 4.0, (double)Get.Pulse.Duratio(new float[]
						{
							0.33333334f,
							0.25f,
							0.16666667f,
							0.125f
						})), null, tremolo);
						@default.PlaySample(sampleName, pan, gain2, pitch, fadeTime, dspTime, loop, Rando.Pick<FX.Modulator>(array), false, false, 0f, true);
						this.mapClickCounter++;
						AudioPlayer.UI.PlaySample("sineFX_35", 0.5f, 0.75f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
						return;
					}
					if (uiaudioProfile != UIAudioProfile.Upgrade)
					{
					}
				}
			}
			else if (uiaudioProfile <= UIAudioProfile.DrawModeToggle)
			{
				if (uiaudioProfile != UIAudioProfile.Picture)
				{
					if (uiaudioProfile == UIAudioProfile.DrawModeToggle)
					{
						gain = 0.5f;
					}
				}
				else
				{
					AudioPlayer.UI.PlaySample("take-photo", 0.5f, 0.025f, 1f, 0.0, -1.0, false, null, true, false, 0f, true);
				}
			}
			else
			{
				if (uiaudioProfile == UIAudioProfile.CreativeModePaint)
				{
					AudioPlayer.UI.PlaySample("paint-0" + Rando.Range(1, 9, -1).ToString(), 0.5f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
					return;
				}
				if (uiaudioProfile == UIAudioProfile.CreativeModeTrash)
				{
					AudioPlayer.UI.PlaySample("Erase", 0.5f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
					return;
				}
				if (uiaudioProfile == UIAudioProfile.CreativeModePaintWheel)
				{
					AudioPlayer.UI.PlaySample("paint-0" + Rando.Range(1, 9, -1).ToString(), 0.5f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
					this.PlayHoverNote(e);
					return;
				}
			}
			AudioPlayer.UI.PlaySample("sineFX_35", 0.5f, gain, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
			AudioPlayer.UI.PlaySample("ui_lineOpens", 0.5f, gain, 0.5f, 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E8D RID: 11917 RVA: 0x000D887C File Offset: 0x000D6A7C
		private void OnCheckbox(AudioEvent e)
		{
			float gain = 0.75f;
			float pitch = 1f;
			if (e.UIEventType == UIEventType.CheckboxUnchecked)
			{
				gain = 0.375f;
				pitch = 2f;
			}
			AudioPlayer.UI.PlaySample("ui_checked", 0.5f, gain, pitch, 0.0, -1.0, false, null, false, false, 0f, true);
		}

		// Token: 0x06002E8E RID: 11918 RVA: 0x000D88E0 File Offset: 0x000D6AE0
		private void OnHover(AudioEvent e)
		{
			if (e.UIAudioProfile == UIAudioProfile.None)
			{
				Dbug.Log.Warn("Audio Event {0} has UIAudioProfile.None", new object[]
				{
					e
				});
			}
			UIAudioProfile uiaudioProfile = e.UIAudioProfile;
			if (uiaudioProfile == UIAudioProfile.None || uiaudioProfile == UIAudioProfile.ArrowLeft || uiaudioProfile == UIAudioProfile.ArrowRight || uiaudioProfile == UIAudioProfile.Map || uiaudioProfile == UIAudioProfile.ElectiveUpgrade || uiaudioProfile == UIAudioProfile.Lock || uiaudioProfile == UIAudioProfile.NoHover)
			{
				return;
			}
			PointerEventData pointerEventData = e.PointerEventData;
			if (pointerEventData != null && pointerEventData.pointerId > -1)
			{
				return;
			}
			this.PlayHoverNote(e);
			if (!e.UIAudioProfile.HasAny(new UIAudioProfile[]
			{
				UIAudioProfile.Checkbox,
				UIAudioProfile.Theme
			}))
			{
				AudioPlayer.UI.PlaySample("sineFX_35", 0.5f, Mathf.Lerp(0.1f, 0.4f, SFX.MouseSpeed), Mathf.Lerp(4f, 2f, SFX.MouseSpeed), 0.0, -1.0, false, null, false, false, 0f, true);
			}
		}

		// Token: 0x06002E8F RID: 11919 RVA: 0x000D89E0 File Offset: 0x000D6BE0
		private void PlayHoverNote(AudioEvent e)
		{
			string note = Note.Transpose(12, Get.Loadout.MusicData.NoteWindow.SafeGet(this.hoverCounter));
			string sampleName = "Boop_3_" + note;
			float gain = Note.GainFactor(note) * Mathf.Lerp(Settings.Gain.UI_CHECKBOX_HOVER.x, Settings.Gain.UI_CHECKBOX_HOVER.y, SFX.MouseSpeed);
			if (!e.UIAudioProfile.HasAny(new UIAudioProfile[]
			{
				UIAudioProfile.Checkbox,
				UIAudioProfile.Theme
			}))
			{
				gain *= 0.5f;
			}
			IGATDynamicMixInfo mix = new FX.Modulator(new FX.Modulator.Portamento((double)Rando.Range(0.5f, 1f, -1), 1.0, (double)Rando.Range(0f, 0.1f, -1)), null, null);
			if (Get.State.HasFlag(StateType.ModeNight))
			{
				float i = UnityEngine.Random.Range(0.33f, 1f);
				AudioPlayer @default = AudioPlayer.Default;
				string sampleName2 = sampleName;
				float gain2 = gain;
				float pan = 0.5f;
				double dspTime = -1.0;
				IGATDynamicMixInfo mix2 = mix;
				@default.PlayDurational(sampleName2, gain2, pan, dspTime, i, 0f, i, 1f, false, mix2, false, false);
			}
			else
			{
				AudioPlayer.Default.PlaySample(sampleName, 0.5f, gain, 1f, 0.0, -1.0, false, mix, false, false, 0f, true);
			}
			this.hoverCounter++;
		}

		// Token: 0x04002862 RID: 10338
		public static float PointerTargetDelta;

		// Token: 0x04002863 RID: 10339
		private static Vector2 mousePosPrev;

		// Token: 0x04002864 RID: 10340
		private static Vector2 mousePos;

		// Token: 0x04002865 RID: 10341
		private static float mouseSpeed;

		// Token: 0x04002866 RID: 10342
		private int mapClickCounter;

		// Token: 0x04002867 RID: 10343
		private int hoverCounter;
	}
}
