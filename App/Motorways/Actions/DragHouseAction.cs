using System;
using Factory;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000705 RID: 1797
	public class DragHouseAction : MotorwaysPlayerAction
	{
		// Token: 0x0600313F RID: 12607 RVA: 0x000E8310 File Offset: 0x000E6510
		public override void Reset()
		{
			base.Reset();
			this._lastCheckedCoordinates = default(Vector2Int);
			this._lastPlacedCoordinates = default(Vector2Int);
			this.draftHouse = null;
			this._previousDragCoordinates = default(Vector2Int);
			this._previousHouseCoordinates = default(Vector2Int);
			this._groupIndex = 0;
			this._drivewayDirection = TileDirection.North;
			this.fromUpgradeMenu = false;
		}

		// Token: 0x06003140 RID: 12608 RVA: 0x000E8370 File Offset: 0x000E6570
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this.SetColourWidgetRadialVisible(false);
			if (this.fromUpgradeMenu)
			{
				this._gameUI.ConfirmEditMenuEdit();
			}
			this._gameUI.SetWorldGridActive(true, TransitionStyle.Tween);
			this.InitializeUpgradeCursor();
			EditMenuPanel editMenuPanel = this._scope.Get<EditMenuPanel>();
			if (!this.fromUpgradeMenu)
			{
				ICreativeModeEditableObject editableObject = editMenuPanel.EditableObject;
				bool wasOriginalDeleted = false;
				if (editableObject != null)
				{
					Vector2Int initialTilePosition = editableObject.GetTilePosition();
					CreativeModeEditableHouse house = editableObject as CreativeModeEditableHouse;
					if (house != null)
					{
						this._groupIndex = house.GroupIndex;
						this._drivewayDirection = house.DrivewayDirection;
						this.draftHouse = (house.GetGhostPreview(out wasOriginalDeleted) as DraftHouse);
						if (wasOriginalDeleted)
						{
							editMenuPanel.CancelEdit();
						}
					}
					else if (editableObject is DraftHouse)
					{
						this.draftHouse = (editMenuPanel.EditableObject as DraftHouse);
					}
					else
					{
						DragHouseAction.Log.Error("There should always be either a draft house or a creative mode editable house associated with DragHouseAction.", Array.Empty<object>());
					}
					this._lastCheckedCoordinates = initialTilePosition;
					this.PlaceHousePreview(initialTilePosition, wasOriginalDeleted, ref this.draftHouse);
					return;
				}
			}
			else
			{
				if (editMenuPanel.isActiveAndEnabled && editMenuPanel.EditableObject != null)
				{
					this.OnActionCancel();
					return;
				}
				if (this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.House) < 1)
				{
					this.OnActionCancel();
					return;
				}
				if (this._gameUI.CurrentRoadDrawMode == RoadDrawMode.Remove)
				{
					this._gameUI.ToggleDrawMode();
				}
				this._gameUI.UpgradeBar.RemoveFromUpgradeButtonStack(UpgradeType.House, true);
				this._lastCheckedCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
				DragHouseAction.Log.Info("Beginning DragHouseAction from tile coordinates {0}.", new object[]
				{
					this._lastCheckedCoordinates
				});
				this._gameUI.UpgradeBar.CreateAlertOnUpgradeButton(UpgradeType.House);
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragged, UpgradeType.House, true, null, default(Vector2)));
			}
		}

		// Token: 0x06003141 RID: 12609 RVA: 0x000E852C File Offset: 0x000E672C
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this.UpdateUpgradeCursorPosition();
			Vector2Int nextTileCoordinates = this._gameUI.GetUpgradeCursorTileCoordinates();
			bool hadPlacedHouse = base.HasSchedulableClientEdits;
			if (nextTileCoordinates != this._lastCheckedCoordinates && (!hadPlacedHouse || nextTileCoordinates != this._lastPlacedCoordinates))
			{
				if (hadPlacedHouse)
				{
					base.ClearDraftClientEdits();
				}
				Vector2Int newHouseCoordinates = nextTileCoordinates;
				if (this.PlaceHousePreview(newHouseCoordinates, false, ref this.draftHouse))
				{
					this._lastPlacedCoordinates = this.draftHouse.tilePosition;
					if (this.fromUpgradeMenu)
					{
						this._upgradeDatabase.ConsumeUpgrade(UpgradeType.House, 1);
					}
					if (this.draftHouse != null && this.draftHouse.HasUnplaceableView)
					{
						this.draftHouse.EndUnplaceableView();
					}
					this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeDragSnap, UpgradeType.House, true, null, this._tilemapView.GetScreenPositionFromTileCoordinates(nextTileCoordinates)));
				}
				else if (this.draftHouse != null && !this.draftHouse.HasUnplaceableView)
				{
					this.draftHouse.StartUnplaceableView();
				}
			}
			this._lastCheckedCoordinates = nextTileCoordinates;
			if (this.draftHouse != null)
			{
				this.draftHouse.UpdatePosition(this._lastCheckedCoordinates);
				this.draftHouse.UpdateDrivewayPosition(this._drivewayDirection);
				if (this.draftHouse.IsTicking)
				{
					this.draftHouse.Tick(frameTime);
				}
			}
		}

		// Token: 0x06003142 RID: 12610 RVA: 0x000E8686 File Offset: 0x000E6886
		protected virtual void InitializeUpgradeCursor()
		{
			this._gameUI.InitializeUpgradeCursor(UpgradeType.House);
			this._gameUI.SetUpgradeCursorVisible(false);
		}

		// Token: 0x06003143 RID: 12611 RVA: 0x000E86A0 File Offset: 0x000E68A0
		protected virtual void UpdateUpgradeCursorPosition()
		{
			if (this._inputState.CurrentDeviceInputType == DeviceInputType.Touch)
			{
				this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.TopLeft);
				return;
			}
			this._gameUI.SetUpgradeCursorPosition(base.GetPointerScreenPosition(), UpgradeCursor.UpgradeCursorOffsetType.OnPointer);
		}

		// Token: 0x06003144 RID: 12612 RVA: 0x000E86E0 File Offset: 0x000E68E0
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if ((inputEvent.Source == InputEventSource.Mouse && inputEvent.InputAction == 19 && inputEvent.ButtonState == InputEventButtonState.JustUp) || (inputEvent.Source == InputEventSource.Touch && inputEvent.ButtonState == InputEventButtonState.JustUp) || (inputEvent.Source == InputEventSource.Remote && inputEvent.InputAction == 2 && inputEvent.ButtonState == InputEventButtonState.JustDown))
			{
				if (this.draftHouse == null || this.draftHouse.CompletelyOutOfPlayArea(this._city))
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
				DragHouseAction.Log.Error(string.Format("Unexpected input: {0}!", inputEvent), Array.Empty<object>());
				this.OnActionCancel();
				return;
			}
		}

		// Token: 0x06003145 RID: 12613 RVA: 0x000E87A0 File Offset: 0x000E69A0
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			if (this._gameUI.HasUpgradeCursor)
			{
				if (this.fromUpgradeMenu)
				{
					this._gameUI.UpgradeBar.AddToUpgradeButtonStack(UpgradeType.House, true, 1);
				}
				this._gameUI.CancelUpgradeCursor();
			}
			this.ClearOutAllDrafts();
			if (!this._cameraView.IsFocussedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
			}
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, UpgradeType.House, false, null, default(Vector2)));
		}

		// Token: 0x06003146 RID: 12614 RVA: 0x000E8828 File Offset: 0x000E6A28
		private void ClearOutAllDrafts()
		{
			if (this.draftHouse != null)
			{
				if (this.fromUpgradeMenu)
				{
					base.Scope.Release(this.draftHouse);
				}
				else if (this._gameUI.editMenuPanel.EditableObject != null)
				{
					this._gameUI.editMenuPanel.CancelEdit();
				}
				else
				{
					this.draftHouse.Cancel();
				}
				this.draftHouse = null;
			}
		}

		// Token: 0x06003147 RID: 12615 RVA: 0x000E8898 File Offset: 0x000E6A98
		public override void OnActionComplete()
		{
			DragHouseAction.Log.Info("Completing House Add action.", Array.Empty<object>());
			this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UpgradePlaced, this._gameCamera.GetPanFromWorld(TilemapView.GetWorldPositionForCoordinates(this._lastPlacedCoordinates)).x, -1f, true, null));
			if (this._gameUI.HasUpgradeCursor)
			{
				if (this.fromUpgradeMenu)
				{
					this._gameUI.PlaceUpgradeCursorAssetAtPosition(this._lastPlacedCoordinates);
					this._gameUI.UpgradeBar.AddToUpgradeButtonStack(UpgradeType.House, true, 1);
				}
				this._gameUI.CancelUpgradeCursor();
			}
			if (!this._cameraView.playerZoomedIn)
			{
				this._gameUI.SetWorldGridActive(false, TransitionStyle.Tween);
				this._tilemapView.viewMode = TilemapView.ViewMode.Normal;
			}
			base.Scope.Get<GameUIScreen>().OpenEditMenu(this.draftHouse, this.fromUpgradeMenu);
			base.OnActionComplete();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeReleased, UpgradeType.House, true, null, default(Vector2)));
		}

		// Token: 0x06003148 RID: 12616 RVA: 0x000E89AC File Offset: 0x000E6BAC
		private bool PlaceHousePreview(Vector2Int houseTileCoordinates, bool isReplacement, ref DraftHouse draftHouse)
		{
			bool housePlaceable = true;
			this._groupIndex = this._scope.Get<ColourWidget>().CurrentColour;
			if (!this._city.IsTileInPlayableArea(houseTileCoordinates, this._clockModel.ExpansionTime))
			{
				DragHouseAction.Log.Info("House coordinates {0} are outside playable area.", Array.Empty<object>());
				housePlaceable = false;
			}
			Tile houseTile = this._tilemapView.GetTile(houseTileCoordinates);
			if (houseTile != null && (houseTile.ContentType != TileContentType.None || houseTile.GetTwoLaneRoadCount(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed, Tile.MotorwayInclusion.Include) > 0 || houseTile.HasRailConnection))
			{
				if (houseTile.ContentType == TileContentType.Tree && this._city.Rules.ShouldBuildingsBulldozeTrees)
				{
					DragHouseAction.Log.Info("Allowing placement over tree at {0} as this will get bulldozed", new object[]
					{
						houseTileCoordinates
					});
				}
				else if (houseTile.ContentType == TileContentType.House && isReplacement)
				{
					DragHouseAction.Log.Info("Allowing placement over house, as that is this ghost previews old self", Array.Empty<object>());
				}
				else
				{
					DragHouseAction.Log.Info("Cannot build house on tile {0} as it already has contents or road", new object[]
					{
						houseTileCoordinates
					});
					housePlaceable = false;
				}
			}
			if (houseTile != null && (houseTile.IsCenterOfRoundabout || houseTile.HasRoundabout(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed)))
			{
				DragHouseAction.Log.Info("Cannot build house on tile {0} as it contains a roundabout", new object[]
				{
					houseTile.Coordinates
				});
				housePlaceable = false;
			}
			if (!this._city.Definition.TileIsBuildable(houseTileCoordinates) || this._city.Definition.TileIsOverWater(houseTileCoordinates) || this._city.Definition.TileIsUnderAMountain(houseTileCoordinates))
			{
				DragHouseAction.Log.Info("Can't place destination over tile at {0} because it's {1}", new object[]
				{
					houseTileCoordinates,
					this._tilemapModel.IsTileReserved(houseTileCoordinates) ? "Reserved" : "Water or Mountain"
				});
				housePlaceable = false;
			}
			this._drivewayDirection = TileDirection.None;
			foreach (object obj in Enum.GetValues(typeof(TileDirection)))
			{
				TileDirection potentialDrivewayDirection = (TileDirection)obj;
				if (potentialDrivewayDirection != TileDirection.None)
				{
					Vector2Int potentialDrivewayCoordinates = TileUtilities.GetAdjacentCoordinates(houseTileCoordinates, potentialDrivewayDirection);
					if (this._city.IsTileInPlayableArea(potentialDrivewayCoordinates, this._clockModel.ExpansionTime))
					{
						Tile drivewayTile = this._tilemapView.GetTile(potentialDrivewayCoordinates);
						bool drivewayPlaceable = drivewayTile == null || (drivewayTile.ContentType == TileContentType.None && !drivewayTile.HasRailConnection);
						if (!this._city.Definition.TileIsBuildable(potentialDrivewayCoordinates) || this._city.Definition.TileIsOverWater(potentialDrivewayCoordinates) || this._city.Definition.TileIsUnderAMountain(potentialDrivewayCoordinates))
						{
							drivewayPlaceable = false;
						}
						Vector2Int adjacentTile = new Vector2Int(houseTileCoordinates.x, potentialDrivewayCoordinates.y);
						Vector2Int adjacentTile2 = new Vector2Int(potentialDrivewayCoordinates.x, houseTileCoordinates.y);
						if (adjacentTile.x != adjacentTile2.x && adjacentTile.y != adjacentTile2.y && this._city.Definition.TileIsOverRail(adjacentTile) && this._city.Definition.TileIsOverRail(adjacentTile2))
						{
							drivewayPlaceable = false;
						}
						if (drivewayPlaceable)
						{
							this._drivewayDirection = potentialDrivewayDirection;
							break;
						}
					}
				}
			}
			if (this._drivewayDirection == TileDirection.None)
			{
				DragHouseAction.Log.Warn("Failed to find a valid driveway direction from house coordinates {0}", new object[]
				{
					houseTileCoordinates
				});
				this._drivewayDirection = TileDirection.North;
				housePlaceable = false;
			}
			if (draftHouse == null)
			{
				draftHouse = this._scope.Get<DraftHouse>();
				draftHouse.Initialize(houseTileCoordinates, base.Scope, this._groupIndex, this._drivewayDirection);
				PlayerAction.Log.Info("Spawned draft house at {0}.", new object[]
				{
					houseTileCoordinates
				});
			}
			return housePlaceable;
		}

		// Token: 0x06003149 RID: 12617 RVA: 0x000E8D64 File Offset: 0x000E6F64
		public static DragHouseAction CreateFromUpgradeMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return DragHouseAction.Create(owningGroup, scope, timestamp, true);
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x000E8D6F File Offset: 0x000E6F6F
		public static DragHouseAction CreateFromEditMenu(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			return DragHouseAction.Create(owningGroup, scope, timestamp, false);
		}

		// Token: 0x0600314B RID: 12619 RVA: 0x000E8D7C File Offset: 0x000E6F7C
		public static DragHouseAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp, bool fromUpgradeMenu)
		{
			DragHouseAction newAction = scope.Get<DragHouseAction>();
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

		// Token: 0x04002A57 RID: 10839
		private new static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DragHouseAction");

		// Token: 0x04002A58 RID: 10840
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A59 RID: 10841
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002A5A RID: 10842
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04002A5B RID: 10843
		[Dependency]
		private IScope _scope;

		// Token: 0x04002A5C RID: 10844
		[Dependency]
		private TilemapModel _tilemapModel;

		// Token: 0x04002A5D RID: 10845
		private Vector2Int _lastCheckedCoordinates;

		// Token: 0x04002A5E RID: 10846
		private Vector2Int _lastPlacedCoordinates;

		// Token: 0x04002A5F RID: 10847
		protected DraftHouse draftHouse;

		// Token: 0x04002A60 RID: 10848
		private Vector2Int _previousDragCoordinates;

		// Token: 0x04002A61 RID: 10849
		private Vector2Int _previousHouseCoordinates;

		// Token: 0x04002A62 RID: 10850
		private int _groupIndex;

		// Token: 0x04002A63 RID: 10851
		private TileDirection _drivewayDirection;

		// Token: 0x04002A64 RID: 10852
		protected bool fromUpgradeMenu;
	}
}
