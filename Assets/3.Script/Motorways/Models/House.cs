using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {
    using Motorways.Utils;
    using Motorways.Process;
    using Motorways.Managers;

    public class House : BuildingBase {
        public List<int> OwnVehicles { get; private set; } = new List<int>();
        public List<int> WaitingVehicles { get; private set; } = new List<int>();

        public override void Initialize(int groupIndex, Vector2Int originCoord, BuildingLayout layout) {
            base.Initialize(groupIndex, originCoord, layout);
            Type = BuildingType.House;

            OccupiedCoordinates = new List<Vector2Int> { originCoord };
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

        public bool TryDispatchVehicle(Vector2Int destNode) {
            if (WaitingVehicles.Count == 0) return false;
            int vehicleId = WaitingVehicles[0];
            Vehicle vehicle = VehicleMovementProcess.Instance.GetVehicle(vehicleId);
            vehicle.Dispatch(this.OriginCoordinate, destNode);
            WaitingVehicles.RemoveAt(0);
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
