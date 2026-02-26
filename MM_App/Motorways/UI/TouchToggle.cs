using System;
using System.Collections;
using Motorways.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000746 RID: 1862
	[AddComponentMenu("UI/Touch Toggle", 35)]
	[RequireComponent(typeof(RectTransform))]
	public class TouchToggle : VariableDeviceSelectable, IPointerDownHandler, IEventSystemHandler, ICanvasElement
	{
		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x06003400 RID: 13312 RVA: 0x000F5923 File Offset: 0x000F3B23
		// (set) Token: 0x06003401 RID: 13313 RVA: 0x000F592B File Offset: 0x000F3B2B
		public ToggleButtonGroup Group
		{
			get
			{
				return this._group;
			}
			set
			{
				this._group = value;
				this.PlayEffect(true);
			}
		}

		// Token: 0x06003402 RID: 13314 RVA: 0x000F593B File Offset: 0x000F3B3B
		protected TouchToggle()
		{
		}

		// Token: 0x06003403 RID: 13315 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void Rebuild(CanvasUpdate executing)
		{
		}

		// Token: 0x06003404 RID: 13316 RVA: 0x000F5960 File Offset: 0x000F3B60
		protected override void OnEnable()
		{
			base.OnEnable();
			this.PlayEffect(true);
		}

		// Token: 0x06003405 RID: 13317 RVA: 0x000F596F File Offset: 0x000F3B6F
		public void AddOnSelectedEvent(UnityAction newEvent)
		{
			this._onSelected.AddListener(newEvent);
		}

		// Token: 0x06003406 RID: 13318 RVA: 0x000F597D File Offset: 0x000F3B7D
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this._onSelected.Invoke();
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.MouseOver, this._audioProfile, -1f, true, null, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
		}

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x06003407 RID: 13319 RVA: 0x000F59B1 File Offset: 0x000F3BB1
		// (set) Token: 0x06003408 RID: 13320 RVA: 0x000F59B9 File Offset: 0x000F3BB9
		public bool IsOn
		{
			get
			{
				return this._isOn;
			}
			set
			{
				this.Set(value, true);
			}
		}

		// Token: 0x06003409 RID: 13321 RVA: 0x000F59C4 File Offset: 0x000F3BC4
		public void Set(bool value, bool sendCallback = true)
		{
			if (this._isOn == value)
			{
				return;
			}
			this._isOn = value;
			if (this._group != null && this.IsActive() && (this._isOn || (!this._group.AnyTogglesOn() && !this._group.allowSwitchOff)))
			{
				this._group.NotifyToggleOn(this);
				this._isOn = true;
			}
			this.PlayEffect(this.toggleTransition == TouchToggle.ToggleTransition.None);
			if (sendCallback)
			{
				this.onValueChanged.Invoke(this._isOn);
			}
		}

		// Token: 0x0600340A RID: 13322 RVA: 0x000F5A50 File Offset: 0x000F3C50
		private void PlayEffect(bool instant)
		{
			if (this.graphic == null)
			{
				return;
			}
			this.graphic.CrossFadeAlpha(this._isOn ? 1f : 0f, instant ? 0f : 0.1f, true);
			if (this.graphic.GetComponent<CanvasGroup>())
			{
				this.graphic.GetComponent<CanvasGroup>().alpha = (this._isOn ? 1f : 0f);
			}
		}

		// Token: 0x0600340B RID: 13323 RVA: 0x000F5AD1 File Offset: 0x000F3CD1
		protected override void Start()
		{
			this.PlayEffect(true);
		}

		// Token: 0x0600340C RID: 13324 RVA: 0x000F5ADC File Offset: 0x000F3CDC
		private void InternalToggle(PointerEventData data = null)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this.IsOn = !this.IsOn;
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(this.IsOn ? UIEventType.CheckboxChecked : UIEventType.CheckboxUnchecked, this._audioProfile, -1f, true, data, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
			if (this._feedbackGenerator != null)
			{
				this._feedbackGenerator.GenerateFeedback(HapticFeedbackType.Selection);
			}
		}

		// Token: 0x0600340D RID: 13325 RVA: 0x000F5B4A File Offset: 0x000F3D4A
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (base.DeviceInputType == DeviceInputType.Touch && state == Selectable.SelectionState.Highlighted)
			{
				state = Selectable.SelectionState.Normal;
			}
			base.DoStateTransition(state, instant);
		}

		// Token: 0x0600340E RID: 13326 RVA: 0x000F5B63 File Offset: 0x000F3D63
		public override void DoPressedAnimation()
		{
			this.DoStateTransition(Selectable.SelectionState.Pressed, true);
			base.StartCoroutine(this.OnFinishSubmit());
		}

		// Token: 0x0600340F RID: 13327 RVA: 0x000F5B7A File Offset: 0x000F3D7A
		private IEnumerator OnFinishSubmit()
		{
			float fadeTime = base.colors.fadeDuration;
			float elapsedTime = 0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			this.DoStateTransition(Selectable.SelectionState.Normal, false);
			yield break;
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x000F5B89 File Offset: 0x000F3D89
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			this.InternalToggle(eventData);
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x000F5B99 File Offset: 0x000F3D99
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			this._audioSystem.ScheduleEvent(AudioEvent.CreateUIEvent(UIEventType.MouseOver, this._audioProfile, -1f, true, eventData, ScreenStack.MotorwaysScreen.None, ScreenStack.MotorwaysScreen.None));
		}

		// Token: 0x06003412 RID: 13330 RVA: 0x000F5BC2 File Offset: 0x000F3DC2
		public override void OnSubmit(BaseEventData eventData)
		{
			this.InternalToggle(null);
		}

		// Token: 0x06003413 RID: 13331 RVA: 0x000022F5 File Offset: 0x000004F5
		public void LayoutComplete()
		{
		}

		// Token: 0x06003414 RID: 13332 RVA: 0x000F5BCB File Offset: 0x000F3DCB
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.DoStateTransition(Selectable.SelectionState.Normal, false);
		}

		// Token: 0x06003415 RID: 13333 RVA: 0x000022F5 File Offset: 0x000004F5
		public void GraphicUpdateComplete()
		{
		}

		// Token: 0x06003416 RID: 13334 RVA: 0x000AB22A File Offset: 0x000A942A
		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		// Token: 0x04002C67 RID: 11367
		public TouchToggle.ToggleTransition toggleTransition = TouchToggle.ToggleTransition.Fade;

		// Token: 0x04002C68 RID: 11368
		public Graphic graphic;

		// Token: 0x04002C69 RID: 11369
		[SerializeField]
		private ToggleButtonGroup _group;

		// Token: 0x04002C6A RID: 11370
		public TouchToggle.ToggleEvent onValueChanged = new TouchToggle.ToggleEvent();

		// Token: 0x04002C6B RID: 11371
		[SerializeField]
		private Button.ButtonClickedEvent _onSelected = new Button.ButtonClickedEvent();

		// Token: 0x04002C6C RID: 11372
		[SerializeField]
		[Tooltip("Is the toggle currently on or off?")]
		[FormerlySerializedAs("m_IsActive")]
		private bool _isOn;

		// Token: 0x02000747 RID: 1863
		public enum ToggleTransition
		{
			// Token: 0x04002C6E RID: 11374
			None,
			// Token: 0x04002C6F RID: 11375
			Fade
		}

		// Token: 0x02000748 RID: 1864
		[Serializable]
		public class ToggleEvent : UnityEvent<bool>
		{
		}
	}
}
