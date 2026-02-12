using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000044 RID: 68
public static class AOTTarget_AdvanceTutorialAction
{
	// Token: 0x06000090 RID: 144 RVA: 0x00003778 File Offset: 0x00001978
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<AdvanceTutorialAction, IScope>();
	}
}
