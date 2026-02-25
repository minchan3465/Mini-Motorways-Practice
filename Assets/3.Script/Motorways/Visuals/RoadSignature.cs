using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Visuals {
    //개별 연결선을 정의하는 구조체
    public struct RoadConnection {
        public TileDirection Start;
        public TileDirection End;

        public RoadConnection(TileDirection start, TileDirection end) {
            Start = start;
            End = end;
        }

        //연결선을 지정된 횟수(1회당 45도)만큼 시계방향으로 회전시킨 새 연결선 반환
        public RoadConnection Rotate(int steps) {
            TileDirection newStart = RotateDirection(Start, steps);
            TileDirection newEnd = RotateDirection(End, steps);
            return new RoadConnection(newStart, newEnd);
        }

        private TileDirection RotateDirection(TileDirection dir, int steps) {
            if (dir == TileDirection.None || dir == TileDirection.All) return dir;
            byte mask = (byte)dir;
            steps = steps % 8;
            byte rotated = (byte)((mask << steps) | (mask >> (8 - steps)));
            return (TileDirection)rotated;
        }
    }

    //기존 서명 구조체에 연결선 리스트 추가
    public struct RoadSignature {
        public TileDirection RawMask;
        public TileDirection CanonicalMask;
        public int RotationSteps;
        public List<RoadConnection> Connections; //새로 추가된 필드

        public RoadSignature(TileDirection raw, TileDirection canonical, int rotationSteps, List<RoadConnection> connections) {
            RawMask = raw;
            CanonicalMask = canonical;
            RotationSteps = rotationSteps;
            Connections = connections;
        }
    }
}