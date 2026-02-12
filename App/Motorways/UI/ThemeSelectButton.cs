using System;
using Motorways.Themes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Motorways.UI
{
	// Token: 0x02000743 RID: 1859
	[RequireComponent(typeof(Animator))]
	public class ThemeSelectButton : TouchButton
	{
		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x060033EC RID: 13292 RVA: 0x000F56C4 File Offset: 0x000F38C4
		// (set) Token: 0x060033ED RID: 13293 RVA: 0x000F56CC File Offset: 0x000F38CC
		public MapButton mapButton { get; set; }

		// Token: 0x060033EE RID: 13294 RVA: 0x000F56D5 File Offset: 0x000F38D5
		protected override void Awake()
		{
			this._animator = base.GetComponent<Animator>();
		}

		// Token: 0x060033EF RID: 13295 RVA: 0x000F56E4 File Offset: 0x000F38E4
		public void SetSelectorAlpha(float alpha)
		{
			Color color = this._themeToggler.GetComponent<Image>().color;
			color.a = alpha;
			this._themeToggler.GetComponent<Image>().color = color;
		}

		// Token: 0x060033F0 RID: 13296 RVA: 0x000F571B File Offset: 0x000F391B
		public void OnSelected()
		{
			this.mapButton.SetThemePreference(this.buttonTheme);
		}

		// Token: 0x060033F1 RID: 13297 RVA: 0x000F571B File Offset: 0x000F391B
		public void OnClicked()
		{
			this.mapButton.SetThemePreference(this.buttonTheme);
		}

		// Token: 0x060033F2 RID: 13298 RVA: 0x000F5730 File Offset: 0x000F3930
		public override void OnDeselect(BaseEventData eventData)
		{
			base.OnDeselect(eventData);
			this.mapButton.EnsureThemeButtonSelectedState(null);
		}

		// Token: 0x060033F3 RID: 13299 RVA: 0x000F5758 File Offset: 0x000F3958
		public void SetUnselected()
		{
			this._themeToggler.SetSelectedTheme(true);
		}

		// Token: 0x060033F4 RID: 13300 RVA: 0x000F5766 File Offset: 0x000F3966
		public void SetSelected()
		{
			this._themeToggler.SetSelectedTheme(false);
		}

		// Token: 0x060033F5 RID: 13301 RVA: 0x000022F5 File Offset: 0x000004F5
		public void SetHighlighted()
		{
		}

		// Token: 0x060033F6 RID: 13302 RVA: 0x000022F5 File Offset: 0x000004F5
		private void Update()
		{
		}

		// Token: 0x04002C5F RID: 11359
		private Animator _animator;

		// Token: 0x04002C60 RID: 11360
		public MotorwaysThemePreference buttonTheme;

		// Token: 0x04002C61 RID: 11361
		public Image themeColorPreviewImage;

		// Token: 0x04002C62 RID: 11362
		[SerializeField]
		private ThemeTypeToggler _themeToggler;
	}
}
