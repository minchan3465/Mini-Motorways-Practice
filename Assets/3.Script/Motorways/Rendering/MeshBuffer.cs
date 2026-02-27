using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Rendering {
	public class MeshBuffer {
		public List<Vector3> vertices = new List<Vector3>(8192);
		public List<Vector2> uvs = new List<Vector2>(8192);

		public List<Vector2> uv2s = new List<Vector2>(8192);
		public List<Vector2> uv3s = new List<Vector2>(8192);	//애니메이션 데이터

		public List<int> triangles = new List<int>(16384);

		public void Clear() {
			vertices.Clear();
			uvs.Clear();
			uv2s.Clear();
			uv3s.Clear();
			triangles.Clear();
		}

		//다른 메쉬 데이터를 이 버퍼에 변환(회전+이동)하여 합칩니다.
		//posOffset: 타일의 월드 좌표 (예: 5, 0, 5)
		//rotationSteps: 아틀라스가 지시한 회전 횟수 (0~3)
		public void Append(RoadMeshData rawData, Vector3 posOffset, int rotationSteps, Vector2 animData) {
			if (rawData == null || rawData.vertices == null) return;

			int startIndex = vertices.Count;

			//UV2 데이터가 있는지 확인 (없으면 기본값 0,0 사용)
			bool hasUV2 = rawData.uv2 != null && rawData.uv2.Length == rawData.vertices.Length;

			for (int i = 0; i < rawData.vertices.Length; i++) {
				Vector3 v = rawData.vertices[i];
				if (rotationSteps > 0) v = RotatePoint8Way(v, rotationSteps);
				v += posOffset;
				vertices.Add(v);

				uvs.Add(rawData.uvs[i]);

				//UV2 (Pivot) 처리. 피벗도 정점과 똑같이 회전+이동해야 월드 좌표가 맞음.
				Vector3 pivotV3 = Vector3.zero;
				if (hasUV2) {
					pivotV3 = new Vector3(rawData.uv2[i].x, 0, rawData.uv2[i].y);
				}

				if (rotationSteps > 0) pivotV3 = RotatePoint8Way(pivotV3, rotationSteps);
				pivotV3 += posOffset;

				//다시 Vector2로 변환하여 저장 (XZ 평면)
				uv2s.Add(new Vector2(pivotV3.x, pivotV3.z));

				//UV3 (Time/Anim) 처리 - 모든 정점에 동일한 값 적용
				uv3s.Add(animData);
			}

			for (int i = 0; i < rawData.triangles.Length; i++) {
				triangles.Add(startIndex + rawData.triangles[i]);
			}
		}

		public void Append(RoadMeshData rawData, Vector3 posOffset, int rotationSteps) {
			Append(rawData, posOffset, rotationSteps, Vector2.zero);
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
