using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200005A RID: 90
public static class AOTTarget_DragEditMotorwayAction
{
	// Token: 0x060000A6 RID: 166 RVA: 0x00003812 File Offset: 0x00001A12
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragEditMotorwayAction, IScope>();
	}
}
