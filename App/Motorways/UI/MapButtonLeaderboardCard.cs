using System;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000732 RID: 1842
	public class MapButtonLeaderboardCard : MapButtonCard
	{
		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x0600332A RID: 13098 RVA: 0x000F28C0 File Offset: 0x000F0AC0
		public LeaderboardPanel LeaderboardPanel
		{
			get
			{
				return this.leaderboardPanel;
			}
		}

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x0600332B RID: 13099 RVA: 0x000F28C8 File Offset: 0x000F0AC8
		public TouchOptionButton RecurringLeaderboardSelector
		{
			get
			{
				return this.recurringLeaderboardSelector;
			}
		}

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x0600332C RID: 13100 RVA: 0x000F28D0 File Offset: 0x000F0AD0
		public TouchButton LeaderboardSelectorPrevious
		{
			get
			{
				return this.leaderboardSelectorPrevious;
			}
		}

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x0600332D RID: 13101 RVA: 0x000F28D8 File Offset: 0x000F0AD8
		public TouchButton LeaderboardSelectorNext
		{
			get
			{
				return this.leaderboardSelectorNext;
			}
		}

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x0600332E RID: 13102 RVA: 0x000F28E0 File Offset: 0x000F0AE0
		public TouchToggle LeaderboardSurroundingButton
		{
			get
			{
				return this.leaderboardPanel.SurroundingLeaderboardsButton;
			}
		}

		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x0600332F RID: 13103 RVA: 0x000F28ED File Offset: 0x000F0AED
		public TouchToggle LeaderboardFriendsButton
		{
			get
			{
				return this.leaderboardPanel.FriendsLeaderboardsButton;
			}
		}

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x06003330 RID: 13104 RVA: 0x000F28FA File Offset: 0x000F0AFA
		public TouchToggle LeaderboardGlobalButton
		{
			get
			{
				return this.leaderboardPanel.GlobalLeaderboardsButton;
			}
		}

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x06003331 RID: 13105 RVA: 0x000F2907 File Offset: 0x000F0B07
		public TouchToggle LeaderboardHistogramButton
		{
			get
			{
				return this.leaderboardPanel.HistogramLeaderboardsButton;
			}
		}

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x06003332 RID: 13106 RVA: 0x000F2914 File Offset: 0x000F0B14
		public TouchButton LeaderboardErrorButton
		{
			get
			{
				return this.leaderboardPanel.LeaderboardErrorButton;
			}
		}

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x06003333 RID: 13107 RVA: 0x000F2921 File Offset: 0x000F0B21
		public GameObject[] RecurringDayOptions
		{
			get
			{
				return this.selectorDayOptions;
			}
		}

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x06003334 RID: 13108 RVA: 0x000F2929 File Offset: 0x000F0B29
		public GameObject[] RecurringWeekOptions
		{
			get
			{
				return this.selectorWeekOptions;
			}
		}

		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x06003335 RID: 13109 RVA: 0x000F2931 File Offset: 0x000F0B31
		public GameObject[] RecurringTypeOptions
		{
			get
			{
				return this.selectorTypeOptions;
			}
		}

		// Token: 0x04002BAA RID: 11178
		[SerializeField]
		private LeaderboardPanel leaderboardPanel;

		// Token: 0x04002BAB RID: 11179
		[SerializeField]
		private TouchOptionButton recurringLeaderboardSelector;

		// Token: 0x04002BAC RID: 11180
		[SerializeField]
		private TouchButton leaderboardSelectorPrevious;

		// Token: 0x04002BAD RID: 11181
		[SerializeField]
		private TouchButton leaderboardSelectorNext;

		// Token: 0x04002BAE RID: 11182
		[SerializeField]
		private GameObject[] selectorDayOptions;

		// Token: 0x04002BAF RID: 11183
		[SerializeField]
		private GameObject[] selectorWeekOptions;

		// Token: 0x04002BB0 RID: 11184
		[SerializeField]
		private GameObject[] selectorTypeOptions;
	}
}
