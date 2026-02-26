using System;
using System.Collections.Generic;
using Factory;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000710 RID: 1808
	public class MoveInGameFocusAction : MotorwaysPlayerAction
	{
		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x060031BA RID: 12730 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool IsInterruptible
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x060031BB RID: 12731 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060031BC RID: 12732 RVA: 0x000EB7FC File Offset: 0x000E99FC
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this._timeSpentAtMaxSpeed = 0f;
			if (this._controllerState.ControllerState != MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles && (this._controllerState.ControllerState != MotorwaysInGameStateToggleController.InGameControllerState.EditMenu || this._inputState.CurrentDeviceInputType != DeviceInputType.Remote))
			{
				this._gameUI.SetFocusPointActive(false, false);
				this.OnActionCancel();
				return;
			}
			this._touchStartingPosition = base.GetPointerScreenPosition();
			this._focusUIStartingPosition = this._gameUI.FocusPointPosition;
			this._focusMovementDelta = Vector2.zero;
			MoveInGameFocusAction.Log.Info("Starting MoveInGameFocusAction. Touch at {0}, UI at {1}", new object[]
			{
				this._touchStartingPosition,
				this._focusUIStartingPosition
			});
			this._gameUI.SetFocusPointActive(true, false);
		}

		// Token: 0x060031BD RID: 12733 RVA: 0x000EB8C0 File Offset: 0x000E9AC0
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this._focusMovementDelta = this.GetMoveFocusJoystickInputValue();
			if (this._focusMovementDelta.sqrMagnitude < 0.001f)
			{
				this.OnActionComplete();
				return;
			}
			if (this._focusMovementDelta.sqrMagnitude >= 1f)
			{
				this._timeSpentAtMaxSpeed += frameTime;
			}
			Vector2 absDelta = new Vector2(Mathf.Abs(this._focusMovementDelta.x), Mathf.Abs(this._focusMovementDelta.y));
			float speedMultiplier = this._visualConstants.BaseControllerSpeed;
			speedMultiplier *= this._visualConstants.ControllerSpeedSensitivityOptions[this._player.ControllerSensitivity];
			bool moveInWorldSpace = false;
			foreach (PlayerActionGroup playerActionGroup in this._playerActionController.ActiveGroups)
			{
				using (IEnumerator<PlayerAction> enumerator2 = playerActionGroup.Actions.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						MotorwaysPlayerAction action = enumerator2.Current as MotorwaysPlayerAction;
						if (action != null && action.PreventsCursorAcceleration)
						{
							moveInWorldSpace = true;
						}
					}
				}
			}
			if (!moveInWorldSpace)
			{
				speedMultiplier *= this._visualConstants.BaseControllerSpeedOverZoom.Evaluate(this._cameraView.DesiredZoom) * this._visualConstants.ControllerAccelerationCurve.Evaluate(this._timeSpentAtMaxSpeed);
			}
			else
			{
				this._timeSpentAtMaxSpeed = 0f;
				TileDirection nearestDirection = TileUtilities.GetClosestDirection(this._focusMovementDelta.normalized);
				float magnitude = this._focusMovementDelta.magnitude;
				if (nearestDirection != TileDirection.None)
				{
					this._focusMovementDelta = TileUtilities.GetVectorForDirection(nearestDirection) * magnitude;
				}
			}
			Vector2 screenStep = this._focusMovementDelta * absDelta * (speedMultiplier * this._tilemapView.ScreenDistanceBetweenTiles) * frameTime;
			this._focusMovementDelta = screenStep;
			if (this._focusMovementDelta != Vector2.zero)
			{
				this._gameUI.SetFocusPointPosition(this._gameUI.FocusPointPosition + this._focusMovementDelta);
			}
		}

		// Token: 0x060031BE RID: 12734 RVA: 0x000EBAD8 File Offset: 0x000E9CD8
		public override void OnActionCancel()
		{
			this._gameUI.SetFocusPointActive(false, false);
			base.OnActionCancel();
		}

		// Token: 0x060031BF RID: 12735 RVA: 0x000EBAF0 File Offset: 0x000E9CF0
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			base.ObserveInput(timestamp, inputEvent, overUI);
			if (inputEvent.ButtonState == InputEventButtonState.Axis && inputEvent is AxisInputEvent && Mathf.Approximately(this.GetMoveFocusJoystickInputValue().sqrMagnitude, 0f))
			{
				this.OnActionComplete();
			}
		}

		// Token: 0x060031C0 RID: 12736 RVA: 0x000EBB37 File Offset: 0x000E9D37
		public override void Reset()
		{
			base.Reset();
			this._touchStartingPosition = default(Vector2);
			this._focusUIStartingPosition = default(Vector2);
			this._focusMovementDelta = default(Vector2);
			this._timeSpentAtMaxSpeed = 0f;
		}

		// Token: 0x060031C1 RID: 12737 RVA: 0x000EBB70 File Offset: 0x000E9D70
		public static MoveInGameFocusAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			MoveInGameFocusAction.Log.Info("Creating MoveInGameFocus action!", Array.Empty<object>());
			MoveInGameFocusAction moveInGameFocusAction = scope.Get<MoveInGameFocusAction>();
			moveInGameFocusAction.InitializeAction(owningGroup, timestamp);
			moveInGameFocusAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 0, InputEventButtonState.Axis), PlayerAction.ObserverGreediness.BlocksNewActions);
			moveInGameFocusAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 1, InputEventButtonState.Axis), PlayerAction.ObserverGreediness.BlocksNewActions);
			moveInGameFocusAction.OnActionBegin(timestamp);
			return moveInGameFocusAction;
		}

		// Token: 0x04002AA8 RID: 10920
		[Dependency]
		private PlayerActionController _playerActionController;

		// Token: 0x04002AA9 RID: 10921
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04002AAA RID: 10922
		public new static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("MoveInGameFocusAction");

		// Token: 0x04002AAB RID: 10923
		protected Vector2 _touchStartingPosition;

		// Token: 0x04002AAC RID: 10924
		protected Vector2 _focusUIStartingPosition;

		// Token: 0x04002AAD RID: 10925
		protected Vector2 _focusMovementDelta;

		// Token: 0x04002AAE RID: 10926
		protected float _timeSpentAtMaxSpeed;

		// Token: 0x04002AAF RID: 10927
		[Dependency]
		protected MotorwaysInGameStateToggleController _controllerState;

		// Token: 0x04002AB0 RID: 10928
		[Dependency]
		protected CameraView _cameraView;

		// Token: 0x04002AB1 RID: 10929
		[Dependency]
		protected VisualConstantsData _visualConstants;
	}
}
