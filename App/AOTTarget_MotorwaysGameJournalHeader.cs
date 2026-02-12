using System;
using Factory;
using FixMath;
using Motorways;

// Token: 0x02000017 RID: 23
public static class AOTTarget_MotorwaysGameJournalHeader
{
	// Token: 0x06000063 RID: 99 RVA: 0x000035B6 File Offset: 0x000017B6
	public static void DontCall_AOTWorkaround()
	{
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGameJournalHeader, GameJournalMotive>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGameJournalHeader, string>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGameJournalHeader, DateTime>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGameJournalHeader, int>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGameJournalHeader, GameMode>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGameJournalHeader, Fix64>();
		Assembler.DontCall_EnsureAOTGenericCallsAreCompiled<MotorwaysGameJournalHeader, MapChallenge.ChallengeType>();
	}
}
