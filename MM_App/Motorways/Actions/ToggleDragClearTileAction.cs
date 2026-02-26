using System;
using Factory;

namespace Motorways.Actions
{
	// Token: 0x02000715 RID: 1813
	public class ToggleDragClearTileAction : DragClearTileAction
	{
		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x060031DC RID: 12764 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060031DD RID: 12765 RVA: 0x000EC2A0 File Offset: 0x000EA4A0
		public override void OnActionBegin(float timestamp)
		{
			if (this._gameUI.CurrentRoadDrawMode != RoadDrawMode.Remove)
			{
				this.OnActionCancel();
				return;
			}
			base.OnActionBegin(timestamp);
		}

		// Token: 0x060031DE RID: 12766 RVA: 0x000EC2C0 File Offset: 0x000EA4C0
		public new static ToggleDragClearTileAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ToggleDragClearTileAction toggleDragClearTileAction = scope.Get<ToggleDragClearTileAction>();
			toggleDragClearTileAction.InitializeAction(owningGroup, timestamp);
			toggleDragClearTileAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			toggleDragClearTileAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			toggleDragClearTileAction.OnActionBegin(timestamp);
			return toggleDragClearTileAction;
		}
	}
}
