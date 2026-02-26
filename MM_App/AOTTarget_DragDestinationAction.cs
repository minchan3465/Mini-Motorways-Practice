using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200005D RID: 93
public static class AOTTarget_DragDestinationAction
{
	// Token: 0x060000A9 RID: 169 RVA: 0x00003827 File Offset: 0x00001A27
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragDestinationAction, IScope>();
	}
}
