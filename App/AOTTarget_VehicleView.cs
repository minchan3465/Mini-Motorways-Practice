using System;
using Factory;
using Motorways;
using Motorways.Views;

// Token: 0x0200003E RID: 62
public static class AOTTarget_VehicleView
{
	// Token: 0x0600008A RID: 138 RVA: 0x0000374E File Offset: 0x0000194E
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<VehicleView, City>();
	}
}
