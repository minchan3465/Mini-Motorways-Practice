using System;
using Motorways;
using Motorways.Models;
using UnityEngine;

// Token: 0x020001B5 RID: 437
public class MotorwaysGameStatistics : IGameStatistics
{
	// Token: 0x17000257 RID: 599
	// (get) Token: 0x06000A3E RID: 2622 RVA: 0x00021DA8 File Offset: 0x0001FFA8
	// (set) Token: 0x06000A3F RID: 2623 RVA: 0x00021DB0 File Offset: 0x0001FFB0
	public string CityId { get; private set; }

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x06000A40 RID: 2624 RVA: 0x00021DB9 File Offset: 0x0001FFB9
	// (set) Token: 0x06000A41 RID: 2625 RVA: 0x00021DC1 File Offset: 0x0001FFC1
	public int TotalTrips { get; private set; }

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x06000A42 RID: 2626 RVA: 0x00021DCA File Offset: 0x0001FFCA
	// (set) Token: 0x06000A43 RID: 2627 RVA: 0x00021DD2 File Offset: 0x0001FFD2
	public int NewTrips { get; private set; }

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x06000A44 RID: 2628 RVA: 0x00021DDB File Offset: 0x0001FFDB
	// (set) Token: 0x06000A45 RID: 2629 RVA: 0x00021DE3 File Offset: 0x0001FFE3
	public int PeakAverageTrips { get; private set; }

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x06000A46 RID: 2630 RVA: 0x00021DEC File Offset: 0x0001FFEC
	// (set) Token: 0x06000A47 RID: 2631 RVA: 0x00021DF4 File Offset: 0x0001FFF4
	public int TotalDuration { get; private set; }

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x06000A48 RID: 2632 RVA: 0x00021DFD File Offset: 0x0001FFFD
	// (set) Token: 0x06000A49 RID: 2633 RVA: 0x00021E05 File Offset: 0x00020005
	public int NewDuration { get; private set; }

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06000A4A RID: 2634 RVA: 0x00021E0E File Offset: 0x0002000E
	// (set) Token: 0x06000A4B RID: 2635 RVA: 0x00021E16 File Offset: 0x00020016
	public int TotalPlayTime { get; private set; }

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x06000A4C RID: 2636 RVA: 0x00021E1F File Offset: 0x0002001F
	// (set) Token: 0x06000A4D RID: 2637 RVA: 0x00021E27 File Offset: 0x00020027
	public int NewPlayTime { get; private set; }

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06000A4E RID: 2638 RVA: 0x00021E30 File Offset: 0x00020030
	// (set) Token: 0x06000A4F RID: 2639 RVA: 0x00021E38 File Offset: 0x00020038
	public GameMode Mode { get; private set; }

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06000A50 RID: 2640 RVA: 0x00021E41 File Offset: 0x00020041
	// (set) Token: 0x06000A51 RID: 2641 RVA: 0x00021E49 File Offset: 0x00020049
	public ActiveChallengesModel Challenge { get; private set; }

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06000A52 RID: 2642 RVA: 0x00021E52 File Offset: 0x00020052
	// (set) Token: 0x06000A53 RID: 2643 RVA: 0x00021E5A File Offset: 0x0002005A
	public GameEndReason? GameEndReason { get; private set; }

	// Token: 0x06000A54 RID: 2644 RVA: 0x00021E64 File Offset: 0x00020064
	public void InitFromGame(MotorwaysGame fromGame)
	{
		if (fromGame.MapDefinition != null)
		{
			this.CityId = fromGame.MapDefinition.cityName;
			this.Mode = fromGame.Scope.Get<CityModel>().Mode;
			this.Challenge = fromGame.Scope.Get<ActiveChallengesModel>();
		}
		else
		{
			this.CityId = "Error";
			this.Mode = GameMode.Normal;
		}
		ScoreModel scoreModel = fromGame.Scope.Get<ScoreModel>();
		this.TotalTrips = scoreModel.Score;
		this.NewTrips = scoreModel.Score;
		this.TotalDuration = Mathf.FloorToInt((float)scoreModel.Clock.Time);
		this.NewDuration = this.TotalDuration;
		this.TotalPlayTime = Mathf.FloorToInt((float)scoreModel.Clock.Time);
		this.NewPlayTime = this.TotalPlayTime;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x00021F40 File Offset: 0x00020140
	public void InitFromGameIncrementally(MotorwaysGame fromGame, MotorwaysGameStatistics initialGameStatistics, GameEndReason? fromGameEndReason)
	{
		this.InitFromGame(fromGame);
		this.NewTrips -= initialGameStatistics.TotalTrips;
		this.NewDuration -= initialGameStatistics.TotalDuration;
		this.NewPlayTime -= initialGameStatistics.TotalPlayTime;
		this.GameEndReason = fromGameEndReason;
	}

	// Token: 0x0400056A RID: 1386
	private const int InvalidTripCount = -1;
}
