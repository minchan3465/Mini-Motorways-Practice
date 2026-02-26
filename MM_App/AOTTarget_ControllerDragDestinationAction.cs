using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200005E RID: 94
public static class AOTTarget_ControllerDragDestinationAction
{
	// Token: 0x060000AA RID: 170 RVA: 0x0000382E File Offset: 0x00001A2E
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDragDestinationAction, IScope>();
	}
}
