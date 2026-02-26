using System;
using Factory;
using Motorways.Actions;

// Token: 0x020001A8 RID: 424
public class SwitchGamepadController : GenericGamepadController
{
	// Token: 0x06000971 RID: 2417 RVA: 0x0001F310 File Offset: 0x0001D510
	public override void RegisterInputActionsForApp(IScope appScope)
	{
		base.RegisterInputActionsForApp(appScope);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(12, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.HandleActivateControllerSelect), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(8, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateBack), appScope, false);
		if (FeatureToggle.IsFeatureEnabled(Feature.CycleLanguages))
		{
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(37, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(SetLanguageAction.CreateCycleForwardSetLanguageAction), appScope, false);
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(36, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(SetLanguageAction.CreateCycleBackwardSetLanguageAction), appScope, false);
		}
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0001F3BC File Offset: 0x0001D5BC
	public override void RegisterInputActionsForGame(IScope gameScope)
	{
		base.RegisterInputActionsForGame(gameScope);
		if (FeatureToggle.IsFeatureEnabled(Feature.ToggleGameUIWithController))
		{
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(32, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleGameUIAction.Create), gameScope, false);
		}
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(31, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleZoomAction.Create), gameScope, false);
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0001F41D File Offset: 0x0001D61D
	public virtual PlayerAction HandleActivateControllerSelect(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		ActivateControllerSelectAction activateControllerSelectAction = scope.Get<ActivateControllerSelectAction>();
		activateControllerSelectAction.InitializeAction(playerActionGroup, time);
		activateControllerSelectAction.OnActionBegin(time);
		return activateControllerSelectAction;
	}
}
