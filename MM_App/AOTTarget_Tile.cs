using System;
using Factory;
using Motorways;
using Server;
using UnityEngine;

// Token: 0x02000029 RID: 41
public static class AOTTarget_Tile
{
	// Token: 0x06000075 RID: 117 RVA: 0x00003670 File Offset: 0x00001870
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Tile, ITilemap>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Tile, Vector2Int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Tile, TileContentType>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Tile, IModel>();
	}
}
