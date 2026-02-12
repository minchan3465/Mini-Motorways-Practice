using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000027 RID: 39
public static class AOTTarget_TilemapModel
{
	// Token: 0x06000073 RID: 115 RVA: 0x00003662 File Offset: 0x00001862
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TilemapModel, Clock>();
	}
}
