using System;

namespace Client
{
	// Token: 0x02000794 RID: 1940
	public interface IThemeComponent
	{
		// Token: 0x06003591 RID: 13713
		void InitializeTheme(IThemeDatabase themeDatabase);

		// Token: 0x06003592 RID: 13714
		void ApplyTheme(ITheme theme);

		// Token: 0x06003593 RID: 13715
		ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress);

		// Token: 0x06003594 RID: 13716
		void ReleaseTheme(IThemeDatabase themeDatabase);
	}
}
