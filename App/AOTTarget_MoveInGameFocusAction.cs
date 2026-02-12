using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200004D RID: 77
public static class AOTTarget_MoveInGameFocusAction
{
	// Token: 0x06000099 RID: 153 RVA: 0x000037B7 File Offset: 0x000019B7
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MoveInGameFocusAction, IScope>();
	}
}
