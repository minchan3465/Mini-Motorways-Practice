using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000051 RID: 81
public static class AOTTarget_ControllerDragMotorwayAction
{
	// Token: 0x0600009D RID: 157 RVA: 0x000037D3 File Offset: 0x000019D3
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDragMotorwayAction, IScope>();
	}
}
