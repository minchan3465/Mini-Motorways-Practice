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

        // 반대 방향 구하기
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

        // 방향 Enum을 0~7 사이의 인덱스로 변환 (TileData 배열 접근용)
        public static int GetDirectionIndex(TileDirection dir) {
            if (dir == TileDirection.None) return -1;
            return (int)Mathf.Log((int)dir, 2);
        }

        public static Vector2Int GetDirectionVector(TileDirection dir) {
            switch (dir) {
                case TileDirection.North: return new Vector2Int(0, 1);
                case TileDirection.South: return new Vector2Int(0, -1);
                case TileDirection.East: return new Vector2Int(1, 0);
                case TileDirection.West: return new Vector2Int(-1, 0);
                case TileDirection.NorthEast: return new Vector2Int(1, 1);
                case TileDirection.SouthEast: return new Vector2Int(1, -1);
                case TileDirection.SouthWest: return new Vector2Int(-1, -1);
                case TileDirection.NorthWest: return new Vector2Int(-1, 1);
                default: return Vector2Int.zero;
            }
        }

        public static void Shuffle<T>(this IList<T> list) {
            for (int i = list.Count - 1; i > 0; i--) {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static TileDirection RotateDirection(TileDirection dir, int steps) {
            if (dir == TileDirection.None || dir == TileDirection.All) return dir;
            byte mask = (byte)dir;

            // 비트 시프트를 이용한 8방향 회전
            int s = steps % 8;
            byte rotated = (byte)((mask << s) | (mask >> (8 - s)));
            return (TileDirection)rotated;
        }
    }
}
