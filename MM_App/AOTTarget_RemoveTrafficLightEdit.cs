using System;
using Factory;
using Motorways;
using UnityEngine;

// Token: 0x02000020 RID: 32
public static class AOTTarget_RemoveTrafficLightEdit
{
	// Token: 0x0600006C RID: 108 RVA: 0x0000362C File Offset: 0x0000182C
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<RemoveTrafficLightEdit, Vector2Int>();
	}
}
