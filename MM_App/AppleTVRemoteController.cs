using System;
using Factory;
using Motorways;
using Motorways.Actions;
using Motorways.Views;

// Token: 0x0200019D RID: 413
public class AppleTVRemoteController : GenericGamepadController, IAppleTVRemoteController, IController
{
	// Token: 0x06000946 RID: 2374 RVA: 0x0001E0C0 File Offset: 0x0001C2C0
	public override void RegisterInputActionsForApp(IScope appScope)
	{
		this._inputState.EnsurePollingAxis(0);
		this._inputState.EnsurePollingAxis(1);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(0, InputEventButtonState.JustDown), (PlayerActionGroup playerActionGroup, IScope scope, float time) => this.menuNavigator.CreateNavigateInDirection(0, 1, playerActionGroup, scope, time), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(1, InputEventButtonState.JustDown), (PlayerActionGroup playerActionGroup, IScope scope, float time) => this.menuNavigator.CreateNavigateInDirection(0, 1, playerActionGroup, scope, time), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateAccept), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(7, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.HandleNavigateBack), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(6, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateLeftAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(4, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateRightAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(5, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateDownAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(3, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateUpAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(29, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateLeftAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(27, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateRightAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(28, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateDownAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(26, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateUpAction), appScope, false);
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x0001E2AC File Offset: 0x0001C4AC
	public override void RegisterInputActionsForGame(IScope gameScope)
	{
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(16, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateToggleSpeed), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.HandleActivateSelected), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(2, InputEventButtonState.DoubleTapDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleDrawModeAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(8, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.HandleNavigateBackOrCancel), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(0, InputEventButtonState.Axis), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragMoveInGameFocusAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(1, InputEventButtonState.Axis), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragMoveInGameFocusAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(18, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleDragClearTileAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.Motorway, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragMotorwayAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.TrafficLight, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragTrafficLightAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.Roundabout, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragRoundaboutAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.MotorwayHandle, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragMotorwayHandleAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.House, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragHouseAction.CreateFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.Destination, InputEventButtonState.JustDown), (PlayerActionGroup owningGroup, IScope scope, float timestamp) => ControllerDragDestinationAction.CreateSingleFromUpgradeMenu(owningGroup, scope, timestamp), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.DoubleDestination, InputEventButtonState.JustDown), (PlayerActionGroup owningGroup, IScope scope, float timestamp) => ControllerDragDestinationAction.CreateDoubleFromUpgradeMenu(owningGroup, scope, timestamp), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.MoveCreativeModeObject, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragCreativeModeEditableObjectAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleCreativeModeEditMenuAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(31, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleZoomAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(29, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateSlowDown), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateRemoteEventFilter(27, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateSpeedUp), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateRemoteUIEventFilter(2, GameUIButtonType.EditMenuOpened, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(RemoteEditMenuNavigateAction.Create), gameScope, false);
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06000948 RID: 2376 RVA: 0x0001E588 File Offset: 0x0001C788
	public override string DeviceName
	{
		get
		{
			return "Apple TV Remote";
		}
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x00016FED File Offset: 0x000151ED
	public override InputEventSource GetInputSource()
	{
		return InputEventSource.Remote;
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x0001E58F File Offset: 0x0001C78F
	public PlayerAction HandleNavigateBack(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		this._playerActionController.CancelAllActions();
		return this.menuNavigator.CreateNavigateBack(playerActionGroup, scope, time);
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0001E5AA File Offset: 0x0001C7AA
	public PlayerAction HandleNavigateBackOrCancel(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		if (scope.Get<GameUIScreen>().CurrentRoadDrawMode == RoadDrawMode.Remove)
		{
			return ToggleDrawModeAction.Create(playerActionGroup, scope, time);
		}
		return this.HandleNavigateBack(playerActionGroup, scope, time);
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x0001E5CC File Offset: 0x0001C7CC
	protected override MotorwaysPlayerAction ControllerDrawRoadAction(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		return Motorways.Actions.ControllerDrawRoadAction.Create(owningGroup, scope, timestamp);
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0001E5D6 File Offset: 0x0001C7D6
	protected override MotorwaysPlayerAction ControllerDeleteRoadAction(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		return ToggleDragClearTileAction.Create(owningGroup, scope, timestamp);
	}

	// Token: 0x040004CC RID: 1228
	public new static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("AppleTVRemoteController");
}
