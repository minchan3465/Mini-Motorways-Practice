using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Motorways.Leaderboards
{
	// Token: 0x0200075E RID: 1886
	// (Invoke) Token: 0x06003495 RID: 13461
	public delegate void HistogramRequestCompleted([CanBeNull] List<int> buckets, int bucketSize, [CanBeNull] LeaderboardError error);
}
