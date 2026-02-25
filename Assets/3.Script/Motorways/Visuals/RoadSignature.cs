using System.Collections.Generic;

namespace Motorways.Visuals {
    //이제 시그니처는 단순 마스크가 아니라, 구체적인 노드 연결의 집합입니다.
    public struct RoadSignature {
        public TileDirection RawMask;// 레거시 지원 및 빠른 기각(Quick Reject)용
        public TileDirection CanonicalMask;
        public int RotationSteps;
        public List<RoadConnection> Connections;

        public RoadSignature(TileDirection raw, TileDirection canonical, int rotationSteps, List<RoadConnection> connections) {
            RawMask = raw;
            CanonicalMask = canonical;
            RotationSteps = rotationSteps;
            Connections = connections;
        }
    }
}