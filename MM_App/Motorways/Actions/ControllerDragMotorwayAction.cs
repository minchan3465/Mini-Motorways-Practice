using System;
using Factory;
using Motorways.Audio;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x020006F7 RID: 1783
	public class ControllerDragMotorwayAction : DragMotorwayAction
	{
		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x060030D2 RID: 12498 RVA: 0x000020AA File Offset: 0x000002AA
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return MotorwaysPlayerAction.PlayerPositionSource.FocusPoint;
			}
		}

		// Token: 0x060030D3 RID: 12499 RVA: 0x000E5784 File Offset: 0x000E3984
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this._confirmedStartTile = false;
			this._hasScheduledOverEvent = false;
		}

		// Token: 0x060030D4 RID: 12500 RVA: 0x000E579C File Offset: 0x000E399C
		public override void Tick(float frameTime)
		{
			if (!this._confirmedStartTile)
			{
				base.Tick(frameTime);
				return;
			}
			Vector2Int nextTileCoordinates = base.GetPointerTilePosition();
			if (nextTileCoordinates != this._danglingCoordinates)
			{
				AddMotorwayAction.MotorwayActionResult danglingErrorCode = base.SetDanglingTile(nextTileCoordinates);
				if (danglingErrorCode == AddMotorwayAction.MotorwayActionResult.Success)
				{
					base.UpdateTileEdit();
					return;
				}
				base.DisplayError(danglingErrorCode, false);
			}
		}

		// Token: 0x060030D5 RID: 12501 RVA: 0x000E57EC File Offset: 0x000E39EC
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			if (base.ActionState == PlayerAction.State.Begun && this._controllerState.ControllerState == MotorwaysInGameStateToggleController.InGameControllerState.EditingTiles)
			{
				if (inputEvent.InputAction == 2)
				{
					if (this._confirmedStartTile)
					{
						this.OnActionComplete();
						return;
					}
					if (this._editResult.IsSuccessful)
					{
						base.ApplyDraftClientEdits();
						this._confirmedStartTile = true;
						return;
					}
				}
				else if (!this._confirmedStartTile)
				{
					this.OnActionCancel();
				}
			}
		}

		// Token: 0x060030D6 RID: 12502 RVA: 0x000E5854 File Offset: 0x000E3A54
		protected override TileEditResult CreateTileEdit(int newMotorwayId, int motorwayNumber, Vector2Int anchorCoordinates, TileDirection anchorDirection, Vector2Int danglingCoordinates, TileDirection danglingDirection)
		{
			if (this._confirmedStartTile)
			{
				TileEditResult newResult = this._tileEditor.AddMotorway(this._tilemapView, newMotorwayId, motorwayNumber, anchorCoordinates, anchorDirection, danglingCoordinates, danglingDirection, -1);
				if (newResult.IsSuccessful)
				{
					this._editResult = newResult;
					if (this._editResult.edit != null)
					{
						this._notificationView.HideNotification();
						this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOver, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
						this._hasScheduledOverEvent = true;
						return this._editResult;
					}
				}
				else
				{
					if (newResult.edit != null)
					{
						this._scope.Release(newResult.edit);
					}
					this._notificationView.AddNotification(newResult.resultCode, newResult.errorPosition);
				}
				if (this._hasScheduledOverEvent)
				{
					this._audioSystem.ScheduleEvent(AudioEvent.CreateUpgradeEvent(AudioEventType.UpgradeOut, UpgradeType.Motorway, true, base.MotorwayBeingEdited, default(Vector2)));
					this._hasScheduledOverEvent = false;
				}
				return newResult;
			}
			return new TileEditResult
			{
				resultCode = TileEditResultCode.NotInitialized
			};
		}

		// Token: 0x060030D7 RID: 12503 RVA: 0x000E5960 File Offset: 0x000E3B60
		public override void Reset()
		{
			base.Reset();
			this._confirmedStartTile = false;
			this._hasScheduledOverEvent = false;
		}

		// Token: 0x060030D8 RID: 12504 RVA: 0x000E5978 File Offset: 0x000E3B78
		public new static ControllerDragMotorwayAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ControllerDragMotorwayAction controllerDragMotorwayAction = scope.Get<ControllerDragMotorwayAction>();
			controllerDragMotorwayAction.InitializeAction(owningGroup, timestamp);
			controllerDragMotorwayAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 2, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragMotorwayAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 7, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragMotorwayAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(InputEventSource.Any, 18, InputEventButtonState.JustDown), PlayerAction.ObserverGreediness.BlocksNewActions);
			controllerDragMotorwayAction.OnActionBegin(timestamp);
			return controllerDragMotorwayAction;
		}

		// Token: 0x040029F7 RID: 10743
		private bool _confirmedStartTile;

		// Token: 0x040029F8 RID: 10744
		private bool _hasScheduledOverEvent;

		// Token: 0x040029F9 RID: 10745
		[Dependency]
		protected MotorwaysInGameStateToggleController _controllerState;

		// Token: 0x040029FA RID: 10746
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x040029FB RID: 10747
		[Dependency]
		private IScope _scope;
	}
}
