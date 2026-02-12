using System;
using Factory;

// Token: 0x02000049 RID: 73
public static class AOTTarget_ChangeUpgradeBarAction
{
	// Token: 0x06000095 RID: 149 RVA: 0x0000379B File Offset: 0x0000199B
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ChangeUpgradeBarAction, IScope>();
	}
}
