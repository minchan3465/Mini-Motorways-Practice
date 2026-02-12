using System;
using Factory;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006FD RID: 1789
	public class ControllerEditMenuNavigateAction : EditMenuNavigateAction
	{
		// Token: 0x060030FC RID: 12540 RVA: 0x000E63D4 File Offset: 0x000E45D4
		protected override void OnTick()
		{
			Vector2 direction = this.GetMoveFocusJoystickInputValue();
			if (direction.magnitude < 0.6f)
			{
				return;
			}
			this.EditMenu.SelectButtonAtDirection(direction);
		}

		// Token: 0x060030FD RID: 12541 RVA: 0x000E6403 File Offset: 0x000E4603
		public static ControllerEditMenuNavigateAction Create(PlayerActionGroup playerActionGroup, IScope scope, float timestamp)
		{
			ControllerEditMenuNavigateAction controllerEditMenuNavigateAction = scope.Get<ControllerEditMenuNavigateAction>();
			controllerEditMenuNavigateAction.InitializeAction(playerActionGroup, timestamp);
			controllerEditMenuNavigateAction.OnActionBegin(timestamp);
			return controllerEditMenuNavigateAction;
		}
	}
}
