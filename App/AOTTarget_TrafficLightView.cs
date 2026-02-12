using System;
using Factory;
using Motorways;
using Motorways.Views;

// Token: 0x02000042 RID: 66
public static class AOTTarget_TrafficLightView
{
	// Token: 0x0600008E RID: 142 RVA: 0x0000376A File Offset: 0x0000196A
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TrafficLightView, City>();
	}
}
