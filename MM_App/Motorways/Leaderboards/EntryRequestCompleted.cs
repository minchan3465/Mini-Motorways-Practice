using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Motorways.Leaderboards
{
	// Token: 0x02000762 RID: 1890
	// (Invoke) Token: 0x060034A3 RID: 13475
	public delegate void EntryRequestCompleted([CanBeNull] List<LeaderboardEntry> entries, long totalLeaderboardEntryCount, [CanBeNull] LeaderboardError error);
}
