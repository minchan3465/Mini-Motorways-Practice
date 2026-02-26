using System;
using Motorways.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.UI.EditMenu
{
	// Token: 0x02000754 RID: 1876
	public class EditMenuButton : TouchButton
	{
		// Token: 0x0600346E RID: 13422 RVA: 0x000F6BE0 File Offset: 0x000F4DE0
		public void SetButtonToState(EditMenuButton.ButtonState buttonState)
		{
			switch (buttonState)
			{
			case EditMenuButton.ButtonState.Disabled:
				base.interactable = false;
				base.animator.ResetTrigger(EditMenuButton.Hidden);
				base.animator.ResetTrigger(EditMenuButton.Normal);
				base.animator.SetTrigger(EditMenuButton.Disabled);
				return;
			case EditMenuButton.ButtonState.Hidden:
				base.interactable = false;
				base.animator.ResetTrigger(EditMenuButton.Normal);
				base.animator.ResetTrigger(EditMenuButton.Disabled);
				base.animator.SetTrigger(EditMenuButton.Hidden);
				return;
			case EditMenuButton.ButtonState.Normal:
				base.interactable = true;
				base.animator.ResetTrigger(EditMenuButton.Hidden);
				base.animator.ResetTrigger(EditMenuButton.Disabled);
				base.animator.SetTrigger(EditMenuButton.Normal);
				return;
			default:
				EditMenuButton.Log.Error("Only button states normal, hidden or disabled are handled by SetButtonToState!", Array.Empty<object>());
				return;
			}
		}

		// Token: 0x0600346F RID: 13423 RVA: 0x000F6CC0 File Offset: 0x000F4EC0
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			if (this.onPressed != null)
			{
				this.onPressed(eventData.clickTime, this.ButtonType, eventData.pointerId, null);
			}
		}

		// Token: 0x06003470 RID: 13424 RVA: 0x000F6CEF File Offset: 0x000F4EEF
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			if (this.onPointerEnter != null)
			{
				this.onPointerEnter(this);
			}
		}

		// Token: 0x06003471 RID: 13425 RVA: 0x000F6D0C File Offset: 0x000F4F0C
		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);
			if (this.onPointerExit != null)
			{
				this.onPointerExit(this);
			}
		}

		// Token: 0x06003472 RID: 13426 RVA: 0x000F6D2C File Offset: 0x000F4F2C
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
				this.onPressed(-1f, this.ButtonType, -1, onController);
			}
		}

		// Token: 0x04002CC4 RID: 11460
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("EditMenuButton");

		// Token: 0x04002CC5 RID: 11461
		private const string AnimatorDisabledFlag = "Disabled";

		// Token: 0x04002CC6 RID: 11462
		private const string AnimatorHiddenFlag = "Hidden";

		// Token: 0x04002CC7 RID: 11463
		private const string AnimatorNormalFlag = "Normal";

		// Token: 0x04002CC8 RID: 11464
		private static readonly int Disabled = Animator.StringToHash("Disabled");

		// Token: 0x04002CC9 RID: 11465
		private static readonly int Hidden = Animator.StringToHash("Hidden");

		// Token: 0x04002CCA RID: 11466
		private static readonly int Normal = Animator.StringToHash("Normal");

		// Token: 0x04002CCB RID: 11467
		public EditMenuButtonType ButtonType;

		// Token: 0x04002CCC RID: 11468
		public EditMenuButton.OnAssetButtonPressed onPressed;

		// Token: 0x04002CCD RID: 11469
		public Image IconImage;

		// Token: 0x04002CCE RID: 11470
		public EditMenuButton.OnFocusPointerEnter onPointerEnter;

		// Token: 0x04002CCF RID: 11471
		public EditMenuButton.OnFocusPointerExit onPointerExit;

		// Token: 0x02000755 RID: 1877
		public enum ButtonState
		{
			// Token: 0x04002CD1 RID: 11473
			Disabled,
			// Token: 0x04002CD2 RID: 11474
			Hidden,
			// Token: 0x04002CD3 RID: 11475
			Normal
		}

		// Token: 0x02000756 RID: 1878
		// (Invoke) Token: 0x06003476 RID: 13430
		public delegate void OnAssetButtonPressed(float pressTime, EditMenuButtonType type, int pointerIndex, IController onController);

		// Token: 0x02000757 RID: 1879
		// (Invoke) Token: 0x0600347A RID: 13434
		public delegate void OnFocusPointerEnter(EditMenuButton button);

		// Token: 0x02000758 RID: 1880
		// (Invoke) Token: 0x0600347E RID: 13438
		public delegate void OnFocusPointerExit(EditMenuButton button);
	}
}
