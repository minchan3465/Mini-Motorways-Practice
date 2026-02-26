using System;

namespace Motorways.Leaderboards
{
	// Token: 0x0200076D RID: 1901
	public class WeeklyLeaderboardId : RecurringLeaderboardId
	{
		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060034D2 RID: 13522 RVA: 0x000F76BF File Offset: 0x000F58BF
		public ChallengeSystem.LeaderboardWeek Week { get; }

		// Token: 0x060034D3 RID: 13523 RVA: 0x000F76C7 File Offset: 0x000F58C7
		public WeeklyLeaderboardId(int startTime) : base(startTime)
		{
			this.Week = ChallengeSystem.GetLeaderboardWeek(startTime);
			this._serializedString = this.Serialize();
		}

		// Token: 0x060034D4 RID: 13524 RVA: 0x000F76E8 File Offset: 0x000F58E8
		public static bool IsWeeklyLeaderboardId(string leaderboardIdString)
		{
			return leaderboardIdString.StartsWith("weekly_challenge");
		}

		// Token: 0x060034D5 RID: 13525 RVA: 0x000F76F8 File Offset: 0x000F58F8
		public override bool IsLeaderboardOpen()
		{
			DateTime dateTime = ChallengeSystem.StartOfWeek(GameDateTime.UtcToday);
			int startOfWeek = ChallengeSystem.ToTimestamp(dateTime);
			int endOfWeekWithGracePeriod = ChallengeSystem.ToTimestamp(dateTime + TimeSpan.FromDays(7.0) + TimeSpan.FromSeconds(3600.0));
			return base.Timestamp >= startOfWeek && base.Timestamp < endOfWeekWithGracePeriod;
		}

		// Token: 0x060034D6 RID: 13526 RVA: 0x000F7758 File Offset: 0x000F5958
		public new static WeeklyLeaderboardId Deserialize(string leaderboardIdString)
		{
			if (!WeeklyLeaderboardId.IsWeeklyLeaderboardId(leaderboardIdString))
			{
				LeaderboardId.Log.Error("Invalid WeeklyLeaderboardId string prefix: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			int prefixLength = "weekly_challenge".Length + 1;
			if (leaderboardIdString.Length < prefixLength)
			{
				LeaderboardId.Log.Error("Too few characters for WeeklyLeaderboardId string: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			string[] idComponents = leaderboardIdString.Substring(prefixLength).Split('_', StringSplitOptions.None);
			if (idComponents.Length != 1)
			{
				LeaderboardId.Log.Error("Invalid component count for WeeklyLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			int timestamp;
			if (!int.TryParse(idComponents[0], out timestamp))
			{
				LeaderboardId.Log.Error("Failed to parse timestamp string from WeeklyLeaderboardId: " + leaderboardIdString, Array.Empty<object>());
				return null;
			}
			return new WeeklyLeaderboardId(timestamp);
		}

		// Token: 0x060034D7 RID: 13527 RVA: 0x000F781B File Offset: 0x000F5A1B
		private string Serialize()
		{
			return string.Format("{0}_{1}", "weekly_challenge", base.Timestamp);
		}

		// Token: 0x04002D0C RID: 11532
		public const string WeeklyChallengeIdPrefix = "weekly_challenge";
	}
}
