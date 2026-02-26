using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Audio;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006FB RID: 1787
	public class ControllerDrawRoadAction : MotorwaysPlayerAction
	{
		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x060030ED RID: 12525 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030EE RID: 12526 RVA: 0x000E5BEC File Offset: 0x000E3DEC
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			if (this._controllerState.ControllerState != MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles)
			{
				ControllerDrawRoadAction.Log.Info("Not in the correct control state: (wanted {0}, currently {1})", new object[]
				{
					MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles,
					this._controllerState.ControllerState
				});
				this.OnActionCancel();
				return;
			}
			if (this._gameUI.CurrentRoadDrawMode != RoadDrawMode.Add)
			{
				this.SetState(ControllerDrawRoadAction.TapDrawState.Cancelling);
				return;
			}
			if (!this._city.IsTileInPlayableArea(base.GetPointerTilePosition(), this._clockModel.ExpansionTime))
			{
				this.SetState(ControllerDrawRoadAction.TapDrawState.Cancelling);
				return;
			}
			this._cursorTileCoordinates = base.GetPointerTilePosition();
			if (base.Scope.Get<City>().Rules is TutorialGameRules)
			{
				TutorialProgressionProcess tutorialProgressionProcess = base.Scope.Get<TutorialProgressionProcess>();
				tutorialProgressionProcess.SetControllerIsDrawingRoads(true);
				tutorialProgressionProcess.SetCurrentControllerCursor(this._cursorTileCoordinates);
			}
			this._roadStartTileCoordinates = this._cursorTileCoordinates;
			this._latestValidRoadEndTileCoordinates = this._cursorTileCoordinates;
			this._roadEndTileCoordinates = this._cursorTileCoordinates;
			this._editCount = 0;
			this.UpdateCursorPosition();
			this.SetCursorVisible(true);
			this.SetState(ControllerDrawRoadAction.TapDrawState.Initializing);
			ControllerDrawRoadAction.Log.Info("Beginning TapDrawRoadAction from tile coordinates {0}.", new object[]
			{
				this._roadStartTileCoordinates
			});
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x000E5D24 File Offset: 0x000E3F24
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			this.UpdateCursorPosition();
			Vector2Int testTileCoordinates = base.GetPointerTilePosition();
			if (this._roadEndTileCoordinates != testTileCoordinates)
			{
				this._roadEndTileCoordinates = testTileCoordinates;
				Tile destinationTileModel = this._tilemapView.GetTile(this._roadEndTileCoordinates);
				if (destinationTileModel != null)
				{
					RoadTileSignature signature = destinationTileModel.CreateSignature(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed);
					if (new List<RoadTileConnection>(signature.Connections).Count >= 1)
					{
						this._completeOnTraversalEnd = true;
						ControllerDrawRoadAction.Log.Info("Targetted another road tile, drawing to it and stopping.", Array.Empty<object>());
					}
					else
					{
						this._completeOnTraversalEnd = false;
					}
					base.Scope.Release(signature);
				}
				Vector2Int destination = this._latestValidRoadEndTileCoordinates;
				Tile tile = this._simulation.GetModel<TilemapModel>().GetTile(this._roadEndTileCoordinates);
				if ((tile == null || tile.CanDrawRoadsOn() || tile.ContentType == TileContentType.House) && this._city.IsTileInPlayableArea(this._roadEndTileCoordinates, this._clockModel.ExpansionTime) && this._city.Definition.TileIsBuildable(this._roadEndTileCoordinates))
				{
					destination = this._roadEndTileCoordinates;
				}
				IEnumerable<Vector2Int> newPath = this._pathfinder.GetPathBetweenPoints(this._roadStartTileCoordinates, destination, this._simulation, this._city, null);
				if (newPath != null)
				{
					this._path.Clear();
					this._path.AddRange(newPath);
				}
				base.ClearDraftClientEdits();
				bool builtToCursor = true;
				Vector2Int lastPathedCoordinate = this._path[this._path.Count - 1];
				for (int pathIndex = 1; pathIndex < this._path.Count; pathIndex++)
				{
					if (!this.DraftRoadBetweenTiles(this._path[pathIndex - 1], this._path[pathIndex]))
					{
						builtToCursor = false;
						break;
					}
					lastPathedCoordinate = this._path[pathIndex];
				}
				builtToCursor &= (lastPathedCoordinate == this._roadEndTileCoordinates);
				this._latestValidRoadEndTileCoordinates = lastPathedCoordinate;
				if (builtToCursor)
				{
					this._notificationView.HideNotification();
				}
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.BuildRoad, base.GetPan().x, -1f, builtToCursor, null));
			}
		}

		// Token: 0x060030F0 RID: 12528 RVA: 0x000E5F40 File Offset: 0x000E4140
		protected bool DraftRoadBetweenTiles(Vector2Int currentPosition, Vector2Int nextTilePosition)
		{
			TileDirection direction = TileUtilities.GetDirectionBetweenAdjacentCoordinates(currentPosition, nextTilePosition);
			Fix64 currentTime = this._clockModel.ExpansionTime;
			if (!this._city.IsTileInPlayableArea(currentPosition, currentTime) || !this._city.IsTileInPlayableArea(nextTilePosition, currentTime))
			{
				return true;
			}
			TileEditResult addRoadResult = this._tileEditor.AddRoad(this._tilemapView, currentPosition, direction);
			if (addRoadResult.IsSuccessful)
			{
				base.AddTileEdit(addRoadResult.edit, MotorwaysPlayerAction.EditExecuteTiming.Draft);
				return true;
			}
			this._notificationView.AddNotification(addRoadResult.resultCode, addRoadResult.errorPosition);
			return false;
		}

		// Token: 0x060030F1 RID: 12529 RVA: 0x000E5FC7 File Offset: 0x000E41C7
		private void SetActionFocusPoint(Vector2Int newFocusCoordinates)
		{
			this._roadStartTileCoordinates = newFocusCoordinates;
			this._roadEndTileCoordinates = newFocusCoordinates;
			this.UpdateCursorPosition();
		}

		// Token: 0x060030F2 RID: 12530 RVA: 0x000E5FE0 File Offset: 0x000E41E0
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (inputEvent.InputAction == 2 || inputEvent.InputAction == 17)
			{
				base.ApplyDraftClientEdits();
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, UIAudioProfile.Generic, -1f, true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
				bool startIsSameAsEnd = this._roadEndTileCoordinates == this._roadStartTileCoordinates;
				this._roadStartTileCoordinates = this._roadEndTileCoordinates;
				this._cursorTileCoordinates = this._roadEndTileCoordinates;
				if (base.Scope.Get<City>().Rules is TutorialGameRules)
				{
					base.Scope.Get<TutorialProgressionProcess>().SetCurrentControllerCursor(this._cursorTileCoordinates);
				}
				if (this._completeOnTraversalEnd || overUI || (this._editCount > 0 && this._upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Concrete) <= 0) || this._upgradeDatabase.GetAvailableUpgradeCount(UpgradeType.Concrete) == 0 || startIsSameAsEnd)
				{
					if (overUI)
					{
						ControllerDrawRoadAction.Log.Info("Complete Action - Input over the UI", Array.Empty<object>());
					}
					this.SetState(ControllerDrawRoadAction.TapDrawState.Completing);
				}
				else
				{
					this.SetState(ControllerDrawRoadAction.TapDrawState.Ready);
					this.SetActionFocusPoint(this._cursorTileCoordinates);
				}
				this._editCount++;
				return;
			}
			if (inputEvent.InputAction == 7)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.MothballRoad, 0.5f, -1f, true, null));
				this.OnActionComplete();
			}
		}

		// Token: 0x060030F3 RID: 12531 RVA: 0x000E612C File Offset: 0x000E432C
		public override void OnActionComplete()
		{
			base.ClearDraftClientEdits();
			this.SetCursorVisible(false);
			if (base.Scope.Get<City>().Rules is TutorialGameRules)
			{
				base.Scope.Get<TutorialProgressionProcess>().SetControllerIsDrawingRoads(false);
			}
			this._notificationView.HideAlertIcon();
			this._notificationView.CancelNotification();
			base.OnActionComplete();
		}

		// Token: 0x060030F4 RID: 12532 RVA: 0x000E618C File Offset: 0x000E438C
		public override void OnActionCancel()
		{
			base.ClearDraftClientEdits();
			this.SetCursorVisible(false);
			if (base.Scope.Get<City>().Rules is TutorialGameRules)
			{
				base.Scope.Get<TutorialProgressionProcess>().SetControllerIsDrawingRoads(false);
			}
			this._notificationView.HideAlertIcon();
			this._notificationView.CancelNotification();
			base.OnActionCancel();
		}

		// Token: 0x060030F5 RID: 12533 RVA: 0x000E61EA File Offset: 0x000E43EA
		private void SetState(ControllerDrawRoadAction.TapDrawState newState)
		{
			this._tapState = newState;
			if (newState == ControllerDrawRoadAction.TapDrawState.Completing)
			{
				this.OnActionComplete();
				return;
			}
			if (newState != ControllerDrawRoadAction.TapDrawState.Cancelling)
			{
				return;
			}
			this.OnActionCancel();
		}

		// Token: 0x060030F6 RID: 12534 RVA: 0x000E6209 File Offset: 0x000E4409
		protected override void SetCursorVisible(bool visible)
		{
			this._gameUI.SetRoadCursorActive(visible);
			this.SetWorldGridVisible(visible);
			this._tilemapView.viewMode = (visible ? TilemapView.ViewMode.Edit : TilemapView.ViewMode.Normal);
		}

		// Token: 0x060030F7 RID: 12535 RVA: 0x000E6230 File Offset: 0x000E4430
		protected override void UpdateCursorPosition()
		{
			this._gameUI.SetRoadCursorPosition(this._gameUI.FocusPointPosition);
		}

		// Token: 0x060030F8 RID: 12536 RVA: 0x000E6248 File Offset: 0x000E4448
		public override void Reset()
		{
			base.Reset();
			this._tapState = ControllerDrawRoadAction.TapDrawState.Initializing;
			this._roadStartTileCoordinates = Vector2Int.zero;
			this._roadEndTileCoordinates = Vector2Int.zero;
			this._latestValidRoadEndTileCoordinates = Vector2Int.zero;
			this._cursorTileCoordinates = Vector2Int.zero;
			this._path.Clear();
			this._completeOnTraversalEnd = false;
			this._editCount = 0;
		}

		// Token: 0x060030F9 RID: 12537 RVA: 0x000E62A8 File Offset: 0x000E44A8
		public static MotorwaysPlayerAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ControllerDrawRoadAction newAction = scope.Get<ControllerDrawRoadAction>();
			TilemapView tilemapView = scope.Get<TilemapView>();
			Tile tile = tilemapView.GetTile(newAction.GetPointerTilePosition());
			if (tile != null)
			{
				foreach (TileDirection motorwayDirection in tile.GetMotorwayRamps(RoadState.Planned | RoadState.Active))
				{
					if (!tilemapView.GetMotorway(tile.GetMotorwayInDirection(motorwayDirection, RoadState.Planned | RoadState.Active)).IsPermanent)
					{
						return ControllerDragEditMotorwayAction.Create(owningGroup, scope, timestamp);
					}
				}
			}
			newAction.InitializeAction(owningGroup, timestamp);
			InputEventSource instigatingEventSource = owningGroup.InstigatingInputEvent.Source;
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(instigatingEventSource, 17, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(instigatingEventSource, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(instigatingEventSource, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(instigatingEventSource, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			PlayerAction.ObserverGreediness toggleDrawModeGreediness = (owningGroup.InstigatingInputEvent.Source == InputEventSource.Remote) ? PlayerAction.ObserverGreediness.BlocksNewActions : PlayerAction.ObserverGreediness.AllowsNewActions;
			newAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(instigatingEventSource, 9, InputEventButtonState.JustDown), toggleDrawModeGreediness);
			ControllerDrawRoadAction.Log.Info("Creating action.", Array.Empty<object>());
			newAction.OnActionBegin(timestamp);
			return newAction;
		}

		// Token: 0x040029FD RID: 10749
		public new static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ControllerDrawRoadAction");

		// Token: 0x040029FE RID: 10750
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x040029FF RID: 10751
		private ControllerDrawRoadAction.TapDrawState _tapState;

		// Token: 0x04002A00 RID: 10752
		private Vector2Int _roadStartTileCoordinates;

		// Token: 0x04002A01 RID: 10753
		private Vector2Int _roadEndTileCoordinates;

		// Token: 0x04002A02 RID: 10754
		private Vector2Int _latestValidRoadEndTileCoordinates;

		// Token: 0x04002A03 RID: 10755
		private Vector2Int _cursorTileCoordinates;

		// Token: 0x04002A04 RID: 10756
		private readonly List<Vector2Int> _path = new List<Vector2Int>();

		// Token: 0x04002A05 RID: 10757
		private bool _completeOnTraversalEnd;

		// Token: 0x04002A06 RID: 10758
		private int _editCount;

		// Token: 0x04002A07 RID: 10759
		[Dependency]
		protected MotorwaysInGameStateToggleController _controllerState;

		// Token: 0x04002A08 RID: 10760
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x04002A09 RID: 10761
		[Dependency]
		private TilePathfinder _pathfinder;

		// Token: 0x020006FC RID: 1788
		public enum TapDrawState
		{
			// Token: 0x04002A0B RID: 10763
			Initializing,
			// Token: 0x04002A0C RID: 10764
			Ready,
			// Token: 0x04002A0D RID: 10765
			DraftingRoad,
			// Token: 0x04002A0E RID: 10766
			AddingRoad,
			// Token: 0x04002A0F RID: 10767
			Realigning,
			// Token: 0x04002A10 RID: 10768
			Completing,
			// Token: 0x04002A11 RID: 10769
			Cancelling
		}
	}
}
