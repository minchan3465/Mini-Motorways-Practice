using System;
using Motorways.Views;
using UnityEngine;

namespace Motorways.UI
{
	// Token: 0x0200072A RID: 1834
	public class MainMenuPins : MonoBehaviour
	{
		// Token: 0x0600327E RID: 12926 RVA: 0x000EED95 File Offset: 0x000ECF95
		public void OnPinAppear(int pinIndex)
		{
			this._mainMenu.OnLogoPinAppear(pinIndex);
		}

		// Token: 0x0600327F RID: 12927 RVA: 0x000EEDA3 File Offset: 0x000ECFA3
		public void OnPinDisappear(int pinIndex)
		{
			this._mainMenu.OnLogoPinDisappear(pinIndex);
		}

		// Token: 0x04002B51 RID: 11089
		[SerializeField]
		private MainMenuScreen _mainMenu;
	}
}
