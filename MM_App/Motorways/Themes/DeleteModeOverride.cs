using System;

namespace Motorways.Themes
{
	// Token: 0x02000473 RID: 1139
	[Serializable]
	public class DeleteModeOverride
	{
		// Token: 0x06001C69 RID: 7273 RVA: 0x0006962C File Offset: 0x0006782C
		public DeleteModeOverride(ThemedMaterialType themeType, float additionalDarken, float additionalSaturation)
		{
			this.type = themeType.ToString();
			this.additionalDarkenMultiplier = additionalDarken;
			this.additionalSaturationMultiplier = additionalSaturation;
		}

		// Token: 0x04001802 RID: 6146
		[StringEnumSearch(typeof(ThemedMaterialType))]
		public string type;

		// Token: 0x04001803 RID: 6147
		public float hueOverride;

		// Token: 0x04001804 RID: 6148
		public float additionalSaturationMultiplier;

		// Token: 0x04001805 RID: 6149
		public float additionalDarkenMultiplier;
	}
}
