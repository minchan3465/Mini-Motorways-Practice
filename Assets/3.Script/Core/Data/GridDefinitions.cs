using System;
using UnityEngine;

namespace Core.Data {

	public enum TileLogicType {
        Empty = 0,
        Obstacle = 1, // 강, 산 (건설 불가)
        Road = 2,     // DORODORO (도로)
        Supply = 10,  // 집 (출발지)
        Demand = 20,  // 상점 (목적지)
        Entrance = 30, // 건물의 입구
        Restricted = 99 // 건설 불가 구역 (No-build zone)
    }

    [Flags]
    public enum RoadDirection : byte {
        None = 0,
        North = 1 << 0, // 1
        East = 1 << 1, // 2
        South = 1 << 2, // 4
        West = 1 << 3, // 8

        NorthEast = 1 << 4, // 16
        SouthEast = 1 << 5, // 32
        SouthWest = 1 << 6, // 64
        NorthWest = 1 << 7  // 128
    }

    /* 비트마스크를 사용한 것에 대한 고찰
    - 왜 비트마스크로 쓰냐?
    단순히 North가 활성화를 bool값으로 체크를 하는 방식은 메모리 자리를 몇개씩 만드는것이다.
    이는 접근할때마다 해당 주소값으로 이동하여 체크하기 때문에, 이 방식보다 정말 미세하게나마라도 느리다.
    도로를 빠르게 쭉 깔아야하는 전략게임이니, 가능하면 연산은 경량화를 하는게 좋다고 생각.

    또한 어디서봤는지 기억이 나지 않는데,
    플레이어의 상태이상을 체크하는 방식으로도 비트마스크를 쓴다고 한다.
    예를 들어 0001 = 독, 0100 = 기절 이라는 상태이상으로 한다면
    0101 = 독과 기절이 걸린 상태라고 체크하는 방법으로 한다고 한다.

    이 방법이 흥미로웠고, 콘솔프로젝트에서 Slay the Spire를 만들어본적이 있는데 
    턴 종료, 카드 사용, 적턴 같은 상황에 그때마다 상태 체크를 하는게 너무 귀찮았다.
    
    따라서 이 방법을 채택해본듯.
    */
    [Serializable]
    public class CellData {
        public Vector2Int Coordinate;   //좌표
        public TileLogicType Type;
        public float HouseWeight;            //확률 가중치 (Alpha Channel 값 등)
        public float DestinationWeight;            //확률 가중치 (Alpha Channel 값 등)
        public RoadDirection ConnectionMask;     //나중에 도로 연결했을 때, 정보를 위한 비트마스크 (8방향)

        public bool IsPendingRemoval;

        // Pending 상태라도 이미 경로를 잡은 차는 지나갈 수 있어야 함
        // 단, 새로운 경로 탐색에서는 장애물 취급해야 함 (Pathfinder 수정 필요 없음, Type은 Road로 유지하되 IsBuildable 체크에서 거름)
        public bool IsWalkable => Type.Equals(TileLogicType.Empty);
    }
}