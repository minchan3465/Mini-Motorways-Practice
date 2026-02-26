using System;
using FixMath;
using Motorways.Models;

namespace Motorways
{
	// Token: 0x020003CF RID: 975
	public class BackgroundGameRules : GameRules
	{
		// Token: 0x06001716 RID: 5910 RVA: 0x00016EC3 File Offset: 0x000150C3
		public override int GetMaximumDemandForDestination(DestinationModel destinationModel)
		{
			return 5;
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x0000222C File Offset: 0x0000042C
		public override int GetNumberOfUpgradeOptionsPerWeek()
		{
			return 0;
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool CanInteract()
		{
			return false;
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool ShowsUI()
		{
			return false;
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool DoesIgnorePlayableArea()
		{
			return true;
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0000222C File Offset: 0x0000042C
		public override int GetExpectedUpgradePackageCount(Fix64 upgradeScheduleTime)
		{
			return 0;
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool SupportsLeaderboards()
		{
			return false;
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool RecordsGameStatistics()
		{
			return false;
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool HasSpawnScheduleVariation()
		{
			return false;
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool ShowDisconnectedBuildingsUI()
		{
			return false;
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool CanSave()
		{
			return false;
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool SupportsChallenges()
		{
			return false;
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06001722 RID: 5922 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool DoRoadsAnimation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001723 RID: 5923 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool ShouldSavePeriodically
		{
			get
			{
				return false;
			}
		}
	}
}
