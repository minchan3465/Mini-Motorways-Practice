using System;
using Factory;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
	// Token: 0x020002DA RID: 730
	public class LoadScreenInterruptionPopup : BasePopup
	{
		// Token: 0x060011FA RID: 4602 RVA: 0x0003BBB6 File Offset: 0x00039DB6
		public void Initialise(StringId headerStringId, StringId contentStringId, Action onClose)
		{
			this._mainPromptText.SetStringId(this._scope, headerStringId);
			this._additionalText.SetStringId(this._scope, contentStringId);
			this._onClose = onClose;
		}

		// Token: 0x060011FB RID: 4603 RVA: 0x0003BBE5 File Offset: 0x00039DE5
		public void OnCloseButtonPressed()
		{
			this._popupStack.PopPopup(false);
		}

		// Token: 0x060011FC RID: 4604 RVA: 0x0003BBF3 File Offset: 0x00039DF3
		public override void OnPopupClosed()
		{
			base.OnPopupClosed();
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		// Token: 0x04000F85 RID: 3973
		[Dependency]
		private IScope _scope;

		// Token: 0x04000F86 RID: 3974
		[Dependency]
		private PopupStack _popupStack;

		// Token: 0x04000F87 RID: 3975
		[SerializeField]
		private TouchButton _closeButton;

		// Token: 0x04000F88 RID: 3976
		[SerializeField]
		private LocalizedTextUI _mainPromptText;

		// Token: 0x04000F89 RID: 3977
		[SerializeField]
		private LocalizedTextUI _additionalText;

		// Token: 0x04000F8A RID: 3978
		private Action _onClose;
	}
}
