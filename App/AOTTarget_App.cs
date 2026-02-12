using System;
using Factory;

// Token: 0x02000010 RID: 16
public static class AOTTarget_App
{
	// Token: 0x0600005C RID: 92 RVA: 0x00003585 File Offset: 0x00001785
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<App, IScope>();
	}
}
