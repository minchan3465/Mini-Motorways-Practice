using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways {
	using Motorways.Utils;

	public enum RoadType {
		TwoLane,    // 일반 도로
		Driveway,   // 집/목적지 진입로 (짧은 곡선)
		Roundabout, // 회전교차로 (추후 확장용)
		Motorway    // 고속도로 (추후 확장용)
	}

	[Serializable]
	public struct RoadTileNode : IEquatable<RoadTileNode> {
		public TileDirection direction;
		public RoadType type;

		public RoadTileNode(TileDirection direction, RoadType type) {
			this.direction = direction;
			this.type = type;
		}

		public RoadTileNode GetRotatedNode(int steps) {
			return new RoadTileNode(TileUtils.RotateDirection(this.direction, steps), this.type);
		}

		//--- 비교 연산자 및 오버라이드 ---
		public bool Equals(RoadTileNode other) {
			return direction == other.direction && type == other.type;
		}

		public override bool Equals(object obj) {
			return obj is RoadTileNode other && Equals(other);
		}

		public override int GetHashCode() {
			return (int)direction * 397 ^ (int)type;
		}

		public override string ToString() => $"{direction}_{type}";
		public static bool operator ==(RoadTileNode left, RoadTileNode right) => left.Equals(right);
		public static bool operator !=(RoadTileNode left, RoadTileNode right) => !left.Equals(right);
	}
}
