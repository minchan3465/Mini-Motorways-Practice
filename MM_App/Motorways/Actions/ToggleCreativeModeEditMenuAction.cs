using System;
using Client;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views;
using UnityEngine;

namespace Motorways.Actions
{
	// Token: 0x02000714 RID: 1812
	public class ToggleCreativeModeEditMenuAction : MotorwaysPlayerAction
	{
		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x060031CD RID: 12749 RVA: 0x000EBCA3 File Offset: 0x000E9EA3
		protected override MotorwaysPlayerAction.PlayerPositionSource _playerPositionSource
		{
			get
			{
				return this._source;
			}
		}

		// Token: 0x060031CE RID: 12750 RVA: 0x000EBCAC File Offset: 0x000E9EAC
		private bool PointerPositionWithinWindow(Vector2 pointerPosition)
		{
			return this._cameraView.GameCamera.UICamera.pixelRect.Contains(pointerPosition);
		}

		// Token: 0x060031CF RID: 12751 RVA: 0x000EBCD8 File Offset: 0x000E9ED8
		public override void OnActionBegin(float timestamp)
		{
			if (this._city.GameMode != GameMode.Creative)
			{
				this.OnActionCancel();
				return;
			}
			EditMenuPanel editMenuPanel = this._scope.Get<EditMenuPanel>();
			if (editMenuPanel.isActiveAndEnabled && (editMenuPanel.IsPlayingCloseEditMenuSequence || editMenuPanel.IsPlayingOpenEditMenuSequence))
			{
				this.OnActionCancel();
				return;
			}
			CarparkView carparkView = this._viewClient.GetCarparkWithEmptySpace(base.GetPointerWorldPosition());
			if (carparkView != null)
			{
				base.MakeExclusive();
				this.SpawnSecondDestination(carparkView.Model);
				this._gameUI.ConfirmEditMenuEdit();
				this.OnActionCancel();
				return;
			}
			this._inputDownOverCreativeModeObject = null;
			DestinationView destinationView = this.GetDestinationView();
			HouseView houseView = this.GetHouseView();
			if (destinationView != null)
			{
				this._inputDownOverCreativeModeObject = destinationView.GetComponent<ICreativeModeEditableObject>();
			}
			else if (houseView != null)
			{
				this._inputDownOverCreativeModeObject = houseView.GetComponent<ICreativeModeEditableObject>();
			}
			if (this._inputDownOverCreativeModeObject == null)
			{
				this.OnActionCancel();
			}
			if (editMenuPanel.isActiveAndEnabled)
			{
				if (editMenuPanel.IsPlayingOpenEditMenuSequence)
				{
					base.MakeExclusive();
					this.OnActionCancel();
					return;
				}
				ICreativeModeEditableObject editingObject = editMenuPanel.EditableObject;
				if (!editingObject.GetEditOptions().HasFlag(EditMenuButtonType.Move))
				{
					this.OnActionCancel();
					return;
				}
				if (editingObject == this._inputDownOverCreativeModeObject)
				{
					DragCreativeModeEditableObjectAction.Create(this._owningGroup, this._scope, Time.time);
					this.OnActionCancel();
					return;
				}
				if (editingObject is DraftDestination || editingObject is DraftHouse)
				{
					Vector2 pointerWorldPosition = base.GetPointerWorldPosition();
					if (editingObject.GetBounds().Contains(new Vector3(pointerWorldPosition.x, pointerWorldPosition.y, 0f)))
					{
						DragCreativeModeEditableObjectAction.Create(this._owningGroup, this._scope, Time.time);
						this.OnActionCancel();
					}
				}
			}
		}

		// Token: 0x060031D0 RID: 12752 RVA: 0x000EBE88 File Offset: 0x000EA088
		public override void Tick(float frameTime)
		{
			base.Tick(frameTime);
			Vector2 pointerWorldPosition = base.GetPointerWorldPosition();
			if (!this._inputDownOverCreativeModeObject.GetBounds().Contains(new Vector3(pointerWorldPosition.x, pointerWorldPosition.y, 0f)))
			{
				this.OnActionCancel();
			}
		}

		// Token: 0x060031D1 RID: 12753 RVA: 0x000EBED4 File Offset: 0x000EA0D4
		private void ShowEditMenu()
		{
			this._gameUI.OpenEditMenu(this._currentlyEditingCreativeModeObject, false);
		}

		// Token: 0x060031D2 RID: 12754 RVA: 0x000EBEE8 File Offset: 0x000EA0E8
		private void ConfirmEditMenuEdit()
		{
			ICreativeModeEditableObject editableObject = this._scope.Get<EditMenuPanel>().EditableObject;
			if (editableObject is CreativeModeEditableDestination || editableObject is CreativeModeEditableHouse)
			{
				this._gameUI.ConfirmEditMenuEdit();
			}
		}

		// Token: 0x060031D3 RID: 12755 RVA: 0x000EBF24 File Offset: 0x000EA124
		public override void ObserveInput(float timestamp, InputEvent inputEvent, bool overUI)
		{
			base.ObserveInput(timestamp, inputEvent, overUI);
			Vector2 pointerScreenPosition = base.GetPointerScreenPosition();
			if (this._cameraView.HasControlOverriden || !this._cameraView.CanChangeFocus || !this.PointerPositionWithinWindow(pointerScreenPosition))
			{
				this.OnActionComplete();
				return;
			}
			this._currentlyEditingCreativeModeObject = null;
			Vector2 pointerWorldPosition = base.GetPointerWorldPosition();
			if (this._inputDownOverCreativeModeObject.GetBounds().Contains(new Vector3(pointerWorldPosition.x, pointerWorldPosition.y, 0f)))
			{
				base.MakeExclusive();
				this._currentlyEditingCreativeModeObject = this._inputDownOverCreativeModeObject;
				if (this._gameUI.editMenuPanel.gameObject.activeInHierarchy)
				{
					if (this._gameUI.editMenuPanel.EditableObject != this._currentlyEditingCreativeModeObject)
					{
						this.ConfirmEditMenuEdit();
						this.ShowEditMenu();
					}
				}
				else
				{
					this.ShowEditMenu();
				}
			}
			else
			{
				this.ConfirmEditMenuEdit();
			}
			this.OnActionComplete();
		}

		// Token: 0x060031D4 RID: 12756 RVA: 0x000EC008 File Offset: 0x000EA208
		private DestinationView GetDestinationView()
		{
			Vector2 pointerWorldPosition = base.GetPointerWorldPosition();
			foreach (DestinationView destinationView in this._viewClient.GetViews<DestinationView>())
			{
				if (destinationView.Model != null && destinationView.GetBounds().Contains(new Vector3(pointerWorldPosition.x, pointerWorldPosition.y, destinationView.transform.position.z)))
				{
					return destinationView;
				}
			}
			return null;
		}

		// Token: 0x060031D5 RID: 12757 RVA: 0x000EC0A4 File Offset: 0x000EA2A4
		private HouseView GetHouseView()
		{
			Vector2 pointerWorldPosition = base.GetPointerWorldPosition();
			foreach (HouseView houseView in this._viewClient.GetViews<HouseView>())
			{
				if (houseView.Model != null && houseView.GetBounds().Contains(new Vector3(pointerWorldPosition.x, pointerWorldPosition.y, houseView.transform.position.z)))
				{
					return houseView;
				}
			}
			return null;
		}

		// Token: 0x060031D6 RID: 12758 RVA: 0x000EC140 File Offset: 0x000EA340
		private void SpawnSecondDestination(CarparkModel carparkModel)
		{
			CityPlanModel cityPlanModel = this._scope.Get<CityPlanModel>();
			CityPlanModel.ScheduledBuilding secondDestination = this._scope.Get<CityPlanModel.ScheduledBuilding>();
			bool isStation = carparkModel.ActiveDestinationCount > 0 && carparkModel.destinations[0].IsTrainStation;
			bool isBoatTerminal = carparkModel.ActiveDestinationCount > 0 && carparkModel.destinations[0].IsBoatTerminal;
			secondDestination.type = CityTileType.Demand;
			secondDestination.carparkPreference = (isBoatTerminal ? CarparkPreference.JoinBoatTerminal : (isStation ? CarparkPreference.Station : CarparkPreference.Double));
			secondDestination.useFixedParameters = true;
			secondDestination.positionOverride = carparkModel.TopLeftWorldCoordinate;
			secondDestination.time = Fix64.Zero;
			secondDestination.demandMultiplier = Fix64.One;
			secondDestination.groupIndex = this._scope.Get<ColourWidget>().CurrentColour;
			secondDestination.initialUpgradeLevel = 0;
			cityPlanModel.ScheduleBuilding(secondDestination);
		}

		// Token: 0x060031D7 RID: 12759 RVA: 0x000EC208 File Offset: 0x000EA408
		public static ToggleCreativeModeEditMenuAction Create(PlayerActionGroup owningGroup, IScope scope, float timestamp)
		{
			ToggleCreativeModeEditMenuAction toggleCreativeModeEditMenuAction = scope.Get<ToggleCreativeModeEditMenuAction>();
			toggleCreativeModeEditMenuAction.InitializeAction(owningGroup, timestamp);
			toggleCreativeModeEditMenuAction._source = ((owningGroup.InstigatingInputEvent.Source == InputEventSource.Any) ? MotorwaysPlayerAction.PlayerPositionSource.FocusPoint : MotorwaysPlayerAction.PlayerPositionSource.InputEvent);
			toggleCreativeModeEditMenuAction.RegisterObserveInputEvent(InputEventFilter.CreateEventFilter(owningGroup.InstigatingInputEvent.Source, owningGroup.InstigatingInputEvent.InputAction, InputEventButtonState.JustUp), PlayerAction.ObserverGreediness.BlocksNewActions);
			toggleCreativeModeEditMenuAction.OnActionBegin(timestamp);
			return toggleCreativeModeEditMenuAction;
		}

		// Token: 0x060031D8 RID: 12760 RVA: 0x000EC265 File Offset: 0x000EA465
		public override void OnActionCancel()
		{
			this._source = MotorwaysPlayerAction.PlayerPositionSource.InputEvent;
			this._currentlyEditingCreativeModeObject = null;
			base.OnActionCancel();
		}

		// Token: 0x060031D9 RID: 12761 RVA: 0x000EC27B File Offset: 0x000EA47B
		public override void OnActionComplete()
		{
			this._source = MotorwaysPlayerAction.PlayerPositionSource.InputEvent;
			this.SetColourWidgetRadialVisible(false);
			base.OnActionComplete();
		}

		// Token: 0x060031DA RID: 12762 RVA: 0x000EC291 File Offset: 0x000EA491
		public override void Reset()
		{
			base.Reset();
			this._currentlyEditingCreativeModeObject = null;
		}

		// Token: 0x04002AB4 RID: 10932
		[Dependency]
		private IAudioSystem _audioSystem;

		// Token: 0x04002AB5 RID: 10933
		[Dependency]
		private CameraView _cameraView;

		// Token: 0x04002AB6 RID: 10934
		[Dependency]
		private ViewClient _viewClient;

		// Token: 0x04002AB7 RID: 10935
		[Dependency]
		private IScope _scope;

		// Token: 0x04002AB8 RID: 10936
		private MotorwaysPlayerAction.PlayerPositionSource _source;

		// Token: 0x04002AB9 RID: 10937
		private ICreativeModeEditableObject _currentlyEditingCreativeModeObject;

		// Token: 0x04002ABA RID: 10938
		private ICreativeModeEditableObject _inputDownOverCreativeModeObject;
	}
}
