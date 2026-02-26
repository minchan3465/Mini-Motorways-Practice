using System;
using Factory;
using Motorways.UI;

namespace Motorways.Actions
{
	// Token: 0x020006F9 RID: 1785
	public class ControllerDragRoundaboutAction : DragRoundaboutAction
	{
		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x060030E1 RID: 12513 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030E2 RID: 12514 RVA: 0x000E5ACC File Offset: 0x000E3CCC
		protected override void InitializeUpgradeCursor()
		{
			base.InitializeUpgradeCursor();
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x060030E3 RID: 12515 RVA: 0x000E5AEB File Offset: 0x000E3CEB
		protected override void UpdateUpgradeCursorPosition()
		{
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x060030E4 RID: 12516 RVA: 0x000E5B04 File Offset: 0x000E3D04
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.InputAction == 2)
			{
				this.OnActionComplete();
				return;
			}
			this.OnActionCancel();
		}

		// Token: 0x060030E5 RID: 12517 RVA: 0x000E5B1C File Offset: 0x000E3D1C
		public new static ControllerDragRoundaboutAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ControllerDragRoundaboutAction controllerDragRoundaboutAction = scope.Get<ControllerDragRoundaboutAction>();
			controllerDragRoundaboutAction.InitializeAction(owningGroup, timestamp);
			controllerDragRoundaboutAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragRoundaboutAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragRoundaboutAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragRoundaboutAction.OnActionBegin(timestamp);
			return controllerDragRoundaboutAction;
		}
	}
}
