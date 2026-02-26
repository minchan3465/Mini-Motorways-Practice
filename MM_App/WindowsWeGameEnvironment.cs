using System;
using Factory;
using Factory.Allocators;

// Token: 0x02000131 RID: 305
public class WindowsWeGameEnvironment : WindowsBaseEnvironment
{
	// Token: 0x060006FD RID: 1789 RVA: 0x00016E05 File Offset: 0x00015005
	public override void PopulateAppAssembler(Assembler baseAssembler)
	{
		base.PopulateAppAssembler(baseAssembler);
		baseAssembler.Register<IContentProfile, RetailContentProfile>().Allocator(new HeapAllocator<RetailContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, WeGameSoftwareCapabilities>().Allocator(new HeapAllocator<WeGameSoftwareCapabilities>()).Binding(Binding.Scope);
	}
}
