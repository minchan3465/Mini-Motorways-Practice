using System;
using Client;
using Factory;
using Factory.Allocators;
using Factory.Pools;
using Motorways.Actions;
using Motorways.Commands;
using Motorways.Leaderboards;
using Motorways.Models;
using Motorways.Processes;
using Motorways.UI;
using Motorways.UI.NewContentIndicators;
using Motorways.Utility;
using Motorways.Views;
using Motorways.Views.Boats;
using Motorways.Views.MeshGeneration;
using Motorways.Views.Trains;
using Popups;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003B2 RID: 946
	public class MotorwaysAppContainer : AppContainer
	{
		// Token: 0x06001683 RID: 5763 RVA: 0x0004DD68 File Offset: 0x0004BF68
		protected override void RegisterSerializers()
		{
			SerializerLibrary.RegisterSerializer<TileDirectionBitfield>(new TileDirectionBitfield.Serializer());
			SerializerLibrary.RegisterSerializer<CornerAdjacencyReference>(new CornerAdjacencyReference.Serializer());
			SerializerLibrary.RegisterSerializer<UpgradePackageDefinition>(new UpgradePackageDefinition.Serializer());
			SerializerLibrary.RegisterSerializer<RoadTileConnection>(new RoadTileConnection.Serializer());
			SerializerLibrary.RegisterSerializer<RoadTileNode>(new RoadTileNode.Serializer());
			SerializerLibrary.RegisterSerializer<RailTileConnection>(new RailTileConnection.Serializer());
			SerializerLibrary.RegisterSerializer<BoatPathTileConnection>(new BoatPathTileConnection.Serializer());
			SerializerLibrary.RegisterSerializer<PlannedBuilding>(new PlannedBuilding.Serializer());
			SerializerLibrary.RegisterSerializer<Spline.BezierSpline>(new Spline.BezierSpline.Serializer());
			SerializerLibrary.RegisterSerializer<Spline.BezierSplineFixed>(new Spline.BezierSplineFixed.Serializer());
			SerializerLibrary.RegisterSerializer<ChallengeData>(new ChallengeData.Serializer());
			SerializerLibrary.RegisterSerializer<AdjacentTileConnection>(new AdjacentTileConnection.Serializer());
		}

		// Token: 0x06001684 RID: 5764 RVA: 0x0004DDF0 File Offset: 0x0004BFF0
		protected override Assembler CreateAppAssembler()
		{
			Assembler appAssembler = base.CreateAppAssembler();
			appAssembler.Register<PseudorandomGenerator>().Allocator(new ObjectPool<PseudorandomGenerator>
			{
				InitialSize = 4
			});
			MotorwaysThemeDatabaseBindings themeBindings = AssetBundleUtility.LoadAsset<MotorwaysThemeDatabaseBindings>("core", "ThemeDatabaseBindings");
			appAssembler.Register<IThemeDatabase, MotorwaysThemeDatabase>().Allocator(new SingletonAllocator<MotorwaysThemeDatabase>(new MotorwaysThemeDatabase(themeBindings))).Binding(Binding.Scope);
			SimulationConstantsData simulationConstantsData = AssetBundleUtility.LoadAsset<SimulationConstantsData>("core", "SimulationConstantsData");
			appAssembler.Register<SimulationConstantsData, SimulationConstantsData>().Allocator(new SingletonAllocator<SimulationConstantsData>(simulationConstantsData)).Binding(Binding.Scope);
			VisualConstantsData visualConstantsData = AssetBundleUtility.LoadAsset<VisualConstantsData>("core", "VisualConstantsData");
			appAssembler.Register<VisualConstantsData, VisualConstantsData>().Allocator(new SingletonAllocator<VisualConstantsData>(visualConstantsData)).Binding(Binding.Scope);
			PermanenceTextureMappingDatabase permanenceTextureMappingDatabase = AssetBundleUtility.LoadAsset<PermanenceTextureMappingDatabase>("core", "PermanenceTextureMappingDatabase");
			appAssembler.Register<PermanenceTextureMappingDatabase, PermanenceTextureMappingDatabase>().Allocator(new SingletonAllocator<PermanenceTextureMappingDatabase>(permanenceTextureMappingDatabase)).Binding(Binding.Scope);
			MotorwayVisualParameters motorwayVisualParameters = AssetBundleUtility.LoadAsset<MotorwayVisualParameters>("core", "MotorwayVisualParameters");
			appAssembler.Register<MotorwayVisualParameters, MotorwayVisualParameters>().Allocator(new SingletonAllocator<MotorwayVisualParameters>(motorwayVisualParameters)).Binding(Binding.Scope);
			RoadTileConstantsData roadTileConstantsData = AssetBundleUtility.LoadAsset<RoadTileConstantsData>("core", "RoadTileConstantsData");
			appAssembler.Register<RoadTileConstantsData>().Allocator(new SingletonAllocator<RoadTileConstantsData>(roadTileConstantsData)).Binding(Binding.Scope);
			TutorialConstantsData tutorialConstantsData = AssetBundleUtility.LoadAsset<TutorialConstantsData>("core", "TutorialConstantsData");
			appAssembler.Register<TutorialConstantsData, TutorialConstantsData>().Allocator(new SingletonAllocator<TutorialConstantsData>(tutorialConstantsData)).Binding(Binding.Scope);
			appAssembler.Register<ChallengeSystem, ChallengeSystem>().Allocator(new HeapAllocator<ChallengeSystem>()).Binding(Binding.Scope);
			appAssembler.Register<ChallengeOverrides>().Allocator(new HeapAllocator<ChallengeOverrides>()).Binding(Binding.Scope);
			ChallengeDatabase challengeDatabase = AssetBundleUtility.LoadAsset<ChallengeDatabase>("core", "ChallengeDatabase");
			appAssembler.Register<ChallengeDatabase, ChallengeDatabase>().Allocator(new SingletonAllocator<ChallengeDatabase>(challengeDatabase)).Binding(Binding.Scope);
			PlayTogetherChallengeDatabase playTogetherChallengeDatabase = AssetBundleUtility.LoadAsset<PlayTogetherChallengeDatabase>("core", "PlayTogetherChallengeDatabase");
			appAssembler.Register<PlayTogetherChallengeDatabase, PlayTogetherChallengeDatabase>().Allocator(new SingletonAllocator<PlayTogetherChallengeDatabase>(playTogetherChallengeDatabase)).Binding(Binding.Scope);
			MapDatabase mapDatabase = AssetBundleUtility.LoadAsset<MapDatabase>("core", "MapDatabase");
			appAssembler.Register<MapDatabase, MapDatabase>().Allocator(new SingletonAllocator<MapDatabase>(mapDatabase)).Binding(Binding.Scope);
			NewContentData newContentData = AssetBundleUtility.LoadAsset<NewContentData>("core", "NewContentData");
			appAssembler.Register<NewContentData>().Allocator(new SingletonAllocator<NewContentData>(newContentData)).Binding(Binding.Scope);
			NewsAndNotificationData newsAndNotificationData = AssetBundleUtility.LoadAsset<NewsAndNotificationData>("core", "NewsAndNotificationsData");
			appAssembler.Register<NewsAndNotificationData, NewsAndNotificationData>().Allocator(new SingletonAllocator<NewsAndNotificationData>(newsAndNotificationData)).Binding(Binding.Scope);
			CombinedMeshMaterials combinedMeshMaterials = AssetBundleUtility.LoadAsset<CombinedMeshMaterials>("core", "CombinedMeshMaterials");
			appAssembler.Register<CombinedMeshMaterials, CombinedMeshMaterials>().Allocator(new SingletonAllocator<CombinedMeshMaterials>(combinedMeshMaterials)).Binding(Binding.Scope);
			appAssembler.Register<InputEvent, InputEvent>().Allocator(new ObjectPool<InputEvent>
			{
				InitialSize = 50,
				BlockSize = 50
			});
			appAssembler.Register<MotorwaysUIInputEvent, MotorwaysUIInputEvent>().Allocator(new ObjectPool<MotorwaysUIInputEvent>
			{
				InitialSize = 50,
				BlockSize = 50
			});
			appAssembler.Register<AxisInputEvent, AxisInputEvent>().Allocator(new ObjectPool<AxisInputEvent>
			{
				InitialSize = 50,
				BlockSize = 50
			});
			appAssembler.Register<FontDatabase>().Allocator(new GameObjectAllocator<FontDatabase>("core", "FontDatabase")).Binding(Binding.Scope);
			appAssembler.Register<IInitialGameScreen, LoadingScreen>().Allocator(new GameObjectPool<LoadingScreen>("core", "LoadingScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<StartupScreen>().Allocator(new GameObjectPool<StartupScreen>("core", "StartupScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<DeepLinkProcessor>().Allocator(new SingletonAllocator<DeepLinkProcessor>(new DeepLinkProcessor())).Binding(Binding.Scope);
			GameObject mainMenuPrefab = AssetBundleUtility.LoadPrefab("core", "MainMenuScreen");
			appAssembler.Register<MainMenuScreen>().Allocator(new GameObjectPool<MainMenuScreen>(mainMenuPrefab)
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<OptionsScreenMain>().Allocator(new GameObjectPool<OptionsScreenMain>("core", "OptionsScreenMain")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<OptionsScreenPause>().Allocator(new GameObjectPool<OptionsScreenPause>("core", "OptionsScreenPause")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<MapSelectScreen>().Allocator(new GameObjectPool<MapSelectScreen>("core", "MapSelectScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<ResumeGameScreen>().Allocator(new GameObjectPool<ResumeGameScreen>("core", "ResumeGameScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<GameContainerScreen>().Allocator(new GameObjectPool<GameContainerScreen>("core", "GameContainerScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			GameObject gameOverScreenPrefab = AssetBundleUtility.LoadPrefab("core", string.Format("GameOverScreen-{0}", AppContainer.Environment.DeviceCategory));
			appAssembler.Register<GameOverScreen>().Allocator(new GameObjectPool<GameOverScreen>(gameOverScreenPrefab)
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<GameUpgradeScreen>().Allocator(new GameObjectPool<GameUpgradeScreen>("core", string.Format("GameUpgradeScreen-{0}", AppContainer.Environment.DeviceCategory))
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			GameObject pauseScreenPrefab = AssetBundleUtility.LoadPrefab("core", "PauseScreen");
			appAssembler.Register<PauseScreen>().Allocator(new GameObjectPool<PauseScreen>(pauseScreenPrefab)
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<PhotoScreen>().Allocator(new GameObjectPool<PhotoScreen>("core", "PhotoScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<CinematicModeScreen>().Allocator(new GameObjectPool<CinematicModeScreen>("core", "CinematicModeScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<ChallengeInfoScreen>().Allocator(new GameObjectPool<ChallengeInfoScreen>("core", "ChallengeInfoScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<ProfileSelectScreen>().Allocator(new GameObjectPool<ProfileSelectScreen>("core", "ProfileSelectScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<ProfileSelectButton>().Allocator(new GameObjectAllocator<ProfileSelectButton>("core", "ProfileSelectButton"));
			appAssembler.Register<ProfileCreationScreen>().Allocator(new GameObjectPool<ProfileCreationScreen>("core", "ProfileCreationScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<MovieScreen>().Allocator(new GameObjectPool<MovieScreen>("core", "MovieScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			appAssembler.Register<ExamplePopup>().Allocator(new GameObjectPool<ExamplePopup>("core", "ExamplePopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<ChallengeInfoPopup>().Allocator(new GameObjectPool<ChallengeInfoPopup>("core", "ChallengeInfoPopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<ModeInfoPopup>().Allocator(new GameObjectPool<ModeInfoPopup>("core", "ModeInfoPopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<ModeInfoPopupInGame>().Allocator(new GameObjectPool<ModeInfoPopupInGame>("core", "ModeInfoPopupInGame")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<ConfirmationPopup>().Allocator(new GameObjectPool<ConfirmationPopup>("core", "ConfirmationPopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<CrossSavePopup>().Allocator(new GameObjectPool<CrossSavePopup>("core", "CrossSavePopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<ColorblindCustomisePopup>().Allocator(new GameObjectPool<ColorblindCustomisePopup>("core", "ColorblindCustomisePopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<GenericPopup>().Allocator(new GameObjectPool<GenericPopup>("core", "GenericPopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<LoadScreenInterruptionPopup>().Allocator(new GameObjectPool<LoadScreenInterruptionPopup>("core", "LoadScreenInterruptionPopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<ExpertUnlockInfoPopup>().Allocator(new GameObjectPool<ExpertUnlockInfoPopup>("core", "ExpertUnlockInfoPopup")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			if (FeatureToggle.IsFeatureEnabled(Feature.AppleStoreDemo))
			{
				appAssembler.Register<AppleDemoCardPopup>().Allocator(new GameObjectPool<AppleDemoCardPopup>("demo", "AppleDemoCardPopup")
				{
					InitialSize = 1,
					GrowthStrategy = GrowthStrategy.OnDemand
				});
			}
			appAssembler.Register<DebugOverlayScreen>().Allocator(new GameObjectPool<DebugOverlayScreen>("core", "DebugOverlayScreen")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<InGameMessage>().Allocator(new GameObjectPool<InGameMessage>("core", "InGameMessage")
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			appAssembler.Register<MenuPlacementDefinition>().Allocator(new GameObjectAllocator<MenuPlacementDefinition>("core", "MenuDefinition")).Binding(Binding.Scope);
			appAssembler.Register<AnalyticsTracker>().Allocator(new GameObjectAllocator<AnalyticsTracker>("core", "AnalyticsTracker")).Binding(Binding.Scope);
			appAssembler.Register<RoadTileAtlas>().Allocator(new HeapAllocator<RoadTileAtlas>()).Binding(Binding.Scope);
			appAssembler.Register<RoadTileSignature>().Allocator(new ObjectPool<RoadTileSignature>());
			appAssembler.Register<RoadTileDefinition>().Allocator(new ObjectPool<RoadTileDefinition>());
			appAssembler.Register<RoadTilePath>().Allocator(new ObjectPool<RoadTilePath>());
			appAssembler.Register<RoadTilePath.Piece>().Allocator(new ObjectPool<RoadTilePath.Piece>());
			appAssembler.Register<RoadTileMesh>().Allocator(new ObjectPool<RoadTileMesh>());
			appAssembler.Register<RoadTileConnectionStrokePath>().Allocator(new ObjectPool<RoadTileConnectionStrokePath>());
			appAssembler.Register<RailTileAtlas>().Allocator(new HeapAllocator<RailTileAtlas>()).Binding(Binding.Scope);
			appAssembler.Register<RailTileDefinition>().Allocator(new ObjectPool<RailTileDefinition>());
			appAssembler.Register<BoatPathTileAtlas>().Allocator(new HeapAllocator<BoatPathTileAtlas>()).Binding(Binding.Scope);
			appAssembler.Register<BoatPathTileDefinition>().Allocator(new ObjectPool<BoatPathTileDefinition>());
			appAssembler.Register<MenuNavigation, MotorwaysInGameStateToggleController>().Allocator(new HeapAllocator<MotorwaysInGameStateToggleController>()).Binding(Binding.Scope);
			appAssembler.Register<ILegacyUserProfile, LegacyMotorwaysUserProfile>().Allocator(new HeapAllocator<LegacyMotorwaysUserProfile>());
			appAssembler.Register<IExtendedUserProfile, MotorwaysExtendedUserProfile>().Allocator(new HeapAllocator<MotorwaysExtendedUserProfile>());
			appAssembler.Register<IDeviceSettings, MotorwaysDeviceSettings>().Allocator(new HeapAllocator<MotorwaysDeviceSettings>());
			appAssembler.Register<IGameJournalSave, MotorwaysGameJournalSave>().Allocator(new HeapAllocator<MotorwaysGameJournalSave>());
			appAssembler.Register<IMotorwaysGameJournalHeader, MotorwaysGameJournalHeader>().Allocator(new HeapAllocator<MotorwaysGameJournalHeader>());
			appAssembler.Register<InGameInputStateChangeAction>().Allocator(new ObjectPool<InGameInputStateChangeAction>
			{
				InitialSize = 50
			});
			appAssembler.Register<AchievementDefinition, MotorwaysAchievementDefinition>().Allocator(new HeapAllocator<MotorwaysAchievementDefinition>());
			appAssembler.Register<Achievement, MotorwaysAchievement>().Allocator(new HeapAllocator<MotorwaysAchievement>());
			appAssembler.Register<MotorwaysCityStatistics>().Allocator(new HeapAllocator<MotorwaysCityStatistics>());
			appAssembler.Register<MotorwaysTimedChallengeScore>().Allocator(new HeapAllocator<MotorwaysTimedChallengeScore>());
			appAssembler.Register<LeaderboardService>().Allocator(new HeapAllocator<LeaderboardService>()).Binding(Binding.Scope);
			NotificationDescriptorDatabase notificationDescriptorDatabase = AssetBundleUtility.LoadAsset<NotificationDescriptorDatabase>("core", "GameNotificationDatabase");
			appAssembler.Register<NotificationDescriptorDatabase>().Allocator(new SingletonAllocator<NotificationDescriptorDatabase>(notificationDescriptorDatabase)).Binding(Binding.Scope);
			SupportedLocaleDatabase supportedLocaleDatabase = AssetBundleUtility.LoadAsset<SupportedLocaleDatabase>("core", "SupportedLocaleDatabase");
			appAssembler.Register<SupportedLocaleDatabase, SupportedLocaleDatabase>().Allocator(new SingletonAllocator<SupportedLocaleDatabase>(supportedLocaleDatabase)).Binding(Binding.Scope);
			if (FeatureToggle.IsFeatureEnabled(Feature.CycleLanguages))
			{
				appAssembler.Register<SetLanguageAction>().Allocator(new ObjectPool<SetLanguageAction>
				{
					InitialSize = 2
				});
			}
			return appAssembler;
		}

		// Token: 0x06001685 RID: 5765 RVA: 0x0004E90C File Offset: 0x0004CB0C
		protected override Assembler CreateGameAssembler(Assembler appAssembler)
		{
			Assembler gameAssembler = new Assembler("motorways");
			gameAssembler.IsValidatingObjectScrubbing = Application.isEditor;
			gameAssembler.Register<PseudorandomGenerator>().Allocator(new ObjectPool<PseudorandomGenerator>
			{
				InitialSize = 1
			});
			gameAssembler.Register<RoadTileSignature>().Allocator(new ObjectPool<RoadTileSignature>());
			gameAssembler.Register<ISimulation, Simulation>().Allocator(new ObjectPool<Simulation>()).Binding(Binding.Scope);
			gameAssembler.Register<CommandJournal>().Allocator(new ObjectPool<CommandJournal>()).Binding(Binding.Scope);
			gameAssembler.Register<Clock>().Allocator(new HeapAllocator<Clock>()).Binding(Binding.Scope);
			gameAssembler.Register<IClient, MotorwaysClient>().Allocator(new ObjectPool<MotorwaysClient>
			{
				InitialSize = 2,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<Passage>().Allocator(new ObjectPool<Passage>
			{
				InitialSize = 10
			});
			gameAssembler.Register<City>().Allocator(new ObjectPool<City>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<TileEditor>().Allocator(new HeapAllocator<TileEditor>()).Binding(Binding.Scope);
			gameAssembler.Register<Pathfinder>().Allocator(new HeapAllocator<Pathfinder>()).Binding(Binding.Scope);
			gameAssembler.Register<TilePathfinder>().Allocator(new HeapAllocator<TilePathfinder>()).Binding(Binding.Scope);
			gameAssembler.Register<GameRules>().Allocator(new HeapAllocator<GameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<EndlessGameRules>().Allocator(new HeapAllocator<EndlessGameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<ExpertGameRules>().Allocator(new HeapAllocator<ExpertGameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<CreativeGameRules>().Allocator(new HeapAllocator<CreativeGameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<TutorialGameRules>().Allocator(new HeapAllocator<TutorialGameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<BackgroundGameRules>().Allocator(new HeapAllocator<BackgroundGameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<MovieGameRules>().Allocator(new HeapAllocator<MovieGameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<CinematicGameRules>().Allocator(new HeapAllocator<CinematicGameRules>()).Binding(Binding.Scope);
			gameAssembler.Register<SelectUpgradeCommand>().Allocator(new ObjectPool<SelectUpgradeCommand>());
			gameAssembler.Register<InitCityCommand>().Allocator(new ObjectPool<InitCityCommand>());
			gameAssembler.Register<EditTileCommand>().Allocator(new ObjectPool<EditTileCommand>());
			gameAssembler.Register<ReserveTileCommand>().Allocator(new ObjectPool<ReserveTileCommand>());
			gameAssembler.Register<RemoveHouseCommand>().Allocator(new ObjectPool<RemoveHouseCommand>());
			gameAssembler.Register<RemoveDestinationCommand>().Allocator(new ObjectPool<RemoveDestinationCommand>());
			gameAssembler.Register<RemoveCarparkCommand>().Allocator(new ObjectPool<RemoveCarparkCommand>());
			gameAssembler.Register<ClearTileReservationsCommand>().Allocator(new ObjectPool<ClearTileReservationsCommand>());
			gameAssembler.Register<SetPausedCommand>().Allocator(new ObjectPool<SetPausedCommand>());
			gameAssembler.Register<AdvanceTutorialCommand>().Allocator(new ObjectPool<AdvanceTutorialCommand>());
			gameAssembler.Register<SnapshotCommand>().Allocator(new ObjectPool<SnapshotCommand>());
			gameAssembler.Register<AddRoadEdit>().Allocator(new ObjectPool<AddRoadEdit>
			{
				InitialSize = 20
			});
			gameAssembler.Register<AddRoundaboutEdit>().Allocator(new ObjectPool<AddRoundaboutEdit>
			{
				InitialSize = 20
			});
			gameAssembler.Register<AddRoadLineEdit>().Allocator(new ObjectPool<AddRoadLineEdit>
			{
				InitialSize = 20
			});
			gameAssembler.Register<AlignDrivewayEdit>().Allocator(new ObjectPool<AlignDrivewayEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<AddMotorwayEdit>().Allocator(new ObjectPool<AddMotorwayEdit>
			{
				InitialSize = 2
			});
			gameAssembler.Register<MothballMotorwayEdit>().Allocator(new ObjectPool<MothballMotorwayEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<ClearTileEdit>().Allocator(new ObjectPool<ClearTileEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<RemoveTrafficLightEdit>().Allocator(new ObjectPool<RemoveTrafficLightEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<RemoveMotorwaysEdit>().Allocator(new ObjectPool<RemoveMotorwaysEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<RemoveUnbuiltMotorwaysEdit>().Allocator(new ObjectPool<RemoveUnbuiltMotorwaysEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<RemovePassagesEdit>().Allocator(new ObjectPool<RemovePassagesEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<RemoveRoundaboutEdit>().Allocator(new ObjectPool<RemoveRoundaboutEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<RestoreMothballedPassageEdit>().Allocator(new ObjectPool<RestoreMothballedPassageEdit>
			{
				InitialSize = 5
			});
			gameAssembler.Register<AddTrafficLightEdit>().Allocator(new ObjectPool<AddTrafficLightEdit>
			{
				InitialSize = 20
			});
			gameAssembler.Register<AddUnbuiltMotorwayEdit>().Allocator(new ObjectPool<AddUnbuiltMotorwayEdit>
			{
				InitialSize = 20
			});
			gameAssembler.Register<TileMatrixInt>().Allocator(new ObjectPool<TileMatrixInt>
			{
				InitialSize = 20
			});
			gameAssembler.Register<TileMatrixBool>().Allocator(new ObjectPool<TileMatrixBool>
			{
				InitialSize = 20
			});
			gameAssembler.Register<ClockProcess>().Allocator(new ObjectPool<ClockProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<EfficiencyCalculationProcess>().Allocator(new ObjectPool<EfficiencyCalculationProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<LaneUpdateProcess>().Allocator(new ObjectPool<LaneUpdateProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<VehicleMovementProcess>().Allocator(new ObjectPool<VehicleMovementProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<ParkVehiclesProcess>().Allocator(new ObjectPool<ParkVehiclesProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<BuildMotorwaysProcess>().Allocator(new ObjectPool<BuildMotorwaysProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<BuildRoundaboutsProcess>().Allocator(new ObjectPool<BuildRoundaboutsProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<TrafficLightAlternatingProcess>().Allocator(new ObjectPool<TrafficLightAlternatingProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<DispatchVehiclesProcess>().Allocator(new ObjectPool<DispatchVehiclesProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<GenerateDemandProcess>().Allocator(new ObjectPool<GenerateDemandProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<FailureStateProcess>().Allocator(new ObjectPool<FailureStateProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<AchievementCheckingProcess>().Allocator(new ObjectPool<AchievementCheckingProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<UpgradeAwardingProcess>().Allocator(new ObjectPool<UpgradeAwardingProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<UpgradeChoice>().Allocator(new ObjectPool<UpgradeChoice>
			{
				InitialSize = 10
			});
			gameAssembler.Register<VehiclePathfindingProcess>().Allocator(new ObjectPool<VehiclePathfindingProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<IntersectionEvaluatingProcess>().Allocator(new ObjectPool<IntersectionEvaluatingProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<ReleaseMothballedLanesProcess>().Allocator(new ObjectPool<ReleaseMothballedLanesProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<ReleaseMotorwaysProcess>().Allocator(new ObjectPool<ReleaseMotorwaysProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<TilePermanenceUpdatingProcess>().Allocator(new ObjectPool<TilePermanenceUpdatingProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<BuildingSpawningProcess>().Allocator(new ObjectPool<BuildingSpawningProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<VehicleSpawningProcess>().Allocator(new ObjectPool<VehicleSpawningProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<TrainSpawningProcess>().Allocator(new ObjectPool<TrainSpawningProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<TrainMovementProcess>().Allocator(new ObjectPool<TrainMovementProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<BoatMovementProcess>().Allocator(new ObjectPool<BoatMovementProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<OpenTrainCrossingsProcess>().Allocator(new ObjectPool<OpenTrainCrossingsProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<BoatSpawningProcess>().Allocator(new ObjectPool<BoatSpawningProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<TutorialProgressionProcess>().Allocator(new ObjectPool<TutorialProgressionProcess>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<CityModel>().Allocator(new ModelPool<CityModel>
			{
				InitialSize = 3
			}).Binding(Binding.Scope);
			gameAssembler.Register<CityPlanModel>().Allocator(new ModelPool<CityPlanModel>
			{
				InitialSize = 3
			}).Binding(Binding.Scope);
			gameAssembler.Register<CityPlanModel.ScheduledBuilding>().Allocator(new ObjectPool<CityPlanModel.ScheduledBuilding>
			{
				InitialSize = 50
			});
			gameAssembler.Register<DemandModel>().Allocator(new ModelPool<DemandModel>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<BuildingPlacer>().Allocator(new ObjectPool<BuildingPlacer>
			{
				InitialSize = 1
			}).Binding(Binding.Scope);
			gameAssembler.Register<TilemapModel>().Allocator(new ModelPool<TilemapModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<TileModel>().Allocator(new ModelPool<TileModel>
			{
				InitialSize = 400,
				GrowthStrategy = GrowthStrategy.OnDemand,
				BlockSize = 50
			});
			gameAssembler.Register<Tile>().Allocator(new ObjectPool<Tile>
			{
				InitialSize = 800,
				GrowthStrategy = GrowthStrategy.OnDemand,
				BlockSize = 50
			});
			gameAssembler.Register<TileCornerModel>().Allocator(new ModelPool<TileCornerModel>
			{
				InitialSize = 200,
				GrowthStrategy = GrowthStrategy.OnDemand,
				BlockSize = 50
			});
			gameAssembler.Register<MotorwayModel>().Allocator(new ModelPool<MotorwayModel>
			{
				InitialSize = 20
			});
			gameAssembler.Register<ClockModel>().Allocator(new ModelPool<ClockModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<ScoreModel>().Allocator(new ModelPool<ScoreModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<UpgradeDatabaseModel>().Allocator(new ModelPool<UpgradeDatabaseModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<ActiveChallengesModel>().Allocator(new ModelPool<ActiveChallengesModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<GameBehaviourModel>().Allocator(new ModelPool<GameBehaviourModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<SnapshotModel>().Allocator(new ModelPool<SnapshotModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<VehicleDispatchRecord>().Allocator(new ObjectPool<VehicleDispatchRecord>
			{
				InitialSize = 10,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<IntersectionDecisionDatabaseModel>().Allocator(new ObjectPool<IntersectionDecisionDatabaseModel>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<IntersectionEntryDecision>().Allocator(new ObjectPool<IntersectionEntryDecision>
			{
				InitialSize = 100,
				GrowthStrategy = GrowthStrategy.Block,
				IsValidatingObjectScrubbing = false
			});
			gameAssembler.Register<IntersectionEntryVehicleContext>().Allocator(new ObjectPool<IntersectionEntryVehicleContext>
			{
				InitialSize = 1000,
				GrowthStrategy = GrowthStrategy.Block,
				IsValidatingObjectScrubbing = false
			});
			gameAssembler.Register<VehicleModel>().Allocator(new ModelPool<VehicleModel>
			{
				InitialSize = 200,
				BlockSize = 20
			});
			if (FeatureToggle.IsFeatureEnabled(Feature.WhatTheCarEasterEgg))
			{
				gameAssembler.Register<EasterEggModel>().Allocator(new ModelPool<EasterEggModel>()).Binding(Binding.Scope);
			}
			gameAssembler.Register<HouseModel>().Allocator(new ModelPool<HouseModel>
			{
				InitialSize = 20,
				BlockSize = 20
			});
			gameAssembler.Register<DestinationModel>().Allocator(new ModelPool<DestinationModel>
			{
				InitialSize = 20,
				BlockSize = 20
			});
			gameAssembler.Register<CarparkModel>().Allocator(new ModelPool<CarparkModel>
			{
				InitialSize = 20,
				BlockSize = 20
			});
			gameAssembler.Register<CarparkModel.ParkingSpace>().Allocator(new ObjectPool<CarparkModel.ParkingSpace>
			{
				InitialSize = 60,
				BlockSize = 60
			});
			gameAssembler.Register<LaneModel>().Allocator(new ModelPool<LaneModel>
			{
				InitialSize = 200,
				BlockSize = 100
			});
			gameAssembler.Register<RoadChunkModel>().Allocator(new ModelPool<RoadChunkModel>
			{
				InitialSize = 100,
				BlockSize = 50
			});
			gameAssembler.Register<RoadChunkModel.InboundVehicle>().Allocator(new ObjectPool<RoadChunkModel.InboundVehicle>
			{
				InitialSize = 100,
				BlockSize = 50
			});
			gameAssembler.Register<TrafficLightModel>().Allocator(new ModelPool<TrafficLightModel>
			{
				InitialSize = 20,
				BlockSize = 20
			});
			gameAssembler.Register<TrainCrossingModel>().Allocator(new ModelPool<TrainCrossingModel>
			{
				InitialSize = 20,
				BlockSize = 20
			});
			gameAssembler.Register<RoundaboutModel>().Allocator(new ModelPool<RoundaboutModel>
			{
				InitialSize = 5,
				BlockSize = 5
			});
			gameAssembler.Register<PassageModel>().Allocator(new ModelPool<PassageModel>
			{
				InitialSize = 5,
				BlockSize = 5
			});
			gameAssembler.Register<AnchoredMessageModel>().Allocator(new ModelPool<AnchoredMessageModel>
			{
				InitialSize = 5,
				BlockSize = 5
			});
			gameAssembler.Register<TreeModel>().Allocator(new ModelPool<TreeModel>
			{
				InitialSize = 10,
				BlockSize = 5
			});
			gameAssembler.Register<TrainLineModel>().Allocator(new ModelPool<TrainLineModel>
			{
				InitialSize = 2,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<RailTileModel>().Allocator(new ModelPool<RailTileModel>
			{
				InitialSize = 20,
				BlockSize = 5
			});
			gameAssembler.Register<TrainModel>().Allocator(new ModelPool<TrainModel>
			{
				InitialSize = 3,
				BlockSize = 3
			});
			gameAssembler.Register<BoatPathModel>().Allocator(new ModelPool<BoatPathModel>
			{
				InitialSize = 2,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<BoatPathTileModel>().Allocator(new ModelPool<BoatPathTileModel>
			{
				InitialSize = 20,
				BlockSize = 5
			});
			gameAssembler.Register<BoatModel>().Allocator(new ModelPool<BoatModel>
			{
				InitialSize = 3,
				BlockSize = 3
			});
			gameAssembler.Register<CameraView>().Allocator(new ObjectPool<CameraView>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<ClockView>().Allocator(new GameObjectPool<ClockView>("core", "ClockView")).Binding(Binding.Scope);
			gameAssembler.Register<ScoreView>().Allocator(new GameObjectPool<ScoreView>("core", "ScoreView")).Binding(Binding.Scope);
			if (FeatureToggle.IsFeatureEnabled(Feature.WrapperGameUI))
			{
				gameAssembler.Register<UpgradeBarClient, UpgradeBarWrapper>().Allocator(new NestedGameObjectAllocator<UpgradeBarWrapper, GameUIScreen>()).Binding(Binding.Scope);
				gameAssembler.Register<UpgradeBarClientHorizontal>();
			}
			else if (AppContainer.Environment.DeviceCategory == DeviceCategory.Desktop)
			{
				gameAssembler.Register<UpgradeBarClient, UpgradeBarClientHorizontal>().Allocator(new NestedGameObjectAllocator<UpgradeBarClientHorizontal, GameUIScreen>()).Binding(Binding.Scope);
			}
			else
			{
				gameAssembler.Register<UpgradeBarClient>().Allocator(new NestedGameObjectAllocator<UpgradeBarClient, GameUIScreen>()).Binding(Binding.Scope);
			}
			gameAssembler.Register<EditMenuPanel>().Allocator(new NestedGameObjectAllocator<EditMenuPanel, GameUIScreen>()).Binding(Binding.Scope);
			gameAssembler.Register<ColourWidget>().Allocator(new NestedGameObjectAllocator<ColourWidget, GameUIScreen>()).Binding(Binding.Scope);
			gameAssembler.Register<NotificationView>().Allocator(new ObjectPool<NotificationView>()).Binding(Binding.Scope);
			gameAssembler.Register<ChallengeView>().Allocator(new ObjectPool<ChallengeView>()).Binding(Binding.Scope);
			gameAssembler.Register<CitySpawningView>().Allocator(new GameObjectPool<CitySpawningView>("core", "CitySpawningView")
			{
				InitialSize = 2,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<TilemapView>().Allocator(new GameObjectPool<TilemapView>("core", "CityMap")
			{
				InitialSize = 2,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<DeadEndRoadView>().Allocator(new GameObjectPool<DeadEndRoadView>("core", "DeadEndRoad")
			{
				InitialSize = 50,
				GrowthStrategy = GrowthStrategy.Block,
				BlockSize = 10
			});
			gameAssembler.Register<AnimatedRoadTileConnectionView>().Allocator(new GameObjectPool<AnimatedRoadTileConnectionView>("core", "AnimatedRoadTileConnection")
			{
				InitialSize = 10,
				GrowthStrategy = GrowthStrategy.Block,
				BlockSize = 5
			});
			gameAssembler.Register<TileView>().Allocator(new GameObjectPool<TileView>("core", "Tile")
			{
				InitialSize = 300,
				GrowthStrategy = GrowthStrategy.Block,
				BlockSize = 20
			});
			gameAssembler.Register<TileSelectedView>().Allocator(new GameObjectPool<TileSelectedView>("core", "TileSelected")
			{
				InitialSize = 30,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<CombinedMeshThemeComponent>().Allocator(new SingletonAllocator<CombinedMeshThemeComponent>(new CombinedMeshThemeComponent()));
			VehicleMeshCombiner meshCombiner = new VehicleMeshCombiner(AssetBundleUtility.LoadPrefab("core", "Vehicle"));
			appAssembler.Register<VehicleMeshCombiner>().Allocator(new SingletonAllocator<VehicleMeshCombiner>(meshCombiner));
			gameAssembler.Register<VehicleView>().Allocator(new GameObjectPool<VehicleView>(meshCombiner.combinedMeshVehiclePrefab)
			{
				InitialSize = 200,
				GrowthStrategy = GrowthStrategy.Block,
				BlockSize = 20
			});
			if (FeatureToggle.IsFeatureEnabled(Feature.WhatTheCarEasterEgg))
			{
				gameAssembler.Register<TribandVehicleEffects>().Allocator(new GameObjectPool<TribandVehicleEffects>("core", "TribandVehicleEffects"));
			}
			HouseMeshCombiner houseMeshCombiner = new HouseMeshCombiner(AssetBundleUtility.LoadPrefab("core", "House"));
			appAssembler.Register<HouseMeshCombiner>().Allocator(new SingletonAllocator<HouseMeshCombiner>(houseMeshCombiner)).Binding(Binding.Scope);
			gameAssembler.Register<HouseView>().Allocator(new GameObjectPool<HouseView>(houseMeshCombiner.combinedMeshHousePrefab)
			{
				InitialSize = 100,
				GrowthStrategy = GrowthStrategy.Block,
				BlockSize = 10
			});
			gameAssembler.Register<IndicatorAnimationView>().Allocator(new GameObjectPool<IndicatorAnimationView>("core", "IndicatorAnimations")
			{
				InitialSize = 10,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<PinView>().Allocator(new GameObjectPool<PinView>("core", "Pin")
			{
				InitialSize = 100,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<AnchoredMessageView>().Allocator(new GameObjectPool<AnchoredMessageView>("core", "AnchoredMessage")
			{
				InitialSize = 5,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			GameObject destinationPrefab = AssetBundleUtility.LoadPrefab("core", "Destination");
			appAssembler.Register<DestinationMeshCombiner>().Allocator(new SingletonAllocator<DestinationMeshCombiner>(new DestinationMeshCombiner(destinationPrefab))).Binding(Binding.Scope);
			gameAssembler.Register<DestinationView>().Allocator(new GameObjectPool<DestinationView>(destinationPrefab)
			{
				InitialSize = 40,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			CarparkMeshCombiner carparkMeshCombiner = new CarparkMeshCombiner(AssetBundleUtility.LoadPrefab("core", "Carpark"));
			appAssembler.Register<CarparkMeshCombiner>().Allocator(new SingletonAllocator<CarparkMeshCombiner>(carparkMeshCombiner)).Binding(Binding.Scope);
			gameAssembler.Register<CarparkView>().Allocator(new GameObjectPool<CarparkView>(carparkMeshCombiner.combinedCarparkPrefab)
			{
				InitialSize = 30,
				GrowthStrategy = GrowthStrategy.OnDemand
			});
			gameAssembler.Register<CombinedMeshView>().Allocator(new GameObjectPool<CombinedMeshView>("core", "CombinedMeshView")
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<RoadView>().Allocator(new GameObjectPool<RoadView>("core", "Road")
			{
				InitialSize = 500,
				BlockSize = 100
			});
			gameAssembler.Register<MotorwayView>().Allocator(new GameObjectPool<MotorwayView>("core", "Motorway")
			{
				InitialSize = 20,
				BlockSize = 10
			});
			gameAssembler.Register<TrafficLightView>().Allocator(new GameObjectPool<TrafficLightView>("core", "TrafficLight")
			{
				InitialSize = 10,
				BlockSize = 10
			});
			gameAssembler.Register<UnbuiltMotorwayView>().Allocator(new GameObjectPool<UnbuiltMotorwayView>("core", "UnbuiltMotorway")
			{
				InitialSize = 10,
				BlockSize = 10
			});
			gameAssembler.Register<RoundaboutView>().Allocator(new GameObjectPool<RoundaboutView>("core", "Roundabout")
			{
				InitialSize = 10,
				BlockSize = 10
			});
			gameAssembler.Register<TreeView>().Allocator(new GameObjectPool<TreeView>("core", "Tree")
			{
				InitialSize = 20,
				BlockSize = 10
			});
			gameAssembler.Register<RailView>().Allocator(new GameObjectPool<RailView>("core", "Rail")
			{
				InitialSize = 50,
				BlockSize = 10
			});
			gameAssembler.Register<TrainCrossingView>().Allocator(new GameObjectPool<TrainCrossingView>("core", "TrainCrossing")
			{
				InitialSize = 5,
				BlockSize = 5
			});
			gameAssembler.Register<TrainView>().Allocator(new GameObjectPool<TrainView>("core", "Train")
			{
				InitialSize = 3,
				BlockSize = 3
			});
			gameAssembler.Register<BoatPathView>().Allocator(new GameObjectPool<BoatPathView>("core", "BoatPath")
			{
				InitialSize = 50,
				BlockSize = 10
			});
			gameAssembler.Register<BoatView>().Allocator(new GameObjectPool<BoatView>("core", "Boat")
			{
				InitialSize = 1,
				BlockSize = 1
			});
			gameAssembler.Register<ViewIndex>().Allocator(new ObjectPool<ViewIndex>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<AlertView>().Allocator(new GameObjectPool<AlertView>("core", "Alert")
			{
				InitialSize = 30
			});
			gameAssembler.Register<BuildingIndicatorEventView>().Allocator(new ObjectPool<BuildingIndicatorEventView>
			{
				InitialSize = 20
			});
			gameAssembler.Register<IndicatorEchoView>().Allocator(new GameObjectPool<IndicatorEchoView>("core", "IndicatorEcho")
			{
				InitialSize = 30
			});
			gameAssembler.Register<IndicatorArrowView>().Allocator(new GameObjectPool<IndicatorArrowView>("core", "IndicatorArrow")
			{
				InitialSize = 30
			});
			gameAssembler.Register<UpgradeCursor>().Allocator(new GameObjectPool<UpgradeCursor>("core", "UpgradeCursor")
			{
				InitialSize = 30
			});
			if (Application.isPlaying)
			{
				GameObject debugView = new GameObject("City Schedule Debug View");
				debugView.SetActive(false);
				debugView.AddComponent<CityScheduleView>();
				gameAssembler.Register<CityScheduleView>().Allocator(new GameObjectPool<CityScheduleView>(debugView)
				{
					InitialSize = 2,
					GrowthStrategy = GrowthStrategy.OnDemand
				});
				debugView = new GameObject("Simulation Toggle Debug View");
				debugView.AddComponent<SimulationToggleDebugView>();
				gameAssembler.Register<SimulationToggleDebugView>().Allocator(new GameObjectPool<SimulationToggleDebugView>(debugView)
				{
					InitialSize = 2,
					GrowthStrategy = GrowthStrategy.OnDemand
				});
				debugView = new GameObject("Hotkey Debug View");
				debugView.AddComponent<HotkeyDebugView>();
				debugView.SetActive(false);
				appAssembler.Register<HotkeyDebugView>().Allocator(new GameObjectPool<HotkeyDebugView>(debugView)
				{
					InitialSize = 1,
					GrowthStrategy = GrowthStrategy.OnDemand
				}).Binding(Binding.Scope);
				debugView = new GameObject("Tutorial Debug View");
				debugView.AddComponent<TutorialDebugView>();
				gameAssembler.Register<TutorialDebugView>().Allocator(new GameObjectPool<TutorialDebugView>(debugView)
				{
					InitialSize = 2,
					GrowthStrategy = GrowthStrategy.OnDemand
				});
				debugView = new GameObject("Idle Vehicle Checker View");
				debugView.AddComponent<IdleVehicleCheckerDebugView>();
				gameAssembler.Register<IdleVehicleCheckerDebugView>().Allocator(new GameObjectPool<IdleVehicleCheckerDebugView>(debugView)
				{
					InitialSize = 2,
					GrowthStrategy = GrowthStrategy.OnDemand
				});
			}
			gameAssembler.Register<NetworkConnectivityUpdater>().Allocator(new ObjectPool<NetworkConnectivityUpdater>
			{
				InitialSize = 2
			}).Binding(Binding.Scope);
			gameAssembler.Register<ClientUpgradeDatabase>().Allocator(new ObjectPool<ClientUpgradeDatabase>
			{
				InitialSize = 1,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			gameAssembler.Register<BuildingsIndicatorView>().Allocator(new GameObjectPool<BuildingsIndicatorView>("core", "BuildingsIndicatorPrefab")
			{
				InitialSize = 2,
				GrowthStrategy = GrowthStrategy.OnDemand
			}).Binding(Binding.Scope);
			if (FeatureToggle.IsFeatureEnabled(Feature.WrapperGameUI))
			{
				gameAssembler.Register<GameUIScreen, GameUIScreenWrapper>().Allocator(new GameObjectPool<GameUIScreenWrapper>("core", "InGameUI-Wrapper")
				{
					InitialSize = 2,
					GrowthStrategy = GrowthStrategy.OnDemand
				}).Binding(Binding.Scope);
			}
			else
			{
				gameAssembler.Register<GameUIScreen>().Allocator(new GameObjectPool<GameUIScreen>("core", string.Format("InGameUI-{0}", AppContainer.Environment.DeviceCategory))
				{
					InitialSize = 2,
					GrowthStrategy = GrowthStrategy.OnDemand
				}).Binding(Binding.Scope);
			}
			gameAssembler.Register<NewRoadPreview>().Allocator(new GameObjectPool<NewRoadPreview>("core", "NewRoadPreview")
			{
				InitialSize = 2
			});
			gameAssembler.Register<NewUpgradeAnimationView>().Allocator(new GameObjectPool<NewUpgradeAnimationView>("core", "NewUpgradeAnimation"));
			gameAssembler.Register<AdvanceTutorialAction>().Allocator(new ObjectPool<AdvanceTutorialAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ToggleDrawModeAction>().Allocator(new ObjectPool<ToggleDrawModeAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DoubleTapToggleDrawModeAction>().Allocator(new ObjectPool<DoubleTapToggleDrawModeAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<PressUIFocusAction>().Allocator(new ObjectPool<PressUIFocusAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ChangeGameSpeedAction>().Allocator(new ObjectPool<ChangeGameSpeedAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ChangeUpgradeBarAction>().Allocator(new ObjectPool<ChangeUpgradeBarAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<LaneCursor>().Allocator(new ObjectPool<LaneCursor>
			{
				InitialSize = 5
			});
			gameAssembler.Register<DrawRoadAction>().Allocator(new ObjectPool<DrawRoadAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ToggleDragClearTileAction>().Allocator(new ObjectPool<ToggleDragClearTileAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragClearTileAction>().Allocator(new ObjectPool<DragClearTileAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<MoveInGameFocusAction>().Allocator(new ObjectPool<MoveInGameFocusAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragMoveInGameFocusAction>().Allocator(new ObjectPool<DragMoveInGameFocusAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDrawRoadAction>().Allocator(new ObjectPool<ControllerDrawRoadAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragMotorwayAction>().Allocator(new ObjectPool<DragMotorwayAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDragMotorwayAction>().Allocator(new ObjectPool<ControllerDragMotorwayAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDragEditMotorwayAction>().Allocator(new ObjectPool<ControllerDragEditMotorwayAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragCreativeModeEditableObjectAction>().Allocator(new ObjectPool<DragCreativeModeEditableObjectAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragTrafficLightAction>().Allocator(new ObjectPool<DragTrafficLightAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDragTrafficLightAction>().Allocator(new ObjectPool<ControllerDragTrafficLightAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragRoundaboutAction>().Allocator(new ObjectPool<DragRoundaboutAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDragRoundaboutAction>().Allocator(new ObjectPool<ControllerDragRoundaboutAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragMotorwayHandleAction>().Allocator(new ObjectPool<DragMotorwayHandleAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDragMotorwayHandleAction>().Allocator(new ObjectPool<ControllerDragMotorwayHandleAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragEditMotorwayAction>().Allocator(new ObjectPool<DragEditMotorwayAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragHouseAction>().Allocator(new ObjectPool<DragHouseAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDragHouseAction>().Allocator(new ObjectPool<ControllerDragHouseAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<DragDestinationAction>().Allocator(new ObjectPool<DragDestinationAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerDragDestinationAction>().Allocator(new ObjectPool<ControllerDragDestinationAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ControllerEditMenuNavigateAction>().Allocator(new ObjectPool<ControllerEditMenuNavigateAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<RemoteEditMenuNavigateAction>().Allocator(new ObjectPool<RemoteEditMenuNavigateAction>
			{
				InitialSize = 2
			});
			GameObject draftHousePrefab = AssetBundleUtility.LoadPrefab("core", "DraftHouse");
			gameAssembler.Register<DraftHouse>().Allocator(new GameObjectPool<DraftHouse>(draftHousePrefab)
			{
				InitialSize = 1
			});
			GameObject draftDestinationPrefab = AssetBundleUtility.LoadPrefab("core", "DraftDestination");
			gameAssembler.Register<DraftDestination>().Allocator(new GameObjectPool<DraftDestination>(draftDestinationPrefab)
			{
				InitialSize = 1
			});
			gameAssembler.Register<TouchCameraAction>().Allocator(new ObjectPool<TouchCameraAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ToggleZoomAction>().Allocator(new ObjectPool<ToggleZoomAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ToggleCreativeModeEditMenuAction>().Allocator(new ObjectPool<ToggleCreativeModeEditMenuAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<OpenElectiveUpgradeScreenAction>().Allocator(new ObjectPool<OpenElectiveUpgradeScreenAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<ToggleGameUIAction>().Allocator(new ObjectPool<ToggleGameUIAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<MouseCameraAction>().Allocator(new ObjectPool<MouseCameraAction>
			{
				InitialSize = 2
			});
			gameAssembler.Register<IGameStatistics, MotorwaysGameStatistics>().Allocator(new HeapAllocator<MotorwaysGameStatistics>());
			gameAssembler.Register<ControllerCameraAction>().Allocator(new ObjectPool<ControllerCameraAction>
			{
				InitialSize = 2
			});
			if (FeatureToggle.IsFeatureEnabled(Feature.InGameDevTools))
			{
				gameAssembler.Register<IInGameDevToolsRegistry, InGameDevToolsRegistry>().Allocator(new ObjectPool<InGameDevToolsRegistry>
				{
					InitialSize = 1
				}).Binding(Binding.Scope);
			}
			else
			{
				gameAssembler.Register<IInGameDevToolsRegistry, NullInGameDevToolsRegistry>().Allocator(new ObjectPool<NullInGameDevToolsRegistry>
				{
					InitialSize = 1
				}).Binding(Binding.Scope);
			}
			gameAssembler.Register<SimpleActionDevTool>().Allocator(new ObjectPool<SimpleActionDevTool>());
			gameAssembler.Register<SimpleActionDevToolCommand>().Allocator(new ObjectPool<SimpleActionDevToolCommand>());
			gameAssembler.Register<MotorwaysDevTool>().Allocator(new ObjectPool<MotorwaysDevTool>());
			gameAssembler.Register<MotorwaysDevToolCommand>().Allocator(new ObjectPool<MotorwaysDevToolCommand>());
			gameAssembler.Register<MotorwaysModelContainerTool>().Allocator(new ObjectPool<MotorwaysModelContainerTool>());
			gameAssembler.Register<HouseDevTool>().Allocator(new ObjectPool<HouseDevTool>());
			gameAssembler.Register<DestinationDevTool>().Allocator(new ObjectPool<DestinationDevTool>());
			gameAssembler.Register<MotorwaysModelDevToolCommand>().Allocator(new ObjectPool<MotorwaysModelDevToolCommand>());
			AppContainer.Environment.PopulateGameAssembler(gameAssembler);
			appAssembler.Register<Game, MotorwaysGame>().Allocator(new HeapAllocator<MotorwaysGame>()).EstablishScope(gameAssembler).Binding(Binding.EstablishedScope);
			gameAssembler.Register<GameplayEventHandler>().Allocator(new ObjectPool<GameplayEventHandler>
			{
				InitialSize = 1
			}).Binding(Binding.Scope);
			return gameAssembler;
		}
	}
}
