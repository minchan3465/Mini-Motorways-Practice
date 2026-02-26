using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000066 RID: 102
public static class AOTTarget_MouseCameraAction
{
	// Token: 0x060000B2 RID: 178 RVA: 0x00003866 File Offset: 0x00001A66
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MouseCameraAction, IScope>();
	}
}
