using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
    [Flags]
    public enum CornerDiagonalType {
        None = 0,
        SW_to_NE = 1 << 0,
        NW_to_SE = 1 << 1
    }

    public class CornerData {
        public Vector2Int coordinate { get; private set; }
        public CornerDiagonalType activeDiagonals { get; private set; }
        
        // 각 대각선의 상태를 저장 (Index: 0 = SW_to_NE, 1 = NW_to_SE)
        public RoadState[] states = new RoadState[2];
        
        public float creationTime { get; private set; }

        public CornerData(Vector2Int coord) {
            coordinate = coord;
            activeDiagonals = CornerDiagonalType.None;
            creationTime = -1f;
            states[0] = RoadState.None;
            states[1] = RoadState.None;
        }

        public void AddDiagonal(CornerDiagonalType type, RoadState state = RoadState.Active) {
            activeDiagonals |= type;
            SetState(type, state);
            if (creationTime < 0f) creationTime = Time.time;
        }

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
