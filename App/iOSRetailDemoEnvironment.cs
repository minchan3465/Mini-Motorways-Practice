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

// Token: 0x02000126 RID: 294
public class iOSRetailDemoEnvironment : IEnvironment
{
	// Token: 0x060006CC RID: 1740 RVA: 0x00011C09 File Offset: 0x0000FE09
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<BaseInputOverride>();
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x000162A8 File Offset: 0x000144A8
	public void PopulateAppAssembler(Assembler baseAssembler)
	{
		baseAssembler.Register<IGameCenterAuthentication, NullGameCenterAuthentication>().Allocator(new HeapAllocator<NullGameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAccessPoint, NullGameCenterAccessPoint>().Allocator(new HeapAllocator<NullGameCenterAccessPoint>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, NullAchievementHandler>().Allocator(new HeapAllocator<NullAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, RetailDemoLeaderboardBackend>().Allocator(new HeapAllocator<RetailDemoLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, MockHistogramBackend>().Allocator(new HeapAllocator<MockHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IContentProfile, DemoContentProfile>().Allocator(new HeapAllocator<DemoContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<IHardwareCapabilities, iOSHardwareCapabilities>().Allocator(new HeapAllocator<iOSHardwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IReachability, NullReachability>().Allocator(new HeapAllocator<NullReachability>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, iOSDemoSoftwareCapabilities>().Allocator(new HeapAllocator<iOSDemoSoftwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, NullStorage>().Allocator(new HeapAllocator<NullStorage>()).Binding(Binding.Scope);
		baseAssembler.Register<IControllerButtonToSymbolService, AppleSfSymbolService>().Allocator(new HeapAllocator<AppleSfSymbolService>()).Binding(Binding.Scope);
		baseAssembler.Register<ISystemNotificationService, NullSystemNotificationService>().Allocator(new HeapAllocator<NullSystemNotificationService>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventSystem, NullNotificationEventSystem>().Allocator(new HeapAllocator<NullNotificationEventSystem>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventPersistence, NullNotificationEventPersistence>().Allocator(new HeapAllocator<NullNotificationEventPersistence>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationScheduleDebugger, NullNotificationScheduleDebugger>().Allocator(new HeapAllocator<NullNotificationScheduleDebugger>()).Binding(Binding.Scope);
		baseAssembler.Register<IFileSystem, DefaultFileSystem>().Allocator(new HeapAllocator<DefaultFileSystem>()).Binding(Binding.Scope);
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x000022F5 File Offset: 0x000004F5
	public void PopulateGameAssembler(Assembler baseAssembler)
	{
	}

	// Token: 0x17000187 RID: 391
	// (get) Token: 0x060006CF RID: 1743 RVA: 0x00016280 File Offset: 0x00014480
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

	// Token: 0x17000188 RID: 392
	// (get) Token: 0x060006D0 RID: 1744 RVA: 0x00016425 File Offset: 0x00014625
	public List<string> FeatureConfigs
	{
		get
		{
			return new List<string>
			{
				"ArcadeConfig",
				"DemoConfig"
			};
		}
	}
}
