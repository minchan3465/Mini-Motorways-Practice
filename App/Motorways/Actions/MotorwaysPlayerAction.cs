using System;
using System.Collections.Generic;
using Factory;
using Motorways.Commands;
using Motorways.Models;
using Motorways.Views;
using Server;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x0200070D RID: 1805
	public abstract class MotorwaysPlayerAction : PlayerAction
	{
		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x0600319B RID: 12699 RVA: 0x0000222C File Offset: 0x0000042C
		protected virtual bool ManuallyHandlesReservations
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x0600319C RID: 12700 RVA: 0x0000222C File Offset: 0x0000042C
		public virtual bool PreventsCursorAcceleration
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x0600319D RID: 12701 RVA: 0x000EAFFD File Offset: 0x000E91FD
		// (set) Token: 0x0600319E RID: 12702 RVA: 0x000EB005 File Offset: 0x000E9205
		protected virtual MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource { get; set; }

		// Token: 0x0600319F RID: 12703 RVA: 0x000EB00E File Offset: 0x000E920E
		public override void InitializeAction(PlayerActionGroup owningGroup, float timestamp)
		{
			base.InitializeAction(owningGroup, timestamp);
			this._inputPointer = this._inputState.GetPointerFromInputEvent(owningGroup.InstigatingInputEvent);
			this._inputButton = this._inputState.GetButtonFromInputEvent(owningGroup.InstigatingInputEvent);
		}

		// Token: 0x060031A0 RID: 12704 RVA: 0x000EB046 File Offset: 0x000E9246
		public override void OnActionBegin(float timestamp)
		{
			this._gameUI.SetFocusPointActive(MotorwaysPlayerAction.DoesInputTypeUseFocusPoint(this._owningGroup.InstigatingInputEvent.Source), false);
			base.OnActionBegin(timestamp);
		}

		// Token: 0x060031A1 RID: 12705 RVA: 0x000EB070 File Offset: 0x000E9270
		public static bool DoesInputTypeUseFocusPoint(InputEventSource source)
		{
			return source == InputEventSource.Generic || source == InputEventSource.Remote;
		}

		// Token: 0x060031A2 RID: 12706 RVA: 0x000EB07C File Offset: 0x000E927C
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			foreach (ClientTileEdit unscheduledEdit in this._unscheduledClientTileEdits)
			{
				this.ReleaseClientTileEdit(unscheduledEdit);
			}
			this._unscheduledClientTileEdits.Clear();
			this.ClearDraftClientEdits();
			this._upgradeDatabase.OnDraftEditsScheduled();
			this.ClearTileReservations();
		}

		// Token: 0x060031A3 RID: 12707 RVA: 0x000EB0F8 File Offset: 0x000E92F8
		public override void OnActionComplete()
		{
			base.OnActionComplete();
			this.ApplyDraftClientEdits();
			foreach (ClientTileEdit clientTileEdit in this._unscheduledClientTileEdits)
			{
				this.ScheduleClientTileEdit(clientTileEdit);
			}
			this._unscheduledClientTileEdits.Clear();
			this.ClearTileReservations();
		}

		// Token: 0x060031A4 RID: 12708 RVA: 0x000EB168 File Offset: 0x000E9368
		protected virtual void SetCursorVisible(bool visible)
		{
			this._gameUI.SetRoadCursorActive(visible);
		}

		// Token: 0x060031A5 RID: 12709 RVA: 0x000EB176 File Offset: 0x000E9376
		protected virtual void SetWorldGridVisible(bool visible)
		{
			this._gameUI.SetWorldGridActive(visible, TransitionStyle.Tween);
		}

		// Token: 0x060031A6 RID: 12710 RVA: 0x000EB185 File Offset: 0x000E9385
		protected virtual void SetMotorwayGridVisible(bool visible)
		{
			this._gameUI.SetMotorwayGridActive(visible, TransitionStyle.Tween);
		}

		// Token: 0x060031A7 RID: 12711 RVA: 0x000EB194 File Offset: 0x000E9394
		protected virtual void UpdateCursorPosition()
		{
			this._gameUI.SetRoadCursorPosition(this.GetPointerScreenPosition());
		}

		// Token: 0x060031A8 RID: 12712 RVA: 0x000EB1A7 File Offset: 0x000E93A7
		protected virtual void SetColourWidgetRadialVisible(bool visible)
		{
			if (this._city.Rules.ShowColourWidget)
			{
				this._gameUI.ColourWidget.SetRadialColourWidgetVisible(visible);
			}
		}

		// Token: 0x060031A9 RID: 12713 RVA: 0x000EB1CC File Offset: 0x000E93CC
		protected ClientTileEdit AddTileEdit(TileEdit edit, MotorwaysPlayerAction.EditExecuteTiming executeTiming)
		{
			if (edit == null)
			{
				return null;
			}
			ClientTileEdit clientTileEdit = this._tilemapView.GenerateClientTileEditAndAddEditToViews(edit, executeTiming == MotorwaysPlayerAction.EditExecuteTiming.Draft);
			clientTileEdit.action = this;
			if (executeTiming == MotorwaysPlayerAction.EditExecuteTiming.Immediate)
			{
				this.ScheduleClientTileEdit(clientTileEdit);
			}
			else if (executeTiming == MotorwaysPlayerAction.EditExecuteTiming.OnComplete)
			{
				this.ReserveTiles(clientTileEdit.edit.GetAffectedTiles(this._tilemapView));
				this._unscheduledClientTileEdits.Add(clientTileEdit);
			}
			else if (executeTiming == MotorwaysPlayerAction.EditExecuteTiming.Draft)
			{
				this.ReserveTiles(clientTileEdit.edit.GetAffectedTiles(this._tilemapView));
				this._draftClientTileEdits.Add(clientTileEdit);
			}
			this._upgradeDatabase.AddTileEdit(clientTileEdit);
			return clientTileEdit;
		}

		// Token: 0x060031AA RID: 12714 RVA: 0x000EB268 File Offset: 0x000E9468
		protected void ClearDraftClientEdits()
		{
			if (this._draftClientTileEdits.Count == 0)
			{
				return;
			}
			foreach (ClientTileEdit clientEdit in this._draftClientTileEdits)
			{
				this.ReleaseClientTileEdit(clientEdit);
			}
			this._draftClientTileEdits.Clear();
			this.ClearTileReservations();
			foreach (ClientTileEdit unscheduledEdit in this._unscheduledClientTileEdits)
			{
				this.ReserveTiles(unscheduledEdit.edit.GetAffectedTiles(this._tilemapView));
			}
		}

		// Token: 0x060031AB RID: 12715 RVA: 0x000EB32C File Offset: 0x000E952C
		protected void ApplyDraftClientEdits()
		{
			foreach (ClientTileEdit clientEdit in this._draftClientTileEdits)
			{
				if (clientEdit.edit.CanApplyToSimulation)
				{
					clientEdit.isDraft = false;
					this._unscheduledClientTileEdits.Add(clientEdit);
				}
				else
				{
					base.Scope.Release(clientEdit.edit);
				}
			}
			this._draftClientTileEdits.Clear();
			this._upgradeDatabase.OnDraftEditsScheduled();
		}

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x060031AC RID: 12716 RVA: 0x000EB3C4 File Offset: 0x000E95C4
		protected bool HasSchedulableClientEdits
		{
			get
			{
				if (this._unscheduledClientTileEdits.Count > 0)
				{
					return true;
				}
				using (List<ClientTileEdit>.Enumerator enumerator = this._draftClientTileEdits.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.edit.CanApplyToSimulation)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x060031AD RID: 12717 RVA: 0x000EB434 File Offset: 0x000E9634
		private void ReserveTile(Tile tile)
		{
			Vector2Int coordinates = tile.Coordinates;
			if (!this._reservedTiles.Contains(coordinates))
			{
				this._reservedTiles.Add(coordinates);
				this._simulation.ScheduleCommand(ReserveTileCommand.Create(base.Scope, coordinates));
			}
		}

		// Token: 0x060031AE RID: 12718 RVA: 0x000EB47C File Offset: 0x000E967C
		private void ReserveTiles(IEnumerable<Tile> tiles)
		{
			foreach (Tile tile in tiles)
			{
				this.ReserveTile(tile);
			}
		}

		// Token: 0x060031AF RID: 12719 RVA: 0x000EB4C4 File Offset: 0x000E96C4
		private void ClearTileReservations()
		{
			if (this._reservedTiles.Count > 0 && !this.ManuallyHandlesReservations)
			{
				this._simulation.ScheduleCommand(ClearTileReservationsCommand.Create(base.Scope));
				this._reservedTiles.Clear();
			}
		}

		// Token: 0x060031B0 RID: 12720 RVA: 0x000EB500 File Offset: 0x000E9700
		private void ScheduleClientTileEdit(ClientTileEdit clientTileEdit)
		{
			EditTileCommand editCommand = EditTileCommand.Create(base.Scope, clientTileEdit.edit);
			this._simulation.ScheduleCommand(editCommand);
			clientTileEdit.isScheduledOnSimulation = true;
		}

		// Token: 0x060031B1 RID: 12721 RVA: 0x000EB534 File Offset: 0x000E9734
		private void ReleaseClientTileEdit(ClientTileEdit clientEdit)
		{
			if (!Diagnostics.Verify(clientEdit != null))
			{
				return;
			}
			TileEdit tileEdit = clientEdit.edit;
			if (!Diagnostics.Verify(tileEdit != null))
			{
				return;
			}
			foreach (Motorway affectedMotorway in tileEdit.GetAffectedMotorways(this._tilemapView))
			{
				if (Diagnostics.Verify(affectedMotorway != null))
				{
					MotorwayView motorwayView = this._tilemapView.GetMotorwayView(affectedMotorway.Id);
					if (Diagnostics.Verify(motorwayView != null))
					{
						motorwayView.RemoveEdit(clientEdit);
					}
				}
			}
			foreach (Tile affectedTile in tileEdit.GetAffectedTiles(this._tilemapView))
			{
				if (Diagnostics.Verify(affectedTile != null))
				{
					TileView tileView = this._tilemapView.GetTileView(affectedTile.Coordinates);
					if (Diagnostics.Verify(tileView != null))
					{
						tileView.RemoveEdit(clientEdit);
					}
				}
			}
			this._upgradeDatabase.RemoveTileEdit(clientEdit);
			base.Scope.Release(tileEdit);
		}

		// Token: 0x060031B2 RID: 12722 RVA: 0x000EB664 File Offset: 0x000E9864
		public Vector2Int GetPointerTilePosition()
		{
			return this._tilemapView.GetTileCoordinatesFromScreenPosition(this.GetPointerScreenPosition());
		}

		// Token: 0x060031B3 RID: 12723 RVA: 0x000EB677 File Offset: 0x000E9877
		protected Vector2 GetPointerWorldPosition()
		{
			return this._tilemapView.GetWorldPositionFromScreenPosition(this.GetPointerScreenPosition());
		}

		// Token: 0x060031B4 RID: 12724 RVA: 0x000EB68A File Offset: 0x000E988A
		protected Vector2 GetPointerScreenPosition()
		{
			if (this._playerPositionSource == MotorwaysPlayerAction.PlayerPositionSource.InputEvent && this._inputPointer != null)
			{
				return this._inputPointer.Position;
			}
			return this._gameUI.FocusPointPosition;
		}

		// Token: 0x060031B5 RID: 12725 RVA: 0x000EB6B4 File Offset: 0x000E98B4
		protected Vector2 GetPan()
		{
			Vector2 screenPos = this._tilemapView.GetScreenPositionFromTileCoordinates(this.GetPointerTilePosition());
			return new Vector2(Mathf.Clamp01(screenPos.x / (float)Screen.width), Mathf.Clamp01(screenPos.y / (float)Screen.height));
		}

		// Token: 0x060031B6 RID: 12726 RVA: 0x000EB6FC File Offset: 0x000E98FC
		protected virtual Vector2 GetMoveFocusJoystickInputValue()
		{
			return new Vector2(this._inputState.GetAxis(0), this._inputState.GetAxis(1));
		}

		// Token: 0x060031B7 RID: 12727 RVA: 0x000EB71B File Offset: 0x000E991B
		protected virtual Vector2 GetPanFocusJoystickInputValue()
		{
			return new Vector2(this._inputState.GetAxis(34), this._inputState.GetAxis(33));
		}

		// Token: 0x060031B8 RID: 12728 RVA: 0x000EB73C File Offset: 0x000E993C
		protected void BlockNewTouchUpgradeActions()
		{
			base.RegisterObserveInputEvent(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(InputEventFilter.AnySourceIndex, GameUIButtonType.Motorway, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			base.RegisterObserveInputEvent(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(InputEventFilter.AnySourceIndex, GameUIButtonType.TrafficLight, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			base.RegisterObserveInputEvent(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(InputEventFilter.AnySourceIndex, GameUIButtonType.Roundabout, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			base.RegisterObserveInputEvent(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(InputEventFilter.AnySourceIndex, GameUIButtonType.MotorwayHandle, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			base.RegisterObserveInputEvent(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(InputEventFilter.AnySourceIndex, GameUIButtonType.House, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			base.RegisterObserveInputEvent(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(InputEventFilter.AnySourceIndex, GameUIButtonType.Destination, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			base.RegisterObserveInputEvent(MotorwaysUIInputEventFilter.CreateTouchUIEventFilter(InputEventFilter.AnySourceIndex, GameUIButtonType.DoubleDestination, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
		}

		// Token: 0x04002A92 RID: 10898
		[Dependency]
		protected TileEditor _tileEditor;

		// Token: 0x04002A93 RID: 10899
		[Dependency]
		protected TilemapView _tilemapView;

		// Token: 0x04002A94 RID: 10900
		[Dependency]
		protected ClientUpgradeDatabase _upgradeDatabase;

		// Token: 0x04002A95 RID: 10901
		[Dependency]
		protected GameUIScreen _gameUI;

		// Token: 0x04002A96 RID: 10902
		[Dependency]
		protected ISimulation _simulation;

		// Token: 0x04002A97 RID: 10903
		[Dependency]
		protected City _city;

		// Token: 0x04002A98 RID: 10904
		[Dependency]
		protected ClockModel _clockModel;

		// Token: 0x04002A99 RID: 10905
		[Dependency]
		protected HapticFeedbackGenerator _feedbackGenerator;

		// Token: 0x04002A9A RID: 10906
		protected List<ClientTileEdit> _unscheduledClientTileEdits = new List<ClientTileEdit>();

		// Token: 0x04002A9B RID: 10907
		protected List<ClientTileEdit> _draftClientTileEdits = new List<ClientTileEdit>();

		// Token: 0x04002A9C RID: 10908
		protected HashSet<Vector2Int> _reservedTiles = new HashSet<Vector2Int>();

		// Token: 0x04002A9D RID: 10909
		protected IPointerState _inputPointer;

		// Token: 0x04002A9E RID: 10910
		protected ButtonState _inputButton;

		// Token: 0x0200070E RID: 1806
		public enum PlayerPositionSource
		{
			// Token: 0x04002AA1 RID: 10913
			InputEvent,
			// Token: 0x04002AA2 RID: 10914
			FocusPoint
		}

		// Token: 0x0200070F RID: 1807
		protected enum EditExecuteTiming
		{
			// Token: 0x04002AA4 RID: 10916
			Immediate,
			// Token: 0x04002AA5 RID: 10917
			OnComplete,
			// Token: 0x04002AA6 RID: 10918
			Draft,
			// Token: 0x04002AA7 RID: 10919
			Manual
		}
	}
}
