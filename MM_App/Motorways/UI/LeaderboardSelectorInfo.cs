using System;
using Motorways.Leaderboards;

namespace Motorways.UI
{
	// Token: 0x02000729 RID: 1833
	public static class LeaderboardSelectorInfo
	{
		// Token: 0x0600327D RID: 12925 RVA: 0x000EED74 File Offset: 0x000ECF74
		public static CityGameMode GetGameModeForIndex(int selectorIndex)
		{
			CityGameMode result;
			if (selectorIndex != 0)
			{
				if (selectorIndex != 1)
				{
					result = CityGameMode.CityChallenge;
				}
				else
				{
					result = CityGameMode.Expert;
				}
			}
			else
			{
				result = CityGameMode.Normal;
			}
			return result;
		}

		// Token: 0x04002B4E RID: 11086
		public const int NormalIndex = 0;

		// Token: 0x04002B4F RID: 11087
		public const int ExpertIndex = 1;

		// Token: 0x04002B50 RID: 11088
		public const int ChallengeOptionOffset = 2;
	}
}
