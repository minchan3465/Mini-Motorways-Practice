using System;
using Factory;

// Token: 0x02000015 RID: 21
public static class AOTTarget_ChangeWindowFocusCommand
{
	// Token: 0x06000061 RID: 97 RVA: 0x000035A8 File Offset: 0x000017A8
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ChangeWindowFocusCommand, float>();
	}
}
