using System;
using System.IO;
using Factory;
using Motorways.Audio;
using Motorways.Commands;
using Motorways.Processes;
using Screens;
using Server;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Motorways.Views
{
	// Token: 0x0200056F RID: 1391
	public class StartupScreen : BaseScalingScreen
	{
		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06002602 RID: 9730 RVA: 0x000A0F4F File Offset: 0x0009F14F
		public CityDefinition BackgroundCityDefinition
		{
			get
			{
				return this._newCity;
			}
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x000A0F57 File Offset: 0x0009F157
		public void StartSimulatingMenuGame()
		{
			this._shouldSimulateMenuGame = true;
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x000A0F60 File Offset: 0x0009F160
		public void StopSimulatingMenuGame()
		{
			this._shouldSimulateMenuGame = false;
		}

		// Token: 0x06002605 RID: 9733 RVA: 0x000A0F69 File Offset: 0x0009F169
		public void PlayMenuGameAudio()
		{
			this._menuGame.StartAudio();
		}

		// Token: 0x06002606 RID: 9734 RVA: 0x000A0F76 File Offset: 0x0009F176
		public void StopMenuGameAudio()
		{
			this._menuGame.StopAudio();
		}

		// Token: 0x06002607 RID: 9735 RVA: 0x000A0F84 File Offset: 0x0009F184
		public override void TransitionIn(ScreenStack.MotorwaysScreen outScreen)
		{
			this._splashScreen = UnityEngine.Object.FindObjectOfType<SplashScreen>();
			this._startupSequence = this.SelectStartupSequence(out this._gameStarter);
			this._holdTimer = this.holdTime;
			this.VerifyResolution();
			base.TransitionIn(outScreen);
			this._themeDatabase.SetCurrentMapDefinition(this.mapDefinition, 0f);
			UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (universalRenderPipelineAsset != null)
			{
				universalRenderPipelineAsset.msaaSampleCount = this._player.AntiAliasingMSAALevelForUniversalRenderPipeline;
			}
		}

		// Token: 0x06002608 RID: 9736 RVA: 0x000A0FFC File Offset: 0x0009F1FC
		public override void TransitionOut(ScreenStack.MotorwaysScreen inScreen)
		{
			base.TransitionOut(inScreen);
			this._scaleToCamera = false;
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06002609 RID: 9737 RVA: 0x000A100C File Offset: 0x0009F20C
		private StartupScreen.StartupScreenStage CurrentStage
		{
			get
			{
				return this._startupSequence[this._stageIndex];
			}
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x000A101B File Offset: 0x0009F21B
		private void AdvanceStartupSequenceStage()
		{
			this._stageIndex++;
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x000A102C File Offset: 0x0009F22C
		private StartupScreen.StartupScreenStage[] SelectStartupSequence(out GameStarter gameStarter)
		{
			gameStarter = null;
			if (this._deepLinkProcessor.hasChallengeToUse)
			{
				return StartupScreen.MainMenuSequence;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.SoakTest))
			{
				return StartupScreen.MainMenuSequence;
			}
			if (Application.isEditor)
			{
				gameStarter = this.LoadSimulationJournalGameStarter();
				if (gameStarter != null)
				{
					return StartupScreen.DeactivatedSaveGameSequence;
				}
			}
			gameStarter = this.LoadDeactivatedSaveGameStarter();
			if (gameStarter != null)
			{
				return StartupScreen.DeactivatedSaveGameSequence;
			}
			if (this.ShouldLoadTutorial())
			{
				gameStarter = this.LoadTutorialGameStarter();
				return StartupScreen.TutorialSequence;
			}
			return StartupScreen.MainMenuSequence;
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x000A10A4 File Offset: 0x0009F2A4
		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			if (!Diagnostics.Verify(this._startupSequence != null, "No startup sequence specified! Transitioning to main menu as backup."))
			{
				this._startupSequence = StartupScreen.MainMenuSequence;
				return;
			}
			if (this.CurrentStage != StartupScreen.StartupScreenStage.SimulateMenuCity && (Input.anyKey || Input.touchCount > 0))
			{
				this._shouldSkipHold = true;
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.StartFade)
			{
				if (this._splashScreen != null)
				{
					this._splashScreen.StartFade();
				}
				this.AdvanceStartupSequenceStage();
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.WaitForFadeToComplete)
			{
				if (this._splashScreen != null && !this._splashScreen.IsFadeComplete())
				{
					return;
				}
				this.AdvanceStartupSequenceStage();
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.LoadAtlas)
			{
				this.StartLoadingAtlas();
				this.AdvanceStartupSequenceStage();
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.WaitForAtlasToLoad)
			{
				if (!this._atlasAsyncLoadResult.HasValue)
				{
					return;
				}
				RoadTileAtlas roadTileAtlas = null;
				TextAsset atlasAsset = this._atlasAsyncLoadResult.asset as TextAsset;
				if (atlasAsset)
				{
					using (BinaryReader reader = new BinaryReader(new MemoryStream(atlasAsset.bytes)))
					{
						roadTileAtlas = this._appScope.Import<RoadTileAtlas>(reader);
					}
				}
				if (roadTileAtlas == null)
				{
					Diagnostics.FailAssert("RoadTileAtlas failed to load from AssetBundle. Try rebuilding the RoadTileAtlas asset bundle (Assets -> Asset Bundles -> Build RoadTileAtlas", Array.Empty<object>());
					roadTileAtlas = this._appScope.Get<RoadTileAtlas>();
					roadTileAtlas.Initialize();
				}
				this._appScope.Get<RailTileAtlas>().Initialize();
				this._appScope.Get<BoatPathTileAtlas>().Initialize();
				this.AdvanceStartupSequenceStage();
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.LoadMenuCity)
			{
				CityDefinition cityDefinition = this.BackgroundCityDefinition;
				cityDefinition.CompileTilemap();
				this._menuGame = this._appScope.Get<MotorwaysGame>();
				this._menuGame.SetMapDefinition(this.mapDefinition);
				this._menuGame.Start(cityDefinition, GameMode.Background, null, false);
				this.BuildMenuCityRoads();
				this.AdvanceStartupSequenceStage();
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.StartGame && this._gameStarter.CanStart)
			{
				if (!this._gameStarter.Start(this._screenStack, this._appScope))
				{
					this.TransitionToMainMenu();
				}
				else
				{
					Get.State |= StateType.SkippingMenu;
				}
				this.AdvanceStartupSequenceStage();
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.WaitForGameToStart)
			{
				return;
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.Hold)
			{
				this._holdTimer -= deltaTime;
				if (this._holdTimer <= 0f || this._shouldSkipHold)
				{
					this._holdTimer = -1f;
					if (this._deepLinkProcessor.hasChallengeToUse)
					{
						this._stageIndex = Array.IndexOf<StartupScreen.StartupScreenStage>(this._startupSequence, StartupScreen.StartupScreenStage.SimulateMenuCity);
						this._deepLinkProcessor.hasChallengeToUse = false;
						this.TransitionToMapSelect();
						return;
					}
					this.AdvanceStartupSequenceStage();
				}
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.TransitionToMainMenu)
			{
				this.TransitionToMainMenu();
				this.AdvanceStartupSequenceStage();
			}
			if (this.CurrentStage == StartupScreen.StartupScreenStage.SimulateMenuCity && this._shouldSimulateMenuGame)
			{
				this._menuGame.Tick(deltaTime);
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x0600260D RID: 9741 RVA: 0x000A1368 File Offset: 0x0009F568
		// (set) Token: 0x0600260E RID: 9742 RVA: 0x000A1380 File Offset: 0x0009F580
		public static bool CanAutoResumeDeactivatedGame
		{
			get
			{
				return PlayerPrefs.GetInt(StartupScreen.CanAutoResumeDeactivatedGamePlayerPrefsKey, StartupScreen.CanResume) == StartupScreen.CanResume;
			}
			set
			{
				PlayerPrefs.SetInt(StartupScreen.CanAutoResumeDeactivatedGamePlayerPrefsKey, value ? StartupScreen.CanResume : StartupScreen.CannotResume);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x0600260F RID: 9743 RVA: 0x000A13A0 File Offset: 0x0009F5A0
		private void TransitionToMainMenu()
		{
			this.StartSimulatingMenuGame();
			this.PlayMenuGameAudio();
			this._screenStack.PushScreen<MainMenuScreen>(ScreenStack.MotorwaysScreen.MainMenu, false, null, true);
		}

		// Token: 0x06002610 RID: 9744 RVA: 0x000A13C0 File Offset: 0x0009F5C0
		private void TransitionToMapSelect()
		{
			this.StartSimulatingMenuGame();
			this.PlayMenuGameAudio();
			this._screenStack.PushScreen<MainMenuScreen>(ScreenStack.MotorwaysScreen.MainMenu, false, null, true);
			this._screenStack.PushScreen<MapSelectScreen>(ScreenStack.MotorwaysScreen.MapSelect, delegate(MapSelectScreen screen)
			{
				screen.PrepareScreen(null, true, true);
			}, false, null, true, null);
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x000A1419 File Offset: 0x0009F619
		private void StartLoadingAtlas()
		{
			this._atlasAsyncLoadResult = AssetBundleUtility.LoadAssetAsync("roadtileatlas", "roadtileatlas", this);
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x000A1434 File Offset: 0x0009F634
		private GameStarter LoadDeactivatedSaveGameStarter()
		{
			if (!StartupScreen.CanAutoResumeDeactivatedGame)
			{
				return null;
			}
			MotorwaysGameJournalSave deactivatedSaveGame = this.LoadDeactivatedSavedGameJournal();
			GameStarter gameStarter = new GameStarter(this);
			if (deactivatedSaveGame == null)
			{
				return null;
			}
			if (!gameStarter.StartFromSavedGame(this.mapLibrary, deactivatedSaveGame, false, true, false))
			{
				return null;
			}
			return gameStarter;
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x000A1474 File Offset: 0x0009F674
		private GameStarter LoadTutorialGameStarter()
		{
			GameStarter gameStarter = new GameStarter(this);
			this._analytics.TrackTutorialStarted(false);
			if (!gameStarter.StartFromMapDefinition(this.tutorialDefinition, GameMode.Tutorial, 2f, false, false))
			{
				return null;
			}
			return gameStarter;
		}

		// Token: 0x06002614 RID: 9748 RVA: 0x000A14B0 File Offset: 0x0009F6B0
		private GameStarter LoadSimulationJournalGameStarter()
		{
			AppRuntime appRuntime = UnityEngine.Object.FindObjectOfType<AppRuntime>();
			string simJournalPath = (appRuntime != null) ? appRuntime._playbackSimJournalPath : null;
			if (string.IsNullOrEmpty(simJournalPath) || !File.Exists(simJournalPath))
			{
				return null;
			}
			Game game = this._appScope.Get<Game>();
			CommandJournal commands = null;
			using (BinaryReader journalReader = new BinaryReader(File.Open(simJournalPath, FileMode.Open, FileAccess.Read)))
			{
				commands = game.Scope.Import<CommandJournal>(journalReader);
			}
			GameStarter gameStarter = null;
			if (commands != null)
			{
				for (int commandIndex = 0; commandIndex < commands.EntryCount; commandIndex++)
				{
					InitCityCommand initCityCommand = commands.GetEntry(commandIndex) as InitCityCommand;
					if (initCityCommand != null)
					{
						MapDefinition mapDefinition = this.mapLibrary.GetMapByName(initCityCommand.CityName);
						gameStarter = new GameStarter(this);
						gameStarter.StartFromMapDefinition(mapDefinition, GameMode.Normal, 0f, false, false);
						break;
					}
				}
			}
			this._appScope.Release(game);
			return gameStarter;
		}

		// Token: 0x06002615 RID: 9749 RVA: 0x000A1594 File Offset: 0x0009F794
		private MotorwaysGameJournalSave LoadDeactivatedSavedGameJournal()
		{
			if (!this._player.HasLocalSavedGame)
			{
				return null;
			}
			MotorwaysGameJournalSave localSave = (MotorwaysGameJournalSave)this._player.LocalSavedGame;
			if (localSave == null)
			{
				return null;
			}
			if (localSave.Motive != GameJournalMotive.AppDeactivated)
			{
				return null;
			}
			return localSave;
		}

		// Token: 0x06002616 RID: 9750 RVA: 0x000A15D4 File Offset: 0x0009F7D4
		private void VerifyResolution()
		{
			if (!this._hardwareCapabilities.SupportsChangingResolution)
			{
				return;
			}
			bool changeToTargetResolution = false;
			Vector2Int targetResolution = Vector2Int.zero;
			if (Screen.width <= StartupScreen.MinimumResolution.x || Screen.height <= StartupScreen.MinimumResolution.y)
			{
				targetResolution = StartupScreen.DefaultResolution;
				changeToTargetResolution = true;
				StartupScreen.Log.Info("Resolution of {0}x{1} is below the minimum. The window will be resized to close to {2}x{3}.", new object[]
				{
					Screen.width,
					Screen.height,
					StartupScreen.DefaultResolution.x,
					StartupScreen.DefaultResolution.y
				});
			}
			if (!PlayerPrefs.HasKey("HasEnforcedMaxResolution"))
			{
				PlayerPrefs.SetInt("HasEnforcedMaxResolution", 1);
				Vector2Int defaultMaxResolution = this._hardwareCapabilities.DefaultMaximumResolution;
				if (defaultMaxResolution.x > 0 && defaultMaxResolution.y > 0 && Screen.width > defaultMaxResolution.x && Screen.height > defaultMaxResolution.y)
				{
					targetResolution = defaultMaxResolution;
					changeToTargetResolution = true;
					StartupScreen.Log.Info("Resolution of {0}x{1} is above the default maximum resolution. The window will be resized to close to {2}x{3}.", new object[]
					{
						Screen.width,
						Screen.height,
						defaultMaxResolution.x,
						defaultMaxResolution.y
					});
				}
			}
			if (changeToTargetResolution)
			{
				float bestResolutionSuitability = -1f;
				Vector2Int bestResolution = Vector2Int.zero;
				foreach (Resolution availableResolution in Screen.resolutions)
				{
					float resolutionSuitability = (new Vector2Int(availableResolution.width, availableResolution.height) - targetResolution).magnitude;
					if (bestResolutionSuitability < 0f || bestResolutionSuitability > resolutionSuitability)
					{
						bestResolutionSuitability = resolutionSuitability;
						bestResolution = new Vector2Int(availableResolution.width, availableResolution.height);
					}
				}
				if (bestResolutionSuitability >= 0f)
				{
					StartupScreen.Log.Info("Changing resolution to {0}x{1}.", new object[]
					{
						bestResolution.x,
						bestResolution.y
					});
					Screen.SetResolution(bestResolution.x, bestResolution.y, Screen.fullScreen);
				}
			}
		}

		// Token: 0x06002617 RID: 9751 RVA: 0x000A1800 File Offset: 0x0009FA00
		private bool ShouldLoadTutorial()
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.AlwaysEnterTutorial))
			{
				return true;
			}
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				return false;
			}
			bool isAnyTutorialCompleted = this._player.IsAnyTutorialCompleted;
			bool isThisTutorialCompleted = this._player.IsTutorialTypeCompleted(TutorialProgressionProcess.TutorialTypeForInputType(this._inputState.CurrentDeviceInputType));
			StartupScreen.Log.Info("Has the player completed the tutorial for {0}? {1}\nHas the player completed any tutorial? {2}", new object[]
			{
				this._inputState.CurrentDeviceInputType,
				isThisTutorialCompleted,
				isAnyTutorialCompleted
			});
			return !isAnyTutorialCompleted;
		}

		// Token: 0x06002618 RID: 9752 RVA: 0x000A188C File Offset: 0x0009FA8C
		private void BuildMenuCityRoads()
		{
			this.ScheduleLineOfRoads(new Vector2Int(-125, 50), 10, TileDirection.North);
			this.ScheduleLineOfRoads(new Vector2Int(-125, 50), 18, TileDirection.SouthWest);
			this.ScheduleLineOfRoads(new Vector2Int(-127, 48), 1, TileDirection.SouthEast);
			this.ScheduleLineOfRoads(new Vector2Int(-132, 43), 42, TileDirection.East);
			this.ScheduleLineOfRoads(new Vector2Int(-95, 43), 8, TileDirection.SouthEast);
			this.ScheduleLineOfRoads(new Vector2Int(-87, 35), 16, TileDirection.East);
			this.ScheduleLineOfRoads(new Vector2Int(-125, 60), 7, TileDirection.NorthEast);
			this.ScheduleLineOfRoads(new Vector2Int(-118, 67), 25, TileDirection.East);
			this.ScheduleLineOfRoads(new Vector2Int(-93, 67), 25, TileDirection.North);
			this.ScheduleLineOfRoads(new Vector2Int(-110, 42), 2, TileDirection.North);
			this._menuGame.Tick(0f);
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x000A195C File Offset: 0x0009FB5C
		private void ScheduleLineOfRoads(Vector2Int start, int length, TileDirection direction)
		{
			EditTileCommand command = EditTileCommand.Create(this._menuGame.Scope, AddRoadLineEdit.Create(this._menuGame.Scope, start, direction, length));
			command.FrameIndex = 0;
			this._menuGame.Scope.Get<Simulation>().ScheduleCommand(command);
		}

		// Token: 0x04001FEE RID: 8174
		public const string DebugAutoStartTypeEditorPrefsKey = "DebugAutoStartType";

		// Token: 0x04001FEF RID: 8175
		public const string DebugAutoStartCityEditorPrefsKey = "DebugAutoStartCity";

		// Token: 0x04001FF0 RID: 8176
		public const string DebugAutoStartGameModeEditorPrefsKey = "DebugAutoStartGameMode";

		// Token: 0x04001FF1 RID: 8177
		public const string DebugAutoStartBookmarkedSavedGameEditorPrefsKey = "DebugAutoStartBookedmarkedSavedGame";

		// Token: 0x04001FF2 RID: 8178
		public const string DebugAutoStartPausedEditorPrefsKey = "DebugAutoStartPaused";

		// Token: 0x04001FF3 RID: 8179
		public const string HasEnforcedMaxResolutionPlayerPrefsKey = "HasEnforcedMaxResolution";

		// Token: 0x04001FF4 RID: 8180
		private static readonly Vector2Int MinimumResolution = new Vector2Int(400, 300);

		// Token: 0x04001FF5 RID: 8181
		private static readonly Vector2Int DefaultResolution = new Vector2Int(1920, 1080);

		// Token: 0x04001FF6 RID: 8182
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("StartupScreen");

		// Token: 0x04001FF7 RID: 8183
		[Dependency]
		private IHardwareCapabilities _hardwareCapabilities;

		// Token: 0x04001FF8 RID: 8184
		[Dependency]
		private DeepLinkProcessor _deepLinkProcessor;

		// Token: 0x04001FF9 RID: 8185
		public MapLibrary mapLibrary;

		// Token: 0x04001FFA RID: 8186
		public MapDefinition tutorialDefinition;

		// Token: 0x04001FFB RID: 8187
		[SerializeField]
		private CityDefinition _newCity;

		// Token: 0x04001FFC RID: 8188
		public MapDefinition mapDefinition;

		// Token: 0x04001FFD RID: 8189
		private MotorwaysGame _menuGame;

		// Token: 0x04001FFE RID: 8190
		private bool _shouldSimulateMenuGame;

		// Token: 0x04001FFF RID: 8191
		private SplashScreen _splashScreen;

		// Token: 0x04002000 RID: 8192
		private GameStarter _gameStarter;

		// Token: 0x04002001 RID: 8193
		private AssetBundleUtility.AsyncLoadResult _atlasAsyncLoadResult;

		// Token: 0x04002002 RID: 8194
		[Tooltip("Duration to stay on the splash screen for")]
		public float holdTime = 1.5f;

		// Token: 0x04002003 RID: 8195
		private float _holdTimer = -1f;

		// Token: 0x04002004 RID: 8196
		private bool _shouldSkipHold;

		// Token: 0x04002005 RID: 8197
		private StartupScreen.StartupScreenStage[] _startupSequence;

		// Token: 0x04002006 RID: 8198
		private int _stageIndex;

		// Token: 0x04002007 RID: 8199
		private static readonly StartupScreen.StartupScreenStage[] DeactivatedSaveGameSequence = new StartupScreen.StartupScreenStage[]
		{
			StartupScreen.StartupScreenStage.LoadAtlas,
			StartupScreen.StartupScreenStage.WaitForAtlasToLoad,
			StartupScreen.StartupScreenStage.LoadMenuCity,
			StartupScreen.StartupScreenStage.StartGame,
			StartupScreen.StartupScreenStage.StartFade,
			StartupScreen.StartupScreenStage.WaitForFadeToComplete,
			StartupScreen.StartupScreenStage.SimulateMenuCity
		};

		// Token: 0x04002008 RID: 8200
		private static readonly StartupScreen.StartupScreenStage[] TutorialSequence = new StartupScreen.StartupScreenStage[]
		{
			StartupScreen.StartupScreenStage.StartFade,
			StartupScreen.StartupScreenStage.WaitForFadeToComplete,
			StartupScreen.StartupScreenStage.LoadAtlas,
			StartupScreen.StartupScreenStage.WaitForAtlasToLoad,
			StartupScreen.StartupScreenStage.LoadMenuCity,
			StartupScreen.StartupScreenStage.Hold,
			StartupScreen.StartupScreenStage.StartGame,
			StartupScreen.StartupScreenStage.SimulateMenuCity
		};

		// Token: 0x04002009 RID: 8201
		private static readonly StartupScreen.StartupScreenStage[] MainMenuSequence = new StartupScreen.StartupScreenStage[]
		{
			StartupScreen.StartupScreenStage.StartFade,
			StartupScreen.StartupScreenStage.WaitForFadeToComplete,
			StartupScreen.StartupScreenStage.LoadAtlas,
			StartupScreen.StartupScreenStage.WaitForAtlasToLoad,
			StartupScreen.StartupScreenStage.LoadMenuCity,
			StartupScreen.StartupScreenStage.Hold,
			StartupScreen.StartupScreenStage.TransitionToMainMenu,
			StartupScreen.StartupScreenStage.SimulateMenuCity
		};

		// Token: 0x0400200A RID: 8202
		private static string CanAutoResumeDeactivatedGamePlayerPrefsKey = "CanAutoResumeDeactivatedGame";

		// Token: 0x0400200B RID: 8203
		private static int CanResume = 1;

		// Token: 0x0400200C RID: 8204
		private static int CannotResume = 0;

		// Token: 0x02000570 RID: 1392
		public enum DebugAutoStartType
		{
			// Token: 0x0400200E RID: 8206
			None,
			// Token: 0x0400200F RID: 8207
			NewGame,
			// Token: 0x04002010 RID: 8208
			SavedGame,
			// Token: 0x04002011 RID: 8209
			BookmarkedSavedGame
		}

		// Token: 0x02000571 RID: 1393
		private enum StartupScreenStage
		{
			// Token: 0x04002013 RID: 8211
			StartFade,
			// Token: 0x04002014 RID: 8212
			WaitForFadeToComplete,
			// Token: 0x04002015 RID: 8213
			LoadAtlas,
			// Token: 0x04002016 RID: 8214
			WaitForAtlasToLoad,
			// Token: 0x04002017 RID: 8215
			LoadMenuCity,
			// Token: 0x04002018 RID: 8216
			StartGame,
			// Token: 0x04002019 RID: 8217
			WaitForGameToStart,
			// Token: 0x0400201A RID: 8218
			Hold,
			// Token: 0x0400201B RID: 8219
			TransitionToMainMenu,
			// Token: 0x0400201C RID: 8220
			SimulateMenuCity
		}
	}
}
