using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000032 RID: 50
public static class AOTTarget_LaneModel
{
	// Token: 0x0600007E RID: 126 RVA: 0x000036E1 File Offset: 0x000018E1
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<LaneModel, Clock>();
	}
}
