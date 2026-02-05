using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils {
	using Core.Data;

	public static class DirUtiles {
		public static Vector2Int GetDirVector(RoadDirection dir) {
			if (dir == RoadDirection.North) return new Vector2Int(0, 1);
			if (dir == RoadDirection.South) return new Vector2Int(0, -1);
			if (dir == RoadDirection.East) return new Vector2Int(1, 0);
			if (dir == RoadDirection.West) return new Vector2Int(-1, 0);

			if (dir.Equals(RoadDirection.NorthEast)) return new Vector2Int(1, 1);
			if (dir.Equals(RoadDirection.NorthWest)) return new Vector2Int(-1, 1);
			if (dir.Equals(RoadDirection.SouthEast)) return new Vector2Int(1, -1);
			if (dir.Equals(RoadDirection.SouthWest)) return new Vector2Int(-1, -1);
			return Vector2Int.zero;
		}
		public static RoadDirection GetVectorDir(Vector2Int dir) {
			if (dir.x == 0 && dir.y == 1) return RoadDirection.North;
			if (dir.x == 0 && dir.y == -1) return RoadDirection.South;
			if (dir.x == 1 && dir.y == 0) return RoadDirection.East;
			if (dir.x == -1 && dir.y == 0) return RoadDirection.West;
			if (dir.x == 1 && dir.y == 1) return RoadDirection.NorthEast;
			if (dir.x == 1 && dir.y == -1) return RoadDirection.SouthEast;
			if (dir.x == -1 && dir.y == -1) return RoadDirection.SouthWest;
			if (dir.x == -1 && dir.y == 1) return RoadDirection.NorthWest;
			return RoadDirection.None;
		}
	}
}

