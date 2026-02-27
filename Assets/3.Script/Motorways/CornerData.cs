using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
    // 코너를 관통하는 대각선 도로의 형태를 비트마스크로 관리합니다.
    [Flags]
    public enum CornerDiagonalType {
        None = 0,
        SW_to_NE = 1 << 0, // '/' 모양 대각선 (Tile(x,y) <-> Tile(x+1, y+1) 연결)
        NW_to_SE = 1 << 1  // '\' 모양 대각선 (Tile(x, y+1) <-> Tile(x+1, y) 연결)
    }

    public class CornerData {
        public Vector2Int coordinate { get; private set; }
        public CornerDiagonalType activeDiagonals { get; private set; }
        public float creationTime { get; private set; }

        public CornerData(Vector2Int coord) {
            coordinate = coord;
            activeDiagonals = CornerDiagonalType.None;
            creationTime = -1f;
        }

        // 대각선 도로 추가
        public void AddDiagonal(CornerDiagonalType type) {
            activeDiagonals |= type;
            if (creationTime < 0f) creationTime = Time.time;
        }

        // 대각선 도로 제거
        public void RemoveDiagonal(CornerDiagonalType type) {
            activeDiagonals &= ~type;
            creationTime = -1f;
        }

        // 렌더링 시 코너 메쉬를 그려야 하는지 판별용
        public bool HasAnyDiagonal => activeDiagonals != CornerDiagonalType.None;
        public bool HasDiagonal(CornerDiagonalType type) => (activeDiagonals & type) == type;
    }
}