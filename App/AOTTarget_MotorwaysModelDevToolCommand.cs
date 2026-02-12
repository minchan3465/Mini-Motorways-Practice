using System;
using Factory;

// Token: 0x02000069 RID: 105
public static class AOTTarget_MotorwaysModelDevToolCommand
{
	// Token: 0x060000B5 RID: 181 RVA: 0x0000387B File Offset: 0x00001A7B
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysModelDevToolCommand, IScope>();
	}
}
