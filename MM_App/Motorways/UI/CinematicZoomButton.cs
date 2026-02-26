using System;
using Motorways.Themes;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x0200071E RID: 1822
	public class CinematicZoomButton : MonoBehaviour
	{
		// Token: 0x0600321F RID: 12831 RVA: 0x000ED17C File Offset: 0x000EB37C
		public void Deactivate()
		{
			this._symbolThemeToggler.SetSelectedTheme(false);
			this._fillCanvasGroup.alpha = 0f;
			this._outlineCanvasGroup.alpha = 1f;
			this.alphaOverride = 0.5f;
			this._highlightColor.SetActive(false);
			this._highlightOutline.SetActive(false);
			this._touchButton.interactable = false;
			this._touchButton.ForceInitializeState();
		}

		// Token: 0x06003220 RID: 12832 RVA: 0x000ED1F0 File Offset: 0x000EB3F0
		public void Activate()
		{
			this._symbolThemeToggler.SetSelectedTheme(true);
			this._fillCanvasGroup.alpha = 1f;
			this._outlineCanvasGroup.alpha = 0f;
			this.alphaOverride = 1f;
			this._highlightColor.SetActive(true);
			this._highlightOutline.SetActive(true);
			this._touchButton.interactable = true;
		}

		// Token: 0x06003221 RID: 12833 RVA: 0x000ED258 File Offset: 0x000EB458
		private void Update()
		{
			this._canvasGroup.alpha = this.alphaOverride;
		}

		// Token: 0x04002AF8 RID: 11000
		[SerializeField]
		private CanvasGroup _canvasGroup;

		// Token: 0x04002AF9 RID: 11001
		[SerializeField]
		private ThemeTypeToggler _symbolThemeToggler;

		// Token: 0x04002AFA RID: 11002
		[SerializeField]
		private CanvasGroup _fillCanvasGroup;

		// Token: 0x04002AFB RID: 11003
		[SerializeField]
		private CanvasGroup _outlineCanvasGroup;

		// Token: 0x04002AFC RID: 11004
		[SerializeField]
		private GameObject _highlightColor;

		// Token: 0x04002AFD RID: 11005
		[SerializeField]
		private GameObject _highlightOutline;

		// Token: 0x04002AFE RID: 11006
		[SerializeField]
		private TouchButton _touchButton;

		// Token: 0x04002AFF RID: 11007
		private float alphaOverride = 1f;
	}
}
