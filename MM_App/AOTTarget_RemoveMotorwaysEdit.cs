using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x02000021 RID: 33
public static class AOTTarget_RemoveMotorwaysEdit
{
	// Token: 0x0600006D RID: 109 RVA: 0x00003633 File Offset: 0x00001833
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RemoveMotorwaysEdit, Vector2Int>();
	}
}
