using System;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.Processes;
using Server;

namespace Motorways
{
	// Token: 0x020003D2 RID: 978
	public class EndlessGameRules : GameRules
	{
		// Token: 0x17000475 RID: 1141
		// (get) Token: 0x06001745 RID: 5957 RVA: 0x000020AA File Offset: 0x000002AA
		public override ScoringMode ScoringMode
		{
			get
			{
				return ScoringMode.EfficiencyMilestones;
			}
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00053EE6 File Offset: 0x000520E6
		public override int GetExpectedUpgradePackageCount(Fix64 upgradeScheduleTime)
		{
			if (FeatureToggle.IsFeatureEnabled(Feature.EndlessWithWeeklyMilestones))
			{
				return base.GetExpectedUpgradePackageCount(upgradeScheduleTime);
			}
			return this._scoreModel.CurrentEfficiencyMilestone;
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x00053EBE File Offset: 0x000520BE
		public override int GetMaximumDemandForDestination(DestinationModel destinationModel)
		{
			return destinationModel.MaximumDemandBeforeTimerStarts;
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x00053F04 File Offset: 0x00052104
		public override Fix64 GetDemandMultiplierForDestination(DestinationModel model)
		{
			return base.GetDemandMultiplierForDestination(model) * this._constants.EndlessDemandMultiplier;
		}

		// Token: 0x17000476 RID: 1142
		// (get) Token: 0x06001749 RID: 5961 RVA: 0x00053F1D File Offset: 0x0005211D
		public override Fix64 SpawnRampMultiplier
		{
			get
			{
				return this._constants.EndlessSpawnRampMultiplier;
			}
		}

		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x0600174A RID: 5962 RVA: 0x00053F2A File Offset: 0x0005212A
		public override int AdditionalHousesPerGroup
		{
			get
			{
				return this._constants.AdditionalHousesPerGroup;
			}
		}

		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x0600174B RID: 5963 RVA: 0x00053F38 File Offset: 0x00052138
		public override bool CanExpansionTimeContinue
		{
			get
			{
				if (FeatureToggle.IsFeatureEnabled(Feature.EndlessWithWeeklyMilestones))
				{
					return base.CanExpansionTimeContinue;
				}
				if (this._clock.ExpansionTime >= (Fix64)((long)(this._scoreModel.CurrentEfficiencyMilestone + 1)) * this._constants.ExpansionTimePerMilestone + this._constants.BonusExpansionTime)
				{
					return false;
				}
				if (this._upgrades.HasPendingUpgrades)
				{
					return false;
				}
				foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
				{
					if (destination.totalServicedPins == 0 && destination.IsSupplySufficient && this._cityPlanModel.groupHouseCounts[destination.GroupIndex] >= this._constants.AdditionalHousesPerGroup)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x0600174C RID: 5964 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool UsesPerCityHouseGraph
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x0600174D RID: 5965 RVA: 0x000020AA File Offset: 0x000002AA
		public override GenerateDemandProcess.DemandGenerationStyle GetDemandGenerationStyle
		{
			get
			{
				return GenerateDemandProcess.DemandGenerationStyle.PermanentBalanced;
			}
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x0600174E RID: 5966 RVA: 0x00054004 File Offset: 0x00052204
		public override int UpgradeWeekMetric
		{
			get
			{
				return this._upgrades.TotalClaimedPackages + 1;
			}
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool SupportsLeaderboards()
		{
			return false;
		}

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06001750 RID: 5968 RVA: 0x0000222C File Offset: 0x0000042C
		public override bool CanDestinationsOvercrowd
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06001751 RID: 5969 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool CanUpgradeDestinationsAfterFailedSpawns
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06001752 RID: 5970 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool FailedSpawnsIgnoreStoppedExpansionTime
		{
			get
			{
				return true;
			}
		}

		// Token: 0x040013CB RID: 5067
		[Dependency]
		private ScoreModel _scoreModel;

		// Token: 0x040013CC RID: 5068
		[Dependency]
		private ClockModel _clock;

		// Token: 0x040013CD RID: 5069
		[Dependency]
		private UpgradeDatabaseModel _upgrades;

		// Token: 0x040013CE RID: 5070
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040013CF RID: 5071
		[Dependency]
		private CityPlanModel _cityPlanModel;
	}
}
