using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000053 RID: 83
public static class AOTTarget_DragCreativeModeEditableObjectAction
{
	// Token: 0x0600009F RID: 159 RVA: 0x000037E1 File Offset: 0x000019E1
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<DragCreativeModeEditableObjectAction, IScope>();
	}
}
