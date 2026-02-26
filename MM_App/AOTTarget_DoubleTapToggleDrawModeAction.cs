using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000046 RID: 70
public static class AOTTarget_DoubleTapToggleDrawModeAction
{
	// Token: 0x06000092 RID: 146 RVA: 0x00003786 File Offset: 0x00001986
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DoubleTapToggleDrawModeAction, IScope>();
	}
}
