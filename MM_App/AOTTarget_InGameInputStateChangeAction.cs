using System;
using Factory;

// Token: 0x02000018 RID: 24
public static class AOTTarget_InGameInputStateChangeAction
{
	// Token: 0x06000064 RID: 100 RVA: 0x000035DB File Offset: 0x000017DB
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<InGameInputStateChangeAction, IScope>();
	}
}
