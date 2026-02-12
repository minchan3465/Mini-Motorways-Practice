using System;
using System.Collections;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using JetBrains.Annotations;
using Motorways.Models;
using Motorways.UI;
using Motorways.Utility;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000561 RID: 1377
	public abstract class OverlayBaseScreen : InGameScalingScreen
	{
		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x0600253C RID: 9532
		protected abstract OverlayBaseScreen.OverlayScreenType overlayScreenType { get; }

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x0600253D RID: 9533 RVA: 0x0009CB81 File Offset: 0x0009AD81
		private GameObject ChallengeTextParent
		{
			get
			{
				return this._challengeTitleText.transform.parent.gameObject;
			}
		}

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x0600253E RID: 9534 RVA: 0x0009CB98 File Offset: 0x0009AD98
		protected CanvasGroup nonPhotoLayer
		{
			get
			{
				return this._nonPhotoLayer;
			}
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x0009CBA0 File Offset: 0x0009ADA0
		public override void Awake()
		{
			base.Awake();
			this._floatingElements.AddRange(base.gameObject.GetComponentsInChildren<FloatingElement>());
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x0009CBBE File Offset: 0x0009ADBE
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			this._canvas.worldCamera = this.gameCamera.UICamera;
			this._cameraFramingCanvasGroup.gameObject.SetActive(this.overlayScreenType == OverlayBaseScreen.OverlayScreenType.PhotoScreen);
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x0009CBF8 File Offset: 0x0009ADF8
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			this.SetTimedFramesActive();
			base.TransitionIn(outScreen);
			this._skipTransitions = false;
			this._transitionDetails.spline = new Spline.BezierSplineWithRotation(this._transitionDetails.spline.inPoint, Vector2.zero, Vector2.zero, this._transitionDetails.spline.outHandle, this._transitionDetails.spline.startRotation, this._transitionDetails.spline.endRotation);
			this.SetToolbarVisible(false, false);
			foreach (FloatingElement floatingElement in this._floatingElements)
			{
				floatingElement.Snap();
			}
			this._cameraFrameAlphaTween.Stop();
			this._cameraFramingCanvasGroup.alpha = 0f;
			this._cityTitle.SetStringId(this._gameScope, this.GetMapDefinition().mapName);
			this._cityTitle.gameObject.SetActive(false);
			this._scoreTitle.LocString = StandaloneLocString.CreateLocalizedNumberString(this._gameScope, this._gameScope.Get<ScoreModel>().Score);
			this._scoreTitle.gameObject.SetActive(false);
			MotorwaysStringKey weekKey = this._gameScope.Get<MotorwaysStringKey>();
			City city = this._gameScope.Get<City>();
			if (city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones)
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
			else
			{
				weekKey.InitWithStringId(StringId.WeekCount, this._gameScope.Get<ClockModel>().Week, new Dictionary<string, string>
				{
					{
						"Num",
						(this._gameScope.Get<ClockModel>().Week + 1).ToString()
					}
				});
			}
			this._weekTitle.LocString = StandaloneLocString.CreateString(this._gameScope, weekKey);
			ActiveChallengesModel activeChallengesModel = this._game.Simulation.GetModel<ActiveChallengesModel>();
			ChallengeDatabase challengeDatabase = this._game.Scope.Get<ChallengeDatabase>();
			for (int challengeIndex = 0; challengeIndex < this._challengeIcons.Length; challengeIndex++)
			{
				if (challengeIndex < activeChallengesModel.challenges.Count)
				{
					this._challengeIcons[challengeIndex].gameObject.SetActive(true);
					ChallengeData challenge = activeChallengesModel.challenges[challengeIndex];
					this._challengeIcons[challengeIndex].SetChallengeIcons(challenge.icon, challengeDatabase.IsChallengeWildcard(challenge), challenge.subIcon, challenge.subIconBackground);
				}
				else
				{
					this._challengeIcons[challengeIndex].gameObject.SetActive(false);
				}
			}
			this._challengeIconContainer.SetActive(false);
			this.ChallengeTextParent.gameObject.SetActive(false);
			if (this.overlayScreenType == OverlayBaseScreen.OverlayScreenType.PhotoScreen)
			{
				this.SetupDefaultFrame();
			}
			this.SetFrameElementsAlpha(0f);
			foreach (VehicleView vehicleView in this._gameScope.Get<ViewClient>().GetViews<VehicleView>())
			{
				vehicleView.SkipHeadlightResponseTime = true;
			}
			this._gameScope.Get<TilemapView>().TurnOffMotorwayTransparency();
			if (this.overlayScreenType == OverlayBaseScreen.OverlayScreenType.PhotoScreen)
			{
				this._followNextCarButton.gameObject.SetActive(false);
				this._zoomInButton.gameObject.SetActive(false);
				this._zoomOutButton.gameObject.SetActive(false);
				if (city.GameMode == GameMode.Endless)
				{
					this._titleOptionButton.optionCount = 3;
					this._endlessOptionButton.gameObject.SetActive(true);
					this._challengeDateText.gameObject.SetActive(false);
					this.ChallengeTextParent.SetActive(this._endlessOptionButton.IsOn);
					this._challengeTitleText.SetStringId(this._appScope, StringId.Endless);
					BaseScalingScreen.SetNavigationOnDown(this._frameTouchCycleButton, this._endlessOptionButton);
				}
				else
				{
					this._titleOptionButton.optionCount = 4;
					this._endlessOptionButton.gameObject.SetActive(false);
				}
				if (city.GameMode == GameMode.Expert && !activeChallengesModel.HasChallenges)
				{
					this._expertOptionButton.gameObject.SetActive(true);
					this._challengeDateText.gameObject.SetActive(false);
					this.ChallengeTextParent.SetActive(this._expertOptionButton.IsOn);
					this._challengeTitleText.SetStringId(this._appScope, StringId.Expert);
					BaseScalingScreen.SetNavigationOnDown(this._frameTouchCycleButton, this._expertOptionButton);
				}
				else
				{
					this._expertOptionButton.gameObject.SetActive(false);
				}
				if (city.GameMode == GameMode.Creative)
				{
					this._titleOptionButton.optionCount = 3;
					this._creativeOptionButton.gameObject.SetActive(true);
					this._challengeDateText.gameObject.SetActive(false);
					this.ChallengeTextParent.SetActive(this._creativeOptionButton.IsOn);
					this._challengeTitleText.SetStringId(this._appScope, StringId.Creative);
					BaseScalingScreen.SetNavigationOnDown(this._frameTouchCycleButton, this._creativeOptionButton);
				}
				else
				{
					this._titleOptionButton.optionCount = 4;
					this._creativeOptionButton.gameObject.SetActive(false);
				}
				if (activeChallengesModel.HasChallenges)
				{
					BaseScalingScreen.SetNavigationOnDown(this._frameTouchCycleButton, this._challengeButton);
					return;
				}
			}
			else
			{
				this._pinToggleButton.gameObject.SetActive(false);
				this._titleOptionButton.gameObject.SetActive(false);
				this._frameOptionButton.gameObject.SetActive(false);
				this._challengeOptionButton.gameObject.SetActive(false);
				this._endlessOptionButton.gameObject.SetActive(false);
				this._expertOptionButton.gameObject.SetActive(false);
				this._creativeOptionButton.gameObject.SetActive(false);
				this._followNextCarButton.gameObject.SetActive(true);
				this._zoomInButton.gameObject.SetActive(true);
				this._zoomOutButton.gameObject.SetActive(true);
			}
		}

		// Token: 0x06002542 RID: 9538 RVA: 0x0009D1F0 File Offset: 0x0009B3F0
		private void SetupDefaultFrame()
		{
			if (this.overlayScreenType == OverlayBaseScreen.OverlayScreenType.PhotoScreen)
			{
				ActiveChallengesModel activeChallengesModel = this._game.Simulation.GetModel<ActiveChallengesModel>();
				this._titleOptionButton.SetOption(1);
				this._frameOptionButton.SetOption(2);
				this._endlessOptionButton.Set(true, true);
				this._expertOptionButton.Set(true, true);
				this._creativeOptionButton.Set(true, true);
				if (activeChallengesModel.HasChallenges)
				{
					this._challengeButton.gameObject.SetActive(true);
					this._challengeOptionButton.optionCount = 6;
					StringId challengeStringId = StringId.MiniMotorways;
					if (activeChallengesModel.challengeType == MapChallenge.ChallengeType.Daily)
					{
						challengeStringId = StringId.DailyChallenge;
					}
					else if (activeChallengesModel.challengeType == MapChallenge.ChallengeType.Weekly)
					{
						challengeStringId = StringId.WeeklyChallenge;
					}
					else if (activeChallengesModel.challengeType == MapChallenge.ChallengeType.Mystery)
					{
						challengeStringId = StringId.Challenge_RandomChallengesMapTitle;
					}
					else if (activeChallengesModel.IsCityChallenge)
					{
						Diagnostics.Verify(Enum.TryParse<StringId>(this._game.MapDefinition.cityChallenges[activeChallengesModel.cityChallengeIndex].titleStringId, out challengeStringId));
						this._challengeOptionButton.optionCount = 4;
					}
					this._challengeTitleText.SetStringId(this._appScope, challengeStringId);
					DateTime challengeStartDate = ChallengeSystem.ToDateTime(activeChallengesModel.timeStart);
					if (FeatureToggle.IsFeatureEnabled(Feature.InjectDebugChallenges))
					{
						challengeStartDate = GameDateTime.UtcNow;
					}
					if (FeatureToggle.IsFeatureEnabled(Feature.RandomChallengesMapButton))
					{
						challengeStartDate = GameDateTime.UtcNow;
					}
					this._challengeDateText.LocString = StandaloneLocString.CreateNonLocalizedString(this._appScope, challengeStartDate.ToString(" - yyyy-MM-dd"));
					this._challengeOptionButton.SetOption(2);
				}
			}
		}

		// Token: 0x06002543 RID: 9539 RVA: 0x0009D360 File Offset: 0x0009B560
		public override void TransitionInTick()
		{
			float movementLerp = Easings.CubicEaseInOut(this.TransitionInPercentage());
			Vector3 newPosition = this._transitionDetails.spline.EvaluateLinear(movementLerp);
			this._gameCamera.SetPosition(newPosition);
			this._gameCamera.transform.rotation = this._transitionDetails.spline.EvaluateRotation(movementLerp);
			newPosition.z = base.transform.position.z;
			base.transform.position = newPosition;
			this._gameCamera.OrthographicSize = Mathf.Lerp(this._previousCameraZoom, this._screenStack.GetZoomFor(base.ScreenType), movementLerp);
		}

		// Token: 0x06002544 RID: 9540 RVA: 0x0009D408 File Offset: 0x0009B608
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			if (!this.isToolbarVisible && this.overlayScreenType == OverlayBaseScreen.OverlayScreenType.PhotoScreen)
			{
				this.ToggleToolbarVisibility();
			}
			base.StartCoroutine(this.FadeFrames());
		}

		// Token: 0x06002545 RID: 9541 RVA: 0x0009D433 File Offset: 0x0009B633
		private IEnumerator FadeFrames()
		{
			int num;
			for (int iteration = 0; iteration < 20; iteration = num + 1)
			{
				float alpha = (float)iteration / 20f;
				alpha = Easings.CubicEaseOut(alpha);
				this.SetFrameElementsAlpha(alpha);
				yield return new WaitForSeconds(0.025f);
				num = iteration;
			}
			yield break;
		}

		// Token: 0x06002546 RID: 9542 RVA: 0x0009D444 File Offset: 0x0009B644
		private void SetFrameElementsAlpha(float alpha)
		{
			CanvasGroup[] displayCanvasGroups = this._displayCanvasGroups;
			for (int i = 0; i < displayCanvasGroups.Length; i++)
			{
				displayCanvasGroups[i].alpha = alpha;
			}
		}

		// Token: 0x06002547 RID: 9543 RVA: 0x0009D46F File Offset: 0x0009B66F
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this.SetToolbarVisible(false, false);
			this._gameScope.Get<TilemapView>().TurnOnMotorwayTransparency();
		}

		// Token: 0x06002548 RID: 9544 RVA: 0x0009D490 File Offset: 0x0009B690
		public override void TransitionOutTick()
		{
			base.TransitionOutTick();
			float alpha = Mathf.Clamp01(1f - this.TransitionOutPercentage() * 2f);
			this.SetFrameElementsAlpha(alpha);
		}

		// Token: 0x06002549 RID: 9545 RVA: 0x0009D4C4 File Offset: 0x0009B6C4
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this._isToolbarVisibilityChangeScheduled && !this.IsAnimating)
			{
				this.SetToolbarVisible(this._scheduledToolbarVisibility, false);
			}
			if (this._cameraFrameAlphaTween.IsActive)
			{
				this._cameraFrameAlphaTween.Tick(deltaTime);
				this._cameraFramingCanvasGroup.alpha = this._cameraFrameAlphaTween.Value;
			}
		}

		// Token: 0x0600254A RID: 9546 RVA: 0x0009D525 File Offset: 0x0009B725
		public void OnBack()
		{
			this._game.Scope.Get<NotificationView>().HideNotification();
			this._screenStack.PopOneScreen();
		}

		// Token: 0x0600254B RID: 9547 RVA: 0x0009D548 File Offset: 0x0009B748
		protected void SetFrameLayer(int layerId)
		{
			foreach (object obj in this._frameCanvasGroup.GetComponentInChildren<Transform>(true))
			{
				((Transform)obj).gameObject.layer = layerId;
			}
		}

		// Token: 0x0600254C RID: 9548 RVA: 0x0009D5AC File Offset: 0x0009B7AC
		public virtual void ToggleToolbarVisibility()
		{
			if (this.IsAnimating)
			{
				this._isToolbarVisibilityChangeScheduled = true;
				this._scheduledToolbarVisibility = !this.isToolbarVisible;
				return;
			}
			this.SetToolbarVisible(!this.isToolbarVisible, true);
		}

		// Token: 0x0600254D RID: 9549 RVA: 0x0009D5E0 File Offset: 0x0009B7E0
		public virtual void SetToolbarVisible(bool visible, bool hasAudio = false)
		{
			this.isToolbarVisible = visible;
			this._backButtonAnchor.SetActive(visible);
			this._pinButtonAnchor.SetActive(visible);
			this._toolbarBackgroundAnchor.SetActive(visible);
			City city = this._gameScope.Get<City>();
			this._titleButtonAnchor.SetActive(visible && !(city.Rules is TutorialGameRules));
			this._frameButtonAnchor.SetActive(visible);
			bool hasChallenges = this._game.Simulation.GetModel<ActiveChallengesModel>().HasChallenges;
			bool isEEMode = city.GameMode == GameMode.Endless || city.GameMode == GameMode.Expert;
			bool isCreativeMode = city.GameMode == GameMode.Creative;
			this._challengeButtonAnchor.baseElement.SetActive((hasChallenges || isEEMode || isCreativeMode) && visible);
			this._challengeButtonAnchor.InactiveAnchor.SetActive(hasChallenges || isEEMode || isCreativeMode);
			this._challengeButton.gameObject.SetActive(hasChallenges && city.GameMode != GameMode.Cinematic);
			bool showCameraButton = this.overlayScreenType == OverlayBaseScreen.OverlayScreenType.PhotoScreen && visible && this.softwareCapabilities.CanShareImage;
			this._takePhotoButtonAnchor.SetActive(showCameraButton);
			this._takePhotoButton.interactable = showCameraButton;
			if (visible)
			{
				if (this._appScope.Get<InputState>().CurrentInputTypeRequiresFocus)
				{
					this._appScope.Get<MenuNavigation>().SetNewFocus(this._topButton);
				}
				this._cameraFrameAlphaTween.Start(0f, 1f, 0.1f, Easings.Functions.Linear, 0f);
			}
			else
			{
				this._appScope.Get<MenuNavigation>().SetNewFocus(this._toggleToolbarButton);
				this._cameraFrameAlphaTween.Start(1f, 0f, 0.1f, Easings.Functions.Linear, 0f);
			}
			this._isToolbarVisibilityChangeScheduled = false;
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x0009D797 File Offset: 0x0009B997
		public override void BackActivated()
		{
			VariableDeviceSelectable toggleToolbarButton = this._toggleToolbarButton;
			if (toggleToolbarButton == null)
			{
				return;
			}
			toggleToolbarButton.OnSubmit(null);
		}

		// Token: 0x0600254F RID: 9551 RVA: 0x0009D7AC File Offset: 0x0009B9AC
		public void OnPinToggle(bool value)
		{
			foreach (DestinationView destinationView in this._gameScope.Get<ViewClient>().GetViews<DestinationView>())
			{
				destinationView.SetPinViewVisible(value);
			}
		}

		// Token: 0x06002550 RID: 9552 RVA: 0x0009D808 File Offset: 0x0009BA08
		public void OnTitleCycle(int value)
		{
			if (value == 0)
			{
				this._cityTitle.gameObject.SetActive(false);
				this._scoreTitle.gameObject.SetActive(false);
				this._weekTitle.gameObject.SetActive(false);
			}
			else if (value == 1)
			{
				this._cityTitle.gameObject.SetActive(true);
				this._scoreTitle.gameObject.SetActive(false);
				this._weekTitle.gameObject.SetActive(false);
			}
			else if (value == 2)
			{
				this._cityTitle.gameObject.SetActive(true);
				this._scoreTitle.gameObject.SetActive(false);
				this._weekTitle.gameObject.SetActive(true);
			}
			else if (value == 3)
			{
				this._cityTitle.gameObject.SetActive(true);
				this._scoreTitle.gameObject.SetActive(true);
				this._weekTitle.gameObject.SetActive(false);
			}
			this._divider.SetActive(this.ShouldShowDivider);
		}

		// Token: 0x06002551 RID: 9553 RVA: 0x0009D90A File Offset: 0x0009BB0A
		[UsedImplicitly]
		public void OnEndlessToggled(bool value)
		{
			this.ChallengeTextParent.SetActive(value);
		}

		// Token: 0x06002552 RID: 9554 RVA: 0x0009D90A File Offset: 0x0009BB0A
		[UsedImplicitly]
		public void OnExpertToggled(bool value)
		{
			this.ChallengeTextParent.SetActive(value);
		}

		// Token: 0x06002553 RID: 9555 RVA: 0x0009D90A File Offset: 0x0009BB0A
		[UsedImplicitly]
		public void OnCreativeToggled(bool value)
		{
			this.ChallengeTextParent.SetActive(value);
		}

		// Token: 0x06002554 RID: 9556 RVA: 0x0009D918 File Offset: 0x0009BB18
		[UsedImplicitly]
		public void OnTitleToggle(bool value)
		{
			this._cityTitle.gameObject.SetActive(value);
			this._scoreTitle.gameObject.SetActive(value);
		}

		// Token: 0x06002555 RID: 9557 RVA: 0x0009D93C File Offset: 0x0009BB3C
		[UsedImplicitly]
		public void OnChallengeIconToggled(bool value)
		{
			this._challengeIconContainer.SetActive(value);
			this._divider.SetActive(this.ShouldShowDivider);
		}

		// Token: 0x06002556 RID: 9558 RVA: 0x0009D95C File Offset: 0x0009BB5C
		public void OnChallengeCycled(int value)
		{
			MotorwaysGame game = this._game;
			if (((game != null) ? game.Simulation : null) == null)
			{
				return;
			}
			if (this._game.Simulation.GetModel<ActiveChallengesModel>().IsCityChallenge)
			{
				this.SetCityChallengeConfiguration(value);
				return;
			}
			this.SetTimedChallengeConfiguration(value);
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._challengeTitleText.transform.parent.GetComponent<RectTransform>());
		}

		// Token: 0x06002557 RID: 9559 RVA: 0x0009D9C0 File Offset: 0x0009BBC0
		private void SetCityChallengeConfiguration(int configIndex)
		{
			this._challengeDateText.gameObject.SetActive(false);
			if (configIndex == 0)
			{
				this._challengeTitleText.gameObject.SetActive(false);
				this._challengeIconContainer.SetActive(false);
				return;
			}
			if (configIndex == 1)
			{
				this.ChallengeTextParent.SetActive(true);
				this._challengeTitleText.gameObject.SetActive(true);
				this._challengeIconContainer.SetActive(false);
				return;
			}
			if (configIndex == 2)
			{
				this.ChallengeTextParent.SetActive(true);
				this._challengeTitleText.gameObject.SetActive(true);
				this._challengeIconContainer.SetActive(true);
				return;
			}
			if (configIndex == 3)
			{
				this.ChallengeTextParent.SetActive(false);
				this._challengeTitleText.gameObject.SetActive(false);
				this._challengeIconContainer.SetActive(true);
			}
		}

		// Token: 0x06002558 RID: 9560 RVA: 0x0009DA88 File Offset: 0x0009BC88
		private void SetTimedChallengeConfiguration(int configIndex)
		{
			if (configIndex == 0)
			{
				this._challengeTitleText.gameObject.SetActive(false);
				this._challengeIconContainer.SetActive(false);
				this._challengeDateText.gameObject.SetActive(false);
				return;
			}
			if (configIndex == 1)
			{
				this.ChallengeTextParent.SetActive(true);
				this._challengeTitleText.gameObject.SetActive(true);
				this._challengeIconContainer.SetActive(true);
				this._challengeDateText.gameObject.SetActive(false);
				return;
			}
			if (configIndex == 2)
			{
				this.ChallengeTextParent.SetActive(true);
				this._challengeTitleText.gameObject.SetActive(true);
				this._challengeIconContainer.SetActive(true);
				this._challengeDateText.gameObject.SetActive(true);
				return;
			}
			if (configIndex == 3)
			{
				this.ChallengeTextParent.SetActive(true);
				this._challengeTitleText.gameObject.SetActive(true);
				this._challengeIconContainer.SetActive(false);
				this._challengeDateText.gameObject.SetActive(true);
				return;
			}
			if (configIndex == 4)
			{
				this.ChallengeTextParent.SetActive(true);
				this._challengeTitleText.gameObject.SetActive(true);
				this._challengeIconContainer.SetActive(false);
				this._challengeDateText.gameObject.SetActive(false);
				return;
			}
			if (configIndex == 5)
			{
				this.ChallengeTextParent.SetActive(false);
				this._challengeTitleText.gameObject.SetActive(false);
				this._challengeIconContainer.SetActive(true);
				this._challengeDateText.gameObject.SetActive(false);
			}
		}

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06002559 RID: 9561 RVA: 0x0009DC01 File Offset: 0x0009BE01
		private bool ShouldShowDivider
		{
			get
			{
				return this._cityTitle.gameObject.activeInHierarchy && (this._scoreTitle.gameObject.activeInHierarchy || this._weekTitle.gameObject.activeInHierarchy);
			}
		}

		// Token: 0x0600255A RID: 9562 RVA: 0x0009DC3B File Offset: 0x0009BE3B
		public override void Reset()
		{
			base.Reset();
			this.isToolbarVisible = false;
			this._isToolbarVisibilityChangeScheduled = false;
			this._scheduledToolbarVisibility = false;
		}

		// Token: 0x0600255B RID: 9563 RVA: 0x0009DC58 File Offset: 0x0009BE58
		private void SetTimedFramesActive()
		{
			DateTime christmasPeriodStart = new DateTime(GameDateTime.LocalNow.Year, 11, 8);
			DateTime newYearPeriodStart = new DateTime(GameDateTime.LocalNow.Year, 12, 22);
			DateTime holidayPeriodEnd = new DateTime(GameDateTime.LocalNow.Year, 1, 5);
			this._frameOptionButton.UnskipOption(4);
			this._frameOptionButton.UnskipOption(5);
			this._frameOptionButton.UnskipOption(6);
			if (!(GameDateTime.LocalNow > christmasPeriodStart) && !(GameDateTime.LocalNow < holidayPeriodEnd))
			{
				this._frameOptionButton.SkipOption(4);
				this._frameOptionButton.SkipOption(5);
			}
			if (!(GameDateTime.LocalNow > newYearPeriodStart) && !(GameDateTime.LocalNow < holidayPeriodEnd))
			{
				this._frameOptionButton.SkipOption(6);
			}
		}

		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x0600255C RID: 9564 RVA: 0x0009DD28 File Offset: 0x0009BF28
		private bool IsAnimating
		{
			get
			{
				if (this._cameraFrameAlphaTween.IsActive)
				{
					return true;
				}
				using (List<FloatingElement>.Enumerator enumerator = this._floatingElements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsAnimating)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x04001F60 RID: 8032
		[Dependency]
		protected ISoftwareCapabilities softwareCapabilities;

		// Token: 0x04001F61 RID: 8033
		[Dependency]
		protected GameCamera gameCamera;

		// Token: 0x04001F62 RID: 8034
		[SerializeField]
		private GameObject _backButtonAnchor;

		// Token: 0x04001F63 RID: 8035
		[SerializeField]
		private GameObject _pinButtonAnchor;

		// Token: 0x04001F64 RID: 8036
		[SerializeField]
		private GameObject _titleButtonAnchor;

		// Token: 0x04001F65 RID: 8037
		[SerializeField]
		private GameObject _frameButtonAnchor;

		// Token: 0x04001F66 RID: 8038
		[SerializeField]
		private FloatingElement _challengeButtonAnchor;

		// Token: 0x04001F67 RID: 8039
		[SerializeField]
		private GameObject _takePhotoButtonAnchor;

		// Token: 0x04001F68 RID: 8040
		[SerializeField]
		private TouchToggle _pinToggleButton;

		// Token: 0x04001F69 RID: 8041
		[SerializeField]
		private SymbolOptionButton _titleOptionButton;

		// Token: 0x04001F6A RID: 8042
		[SerializeField]
		private SymbolOptionButton _frameOptionButton;

		// Token: 0x04001F6B RID: 8043
		[SerializeField]
		private SymbolOptionButton _challengeOptionButton;

		// Token: 0x04001F6C RID: 8044
		[SerializeField]
		private TouchToggle _endlessOptionButton;

		// Token: 0x04001F6D RID: 8045
		[SerializeField]
		private TouchToggle _expertOptionButton;

		// Token: 0x04001F6E RID: 8046
		[SerializeField]
		private TouchToggle _creativeOptionButton;

		// Token: 0x04001F6F RID: 8047
		[SerializeField]
		private TouchButton _challengeButton;

		// Token: 0x04001F70 RID: 8048
		[SerializeField]
		private TouchButton _followNextCarButton;

		// Token: 0x04001F71 RID: 8049
		[SerializeField]
		protected TouchButton _zoomInButton;

		// Token: 0x04001F72 RID: 8050
		[SerializeField]
		protected TouchButton _zoomOutButton;

		// Token: 0x04001F73 RID: 8051
		[SerializeField]
		private GameObject _toolbarBackgroundAnchor;

		// Token: 0x04001F74 RID: 8052
		[SerializeField]
		private VariableDeviceSelectable _topButton;

		// Token: 0x04001F75 RID: 8053
		[SerializeField]
		private VariableDeviceSelectable _toggleToolbarButton;

		// Token: 0x04001F76 RID: 8054
		[SerializeField]
		private TouchButton _takePhotoButton;

		// Token: 0x04001F77 RID: 8055
		[SerializeField]
		private TouchButton _frameTouchCycleButton;

		// Token: 0x04001F78 RID: 8056
		[SerializeField]
		private LocalizedTextUI _cityTitle;

		// Token: 0x04001F79 RID: 8057
		[SerializeField]
		private LocalizedTextUI _scoreTitle;

		// Token: 0x04001F7A RID: 8058
		[SerializeField]
		private LocalizedTextUI _weekTitle;

		// Token: 0x04001F7B RID: 8059
		[SerializeField]
		private GameObject _divider;

		// Token: 0x04001F7C RID: 8060
		[SerializeField]
		private GameObject _challengeIconContainer;

		// Token: 0x04001F7D RID: 8061
		[SerializeField]
		private LocalizedTextUI _challengeTitleText;

		// Token: 0x04001F7E RID: 8062
		[SerializeField]
		private LocalizedTextUI _challengeDateText;

		// Token: 0x04001F7F RID: 8063
		[SerializeField]
		private CanvasGroup _nonPhotoLayer;

		// Token: 0x04001F80 RID: 8064
		[SerializeField]
		private CanvasGroup _cameraFramingCanvasGroup;

		// Token: 0x04001F81 RID: 8065
		[SerializeField]
		private CanvasGroup[] _displayCanvasGroups;

		// Token: 0x04001F82 RID: 8066
		[SerializeField]
		private CanvasGroup _frameCanvasGroup;

		// Token: 0x04001F83 RID: 8067
		[SerializeField]
		private ChallengeIcon[] _challengeIcons;

		// Token: 0x04001F84 RID: 8068
		private bool _isToolbarVisibilityChangeScheduled;

		// Token: 0x04001F85 RID: 8069
		private bool _scheduledToolbarVisibility;

		// Token: 0x04001F86 RID: 8070
		private List<FloatingElement> _floatingElements = new List<FloatingElement>();

		// Token: 0x04001F87 RID: 8071
		private TweenFloat _cameraFrameAlphaTween = new TweenFloat();

		// Token: 0x04001F88 RID: 8072
		protected bool isToolbarVisible;

		// Token: 0x02000562 RID: 1378
		public enum OverlayScreenType
		{
			// Token: 0x04001F8A RID: 8074
			PhotoScreen,
			// Token: 0x04001F8B RID: 8075
			CinematicModeScreen
		}
	}
}
