using System;
using Factory;
using FixMath;
using Motorways.Models;

// Token: 0x02000026 RID: 38
public static class AOTTarget_CityPlanModel
{
	// Token: 0x06000072 RID: 114 RVA: 0x0000365B File Offset: 0x0000185B
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<CityPlanModel, Fix64>();
	}
}
