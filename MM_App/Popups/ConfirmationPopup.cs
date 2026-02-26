using System;
using Factory;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
	// Token: 0x020002D6 RID: 726
	public class ConfirmationPopup : AbstractConfirmationPopup
	{
		// Token: 0x060011E3 RID: 4579 RVA: 0x0003B8E6 File Offset: 0x00039AE6
		public override void Initialise(IScope scope, StringId mainPromptStringId, Action onNoPressed, Action onYesPressed, StringId additionalInfoStringId = StringId.None)
		{
			this._mainPromptText.SetStringId(scope, mainPromptStringId);
			this._onNoPressed = onNoPressed;
			this._onYesPressed = onYesPressed;
			this._yesButton.gameObject.SetActive(true);
			this.SetAdditionalInfo(scope, additionalInfoStringId);
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x0003B91F File Offset: 0x00039B1F
		public override void Initialise(IScope scope, StringId mainPromptStringId, Action onClosed, StringId additionalInfoStringId = StringId.None)
		{
			this._mainPromptText.SetStringId(scope, mainPromptStringId);
			this._onNoPressed = onClosed;
			this._yesButton.gameObject.SetActive(false);
			this.SetAdditionalInfo(scope, additionalInfoStringId);
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x0003B950 File Offset: 0x00039B50
		private void SetAdditionalInfo(IScope scope, StringId additionalInfoStringId)
		{
			if (additionalInfoStringId != StringId.None)
			{
				this._additionalText.gameObject.SetActive(true);
				this._additionalText.SetStringId(scope, additionalInfoStringId);
				return;
			}
			this._additionalText.gameObject.SetActive(false);
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x0003B986 File Offset: 0x00039B86
		public void NoPressed()
		{
			this._popupStack.PopPopup(false);
			Action onNoPressed = this._onNoPressed;
			if (onNoPressed == null)
			{
				return;
			}
			onNoPressed();
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x0003B9A4 File Offset: 0x00039BA4
		public void YesPressed()
		{
			this._popupStack.PopPopup(false);
			Action onYesPressed = this._onYesPressed;
			if (onYesPressed == null)
			{
				return;
			}
			onYesPressed();
		}

		// Token: 0x04000F6C RID: 3948
		[SerializeField]
		private TouchButton _yesButton;

		// Token: 0x04000F6D RID: 3949
		[SerializeField]
		private LocalizedTextUI _mainPromptText;

		// Token: 0x04000F6E RID: 3950
		[SerializeField]
		private LocalizedTextUI _additionalText;

		// Token: 0x04000F6F RID: 3951
		[Dependency]
		private PopupStack _popupStack;

		// Token: 0x04000F70 RID: 3952
		private Action _onNoPressed;

		// Token: 0x04000F71 RID: 3953
		private Action _onYesPressed;
	}
}
