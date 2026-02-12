using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x02000024 RID: 36
public static class AOTTarget_AddTrafficLightEdit
{
	// Token: 0x06000070 RID: 112 RVA: 0x00003648 File Offset: 0x00001848
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<AddTrafficLightEdit, Vector2Int>();
	}
}
