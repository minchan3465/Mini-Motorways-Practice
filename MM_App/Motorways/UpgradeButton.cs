using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways
{
	// Token: 0x0200044E RID: 1102
	[RequireComponent(typeof(UpgradeButtonStack))]
	public class UpgradeButton : TouchButton, IPointerDownHandler, IEventSystemHandler
	{
		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x06001B66 RID: 7014 RVA: 0x0000222C File Offset: 0x0000042C
		protected override bool OverrideSelectedState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06001B67 RID: 7015 RVA: 0x00064279 File Offset: 0x00062479
		protected override void Awake()
		{
			base.Awake();
			this._stack = base.GetComponent<UpgradeButtonStack>();
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x0006428D File Offset: 0x0006248D
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			if (this._stack.AccountedIconNumber == 0)
			{
				return;
			}
			if (this.onPressed != null)
			{
				this.onPressed(eventData.clickTime, this.buttonType, eventData.pointerId, null);
			}
		}

		// Token: 0x06001B69 RID: 7017 RVA: 0x000642CA File Offset: 0x000624CA
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
		}

		// Token: 0x06001B6A RID: 7018 RVA: 0x000642D3 File Offset: 0x000624D3
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			this.OnPointerUp(eventData);
		}

		// Token: 0x06001B6B RID: 7019 RVA: 0x000642E4 File Offset: 0x000624E4
		public override void OnSubmit(BaseEventData eventData)
		{
			base.OnSubmit(eventData);
			if (this.onPressed != null)
			{
				IController onController = null;
				ControllerInputEventData controllerInputEventData = eventData as ControllerInputEventData;
				if (controllerInputEventData != null)
				{
					onController = controllerInputEventData.instigatingController;
				}
				this.onPressed(-1f, this.buttonType, -1, onController);
			}
		}

		// Token: 0x06001B6C RID: 7020 RVA: 0x0006432B File Offset: 0x0006252B
		public void DoStateTransition(ButtonAnimationState state, bool instant)
		{
			this.state = state;
			this.DoStateTransition((Selectable.SelectionState)state, instant);
		}

		// Token: 0x06001B6D RID: 7021 RVA: 0x0006433C File Offset: 0x0006253C
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.buttonType == GameUIButtonType.None)
			{
				return;
			}
			if (this.IsInteractable())
			{
				if (base.DeviceInputType == DeviceInputType.Touch && state == Selectable.SelectionState.Highlighted)
				{
					state = Selectable.SelectionState.Normal;
				}
			}
			else
			{
				state = Selectable.SelectionState.Normal;
			}
			UpgradeButtonStack stack = this._stack;
			if (stack != null)
			{
				stack.DoStateTransition((ButtonAnimationState)state, instant);
			}
			base.DoStateTransition(state, instant);
		}

		// Token: 0x040016E2 RID: 5858
		public GameUIButtonType buttonType;

		// Token: 0x040016E3 RID: 5859
		public UpgradeButton.OnAssetButtonPressed onPressed;

		// Token: 0x040016E4 RID: 5860
		private UpgradeButtonStack _stack;

		// Token: 0x040016E5 RID: 5861
		public UpgradeIcon _upgradeIcon;

		// Token: 0x040016E6 RID: 5862
		public ButtonAnimationState state;

		// Token: 0x0200044F RID: 1103
		// (Invoke) Token: 0x06001B70 RID: 7024
		public delegate void OnAssetButtonPressed(float pressTime, GameUIButtonType type, int pointerIndex, IController onController);
	}
}
