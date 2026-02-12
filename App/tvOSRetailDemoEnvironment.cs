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

// Token: 0x0200012D RID: 301
public class tvOSRetailDemoEnvironment : IEnvironment
{
	// Token: 0x060006EC RID: 1772 RVA: 0x00016868 File Offset: 0x00014A68
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<DisableUITouchInputOverride>();
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00016A40 File Offset: 0x00014C40
	public void PopulateAppAssembler(Assembler baseAssembler)
	{
		baseAssembler.Register<IGameCenterAuthentication, NullGameCenterAuthentication>().Allocator(new HeapAllocator<NullGameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAccessPoint, NullGameCenterAccessPoint>().Allocator(new HeapAllocator<NullGameCenterAccessPoint>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, NullAchievementHandler>().Allocator(new HeapAllocator<NullAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, RetailDemoLeaderboardBackend>().Allocator(new HeapAllocator<RetailDemoLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, MockHistogramBackend>().Allocator(new HeapAllocator<MockHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IContentProfile, DemoContentProfile>().Allocator(new HeapAllocator<DemoContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<IHardwareCapabilities, tvOSHardwareCapabilities>().Allocator(new HeapAllocator<tvOSHardwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IReachability, NullReachability>().Allocator(new HeapAllocator<NullReachability>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, NullStorage>().Allocator(new HeapAllocator<NullStorage>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, tvOSDemoSoftwareCapabilities>().Allocator(new HeapAllocator<tvOSDemoSoftwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IControllerButtonToSymbolService, AppleSfSymbolService>().Allocator(new HeapAllocator<AppleSfSymbolService>()).Binding(Binding.Scope);
		baseAssembler.Register<ISystemNotificationService, NullSystemNotificationService>().Allocator(new HeapAllocator<NullSystemNotificationService>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventSystem, NullNotificationEventSystem>().Allocator(new HeapAllocator<NullNotificationEventSystem>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventPersistence, NullNotificationEventPersistence>().Allocator(new HeapAllocator<NullNotificationEventPersistence>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationScheduleDebugger, NullNotificationScheduleDebugger>().Allocator(new HeapAllocator<NullNotificationScheduleDebugger>()).Binding(Binding.Scope);
		baseAssembler.Register<IFileSystem, DefaultFileSystem>().Allocator(new HeapAllocator<DefaultFileSystem>()).Binding(Binding.Scope);
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x000022F5 File Offset: 0x000004F5
	public void PopulateGameAssembler(Assembler baseAssembler)
	{
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x060006EF RID: 1775 RVA: 0x0000EFC6 File Offset: 0x0000D1C6
	public DeviceCategory DeviceCategory
	{
		get
		{
			return DeviceCategory.Console;
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x060006F0 RID: 1776 RVA: 0x00016425 File Offset: 0x00014625
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
