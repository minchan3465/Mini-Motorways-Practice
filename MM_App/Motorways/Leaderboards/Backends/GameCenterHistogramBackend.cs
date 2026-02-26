using System;

namespace Motorways.Leaderboards.Backends
{
	// Token: 0x0200077A RID: 1914
	public class GameCenterHistogramBackend : ScrapedHistogramBackend
	{
		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x06003512 RID: 13586 RVA: 0x000F85B2 File Offset: 0x000F67B2
		protected override string ServiceId
		{
			get
			{
				return "game-center";
			}
		}
	}
}
