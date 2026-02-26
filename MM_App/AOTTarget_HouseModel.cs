using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200002F RID: 47
public static class AOTTarget_HouseModel
{
	// Token: 0x0600007B RID: 123 RVA: 0x000036B8 File Offset: 0x000018B8
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<HouseModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<HouseModel, TileModel>();
	}
}
