using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Themes
{
	// Token: 0x02000472 RID: 1138
	[CreateAssetMenu(menuName = "Motorways/Themes/DeleteModeOverrideList")]
	public class DeleteModeOverrideList : ScriptableObject
	{
		// Token: 0x04001801 RID: 6145
		public List<ThemedMaterialType> themeTypesToOverride = new List<ThemedMaterialType>();
	}
}
