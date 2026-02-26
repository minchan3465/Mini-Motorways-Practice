using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000056 RID: 86
public static class AOTTarget_DragRoundaboutAction
{
	// Token: 0x060000A2 RID: 162 RVA: 0x000037F6 File Offset: 0x000019F6
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragRoundaboutAction, IScope>();
	}
}
