using System;
using DevTools.OnScreenDebugTools;
using Factory;
using Factory.Allocators;
using Factory.Pools;
using Motorways;
using Motorways.Audio;
using Motorways.UI;
using Motorways.Views;
using Popups;
using UnityEngine;

// Token: 0x0200006E RID: 110
public abstract class AppContainer
{
	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060000C6 RID: 198 RVA: 0x00003D72 File Offset: 0x00001F72
	// (set) Token: 0x060000C7 RID: 199 RVA: 0x00003D7A File Offset: 0x00001F7A
	public Assembler AppAssembler { get; private set; }

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x060000C8 RID: 200 RVA: 0x00003D83 File Offset: 0x00001F83
	// (set) Token: 0x060000C9 RID: 201 RVA: 0x00003D8B File Offset: 0x00001F8B
	public Assembler GameAssembler { get; private set; }

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060000CA RID: 202 RVA: 0x00003D94 File Offset: 0x00001F94
	// (set) Token: 0x060000CB RID: 203 RVA: 0x00003D9C File Offset: 0x00001F9C
	public IScope AppScope { get; private set; }

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060000CC RID: 204 RVA: 0x00003DA5 File Offset: 0x00001FA5
	// (set) Token: 0x060000CD RID: 205 RVA: 0x00003DAD File Offset: 0x00001FAD
	public IApp App { get; private set; }

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x060000CE RID: 206 RVA: 0x00003DB6 File Offset: 0x00001FB6
	// (set) Token: 0x060000CF RID: 207 RVA: 0x00003DBD File Offset: 0x00001FBD
	public static IEnvironment Environment { get; private set; }

	// Token: 0x060000D0 RID: 208 RVA: 0x00003DC5 File Offset: 0x00001FC5
	public void SetEnvironment(IEnvironment environment)
	{
		AppContainer.Environment = environment;
		FeatureToggle.AddSource(new BuildTimeConfigSettingSource(environment));
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x060000D1 RID: 209 RVA: 0x00003DD8 File Offset: 0x00001FD8
	// (set) Token: 0x060000D2 RID: 210 RVA: 0x00003DE0 File Offset: 0x00001FE0
	public AppCommandJournal CommandJournal { get; private set; }

	// Token: 0x060000D3 RID: 211 RVA: 0x00003DEC File Offset: 0x00001FEC
	public void CreateAssemblers()
	{
		if (AppContainer.Environment == null)
		{
			this.SetEnvironment(this.CreateDefaultEnvironment());
		}
		this.RegisterSerializers();
		this.AppAssembler = this.CreateAppAssembler();
		this.GameAssembler = this.CreateGameAssembler(this.AppAssembler);
		Debug.LogFormat("Assembler serializer hash codes: {0}, {1}", new object[]
		{
			this.AppAssembler.GlobalTypeSerializerHashCode,
			this.GameAssembler.GlobalTypeSerializerHashCode
		});
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00003E66 File Offset: 0x00002066
	public IScope CreateScope()
	{
		this.AppScope = new Scope(this.AppAssembler, null);
		this.RegisterStorableTypeHandlers();
		return this.AppScope;
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00003E86 File Offset: 0x00002086
	public IApp CreateApp()
	{
		this.App = this.AppScope.Get<IApp>();
		return this.App;
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00003E9F File Offset: 0x0000209F
	public void Start(bool recordJournal = false)
	{
		if (recordJournal)
		{
			this.CommandJournal = this.AppScope.Get<AppCommandJournal>();
		}
		this._commandSource = this.AppScope.Get<IAppCommandSource>();
		this._commandSource.Start();
		this.App.Start();
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00003EDC File Offset: 0x000020DC
	public void Tick()
	{
		foreach (IAppCommand command in this._commandSource.GetFrameCommands())
		{
			AppCommandJournal commandJournal = this.CommandJournal;
			if (commandJournal != null)
			{
				commandJournal.Record(command);
			}
			command.Execute(this.App);
			if (this.CommandJournal == null)
			{
				this.AppScope.Release(command);
			}
		}
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x000022F5 File Offset: 0x000004F5
	protected virtual void RegisterSerializers()
	{
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00003F5C File Offset: 0x0000215C
	protected virtual void RegisterStorableTypeHandlers()
	{
		IStorableTypeHandlerRegistry storableTypeHandlerRegistry = this.AppScope.Get<IStorableTypeHandlerRegistry>();
		storableTypeHandlerRegistry.RegisterHandler<ILegacyUserProfile>(this.AppScope.Get<UserProfileStorableTypeHandler>());
		storableTypeHandlerRegistry.RegisterHandler<IExtendedUserProfile>(this.AppScope.Get<ExtendedUserProfileStorableTypeHandler>());
		storableTypeHandlerRegistry.RegisterHandler<IDeviceSettings>(this.AppScope.Get<DeviceSettingsStorableTypeHandler>());
		storableTypeHandlerRegistry.RegisterHandler<IGameJournalSave>(this.AppScope.Get<SavedGameStorableTypeHandler>());
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00003FB8 File Offset: 0x000021B8
	protected virtual Assembler CreateAppAssembler()
	{
		Assembler appAssembler = new Assembler("app");
		appAssembler.IsValidatingObjectScrubbing = Application.isEditor;
		appAssembler.Register<LocaleDatabase>().Allocator(new HeapAllocator<LocaleDatabase>()).Binding(Binding.Scope);
		appAssembler.Register<Diagnostics.StorageAuditTrail>().Allocator(new HeapAllocator<Diagnostics.StorageAuditTrail>()).Binding(Binding.Scope);
		appAssembler.Register<IStorableTypeHandlerRegistry, StorableTypeHandlerRegistry>().Allocator(new HeapAllocator<StorableTypeHandlerRegistry>()).Binding(Binding.Scope);
		appAssembler.Register<UserProfileStorableTypeHandler>().Allocator(new HeapAllocator<UserProfileStorableTypeHandler>()).Binding(Binding.Scope);
		appAssembler.Register<ExtendedUserProfileStorableTypeHandler>().Allocator(new HeapAllocator<ExtendedUserProfileStorableTypeHandler>()).Binding(Binding.Scope);
		appAssembler.Register<DeviceSettingsStorableTypeHandler>().Allocator(new HeapAllocator<DeviceSettingsStorableTypeHandler>()).Binding(Binding.Scope);
		appAssembler.Register<SavedGameStorableTypeHandler>().Allocator(new HeapAllocator<SavedGameStorableTypeHandler>()).Binding(Binding.Scope);
		appAssembler.Register<IPersistentStorageService, PersistentStorageService>().Allocator(new HeapAllocator<PersistentStorageService>()).Binding(Binding.Scope);
		appAssembler.Register<IOAuthClient, BrowserOAuthClient>().Allocator(new HeapAllocator<BrowserOAuthClient>()).Binding(Binding.Scope);
		appAssembler.Register<ISteamCloudSyncService, SteamworksCloudSyncService>().Allocator(new HeapAllocator<SteamworksCloudSyncService>()).Binding(Binding.Scope);
		appAssembler.Register<PlayerDatabase>().Allocator(new HeapAllocator<PlayerDatabase>()).Binding(Binding.Scope);
		appAssembler.Register<Player>().Allocator(new HeapAllocator<Player>());
		appAssembler.Register<IActivePlayer, ActivePlayer>().Allocator(new HeapAllocator<ActivePlayer>()).Binding(Binding.Scope);
		appAssembler.Register<StringKey, MotorwaysStringKey>().Allocator(new StringKeyPool<MotorwaysStringKey>
		{
			InitialSize = 10000,
			BlockSize = 1000
		});
		appAssembler.Register<StandaloneLocString>().Allocator(new StringPool<StandaloneLocString>
		{
			InitialSize = 10000,
			BlockSize = 1000
		});
		appAssembler.Register<IApp, App>().Allocator(new HeapAllocator<App>()).Binding(Binding.Scope);
		appAssembler.Register<TickRegistry>().Allocator(new HeapAllocator<TickRegistry>()).Binding(Binding.Scope);
		appAssembler.Register<IAudioSystem, AudioSystem>().Allocator(new HeapAllocator<AudioSystem>()).Binding(Binding.Scope);
		appAssembler.Register<HapticFeedbackGenerator>().Allocator(new HeapAllocator<HapticFeedbackGenerator>()).Binding(Binding.Scope);
		appAssembler.Register<IInputState, InputState>().Allocator(new HeapAllocator<InputState>()).Binding(Binding.Scope);
		appAssembler.Register<IPointerState, PointerState>().Allocator(new HeapAllocator<PointerState>());
		appAssembler.Register<IMouseController, MouseController>().Allocator(new HeapAllocator<MouseController>()).Binding(Binding.Scope);
		appAssembler.Register<IKeyboardController, KeyboardController>().Allocator(new HeapAllocator<KeyboardController>()).Binding(Binding.Scope);
		appAssembler.Register<ITouchScreenController, TouchScreenController>().Allocator(new HeapAllocator<TouchScreenController>()).Binding(Binding.Scope);
		appAssembler.Register<IAppleTVRemoteController, AppleTVRemoteController>().Allocator(new HeapAllocator<AppleTVRemoteController>()).Binding(Binding.Scope);
		appAssembler.Register<IGamepadController, GenericGamepadController>().Allocator(new HeapAllocator<GenericGamepadController>()).Binding(Binding.Scope);
		appAssembler.Register<ButtonState>().Allocator(new HeapAllocator<ButtonState>());
		appAssembler.Register<IAppCommandSource, RuntimeAppCommandSource>().Allocator(new HeapAllocator<RuntimeAppCommandSource>()).Binding(Binding.Scope);
		appAssembler.Register<AppCommandJournal>().Allocator(new HeapAllocator<AppCommandJournal>()).Binding(Binding.Scope);
		appAssembler.Register<TickAppCommand>().Allocator(new ObjectPool<TickAppCommand>
		{
			InitialSize = 1,
			BlockSize = 60
		});
		appAssembler.Register<ProcessInputEventCommand>().Allocator(new ObjectPool<ProcessInputEventCommand>
		{
			InitialSize = 10,
			BlockSize = 100
		});
		appAssembler.Register<ConfigureDeviceCommand>().Allocator(new ObjectPool<ConfigureDeviceCommand>
		{
			InitialSize = 1
		});
		appAssembler.Register<InitRandomCommand>().Allocator(new ObjectPool<InitRandomCommand>
		{
			InitialSize = 1
		});
		appAssembler.Register<ChangeWindowFocusCommand>().Allocator(new ObjectPool<ChangeWindowFocusCommand>
		{
			InitialSize = 1
		});
		appAssembler.Register<PlayerActionController>().Allocator(new HeapAllocator<PlayerActionController>()).Binding(Binding.Scope);
		appAssembler.Register<PlayerActionGroup>().Allocator(new ObjectPool<PlayerActionGroup>
		{
			InitialSize = 10
		});
		appAssembler.Register<ScreenStack>().Allocator(new HeapAllocator<ScreenStack>()).Binding(Binding.Scope);
		appAssembler.Register<InGameMessageUIManager>().Allocator(new HeapAllocator<InGameMessageUIManager>()).Binding(Binding.Scope);
		appAssembler.Register<InGameMessageService>().Allocator(new HeapAllocator<InGameMessageService>()).Binding(Binding.Scope);
		appAssembler.Register<PopupParent>().Allocator(new GameObjectPool<PopupParent>("core", "PopupParent")
		{
			InitialSize = 1,
			GrowthStrategy = GrowthStrategy.OnDemand
		}).Binding(Binding.Scope);
		appAssembler.Register<PopupStack>().Allocator(new HeapAllocator<PopupStack>()).Binding(Binding.Scope);
		appAssembler.Register<NewContentIndicator>().Allocator(new GameObjectPool<NewContentIndicator>("core", "NewContentIndicator")
		{
			InitialSize = 5,
			GrowthStrategy = GrowthStrategy.Block,
			BlockSize = 5
		});
		GameCamera gameCamera = UnityEngine.Object.FindObjectOfType<GameCamera>();
		if (Diagnostics.Verify(gameCamera != null, "Unable to find GameCamera."))
		{
			appAssembler.Register<GameCamera>().Allocator(new SingletonAllocator<GameCamera>(gameCamera)).Binding(Binding.Scope);
		}
		AchievementDatabase achievementDatabase = AssetBundleUtility.LoadAsset<AchievementDatabase>("core", "AchievementDatabase");
		appAssembler.Register<AchievementDatabase, AchievementDatabase>().Allocator(new SingletonAllocator<AchievementDatabase>(achievementDatabase)).Binding(Binding.Scope);
		appAssembler.Register<MenuNavigationAction>().Allocator(new ObjectPool<MenuNavigationAction>
		{
			InitialSize = 5
		});
		appAssembler.Register<PermanenceZoneTextureLibrary>().Allocator(new GameObjectAllocator<PermanenceZoneTextureLibrary>("core", "PermanenceZoneTextureLibrary")).Binding(Binding.Scope);
		appAssembler.Register<NotificationScheduler>().Allocator(new HeapAllocator<NotificationScheduler>()).Binding(Binding.Scope);
		if (Application.isPlaying && FeatureToggle.IsFeatureEnabled(Feature.OnScreenDebugTools))
		{
			OnScreenDebugToolsActivator debugToolsActivator = new GameObject("DebugToolActivator").AddComponent<OnScreenDebugToolsActivator>();
			appAssembler.Register<OnScreenDebugToolsActivator>().Allocator(new SingletonAllocator<OnScreenDebugToolsActivator>(debugToolsActivator)).Binding(Binding.Scope);
			appAssembler.Register<IDebugRenderSetManager, DebugRenderSetManager>().Allocator(new SingletonAllocator<DebugRenderSetManager>(new DebugRenderSetManager())).Binding(Binding.Scope);
			OnScreenToolManager toolManager = new GameObject("OnScreenToolManager").AddComponent<OnScreenToolManager>();
			toolManager.Initialize(debugToolsActivator);
			appAssembler.Register<IOnScreenToolManager, OnScreenToolManager>().Allocator(new SingletonAllocator<OnScreenToolManager>(toolManager)).Binding(Binding.Scope);
			appAssembler.Register<OnScreenDebugStorage>().Allocator(new SingletonAllocator<OnScreenDebugStorage>(new OnScreenDebugStorage())).Binding(Binding.Scope);
		}
		else
		{
			appAssembler.Register<IDebugRenderSetManager, NullDebugRenderSetManager>().Allocator(new SingletonAllocator<NullDebugRenderSetManager>(new NullDebugRenderSetManager())).Binding(Binding.Scope);
			appAssembler.Register<IOnScreenToolManager, NullOnScreenToolManager>().Allocator(new SingletonAllocator<NullOnScreenToolManager>(new NullOnScreenToolManager())).Binding(Binding.Scope);
		}
		AppContainer.Environment.PopulateAppAssembler(appAssembler);
		return appAssembler;
	}

	// Token: 0x060000DB RID: 219
	protected abstract Assembler CreateGameAssembler(Assembler appAssembler);

	// Token: 0x060000DC RID: 220 RVA: 0x00004590 File Offset: 0x00002790
	private IEnvironment CreateDefaultEnvironment()
	{
		IEnvironment environment = new WindowsSteamEnvironment();
		if (FeatureToggle.IsFeatureEnabled(Feature.MockPhone))
		{
			environment = new MockEnvironment(environment);
		}
		if (Diagnostics.Verify(environment != null, "We didn't get a default environment for this given platform and variant combination. Is this a new platform or variant?"))
		{
			AppContainer.Log.Info("Using {0} for the current platform and variant.", new object[]
			{
				environment.GetType().ToString()
			});
		}
		return environment;
	}

	// Token: 0x04000048 RID: 72
	protected static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("AppContainer");

	// Token: 0x0400004E RID: 78
	private IAppCommandSource _commandSource;
}
