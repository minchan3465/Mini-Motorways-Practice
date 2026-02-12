using System;
using Factory;
using Motorways.UI;
using Motorways.Views;

namespace Motorways.Actions
{
	// Token: 0x020006F4 RID: 1780
	public class ControllerDragDestinationAction : DragDestinationAction
	{
		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x060030BD RID: 12477 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030BE RID: 12478 RVA: 0x000E53F4 File Offset: 0x000E35F4
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			ICreativeModeEditableObject editableObject = base.Scope.Get<EditMenuPanel>().EditableObject;
			if (editableObject is CreativeModeEditableDestination || editableObject is CreativeModeEditableHouse)
			{
				this._gameUI.ConfirmEditMenuEdit();
			}
		}

		// Token: 0x060030BF RID: 12479 RVA: 0x000E5434 File Offset: 0x000E3634
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.InputAction == 2)
			{
				if (this.draftDestination == null || this.draftDestination.CompletelyOutOfPlayArea(this._city))
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

		// Token: 0x060030C0 RID: 12480 RVA: 0x000E54B3 File Offset: 0x000E36B3
		public new static ControllerDragDestinationAction CreateSingleFromEditMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return ControllerDragDestinationAction.Create(owningGroup, scope, timestamp, false, false);
		}

		// Token: 0x060030C1 RID: 12481 RVA: 0x000E54BF File Offset: 0x000E36BF
		public new static ControllerDragDestinationAction CreateDoubleFromEditMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return ControllerDragDestinationAction.Create(owningGroup, scope, timestamp, true, false);
		}

		// Token: 0x060030C2 RID: 12482 RVA: 0x000E54CB File Offset: 0x000E36CB
		public new static ControllerDragDestinationAction CreateSingleFromUpgradeMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return ControllerDragDestinationAction.Create(owningGroup, scope, timestamp, false, true);
		}

		// Token: 0x060030C3 RID: 12483 RVA: 0x000E54D7 File Offset: 0x000E36D7
		public new static ControllerDragDestinationAction CreateDoubleFromUpgradeMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return ControllerDragDestinationAction.Create(owningGroup, scope, timestamp, true, true);
		}

		// Token: 0x060030C4 RID: 12484 RVA: 0x000E54E4 File Offset: 0x000E36E4
		private static ControllerDragDestinationAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp, bool isDouble, bool fromUpgradeMenu)
		{
			ControllerDragDestinationAction controllerDragDestinationAction = scope.Get<ControllerDragDestinationAction>();
			controllerDragDestinationAction.isDouble = isDouble;
			controllerDragDestinationAction.fromUpgradeMenu = fromUpgradeMenu;
			controllerDragDestinationAction.InitializeAction(owningGroup, timestamp);
			controllerDragDestinationAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragDestinationAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragDestinationAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragDestinationAction.OnActionBegin(timestamp);
			controllerDragDestinationAction.MakeExclusive();
			controllerDragDestinationAction.SetWorldGridVisible(true);
			return controllerDragDestinationAction;
		}
	}
}
