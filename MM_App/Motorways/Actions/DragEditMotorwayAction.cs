using System;
using Factory;
using Motorways.Audio;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000704 RID: 1796
	public class DragEditMotorwayAction : AddMotorwayAction
	{
		// Token: 0x06003136 RID: 12598 RVA: 0x000E7CEA File Offset: 0x000E5EEA
		public override void Reset()
		{
			this._editedMotorwayId = -1;
			this._hasReplacedMothballEdit = false;
			this._didShowGrid = false;
			this._hasScheduledOverEvent = false;
			this._previousTilePosition = default(Vector2Int);
			base.Reset();
		}

		// Token: 0x06003137 RID: 12599 RVA: 0x000E7D1C File Offset: 0x000E5F1C
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this._didShowGrid = false;
			if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove && ((base.OwningGroup.InstigatingInputEvent.Source == InputEventSource.Touch && this._cameraView.IsFocussedIn) || base.OwningGroup.InstigatingInputEvent.Source != InputEventSource.Touch))
			{
				this.OnActionCancel();
				return;
			}
			if (this._inputState.TouchCount > 1)
			{
				this.OnActionCancel();
				return;
			}
			this.SetColourWidgetRadialVisible(false);
			this._editedMotorwayId = -1;
			this._hasReplacedMothballEdit = false;
			Vector2Int tilePosition = base.GetPointerTilePosition();
			Tile tile = this._tilemapView.GetTile(tilePosition);
			if (tile != null)
			{
				if (tile.UnbuiltMotorwayId != -1)
				{
					this._newMotorwayId = tile.UnbuiltMotorwayId;
					this._newMotorwayNumber = tile.UnbuiltMotorwayNumber;
					PlayerAction.Log.Info("Extending the unbuilt motorway {0}", new object[]
					{
						this._newMotorwayId
					});
					AddMotorwayAction.MotorwayActionResult anchorError = base.SetAnchorTile(tilePosition, TileDirection.None);
					if (anchorError != AddMotorwayAction.MotorwayActionResult.Success)
					{
						base.DisplayError(anchorError, true);
						this.OnActionCancel();
						return;
					}
				}
				else
				{
					TileDirectionBitfield motorwayRamps = tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active);
					if (motorwayRamps.Count > 0)
					{
						int motorwayId = -1;
						foreach (TileDirection motorwayDirection in motorwayRamps)
						{
							int potentialMotorwayId = tile.GetMotorwayInDirection(motorwayDirection, RoadState.Planned | RoadState.Active);
							if (potentialMotorwayId != -1 && (!this._tilemapView.GetMotorway(potentialMotorwayId).IsPermanent || !this._city.Rules.RoadsBecomePermanentOverTime))
							{
								motorwayId = potentialMotorwayId;
								break;
							}
						}
						if (motorwayId != -1)
						{
							Motorway motorway = this._tilemapView.GetMotorway(motorwayId);
							if (Diagnostics.Verify(motorway != null, "Tile {0} has a reference to missing motorway {1}.", tile.Coordinates, motorwayId))
							{
								bool didSetAnchor = false;
								AddMotorwayAction.MotorwayActionResult anchorError2 = AddMotorwayAction.MotorwayActionResult.Success;
								if (motorway.StartCoordinates == tile.Coordinates)
								{
									anchorError2 = base.SetAnchorTile(motorway.EndCoordinates, motorway.EndDirection);
									didSetAnchor = (anchorError2 == AddMotorwayAction.MotorwayActionResult.Success);
								}
								else if (motorway.EndCoordinates == tile.Coordinates)
								{
									anchorError2 = base.SetAnchorTile(motorway.StartCoordinates, motorway.StartDirection);
									didSetAnchor = (anchorError2 == AddMotorwayAction.MotorwayActionResult.Success);
								}
								else
								{
									Diagnostics.FailAssert("Expected motorway {0} to connect to tile at {1}, but ends are at {2} and {3}.", new object[]
									{
										motorway.Id,
										tile.Coordinates,
										motorway.StartCoordinates,
										motorway.EndCoordinates
									});
								}
								if (didSetAnchor)
								{
									this._editedMotorwayId = motorway.Id;
									this._newMotorwayNumber = motorway.Number;
									TileEdit mothballEdit = MothballMotorwayEdit.Create(base.Scope, this._editedMotorwayId);
									base.AddTileEdit(mothballEdit, MotorwaysPlayerAction.EditExecuteTiming.Draft);
								}
								else
								{
									base.DisplayError(anchorError2, true);
								}
							}
						}
					}
				}
			}
			if ((tile != null && tile.UnbuiltMotorwayId != -1) || this._editedMotorwayId != -1)
			{
				base.MakeExclusive();
				this.SetGridVisible(true);
				return;
			}
			this.OnActionCancel();
		}

		// Token: 0x06003138 RID: 12600 RVA: 0x000E7FFC File Offset: 0x000E61FC
		public override void OnActionComplete()
		{
			if (!this._hasReplacedMothballEdit)
			{
				base.ClearDraftClientEdits();
			}
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
			if (!this._cameraView.IsFocussedIn)
			{
				this.SetGridVisible(false);
			}
			this.SetMotorwayGridVisible(false);
			base.OnActionComplete();
		}

		// Token: 0x06003139 RID: 12601 RVA: 0x000E805F File Offset: 0x000E625F
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			base.ClearDraftClientEdits();
			this.SetGridVisible(false);
			this.SetMotorwayGridVisible(false);
		}

		// Token: 0x0600313A RID: 12602 RVA: 0x000E807C File Offset: 0x000E627C
		private void SetGridVisible(bool visible)
		{
			if (visible)
			{
				this._didShowGrid = true;
			}
			else if (!this._didShowGrid)
			{
				return;
			}
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(visible, TransitionStyle.Tween);
				this._tilemapView.viewMode = (visible ? TilemapView.ViewMode.Edit : TilemapView.ViewMode.Normal);
			}
			this._gameUI.SetMotorwayGridActive(visible, TransitionStyle.Tween);
		}

		// Token: 0x0600313B RID: 12603 RVA: 0x000E80D8 File Offset: 0x000E62D8
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			if (this._inputState.TouchCount > 1)
			{
				TouchCameraAction.Create(base.OwningGroup, this._scope, this._inputState.LastInputTimestamp);
				this.OnActionCancel();
				return;
			}
			Vector2Int pointerTileCoordinates = base.GetPointerTilePosition();
			if (pointerTileCoordinates != this._danglingCoordinates && pointerTileCoordinates != this._previousTilePosition)
			{
				AddMotorwayAction.MotorwayActionResult danglingError = base.SetDanglingTile(pointerTileCoordinates);
				if (base.HasMotorwayOnTile(pointerTileCoordinates, this._editedMotorwayId))
				{
					danglingError = AddMotorwayAction.MotorwayActionResult.TileDoesNotSupportMotorway;
				}
				if (danglingError == AddMotorwayAction.MotorwayActionResult.Success)
				{
					base.UpdateTileEdit();
					this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragSnap, UpgradeType.Motorway, true, null, this._tilemapView.GetScreenPositionFromTileCoordinates(pointerTileCoordinates)));
				}
				else
				{
					base.DisplayError(danglingError, false);
				}
			}
			this._previousTilePosition = pointerTileCoordinates;
		}

		// Token: 0x0600313C RID: 12604 RVA: 0x000E81A0 File Offset: 0x000E63A0
		protected override TileEditResult CreateTileEdit(int newMotorwayId, int motorwayNumber, Vector2Int anchorCoordinates, TileDirection anchorDirection, Vector2Int danglingCoordinates, TileDirection danglingDirection)
		{
			TileEditResult result = this._tileEditor.AddMotorway(this._tilemapView, newMotorwayId, motorwayNumber, anchorCoordinates, anchorDirection, danglingCoordinates, danglingDirection, this._editedMotorwayId);
			if (result.IsSuccessful)
			{
				if (result.edit != null)
				{
					this._notificationView.HideNotification();
					this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOver, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
					this._hasScheduledOverEvent = true;
					this._hasReplacedMothballEdit = true;
					return result;
				}
			}
			else
			{
				if (result.edit != null)
				{
					this._scope.Release(result.edit);
				}
				this._notificationView.AddNotification(result.resultCode, result.errorPosition);
			}
			if (this._hasScheduledOverEvent)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOut, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
				this._hasScheduledOverEvent = false;
			}
			return result;
		}

		// Token: 0x0600313D RID: 12605 RVA: 0x000E828C File Offset: 0x000E648C
		public static DragEditMotorwayAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragEditMotorwayAction newAction = scope.Get<DragEditMotorwayAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Mouse)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(19, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(20, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			else if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
				newAction.BlockNewTouchUpgradeActions();
			}
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x04002A4F RID: 10831
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002A50 RID: 10832
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x04002A51 RID: 10833
		[Dependency]
		private IScope _scope;

		// Token: 0x04002A52 RID: 10834
		private int _editedMotorwayId = -1;

		// Token: 0x04002A53 RID: 10835
		private bool _hasReplacedMothballEdit;

		// Token: 0x04002A54 RID: 10836
		private bool _didShowGrid;

		// Token: 0x04002A55 RID: 10837
		private bool _hasScheduledOverEvent;

		// Token: 0x04002A56 RID: 10838
		private Vector2Int _previousTilePosition;
	}
}
