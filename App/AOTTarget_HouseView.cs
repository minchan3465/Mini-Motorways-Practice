using System;
using Factory;
using Motorways;
using Motorways.Views;

// Token: 0x0200003F RID: 63
public static class AOTTarget_HouseView
{
	// Token: 0x0600008B RID: 139 RVA: 0x00003755 File Offset: 0x00001955
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<HouseView, City>();
	}
}
