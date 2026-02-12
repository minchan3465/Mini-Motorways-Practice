using System;
using Factory;
using Motorways.Audio;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000717 RID: 1815
	public class ToggleZoomAction : MotorwaysPlayerAction
	{
		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x060031E4 RID: 12772 RVA: 0x000EC38B File Offset: 0x000EA58B
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return this._source;
			}
		}

		// Token: 0x060031E5 RID: 12773 RVA: 0x000EC394 File Offset: 0x000EA594
		private bool PointerPositionWithinWindow(Vector2 pointerPosition)
		{
			return this._cameraView.GameCamera.UICamera.pixelRect.Contains(pointerPosition);
		}

		// Token: 0x060031E6 RID: 12774 RVA: 0x000EC3C0 File Offset: 0x000EA5C0
		public override void OnActionBegin(float timestamp)
		{
			Vector2 pointerPosition = base.GetPointerScreenPosition();
			this.SetColourWidgetRadialVisible(false);
			if (this._cameraView.HasControlOverriden || !this._cameraView.CanChangeFocus || !this.PointerPositionWithinWindow(pointerPosition))
			{
				this.OnActionComplete();
				return;
			}
			if ((!this._cameraView.IsFocussedIn && !this._hasDirection) || (!this._cameraView.IsFocussedIn && this._hasDirection && this._zoomIn))
			{
				this.SetWorldGridVisible(this._visualConstants.ShowGridOnZoom);
				this._cameraView.FocusOnWorldPosition(this._tilemapView.GetWorldPositionFromScreenPosition(pointerPosition), CameraView.CameraFocusOffsetType.MaintainScreenPosition);
				this._tilemapView.viewMode = TilemapView.ViewMode.Edit;
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.FocusZoomIn, UIAudioProfile.None, this._cameraView.GetInterpolationSpeed(), true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
				return;
			}
			if ((this._cameraView.IsFocussedIn && !this._hasDirection) || (this._cameraView.IsFocussedIn && this._hasDirection && !this._zoomIn))
			{
				this.SetWorldGridVisible(false);
				this.SetMotorwayGridVisible(false);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
				this._cameraView.ResetPlayerViewport();
				if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
				{
					this._gameUI.ToggleDrawMode();
				}
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.FocusZoomOut, UIAudioProfile.None, this._cameraView.GetInterpolationSpeed(), true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
			}
		}

		// Token: 0x060031E7 RID: 12775 RVA: 0x000020A2 File Offset: 0x000002A2
		public override void Tick(float frameTime)
		{
			this.OnActionComplete();
		}

		// Token: 0x060031E8 RID: 12776 RVA: 0x000EC52C File Offset: 0x000EA72C
		public static ToggleZoomAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ToggleZoomAction toggleZoomAction = scope.Get<ToggleZoomAction>();
			toggleZoomAction.InitializeAction(owningGroup, timestamp);
			toggleZoomAction._source = ((owningGroup.InstigatingInputEvent.Source == InputEventSource.Any) ? MotorwaysPlayerAction.PlayerPositionSource.FocusPoint : MotorwaysPlayerAction.PlayerPositionSource.InputEvent);
			toggleZoomAction.OnActionBegin(timestamp);
			return toggleZoomAction;
		}

		// Token: 0x060031E9 RID: 12777 RVA: 0x000EC55B File Offset: 0x000EA75B
		public static ToggleZoomAction CreateZoomIn(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ToggleZoomAction toggleZoomAction = scope.Get<ToggleZoomAction>();
			toggleZoomAction._hasDirection = true;
			toggleZoomAction._zoomIn = true;
			toggleZoomAction.InitializeAction(owningGroup, timestamp);
			toggleZoomAction._source = ((owningGroup.InstigatingInputEvent.Source == InputEventSource.Any) ? MotorwaysPlayerAction.PlayerPositionSource.FocusPoint : MotorwaysPlayerAction.PlayerPositionSource.InputEvent);
			toggleZoomAction.OnActionBegin(timestamp);
			return toggleZoomAction;
		}

		// Token: 0x060031EA RID: 12778 RVA: 0x000EC598 File Offset: 0x000EA798
		public static ToggleZoomAction CreateZoomOut(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ToggleZoomAction toggleZoomAction = scope.Get<ToggleZoomAction>();
			toggleZoomAction._hasDirection = true;
			toggleZoomAction._zoomIn = false;
			toggleZoomAction.InitializeAction(owningGroup, timestamp);
			toggleZoomAction._source = ((owningGroup.InstigatingInputEvent.Source == InputEventSource.Any) ? MotorwaysPlayerAction.PlayerPositionSource.FocusPoint : MotorwaysPlayerAction.PlayerPositionSource.InputEvent);
			toggleZoomAction.OnActionBegin(timestamp);
			return toggleZoomAction;
		}

		// Token: 0x060031EB RID: 12779 RVA: 0x000EC5D5 File Offset: 0x000EA7D5
		public override void OnActionCancel()
		{
			this._hasDirection = false;
			this._zoomIn = false;
			this._source = MotorwaysPlayerAction.PlayerPositionSource.InputEvent;
		}

		// Token: 0x060031EC RID: 12780 RVA: 0x000EC5EC File Offset: 0x000EA7EC
		public override void OnActionComplete()
		{
			this._hasDirection = false;
			this._zoomIn = false;
			this._source = MotorwaysPlayerAction.PlayerPositionSource.InputEvent;
			base.OnActionComplete();
		}

		// Token: 0x04002ABC RID: 10940
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002ABD RID: 10941
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002ABE RID: 10942
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04002ABF RID: 10943
		private bool _hasDirection;

		// Token: 0x04002AC0 RID: 10944
		private bool _zoomIn;

		// Token: 0x04002AC1 RID: 10945
		private MotorwaysPlayerAction.PlayerPositionSource _source;
	}
}
