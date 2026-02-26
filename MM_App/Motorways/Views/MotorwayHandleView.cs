using System;
using Factory;
using Motorways.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Motorways.Views
{
	// Token: 0x020005D6 RID: 1494
	public class MotorwayHandleView : BaseMotorwayHandleView, IPointerDownHandler, IEventSystemHandler, ISubmitHandler
	{
		// Token: 0x060029B7 RID: 10679 RVA: 0x000B3440 File Offset: 0x000B1640
		public void Initialize(IScope scope, MotorwayView owningMotorway, int motorwayNumber)
		{
			base.Initialize(scope, motorwayNumber);
			this._owningMotorway = owningMotorway;
			this._scope = scope;
			if (Diagnostics.Verify(this._scope != null, "Scope invalid on MotorwayHandleView::Initialize"))
			{
				this._playerActionController = this._scope.Get<PlayerActionController>();
				this._clockModel = this._scope.Get<ClockModel>();
			}
		}

		// Token: 0x060029B8 RID: 10680 RVA: 0x000B349C File Offset: 0x000B169C
		public void OnPointerDown(PointerEventData eventData)
		{
			InputEvent inputEvent;
			if (eventData.pointerId < 0)
			{
				inputEvent = MotorwaysUIInputEvent.CreateMouseUIEvent(this._scope, (InputEventMouseButtonType)(-eventData.pointerId - 1), InputEventButtonState.JustDown, GameUIButtonType.MotorwayHandle, this._owningMotorway.Motorway.Id);
			}
			else
			{
				inputEvent = MotorwaysUIInputEvent.CreateTouchUIEvent(this._scope, eventData.pointerId, InputEventButtonState.JustDown, GameUIButtonType.MotorwayHandle, this._owningMotorway.Motorway.Id);
			}
			this._playerActionController.OnInputEvent(eventData.clickTime, inputEvent);
		}

		// Token: 0x060029B9 RID: 10681 RVA: 0x000B3514 File Offset: 0x000B1714
		public void OnSubmit(BaseEventData eventData)
		{
			ControllerInputEventData controllerInputEventData = eventData as ControllerInputEventData;
			if (controllerInputEventData != null)
			{
				IController onController = controllerInputEventData.instigatingController;
				InputEvent handleEvent = MotorwaysUIInputEvent.CreateGenericUIEvent(this._scope, 2, onController.GetInputSource(), InputEventButtonState.JustDown, GameUIButtonType.MotorwayHandle, this._owningMotorway.Motorway.Id);
				float inputTime = (float)this._clockModel.Time;
				this._playerActionController.OnInputEvent(inputTime, handleEvent);
			}
		}

		// Token: 0x060029BA RID: 10682 RVA: 0x000B3577 File Offset: 0x000B1777
		public void SetHandlePosition(Vector3 position)
		{
			base.transform.position = position;
			if (FeatureToggle.IsFeatureDisabled(Feature.BringMotorwaysToTopWhenEdited) && this._owningMotorway != null)
			{
				this._owningMotorway.Tilemap.ResortMotorwaysOnNextTick();
			}
		}

		// Token: 0x04002380 RID: 9088
		private MotorwayView _owningMotorway;

		// Token: 0x04002381 RID: 9089
		private IScope _scope;

		// Token: 0x04002382 RID: 9090
		private ClockModel _clockModel;

		// Token: 0x04002383 RID: 9091
		private PlayerActionController _playerActionController;
	}
}
