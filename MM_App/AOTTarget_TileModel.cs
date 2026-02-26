using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000028 RID: 40
public static class AOTTarget_TileModel
{
	// Token: 0x06000074 RID: 116 RVA: 0x00003669 File Offset: 0x00001869
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TileModel, Clock>();
	}
}
