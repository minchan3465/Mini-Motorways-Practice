using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000036 RID: 54
public static class AOTTarget_AnchoredMessageModel
{
	// Token: 0x06000082 RID: 130 RVA: 0x00003702 File Offset: 0x00001902
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<AnchoredMessageModel, Clock>();
	}
}
