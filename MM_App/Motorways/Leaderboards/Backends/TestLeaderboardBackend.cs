using System;
using System.Collections.Generic;
using System.Linq;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x02000789 RID: 1929
	public class TestLeaderboardBackend : ILeaderboardBackend
	{
		// Token: 0x06003558 RID: 13656 RVA: 0x000F9687 File Offset: 0x000F7887
		private LeaderboardStorage GetOrCreateLeaderboardStorage(LeaderboardId leaderboard)
		{
			if (!this._leaderboardStorage.ContainsKey(leaderboard))
			{
				this._leaderboardStorage.Add(leaderboard, new LeaderboardStorage());
			}
			return this._leaderboardStorage[leaderboard];
		}

		// Token: 0x06003559 RID: 13657 RVA: 0x000F96B4 File Offset: 0x000F78B4
		public void RequestLocalEntry(LeaderboardId leaderboardId, LocalEntryRequestCompleted localEntryRequestCompleted)
		{
			LeaderboardStorage leaderboardStorage = this.GetOrCreateLeaderboardStorage(leaderboardId);
			localEntryRequestCompleted(leaderboardStorage.LocalEntry, 0L, null);
		}

		// Token: 0x0600355A RID: 13658 RVA: 0x000F96D8 File Offset: 0x000F78D8
		public void SubmitScore(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState, SubmitScoreRequestCompleted submitScoreRequestCompleted)
		{
			LeaderboardStorage orCreateLeaderboardStorage = this.GetOrCreateLeaderboardStorage(leaderboardId);
			int context = LeaderboardService.EncodeScoreContext(leaderboardId, scoreState);
			orCreateLeaderboardStorage.InsertOrUpdateEntry("Test User", LeaderboardEntryType.Local, score, context);
			submitScoreRequestCompleted(true);
		}

		// Token: 0x0600355B RID: 13659 RVA: 0x000F970C File Offset: 0x000F790C
		public void RequestTopEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			string leaderboardName = this.GetLeaderboardName(leaderboardId);
			LeaderboardStorage leaderboardStorage = this.GetOrCreateLeaderboardStorage(leaderboardId);
			List<LeaderboardEntry> topLeaderboardEntries = leaderboardStorage.entries.Take(entryCount).ToList<LeaderboardEntry>();
			LeaderboardEntry topLeaderboardEntry = topLeaderboardEntries[0];
			topLeaderboardEntry.Name = leaderboardName + " Master";
			topLeaderboardEntries[0] = topLeaderboardEntry;
			if (leaderboardStorage.localEntryIndex > entryCount)
			{
				topLeaderboardEntries.Add(leaderboardStorage.LocalEntry);
			}
			this.ReturnCompletedRequest(entryRequestCompleted, topLeaderboardEntries, leaderboardStorage.entries.Count);
		}

		// Token: 0x0600355C RID: 13660 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RequestPlayerCenteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
		}

		// Token: 0x0600355D RID: 13661 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RequestTopFriendFilteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
		}

		// Token: 0x0600355E RID: 13662 RVA: 0x000022F5 File Offset: 0x000004F5
		public void PresentError(LeaderboardError error)
		{
		}

		// Token: 0x0600355F RID: 13663 RVA: 0x000F9784 File Offset: 0x000F7984
		private void ReturnCompletedRequest(EntryRequestCompleted entryRequestCompleted, List<LeaderboardEntry> topLeaderboardEntries, int totalLeaderboardEntryCount)
		{
			if (entryRequestCompleted != null)
			{
				entryRequestCompleted(topLeaderboardEntries, (long)totalLeaderboardEntryCount, null);
			}
		}

		// Token: 0x06003560 RID: 13664 RVA: 0x000F9794 File Offset: 0x000F7994
		private string GetLeaderboardName(LeaderboardId leaderboardId)
		{
			CityLeaderboardId cityLeaderboardId = leaderboardId as CityLeaderboardId;
			if (cityLeaderboardId != null)
			{
				return cityLeaderboardId.City.ToString();
			}
			DailyLeaderboardId dailyLeaderboardId = leaderboardId as DailyLeaderboardId;
			if (dailyLeaderboardId != null)
			{
				return dailyLeaderboardId.Day.ToString();
			}
			WeeklyLeaderboardId weeklyLeaderboardId = leaderboardId as WeeklyLeaderboardId;
			if (weeklyLeaderboardId == null)
			{
				Diagnostics.FailAssert("Invalid ILeaderboard derived type: {0}", new object[]
				{
					leaderboardId
				});
				return null;
			}
			return weeklyLeaderboardId.Week.ToString();
		}

		// Token: 0x06003561 RID: 13665 RVA: 0x000F9089 File Offset: 0x000F7289
		public bool IsLeaderboardTypeSupported(LeaderboardType type)
		{
			return type == LeaderboardType.Histogram || type == LeaderboardType.Global;
		}

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06003562 RID: 13666 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CanAuthenticate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003563 RID: 13667 RVA: 0x0000222C File Offset: 0x0000042C
		public bool Authenticate(AuthenticationCompleted authenticationCompleted)
		{
			return false;
		}

		// Token: 0x04002D6C RID: 11628
		private Dictionary<LeaderboardId, LeaderboardStorage> _leaderboardStorage = new Dictionary<LeaderboardId, LeaderboardStorage>();
	}
}
