using System;
using System.Collections.Generic;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005D0 RID: 1488
	public class CarparkMesh : MonoBehaviour
	{
		// Token: 0x0400236D RID: 9069
		public List<CarparkMesh.ThemedMesh> meshes;

		// Token: 0x020005D1 RID: 1489
		[Serializable]
		public struct ThemedMesh
		{
			// Token: 0x0400236E RID: 9070
			public MeshRenderer meshRenderer;

			// Token: 0x0400236F RID: 9071
			public ThemedMaterialType themedMaterialType;
		}
	}
}
