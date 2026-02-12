using System;
using System.Collections.Generic;
using Factory;
using Factory.Allocators;
using Helpers.GameCenter;
using Motorways.Leaderboards;
using Motorways.Leaderboards.Backends;
using Notifications;
using Notifications.Services;
using NotificationService.Persistence;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class iOSAppStoreEnvironment : IEnvironment
{
	// Token: 0x060006C6 RID: 1734 RVA: 0x00011C09 File Offset: 0x0000FE09
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<BaseInputOverride>();
	}

	// Token: 0x060006C7 RID: 1735 RVA: 0x000160D0 File Offset: 0x000142D0
	public void PopulateAppAssembler(Assembler baseAssembler)
	{
		baseAssembler.Register<IGameCenterAuthentication, GameCenterAuthentication>().Allocator(new HeapAllocator<GameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAccessPoint, GameCenterAccessPoint>().Allocator(new HeapAllocator<GameCenterAccessPoint>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, GameCenterAchievementHandler>().Allocator(new HeapAllocator<GameCenterAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, GameCenterLeaderboardBackend>().Allocator(new HeapAllocator<GameCenterLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, GameCenterHistogramBackend>().Allocator(new HeapAllocator<GameCenterHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<iCloudKernel>().Allocator(new SingletonAllocator<iCloudKernel>(iCloudKernel.Instance)).Binding(Binding.Scope);
		baseAssembler.Register<IiCloudCache, iCloudFileCache>().Allocator(new HeapAllocator<iCloudFileCache>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, iCloudStorage>().Allocator(new HeapAllocator<iCloudStorage>()).Binding(Binding.Scope);
		baseAssembler.Register<IContentProfile, RetailContentProfile>().Allocator(new HeapAllocator<RetailContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<IHardwareCapabilities, iOSHardwareCapabilities>().Allocator(new HeapAllocator<iOSHardwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IReachability, AppleReachability>().Allocator(new HeapAllocator<AppleReachability>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, iOSSoftwareCapabilities>().Allocator(new HeapAllocator<iOSSoftwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IControllerButtonToSymbolService, AppleSfSymbolService>().Allocator(new HeapAllocator<AppleSfSymbolService>()).Binding(Binding.Scope);
		baseAssembler.Register<ISystemNotificationService, iOSSystemNotificationService>().Allocator(new HeapAllocator<iOSSystemNotificationService>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventSystem, NotificationEventSystem>().Allocator(new HeapAllocator<NotificationEventSystem>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventPersistence, ActivePlayerNotificationEventPersistence>().Allocator(new HeapAllocator<ActivePlayerNotificationEventPersistence>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationScheduleDebugger, NullNotificationScheduleDebugger>().Allocator(new HeapAllocator<NullNotificationScheduleDebugger>()).Binding(Binding.Scope);
		baseAssembler.Register<IFileSystem, DefaultFileSystem>().Allocator(new HeapAllocator<DefaultFileSystem>()).Binding(Binding.Scope);
	}

	// Token: 0x060006C8 RID: 1736 RVA: 0x000022F5 File Offset: 0x000004F5
	public void PopulateGameAssembler(Assembler baseAssembler)
	{
	}

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x060006C9 RID: 1737 RVA: 0x00016280 File Offset: 0x00014480
	public DeviceCategory DeviceCategory
	{
		get
		{
			if (!SystemInfo.deviceModel.StartsWith("iPad"))
			{
				return DeviceCategory.Phone;
			}
			return DeviceCategory.Tablet;
		}
	}

	// Token: 0x17000186 RID: 390
	// (get) Token: 0x060006CA RID: 1738 RVA: 0x00016296 File Offset: 0x00014496
	public List<string> FeatureConfigs
	{
		get
		{
			return new List<string>
			{
				"ArcadeConfig"
			};
		}
	}
}
