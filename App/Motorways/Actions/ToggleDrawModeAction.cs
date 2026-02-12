using System;
using Factory;

namespace Motorways.Actions
{
	// Token: 0x02000716 RID: 1814
	public class ToggleDrawModeAction : MotorwaysPlayerAction
	{
		// Token: 0x060031E0 RID: 12768 RVA: 0x000EC31C File Offset: 0x000EA51C
		public override void OnActionBegin(float timestamp)
		{
			this._playerActionController.CancelAllActions();
			this.SetColourWidgetRadialVisible(false);
			base.OnActionBegin(timestamp);
			this._gameUI.ToggleDrawMode();
		}

		// Token: 0x060031E1 RID: 12769 RVA: 0x000020A2 File Offset: 0x000002A2
		public override void Tick(float frameTime)
		{
			this.OnActionComplete();
		}

		// Token: 0x060031E2 RID: 12770 RVA: 0x000EC344 File Offset: 0x000EA544
		public static ToggleDrawModeAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ToggleDrawModeAction newAction = scope.Get<ToggleDrawModeAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Keyboard && !scope.Get<ActivePlayer>().IsDrawModeToggleEnabled)
			{
				newAction.OnActionCancel();
			}
			else
			{
				newAction.OnActionBegin(timestamp);
			}
			return newAction;
		}

		// Token: 0x04002ABB RID: 10939
		[Dependency]
		private PlayerActionController _playerActionController;
	}
}
