using System;
using Factory;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000700 RID: 1792
	public class DragClearTileAction : MotorwaysPlayerAction
	{
		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x0600310E RID: 12558 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool PreventsCursorAcceleration
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x0600310F RID: 12559 RVA: 0x000E6AC5 File Offset: 0x000E4CC5
		private bool TwoFingerGracePeriodActive
		{
			get
			{
				return this._twoFingerPanGracePeriodTimeRemaining > 0f;
			}
		}

		// Token: 0x06003110 RID: 12560 RVA: 0x000E6AD4 File Offset: 0x000E4CD4
		public override void OnActionBegin(float timestamp)
		{
			this._didShowCursor = false;
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			this._twoFingerPanGracePeriodTimeRemaining = 0.5f;
			this._lastCoordinates = base.GetPointerTilePosition();
			this.StartAction();
		}

		// Token: 0x06003111 RID: 12561 RVA: 0x000E6B08 File Offset: 0x000E4D08
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			if (base.ActionState != PlayerAction.State.Begun)
			{
				return;
			}
			if (this.TwoFingerGracePeriodActive)
			{
				if (this._inputState.TouchCount > 1)
				{
					this.OnActionCancel();
					return;
				}
				this._twoFingerPanGracePeriodTimeRemaining -= frameTime;
				Diagnostics.Log.Info("DragClearTileAction", "Grace Period Active: {0}s left", new object[]
				{
					this._twoFingerPanGracePeriodTimeRemaining
				});
			}
			Vector2Int nextTileCoordinates = base.GetPointerTilePosition();
			Tile tile = this._tilemapView.GetTile(nextTileCoordinates);
			if (tile != null && (tile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) > 0 || tile.IsCenterOfRoundabout) && tile.ContentType != TileContentType.Carpark && tile.ContentType != TileContentType.House)
			{
				base.MakeExclusive();
			}
			bool deletedOriginalTileAsCoordinatesChanged = false;
			this.UpdateCursorPosition();
			if (nextTileCoordinates != this._currentCoordinates)
			{
				Diagnostics.Log.Info("DragClearTileAction", "Coordinates Changed from {0} to {1}", new object[]
				{
					this._currentCoordinates,
					nextTileCoordinates
				});
				this.ClearTile(nextTileCoordinates);
				this._currentCoordinates = nextTileCoordinates;
				this._twoFingerPanGracePeriodTimeRemaining = 0f;
				deletedOriginalTileAsCoordinatesChanged = true;
			}
			if (!this.TwoFingerGracePeriodActive && !this._hasDeletedOriginalCoordinate)
			{
				Diagnostics.Log.Info("DragClearTileAction", deletedOriginalTileAsCoordinatesChanged ? "Clearing original tile as coordinates changed" : "Clearing original tile as grace period ended", Array.Empty<object>());
				this.ClearTile(this._lastCoordinates);
				this._hasDeletedOriginalCoordinate = true;
			}
		}

		// Token: 0x06003112 RID: 12562 RVA: 0x000E6C54 File Offset: 0x000E4E54
		private void ClearTile(Vector2Int tileToClear)
		{
			bool respectPermanence = this._city.Rules.RoadsBecomePermanentOverTime;
			TileEditResult clearTileResult = this._tileEditor.ClearTile(this._tilemapView, tileToClear, respectPermanence ? Tile.TileChangePermissions.RespectPermanence : Tile.TileChangePermissions.Full);
			if (clearTileResult.IsSuccessful && clearTileResult.edit != null)
			{
				Tile clearedTile = this._tilemapView.GetTile(tileToClear);
				if (clearedTile.HasTrafficLight || clearedTile.GetTwoLaneRoads(RoadState.VisiblyActive, Tile.MotorwayInclusion.Include).Count > 0 || clearedTile.HasRoundabout(RoadState.Planned | RoadState.Active))
				{
					this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.LightImpact);
				}
				base.AddTileEdit(clearTileResult.edit, MotorwaysPlayerAction.EditExecuteTiming.Immediate);
				this._lastSuccessfulEditDirection = TileUtilities.GetClosestDirection(tileToClear - this._currentCoordinates);
				this._currentCoordinates = tileToClear;
				base.MakeExclusive();
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.MothballRoad, 0.5f, -1f, true, null));
				this._notificationView.HideAlertIcon();
				this._notificationView.CancelNotification();
				this._isShowingError = false;
				return;
			}
			if ((clearTileResult.resultCode == TileEditResultCode.NoDeletableRoads || clearTileResult.resultCode == TileEditResultCode.NoDeletableUpgrade) && !this._isShowingError)
			{
				this._notificationView.AddNotification(clearTileResult.resultCode, clearTileResult.errorPosition);
				this._isShowingError = true;
			}
		}

		// Token: 0x06003113 RID: 12563 RVA: 0x000E6D9C File Offset: 0x000E4F9C
		private void StartAction()
		{
			this.shouldSwitchBackToAddMode = (this._gameUI.CurrentRoadDrawMode != RoadDrawMode.Remove);
			if (base.OwningGroup.InstigatingInputEvent.Source == InputEventSource.Touch && !this._cameraView.IsFocussedIn)
			{
				this.OnActionCancel();
				return;
			}
			if (base.Scope.Get<EditMenuPanel>().IsOpen)
			{
				this._gameUI.ConfirmEditMenuEdit();
				this.OnActionCancel();
				return;
			}
			this._currentCoordinates = base.GetPointerTilePosition();
			this._hasDeletedOriginalCoordinate = false;
			this._lastSuccessfulEditDirection = TileDirection.None;
			this._gameUI.CurrentRoadDrawMode = RoadDrawMode.Remove;
			this.SetCursorVisible(true);
		}

		// Token: 0x06003114 RID: 12564 RVA: 0x000E6E40 File Offset: 0x000E5040
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.Source == InputEventSource.Generic && this._player.IsTapDrawEnabled && inputEvent.ButtonState == InputEventButtonState.JustUp)
			{
				this.OnActionBegin(timestamp);
				return;
			}
			Vector2Int nextTileCoordinates = base.GetPointerTilePosition();
			this.ClearTile(nextTileCoordinates);
			this.OnActionComplete();
		}

		// Token: 0x06003115 RID: 12565 RVA: 0x000E6E88 File Offset: 0x000E5088
		public override void OnActionComplete()
		{
			base.OnActionComplete();
			this._notificationView.HideAlertIcon();
			this._notificationView.CancelNotification();
			this.SetCursorVisible(false);
			if (this.shouldSwitchBackToAddMode)
			{
				this._gameUI.CurrentRoadDrawMode = RoadDrawMode.Add;
			}
		}

		// Token: 0x06003116 RID: 12566 RVA: 0x000E6EC1 File Offset: 0x000E50C1
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			this._notificationView.HideNotification();
			this._notificationView.HideAlertIcon();
			this.SetCursorVisible(false);
			if (this.shouldSwitchBackToAddMode)
			{
				this._gameUI.CurrentRoadDrawMode = RoadDrawMode.Add;
			}
		}

		// Token: 0x06003117 RID: 12567 RVA: 0x000E6EFC File Offset: 0x000E50FC
		public override void Reset()
		{
			this._currentCoordinates = default(Vector2Int);
			this._hasDeletedOriginalCoordinate = false;
			this._didShowCursor = false;
			this.shouldSwitchBackToAddMode = true;
			this._lastSuccessfulEditDirection = TileDirection.North;
			this._isShowingError = false;
			this._twoFingerPanGracePeriodTimeRemaining = 0.5f;
			this._lastCoordinates = default(Vector2Int);
			base.Reset();
		}

		// Token: 0x06003118 RID: 12568 RVA: 0x000E6F58 File Offset: 0x000E5158
		protected override void SetCursorVisible(bool visible)
		{
			if (visible)
			{
				this._didShowCursor = true;
			}
			else if (!this._didShowCursor)
			{
				return;
			}
			this._gameUI.SetRoadCursorActive(visible);
			if (!this._cameraView.IsFocussedIn)
			{
				this.SetWorldGridVisible(visible);
				this._tilemapView.viewMode = (visible ? TilemapView.ViewMode.Edit : TilemapView.ViewMode.Normal);
			}
		}

		// Token: 0x06003119 RID: 12569 RVA: 0x000E6FAC File Offset: 0x000E51AC
		public static DragClearTileAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragClearTileAction newAction = scope.Get<DragClearTileAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			newAction._playerPositionSource = (MotorwaysPlayerAction.DoesInputTypeUseFocusPoint(owningGroup.InstigatingInputEvent.Source) ? MotorwaysPlayerAction.PlayerPositionSource.FocusPoint : MotorwaysPlayerAction.PlayerPositionSource.InputEvent);
			bool flag = owningGroup.InstigatingInputEvent.Source == InputEventSource.Generic && scope.Get<ActivePlayer>().IsTapDrawEnabled;
			if (flag)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Generic, owningGroup.InstigatingInputEvent.InputAction, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, owningGroup.InstigatingInputEvent.InputAction, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 17, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			if (!flag)
			{
				newAction.OnActionBegin(timestamp);
			}
			return newAction;
		}

		// Token: 0x04002A1B RID: 10779
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A1C RID: 10780
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002A1D RID: 10781
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04002A1E RID: 10782
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x04002A1F RID: 10783
		private Fix64 _minMouseDistance = TilemapModel.TileWidth * (Fix64)1.15f;

		// Token: 0x04002A20 RID: 10784
		private Vector2Int _currentCoordinates;

		// Token: 0x04002A21 RID: 10785
		private Vector2Int _lastCoordinates;

		// Token: 0x04002A22 RID: 10786
		private bool _hasDeletedOriginalCoordinate;

		// Token: 0x04002A23 RID: 10787
		private TileDirection _lastSuccessfulEditDirection;

		// Token: 0x04002A24 RID: 10788
		private bool _isShowingError;

		// Token: 0x04002A25 RID: 10789
		private bool _didShowCursor;

		// Token: 0x04002A26 RID: 10790
		private bool shouldSwitchBackToAddMode = true;

		// Token: 0x04002A27 RID: 10791
		private const float TwoFingerGracePeriod = 0.5f;

		// Token: 0x04002A28 RID: 10792
		private float _twoFingerPanGracePeriodTimeRemaining;
	}
}
