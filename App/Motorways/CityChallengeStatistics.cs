using System;
using System.Collections.Generic;

namespace Motorways
{
	// Token: 0x020003DB RID: 987
	public class CityChallengeStatistics
	{
		// Token: 0x1400003C RID: 60
		// (add) Token: 0x060017D6 RID: 6102 RVA: 0x00054FAC File Offset: 0x000531AC
		// (remove) Token: 0x060017D7 RID: 6103 RVA: 0x00054FE4 File Offset: 0x000531E4
		public event Action DataChanged;

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x060017D8 RID: 6104 RVA: 0x00055019 File Offset: 0x00053219
		// (set) Token: 0x060017D9 RID: 6105 RVA: 0x00055021 File Offset: 0x00053221
		public string CityId { get; private set; }

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x060017DA RID: 6106 RVA: 0x0005502A File Offset: 0x0005322A
		// (set) Token: 0x060017DB RID: 6107 RVA: 0x00055032 File Offset: 0x00053232
		public GameMode Mode { get; private set; }

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x060017DC RID: 6108 RVA: 0x0005503B File Offset: 0x0005323B
		// (set) Token: 0x060017DD RID: 6109 RVA: 0x00055043 File Offset: 0x00053243
		public int ChallengeIndex { get; private set; }

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x060017DE RID: 6110 RVA: 0x0005504C File Offset: 0x0005324C
		// (set) Token: 0x060017DF RID: 6111 RVA: 0x00055054 File Offset: 0x00053254
		public int BestScore
		{
			get
			{
				return this._bestScore;
			}
			set
			{
				if (this._bestScore != value)
				{
					this._bestScore = value;
					Action dataChanged = this.DataChanged;
					if (dataChanged == null)
					{
						return;
					}
					dataChanged();
				}
			}
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x00055076 File Offset: 0x00053276
		public void Merge(CityChallengeStatistics otherStatistics)
		{
			if (otherStatistics.BestScore > this.BestScore)
			{
				this.BestScore = otherStatistics.BestScore;
			}
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x00055092 File Offset: 0x00053292
		public CityChallengeStatistics(string cityId, GameMode mode, int challengeIndex, int bestScore = 0)
		{
			this.CityId = cityId;
			this.Mode = mode;
			this.ChallengeIndex = challengeIndex;
			this.BestScore = bestScore;
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x000550B8 File Offset: 0x000532B8
		public static CityChallengeStatistics InitFromJson(JSON.Dictionary jsonDictionary)
		{
			string @string = jsonDictionary.GetString("CityId");
			GameMode mode = (GameMode)jsonDictionary.GetInt("Mode", 0);
			int challengeIndex = jsonDictionary.GetInt("ChallengeIndex", 0);
			int bestScore = jsonDictionary.GetInt("BestScore", 0);
			return new CityChallengeStatistics(@string, mode, challengeIndex, bestScore);
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x00055100 File Offset: 0x00053300
		public object ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["CityId"] = this.CityId;
			dictionary["Mode"] = (int)this.Mode;
			dictionary["ChallengeIndex"] = this.ChallengeIndex;
			dictionary["BestScore"] = this.BestScore;
			return dictionary;
		}

		// Token: 0x0400147C RID: 5244
		private int _bestScore;
	}
}
