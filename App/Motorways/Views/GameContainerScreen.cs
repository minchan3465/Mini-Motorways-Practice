using System;
using System.Collections.Generic;
using Client;
using Easing;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.Processes;
using NaughtyAttributes;
using NotificationService.Events;
using Screens;
using Server;
using SoftwareCapabilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

namespace Motorways.Views
{
	// Token: 0x02000535 RID: 1333
	public class GameContainerScreen : BaseScalingScreen, IGameStartScreen
	{
		// Token: 0x17000641 RID: 1601
		// (get) Token: 0x06002331 RID: 9009 RVA: 0x0008FD68 File Offset: 0x0008DF68
		public string CurrentCityName
		{
			get
			{
				MapDefinition newMapDefinition = this._newMapDefinition;
				if (newMapDefinition == null)
				{
					return null;
				}
				return newMapDefinition.cityName;
			}
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x0008FD7C File Offset: 0x0008DF7C
		public override void Reset()
		{
			base.Reset();
			this._hasSeenChallenges = false;
			this._hasSeenModeInfo = false;
			this._startGameOnTransition = false;
			this._gameMode = GameMode.Normal;
			this._playerPausedGame = false;
			this._isTransitioningOutToBeReleased = false;
			this._overrideTransitionInAnimation = false;
			this._startPaused = false;
			this._recentlyExitedCinematicMode = false;
			this._gameSuspended = false;
		}

		// Token: 0x06002333 RID: 9011 RVA: 0x0008FDD8 File Offset: 0x0008DFD8
		public virtual void PrepareForMap(CityDefinition newCity, MapDefinition newMapDefinition, GameMode gameMode, MapChallenge newMapChallenge = null, bool startPaused = false)
		{
			if (this._game != null || this._newCity != null)
			{
				GameContainerScreen.Log.Warn("We have not properly cleaned up the last game! It is possible the game container screen has not been disposed quickly enough. Cleaning up the game now.", Array.Empty<object>());
				this.CleanupPreviousGame();
			}
			this._newCity = newCity;
			this._newMapDefinition = newMapDefinition;
			this._gameMode = gameMode;
			this._newMapChallenge = newMapChallenge;
			this._startGameOnTransition = true;
			this._overrideTransitionInAnimation = true;
			this._startPaused = startPaused;
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			MapChallenge.ChallengeType challengeType = (newMapChallenge != null) ? newMapChallenge.type : MapChallenge.ChallengeType.None;
			int challengeIndex = (newMapChallenge != null) ? newMapChallenge.cityChallengeIndex : -1;
			this._analytics.TrackGameStarted(newMapDefinition, challengeType, challengeIndex, gameMode, this._themeDatabase.ThemePreference);
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
			{
				ParticleSystem[] componentsInChildren = newCity.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06002334 RID: 9012 RVA: 0x0008FEC0 File Offset: 0x0008E0C0
		public virtual void PrepareForNewGame(CityDefinition newCity, MapDefinition newMapDefinition, MotorwaysGame game, MapChallenge newMapChallenge = null, bool startPaused = false)
		{
			if (this._game != null || this._newCity != null)
			{
				GameContainerScreen.Log.Warn("We have not properly cleaned up the last game! It is possible the game container screen has not been disposed quickly enough. Cleaning up the game now.", Array.Empty<object>());
				this.CleanupPreviousGame();
			}
			this._game = game;
			this._newCity = newCity;
			this._newMapDefinition = newMapDefinition;
			ISimulation simulation = this._game.Simulation;
			GameMode? gameMode;
			if (simulation == null)
			{
				gameMode = null;
			}
			else
			{
				CityModel model = simulation.GetModel<CityModel>();
				gameMode = ((model != null) ? new GameMode?(model.Mode) : null);
			}
			GameMode? gameMode2 = gameMode;
			this._gameMode = gameMode2.GetValueOrDefault();
			this._newMapChallenge = newMapChallenge;
			this._startGameOnTransition = true;
			this._overrideTransitionInAnimation = true;
			this._startPaused = startPaused;
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
			this._analytics.TrackGameResumed(game, newMapDefinition);
			if (this._newCity.bonusTreeGrassObjects != null)
			{
				bool usesBonusTrees = this._game.Simulation.GetModel<GameBehaviourModel>().UsesBonusTrees;
				GameObject[] bonusTreeGrassObjects = this._newCity.bonusTreeGrassObjects;
				for (int i = 0; i < bonusTreeGrassObjects.Length; i++)
				{
					bonusTreeGrassObjects[i].SetActive(usesBonusTrees);
				}
			}
		}

		// Token: 0x06002335 RID: 9013 RVA: 0x0008FFDC File Offset: 0x0008E1DC
		public override void RegisterThemeComponents(ITheme theme)
		{
			base.RegisterThemeComponents(theme);
			if (this._newCity != null)
			{
				List<IThemeComponent> mapAssets = new List<IThemeComponent>();
				this._newCity.GetComponentsInChildren<IThemeComponent>(true, mapAssets);
				if (mapAssets != null)
				{
					foreach (IThemeComponent themeComponent in mapAssets)
					{
						themeComponent.InitializeTheme(this._themeDatabase);
					}
				}
				if (this.themeComponents == null)
				{
					this.themeComponents = mapAssets;
					return;
				}
				this.themeComponents.AddRange(mapAssets);
			}
		}

		// Token: 0x06002336 RID: 9014 RVA: 0x00090074 File Offset: 0x0008E274
		public override void ScaleToCamera()
		{
			base.ScaleToGameCamera();
		}

		// Token: 0x06002337 RID: 9015 RVA: 0x0009007C File Offset: 0x0008E27C
		public virtual void PrepareForRestartMap(GameMode gameMode)
		{
			this._gameMode = gameMode;
			this.ReleaseGame();
			MapChallenge newMapChallenge = this._newMapChallenge;
			MapChallenge.ChallengeType challengeType = (newMapChallenge != null) ? newMapChallenge.type : MapChallenge.ChallengeType.None;
			MapChallenge newMapChallenge2 = this._newMapChallenge;
			int challengeIndex = (newMapChallenge2 != null) ? newMapChallenge2.cityChallengeIndex : -1;
			this._analytics.TrackGameStarted(this._newMapDefinition, challengeType, challengeIndex, this._gameMode, this._themeDatabase.ThemePreference);
			this._startGameOnTransition = true;
		}

		// Token: 0x06002338 RID: 9016 RVA: 0x000900E8 File Offset: 0x0008E2E8
		public virtual void PrepareForContinueInEndless()
		{
			this._analytics.TrackGameContinued(this._game, this._newMapDefinition);
			this._startGameOnTransition = false;
			this._gameMode = GameMode.Endless;
			foreach (RoundaboutModel roundabout in this._game.Simulation.GetModels<RoundaboutModel>())
			{
				if (roundabout.CenterTileModel.Tile.IsRoundaboutPermanent)
				{
					roundabout.RestoreConcreteFromStoredReplacedConnections(RoundaboutModel.ConcreteRestoreType.Release);
				}
				roundabout.ClearReplacedConnections();
			}
			this._game.ContinueInMode(GameMode.Endless);
			this._playerActionController.SetGameScope(this._game.Scope);
			this._gameUIScreen.ScoreView.SetupView();
			this._hasSeenModeInfo = false;
			this._game.Simulation.GetModel<ScoreModel>().OnContinuedInEndless();
			this._game.Scope.Get<ActiveChallengesModel>().RemoveChallengesForEndless();
			this._game.Scope.Get<AchievementCheckingProcess>().Reset();
			this._gameUIScreen.UpgradeBar.RefreshAllAvailableUpgradeStacks();
			this.ReconfigurePermanenceVisibility();
			this.ResetLaneModelLaneSpeeds(this._game.Simulation);
		}

		// Token: 0x06002339 RID: 9017 RVA: 0x00090204 File Offset: 0x0008E404
		private void ResetLaneModelLaneSpeeds(ISimulation simulation)
		{
			foreach (LaneModel laneModel in simulation.GetModels<LaneModel>())
			{
				laneModel.SetSpeedLimitScale(Fix64.One);
			}
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x0009023C File Offset: 0x0008E43C
		private void ReconfigurePermanenceVisibility()
		{
			ViewClient viewClient = this._game.Scope.Get<ViewClient>();
			foreach (RoundaboutView roundaboutView in viewClient.GetViews<RoundaboutView>())
			{
				roundaboutView.ReconfigurePermanenceVisibility();
			}
			foreach (TrafficLightView trafficLightView in viewClient.GetViews<TrafficLightView>())
			{
				trafficLightView.ReconfigurePermanenceVisibility();
			}
			foreach (MotorwayView motorwayView in viewClient.GetViews<MotorwayView>())
			{
				motorwayView.ReconfigurePermanenceVisibility();
			}
			foreach (TileView tileView in viewClient.GetViews<TileView>())
			{
				tileView.ReconfigurePermanenceVisibility();
			}
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x0009035C File Offset: 0x0008E55C
		public void SetRecentlyExitedCinematicMode(bool recentlyExitedCinematicMode)
		{
			this._recentlyExitedCinematicMode = recentlyExitedCinematicMode;
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x00090368 File Offset: 0x0008E568
		public override void TransitionInTick()
		{
			base.TransitionInTick();
			float oneOverPercentageToUseForFirstHalf = 1f / this._constants.PercentageOfDurationToUseForInitialMovement;
			float menuGridVisibility = Mathf.Clamp01((this.TransitionInPercentage() - this._constants.PercentageOfDurationToUseForInitialMovement) * -1f * oneOverPercentageToUseForFirstHalf);
			this._menuPlacementDefinition.SetGridAlpha(menuGridVisibility);
			if (this._overrideTransitionInAnimation)
			{
				Vector3 newPosition = this._constants.GetCameraPositionForTransitionToGame(this._transitionDetails, this.TransitionInPercentage(), this._newCity);
				this._gameCamera.SetPosition(newPosition);
				if (this.TransitionInPercentage() < this._constants.PercentageOfDurationToUseForInitialMovement)
				{
					this._newCity.gameObject.SetActive(false);
				}
				else
				{
					this._newCity.gameObject.SetActive(true);
				}
			}
			else
			{
				float lerp = Easings.CubicEaseInOut(this.TransitionInPercentage());
				Vector3 newPosition2 = this._transitionDetails.spline.Evaluate(lerp);
				newPosition2 = Vector3.Lerp(newPosition2, this._game.Scope.Get<CameraView>().DesiredPosition, lerp);
				this._gameCamera.SetPosition(newPosition2);
			}
			if (this.TransitionInPercentage() > this._percentageOfTransitionInToStartBluringForChallenges && !this._game.PlayingBackSimJournal && this._game.Simulation.GetModel<ActiveChallengesModel>().HasChallenges)
			{
				float multiplier = 1f / (1f - this._percentageOfTransitionInToStartBluringForChallenges);
				float blur = (this.TransitionInPercentage() - this._percentageOfTransitionInToStartBluringForChallenges) * multiplier;
				this._gameCamera.customBlur.Strength = blur;
			}
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x000904E0 File Offset: 0x0008E6E0
		public override void TransitionOutTick()
		{
			base.TransitionOutTick();
			if (!this._screenStack.IsScreenInStack(ScreenStack.MotorwaysScreen.InGame))
			{
				this._menuPlacementDefinition.background.SetActive(true);
				this._menuPlacementDefinition.grid.enabled = true;
				float invertedPercentageToUse = 1f - this._constants.PercentageOfDurationToUseForInitialMovement;
				float multiplierForScale = 1f / this._constants.PercentageOfDurationToUseForInitialMovement;
				float menuGridVisibility = Mathf.Clamp01((this.TransitionOutPercentage() - invertedPercentageToUse) * multiplierForScale);
				this._menuPlacementDefinition.SetGridAlpha(menuGridVisibility);
			}
			if (this.TransitionOutPercentage() > 1f - this._constants.PercentageOfDurationToUseForInitialMovement && this._isTransitioningOutToBeReleased && this._newCity != null)
			{
				this._game.Scope.Get<ViewClient>().SetAllGameObjectsEnabled(false);
				this._newCity.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x000905BC File Offset: 0x0008E7BC
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			StartupScreen startupScreen = this._screenStack.GetActiveScreen<StartupScreen>();
			if (startupScreen != null)
			{
				startupScreen.StopMenuGameAudio();
			}
			this._skipTransitions = (this._skipTransitions && outScreen != ScreenStack.MotorwaysScreen.GameOver);
			if (this._startGameOnTransition)
			{
				this._startGameOnTransition = false;
				if (this._game == null)
				{
					this._game = this._appScope.Get<MotorwaysGame>();
				}
				this._game.SetMapDefinition(this._newMapDefinition);
				this._game.Start(this._newCity, this._gameMode, this._newMapChallenge, false);
				this._game.SetPaused(false);
				this._game.Tick(0f);
				this._game.Tick(0f);
				this._gameUIScreen = this._game.Scope.Get<GameUIScreen>();
				this.backButton = this._gameUIScreen.backButton;
				this._cameraView = this._game.Scope.Get<CameraView>();
				this._game.OnGameStarted();
				if (this._newMapChallenge != null)
				{
					ChallengeType? notificationChallengeType = null;
					switch (this._newMapChallenge.type)
					{
					case MapChallenge.ChallengeType.Daily:
						notificationChallengeType = new ChallengeType?(ChallengeType.Daily);
						break;
					case MapChallenge.ChallengeType.Weekly:
						notificationChallengeType = new ChallengeType?(ChallengeType.Weekly);
						break;
					case MapChallenge.ChallengeType.Mystery:
					case MapChallenge.ChallengeType.City:
						break;
					default:
						Diagnostics.FailAssert("Unknown challenge type for notifications. ({0})", new object[]
						{
							this._newMapChallenge.type
						});
						break;
					}
					if (notificationChallengeType != null)
					{
						this._notificationEventSystem.RecordEvent(new PlayedChallenge
						{
							Type = notificationChallengeType.Value,
							TimeStart = this._newMapChallenge.TimeStart
						}, true);
					}
				}
				else
				{
					this._notificationEventSystem.RecordEvent(new PlayedMap
					{
						Map = this._newMapDefinition.CityNameEnum
					}, true);
				}
				if (this.GetTransitionDuration() <= 1E-45f)
				{
					this._themeDatabase.SnapCurrentTransition();
				}
			}
			this._softwareCapabilities.SetIsInMainMenuScreen(false);
			this._softwareCapabilities.SetIsInGame(true);
			this._softwareCapabilities.SetRichPresence(this.GetSteamRichPresenceTokens());
			base.TransitionIn(outScreen);
			this._themeDatabase.SetCurrentMapDefinition(this._newMapDefinition, this.GetTransitionDuration());
			RuntimeAppCommandSource runtimeAppCommandSource = this._runtimeAppCommandSource as RuntimeAppCommandSource;
			if (runtimeAppCommandSource != null)
			{
				runtimeAppCommandSource.SetRewiredMode(2);
			}
			this._skipTransitions = (this._skipTransitions && outScreen != ScreenStack.MotorwaysScreen.GameOver);
			if (this._newCity.CityTilemapMeshGenerator != null)
			{
				Material previewMaterial = this._themeDatabase.bindings.materialCollection.materialBindings[28];
				this._newCity.CityTilemapMeshGenerator.SetMeshPreviewMaterials(previewMaterial);
			}
		}

		// Token: 0x0600233F RID: 9023 RVA: 0x00090864 File Offset: 0x0008EA64
		private Dictionary<string, string> GetSteamRichPresenceTokens()
		{
			if (!(this._softwareCapabilities is SteamSoftwareCapabilities))
			{
				return null;
			}
			string cityName = this._newMapDefinition.cityName;
			MapChallenge.ChallengeType challengeType = (this._newMapChallenge == null) ? MapChallenge.ChallengeType.None : this._newMapChallenge.type;
			string displayString;
			if (challengeType == MapChallenge.ChallengeType.Daily)
			{
				displayString = "#ModeDailyChallenge";
			}
			else if (challengeType == MapChallenge.ChallengeType.Weekly)
			{
				displayString = "#ModeWeeklyChallenge";
			}
			else
			{
				displayString = "#ModeCity";
			}
			return SteamSoftwareCapabilities.GetRichPresenceTokens(cityName, displayString);
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x000908C8 File Offset: 0x0008EAC8
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			if (!this._playerPausedGame && !this._startPaused && this._recentlyExitedCinematicMode)
			{
				this._gameUIScreen.OnPlayPressed();
			}
			else
			{
				this._game.SetPaused(this._playerPausedGame || this._startPaused);
			}
			this._startPaused = false;
			this._recentlyExitedCinematicMode = false;
			this._gameUIScreen.OnTransitionedIn();
			this._menuPlacementDefinition.background.SetActive(false);
			this._menuPlacementDefinition.grid.enabled = false;
			StartupScreen startupScreen = this._screenStack.GetActiveScreen<StartupScreen>();
			if (startupScreen != null)
			{
				startupScreen.StopSimulatingMenuGame();
			}
			this._overrideTransitionInAnimation = false;
		}

		// Token: 0x06002341 RID: 9025 RVA: 0x0009097C File Offset: 0x0008EB7C
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._playerPausedGame = this._game.Simulation.IsPaused;
			this._overrideTransitionInAnimation = false;
			this._game.Scope.Get<GameUIScreen>().TransitionOut(inScreen);
			this._softwareCapabilities.SetIsInGame(false);
			if (inScreen == ScreenStack.MotorwaysScreen.MapSelect || inScreen == ScreenStack.MotorwaysScreen.ResumeGame || inScreen == ScreenStack.MotorwaysScreen.MainMenu || inScreen == ScreenStack.MotorwaysScreen.None)
			{
				this._isTransitioningOutToBeReleased = true;
				StartupScreen startupScreen = this._screenStack.GetActiveScreen<StartupScreen>();
				if (startupScreen != null)
				{
					startupScreen.StartSimulatingMenuGame();
					startupScreen.PlayMenuGameAudio();
				}
				this._softwareCapabilities.SetRichPresence(null);
			}
			else
			{
				this._isTransitioningOutToBeReleased = false;
			}
			RuntimeAppCommandSource runtimeAppCommandSource = this._runtimeAppCommandSource as RuntimeAppCommandSource;
			if (runtimeAppCommandSource == null)
			{
				return;
			}
			runtimeAppCommandSource.SetRewiredMode(0);
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x00090A31 File Offset: 0x0008EC31
		public override void OnLostFocus()
		{
			base.OnLostFocus();
			this._playerActionController.TutorialBlockInputFlag = true;
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x00090A45 File Offset: 0x0008EC45
		public override void OnGainedFocus()
		{
			base.OnGainedFocus();
			this._playerActionController.TutorialBlockInputFlag = false;
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x00090A59 File Offset: 0x0008EC59
		public override void ApplyTheme(ITheme newTheme)
		{
			base.ApplyTheme(newTheme);
			if (this._gameUIScreen)
			{
				this._gameUIScreen.ApplyTheme(newTheme);
			}
		}

		// Token: 0x06002345 RID: 9029 RVA: 0x00090A7B File Offset: 0x0008EC7B
		public override void ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			base.ApplyBlendedTheme(oldTheme, newTheme, progress);
			if (this._gameUIScreen)
			{
				this._gameUIScreen.ApplyBlendedTheme(oldTheme, newTheme, progress);
			}
		}

		// Token: 0x06002346 RID: 9030 RVA: 0x00090AA4 File Offset: 0x0008ECA4
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (!this.IsTransitioningIn() && !this.IsTransitioningOut())
			{
				if (!this._screenStack.IsFading && !this._game.PlayingBackSimJournal)
				{
					ActiveChallengesModel challengeModel = this._game.Scope.Get<ActiveChallengesModel>();
					if (!this._hasSeenChallenges && challengeModel.HasChallenges)
					{
						this._screenStack.PushScreen<ChallengeInfoScreen>(ScreenStack.MotorwaysScreen.ChallengeInfo, delegate(ChallengeInfoScreen screen)
						{
							MotorwaysGame game = this._game;
							bool flag;
							if (game == null)
							{
								flag = false;
							}
							else
							{
								IScope scope = game.Scope;
								int? num;
								if (scope == null)
								{
									num = null;
								}
								else
								{
									ScoreModel scoreModel = scope.Get<ScoreModel>();
									num = ((scoreModel != null) ? new int?(scoreModel.Score) : null);
								}
								int? num2 = num;
								int num3 = 0;
								flag = (num2.GetValueOrDefault() == num3 & num2 != null);
							}
							bool firstTimeShownScreen = flag;
							screen.PrepareScreen(challengeModel.challengeType, challengeModel.challenges, challengeModel.timeStart, challengeModel.timeEnd, firstTimeShownScreen ? StringId.Begin : StringId.Continue, false, false, this._game.Scope, true);
						}, false, null, true, null);
						this._hasSeenChallenges = true;
					}
					else if (!this._hasSeenModeInfo && !challengeModel.HasChallenges && this._screenStack.IsScreenVisible(ScreenStack.MotorwaysScreen.InGame))
					{
						if (this._gameMode == GameMode.Endless && !this._player.HasSeenNewContent("EndlessInfoPopupContentKey"))
						{
							this.ShowModeInfoPopup();
							this._player.SetNewContentSeen("EndlessInfoPopupContentKey");
						}
						if (this._gameMode == GameMode.Expert && !this._player.HasSeenNewContent("ExpertInfoPopupContentKey"))
						{
							this.ShowModeInfoPopup();
							this._player.SetNewContentSeen("ExpertInfoPopupContentKey");
						}
						if (this._gameMode == GameMode.Creative && !this._player.HasSeenNewContent("CreativeInfoPopupContentKey"))
						{
							this.ShowModeInfoPopup();
							this._player.SetNewContentSeen("CreativeInfoPopupContentKey");
						}
					}
				}
				if (this._game != null && this.ShouldTickGame())
				{
					this._game.Tick(deltaTime);
					return;
				}
			}
			else if (this._game != null)
			{
				this._game.TickDuringTransition(deltaTime);
			}
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x00090C40 File Offset: 0x0008EE40
		private void ShowModeInfoPopup()
		{
			this._startPaused = true;
			this._gameUIScreen.OnPausePressed();
			this.popupStack.PushPopup<ModeInfoPopupInGame>(0f, false).Initialize(this._appScope, this._gameMode, new Action(this._gameUIScreen.OnPlayPressed));
			this._hasSeenModeInfo = true;
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x00090C9C File Offset: 0x0008EE9C
		private void OnApplicationPause(bool isPaused)
		{
			if (isPaused && this._game != null)
			{
				this._game.TrySave(GameJournalMotive.AppDeactivated);
				GameRules rules = this._game.Scope.Get<City>().Rules;
				if (rules != null && rules.RecordsGameStatistics())
				{
					this._game.RecordGameStatistics(null);
				}
			}
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x00090CF8 File Offset: 0x0008EEF8
		private void ReleaseGame()
		{
			if (Diagnostics.Verify(this._game != null, "Trying to release a game when we don't have one!"))
			{
				this._game.StopAudio();
				this._game.ClearPathfinder();
				this._game.Scope.ParentScope.Release(this._game);
				this._game = null;
				this._playerPausedGame = false;
			}
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x00090D5C File Offset: 0x0008EF5C
		public override void OnReleasedFromScope(IScope scope)
		{
			GameContainerScreen.Log.Info("Disposing GameContainerScreen containing city {0}.", new object[]
			{
				(this._newCity != null) ? this._newCity.name : "unknown"
			});
			base.OnReleasedFromScope(scope);
			this.UnregisterThemeComponents();
			this.CleanupPreviousGame();
			StartupScreen startupScreen = this._screenStack.GetActiveScreen<StartupScreen>();
			if (startupScreen != null)
			{
				startupScreen.StartSimulatingMenuGame();
			}
			this._newMapDefinition = null;
			this._newMapChallenge = null;
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x00090DDD File Offset: 0x0008EFDD
		private void CleanupPreviousGame()
		{
			if (this._newCity != null)
			{
				UnityEngine.Object.Destroy(this._newCity.gameObject);
				this._newCity = null;
			}
			if (this._game != null)
			{
				this.ReleaseGame();
			}
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x00090E12 File Offset: 0x0008F012
		public void SetGameSuspended(bool suspendGame)
		{
			this._gameSuspended = suspendGame;
		}

		// Token: 0x0600234D RID: 9037 RVA: 0x00090E1B File Offset: 0x0008F01B
		private bool ShouldTickGame()
		{
			return !this._gameSuspended;
		}

		// Token: 0x0600234E RID: 9038 RVA: 0x00090E26 File Offset: 0x0008F026
		public virtual Game GetActiveGame()
		{
			return this._game;
		}

		// Token: 0x04001D3F RID: 7487
		[Dependency]
		private VisualConstantsData _constants;

		// Token: 0x04001D40 RID: 7488
		[Dependency]
		protected PlayerActionController _playerActionController;

		// Token: 0x04001D41 RID: 7489
		[Dependency]
		private ISoftwareCapabilities _softwareCapabilities;

		// Token: 0x04001D42 RID: 7490
		[Dependency]
		private IAppCommandSource _runtimeAppCommandSource;

		// Token: 0x04001D43 RID: 7491
		private MotorwaysGame _game;

		// Token: 0x04001D44 RID: 7492
		private bool _startGameOnTransition;

		// Token: 0x04001D45 RID: 7493
		private CityDefinition _newCity;

		// Token: 0x04001D46 RID: 7494
		private MapDefinition _newMapDefinition;

		// Token: 0x04001D47 RID: 7495
		private MapChallenge _newMapChallenge;

		// Token: 0x04001D48 RID: 7496
		private GameUIScreen _gameUIScreen;

		// Token: 0x04001D49 RID: 7497
		private CameraView _cameraView;

		// Token: 0x04001D4A RID: 7498
		private GameMode _gameMode;

		// Token: 0x04001D4B RID: 7499
		private bool _playerPausedGame;

		// Token: 0x04001D4C RID: 7500
		private bool _startPaused;

		// Token: 0x04001D4D RID: 7501
		private bool _gameSuspended;

		// Token: 0x04001D4E RID: 7502
		private bool _recentlyExitedCinematicMode;

		// Token: 0x04001D4F RID: 7503
		[MaxValue(0.9999f)]
		[SerializeField]
		[MinValue(0.001f)]
		private float _percentageOfTransitionInToStartBluringForChallenges = 0.6f;

		// Token: 0x04001D50 RID: 7504
		private bool _hasSeenChallenges;

		// Token: 0x04001D51 RID: 7505
		private bool _hasSeenModeInfo;

		// Token: 0x04001D52 RID: 7506
		private bool _isTransitioningOutToBeReleased;

		// Token: 0x04001D53 RID: 7507
		private bool _overrideTransitionInAnimation;

		// Token: 0x04001D54 RID: 7508
		[Dependency]
		private MenuPlacementDefinition _menuPlacementDefinition;

		// Token: 0x04001D55 RID: 7509
		[Dependency]
		private INotificationEventSystem _notificationEventSystem;

		// Token: 0x04001D56 RID: 7510
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("GameContainerScreen");

		// Token: 0x04001D57 RID: 7511
		private static readonly ProfilerMarker Profiler_Tick = new ProfilerMarker(ProfilerCategory.Scripts, "GameContainerScreen.Tick()");
	}
}
