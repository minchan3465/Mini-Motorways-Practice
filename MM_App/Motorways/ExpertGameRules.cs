using System;

namespace Motorways
{
	// Token: 0x020003D3 RID: 979
	public class ExpertGameRules : GameRules
	{
		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06001754 RID: 5972 RVA: 0x00054013 File Offset: 0x00052213
		public override bool CanBuildingsDemolishUnusedRoads
		{
			get
			{
				return !FeatureToggle.IsFeatureEnabled(Feature.ExpertNoDemolish);
			}
		}

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool RoadsBecomePermanentOverTime
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x00054021 File Offset: 0x00052221
		public override int GetNumberOfUpgradeOptionsPerWeek()
		{
			return 9;
		}
	}
}
