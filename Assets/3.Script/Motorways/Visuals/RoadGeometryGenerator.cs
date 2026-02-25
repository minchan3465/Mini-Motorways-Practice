using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	using Motorways.Utils;

	public struct BezierCurveData {
		public Vector3 P0, P1, P2, P3;
	}

	public static class RoadGeometryGenerator{
        private const float EDGE_EXTRUSION = 1.005f;

        public static BezierCurveData ConstructPathFromConnection(TileDirection from, TileDirection to, bool isDeadEnd, bool isHubSpoke) {
            Vector3 p0 = GetEdgePoint(from);
            Vector3 p3 = (to == TileDirection.None) ? Vector3.zero : GetEdgePoint(to); // 끝이 없으면(DeadEnd) 중앙(0,0)으로

            //가장자리에서 타일 중심(0,0,0)을 향하는 방향 벡터
            Vector3 dir0 = -p0.normalized;
            Vector3 dir3 = (to == TileDirection.None) ? -dir0 : -p3.normalized;

            //두 방향 사이의 각도를 구하여 핸들의 길이 결정
            float angle = (to == TileDirection.None) ? 180f : Vector3.Angle(dir0, dir3);
            float tension = GetTension(angle);

            //막다른 길 ~> 중심점에서 멈춤
            if (to == TileDirection.None) {
                if (isDeadEnd) {
                    //[막다른 길] 둥근 마감을 씌울 공간을 남기기 위해 타일 중앙에 도달하기 전에 멈춤
                    tension = 0.15f;
                    p3 = -dir0 * 0.1f;
                } else {
                    //[교차로 팔] 중앙(0,0,0)을 살짝(5%) 관통하여, 다른 방향에서 오는 팔들과 완벽하게 겹치도록(구멍 소멸) 강제
                    tension = 0.1f;
                    p3 = dir0 * 0.05f;
                }
            } else if (Mathf.Approximately(angle, 45f) || angle < 90f) {
                // 45도 커브가 부풀어 오르는 것을 막기 위해 장력을 0에 가깝게 당김
                tension = 0.05f;
            }

            Vector3 p1 = p0 + (dir0 * tension);
            Vector3 p2 = p3 + (dir3 * tension);

            p0 = p0 * EDGE_EXTRUSION;
            if (to != TileDirection.None) {
                p3 = p3 * EDGE_EXTRUSION;
            }

            return new BezierCurveData {
                P0 = p0,
                P1 = p1,
                P2 = p2,
                P3 = p3
            };
        }

        //3D 공간(XZ 평면)에서 해당 방향의 가장자리 로컬 좌표를 반환 (타일 크기 1.0 기준)
        private static Vector3 GetEdgePoint(TileDirection dir) {
            Vector2Int v = TileUtils.GetDirectionVector(dir);

            //대각선 방향도 정확히 타일의 끝에 맞추기 위해 정규화 후 반지름(0.5f)을 곱함
            //Vector2 normalizedVec = new Vector2(vec2.x, vec2.y).normalized * 0.5f;
            //return new Vector3(normalizedVec.x, 0, normalizedVec.y);
            
            //근데 대각선 방향도 정규화해버리니까 길이가 짧다. 어짜피 타일 크기는 0.5니까, 그거에 맞춰서 수정.
            Vector2 edge = new Vector2(v.x, v.y) * 0.5f;
            return new Vector3(edge.x, 0, edge.y);
        }

        //각도에 따른 베지어 핸들의 적정 길이 계산
        private static float GetTension(float angle) {
            if (Mathf.Approximately(angle, 180f)) return 0.25f; // 직선
            if (Mathf.Approximately(angle, 90f)) return 0.276f; // 90도 코너 (원호 마법의 숫자: 0.552 / 2)
            if (angle < 90f) return 0.05f;  // 예각 코너 (뾰족한 부분)
            return 0.35f; // 둔각 코너 (완만한 부분)
        }

    }
}
