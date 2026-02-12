using System;
using Factory;
using FixMath;
using Motorways.Models;
using Server;

// Token: 0x0200002C RID: 44
public static class AOTTarget_ScoreModel
{
	// Token: 0x06000078 RID: 120 RVA: 0x00003694 File Offset: 0x00001894
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ScoreModel, Clock>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ScoreModel, int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<ScoreModel, Fix64>();
	}
}
