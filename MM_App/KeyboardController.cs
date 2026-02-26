using System;
using Factory;
using Motorways.Actions;

// Token: 0x020001A3 RID: 419
public class KeyboardController : BaseController, IKeyboardController, IController
{
	// Token: 0x06000963 RID: 2403 RVA: 0x0001ED00 File Offset: 0x0001CF00
	public override void RegisterInputActionsForApp(IScope appScope)
	{
		base.RegisterInputActionsForApp(appScope);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(6, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateLeftAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(4, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateRightAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(5, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateDownAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(3, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateUpAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateAccept), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(7, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateBack), appScope, false);
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x0001EE00 File Offset: 0x0001D000
	public override void RegisterInputActionsForGame(IScope gameScope)
	{
		base.RegisterInputActionsForGame(gameScope);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(13, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreatePauseSpeed), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(14, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreatePlaySpeed), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(15, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateFastForwardSpeed), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(45, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateExtraFastForwardSpeed), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(16, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateToggleSpeed), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(11, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateSlowDown), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(10, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateSpeedUp), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(21, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeUpgradeBarAction.CreateShowOrLockUpgradeBar), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(22, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeUpgradeBarAction.CreateHideUpgradeBar), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(9, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleDrawModeAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(40, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleZoomAction.CreateZoomIn), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(41, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleZoomAction.CreateZoomOut), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateKeyboardEventFilter(44, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(OpenElectiveUpgradeScreenAction.Create), gameScope, false);
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06000965 RID: 2405 RVA: 0x0001EFCE File Offset: 0x0001D1CE
	public override string DeviceName
	{
		get
		{
			return "Keyboard";
		}
	}

	// Token: 0x040004EF RID: 1263
	[Dependency]
	protected MenuNavigation menuNavigator;
}
