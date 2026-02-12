using System;
using System.Collections.Generic;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000031 RID: 49
public static class AOTTarget_CarparkModel
{
	// Token: 0x0600007D RID: 125 RVA: 0x000036D5 File Offset: 0x000018D5
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<CarparkModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<CarparkModel, List<TileModel>>();
	}
}
