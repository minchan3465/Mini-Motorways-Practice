using System;
using System.Collections.Generic;
using FixMath;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000392 RID: 914
	[Serializable]
	public class WeeklyUpgradeDefinition
	{
		// Token: 0x060015EA RID: 5610 RVA: 0x0004B400 File Offset: 0x00049600
		public WeeklyUpgradeDefinition NewCopy()
		{
			return new WeeklyUpgradeDefinition
			{
				package = new UpgradePackageDefinition
				{
					type = this.package.type,
					amount = this.package.amount,
					additionalConcrete = this.package.additionalConcrete
				},
				baseWeight = this.baseWeight,
				weightIncreaseWhenNotOffered = this.weightIncreaseWhenNotOffered,
				maxPackageCount = this.maxPackageCount,
				startingWeek = this.startingWeek,
				lastWeek = this.lastWeek,
				expectedUpgradeTimeline = this.expectedUpgradeTimeline,
				relativeUpgradeType = this.relativeUpgradeType,
				relativeUpgradeMultiplierCurve = this.relativeUpgradeMultiplierCurve
			};
		}

		// Token: 0x0400129E RID: 4766
		public UpgradePackageDefinition package;

		// Token: 0x0400129F RID: 4767
		public Fix64 baseWeight = Fix64.One;

		// Token: 0x040012A0 RID: 4768
		[Tooltip("How much this should increase in weight each week it isn't offered.")]
		public Fix64 weightIncreaseWhenNotOffered;

		// Token: 0x040012A1 RID: 4769
		[Tooltip("The maximum number of packages that can be taken by the player. 0 is infinite")]
		public int maxPackageCount;

		// Token: 0x040012A2 RID: 4770
		[Tooltip("The first week this package can appear (inclusive)")]
		public int startingWeek;

		// Token: 0x040012A3 RID: 4771
		[Tooltip("The last week that this package can appear (inclusive)")]
		public int lastWeek;

		// Token: 0x040012A4 RID: 4772
		[Tooltip("The number of upgrades that the player must have taken by the provided week")]
		public List<ExpectedUpgradeTimeline> expectedUpgradeTimeline;

		// Token: 0x040012A5 RID: 4773
		[Tooltip("The type of upgrade to check that the player has remaining to change weights based on.")]
		public UpgradeType relativeUpgradeType;

		// Token: 0x040012A6 RID: 4774
		[Tooltip("Weight change (y-axis) against remaining upgrades (x-axis).")]
		public AnimationCurve relativeUpgradeMultiplierCurve;
	}
}
