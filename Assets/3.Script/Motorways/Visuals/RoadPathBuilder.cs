using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
	using Motorways.Utils;

    public static class RoadPathBuilder {
        private const int RESOLUTION = 10; // 곡선의 부드러움 정도

        public static List<RoadVisualPath> BuildPathsFromConnections(RoadSignature signature) {
            List<RoadVisualPath> paths = new List<RoadVisualPath>();
            List<RoadConnection> connections = signature.Connections;

            foreach (RoadConnection conn in connections) {
                //연결선의 성격 파악 (End가 None이면 막다른 길이거나 교차로의 중앙행 스포크)
                bool isDeadEnd = (conn.End.Direction == TileDirection.None) && (connections.Count == 1);
                bool isHubSpoke = (conn.End.Direction == TileDirection.None) && (connections.Count > 1);

                BezierCurveData curve = RoadGeometryGenerator.ConstructPathFromConnection(
                    conn.Start.Direction, conn.End.Direction, isDeadEnd, isHubSpoke
                );

                RoadVisualPath path = GeneratePath(curve, conn.Start, conn.End);
                path.IsDeadEnd = isDeadEnd;
                paths.Add(path);
            }

            return paths;
        }

        // 베지어 곡선 데이터를 실제 시각적 정점 리스트로 변환
        private static RoadVisualPath GeneratePath(BezierCurveData curve, RoadTileNode start, RoadTileNode end) {
            RoadVisualPath path = new RoadVisualPath();
            path.StartNode = start;
            path.EndNode = end;

            float step = 1f / (RESOLUTION - 1);
            for (int i = 0; i < RESOLUTION; i++) {
                path.AddPoint(BezierUtils.GetPoint(curve.P0, curve.P1, curve.P2, curve.P3, step * i));
            }
            return path;
        }
    }
}
