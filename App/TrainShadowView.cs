using System;
using System.Collections.Generic;
using Motorways;
using UnityEngine;

// Token: 0x020001E0 RID: 480
public class TrainShadowView : MonoBehaviour
{
	// Token: 0x06000B7B RID: 2939 RVA: 0x0002722C File Offset: 0x0002542C
	public void OnCreatedInScope(VisualConstantsData visualConstantsData)
	{
		this._visualConstantsData = visualConstantsData;
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00027238 File Offset: 0x00025438
	private void Start()
	{
		this.UpdateMainShadowPosition();
		Mesh mainShadowMesh = this._mainShadowMeshFilter.mesh;
		List<TrainShadowView.Edge> boundaryEdges = this.GenerateEdgesFromTriangles(mainShadowMesh.triangles);
		boundaryEdges = this.FindBoundaryEdges(boundaryEdges);
		Vector3[] mainShadowMeshVertices = this._mainShadowMeshFilter.mesh.vertices;
		this._mainShadowMeshBoundaryVertices = new Vector3[boundaryEdges.Count];
		for (int boundaryEdgeIndex = 0; boundaryEdgeIndex < boundaryEdges.Count; boundaryEdgeIndex++)
		{
			this._mainShadowMeshBoundaryVertices[boundaryEdgeIndex] = mainShadowMeshVertices[boundaryEdges[boundaryEdgeIndex].firstVertIndex];
		}
		this._lightToWorldRotation = Quaternion.LookRotation(Vector3.forward, TrainShadowView.SunDirectionWorld);
		this._worldToLightRotation = Quaternion.Inverse(this._lightToWorldRotation);
		this._gapShadowMesh = new Mesh
		{
			vertices = this.CalculateGapMeshVertices(),
			triangles = new int[]
			{
				0,
				1,
				3,
				0,
				3,
				2
			}
		};
		this._gapShadowMeshFilter.mesh = this._gapShadowMesh;
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00027328 File Offset: 0x00025528
	private void UpdateMainShadowPosition()
	{
		this._mainShadowMeshFilter.transform.position = this._carriageTransform.position + this._visualConstantsData.TrainShadowOffset * TrainShadowView.SunDirectionWorld;
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x00027360 File Offset: 0x00025560
	private void Update()
	{
		this.UpdateMainShadowPosition();
		Vector3[] gapMeshVertices = this.CalculateGapMeshVertices();
		this._gapShadowMesh.vertices = gapMeshVertices;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x00027388 File Offset: 0x00025588
	private Vector3[] CalculateGapMeshVertices()
	{
		Quaternion objectToLightRotation = this._carriageTransform.rotation * this._worldToLightRotation;
		Quaternion lightToObjectRotation = Quaternion.Inverse(objectToLightRotation);
		Vector4 v = objectToLightRotation * this._mainShadowMeshBoundaryVertices[0];
		Vector3 minLightVertex = v;
		Vector3 maxLightVertex = v;
		for (int vertexIndex = 1; vertexIndex < this._mainShadowMeshBoundaryVertices.Length; vertexIndex++)
		{
			Vector4 lightSpaceVertex = objectToLightRotation * this._mainShadowMeshBoundaryVertices[vertexIndex];
			if (lightSpaceVertex.x > maxLightVertex.x)
			{
				maxLightVertex = lightSpaceVertex;
			}
			if (lightSpaceVertex.x < minLightVertex.x)
			{
				minLightVertex = lightSpaceVertex;
			}
		}
		Vector3 minLightVertexOffset = minLightVertex - this._visualConstantsData.TrainShadowOffset * Vector3.up;
		Vector3 maxLightVertexOffset = maxLightVertex - this._visualConstantsData.TrainShadowOffset * Vector3.up;
		Vector3 minLocalVertex = lightToObjectRotation * minLightVertex;
		Vector3 maxLocalVertex = lightToObjectRotation * maxLightVertex;
		Vector3 minLocalVertexOffset = lightToObjectRotation * minLightVertexOffset;
		Vector3 maxLocalVertexOffset = lightToObjectRotation * maxLightVertexOffset;
		return new Vector3[]
		{
			minLocalVertex,
			maxLocalVertex,
			minLocalVertexOffset,
			maxLocalVertexOffset
		};
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x000274C0 File Offset: 0x000256C0
	private List<TrainShadowView.Edge> GenerateEdgesFromTriangles(int[] triangles)
	{
		List<TrainShadowView.Edge> result = new List<TrainShadowView.Edge>();
		for (int triIndex = 0; triIndex < triangles.Length; triIndex += 3)
		{
			int v = triangles[triIndex];
			int v2 = triangles[triIndex + 1];
			int v3 = triangles[triIndex + 2];
			result.Add(new TrainShadowView.Edge(v, v2));
			result.Add(new TrainShadowView.Edge(v2, v3));
			result.Add(new TrainShadowView.Edge(v3, v));
		}
		return result;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x0002751C File Offset: 0x0002571C
	private List<TrainShadowView.Edge> FindBoundaryEdges(List<TrainShadowView.Edge> edges)
	{
		List<TrainShadowView.Edge> boundaryEdges = new List<TrainShadowView.Edge>(edges);
		for (int firstIndex = boundaryEdges.Count - 1; firstIndex > 0; firstIndex--)
		{
			for (int secondIndex = firstIndex - 1; secondIndex >= 0; secondIndex--)
			{
				if (boundaryEdges[firstIndex].firstVertIndex == boundaryEdges[secondIndex].secondVertIndex && boundaryEdges[firstIndex].secondVertIndex == boundaryEdges[secondIndex].firstVertIndex)
				{
					boundaryEdges.RemoveAt(firstIndex);
					boundaryEdges.RemoveAt(secondIndex);
					firstIndex--;
					break;
				}
			}
		}
		return boundaryEdges;
	}

	// Token: 0x04000696 RID: 1686
	private static readonly Vector3 SunDirectionWorld = new Vector3(1f, -1f, 0f).normalized;

	// Token: 0x04000697 RID: 1687
	[SerializeField]
	private Transform _carriageTransform;

	// Token: 0x04000698 RID: 1688
	[SerializeField]
	private MeshFilter _mainShadowMeshFilter;

	// Token: 0x04000699 RID: 1689
	[SerializeField]
	private MeshFilter _gapShadowMeshFilter;

	// Token: 0x0400069A RID: 1690
	private Mesh _gapShadowMesh;

	// Token: 0x0400069B RID: 1691
	private Vector3[] _mainShadowMeshBoundaryVertices;

	// Token: 0x0400069C RID: 1692
	private Quaternion _lightToWorldRotation = Quaternion.identity;

	// Token: 0x0400069D RID: 1693
	private Quaternion _worldToLightRotation = Quaternion.identity;

	// Token: 0x0400069E RID: 1694
	private VisualConstantsData _visualConstantsData;

	// Token: 0x020001E1 RID: 481
	private struct Edge
	{
		// Token: 0x06000B84 RID: 2948 RVA: 0x000275E6 File Offset: 0x000257E6
		public Edge(int firstVertIndex, int secondVertIndex)
		{
			this.firstVertIndex = firstVertIndex;
			this.secondVertIndex = secondVertIndex;
		}

		// Token: 0x0400069F RID: 1695
		public readonly int firstVertIndex;

		// Token: 0x040006A0 RID: 1696
		public readonly int secondVertIndex;
	}
}
