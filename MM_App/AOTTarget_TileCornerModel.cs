using System;
using Factory;
using Motorways.Models;
using Server;

// Token: 0x0200002A RID: 42
public static class AOTTarget_TileCornerModel
{
	// Token: 0x06000076 RID: 118 RVA: 0x00003686 File Offset: 0x00001886
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TileCornerModel, Clock>();
	}
}
