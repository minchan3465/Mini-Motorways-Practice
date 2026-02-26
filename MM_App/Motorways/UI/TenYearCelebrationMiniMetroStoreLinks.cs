using System;

namespace Motorways.UI
{
	// Token: 0x02000718 RID: 1816
	public static class TenYearCelebrationMiniMetroStoreLinks
	{
		// Token: 0x04002AC2 RID: 10946
		private const string SteamUtmSource = "game";

		// Token: 0x04002AC3 RID: 10947
		private const string SteamUtmCampaign = "tenyearcelebration";

		// Token: 0x04002AC4 RID: 10948
		private const string SteamUtmMedium = "button";

		// Token: 0x04002AC5 RID: 10949
		private static readonly string SteamStoreBaseLink = "https://store.steampowered.com/app/287980/Mini_Metro";

		// Token: 0x04002AC6 RID: 10950
		private static readonly string SteamStoreParameters = "?utm_source=game&utm_campaign=tenyearcelebration&utm_medium=button";

		// Token: 0x04002AC7 RID: 10951
		public static readonly string SteamStoreLink = TenYearCelebrationMiniMetroStoreLinks.SteamStoreBaseLink + "/" + TenYearCelebrationMiniMetroStoreLinks.SteamStoreParameters;

		// Token: 0x04002AC8 RID: 10952
		public const string ArcadeAppStoreLink = "https://apple.co/-MiniMetro";
	}
}
