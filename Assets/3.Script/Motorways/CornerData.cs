using System;
using UnityEngine;

namespace Motorways {
    /// <summary>
    /// 대각선 도로를 구성하는 방향 타입 (플래그 사용 가능)
    /// </summary>
    [Flags]
    public enum CornerDiagonalType {
        None = 0,
        SW_to_NE = 1 << 0, // '/' 형태 대각선
        NW_to_SE = 1 << 1  // '\' 형태 대각선
    }

    /// <summary>
    /// 타일 우측 하단 모서리(Corner)를 기준으로 생성되는 대각선 도로 데이터를 저장합니다.
    /// </summary>
    public class CornerData {
        public Vector2Int coordinate { get; private set; }
        public CornerDiagonalType activeDiagonals { get; private set; }
        
        // 두 종류의 대각선 상태(Active/Mothballed)를 별도로 관리
        public RoadState[] states = new RoadState[2];
        
        public float creationTime { get; private set; }

        public CornerData(Vector2Int coord) {
            coordinate = coord;
            activeDiagonals = CornerDiagonalType.None;
            creationTime = -1f;
            states[0] = RoadState.None;
            states[1] = RoadState.None;
        }

        /// <summary>
        /// 대각선 연결 추가 및 초기 상태 설정
        /// </summary>
        public void AddDiagonal(CornerDiagonalType type, RoadState state = RoadState.Active) {
            activeDiagonals |= type;
            SetState(type, state);
            if (creationTime < 0f) creationTime = Time.time;
        }

        /// <summary>
        /// 대각선 연결 삭제
        /// </summary>
        public void RemoveDiagonal(CornerDiagonalType type) {
            activeDiagonals &= ~type;
            SetState(type, RoadState.None);
            if (activeDiagonals == CornerDiagonalType.None) creationTime = -1f;
        }

        public void SetState(CornerDiagonalType type, RoadState state) {
            if (type == CornerDiagonalType.SW_to_NE) states[0] = state;
            else if (type == CornerDiagonalType.NW_to_SE) states[1] = state;
        }

        public RoadState GetState(CornerDiagonalType type) {
            if (type == CornerDiagonalType.SW_to_NE) return states[0];
            if (type == CornerDiagonalType.NW_to_SE) return states[1];
            return RoadState.None;
        }

        public bool HasAnyDiagonal => activeDiagonals != CornerDiagonalType.None;
        public bool HasDiagonal(CornerDiagonalType type) => (activeDiagonals & type) == type;
    }
}
