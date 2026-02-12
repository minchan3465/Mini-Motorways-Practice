using System;
using JetBrains.Annotations;

namespace Motorways.Leaderboards
{
	// Token: 0x0200075F RID: 1887
	public interface ILeaderboardBackend
	{
		// Token: 0x06003498 RID: 13464
		void RequestLocalEntry(LeaderboardId leaderboardId, [NotNull] LocalEntryRequestCompleted localEntryRequestCompleted);

		// Token: 0x06003499 RID: 13465
		void SubmitScore(LeaderboardId leaderboardId, int score, LeaderboardScoreState scoreState, [NotNull] SubmitScoreRequestCompleted submitScoreRequestCompleted);

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x0600349A RID: 13466 RVA: 0x0000222C File Offset: 0x0000042C
		bool CanSubmitScoresOffline
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600349B RID: 13467
		void RequestTopEntries(LeaderboardId leaderboardId, int entryCount, [NotNull] EntryRequestCompleted entryRequestCompleted);

		// Token: 0x0600349C RID: 13468
		void RequestPlayerCenteredEntries(LeaderboardId leaderboardId, int entryCount, [NotNull] EntryRequestCompleted entryRequestCompleted);

		// Token: 0x0600349D RID: 13469
		void RequestTopFriendFilteredEntries(LeaderboardId leaderboardId, int entryCount, [NotNull] EntryRequestCompleted entryRequestCompleted);

		// Token: 0x0600349E RID: 13470
		void PresentError([NotNull] LeaderboardError error);

		// Token: 0x0600349F RID: 13471
		bool IsLeaderboardTypeSupported(LeaderboardType type);

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x060034A0 RID: 13472
		bool CanAuthenticate { get; }

		// Token: 0x060034A1 RID: 13473
		bool Authenticate(AuthenticationCompleted authenticationCompleted);
	}
}
