using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200005F RID: 95
public static class AOTTarget_ControllerEditMenuNavigateAction
{
	// Token: 0x060000AB RID: 171 RVA: 0x00003835 File Offset: 0x00001A35
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerEditMenuNavigateAction, IScope>();
	}
}
