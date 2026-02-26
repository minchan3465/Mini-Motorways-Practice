using System;
using Factory;
using FixMath;
using Server;

namespace Motorways.Models
{
	// Token: 0x020004FC RID: 1276
	public class ScoreModel : Model<EmptyModelFrame, ScoreModel.IObserver>
	{
		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x060021D6 RID: 8662 RVA: 0x000886B7 File Offset: 0x000868B7
		// (set) Token: 0x060021D7 RID: 8663 RVA: 0x000886BF File Offset: 0x000868BF
		[Serialize(true, null)]
		public int Score { get; private set; }

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x060021D8 RID: 8664 RVA: 0x000886C8 File Offset: 0x000868C8
		// (set) Token: 0x060021D9 RID: 8665 RVA: 0x000886D0 File Offset: 0x000868D0
		[Serialize(true, null)]
		public Fix64 EfficiencyScore { get; private set; } = Fix64.Zero;

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x060021DA RID: 8666 RVA: 0x000886D9 File Offset: 0x000868D9
		// (set) Token: 0x060021DB RID: 8667 RVA: 0x000886E1 File Offset: 0x000868E1
		[Serialize(true, null)]
		public int CurrentEfficiencyMilestone { get; private set; }

		// Token: 0x060021DC RID: 8668 RVA: 0x000886EC File Offset: 0x000868EC
		public void AddScore()
		{
			int score = this.Score;
			this.Score = score + 1;
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x0008870C File Offset: 0x0008690C
		public void AddEfficiencyScoreFromTripLength(Fix64 vehiclePathLength)
		{
			if (this._upgrades.HasPendingUpgrades)
			{
				return;
			}
			Fix64 scoreToAdd = this._constants.GetEfficiencyScoreForVehiclePathLength(vehiclePathLength);
			this.EfficiencyScore = Fix64.Min(scoreToAdd + this.EfficiencyScore, this._city.Definition.GetEfficiencyMilestone(this.CurrentEfficiencyMilestone, this._constants.MilestoneIncreaseAfterPrecalculatedIntervals));
			foreach (ScoreModel.IObserver observer in base.Observers)
			{
				observer.OnEfficiencyScoreIncreased(scoreToAdd);
			}
		}

		// Token: 0x060021DE RID: 8670 RVA: 0x00088790 File Offset: 0x00086990
		public bool HasAchievedCurrentMilestone()
		{
			return this.EfficiencyScore >= this._city.Definition.GetEfficiencyMilestone(this.CurrentEfficiencyMilestone, this._constants.MilestoneIncreaseAfterPrecalculatedIntervals);
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x000887C0 File Offset: 0x000869C0
		public void ProgressToNextMilestone()
		{
			int currentEfficiencyMilestone = this.CurrentEfficiencyMilestone;
			this.CurrentEfficiencyMilestone = currentEfficiencyMilestone + 1;
			this.EfficiencyScore = Fix64.Zero;
			if (this._city.Rules.RecordsGameStatistics() && this._city.Rules.ScoringMode == ScoringMode.EfficiencyMilestones)
			{
				this._player.AchievementStatistics.OnEndlessMilestoneAchieved(this._achievementHandler);
				this._player.CheckLifetimeAchievements();
			}
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x00088830 File Offset: 0x00086A30
		public void DeductEfficiencyScore(Fix64 deltaTime)
		{
			Fix64 progressThroughMilestone = this.EfficiencyScore / this._city.Definition.GetEfficiencyMilestone(this.CurrentEfficiencyMilestone, this._constants.MilestoneIncreaseAfterPrecalculatedIntervals);
			Fix64 percentageOfMilestoneToLose = this._constants.GetPercentageOfMilestoneToLoseFromProgress(progressThroughMilestone);
			this.EfficiencyScore = Fix64.Max(this.EfficiencyScore - this._city.Definition.GetEfficiencyMilestone(this.CurrentEfficiencyMilestone, this._constants.MilestoneIncreaseAfterPrecalculatedIntervals) * percentageOfMilestoneToLose * deltaTime, Fix64.Zero);
		}

		// Token: 0x060021E1 RID: 8673 RVA: 0x000888BF File Offset: 0x00086ABF
		public void OnContinuedInEndless()
		{
			this.CurrentEfficiencyMilestone = Math.Max(this._upgrades.TotalClaimedPackages, this.CurrentEfficiencyMilestone);
		}

		// Token: 0x060021E2 RID: 8674 RVA: 0x000888DD File Offset: 0x00086ADD
		public void ResetForEndless()
		{
			this.EfficiencyScore = Fix64.Zero;
			this.CurrentEfficiencyMilestone = Math.Min(this._upgrades.TotalGrantedUpgradesCount, this.CurrentEfficiencyMilestone);
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x00088906 File Offset: 0x00086B06
		public override void Reset()
		{
			base.Reset();
			this.Score = 0;
			this.EfficiencyScore = Fix64.Zero;
			this.CurrentEfficiencyMilestone = 0;
		}

		// Token: 0x060021E4 RID: 8676 RVA: 0x00088927 File Offset: 0x00086B27
		public ScoreModel() : base(1)
		{
		}

		// Token: 0x04001BCC RID: 7116
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001BCD RID: 7117
		[Dependency]
		private City _city;

		// Token: 0x04001BCE RID: 7118
		[Dependency]
		private UpgradeDatabaseModel _upgrades;

		// Token: 0x04001BCF RID: 7119
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04001BD0 RID: 7120
		[Dependency]
		private IAchievementHandler _achievementHandler;

		// Token: 0x020004FD RID: 1277
		public interface IObserver
		{
			// Token: 0x060021E5 RID: 8677
			void OnEfficiencyScoreIncreased(Fix64 addedScore);
		}
	}
}
