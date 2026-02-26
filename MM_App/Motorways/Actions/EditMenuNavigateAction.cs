using System;
using Motorways.UI;

namespace Motorways.Actions
{
	// Token: 0x0200070C RID: 1804
	public class EditMenuNavigateAction : MotorwaysPlayerAction
	{
		// Token: 0x06003197 RID: 12695 RVA: 0x000EAF9E File Offset: 0x000E919E
		public override void InitializeAction(PlayerActionGroup owningGroup, float timestamp)
		{
			base.InitializeAction(owningGroup, timestamp);
			base.MakeExclusive();
		}

		// Token: 0x06003198 RID: 12696 RVA: 0x000EAFB0 File Offset: 0x000E91B0
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this.EditMenu = base.Scope.Get<EditMenuPanel>();
			if (this.EditMenu == null || !this.EditMenu.IsOpen)
			{
				this.OnActionComplete();
				return;
			}
			this.OnTick();
		}

		// Token: 0x06003199 RID: 12697 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnTick()
		{
		}

		// Token: 0x04002A91 RID: 10897
		protected EditMenuPanel EditMenu;
	}
}
