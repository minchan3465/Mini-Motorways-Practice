using System;
using Factory;

// Token: 0x02000011 RID: 17
public static class AOTTarget_TickAppCommand
{
	// Token: 0x0600005D RID: 93 RVA: 0x0000358C File Offset: 0x0000178C
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TickAppCommand, float>();
	}
}
