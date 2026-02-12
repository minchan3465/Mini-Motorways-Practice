using System;

namespace Motorways.Leaderboards
{
	// Token: 0x02000769 RID: 1897
	public abstract class LeaderboardId : IEquatable<LeaderboardId>
	{
		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x060034B8 RID: 13496 RVA: 0x000F723D File Offset: 0x000F543D
		public string SerializedString
		{
			get
			{
				return this._serializedString;
			}
		}

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x060034B9 RID: 13497
		public abstract bool IsRecurringLeaderboard { get; }

		// Token: 0x060034BA RID: 13498 RVA: 0x000F7248 File Offset: 0x000F5448
		public static LeaderboardId Deserialize(string leaderboardIdString)
		{
			if (CityLeaderboardId.IsCityLeaderboardId(leaderboardIdString))
			{
				return CityLeaderboardId.Deserialize(leaderboardIdString);
			}
			if (DailyLeaderboardId.IsDailyLeaderboardId(leaderboardIdString))
			{
				return DailyLeaderboardId.Deserialize(leaderboardIdString);
			}
			if (WeeklyLeaderboardId.IsWeeklyLeaderboardId(leaderboardIdString))
			{
				return WeeklyLeaderboardId.Deserialize(leaderboardIdString);
			}
			LeaderboardId.Log.Error("Invalid LeaderboardId string prefix: " + leaderboardIdString, Array.Empty<object>());
			return null;
		}

		// Token: 0x060034BB RID: 13499 RVA: 0x000F729D File Offset: 0x000F549D
		public bool Equals(LeaderboardId leaderboardId)
		{
			return leaderboardId != null && this.SerializedString == leaderboardId.SerializedString;
		}

		// Token: 0x060034BC RID: 13500 RVA: 0x000F72B5 File Offset: 0x000F54B5
		public override int GetHashCode()
		{
			return this.SerializedString.GetHashCode();
		}

		// Token: 0x060034BD RID: 13501 RVA: 0x000F72C2 File Offset: 0x000F54C2
		public override string ToString()
		{
			return this.SerializedString;
		}

		// Token: 0x04002D02 RID: 11522
		protected static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("LeaderboardId");

		// Token: 0x04002D03 RID: 11523
		protected const int InvalidTimestamp = -1;

		// Token: 0x04002D04 RID: 11524
		protected string _serializedString;
	}
}
