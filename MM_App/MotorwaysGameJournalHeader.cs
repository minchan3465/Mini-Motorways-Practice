using System;
using Factory;
using FixMath;
using Motorways;
using Motorways.Models;
using Server;
using UnityEngine;

// Token: 0x020001B3 RID: 435
[Factory.Serializable(1)]
internal class MotorwaysGameJournalHeader : IMotorwaysGameJournalHeader
{
	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06000A10 RID: 2576 RVA: 0x0002184B File Offset: 0x0001FA4B
	// (set) Token: 0x06000A11 RID: 2577 RVA: 0x00021853 File Offset: 0x0001FA53
	[Serialize(true, null)]
	public GameJournalMotive Motive { get; private set; }

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06000A12 RID: 2578 RVA: 0x0002185C File Offset: 0x0001FA5C
	// (set) Token: 0x06000A13 RID: 2579 RVA: 0x00021864 File Offset: 0x0001FA64
	[Serialize(true, null)]
	public string DeviceModel { get; private set; }

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06000A14 RID: 2580 RVA: 0x0002186D File Offset: 0x0001FA6D
	// (set) Token: 0x06000A15 RID: 2581 RVA: 0x00021875 File Offset: 0x0001FA75
	[Serialize(true, null)]
	public string DeviceName { get; private set; }

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06000A16 RID: 2582 RVA: 0x0002187E File Offset: 0x0001FA7E
	// (set) Token: 0x06000A17 RID: 2583 RVA: 0x00021886 File Offset: 0x0001FA86
	[Serialize(true, null)]
	public DateTime UtcTimestamp { get; private set; }

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06000A18 RID: 2584 RVA: 0x0002188F File Offset: 0x0001FA8F
	// (set) Token: 0x06000A19 RID: 2585 RVA: 0x00021897 File Offset: 0x0001FA97
	[Serialize(true, null)]
	public int GameAssemblerSerializerHashCode { get; private set; }

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06000A1A RID: 2586 RVA: 0x000218A0 File Offset: 0x0001FAA0
	// (set) Token: 0x06000A1B RID: 2587 RVA: 0x000218A8 File Offset: 0x0001FAA8
	[Serialize(true, null)]
	public string CityId { get; private set; }

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06000A1C RID: 2588 RVA: 0x000218B1 File Offset: 0x0001FAB1
	// (set) Token: 0x06000A1D RID: 2589 RVA: 0x000218B9 File Offset: 0x0001FAB9
	[Serialize(true, null)]
	public GameMode Mode { get; private set; }

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06000A1E RID: 2590 RVA: 0x000218C2 File Offset: 0x0001FAC2
	// (set) Token: 0x06000A1F RID: 2591 RVA: 0x000218CA File Offset: 0x0001FACA
	[Serialize(true, null)]
	public int TripCount { get; private set; }

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06000A20 RID: 2592 RVA: 0x000218D3 File Offset: 0x0001FAD3
	// (set) Token: 0x06000A21 RID: 2593 RVA: 0x000218DB File Offset: 0x0001FADB
	[Serialize(true, null)]
	public Fix64 TimeElapsed { get; private set; }

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x06000A22 RID: 2594 RVA: 0x000218E4 File Offset: 0x0001FAE4
	// (set) Token: 0x06000A23 RID: 2595 RVA: 0x000218EC File Offset: 0x0001FAEC
	[Serialize(true, null)]
	public MapChallenge.ChallengeType ChallengeType { get; private set; }

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06000A24 RID: 2596 RVA: 0x000218F5 File Offset: 0x0001FAF5
	// (set) Token: 0x06000A25 RID: 2597 RVA: 0x000218FD File Offset: 0x0001FAFD
	[Serialize(true, null)]
	public int ChallengeEndTime { get; private set; }

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06000A26 RID: 2598 RVA: 0x00021906 File Offset: 0x0001FB06
	// (set) Token: 0x06000A27 RID: 2599 RVA: 0x0002190E File Offset: 0x0001FB0E
	[Serialize(true, null)]
	public int ChallengeIndex { get; private set; }

	// Token: 0x06000A28 RID: 2600 RVA: 0x00021918 File Offset: 0x0001FB18
	public bool Initialize(ISimulation simulation, GameJournalMotive motive)
	{
		CityModel cityModel = simulation.GetModel<CityModel>();
		ScoreModel scoreModel = simulation.GetModel<ScoreModel>();
		ClockModel clockModel = simulation.GetModel<ClockModel>();
		ActiveChallengesModel activeChallenges = simulation.GetModel<ActiveChallengesModel>();
		if (cityModel == null || scoreModel == null || clockModel == null)
		{
			return false;
		}
		this.Motive = motive;
		this.DeviceModel = SystemInfo.deviceModel;
		this.DeviceName = SystemInfo.deviceName;
		this.UtcTimestamp = DateTime.UtcNow;
		this.GameAssemblerSerializerHashCode = simulation.Scope.Assembler.GlobalTypeSerializerHashCode;
		this.CityId = cityModel.cityName;
		this.Mode = cityModel.Mode;
		this.TripCount = scoreModel.Score;
		this.TimeElapsed = clockModel.Time;
		this.ChallengeType = activeChallenges.challengeType;
		this.ChallengeEndTime = activeChallenges.timeEnd;
		this.ChallengeIndex = activeChallenges.cityChallengeIndex;
		return true;
	}
}
