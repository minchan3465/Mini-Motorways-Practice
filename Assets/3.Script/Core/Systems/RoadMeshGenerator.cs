using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems {
	public class RoadMeshGenerator {
		//start = 시작점, end = 끝점
		//width = 도로폭
		public static Mesh GenerateStraightMesh(Vector3 start, Vector3 end, float width = 1.0f) {
			Mesh mesh = new Mesh();

			Vector3 forward = (end - start).normalized; //방향벡터.

			//오른쪽 벡터. (외적)
			//3D TopView라서, 기준은 x,z가 된다. 따라서 y축과 외적.
			Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

			//정점 4개. 기준점으로부터 거리 두고 사각형 꼭짓점 두는거라 생각하면 편할듯
			float halfWidth = width * 0.5f;
			Vector3 v0 = start - right * halfWidth; // 시작점 왼쪽
			Vector3 v1 = start + right * halfWidth; // 시작점 오른쪽
			Vector3 v2 = end - right * halfWidth;   // 끝점 왼쪽
			Vector3 v3 = end + right * halfWidth;   // 끝점 오른쪽

			mesh.vertices = new Vector3[] { v0, v1, v2, v3 };
			mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 }; //삼각형 연결

			//UV 매핑. 
			mesh.uv = new Vector2[] {
				new Vector2(0, 0), // v0
                new Vector2(1, 0), // v1
                new Vector2(0, 1), // v2
                new Vector2(1, 1)  // v3	
			};
			mesh.RecalculateNormals();

			//mesh.RecalculateNormals();	//조명을 위해 법선 계산...? 이 게임은 단색 그래픽이라 삭제 예정.
			return mesh;
		}

		//public static Mesh
	}
}

/* UV에 대한 고찰
- UV란 뭘까?
설명으로는 수학이나 벡터의 XY를 그래픽에서는 UV라고 한다. (색상은 RG로 표현)
즉, 2차원 상태의 좌표값을 의미하는것 같은데...
범위는 0~1 까지밖에 안가진다고 한다.
즉, 무조건 이 그래픽에서의 최대 범위는 1이란건가?
UV를 쉐이더 그래프에 켰을때, X값이 높고 Y값이 낮으면 빨간색, Y값이 높고 X값이 낮으면 초록색으로 표현되는 이유가
UV = XY = RG 라서.
솔직히 잘 모르겟다! 왜 UV라고 표현하는지는 모르겠다.
*/