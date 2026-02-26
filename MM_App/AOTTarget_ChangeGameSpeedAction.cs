using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000048 RID: 72
public static class AOTTarget_ChangeGameSpeedAction
{
	// Token: 0x06000094 RID: 148 RVA: 0x00003794 File Offset: 0x00001994
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ChangeGameSpeedAction, IScope>();
	}
}
