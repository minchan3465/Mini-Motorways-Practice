using System;
using Factory;
using Motorways;
using Motorways.Models;

// Token: 0x0200002D RID: 45
public static class AOTTarget_UpgradeDatabaseModel
{
	// Token: 0x06000079 RID: 121 RVA: 0x000036A5 File Offset: 0x000018A5
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<UpgradeDatabaseModel, int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<UpgradeDatabaseModel, UpgradeType>();
	}
}
