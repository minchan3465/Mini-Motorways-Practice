using System;
using Client;
using Factory;
using Motorways.Audio;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000706 RID: 1798
	public class DragMotorwayAction : AddMotorwayAction
	{
		// Token: 0x0600314E RID: 12622 RVA: 0x000E8E41 File Offset: 0x000E7041
		public override void Reset()
		{
			this._currentCoordinates = default(Vector2Int);
			this._hasScheduledOverEvent = false;
			this._editResult = default(TileEditResult);
			base.Reset();
		}

		// Token: 0x0600314F RID: 12623 RVA: 0x000E8E68 File Offset: 0x000E7068
		public override void OnActionBegin(float timestamp)
		{
			if (this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.Motorway) < 1)
			{
				this.OnActionCancel();
				return;
			}
			if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
			{
				this._gameUI.ToggleDrawMode();
			}
			base.OnActionBegin(timestamp);
			this._hasScheduledOverEvent = false;
			this._editResult = TileEditResult.InvalidTileCoordinate(this._currentCoordinates);
			this.InitializeUpgradeCursor();
			this._gameUI.UpgradeBar.RemoveFromUpgradeButtonStack(UpgradeType.Motorway, true);
			this._newMotorwayId = -1;
			this._currentCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
			PlayerAction.Log.Info("Beginning DragMotorwayAction from tile coordinates {0}.", new object[]
			{
				this._currentCoordinates
			});
			this._gameUI.UpgradeBar.CreateAlertOnUpgradeButton(UpgradeType.Motorway);
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(true, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Edit;
			}
			this._gameUI.SetMotorwayGridActive(true, TransitionStyle.Tween);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragged, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
		}

		// Token: 0x06003150 RID: 12624 RVA: 0x000E8F84 File Offset: 0x000E7184
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this.UpdateUpgradeCursorPosition();
			Vector2Int nextTileCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
			if (nextTileCoordinates != this._currentCoordinates)
			{
				if (base.DoesTileSupportMotorway(nextTileCoordinates) && !base.HasMotorwayOnTile(nextTileCoordinates, -1))
				{
					this._editResult = this.SetDraftMotorwayAt(nextTileCoordinates);
					if (this._editResult.IsSuccessful)
					{
						base.ClearDraftClientEdits();
						base.AddTileEdit(this._editResult.edit, MotorwaysPlayerAction.EditExecuteTiming.Draft);
						this._gameUI.SetUpgradeCursorVisible(false);
						this._gameUI.SetUpgradeCursorPosition(this._tilemapView.GetScreenPositionFromTileCoordinates(nextTileCoordinates), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
						PlayerAction.Log.Info("Set draft motorway at {0}.", new object[]
						{
							nextTileCoordinates
						});
						this._hasScheduledOverEvent = true;
						this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOver, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
						this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.LightImpact);
					}
					else
					{
						if (this._hasScheduledOverEvent)
						{
							this._hasScheduledOverEvent = false;
							this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOut, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
						}
						if (this._editResult.edit != null)
						{
							base.Scope.Release(this._editResult.edit);
						}
					}
				}
				else
				{
					base.ClearDraftClientEdits();
					if (this._editResult.edit != null)
					{
						base.Scope.Release(this._editResult.edit);
					}
					this._gameUI.SetUpgradeCursorVisible(true);
					this._editResult = TileEditResult.InvalidTileCoordinate(this._currentCoordinates);
				}
			}
			this._currentCoordinates = nextTileCoordinates;
		}

		// Token: 0x06003151 RID: 12625 RVA: 0x000E913A File Offset: 0x000E733A
		protected virtual void InitializeUpgradeCursor()
		{
			this._gameUI.InitializeUpgradeCursor(UpgradeType.Motorway);
		}

		// Token: 0x06003152 RID: 12626 RVA: 0x000E9148 File Offset: 0x000E7348
		protected virtual void UpdateUpgradeCursorPosition()
		{
			if (base.OwningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.TopLeft);
				return;
			}
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x06003153 RID: 12627 RVA: 0x000E9198 File Offset: 0x000E7398
		private TileEditResult SetDraftMotorwayAt(Vector2Int coordinates)
		{
			if (this._newMotorwayId == -1)
			{
				this._newMotorwayId = this._city.GetNextMotorwayIdAndIncrement();
			}
			if (this._newMotorwayNumber == 0)
			{
				this._newMotorwayNumber = this._tilemapView.GetLowestAvailableMotorwayNumber();
			}
			this._editResult = this._tileEditor.AddUnbuiltMotorway(this._tilemapView, this._newMotorwayId, this._newMotorwayNumber, coordinates);
			base.SetAnchorTile(coordinates, TileDirection.None);
			return this._editResult;
		}

		// Token: 0x06003154 RID: 12628 RVA: 0x000E920B File Offset: 0x000E740B
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.InputAction == 18 || inputEvent.InputAction == 20)
			{
				this.OnActionCancel();
				return;
			}
			this.OnActionComplete();
		}

		// Token: 0x06003155 RID: 12629 RVA: 0x000E9230 File Offset: 0x000E7430
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			if (this._gameUI.HasUpgradeCursor)
			{
				this._gameUI.SetUpgradeCursorVisible(false);
				this._gameUI.CancelUpgradeCursor();
			}
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			this._gameUI.SetMotorwayGridActive(false, TransitionStyle.Tween);
			this._gameUI.UpgradeBar.AddToUpgradeButtonStack(UpgradeType.Motorway, true, 1);
			this.CancelDrafts();
		}

		// Token: 0x06003156 RID: 12630 RVA: 0x000E92B3 File Offset: 0x000E74B3
		protected void CancelDrafts()
		{
			base.ClearDraftClientEdits();
		}

		// Token: 0x06003157 RID: 12631 RVA: 0x000E92BC File Offset: 0x000E74BC
		public override void OnActionComplete()
		{
			this._gameUI.SetMotorwayGridActive(false, TransitionStyle.Tween);
			PlayerAction.Log.Info("Completing MotorwayAdd action. Success? {0} {1}", new object[]
			{
				this._editResult.IsSuccessful,
				this._editResult.resultCode
			});
			if (!this._editResult.IsSuccessful || !this._editResult.edit.CanApplyToSimulation)
			{
				this.CancelDrafts();
				if (this._gameUI.HasUpgradeCursor)
				{
					this._gameUI.SetUpgradeCursorVisible(false);
					this._gameUI.CancelUpgradeCursor();
				}
				this.OnActionCancel();
				return;
			}
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UpgradePlaced, this._gameCamera.GetPanFromWorld(TilemapView.GetWorldPositionForCoordinates(this._currentCoordinates)).x, -1f, true, null));
			if (this._gameUI.HasUpgradeCursor)
			{
				this._gameUI.PlaceUpgradeCursorAssetAtPosition(this._currentCoordinates);
			}
			AlertView.Create(this._viewClient, TilemapView.GetWorldPositionForCoordinates(this._currentCoordinates), new Color?(this._theme.GetGlobalColor(this._constants.UpgradeAlertColor)), new float?(1f), new float?(0.8f), null);
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			base.ApplyDraftClientEdits();
			base.OnActionComplete();
		}

		// Token: 0x06003158 RID: 12632 RVA: 0x000E9444 File Offset: 0x000E7644
		protected override TileEditResult CreateTileEdit(int newMotorwayId, int motorwayNumber, Vector2Int anchorCoordinates, TileDirection anchorDirection, Vector2Int danglingCoordinates, TileDirection danglingDirection)
		{
			return new TileEditResult
			{
				resultCode = TileEditResultCode.Success
			};
		}

		// Token: 0x06003159 RID: 12633 RVA: 0x000E9464 File Offset: 0x000E7664
		public static DragMotorwayAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragMotorwayAction newAction = scope.Get<DragMotorwayAction>();
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

		// Token: 0x04002A65 RID: 10853
		[Dependency]
		protected ViewClient _viewClient;

		// Token: 0x04002A66 RID: 10854
		[Dependency]
		protected CameraView _cameraView;

		// Token: 0x04002A67 RID: 10855
		[Dependency]
		protected GameCamera _gameCamera;

		// Token: 0x04002A68 RID: 10856
		[Dependency]
		protected MotorwaysThemeDatabase _theme;

		// Token: 0x04002A69 RID: 10857
		[Dependency]
		protected VisualConstantsData _constants;

		// Token: 0x04002A6A RID: 10858
		private Vector2Int _currentCoordinates;

		// Token: 0x04002A6B RID: 10859
		private bool _hasScheduledOverEvent;

		// Token: 0x04002A6C RID: 10860
		protected TileEditResult _editResult;
	}
}
