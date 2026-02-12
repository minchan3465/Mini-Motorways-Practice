using System;
using System.Collections.Generic;
using Factory;
using Factory.Allocators;
using Helpers.GameCenter;
using Motorways.Leaderboards;
using Motorways.Leaderboards.Backends;
using SoftwareCapabilities;

// Token: 0x0200012A RID: 298
public class macOSSteamEnvironment : macOSBaseEnvironment
{
	// Token: 0x060006DD RID: 1757 RVA: 0x000166F8 File Offset: 0x000148F8
	public override void PopulateAppAssembler(Assembler baseAssembler)
	{
		base.PopulateAppAssembler(baseAssembler);
		baseAssembler.Register<IGameCenterAuthentication, NullGameCenterAuthentication>().Allocator(new HeapAllocator<NullGameCenterAuthentication>()).Binding(Binding.Scope);
		baseAssembler.Register<IControllerButtonToSymbolService, DefaultControllerButtonToSymbolService>().Allocator(new HeapAllocator<DefaultControllerButtonToSymbolService>()).Binding(Binding.Scope);
		baseAssembler.Register<IGameCenterAccessPoint, NullGameCenterAccessPoint>().Allocator(new HeapAllocator<NullGameCenterAccessPoint>()).Binding(Binding.Scope);
		baseAssembler.Register<IAchievementHandler, SteamworksAchievementHandler>().Allocator(new HeapAllocator<SteamworksAchievementHandler>()).Binding(Binding.Scope);
		baseAssembler.Register<ILeaderboardBackend, SteamworksLeaderboardBackend>().Allocator(new HeapAllocator<SteamworksLeaderboardBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IHistogramBackend, SteamHistogramBackend>().Allocator(new HeapAllocator<SteamHistogramBackend>()).Binding(Binding.Scope);
		baseAssembler.Register<IContentProfile, RetailContentProfile>().Allocator(new HeapAllocator<RetailContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<IPersistentStorageProvider, SteamCloud>().Allocator(new HeapAllocator<SteamCloud>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, SteamSoftwareCapabilities>().Allocator(new HeapAllocator<SteamSoftwareCapabilities>()).Binding(Binding.Scope);
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x060006DE RID: 1758 RVA: 0x000167DB File Offset: 0x000149DB
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
