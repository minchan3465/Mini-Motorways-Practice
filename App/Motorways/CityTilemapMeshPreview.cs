using System;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200039C RID: 924
	[ExecuteAlways]
	public class CityTilemapMeshPreview : MonoBehaviour
	{
		// Token: 0x06001606 RID: 5638 RVA: 0x0004BB48 File Offset: 0x00049D48
		private void Awake()
		{
			this._previewMeshFilter = base.gameObject.AddComponent<MeshFilter>();
			this._previewMeshFilter.gameObject.transform.localScale = new Vector3(2f, 2f, 1f);
			this._meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x0004BBA0 File Offset: 0x00049DA0
		public void SetPreviewMaterial(Material material)
		{
			this._landPreviewMaterial = material;
			base.GetComponent<MeshRenderer>().sharedMaterial = this._landPreviewMaterial;
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0004BBBA File Offset: 0x00049DBA
		public void SetPreviewMesh(Mesh mesh)
		{
			this._previewMeshFilter.sharedMesh = mesh;
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x0004BBC8 File Offset: 0x00049DC8
		public void SetSortingLayer(string sortingLayerName)
		{
			this._meshRenderer.sortingLayerID = SortingLayer.NameToID(sortingLayerName);
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x0004BBDB File Offset: 0x00049DDB
		public void SetSortingOrder(int sortingOrder)
		{
			this._meshRenderer.sortingOrder = sortingOrder;
		}

		// Token: 0x040012C8 RID: 4808
		private MeshFilter _previewMeshFilter;

		// Token: 0x040012C9 RID: 4809
		private Material _landPreviewMaterial;

		// Token: 0x040012CA RID: 4810
		private MeshRenderer _meshRenderer;
	}
}
