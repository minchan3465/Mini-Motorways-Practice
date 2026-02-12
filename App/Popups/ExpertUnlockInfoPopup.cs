using System;
using Factory;
using UnityEngine;

namespace Popups
{
	// Token: 0x020002D8 RID: 728
	public class ExpertUnlockInfoPopup : BasePopup
	{
		// Token: 0x17000391 RID: 913
		// (get) Token: 0x060011ED RID: 4589 RVA: 0x0003BA2A File Offset: 0x00039C2A
		public LocalizedTextUI InfoText
		{
			get
			{
				return this._infoText;
			}
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x0003BA32 File Offset: 0x00039C32
		public void Initialize(Action onConfirmed = null)
		{
			this._tickButton.SetActive(true);
			this._onTick = onConfirmed;
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x0003BA47 File Offset: 0x00039C47
		public void OnTickPressed()
		{
			this._popupStack.PopPopup(false);
			Action onTick = this._onTick;
			if (onTick == null)
			{
				return;
			}
			onTick();
		}

		// Token: 0x04000F73 RID: 3955
		[Dependency]
		private PopupStack _popupStack;

		// Token: 0x04000F74 RID: 3956
		[SerializeField]
		private GameObject _tickButton;

		// Token: 0x04000F75 RID: 3957
		[SerializeField]
		private LocalizedTextUI _infoText;

		// Token: 0x04000F76 RID: 3958
		private Action _onTick;
	}
}
