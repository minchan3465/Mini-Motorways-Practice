using System;
using Factory;
using Motorways;
using Motorways.Models;
using Server;

// Token: 0x02000025 RID: 37
public static class AOTTarget_CityModel
{
	// Token: 0x06000071 RID: 113 RVA: 0x0000364F File Offset: 0x0000184F
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<CityModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<CityModel, GameMode>();
	}
}
