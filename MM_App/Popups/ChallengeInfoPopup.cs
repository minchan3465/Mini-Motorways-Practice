using System;
using Factory;
using JetBrains.Annotations;
using UnityEngine;

namespace Popups
{
	// Token: 0x020002D5 RID: 725
	public class ChallengeInfoPopup : BasePopup
	{
		// Token: 0x060011DE RID: 4574 RVA: 0x0003B89A File Offset: 0x00039A9A
		public void Initialise(IScope scope, StringId headerStringId, StringId contentStringId, Action onConfirmed = null)
		{
			this._headerText.SetStringId(scope, headerStringId);
			this._infoText.SetStringId(scope, contentStringId);
			this._onConfirmed = onConfirmed;
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x0003B8C0 File Offset: 0x00039AC0
		public override void OnPopupClosed()
		{
			base.OnPopupClosed();
			Action onConfirmed = this._onConfirmed;
			if (onConfirmed == null)
			{
				return;
			}
			onConfirmed();
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x0003B8D8 File Offset: 0x00039AD8
		[UsedImplicitly]
		public void ClosePressed()
		{
			this._popupStack.PopPopup(false);
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x0002C46B File Offset: 0x0002A66B
		public override void Reset()
		{
			base.Reset();
		}

		// Token: 0x04000F68 RID: 3944
		[Dependency]
		private PopupStack _popupStack;

		// Token: 0x04000F69 RID: 3945
		[SerializeField]
		private LocalizedTextUI _headerText;

		// Token: 0x04000F6A RID: 3946
		[SerializeField]
		private LocalizedTextUI _infoText;

		// Token: 0x04000F6B RID: 3947
		private Action _onConfirmed;
	}
}
