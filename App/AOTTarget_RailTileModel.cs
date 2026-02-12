using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x02000039 RID: 57
public static class AOTTarget_RailTileModel
{
	// Token: 0x06000085 RID: 133 RVA: 0x00003721 File Offset: 0x00001921
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RailTileModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RailTileModel, TileModel>();
	}
}
