using System;

namespace Motorways.Leaderboards
{
	// Token: 0x0200075D RID: 1885
	public interface IHistogramBackend
	{
		// Token: 0x06003493 RID: 13459
		void RequestHistogram(LeaderboardId leaderboardId, HistogramRequestCompleted histogramRequestCompleted);
	}
}
