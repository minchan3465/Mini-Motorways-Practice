using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {


    public class House : BuildingBase {
        public List<int> OwnVehicles { get; private set; }
        public List<int> WaitingVehicles { get; private set; }

        public override void Initiallize(int groupIndex, Vector2Int originCoord, TileDirection drivewayDir) {
            base.Initiallize(groupIndex, originCoord, drivewayDir);
            Type = BuildingType.House;

            OccupiedCoordinates = new List<Vector2Int> { originCoord };

            OwnVehicles = new List<int>();
            WaitingVehicles = new List<int>();
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
        }

        public int DispatcVehicle() {
            if(WaitingVehicles.Count == 0) {
                return -1;
            }

            int dispatchingVehicleId = WaitingVehicles[0];
            WaitingVehiclesIds.RemoeAt(0);

            return dispatchingVehicleId;
        }   
    }
}

