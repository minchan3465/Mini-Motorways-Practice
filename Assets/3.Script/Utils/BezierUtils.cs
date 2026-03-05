using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Utils {
	public static class BezierUtils {
		// 베지어 곡선 공식
		// 2차 베지어 곡선 공식 : (1-t)^2*P0 + 2(1-t)t*P1 + t^2*P2
		// 3차 베지어 곡선 공식 : (1-t)^3*P0 + 3(1-t)^2*t*P1 + 3(1-t)t^2*P2 + t^3*P3

		// 2차 베지어 포인트 계산
		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;

			Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
			return p;
		}

		// 3차 베지어 포인트 계산
		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;
			float ttt = tt * t;
			float uuu = uu * u;

			Vector3 p = (uuu * p0) + (3 * uu * t * p1) + (3 * u * tt * p2) + (ttt * p3);
			return p;
		}

		// 베지어 곡선을 미분하여 접선(Tangent) 벡터를 구합니다.
		// 이는 객체가 진행 방향을 바라보게 하거나, 도로의 너비를 계산할 때 사용됩니다.
		// 2차 베지어 미분 : 2(1-t)(P1-P0) + 2t(P2-P1)
		// 3차 베지어 미분 : 3(1-t)^2(P1-P0) + 6(1-t)t(P2-P1) + 3t^2(P3-P2)

		// 2차 베지어 접선 벡터 계산
		public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
			Vector3 p = (2 * (1 - t) * (p1 - p0)) + (2 * t * (p2 - p1));
			return p.normalized;
		}

		// 3차 베지어 접선 벡터 계산
		public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			float u = 1 - t;
			float tt = t * t;
			float uu = u * u;

			Vector3 p = (3 * uu * (p1 - p0)) + (6 * u * t * (p2 - p1)) + (3 * tt * (p3 - p2));
			return p.normalized;
		}
	}
}
