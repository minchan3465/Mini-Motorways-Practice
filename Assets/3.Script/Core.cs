using System;
using UnityEngine;

namespace Core.Data {
    public enum TileLogicType {
        Empty = 0,
        Obstacle = 1, // 강, 산 (건설 불가)
        Road = 2,     // DORODORO (도로)
        Supply = 10,  // 집 (출발지)
        Demand = 20,  // 상점 (목적지)
        Restricted = 99 // 건설 불가 구역 (No-build zone)
    }

    // 런타임에서 사용할 순수 데이터 구조체 (가볍게 유지)
    [Serializable]
    public struct CellData {
        public Vector2Int Coordinate;   //좌표
        public TileLogicType Type;
        public float Weight;            //확률 가중치 (Alpha Channel 값 등)
        public byte ConnectionMask;     //나중에 도로 연결했을 때, 정보를 위한 비트마스크 (8방향)

        public bool IsWalkable => Type.Equals(TileLogicType.Empty);
    }
}