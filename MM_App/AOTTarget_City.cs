using System;
using Factory;
using Motorways;

// Token: 0x0200001C RID: 28
public static class AOTTarget_City
{
	// Token: 0x06000068 RID: 104 RVA: 0x00003606 File Offset: 0x00001806
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<City, IScope>();
	}
}
