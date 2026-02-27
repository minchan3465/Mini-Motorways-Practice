using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	//순수 배열 데이터만 보관하는 구조체? -> 클래스
	[Serializable]
	public class RoadMeshData {
		public Vector3[] vertices;
		public Vector2[] uvs;
		public int[] triangles;
	}

	[Serializable]
	public class RoadTileMesh {
		public RoadMeshData road;
		public RoadMeshData outline;
		public RoadMeshData dashedOutline;

		public void Reset() {
			road = null;
			outline = null;
			dashedOutline = null;
		}
	}
}
