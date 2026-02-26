using System;
using FixMath;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000393 RID: 915
	[Serializable]
	public class ExpectedUpgradeTimeline
	{
		// Token: 0x040012A7 RID: 4775
		public int week;

		// Token: 0x040012A8 RID: 4776
		[Tooltip("How many of this upgrade do we expect to have by this week?")]
		public int expectedUpgradeCount;

		// Token: 0x040012A9 RID: 4777
		[Tooltip("How much extra weight do we put on the package if they haven't had this many of this upgrade by now?")]
		public Fix64 bonusWeightIfNotMet;
	}
}
