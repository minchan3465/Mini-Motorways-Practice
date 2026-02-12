using System;
using System.Runtime.CompilerServices;
using Factory;
using Motorways.Audio;
using Motorways.Leaderboards;
using Motorways.Models;
using Motorways.Processes;
using Motorways.UI;
using Popups;
using Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000564 RID: 1380
	public class PauseScreen : InGameScalingScreen
	{
		// Token: 0x06002564 RID: 9572 RVA: 0x0009DE53 File Offset: 0x0009C053
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			this._gameCenterAccessPointButton.Initialise(scope);
		}

		// Token: 0x06002565 RID: 9573 RVA: 0x0009DE68 File Offset: 0x0009C068
		public void OnExit()
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Transition, UIAudioProfile.None, this.GetTransitionDuration(), true, null, ScreenStack.MotorwaysScreen.MainMenu, ScreenStack.MotorwaysScreen.None));
			this._game.TrySave(GameJournalMotive.PlayerQuit);
			this._game.StopAudio();
			this._game.OnGameEnd(GameEndReason.Exit);
			this._game.Scope.Get<GameUIScreen>().SetUIVisible(false, false, true, false);
			ScreenStack.MotorwaysScreen desiredScreen;
			if (!this._screenStack.IsScreenActive<MainMenuScreen>())
			{
				desiredScreen = ScreenStack.MotorwaysScreen.MainMenu;
				this._screenStack.ReplaceScreens<MainMenuScreen>(desiredScreen, typeof(GameContainerScreen), null, true);
			}
			else if (this._gameScope.Get<City>().Rules is TutorialGameRules && this._player.IsAnyTutorialCompleted)
			{
				if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
				{
					this.popupStack.PushPopup<AppleDemoCardPopup>(0f, false).Initialise(false);
				}
				desiredScreen = ScreenStack.MotorwaysScreen.MainMenu;
				this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.MainMenu, false);
			}
			else if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				this.popupStack.PushPopup<AppleDemoCardPopup>(0f, false).Initialise(false);
				desiredScreen = ScreenStack.MotorwaysScreen.MainMenu;
				this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.MainMenu, false);
			}
			else if (!this._screenStack.IsScreenInStack<MapSelectScreen>())
			{
				desiredScreen = ScreenStack.MotorwaysScreen.MapSelect;
				this._screenStack.ReplaceScreens<MapSelectScreen>(desiredScreen, delegate(MapSelectScreen mapSelectScreen)
				{
					mapSelectScreen.PrepareScreen(this._game, false, false);
				}, typeof(GameContainerScreen), null, true);
			}
			else
			{
				desiredScreen = ScreenStack.MotorwaysScreen.MapSelect;
				this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.MapSelect, false);
			}
			StartupScreen startupScreen = this._screenStack.GetActiveScreen<StartupScreen>();
			float duration = this._screenStack.GetTransitionDetailsFrom(base.ScreenType, desiredScreen).duration;
			if (startupScreen != null)
			{
				this._themeDatabase.SetCurrentMapDefinition(startupScreen.mapDefinition, duration);
			}
			if (this._gameScope.Get<City>().Rules is TutorialGameRules)
			{
				TutorialProgressionProcess tutorialProcess = this._game.Scope.Get<TutorialProgressionProcess>();
				this._analytics.TrackTutorialSkipped((int)tutorialProcess.LastReachedMarker);
				this._player.SetTutorialTypeComplete(TutorialProgressionProcess.TutorialTypeForInputType(this._inputState.CurrentDeviceInputType));
				tutorialProcess.SkipTutorial();
			}
		}

		// Token: 0x06002566 RID: 9574 RVA: 0x0009E063 File Offset: 0x0009C263
		public void OnResume()
		{
			this._screenStack.PopOneScreen();
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			this._appScope.Get<ISoftwareCapabilities>().SetIsInGame(true);
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x0009E090 File Offset: 0x0009C290
		public void OnRestart()
		{
			bool shouldRestartGame = true;
			ActiveChallengesModel model = this._simulation.GetModel<ActiveChallengesModel>();
			MotorwaysTimedChallengeScore challengeScore = this._player.GetChallengeScore(MapChallenge.ChallengeType.Daily, this._challengeSystem.DailyChallenge.TimeEnd);
			if (model.challengeType == MapChallenge.ChallengeType.Daily && challengeScore.ScoreState != LeaderboardScoreState.Locked)
			{
				shouldRestartGame = false;
				this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.DailyChallenge, null, delegate()
				{
					this._restartGameWhenGainedFocus = true;
				}, StringId.DailyChallenge_RestartConfirmation);
			}
			if (this._gameScope.Get<City>().Rules is TutorialGameRules)
			{
				this._game.Scope.Get<TutorialProgressionProcess>().UnregisterActions();
			}
			if (shouldRestartGame)
			{
				this.RestartGame();
			}
		}

		// Token: 0x06002568 RID: 9576 RVA: 0x0009E142 File Offset: 0x0009C342
		public void OnPause()
		{
			this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.OptionsPause, false, null, true);
		}

		// Token: 0x06002569 RID: 9577 RVA: 0x0009E158 File Offset: 0x0009C358
		private void RestartGame()
		{
			this._game.OnGameEnd(GameEndReason.Restart);
			GameContainerScreen gameContainerScreen = this._screenStack.GetActiveScreen<GameContainerScreen>();
			if (Diagnostics.Verify(gameContainerScreen != null, "We don't have an active GameContainerScreen even though we're at the PauseScreen!"))
			{
				GameMode gameMode = this._gameScope.Get<MotorwaysGame>().StartedWithGameMode;
				gameContainerScreen.PrepareForRestartMap(gameMode);
			}
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.InGame, false);
			gameContainerScreen.SkipNextTransition();
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x0009E1D8 File Offset: 0x0009C3D8
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			this._changeBlurWhenTransitioning = (inScreen != ScreenStack.MotorwaysScreen.ChallengeInfo && inScreen != ScreenStack.MotorwaysScreen.OptionsPause);
			this._fastFadeOut = (inScreen != ScreenStack.MotorwaysScreen.InGame && inScreen != ScreenStack.MotorwaysScreen.ChallengeInfo && inScreen != ScreenStack.MotorwaysScreen.OptionsPause);
			this.submitDiagnosticsReportModal.HideModal();
			base.TransitionOut(inScreen);
			this._skipTransitions = ((inScreen != ScreenStack.MotorwaysScreen.InGame && this._skipTransitions) || inScreen == ScreenStack.MotorwaysScreen.ChallengeInfo || inScreen == ScreenStack.MotorwaysScreen.OptionsPause);
			this._gameCenterAccessPointButton.Hide();
		}

		// Token: 0x0600256B RID: 9579 RVA: 0x0009E254 File Offset: 0x0009C454
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.TransitionIn(outScreen);
			this._skipTransitions = false;
			this._canvasGroup.Alpha = 0f;
			this.restartButtonText.SetStringId(this._scope, PauseScreen.GetRestartText(this._game));
			this._changeBlurWhenTransitioning = (outScreen != ScreenStack.MotorwaysScreen.ChallengeInfo && outScreen != ScreenStack.MotorwaysScreen.OptionsPause && outScreen != ScreenStack.MotorwaysScreen.CinematicMode);
			if (this._gameScope.Get<City>().Rules is TutorialGameRules && !this._player.IsAnyTutorialCompleted)
			{
				this.exitButtonText.LocString = StandaloneLocString.CreateString(this._gameScope, StringId.SkipTutorial);
			}
			else
			{
				this.exitButtonText.LocString = StandaloneLocString.CreateString(this._gameScope, StringId.MainMenu);
			}
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.PauseScreen, this._scope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.CancelActions);
			this._appScope.Get<ISoftwareCapabilities>().SetIsInGame(false);
			this.UpdateButtonStates();
			GameUIScreen gameUIScreen = this._gameScope.Get<GameUIScreen>();
			gameUIScreen.SetUIVisible(false, false, false, false);
			gameUIScreen.ExitEditModeUI();
			this._photoModeButton.gameObject.SetActive(this._softwareCapabilities.CanShareImage);
			this._challengeInfoButton.gameObject.SetActive(this._game.Simulation.GetModel<ActiveChallengesModel>().HasChallenges);
			GameMode gameMode = this._gameScope.Get<CityModel>().Mode;
			this._cinematicModeButton.gameObject.SetActive(gameMode != GameMode.Tutorial);
			bool showEndlessModeInfoButton = gameMode == GameMode.Endless;
			this._endlessModeInfoButton.gameObject.SetActive(showEndlessModeInfoButton);
			bool showExpertModeInfoButton = gameMode == GameMode.Expert;
			this._expertModeInfoButton.gameObject.SetActive(showExpertModeInfoButton);
			bool showCreativeModeInfoButton = gameMode == GameMode.Creative;
			this._creativeModeInfoButton.gameObject.SetActive(showCreativeModeInfoButton);
			this._movieButton.gameObject.SetActive(this._softwareCapabilities.CanShareImage && this._softwareCapabilities.SupportsMovieScreen);
			if (this._gameScope.Get<City>().Rules is TutorialGameRules)
			{
				TutorialProgressionProcess tutorial = this._gameScope.Get<TutorialProgressionProcess>();
				if (tutorial.HasVisibleMessage)
				{
					tutorial.TemporarilyHideMessage();
				}
				this._movieButton.gameObject.SetActive(false);
				this._photoModeButton.gameObject.SetActive(false);
			}
			this._gameScope.Get<CameraView>().ResetPlayerViewport();
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			this._reportUpload = null;
			this.SetupNavigationOnBottomRightButtons(showEndlessModeInfoButton, showExpertModeInfoButton, showCreativeModeInfoButton);
		}

		// Token: 0x0600256C RID: 9580 RVA: 0x0009E4B0 File Offset: 0x0009C6B0
		public static StringId GetRestartText(MotorwaysGame game)
		{
			GameMode restartGameMode = game.StartedWithGameMode;
			if (restartGameMode == GameMode.Normal)
			{
				ActiveChallengesModel activeChallengesModel = game.Scope.Get<ActiveChallengesModel>();
				if (activeChallengesModel.challengeType == MapChallenge.ChallengeType.Daily)
				{
					return StringId.Replay_Challenge;
				}
				if (activeChallengesModel.challengeType == MapChallenge.ChallengeType.City || activeChallengesModel.challengeType == MapChallenge.ChallengeType.Weekly)
				{
					return StringId.Restart_Challenge;
				}
				return StringId.Restart_Classic;
			}
			else
			{
				if (restartGameMode == GameMode.Endless)
				{
					return StringId.Restart_Endless;
				}
				if (restartGameMode == GameMode.Expert)
				{
					return StringId.Restart_Expert;
				}
				return StringId.Restart;
			}
		}

		// Token: 0x0600256D RID: 9581 RVA: 0x0009E516 File Offset: 0x0009C716
		public override void TransitionInTick()
		{
			base.TransitionInTick();
			if (this._changeBlurWhenTransitioning)
			{
				this._gameCamera.customBlur.Strength = this.TransitionInPercentage();
			}
			this._canvasGroup.Alpha = this.TransitionInPercentage();
		}

		// Token: 0x0600256E RID: 9582 RVA: 0x0009E550 File Offset: 0x0009C750
		public override void TransitionOutTick()
		{
			base.TransitionOutTick();
			if (this._changeBlurWhenTransitioning)
			{
				this._gameCamera.customBlur.Strength = 1f - this.TransitionOutPercentage();
			}
			float alphaOutProgress = this.TransitionOutPercentage();
			if (this._fastFadeOut)
			{
				alphaOutProgress *= 4f;
			}
			this._canvasGroup.Alpha = 1f - alphaOutProgress;
		}

		// Token: 0x0600256F RID: 9583 RVA: 0x0009E5B0 File Offset: 0x0009C7B0
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this._gameCenterAccessPointButton.Show();
		}

		// Token: 0x06002570 RID: 9584 RVA: 0x0009E5C4 File Offset: 0x0009C7C4
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this._shouldFadeIn || this._shouldFadeOut)
			{
				float newAlpha = this._canvasGroup.Alpha + (float)(this._shouldFadeIn ? 1 : -1) * Time.deltaTime / this._fadeDuration;
				if (newAlpha <= 0f || newAlpha >= 1f)
				{
					newAlpha = Mathf.Clamp(newAlpha, 0f, 1f);
					this._shouldFadeIn = false;
					this._shouldFadeOut = false;
				}
				this._canvasGroup.Alpha = newAlpha;
			}
			if (this._customScreenGameStarter == null || !this._customScreenGameStarter.CanStart)
			{
				return;
			}
			this._customScreenGameStarter.Start(this._screenStack, this._scope);
			this._customScreenGameStarter = null;
		}

		// Token: 0x06002571 RID: 9585 RVA: 0x0009E67F File Offset: 0x0009C87F
		public override void OnLostFocus()
		{
			base.OnLostFocus();
			this._shouldFadeIn = false;
			this._shouldFadeOut = true;
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x0009E695 File Offset: 0x0009C895
		public override void OnGainedFocus()
		{
			base.OnGainedFocus();
			this._shouldFadeIn = true;
			this._shouldFadeOut = false;
			if (this._restartGameWhenGainedFocus)
			{
				this._restartGameWhenGainedFocus = false;
				this.RestartGame();
			}
		}

		// Token: 0x06002573 RID: 9587 RVA: 0x00091CB7 File Offset: 0x0008FEB7
		public void OnPhotoMode()
		{
			this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.Photo, false, this._gameScope, true);
		}

		// Token: 0x06002574 RID: 9588 RVA: 0x0009E6C0 File Offset: 0x0009C8C0
		public void OnMovieButtonPressed()
		{
			MotorwaysGameJournalSave save = this._gameScope.Get<MotorwaysGameJournalSave>();
			if (save.InitializeFromSimulation(this._simulation, GameJournalMotive.PlayerQuit))
			{
				if (this._customScreenGameStarter == null)
				{
					this._customScreenGameStarter = new GameStarter(this);
				}
				MapDatabase mapDatabase = this._appScope.Get<MapDatabase>();
				this._customScreenGameStarter.StartSavedGameFromCustomScreen(mapDatabase.MapLibrary, save, ScreenStack.MotorwaysScreen.Movie, false, false);
			}
		}

		// Token: 0x06002575 RID: 9589 RVA: 0x0009E720 File Offset: 0x0009C920
		public void OnCinematicModeButtonPressed()
		{
			if (this._game.StartedWithGameMode == GameMode.Endless)
			{
				this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.CinematicMode, true, this._gameScope, true);
				return;
			}
			MotorwaysGameJournalSave journalSave = this._gameScope.Get<MotorwaysGameJournalSave>();
			if (journalSave.InitializeFromSimulation(this._simulation, GameJournalMotive.PlayerQuit))
			{
				if (this._customScreenGameStarter == null)
				{
					this._customScreenGameStarter = new GameStarter(this);
				}
				MapDatabase mapDatabase = this._appScope.Get<MapDatabase>();
				this._customScreenGameStarter.StartSavedGameFromCustomScreen(mapDatabase.MapLibrary, journalSave, ScreenStack.MotorwaysScreen.CinematicMode, false, false);
				return;
			}
			Diagnostics.FailAssert("Cinematic mode failed to start from pause screen. Likely due to save not initializing correctly.", Array.Empty<object>());
		}

		// Token: 0x06002576 RID: 9590 RVA: 0x0009E7B4 File Offset: 0x0009C9B4
		public void OnNightModeButtonToggled(bool toggleOn)
		{
			this._themeDatabase.SetNightMode(toggleOn, true);
		}

		// Token: 0x06002577 RID: 9591 RVA: 0x0009E7C4 File Offset: 0x0009C9C4
		public void OnChallengesButton()
		{
			ActiveChallengesModel challengeModel = this._game.Simulation.GetModel<ActiveChallengesModel>();
			this._screenStack.PushScreen<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, delegate(ChallengeInfoScreen screen)
			{
				screen.PrepareScreen(challengeModel.challengeType, challengeModel.challenges, challengeModel.timeStart, challengeModel.timeEnd, StringId.Continue, false, true, this._game.Scope, true);
			}, false, null, true, null);
		}

		// Token: 0x06002578 RID: 9592 RVA: 0x0009E812 File Offset: 0x0009CA12
		public void OnModeInfoButton()
		{
			this.popupStack.PushPopup<ModeInfoPopupInGame>(0f, false).Initialize(this._appScope, this._game.Scope.Get<CityModel>().Mode, delegate
			{
				this._appScope.Get<InputState>().BlockGameInput = this._blocksGameInput;
			});
		}

		// Token: 0x06002579 RID: 9593 RVA: 0x0009E854 File Offset: 0x0009CA54
		public void OnVolumeButtonToggled(bool toggledOn)
		{
			int newVolume = toggledOn ? this._player.PreviousVolumeSetting : 0;
			this._player.VolumeSetting = newVolume;
		}

		// Token: 0x0600257A RID: 9594 RVA: 0x0009E880 File Offset: 0x0009CA80
		public void OnSubmitDiagnosticsReport()
		{
			if (this._reportUpload != null)
			{
				return;
			}
			if (this._scope.Get<IAppCommandSource>() is JournalAppCommandSource)
			{
				return;
			}
			this.submitDiagnosticsReportModal.ShowModal();
			this.finishSubmittingReportButton.interactable = false;
			this._canvasGroup.SetInteractable(false);
			this.previousBackButton = this.backButton;
			this.backButton = this.finishSubmittingReportButton;
			this._navigation.SetNewFocus(this.finishSubmittingReportButton);
			this.reportIdLabel.text = "Submitting...";
			Diagnostics.Report report = this._game.GenerateDiagnosticReport("manual", DiagnosticReportAttachments.SimCommandJournal | DiagnosticReportAttachments.SimArchive | DiagnosticReportAttachments.Screenshot | DiagnosticReportAttachments.Log);
			this._reportUpload = report.Upload();
		}

		// Token: 0x0600257B RID: 9595 RVA: 0x0009E924 File Offset: 0x0009CB24
		private void UpdateButtonStates()
		{
			this.nightmodeToggle.SetOption(this._themeDatabase.IsInNightMode ? 1 : 0, true, false);
			this.volumeToggle.gameObject.SetActive(this._audioSystem.RequiresVolumeControl);
			this.volumeToggle.SetOption((this._player.VolumeSetting != 0) ? 1 : 0, true, false);
			if (FeatureToggle.IsFeatureEnabled(Feature.DiagnosticReportsButton))
			{
				this.submitDiagnosticsReportButton.gameObject.SetActive(true);
				return;
			}
			this.submitDiagnosticsReportButton.gameObject.SetActive(false);
		}

		// Token: 0x0600257C RID: 9596 RVA: 0x0009E9B3 File Offset: 0x0009CBB3
		public override bool CanTransitionIn()
		{
			return !this._screenStack.AreAnyScreensTransitioning;
		}

		// Token: 0x0600257D RID: 9597 RVA: 0x0009E9C3 File Offset: 0x0009CBC3
		public override void Reset()
		{
			base.Reset();
			this._changeBlurWhenTransitioning = false;
			this._shouldFadeIn = false;
			this._shouldFadeOut = false;
			this._fastFadeOut = false;
		}

		// Token: 0x0600257E RID: 9598 RVA: 0x0009E9E7 File Offset: 0x0009CBE7
		private void OnEnable()
		{
			this._canvasGroup.Alpha = 0f;
		}

		// Token: 0x0600257F RID: 9599 RVA: 0x0009E9FC File Offset: 0x0009CBFC
		public void Update()
		{
			if (this._reportUpload != null)
			{
				string reportText = "Uploading metadata ...";
				if (this._reportUpload.IsComplete)
				{
					reportText = string.Format("Done!\nReport id: {0}", this._reportUpload.Id);
				}
				else if (this._reportUpload.Id > 0)
				{
					reportText = string.Format("Report id: {0}\nUploaded {1} KiB / {2} KiB", this._reportUpload.Id, Mathf.Max(1, this._reportUpload.BytesUploaded / 1024), Mathf.Max(1, this._reportUpload.BytesToUpload / 1024));
				}
				this.reportIdLabel.text = reportText;
				if (this._reportUpload.IsComplete)
				{
					this.finishSubmittingReportButton.interactable = true;
					this.backButton = this.previousBackButton;
					this._navigation.SetNewFocus(this.finishSubmittingReportButton);
					this._reportUpload = null;
				}
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.ToggleDiagnosticReportButtonWithKeyCode))
			{
				this.SubmitReportButtonKeySequenceCheck();
			}
		}

		// Token: 0x06002580 RID: 9600 RVA: 0x0009EB00 File Offset: 0x0009CD00
		private void SubmitReportButtonKeySequenceCheck()
		{
			if (Input.GetKeyDown(PauseScreen.SubmitReportKeySequence[this._nextKeyIndex]))
			{
				this._lastTimeKeyHitInSequence = Time.time;
				this._nextKeyIndex++;
				if (this._nextKeyIndex >= PauseScreen.SubmitReportKeySequence.Length)
				{
					this.submitDiagnosticsReportButton.gameObject.SetActive(!this.submitDiagnosticsReportButton.gameObject.activeSelf);
					this._lastTimeKeyHitInSequence = float.MinValue;
					this._nextKeyIndex = 0;
				}
			}
			else if (Input.anyKeyDown)
			{
				this._lastTimeKeyHitInSequence = float.MinValue;
				this._nextKeyIndex = 0;
			}
			if (this._lastTimeKeyHitInSequence > -3.4028235E+38f && Time.time - this._lastTimeKeyHitInSequence > 2f)
			{
				this._lastTimeKeyHitInSequence = float.MinValue;
				this._nextKeyIndex = 0;
			}
		}

		// Token: 0x06002581 RID: 9601 RVA: 0x0009EBCC File Offset: 0x0009CDCC
		private void SetupNavigationOnBottomRightButtons(bool showEndlessModeInfoButton, bool showExpertModeInfoButton, bool showCreativeModeInfoButton)
		{
			if (showEndlessModeInfoButton)
			{
				this.SetNavigationOnLeftMostButton(this._endlessModeInfoButton);
				return;
			}
			if (showExpertModeInfoButton)
			{
				this.SetNavigationOnLeftMostButton(this._expertModeInfoButton);
				return;
			}
			if (showCreativeModeInfoButton)
			{
				this.SetNavigationOnLeftMostButton(this._creativeModeInfoButton);
				return;
			}
			if (this._game.Simulation.GetModel<ActiveChallengesModel>().HasChallenges)
			{
				this.SetNavigationOnLeftMostButton(this._challengeInfoButton);
				return;
			}
			this.SetNavigationOnLeftMostButton(this._cinematicModeButton);
		}

		// Token: 0x06002582 RID: 9602 RVA: 0x0009EC39 File Offset: 0x0009CE39
		private void SetNavigationOnLeftMostButton(TouchButton leftMostButton)
		{
			BaseScalingScreen.SetNavigationOnDown(this.volumeToggleTouchButton, leftMostButton);
			BaseScalingScreen.SetNavigationOnRight(this.volumeToggleTouchButton, leftMostButton);
			BaseScalingScreen.SetNavigationOnUp(leftMostButton, this.volumeToggleTouchButton);
		}

		// Token: 0x06002584 RID: 9604 RVA: 0x0009EC72 File Offset: 0x0009CE72
		// Note: this type is marked as 'beforefieldinit'.
		static PauseScreen()
		{
			KeyCode[] array = new KeyCode[8];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.9354A8542FC89AC1E99009FC547B06E9E591D378CEEBE5AA15EC5F01080BC7EE).FieldHandle);
			PauseScreen.SubmitReportKeySequence = array;
		}

		// Token: 0x04001F90 RID: 8080
		[Dependency]
		private IScope _scope;

		// Token: 0x04001F91 RID: 8081
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001F92 RID: 8082
		[Dependency]
		private ChallengeSystem _challengeSystem;

		// Token: 0x04001F93 RID: 8083
		private Diagnostics.ReportUpload _reportUpload;

		// Token: 0x04001F94 RID: 8084
		public LocalizedTextUI exitButtonText;

		// Token: 0x04001F95 RID: 8085
		public SymbolOptionButton volumeToggle;

		// Token: 0x04001F96 RID: 8086
		public SymbolOptionButton nightmodeToggle;

		// Token: 0x04001F97 RID: 8087
		public TouchButton submitDiagnosticsReportButton;

		// Token: 0x04001F98 RID: 8088
		public NewsletterModal submitDiagnosticsReportModal;

		// Token: 0x04001F99 RID: 8089
		public TouchButton finishSubmittingReportButton;

		// Token: 0x04001F9A RID: 8090
		public TextMeshProUGUI reportIdLabel;

		// Token: 0x04001F9B RID: 8091
		public LocalizedTextUI restartButtonText;

		// Token: 0x04001F9C RID: 8092
		[SerializeField]
		private TouchButton _photoModeButton;

		// Token: 0x04001F9D RID: 8093
		[SerializeField]
		private TouchButton _challengeInfoButton;

		// Token: 0x04001F9E RID: 8094
		[SerializeField]
		private TouchButton _movieButton;

		// Token: 0x04001F9F RID: 8095
		[SerializeField]
		private TouchButton _endlessModeInfoButton;

		// Token: 0x04001FA0 RID: 8096
		[SerializeField]
		private TouchButton _expertModeInfoButton;

		// Token: 0x04001FA1 RID: 8097
		[SerializeField]
		private TouchButton _creativeModeInfoButton;

		// Token: 0x04001FA2 RID: 8098
		[SerializeField]
		private TouchButton _cinematicModeButton;

		// Token: 0x04001FA3 RID: 8099
		[SerializeField]
		private GameCenterAccessPointButton _gameCenterAccessPointButton;

		// Token: 0x04001FA4 RID: 8100
		[SerializeField]
		[Tooltip("How long in seconds the pause menu should fade in/out when it loses focus")]
		private float _fadeDuration;

		// Token: 0x04001FA5 RID: 8101
		[SerializeField]
		private TouchButton volumeToggleTouchButton;

		// Token: 0x04001FA6 RID: 8102
		private GameStarter _customScreenGameStarter;

		// Token: 0x04001FA7 RID: 8103
		private bool _changeBlurWhenTransitioning;

		// Token: 0x04001FA8 RID: 8104
		private bool _fastFadeOut;

		// Token: 0x04001FA9 RID: 8105
		private bool _shouldFadeIn;

		// Token: 0x04001FAA RID: 8106
		private bool _shouldFadeOut;

		// Token: 0x04001FAB RID: 8107
		private bool _restartGameWhenGainedFocus;

		// Token: 0x04001FAC RID: 8108
		public static readonly KeyCode[] SubmitReportKeySequence;

		// Token: 0x04001FAD RID: 8109
		private const float MaxTimeBetweenKeysInSeconds = 2f;

		// Token: 0x04001FAE RID: 8110
		private float _lastTimeKeyHitInSequence = float.MinValue;

		// Token: 0x04001FAF RID: 8111
		private int _nextKeyIndex;
	}
}
