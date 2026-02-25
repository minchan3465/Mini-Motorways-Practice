using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Motorways.Visuals {
	using Motorways.Utils; // TileDirection 사용을 위해

	[Serializable]
	public struct RoadTileNode : IEquatable<RoadTileNode> {
		public TileDirection Direction;
		public RoadType Type;
		public int MotorwayId;

		public RoadTileNode(TileDirection direction, RoadType type, int motorwayId = -1) {
			Direction = direction;
			Type = type;
			MotorwayId = motorwayId;
		}

		public RoadTileNode Rotate(int steps) {
			TileDirection newDir = TileUtils.RotateDirection(Direction, steps);
			return new RoadTileNode(newDir, Type, MotorwayId);
		}

		//IEquatable 구현 (Dictionary 키나 비교 연산 최적화)
		public bool Equals(RoadTileNode other) { return Direction == other.Direction && Type == other.Type && MotorwayId == other.MotorwayId; }
		public override bool Equals(object obj) { return obj is RoadTileNode other && Equals(other); }
		public override int GetHashCode() { return HashCode.Combine(Direction, Type, MotorwayId); }
		public static bool operator ==(RoadTileNode left, RoadTileNode right) { return left.Equals(right); }
		public static bool operator !=(RoadTileNode left, RoadTileNode right) { return !left.Equals(right); }
	}
}
