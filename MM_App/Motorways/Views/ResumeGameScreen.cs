using System;
using System.Collections.Generic;
using Client;
using Factory;
using Motorways.Audio;
using Motorways.Leaderboards;
using Motorways.UI;
using NaughtyAttributes;
using Popups;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x0200056B RID: 1387
	public class ResumeGameScreen : ScrollingButtonScreen
	{
		// Token: 0x060025C5 RID: 9669 RVA: 0x0008F34A File Offset: 0x0008D54A
		public void OnBack()
		{
			this._screenStack.PopOneScreen();
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x0009FD30 File Offset: 0x0009DF30
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			if (this._player.HasLocalSavedGame || this._player.HasForeignSavedGames)
			{
				this.CreateResumeMapButtons();
			}
			base.TransitionIn(outScreen);
			this._recreateResumeMapButtons = false;
			this._player.SavedGamesChanged += this.ScheduleMapButtonRecreation;
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x0009FD83 File Offset: 0x0009DF83
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._player.SavedGamesChanged -= this.ScheduleMapButtonRecreation;
		}

		// Token: 0x060025C8 RID: 9672 RVA: 0x0009FDA3 File Offset: 0x0009DFA3
		public override void OnTransitionedOut()
		{
			base.OnTransitionedOut();
			this.DestroyResumeButtons();
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x0009FDB1 File Offset: 0x0009DFB1
		public ResumeMapButton ResumeButtonAt(int index)
		{
			return this.buttons[index].GetComponent<ResumeMapButton>();
		}

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x060025CA RID: 9674 RVA: 0x0009FDC4 File Offset: 0x0009DFC4
		public IEnumerable<ResumeMapButton> MapButtons
		{
			get
			{
				foreach (AnimatedCard button in this.buttons)
				{
					yield return button.GetComponent<ResumeMapButton>();
				}
				List<AnimatedCard>.Enumerator enumerator = default(List<AnimatedCard>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060025CB RID: 9675 RVA: 0x0009FDD4 File Offset: 0x0009DFD4
		public override void Tick(float deltaTime)
		{
			if (this._recreateResumeMapButtons)
			{
				this._recreateResumeMapButtons = false;
				if (!this.CreateResumeMapButtons() && !this.IsTransitioningOut())
				{
					this.OnBack();
				}
			}
			base.Tick(deltaTime);
			if (this._gameStarter != null && this._gameStarter.CanStart)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.MenuExit, 0.5f, -1f, true, null));
				this._gameStarter.Start(this._screenStack, this._appScope);
				this._gameStarter = null;
			}
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x0009FE70 File Offset: 0x0009E070
		public void SelectGame(ResumeMapButton button)
		{
			if (!Diagnostics.Verify(this._screenStack.GetActiveScreen<GameContainerScreen>() == null, "Attempting to start a game while there already is a game. Earlying out for safety"))
			{
				return;
			}
			MotorwaysGameJournalSave save;
			if (button.GameID == "localsave")
			{
				save = (MotorwaysGameJournalSave)this._player.LocalSavedGame;
			}
			else
			{
				save = (MotorwaysGameJournalSave)this._player.GetForeignSavedGame(button.GameID);
			}
			if (Diagnostics.Verify(save != null, "Tried to reload a save of a game we should have by now!"))
			{
				List<MotorwaysGameJournalSave> activeDailyChallengeSaves = this._challengeSystem.GetActiveDailyChallengeSaves(this._player, false);
				if (activeDailyChallengeSaves.Count == 0 || activeDailyChallengeSaves.Contains(save))
				{
					this.BeginTransitionIntoSaveGame(save);
					return;
				}
				if (this._player.GetChallengeScore(MapChallenge.ChallengeType.Daily, this._challengeSystem.DailyChallenge.TimeEnd).ScoreState == LeaderboardScoreState.Editable)
				{
					this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.DailyChallenge, null, delegate()
					{
						this.BeginTransitionIntoSaveGame(save);
					}, StringId.DailyChallenge_SaveGameConfirmationResumeGame);
					return;
				}
				this.BeginTransitionIntoSaveGame(save);
			}
		}

		// Token: 0x060025CD RID: 9677 RVA: 0x0009FFA0 File Offset: 0x0009E1A0
		private void BeginTransitionIntoSaveGame(MotorwaysGameJournalSave save)
		{
			if (this._gameStarter == null)
			{
				if (this._skipTransitions)
				{
					this._screenStack.FadeNextTransition(this.skippedTransitionFadeDuration);
				}
				this._gameStarter = new GameStarter(this);
			}
			this._gameStarter.StartFromSavedGame(this.mapLibrary, save, true, false, false);
		}

		// Token: 0x060025CE RID: 9678 RVA: 0x0009FFF0 File Offset: 0x0009E1F0
		public void DeleteGame(ResumeMapButton button)
		{
			MotorwaysGameJournalSave save;
			if (button.GameID == "localsave")
			{
				save = (MotorwaysGameJournalSave)this._player.LocalSavedGame;
			}
			else
			{
				save = (MotorwaysGameJournalSave)this._player.GetForeignSavedGame(button.GameID);
			}
			if (Diagnostics.Verify(save != null, "Tried to delete a save of a game we should have by now!"))
			{
				this._savePendingDelete = save;
				this.popupStack.PushPopup<ConfirmationPopup>(0f, false).Initialise(this._appScope, StringId.ConfirmDeleteSpecificJournal, new Action(this.OnCancelDeleteSpecificSave), new Action(this.OnConfirmSpecificSaveData), this._softwareCapabilities.DeleteCloudGameStringId);
			}
		}

		// Token: 0x060025CF RID: 9679 RVA: 0x000A0094 File Offset: 0x0009E294
		public void OnConfirmSpecificSaveData()
		{
			this._player.RemoveSavedGame(this._savePendingDelete);
			this._savePendingDelete = null;
			if (this.CreateResumeMapButtons())
			{
				this.OnTransitionedIn();
				this._appScope.Get<MenuNavigation>().SetNewFocus(this.ResumeButtonAt(0).playTouchButton);
				return;
			}
			this.OnBack();
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x000A00EA File Offset: 0x0009E2EA
		public void OnCancelDeleteSpecificSave()
		{
			this._appScope.Get<MenuNavigation>().SetNewFocus((base.ButtonCount > 0) ? this.ResumeButtonAt(0).playTouchButton : this.backButton);
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x000A0119 File Offset: 0x0009E319
		private void ScheduleMapButtonRecreation()
		{
			ResumeGameScreen.Log.Info("Changes to the remote saves detected, scheduling an update for the next tick.", Array.Empty<object>());
			this._recreateResumeMapButtons = true;
		}

		// Token: 0x060025D2 RID: 9682 RVA: 0x000A0138 File Offset: 0x0009E338
		private bool CreateResumeMapButtons()
		{
			this.DestroyResumeButtons();
			if (this._player.HasLocalSavedGame)
			{
				MotorwaysGameJournalSave localSavedGame = (MotorwaysGameJournalSave)this._player.LocalSavedGame;
				this.AddResumeButton(localSavedGame, "localsave");
			}
			foreach (IGameJournalSave foreignSavedGame in this._player.ForeignSavedGames)
			{
				this.AddResumeButton((MotorwaysGameJournalSave)foreignSavedGame, foreignSavedGame.DeviceId);
			}
			if (base.ButtonCount > 0)
			{
				this.firstFocus = this.ResumeButtonAt(0).playTouchButton;
			}
			base.RegisterAllLocalizedTextChildren();
			base.RegisterButtons();
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			base.SetMapButtonValues(this.scrollRect.normalizedPosition);
			return base.ButtonCount > 0;
		}

		// Token: 0x060025D3 RID: 9683 RVA: 0x000A0218 File Offset: 0x0009E418
		private void AddResumeButton(MotorwaysGameJournalSave savedGame, string savedGameId)
		{
			if (savedGame != null)
			{
				MapDefinition definition = this.mapLibrary.GetMapByName(savedGame.CityId);
				if (definition != null)
				{
					ResumeMapButton newButton = UnityEngine.Object.Instantiate<ResumeMapButton>(this.resumeButtonPrefab, this.buttonParent);
					newButton.Initialize(this, savedGameId, savedGame, definition, this._appScope);
					this.buttons.Add(newButton);
				}
			}
		}

		// Token: 0x060025D4 RID: 9684 RVA: 0x000A0274 File Offset: 0x0009E474
		public override void ApplyTheme(ITheme newTheme)
		{
			base.ApplyTheme(newTheme);
			foreach (ResumeMapButton resumeMapButton in this.MapButtons)
			{
				resumeMapButton.ApplyTheme();
			}
		}

		// Token: 0x060025D5 RID: 9685 RVA: 0x000A02C8 File Offset: 0x0009E4C8
		public override void ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			base.ApplyBlendedTheme(oldTheme, newTheme, progress);
			foreach (ResumeMapButton resumeMapButton in this.MapButtons)
			{
				resumeMapButton.ApplyTheme();
			}
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x000A031C File Offset: 0x0009E51C
		private void DestroyResumeButtons()
		{
			base.UnregisterButtons();
			base.UnregisterLocalizedTextChildren();
			this.UnregisterThemeComponents();
			if (base.ButtonCount > 0)
			{
				for (int buttonIndex = 0; buttonIndex < base.ButtonCount; buttonIndex++)
				{
					this.buttons[buttonIndex].gameObject.transform.SetParent(null);
					UnityEngine.Object.Destroy(this.buttons[buttonIndex].gameObject);
				}
				this.buttons.Clear();
			}
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x000A0392 File Offset: 0x0009E592
		public override void OnCreatedInScope(IScope scope)
		{
			base.OnCreatedInScope(scope);
			this.scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(base.SetMapButtonValues));
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x000A03B7 File Offset: 0x0009E5B7
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			this.DestroyResumeButtons();
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x000022F5 File Offset: 0x000004F5
		public override void OnMoveCursor(Selectable currentFocus, MoveDirection direction)
		{
		}

		// Token: 0x04001FD1 RID: 8145
		[Dependency]
		private ChallengeSystem _challengeSystem;

		// Token: 0x04001FD2 RID: 8146
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001FD3 RID: 8147
		public MapLibrary mapLibrary;

		// Token: 0x04001FD4 RID: 8148
		private GameStarter _gameStarter;

		// Token: 0x04001FD5 RID: 8149
		public ResumeMapButton resumeButtonPrefab;

		// Token: 0x04001FD6 RID: 8150
		private IGameJournalSave _savePendingDelete;

		// Token: 0x04001FD7 RID: 8151
		public CanvasGroup mapButtonsCanvas;

		// Token: 0x04001FD8 RID: 8152
		[MinValue(0)]
		[Tooltip("The duration of the fade to black if Skip Transitions is on")]
		public float skippedTransitionFadeDuration = 1f;

		// Token: 0x04001FD9 RID: 8153
		private bool _recreateResumeMapButtons;

		// Token: 0x04001FDA RID: 8154
		public const string LocalSaveGameID = "localsave";

		// Token: 0x04001FDB RID: 8155
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ResumeGameScreen");
	}
}
