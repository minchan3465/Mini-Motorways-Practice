using System;

namespace Motorways.Leaderboards
{
	// Token: 0x0200076C RID: 1900
	public class DailyLeaderboardId : RecurringLeaderboardId
	{
		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x060034CC RID: 13516 RVA: 0x000F7534 File Offset: 0x000F5734
		public DayOfWeek Day { get; }

		// Token: 0x060034CD RID: 13517 RVA: 0x000F753C File Offset: 0x000F573C
		public DailyLeaderboardId(int startTime) : base(startTime)
		{
			this.Day = ChallengeSystem.ToDateTime(startTime).DayOfWeek;
			this._serializedString = this.Serialize();
		}

		// Token: 0x060034CE RID: 13518 RVA: 0x000F7570 File Offset: 0x000F5770
		public static bool IsDailyLeaderboardId(string leaderboardIdString)
		{
			return leaderboardIdString.StartsWith("daily_challenge");
		}

		// Token: 0x060034CF RID: 13519 RVA: 0x000F7580 File Offset: 0x000F5780
		public override bool IsLeaderboardOpen()
		{
			int startOfToday = ChallengeSystem.ToTimestamp(GameDateTime.UtcToday);
			int endOfTodayWithGracePeriod = ChallengeSystem.ToTimestamp(GameDateTime.UtcToday + TimeSpan.FromDays(1.0) + TimeSpan.FromSeconds(3600.0));
			return base.Timestamp >= startOfToday && base.Timestamp < endOfTodayWithGracePeriod;
		}

		// Token: 0x060034D0 RID: 13520 RVA: 0x000F75E0 File Offset: 0x000F57E0
		public new static DailyLeaderboardId Deserialize(string leaderboardIdString)
		{
			if (!DailyLeaderboardId.IsDailyLeaderboardId(leaderboardIdString))
			{
				LeaderboardId.Log.Error("Invalid DailyLeaderboardId string prefix: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			int prefixLength = "daily_challenge".Length + 1;
			if (leaderboardIdString.Length < prefixLength)
			{
				LeaderboardId.Log.Error("Too few characters for DailyLeaderboardId string: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			string[] idComponents = leaderboardIdString.Substring(prefixLength).Split('_', StringSplitOptions.None);
			if (idComponents.Length != 1)
			{
				LeaderboardId.Log.Error("Invalid component count for DailyLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			int timestamp;
			if (!int.TryParse(idComponents[0], out timestamp))
			{
				LeaderboardId.Log.Error("Failed to parse timestamp string from DailyLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			return new DailyLeaderboardId(timestamp);
		}

		// Token: 0x060034D1 RID: 13521 RVA: 0x000F76A3 File Offset: 0x000F58A3
		private string Serialize()
		{
			return string.Format("{0}_{1}", "daily_challenge", base.Timestamp);
		}

		// Token: 0x04002D0A RID: 11530
		public const string DailyChallengeIdPrefix = "daily_challenge";
	}
}
