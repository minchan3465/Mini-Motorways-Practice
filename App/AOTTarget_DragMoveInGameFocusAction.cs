using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200004E RID: 78
public static class AOTTarget_DragMoveInGameFocusAction
{
	// Token: 0x0600009A RID: 154 RVA: 0x000037BE File Offset: 0x000019BE
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragMoveInGameFocusAction, IScope>();
	}
}
