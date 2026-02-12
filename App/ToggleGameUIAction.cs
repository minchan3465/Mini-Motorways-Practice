using System;
using Factory;
using Motorways.Actions;

// Token: 0x02000192 RID: 402
public class ToggleGameUIAction : MotorwaysPlayerAction
{
	// Token: 0x06000916 RID: 2326 RVA: 0x0001DA5C File Offset: 0x0001BC5C
	public override void OnActionBegin(float timestamp)
	{
		bool flipTo = !this._gameUI.IsUiVisible;
		this._gameUI.SetUIVisible(flipTo, false, true, false);
		this._gameUI.SetDrawButtonsVisible(flipTo);
		this._gameUI.SetFocusPointActive(flipTo, false);
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x000020A2 File Offset: 0x000002A2
	public override void Tick(float frameTime)
	{
		this.OnActionComplete();
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x0001DAA0 File Offset: 0x0001BCA0
	public static ToggleGameUIAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		ToggleGameUIAction toggleGameUIAction = scope.Get<ToggleGameUIAction>();
		toggleGameUIAction.InitializeAction(owningGroup, timestamp);
		toggleGameUIAction.OnActionBegin(timestamp);
		return toggleGameUIAction;
	}
}
