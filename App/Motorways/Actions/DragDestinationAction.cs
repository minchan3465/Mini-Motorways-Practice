using System;
using Factory;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000702 RID: 1794
	public class DragDestinationAction : MotorwaysPlayerAction
	{
		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x06003123 RID: 12579 RVA: 0x000E7456 File Offset: 0x000E5656
		private UpgradeType UpgradeType
		{
			get
			{
				if (!this.isDouble)
				{
					return UpgradeType.Destination;
				}
				return UpgradeType.DoubleDestination;
			}
		}

		// Token: 0x06003124 RID: 12580 RVA: 0x000E7464 File Offset: 0x000E5664
		public override void Reset()
		{
			base.Reset();
			this._lastCheckedCoordinates = default(Vector2Int);
			this._lastPlacedCoordinates = default(Vector2Int);
			this._dragStartTileCoordinates = default(Vector2Int);
			this._originalDestinationCoordinates = default(Vector2Int);
			this.draftDestination = null;
			this._singleDestinationAboveDrivewayDirections = DrivewayDirection.West;
			this._singleDestinationToSideDrivewayDirections = DrivewayDirection.North;
			this._carparkSide = TileDirection.West;
			this._buildingLayout = BuildingLayout.BuildingAbove;
			this.isDouble = false;
			this.fromUpgradeMenu = false;
		}

		// Token: 0x06003125 RID: 12581 RVA: 0x000E74D8 File Offset: 0x000E56D8
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			if (this.fromUpgradeMenu)
			{
				this._gameUI.ConfirmEditMenuEdit();
			}
			this._gameUI.SetWorldGridActive(true, TransitionStyle.Tween);
			this._pivotCorner = DragDestinationAction.PivotCorner.BottomLeft;
			if (this.fromUpgradeMenu && this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(this.UpgradeType) < 1)
			{
				this.OnActionCancel();
				return;
			}
			this._buildingLayout = BuildingLayout.BuildingToSide;
			if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
			{
				this._gameUI.ToggleDrawMode();
			}
			if (this.fromUpgradeMenu)
			{
				this.InitializeUpgradeCursor();
				this._gameUI.UpgradeBar.RemoveFromUpgradeButtonStack(this.UpgradeType, true);
				this._lastCheckedCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
				this._gameUI.UpgradeBar.CreateAlertOnUpgradeButton(this.UpgradeType);
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragged, this.UpgradeType, true, null, default(Vector2)));
			}
			else
			{
				this._lastCheckedCoordinates = this.GetNextTileCoordinates();
				EditMenuPanel editMenuPanel = this._scope.Get<EditMenuPanel>();
				ICreativeModeEditableObject editableObject = editMenuPanel.EditableObject;
				bool wasOriginalDeleted;
				DraftDestination newDraftDestination = ((editableObject != null) ? editableObject.GetGhostPreview(out wasOriginalDeleted) : null) as DraftDestination;
				if (newDraftDestination != null)
				{
					if (wasOriginalDeleted)
					{
						editMenuPanel.CancelEdit();
					}
					this.draftDestination = newDraftDestination;
					this._singleDestinationAboveDrivewayDirections = this.draftDestination.viewModel.singleDestinationAboveDrivewayDirections;
					this._singleDestinationToSideDrivewayDirections = this.draftDestination.viewModel.singleDestinationToSideDrivewayDirections;
				}
			}
			if (this.draftDestination == null)
			{
				this.draftDestination = base.Scope.Get<DraftDestination>();
				this.draftDestination.Initialize(base.Scope, this.isDouble);
				PlayerAction.Log.Info("Spawned draft carpark at {0}", new object[]
				{
					base.GetPointerTilePosition()
				});
			}
			DragDestinationAction.Log.Info("Beginning DragDestinationAction from tile coordinates {0}.", new object[]
			{
				this._lastCheckedCoordinates
			});
		}

		// Token: 0x06003126 RID: 12582 RVA: 0x000E76C0 File Offset: 0x000E58C0
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			if (this.fromUpgradeMenu)
			{
				this.UpdateUpgradeCursorPosition();
			}
			Vector2Int nextTileCoordinates = this.GetNextTileCoordinates();
			bool hadPlacedDestination = this.draftDestination != null;
			if (nextTileCoordinates != this._lastCheckedCoordinates && (!hadPlacedDestination || nextTileCoordinates != this._lastPlacedCoordinates))
			{
				Vector2Int newDestinationCoordinates = this._originalDestinationCoordinates + nextTileCoordinates - this._dragStartTileCoordinates + this.GetDragOffsetTileCoordinates();
				if (hadPlacedDestination)
				{
					this.draftDestination.UpdatePosition(newDestinationCoordinates, false);
					this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragSnap, UpgradeType.Destination, true, null, this._tilemapView.GetScreenPositionFromTileCoordinates(nextTileCoordinates)));
				}
				this._lastPlacedCoordinates = this.draftDestination.BottomLeftCoordinate;
			}
			this._lastCheckedCoordinates = nextTileCoordinates;
		}

		// Token: 0x06003127 RID: 12583 RVA: 0x000E7788 File Offset: 0x000E5988
		private Vector2Int GetDragOffsetTileCoordinates()
		{
			if (this.isDouble)
			{
				BuildingLayout buildingLayout = this._buildingLayout;
				if (buildingLayout == BuildingLayout.BuildingAbove)
				{
					return new Vector2Int(0, 0);
				}
				if (buildingLayout == BuildingLayout.BuildingToSide)
				{
					return new Vector2Int(0, -2);
				}
			}
			else
			{
				BuildingLayout buildingLayout = this._buildingLayout;
				if (buildingLayout == BuildingLayout.BuildingAbove)
				{
					return new Vector2Int(0, 0);
				}
				if (buildingLayout == BuildingLayout.BuildingToSide)
				{
					return new Vector2Int(-1, 1);
				}
			}
			return new Vector2Int(0, 0);
		}

		// Token: 0x06003128 RID: 12584 RVA: 0x000E77E4 File Offset: 0x000E59E4
		private Vector2Int GetNextTileCoordinates()
		{
			switch (this._pivotCorner)
			{
			case DragDestinationAction.PivotCorner.TopLeft:
				return base.GetPointerTilePosition() + (this.isDouble ? 3 : 1) * Vector2Int.down;
			case DragDestinationAction.PivotCorner.TopRight:
				return base.GetPointerTilePosition() + 2 * Vector2Int.left + (this.isDouble ? 3 : 1) * Vector2Int.down;
			case DragDestinationAction.PivotCorner.BottomLeft:
				return base.GetPointerTilePosition();
			case DragDestinationAction.PivotCorner.BottomRight:
				return base.GetPointerTilePosition() + 2 * Vector2Int.left;
			default:
				return base.GetPointerTilePosition();
			}
		}

		// Token: 0x06003129 RID: 12585 RVA: 0x000E7888 File Offset: 0x000E5A88
		protected virtual void InitializeUpgradeCursor()
		{
			this._gameUI.InitializeUpgradeCursor(this.UpgradeType);
			this._gameUI.SetUpgradeCursorVisible(false);
		}

		// Token: 0x0600312A RID: 12586 RVA: 0x000E78A8 File Offset: 0x000E5AA8
		protected virtual void UpdateUpgradeCursorPosition()
		{
			if (base.OwningGroup.InstigatingInputEvent.Source == InputEventSource.Touch)
			{
				this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.TopLeft);
				return;
			}
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x0600312B RID: 12587 RVA: 0x000E78F8 File Offset: 0x000E5AF8
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if ((inputEvent.Source == InputEventSource.Mouse && inputEvent.InputAction == 19 && inputEvent.ButtonState == InputEventButtonState.JustUp) || (inputEvent.Source == InputEventSource.Touch && inputEvent.ButtonState == InputEventButtonState.JustUp) || (inputEvent.Source == InputEventSource.Remote && inputEvent.InputAction == 2 && inputEvent.ButtonState == InputEventButtonState.JustDown))
			{
				if (this.draftDestination == null || this.draftDestination.CompletelyOutOfPlayArea(this._city))
				{
					this.OnActionCancel();
					return;
				}
				this.OnActionComplete();
				return;
			}
			else
			{
				if (inputEvent.InputAction == 18 || inputEvent.InputAction == 20)
				{
					this.OnActionCancel();
					return;
				}
				DragDestinationAction.Log.Error(string.Format("Unexpected input: {0}!", inputEvent), Array.Empty<object>());
				this.OnActionCancel();
				return;
			}
		}

		// Token: 0x0600312C RID: 12588 RVA: 0x000E79B8 File Offset: 0x000E5BB8
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			if (this._gameUI.HasUpgradeCursor && this.fromUpgradeMenu)
			{
				this._gameUI.UpgradeBar.AddToUpgradeButtonStack(this.UpgradeType, true, 1);
				this._gameUI.CancelUpgradeCursor();
			}
			this.ClearOutAllDrafts();
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
			}
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, this.UpgradeType, false, null, default(Vector2)));
		}

		// Token: 0x0600312D RID: 12589 RVA: 0x000E7A4C File Offset: 0x000E5C4C
		private void ClearOutAllDrafts()
		{
			if (this.draftDestination != null)
			{
				if (this.fromUpgradeMenu)
				{
					base.Scope.Release(this.draftDestination);
				}
				else if (this._gameUI.editMenuPanel.EditableObject != null)
				{
					this._gameUI.editMenuPanel.CancelEdit();
				}
				else
				{
					this.draftDestination.Cancel();
				}
				this.draftDestination = null;
			}
		}

		// Token: 0x0600312E RID: 12590 RVA: 0x000E7ABC File Offset: 0x000E5CBC
		public override void OnActionComplete()
		{
			DragDestinationAction.Log.Info("Completing Drag Destination action.", Array.Empty<object>());
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UpgradePlaced, this._gameCamera.GetPanFromWorld(TilemapView.GetWorldPositionForCoordinates(this._lastPlacedCoordinates)).x, -1f, true, null));
			if (this._gameUI.HasUpgradeCursor && this.fromUpgradeMenu)
			{
				this._gameUI.PlaceUpgradeCursorAssetAtPosition(this._lastPlacedCoordinates);
				this._gameUI.UpgradeBar.AddToUpgradeButtonStack(this.UpgradeType, true, 1);
			}
			if (!this._cameraView.playerZoomedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			base.Scope.Get<GameUIScreen>().OpenEditMenu(this.draftDestination, this.fromUpgradeMenu);
			base.OnActionComplete();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, this.UpgradeType, true, null, default(Vector2)));
		}

		// Token: 0x0600312F RID: 12591 RVA: 0x000E7BCE File Offset: 0x000E5DCE
		public static DragDestinationAction CreateSingleFromEditMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return DragDestinationAction.Create(owningGroup, scope, timestamp, false, false);
		}

		// Token: 0x06003130 RID: 12592 RVA: 0x000E7BDA File Offset: 0x000E5DDA
		public static DragDestinationAction CreateDoubleFromEditMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return DragDestinationAction.Create(owningGroup, scope, timestamp, true, false);
		}

		// Token: 0x06003131 RID: 12593 RVA: 0x000E7BE6 File Offset: 0x000E5DE6
		public static DragDestinationAction CreateSingleFromUpgradeMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return DragDestinationAction.Create(owningGroup, scope, timestamp, false, true);
		}

		// Token: 0x06003132 RID: 12594 RVA: 0x000E7BF2 File Offset: 0x000E5DF2
		public static DragDestinationAction CreateDoubleFromUpgradeMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return DragDestinationAction.Create(owningGroup, scope, timestamp, true, true);
		}

		// Token: 0x06003133 RID: 12595 RVA: 0x000E7C00 File Offset: 0x000E5E00
		private static DragDestinationAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp, bool isDouble, bool fromUpgradeMenu)
		{
			DragDestinationAction newAction = scope.Get<DragDestinationAction>();
			newAction.isDouble = isDouble;
			newAction.fromUpgradeMenu = fromUpgradeMenu;
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
			else if (owningGroup.InstigatingInputEvent.Source == InputEventSource.Remote)
			{
				newAction.RegisterObserveInputEvent(InputEventFilter.CreateRemoteEventFilter(2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			}
			newAction.OnActionBegin(timestamp);
			newAction.MakeExclusive();
			newAction.SetWorldGridVisible(true);
			return newAction;
		}

		// Token: 0x04002A35 RID: 10805
		private new static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DragDestinationAction");

		// Token: 0x04002A36 RID: 10806
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A37 RID: 10807
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002A38 RID: 10808
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04002A39 RID: 10809
		[Dependency]
		private BuildingPlacer _placer;

		// Token: 0x04002A3A RID: 10810
		[Dependency]
		private TilemapModel _tilemapModel;

		// Token: 0x04002A3B RID: 10811
		[Dependency]
		private IScope _scope;

		// Token: 0x04002A3C RID: 10812
		private Vector2Int _lastCheckedCoordinates;

		// Token: 0x04002A3D RID: 10813
		private Vector2Int _lastPlacedCoordinates;

		// Token: 0x04002A3E RID: 10814
		protected DraftDestination draftDestination;

		// Token: 0x04002A3F RID: 10815
		protected bool fromUpgradeMenu;

		// Token: 0x04002A40 RID: 10816
		private Vector2Int _dragStartTileCoordinates;

		// Token: 0x04002A41 RID: 10817
		private Vector2Int _originalDestinationCoordinates;

		// Token: 0x04002A42 RID: 10818
		private Vector2Int _previousDragCoordinates;

		// Token: 0x04002A43 RID: 10819
		private Vector2Int _previousDestinationCoordinates;

		// Token: 0x04002A44 RID: 10820
		protected bool isDouble;

		// Token: 0x04002A45 RID: 10821
		private BuildingLayout _buildingLayout;

		// Token: 0x04002A46 RID: 10822
		private DrivewayDirection _singleDestinationAboveDrivewayDirections;

		// Token: 0x04002A47 RID: 10823
		private DrivewayDirection _singleDestinationToSideDrivewayDirections = DrivewayDirection.North;

		// Token: 0x04002A48 RID: 10824
		private TileDirection _carparkSide = TileDirection.West;

		// Token: 0x04002A49 RID: 10825
		private DragDestinationAction.PivotCorner _pivotCorner = DragDestinationAction.PivotCorner.TopRight;

		// Token: 0x02000703 RID: 1795
		private enum PivotCorner
		{
			// Token: 0x04002A4B RID: 10827
			TopLeft,
			// Token: 0x04002A4C RID: 10828
			TopRight,
			// Token: 0x04002A4D RID: 10829
			BottomLeft,
			// Token: 0x04002A4E RID: 10830
			BottomRight
		}
	}
}
