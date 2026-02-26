using System;
using System.Collections.Generic;
using Factory;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x02000782 RID: 1922
	public class RetailDemoLeaderboardBackend : ILeaderboardBackend
	{
		// Token: 0x06003532 RID: 13618 RVA: 0x000F8E7C File Offset: 0x000F707C
		public void RequestLocalEntry(LeaderboardId leaderboardId, LocalEntryRequestCompleted localEntryRequestCompleted)
		{
			CityLeaderboardId cityLeaderboardId = leaderboardId as CityLeaderboardId;
			if (cityLeaderboardId != null)
			{
				MotorwaysCityStatistics cityStatistics = this._player.GetCityStatisticsForCity(cityLeaderboardId.City.ToString(), GameMode.Normal, false);
				localEntryRequestCompleted(LeaderboardEntry.TestEntry("Me", LeaderboardEntryType.Local, (cityStatistics != null) ? cityStatistics.MaxTrips : 0, 1L, 0, LeaderboardScoreState.Editable), 1L, null);
				return;
			}
			localEntryRequestCompleted(null, 1L, null);
		}

		// Token: 0x06003533 RID: 13619 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SubmitScore(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState, SubmitScoreRequestCompleted submitScoreRequestCompleted)
		{
		}

		// Token: 0x06003534 RID: 13620 RVA: 0x000F8EE4 File Offset: 0x000F70E4
		public void RequestTopEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			List<LeaderboardEntry> entries = new List<LeaderboardEntry>
			{
				LeaderboardEntry.TestEntry("Robena Rotolo", LeaderboardEntryType.Global, global::Random.Range(2500, 3000), 1L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Rosalina Vancleve", LeaderboardEntryType.Global, global::Random.Range(2300, 2499), 2L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Miki Varona", LeaderboardEntryType.Global, global::Random.Range(2100, 2299), 3L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Sanford Lenk", LeaderboardEntryType.Global, global::Random.Range(2000, 2099), 4L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Indira Obando", LeaderboardEntryType.Global, global::Random.Range(1950, 1999), 5L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Marguerite Kells", LeaderboardEntryType.Global, global::Random.Range(1920, 1949), 6L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Lakeesha Cieslak", LeaderboardEntryType.Global, global::Random.Range(1900, 1919), 7L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Cherri Smart", LeaderboardEntryType.Global, global::Random.Range(1870, 1899), 8L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Grady Feist", LeaderboardEntryType.Global, global::Random.Range(1860, 1869), 9L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Jeanmarie Pearce", LeaderboardEntryType.Global, global::Random.Range(1850, 1859), 10L, 0, LeaderboardScoreState.Editable),
				LeaderboardEntry.TestEntry("Me", LeaderboardEntryType.Local, 0, 0L, 0, LeaderboardScoreState.NotSubmitted)
			};
			if (entryRequestCompleted != null)
			{
				entryRequestCompleted(entries, (long)entries.Count, null);
			}
		}

		// Token: 0x06003535 RID: 13621 RVA: 0x00015E3F File Offset: 0x0001403F
		public void RequestPlayerCenteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06003536 RID: 13622 RVA: 0x00015E3F File Offset: 0x0001403F
		public void RequestTopFriendFilteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06003537 RID: 13623 RVA: 0x000022F5 File Offset: 0x000004F5
		public void PresentError(LeaderboardError error)
		{
		}

		// Token: 0x06003538 RID: 13624 RVA: 0x000F9089 File Offset: 0x000F7289
		public bool IsLeaderboardTypeSupported(LeaderboardType type)
		{
			return type == LeaderboardType.Histogram || type == LeaderboardType.Global;
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06003539 RID: 13625 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CanAuthenticate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600353A RID: 13626 RVA: 0x0000222C File Offset: 0x0000042C
		public bool Authenticate(AuthenticationCompleted authenticationCompleted)
		{
			return false;
		}

		// Token: 0x04002D58 RID: 11608
		[Dependency]
		private ActivePlayer _player;
	}
}
