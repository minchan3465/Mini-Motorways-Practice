using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200005C RID: 92
public static class AOTTarget_ControllerDragHouseAction
{
	// Token: 0x060000A8 RID: 168 RVA: 0x00003820 File Offset: 0x00001A20
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDragHouseAction, IScope>();
	}
}
