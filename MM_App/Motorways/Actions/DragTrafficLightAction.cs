using System;
using Client;
using Factory;
using Motorways.Audio;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x0200070A RID: 1802
	public class DragTrafficLightAction : MotorwaysPlayerAction
	{
		// Token: 0x0600317A RID: 12666 RVA: 0x000EA040 File Offset: 0x000E8240
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			if (this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.TrafficLight) < 1)
			{
				this.OnActionCancel();
				return;
			}
			if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
			{
				this._gameUI.ToggleDrawMode();
			}
			this.result = TileEditResult.InvalidTileCoordinate(Vector2Int.zero);
			this.InitializeUpgradeCursor();
			this._gameUI.UpgradeBar.RemoveFromUpgradeButtonStack(UpgradeType.TrafficLight, true);
			this._currentCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
			PlayerAction.Log.Info("Beginning DragTrafficLightAction from tile coordinates {0}.", new object[]
			{
				this._currentCoordinates
			});
			this._gameUI.UpgradeBar.CreateAlertOnUpgradeButton(UpgradeType.TrafficLight);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragged, UpgradeType.TrafficLight, true, null, default(Vector2)));
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(true, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Edit;
			}
		}

		// Token: 0x0600317B RID: 12667 RVA: 0x000EA140 File Offset: 0x000E8340
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this.UpdateUpgradeCursorPosition();
			Vector2Int nextTileCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
			if (nextTileCoordinates != this._currentCoordinates || !this.result.IsSuccessful)
			{
				this.result = this.SetDraftTrafficLightsAt(nextTileCoordinates);
				if (this.result.IsSuccessful)
				{
					base.ClearDraftClientEdits();
					base.AddTileEdit(this.result.edit, MotorwaysPlayerAction.EditExecuteTiming.Draft);
					this._gameUI.SetUpgradeCursorVisible(false);
					this._gameUI.SetUpgradeCursorPosition(this._tilemapView.GetScreenPositionFromTileCoordinates(nextTileCoordinates), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
					this._selectedCoordinates = nextTileCoordinates;
					PlayerAction.Log.Info("Set draft traffic light at {0}.", new object[]
					{
						this._selectedCoordinates
					});
					AlertView.Create(this._viewClient, TilemapView.GetWorldPositionForCoordinates(nextTileCoordinates), new Color?(this._theme.GetGlobalColor(this._constants.UpgradeAlertColor)), new float?(1f), new float?(0.8f), null);
					this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOver, UpgradeType.TrafficLight, true, null, default(Vector2)));
					this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.LightImpact);
				}
				else
				{
					if (this.result.edit != null)
					{
						base.Scope.Release(this.result.edit);
					}
					if (Vector2Int.Distance(this._selectedCoordinates, nextTileCoordinates) >= 1f)
					{
						if (base.HasSchedulableClientEdits)
						{
							this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOut, UpgradeType.TrafficLight, true, null, default(Vector2)));
						}
						base.ClearDraftClientEdits();
						this._gameUI.SetUpgradeCursorVisible(true);
						this._selectedCoordinates = default(Vector2Int);
					}
				}
			}
			this._currentCoordinates = nextTileCoordinates;
		}

		// Token: 0x0600317C RID: 12668 RVA: 0x000EA30D File Offset: 0x000E850D
		protected virtual void InitializeUpgradeCursor()
		{
			this._gameUI.InitializeUpgradeCursor(UpgradeType.TrafficLight);
		}

		// Token: 0x0600317D RID: 12669 RVA: 0x000EA31C File Offset: 0x000E851C
		protected virtual void UpdateUpgradeCursorPosition()
		{
			if (base.OwningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.TopLeft);
				return;
			}
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x0600317E RID: 12670 RVA: 0x000EA36B File Offset: 0x000E856B
		protected TileEditResult SetDraftTrafficLightsAt(Vector2Int coordinates)
		{
			this.result = this._tileEditor.AddTrafficLight(this._tilemapView, coordinates);
			return this.result;
		}

		// Token: 0x0600317F RID: 12671 RVA: 0x000EA38C File Offset: 0x000E858C
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

		// Token: 0x06003180 RID: 12672 RVA: 0x000EA414 File Offset: 0x000E8614
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
			this._gameUI.UpgradeBar.AddToUpgradeButtonStack(UpgradeType.TrafficLight, true, 1);
			this.CancelDrafts();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, UpgradeType.TrafficLight, false, null, default(Vector2)));
		}

		// Token: 0x06003181 RID: 12673 RVA: 0x000E92B3 File Offset: 0x000E74B3
		protected void CancelDrafts()
		{
			base.ClearDraftClientEdits();
		}

		// Token: 0x06003182 RID: 12674 RVA: 0x000EA4A0 File Offset: 0x000E86A0
		public override void OnActionComplete()
		{
			PlayerAction.Log.Info("Completing Traffic Light Add action. Success? {0} {1}", new object[]
			{
				this.result.IsSuccessful,
				this.result.resultCode
			});
			if (!this.result.IsSuccessful || !this.result.edit.CanApplyToSimulation)
			{
				this.CancelDrafts();
				if (this._gameUI.HasUpgradeCursor)
				{
					this._gameUI.CancelUpgradeCursor();
				}
				this.OnActionCancel();
				return;
			}
			if (this._gameUI.HasUpgradeCursor)
			{
				this._gameUI.PlaceUpgradeCursorAssetAtPosition(this._selectedCoordinates);
			}
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			base.ApplyDraftClientEdits();
			base.OnActionComplete();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, UpgradeType.TrafficLight, true, null, default(Vector2)));
		}

		// Token: 0x06003183 RID: 12675 RVA: 0x000EA59E File Offset: 0x000E879E
		public override void Reset()
		{
			base.Reset();
			this._currentCoordinates = Vector2Int.zero;
			this._selectedCoordinates = Vector2Int.zero;
			this.result = default(TileEditResult);
		}

		// Token: 0x06003184 RID: 12676 RVA: 0x000EA5C8 File Offset: 0x000E87C8
		public static DragTrafficLightAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragTrafficLightAction newAction = scope.Get<DragTrafficLightAction>();
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

		// Token: 0x04002A7B RID: 10875
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A7C RID: 10876
		[Dependency]
		protected ViewClient _viewClient;

		// Token: 0x04002A7D RID: 10877
		[Dependency]
		protected CameraView _cameraView;

		// Token: 0x04002A7E RID: 10878
		[Dependency]
		protected MotorwaysThemeDatabase _theme;

		// Token: 0x04002A7F RID: 10879
		[Dependency]
		protected VisualConstantsData _constants;

		// Token: 0x04002A80 RID: 10880
		protected Vector2Int _currentCoordinates;

		// Token: 0x04002A81 RID: 10881
		protected Vector2Int _selectedCoordinates;

		// Token: 0x04002A82 RID: 10882
		protected TileEditResult result;
	}
}
