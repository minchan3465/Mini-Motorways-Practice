using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Rendering {
	public class MeshBuffer {
		public List<Vector3> vertices = new List<Vector3>(8192);
		public List<Vector2> uvs = new List<Vector2>(8192);
		public List<int> triangles = new List<int>(16384);

		public void Clear() {
			vertices.Clear();
			uvs.Clear();
			triangles.Clear();
		}

		//다른 메쉬 데이터를 이 버퍼에 변환(회전+이동)하여 합칩니다.
		//posOffset: 타일의 월드 좌표 (예: 5, 0, 5)
		//rotationSteps: 아틀라스가 지시한 회전 횟수 (0~3)
		public void Append(RoadMeshData rawData, Vector3 posOffset, int rotationSteps) {
			if (rawData == null || rawData.vertices == null) return;

			int startIndex = vertices.Count;

			for (int i = 0; i < rawData.vertices.Length; i++) {
				Vector3 v = rawData.vertices[i];
				if (rotationSteps > 0) v = RotatePoint8Way(v, rotationSteps);
				v += posOffset;
				vertices.Add(v);
				uvs.Add(rawData.uvs[i]);
			}

			for (int i = 0; i < rawData.triangles.Length; i++) {
				triangles.Add(startIndex + rawData.triangles[i]);
			}
		}

		private Vector3 RotatePoint8Way(Vector3 v, int steps) {
			steps = (steps % 8 + 8) % 8;
			float x = v.x, z = v.z;
			float sq = 0.70710678f; // sqrt(2)/2

			switch (steps) {
				case 0: return v;
				case 1: return new Vector3(x * sq + z * sq, v.y, -x * sq + z * sq); // 45
				case 2: return new Vector3(z, v.y, -x); // 90
				case 3: return new Vector3(-x * sq + z * sq, v.y, -x * sq - z * sq); // 135
				case 4: return new Vector3(-x, v.y, -z); // 180
				case 5: return new Vector3(-x * sq - z * sq, v.y, x * sq - z * sq); // 225
				case 6: return new Vector3(-z, v.y, x); // 270
				case 7: return new Vector3(x * sq - z * sq, v.y, x * sq + z * sq); // 315
				default: return v;
			}
		}
	}
}
