using System;
using Factory;
using Motorways;

// Token: 0x0200001B RID: 27
public static class AOTTarget_MotorwaysClient
{
	// Token: 0x06000067 RID: 103 RVA: 0x000035FF File Offset: 0x000017FF
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysClient, Scope>();
	}
}
