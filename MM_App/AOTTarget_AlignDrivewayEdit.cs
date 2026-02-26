using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x0200001E RID: 30
public static class AOTTarget_AlignDrivewayEdit
{
	// Token: 0x0600006A RID: 106 RVA: 0x00003619 File Offset: 0x00001819
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<AlignDrivewayEdit, Vector2Int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<AlignDrivewayEdit, TileDirection>();
	}
}
