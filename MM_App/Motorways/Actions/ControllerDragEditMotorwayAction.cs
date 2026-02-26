using System;
using Factory;

namespace Motorways.Actions
{
	// Token: 0x020006F5 RID: 1781
	public class ControllerDragEditMotorwayAction : DragEditMotorwayAction
	{
		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x060030C6 RID: 12486 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030C7 RID: 12487 RVA: 0x000E5558 File Offset: 0x000E3758
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			PlayerAction.Log.Info("Beginning drag edit!", Array.Empty<object>());
		}

		// Token: 0x060030C8 RID: 12488 RVA: 0x000E5578 File Offset: 0x000E3778
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (base.ActionState != PlayerAction.State.Begun)
			{
				PlayerAction.Log.Info("Completing drag edit!", Array.Empty<object>());
				this.OnActionCancel();
				return;
			}
			if (inputEvent.InputAction == 2)
			{
				PlayerAction.Log.Info("Completing drag edit!", Array.Empty<object>());
				this.OnActionComplete();
				return;
			}
			PlayerAction.Log.Info("Cancelling drag edit!", Array.Empty<object>());
			this.OnActionCancel();
		}

		// Token: 0x060030C9 RID: 12489 RVA: 0x000E55E8 File Offset: 0x000E37E8
		public new static ControllerDragEditMotorwayAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ControllerDragEditMotorwayAction controllerDragEditMotorwayAction = scope.Get<ControllerDragEditMotorwayAction>();
			controllerDragEditMotorwayAction.InitializeAction(owningGroup, timestamp);
			controllerDragEditMotorwayAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragEditMotorwayAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragEditMotorwayAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragEditMotorwayAction.OnActionBegin(timestamp);
			return controllerDragEditMotorwayAction;
		}
	}
}
