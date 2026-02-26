using System;
using Factory;
using FixMath;
using Server;

// Token: 0x0200001A RID: 26
public static class AOTTarget_Clock
{
	// Token: 0x06000066 RID: 102 RVA: 0x000035F3 File Offset: 0x000017F3
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Clock, int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Clock, Fix64>();
	}
}
