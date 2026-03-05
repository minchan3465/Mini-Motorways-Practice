using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
    ///<summary>
    ///도로의 상태를 나타내는 열거형
    ///</summary>
    public enum RoadState {
        None,
        Planned,    //플레이어가 드래그 중인 유령 도로
        Pending,    //건설 대기 중
        Active,     //활성화된 도로
        Mothballed, //삭제 대기 중 (차량이 지나갈 때까지 유지)
    }

    ///<summary>
    ///두 노드 사이의 단방향 연결을 나타내는 클래스
    ///</summary>
    [System.Serializable]
    public class Lane {
        public int Id { get; private set; }
        private static int _nextId = 0;

        //연결 정보
        public Vector2Int StartNode;
        public Vector2Int EndNode;

        public RoadState State = RoadState.Active;

        //차량 관리
        public List<int> VehiclesOnLane = new List<int>();            //현재 도로 위에 있는 차량들
        public HashSet<int> InboundVehicles = new HashSet<int>();   //이 도로로 진입 예정인 차량들

        public float Length;    //도로의 길이 (비용 계산용)
        public float BaseCost => Length;

        public Utils.Spline.BezierSpline PathSpline { get; private set; }

        //controlPoint를 받는 생성자로 원복
        public Lane(Vector2Int start, Vector2Int end, Vector2? controlPoint = null) {
            Id = _nextId++;
            StartNode = start;
            EndNode = end;

            //월드 좌표 계산 (타일 중심)
            Vector3 pStart = new Vector3(start.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, start.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
            Vector3 pEnd = new Vector3(end.x * MapSettings.TILE_SIZE + MapSettings.HALF_TILE, 0, end.y * MapSettings.TILE_SIZE + MapSettings.HALF_TILE);
            
            if (controlPoint.HasValue) {
                //곡선: 주어진 제어점을 사용
                Vector3 pMid = new Vector3(controlPoint.Value.x, 0, controlPoint.Value.y);
                PathSpline = new Utils.Spline.BezierSpline(pStart, pMid, pEnd);
            } else {
                //직선: 시작과 끝의 중간점을 제어점으로 사용
                Vector3 pMid = Vector3.Lerp(pStart, pEnd, 0.5f);
                PathSpline = new Utils.Spline.BezierSpline(pStart, pMid, pEnd);
            }

            Length = PathSpline.Length(10);
        }

        ///<summary>
        ///경로 탐색 시 가중치 계산
        ///</summary>
        public float GetPathfindingCost() {
            if (State == RoadState.Mothballed) return 100000f; //삭제 중인 도로는 가급적 피함
            return BaseCost;
        }

        ///<summary>
        ///도로를 완전히 삭제해도 되는지 확인 (올라와 있거나 진입 예정인 차가 없어야 함)
        ///</summary>
        public bool CanRelease() {
            return VehiclesOnLane.Count == 0 && InboundVehicles.Count == 0;
        }

        public void Reserve(int vehicleID) {
            if (!InboundVehicles.Contains(vehicleID)) InboundVehicles.Add(vehicleID);
        }

        public void CancelReservation(int vehicleId) {
            if (InboundVehicles.Contains(vehicleId)) InboundVehicles.Remove(vehicleId);
        }

        public void Enter(int vehicleId) {
            CancelReservation(vehicleId);
            if (!VehiclesOnLane.Contains(vehicleId)) VehiclesOnLane.Add(vehicleId);
        }

        public void Exit(int vehicleId) {
            VehiclesOnLane.Remove(vehicleId);
        }
    }
}