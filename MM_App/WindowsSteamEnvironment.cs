using System;
using System.Collections.Generic;
using Factory;
using Factory.Allocators;
using Helpers.GameCenter;
using Motorways.Leaderboards;
using Motorways.Leaderboards.Backends;
using SoftwareCapabilities;

// Token: 0x02000130 RID: 304
public class WindowsSteamEnvironment : WindowsBaseEnvironment
{
	// Token: 0x060006FA RID: 1786 RVA: 0x00016D50 File Offset: 0x00014F50
	public override void PopulateAppAssembler(Assembler baseAssembler)
	{
		base.PopulateAppAssembler(baseAssembler);
		baseAssembler.Register<IGameCenterAuthentication, NullGameCenterAuthentication>().Allocator(new HeapAllocator<NullGameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, SteamworksLeaderboardBackend>().Allocator(new HeapAllocator<SteamworksLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, SteamHistogramBackend>().Allocator(new HeapAllocator<SteamHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IContentProfile, RetailContentProfile>().Allocator(new HeapAllocator<RetailContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, SteamSoftwareCapabilities>().Allocator(new HeapAllocator<SteamSoftwareCapabilities>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, SteamworksAchievementHandler>().Allocator(new HeapAllocator<SteamworksAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, SteamCloud>().Allocator(new HeapAllocator<SteamCloud>()).Binding(Binding.Scope);
	}

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x060006FB RID: 1787 RVA: 0x000167DB File Offset: 0x000149DB
	public override List<string> FeatureConfigs
	{
		get
		{
			return new List<string>
			{
				"SteamConfig"
			};
		}
	}
}
