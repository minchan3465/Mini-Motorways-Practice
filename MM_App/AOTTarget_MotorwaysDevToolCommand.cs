using System;
using Factory;

// Token: 0x02000068 RID: 104
public static class AOTTarget_MotorwaysDevToolCommand
{
	// Token: 0x060000B4 RID: 180 RVA: 0x00003874 File Offset: 0x00001A74
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysDevToolCommand, IScope>();
	}
}
