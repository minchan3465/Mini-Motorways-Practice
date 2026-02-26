using System;
using Factory;
using Motorways.Actions;

// Token: 0x0200005B RID: 91
public static class AOTTarget_DragHouseAction
{
	// Token: 0x060000A7 RID: 167 RVA: 0x00003819 File Offset: 0x00001A19
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragHouseAction, IScope>();
	}
}
