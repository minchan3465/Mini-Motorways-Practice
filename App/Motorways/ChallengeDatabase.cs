using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200033F RID: 831
	[CreateAssetMenu(fileName = "New Challenge Database", menuName = "Motorways/Challenges/Challenge Database", order = 2)]
	public class ChallengeDatabase : ScriptableObject
	{
		// Token: 0x0600148C RID: 5260 RVA: 0x000431AD File Offset: 0x000413AD
		public bool IsChallengeWildcard(ChallengeData challenge)
		{
			return this.wildcardChallenges.Contains(challenge);
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x000431BC File Offset: 0x000413BC
		public bool TryGetChallenge(string challengeName, out ChallengeData result)
		{
			foreach (ChallengeData challenge in this.regularChallenges)
			{
				if (challenge.name == challengeName)
				{
					result = challenge;
					return true;
				}
			}
			foreach (ChallengeData challenge2 in this.wildcardChallenges)
			{
				if (challenge2.name == challengeName)
				{
					result = challenge2;
					return true;
				}
			}
			if (this.expertModeChallenge.name == challengeName)
			{
				result = this.expertModeChallenge;
				return true;
			}
			ChallengeDatabase.Log.Error("Unable to find challenge matching the name: '" + challengeName + "'!", Array.Empty<object>());
			result = null;
			return false;
		}

		// Token: 0x040010EE RID: 4334
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ChallengeDatabase");

		// Token: 0x040010EF RID: 4335
		[NonReorderable]
		public List<ChallengeData> regularChallenges = new List<ChallengeData>();

		// Token: 0x040010F0 RID: 4336
		[NonReorderable]
		public List<ChallengeData> wildcardChallenges = new List<ChallengeData>();

		// Token: 0x040010F1 RID: 4337
		[NonReorderable]
		public List<ChallengeData> debugInjectedChallenges = new List<ChallengeData>();

		// Token: 0x040010F2 RID: 4338
		[NonReorderable]
		public List<YearOfChallenges> precalculatedChallenges;

		// Token: 0x040010F3 RID: 4339
		[Tooltip("Unlocking any of these achievements will unlock daily & weekly challenges")]
		[NonReorderable]
		public MotorwaysAchievementData[] qualifyingAchievementsToUnlockTimedChallenges;

		// Token: 0x040010F4 RID: 4340
		public ChallengeData expertModeChallenge;
	}
}
