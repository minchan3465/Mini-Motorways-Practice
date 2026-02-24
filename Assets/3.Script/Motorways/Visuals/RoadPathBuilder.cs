using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	using Motorways.Utils;

    public class RoadVisualPath {
        public List<Vector3> VisualPoints { get; private set; } = new List<Vector3>();
        public void AddPoint(Vector3 point) { VisualPoints.Add(point); }
    }

    public static class RoadPathBuilder {
        private const int RESOLUTION = 10; // 곡선의 부드러움 정도

        // 파라미터에서 DefinitionData가 완전히 제거되었습니다.
        public static List<RoadVisualPath> BuildPaths(RoadSignature signature, Vector3 tileWorldPos) {
            List<RoadVisualPath> paths = new List<RoadVisualPath>();

            // 1. RawMask 비트를 기반으로 활성화된 실제 방향 추출
            List<TileDirection> activeDirs = new List<TileDirection>();
            TileDirection[] allDirs = (TileDirection[])Enum.GetValues(typeof(TileDirection));

            foreach (TileDirection dir in allDirs) {
                if (dir == TileDirection.None || dir == TileDirection.All) continue;
                if ((signature.RawMask & dir) == dir) {
                    activeDirs.Add(dir);
                }
            }

            if (activeDirs.Count == 0) return paths;

            // 2. 쌍(Pair) 연결을 통한 기하학적 데이터 생성
            List<BezierCurveData> curves = new List<BezierCurveData>();

            if (activeDirs.Count == 1) {
                // 막다른 길 
                curves.Add(RoadGeometryGenerator.ConstructPathFromConnection(activeDirs[0], TileDirection.None));
            } else {
                // 2개 이상 (직선, 코너, 그리고 교차로의 쌍방향 중첩)
                for (int i = 0; i < activeDirs.Count; i++) {
                    for (int j = i + 1; j < activeDirs.Count; j++) {
                        curves.Add(RoadGeometryGenerator.ConstructPathFromConnection(activeDirs[i], activeDirs[j]));
                    }
                }
            }

            // 3. 베지어 점 데이터를 시각적 정점(VisualPoints)으로 변환
            foreach (BezierCurveData curve in curves) {
                RoadVisualPath path = new RoadVisualPath();
                float step = 1f / (RESOLUTION - 1);

                for (int i = 0; i < RESOLUTION; i++) {
                    float t = step * i;
                    Vector3 localPos = BezierUtils.GetPoint(curve.P0, curve.P1, curve.P2, curve.P3, t);
                    path.AddPoint(tileWorldPos + localPos);
                }
                paths.Add(path);
            }

            return paths;
        }
    }
}
