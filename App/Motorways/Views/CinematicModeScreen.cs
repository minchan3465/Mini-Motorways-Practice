using System;
using System.Collections.Generic;
using Client;
using Factory;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x02000532 RID: 1330
	public class CinematicModeScreen : OverlayBaseScreen, IGameStartScreen
	{
		// Token: 0x0600230F RID: 8975 RVA: 0x0008F443 File Offset: 0x0008D643
		protected override MapDefinition GetMapDefinition()
		{
			if (!(this._newMapDefinition != null))
			{
				return this._game.MapDefinition;
			}
			return this._newMapDefinition;
		}

		// Token: 0x06002310 RID: 8976 RVA: 0x0008F468 File Offset: 0x0008D668
		public override void OnTransitionedIn()
		{
			if (this._game == null)
			{
				Diagnostics.FailAssert("Cinematic mode transitioning in without a valid game.", Array.Empty<object>());
				return;
			}
			this._game.SetPaused(false);
			this._game.SetTimeScale(TimeScale.Single);
			if (Get.Pulse.Scale != TimeScale.Single)
			{
				Get.Pulse.Scale = TimeScale.Single;
				AudioPlayer ui = AudioPlayer.UI;
				if (ui != null)
				{
					ui.PlaySample("ui_clockSlow", 0.75f, 0.5f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
				}
			}
			this._game.Scope.Get<GameContainerScreen>().SetRecentlyExitedCinematicMode(true);
			if (this._game.StartedWithGameMode == GameMode.Endless)
			{
				this._simulation.GetModel<ClockModel>().expansionTimeManuallyPaused = true;
			}
			else
			{
				this.SetBaseGameSuspended(true);
				this.StartCinematicGame();
			}
			this._cameraView = this._game.Scope.Get<CameraView>();
			this._cameraView.EnterCinematicMode();
			this._cameraView.GoToNextAgentInCinematicMode();
			base.OnTransitionedIn();
		}

		// Token: 0x06002311 RID: 8977 RVA: 0x0008F576 File Offset: 0x0008D776
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._simulation.GetModel<ClockModel>().expansionTimeManuallyPaused = false;
			this._waitingOnCinematicModeExit = false;
			if (this._cinematicGameRunning)
			{
				this.ReleaseCinematicGame();
				this.SetBaseGameSuspended(false);
			}
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x0008F5AC File Offset: 0x0008D7AC
		public void OnBackPressed()
		{
			if (!this.isToolbarVisible)
			{
				return;
			}
			this._cameraView.ExitCinematicMode();
			this._waitingOnCinematicModeExit = true;
			this.ToggleToolbarVisibility();
			MotorwaysInGameStateToggleController.SwitchToStateIfNeeded(MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles, this._gameScope, MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions);
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x0008F5DC File Offset: 0x0008D7DC
		public override void SetToolbarVisible(bool visible, bool hasAudio = false)
		{
			base.SetToolbarVisible(visible, false);
			if (hasAudio)
			{
				AudioPlayer @default = AudioPlayer.Default;
				if (@default != null)
				{
					@default.PlaySample("iso-ui-" + (visible ? "show" : "hide") + "-controls", 0.5f, 1f, 1f, 0.0, -1.0, false, null, false, false, 0f, true);
				}
			}
			this._zoomOutButtonAnchor.SetActive(visible);
			this._zoomOutButtonInactiveAnchor.SetActive(true);
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x0008F666 File Offset: 0x0008D866
		public override void ToggleToolbarVisibility()
		{
			base.ToggleToolbarVisibility();
			this.RefreshCinematicButtonDisplay();
		}

		// Token: 0x06002315 RID: 8981 RVA: 0x0008F674 File Offset: 0x0008D874
		public void ZoomIn()
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.FocusZoomIn, UIAudioProfile.None, this._cameraView.GetInterpolationSpeed(), true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
			this.SetCinematicZoomLevel(this._cameraView.CinematicZoomIndex + 1);
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x0008F6AE File Offset: 0x0008D8AE
		public void ZoomOut()
		{
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.FocusZoomOut, UIAudioProfile.None, this._cameraView.GetInterpolationSpeed(), true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
			this.SetCinematicZoomLevel(this._cameraView.CinematicZoomIndex - 1);
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x0008F6E8 File Offset: 0x0008D8E8
		public void OnNextVehiclePressed()
		{
			this._cameraView.GoToNextAgentInCinematicMode();
		}

		// Token: 0x06002318 RID: 8984 RVA: 0x0008F6F5 File Offset: 0x0008D8F5
		private void SetCinematicZoomLevel(int newZoomLevel)
		{
			this._cameraView.SetCinematicZoomLevel(newZoomLevel);
			this.RefreshCinematicButtonDisplay();
		}

		// Token: 0x06002319 RID: 8985 RVA: 0x0008F70C File Offset: 0x0008D90C
		private void RefreshCinematicButtonDisplay()
		{
			if (this._cameraView.CinematicZoomIndex == this._cameraView.ZoomLevelCount - 1)
			{
				this._zoomInButton.GetComponent<CinematicZoomButton>().Deactivate();
			}
			else
			{
				this._zoomInButton.GetComponent<CinematicZoomButton>().Activate();
			}
			if (this._cameraView.CinematicZoomIndex == 0)
			{
				this._zoomOutButton.GetComponent<CinematicZoomButton>().Deactivate();
				return;
			}
			this._zoomOutButton.GetComponent<CinematicZoomButton>().Activate();
		}

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x0600231A RID: 8986 RVA: 0x000020AA File Offset: 0x000002AA
		protected override OverlayBaseScreen.OverlayScreenType overlayScreenType
		{
			get
			{
				return OverlayBaseScreen.OverlayScreenType.CinematicModeScreen;
			}
		}

		// Token: 0x0600231B RID: 8987 RVA: 0x0008F784 File Offset: 0x0008D984
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (this._cinematicGameRunning)
			{
				MotorwaysGame game = this._game;
				if (game != null)
				{
					game.Tick(deltaTime);
				}
				if (this._waitingOnCinematicModeExit)
				{
					if (!Diagnostics.Verify(this._gameCamera.customBlur.Strength >= 0f, "Cinematic mode blur strength should never be negative.") || !Diagnostics.Verify((double)this._visualConstantsData.CinematicTransitionOutBlurSpeed >= 0.1, "Cinematic transition out blur speed should never be less than 0.1"))
					{
						this._waitingOnCinematicModeExit = false;
						base.OnBack();
						return;
					}
					this._gameCamera.customBlur.Strength = Math.Clamp(this._gameCamera.customBlur.Strength + deltaTime * this._visualConstantsData.CinematicTransitionOutBlurSpeed, 0f, 1f);
					if (this._gameCamera.customBlur.Strength >= 1f)
					{
						this._waitingOnCinematicModeExit = false;
						base.OnBack();
						return;
					}
				}
			}
			else if (this._waitingOnCinematicModeExit && !this._cameraView.IsInCinematicMode)
			{
				this._waitingOnCinematicModeExit = false;
				this._game.Scope.Get<NotificationView>().HideNotification();
				this._screenStack.PopToScreenOfType(ScreenStack.MotorwaysScreen.InGame, false);
			}
		}

		// Token: 0x0600231C RID: 8988 RVA: 0x0008F8C0 File Offset: 0x0008DAC0
		public override void RegisterThemeComponents(ITheme theme)
		{
			base.RegisterThemeComponents(theme);
			if (this._newCity != null)
			{
				List<IThemeComponent> mapAssets = new List<IThemeComponent>();
				this._newCity.GetComponentsInChildren<IThemeComponent>(mapAssets);
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

		// Token: 0x0600231D RID: 8989 RVA: 0x0008F958 File Offset: 0x0008DB58
		public void PrepareForNewGame(CityDefinition newCity, MapDefinition newMapDefinition, MotorwaysGame game, MapChallenge newMapChallenge = null, bool startPaused = false)
		{
			this._game = game;
			this._newCity = newCity;
			this._newMapDefinition = newMapDefinition;
			this.RegisterThemeComponents(this._themeDatabase.GetTheme());
		}

		// Token: 0x0600231E RID: 8990 RVA: 0x0008F980 File Offset: 0x0008DB80
		private void StartCinematicGame()
		{
			this._game.SetMapDefinition(this._newMapDefinition);
			this._game.Start(this._newCity, GameMode.Cinematic, this._newMapChallenge, true);
			this._game.Scope.Get<GameBehaviourModel>().CanGameOver = false;
			this._game.Scope.Get<CityPlanModel>().SpawningMode = CityPlanModel.BuildingSpawningMode.None;
			this._game.SetPaused(false);
			this._game.Tick(0f);
			this._game.StartAudio();
			this._cinematicGameRunning = true;
		}

		// Token: 0x0600231F RID: 8991 RVA: 0x0008FA14 File Offset: 0x0008DC14
		private void ReleaseCinematicGame()
		{
			if (this._game != null && this._game.StartedWithGameMode != GameMode.Endless)
			{
				this._cinematicGameRunning = false;
				this.UnregisterThemeComponents();
				this._game.StopAudio();
				this._game.ClearPathfinder();
				this._game.Scope.ParentScope.Release(this._game);
				this._game = null;
				UnityEngine.Object.Destroy(this._newCity.gameObject);
			}
		}

		// Token: 0x06002320 RID: 8992 RVA: 0x0008FA90 File Offset: 0x0008DC90
		private void SetBaseGameSuspended(bool suspend)
		{
			GameContainerScreen baseGameScreen = this._appScope.Get<GameContainerScreen>();
			if (baseGameScreen == null)
			{
				return;
			}
			MotorwaysGame baseGame = baseGameScreen.GetActiveGame() as MotorwaysGame;
			if (baseGame == null)
			{
				return;
			}
			baseGameScreen.SetGameSuspended(suspend);
			if (suspend)
			{
				baseGame.StopAudio();
			}
			else
			{
				baseGame.StartAudio();
			}
			baseGame.Scope.Get<ViewClient>().SetAllGameObjectsEnabled(!suspend);
			foreach (DestinationView destinationView in baseGame.Scope.Get<ViewClient>().GetViews<DestinationView>())
			{
				destinationView.SetPinViewVisible(!suspend);
			}
		}

		// Token: 0x06002321 RID: 8993 RVA: 0x0008FB40 File Offset: 0x0008DD40
		public override void Reset()
		{
			base.Reset();
			this._waitingOnCinematicModeExit = false;
			this._cinematicGameRunning = false;
			this._cameraView = null;
			this._newCity = null;
			this._newMapDefinition = null;
			this._newMapChallenge = null;
		}

		// Token: 0x04001D29 RID: 7465
		[Dependency]
		private VisualConstantsData _visualConstantsData;

		// Token: 0x04001D2A RID: 7466
		[SerializeField]
		private GameObject _zoomOutButtonAnchor;

		// Token: 0x04001D2B RID: 7467
		[SerializeField]
		private GameObject _zoomOutButtonInactiveAnchor;

		// Token: 0x04001D2C RID: 7468
		private CameraView _cameraView;

		// Token: 0x04001D2D RID: 7469
		private CityDefinition _newCity;

		// Token: 0x04001D2E RID: 7470
		private MapDefinition _newMapDefinition;

		// Token: 0x04001D2F RID: 7471
		private MapChallenge _newMapChallenge;

		// Token: 0x04001D30 RID: 7472
		private bool _cinematicGameRunning;

		// Token: 0x04001D31 RID: 7473
		private bool _waitingOnCinematicModeExit;

		// Token: 0x04001D32 RID: 7474
		public const string ShowCinematicModeDebugInfo = "ShowCinematicModeDebugInfo";
	}
}
