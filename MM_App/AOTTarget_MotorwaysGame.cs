using System;
using Factory;
using Motorways;

// Token: 0x0200006A RID: 106
public static class AOTTarget_MotorwaysGame
{
	// Token: 0x060000B6 RID: 182 RVA: 0x00003882 File Offset: 0x00001A82
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGame, IScope>();
	}
}
