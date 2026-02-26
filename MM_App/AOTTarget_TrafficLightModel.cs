using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000034 RID: 52
public static class AOTTarget_TrafficLightModel
{
	// Token: 0x06000080 RID: 128 RVA: 0x000036F4 File Offset: 0x000018F4
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TrafficLightModel, Clock>();
	}
}
