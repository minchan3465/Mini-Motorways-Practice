using System;
using Factory;
using Motorways.Audio;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000709 RID: 1801
	public class DragRoundaboutAction : MotorwaysPlayerAction
	{
		// Token: 0x0600316D RID: 12653 RVA: 0x000E9927 File Offset: 0x000E7B27
		public override void Reset()
		{
			base.Reset();
			this._lastCheckedCoordinates = default(Vector2Int);
			this._lastPlacedCoordinates = default(Vector2Int);
			this._result = default(TileEditResult);
			this._isRebuildingMothballedRoundabout = false;
		}

		// Token: 0x0600316E RID: 12654 RVA: 0x000E995C File Offset: 0x000E7B5C
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			if (this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.Roundabout) < 1)
			{
				this.OnActionCancel();
				return;
			}
			if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
			{
				this._gameUI.ToggleDrawMode();
			}
			this._result = TileEditResult.InvalidTileCoordinate(Vector2Int.zero);
			this.InitializeUpgradeCursor();
			this._gameUI.UpgradeBar.RemoveFromUpgradeButtonStack(UpgradeType.Roundabout, true);
			this._lastCheckedCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
			PlayerAction.Log.Info("Beginning DragRoundaboutAction from tile coordinates {0}.", new object[]
			{
				this._lastCheckedCoordinates
			});
			this._gameUI.UpgradeBar.CreateAlertOnUpgradeButton(UpgradeType.Roundabout);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragged, UpgradeType.Roundabout, true, null, default(Vector2)));
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(true, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Edit;
			}
		}

		// Token: 0x0600316F RID: 12655 RVA: 0x000E9A5C File Offset: 0x000E7C5C
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this.UpdateUpgradeCursorPosition();
			Vector2Int nextTileCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
			bool hadPlacedRoundabout = base.HasSchedulableClientEdits;
			if ((nextTileCoordinates != this._lastCheckedCoordinates && (!hadPlacedRoundabout || nextTileCoordinates != this._lastPlacedCoordinates)) || this._isRebuildingMothballedRoundabout)
			{
				if (hadPlacedRoundabout)
				{
					base.ClearDraftClientEdits();
					this._isRebuildingMothballedRoundabout = false;
				}
				Vector2Int minCoordinates = nextTileCoordinates + new Vector2Int(-1, -1);
				Vector2Int maxCoordinates = nextTileCoordinates + new Vector2Int(1, 1);
				if (this._city.IsTileInPlayableArea(minCoordinates, this._clockModel.ExpansionTime) && this._city.IsTileInPlayableArea(maxCoordinates, this._clockModel.ExpansionTime))
				{
					this._result = this.SetDraftRoundaboutAt(nextTileCoordinates);
					if (this._result.IsSuccessful)
					{
						this._isRebuildingMothballedRoundabout = this.IsTileMothballedRoundaboutCenter(nextTileCoordinates);
						base.AddTileEdit(this._result.edit, MotorwaysPlayerAction.EditExecuteTiming.Draft);
						this._gameUI.SetUpgradeCursorVisible(false);
						this._gameUI.SetUpgradeCursorPosition(this._tilemapView.GetScreenPositionFromTileCoordinates(nextTileCoordinates), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
						this._lastPlacedCoordinates = nextTileCoordinates;
						PlayerAction.Log.Info("Set draft roundabout at {0}.", new object[]
						{
							this._lastPlacedCoordinates
						});
						this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOver, UpgradeType.Roundabout, true, null, default(Vector2)));
						this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.LightImpact);
					}
					else
					{
						if (this._result.edit != null)
						{
							base.Scope.Release(this._result.edit);
							this._result.edit = null;
						}
						if (hadPlacedRoundabout)
						{
							this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOut, UpgradeType.Roundabout, true, null, default(Vector2)));
						}
						this._gameUI.SetUpgradeCursorVisible(true);
					}
				}
				else
				{
					if (this._result.edit != null)
					{
						base.Scope.Release(this._result.edit);
						this._result.edit = null;
					}
					this._gameUI.SetUpgradeCursorVisible(true);
					this._result = TileEditResult.InvalidTileCoordinate(nextTileCoordinates);
				}
			}
			this._lastCheckedCoordinates = nextTileCoordinates;
		}

		// Token: 0x06003170 RID: 12656 RVA: 0x000E9C8E File Offset: 0x000E7E8E
		protected virtual void InitializeUpgradeCursor()
		{
			this._gameUI.InitializeUpgradeCursor(UpgradeType.Roundabout);
		}

		// Token: 0x06003171 RID: 12657 RVA: 0x000E9C9C File Offset: 0x000E7E9C
		protected virtual void UpdateUpgradeCursorPosition()
		{
			if (base.OwningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.TopLeft);
				return;
			}
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x06003172 RID: 12658 RVA: 0x000E9CEB File Offset: 0x000E7EEB
		private TileEditResult SetDraftRoundaboutAt(Vector2Int coordinates)
		{
			this._result = this._tileEditor.AddRoundabout(this._tilemapView, coordinates);
			return this._result;
		}

		// Token: 0x06003173 RID: 12659 RVA: 0x000E9D0C File Offset: 0x000E7F0C
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.Source != InputEventSource.Mouse)
			{
				this.OnActionComplete();
				return;
			}
			if (inputEvent.InputAction == 19 && inputEvent.ButtonState == InputEventButtonState.JustUp)
			{
				this.OnActionComplete();
				return;
			}
			if (inputEvent.InputAction == 18 || inputEvent.InputAction == 20)
			{
				this.OnActionCancel();
				return;
			}
			PlayerAction.Log.Error(string.Format("Unexpected mouse button index {0} with state {1} from input {2}!", inputEvent.InputAction, inputEvent.ButtonState, inputEvent), Array.Empty<object>());
			this.OnActionCancel();
		}

		// Token: 0x06003174 RID: 12660 RVA: 0x000E9D94 File Offset: 0x000E7F94
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			if (this._gameUI.HasUpgradeCursor)
			{
				this._gameUI.CancelUpgradeCursor();
			}
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			this._gameUI.UpgradeBar.AddToUpgradeButtonStack(UpgradeType.Roundabout, true, 1);
			this.CancelDrafts();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, UpgradeType.Roundabout, false, null, default(Vector2)));
		}

		// Token: 0x06003175 RID: 12661 RVA: 0x000E92B3 File Offset: 0x000E74B3
		private void CancelDrafts()
		{
			base.ClearDraftClientEdits();
		}

		// Token: 0x06003176 RID: 12662 RVA: 0x000E9E20 File Offset: 0x000E8020
		public override void OnActionComplete()
		{
			PlayerAction.Log.Info("Completing Roundabout Add action. Success? {0} {1}", new object[]
			{
				this._result.IsSuccessful,
				this._result.resultCode
			});
			if (!this._result.IsSuccessful || !this._result.edit.CanApplyToSimulation)
			{
				this.CancelDrafts();
				if (this._gameUI.HasUpgradeCursor)
				{
					this._gameUI.CancelUpgradeCursor();
				}
				this.OnActionCancel();
				return;
			}
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UpgradePlaced, this._gameCamera.GetPanFromWorld(TilemapView.GetWorldPositionForCoordinates(this._lastPlacedCoordinates)).x, -1f, true, null));
			if (this._gameUI.HasUpgradeCursor)
			{
				this._gameUI.PlaceUpgradeCursorAssetAtPosition(this._lastPlacedCoordinates);
				this._gameUI.UpgradeBar.AddToUpgradeButtonStack(UpgradeType.Roundabout, true, 1);
			}
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			base.ApplyDraftClientEdits();
			base.OnActionComplete();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, UpgradeType.Roundabout, true, null, default(Vector2)));
		}

		// Token: 0x06003177 RID: 12663 RVA: 0x000E9F78 File Offset: 0x000E8178
		public static DragRoundaboutAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragRoundaboutAction newAction = scope.Get<DragRoundaboutAction>();
			newAction.InitializeAction(owningGroup, timestamp);
			if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Mouse)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(19, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(20, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateMouseEventFilter(18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			else if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateTouchEventFilter(0, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
				newAction.BlockNewTouchUpgradeActions();
			}
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x06003178 RID: 12664 RVA: 0x000E9FFC File Offset: 0x000E81FC
		private bool IsTileMothballedRoundaboutCenter(Vector2Int tileCoordinates)
		{
			Vector2Int referenceOffset = Roundabout.GetCoordinatesOffsets()[0];
			Vector2Int referenceCoordinates = tileCoordinates + referenceOffset;
			Tile editedTile = this._tilemapView.GetTile(referenceCoordinates);
			return editedTile != null && editedTile.GetRoundaboutState(Roundabout.GetConnectionForCoordinatesOffset(referenceOffset)) == RoadState.Mothballed;
		}

		// Token: 0x04002A74 RID: 10868
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A75 RID: 10869
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002A76 RID: 10870
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04002A77 RID: 10871
		private Vector2Int _lastCheckedCoordinates;

		// Token: 0x04002A78 RID: 10872
		private Vector2Int _lastPlacedCoordinates;

		// Token: 0x04002A79 RID: 10873
		private TileEditResult _result;

		// Token: 0x04002A7A RID: 10874
		private bool _isRebuildingMothballedRoundabout;
	}
}
