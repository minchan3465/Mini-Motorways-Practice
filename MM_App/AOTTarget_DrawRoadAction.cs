using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200004A RID: 74
public static class AOTTarget_DrawRoadAction
{
	// Token: 0x06000096 RID: 150 RVA: 0x000037A2 File Offset: 0x000019A2
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DrawRoadAction, IScope>();
	}
}
