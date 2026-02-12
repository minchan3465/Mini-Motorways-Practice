using System;
using System.Collections;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using Factory.Pools;
using Motorways.Models;
using Motorways.Processes;
using Motorways.UI;
using NaughtyAttributes;
using Popups;
using Screens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000537 RID: 1335
	public class GameOverScreen : InGameScalingScreen, IReusable
	{
		// Token: 0x06002353 RID: 9043 RVA: 0x00090F20 File Offset: 0x0008F120
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.TransitionIn(outScreen);
			this._doTransitionAnimation = (outScreen != ScreenStack.MotorwaysScreen.ChallengeInfo);
			bool isInTutorial = this._gameScope.Get<City>().Rules is TutorialGameRules;
			this._skipTransitions = false;
			if (isInTutorial)
			{
				this.restartButton.gameObject.SetActive(false);
				this.continueInEndlessButton.gameObject.SetActive(false);
				this.firstFocus = this.exitButton;
			}
			else
			{
				this.restartButton.gameObject.SetActive(true);
				this.continueInEndlessButton.gameObject.SetActive(true);
				this.firstFocus = this.restartButton;
			}
			this.restartButtonText.SetStringId(this._appScope, PauseScreen.GetRestartText(this._game));
			if (this._doTransitionAnimation)
			{
				this.restartButton.image.fillAmount = 0f;
				this.exitButton.image.fillAmount = 0f;
				this.continueInEndlessButton.image.fillAmount = 0f;
			}
			ActiveChallengesModel challenges = this._game.Simulation.GetModel<ActiveChallengesModel>();
			if (outScreen == ScreenStack.MotorwaysScreen.InGame)
			{
				this._game.OnGameEnd(GameEndReason.GameOver);
				foreach (NewUpgradeAnimationView newUpgradeAnimationView in this._gameScope.Get<ViewClient>().GetViews<NewUpgradeAnimationView>())
				{
					newUpgradeAnimationView.Hide();
				}
				MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.OutOfGame, this._gameScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.CancelActions);
				ScoreModel scoreKeep = this._gameScope.Get<ScoreModel>();
				ClockModel clock = this._gameScope.Get<ClockModel>();
				GameRules rules = this._game.Scope.Get<City>().Rules;
				MotorwaysStringKey lineOneKey = this._gameScope.Get<MotorwaysStringKey>();
				lineOneKey.InitWithStringId(rules.GetGameOverLineOne(), clock.Day, null);
				MotorwaysStringKey lineTwoKey = this._gameScope.Get<MotorwaysStringKey>();
				lineTwoKey.InitWithStringId(rules.GetGameOverLineTwo(), scoreKeep.Score, new Dictionary<string, string>
				{
					{
						"Num",
						scoreKeep.Score.ToString()
					},
					{
						"Day",
						clock.Day.ToString()
					}
				});
				this.textLineOne.LocString = StandaloneLocString.CreateString(this._gameScope, lineOneKey);
				this.textLineTwo.LocString = StandaloneLocString.CreateString(this._gameScope, lineTwoKey);
				this.textLineOne.gameObject.SetActive(!isInTutorial);
				this.textLineTwo.TextField.maxVisibleCharacters = 0;
				StringId titleId = StringId.GameOver;
				StringId exitButtonId = StringId.Menu;
				if (isInTutorial)
				{
					titleId = StringId.Tutorial_Completed;
					exitButtonId = StringId.GameOver_Tutorial_MenuButton;
				}
				else if (challenges.HasChallenges)
				{
					if (challenges.challengeType == MapChallenge.ChallengeType.Daily)
					{
						titleId = StringId.DailyChallenge;
					}
					else if (challenges.challengeType == MapChallenge.ChallengeType.Weekly)
					{
						titleId = StringId.WeeklyChallenge;
					}
					else if (challenges.challengeType == MapChallenge.ChallengeType.City)
					{
						string titleStringId = this._game.MapDefinition.cityChallenges[challenges.cityChallengeIndex].titleStringId;
					}
				}
				this.textTitle.LocString = StandaloneLocString.CreateString(this._gameScope, titleId);
				this.exitButtonText.LocString = StandaloneLocString.CreateString(this._gameScope, exitButtonId);
				this.continueInEndlessText.LocString = StandaloneLocString.CreateString(this._gameScope, StringId.ContinueInEndless);
				CityModel cityModel = this._game.Simulation.GetModel<CityModel>();
				int bestKnownScore = scoreKeep.Score + 1;
				if (challenges.HasChallenges)
				{
					if (challenges.challengeType == MapChallenge.ChallengeType.City)
					{
						bestKnownScore = this._player.GetCityChallengeScore(cityModel.cityName, cityModel.Mode, challenges.cityChallengeIndex, true).BestScore;
					}
					else if (challenges.challengeType == MapChallenge.ChallengeType.Weekly)
					{
						bestKnownScore = this._player.GetChallengeScore(MapChallenge.ChallengeType.Weekly, challenges.timeEnd).Score;
					}
				}
				else
				{
					MotorwaysCityStatistics stats = this._player.GetCityStatisticsForCity(cityModel.cityName, cityModel.Mode, false);
					if (stats != null)
					{
						bestKnownScore = stats.MaxTrips;
					}
				}
				this._hasNewHighScore = (scoreKeep.Score == bestKnownScore);
			}
			if (this._doTransitionAnimation)
			{
				this.photoModeButtonAnchor.SetActive(false);
				this.photoModeButton.interactable = this._softwareCapabilities.CanShareImage;
			}
			for (int iconIndex = 0; iconIndex < this._challengeIcons.Length; iconIndex++)
			{
				if (iconIndex < challenges.challenges.Count)
				{
					ChallengeData challenge = challenges.challenges[iconIndex];
					this._challengeIcons[iconIndex].gameObject.SetActive(true);
					this._challengeIcons[iconIndex].SetChallengeIcons(challenge.icon, false, challenge.subIcon, challenge.subIconBackground);
				}
				else
				{
					this._challengeIcons[iconIndex].gameObject.SetActive(false);
				}
			}
			if (this._doTransitionAnimation)
			{
				this._challengeIconContainer.gameObject.SetActive(false);
				this.restartButtonText.TextField.alpha = 0f;
				this.continueInEndlessText.TextField.alpha = 0f;
				this.exitButtonText.TextField.alpha = 0f;
			}
			GameUIScreen gameUI = this._gameScope.Get<GameUIScreen>();
			gameUI.SetUIVisible(false, false, true, true);
			gameUI.SetRoadCursorActive(false);
			if (gameUI.IsFocusPointActive)
			{
				gameUI.SetFocusPointActive(false, false);
			}
			if (isInTutorial)
			{
				this._player.SetTutorialTypeComplete(TutorialProgressionProcess.TutorialTypeForInputType(this._inputState.CurrentDeviceInputType));
			}
			this._appScope.Get<ISoftwareCapabilities>().SetIsInGame(false);
			if (FeatureToggle.IsFeatureEnabled(Feature.DiagnosticReportsButton))
			{
				this.submitDiagnosticsReportButton.gameObject.SetActive(!isInTutorial);
			}
			else
			{
				this.submitDiagnosticsReportButton.gameObject.SetActive(false);
			}
			this._soakTestCountdown = 4f;
			this._reportUpload = null;
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._gameOverTextContainer.GetComponent<RectTransform>());
			this._gameOverTextContainer.Snap();
		}

		// Token: 0x06002354 RID: 9044 RVA: 0x000914D4 File Offset: 0x0008F6D4
		public override void OnTransitionedIn()
		{
			if (this._doTransitionAnimation)
			{
				this.FireAllAnimations();
				return;
			}
			base.OnTransitionedIn();
		}

		// Token: 0x06002355 RID: 9045 RVA: 0x000914EB File Offset: 0x0008F6EB
		public override void OnTransitionedOut()
		{
			base.OnTransitionedOut();
			if (this._doTransitionAnimation)
			{
				this.textLineTwo.TextField.maxVisibleCharacters = 0;
			}
		}

		// Token: 0x06002356 RID: 9046 RVA: 0x0009150C File Offset: 0x0008F70C
		private void FireAllAnimations()
		{
			this._canvasGroup.SetBlocksRaycasts(false);
			this._canvasGroup.SetInteractable(false);
			base.StartCoroutine(this.AnimateFillText(this.textLineTwo));
			if (this._hasNewHighScore)
			{
				base.StartCoroutine(this.AnimatePing(this._delayToFillText, this.textLineTwo));
			}
			ActiveChallengesModel model = this._game.Simulation.GetModel<ActiveChallengesModel>();
			float buttonDelay = this._buttonAnimationDelay;
			if (model.HasChallenges)
			{
				base.StartCoroutine(this.EnableChallengeButtons(buttonDelay));
				buttonDelay += this._challengeIconsAdditionalDelay;
			}
			else
			{
				this._challengeIconContainer.gameObject.SetActive(false);
			}
			base.StartCoroutine(this.AnimateButtonFill(buttonDelay, this.restartButton));
			base.StartCoroutine(this.AnimateButtonText(buttonDelay + this._timeToAlphaButtonText, this.restartButtonText));
			base.StartCoroutine(this.AnimateButtonFill(buttonDelay + this._timeToFillButton, this.continueInEndlessButton));
			base.StartCoroutine(this.AnimateButtonText(buttonDelay + this._timeToFillButton + this._timeToAlphaButtonText, this.continueInEndlessText));
			base.StartCoroutine(this.AnimateButtonFill(buttonDelay + this._timeToFillButton * 2f, this.exitButton));
			base.StartCoroutine(this.AnimateButtonText(buttonDelay + this._timeToFillButton * 2f + this._timeToAlphaButtonText, this.exitButtonText));
			base.StartCoroutine(this.EnablePhotoModeButton(buttonDelay + this._timeToFillButton * 2f + this._timeToAlphaButtonText));
			base.StartCoroutine(this.EnableScreenInteraction(buttonDelay + this._timeToFillButton * 2f + this._timeToAlphaButtonText));
		}

		// Token: 0x06002357 RID: 9047 RVA: 0x000916A6 File Offset: 0x0008F8A6
		private IEnumerator EnableChallengeButtons(float delay)
		{
			yield return new WaitForSeconds(delay);
			this._challengeIconContainer.gameObject.SetActive(true);
			this._challengeIconContainer.Play(GameOverScreen.AnimateInStateID);
			this._challengeIconContainer.ResetTrigger(GameOverScreen.NormalTriggerID);
			yield break;
		}

		// Token: 0x06002358 RID: 9048 RVA: 0x000916BC File Offset: 0x0008F8BC
		private IEnumerator EnablePhotoModeButton(float delay)
		{
			yield return new WaitForSeconds(delay);
			if (this._softwareCapabilities.CanShareImage)
			{
				bool isInTutorial = this._gameScope.Get<City>().Rules is TutorialGameRules;
				this.photoModeButtonAnchor.SetActive(!isInTutorial);
				this.photoModeButton.interactable = (!isInTutorial && this._softwareCapabilities.CanShareImage);
			}
			yield break;
		}

		// Token: 0x06002359 RID: 9049 RVA: 0x000916D2 File Offset: 0x0008F8D2
		private IEnumerator EnableScreenInteraction(float delay)
		{
			yield return new WaitForSeconds(delay);
			base.OnTransitionedIn();
			this._canvasGroup.SetBlocksRaycasts(true);
			this._canvasGroup.SetInteractable(true);
			yield break;
		}

		// Token: 0x0600235A RID: 9050 RVA: 0x000916E8 File Offset: 0x0008F8E8
		private IEnumerator AnimateFillText(LocalizedTextUI text)
		{
			text.TextField.maxVisibleCharacters = 0;
			yield return new WaitForSeconds(this._delayToFillText);
			int textLength = text.TextField.text.Length;
			if (textLength <= 0)
			{
				yield break;
			}
			float step = this._timeToFillText / (float)textLength;
			while (textLength > text.TextField.maxVisibleCharacters)
			{
				TMP_Text textField = text.TextField;
				int maxVisibleCharacters = textField.maxVisibleCharacters;
				textField.maxVisibleCharacters = maxVisibleCharacters + 1;
				yield return new WaitForSeconds(step);
			}
			yield break;
		}

		// Token: 0x0600235B RID: 9051 RVA: 0x000916FE File Offset: 0x0008F8FE
		private IEnumerator AnimatePing(float delay, LocalizedTextUI text)
		{
			yield return new WaitForSeconds(delay);
			this._uiPing.SetTrigger(GameOverScreen.PingTriggerID);
			yield break;
		}

		// Token: 0x0600235C RID: 9052 RVA: 0x00091714 File Offset: 0x0008F914
		private IEnumerator AnimateButtonFill(float delay, TouchButton button)
		{
			button.image.fillAmount = 0f;
			yield return new WaitForSeconds(delay);
			float runningTime = 0f;
			while (button.image.fillAmount < 1f)
			{
				runningTime += Time.deltaTime;
				button.image.fillAmount = Mathf.Lerp(0f, 1f, runningTime / this._timeToFillButton);
				yield return new WaitForFixedUpdate();
			}
			yield break;
		}

		// Token: 0x0600235D RID: 9053 RVA: 0x00091731 File Offset: 0x0008F931
		private IEnumerator AnimateButtonText(float delay, LocalizedTextUI button)
		{
			button.TextField.alpha = 0f;
			yield return new WaitForSeconds(delay);
			float runningTime = 0f;
			while (button.TextField.alpha < 1f)
			{
				runningTime += Time.deltaTime;
				button.TextField.alpha = Mathf.Lerp(0f, 1f, runningTime / this._timeToFillButton);
				yield return new WaitForFixedUpdate();
			}
			yield break;
		}

		// Token: 0x0600235E RID: 9054 RVA: 0x00091750 File Offset: 0x0008F950
		private void SetupWorldSpaceCanvas(float endZoom)
		{
			float scale = endZoom * 2f / (float)Screen.height;
			scale *= (float)Screen.height / this.scalingReferenceResolution.y;
			float aspectRatio = (float)Screen.width / (float)Screen.height;
			this._rectTransform.sizeDelta = new Vector2(this.scalingReferenceResolution.y * aspectRatio, this.scalingReferenceResolution.y);
			base.transform.localScale = scale * Vector3.one;
		}

		// Token: 0x0600235F RID: 9055 RVA: 0x000917D0 File Offset: 0x0008F9D0
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest) && !this.IsTransitioningIn() && !this.IsTransitioningOut() && !this._hasSoakTestTransitionedOut && this._soakTestCountdown > 0f)
			{
				this._soakTestCountdown -= deltaTime;
				if ((int)this._soakTestCountdown != (int)(this._soakTestCountdown + deltaTime))
				{
					Diagnostics.Log.Message(Diagnostics.Log.Level.Info, "Soak", "GameOverScreen.Tick() soak countdown down to {0}.", new object[]
					{
						this._soakTestCountdown
					});
				}
				if (this._soakTestCountdown <= 0f)
				{
					this._soakTestCountdown = 2f;
					if (global::Random.Bool())
					{
						Diagnostics.Log.Message(Diagnostics.Log.Level.Info, "Soak", "GameOverScreen restarting.", Array.Empty<object>());
						this.OnRestart();
						return;
					}
					Diagnostics.Log.Message(Diagnostics.Log.Level.Info, "Soak", "GameOverScreen quitting.", Array.Empty<object>());
					this.OnQuit();
				}
			}
		}

		// Token: 0x06002360 RID: 9056 RVA: 0x000918BB File Offset: 0x0008FABB
		public override void Reset()
		{
			base.Reset();
			this._doTransitionAnimation = false;
			this.focusPoint = default(Vector3);
			this._soakTestCountdown = -1f;
			this._hasSoakTestTransitionedOut = false;
			this._hasNewHighScore = false;
		}

		// Token: 0x06002361 RID: 9057 RVA: 0x000918F0 File Offset: 0x0008FAF0
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
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x000919E4 File Offset: 0x0008FBE4
		public void OnChallengeButtonsPressed()
		{
			ActiveChallengesModel challengeModel = this._game.Simulation.GetModel<ActiveChallengesModel>();
			this._screenStack.PushScreen<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, delegate(ChallengeInfoScreen screen)
			{
				screen.PrepareScreen(challengeModel.challengeType, challengeModel.challenges, challengeModel.timeStart, challengeModel.timeEnd, StringId.Continue, false, true, this._game.Scope, true);
			}, false, null, true, null);
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x00091A34 File Offset: 0x0008FC34
		public override void TransitionInTick()
		{
			float lerp = this.TransitionInPercentage();
			float alpha = this.blurTransitionCurve.Evaluate(lerp);
			this._canvasGroup.Alpha = alpha;
			if (this._gameOverTextContainer.IsAnimating)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(this._gameOverTextContainer.GetComponent<RectTransform>());
				this._gameOverTextContainer.Snap();
			}
			if (!this._doTransitionAnimation)
			{
				lerp = 1f;
				alpha = this.blurTransitionCurve.Evaluate(lerp);
			}
			float movementLerp = this.movementTransitionCurve.Evaluate(lerp);
			Vector3 newPosition = Vector3.Lerp(this._transitionDetails.spline.inPoint, this.focusPoint, movementLerp);
			newPosition = Vector3.Lerp(this._gameCamera.transform.position, newPosition, this.TransitionInPercentage() * 5f);
			this._gameCamera.SetPosition(newPosition);
			float rotationLerp = this.rotationTransitionCurve.Evaluate(lerp);
			this._gameCamera.transform.rotation = this._transitionDetails.spline.EvaluateRotation(rotationLerp);
			float endZoom = this._screenStack.GetZoomFor(base.ScreenType);
			float zoomLerp = this.zoomTransitionCurve.Evaluate(lerp);
			this._gameCamera.OrthographicSize = Mathf.Lerp(this._previousCameraZoom, endZoom, zoomLerp);
			newPosition = this._transitionDetails.spline.outPoint;
			newPosition.z = base.transform.position.z;
			base.transform.position = newPosition;
			base.transform.rotation = this._transitionDetails.spline.endRotation;
			this.SetupWorldSpaceCanvas(endZoom);
			this._gameCamera.customBlur.Strength = alpha;
			this._navigation.SetNewFocus(null);
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x00091BE8 File Offset: 0x0008FDE8
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._doTransitionAnimation = (inScreen != ScreenStack.MotorwaysScreen.ChallengeInfo);
			this._alignToCamera = false;
			this._scaleToCamera = false;
			if (this._doTransitionAnimation)
			{
				this.photoModeButtonAnchor.SetActive(false);
			}
			this._skipTransitions = (this._skipTransitions && inScreen != ScreenStack.MotorwaysScreen.Photo && inScreen != ScreenStack.MotorwaysScreen.InGame);
			this._hasSoakTestTransitionedOut = true;
			this.submitDiagnosticsReportModal.HideModal();
		}

		// Token: 0x06002365 RID: 9061 RVA: 0x00091C5C File Offset: 0x0008FE5C
		public override void TransitionOutTick()
		{
			base.TransitionOutTick();
			float alpha = Easings.QuarticEaseIn(Mathf.Clamp01((1f - this.TransitionOutPercentage()) * 2f - 0.5f));
			this._canvasGroup.Alpha = alpha;
			if (this._doTransitionAnimation)
			{
				this._gameCamera.customBlur.Strength = alpha;
			}
		}

		// Token: 0x06002366 RID: 9062 RVA: 0x00091CB7 File Offset: 0x0008FEB7
		public void OnCameraMode()
		{
			this._screenStack.PushScreen(ScreenStack.MotorwaysScreen.Photo, false, this._gameScope, true);
		}

		// Token: 0x06002367 RID: 9063 RVA: 0x00091CD0 File Offset: 0x0008FED0
		public void OnRestart()
		{
			GameContainerScreen gameContainerScreen = this._screenStack.GetActiveScreen<GameContainerScreen>();
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._gameScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			if (Diagnostics.Verify(gameContainerScreen != null, "We don't have an active GameContainerScreen even though we're at the GameOverScreen!"))
			{
				GameMode gameMode = this._gameScope.Get<MotorwaysGame>().StartedWithGameMode;
				gameContainerScreen.PrepareForRestartMap(gameMode);
			}
			this._canvasGroup.Alpha = 0f;
			this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.InGame, false);
			this._appScope.Get<ISoftwareCapabilities>().SetIsInGame(true);
		}

		// Token: 0x06002368 RID: 9064 RVA: 0x00091D50 File Offset: 0x0008FF50
		public void OnContinueInEndless()
		{
			GameContainerScreen gameContainerScreen = this._screenStack.GetActiveScreen<GameContainerScreen>();
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._gameScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			if (Diagnostics.Verify(gameContainerScreen != null, "We don't have an active GameContainerScreen even though we're at the GameOverScreen!"))
			{
				gameContainerScreen.PrepareForContinueInEndless();
			}
			this._canvasGroup.Alpha = 0f;
			this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.InGame, false);
			this._appScope.Get<ISoftwareCapabilities>().SetIsInGame(true);
			this._gameScope.Get<GameUIScreen>().ResetForceHiddenState();
			this._gameScope.Get<ScoreModel>().ResetForEndless();
		}

		// Token: 0x06002369 RID: 9065 RVA: 0x00091DE0 File Offset: 0x0008FFE0
		public void OnQuit()
		{
			this._game.StopAudio();
			ScreenStack.MotorwaysScreen desiredScreen;
			if (this._gameScope.Get<City>().Rules is TutorialGameRules)
			{
				if (!this._screenStack.IsScreenActive<MainMenuScreen>())
				{
					desiredScreen = ScreenStack.MotorwaysScreen.MainMenu;
					this._screenStack.ReplaceScreens<MainMenuScreen>(desiredScreen, typeof(GameContainerScreen), null, true);
				}
				else
				{
					desiredScreen = this._screenStack.GetScreenTypeBelowScreenType(ScreenStack.MotorwaysScreen.InGame);
					this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.InGame, true);
				}
			}
			else if (!this._screenStack.IsScreenActive<MapSelectScreen>())
			{
				desiredScreen = ScreenStack.MotorwaysScreen.MapSelect;
				this._screenStack.ReplaceScreens<MapSelectScreen>(desiredScreen, delegate(MapSelectScreen mapSelectScreen)
				{
					mapSelectScreen.PrepareScreen(this._game, false, false);
				}, typeof(GameContainerScreen), null, true);
			}
			else if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				this.popupStack.PushPopup<AppleDemoCardPopup>(0f, false).Initialise(false);
				desiredScreen = ScreenStack.MotorwaysScreen.MainMenu;
				this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.MainMenu, false);
			}
			else
			{
				desiredScreen = this._screenStack.GetScreenTypeBelowScreenType(ScreenStack.MotorwaysScreen.InGame);
				this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.InGame, true);
			}
			StartupScreen startupScreen = this._screenStack.GetActiveScreen<StartupScreen>();
			float duration = this._screenStack.GetTransitionDetailsFrom(base.ScreenType, desiredScreen).duration;
			if (startupScreen != null)
			{
				this._themeDatabase.SetCurrentMapDefinition(startupScreen.mapDefinition, duration);
			}
			this._appScope.Get<ISoftwareCapabilities>().SetIsInGame(false);
		}

		// Token: 0x0600236A RID: 9066 RVA: 0x00091F30 File Offset: 0x00090130
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

		// Token: 0x04001D5A RID: 7514
		[Dependency]
		private IScope _scope;

		// Token: 0x04001D5B RID: 7515
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001D5C RID: 7516
		[SerializeField]
		private float _timeToFillText = 1f;

		// Token: 0x04001D5D RID: 7517
		[SerializeField]
		private float _buttonAnimationDelay = 2f;

		// Token: 0x04001D5E RID: 7518
		[SerializeField]
		private float _challengeIconsAdditionalDelay = 1f;

		// Token: 0x04001D5F RID: 7519
		[SerializeField]
		private float _delayToFillText = 1f;

		// Token: 0x04001D60 RID: 7520
		[SerializeField]
		private float _timeToFillButton = 0.25f;

		// Token: 0x04001D61 RID: 7521
		[SerializeField]
		private float _timeToAlphaButtonText = 0.25f;

		// Token: 0x04001D62 RID: 7522
		[SerializeField]
		private ChallengeIcon[] _challengeIcons;

		// Token: 0x04001D63 RID: 7523
		[SerializeField]
		private Animator _challengeIconContainer;

		// Token: 0x04001D64 RID: 7524
		private static readonly int AnimateInStateID = Animator.StringToHash("AnimateIn");

		// Token: 0x04001D65 RID: 7525
		private static readonly int NormalTriggerID = Animator.StringToHash("Normal");

		// Token: 0x04001D66 RID: 7526
		[SerializeField]
		private Animator _uiPing;

		// Token: 0x04001D67 RID: 7527
		private static readonly int PingTriggerID = Animator.StringToHash("Ping");

		// Token: 0x04001D68 RID: 7528
		[SerializeField]
		private FloatingElement _gameOverTextContainer;

		// Token: 0x04001D69 RID: 7529
		public Vector3 focusPoint;

		// Token: 0x04001D6A RID: 7530
		public LocalizedTextUI textTitle;

		// Token: 0x04001D6B RID: 7531
		public LocalizedTextUI textLineOne;

		// Token: 0x04001D6C RID: 7532
		public LocalizedTextUI textLineTwo;

		// Token: 0x04001D6D RID: 7533
		public LocalizedTextUI restartButtonText;

		// Token: 0x04001D6E RID: 7534
		public LocalizedTextUI exitButtonText;

		// Token: 0x04001D6F RID: 7535
		public LocalizedTextUI continueInEndlessText;

		// Token: 0x04001D70 RID: 7536
		public TouchButton restartButton;

		// Token: 0x04001D71 RID: 7537
		public TouchButton exitButton;

		// Token: 0x04001D72 RID: 7538
		public TouchButton continueInEndlessButton;

		// Token: 0x04001D73 RID: 7539
		public GameObject photoModeButtonAnchor;

		// Token: 0x04001D74 RID: 7540
		public TouchButton photoModeButton;

		// Token: 0x04001D75 RID: 7541
		private float _soakTestCountdown = -1f;

		// Token: 0x04001D76 RID: 7542
		private bool _hasSoakTestTransitionedOut;

		// Token: 0x04001D77 RID: 7543
		private Diagnostics.ReportUpload _reportUpload;

		// Token: 0x04001D78 RID: 7544
		public TouchButton submitDiagnosticsReportButton;

		// Token: 0x04001D79 RID: 7545
		public NewsletterModal submitDiagnosticsReportModal;

		// Token: 0x04001D7A RID: 7546
		public TouchButton finishSubmittingReportButton;

		// Token: 0x04001D7B RID: 7547
		public TextMeshProUGUI reportIdLabel;

		// Token: 0x04001D7C RID: 7548
		private const string TransitionDetails = "Transition Details";

		// Token: 0x04001D7D RID: 7549
		[FoldoutGroup("Transition Details")]
		public AnimationCurve movementTransitionCurve;

		// Token: 0x04001D7E RID: 7550
		[FoldoutGroup("Transition Details")]
		public AnimationCurve rotationTransitionCurve;

		// Token: 0x04001D7F RID: 7551
		[FoldoutGroup("Transition Details")]
		public AnimationCurve zoomTransitionCurve;

		// Token: 0x04001D80 RID: 7552
		[FoldoutGroup("Transition Details")]
		public AnimationCurve blurTransitionCurve;

		// Token: 0x04001D81 RID: 7553
		public Vector2 scalingReferenceResolution = new Vector2(1920f, 1080f);

		// Token: 0x04001D82 RID: 7554
		private bool _doTransitionAnimation;

		// Token: 0x04001D83 RID: 7555
		private bool _hasNewHighScore;
	}
}
