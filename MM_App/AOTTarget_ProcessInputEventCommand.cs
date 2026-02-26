using System;
using Factory;

// Token: 0x02000012 RID: 18
public static class AOTTarget_ProcessInputEventCommand
{
	// Token: 0x0600005E RID: 94 RVA: 0x00003593 File Offset: 0x00001793
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ProcessInputEventCommand, float>();
	}
}
