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

// Token: 0x0200012E RID: 302
public abstract class WindowsBaseEnvironment : IEnvironment
{
	// Token: 0x060006F2 RID: 1778 RVA: 0x00011C09 File Offset: 0x0000FE09
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<BaseInputOverride>();
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00016BC0 File Offset: 0x00014DC0
	public virtual void PopulateAppAssembler(Assembler baseAssembler)
	{
		baseAssembler.Register<IHardwareCapabilities, DesktopHardwareCapabilities>().Allocator(new HeapAllocator<DesktopHardwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, LocalFileStorage>().Allocator(new HeapAllocator<LocalFileStorage>()).Binding(Binding.Scope);
		baseAssembler.Register<IReachability, NullReachability>().Allocator(new HeapAllocator<NullReachability>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAuthentication, NullGameCenterAuthentication>().Allocator(new HeapAllocator<NullGameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAccessPoint, NullGameCenterAccessPoint>().Allocator(new HeapAllocator<NullGameCenterAccessPoint>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, NullAchievementHandler>().Allocator(new HeapAllocator<NullAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, TestLeaderboardBackend>().Allocator(new HeapAllocator<TestLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, SteamHistogramBackend>().Allocator(new HeapAllocator<SteamHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IControllerButtonToSymbolService, DefaultControllerButtonToSymbolService>().Allocator(new HeapAllocator<DefaultControllerButtonToSymbolService>()).Binding(Binding.Scope);
		baseAssembler.Register<ISystemNotificationService, NullSystemNotificationService>().Allocator(new HeapAllocator<NullSystemNotificationService>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventSystem, NullNotificationEventSystem>().Allocator(new HeapAllocator<NullNotificationEventSystem>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventPersistence, NullNotificationEventPersistence>().Allocator(new HeapAllocator<NullNotificationEventPersistence>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationScheduleDebugger, NullNotificationScheduleDebugger>().Allocator(new HeapAllocator<NullNotificationScheduleDebugger>()).Binding(Binding.Scope);
		baseAssembler.Register<IFileSystem, DefaultFileSystem>().Allocator(new HeapAllocator<DefaultFileSystem>()).Binding(Binding.Scope);
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x000022F5 File Offset: 0x000004F5
	public void PopulateGameAssembler(Assembler baseAssembler)
	{
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x060006F5 RID: 1781 RVA: 0x0000222C File Offset: 0x0000042C
	public DeviceCategory DeviceCategory
	{
		get
		{
			return DeviceCategory.Desktop;
		}
	}

	// Token: 0x17000194 RID: 404
	// (get) Token: 0x060006F6 RID: 1782 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public virtual List<string> FeatureConfigs
	{
		get
		{
			return null;
		}
	}
}
