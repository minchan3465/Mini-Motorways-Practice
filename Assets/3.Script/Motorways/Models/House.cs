using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {
    using Motorways.Utils;
    using Motorways.Process;
    using Motorways.Managers;

    public class House : BuildingBase {
        public List<int> OwnVehicles { get; private set; } = new List<int>();
        public List<int> WaitingVehicles { get; private set; } = new List<int>();
        
        private float _nextDispatchTime = 0f;
        private const float DISPATCH_COOLDOWN = 1.0f; 

        public override void Initialize(int groupIndex, Vector2Int originCoord, BuildingLayout layout) {
            base.Initialize(groupIndex, originCoord, layout);
            Type = BuildingType.House;
            OccupiedCoordinates = new List<Vector2Int> { originCoord };
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

        //Destination 객체를 직접 받도록 변경
        public bool TryDispatchVehicle(Destination dest) {
            float currentTime = ClockProcess.Instance != null ? ClockProcess.Instance.Model.Time : Time.time;
            if (currentTime < _nextDispatchTime) return false;

            if (WaitingVehicles.Count == 0) return false;
            
            int vehicleId = WaitingVehicles[0];
            WaitingVehicles.RemoveAt(0);

            Vehicle vehicle = VehicleMovementProcess.Instance.GetVehicle(vehicleId);
            
            _nextDispatchTime = currentTime + DISPATCH_COOLDOWN;

            //차량에게 직접 목적지 객체를 할당하며 출발 명령
            vehicle.Dispatch(dest); 
            
            return true;
        }   

        public void RotateEntrance(TileDirection newDir) {
            if (DrivewayDirection == newDir) return;
            RoadNetworkManager.Instance.MothballSystemRoad(EntranceLane, IncomingLane);
            DrivewayDirection = newDir;
            Vector2Int entranceNode = OriginCoordinate;
            Vector2Int newRoadCoord = entranceNode + TileUtils.GetDirectionVector(DrivewayDirection);
            RoadNetworkManager.Instance.BuildSystemRoad(entranceNode, newRoadCoord, out Lane eLane , out Lane iLane);
            EntranceLane = eLane;
            IncomingLane = iLane;
        }
    }
}
