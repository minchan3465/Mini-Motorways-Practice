using System;
using Factory;
using Motorways.Views;

namespace Motorways.Actions
{
	// Token: 0x02000711 RID: 1809
	public class OpenElectiveUpgradeScreenAction : MotorwaysPlayerAction
	{
		// Token: 0x060031C4 RID: 12740 RVA: 0x000EBBE9 File Offset: 0x000E9DE9
		public static OpenElectiveUpgradeScreenAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			OpenElectiveUpgradeScreenAction openElectiveUpgradeScreenAction = scope.Get<OpenElectiveUpgradeScreenAction>();
			openElectiveUpgradeScreenAction.InitializeAction(owningGroup, timestamp);
			openElectiveUpgradeScreenAction.OnActionBegin(timestamp);
			return openElectiveUpgradeScreenAction;
		}

		// Token: 0x060031C5 RID: 12741 RVA: 0x000EBC00 File Offset: 0x000E9E00
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this._gameUIScreen.OnElectiveUpgradeButtonPressed();
			this.OnActionComplete();
		}

		// Token: 0x04002AB2 RID: 10930
		[Dependency]
		private GameUIScreen _gameUIScreen;
	}
}
