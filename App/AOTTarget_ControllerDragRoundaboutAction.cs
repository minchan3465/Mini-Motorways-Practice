using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000057 RID: 87
public static class AOTTarget_ControllerDragRoundaboutAction
{
	// Token: 0x060000A3 RID: 163 RVA: 0x000037FD File Offset: 0x000019FD
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDragRoundaboutAction, IScope>();
	}
}
