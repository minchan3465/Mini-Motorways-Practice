using System;
using Factory;

// Token: 0x02000013 RID: 19
public static class AOTTarget_ConfigureDeviceCommand
{
	// Token: 0x0600005F RID: 95 RVA: 0x0000359A File Offset: 0x0000179A
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ConfigureDeviceCommand, float>();
	}
}
