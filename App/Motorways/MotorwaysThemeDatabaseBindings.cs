using System;
using Motorways.Themes;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003DF RID: 991
	[CreateAssetMenu(menuName = "Motorways/Theme/Theme Database Bindings")]
	public class MotorwaysThemeDatabaseBindings : ScriptableObject
	{
		// Token: 0x06001812 RID: 6162 RVA: 0x0005610C File Offset: 0x0005430C
		public int GetPerGroupThemeTargetForMaterial(Material materialToCompare)
		{
			for (int materialIndex = 0; materialIndex < this.perGroupMaterials.materialBindings.Length; materialIndex++)
			{
				if (materialToCompare == this.perGroupMaterials.materialBindings[materialIndex])
				{
					return materialIndex;
				}
			}
			return -1;
		}

		// Token: 0x0400149B RID: 5275
		[SerializeField]
		public PerGroupMaterialBindings perGroupMaterials;

		// Token: 0x0400149C RID: 5276
		[Space(20f)]
		[SerializeField]
		public ThemeMaterialCollection materialCollection;

		// Token: 0x0400149D RID: 5277
		public Theme colorblindThemeColorful;

		// Token: 0x0400149E RID: 5278
		public Theme colorblindThemeDark;
	}
}
