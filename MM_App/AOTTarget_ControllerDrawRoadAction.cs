using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200004F RID: 79
public static class AOTTarget_ControllerDrawRoadAction
{
	// Token: 0x0600009B RID: 155 RVA: 0x000037C5 File Offset: 0x000019C5
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerDrawRoadAction, IScope>();
	}
}
