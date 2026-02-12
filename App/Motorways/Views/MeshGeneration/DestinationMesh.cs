using System;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x02000629 RID: 1577
	public class DestinationMesh : MonoBehaviour
	{
		// Token: 0x04002648 RID: 9800
		public DestinationMesh.Type type;

		// Token: 0x04002649 RID: 9801
		public TileDirection direction;

		// Token: 0x0400264A RID: 9802
		public ThemeComponentGroupTarget groupTarget;

		// Token: 0x0200062A RID: 1578
		public enum Type
		{
			// Token: 0x0400264C RID: 9804
			Square,
			// Token: 0x0400264D RID: 9805
			Circle,
			// Token: 0x0400264E RID: 9806
			StationHorizontal,
			// Token: 0x0400264F RID: 9807
			StationVertical
		}
	}
}
