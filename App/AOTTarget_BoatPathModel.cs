using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200003B RID: 59
public static class AOTTarget_BoatPathModel
{
	// Token: 0x06000087 RID: 135 RVA: 0x00003734 File Offset: 0x00001934
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<BoatPathModel, Clock>();
	}
}
