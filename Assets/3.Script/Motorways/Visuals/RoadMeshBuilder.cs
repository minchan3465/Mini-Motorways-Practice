using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	public static class RoadMeshBuilder {
		public static Mesh BuildRoadMesh(RoadVisualPath path, RoadVisualSettings settings) {
			if (path.VisualPoints == null || path.VisualPoints.Count < 2) return null;

			List<Vector3> points = path.VisualPoints;
			int pointCount = points.Count;

			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> triangles = new List<int>();

			float halfWidth = settings.RoadWidth * 0.5f;

            for (int i = 0; i < pointCount; i++) {
                Vector3 currentPoint = points[i];
                Vector3 forward;

                // 접선(Tangent, 진행 방향) 계산
                if (i == 0) {
                    forward = (points[1] - points[0]).normalized;
                } else if (i == pointCount - 1) {
                    forward = (points[i] - points[i - 1]).normalized;
                } else {
                    // 중간 점들은 이전 점과 다음 점을 이어 부드러운 방향을 구함
                    forward = (points[i + 1] - points[i - 1]).normalized;
                }

                // 진행 방향을 기준으로 오른쪽(Right) 벡터 도출 (Up 벡터와 외적)
                Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

                // 중심점에서 양옆으로 halfWidth만큼 밀어내어 좌/우 정점 생성
                Vector3 leftVertex = currentPoint - (right * halfWidth);
                Vector3 rightVertex = currentPoint + (right * halfWidth);

                vertices.Add(leftVertex);
                vertices.Add(rightVertex);

                // UV 전개 (도로의 길이를 V로, 폭을 U로 사용)
                float v = (float)i / (pointCount - 1);
                uvs.Add(new Vector2(0f, v));
                uvs.Add(new Vector2(1f, v));
            }

            for (int i = 0; i < pointCount - 1; i++) {
                int rootIndex = i * 2;

                int leftCurrent = rootIndex;
                int rightCurrent = rootIndex + 1;
                int leftNext = rootIndex + 2;
                int rightNext = rootIndex + 3;

                // 첫 번째 삼각형 (좌하 -> 우하 -> 좌상)
                triangles.Add(leftCurrent);
                triangles.Add(rightNext);
                triangles.Add(rightCurrent);

                // 두 번째 삼각형 (좌하 -> 좌상 -> 우상)
                triangles.Add(leftCurrent);
                triangles.Add(leftNext);
                triangles.Add(rightNext);
            }

            // 3. Unity Mesh 객체 조립
            Mesh roadMesh = new Mesh();
            roadMesh.name = "GeneratedRoadMesh";
            roadMesh.SetVertices(vertices);
            roadMesh.SetUVs(0, uvs);
            roadMesh.SetTriangles(triangles, 0);

            // 조명 연산을 위한 노멀 재계산
            roadMesh.RecalculateNormals();

            return roadMesh;
        }
	}
}
