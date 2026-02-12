using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000064 RID: 100
public static class AOTTarget_OpenElectiveUpgradeScreenAction
{
	// Token: 0x060000B0 RID: 176 RVA: 0x00003858 File Offset: 0x00001A58
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<OpenElectiveUpgradeScreenAction, IScope>();
	}
}
