using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x02000022 RID: 34
public static class AOTTarget_RemoveUnbuiltMotorwaysEdit
{
	// Token: 0x0600006E RID: 110 RVA: 0x0000363A File Offset: 0x0000183A
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RemoveUnbuiltMotorwaysEdit, Vector2Int>();
	}
}
