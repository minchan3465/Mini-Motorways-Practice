using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200003C RID: 60
public static class AOTTarget_BoatPathTileModel
{
	// Token: 0x06000088 RID: 136 RVA: 0x0000373B File Offset: 0x0000193B
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<BoatPathTileModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<BoatPathTileModel, TileModel>();
	}
}
