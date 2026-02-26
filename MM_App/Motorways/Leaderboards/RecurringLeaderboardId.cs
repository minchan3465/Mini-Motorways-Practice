using System;

namespace Motorways.Leaderboards
{
	// Token: 0x0200076B RID: 1899
	public abstract class RecurringLeaderboardId : LeaderboardId
	{
		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x060034C8 RID: 13512 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool IsRecurringLeaderboard
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060034C9 RID: 13513 RVA: 0x000F7516 File Offset: 0x000F5716
		protected RecurringLeaderboardId(int timestamp)
		{
			this._timestamp = timestamp;
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x060034CA RID: 13514 RVA: 0x000F752C File Offset: 0x000F572C
		public int Timestamp
		{
			get
			{
				return this._timestamp;
			}
		}

		// Token: 0x060034CB RID: 13515
		public abstract bool IsLeaderboardOpen();

		// Token: 0x04002D09 RID: 11529
		private readonly int _timestamp = -1;
	}
}
