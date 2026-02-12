using System;
using System.Collections.Generic;
using System.Linq;
using Client;
using Easing;
using Factory;
using Motorways.Audio;
using Motorways.Commands;
using Motorways.Models;
using Motorways.Processes;
using Motorways.UI;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000545 RID: 1349
	public class GameUpgradeScreen : InGameScalingScreen
	{
		// Token: 0x060023FC RID: 9212 RVA: 0x00094260 File Offset: 0x00092460
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this._delayTimer >= 0f)
			{
				this._delayTimer -= deltaTime;
				if (this._delayTimer < 0f)
				{
					if (this._nextUpgradeChoice != null)
					{
						this.SetButtonOptions(this._nextUpgradeChoice);
					}
					else if (!this._hasPopped)
					{
						this._screenStack.PopOneScreen();
						this._hasPopped = true;
					}
					this._delayTimer = -1f;
				}
			}
			if (this._visibilityToggleTween.IsActive)
			{
				float alpha = this._visibilityToggleTween.Tick(deltaTime);
				this.SetFadeAmount(alpha, false);
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest) && !this.IsTransitioningIn() && !this.IsTransitioningOut() && this._soakTestCountdown > 0f)
			{
				UpgradeDatabaseModel upgrades = this._simulation.GetModel<UpgradeDatabaseModel>();
				if (upgrades.pendingUpgradeChoices.Count > 0 && upgrades.pendingUpgradeChoices[0].choices.Count > 0)
				{
					this._soakTestCountdown -= deltaTime;
					if ((int)this._soakTestCountdown != (int)(this._soakTestCountdown + deltaTime))
					{
						Diagnostics.Log.Message(Diagnostics.Log.Level.Info, "Soak", "GameUpgradeScreen.Tick() soak countdown down to {0}.", new object[]
						{
							this._soakTestCountdown
						});
					}
					if (this._soakTestCountdown <= 0f)
					{
						int upgradeIndex = global::Random.Int(upgrades.pendingUpgradeChoices[0].choices.Count);
						int buttonIndex = Array.IndexOf<int>(this._buttonIndexToUpgradeIndex, upgradeIndex);
						Diagnostics.Log.Message(Diagnostics.Log.Level.Info, "Soak", "GameUpgradeScreen.Tick() selecting upgrade {0}.", new object[]
						{
							buttonIndex
						});
						this.OnUpgradeSelect(buttonIndex);
						this._soakTestCountdown = 1f;
					}
				}
			}
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x00094410 File Offset: 0x00092610
		public override void TransitionInTick()
		{
			base.TransitionInTick();
			float fadeInLerp = Mathf.Clamp01(this.TransitionInPercentage() * 2f - 0.5f);
			this.SetFadeAmount(Easings.QuarticEaseOut(fadeInLerp), true);
		}

		// Token: 0x060023FE RID: 9214 RVA: 0x00094448 File Offset: 0x00092648
		public override void TransitionOutTick()
		{
			base.TransitionOutTick();
			float fadeOutLerp = Mathf.Clamp01((1f - this.TransitionOutPercentage()) * 2f - 1f);
			this.SetFadeAmount(Easings.QuarticEaseIn(fadeOutLerp), true);
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x00094486 File Offset: 0x00092686
		private void SetFadeAmount(float alpha, bool includeRootCanvasGroup = false)
		{
			this.upgradeContainer.alpha = alpha;
			this._gameCamera.customBlur.Strength = alpha;
			if (includeRootCanvasGroup)
			{
				this._canvasGroup.Alpha = alpha;
			}
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x000944B4 File Offset: 0x000926B4
		public void OnUpgradeSelect(int buttonIndex)
		{
			if (this.IsTransitioningIn() || this.IsTransitioningOut())
			{
				return;
			}
			if (this._delayTimer >= 0f || this._hasPopped)
			{
				return;
			}
			int upgradeIndex = this._buttonIndexToUpgradeIndex[buttonIndex];
			UpgradeDatabaseModel upgrades = this._simulation.GetModel<UpgradeDatabaseModel>();
			if (Diagnostics.Verify(upgrades.pendingUpgradeChoices.Count >= 1, "We selected an upgrade but we don't actually have any pending upgrades!") && Diagnostics.Verify(upgradeIndex != -1, "We selected an invalid upgrade choice!"))
			{
				UpgradePackageDefinition selectedUpgrade = upgrades.pendingUpgradeChoices[0].choices[upgradeIndex];
				int animationsToSend = Mathf.Min(selectedUpgrade.amount, 5);
				if (selectedUpgrade.type == UpgradeType.Concrete)
				{
					animationsToSend = 1;
				}
				bool hasAdditionalConcrete = false;
				if (selectedUpgrade.additionalConcrete > 0)
				{
					animationsToSend++;
					hasAdditionalConcrete = true;
				}
				if (upgrades.pendingUpgradeChoices[0].choices.Count > 1)
				{
					List<UpgradeType> otherTypes = new List<UpgradeType>();
					foreach (UpgradePackageDefinition choice in upgrades.pendingUpgradeChoices[0].choices)
					{
						if (choice.type != selectedUpgrade.type)
						{
							otherTypes.Add(choice.type);
						}
					}
					this._analytics.TrackUpgradeChoice(selectedUpgrade.type, otherTypes, this._game);
				}
				if (!(this._gameScope.Get<City>().Rules is TutorialGameRules))
				{
					if (this._gameScope.Get<GameBehaviourModel>().MysteryUpgradesActive)
					{
						this._player.SetNewContentSeen("SelectedUpgradeTypeMystery");
					}
					else
					{
						this._player.SetNewContentSeen(GameUpgradeScreen.GetContentIdStringForSelectedUpgradeType(selectedUpgrade.type));
					}
				}
				float animationDelay = 0f;
				float totalAnimationDuration = 0f;
				for (int animationCount = 0; animationCount < animationsToSend; animationCount++)
				{
					NewUpgradeAnimationView iconAnimation = this._gameScope.Get<NewUpgradeAnimationView>();
					UpgradeType upgradeType = selectedUpgrade.type;
					int upgradeAmount = selectedUpgrade.amount;
					int iconRectIndex = 0;
					if (hasAdditionalConcrete && animationCount == animationsToSend - 1)
					{
						upgradeType = UpgradeType.Concrete;
						upgradeAmount = selectedUpgrade.additionalConcrete;
						iconRectIndex = 1;
					}
					int enumIndex = (int)upgradeType;
					GameUIScreen gameUiScreen = this._gameScope.Get<GameUIScreen>();
					RectTransform end = gameUiScreen.UpgradeBar.GetRectTransformForUpgrade(upgradeType);
					RectTransform start = this._activeUpgradeContainer.buttons[buttonIndex].GetIconRect(iconRectIndex);
					iconAnimation.GetComponent<RectTransform>().anchoredPosition = start.anchoredPosition;
					iconAnimation.transform.SetParent(start.parent, false);
					iconAnimation.transform.SetParent(gameUiScreen.OverlayTransform, true);
					int count = (upgradeType == UpgradeType.Concrete) ? upgradeAmount : 1;
					iconAnimation.Initialize(start.sizeDelta, end, this.upgradeSprites[enumIndex], upgradeType, this._themeDatabase.TargetTheme, animationDelay, count);
					animationDelay += iconAnimation.animationSpacing;
					if (animationCount == 0)
					{
						totalAnimationDuration = iconAnimation.animationDuration;
					}
					else
					{
						totalAnimationDuration += iconAnimation.animationSpacing;
					}
					if (gameUiScreen.UpgradeBar.IsSpriteForUpgradeACircle(upgradeType))
					{
						iconAnimation.UpgradeIcon.SetToCircle();
					}
					else
					{
						iconAnimation.UpgradeIcon.SetToDiamond();
					}
					this._gameScope.Get<ViewClient>().AddView(iconAnimation);
				}
				if (upgrades.pendingUpgradeChoices.Count > 1)
				{
					this.SetNextButtonOptions(upgrades.pendingUpgradeChoices[1], 0f, buttonIndex);
				}
				else
				{
					this.CloseScreenAfterSelection();
					GameContainerScreen activeScreen = this._screenStack.GetActiveScreen<GameContainerScreen>();
					if (activeScreen != null)
					{
						activeScreen.SkipNextTransition();
					}
					this._overrideNextTransitionDuration = totalAnimationDuration;
				}
				upgrades.numChoicesMade++;
				this._simulation.ScheduleCommand(SelectUpgradeCommand.Create(this._gameScope, upgradeIndex));
				return;
			}
			this.CloseScreenAfterSelection();
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x0009484C File Offset: 0x00092A4C
		public void SetNextButtonOptions(UpgradeChoice options, float delay, int selectedOption = -1)
		{
			if (selectedOption >= 0)
			{
				this._activeUpgradeContainer.buttons[selectedOption].iconParent.gameObject.SetActive(false);
			}
			this._delayTimer = delay;
			this._nextUpgradeChoice = options;
		}

		// Token: 0x06002402 RID: 9218 RVA: 0x0009487D File Offset: 0x00092A7D
		public void CloseScreenAfterSelection()
		{
			this._nextUpgradeChoice = null;
			this._delayTimer = 0f;
		}

		// Token: 0x06002403 RID: 9219 RVA: 0x00094894 File Offset: 0x00092A94
		private void SetButtonOptions(UpgradeChoice choice)
		{
			City city = this._gameScope.Get<City>();
			GameRules rules = city.Rules;
			bool isInTutorial = rules is TutorialGameRules;
			MotorwaysStringKey weekKey = this._gameScope.Get<MotorwaysStringKey>();
			StringId weekDescriptionId = StringId.None;
			this._activeUpgradeContainer = this._classicUpgradeContainer;
			UpgradeContainer inactiveContainer = this._expertUpgradeContainer;
			if (choice.choices.Count > 2)
			{
				this._activeUpgradeContainer = this._expertUpgradeContainer;
				inactiveContainer = this._classicUpgradeContainer;
			}
			this._activeUpgradeContainer.root.SetActive(true);
			inactiveContainer.root.SetActive(false);
			if (rules.ScoringMode == ScoringMode.EfficiencyMilestones)
			{
				int milestone = this._gameScope.Get<UpgradeDatabaseModel>().TotalClaimedPackages + 1;
				weekKey.InitWithStringId(StringId.MilestoneCount, milestone, new Dictionary<string, string>
				{
					{
						"Num",
						milestone.ToString()
					}
				});
			}
			else if (city.GameMode == GameMode.Expert)
			{
				UpgradeDatabaseModel database = this._gameScope.Get<UpgradeDatabaseModel>();
				int weeksRemainingWithUpgrades = this._constants.MaxUpgradeChoicesAwardedInExpertMode - database.TotalGrantedUpgradesCount;
				if (weeksRemainingWithUpgrades > 0)
				{
					weekKey.InitWithStringId(StringId.WeeksRemaining, weeksRemainingWithUpgrades + 1, new Dictionary<string, string>
					{
						{
							"Num",
							(weeksRemainingWithUpgrades + 1).ToString()
						}
					});
				}
				else if (weeksRemainingWithUpgrades == 0)
				{
					weekKey.InitWithStringId(StringId.WeeksRemainingNone);
					weekDescriptionId = StringId.WeeksRemainingNone_Body;
				}
				else
				{
					int week = this._gameScope.Get<ClockModel>().Week;
					weekKey.InitWithStringId(StringId.WeekCount, week, new Dictionary<string, string>
					{
						{
							"Num",
							week.ToString()
						}
					});
					weekDescriptionId = StringId.WeekTagline_Concrete;
				}
			}
			else
			{
				int week2 = this._gameScope.Get<ClockModel>().Week;
				weekKey.InitWithStringId(StringId.WeekCount, week2, new Dictionary<string, string>
				{
					{
						"Num",
						week2.ToString()
					}
				});
			}
			this._activeUpgradeContainer.weekText.LocString = StandaloneLocString.CreateString(this._gameScope, weekKey);
			List<UpgradePackageDefinition> upgrades = choice.choices;
			if (weekDescriptionId == StringId.None)
			{
				if (choice.isFree)
				{
					weekDescriptionId = StringId.WeekTagline_Concrete;
				}
				else
				{
					weekDescriptionId = rules.GetUpgradeScreenDescriptionUpgrades(upgrades.Count);
				}
			}
			this._activeUpgradeContainer.weekDescriptionText.LocString = StandaloneLocString.CreateString(this._gameScope, weekDescriptionId);
			this._activeUpgradeContainer.weekText.gameObject.SetActive(!isInTutorial);
			this._activeUpgradeContainer.weekDescriptionText.gameObject.SetActive(!isInTutorial);
			bool useMysteryUpgrades = this._gameScope.Get<GameBehaviourModel>().MysteryUpgradesActive;
			this._buttonIndexToUpgradeIndex = GameUpgradeScreen.BuildButtonIndexToUpgradeIndexMapping(this._activeUpgradeContainer.buttons.Length, upgrades.Count);
			for (int buttonIndex = 0; buttonIndex < this._activeUpgradeContainer.buttons.Length; buttonIndex++)
			{
				NewUpgradeButton button2 = this._activeUpgradeContainer.buttons[buttonIndex];
				int upgradeIndex = this._buttonIndexToUpgradeIndex[buttonIndex];
				if (upgradeIndex != -1)
				{
					button2.transform.gameObject.SetActive(true);
					button2.SecondaryIcon.gameObject.SetActive(true);
					UpgradePackageDefinition upgrade = upgrades[upgradeIndex];
					if (useMysteryUpgrades)
					{
						button2.Sprite = this._mysterySprite;
						button2.PrimaryIcon.SetToDiamond();
						button2.SecondaryIcon.gameObject.SetActive(false);
						this.SetNumberBubbleAmount(button2.PrimaryNumberBubble, 1);
					}
					else
					{
						button2.primaryUpgradeType = upgrade.type;
						button2.Sprite = this.upgradeSprites[(int)upgrade.type];
						if (this._gameScope.Get<GameUIScreen>().UpgradeBar.IsSpriteForUpgradeACircle(upgrade.type))
						{
							button2.PrimaryIcon.SetToCircle();
						}
						else
						{
							button2.PrimaryIcon.SetToDiamond();
						}
						this.SetNumberBubbleAmount(button2.PrimaryNumberBubble, upgrade.amount);
						if (upgrade.additionalConcrete > 0)
						{
							this.SetNumberBubbleAmount(button2.SecondaryNumberBubble, upgrade.additionalConcrete);
							RectTransform primaryRect = button2.PrimaryIcon.Rect;
							button2.SecondaryIcon.SetCutoutRect(primaryRect);
						}
						else
						{
							button2.SecondaryIcon.gameObject.SetActive(false);
						}
					}
					button2.iconParent.gameObject.SetActive(true);
					MotorwaysStringKey nameKey = this._gameScope.Get<MotorwaysStringKey>();
					MotorwaysStringKey descriptionKey = this._gameScope.Get<MotorwaysStringKey>();
					MotorwaysStringKey additionalConcreteKey = this._gameScope.Get<MotorwaysStringKey>();
					if (button2.buttonName != null)
					{
						if (useMysteryUpgrades)
						{
							nameKey.InitWithStringId(StringId.MysteryUpgradeName);
							descriptionKey.InitWithStringId(StringId.MysteryUpgradeDescription);
						}
						else
						{
							nameKey.InitWithString(string.Format("{0}_Title", upgrade.type), upgrade.amount, new Dictionary<string, string>
							{
								{
									"Num",
									upgrade.amount.ToString()
								}
							});
							descriptionKey.InitWithStringId(this.DescriptionIds[(int)upgrade.type]);
							additionalConcreteKey.InitWithString(UpgradeType.Concrete.ToString(), upgrade.additionalConcrete, new Dictionary<string, string>
							{
								{
									"Num",
									upgrade.additionalConcrete.ToString()
								}
							});
						}
						button2.buttonName.LocString = StandaloneLocString.CreateString(this._gameScope, nameKey);
					}
					if (button2.buttonAdditionalConcrete != null)
					{
						if (upgrade.additionalConcrete > 0 && !useMysteryUpgrades)
						{
							button2.buttonAdditionalConcrete.LocString = StandaloneLocString.CreateString(this._gameScope, additionalConcreteKey);
						}
						else
						{
							button2.buttonAdditionalConcrete.LocString = StandaloneLocString.CreateNonLocalizedString(this._gameScope, "");
						}
					}
					if (button2.buttonDescription != null)
					{
						bool shouldShowNewUpgradeIcon = rules.ShouldShowNewUpgradeIconDescriptionForType(upgrade.type);
						if (useMysteryUpgrades)
						{
							shouldShowNewUpgradeIcon = !this._player.HasSeenNewContent("SelectedUpgradeTypeMystery");
						}
						if (this._game.Scope.Get<CityModel>().Mode == GameMode.Expert)
						{
							shouldShowNewUpgradeIcon = false;
						}
						if (shouldShowNewUpgradeIcon)
						{
							button2.buttonDescription.LocString = StandaloneLocString.CreateString(this._gameScope, descriptionKey);
						}
						else
						{
							button2.buttonDescription.LocString = StandaloneLocString.CreateNonLocalizedString(this._gameScope, "");
						}
					}
					bool isButtonEnabled = !choice.disabledOptions.HasFlag((DisabledUpgradeOptions)(1 << upgradeIndex + 1));
					button2.SetInteractable(isButtonEnabled);
				}
				else
				{
					button2.transform.gameObject.SetActive(false);
				}
			}
			if (this._inputState.CurrentInputTypeRequiresFocus)
			{
				NewUpgradeButton focus = this._activeUpgradeContainer.buttons.FirstOrDefault((NewUpgradeButton button) => button.interactable);
				this._menuNavigation.SetNewFocus(focus);
				this.firstFocus = focus;
			}
		}

		// Token: 0x06002404 RID: 9220 RVA: 0x00094F1C File Offset: 0x0009311C
		private static int[] BuildButtonIndexToUpgradeIndexMapping(int numButtons, int numUpgrades)
		{
			int[] buttonIndexToUpgradeIndex = new int[numButtons];
			for (int buttonIndex = 0; buttonIndex < numButtons; buttonIndex++)
			{
				buttonIndexToUpgradeIndex[buttonIndex] = -1;
			}
			if (numUpgrades <= 2)
			{
				for (int upgradeIndex = 0; upgradeIndex < numUpgrades; upgradeIndex++)
				{
					buttonIndexToUpgradeIndex[upgradeIndex] = upgradeIndex;
				}
			}
			else
			{
				int numTopRowItems = numUpgrades / 2;
				int numBottomRowItems = numUpgrades - numTopRowItems;
				int buttonIndex2 = 0;
				for (int upgradeIndex2 = 0; upgradeIndex2 < numTopRowItems; upgradeIndex2++)
				{
					buttonIndexToUpgradeIndex[buttonIndex2++] = upgradeIndex2;
				}
				buttonIndex2 = 3;
				for (int upgradeIndex3 = 0; upgradeIndex3 < numBottomRowItems; upgradeIndex3++)
				{
					buttonIndexToUpgradeIndex[buttonIndex2++] = numTopRowItems + upgradeIndex3;
				}
			}
			return buttonIndexToUpgradeIndex;
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x00094F9F File Offset: 0x0009319F
		private void SetNumberBubbleAmount(NumberBubble numberBubble, int amount)
		{
			if (numberBubble != null)
			{
				if (amount == 1)
				{
					numberBubble.Hide(true);
					return;
				}
				numberBubble.SetValue(amount, false);
			}
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x00094FBE File Offset: 0x000931BE
		public static string GetContentIdStringForSelectedUpgradeType(UpgradeType type)
		{
			return string.Format("SelectedUpgradeType{0}", type);
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x00094FD0 File Offset: 0x000931D0
		public override void ApplyTheme(ITheme newTheme)
		{
			base.ApplyTheme(newTheme);
			NewUpgradeButton[] buttons = this._classicUpgradeContainer.buttons;
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].ApplyTheme(newTheme);
			}
			buttons = this._expertUpgradeContainer.buttons;
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].ApplyTheme(newTheme);
			}
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x0009502A File Offset: 0x0009322A
		public override bool CanTransitionIn()
		{
			return this._playerActionController.BlockingPlayerActionCount == 0;
		}

		// Token: 0x06002409 RID: 9225 RVA: 0x0009503C File Offset: 0x0009323C
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			this.ApplyTheme(this._themeDatabase.TargetTheme);
			base.TransitionIn(outScreen);
			this._skipTransitions = false;
			this._canvasGroup.Alpha = 1f;
			this._canvasGroup.SetBlocksRaycasts(false);
			this._gameScope.Get<GameUIScreen>().IsUpgradeBarOnOverlay = true;
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.InGameOverlayScreen, this._gameScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.CancelActions);
			this._hasPopped = false;
			if (this._gameScope.Get<City>().Rules.ShouldUseUpgradeScreenOffsets())
			{
				foreach (GameUpgradeScreen.RectTransformPositionOffset transformPositionOffset in this.tutorialRectTransformOffsets)
				{
					transformPositionOffset.rectTransform.anchoredPosition3D += transformPositionOffset.positionOffset;
				}
			}
			this._soakTestCountdown = 1.1f;
			this._showMapButton.gameObject.SetActive(FeatureToggle.IsFeatureEnabled(Feature.UpgradeScreenViewMap));
			GameUIScreen gameUI = this._gameScope.Get<GameUIScreen>();
			this._visibleElements = GameUpgradeScreen.GameUIElements.None;
			if (gameUI.IsClockVisible)
			{
				gameUI.SetClockVisibility(false);
				this._visibleElements |= GameUpgradeScreen.GameUIElements.Clock;
			}
			if (gameUI.IsScoreVisible)
			{
				gameUI.SetScoreVisible(false);
				this._visibleElements |= GameUpgradeScreen.GameUIElements.Score;
			}
			gameUI.SetMenuButtonVisible(false);
			gameUI.ExitEditModeUI();
			this._showMapButtonTouchCatcher.raycastTarget = false;
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x000951A4 File Offset: 0x000933A4
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this._canvasGroup.SetBlocksRaycasts(true);
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x000951B8 File Offset: 0x000933B8
		public override void OnGainedFocus()
		{
			this._canvasGroup.SetInteractable(true);
			this._canvasGroup.SetBlocksRaycasts(true);
			Selectable currentFocus = this._menuNavigation.GetCurrentFocus();
			if (this._activeUpgradeContainer.buttons.Contains(currentFocus) || currentFocus == this._showMapButton)
			{
				return;
			}
			if (this.firstFocus != null && this._appScope.Get<InputState>().CurrentInputTypeRequiresFocus)
			{
				this._navigation.SetNewFocus(this.firstFocus);
			}
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x0009523C File Offset: 0x0009343C
		public void OnHideUIToggled()
		{
			if (FeatureToggle.IsFeatureDisabled(Feature.UpgradeScreenViewMap))
			{
				this._uiHidden = false;
				return;
			}
			this._uiHidden = !this._uiHidden;
			GameUIScreen gameUIScreen = this._gameScope.Get<GameUIScreen>();
			gameUIScreen.backButton.gameObject.SetActive(false);
			gameUIScreen.SetClockVisibility(false);
			gameUIScreen.SetScoreVisible(false);
			gameUIScreen.SetWorldGridActive(this._uiHidden, TransitionStyle.Tween);
			this.upgradeContainer.interactable = !this._uiHidden;
			this.upgradeContainer.blocksRaycasts = !this._uiHidden;
			this._showMapButtonTouchCatcher.raycastTarget = this._uiHidden;
			float startValue = (float)(this._uiHidden ? 1 : 0);
			startValue = (this._visibilityToggleTween.IsActive ? this._visibilityToggleTween.Value : startValue);
			if (this._uiHidden)
			{
				this._visibilityToggleTween.Start(startValue, 0f, 0.2f, Easings.Functions.QuarticEaseIn, 0f);
			}
			else
			{
				this._visibilityToggleTween.Start(startValue, 1f, 0.2f, Easings.Functions.QuarticEaseOut, 0f);
			}
			this._menuNavigation.SetNewFocus(this._showMapButton);
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x00095358 File Offset: 0x00093558
		public override void OnTransitionedOut()
		{
			base.OnTransitionedOut();
			this._canvasGroup.Alpha = 0f;
			this._gameScope.Get<GameUIScreen>().IsUpgradeBarOnOverlay = false;
			if (this._gameScope.Get<City>().Rules.ShouldUseUpgradeScreenOffsets())
			{
				foreach (GameUpgradeScreen.RectTransformPositionOffset transformPositionOffset in this.tutorialRectTransformOffsets)
				{
					transformPositionOffset.rectTransform.anchoredPosition3D -= transformPositionOffset.positionOffset;
				}
			}
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x00095400 File Offset: 0x00093600
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._gameScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.WeekStart, 0.5f, -1f, true, null));
			GameUIScreen gameUI = this._gameScope.Get<GameUIScreen>();
			if (this._visibleElements.HasFlag(GameUpgradeScreen.GameUIElements.Clock))
			{
				gameUI.SetClockVisibility(true);
			}
			if (this._visibleElements.HasFlag(GameUpgradeScreen.GameUIElements.Score))
			{
				gameUI.SetScoreVisible(true);
			}
			gameUI.backButton.gameObject.SetActive(true);
			gameUI.SetMenuButtonVisible(true);
			this._visibleElements = GameUpgradeScreen.GameUIElements.None;
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x000954B5 File Offset: 0x000936B5
		public override void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
		{
			base.OnCurrentDeviceInputTypeChanged(newInputType);
			if (InputState.DeviceInputTypeRequiresFocus(newInputType) && this._activeUpgradeContainer != null)
			{
				this._menuNavigation.SetNewFocus(this._activeUpgradeContainer.buttons[0]);
			}
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x000954E6 File Offset: 0x000936E6
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			this._canvasGroup.Alpha = 0f;
			this.upgradeContainer.alpha = 0f;
		}

		// Token: 0x06002411 RID: 9233 RVA: 0x0009550F File Offset: 0x0009370F
		public override void Reset()
		{
			base.Reset();
			this._visibleElements = GameUpgradeScreen.GameUIElements.None;
			this._delayTimer = 0f;
			this._nextUpgradeChoice = null;
			this._activeUpgradeContainer = null;
			this._buttonIndexToUpgradeIndex = null;
			this._hasPopped = false;
		}

		// Token: 0x04001E01 RID: 7681
		public CanvasGroup upgradeContainer;

		// Token: 0x04001E02 RID: 7682
		[Dependency]
		private MenuNavigation _menuNavigation;

		// Token: 0x04001E03 RID: 7683
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001E04 RID: 7684
		private float _delayTimer;

		// Token: 0x04001E05 RID: 7685
		private bool _hasPopped;

		// Token: 0x04001E06 RID: 7686
		private UpgradeChoice _nextUpgradeChoice;

		// Token: 0x04001E07 RID: 7687
		[EnumTypedArray(typeof(UpgradeType))]
		[NonReorderable]
		public Sprite[] upgradeSprites = new Sprite[9];

		// Token: 0x04001E08 RID: 7688
		[SerializeField]
		private Sprite _mysterySprite;

		// Token: 0x04001E09 RID: 7689
		private const string MysteryUpgradeDescriptionContentId = "SelectedUpgradeTypeMystery";

		// Token: 0x04001E0A RID: 7690
		[SerializeField]
		private TouchButton _showMapButton;

		// Token: 0x04001E0B RID: 7691
		[SerializeField]
		private Image _showMapButtonTouchCatcher;

		// Token: 0x04001E0C RID: 7692
		public List<GameUpgradeScreen.RectTransformPositionOffset> tutorialRectTransformOffsets = new List<GameUpgradeScreen.RectTransformPositionOffset>();

		// Token: 0x04001E0D RID: 7693
		[SerializeField]
		private UpgradeContainer _classicUpgradeContainer;

		// Token: 0x04001E0E RID: 7694
		[SerializeField]
		private UpgradeContainer _expertUpgradeContainer;

		// Token: 0x04001E0F RID: 7695
		private UpgradeContainer _activeUpgradeContainer;

		// Token: 0x04001E10 RID: 7696
		private int[] _buttonIndexToUpgradeIndex;

		// Token: 0x04001E11 RID: 7697
		private StringId[] DescriptionIds = new StringId[]
		{
			StringId.UpgradeConcreteDescription,
			StringId.UpgradeBridgeDescription,
			StringId.UpgradeMotorwayDescription,
			StringId.UpgradeTrafficLightDescription,
			StringId.UpgradeRoundaboutDescription,
			StringId.UpgradeTunnelDescription
		};

		// Token: 0x04001E12 RID: 7698
		private float _soakTestCountdown;

		// Token: 0x04001E13 RID: 7699
		private bool _uiHidden;

		// Token: 0x04001E14 RID: 7700
		private TweenFloat _visibilityToggleTween = new TweenFloat();

		// Token: 0x04001E15 RID: 7701
		private GameUpgradeScreen.GameUIElements _visibleElements;

		// Token: 0x02000546 RID: 1350
		[System.Serializable]
		public struct RectTransformPositionOffset
		{
			// Token: 0x04001E16 RID: 7702
			public RectTransform rectTransform;

			// Token: 0x04001E17 RID: 7703
			public Vector3 positionOffset;
		}

		// Token: 0x02000547 RID: 1351
		[Flags]
		private enum GameUIElements
		{
			// Token: 0x04001E19 RID: 7705
			None = 0,
			// Token: 0x04001E1A RID: 7706
			Clock = 1,
			// Token: 0x04001E1B RID: 7707
			Score = 2
		}
	}
}
