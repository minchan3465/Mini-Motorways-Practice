using System;
using Factory;
using UnityEngine;

namespace Popups
{
	// Token: 0x020002D7 RID: 727
	public class ExamplePopup : BasePopup
	{
		// Token: 0x060011E9 RID: 4585 RVA: 0x0003B9CA File Offset: 0x00039BCA
		public void YesPressed()
		{
			if (this.isFullyVisible)
			{
				Debug.Log("Yes");
				this._popupStack.PopPopup(false);
			}
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x0003B9EA File Offset: 0x00039BEA
		public void NoPressed()
		{
			if (this.isFullyVisible)
			{
				Debug.Log("No");
				this._popupStack.PopPopup(false);
			}
		}

		// Token: 0x060011EB RID: 4587 RVA: 0x0003BA0A File Offset: 0x00039C0A
		public void BackPressed()
		{
			if (this.isFullyVisible)
			{
				Debug.Log("Back");
				this._popupStack.PopPopup(false);
			}
		}

		// Token: 0x04000F72 RID: 3954
		[Dependency]
		private PopupStack _popupStack;
	}
}
