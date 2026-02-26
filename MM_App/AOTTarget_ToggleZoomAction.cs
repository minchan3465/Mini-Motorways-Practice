using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000062 RID: 98
public static class AOTTarget_ToggleZoomAction
{
	// Token: 0x060000AE RID: 174 RVA: 0x0000384A File Offset: 0x00001A4A
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ToggleZoomAction, IScope>();
	}
}
