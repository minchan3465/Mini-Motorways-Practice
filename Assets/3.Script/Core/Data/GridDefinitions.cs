using System;
using UnityEngine;

namespace Core.Data {

	public enum TileLogicType {
        Empty = 0,
        Obstacle = 1, //강, 산 (건설 불가)
        Road = 2,     //DORODORO (도로)
        Supply = 10,  //집 (출발지)
        Demand = 20,  //상점 (목적지)
        Entrance = 30, //건물의 입구
        Restricted = 99 //건설 불가 구역 (No-build zone)
    }

    [Serializable]
    public class CellData {
        public Vector2Int Coordinate;   //좌표
        public TileLogicType Type;

        //가중치 (건물 스폰용)
        public float HouseWeight;
        public float DestinationWeight;

        //--- 생성자 ---
        public CellData(Vector2Int coord) {
            Coordinate = coord;
            Type = TileLogicType.Empty;
            HouseWeight = 0f;
            DestinationWeight = 0f;
        }

        //--- 유틸 ---
        //건물이 들어설 수 있는 땅인가?
        public bool IsBuildable => Type == TileLogicType.Empty;

        // 도로를 깔 수 있는 땅인가? (빈 땅이거나, 이미 도로거나, 입구거나)
        public bool IsRoadBuildable => Type == TileLogicType.Empty || Type == TileLogicType.Road || Type == TileLogicType.Entrance;
    }
}
 
/* 비트마스크를 사용한 것에 대한 고찰 (현재 도로 시스템 개편으로, 사용하지 않음.)
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