using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000055 RID: 85
public static class AOTTarget_ControllerDragTrafficLightAction
{
	// Token: 0x060000A1 RID: 161 RVA: 0x000037EF File Offset: 0x000019EF
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDragTrafficLightAction, IScope>();
	}
}
