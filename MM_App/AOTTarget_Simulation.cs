using System;
using Factory;
using FixMath;
using Server;

// Token: 0x02000019 RID: 25
public static class AOTTarget_Simulation
{
	// Token: 0x06000065 RID: 101 RVA: 0x000035E2 File Offset: 0x000017E2
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Simulation, IScope>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Simulation, Fix64>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<Simulation, bool>();
	}
}
