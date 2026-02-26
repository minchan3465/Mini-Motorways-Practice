using System;
using Factory;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways
{
	// Token: 0x020003D1 RID: 977
	public class CreativeGameRules : GameRules
	{
		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x00028DA8 File Offset: 0x00026FA8
		public override ScoringMode ScoringMode
		{
			get
			{
				return ScoringMode.None;
			}
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool HasDisabledAutomaticSpawn()
		{
			return true;
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x0000222C File Offset: 0x0000042C
		public override int GetExpectedUpgradePackageCount(Fix64 upgradeScheduleTime)
		{
			return 0;
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x00053EBE File Offset: 0x000520BE
		public override int GetMaximumDemandForDestination(DestinationModel destinationModel)
		{
			return destinationModel.MaximumDemandBeforeTimerStarts;
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x00053EC6 File Offset: 0x000520C6
		public override Fix64 GetDemandMultiplierForDestination(DestinationModel model)
		{
			return base.GetDemandMultiplierForDestination(model) * this._constants.CreativeDemandMultiplier;
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool SupportsLeaderboards()
		{
			return false;
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool RecordsGameStatistics()
		{
			return false;
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x00053EDF File Offset: 0x000520DF
		public override float GetCameraPanRange()
		{
			return 50f;
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001730 RID: 5936 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool CanDestinationsOvercrowd
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool CanUpgradeDestinationsAfterFailedSpawns
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06001732 RID: 5938 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool FailedSpawnsIgnoreStoppedExpansionTime
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001733 RID: 5939 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ShouldGameStartFullyExpanded
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001734 RID: 5940 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool HasUnlimitedUpgrades
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001735 RID: 5941 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool BuildingsIgnoreOtherBuildings
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001736 RID: 5942 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool NoDestinationDeadzoneForHouses
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001737 RID: 5943 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool AllowPlacingBuildingsOnUnzoneableTiles
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001738 RID: 5944 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool AllowSpawningAtMapEdges
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001739 RID: 5945 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool AllowBlockingSpawns
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x0600173A RID: 5946 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool AllowSpawnsOnRoundaboutDeadzone
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700046D RID: 1133
		// (get) Token: 0x0600173B RID: 5947 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool AllowConnectingDriveways
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x0600173C RID: 5948 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ShouldHideStaticUpgrades
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x0600173D RID: 5949 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ShowColourWidget
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x0600173E RID: 5950 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool AllowSecondDestinationStartUpgraded
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x0600173F RID: 5951 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ShouldSavePeriodically
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001740 RID: 5952 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool AllowDemandRelocation
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000473 RID: 1139
		// (get) Token: 0x06001741 RID: 5953 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool ShowUpgradeCounters
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool ShowDisconnectedBuildingsUI()
		{
			return false;
		}

		// Token: 0x17000474 RID: 1140
		// (get) Token: 0x06001743 RID: 5955 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool ShouldBuildingsBulldozeTrees
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040013C6 RID: 5062
		[Dependency]
		private ScoreModel _scoreModel;

		// Token: 0x040013C7 RID: 5063
		[Dependency]
		private ClockModel _clock;

		// Token: 0x040013C8 RID: 5064
		[Dependency]
		private UpgradeDatabaseModel _upgrades;

		// Token: 0x040013C9 RID: 5065
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040013CA RID: 5066
		[Dependency]
		private CityPlanModel _cityPlanModel;
	}
}
