using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {
    using Motorways.Process;

    public class House : BuildingBase {
        public List<int> OwnVehicles { get; private set; } = new List<int>();
        public List<int> WaitingVehicles { get; private set; } = new List<int>();

        public override void Initialize(int groupIndex, Vector2Int originCoord, TileDirection drivewayDir) {
            base.Initialize(groupIndex, originCoord, drivewayDir);
            Type = BuildingType.House;

            OccupiedCoordinates = new List<Vector2Int> { originCoord };
        }

        public void RegisterVeicle(int vehicleId) {
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

        public bool TryDispatcVehicle(Vector2Int destNode) {
            if (WaitingVehicles.Count == 0) return false;
            int vehicleId = WaitingVehicles[0];
            Vehicle vehicle = VehicleMovementProcess.Instance.GetVehicle(vehicleId);
            vehicle.Dispatch(this.OriginCoordinate, destNode);
            WaitingVehicles.RemoveAt(0);
            return true;
        }   
    }
}

