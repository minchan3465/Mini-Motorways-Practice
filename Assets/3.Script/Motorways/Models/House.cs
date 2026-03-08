using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {
    using Motorways.Utils;
    using Motorways.Process;
    using Motorways.Managers;

    public class House : BuildingBase {
        public List<int> OwnVehicles { get; private set; } = new List<int>();
        public List<int> WaitingVehicles { get; private set; } = new List<int>();
        
        // 출발 딜레이 중인 차량 관리 (차량ID, 남은 딜레이 시간)
        private Dictionary<int, float> _departingVehicles = new Dictionary<int, float>();
        private const float DISPATCH_DELAY = 0.8f; // 원작 스타일의 출발 딜레이

        // 배차 사이의 최소 쿨다운 (한 대 보낸 후 다음 차 보낼 때까지의 시간)
        private float _nextDispatchTime = 0f;
        private const float DISPATCH_COOLDOWN = 1.0f; // 1.5 -> 1.0으로 단축

        public override void Initialize(int groupIndex, Vector2Int originCoord, BuildingLayout layout) {
            base.Initialize(groupIndex, originCoord, layout);
            Type = BuildingType.House;

            OccupiedCoordinates = new List<Vector2Int> { originCoord };
            
            // DispatchProcess에 등록하여 Tick이 돌게 함
            DispatchProcess.Instance.RegisterHouse(this);
        }

        public void RegisterVehicle(int vehicleId) {
            if(!OwnVehicles.Contains(vehicleId)) {
                OwnVehicles.Add(vehicleId);
                WaitingVehicles.Add(vehicleId);
            }
        }

        public override void OnVehicleArrived(int vehicleId) {
            if (OwnVehicles.Contains(vehicleId) && !WaitingVehicles.Contains(vehicleId)) {
                WaitingVehicles.Add(vehicleId);
            }
        }

        public void Tick(float dt) {
            // 출발 딜레이 타이머 처리
            List<int> toDispatch = new List<int>();
            List<int> keys = new List<int>(_departingVehicles.Keys);

            foreach (int id in keys) {
                _departingVehicles[id] -= dt;
                if (_departingVehicles[id] <= 0) {
                    toDispatch.Add(id);
                }
            }

            foreach (int id in toDispatch) {
                // 실제 출발 처리는 현재 TryDispatchVehicle에서 간소화하여 처리 중이므로 
                // 향후 딜레이 시점의 정교한 분리가 필요할 때 사용 가능합니다.
            }
        }

        public bool TryDispatchVehicle(Vector2Int destNode) {
            // 쿨다운 체크: 아직 다음 배차 시간이 되지 않았다면 실패 처리
            float currentTime = ClockProcess.Instance != null ? ClockProcess.Instance.Model.Time : Time.time;
            if (currentTime < _nextDispatchTime) return false;

            if (WaitingVehicles.Count == 0) return false;
            
            int vehicleId = WaitingVehicles[0];
            WaitingVehicles.RemoveAt(0);

            Vehicle vehicle = VehicleMovementProcess.Instance.GetVehicle(vehicleId);
            
            // 배차 성공 시 다음 가능한 배차 시간 설정 (쿨다운 적용)
            _nextDispatchTime = currentTime + DISPATCH_COOLDOWN;

            // [수정] 집 위치를 굳이 넘기지 않고 목적지만 설정합니다.
            vehicle.Dispatch(destNode); 
            
            return true;
        }   

        public void RotateEntrance(TileDirection newDir) {
            if (DrivewayDirection == newDir) return;    // 이미 같은 방향이면 회전하지 않음

            // 기존 입구 도로 삭제 대기 상태로 전환
            RoadNetworkManager.Instance.MothballSystemRoad(EntranceLane, IncomingLane);

            // 방향 정보 업데이트
            DrivewayDirection = newDir;

            // 새 위치 계산
            Vector2Int entranceNode = OriginCoordinate;
            Vector2Int newRoadCoord = entranceNode + TileUtils.GetDirectionVector(DrivewayDirection);

            // 새 시스템 도로 건설
            RoadNetworkManager.Instance.BuildSystemRoad(entranceNode, newRoadCoord, out Lane eLane , out Lane iLane);
            EntranceLane = eLane;
            IncomingLane = iLane;
        }
    }
}
