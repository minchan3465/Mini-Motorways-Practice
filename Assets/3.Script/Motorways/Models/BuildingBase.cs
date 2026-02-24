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

        public Vector2Int OriginCoordinate { get; protected set; }
        public List<Vector2Int> OccupiedCoordinates { get; protected set; }

        public TileDirection DrivewayDirection { get; protected set; }
        public Lane EntranceLane { get; protected set; }
        public Lane IncomingLane { get; protected set; }
        public Vector2Int EntranceCoordinate => IncomingLane.EndNode;
        public Vector2Int RoadCoordinate => IncomingLane.StartNode;

        //----초기화---
        public virtual void Initialize(int groupIndex, Vector2Int originCoord, BuildingLayout layout) {
            GroupIndex = groupIndex;
            OriginCoordinate = originCoord;
            DrivewayDirection = layout.Driveways[0];

            Vector2Int entranceNode = originCoord + layout.LocalEntrance;
            Vector2Int roadCoord = entranceNode + Utils.TileUtils.GetDirectionVector(DrivewayDirection);

            RoadNetworkManager.Instance.BuildSystemRoad(entranceNode, roadCoord, out Lane eLane, out Lane iLane);
            EntranceLane = eLane;
            IncomingLane = iLane;
        }

        public abstract void OnVehicleArrived(int vehicleId);

        //--- 유탈. 입구 방향에 따른 도로타일 위치 계산. ---
        protected Vector2Int GetRoadCoordinate(Vector2Int origin, TileDirection dir, Vector2Int size) {
            switch (dir) {
                case TileDirection.North: return origin + new Vector2Int(0, size.y);
                case TileDirection.South: return origin + new Vector2Int(0, -1);
                case TileDirection.East: return origin + new Vector2Int(size.x, 0);
                case TileDirection.West: return origin + new Vector2Int(-1, 0);
                default: return origin;
            }
        }

        protected Vector2Int GetEntranceNode(Vector2Int origin, TileDirection dir, Vector2Int size) {
            switch (dir) {
                case TileDirection.North: return origin + new Vector2Int(0, size.y - 1);
                case TileDirection.South: return origin + new Vector2Int(0, 0);
                case TileDirection.East: return origin + new Vector2Int(size.x - 1, 0);
                case TileDirection.West: return origin + new Vector2Int(0, 0);
                default: return origin;
            }
        }
    }
}

