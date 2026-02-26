using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000745 RID: 1861
	public class TouchButtonCouldNotLoadLibraryPopup : Selectable
	{
		// Token: 0x060033FE RID: 13310 RVA: 0x000F5903 File Offset: 0x000F3B03
		public override void OnPointerDown(PointerEventData eventData)
		{
			Application.OpenURL("https://dinopoloclub.com/support/mini-motorways/");
			Application.Quit();
			base.OnPointerDown(eventData);
		}

		// Token: 0x04002C66 RID: 11366
		private const string SupportURL = "https://dinopoloclub.com/support/mini-motorways/";
	}
}
