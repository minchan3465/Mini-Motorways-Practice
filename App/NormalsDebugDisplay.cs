using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
[RequireComponent(typeof(MeshFilter))]
public class NormalsDebugDisplay : MonoBehaviour
{
	// Token: 0x06000A65 RID: 2661 RVA: 0x00022398 File Offset: 0x00020598
	public void OnDrawGizmos()
	{
		if (this.meshFilter == null)
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
			if (this.meshFilter == null)
			{
				return;
			}
		}
		for (int vertIndex = 0; vertIndex < this.meshFilter.sharedMesh.vertices.Length; vertIndex++)
		{
			Gizmos.color = Color.blue;
			Vector3 pos = base.transform.TransformPoint(this.meshFilter.sharedMesh.vertices[vertIndex]);
			Vector3 next = base.transform.TransformVector(this.meshFilter.sharedMesh.normals[vertIndex]) * this.normalScale + pos;
			Gizmos.DrawLine(pos, next);
		}
	}

	// Token: 0x0400057F RID: 1407
	private MeshFilter meshFilter;

	// Token: 0x04000580 RID: 1408
	public float normalScale = 1f;
}
