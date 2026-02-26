using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200004B RID: 75
public static class AOTTarget_ToggleDragClearTileAction
{
	// Token: 0x06000097 RID: 151 RVA: 0x000037A9 File Offset: 0x000019A9
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ToggleDragClearTileAction, IScope>();
	}
}
