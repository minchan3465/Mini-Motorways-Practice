using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Utils {
	public static class TileUtils {
        public static TileDirection GetDirection(Vector2Int from, Vector2Int to) {
            Vector2Int diff = to - from;

            if (diff.x == 0 && diff.y == 1) return TileDirection.North;
            if (diff.x == 1 && diff.y == 1) return TileDirection.NorthEast;
            if (diff.x == 1 && diff.y == 0) return TileDirection.East;
            if (diff.x == 1 && diff.y == -1) return TileDirection.SouthEast;
            if (diff.x == 0 && diff.y == -1) return TileDirection.South;
            if (diff.x == -1 && diff.y == -1) return TileDirection.SouthWest;
            if (diff.x == -1 && diff.y == 0) return TileDirection.West;
            if (diff.x == -1 && diff.y == 1) return TileDirection.NorthWest;

            return TileDirection.None;
        }

        //반대 방향 구하기
        public static TileDirection GetOppositeDirection(TileDirection dir) {
            switch (dir) {
                case TileDirection.North: return TileDirection.South;
                case TileDirection.NorthEast: return TileDirection.SouthWest;
                case TileDirection.East: return TileDirection.West;
                case TileDirection.SouthEast: return TileDirection.NorthWest;
                case TileDirection.South: return TileDirection.North;
                case TileDirection.SouthWest: return TileDirection.NorthEast;
                case TileDirection.West: return TileDirection.East;
                case TileDirection.NorthWest: return TileDirection.SouthEast;
                default: return TileDirection.None;
            }
        }

        // 방향 Enum을 정수 인덱스(0~7)로 변환 (TileData 배열 접근용)
        public static int GetDirectionIndex(TileDirection dir) {
            // 사용자가 제공한 Math.Log 방식 활용 (단, 0인 경우 예외처리 필요)
            if (dir == TileDirection.None) return -1;
            return (int)Mathf.Log((int)dir, 2);
        }
    }
}

