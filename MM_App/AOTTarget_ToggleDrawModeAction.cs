using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000045 RID: 69
public static class AOTTarget_ToggleDrawModeAction
{
	// Token: 0x06000091 RID: 145 RVA: 0x0000377F File Offset: 0x0000197F
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ToggleDrawModeAction, IScope>();
	}
}
