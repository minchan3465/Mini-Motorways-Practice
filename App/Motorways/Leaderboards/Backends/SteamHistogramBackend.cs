using System;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x02000786 RID: 1926
	public class SteamHistogramBackend : ScrapedHistogramBackend
	{
		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x0600354A RID: 13642 RVA: 0x000F9476 File Offset: 0x000F7676
		protected override string ServiceId
		{
			get
			{
				return "steam";
			}
		}
	}
}
