using System;
using UnityEngine;

namespace Motorways.Themes
{
	// Token: 0x02000477 RID: 1143
	[Serializable]
	public class ThemedColor
	{
		// Token: 0x06001C77 RID: 7287 RVA: 0x00069D2D File Offset: 0x00067F2D
		public ThemedColor(ThemedMaterialType type, Color color, string propertyToChange = "_Color")
		{
			this.type = type.ToString();
			this.color = color;
			this.propertyToChange = propertyToChange;
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001C78 RID: 7288 RVA: 0x00069D64 File Offset: 0x00067F64
		public ThemedMaterialType MaterialType
		{
			get
			{
				if (this._materialType.ToString() != this.type && !Diagnostics.Verify(this.type.TryParse(out this._materialType), "{0} isn't a valid ThemedMaterialType!", this.type))
				{
					return ThemedMaterialType.Land;
				}
				return this._materialType;
			}
		}

		// Token: 0x04001816 RID: 6166
		[StringEnumSearch(typeof(ThemedMaterialType))]
		public string type;

		// Token: 0x04001817 RID: 6167
		public Color color;

		// Token: 0x04001818 RID: 6168
		public string propertyToChange = "_Color";

		// Token: 0x04001819 RID: 6169
		private ThemedMaterialType _materialType;
	}
}
