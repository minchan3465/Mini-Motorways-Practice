using System;
using System.Collections.Generic;
using Factory;
using Factory.Allocators;
using Helpers.GameCenter;
using Motorways.Leaderboards;
using Motorways.Leaderboards.Backends;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class NullEnvironment : IEnvironment
{
	// Token: 0x06000515 RID: 1301 RVA: 0x00011C09 File Offset: 0x0000FE09
	public BaseInputOverride AddInputOverrideToGameObject(GameObject gameObject)
	{
		return gameObject.AddComponent<BaseInputOverride>();
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x00011C14 File Offset: 0x0000FE14
	public virtual void PopulateAppAssembler(Assembler baseAssembler)
	{
		baseAssembler.Register<IGameCenterAuthentication, NullGameCenterAuthentication>().Allocator(new HeapAllocator<NullGameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAccessPoint, NullGameCenterAccessPoint>().Allocator(new HeapAllocator<NullGameCenterAccessPoint>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, NullAchievementHandler>().Allocator(new HeapAllocator<NullAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, NullLeaderboardBackend>().Allocator(new HeapAllocator<NullLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, NullHistogramBackend>().Allocator(new HeapAllocator<NullHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, NullStorage>().Allocator(new HeapAllocator<NullStorage>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, NullAchievementHandler>().Allocator(new HeapAllocator<NullAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<IContentProfile, NullContentProfile>().Allocator(new HeapAllocator<NullContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<IHardwareCapabilities, NullHardwareCapabilities>().Allocator(new HeapAllocator<NullHardwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IReachability, NullReachability>().Allocator(new HeapAllocator<NullReachability>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, NullSoftwareCapabilities>().Allocator(new HeapAllocator<NullSoftwareCapabilities>()).Binding(Binding.Scope);
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void PopulateGameAssembler(Assembler baseAssembler)
	{
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000518 RID: 1304 RVA: 0x0000222C File Offset: 0x0000042C
	public DeviceCategory DeviceCategory
	{
		get
		{
			return DeviceCategory.Desktop;
		}
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000519 RID: 1305 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public List<string> FeatureConfigs
	{
		get
		{
			return null;
		}
	}
}
