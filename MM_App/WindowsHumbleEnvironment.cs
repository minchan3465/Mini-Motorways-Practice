using System;
using Factory;
using Factory.Allocators;

// Token: 0x0200012F RID: 303
public class WindowsHumbleEnvironment : WindowsBaseEnvironment
{
	// Token: 0x060006F8 RID: 1784 RVA: 0x00016D0F File Offset: 0x00014F0F
	public override void PopulateAppAssembler(Assembler baseAssembler)
	{
		base.PopulateAppAssembler(baseAssembler);
		baseAssembler.Register<IContentProfile, RetailContentProfile>().Allocator(new HeapAllocator<RetailContentProfile>()).Binding(Binding.Scope);
		baseAssembler.Register<ISoftwareCapabilities, NullSoftwareCapabilities>().Allocator(new HeapAllocator<NullSoftwareCapabilities>()).Binding(Binding.Scope);
	}
}
