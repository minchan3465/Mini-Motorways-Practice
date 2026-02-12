using System;
using Factory;
using Motorways;
using Motorways.Actions;

// Token: 0x020001A7 RID: 423
public class MouseController : BaseController, IMouseController, IController
{
	// Token: 0x17000210 RID: 528
	// (get) Token: 0x0600096D RID: 2413 RVA: 0x0001F0FC File Offset: 0x0001D2FC
	public override string DeviceName
	{
		get
		{
			return "Mouse";
		}
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x0001F103 File Offset: 0x0001D303
	public override void RegisterInputActionsForApp(IScope appScope)
	{
		base.RegisterInputActionsForApp(appScope);
		this._inputState.EnsurePollingRewiredAction(19);
		this._inputState.EnsurePollingRewiredAction(20);
		this._inputState.EnsurePollingRewiredAction(25);
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0001F134 File Offset: 0x0001D334
	public override void RegisterInputActionsForGame(IScope gameScope)
	{
		base.RegisterInputActionsForGame(gameScope);
		this._playerActionController.RegisterAction(InputEventFilter.CreateMouseEventFilter(19, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DrawRoadAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateMouseEventFilter(19, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleCreativeModeEditMenuAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateMouseEventFilter(20, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragClearTileAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateMouseEventFilter(19, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragEditMotorwayAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateMouseEventFilter(30, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(MouseCameraAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.Motorway, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragMotorwayAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.TrafficLight, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragTrafficLightAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.Roundabout, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragRoundaboutAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.MotorwayHandle, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragMotorwayHandleAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.House, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragHouseAction.CreateFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.Destination, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragDestinationAction.CreateSingleFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.DoubleDestination, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragDestinationAction.CreateDoubleFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateMouseUIEventFilter(19, GameUIButtonType.MoveCreativeModeObject, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragCreativeModeEditableObjectAction.Create), gameScope, false);
	}
}
