using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000052 RID: 82
public static class AOTTarget_ControllerDragEditMotorwayAction
{
	// Token: 0x0600009E RID: 158 RVA: 0x000037DA File Offset: 0x000019DA
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDragEditMotorwayAction, IScope>();
	}
}
