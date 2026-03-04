using UnityEngine;
using System.Collections.Generic;

namespace Motorways.Utils {
    public static class Spline {
        
        public class BezierSpline {
            public readonly Vector3 p0;
            public readonly Vector3 p1;
            public readonly Vector3 p2;
            public readonly Vector3 p3;
            public readonly bool isCubic;

            // 2차 베지어 곡선용 생성자 (직선도 이를 이용 가능)
            public BezierSpline(Vector3 p0, Vector3 p1, Vector3 p2) {
                this.p0 = p0;
                this.p1 = p1;
                this.p2 = p2;
                this.isCubic = false;
            }

            // 3차 베지어 곡선용 생성자
            public BezierSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
                this.p0 = p0;
                this.p1 = p1;
                this.p2 = p2;
                this.p3 = p3;
                this.isCubic = true;
            }

            // t(0~1) 위치에서의 좌표 계산
            public Vector3 Evaluate(float t) {
                if (isCubic) {
                    return BezierUtils.GetPoint(p0, p1, p2, p3, t);
                } else {
                    return BezierUtils.GetPoint(p0, p1, p2, t);
                }
            }

            // t(0~1) 위치에서의 접선(방향) 계산
            public Vector3 EvaluateTangent(float t) {
                if (isCubic) {
                    return BezierUtils.GetTangent(p0, p1, p2, p3, t);
                } else {
                    return BezierUtils.GetTangent(p0, p1, p2, t);
                }
            }

            // 곡선의 전체 길이 근사치 계산
            public float Length(int resolution = 20) {
                float length = 0f;
                Vector3 previousPoint = Evaluate(0f);
                for (int i = 1; i <= resolution; i++) {
                    float t = (float)i / resolution;
                    Vector3 currentPoint = Evaluate(t);
                    length += Vector3.Distance(previousPoint, currentPoint);
                    previousPoint = currentPoint;
                }
                return length;
            }
        }
    }
}
