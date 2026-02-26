using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000067 RID: 103
public static class AOTTarget_ControllerCameraAction
{
	// Token: 0x060000B3 RID: 179 RVA: 0x0000386D File Offset: 0x00001A6D
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ControllerCameraAction, IScope>();
	}
}
