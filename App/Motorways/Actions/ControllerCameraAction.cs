using System;
using Factory;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006F0 RID: 1776
	public class ControllerCameraAction : MotorwaysPlayerAction
	{
		// Token: 0x0600309F RID: 12447 RVA: 0x000E4808 File Offset: 0x000E2A08
		public override void OnActionBegin(float timestamp)
		{
			if (!this._cameraView.IsFocussedIn)
			{
				this.OnActionCancel();
				return;
			}
			base.OnActionBegin(timestamp);
			this._gameUI.SetFocusPointActive(false, false);
			this._initialScreenPosition = base.GetPointerScreenPosition();
			PlayerAction.Log.Info("Beginning MouseCameraAction from {0}.", new object[]
			{
				this._initialScreenPosition
			});
		}

		// Token: 0x060030A0 RID: 12448 RVA: 0x000E486C File Offset: 0x000E2A6C
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this._focusPanDelta = this.GetPanFocusJoystickInputValue();
			if (this._focusPanDelta.sqrMagnitude < 0.001f)
			{
				this.OnActionComplete();
				return;
			}
			Vector2 absDelta = new Vector2(Mathf.Abs(this._focusPanDelta.x), Mathf.Abs(this._focusPanDelta.y));
			Vector2 screenStep = this._focusPanDelta * absDelta * (this._visualConstants.BaseControllerSpeed * this._tilemapView.ScreenDistanceBetweenTiles) * frameTime;
			this._focusPanDelta = screenStep;
			Vector2 newPosition = base.GetPointerScreenPosition();
			this._panOriginWorldPosition = this._tilemapView.GetWorldPositionFromScreenPosition(newPosition);
			if (!Diagnostics.Verify(this._visualConstants.PanningSpeedPerZoomLevel.Count > 0 && this._player.ZoomLevel < 0))
			{
				int indexZoom = Mathf.Clamp(this._player.ZoomLevel, 0, this._visualConstants.PanningSpeedPerZoomLevel.Count - 1);
				this._focusPanDelta *= this._visualConstants.PanningSpeedPerZoomLevel[indexZoom];
			}
			if (this._focusPanDelta != Vector2.zero)
			{
				this._cameraView.ApplyPlayerPanPosition(this._panOriginWorldPosition, newPosition - this._focusPanDelta);
			}
		}

		// Token: 0x060030A1 RID: 12449 RVA: 0x000E49BC File Offset: 0x000E2BBC
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			base.ObserveInput(timestamp, inputEvent, overUI);
			if (inputEvent.ButtonState == InputEventButtonState.Axis && inputEvent is AxisInputEvent && Mathf.Approximately(this.GetPanFocusJoystickInputValue().sqrMagnitude, 0f))
			{
				this.OnActionComplete();
			}
		}

		// Token: 0x060030A2 RID: 12450 RVA: 0x000E4A04 File Offset: 0x000E2C04
		public static ControllerCameraAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ControllerCameraAction newAction = scope.Get<ControllerCameraAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			PlayerAction.Log.Info("[ControllerCameraAction] Creating new instance of action: {0}", new object[]
			{
				timestamp
			});
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 34, InputEventButtonState.Axis), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 33, InputEventButtonState.Axis), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x060030A3 RID: 12451 RVA: 0x000E4A7A File Offset: 0x000E2C7A
		public override void Reset()
		{
			base.Reset();
			this._initialScreenPosition = default(Vector2);
			this._panOriginWorldPosition = default(Vector2);
			this._focusPanDelta = default(Vector2);
		}

		// Token: 0x040029DC RID: 10716
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x040029DD RID: 10717
		[Dependency]
		protected VisualConstantsData _visualConstants;

		// Token: 0x040029DE RID: 10718
		[Dependency]
		private PlayerActionController _playerActionController;

		// Token: 0x040029DF RID: 10719
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x040029E0 RID: 10720
		private Vector2 _initialScreenPosition;

		// Token: 0x040029E1 RID: 10721
		private Vector2 _panOriginWorldPosition;

		// Token: 0x040029E2 RID: 10722
		private Vector2 _focusPanDelta;
	}
}
