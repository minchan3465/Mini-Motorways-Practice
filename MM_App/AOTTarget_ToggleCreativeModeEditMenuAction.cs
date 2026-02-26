using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000063 RID: 99
public static class AOTTarget_ToggleCreativeModeEditMenuAction
{
	// Token: 0x060000AF RID: 175 RVA: 0x00003851 File Offset: 0x00001A51
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ToggleCreativeModeEditMenuAction, IScope>();
	}
}
