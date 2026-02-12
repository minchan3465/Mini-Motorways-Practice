using System;
using Factory;

// Token: 0x02000016 RID: 22
public static class AOTTarget_MenuNavigationAction
{
	// Token: 0x06000062 RID: 98 RVA: 0x000035AF File Offset: 0x000017AF
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MenuNavigationAction, IScope>();
	}
}
