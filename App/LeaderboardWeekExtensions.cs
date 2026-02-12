using System;
using Motorways;

// Token: 0x02000194 RID: 404
public static class LeaderboardWeekExtensions
{
	// Token: 0x0600091B RID: 2331 RVA: 0x0001DAB7 File Offset: 0x0001BCB7
	public static ChallengeSystem.LeaderboardWeek Other(this ChallengeSystem.LeaderboardWeek week)
	{
		if (week != ChallengeSystem.LeaderboardWeek.WeekA)
		{
			return ChallengeSystem.LeaderboardWeek.WeekA;
		}
		return ChallengeSystem.LeaderboardWeek.WeekB;
	}
}
