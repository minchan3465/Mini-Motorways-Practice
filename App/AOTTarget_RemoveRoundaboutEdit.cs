using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x02000023 RID: 35
public static class AOTTarget_RemoveRoundaboutEdit
{
	// Token: 0x0600006F RID: 111 RVA: 0x00003641 File Offset: 0x00001841
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RemoveRoundaboutEdit, Vector2Int>();
	}
}
