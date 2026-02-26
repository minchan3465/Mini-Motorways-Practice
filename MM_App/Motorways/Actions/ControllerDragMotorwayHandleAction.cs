using System;
using Factory;

namespace Motorways.Actions
{
	// Token: 0x020006F8 RID: 1784
	public class ControllerDragMotorwayHandleAction : DragMotorwayHandleAction
	{
		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x060030DA RID: 12506 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x000E59D0 File Offset: 0x000E3BD0
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this._gameUI.SetFocusPointActive(false, false);
			this._gameUI.SetFocusPointBlocked(true);
		}

		// Token: 0x060030DC RID: 12508 RVA: 0x000E59F2 File Offset: 0x000E3BF2
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			this.ResetFocusPoint();
		}

		// Token: 0x060030DD RID: 12509 RVA: 0x000E5A00 File Offset: 0x000E3C00
		public override void OnActionComplete()
		{
			base.OnActionComplete();
			this.ResetFocusPoint();
		}

		// Token: 0x060030DE RID: 12510 RVA: 0x000E5A10 File Offset: 0x000E3C10
		private void ResetFocusPoint()
		{
			this._gameUI.SetFocusPointBlocked(false);
			this._gameUI.SetFocusPointPosition(this._camera.GetScreenFromWorld(this._motorwayView.HandlePosition));
			this._gameUI.SetFocusPointActive(true, false);
		}

		// Token: 0x060030DF RID: 12511 RVA: 0x000E5A5C File Offset: 0x000E3C5C
		public new static ControllerDragMotorwayHandleAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ControllerDragMotorwayHandleAction controllerDragMotorwayHandleAction = scope.Get<ControllerDragMotorwayHandleAction>();
			controllerDragMotorwayHandleAction.InitializeAction(owningGroup, timestamp);
			MotorwaysUIInputEvent inputEvent = owningGroup.InstigatingInputEvent as MotorwaysUIInputEvent;
			controllerDragMotorwayHandleAction._editedMotorwayId = inputEvent.UIButtonIndex;
			controllerDragMotorwayHandleAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragMotorwayHandleAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragMotorwayHandleAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragMotorwayHandleAction.OnActionBegin(timestamp);
			return controllerDragMotorwayHandleAction;
		}

		// Token: 0x040029FC RID: 10748
		[Dependency]
		private GameCamera _camera;
	}
}
