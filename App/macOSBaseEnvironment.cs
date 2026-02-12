using System;
using System.Collections.Generic;
using Factory;
using Factory.Allocators;
using Notifications;
using Notifications.Services;
using NotificationService;
using NotificationService.Persistence;
using UnityEngine;

// Token: 0x02000128 RID: 296
public abstract class macOSBaseEnvironment : IEnvironment
{
	// Token: 0x060006D5 RID: 1749 RVA: 0x00011C09 File Offset: 0x0000FE09
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<BaseInputOverride>();
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00016564 File Offset: 0x00014764
	public virtual void PopulateAppAssembler(Assembler baseAssembler)
	{
		baseAssembler.Register<IHardwareCapabilities, DesktopHardwareCapabilities>().Allocator(new HeapAllocator<DesktopHardwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IReachability, AppleReachability>().Allocator(new HeapAllocator<AppleReachability>()).Binding(Binding.Scope);
		baseAssembler.Register<ISystemNotificationService, NullSystemNotificationService>().Allocator(new HeapAllocator<NullSystemNotificationService>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventSystem, NullNotificationEventSystem>().Allocator(new HeapAllocator<NullNotificationEventSystem>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationEventPersistence, NullNotificationEventPersistence>().Allocator(new HeapAllocator<NullNotificationEventPersistence>()).Binding(Binding.Scope);
		baseAssembler.Register<INotificationScheduleDebugger, NullNotificationScheduleDebugger>().Allocator(new HeapAllocator<NullNotificationScheduleDebugger>()).Binding(Binding.Scope);
		baseAssembler.Register<IFileSystem, DefaultFileSystem>().Allocator(new HeapAllocator<DefaultFileSystem>()).Binding(Binding.Scope);
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void PopulateGameAssembler(Assembler baseAssembler)
	{
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x060006D8 RID: 1752 RVA: 0x0000222C File Offset: 0x0000042C
	public DeviceCategory DeviceCategory
	{
		get
		{
			return DeviceCategory.Desktop;
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x060006D9 RID: 1753 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public virtual List<string> FeatureConfigs
	{
		get
		{
			return null;
		}
	}
}
