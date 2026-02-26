using System;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x02000779 RID: 1913
	public class NullLeaderboardBackend : ILeaderboardBackend
	{
		// Token: 0x06003508 RID: 13576 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RequestLocalEntry(LeaderboardId leaderboardId, LocalEntryRequestCompleted localEntryRequestCompleted)
		{
		}

		// Token: 0x06003509 RID: 13577 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SubmitScore(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState, SubmitScoreRequestCompleted submitScoreRequestCompleted)
		{
		}

		// Token: 0x0600350A RID: 13578 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RequestTopEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
		}

		// Token: 0x0600350B RID: 13579 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RequestPlayerCenteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
		}

		// Token: 0x0600350C RID: 13580 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RequestTopFriendFilteredEntries(LeaderboardId leaderboardId, int entryCount, EntryRequestCompleted entryRequestCompleted)
		{
		}

		// Token: 0x0600350D RID: 13581 RVA: 0x000022F5 File Offset: 0x000004F5
		public void PresentError(LeaderboardError error)
		{
		}

		// Token: 0x0600350E RID: 13582 RVA: 0x0000222C File Offset: 0x0000042C
		public bool IsLeaderboardTypeSupported(LeaderboardType type)
		{
			return false;
		}

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x0600350F RID: 13583 RVA: 0x0000222C File Offset: 0x0000042C
		public bool CanAuthenticate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003510 RID: 13584 RVA: 0x0000222C File Offset: 0x0000042C
		public bool Authenticate(AuthenticationCompleted authenticationCompleted)
		{
			return false;
		}
	}
}
