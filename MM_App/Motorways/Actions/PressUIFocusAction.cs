using System;
using Factory;
using UnityEngine.EventSystems;

namespace Motorways.Actions
{
	// Token: 0x02000712 RID: 1810
	public class PressUIFocusAction : MotorwaysPlayerAction
	{
		// Token: 0x060031C7 RID: 12743 RVA: 0x000EBC1C File Offset: 0x000E9E1C
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			if (this._gameUI.FocussedSelectable != null)
			{
				ControllerInputEventData inputEventData = new ControllerInputEventData(EventSystem.current, this.onController);
				this._gameUI.FocussedSelectable.OnSubmit(inputEventData);
			}
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x000020A2 File Offset: 0x000002A2
		public override void Tick(float frameTime)
		{
			this.OnActionComplete();
		}

		// Token: 0x060031C9 RID: 12745 RVA: 0x000EBC5F File Offset: 0x000E9E5F
		public static PressUIFocusAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp, IController controller)
		{
			PressUIFocusAction pressUIFocusAction = scope.Get<PressUIFocusAction>();
			pressUIFocusAction.onController = controller;
			pressUIFocusAction.InitializeAction(owningGroup, timestamp);
			pressUIFocusAction.OnActionBegin(timestamp);
			return pressUIFocusAction;
		}

		// Token: 0x04002AB3 RID: 10931
		protected IController onController;
	}
}
