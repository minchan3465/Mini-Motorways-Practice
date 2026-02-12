using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000058 RID: 88
public static class AOTTarget_DragMotorwayHandleAction
{
	// Token: 0x060000A4 RID: 164 RVA: 0x00003804 File Offset: 0x00001A04
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragMotorwayHandleAction, IScope>();
	}
}
