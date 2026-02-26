using System;
using System.Collections.Generic;
using Factory;
using Motorways.Models;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000530 RID: 1328
	public class ChallengeInfoScreen : BaseScalingScreen
	{
		// Token: 0x06002300 RID: 8960 RVA: 0x0008EDAB File Offset: 0x0008CFAB
		public override void TransitionInTick()
		{
			base.TransitionInTick();
			if (this._changeBlurWhenTransitioning)
			{
				this._gameCamera.customBlur.Strength = this.TransitionInPercentage();
			}
			this._canvasGroup.Alpha = this.TransitionInPercentage();
		}

		// Token: 0x06002301 RID: 8961 RVA: 0x0008EDE2 File Offset: 0x0008CFE2
		public override void TransitionOutTick()
		{
			base.TransitionOutTick();
			if (this._changeBlurWhenTransitioning)
			{
				this._gameCamera.customBlur.Strength = 1f - this.TransitionOutPercentage();
			}
			this._canvasGroup.Alpha = this.TransitionInPercentage();
		}

		// Token: 0x06002302 RID: 8962 RVA: 0x0008EE20 File Offset: 0x0008D020
		public void PrepareScreenForCityChallenge(MapDefinition definition, int challengeIndex, StringId buttonString, bool changeBlurWhenTransitioning, bool showBackButton)
		{
			this._challengeType = MapChallenge.ChallengeType.City;
			this._definition = definition;
			this._challengeIndex = challengeIndex;
			this._changeBlurWhenTransitioning = changeBlurWhenTransitioning;
			this._challenges = new List<ChallengeData>();
			this._challenges.AddRange(definition.cityChallenges[this._challengeIndex].challenges);
			this._playButtonText.SetStringId(this._appScope, buttonString);
			this.backButton.gameObject.SetActive(showBackButton);
			this.firstFocus = this._closeButton;
			this._closeButton.transform.parent.gameObject.SetActive(true);
			this._continueButton.transform.parent.gameObject.SetActive(false);
		}

		// Token: 0x06002303 RID: 8963 RVA: 0x0008EEDC File Offset: 0x0008D0DC
		public void PrepareScreen(MapChallenge.ChallengeType challengeType, List<ChallengeData> challenges, int timeStart, int timeEnd, StringId buttonString, bool changeBlurWhenTransitioning, bool showBackButton, IScope gameScope = null, bool continueIsBack = true)
		{
			this._challengeType = challengeType;
			this._challenges = challenges;
			this._timeStart = timeStart;
			this._timeEnd = timeEnd;
			this._gameScope = gameScope;
			this.backButton.gameObject.SetActive(showBackButton);
			this._playButtonText.SetStringId(this._appScope, buttonString);
			this._changeBlurWhenTransitioning = changeBlurWhenTransitioning;
			this._continueButtonPopsScreen = continueIsBack;
			if (this._gameScope != null)
			{
				this.firstFocus = this._continueButton;
				this._continueButton.transform.parent.gameObject.SetActive(true);
				this._closeButton.transform.parent.gameObject.SetActive(false);
				return;
			}
			this.firstFocus = this._closeButton;
			this._closeButton.transform.parent.gameObject.SetActive(true);
			this._continueButton.transform.parent.gameObject.SetActive(false);
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x0008EFD1 File Offset: 0x0008D1D1
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			if (this._gameScope != null)
			{
				this._appScope.Get<InputState>().BlockGameInput = true;
				this._playerActionController.CancelAllActions();
			}
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x0008F000 File Offset: 0x0008D200
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			base.TransitionIn(outScreen);
			this._skipTransitions = false;
			MotorwaysGame game = null;
			if (this._gameScope != null)
			{
				game = (this._gameScope.Get<Game>() as MotorwaysGame);
				game.SetPaused(true);
			}
			StringId titleString = StringId.MiniMotorways;
			if (this._challengeType == MapChallenge.ChallengeType.Daily)
			{
				titleString = StringId.DailyChallenge;
			}
			else if (this._challengeType == MapChallenge.ChallengeType.Weekly)
			{
				titleString = StringId.WeeklyChallenge;
			}
			else if (this._challengeType == MapChallenge.ChallengeType.City)
			{
				CityChallengeData data;
				if (game != null)
				{
					data = game.MapDefinition.cityChallenges[game.Simulation.GetModel<ActiveChallengesModel>().cityChallengeIndex];
				}
				else
				{
					data = this._definition.cityChallenges[this._challengeIndex];
				}
				Diagnostics.Verify(Enum.TryParse<StringId>(data.titleStringId, out titleString));
			}
			this._challengeTitleType.SetStringId(this._appScope, titleString);
			MapChallenge.ChallengeType challengeType = this._challengeType;
			if (challengeType != MapChallenge.ChallengeType.Daily)
			{
				if (challengeType != MapChallenge.ChallengeType.Weekly)
				{
					this._dateString.LocString = StandaloneLocString.CreateNonLocalizedString(this._appScope, string.Empty);
				}
				else
				{
					DateTime startDate = ChallengeSystem.ToDateTime(this._timeStart);
					DateTime endDate = ChallengeSystem.ToDateTime(this._timeEnd).AddDays(-1.0);
					Dictionary<StringParameterId, string> stringParameters = new Dictionary<StringParameterId, string>
					{
						{
							StringParameterId.StartDate,
							this._localeDatabase.CurrentLocale.FormatDate(startDate, true)
						},
						{
							StringParameterId.EndDate,
							this._localeDatabase.CurrentLocale.FormatDate(endDate, true)
						}
					};
					this._dateString.LocString = StandaloneLocString.CreateString(this._appScope, new MotorwaysStringKey(StringId.WeeklyChallengeDateDuration, stringParameters));
				}
			}
			else
			{
				DateTime startDate2 = ChallengeSystem.ToDateTime(this._timeStart);
				this._dateString.LocString = StandaloneLocString.CreateNonLocalizedString(this._appScope, this._localeDatabase.CurrentLocale.FormatDate(startDate2, false));
			}
			ChallengeDatabase challengeDatabase = this._appScope.Get<ChallengeDatabase>();
			List<ChallengeData> challengesToShow = new List<ChallengeData>();
			challengesToShow.AddRange(this._challenges);
			challengesToShow.Sort(delegate(ChallengeData a, ChallengeData b)
			{
				int aValue = challengeDatabase.IsChallengeWildcard(a) ? 0 : 1;
				int bValue = challengeDatabase.IsChallengeWildcard(b) ? 0 : 1;
				return aValue.CompareTo(bValue);
			});
			for (int challengeIndex = 0; challengeIndex < Math.Max(challengesToShow.Count, this._challengeInfoText.Length); challengeIndex++)
			{
				if (Diagnostics.Verify(challengeIndex < this._challengeInfoText.Length, "We don't have enough challenge info text for the number of challenges! Have {0} need {1}.", this._challengeInfoText.Length, this._challenges.Count))
				{
					if (challengeIndex < challengesToShow.Count)
					{
						this._challengeInfoText[challengeIndex].gameObject.SetActive(true);
						ChallengeData challenge = challengesToShow[challengeIndex];
						bool isWildcard = challengeDatabase.IsChallengeWildcard(challenge);
						this._challengeInfoText[challengeIndex].SetChallengeInfo(challenge, isWildcard, this._appScope);
					}
					else
					{
						this._challengeInfoText[challengeIndex].gameObject.SetActive(false);
					}
				}
			}
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.PauseScreen, this._appScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x0008F2CB File Offset: 0x0008D4CB
		public override void BackActivated()
		{
			if (this.backButton.gameObject.activeInHierarchy)
			{
				base.BackActivated();
				return;
			}
			this.firstFocus.OnSubmit(null);
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x0008F2F2 File Offset: 0x0008D4F2
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._changeBlurWhenTransitioning = (inScreen != ScreenStack.MotorwaysScreen.Pause && inScreen != ScreenStack.MotorwaysScreen.GameOver);
			this._skipTransitions = false;
			if (inScreen == ScreenStack.MotorwaysScreen.InGame)
			{
				MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._appScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
			}
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x0008F327 File Offset: 0x0008D527
		public void OnContinue()
		{
			if (this._continueButtonPopsScreen)
			{
				this._screenStack.PopOneScreen();
				return;
			}
			this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.InGame, false);
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x0008F34A File Offset: 0x0008D54A
		public void OnBack()
		{
			this._screenStack.PopOneScreen();
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x0008F358 File Offset: 0x0008D558
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			if (this._canvas != null && base.gameObject.layer == this._gameCamera.OverlayLayerIndex)
			{
				this._gameCamera.AttachCameraToCanvas(this._canvas, CameraLayer.Overlay);
			}
		}

		// Token: 0x0600230B RID: 8971 RVA: 0x0008F3A4 File Offset: 0x0008D5A4
		public override void Reset()
		{
			this._timeStart = 0;
			this._timeEnd = 0;
			this._challenges = null;
			this._challengeType = MapChallenge.ChallengeType.None;
			this._challengeIndex = -1;
			this._definition = null;
			this._gameScope = null;
			this._changeBlurWhenTransitioning = false;
			this._continueButtonPopsScreen = false;
			base.Reset();
		}

		// Token: 0x04001D17 RID: 7447
		[Dependency]
		protected PlayerActionController _playerActionController;

		// Token: 0x04001D18 RID: 7448
		[Dependency]
		private LocaleDatabase _localeDatabase;

		// Token: 0x04001D19 RID: 7449
		[SerializeField]
		private LocalizedTextUI _challengeTitleType;

		// Token: 0x04001D1A RID: 7450
		[SerializeField]
		private ChallengeInfoText[] _challengeInfoText;

		// Token: 0x04001D1B RID: 7451
		[SerializeField]
		private LocalizedTextUI _playButtonText;

		// Token: 0x04001D1C RID: 7452
		[SerializeField]
		private LocalizedTextUI _dateString;

		// Token: 0x04001D1D RID: 7453
		[SerializeField]
		private TouchButton _continueButton;

		// Token: 0x04001D1E RID: 7454
		[SerializeField]
		private TouchButton _closeButton;

		// Token: 0x04001D1F RID: 7455
		private bool _changeBlurWhenTransitioning;

		// Token: 0x04001D20 RID: 7456
		private bool _continueButtonPopsScreen;

		// Token: 0x04001D21 RID: 7457
		private MapChallenge.ChallengeType _challengeType;

		// Token: 0x04001D22 RID: 7458
		private int _challengeIndex = -1;

		// Token: 0x04001D23 RID: 7459
		private MapDefinition _definition;

		// Token: 0x04001D24 RID: 7460
		private List<ChallengeData> _challenges;

		// Token: 0x04001D25 RID: 7461
		private int _timeStart;

		// Token: 0x04001D26 RID: 7462
		private int _timeEnd;

		// Token: 0x04001D27 RID: 7463
		private IScope _gameScope;
	}
}
