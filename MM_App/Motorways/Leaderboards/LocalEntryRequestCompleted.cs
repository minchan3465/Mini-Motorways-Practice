using System;
using JetBrains.Annotations;

namespace Motorways.Leaderboards
{
	// Token: 0x02000763 RID: 1891
	// (Invoke) Token: 0x060034A7 RID: 13479
	public delegate void LocalEntryRequestCompleted([CanBeNull] LeaderboardEntry localEntry, long totalLeaderboardEntryCount, [CanBeNull] LeaderboardError error);
}
