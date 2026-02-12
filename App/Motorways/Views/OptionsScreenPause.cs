using System;
using Factory;

namespace Motorways.Views
{
	// Token: 0x02000560 RID: 1376
	public class OptionsScreenPause : OptionsScreenBase
	{
		// Token: 0x0600253A RID: 9530 RVA: 0x0009CB5D File Offset: 0x0009AD5D
		public override void OnTransitionedIn()
		{
			base.OnTransitionedIn();
			this._appScope.Get<InputState>().BlockGameInput = true;
			this._playerActionController.CancelAllActions();
		}

		// Token: 0x04001F5F RID: 8031
		[Dependency]
		protected PlayerActionController _playerActionController;
	}
}
