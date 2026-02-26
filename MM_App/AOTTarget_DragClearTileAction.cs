using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200004C RID: 76
public static class AOTTarget_DragClearTileAction
{
	// Token: 0x06000098 RID: 152 RVA: 0x000037B0 File Offset: 0x000019B0
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragClearTileAction, IScope>();
	}
}
