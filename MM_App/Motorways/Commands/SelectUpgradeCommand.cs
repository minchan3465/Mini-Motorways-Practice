using System;
using Factory;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Views;
using Server;

namespace Motorways.Commands
{
	// Token: 0x02000523 RID: 1315
	public class SelectUpgradeCommand : Command
	{
		// Token: 0x060022C3 RID: 8899 RVA: 0x0008C98C File Offset: 0x0008AB8C
		public override void Execute(ISimulation simulation)
		{
			if (!Diagnostics.Verify(this._upgradeIndex >= 0, "Upgrade index must be greater than zero! Not {0}", this._upgradeIndex))
			{
				return;
			}
			if (!Diagnostics.Verify(this._upgradeDatabase.pendingUpgradeChoices.Count > 0, "There are no upgrade choices available! Cancelling SelectUpgradeCommand"))
			{
				return;
			}
			this._upgradeDatabase.numChoicesMade--;
			UpgradeChoice upgrade = this._upgradeDatabase.pendingUpgradeChoices[0];
			this._upgradeDatabase.pendingUpgradeChoices.RemoveAt(0);
			if (Diagnostics.Verify(upgrade.choices.Count > this._upgradeIndex, "Index {0} is not a valid choice here!", this._upgradeIndex))
			{
				UpgradePackageDefinition upgradePackage = upgrade.choices[this._upgradeIndex];
				Command.Log.Info("Choosing upgrade {0}, index {1}", new object[]
				{
					upgradePackage.type,
					this._upgradeIndex
				});
				this._upgradeDatabase.ApplyUpgradePackage(upgradePackage, upgrade.isFree);
				simulation.Scope.Get<UpgradeBarClient>().AddPendingToUpgradeButtonStack(upgradePackage.type, upgradePackage.amount);
				if (upgradePackage.additionalConcrete > 0)
				{
					simulation.Scope.Get<UpgradeBarClient>().AddPendingToUpgradeButtonStack(UpgradeType.Concrete, upgradePackage.additionalConcrete);
				}
			}
			this._scope.Release(upgrade);
		}

		// Token: 0x060022C4 RID: 8900 RVA: 0x0008CADE File Offset: 0x0008ACDE
		public override void Reset()
		{
			base.Reset();
			this._upgradeIndex = 0;
		}

		// Token: 0x060022C5 RID: 8901 RVA: 0x0008CAED File Offset: 0x0008ACED
		public static SelectUpgradeCommand Create(IScope scope, int upgradeIndex)
		{
			SelectUpgradeCommand selectUpgradeCommand = scope.Get<SelectUpgradeCommand>();
			selectUpgradeCommand._upgradeIndex = upgradeIndex;
			return selectUpgradeCommand;
		}

		// Token: 0x04001CD1 RID: 7377
		[Dependency]
		private IScope _scope;

		// Token: 0x04001CD2 RID: 7378
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabase;

		// Token: 0x04001CD3 RID: 7379
		private int _upgradeIndex;
	}
}
