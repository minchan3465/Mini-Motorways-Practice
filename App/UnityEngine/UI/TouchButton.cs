using System;
using System.Collections;
using Motorways.Audio;
using Motorways.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	// Token: 0x020002CC RID: 716
	[AddComponentMenu("UI/Touch Button", 30)]
	public class TouchButton : VariableDeviceSelectable
	{
		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06001191 RID: 4497 RVA: 0x000020AA File Offset: 0x000002AA
		protected virtual bool OverrideSelectedState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x0003A812 File Offset: 0x00038A12
		protected TouchButton()
		{
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06001193 RID: 4499 RVA: 0x0003A83B File Offset: 0x00038A3B
		// (set) Token: 0x06001194 RID: 4500 RVA: 0x0003A843 File Offset: 0x00038A43
		protected TouchButton.ButtonClickedEvent onClick
		{
			get
			{
				return this._onClick;
			}
			set
			{
				this._onClick = value;
			}
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x0003A84C File Offset: 0x00038A4C
		public void Press()
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this._wasPressed = true;
			UISystemProfilerApi.AddMarker("TouchButton.onClick", this);
			this._onClick.Invoke();
			if (this._feedbackGenerator != null)
			{
				this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.Selection);
			}
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x0003A89B File Offset: 0x00038A9B
		public void AddOnClickedEvent(UnityAction newEvent)
		{
			this._onClick.AddListener(newEvent);
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x0003A8A9 File Offset: 0x00038AA9
		public void AddOnSelectedEvent(UnityAction newEvent)
		{
			this._onSelected.AddListener(newEvent);
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x0003A8B7 File Offset: 0x00038AB7
		public void AddOnDeselectedEvent(UnityAction newEvent)
		{
			this._onDeselected.AddListener(newEvent);
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x0003A8C8 File Offset: 0x00038AC8
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this._pointerOverButton = true;
			if (base.DeviceInputType == DeviceInputType.Touch)
			{
				this.DoStateTransition(Selectable.SelectionState.Selected, false);
			}
			if (this.doSelectedOnHighlight)
			{
				this._onSelected.Invoke();
			}
			if (this._audioProfile != UIAudioProfile.None && this._audioSystem != null)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.MouseOver, this._audioProfile, -1f, true, eventData, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
			}
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0003A948 File Offset: 0x00038B48
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this._pointerOverButton = false;
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (!this._wasPressed)
			{
				this.DoStateTransition(Selectable.SelectionState.Normal, false);
			}
			if (this.doSelectedOnHighlight)
			{
				this._onDeselected.Invoke();
			}
			if (!EventSystem.current.alreadySelecting && base.DeviceInputType != DeviceInputType.Remote)
			{
				this.menuNavigation.ClearFocus(false);
			}
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x0003A9B8 File Offset: 0x00038BB8
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (this._pointerOverButton)
			{
				this.DoStateTransition(Selectable.SelectionState.Highlighted, false);
				this.Press();
				Dbug.Log.Info("{0}.Press() : {1}", new object[]
				{
					base.name,
					this._audioProfile
				});
				if (this._audioProfile != UIAudioProfile.None && this._audioSystem != null)
				{
					this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, this._audioProfile, -1f, true, eventData, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
					return;
				}
			}
			else
			{
				this.DoStateTransition(Selectable.SelectionState.Normal, false);
			}
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x0003AA59 File Offset: 0x00038C59
		public void ClearSelectionState()
		{
			this.InstantClearState();
			this._wasPressed = false;
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x0003AA68 File Offset: 0x00038C68
		public void OnInteractableToggled(bool isInteractable)
		{
			if (isInteractable)
			{
				this.ClearSelectionState();
			}
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x0003AA74 File Offset: 0x00038C74
		public override void OnSubmit(BaseEventData eventData)
		{
			this.Press();
			if (this._audioProfile != UIAudioProfile.None && this._audioSystem != null)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.Click, this._audioProfile, -1f, true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
			}
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this.DoPressedAnimation();
			if (base.DeviceInputType != DeviceInputType.Touch)
			{
				base.StartCoroutine(this.OnFinishSubmit());
			}
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x0003AAE4 File Offset: 0x00038CE4
		public override void OnActivate()
		{
			base.OnActivate();
			this.Press();
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (base.DeviceInputType != DeviceInputType.Touch)
			{
				base.StartCoroutine(this.OnFinishSubmit());
			}
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x0003AB18 File Offset: 0x00038D18
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this._onSelected.Invoke();
			if (this._audioProfile != UIAudioProfile.None && this._audioSystem != null)
			{
				this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.MouseOver, this._audioProfile, -1f, true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
			}
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x0003AB67 File Offset: 0x00038D67
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			this._onDeselected.Invoke();
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x0003AB7B File Offset: 0x00038D7B
		public override void DoPressedAnimation()
		{
			if (base.DeviceInputType == DeviceInputType.Touch)
			{
				this.DoStateTransition(Selectable.SelectionState.Highlighted, false);
				return;
			}
			this.DoStateTransition(Selectable.SelectionState.Pressed, false);
			base.StartCoroutine(this.OnFinishSubmit());
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x0003ABA3 File Offset: 0x00038DA3
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (base.DeviceInputType == DeviceInputType.Touch)
			{
				if (state == Selectable.SelectionState.Pressed)
				{
					state = Selectable.SelectionState.Highlighted;
				}
			}
			else if (state == Selectable.SelectionState.Selected && this.OverrideSelectedState)
			{
				state = Selectable.SelectionState.Highlighted;
			}
			base.DoStateTransition(state, instant);
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x0003ABCD File Offset: 0x00038DCD
		private IEnumerator OnFinishSubmit()
		{
			float fadeTime = base.colors.fadeDuration;
			float elapsedTime = 0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			this.DoStateTransition(base.currentSelectionState, false);
			yield break;
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x0003ABDC File Offset: 0x00038DDC
		public void ForceInitializeState()
		{
			this.DoStateTransition(Selectable.SelectionState.Normal, true);
		}

		// Token: 0x04000F33 RID: 3891
		[FormerlySerializedAs("onClick")]
		[SerializeField]
		private TouchButton.ButtonClickedEvent _onClick = new TouchButton.ButtonClickedEvent();

		// Token: 0x04000F34 RID: 3892
		[SerializeField]
		private TouchButton.ButtonClickedEvent _onSelected = new TouchButton.ButtonClickedEvent();

		// Token: 0x04000F35 RID: 3893
		[SerializeField]
		private TouchButton.ButtonClickedEvent _onDeselected = new TouchButton.ButtonClickedEvent();

		// Token: 0x04000F36 RID: 3894
		[Tooltip("Should OnSelected also call when it's highlighted?")]
		public bool doSelectedOnHighlight;

		// Token: 0x04000F37 RID: 3895
		private bool _pointerOverButton;

		// Token: 0x04000F38 RID: 3896
		private bool _wasPressed;

		// Token: 0x020002CD RID: 717
		[Serializable]
		public class ButtonClickedEvent : UnityEvent
		{
		}
	}
}
