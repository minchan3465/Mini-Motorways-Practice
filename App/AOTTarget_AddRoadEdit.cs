using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x0200001D RID: 29
public static class AOTTarget_AddRoadEdit
{
	// Token: 0x06000069 RID: 105 RVA: 0x0000360D File Offset: 0x0000180D
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<AddRoadEdit, Vector2Int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<AddRoadEdit, TileDirection>();
	}
}
