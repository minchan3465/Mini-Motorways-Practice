using System;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.Themes
{
	// Token: 0x02000470 RID: 1136
	[CreateAssetMenu(menuName = "Motorways/Theme/Color Group")]
	public class ColorGroup : ScriptableObject
	{
		// Token: 0x06001C65 RID: 7269 RVA: 0x000695B8 File Offset: 0x000677B8
		public Color GetColor(ThemeComponentGroupTarget groupTarget)
		{
			if (Diagnostics.Verify(groupTarget < (ThemeComponentGroupTarget)this.targets.Length, this, "Unable to find matching color for index: {0} - targets.Length: {1} ", (int)groupTarget, this.targets.Length))
			{
				return this.targets[(int)groupTarget];
			}
			return Color.magenta;
		}

		// Token: 0x06001C66 RID: 7270 RVA: 0x000022F5 File Offset: 0x000004F5
		[Button(null)]
		public void UpdateTheme()
		{
		}

		// Token: 0x040017F4 RID: 6132
		[SerializeField]
		[EnumTypedArray(typeof(ThemeComponentGroupTarget))]
		private Color[] targets = new Color[10];
	}
}
