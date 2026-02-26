using System;
using Factory;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x02000787 RID: 1927
	public class SteamworksLeaderboardBackend : ILeaderboardBackend
	{
		// Token: 0x0600354C RID: 13644 RVA: 0x000F947D File Offset: 0x000F767D
		public void RequestLocalEntry(LeaderboardId leaderboardId, LocalEntryRequestCompleted localEntryRequestCompleted)
		{
			SteamworksShared.RequestLocalLeaderboardEntry(SteamworksLeaderboardBackend.GetBackendLeaderboardIdWithPrefix(leaderboardId), localEntryRequestCompleted);
		}

		// Token: 0x0600354D RID: 13645 RVA: 0x000F948C File Offset: 0x000F768C
		public void SubmitScore(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState, SubmitScoreRequestCompleted submitScoreRequestCompleted)
		{
			string leaderboardName = SteamworksLeaderboardBackend.GetBackendLeaderboardIdWithPrefix(leaderboardId);
			SteamworksShared.SubmitScore(leaderboardId, leaderboardName, score, scoreState, submitScoreRequestCompleted);
		}

		// Token: 0x0600354E RID: 13646 RVA: 0x000F94AB File Offset: 0x000F76AB
		public void RequestTopEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			SteamworksShared.RequestTopLeaderboardEntries(SteamworksLeaderboardBackend.GetBackendLeaderboardIdWithPrefix(leaderboardId), entryCount, entryRequestCompleted);
		}

		// Token: 0x0600354F RID: 13647 RVA: 0x000F94BA File Offset: 0x000F76BA
		public void RequestPlayerCenteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			SteamworksShared.RequestPlayerCenteredLeaderboardEntries(SteamworksLeaderboardBackend.GetBackendLeaderboardIdWithPrefix(leaderboardId), entryCount, entryRequestCompleted);
		}

		// Token: 0x06003550 RID: 13648 RVA: 0x000F94C9 File Offset: 0x000F76C9
		public void RequestTopFriendFilteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			SteamworksShared.RequestTopFriendLeaderboardEntries(SteamworksLeaderboardBackend.GetBackendLeaderboardIdWithPrefix(leaderboardId), entryCount, entryRequestCompleted);
		}

		// Token: 0x06003551 RID: 13649 RVA: 0x000022F5 File Offset: 0x000004F5
		public void PresentError(LeaderboardError error)
		{
		}

		// Token: 0x06003552 RID: 13650 RVA: 0x000020AA File Offset: 0x000002AA
		public bool IsLeaderboardTypeSupported(LeaderboardType type)
		{
			return true;
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06003553 RID: 13651 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CanAuthenticate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003554 RID: 13652 RVA: 0x0000222C File Offset: 0x0000042C
		public bool Authenticate(AuthenticationCompleted authenticationCompleted)
		{
			return false;
		}

		// Token: 0x06003555 RID: 13653 RVA: 0x000F94D8 File Offset: 0x000F76D8
		private static string GetBackendLeaderboardIdWithPrefix(LeaderboardId leaderboardId)
		{
			string leaderboardIdString = SteamworksLeaderboardBackend.GetBackendLeaderboardId(leaderboardId);
			if (FeatureToggle.IsFeatureEnabled(Feature.SteamBetaLeaderboards))
			{
				return "beta_" + leaderboardIdString;
			}
			return leaderboardIdString;
		}

		// Token: 0x06003556 RID: 13654 RVA: 0x000F9504 File Offset: 0x000F7704
		public static string GetBackendLeaderboardId(LeaderboardId leaderboardId)
		{
			CityLeaderboardId cityLeaderboardId = leaderboardId as CityLeaderboardId;
			if (cityLeaderboardId == null)
			{
				DailyLeaderboardId dailyLeaderboardId = leaderboardId as DailyLeaderboardId;
				if (dailyLeaderboardId != null)
				{
					DateTime dateTimeNow = GameDateTime.UtcNow;
					int deltaDays = dateTimeNow.DayOfWeek - dailyLeaderboardId.Day;
					if (deltaDays < 0)
					{
						deltaDays += 7;
					}
					DateTime leaderboardDate = dateTimeNow.Subtract(TimeSpan.FromDays((double)deltaDays));
					return string.Format("daily_challenge_{0:0000}-{1:00}-{2:00}", leaderboardDate.Year, leaderboardDate.Month, leaderboardDate.Day);
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
				DateTime leaderboardDate2 = ChallengeSystem.GetStartOfLastOccurence(weeklyLeaderboardId.Week);
				return string.Format("weekly_challenge_{0:0000}-{1:00}-{2:00}", leaderboardDate2.Year, leaderboardDate2.Month, leaderboardDate2.Day);
			}
			else
			{
				if (cityLeaderboardId.Mode == CityGameMode.CityChallenge)
				{
					return string.Format("{0}_{1}_challenge{2}", cityLeaderboardId.City.ToString().ToLower(), cityLeaderboardId.Mode.ToString().ToLower(), cityLeaderboardId.CityChallengeIndex);
				}
				CityLeaderboardId cityLeaderboardId2 = cityLeaderboardId;
				return cityLeaderboardId2.City.ToString().ToLower() + "_" + cityLeaderboardId2.Mode.ToString().ToLower();
			}
		}

		// Token: 0x04002D66 RID: 11622
		[Dependency]
		private LeaderboardService _leaderboardService;
	}
}
