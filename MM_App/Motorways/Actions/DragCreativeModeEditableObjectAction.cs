using System;
using Factory;
using Motorways.Audio;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000701 RID: 1793
	public class DragCreativeModeEditableObjectAction : MotorwaysPlayerAction
	{
		// Token: 0x0600311B RID: 12571 RVA: 0x000E70C1 File Offset: 0x000E52C1
		public override void Reset()
		{
			this._editedMotorwayId = -1;
			this._hasReplacedMothballEdit = false;
			this._didShowGrid = false;
			this._previousTilePosition = default(Vector2Int);
			this._draftHouse = null;
			this._draftDestination = null;
			this._editMenuPanel = null;
			base.Reset();
		}

		// Token: 0x0600311C RID: 12572 RVA: 0x000E7100 File Offset: 0x000E5300
		public override void OnActionBegin(float timestamp)
		{
			base.OnActionBegin(timestamp);
			this._didShowGrid = false;
			if (this._inputState.TouchCount > 1)
			{
				this.OnActionCancel();
				return;
			}
			this._editMenuPanel = this._scope.Get<EditMenuPanel>();
			ICreativeModeEditableObject editableObject = this._editMenuPanel.EditableObject;
			if (editableObject != null)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateEvent(-1.0, AudioEventType.UpgradeDragged, 0.5f, -1f, true, null));
				this._editMenuPanel.ShowHideEditMenu(false);
				if (editableObject is CreativeModeEditableHouse || editableObject is DraftHouse)
				{
					if (InputState.DeviceInputTypeRequiresFocus(this._inputState.CurrentDeviceInputType))
					{
						ControllerDragHouseAction.CreateFromEditMenu(this._owningGroup, this._scope, timestamp);
					}
					else
					{
						DragHouseAction.CreateFromEditMenu(this._owningGroup, this._scope, timestamp);
					}
					this.OnActionComplete();
					return;
				}
				if (editableObject is CreativeModeEditableDestination)
				{
					if ((editableObject as CreativeModeEditableDestination).IsDouble)
					{
						if (InputState.DeviceInputTypeRequiresFocus(this._inputState.CurrentDeviceInputType))
						{
							ControllerDragDestinationAction.CreateDoubleFromEditMenu(this._owningGroup, this._scope, timestamp);
						}
						else
						{
							DragDestinationAction.CreateDoubleFromEditMenu(this._owningGroup, this._scope, timestamp);
						}
					}
					else if (InputState.DeviceInputTypeRequiresFocus(this._inputState.CurrentDeviceInputType))
					{
						ControllerDragDestinationAction.CreateSingleFromEditMenu(this._owningGroup, this._scope, timestamp);
					}
					else
					{
						DragDestinationAction.CreateSingleFromEditMenu(this._owningGroup, this._scope, timestamp);
					}
					this.OnActionComplete();
					return;
				}
				if (editableObject is DraftDestination)
				{
					if (!(editableObject as DraftDestination).IsDouble)
					{
						if (InputState.DeviceInputTypeRequiresFocus(this._inputState.CurrentDeviceInputType))
						{
							ControllerDragDestinationAction.CreateSingleFromEditMenu(this._owningGroup, this._scope, timestamp);
						}
						else
						{
							DragDestinationAction.CreateSingleFromEditMenu(this._owningGroup, this._scope, timestamp);
						}
					}
					else if (InputState.DeviceInputTypeRequiresFocus(this._inputState.CurrentDeviceInputType))
					{
						ControllerDragDestinationAction.CreateDoubleFromEditMenu(this._owningGroup, this._scope, timestamp);
					}
					else
					{
						DragDestinationAction.CreateDoubleFromEditMenu(this._owningGroup, this._scope, timestamp);
					}
					this.OnActionComplete();
				}
			}
		}

		// Token: 0x0600311D RID: 12573 RVA: 0x000020A2 File Offset: 0x000002A2
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			this.OnActionComplete();
		}

		// Token: 0x0600311E RID: 12574 RVA: 0x000E7304 File Offset: 0x000E5504
		public override void OnActionComplete()
		{
			if (!this._cameraView.IsFocussedIn)
			{
				this.SetGridVisible(false);
			}
			GameUIScreen gameUIScreen = base.Scope.Get<GameUIScreen>();
			if (this._draftHouse)
			{
				gameUIScreen.OpenEditMenu(this._draftHouse, false);
			}
			else if (this._draftDestination)
			{
				gameUIScreen.OpenEditMenu(this._draftDestination, false);
			}
			base.OnActionComplete();
		}

		// Token: 0x0600311F RID: 12575 RVA: 0x000E736D File Offset: 0x000E556D
		public override void OnActionCancel()
		{
			base.OnActionCancel();
			base.ClearDraftClientEdits();
			this.SetGridVisible(false);
		}

		// Token: 0x06003120 RID: 12576 RVA: 0x000E7384 File Offset: 0x000E5584
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
		}

		// Token: 0x06003121 RID: 12577 RVA: 0x000E73D4 File Offset: 0x000E55D4
		public static DragCreativeModeEditableObjectAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			DragCreativeModeEditableObjectAction newAction = scope.Get<DragCreativeModeEditableObjectAction>();
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

		// Token: 0x04002A29 RID: 10793
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002A2A RID: 10794
		[Dependency]
		private NotificationView _notificationView;

		// Token: 0x04002A2B RID: 10795
		[Dependency]
		private IScope _scope;

		// Token: 0x04002A2C RID: 10796
		[Dependency]
		private TilemapModel _tilemapModel;

		// Token: 0x04002A2D RID: 10797
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002A2E RID: 10798
		private int _editedMotorwayId = -1;

		// Token: 0x04002A2F RID: 10799
		private bool _hasReplacedMothballEdit;

		// Token: 0x04002A30 RID: 10800
		private bool _didShowGrid;

		// Token: 0x04002A31 RID: 10801
		private Vector2Int _previousTilePosition;

		// Token: 0x04002A32 RID: 10802
		private DraftHouse _draftHouse;

		// Token: 0x04002A33 RID: 10803
		private DraftDestination _draftDestination;

		// Token: 0x04002A34 RID: 10804
		private EditMenuPanel _editMenuPanel;
	}
}
