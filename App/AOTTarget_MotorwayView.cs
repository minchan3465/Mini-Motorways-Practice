using System;
using Factory;
using Motorways;
using Motorways.Views;

// Token: 0x02000041 RID: 65
public static class AOTTarget_MotorwayView
{
	// Token: 0x0600008D RID: 141 RVA: 0x00003763 File Offset: 0x00001963
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwayView, City>();
	}
}
