using System;
using Motorways.Themes;
using Motorways.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Motorways
{
	// Token: 0x02000449 RID: 1097
	public class MapButtonTab : MonoBehaviour
	{
		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x06001B30 RID: 6960 RVA: 0x00063A72 File Offset: 0x00061C72
		public TouchButton TouchButton
		{
			get
			{
				return this._touchButton;
			}
		}

		// Token: 0x06001B31 RID: 6961 RVA: 0x00063A7A File Offset: 0x00061C7A
		public void Show()
		{
			this._animator.SetBool(MapButtonTab.Shown, true);
			if (this._touchButton.IsInitialized && !string.IsNullOrEmpty(this._touchButton.NewContentId))
			{
				this._touchButton.ShowNewContentIndicatorIfNeeded(true);
			}
		}

		// Token: 0x06001B32 RID: 6962 RVA: 0x00063AB9 File Offset: 0x00061CB9
		public void Hide()
		{
			this._animator.SetBool(MapButtonTab.Shown, false);
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x00063ACC File Offset: 0x00061CCC
		private void Awake()
		{
			this._animator = base.GetComponent<Animator>();
			this._touchButton = base.GetComponent<TouchButton>();
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x00063AE6 File Offset: 0x00061CE6
		public void OnOtherTabSelected()
		{
			this.backgroundThemeToggler.SetSelectedTheme(false);
			this.iconThemeToggler.SetSelectedTheme(true);
			this._animator.SetBool(MapButtonTab.Selected, false);
		}

		// Token: 0x06001B35 RID: 6965 RVA: 0x00063B11 File Offset: 0x00061D11
		public void OnClicked()
		{
			if (this._gameMode != GameMode.Endless && this._gameMode != GameMode.Expert)
			{
				this.backgroundThemeToggler.SetSelectedTheme(true);
			}
			this.iconThemeToggler.SetSelectedTheme(false);
			this._animator.SetBool(MapButtonTab.Selected, true);
		}

		// Token: 0x06001B36 RID: 6966 RVA: 0x00063B4E File Offset: 0x00061D4E
		public void SetSelected(bool isSelected)
		{
			if (isSelected)
			{
				this.OnClicked();
				return;
			}
			this.OnOtherTabSelected();
		}

		// Token: 0x040016B8 RID: 5816
		public MapButton button;

		// Token: 0x040016B9 RID: 5817
		public ThemeTypeToggler backgroundThemeToggler;

		// Token: 0x040016BA RID: 5818
		public ThemeTypeToggler iconThemeToggler;

		// Token: 0x040016BB RID: 5819
		private Animator _animator;

		// Token: 0x040016BC RID: 5820
		private TouchButton _touchButton;

		// Token: 0x040016BD RID: 5821
		private GameMode _gameMode;

		// Token: 0x040016BE RID: 5822
		private static readonly int Selected = Animator.StringToHash("Selected");

		// Token: 0x040016BF RID: 5823
		private static readonly int Shown = Animator.StringToHash("Shown");
	}
}
