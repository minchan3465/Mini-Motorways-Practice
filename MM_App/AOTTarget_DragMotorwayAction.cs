using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000050 RID: 80
public static class AOTTarget_DragMotorwayAction
{
	// Token: 0x0600009C RID: 156 RVA: 0x000037CC File Offset: 0x000019CC
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragMotorwayAction, IScope>();
	}
}
