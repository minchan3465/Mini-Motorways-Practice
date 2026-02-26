using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Rendering {
	public class MeshBuffer {
		public List<Vector3> vertices = new List<Vector3>(4096);
		public List<Vector2> uvs = new List<Vector2>(4096);
		public List<int> triangles = new List<int>(8192);

		public void Clear() {
			vertices.Clear();
			uvs.Clear();
			triangles.Clear();
		}

		//다른 메쉬 데이터를 이 버퍼에 변환(회전+이동)하여 합칩니다.
		//posOffset: 타일의 월드 좌표 (예: 5, 0, 5)
		//rotationSteps: 아틀라스가 지시한 회전 횟수 (0~3)
		public void Append(Vector3[] srcVerts, Vector2[] srcUVs, int[] srcTris, Vector3 posOffset, int rotationSteps) {
			if (srcVerts == null || srcVerts.Length == 0) return;

			int startIndex = vertices.Count;

			for (int i = 0; i < srcVerts.Length; i++) {
				Vector3 v = srcVerts[i];

				if (rotationSteps > 0) {
					v = RotatePoint45Degrees(v, rotationSteps); // 8방향 지원으로 함수명/로직 변경 예정
				}

				v += posOffset;
				vertices.Add(v);
				uvs.Add(srcUVs[i]);
			}

			for (int i = 0; i < srcTris.Length; i++) {
				triangles.Add(startIndex + srcTris[i]);
			}
		}

		private Vector3 RotatePoint45Degrees(Vector3 v, int steps) {
			float angle = steps * 45f;
			float rad = angle * Mathf.Deg2Rad;
			float cos = Mathf.Cos(rad);
			float sin = Mathf.Sin(rad);

			// 타일의 로컬 중심점(0,0)을 기준으로 XZ 평면 회전
			float x = v.x * cos - v.z * sin;
			float z = v.x * sin + v.z * cos;

			return new Vector3(x, v.y, z);
		}
	}
}
