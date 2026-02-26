using System;
using Factory;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000708 RID: 1800
	public class DragMoveInGameFocusAction : MoveInGameFocusAction
	{
		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x06003165 RID: 12645 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x06003166 RID: 12646 RVA: 0x000E976D File Offset: 0x000E796D
		private float ControllerMoveSpeedCoefficient
		{
			get
			{
				return 2f;
			}
		}

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x06003167 RID: 12647 RVA: 0x000E9774 File Offset: 0x000E7974
		private float ControllerDragSpeedRamp
		{
			get
			{
				return 2.5f;
			}
		}

		// Token: 0x06003168 RID: 12648 RVA: 0x000E977C File Offset: 0x000E797C
		public override void Tick(float frameTime)
		{
			Vector2 newJoystickValue = this.GetMoveFocusJoystickInputValue();
			if (!this._hasInitialized)
			{
				this._prevJoystickValue = this.GetMoveFocusJoystickInputValue();
				this._hasInitialized = true;
			}
			if (newJoystickValue == Vector2.zero)
			{
				this.OnActionComplete();
				return;
			}
			this._focusMovementDelta = newJoystickValue - this._prevJoystickValue;
			float sqrStepLength = this._focusMovementDelta.sqrMagnitude;
			this._focusMovementDelta += this._focusMovementDelta.normalized * (sqrStepLength * this.ControllerDragSpeedRamp);
			Vector2 screenStep = this._focusMovementDelta * (this.ControllerMoveSpeedCoefficient * this._tilemapView.ScreenDistanceBetweenTiles);
			this._focusMovementDelta = screenStep;
			if (this._focusMovementDelta != Vector2.zero)
			{
				this._gameUI.SetFocusPointPosition(this._gameUI.FocusPointPosition + this._focusMovementDelta);
			}
			this._prevJoystickValue = newJoystickValue;
		}

		// Token: 0x06003169 RID: 12649 RVA: 0x000E9865 File Offset: 0x000E7A65
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.ButtonState == InputEventButtonState.Axis && inputEvent is AxisInputEvent)
			{
				if (this._hasInitialized && Mathf.Approximately(this._prevJoystickValue.sqrMagnitude, 0f))
				{
					this.OnActionComplete();
					return;
				}
			}
			else
			{
				this.OnActionComplete();
			}
		}

		// Token: 0x0600316A RID: 12650 RVA: 0x000E98A4 File Offset: 0x000E7AA4
		public override void Reset()
		{
			base.Reset();
			this._prevJoystickValue = default(Vector2);
			this._hasInitialized = false;
		}

		// Token: 0x0600316B RID: 12651 RVA: 0x000E98C0 File Offset: 0x000E7AC0
		public new static DragMoveInGameFocusAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragMoveInGameFocusAction dragMoveInGameFocusAction = scope.Get<DragMoveInGameFocusAction>();
			dragMoveInGameFocusAction.InitializeAction(owningGroup, timestamp);
			dragMoveInGameFocusAction.OnActionBegin(timestamp);
			dragMoveInGameFocusAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 0, InputEventButtonState.Axis), PlayerAction.ObserverGreediness.BlocksNewActions);
			dragMoveInGameFocusAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 1, InputEventButtonState.Axis), PlayerAction.ObserverGreediness.BlocksNewActions);
			return dragMoveInGameFocusAction;
		}

		// Token: 0x04002A72 RID: 10866
		private Vector2 _prevJoystickValue = Vector2.zero;

		// Token: 0x04002A73 RID: 10867
		private bool _hasInitialized;
	}
}
