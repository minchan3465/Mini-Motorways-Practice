using System;
using Factory;

// Token: 0x02000014 RID: 20
public static class AOTTarget_InitRandomCommand
{
	// Token: 0x06000060 RID: 96 RVA: 0x000035A1 File Offset: 0x000017A1
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<InitRandomCommand, float>();
	}
}
