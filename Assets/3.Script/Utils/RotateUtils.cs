using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Utils {
	public static class RotateUtils {
		public static Vector2Int RotateSize(Vector2Int size, int rotationIndex) {
			//Rotation Index
			//0 = 0
			//1 = 90
			//2 = 180
			//3 = 270

			if (rotationIndex % 2 == 1) {
				return new Vector2Int(size.y, size.x);
			}
			return size;
		}

		public static TileDirection RotateDirection(TileDirection dir, int rotationIndex) {
			TileDirection current = dir;
			for (int i = 0; i < rotationIndex; i++) {
				current = RotateDir90(current);
			}
			return current;
		}

		private static TileDirection RotateDir90(TileDirection dir) {
			switch (dir) {
				case TileDirection.North: return TileDirection.East;
				case TileDirection.East: return TileDirection.South;
				case TileDirection.South: return TileDirection.West;
				case TileDirection.West: return TileDirection.North;
				default: return dir;
			}
		}


		public static Vector2Int RotatePoint(Vector2Int point, Vector2Int originalSize, int rotationIndex) {
			int x = point.x;
			int y = point.y;
			int w = originalSize.x;
			int h = originalSize.y;

			for (int i = 0; i < rotationIndex; i++) {
				int newX = y;
				int newY = w - 1 - x;

				x = newX;
				y = newY;

				// 크기 스왑
				int temp = w;
				w = h;
				h = temp;
			}
			return new Vector2Int(x, y);
		}
	}
}
