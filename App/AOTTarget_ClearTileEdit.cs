using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x0200001F RID: 31
public static class AOTTarget_ClearTileEdit
{
	// Token: 0x0600006B RID: 107 RVA: 0x00003625 File Offset: 0x00001825
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ClearTileEdit, Vector2Int>();
	}
}
