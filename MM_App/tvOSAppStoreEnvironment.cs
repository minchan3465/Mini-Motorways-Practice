using System;
using System.Collections.Generic;
using Factory;
using Factory.Allocators;
using Helpers.GameCenter;
using Motorways.Leaderboards;
using Motorways.Leaderboards.Backends;
using Notifications;
using Notifications.Services;
using NotificationService;
using NotificationService.Persistence;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class tvOSAppStoreEnvironment : IEnvironment
{
	// Token: 0x060006E6 RID: 1766 RVA: 0x00016868 File Offset: 0x00014A68
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<DisableUITouchInputOverride>();
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00016870 File Offset: 0x00014A70
	public void PopulateAppAssembler(Assembler baseAssembler)
	{
		baseAssembler.Register<IGameCenterAuthentication, GameCenterAuthentication>().Allocator(new HeapAllocator<GameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAccessPoint, GameCenterAccessPoint>().Allocator(new HeapAllocator<GameCenterAccessPoint>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, GameCenterAchievementHandler>().Allocator(new HeapAllocator<GameCenterAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, GameCenterLeaderboardBackend>().Allocator(new HeapAllocator<GameCenterLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, GameCenterHistogramBackend>().Allocator(new HeapAllocator<GameCenterHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IContentProfile, RetailContentProfile>().Allocator(new HeapAllocator<RetailContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<IHardwareCapabilities, tvOSHardwareCapabilities>().Allocator(new HeapAllocator<tvOSHardwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IReachability, AppleReachability>().Allocator(new HeapAllocator<AppleReachability>()).Binding(Binding.Scope);
		baseAssembler.Register<iCloudKernel>().Allocator(new SingletonAllocator<iCloudKernel>(iCloudKernel.Instance)).Binding(Binding.Scope);
		baseAssembler.Register<IiCloudCache, iCloudUserDefaultsCache>().Allocator(new HeapAllocator<iCloudUserDefaultsCache>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, iCloudStorage>().Allocator(new HeapAllocator<iCloudStorage>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, tvOSSoftwareCapabilities>().Allocator(new HeapAllocator<tvOSSoftwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IControllerButtonToSymbolService, AppleSfSymbolService>().Allocator(new HeapAllocator<AppleSfSymbolService>()).Binding(Binding.Scope);
		baseAssembler.Register<ISystemNotificationService, NullSystemNotificationService>().Allocator(new HeapAllocator<NullSystemNotificationService>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventSystem, NullNotificationEventSystem>().Allocator(new HeapAllocator<NullNotificationEventSystem>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventPersistence, NullNotificationEventPersistence>().Allocator(new HeapAllocator<NullNotificationEventPersistence>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationScheduleDebugger, NullNotificationScheduleDebugger>().Allocator(new HeapAllocator<NullNotificationScheduleDebugger>()).Binding(Binding.Scope);
		baseAssembler.Register<IFileSystem, DefaultFileSystem>().Allocator(new HeapAllocator<DefaultFileSystem>()).Binding(Binding.Scope);
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x000022F5 File Offset: 0x000004F5
	public void PopulateGameAssembler(Assembler baseAssembler)
	{
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x060006E9 RID: 1769 RVA: 0x0000EFC6 File Offset: 0x0000D1C6
	public DeviceCategory DeviceCategory
	{
		get
		{
			return DeviceCategory.Console;
		}
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x060006EA RID: 1770 RVA: 0x00016A20 File Offset: 0x00014C20
	public List<string> FeatureConfigs
	{
		get
		{
			return new List<string>
			{
				"ArcadeConfig",
				"TvOSConfig"
			};
		}
	}
}
