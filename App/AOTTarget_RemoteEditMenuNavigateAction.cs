using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000060 RID: 96
public static class AOTTarget_RemoteEditMenuNavigateAction
{
	// Token: 0x060000AC RID: 172 RVA: 0x0000383C File Offset: 0x00001A3C
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RemoteEditMenuNavigateAction, IScope>();
	}
}
