using System;
using Client;
using UnityEngine;

namespace Motorways.Themes
{
	// Token: 0x02000481 RID: 1153
	[RequireComponent(typeof(ThemedComponent))]
	public class ThemeTypeToggler : MonoBehaviour, IThemeComponent
	{
		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x06001C9E RID: 7326 RVA: 0x0006AB34 File Offset: 0x00068D34
		public ThemedMaterialType FirstMaterialType
		{
			get
			{
				if (this._firstMaterialType.ToString() != this.firstType && !Diagnostics.Verify(this.firstType.TryParse(out this._firstMaterialType), "{0} isn't a valid ThemedMaterialType!", this.firstType))
				{
					return ThemedMaterialType.Land;
				}
				return this._firstMaterialType;
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x06001C9F RID: 7327 RVA: 0x0006AB8C File Offset: 0x00068D8C
		public ThemedMaterialType SecondMaterialType
		{
			get
			{
				if (this._secondMaterialType.ToString() != this.secondType && !Diagnostics.Verify(this.secondType.TryParse(out this._secondMaterialType), "{0} isn't a valid ThemedMaterialType!", this.secondType))
				{
					return ThemedMaterialType.Land;
				}
				return this._secondMaterialType;
			}
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x0006ABE4 File Offset: 0x00068DE4
		public void SetSelectedTheme(bool isFirstSelected)
		{
			this._isFirstColorSelected = isFirstSelected;
			this._componentToChange.MaterialType = (this._isFirstColorSelected ? this.FirstMaterialType : this.SecondMaterialType);
			if (this._currentTheme != null)
			{
				this._componentToChange.ApplyTheme(this._currentTheme);
			}
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x0006AC32 File Offset: 0x00068E32
		private void Awake()
		{
			this._componentToChange = base.GetComponent<ThemedComponent>();
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x0006AC40 File Offset: 0x00068E40
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			this.ApplyTheme(newTheme);
			return ThemeBlendingResult.StopBlending;
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x0006AC4A File Offset: 0x00068E4A
		public void ApplyTheme(ITheme theme)
		{
			this._currentTheme = theme;
		}

		// Token: 0x06001CA4 RID: 7332 RVA: 0x000022F5 File Offset: 0x000004F5
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x06001CA5 RID: 7333 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x04001893 RID: 6291
		private bool _isFirstColorSelected = true;

		// Token: 0x04001894 RID: 6292
		[StringEnumSearch(typeof(ThemedMaterialType))]
		public string firstType;

		// Token: 0x04001895 RID: 6293
		[StringEnumSearch(typeof(ThemedMaterialType))]
		public string secondType;

		// Token: 0x04001896 RID: 6294
		private ThemedMaterialType _firstMaterialType;

		// Token: 0x04001897 RID: 6295
		private ThemedMaterialType _secondMaterialType;

		// Token: 0x04001898 RID: 6296
		private ThemedComponent _componentToChange;

		// Token: 0x04001899 RID: 6297
		private ITheme _currentTheme;
	}
}
