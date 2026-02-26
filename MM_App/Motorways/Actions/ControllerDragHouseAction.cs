using System;
using Factory;
using Motorways.UI;
using Motorways.Views;

namespace Motorways.Actions
{
	// Token: 0x020006F6 RID: 1782
	public class ControllerDragHouseAction : DragHouseAction
	{
		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x060030CB RID: 12491 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030CC RID: 12492 RVA: 0x000E5640 File Offset: 0x000E3840
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			ICreativeModeEditableObject editableObject = base.Scope.Get<EditMenuPanel>().EditableObject;
			if (editableObject is CreativeModeEditableDestination || editableObject is CreativeModeEditableHouse)
			{
				this._gameUI.ConfirmEditMenuEdit();
			}
		}

		// Token: 0x060030CD RID: 12493 RVA: 0x000E5680 File Offset: 0x000E3880
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.InputAction == 2)
			{
				if (this.draftHouse == null || this.draftHouse.CompletelyOutOfPlayArea(this._city))
				{
					this.OnActionCancel();
					return;
				}
				this.OnActionComplete();
				return;
			}
			else
			{
				if (inputEvent.InputAction == 18 || inputEvent.InputAction == 7)
				{
					this.OnActionCancel();
					return;
				}
				PlayerAction.Log.Error(string.Format("Unexpected input: {0}!", inputEvent), Array.Empty<object>());
				this.OnActionCancel();
				return;
			}
		}

		// Token: 0x060030CE RID: 12494 RVA: 0x000E56FF File Offset: 0x000E38FF
		public new static ControllerDragHouseAction CreateFromUpgradeMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return ControllerDragHouseAction.Create(owningGroup, scope, timestamp, true);
		}

		// Token: 0x060030CF RID: 12495 RVA: 0x000E570A File Offset: 0x000E390A
		public new static ControllerDragHouseAction CreateFromEditMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return ControllerDragHouseAction.Create(owningGroup, scope, timestamp, false);
		}

		// Token: 0x060030D0 RID: 12496 RVA: 0x000E5718 File Offset: 0x000E3918
		public new static ControllerDragHouseAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp, bool fromUpgradeMenu)
		{
			ControllerDragHouseAction controllerDragHouseAction = scope.Get<ControllerDragHouseAction>();
			controllerDragHouseAction.fromUpgradeMenu = fromUpgradeMenu;
			controllerDragHouseAction.InitializeAction(owningGroup, timestamp);
			controllerDragHouseAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragHouseAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragHouseAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragHouseAction.OnActionBegin(timestamp);
			controllerDragHouseAction.MakeExclusive();
			controllerDragHouseAction.SetWorldGridVisible(true);
			return controllerDragHouseAction;
		}
	}
}
