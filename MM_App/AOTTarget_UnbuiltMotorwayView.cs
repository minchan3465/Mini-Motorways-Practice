using System;
using Factory;
using Motorways;
using Motorways.Views;

// Token: 0x02000043 RID: 67
public static class AOTTarget_UnbuiltMotorwayView
{
	// Token: 0x0600008F RID: 143 RVA: 0x00003771 File Offset: 0x00001971
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<UnbuiltMotorwayView, City>();
	}
}
