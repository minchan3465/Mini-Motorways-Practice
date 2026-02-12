using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000391 RID: 913
	[Serializable]
	public struct UpgradeCycleDefinition
	{
		// Token: 0x0400129C RID: 4764
		[Tooltip("The packages the player starts with on this map, including concrete")]
		public UpgradePackageDefinition[] startingPackages;

		// Token: 0x0400129D RID: 4765
		[Tooltip("The potential packages that can be presented each week")]
		public WeeklyUpgradeDefinition[] weeklyChoicePackages;
	}
}
