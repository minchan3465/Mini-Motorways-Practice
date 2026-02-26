using System;
using System.Collections.Generic;
using Factory;
using Motorways.Leaderboards;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class LeaderboardEntry
{
	// Token: 0x1700010B RID: 267
	// (get) Token: 0x060004FE RID: 1278 RVA: 0x000117A6 File Offset: 0x0000F9A6
	public string Id { get; }

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x060004FF RID: 1279 RVA: 0x000117AE File Offset: 0x0000F9AE
	// (set) Token: 0x06000500 RID: 1280 RVA: 0x000117B6 File Offset: 0x0000F9B6
	public string Name { get; set; }

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000501 RID: 1281 RVA: 0x000117BF File Offset: 0x0000F9BF
	public LeaderboardEntryType Type { get; }

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000502 RID: 1282 RVA: 0x000117C7 File Offset: 0x0000F9C7
	public int Score { get; }

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000503 RID: 1283 RVA: 0x000117CF File Offset: 0x0000F9CF
	// (set) Token: 0x06000504 RID: 1284 RVA: 0x000117D7 File Offset: 0x0000F9D7
	public long Rank { get; set; }

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000505 RID: 1285 RVA: 0x000117E0 File Offset: 0x0000F9E0
	public int Timestamp { get; }

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000506 RID: 1286 RVA: 0x000117E8 File Offset: 0x0000F9E8
	public LeaderboardScoreState ScoreState { get; }

	// Token: 0x06000507 RID: 1287 RVA: 0x000117F0 File Offset: 0x0000F9F0
	public LeaderboardEntry(string id, string name, LeaderboardEntryType type, int score, long rank, int timestamp, LeaderboardScoreState scoreState)
	{
		this.Id = id;
		this.Name = name;
		this.Type = type;
		this.Score = score;
		this.Rank = rank;
		this.Timestamp = timestamp;
		this.ScoreState = scoreState;
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0001182D File Offset: 0x0000FA2D
	public static LeaderboardEntry TestEntry(string name, LeaderboardEntryType type, int score, long rank, int timeStamp = 0, LeaderboardScoreState scoreState = LeaderboardScoreState.Editable)
	{
		return new LeaderboardEntry(name, name, type, score, rank, timeStamp, scoreState);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00011840 File Offset: 0x0000FA40
	public override bool Equals(object obj)
	{
		LeaderboardEntry leaderboardEntry = obj as LeaderboardEntry;
		return leaderboardEntry != null && leaderboardEntry.Id == this.Id;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x0001186A File Offset: 0x0000FA6A
	public override int GetHashCode()
	{
		return this.Id.GetHashCode();
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00011878 File Offset: 0x0000FA78
	public StandaloneLocString FormatLocalUserString(IScope scope, long totalLeaderboardEntryCount, LeaderboardEntryFormatOptions options = (LeaderboardEntryFormatOptions)0)
	{
		StringKey nameKey = scope.Get<StringKey>();
		nameKey.InitWithStringId(StringId.You);
		StandaloneLocString userEntryLocString = StandaloneLocString.CreateString(scope, nameKey);
		string userEntryString = userEntryLocString.ToString();
		if (options.HasFlag(LeaderboardEntryFormatOptions.BoldYou))
		{
			userEntryString = "<b>" + userEntryString + "</b>";
		}
		scope.Release(userEntryLocString);
		scope.Release(nameKey);
		bool flag = this.Rank > 0L;
		bool inTop10 = this.Rank <= 10L;
		bool leaderboardHasEnoughEntriesForMeaningfulPercentile = totalLeaderboardEntryCount > 10L;
		if (flag && (!inTop10 || options.HasFlag(LeaderboardEntryFormatOptions.IncludePercentileInTopTen)) && leaderboardHasEnoughEntriesForMeaningfulPercentile)
		{
			StringId percentileStringId;
			int displayPercentile = LeaderboardEntry.GetDisplayPercentile(this.Rank, totalLeaderboardEntryCount, out percentileStringId);
			StringKey scoreKey = scope.Get<StringKey>();
			scoreKey.InitWithStringId(percentileStringId, displayPercentile, new Dictionary<string, string>
			{
				{
					"Num",
					displayPercentile.ToString()
				}
			});
			StandaloneLocString percentileLocString = StandaloneLocString.CreateString(scope, scoreKey);
			string percentileString = percentileLocString.ToString();
			scope.Release(percentileLocString);
			scope.Release(scoreKey);
			if (options.HasFlag(LeaderboardEntryFormatOptions.MultiLine))
			{
				userEntryString = userEntryString + "\n" + percentileString.TrimStart();
			}
			else
			{
				userEntryString += percentileString;
			}
		}
		return StandaloneLocString.CreateNonLocalizedString(scope, userEntryString);
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x000119B4 File Offset: 0x0000FBB4
	public static int GetDisplayPercentile(long rank, long totalLeaderboardEntryCount, out StringId percentileStringId)
	{
		float percentile = (float)rank / (float)totalLeaderboardEntryCount * 100f;
		int percentileInIncrementsOfOne = Mathf.Clamp(Mathf.CeilToInt(percentile), 1, 100);
		int displayPercentile = percentileInIncrementsOfOne;
		percentileStringId = StringId.TopPercentile;
		if (percentile > 5f)
		{
			displayPercentile = Mathf.CeilToInt((float)percentileInIncrementsOfOne / 5f) * 5;
			if (percentile > 50f)
			{
				percentileStringId = StringId.BottomPercentile;
				return 100 - displayPercentile + 5;
			}
		}
		return displayPercentile;
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00011A18 File Offset: 0x0000FC18
	public override string ToString()
	{
		return string.Format("[LeaderboardEntry: ID={0}, Name={1}, Type={2}, Score={3}, Rank={4}, ScoreState={5}]", new object[]
		{
			this.Id,
			this.Name,
			this.Type,
			this.Score,
			this.Rank,
			this.ScoreState
		});
	}
}
