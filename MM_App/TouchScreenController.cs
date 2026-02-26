using System;
using Factory;
using Motorways;
using Motorways.Actions;

// Token: 0x020001A9 RID: 425
public class TouchScreenController : ITouchScreenController, IController
{
	// Token: 0x06000975 RID: 2421 RVA: 0x000022F5 File Offset: 0x000004F5
	public void RegisterInputActionsForApp(IScope appScope)
	{
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0001F434 File Offset: 0x0001D634
	public void RegisterInputActionsForGame(IScope gameScope)
	{
		this._playerActionController.RegisterAction(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleCreativeModeEditMenuAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(TouchCameraAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DrawRoadAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragEditMotorwayAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.Motorway, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragMotorwayAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.TrafficLight, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragTrafficLightAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.Roundabout, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragRoundaboutAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.MotorwayHandle, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragMotorwayHandleAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.House, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragHouseAction.CreateFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.Destination, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragDestinationAction.CreateSingleFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.DoubleDestination, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragDestinationAction.CreateDoubleFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(0, GameUIButtonType.MoveCreativeModeObject, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragCreativeModeEditableObjectAction.Create), gameScope, false);
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnControllerConnected()
	{
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnControllerDisconnected()
	{
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x000022F5 File Offset: 0x000004F5
	public void EnsureActionsAreRegistered(IScope scope)
	{
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x000020AA File Offset: 0x000002AA
	public virtual InputEventSource GetInputSource()
	{
		return InputEventSource.Touch;
	}

	// Token: 0x040004FD RID: 1277
	[Dependency]
	protected PlayerActionController _playerActionController;
}
