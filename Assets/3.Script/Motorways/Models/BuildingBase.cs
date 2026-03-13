using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Models {
    using Motorways.Managers;

    public enum BuildingType {
        None = 0,
        House = 1,
        Destination = 2
    }

    public abstract class BuildingBase {
        public BuildingType Type { get; protected set; }
        public int GroupIndex { get; protected set; }

        public Vector2Int OriginCoordinate { get; protected set; } //입구(Entrance) 타일 좌표
        public List<Vector2Int> OccupiedCoordinates { get; protected set; }

        public TileDirection DrivewayDirection { get; protected set; }
        public Lane EntranceLane { get; protected set; }
        public Lane IncomingLane { get; protected set; }
        public Vector2Int EntranceCoordinate => OriginCoordinate;
        public Vector2Int RoadCoordinate => OriginCoordinate + Utils.TileUtils.GetDirectionVector(DrivewayDirection);

        //---- 초기화 ---
        public virtual void Initialize(int groupIndex, Vector2Int entranceCoord, BuildingLayout layout) {
            GroupIndex = groupIndex;
            OriginCoordinate = entranceCoord;
            DrivewayDirection = layout.Driveways[0];

            Vector2Int entranceNode = entranceCoord;
            Vector2Int roadCoord = EntranceCoordinate + Utils.TileUtils.GetDirectionVector(DrivewayDirection);

            //시스템 도로 건설 (입구 <-> 도로)
            RoadNetworkManager.Instance.BuildSystemRoad(entranceNode, roadCoord, out Lane eLane, out Lane iLane);
            EntranceLane = eLane;
            IncomingLane = iLane;

            //점유 타일 계산 (입구 타일 기준 상대적 역산)
            OccupiedCoordinates = new List<Vector2Int>();
            Vector2Int bottomLeft = entranceCoord - layout.LocalEntrance;
            for (int x = 0; x < layout.Footprint.x; x++) {
                for (int y = 0; y < layout.Footprint.y; y++) {
                    OccupiedCoordinates.Add(bottomLeft + new Vector2Int(x, y));
                }
            }
        }

        public abstract void OnVehicleArrived(int vehicleId);
    }
}
