using System;
using Factory;
using Factory.Pools;
using UnityEngine;

// Token: 0x020001B8 RID: 440
[Factory.Serializable(1)]
[System.Serializable]
public class RoadTileMesh : IReusable
{
	// Token: 0x06000A67 RID: 2663 RVA: 0x00022464 File Offset: 0x00020664
	public void Reset()
	{
		this.roadMesh = null;
		this.outlineMesh = null;
		this.dashedOutlineMesh = null;
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x0002247C File Offset: 0x0002067C
	public void ApplyMeshOverrides(RoadTileMesh overrides)
	{
		if (overrides.roadMesh != null)
		{
			this.roadMesh = overrides.roadMesh;
		}
		if (overrides.outlineMesh != null)
		{
			this.outlineMesh = overrides.outlineMesh;
		}
		if (overrides.dashedOutlineMesh != null)
		{
			this.dashedOutlineMesh = overrides.dashedOutlineMesh;
		}
	}

	// Token: 0x04000581 RID: 1409
	[Serialize(true, typeof(MeshSerializer))]
	public Mesh roadMesh;

	// Token: 0x04000582 RID: 1410
	[Serialize(true, typeof(MeshSerializer))]
	public Mesh outlineMesh;

	// Token: 0x04000583 RID: 1411
	[Serialize(true, typeof(MeshSerializer))]
	public Mesh dashedOutlineMesh;
}
