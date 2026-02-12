using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200003D RID: 61
public static class AOTTarget_BoatModel
{
	// Token: 0x06000089 RID: 137 RVA: 0x00003747 File Offset: 0x00001947
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<BoatModel, Clock>();
	}
}
