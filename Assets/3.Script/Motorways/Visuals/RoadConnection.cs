using System;
using UnityEngine;

namespace Motorways.Visuals {
    [Serializable]
    public struct RoadConnection {
        public RoadTileNode Start;
        public RoadTileNode End;

        public RoadConnection(RoadTileNode start, RoadTileNode end) {
            Start = start;
            End = end;
        }

        // 노드 기반 회전으로 변경
        public RoadConnection Rotate(int steps) {
            RoadTileNode newStart = Start.Rotate(steps);
            RoadTileNode newEnd = End.Rotate(steps);
            return new RoadConnection(newStart, newEnd);
        }
    }
}