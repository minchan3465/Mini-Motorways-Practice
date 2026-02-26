using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200002B RID: 43
public static class AOTTarget_ClockModel
{
	// Token: 0x06000077 RID: 119 RVA: 0x0000368D File Offset: 0x0000188D
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ClockModel, Clock>();
	}
}
