using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000061 RID: 97
public static class AOTTarget_TouchCameraAction
{
	// Token: 0x060000AD RID: 173 RVA: 0x00003843 File Offset: 0x00001A43
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<TouchCameraAction, IScope>();
	}
}
