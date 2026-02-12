using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Processes;
using Server;

namespace Motorways.Models
{
	// Token: 0x02000512 RID: 1298
	public class UpgradeDatabaseModel : UpgradeDatabase, IModel, IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x0600225F RID: 8799 RVA: 0x0008AAD2 File Offset: 0x00088CD2
		// (set) Token: 0x06002260 RID: 8800 RVA: 0x0008AADA File Offset: 0x00088CDA
		[Serialize(true, null)]
		public int TotalClaimedPackages { get; private set; }

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x06002261 RID: 8801 RVA: 0x0008AAE3 File Offset: 0x00088CE3
		// (set) Token: 0x06002262 RID: 8802 RVA: 0x0008AAEB File Offset: 0x00088CEB
		[Serialize(true, null)]
		public UpgradeType LastClaimedPackageType { get; private set; } = UpgradeType.Count;

		// Token: 0x06002263 RID: 8803 RVA: 0x0008AAF4 File Offset: 0x00088CF4
		public override void Reset()
		{
			base.Reset();
			for (int upgradeIndex = 0; upgradeIndex < 9; upgradeIndex++)
			{
				this._claimedPackageCounts[upgradeIndex] = 0;
				this._consecutiveWeeksSinceUpgradeLastPresented[upgradeIndex] = 0;
				this.timesUpgradePresented[upgradeIndex] = 0;
			}
			this.LastClaimedPackageType = UpgradeType.Count;
			this.pendingUpgradeChoices.Clear();
			this.TotalClaimedPackages = 0;
			this.accumulatedUpgradeScheduleDelayTime = Fix64Consts.Zero;
			this.upgradeSchedulePaused = false;
		}

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06002264 RID: 8804 RVA: 0x0008AB5B File Offset: 0x00088D5B
		public bool IsPendingUpgradeAvailable
		{
			get
			{
				return this.pendingUpgradeChoices.Count > 0;
			}
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06002265 RID: 8805 RVA: 0x0008AB6B File Offset: 0x00088D6B
		public virtual int TotalGrantedUpgradesCount
		{
			get
			{
				return this.TotalClaimedPackages + this.pendingUpgradeChoices.Count;
			}
		}

		// Token: 0x06002266 RID: 8806 RVA: 0x0008AB7F File Offset: 0x00088D7F
		public virtual void AddPendingUpgradeChoice(UpgradeChoice upgradeChoice)
		{
			this.pendingUpgradeChoices.Add(upgradeChoice);
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x0008AB90 File Offset: 0x00088D90
		public virtual void ApplyUpgradePackage(UpgradePackageDefinition upgradePackage, bool freeUpgrade = false)
		{
			this.LastClaimedPackageType = upgradePackage.type;
			this._totalUpgrades[(int)upgradePackage.type] += upgradePackage.amount;
			this._availableUpgrades[(int)upgradePackage.type] += upgradePackage.amount;
			if (!freeUpgrade)
			{
				this._claimedPackageCounts[(int)upgradePackage.type]++;
				int totalClaimedPackages = this.TotalClaimedPackages;
				this.TotalClaimedPackages = totalClaimedPackages + 1;
			}
			if (upgradePackage.additionalConcrete > 0)
			{
				this._totalUpgrades[0] += upgradePackage.additionalConcrete;
				this._availableUpgrades[0] += upgradePackage.additionalConcrete;
			}
			base.NotifyUpgradesChanged();
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x0008AC43 File Offset: 0x00088E43
		public virtual bool HasUpgradeBeenPresented(UpgradeType upgradeType)
		{
			return this._consecutiveWeeksSinceUpgradeLastPresented[(int)upgradeType] < this._clock.Week;
		}

		// Token: 0x06002269 RID: 8809 RVA: 0x0008AC5A File Offset: 0x00088E5A
		public virtual int NumberOfPackagesTakenOf(UpgradeType type)
		{
			return this._claimedPackageCounts[(int)type];
		}

		// Token: 0x0600226A RID: 8810 RVA: 0x0008AC64 File Offset: 0x00088E64
		public virtual void OnUpgradeNotPresented(UpgradeType upgradeType)
		{
			this._consecutiveWeeksSinceUpgradeLastPresented[(int)upgradeType]++;
		}

		// Token: 0x0600226B RID: 8811 RVA: 0x0008AC77 File Offset: 0x00088E77
		public virtual void OnUpgradePresented(UpgradeType upgradeType)
		{
			this.timesUpgradePresented[(int)upgradeType]++;
			this._consecutiveWeeksSinceUpgradeLastPresented[(int)upgradeType] = 0;
		}

		// Token: 0x0600226C RID: 8812 RVA: 0x0008AC93 File Offset: 0x00088E93
		public virtual int WeeksSinceUpgradePresented(UpgradeType upgradeType)
		{
			return this._consecutiveWeeksSinceUpgradeLastPresented[(int)upgradeType];
		}

		// Token: 0x0600226D RID: 8813 RVA: 0x0008ACA0 File Offset: 0x00088EA0
		public void OnReleasedFromScope(IScope scope)
		{
			foreach (UpgradeChoice upgradeChoice in this.pendingUpgradeChoices)
			{
				scope.Release(upgradeChoice);
			}
			this.pendingUpgradeChoices.Clear();
		}

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x0600226E RID: 8814 RVA: 0x0008AB5B File Offset: 0x00088D5B
		public bool HasPendingUpgrades
		{
			get
			{
				return this.pendingUpgradeChoices.Count > 0;
			}
		}

		// Token: 0x04001C2D RID: 7213
		private int[] _claimedPackageCounts = new int[9];

		// Token: 0x04001C2F RID: 7215
		private int[] _consecutiveWeeksSinceUpgradeLastPresented = new int[9];

		// Token: 0x04001C30 RID: 7216
		[Serialize(false, null)]
		public int[] timesUpgradePresented = new int[9];

		// Token: 0x04001C32 RID: 7218
		public List<UpgradeChoice> pendingUpgradeChoices = new List<UpgradeChoice>();

		// Token: 0x04001C33 RID: 7219
		public int numChoicesMade;

		// Token: 0x04001C34 RID: 7220
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001C35 RID: 7221
		[Serialize(true, null)]
		public Fix64 accumulatedUpgradeScheduleDelayTime = Fix64Consts.Zero;

		// Token: 0x04001C36 RID: 7222
		[Serialize(true, null)]
		public bool upgradeSchedulePaused;
	}
}
