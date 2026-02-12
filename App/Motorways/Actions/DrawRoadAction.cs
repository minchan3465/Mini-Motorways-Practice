using System;
using Client;
using Factory;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x0200070B RID: 1803
	public class DrawRoadAction : MotorwaysPlayerAction
	{
		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x06003186 RID: 12678 RVA: 0x000020AA File Offset: 0x000002AA
		protected override bool ManuallyHandlesReservations
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x06003187 RID: 12679 RVA: 0x000020AA File Offset: 0x000002AA
		public override bool PreventsCursorAcceleration
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003188 RID: 12680 RVA: 0x000EA64C File Offset: 0x000E884C
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			if (this._inputState.GetButtonDown(25) || (base.OwningGroup.InstigatingInputEvent.Source == InputEventSource.Touch && !this._cameraView.IsFocussedIn) || this._gameUI.CurrentRoadDrawMode != RoadDrawMode.Add)
			{
				this.OnActionCancel();
				return;
			}
			this._currentCoordinates = base.GetPointerTilePosition();
			this._previousMousePosition = base.GetPointerWorldPosition();
			Tile tile = this._tilemapView.GetOrCreateTile(this._currentCoordinates);
			if (tile != null)
			{
				foreach (TileDirection motorwayDirection in tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active))
				{
					if (!this._tilemapView.GetMotorway(tile.GetMotorwayInDirection(motorwayDirection, RoadState.Planned | RoadState.Active)).IsPermanent)
					{
						this.OnActionCancel();
						return;
					}
				}
			}
			this.UpdateCursorPosition();
			this.SetCursorVisible(true);
			if (this._gameUI.IsFocusPointActive)
			{
				this._gameUI.SetFocusPointPosition(this._tilemapView.GetScreenPositionFromTileCoordinates(this._currentCoordinates));
			}
			PlayerAction.Log.Info("Beginning DrawRoadAction from tile coordinates {0}.", new object[]
			{
				this._currentCoordinates
			});
		}

		// Token: 0x06003189 RID: 12681 RVA: 0x000EA778 File Offset: 0x000E8978
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			if (base.ActionState != PlayerAction.State.Begun)
			{
				return;
			}
			if (base.Scope.Get<EditMenuPanel>().IsOpen)
			{
				this._gameUI.ConfirmEditMenuEdit();
				this.OnActionCancel();
				return;
			}
			Vector2Int nextTileCoordinates = base.GetPointerTilePosition();
			Fix64 currentTime = this._clockModel.ExpansionTime;
			bool currentCoordinatesInPlayableArea = this._city.IsTileInPlayableArea(this._currentCoordinates, currentTime);
			if (nextTileCoordinates != this._currentCoordinates)
			{
				if (currentCoordinatesInPlayableArea && this._city.IsTileInPlayableArea(nextTileCoordinates, currentTime))
				{
					Vector2 testWorldPosition = base.GetPointerWorldPosition();
					Vector2 currentTileWorldPosition = TilemapView.GetWorldPositionForCoordinates(this._currentCoordinates);
					Vector2 currentDirection = testWorldPosition - currentTileWorldPosition;
					TileDirection nextEditDirection = TileUtilities.GetClosestDirection(currentDirection.normalized);
					float stepDistance = this._constants.RoadDrawingStepDistance;
					if (TileUtilities.IsDirectionDiagonal(nextEditDirection))
					{
						stepDistance = this._constants.DiagonalRoadDrawingStepDistance;
					}
					if (FeatureToggle.IsFeatureDisabled(Feature.RoadDrawingAnimations))
					{
						stepDistance = DrawRoadAction.OriginalStepMultiplier;
					}
					float minimumStepDistanceForDirection = stepDistance * (TileUtilities.IsDirectionDiagonal(nextEditDirection) ? ((float)TilemapModel.HalfTileWidth * Mathf.Sqrt(2f)) : ((float)TilemapModel.HalfTileWidth));
					while (currentDirection.sqrMagnitude >= minimumStepDistanceForDirection * minimumStepDistanceForDirection)
					{
						Vector2Int targetCoordinates = TileUtilities.GetAdjacentCoordinates(this._currentCoordinates, nextEditDirection);
						if (!this._city.IsTileInPlayableArea(targetCoordinates, currentTime))
						{
							break;
						}
						PlayerAction.Log.Info("Building from {0} in direction {1}.", new object[]
						{
							this._currentCoordinates,
							currentDirection
						});
						if (!this.TryAddRoadInDirection(ref this._currentCoordinates, currentDirection))
						{
							break;
						}
						currentTileWorldPosition = TilemapView.GetWorldPositionForCoordinates(this._currentCoordinates);
						currentDirection = testWorldPosition - currentTileWorldPosition;
						nextEditDirection = TileUtilities.GetClosestDirection(currentDirection.normalized);
						minimumStepDistanceForDirection = stepDistance * (TileUtilities.IsDirectionDiagonal(nextEditDirection) ? ((float)TilemapModel.HalfTileWidth * Mathf.Sqrt(2f)) : ((float)TilemapModel.HalfTileWidth));
					}
				}
				else
				{
					this._currentCoordinates = nextTileCoordinates;
					this._previousMousePosition = base.GetPointerWorldPosition();
				}
			}
			if (currentCoordinatesInPlayableArea && !this._city.Definition.TileIsUnderAMountain(this._currentCoordinates))
			{
				if (FeatureToggle.IsFeatureEnabled(Feature.RoadDrawingAnimations))
				{
					this.UpdateRoadPreview();
				}
				else
				{
					this.HideRoadPreview();
				}
			}
			else
			{
				this.HideRoadPreview();
			}
			this.UpdateCursorPosition();
		}

		// Token: 0x0600318A RID: 12682 RVA: 0x000EA9C6 File Offset: 0x000E8BC6
		private void HideRoadPreview()
		{
			if (this._roadPreview != null)
			{
				this._roadPreview.Remove();
				this._roadPreview = null;
			}
		}

		// Token: 0x0600318B RID: 12683 RVA: 0x000EA9E8 File Offset: 0x000E8BE8
		private void UpdateRoadPreview()
		{
			if (this._roadPreview == null)
			{
				this._roadPreview = this._scope.Get<NewRoadPreview>();
				this._viewClient.AddView(this._roadPreview);
				this.CheckHazardStripes();
			}
			if (!this._currentlyInErrorState)
			{
				this.CheckHazardStripes();
			}
			Vector2Int startPosition = this._currentlyInErrorState ? this._lastErrorPosition : this._currentCoordinates;
			Vector2 endPosition = base.GetPointerWorldPosition();
			this._roadPreview.SetPosition(startPosition, endPosition);
		}

		// Token: 0x0600318C RID: 12684 RVA: 0x000EAA64 File Offset: 0x000E8C64
		private bool IsCurrentTileHouse()
		{
			Tile tile = this._tilemapView.GetTile(this._currentCoordinates);
			return tile != null && tile.ContentType == TileContentType.House;
		}

		// Token: 0x0600318D RID: 12685 RVA: 0x000EAA94 File Offset: 0x000E8C94
		private void CheckHazardStripes()
		{
			bool stripesEnabled = !this.IsCurrentTileHouse() && !this._upgradeDatabase.HasUpgradeAvailable(UpgradeType.Concrete, 1);
			this._roadPreview.SetHazardStripesEnabled(stripesEnabled, true);
		}

		// Token: 0x0600318E RID: 12686 RVA: 0x000EAACC File Offset: 0x000E8CCC
		private bool TryAddRoadInDirection(ref Vector2Int currentPosition, Vector2 directionVector)
		{
			TileDirection direction = TileUtilities.GetClosestDirection(directionVector);
			TileEditResult addRoadResult = this._tileEditor.AddRoad(this._tilemapView, currentPosition, direction);
			if (addRoadResult.edit != null || !addRoadResult.IsSuccessful)
			{
				AudioEventType eventType = AudioEventType.BuildRoad;
				Vector2Int adjacentCoords = TileUtilities.GetAdjacentCoordinates(currentPosition, direction);
				if (this._city.Definition.TileIsOverWater(adjacentCoords))
				{
					eventType = AudioEventType.BuildBridge;
				}
				else if (this._city.Definition.TileIsUnderAMountain(adjacentCoords))
				{
					eventType = AudioEventType.BuildTunnel;
				}
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, eventType, base.GetPan().x, -1f, addRoadResult.IsSuccessful, null));
				if (!addRoadResult.IsSuccessful && !this._currentlyInErrorState)
				{
					this._lastErrorPosition = addRoadResult.errorPosition;
				}
			}
			if (addRoadResult.IsSuccessful)
			{
				base.AddTileEdit(addRoadResult.edit, MotorwaysPlayerAction.EditExecuteTiming.Immediate);
				this._notificationView.HideNotification();
				this._notificationView.HideAlertIcon();
				this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.LightImpact);
				currentPosition = TileUtilities.GetAdjacentCoordinates(currentPosition, direction);
				this._previousMousePosition = base.GetPointerWorldPosition();
				this._currentlyInErrorState = false;
				if (this._roadPreview != null)
				{
					this._roadPreview.SetHazardStripesEnabled(false, true);
				}
				return true;
			}
			if (addRoadResult.resultCode == TileEditResultCode.EditAlreadyExists)
			{
				this._notificationView.HideNotification();
				this._notificationView.HideAlertIcon();
				currentPosition = TileUtilities.GetAdjacentCoordinates(currentPosition, direction);
				this._previousMousePosition = base.GetPointerWorldPosition();
			}
			else
			{
				if (!this._currentlyInErrorState)
				{
					this._notificationView.AddNotification(addRoadResult.resultCode, addRoadResult.errorPosition);
					this._currentlyInErrorState = true;
					if (this._roadPreview != null)
					{
						this._roadPreview.SetHazardStripesEnabled(true, true);
					}
				}
				this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.MediumImpact);
				currentPosition = TileUtilities.GetAdjacentCoordinates(currentPosition, direction);
				this._previousMousePosition = base.GetPointerWorldPosition();
			}
			return false;
		}

		// Token: 0x0600318F RID: 12687 RVA: 0x000EACD0 File Offset: 0x000E8ED0
		protected override void SetCursorVisible(bool visible)
		{
			this._gameUI.SetRoadCursorActive(visible);
			if (!this._cameraView.IsFocussedIn)
			{
				this.SetWorldGridVisible(visible);
				this._tilemapView.viewMode = (visible ? TilemapView.ViewMode.Edit : TilemapView.ViewMode.Normal);
			}
			this.HideRoadPreview();
		}

		// Token: 0x06003190 RID: 12688 RVA: 0x000EAD0A File Offset: 0x000E8F0A
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.Source == InputEventSource.Generic && this._player.IsTapDrawEnabled && inputEvent.ButtonState == InputEventButtonState.JustUp)
			{
				this.OnActionBegin(timestamp);
				return;
			}
			this.OnActionComplete();
		}

		// Token: 0x06003191 RID: 12689 RVA: 0x000EAD3C File Offset: 0x000E8F3C
		public override void OnActionComplete()
		{
			PlayerAction.Log.Info("Completing DrawRoadAction.", Array.Empty<object>());
			if (FeatureToggle.IsFeatureEnabled(Feature.RoadDrawingEndTileCommit))
			{
				Vector2Int endTile = base.GetPointerTilePosition();
				Vector2Int diff = endTile - this._currentCoordinates;
				Fix64 currentTime = this._clockModel.ExpansionTime;
				if (endTile != this._currentCoordinates && diff.magnitude <= Vector2Int.one.magnitude && this._city.IsTileInPlayableArea(this._currentCoordinates, currentTime) && this._city.IsTileInPlayableArea(endTile, currentTime))
				{
					this.TryAddRoadInDirection(ref this._currentCoordinates, diff);
				}
			}
			this.SetCursorVisible(false);
			this._notificationView.HideAlertIcon();
			this._notificationView.CancelNotification();
			base.OnActionComplete();
		}

		// Token: 0x06003192 RID: 12690 RVA: 0x000EAE03 File Offset: 0x000E9003
		public override void OnActionCancel()
		{
			PlayerAction.Log.Info("Cancelling DrawRoadAction.", Array.Empty<object>());
			base.OnActionCancel();
			this.SetCursorVisible(false);
			this._notificationView.CancelNotification();
			this._notificationView.HideAlertIcon();
		}

		// Token: 0x06003193 RID: 12691 RVA: 0x000EAE3C File Offset: 0x000E903C
		public override void Reset()
		{
			base.Reset();
			this._currentCoordinates = default(Vector2Int);
			this._previousMousePosition = default(Vector2);
			this._currentlyInErrorState = false;
			this._lastErrorPosition = default(Vector2Int);
		}

		// Token: 0x06003194 RID: 12692 RVA: 0x000EAE70 File Offset: 0x000E9070
		public static MotorwaysPlayerAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			InputState state = scope.Get<InputState>();
			if (state.GetButtonDown(25) || state.GetButton(25) || scope.Get<GameUIScreen>().CurrentRoadDrawMode != RoadDrawMode.Add)
			{
				return DragClearTileAction.Create(owningGroup, scope, timestamp);
			}
			DrawRoadAction newAction = scope.Get<DrawRoadAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			newAction._playerPositionSource = (MotorwaysPlayerAction.DoesInputTypeUseFocusPoint(owningGroup.InstigatingInputEvent.Source) ? MotorwaysPlayerAction.PlayerPositionSource.FocusPoint : MotorwaysPlayerAction.PlayerPositionSource.InputEvent);
			bool flag = owningGroup.InstigatingInputEvent.Source == InputEventSource.Generic && scope.Get<ActivePlayer>().IsTapDrawEnabled;
			if (flag)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Generic, owningGroup.InstigatingInputEvent.InputAction, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, owningGroup.InstigatingInputEvent.InputAction, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(20, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
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

		// Token: 0x04002A83 RID: 10883
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A84 RID: 10884
		[Dependency]
		private IScope _scope;

		// Token: 0x04002A85 RID: 10885
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002A86 RID: 10886
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002A87 RID: 10887
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x04002A88 RID: 10888
		[Dependency]
		private VisualConstantsData _constants;

		// Token: 0x04002A89 RID: 10889
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04002A8A RID: 10890
		private static readonly float OriginalStepMultiplier = 1.65f;

		// Token: 0x04002A8B RID: 10891
		private Vector2Int _currentCoordinates;

		// Token: 0x04002A8C RID: 10892
		public static readonly Fix64 UTurnNubChangeTolerance = (Fix64)0.2f;

		// Token: 0x04002A8D RID: 10893
		private Vector2 _previousMousePosition;

		// Token: 0x04002A8E RID: 10894
		private NewRoadPreview _roadPreview;

		// Token: 0x04002A8F RID: 10895
		private Vector2Int _lastErrorPosition;

		// Token: 0x04002A90 RID: 10896
		private bool _currentlyInErrorState;
	}
}
