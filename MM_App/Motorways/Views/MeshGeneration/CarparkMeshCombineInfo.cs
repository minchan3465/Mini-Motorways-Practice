using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x02000628 RID: 1576
	public class CarparkMeshCombineInfo : MonoBehaviour
	{
		// Token: 0x06002C11 RID: 11281 RVA: 0x000C3600 File Offset: 0x000C1800
		public void Remove()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			foreach (object obj in base.transform)
			{
				UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x04002633 RID: 9779
		[SerializeField]
		public CarparkMesh[] singleCarparkMeshes;

		// Token: 0x04002634 RID: 9780
		[SerializeField]
		public CarparkMesh singleCarparkTopLeftOpen;

		// Token: 0x04002635 RID: 9781
		[SerializeField]
		public CarparkMesh singleCarparkTopLeftClosed;

		// Token: 0x04002636 RID: 9782
		[SerializeField]
		public CarparkMesh singleCarparkBottomRightOpen;

		// Token: 0x04002637 RID: 9783
		[SerializeField]
		public CarparkMesh singleCarparkBottomRightClosed;

		// Token: 0x04002638 RID: 9784
		[SerializeField]
		public CarparkMesh[] doubleCarparkTiles;

		// Token: 0x04002639 RID: 9785
		[SerializeField]
		public CarparkMesh doubleCarparkTopLeftOpen;

		// Token: 0x0400263A RID: 9786
		[SerializeField]
		public CarparkMesh doubleCarparkTopLeftClosed;

		// Token: 0x0400263B RID: 9787
		[SerializeField]
		public CarparkMesh doubleCarparkBottomRightOpen;

		// Token: 0x0400263C RID: 9788
		[SerializeField]
		public CarparkMesh doubleCarparkBottomRightClosed;

		// Token: 0x0400263D RID: 9789
		[SerializeField]
		public CarparkMesh doubleCarparkTopLeftCorner;

		// Token: 0x0400263E RID: 9790
		[SerializeField]
		public CarparkMesh doubleCarparkTopRightCorner;

		// Token: 0x0400263F RID: 9791
		[SerializeField]
		[FormerlySerializedAs("ferryTopLeftCorner")]
		public CarparkMesh boatTopLeftCorner;

		// Token: 0x04002640 RID: 9792
		[SerializeField]
		[FormerlySerializedAs("ferryTopRightCorner")]
		public CarparkMesh boatTopRightCorner;

		// Token: 0x04002641 RID: 9793
		[FormerlySerializedAs("ferryTopLeftEntrance")]
		[SerializeField]
		public CarparkMesh boatTopLeftEntrance;

		// Token: 0x04002642 RID: 9794
		[SerializeField]
		[FormerlySerializedAs("ferryTopRightEntrance")]
		public CarparkMesh boatTopRightEntrance;

		// Token: 0x04002643 RID: 9795
		[SerializeField]
		public CarparkMesh[] reversedDoubleCarparkTiles;

		// Token: 0x04002644 RID: 9796
		[SerializeField]
		public CarparkMesh reversedDoubleCarparkTopLeftOpen;

		// Token: 0x04002645 RID: 9797
		[SerializeField]
		public CarparkMesh reversedDoubleCarparkTopLeftClosed;

		// Token: 0x04002646 RID: 9798
		[SerializeField]
		public CarparkMesh reversedDoubleCarparkBottomRightOpen;

		// Token: 0x04002647 RID: 9799
		[SerializeField]
		public CarparkMesh reversedDoubleCarparkBottomRightClosed;
	}
}
