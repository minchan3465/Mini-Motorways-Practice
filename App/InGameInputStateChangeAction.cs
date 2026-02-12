using System;
using Factory;

// Token: 0x0200018F RID: 399
public class InGameInputStateChangeAction : PlayerAction
{
	// Token: 0x0600090B RID: 2315 RVA: 0x0001D925 File Offset: 0x0001BB25
	public override void OnActionBegin(float timestamp)
	{
		base.OnActionBegin(timestamp);
		this._motorwaysController.SwitchToState(this._action, base.Scope, this._swapActionBehaviour);
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x000020A2 File Offset: 0x000002A2
	public override void Tick(float frameTime)
	{
		this.OnActionComplete();
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x0001D94B File Offset: 0x0001BB4B
	public static InGameInputStateChangeAction CreateSwitchToState(PlayerActionGroup owningGroup, IScope scope, float timestamp, MotorwaysInGameStateToggleController.InGameControllerState stateChangeAction, MotorwaysInGameStateToggleController.StateSwapActionBehaviour swapActionBehaviour = MotorwaysInGameStateToggleController.StateSwapActionBehaviour.MaintainActions)
	{
		InGameInputStateChangeAction inGameInputStateChangeAction = scope.Get<InGameInputStateChangeAction>();
		inGameInputStateChangeAction._action = stateChangeAction;
		inGameInputStateChangeAction._swapActionBehaviour = swapActionBehaviour;
		inGameInputStateChangeAction.Scope = scope;
		inGameInputStateChangeAction.InitializeAction(owningGroup, timestamp);
		inGameInputStateChangeAction.OnActionBegin(timestamp);
		return inGameInputStateChangeAction;
	}

	// Token: 0x04000484 RID: 1156
	[Dependency]
	protected MotorwaysInGameStateToggleController _motorwaysController;

	// Token: 0x04000485 RID: 1157
	protected MotorwaysInGameStateToggleController.InGameControllerState _action;

	// Token: 0x04000486 RID: 1158
	protected MotorwaysInGameStateToggleController.StateSwapActionBehaviour _swapActionBehaviour;
}
