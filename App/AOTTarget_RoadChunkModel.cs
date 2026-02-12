using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000033 RID: 51
public static class AOTTarget_RoadChunkModel
{
	// Token: 0x0600007F RID: 127 RVA: 0x000036E8 File Offset: 0x000018E8
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RoadChunkModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RoadChunkModel, TrainCrossingModel>();
	}
}
