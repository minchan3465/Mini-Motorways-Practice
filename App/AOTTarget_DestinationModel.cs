using System;
using System.Collections.Generic;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000030 RID: 48
public static class AOTTarget_DestinationModel
{
	// Token: 0x0600007C RID: 124 RVA: 0x000036C4 File Offset: 0x000018C4
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DestinationModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DestinationModel, List<TileModel>>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DestinationModel, CarparkModel>();
	}
}
