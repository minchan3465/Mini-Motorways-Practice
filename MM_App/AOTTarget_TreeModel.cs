using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000037 RID: 55
public static class AOTTarget_TreeModel
{
	// Token: 0x06000083 RID: 131 RVA: 0x00003709 File Offset: 0x00001909
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TreeModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TreeModel, int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TreeModel, TileModel>();
	}
}
