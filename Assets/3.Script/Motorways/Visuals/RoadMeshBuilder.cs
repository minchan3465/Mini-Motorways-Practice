using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
    using Motorways.Utils;

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

                if (i == 0 && path.StartNode.Direction != TileDirection.None) {
                    // 시작점: 곡선의 미세한 휨을 무시하고 타일 경계선에 완벽히 수직이 되도록 강제
                    Vector2Int v = TileUtils.GetDirectionVector(path.StartNode.Direction);
                    forward = new Vector3(-v.x, 0, -v.y).normalized;
                    currentPoint = new Vector3(v.x * 0.5f, 0, v.y * 0.5f);
                } else if (i == pointCount - 1 && path.EndNode.Direction != TileDirection.None) {
                    // 끝점: 타일 밖으로 나가는 절대 방향 강제
                    Vector2Int v = TileUtils.GetDirectionVector(path.EndNode.Direction);
                    forward = new Vector3(v.x, 0, v.y).normalized;
                    currentPoint = new Vector3(v.x * 0.5f, 0, v.y * 0.5f);
                } else {
                    // 도로 내부의 점들은 부드러운 곡선을 따름
                    if (i == 0) forward = (points[1] - points[0]).normalized;
                    else if (i == pointCount - 1) forward = (points[i] - points[i - 1]).normalized;
                    else forward = (points[i + 1] - points[i - 1]).normalized;
                }

                // 진행 방향을 기준으로 오른쪽(Right) 벡터 도출 (Up 벡터와 외적)
                Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

                // 중심점에서 양옆으로 halfWidth만큼 밀어내어 좌/우 정점 생성
                Vector3 leftVertex = currentPoint - (right * halfWidth);
                Vector3 rightVertex = currentPoint + (right * halfWidth);

                vertices.Add(leftVertex);
                vertices.Add(rightVertex);

                // UV 전개(U: 가로, V: 세로)
                float vCoord = (float)i / (pointCount - 1);
                uvs.Add(new Vector2(0f, vCoord)); // 향후 셰이더에서 0.0~0.1은 좌측 연석으로 판별
                uvs.Add(new Vector2(1f, vCoord)); // 향후 셰이더에서 0.9~1.0은 우측 연석으로 판별
            }

            for (int i = 0; i < pointCount - 1; i++) {
                int rootIndex = i * 2;
                triangles.Add(rootIndex);
                triangles.Add(rootIndex + 3);
                triangles.Add(rootIndex + 1);

                triangles.Add(rootIndex);
                triangles.Add(rootIndex + 2);
                triangles.Add(rootIndex + 3);
            }

            // 2. 둥근 마감 (Round Cap) 생성
            if (path.IsDeadEnd && settings.CapResolution > 0) {
                int capRes = settings.CapResolution;
                Vector3 endCenter = points[pointCount - 1];
                Vector3 endForward = (points[pointCount - 1] - points[pointCount - 2]).normalized;

                if (path.StartNode.Direction != TileDirection.None) {
                    Vector2Int v = TileUtils.GetDirectionVector(path.StartNode.Direction);
                    endForward = new Vector3(-v.x, 0, -v.y).normalized;
                } else {
                    endForward = (points[pointCount - 1] - points[pointCount - 2]).normalized;
                }


                // 부채꼴 중심점
                int centerIndex = vertices.Count;
                vertices.Add(endCenter);
                uvs.Add(new Vector2(0.5f, 1f));

                int arcStartIndex = vertices.Count;

                // -90도(좌측)에서 90도(우측)로 회전하며 정점 추가
                for (int i = 0; i <= capRes; i++) {
                    float t = (float)i / capRes;
                    float angle = Mathf.Lerp(-90f, 90f, t);

                    Vector3 direction = Quaternion.Euler(0, angle, 0) * endForward;
                    vertices.Add(endCenter + (direction * halfWidth));
                    //보간.
                    uvs.Add(new Vector2(Mathf.Lerp(0f, 1f, t), 1f));
                }

                // 부채꼴 삼각형 연결
                for (int i = 0; i < capRes; i++) {
                    triangles.Add(centerIndex);
                    triangles.Add(arcStartIndex + i);
                    triangles.Add(arcStartIndex + i + 1);
                }
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
