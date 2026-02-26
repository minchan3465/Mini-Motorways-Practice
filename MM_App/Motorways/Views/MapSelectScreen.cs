using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Client;
using Factory;
using JetBrains.Annotations;
using Motorways.Audio;
using Motorways.Leaderboards;
using Motorways.Models;
using Motorways.UI;
using Motorways.UI.NewContentIndicators;
using NaughtyAttributes;
using Popups;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000552 RID: 1362
	public class MapSelectScreen : ScrollingButtonScreen
	{
		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06002455 RID: 9301 RVA: 0x000966D1 File Offset: 0x000948D1
		// (set) Token: 0x06002456 RID: 9302 RVA: 0x000966D9 File Offset: 0x000948D9
		public LeaderboardType? PlayerSelectedLeaderboardType { get; set; }

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06002457 RID: 9303 RVA: 0x000966E2 File Offset: 0x000948E2
		private bool ShouldPushGameScreen
		{
			get
			{
				return this._mapLoadedForGameScreen && this._popupHidden;
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06002458 RID: 9304 RVA: 0x000966F4 File Offset: 0x000948F4
		public MapButton CurrentlySelectedMapButton
		{
			get
			{
				return base.CurrentlySelectedButton.GetComponent<MapButton>();
			}
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06002459 RID: 9305 RVA: 0x00096701 File Offset: 0x00094901
		public IEnumerable<MapButton> MapButtons
		{
			get
			{
				foreach (AnimatedCard button in this.buttons)
				{
					yield return button.GetComponent<MapButton>();
				}
				List<AnimatedCard>.Enumerator enumerator = default(List<AnimatedCard>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x0600245A RID: 9306 RVA: 0x00096714 File Offset: 0x00094914
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			bool flag = base.IsVisible();
			if (flag)
			{
				this._gameCamera.transform.position = base.GetCameraPosition();
			}
			if (this.ShouldPushGameScreen)
			{
				this.HideUnselectedButtons();
				this._screenStack.PushScreen<GameContainerScreen>(ScreenStack.MotorwaysScreen.InGame, delegate(GameContainerScreen newScreen)
				{
					this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.MenuExit, 0.5f, -1f, true, null));
					this._selectedChallengeIndex = this.CurrentlySelectedMapButton.SelectedChallengeIndex;
					GameMode mode = this.CurrentlySelectedMapButton.GetCurrentSelectedGameMode();
					newScreen.PrepareForMap(UnityEngine.Object.Instantiate<GameObject>(this._cityDefinition.asset as GameObject).GetComponent<CityDefinition>(), this.CurrentlySelectedMapButton.MapDefinition, mode, this.CurrentlySelectedMapButton.MapChallenge, false);
					this.CurrentlySelectedMapButton.DeselectCityChallenge();
				}, false, null, true, null);
				this._cityDefinition = null;
				this.scrollRect.enabled = true;
				this._mapLoadedForGameScreen = false;
				if (FeatureToggle.IsFeatureEnabled(Feature.RandomChallengesMapButton) && this.CurrentlySelectedMapButton.IsRandomChallengeCard)
				{
					this.CurrentlySelectedMapButton.AssignRandomMapChallenge();
				}
			}
			if (this._popupHidden && this._timerTillTransition >= 0f)
			{
				this._timerTillTransition -= deltaTime;
			}
			if (this._cityDefinition != null && this._cityDefinition.HasValue && this._timerTillTransition < 0f)
			{
				if (this._skipTransitions)
				{
					this._screenStack.FadeNextTransition(this.skippedTransitionFadeDuration);
				}
				this._mapLoadedForGameScreen = true;
			}
			if (flag)
			{
				if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest) && !this.IsTransitioningIn() && !this.IsTransitioningOut() && this._soakTestCountdown > 0f)
				{
					this._soakTestCountdown -= deltaTime;
					if (this._soakTestCountdown <= 0f)
					{
						this._soakTestCountdown = -1f;
						this.ScrollToButton(global::Random.AnyItem<AnimatedCard>(this.buttons), true);
						this.SelectCurrentMap();
					}
				}
				int nearestButtonIndex = base.GetNearestButtonIndex();
				bool oldInteractable = this.firstFocus.interactable;
				MapButton nearestMapButton = this.MapButtonAt(nearestButtonIndex);
				if (nearestMapButton != null)
				{
					bool isPlayButtonNowInteractable = !nearestMapButton.IsLocked && (nearestMapButton.CurrentCard != MapButton.Card.Challenge || nearestMapButton.SelectedChallenge != null) && !this._isPlayingAnimation;
					float playButtonAlpha = isPlayButtonNowInteractable ? 1f : 0.5f;
					if (oldInteractable != isPlayButtonNowInteractable)
					{
						if (isPlayButtonNowInteractable && this._playHighlightedWhenLastActive && EventSystem.current.currentSelectedGameObject == null)
						{
							EventSystem.current.SetSelectedGameObject(this.firstFocus.gameObject);
						}
						else
						{
							this._playHighlightedWhenLastActive = (EventSystem.current.currentSelectedGameObject == this.firstFocus.gameObject);
						}
						this.firstFocus.interactable = isPlayButtonNowInteractable;
						Color oldColor = this._playButtonTextMeshPro.color;
						this._playButtonTextMeshPro.color = new Color(oldColor.r, oldColor.b, oldColor.g, playButtonAlpha);
					}
					bool shouldShowChallengeIcon = nearestMapButton.Type == MapButton.MapButtonType.DailyChallenge || nearestMapButton.Type == MapButton.MapButtonType.WeeklyChallenge || nearestMapButton.IsRandomChallengeCard || nearestMapButton.CurrentCard == MapButton.Card.Challenge;
					this.firstFocus.animator.SetBool(MapSelectScreen.ShouldShowChallengeIcon, shouldShowChallengeIcon);
					bool shouldShowEndlessIcon = nearestMapButton.CurrentCard != MapButton.Card.Challenge && nearestMapButton.GetCurrentSelectedGameMode() == GameMode.Endless;
					this.firstFocus.animator.SetBool(MapSelectScreen.ShouldShowEndlessIcon, shouldShowEndlessIcon);
					bool shouldShowExpertIcon = nearestMapButton.CurrentCard != MapButton.Card.Challenge && nearestMapButton.GetCurrentSelectedGameMode() == GameMode.Expert;
					this.firstFocus.animator.SetBool(MapSelectScreen.ShouldShowExpertIcon, shouldShowExpertIcon);
					bool shouldShowCreativeIcon = nearestMapButton.CurrentCard != MapButton.Card.Challenge && nearestMapButton.GetCurrentSelectedGameMode() == GameMode.Creative;
					this.firstFocus.animator.SetBool(MapSelectScreen.ShouldShowCreativeIcon, shouldShowCreativeIcon);
					this._playButtonChallengeIcon.SetAlpha(playButtonAlpha);
					this._playButtonEndlessIcon.SetAlpha(playButtonAlpha);
					this._playButtonExpertIcon.SetAlpha(playButtonAlpha);
					this._playButtonCreativeIcon.SetAlpha(playButtonAlpha);
					if (nearestMapButton.PlayTextStringId != this._playButtonStringId)
					{
						this._playButtonStringId = nearestMapButton.PlayTextStringId;
						MotorwaysStringKey playStringKey = this._appScope.Get<MotorwaysStringKey>();
						playStringKey.InitWithStringId(this._playButtonStringId);
						this._playButtonText.LocString = StandaloneLocString.CreateString(this._appScope, playStringKey);
					}
				}
				if (this._handleDeepLinkOnTransition && this._screenStack.HasVisibleScreens())
				{
					this._handleDeepLinkOnTransition = false;
					this._blurWhileTransitioning = false;
					this.SelectMap(this.CurrentlySelectedMapButton);
				}
			}
		}

		// Token: 0x0600245B RID: 9307 RVA: 0x00096AF4 File Offset: 0x00094CF4
		private MapButton MapButtonAt(int index)
		{
			if (!Diagnostics.Verify(index >= 0 && index < this.buttons.Count, "Unexpected index of {0} when we have a count of {1}", index, this.buttons.Count))
			{
				return null;
			}
			return this.buttons[index].GetComponent<MapButton>();
		}

		// Token: 0x0600245C RID: 9308 RVA: 0x00096B4C File Offset: 0x00094D4C
		public MapButton GetPreviousButton(MapButton button)
		{
			MapButton result = null;
			int index = this.buttons.IndexOf(button) - 1;
			if (index >= 0 && index < this.buttons.Count)
			{
				result = this.MapButtonAt(index);
			}
			return result;
		}

		// Token: 0x0600245D RID: 9309 RVA: 0x00096B88 File Offset: 0x00094D88
		public MapButton GetNextButton(MapButton button)
		{
			MapButton result = null;
			int index = this.buttons.IndexOf(button) + 1;
			if (index >= 0 && index < this.buttons.Count)
			{
				result = this.MapButtonAt(index);
			}
			return result;
		}

		// Token: 0x0600245E RID: 9310 RVA: 0x00096BC4 File Offset: 0x00094DC4
		public void PrepareScreen(Game currentGame = null, bool handleDeeplinkChallenge = false, bool changeBlurWhenTransitioning = false)
		{
			this._blurWhileTransitioning = changeBlurWhenTransitioning;
			base.RegisterAllLocalizedTextChildren();
			this.CreateMapButtons();
			base.AssignOriginPosition();
			if (currentGame != null)
			{
				MapButton currentMapButton = null;
				MapButton firstNonChallengeButton = null;
				ActiveChallengesModel challenges = currentGame.Simulation.GetModel<ActiveChallengesModel>();
				MapChallenge.ChallengeType activeChallengeType = challenges.challengeType;
				this._selectedChallengeIndex = challenges.cityChallengeIndex;
				foreach (MapButton mapButton in this.MapButtons)
				{
					if (activeChallengeType != MapChallenge.ChallengeType.None && mapButton.MapChallenge != null && mapButton.MapChallenge.type == activeChallengeType)
					{
						currentMapButton = mapButton;
						break;
					}
					if ((activeChallengeType == MapChallenge.ChallengeType.None || activeChallengeType == MapChallenge.ChallengeType.City) && (mapButton.MapChallenge == null || mapButton.MapChallenge.type == MapChallenge.ChallengeType.None) && mapButton.MapDefinition.cityName == this._gameContainer.CurrentCityName)
					{
						currentMapButton = mapButton;
						break;
					}
					if (firstNonChallengeButton == null && mapButton.Type == MapButton.MapButtonType.City)
					{
						firstNonChallengeButton = mapButton;
					}
				}
				if (currentMapButton == null && firstNonChallengeButton != null)
				{
					currentMapButton = firstNonChallengeButton;
				}
				if (Diagnostics.Verify(currentMapButton != null, "Game {0} passed to PrepareScreen but we failed to find a map button matching current game. City: {1}. Challenge: {2}", currentGame, this._gameContainer.CurrentCityName, activeChallengeType))
				{
					this.ScrollToButton(currentMapButton, true);
				}
			}
			if (handleDeeplinkChallenge)
			{
				this.PrepareScreenForDeeplinkChallenge();
			}
		}

		// Token: 0x0600245F RID: 9311 RVA: 0x00096D18 File Offset: 0x00094F18
		public void PrepareScreenForDeeplinkChallenge()
		{
			if (this.popupStack.HasActivePopups && this.popupStack.GetTopPopup().CanBeDismissed())
			{
				this.popupStack.PopPopup(true);
			}
			MapButton challengeMap = null;
			foreach (MapButton button in this.MapButtons)
			{
				if (button.MapChallenge == null && string.Equals(button.MapDefinition.cityName, this._deepLinkProcessor.challengeMap, StringComparison.OrdinalIgnoreCase))
				{
					challengeMap = button;
					break;
				}
			}
			if (!Diagnostics.Verify(challengeMap != null, "attempting to deep link to " + this._deepLinkProcessor.challengeMap + " but no map button was found"))
			{
				return;
			}
			GameMode gameMode = this._deepLinkProcessor.challengeMode;
			this._player.SetSelectedGameMode(challengeMap.MapDefinition.mapName, gameMode);
			this.ScrollToButton(challengeMap, true);
			this._handleDeepLinkOnTransition = true;
		}

		// Token: 0x06002460 RID: 9312 RVA: 0x00096E10 File Offset: 0x00095010
		private void RefreshChallengeOverridesFromServer()
		{
			this._challengeSystem.RefreshOverridesFromServer(delegate(ChallengeOverrides.RefreshResult result, ChallengeSystem.RefreshOverridesDetails details)
			{
				if (result == ChallengeOverrides.RefreshResult.Success)
				{
					MapButton weeklyChallengeButton;
					if ((details & ChallengeSystem.RefreshOverridesDetails.NewWeeklyChallenge) != ChallengeSystem.RefreshOverridesDetails.None && this.TryGetButtonOfType(MapButton.MapButtonType.WeeklyChallenge, out weeklyChallengeButton))
					{
						this.OnChallengeExpired(weeklyChallengeButton);
					}
					MapButton dailyChallengeButton;
					if ((details & ChallengeSystem.RefreshOverridesDetails.NewDailyChallenge) != ChallengeSystem.RefreshOverridesDetails.None && this.TryGetButtonOfType(MapButton.MapButtonType.DailyChallenge, out dailyChallengeButton))
					{
						this.OnChallengeExpired(dailyChallengeButton);
					}
				}
			});
		}

		// Token: 0x06002461 RID: 9313 RVA: 0x00096E2C File Offset: 0x0009502C
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			this._previousScreen = outScreen;
			this.RefreshChallengeOverridesFromServer();
			foreach (MapButton button in this.MapButtons)
			{
				if (this._previouslyLockedMapButtons.Contains(button.MapDefinition) && !button.MapDefinition.IsLocked(this._appScope) && !button.IsChallengeMapButton())
				{
					this._buttonsToUnlockOnTransitioned.Add(button);
				}
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				this._buttonsToUnlockOnTransitioned.Clear();
			}
			if (this._buttonsToUnlockOnTransitioned.Count > 0)
			{
				this._lastSelectedButtonBeforeTransitionOut = this.CurrentlySelectedMapButton;
				MapButton firstUnlockedMap = this._buttonsToUnlockOnTransitioned[0];
				this.ScrollToButton(firstUnlockedMap, true);
				firstUnlockedMap.ShowCard(firstUnlockedMap.CurrentCard);
				base.SetMapButtonValues(this.scrollRect.normalizedPosition);
			}
			foreach (MapButton button2 in this.MapButtons)
			{
				if (this._previouslyLockedCityChallengeMapButtons.Contains(button2.MapDefinition) && !button2.MapDefinition.IsCityChallengeLocked(this._appScope))
				{
					this._buttonToUnlockCityChallengeOnTransitioned = button2;
					if (this._buttonsToUnlockOnTransitioned.Count == 0)
					{
						this.ScrollToButton(button2, true);
						base.SetMapButtonValues(this.scrollRect.normalizedPosition);
					}
				}
			}
			if (!this._player.HasSeenNewContent(MapButtonModeSelectCard.GetUnlockAnimationNciID(this.CurrentlySelectedMapButton.MapDefinition)) && this.CurrentlySelectedMapButton.MapDefinition.IsExpertModeUnlocked(this._appScope) && !this.CurrentlySelectedMapButton.IsRandomChallengeCard && this.CurrentlySelectedMapButton.Type == MapButton.MapButtonType.City)
			{
				this._buttonToUnlockExpertModeOnTransitioned = this.CurrentlySelectedMapButton;
				if (FeatureToggle.IsFeatureDisabled(Feature.MapUnlocks))
				{
					this._buttonToUnlockExpertModeOnTransitioned = null;
				}
			}
			this.SavePreviouslyLockedMaps();
			this.TrySubmitOutstandingScores();
			this.HACK_CompleteMapScore300AchievementFallback();
			base.TransitionIn(outScreen);
			if (this._originPosition == null)
			{
				Vector3 newPosition = this._screenStack.GetPositionFor(base.ScreenType);
				newPosition.z = -0.25f;
				newPosition.x -= this.scrollRect.horizontalNormalizedPosition * this.buttonParent.sizeDelta.x * base.transform.localScale.x * 0.805f;
				this._originPosition = new Vector3?(newPosition);
				base.transform.position = newPosition;
			}
			foreach (MapButton button3 in this.MapButtons)
			{
				if (!button3.IsHidden)
				{
					button3.CanvasGroup.Alpha = 1f;
				}
				button3.CanvasGroup.SetInteractable(true);
				int bestScore = -1;
				switch (button3.Type)
				{
				case MapButton.MapButtonType.City:
				{
					GameMode gameMode = button3.GetCurrentSelectedGameMode();
					bestScore = this.GetBestScoreForCityLeaderboard(button3.MapDefinition.cityName, gameMode);
					break;
				}
				case MapButton.MapButtonType.DailyChallenge:
					bestScore = this.GetBestScoreForChallenge(MapChallenge.ChallengeType.Daily);
					break;
				case MapButton.MapButtonType.WeeklyChallenge:
					bestScore = this.GetBestScoreForChallenge(MapChallenge.ChallengeType.Weekly);
					break;
				}
				button3.SetBestScoreTextOnMainCard(this._appScope, bestScore);
				button3.RefreshTabs();
			}
			if (this._selectedChallengeIndex != -1)
			{
				this.CurrentlySelectedMapButton.SetupFrontCardForCityChallenge(this._selectedChallengeIndex);
			}
			this.ScrollToButton(base.CurrentlySelectedButton, true);
			this.CurrentlySelectedMapButton.SetSelected(true);
			foreach (MapButton button4 in this.MapButtons)
			{
				button4.EnsureThemeButtonSelectedState(new MotorwaysThemePreference?(this._themeDatabase.ThemePreference));
				GameMode gameMode2 = this._player.GetSelectedModeForMap(button4.MapDefinition.mapName);
				button4.MainCard.UpdateModeStrings(gameMode2);
			}
			if (this._screenStack.IsScreenActive(ScreenStack.MotorwaysScreen.InGame))
			{
				MotorwaysGame game = (MotorwaysGame)this._screenStack.GetActiveScreen<GameContainerScreen>().GetActiveGame();
				this._cityCameraTransitionHandle = game.StartedWithCityDefinition.cameraZoom.cameraEntrySplineHandle;
				this._cityCameraTransitionPosition = game.StartedWithCityDefinition.cameraZoom.cameraEntryPosition;
				this._transitioningFromGameScreen = true;
			}
			else
			{
				this._transitioningFromGameScreen = false;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.ChallengeTimeControl) && !(GameDateTime.Backend is AdjustableGameDateTime))
			{
				GameDateTime.Backend = new AdjustableGameDateTime();
			}
			this._soakTestCountdown = 0.5f;
		}

		// Token: 0x06002462 RID: 9314 RVA: 0x000972B0 File Offset: 0x000954B0
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			if (inScreen == ScreenStack.MotorwaysScreen.MainMenu)
			{
				this._previouslyLockedMapButtons.Clear();
				this._previouslyLockedCityChallengeMapButtons.Clear();
			}
		}

		// Token: 0x06002463 RID: 9315 RVA: 0x000972D4 File Offset: 0x000954D4
		private void HACK_CompleteMapScore300AchievementFallback()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				return;
			}
			for (int mapButtonIndex = 0; mapButtonIndex < this.buttons.Count; mapButtonIndex++)
			{
				MapButton mapButton = this.MapButtonAt(mapButtonIndex);
				if (mapButton.MapDefinition.CityNameEnum == MapDefinition.CityNames.DarEsSalaam && mapButton.IsLocked && (this.GetBestScoreForCityLeaderboard(MapDefinition.CityNames.LosAngeles.ToString(), GameMode.Normal) >= 300 || this.GetBestScoreForCityLeaderboard(MapDefinition.CityNames.Beijing.ToString(), GameMode.Normal) >= 300 || this.GetBestScoreForCityLeaderboard(MapDefinition.CityNames.Tokyo.ToString(), GameMode.Normal) >= 300))
				{
					if (!this._buttonsToUnlockOnTransitioned.Contains(mapButton))
					{
						Diagnostics.FailAssert("Map locked even though requirement complete. TotalPointsScored {0}", new object[]
						{
							this._player.AchievementStatistics.TotalPointsScored
						});
						mapButton.HackSetUnlocked();
					}
					AchievementDefinition achievement_300 = null;
					for (int achievementIndex = 0; achievementIndex < this._achievements.Count; achievementIndex++)
					{
						AchievementDefinition achievement = this._achievements[achievementIndex];
						if (achievement.Id == "Map_Score_300")
						{
							achievement_300 = achievement;
						}
					}
					if (achievement_300 != null && !this._player.IsAchievementCompleted(achievement_300))
					{
						this._player.CompleteAchievement(achievement_300, false);
					}
				}
			}
		}

		// Token: 0x06002464 RID: 9316 RVA: 0x00097428 File Offset: 0x00095628
		private IEnumerator RunUnlockAnimations()
		{
			this._canvasGroup.SetInteractable(false);
			this._inputState.BlockAllInput = true;
			this._isPlayingAnimation = true;
			bool skipScroll = this._player.IsSkipTransitionsEnabled;
			bool isFirstButton = true;
			foreach (MapButton button in this._buttonsToUnlockOnTransitioned)
			{
				MapButton previousButton = this.GetPreviousButton(button);
				MapButton nextButton = this.GetNextButton(button);
				if (skipScroll)
				{
					button.SetUnlocked();
					button.SetupButtonNavigation();
					if (previousButton != null)
					{
						previousButton.SetupButtonNavigation();
					}
					if (nextButton != null)
					{
						nextButton.SetupButtonNavigation();
					}
					yield return new WaitForSeconds(this._scrollToFlipTime);
					button.FlipCard();
					yield return new WaitForSeconds(this._scrollWaitTime);
				}
				else
				{
					if (isFirstButton)
					{
						this.ScrollToButton(button, false);
						yield return new WaitForSeconds(this._unlockAnimationTimeToScrollToFirstElement);
						isFirstButton = false;
					}
					button.SetUnlocked();
					button.SetupButtonNavigation();
					if (previousButton != null)
					{
						previousButton.SetupButtonNavigation();
					}
					if (nextButton != null)
					{
						nextButton.SetupButtonNavigation();
					}
					yield return new WaitForSeconds(this._unlockToScrollTime);
					if (nextButton != null)
					{
						if (this._buttonsToUnlockOnTransitioned.Contains(nextButton))
						{
							this.ScrollToButton(nextButton, false);
						}
						else
						{
							int nextButtonIndex = this._buttonsToUnlockOnTransitioned.IndexOf(button) + 1;
							if (this._buttonsToUnlockOnTransitioned.Count > nextButtonIndex)
							{
								this.ScrollToButton(this._buttonsToUnlockOnTransitioned[nextButtonIndex], false);
								yield return new WaitForSeconds(this._scrollToFlipTime);
							}
						}
					}
					yield return new WaitForSeconds(this._scrollToFlipTime);
					button.FlipCard();
					yield return new WaitForSeconds(this._scrollWaitTime);
					previousButton = null;
					nextButton = null;
					button = null;
				}
			}
			List<MapButton>.Enumerator enumerator = default(List<MapButton>.Enumerator);
			if (!skipScroll)
			{
				yield return new WaitForSeconds(this._unlockAnimationEndDelay);
				this.ScrollToButton(this._lastSelectedButtonBeforeTransitionOut, skipScroll);
				yield return new WaitForSeconds(this._unlockAnimationTimeToScrollToFirstElement);
			}
			foreach (MapButton button2 in this._buttonsToUnlockOnTransitioned)
			{
				this._player.ClearNewContentSeen(button2.NewContentId);
				button2.ShowNewContentIndicatorIfNeeded(false);
			}
			if (this._buttonToUnlockCityChallengeOnTransitioned != null)
			{
				this._buttonToUnlockCityChallengeOnTransitioned.ShowCard(MapButton.Card.Challenge);
				this._buttonToUnlockCityChallengeOnTransitioned = null;
			}
			this._buttonsToUnlockOnTransitioned.Clear();
			this._inputState.BlockAllInput = false;
			this._isPlayingAnimation = false;
			this._canvasGroup.SetInteractable(true);
			yield break;
			yield break;
		}

		// Token: 0x06002465 RID: 9317 RVA: 0x00097438 File Offset: 0x00095638
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this._scaleToCamera = false;
			MapButton mapButton;
			if (this._newContentDatabase.IsNewContent("DailyWeeklyChallengeCards", false) && this.TryGetButtonOfType(MapButton.MapButtonType.WeeklyChallenge, out mapButton) && this.TryGetButtonOfType(MapButton.MapButtonType.DailyChallenge, out mapButton))
			{
				base.StartCoroutine(this.TransitionInChallengeCards());
				this._newContentDatabase.SetNewContentSeen("DailyWeeklyChallengeCards");
			}
			MapButton weeklyChallengeButton;
			if (this.TryGetButtonOfType(MapButton.MapButtonType.WeeklyChallenge, out weeklyChallengeButton) && weeklyChallengeButton.HasExpired)
			{
				this.OnChallengeExpired(weeklyChallengeButton);
			}
			MapButton dailyChallengeButton;
			if (this.TryGetButtonOfType(MapButton.MapButtonType.DailyChallenge, out dailyChallengeButton) && dailyChallengeButton.HasExpired)
			{
				this.OnChallengeExpired(dailyChallengeButton);
			}
			if (this._buttonsToUnlockOnTransitioned.Count > 0)
			{
				base.StartCoroutine(this.RunUnlockAnimations());
			}
			else if (this._buttonToUnlockCityChallengeOnTransitioned != null)
			{
				int buttonIndex = base.IndexOf(this._buttonToUnlockCityChallengeOnTransitioned);
				this.MapButtonAt(buttonIndex).ShowCard(MapButton.Card.Challenge);
				this._buttonToUnlockCityChallengeOnTransitioned = null;
			}
			else if (this._buttonToUnlockExpertModeOnTransitioned != null)
			{
				int buttonIndex2 = base.IndexOf(this._buttonToUnlockExpertModeOnTransitioned);
				this.MapButtonAt(buttonIndex2).ShowCard(MapButton.Card.Mode);
				this._buttonToUnlockExpertModeOnTransitioned = null;
			}
			else if (this._selectedChallengeIndex != -1 && this._transitioningFromGameScreen)
			{
				this.CurrentlySelectedMapButton.ShowCard(MapButton.Card.Challenge);
				this.CurrentlySelectedMapButton.SelectedChallengeIndex = this._selectedChallengeIndex;
				this.CurrentlySelectedMapButton.LeaderboardShowsSelectedChallenge = true;
			}
			if ((this._storageService.Status.issues & PersistentStorageServiceIssues.QuotaExceeded) > PersistentStorageServiceIssues.None)
			{
				this.ShowiCloudStorageFullPopup();
			}
		}

		// Token: 0x06002466 RID: 9318 RVA: 0x000975A8 File Offset: 0x000957A8
		private IEnumerator WaitWhileThenExecute(Func<bool> predicate, Action action)
		{
			yield return new WaitWhile(predicate);
			if (action != null)
			{
				action();
			}
			yield break;
		}

		// Token: 0x06002467 RID: 9319 RVA: 0x000975C0 File Offset: 0x000957C0
		public void SavePreviouslyLockedMaps()
		{
			this._previouslyLockedMapButtons.Clear();
			this._previouslyLockedCityChallengeMapButtons.Clear();
			foreach (MapDefinition map in this._mapDatabase.MapLibrary.Maps)
			{
				if (map.IsLocked(this._appScope))
				{
					this._previouslyLockedMapButtons.Add(map);
				}
				if (map.IsCityChallengeLocked(this._appScope))
				{
					this._previouslyLockedCityChallengeMapButtons.Add(map);
				}
			}
		}

		// Token: 0x06002468 RID: 9320 RVA: 0x0009765C File Offset: 0x0009585C
		public override void TransitionInTick()
		{
			base.TransitionInTick();
			if (this._transitioningFromGameScreen)
			{
				if (this.TransitionInPercentage() > 1f - this._constants.PercentageOfDurationToUseForInitialMovement)
				{
					this._canvasGroup.Alpha = 1f;
				}
				else
				{
					this._canvasGroup.Alpha = 0f;
				}
				this._transitionDetails = this._screenStack.GetTransitionDetailsFrom(this._previousScreen, base.ScreenType);
				Vector3 newPosition = this._constants.GetCameraPositionForTransitionFromGame(this._transitionDetails, this.TransitionInPercentage(), this._cityCameraTransitionPosition, this._cityCameraTransitionHandle);
				this._gameCamera.SetPosition(newPosition);
			}
			if (this._blurWhileTransitioning)
			{
				this._gameCamera.customBlur.Strength = 1f - this.TransitionInPercentage();
			}
		}

		// Token: 0x06002469 RID: 9321 RVA: 0x00097728 File Offset: 0x00095928
		private bool TryGetButtonOfType(MapButton.MapButtonType buttonType, out MapButton result)
		{
			foreach (MapButton mapButton in this.MapButtons)
			{
				if (mapButton.Type == buttonType)
				{
					result = mapButton;
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x0600246A RID: 9322 RVA: 0x00097784 File Offset: 0x00095984
		private IEnumerator TransitionInChallengeCards()
		{
			if (this._skipTransitions)
			{
				yield return null;
			}
			int num;
			for (int buttonIndex = this._challengeButtonCount - 1; buttonIndex >= 0; buttonIndex = num)
			{
				this.MapButtonAt(buttonIndex).EnterFromHidden(null);
				yield return new WaitForSeconds(this._nextChallengeCardAppearDelay);
				num = buttonIndex - 1;
			}
			yield break;
		}

		// Token: 0x0600246B RID: 9323 RVA: 0x00097794 File Offset: 0x00095994
		public override void OnTransitionedOut()
		{
			this.firstFocus.animator.SetBool(MapSelectScreen.DroppedDown, false);
			this.firstFocus.animator.Update(1f);
			foreach (MapButton mapButton in this.MapButtons)
			{
				mapButton.ResetAnimations();
			}
			if (this._screenStack.GetTopActiveScreenType() == ScreenStack.MotorwaysScreen.ChallengeInfo)
			{
				this._overrideNextTransitionDuration = -1f;
				return;
			}
			base.CancelButtonScrolling();
			base.OnTransitionedOut();
		}

		// Token: 0x0600246C RID: 9324 RVA: 0x00097830 File Offset: 0x00095A30
		public void SelectMap(MapButton button)
		{
			this._currentlySelectedButtonIndex = base.IndexOf(button);
			bool shouldLoadMap = true;
			bool hasActiveDailyChallengeSaves = this._challengeSystem.GetActiveDailyChallengeSaves(this._player, true).Count > 0;
			bool hasActiveWeeklyChallengeSaves = this._challengeSystem.GetActiveWeeklyChallengeSaves(this._player, true).Count > 0;
			MotorwaysTimedChallengeScore challengeScore = this._player.GetChallengeScore(MapChallenge.ChallengeType.Daily, this._challengeSystem.DailyChallenge.TimeEnd);
			bool dailyChallengeIsLocked = challengeScore.ScoreState == LeaderboardScoreState.Locked;
			bool dailyChallengeIsEditable = challengeScore.ScoreState == LeaderboardScoreState.Editable;
			bool isSaveActive = this._player.HasLocalSavedGame;
			MapChallenge mapChallenge = button.MapChallenge;
			if (mapChallenge != null)
			{
				if (mapChallenge.type == MapChallenge.ChallengeType.Daily)
				{
					if (this._newContentDatabase.IsNewContent("DailyChallengeTutorialPopup", false))
					{
						this.ShowDailyChallengePopup(this.delayBeforeTransitioning);
					}
					else if (dailyChallengeIsLocked && !this._hasSeenDailyChallengeCompletePopUp && (!isSaveActive && !hasActiveWeeklyChallengeSaves))
					{
						shouldLoadMap = false;
						this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.DailyChallenge, null, delegate()
						{
							this._hasSeenDailyChallengeCompletePopUp = true;
							this.BeginTransitionIntoGame(button.MapDefinition);
						}, StringId.DailyChallenge_LockedConfirmation);
					}
					else if (hasActiveDailyChallengeSaves && dailyChallengeIsEditable)
					{
						shouldLoadMap = false;
						this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.DailyChallenge, null, delegate()
						{
							this.BeginTransitionIntoGame(button.MapDefinition);
						}, StringId.DailyChallenge_SaveGameConfirmation);
					}
					else if (isSaveActive || hasActiveWeeklyChallengeSaves)
					{
						shouldLoadMap = false;
						this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.StartNewGameHeader, null, delegate()
						{
							this.BeginTransitionIntoGame(button.MapDefinition);
						}, StringId.SaveGameOverwriteConfirmation);
					}
				}
				else if (hasActiveDailyChallengeSaves && dailyChallengeIsEditable)
				{
					shouldLoadMap = false;
					this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.DailyChallenge, null, delegate()
					{
						this.BeginTransitionIntoGame(button.MapDefinition);
					}, StringId.DailyChallenge_SaveGameConfirmationNewMap);
				}
				else if (mapChallenge.type == MapChallenge.ChallengeType.Weekly)
				{
					if (this._newContentDatabase.IsNewContent("WeeklyChallengeTutorialPopup", false))
					{
						this.ShowWeeklyChallengePopup(this.delayBeforeTransitioning);
					}
					else if (isSaveActive || hasActiveWeeklyChallengeSaves)
					{
						shouldLoadMap = false;
						this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.StartNewGameHeader, null, delegate()
						{
							this.BeginTransitionIntoGame(button.MapDefinition);
						}, StringId.SaveGameOverwriteConfirmation);
					}
				}
				else if (mapChallenge.type == MapChallenge.ChallengeType.City && !this._player.HasSeenNewContent("NewCityChallengeUnlockInfoPopup"))
				{
					this.ShowChallengeModeInfoPopup();
				}
				else if (isSaveActive || hasActiveWeeklyChallengeSaves)
				{
					shouldLoadMap = false;
					this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.StartNewGameHeader, null, delegate()
					{
						this.BeginTransitionIntoGame(button.MapDefinition);
					}, StringId.SaveGameOverwriteConfirmation);
				}
			}
			else if (hasActiveDailyChallengeSaves && dailyChallengeIsEditable)
			{
				shouldLoadMap = false;
				this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.DailyChallenge, null, delegate()
				{
					this.BeginTransitionIntoGame(button.MapDefinition);
				}, StringId.DailyChallenge_SaveGameConfirmationNewMap);
			}
			else if (isSaveActive || hasActiveWeeklyChallengeSaves)
			{
				shouldLoadMap = false;
				this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.StartNewGameHeader, null, delegate()
				{
					this.BeginTransitionIntoGame(button.MapDefinition);
				}, StringId.SaveGameOverwriteConfirmation);
			}
			if (shouldLoadMap)
			{
				this.BeginTransitionIntoGame(button.MapDefinition);
			}
		}

		// Token: 0x0600246D RID: 9325 RVA: 0x00097B94 File Offset: 0x00095D94
		private void BeginTransitionIntoGame(MapDefinition map)
		{
			for (int buttonIndex = 0; buttonIndex < this.buttons.Count; buttonIndex++)
			{
				if (this._currentlySelectedButtonIndex == buttonIndex)
				{
					this.MapButtonAt(buttonIndex).OnCardConfirmed();
				}
				else
				{
					float delay = (float)(Math.Abs(this._currentlySelectedButtonIndex - buttonIndex) - 1) * this.intervalBetweenButtonPushAnimations;
					delay = Mathf.Min(delay, 0.5f);
					this.MapButtonAt(buttonIndex).OnOtherCardConfirmed(buttonIndex < this._currentlySelectedButtonIndex, delay);
				}
			}
			this.firstFocus.animator.SetBool(MapSelectScreen.DroppedDown, true);
			this._cityDefinition = AssetBundleUtility.LoadPrefabAsync(map.mapAssetBundle, map.mapPrefabName, this);
			this.scrollRect.enabled = false;
			this._timerTillTransition = this.delayBeforeTransitioning;
		}

		// Token: 0x0600246E RID: 9326 RVA: 0x00097C4E File Offset: 0x00095E4E
		public void SelectCurrentMap()
		{
			base.ScrollToNearestButton();
			this.SelectMap(this.CurrentlySelectedMapButton);
		}

		// Token: 0x0600246F RID: 9327 RVA: 0x00097C62 File Offset: 0x00095E62
		public void UpdateTheme()
		{
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			this.ApplyTheme(this._themeDatabase.GetTheme());
		}

		// Token: 0x06002470 RID: 9328 RVA: 0x00097C88 File Offset: 0x00095E88
		public override void RegisterThemeComponents(ITheme theme)
		{
			base.RegisterThemeComponents(theme);
			foreach (AnimatedCard animatedCard in this.buttons)
			{
				animatedCard.RegisterThemeComponents();
			}
		}

		// Token: 0x06002471 RID: 9329 RVA: 0x00097CE0 File Offset: 0x00095EE0
		protected override void GetAutoThemeComponents(List<IThemeComponent> components)
		{
			List<GameObject> openObjects = new List<GameObject>();
			openObjects.Add(base.gameObject);
			GameObject mapButtonParent = this.buttonParent.gameObject;
			while (openObjects.Count > 0)
			{
				GameObject openObject = openObjects[openObjects.Count - 1];
				openObjects.RemoveAt(openObjects.Count - 1);
				IThemeComponent themeComponent = openObject.GetComponent<IThemeComponent>();
				if (themeComponent != null)
				{
					components.Add(themeComponent);
				}
				Transform openTransform = openObject.transform;
				int childCount = openTransform.childCount;
				for (int childIndex = 0; childIndex < childCount; childIndex++)
				{
					GameObject child = openTransform.GetChild(childIndex).gameObject;
					if (!(child == mapButtonParent))
					{
						openObjects.Add(child);
					}
				}
			}
		}

		// Token: 0x06002472 RID: 9330 RVA: 0x00097D88 File Offset: 0x00095F88
		public int GetBestScoreForCityLeaderboard(string cityId, GameMode mode)
		{
			MotorwaysCityStatistics cityStatisticsForCity = this._player.GetCityStatisticsForCity(cityId, mode, false);
			if (cityStatisticsForCity == null)
			{
				return 0;
			}
			return cityStatisticsForCity.MaxTrips;
		}

		// Token: 0x06002473 RID: 9331 RVA: 0x00097DA4 File Offset: 0x00095FA4
		private int GetBestScoreForChallenge(MapChallenge.ChallengeType challengeType)
		{
			MapChallenge mapChallenge;
			if (!this._challengeSystem.TryGetChallenge(challengeType, out mapChallenge))
			{
				Diagnostics.FailAssert("TryGetChallenge failed in GetBestScoreForChallenge", Array.Empty<object>());
				return 0;
			}
			int expiry = mapChallenge.TimeEnd;
			MotorwaysTimedChallengeScore challengeScore = this._player.GetChallengeScore(challengeType, expiry);
			if (challengeType == MapChallenge.ChallengeType.Daily && challengeScore.ScoreState == LeaderboardScoreState.Editable)
			{
				return -2;
			}
			return challengeScore.Score;
		}

		// Token: 0x06002474 RID: 9332 RVA: 0x00097DFD File Offset: 0x00095FFD
		public override void ScrollToButton(AnimatedCard button, bool instantly = false)
		{
			if (this.CurrentlySelectedMapButton != button)
			{
				this.CurrentlySelectedMapButton.SetSelected(false);
				if (!instantly)
				{
					this._selectedChallengeIndex = -1;
				}
			}
			base.ScrollToButton(button, instantly);
			this.CurrentlySelectedMapButton.SetSelected(true);
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x00097E38 File Offset: 0x00096038
		private void CreateMapButtons()
		{
			if (base.ButtonCount > 0)
			{
				foreach (MapButton mapButton2 in this.MapButtons)
				{
					mapButton2.ResetAnimations();
				}
				return;
			}
			List<AnimatedCard> newButtons = new List<AnimatedCard>();
			this._challengeButtonCount = 0;
			ChallengeDatabase challengeDatabase = this._appScope.Get<ChallengeDatabase>();
			if (FeatureToggle.IsFeatureEnabled(Feature.RandomChallengesMapButton))
			{
				this._challengeButtonCount++;
				MapButton randomChallengeButton = UnityEngine.Object.Instantiate<MapButton>(this.mapButtonPrefab, this.buttonParent);
				randomChallengeButton.name = "Mystery Challenge Map Button";
				randomChallengeButton.Initialize(this, this._appScope, this._constants);
				newButtons.Add(randomChallengeButton);
			}
			if (this._challengeSystem.WeeklyChallenge != null && this._challengeSystem.AreChallengesUnlocked(this._player))
			{
				this._challengeButtonCount++;
				MapDefinition weeklyMap = this._challengeSystem.WeeklyChallenge.mapDefinition;
				MapButton weeklyChallengeButton = UnityEngine.Object.Instantiate<MapButton>(this.mapButtonPrefab, this.buttonParent);
				weeklyChallengeButton.name = "Weekly Challenge Map Button";
				int bestScore = this.GetBestScoreForChallenge(MapChallenge.ChallengeType.Weekly);
				weeklyChallengeButton.Initialize(this, weeklyMap, this._appScope, bestScore, this._constants, this._challengeSystem.WeeklyChallenge);
				weeklyChallengeButton.SetChallengeIcons(this._challengeSystem.WeeklyChallenge.challenges, challengeDatabase);
				weeklyChallengeButton.SetSelected(false);
				weeklyChallengeButton.onChallengeExpired += this.OnChallengeExpired;
				weeklyChallengeButton.onShowMoreChallengeInfo += this.ShowWeeklyChallengePopup;
				if (this._newContentDatabase.IsNewContent("DailyWeeklyChallengeCards", false))
				{
					weeklyChallengeButton.SetHideLeft();
				}
				newButtons.Add(weeklyChallengeButton);
			}
			if (this._challengeSystem.DailyChallenge != null && this._challengeSystem.AreChallengesUnlocked(this._player))
			{
				this._challengeButtonCount++;
				MapDefinition dailyMap = this._challengeSystem.DailyChallenge.mapDefinition;
				MapButton dailyChallengeButton = UnityEngine.Object.Instantiate<MapButton>(this.mapButtonPrefab, this.buttonParent);
				dailyChallengeButton.name = "Daily Challenge Map Button";
				int bestScore2 = this.GetBestScoreForChallenge(MapChallenge.ChallengeType.Daily);
				dailyChallengeButton.Initialize(this, dailyMap, this._appScope, bestScore2, this._constants, this._challengeSystem.DailyChallenge);
				dailyChallengeButton.SetChallengeIcons(this._challengeSystem.DailyChallenge.challenges, challengeDatabase);
				dailyChallengeButton.SetSelected(false);
				dailyChallengeButton.onChallengeExpired += this.OnChallengeExpired;
				dailyChallengeButton.onShowMoreChallengeInfo += this.ShowDailyChallengePopup;
				if (this._newContentDatabase.IsNewContent("DailyWeeklyChallengeCards", false))
				{
					dailyChallengeButton.SetHideLeft();
				}
				newButtons.Add(dailyChallengeButton);
			}
			int mapIndex = 0;
			foreach (MapDefinition map in this._mapDatabase.MapLibrary.Maps)
			{
				int bestScore3 = this.GetBestScoreForCityLeaderboard(map.cityName, GameMode.Normal);
				MapButton newButton = UnityEngine.Object.Instantiate<MapButton>(this.mapButtonPrefab, this.buttonParent);
				newButton.name = map.cityName + " Map Button";
				newButton.Initialize(this, map, this._appScope, bestScore3, this._constants, null);
				newButton.onShowModeInfo += this.ShowModeInfoPopup;
				newButton.onExpertModeLockedPressed += this.ShowExpertUnlockInfoPopup;
				if (mapIndex > 0)
				{
					newButton.SetSelected(false);
				}
				newButtons.Add(newButton);
				mapIndex++;
			}
			base.SetNewButtons(newButtons);
			this.ScrollToButton(this.buttons[this._challengeButtonCount], true);
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				for (int lockedMapIndex = 3; lockedMapIndex < this.buttons.Count; lockedMapIndex++)
				{
					this.MapButtonAt(lockedMapIndex).SetLocked(StringId.None, StringId.AppleDemo_FeatureNotEnabled);
				}
			}
			else
			{
				for (int mapButtonIndex = 0; mapButtonIndex < this.buttons.Count; mapButtonIndex++)
				{
					MapButton mapButton = this.MapButtonAt(mapButtonIndex);
					if (mapButton.Type == MapButton.MapButtonType.City && !mapButton.IsRandomChallengeCard && (mapButton.MapDefinition.IsLocked(this._appScope) || this._previouslyLockedMapButtons.Contains(mapButton.MapDefinition)))
					{
						mapButton.SetLocked(StringId.MapUnlock_ToUnlock, mapButton.MapDefinition.HowToUnlockDescription);
					}
				}
			}
			for (int mapButtonIndex2 = 0; mapButtonIndex2 < this.buttons.Count; mapButtonIndex2++)
			{
				this.MapButtonAt(mapButtonIndex2).SetupButtonNavigation();
			}
			this.CurrentlySelectedMapButton.SetSelected(true);
			base.RegisterButtons();
			this.SetScreenButtonNavigation();
			this.ScrollToButton(base.CurrentlySelectedButton, true);
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x000982E4 File Offset: 0x000964E4
		public override void OnMoveCursor(Selectable currentFocus, MoveDirection direction)
		{
			if (this._cityDefinition == null && !this._isPlayingAnimation)
			{
				base.OnMoveCursor(currentFocus, direction);
			}
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x000982FE File Offset: 0x000964FE
		public void ShowDailyChallengeInfo()
		{
			this._screenStack.PushScreen<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, delegate(ChallengeInfoScreen screen)
			{
				MapChallenge challenge = this._challengeSystem.DailyChallenge;
				screen.PrepareScreen(MapChallenge.ChallengeType.Daily, new List<ChallengeData>(challenge.challenges), challenge.TimeStart, challenge.TimeEnd, StringId.Continue, true, false, null, true);
			}, true, null, true, null);
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x0009831E File Offset: 0x0009651E
		public void ShowWeeklyChallengeInfo()
		{
			this._screenStack.PushScreen<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, delegate(ChallengeInfoScreen screen)
			{
				MapChallenge challenge = this._challengeSystem.WeeklyChallenge;
				screen.PrepareScreen(MapChallenge.ChallengeType.Weekly, new List<ChallengeData>(challenge.challenges), challenge.TimeStart, challenge.TimeEnd, StringId.Continue, true, false, null, true);
			}, true, null, true, null);
		}

		// Token: 0x06002479 RID: 9337 RVA: 0x00098340 File Offset: 0x00096540
		private void TrySubmitOutstandingScores()
		{
			if (this._leaderboardService.CanSubmitScoresOffline)
			{
				this._player.MotorwaysExtendedUserProfile.GetAndClearUnsubmittedScores();
				return;
			}
			foreach (ValueTuple<LeaderboardId, int, LeaderboardScoreState> valueTuple in this._player.MotorwaysExtendedUserProfile.GetAndClearUnsubmittedScores())
			{
				LeaderboardId id = valueTuple.Item1;
				int score = valueTuple.Item2;
				LeaderboardScoreState state = valueTuple.Item3;
				this._leaderboardService.SubmitScore(id, score, state);
			}
		}

		// Token: 0x0600247A RID: 9338 RVA: 0x000983D0 File Offset: 0x000965D0
		private void OnChallengeExpired(MapButton challengeMapButton)
		{
			MapSelectScreen.<>c__DisplayClass105_0 CS$<>8__locals1 = new MapSelectScreen.<>c__DisplayClass105_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.challengeMapButton = challengeMapButton;
			CS$<>8__locals1.challengeMapButton.ShowCard(MapButton.Card.Main);
			CS$<>8__locals1.challengeMapButton.onAnimationMidFlip += CS$<>8__locals1.<OnChallengeExpired>g__OnButtonMidFlip|0;
		}

		// Token: 0x0600247B RID: 9339 RVA: 0x00098414 File Offset: 0x00096614
		public void ShowDailyChallengePopup()
		{
			this.ShowDailyChallengePopup(0f);
		}

		// Token: 0x0600247C RID: 9340 RVA: 0x00098424 File Offset: 0x00096624
		public void ShowDailyChallengePopup(float delay)
		{
			this.popupStack.PushPopup<ChallengeInfoPopup>(delay, false).Initialise(this._appScope, StringId.DailyChallenge, StringId.DailyChallenge_Tutorial, new Action(this.OnPopupHidden));
			this._appScope.Get<NewContentData>().SetNewContentSeen("DailyChallengeTutorialPopup");
			this._popupHidden = false;
		}

		// Token: 0x0600247D RID: 9341 RVA: 0x0009847B File Offset: 0x0009667B
		private void ShowModeInfoPopup()
		{
			this.popupStack.PushPopup<ModeInfoPopup>(0f, false).Initialize(this._appScope, this.CurrentlySelectedMapButton.GetCurrentSelectedGameMode(), new Action(this.OnPopupHidden));
			this._popupHidden = false;
		}

		// Token: 0x0600247E RID: 9342 RVA: 0x000984B8 File Offset: 0x000966B8
		public void ShowExpertUnlockInfoPopup()
		{
			ExpertUnlockInfoPopup modeInfoPopup = this.popupStack.PushPopup<ExpertUnlockInfoPopup>(0f, false);
			if (FeatureToggle.IsFeatureEnabled(Feature.ExpertLock))
			{
				modeInfoPopup.InfoText.SetStringId(this._appScope, StringId.To_Unlock);
			}
			modeInfoPopup.Initialize(new Action(this.OnPopupHidden));
			this._popupHidden = false;
		}

		// Token: 0x0600247F RID: 9343 RVA: 0x00098510 File Offset: 0x00096710
		public void ShowWeeklyChallengePopup()
		{
			this.ShowWeeklyChallengePopup(0f);
		}

		// Token: 0x06002480 RID: 9344 RVA: 0x00098520 File Offset: 0x00096720
		public void ShowWeeklyChallengePopup(float delay)
		{
			this.popupStack.PushPopup<ChallengeInfoPopup>(delay, false).Initialise(this._appScope, StringId.WeeklyChallenge, StringId.WeeklyChallenge_Tutorial, new Action(this.OnPopupHidden));
			this._appScope.Get<NewContentData>().SetNewContentSeen("WeeklyChallengeTutorialPopup");
			this._popupHidden = false;
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x00098578 File Offset: 0x00096778
		public void ShowChallengeModeInfoPopup()
		{
			this.popupStack.PushPopup<ChallengeInfoPopup>(0f, false).Initialise(this._appScope, StringId.CityChallenge_InfoPopup_Title, StringId.CityChallenge_InfoPopup_Body, new Action(this.OnPopupHidden));
			this._player.SetNewContentSeen("NewCityChallengeUnlockInfoPopup");
			this._popupHidden = false;
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x000985CE File Offset: 0x000967CE
		public override void OnGainedFocus()
		{
			base.OnGainedFocus();
			MapButton currentlySelectedMapButton = this.CurrentlySelectedMapButton;
			if (currentlySelectedMapButton == null)
			{
				return;
			}
			MapButtonModeSelectCard modeSelectCard = currentlySelectedMapButton.ModeSelectCard;
			if (modeSelectCard == null)
			{
				return;
			}
			modeSelectCard.OnRegainedFocus();
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x000985F0 File Offset: 0x000967F0
		private void OnPopupHidden()
		{
			this._popupHidden = true;
		}

		// Token: 0x06002484 RID: 9348 RVA: 0x000985FC File Offset: 0x000967FC
		public void HideUnselectedButtons()
		{
			foreach (MapButton button in this.MapButtons)
			{
				if (button != this.CurrentlySelectedMapButton)
				{
					button.CanvasGroup.Alpha = 0f;
				}
			}
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x00098660 File Offset: 0x00096860
		public void OffsetNeighbouringCardsToButton(MapButton button, AnimatedCard.ExpansionLevel mainCardExpansionLevel)
		{
			if (!button.IsInitialized)
			{
				return;
			}
			int indexOfButton = base.IndexOf(button);
			if (indexOfButton > 0)
			{
				this.MapButtonAt(indexOfButton - 1).SetOffset(mainCardExpansionLevel, true);
			}
			if (indexOfButton < this.buttons.Count - 1)
			{
				this.MapButtonAt(indexOfButton + 1).SetOffset(mainCardExpansionLevel, false);
			}
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x000986B4 File Offset: 0x000968B4
		public void SetThemePreference(MotorwaysThemePreference preference)
		{
			MotorwaysThemeDatabase.Log.Info("Setting theme to {0}", new object[]
			{
				preference
			});
			MotorwaysThemePreference themePreference = this._themeDatabase.ThemePreference;
			MotorwaysThemePreference newPreference = preference;
			if (themePreference == MotorwaysThemePreference.DarkColorblind || themePreference == MotorwaysThemePreference.Colorblind)
			{
				if (newPreference == MotorwaysThemePreference.Colorful)
				{
					newPreference = MotorwaysThemePreference.Colorblind;
				}
				else if (newPreference == MotorwaysThemePreference.Dark)
				{
					newPreference = MotorwaysThemePreference.DarkColorblind;
				}
			}
			this._themeDatabase.SetThemePreference(newPreference, true, true, true);
			foreach (MapButton mapButton in this.MapButtons)
			{
				mapButton.EnsureThemeButtonSelectedState(new MotorwaysThemePreference?(preference));
			}
		}

		// Token: 0x06002487 RID: 9351 RVA: 0x00098754 File Offset: 0x00096954
		public override void ApplyTheme(ITheme newTheme)
		{
			base.ApplyTheme(newTheme);
			foreach (MapButton mapButton in this.MapButtons)
			{
				mapButton.ApplyTheme();
				GameMode mode = this._player.GetSelectedModeForMap(mapButton.MapDefinition.cityName);
				mapButton.MainCard.UpdateModeStrings(mode);
			}
			this.SetScreenButtonNavigation();
		}

		// Token: 0x06002488 RID: 9352 RVA: 0x000987D0 File Offset: 0x000969D0
		public override void ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			if (progress >= 1f)
			{
				this.SetScreenButtonNavigation();
			}
			base.ApplyBlendedTheme(oldTheme, newTheme, progress);
			if (base.ButtonCount > 0)
			{
				foreach (MapButton mapButton in this.MapButtons)
				{
					mapButton.ApplyBlendedTheme(progress);
				}
			}
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x0009883C File Offset: 0x00096A3C
		protected override void OnSelectButton()
		{
			if (this._cityDefinition == null)
			{
				base.OnSelectButton();
				this.SetScreenButtonNavigation();
			}
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x00098854 File Offset: 0x00096A54
		public void SetScreenButtonNavigation()
		{
			if (base.ButtonCount > 0 && this.CurrentlySelectedMapButton != null)
			{
				Navigation nav = this.firstFocus.GetComponent<TouchButton>().navigation;
				if (this.CurrentlySelectedMapButton.IsLocked)
				{
					nav.selectOnUp = this.backButton;
				}
				else if (this.CurrentlySelectedMapButton.CurrentCard != MapButton.Card.Leaderboard)
				{
					if (this.CurrentlySelectedMapButton.IsChallengeMapButton())
					{
						nav.selectOnUp = this.CurrentlySelectedMapButton.MainCard.ChallengeButtonSet;
					}
					else if (this.CurrentlySelectedMapButton.CurrentCard != MapButton.Card.Challenge)
					{
						nav.selectOnUp = this.CurrentlySelectedMapButton.LeaderboardTabButton.GetComponent<Selectable>();
					}
				}
				this.firstFocus.GetComponent<TouchButton>().navigation = nav;
				nav = this.backButton.GetComponent<TouchButton>().navigation;
				if (this.CurrentlySelectedMapButton.IsLocked)
				{
					nav.selectOnDown = this.firstFocus;
					nav.selectOnRight = this.firstFocus;
				}
				else if (this.CurrentlySelectedMapButton.IsChallengeMapButton())
				{
					nav.selectOnDown = this.CurrentlySelectedMapButton.MoreInfoButton;
					nav.selectOnRight = this.CurrentlySelectedMapButton.MoreInfoButton;
				}
				else
				{
					switch (this._themeDatabase.ThemePreference)
					{
					case MotorwaysThemePreference.Dark:
					case MotorwaysThemePreference.DarkColorblind:
						nav.selectOnDown = this.CurrentlySelectedMapButton.DarkSelect;
						nav.selectOnRight = this.CurrentlySelectedMapButton.DarkSelect;
						goto IL_1BF;
					case MotorwaysThemePreference.Maps:
						nav.selectOnDown = this.CurrentlySelectedMapButton.MapsSelect;
						nav.selectOnRight = this.CurrentlySelectedMapButton.MapsSelect;
						goto IL_1BF;
					}
					nav.selectOnDown = this.CurrentlySelectedMapButton.ColorfulSelect;
					nav.selectOnRight = this.CurrentlySelectedMapButton.ColorfulSelect;
				}
				IL_1BF:
				this.backButton.GetComponent<TouchButton>().navigation = nav;
			}
		}

		// Token: 0x0600248B RID: 9355 RVA: 0x0008F34A File Offset: 0x0008D54A
		public void OnBack()
		{
			this._screenStack.PopOneScreen();
		}

		// Token: 0x0600248C RID: 9356 RVA: 0x00098A31 File Offset: 0x00096C31
		public override void BackActivated()
		{
			if (this._cityDefinition == null)
			{
				base.BackActivated();
			}
		}

		// Token: 0x0600248D RID: 9357 RVA: 0x00098A44 File Offset: 0x00096C44
		public override void PageSelected(Vector2 direction)
		{
			if (this._isPlayingAnimation)
			{
				return;
			}
			base.PageSelected(direction);
			if (direction.x > 0f && this._currentlySelectedButtonIndex < this.MapButtons.Count<MapButton>() - 1)
			{
				this.ScrollToButton(this.buttons[this._currentlySelectedButtonIndex + 1], false);
			}
			else if (direction.x < 0f && this._currentlySelectedButtonIndex > 0)
			{
				this.ScrollToButton(this.buttons[this._currentlySelectedButtonIndex - 1], false);
			}
			this.ScrollToButton(base.CurrentlySelectedButton, false);
			this._menuNavigation.SetNewFocus(this.CurrentlySelectedMapButton.PlayButton);
		}

		// Token: 0x0600248E RID: 9358 RVA: 0x00098AF4 File Offset: 0x00096CF4
		[UsedImplicitly]
		public void OnChallengeSystemChangeDebugOffset(int numDays)
		{
			AdjustableGameDateTime adjustableGameDateTime = GameDateTime.Backend as AdjustableGameDateTime;
			if (adjustableGameDateTime != null)
			{
				adjustableGameDateTime.UtcOffset += TimeSpan.FromDays((double)numDays);
			}
		}

		// Token: 0x0600248F RID: 9359 RVA: 0x00098B27 File Offset: 0x00096D27
		public void OnOpenChallengeCalendar()
		{
			this.popupStack.PushPopup<DebugOverlayScreen>(0f, false);
		}

		// Token: 0x06002490 RID: 9360 RVA: 0x00098B3B File Offset: 0x00096D3B
		private void ShowiCloudStorageFullPopup()
		{
			this.popupStack.PushPopup<LoadScreenInterruptionPopup>(0f, false).Initialise(StringId.Options_iCloud, StringId.iCloudQuotaExceeded, new Action(this.OnPopupHidden));
			this._player.NotifyPlayerOfSaveFailure();
			this._popupHidden = false;
		}

		// Token: 0x06002491 RID: 9361 RVA: 0x00098B78 File Offset: 0x00096D78
		protected override void UnregisterThemeComponents()
		{
			base.UnregisterThemeComponents();
			foreach (AnimatedCard animatedCard in this.buttons)
			{
				animatedCard.UnregisterThemeComponents();
			}
		}

		// Token: 0x06002492 RID: 9362 RVA: 0x00098BD0 File Offset: 0x00096DD0
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			base.DestroyButtons();
		}

		// Token: 0x06002493 RID: 9363 RVA: 0x00098BE0 File Offset: 0x00096DE0
		public override void Reset()
		{
			base.Reset();
			this.scrollRect.enabled = true;
			this.scrollRect.horizontalNormalizedPosition = 0f;
			this._originPosition = null;
			this._scaleToCamera = true;
			this._timerTillTransition = -1f;
			this._mapLoadedForGameScreen = false;
			this._popupHidden = true;
			this._transitioningFromGameScreen = false;
			this._playButtonStringId = StringId.None;
			this._handleDeepLinkOnTransition = false;
			this._blurWhileTransitioning = false;
			this._soakTestCountdown = 0f;
			this._challengeButtonCount = 0;
			this._selectedChallengeIndex = -1;
			this._cityCameraTransitionPosition = default(Vector2);
			this._cityCameraTransitionHandle = default(Vector2);
			this._buttonsToUnlockOnTransitioned.Clear();
			this._playHighlightedWhenLastActive = false;
			this.PlayerSelectedLeaderboardType = null;
			this._isPlayingAnimation = false;
			this._previousScreen = ScreenStack.MotorwaysScreen.MainMenu;
		}

		// Token: 0x04001E61 RID: 7777
		public const int InvalidScore = -1;

		// Token: 0x04001E62 RID: 7778
		public const int InProgressScore = -2;

		// Token: 0x04001E63 RID: 7779
		[Dependency]
		private MapDatabase _mapDatabase;

		// Token: 0x04001E64 RID: 7780
		[Dependency]
		private ChallengeSystem _challengeSystem;

		// Token: 0x04001E65 RID: 7781
		[Dependency]
		private NewContentData _newContentDatabase;

		// Token: 0x04001E66 RID: 7782
		[Dependency]
		private VisualConstantsData _constants;

		// Token: 0x04001E67 RID: 7783
		[Dependency]
		private AchievementDatabase _achievements;

		// Token: 0x04001E68 RID: 7784
		[Dependency]
		private GameContainerScreen _gameContainer;

		// Token: 0x04001E69 RID: 7785
		[Dependency]
		private LeaderboardService _leaderboardService;

		// Token: 0x04001E6A RID: 7786
		[Dependency]
		private MenuNavigation _menuNavigation;

		// Token: 0x04001E6B RID: 7787
		[Dependency]
		private IPersistentStorageService _storageService;

		// Token: 0x04001E6C RID: 7788
		[Dependency]
		private DeepLinkProcessor _deepLinkProcessor;

		// Token: 0x04001E6D RID: 7789
		public const string WeeklyChallengeTutorialPopupContentID = "WeeklyChallengeTutorialPopup";

		// Token: 0x04001E6E RID: 7790
		public const string DailyChallengeTutorialPopupContentID = "DailyChallengeTutorialPopup";

		// Token: 0x04001E6F RID: 7791
		public const string ChallengeCardsNewContentID = "DailyWeeklyChallengeCards";

		// Token: 0x04001E70 RID: 7792
		public const string NewCityChallengeUnlockInfoPopup = "NewCityChallengeUnlockInfoPopup";

		// Token: 0x04001E71 RID: 7793
		public MapButton mapButtonPrefab;

		// Token: 0x04001E72 RID: 7794
		[Tooltip("The duration of the fade to black if Skip Transitions is on")]
		[MinValue(0)]
		public float skippedTransitionFadeDuration = 1f;

		// Token: 0x04001E73 RID: 7795
		[MinValue(0)]
		[Tooltip("The delay between the confirmed press and the non-confirmed cards being pushed to the side")]
		public float intervalBetweenButtonPushAnimations = 0.1f;

		// Token: 0x04001E74 RID: 7796
		[MinValue(1)]
		[Tooltip("The delay before transitioning into the map. Min 1")]
		public float delayBeforeTransitioning = 2f;

		// Token: 0x04001E75 RID: 7797
		[SerializeField]
		[MinValue(0)]
		[Tooltip("The delay in seconds between each challenge card slides in from left the first time a users sees them")]
		private float _nextChallengeCardAppearDelay;

		// Token: 0x04001E76 RID: 7798
		[SerializeField]
		protected LocalizedTextUI _playButtonText;

		// Token: 0x04001E77 RID: 7799
		private StringId _playButtonStringId;

		// Token: 0x04001E78 RID: 7800
		[SerializeField]
		private TextMeshProUGUI _playButtonTextMeshPro;

		// Token: 0x04001E79 RID: 7801
		[SerializeField]
		private CanvasRenderer _playButtonChallengeIcon;

		// Token: 0x04001E7A RID: 7802
		[SerializeField]
		private CanvasRenderer _playButtonEndlessIcon;

		// Token: 0x04001E7B RID: 7803
		[SerializeField]
		private CanvasRenderer _playButtonExpertIcon;

		// Token: 0x04001E7C RID: 7804
		[SerializeField]
		private CanvasRenderer _playButtonCreativeIcon;

		// Token: 0x04001E7D RID: 7805
		private const float PlayButtonUninteractableAlpha = 0.5f;

		// Token: 0x04001E7E RID: 7806
		private const float ScrollParallaxConstant = 0.805f;

		// Token: 0x04001E7F RID: 7807
		[SerializeField]
		[Tooltip("Wait time when scrolling to the first unlock element.")]
		private float _unlockAnimationTimeToScrollToFirstElement = 0.25f;

		// Token: 0x04001E80 RID: 7808
		[SerializeField]
		[Tooltip("The time between starting the padlock unlock anim and the scroll anim.")]
		private float _unlockToScrollTime = 0.25f;

		// Token: 0x04001E81 RID: 7809
		[SerializeField]
		[Tooltip("The time between starting the scroll and starting the flip")]
		private float _scrollToFlipTime = 0.1f;

		// Token: 0x04001E82 RID: 7810
		[Tooltip("Wait time between starting scroll to next element animation and starting the next unlock animation.")]
		[SerializeField]
		private float _scrollWaitTime = 0.25f;

		// Token: 0x04001E83 RID: 7811
		[Tooltip("Delay before scrolling back to original map after unlock sequence.")]
		[SerializeField]
		private float _unlockAnimationEndDelay = 0.5f;

		// Token: 0x04001E84 RID: 7812
		private Vector3? _originPosition;

		// Token: 0x04001E85 RID: 7813
		private AssetBundleUtility.AsyncLoadResult _cityDefinition;

		// Token: 0x04001E86 RID: 7814
		private Vector2 _cityCameraTransitionPosition;

		// Token: 0x04001E87 RID: 7815
		private Vector2 _cityCameraTransitionHandle;

		// Token: 0x04001E88 RID: 7816
		private bool _transitioningFromGameScreen;

		// Token: 0x04001E89 RID: 7817
		private float _timerTillTransition = -1f;

		// Token: 0x04001E8A RID: 7818
		private static readonly int DroppedDown = Animator.StringToHash("DroppedDown");

		// Token: 0x04001E8B RID: 7819
		private static readonly int ShouldShowChallengeIcon = Animator.StringToHash("ShouldShowChallengeIcon");

		// Token: 0x04001E8C RID: 7820
		private static readonly int ShouldShowEndlessIcon = Animator.StringToHash("ShouldShowEndlessIcon");

		// Token: 0x04001E8D RID: 7821
		private static readonly int ShouldShowExpertIcon = Animator.StringToHash("ShouldShowExpertIcon");

		// Token: 0x04001E8E RID: 7822
		private static readonly int ShouldShowCreativeIcon = Animator.StringToHash("ShouldShowCreativeIcon");

		// Token: 0x04001E8F RID: 7823
		private bool _mapLoadedForGameScreen;

		// Token: 0x04001E90 RID: 7824
		private bool _popupHidden = true;

		// Token: 0x04001E91 RID: 7825
		private bool _handleDeepLinkOnTransition;

		// Token: 0x04001E92 RID: 7826
		private bool _blurWhileTransitioning;

		// Token: 0x04001E93 RID: 7827
		private bool _hasSeenDailyChallengeCompletePopUp;

		// Token: 0x04001E94 RID: 7828
		private readonly HashSet<MapDefinition> _previouslyLockedMapButtons = new HashSet<MapDefinition>();

		// Token: 0x04001E95 RID: 7829
		private readonly HashSet<MapDefinition> _previouslyLockedCityChallengeMapButtons = new HashSet<MapDefinition>();

		// Token: 0x04001E97 RID: 7831
		private float _soakTestCountdown;

		// Token: 0x04001E98 RID: 7832
		private int _challengeButtonCount;

		// Token: 0x04001E99 RID: 7833
		private readonly List<MapButton> _buttonsToUnlockOnTransitioned = new List<MapButton>();

		// Token: 0x04001E9A RID: 7834
		private MapButton _buttonToUnlockCityChallengeOnTransitioned;

		// Token: 0x04001E9B RID: 7835
		private MapButton _buttonToUnlockExpertModeOnTransitioned;

		// Token: 0x04001E9C RID: 7836
		private MapButton _lastSelectedButtonBeforeTransitionOut;

		// Token: 0x04001E9D RID: 7837
		private int _selectedChallengeIndex = -1;

		// Token: 0x04001E9E RID: 7838
		private bool _playHighlightedWhenLastActive;

		// Token: 0x04001E9F RID: 7839
		private bool _isPlayingAnimation;

		// Token: 0x04001EA0 RID: 7840
		private ScreenStack.MotorwaysScreen _previousScreen;

		// Token: 0x04001EA1 RID: 7841
		private static readonly ProfilerMarker Profiler_Tick = new ProfilerMarker(ProfilerCategory.Scripts, "MapSelectScreen.Tick()");

		// Token: 0x04001EA2 RID: 7842
		private static readonly ProfilerMarker Profiler_ApplyBlendedTheme = new ProfilerMarker(ProfilerCategory.Scripts, "MapSelectScreen.ApplyBlendedTheme()");
	}
}
