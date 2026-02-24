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

            //이후 특정 시각적 처리
        }

        public bool TryDispatchVehicle(Vector2Int destNode) {
            if (WaitingVehicles.Count == 0) return false;
            int vehicleId = WaitingVehicles[0];
            Vehicle vehicle = VehicleMovementProcess.Instance.GetVehicle(vehicleId);
            //vehicle.Dispatch(this.EntranceCoordinate, destNode, this.IncomingLane);
            vehicle.Dispatch(this.OriginCoordinate, destNode);
            WaitingVehicles.RemoveAt(0);
            return true;
        }   

        public void RotateEntrance(TileDirection newDir) {
            if (DrivewayDirection == newDir) return;    //이미 그 방향과 동일하면 회전 X

            //삭제 요청
            RoadNetworkManager.Instance.MothballSystemRoad(EntranceLane, IncomingLane);

            //적용
            Vector2Int entranceNode = OriginCoordinate;
            Vector2Int newRoadCoord = entranceNode + TileUtils.GetDirectionVector(newDir);

            //도로 건설.
            RoadNetworkManager.Instance.BuildSystemRoad(entranceNode, newRoadCoord, out Lane eLane , out Lane iLane);
            EntranceLane = eLane;
            IncomingLane = iLane;

            //생성.
            var grid = MapManager.Instance._grid;
            if (grid.TryGetValue(entranceNode, out TileData newEntranceTile)) {
                newEntranceTile.ConnectLane(DrivewayDirection, EntranceLane);
            }
            if (grid.TryGetValue(newRoadCoord, out TileData newRoadTile)) {
                TileDirection newOpposite = TileUtils.GetOppositeDirection(DrivewayDirection);
                newRoadTile.ConnectLane(newOpposite, IncomingLane);
            }
        }
    }
}

