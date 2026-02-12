using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000047 RID: 71
public static class AOTTarget_PressUIFocusAction
{
	// Token: 0x06000093 RID: 147 RVA: 0x0000378D File Offset: 0x0000198D
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<PressUIFocusAction, IScope>();
	}
}
