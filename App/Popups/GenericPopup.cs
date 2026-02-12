using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Popups
{
	// Token: 0x020002D9 RID: 729
	public class GenericPopup : BasePopup
	{
		// Token: 0x060011F1 RID: 4593 RVA: 0x0003BA65 File Offset: 0x00039C65
		public void Initialise(StringId headerStringId, StringId contentStringId)
		{
			this._headerText.SetStringId(this._scope, headerStringId);
			this._infoText.SetStringId(this._scope, contentStringId);
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x0003BA90 File Offset: 0x00039C90
		public void Initialise(StringId headerStringId, [NotNull] StringId[] contentStringIds)
		{
			this._tickButton.gameObject.SetActive(false);
			this._crossButton.gameObject.SetActive(false);
			this._headerText.SetStringId(this._scope, headerStringId);
			this._pages = new List<StringId>(contentStringIds);
			this.SelectPage(0);
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x0003BAE8 File Offset: 0x00039CE8
		private void SelectPage(int pageIndex)
		{
			this._currentPageIndex = Mathf.Clamp(pageIndex, 0, this._pages.Count - 1);
			this._infoText.SetStringId(this._scope, this._pages[pageIndex]);
			this._paginationText.LocString = StandaloneLocString.CreateNonLocalizedString(this._scope, string.Format("{0} / {1}", this._currentPageIndex + 1, this._pages.Count));
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x0003BB6A File Offset: 0x00039D6A
		public void OnClosePressed()
		{
			this._popupStack.PopPopup(false);
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x0003BB78 File Offset: 0x00039D78
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

		// Token: 0x060011F6 RID: 4598 RVA: 0x0003BB6A File Offset: 0x00039D6A
		public void OnCrossPressed()
		{
			this._popupStack.PopPopup(false);
		}

		// Token: 0x060011F7 RID: 4599 RVA: 0x0003BB96 File Offset: 0x00039D96
		public void OnLeftPressed()
		{
			this.SelectPage(this._currentPageIndex - 1);
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x0003BBA6 File Offset: 0x00039DA6
		public void OnRightPressed()
		{
			this.SelectPage(this._currentPageIndex + 1);
		}

		// Token: 0x04000F77 RID: 3959
		[Dependency]
		private IScope _scope;

		// Token: 0x04000F78 RID: 3960
		[Dependency]
		private PopupStack _popupStack;

		// Token: 0x04000F79 RID: 3961
		[SerializeField]
		private LocalizedTextUI _headerText;

		// Token: 0x04000F7A RID: 3962
		[SerializeField]
		private LocalizedTextUI _infoText;

		// Token: 0x04000F7B RID: 3963
		[SerializeField]
		private LocalizedTextUI _paginationText;

		// Token: 0x04000F7C RID: 3964
		[SerializeField]
		private TouchButton _tickButton;

		// Token: 0x04000F7D RID: 3965
		[SerializeField]
		private TouchButton _crossButton;

		// Token: 0x04000F7E RID: 3966
		[SerializeField]
		private TouchButton _closeButton;

		// Token: 0x04000F7F RID: 3967
		[SerializeField]
		private TouchButton _leftButton;

		// Token: 0x04000F80 RID: 3968
		[SerializeField]
		private TouchButton _rightButton;

		// Token: 0x04000F81 RID: 3969
		private Action _onTick;

		// Token: 0x04000F82 RID: 3970
		private Action _onCross;

		// Token: 0x04000F83 RID: 3971
		private List<StringId> _pages;

		// Token: 0x04000F84 RID: 3972
		private int _currentPageIndex;
	}
}
