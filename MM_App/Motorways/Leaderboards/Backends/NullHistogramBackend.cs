using System;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x02000778 RID: 1912
	public class NullHistogramBackend : IHistogramBackend
	{
		// Token: 0x06003506 RID: 13574 RVA: 0x000F85A1 File Offset: 0x000F67A1
		public void RequestHistogram(LeaderboardId leaderboardId, HistogramRequestCompleted histogramRequestCompleted)
		{
			histogramRequestCompleted(null, 0, new LeaderboardError(LeaderboardErrorCode.Unknown, StringId.None));
		}
	}
}
