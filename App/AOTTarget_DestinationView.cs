using System;
using Factory;
using Motorways;
using Motorways.Views;

// Token: 0x02000040 RID: 64
public static class AOTTarget_DestinationView
{
	// Token: 0x0600008C RID: 140 RVA: 0x0000375C File Offset: 0x0000195C
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DestinationView, City>();
	}
}
