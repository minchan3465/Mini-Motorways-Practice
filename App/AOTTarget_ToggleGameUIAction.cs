using System;
using Factory;

// Token: 0x02000065 RID: 101
public static class AOTTarget_ToggleGameUIAction
{
	// Token: 0x060000B1 RID: 177 RVA: 0x0000385F File Offset: 0x00001A5F
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ToggleGameUIAction, IScope>();
	}
}
