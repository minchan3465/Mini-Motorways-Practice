using System;
using Factory;
using Motorways.UI;
using Screens;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x02000573 RID: 1395
	public class TwoButtonPrompt : MonoBehaviour
	{
		// Token: 0x0600261F RID: 9759 RVA: 0x000A1A74 File Offset: 0x0009FC74
		public void ShowSinglePromptConfirmation(IScope scope, BaseScalingScreen originalScreen, StringId messageTextId, Action onCancel, Action onConfirm)
		{
			this.ShowTwoPromptConfirmation(scope, originalScreen, messageTextId, onCancel, onConfirm, true);
			this.cancelButton.gameObject.SetActive(false);
			this.confirmButton.gameObject.SetActive(true);
		}

		// Token: 0x06002620 RID: 9760 RVA: 0x000A1AA8 File Offset: 0x0009FCA8
		public void ShowTwoPromptConfirmation(IScope scope, BaseScalingScreen originalScreen, StringId messageTextId, Action onCancel, Action onConfirm, bool selectConfirmByDefault = true)
		{
			this.twoButtonDialogPanel.gameObject.SetActive(true);
			StandaloneLocString messageText = StandaloneLocString.CreateString(scope, messageTextId);
			this.dialogMessageText.LocString = messageText;
			this._onCancelActivated = onCancel;
			this._onConfirmActivated = onConfirm;
			this._originalScreen = originalScreen;
			this._originalScreen.previousBackButton = this._originalScreen.backButton;
			this._originalScreen.backButton = this.backButton;
			this.cancelButton.gameObject.SetActive(true);
			this.confirmButton.gameObject.SetActive(true);
			LocaleDatabase localeDatabase = scope.Get<LocaleDatabase>();
			if (scope.Get<LocaleDatabase>().CurrentLocale.TextDirection == TextDirection.RightToLeft)
			{
				if (this.confirmButton.transform.GetSiblingIndex() > this.cancelButton.transform.GetSiblingIndex())
				{
					this.confirmButton.transform.SetSiblingIndex(this.cancelButton.transform.GetSiblingIndex());
				}
			}
			else if (this.confirmButton.transform.GetSiblingIndex() < this.cancelButton.transform.GetSiblingIndex())
			{
				this.confirmButton.transform.SetSiblingIndex(this.cancelButton.transform.GetSiblingIndex());
			}
			localeDatabase.AddLocalizedObject(this.dialogMessageText);
			if (scope.Get<InputState>().CurrentInputTypeRequiresFocus)
			{
				MenuNavigation menuNavigation = scope.Get<MenuNavigation>();
				if (menuNavigation == null)
				{
					return;
				}
				menuNavigation.SetNewFocus(selectConfirmByDefault ? this.confirmButton : this.cancelButton);
			}
		}

		// Token: 0x06002621 RID: 9761 RVA: 0x000A1C13 File Offset: 0x0009FE13
		public void OnCancelActivated()
		{
			if (Diagnostics.Verify(this._onCancelActivated != null))
			{
				this._onCancelActivated();
			}
		}

		// Token: 0x06002622 RID: 9762 RVA: 0x000A1C30 File Offset: 0x0009FE30
		public void OnConfirmActivated()
		{
			if (Diagnostics.Verify(this._onConfirmActivated != null))
			{
				this._onConfirmActivated();
			}
		}

		// Token: 0x06002623 RID: 9763 RVA: 0x000A1C50 File Offset: 0x0009FE50
		public void HidePrompt(IScope scope)
		{
			if (this.twoButtonDialogPanel.gameObject.activeInHierarchy)
			{
				this._originalScreen.backButton = this._originalScreen.previousBackButton;
				this.twoButtonDialogPanel.gameObject.SetActive(false);
				scope.Get<LocaleDatabase>().RemoveLocalizedObject(this.dialogMessageText);
				this._onCancelActivated = null;
				this._onConfirmActivated = null;
			}
		}

		// Token: 0x0400201F RID: 8223
		public RectTransform twoButtonDialogPanel;

		// Token: 0x04002020 RID: 8224
		public LocalizedTextUI dialogMessageText;

		// Token: 0x04002021 RID: 8225
		public VariableDeviceSelectable cancelButton;

		// Token: 0x04002022 RID: 8226
		public VariableDeviceSelectable confirmButton;

		// Token: 0x04002023 RID: 8227
		public TouchButton backButton;

		// Token: 0x04002024 RID: 8228
		protected Action _onCancelActivated;

		// Token: 0x04002025 RID: 8229
		protected Action _onConfirmActivated;

		// Token: 0x04002026 RID: 8230
		private BaseScalingScreen _originalScreen;
	}
}
