using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models { 
    public enum BuildingType {
        None = 0,
        House = 1,
        Destination = 2
    }

    public abstract class BuildingBase {
        public BuildingType Type { get; protected set; }
        public int GroupIndex { get; protected set; }

        public Vector2Int OriginCoordinate { get; protected set; }
        public List<Vector2Int> OccupiedCoordinates { get; protected set; }

        public TileDirection DrivewayDirection { get; protected set; }
        public Lane EntranceLane { get; protected set; }

        //----√ ±‚»≠---
        public virtual void Initiallize(int groupIndex, Vector2Int originCoord, TileDirection drivewayDir) {
            GroupIndex = groupIndex;
            OriginCoordinate = originCoord;
            DrivewayDirection = drivewayDir;
        }

        public abstract void OnVehicleArrived(int vehicleId);
    }
}

