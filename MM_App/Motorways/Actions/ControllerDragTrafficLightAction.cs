using System;
using Factory;
using Motorways.UI;

namespace Motorways.Actions
{
	// Token: 0x020006FA RID: 1786
	public class ControllerDragTrafficLightAction : DragTrafficLightAction
	{
		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x060030E7 RID: 12519 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030E8 RID: 12520 RVA: 0x000E5B74 File Offset: 0x000E3D74
		protected override void InitializeUpgradeCursor()
		{
			base.InitializeUpgradeCursor();
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x060030E9 RID: 12521 RVA: 0x000E5AEB File Offset: 0x000E3CEB
		protected override void UpdateUpgradeCursorPosition()
		{
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x060030EA RID: 12522 RVA: 0x000E5B04 File Offset: 0x000E3D04
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.InputAction == 2)
			{
				this.OnActionComplete();
				return;
			}
			this.OnActionCancel();
		}

		// Token: 0x060030EB RID: 12523 RVA: 0x000E5B94 File Offset: 0x000E3D94
		public new static ControllerDragTrafficLightAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ControllerDragTrafficLightAction controllerDragTrafficLightAction = scope.Get<ControllerDragTrafficLightAction>();
			controllerDragTrafficLightAction.InitializeAction(owningGroup, timestamp);
			controllerDragTrafficLightAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragTrafficLightAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragTrafficLightAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragTrafficLightAction.OnActionBegin(timestamp);
			return controllerDragTrafficLightAction;
		}
	}
}
