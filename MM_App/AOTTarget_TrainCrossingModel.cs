using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000035 RID: 53
public static class AOTTarget_TrainCrossingModel
{
	// Token: 0x06000081 RID: 129 RVA: 0x000036FB File Offset: 0x000018FB
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TrainCrossingModel, Clock>();
	}
}
