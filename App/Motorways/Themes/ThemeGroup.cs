using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.Themes
{
	// Token: 0x0200047C RID: 1148
	[CreateAssetMenu(menuName = "Motorways/Themes/ThemeGroup")]
	public class ThemeGroup : ScriptableObject
	{
		// Token: 0x06001C87 RID: 7303 RVA: 0x0006A27D File Offset: 0x0006847D
		public ThemeGroup()
		{
			this.themedColors = new List<ThemedColor>();
		}

		// Token: 0x06001C88 RID: 7304 RVA: 0x0006A290 File Offset: 0x00068490
		public ThemedColor ThemeColorFor(ThemedMaterialType colorType)
		{
			return this.themedColors.Find((ThemedColor col) => col.MaterialType == colorType);
		}

		// Token: 0x06001C89 RID: 7305 RVA: 0x000022F5 File Offset: 0x000004F5
		[Button(null)]
		public void UpdateTheme()
		{
		}

		// Token: 0x04001883 RID: 6275
		[Tooltip("An optional name to help with identifying what this is.")]
		public string title;

		// Token: 0x04001884 RID: 6276
		public List<ThemedColor> themedColors;
	}
}
