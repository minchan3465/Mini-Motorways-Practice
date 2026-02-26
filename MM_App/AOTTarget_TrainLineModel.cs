using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000038 RID: 56
public static class AOTTarget_TrainLineModel
{
	// Token: 0x06000084 RID: 132 RVA: 0x0000371A File Offset: 0x0000191A
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TrainLineModel, Clock>();
	}
}
