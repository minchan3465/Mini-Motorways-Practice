using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000054 RID: 84
public static class AOTTarget_DragTrafficLightAction
{
	// Token: 0x060000A0 RID: 160 RVA: 0x000037E8 File Offset: 0x000019E8
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragTrafficLightAction, IScope>();
	}
}
