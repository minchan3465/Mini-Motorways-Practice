using System;
using Factory;
using Motorways;

// Token: 0x0200000F RID: 15
public static class AOTTarget_ActivePlayer
{
	// Token: 0x0600005B RID: 91 RVA: 0x0000357E File Offset: 0x0000177E
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ActivePlayer, IScope>();
	}
}
