using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000059 RID: 89
public static class AOTTarget_ControllerDragMotorwayHandleAction
{
	// Token: 0x060000A5 RID: 165 RVA: 0x0000380B File Offset: 0x00001A0B
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDragMotorwayHandleAction, IScope>();
	}
}
