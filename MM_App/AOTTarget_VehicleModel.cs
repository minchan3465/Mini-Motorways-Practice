using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200002E RID: 46
public static class AOTTarget_VehicleModel
{
	// Token: 0x0600007A RID: 122 RVA: 0x000036B1 File Offset: 0x000018B1
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<VehicleModel, Clock>();
	}
}
