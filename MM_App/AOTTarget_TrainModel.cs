using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200003A RID: 58
public static class AOTTarget_TrainModel
{
	// Token: 0x06000086 RID: 134 RVA: 0x0000372D File Offset: 0x0000192D
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TrainModel, Clock>();
	}
}
