using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	[Serializable]
	public class RoadTileMesh{
		public Mesh roadMesh;
		public Mesh outlineMesh;

		public Vector3[] roadVertices;
		public Vector2[] roadUVs;
		public int[] roadTriangles;

		public Vector3[] outlineVertices;
		public Vector2[] outlineUVs;
		public int[] outlineTriangles;

		public void CacheMeshData() {
			if (roadMesh != null) {
				roadVertices = roadMesh.vertices;
				roadUVs = roadMesh.uv;
				roadTriangles = roadMesh.triangles;
			}
			if (outlineMesh != null) {
				outlineVertices = outlineMesh.vertices;
				outlineUVs = outlineMesh.uv;
				outlineTriangles = outlineMesh.triangles;
			}
		}
	}
}
