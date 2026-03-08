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

        // 두 차선(In->Out)이 교차로 내에서 충돌하는지 여부 판정 (원작 로직 기반)
        public static bool ConnectionsIntersect(TileDirection in1, TileDirection out1, TileDirection in2, TileDirection out2) {
            // 1. 같은 출구로 나가는 경우 충돌
            if (out1 == out2) return true;
            // 2. 같은 입구에서 들어오는 경우 (분기) 충돌 안함
            if (in1 == in2) return false;
            // 3. 서로 반대 방향으로 스쳐지나가는 경우 충돌 안함
            if (in1 == out2 && out1 == in2) return false;

            // 4. 기하학적 교차 판정 (시계방향 순서 이용)
            // 방향을 0~7 숫자로 변환
            int i1 = GetDirectionIndex(in1);
            int o1 = GetDirectionIndex(out1);
            int i2 = GetDirectionIndex(in2);
            int o2 = GetDirectionIndex(out2);

            if (i1 == -1 || o1 == -1 || i2 == -1 || o2 == -1) return false;

            // 각 경로를 (Start, End) 구간으로 보고, 구간이 겹치는지 확인
            // 단순화를 위해: 한 경로의 한 끝점이 다른 경로의 양 끝점 사이에 '하나만' 있으면 교차함.
            return IsBetween(i2, i1, o1) != IsBetween(o2, i1, o1);
        }

        private static bool IsBetween(int target, int a, int b) {
            if (a < b) return target > a && target < b;
            else return target > a || target < b; // 원형 순환 고려
        }
    }
}
