using System;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;

namespace Motorways.Processes
{
	// Token: 0x020004BB RID: 1211
	public class UpgradeAwardingProcess : IProcess, IReusable
	{
		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06001F92 RID: 8082 RVA: 0x0007B515 File Offset: 0x00079715
		private GameRules _rules
		{
			get
			{
				return this._city.Rules;
			}
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x0007B524 File Offset: 0x00079724
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			if (this._upgrades.upgradeSchedulePaused)
			{
				this._upgrades.accumulatedUpgradeScheduleDelayTime += deltaTime;
			}
			int requiredUpgradeCount = this._rules.GetExpectedUpgradePackageCount(this._clock.ExpansionTime - this._upgrades.accumulatedUpgradeScheduleDelayTime) - this._upgrades.TotalGrantedUpgradesCount;
			if (requiredUpgradeCount > 0)
			{
				this.GrantUpgradeChoice(requiredUpgradeCount);
			}
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x0007B594 File Offset: 0x00079794
		public void GrantUpgradeChoice(int requiredUpgradeCount)
		{
			UpgradeAwardingProcess.Log.Info("Granting upgrade choice", Array.Empty<object>());
			for (int upgradeCount = 0; upgradeCount < requiredUpgradeCount; upgradeCount++)
			{
				bool hasChoices = false;
				UpgradeChoice upgradeChoices = this._behaviour.GenerateNextUpgradeChoices();
				if (upgradeChoices.choices.Count > 0)
				{
					this._upgrades.AddPendingUpgradeChoice(upgradeChoices);
					hasChoices = true;
				}
				if (!hasChoices)
				{
					this._upgrades.ApplyUpgradePackage(new UpgradePackageDefinition
					{
						type = UpgradeType.Concrete,
						amount = 0,
						additionalConcrete = 0
					}, false);
				}
			}
		}

		// Token: 0x04001A33 RID: 6707
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("UpgradeAwardingProcess");

		// Token: 0x04001A34 RID: 6708
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001A35 RID: 6709
		[Dependency]
		private UpgradeDatabaseModel _upgrades;

		// Token: 0x04001A36 RID: 6710
		[Dependency]
		private City _city;

		// Token: 0x04001A37 RID: 6711
		[Dependency]
		private GameBehaviourModel _behaviour;
	}
}
