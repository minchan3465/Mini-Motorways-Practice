using System;
using Factory;
using Motorways;
using Motorways.Actions;
using Motorways.Views;

// Token: 0x020001A1 RID: 417
public class GenericGamepadController : BaseController, IGamepadController, IController
{
	// Token: 0x06000956 RID: 2390 RVA: 0x0001E62C File Offset: 0x0001C82C
	public override void RegisterInputActionsForApp(IScope appScope)
	{
		base.RegisterInputActionsForApp(appScope);
		if (FeatureToggle.IsFeatureEnabled(Feature.MockControllerAsRemote))
		{
			this._inputState.IgnorePollingAxis(0);
			this._inputState.IgnorePollingAxis(1);
		}
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(6, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateLeftAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(4, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateRightAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(5, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateDownAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(3, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateUpAction), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateAccept), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(7, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigateBack), appScope, false);
		this._inputState.EnsurePollingAxis(0);
		this._inputState.EnsurePollingAxis(1);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(0, InputEventButtonState.JustDown), (PlayerActionGroup playerActionGroup, IScope scope, float time) => this.menuNavigator.CreateNavigateInDirection(0, 1, playerActionGroup, scope, time), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(1, InputEventButtonState.JustDown), (PlayerActionGroup playerActionGroup, IScope scope, float time) => this.menuNavigator.CreateNavigateInDirection(0, 1, playerActionGroup, scope, time), appScope, false);
		if (FeatureToggle.IsFeatureEnabled(Feature.CycleLanguages))
		{
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(37, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(SetLanguageAction.CreateCycleForwardSetLanguageAction), appScope, false);
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(36, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(SetLanguageAction.CreateCycleBackwardSetLanguageAction), appScope, false);
		}
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(42, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigatePageLeft), appScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(43, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.menuNavigator.CreateNavigatePageRight), appScope, false);
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x0001E844 File Offset: 0x0001CA44
	public override void RegisterInputActionsForGame(IScope gameScope)
	{
		base.RegisterInputActionsForGame(gameScope);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(16, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateToggleSpeed), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(11, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateSlowDown), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(10, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeGameSpeedAction.CreateSpeedUp), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(9, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleDrawModeAction.Create), gameScope, false);
		if (FeatureToggle.IsFeatureEnabled(Feature.ToggleGameUIWithController))
		{
			this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(32, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleGameUIAction.Create), gameScope, false);
		}
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(31, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleZoomAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(34, InputEventButtonState.Axis), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerCameraAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(33, InputEventButtonState.Axis), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerCameraAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(this.HandleActivateSelected), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(2, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ToggleCreativeModeEditMenuAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(0, InputEventButtonState.Axis), new Func<PlayerActionGroup, IScope, float, PlayerAction>(MoveInGameFocusAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(1, InputEventButtonState.Axis), new Func<PlayerActionGroup, IScope, float, PlayerAction>(MoveInGameFocusAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(21, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeUpgradeBarAction.CreateShowOrLockUpgradeBar), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(22, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ChangeUpgradeBarAction.CreateHideUpgradeBar), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(18, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragClearTileAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.Motorway, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragMotorwayAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.TrafficLight, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragTrafficLightAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.Roundabout, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragRoundaboutAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.MotorwayHandle, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragMotorwayHandleAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.House, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerDragHouseAction.CreateFromUpgradeMenu), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.Destination, InputEventButtonState.JustDown), (PlayerActionGroup owningGroup, IScope scope, float timestamp) => ControllerDragDestinationAction.CreateSingleFromUpgradeMenu(owningGroup, scope, timestamp), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.DoubleDestination, InputEventButtonState.JustDown), (PlayerActionGroup owningGroup, IScope scope, float timestamp) => ControllerDragDestinationAction.CreateDoubleFromUpgradeMenu(owningGroup, scope, timestamp), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.EditMenuOpened, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(ControllerEditMenuNavigateAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(MotorwaysUIInputEventFilter.CreateGenericUIEventFilter(2, GameUIButtonType.MoveCreativeModeObject, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(DragCreativeModeEditableObjectAction.Create), gameScope, false);
		this._playerActionController.RegisterAction(InputEventFilter.CreateGenericEventFilter(44, InputEventButtonState.JustDown), new Func<PlayerActionGroup, IScope, float, PlayerAction>(OpenElectiveUpgradeScreenAction.Create), gameScope, false);
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06000958 RID: 2392 RVA: 0x0001EBDC File Offset: 0x0001CDDC
	public override string DeviceName
	{
		get
		{
			return "Gamepad";
		}
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x0001EBE4 File Offset: 0x0001CDE4
	public virtual PlayerAction HandleActivateSelected(PlayerActionGroup playerActionGroup, IScope scope, float time)
	{
		if (!this._playerActionController.TutorialBlockInputFlag && this.menuNavigator.ControllerState == MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles)
		{
			GameUIScreen gameUI = scope.Get<GameUIScreen>();
			TilemapView tilemapView = scope.Get<TilemapView>();
			if (gameUI != null)
			{
				if (gameUI.FocussedSelectable != null)
				{
					return PressUIFocusAction.Create(playerActionGroup, scope, time, this);
				}
				if (gameUI.CurrentRoadDrawMode == RoadDrawMode.Add)
				{
					Tile tile = tilemapView.GetTile(tilemapView.GetTileCoordinatesFromScreenPosition(gameUI.FocusPointPosition));
					if (tile != null)
					{
						TileDirectionBitfield motorwayRamps = tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active);
						if (motorwayRamps.Count > 0 || tile.UnbuiltMotorwayId != -1)
						{
							foreach (TileDirection motorwayDirection in motorwayRamps)
							{
								if (!tilemapView.GetMotorway(tile.GetMotorwayInDirection(motorwayDirection, RoadState.Planned | RoadState.Active)).IsPermanent)
								{
									return ControllerDragEditMotorwayAction.Create(playerActionGroup, scope, time);
								}
							}
						}
					}
					return this.ControllerDrawRoadAction(playerActionGroup, scope, time);
				}
				return this.ControllerDeleteRoadAction(playerActionGroup, scope, time);
			}
		}
		return this.menuNavigator.CreateNavigateAccept(playerActionGroup, scope, time);
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x0001ECD7 File Offset: 0x0001CED7
	protected virtual MotorwaysPlayerAction ControllerDrawRoadAction(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		return DrawRoadAction.Create(owningGroup, scope, timestamp);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x0001ECE1 File Offset: 0x0001CEE1
	protected virtual MotorwaysPlayerAction ControllerDeleteRoadAction(PlayerActionGroup owningGroup, IScope scope, float timestamp)
	{
		return DragClearTileAction.Create(owningGroup, scope, timestamp);
	}

	// Token: 0x040004EB RID: 1259
	[Dependency]
	protected MotorwaysInGameStateToggleController menuNavigator;
}
