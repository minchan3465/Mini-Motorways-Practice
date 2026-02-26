using System;
using System.Collections.Generic;
using FixMath;
using Motorways.Models;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000440 RID: 1088
	public static class TileUtilities
	{
		// Token: 0x06001AFD RID: 6909 RVA: 0x00062E03 File Offset: 0x00061003
		public static Vector2Int GetAdjacentCoordinates(Vector2Int originCoordinates, TileDirection direction)
		{
			return originCoordinates + TileUtilities.DirectionToTileAdjacencyOffset[(int)direction];
		}

		// Token: 0x06001AFE RID: 6910 RVA: 0x00062E18 File Offset: 0x00061018
		public static TileDirection GetDirectionBetweenAdjacentCoordinates(Vector2Int originCoordinates, Vector2Int adjacentCoordinates)
		{
			Vector2Int adjacencyOffset = adjacentCoordinates - originCoordinates;
			for (int adjacencyOffsetIndex = 0; adjacencyOffsetIndex < TileUtilities.DirectionToTileAdjacencyOffset.Length; adjacencyOffsetIndex++)
			{
				if (TileUtilities.DirectionToTileAdjacencyOffset[adjacencyOffsetIndex] == adjacencyOffset)
				{
					return (TileDirection)adjacencyOffsetIndex;
				}
			}
			return TileDirection.None;
		}

		// Token: 0x06001AFF RID: 6911 RVA: 0x00062E55 File Offset: 0x00061055
		public static TileDirection GetRotatedDirection(TileDirection direction, RoadTileRotation rotation)
		{
			return TileUtilities.GetRotatedDirection(direction, (int)(rotation * RoadTileRotation.HalfTurn));
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x00062E60 File Offset: 0x00061060
		public static TileDirection GetRotatedDirection(TileDirection direction, int rotationCount)
		{
			if (direction == TileDirection.None)
			{
				return TileDirection.None;
			}
			return (TileDirection)TileUtilities.Wrap((int)(direction + rotationCount), 8);
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x00062E74 File Offset: 0x00061074
		public static Vector2Fixed GetRotatedVector(Vector2Fixed original, RoadTileRotation rotation)
		{
			switch (rotation)
			{
			case RoadTileRotation.None:
				return original;
			case RoadTileRotation.QuarterTurn:
				return new Vector2Fixed(original.y, -original.x);
			case RoadTileRotation.HalfTurn:
				return new Vector2Fixed(-original.x, -original.y);
			case RoadTileRotation.ThreeQuarterTurn:
				return new Vector2Fixed(-original.y, original.x);
			default:
				return original;
			}
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x00062EE8 File Offset: 0x000610E8
		public static Vector2 GetRotatedVector(Vector2 original, RoadTileRotation rotation)
		{
			switch (rotation)
			{
			case RoadTileRotation.None:
				return original;
			case RoadTileRotation.QuarterTurn:
				return new Vector2(original.y, -original.x);
			case RoadTileRotation.HalfTurn:
				return new Vector2(-original.x, -original.y);
			case RoadTileRotation.ThreeQuarterTurn:
				return new Vector2(-original.y, original.x);
			default:
				return original;
			}
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x00062F4A File Offset: 0x0006114A
		public static TileDirection GetOppositeDirection(TileDirection direction)
		{
			return TileUtilities.GetRotatedDirection(direction, RoadTileRotation.HalfTurn);
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x00062F53 File Offset: 0x00061153
		public static Vector2Int GetAdjacencyOffsetForDirection(TileDirection direction)
		{
			if (direction == TileDirection.None)
			{
				return Vector2Int.zero;
			}
			return TileUtilities.DirectionToTileAdjacencyOffset[(int)direction];
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x00062F6A File Offset: 0x0006116A
		public static Vector2 GetVectorForDirection(TileDirection direction)
		{
			if (direction == TileDirection.None)
			{
				return Vector2.zero;
			}
			return TileUtilities.DirectionToVector[(int)direction];
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x00062F81 File Offset: 0x00061181
		public static Vector2Fixed GetVectorFixedForDirection(TileDirection direction)
		{
			if (direction == TileDirection.None)
			{
				return Vector2Fixed.zero;
			}
			return TileUtilities.DirectionToVectorFixed[(int)direction];
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x00062F98 File Offset: 0x00061198
		public static Vector2Fixed GetTileEdgeForDirection(TileDirection direction)
		{
			if (direction == TileDirection.None)
			{
				return Vector2Fixed.zero;
			}
			return TileUtilities.DirectionToTileEdgeVectorFixed[(int)direction];
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x00062FB0 File Offset: 0x000611B0
		public static int GetDistanceBetweenDirections(TileDirection start, TileDirection end)
		{
			int difference = Math.Max((int)start, (int)end) - Math.Min((int)start, (int)end);
			if (difference > 4)
			{
				return 8 - difference;
			}
			return difference;
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x00062FDC File Offset: 0x000611DC
		public static TileDirection GetClosestDirection(Vector2 direction)
		{
			int closestIndex = -1;
			float closestDot = float.MinValue;
			for (int directionIndex = 0; directionIndex < TileUtilities.DirectionToVector.Length; directionIndex++)
			{
				float currentDot = Vector2.Dot(TileUtilities.DirectionToVector[directionIndex], direction);
				if (currentDot > closestDot)
				{
					closestIndex = directionIndex;
					closestDot = currentDot;
				}
			}
			return (TileDirection)closestIndex;
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x00063020 File Offset: 0x00061220
		public static TileDirection GetClosestDirection(Vector2Fixed direction)
		{
			int closestIndex = -1;
			Fix64 closestDot = -Fix64.One;
			for (int directionIndex = 0; directionIndex < TileUtilities.DirectionToVectorFixed.Length; directionIndex++)
			{
				Fix64 currentDot = Vector2Fixed.Dot(TileUtilities.DirectionToVectorFixed[directionIndex], direction);
				if (currentDot > closestDot)
				{
					closestIndex = directionIndex;
					closestDot = currentDot;
				}
			}
			return (TileDirection)closestIndex;
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x0006306C File Offset: 0x0006126C
		public static IEnumerable<TileDirection> GetRadiatedDirections(TileDirection startDirection, bool preferClockwise = true)
		{
			yield return startDirection;
			int num;
			for (int i = 1; i < 4; i = num + 1)
			{
				yield return TileUtilities.GetRotatedDirection(startDirection, i * (preferClockwise ? 1 : -1));
				yield return TileUtilities.GetRotatedDirection(startDirection, i * (preferClockwise ? -1 : 1));
				num = i;
			}
			yield return TileUtilities.GetOppositeDirection(startDirection);
			yield break;
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x00063083 File Offset: 0x00061283
		public static Fix64 GetRotationAngle(RoadTileRotation rotation)
		{
			return new Fix64((int)rotation) * (Fix64)90L;
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x00063098 File Offset: 0x00061298
		private static RoadTileRotation GetRotatedRotation(RoadTileRotation startingRotation, int rotationsNeeded)
		{
			return (RoadTileRotation)TileUtilities.Wrap((int)(startingRotation + rotationsNeeded), 4);
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x000630A3 File Offset: 0x000612A3
		public static RoadTileRotation AddRotation(RoadTileRotation original, RoadTileRotation add)
		{
			return TileUtilities.GetRotatedRotation(original, (int)add);
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x000630AC File Offset: 0x000612AC
		public static RoadTileRotation SubtractRotation(RoadTileRotation original, RoadTileRotation subtract)
		{
			return TileUtilities.GetRotatedRotation(original, (int)(-(int)subtract));
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x000630B6 File Offset: 0x000612B6
		public static bool IsDirectionDiagonal(TileDirection direction)
		{
			return direction == TileDirection.NorthEast || direction == TileDirection.SouthEast || direction == TileDirection.SouthWest || direction == TileDirection.NorthWest;
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x0001DAB7 File Offset: 0x0001BCB7
		public static RailDirection GetOppositeDirection(RailDirection direction)
		{
			if (direction != RailDirection.Forwards)
			{
				return RailDirection.Forwards;
			}
			return RailDirection.Backwards;
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x000630CC File Offset: 0x000612CC
		public static int Wrap(int value, int maximum)
		{
			if (!Diagnostics.Verify(maximum > 0, "Illegal wrap maximum of 0 or negative."))
			{
				return 0;
			}
			int result = value % maximum;
			if (result < 0)
			{
				result += maximum;
			}
			return result;
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x000630F8 File Offset: 0x000612F8
		public static List<Vector2Int> GetThePerpendicularDiagonalPositions(Vector2Int firstPosition, Vector2Int secondPosition)
		{
			List<Vector2Int> returnList = new List<Vector2Int>();
			TileDirection direction = TileUtilities.GetDirectionBetweenAdjacentCoordinates(firstPosition, secondPosition);
			foreach (TileDirection nonDiagDir in TileUtilities.NonDiagonalDirections)
			{
				if (direction == nonDiagDir)
				{
					return null;
				}
			}
			TileDirection clockwiseDirection = TileUtilities.GetRotatedDirection(direction, 1);
			TileDirection counterClockwiseDirection = TileUtilities.GetRotatedDirection(direction, -1);
			returnList.Add(firstPosition + TileUtilities.GetAdjacencyOffsetForDirection(clockwiseDirection));
			returnList.Add(firstPosition + TileUtilities.GetAdjacencyOffsetForDirection(counterClockwiseDirection));
			return returnList;
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x00063170 File Offset: 0x00061370
		public static RectInt GetBoundsWithBoundary(Vector2Int topLeftCoordinate, Vector2Int footprint, int boundary = 1)
		{
			int xMin = topLeftCoordinate.x - boundary;
			int minY = topLeftCoordinate.y - (footprint.y - 1) - boundary;
			return new RectInt(xMin, minY, footprint.x + boundary * 2, footprint.y + boundary * 2);
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x000631B6 File Offset: 0x000613B6
		public static TileDirection DeserializeDirection(byte serializedDirection)
		{
			if (serializedDirection > 7)
			{
				return TileDirection.None;
			}
			return (TileDirection)serializedDirection;
		}

		// Token: 0x0400166E RID: 5742
		public const int DirectionCount = 8;

		// Token: 0x0400166F RID: 5743
		public static readonly Vector2Int[] DirectionToTileAdjacencyOffset = new Vector2Int[]
		{
			Vector2Int.up,
			Vector2Int.up + Vector2Int.right,
			Vector2Int.right,
			Vector2Int.down + Vector2Int.right,
			Vector2Int.down,
			Vector2Int.down + Vector2Int.left,
			Vector2Int.left,
			Vector2Int.up + Vector2Int.left
		};

		// Token: 0x04001670 RID: 5744
		public static readonly TileDirection[] NonDiagonalDirections = new TileDirection[]
		{
			TileDirection.North,
			TileDirection.East,
			TileDirection.South,
			TileDirection.West
		};

		// Token: 0x04001671 RID: 5745
		public static readonly TileDirection[] DiagonalDirections = new TileDirection[]
		{
			TileDirection.NorthEast,
			TileDirection.SouthEast,
			TileDirection.SouthWest,
			TileDirection.NorthWest
		};

		// Token: 0x04001672 RID: 5746
		public static readonly TileDirection[] Directions = new TileDirection[]
		{
			TileDirection.North,
			TileDirection.NorthEast,
			TileDirection.East,
			TileDirection.SouthEast,
			TileDirection.South,
			TileDirection.SouthWest,
			TileDirection.West,
			TileDirection.NorthWest
		};

		// Token: 0x04001673 RID: 5747
		private static readonly Vector2[] DirectionToVector = new Vector2[]
		{
			Vector2.up,
			(Vector2.up + Vector2.right).normalized,
			Vector2.right,
			(Vector2.down + Vector2.right).normalized,
			Vector2.down,
			(Vector2.down + Vector2.left).normalized,
			Vector2.left,
			(Vector2.up + Vector2.left).normalized
		};

		// Token: 0x04001674 RID: 5748
		private static readonly Vector2Fixed[] DirectionToVectorFixed = new Vector2Fixed[]
		{
			Vector2Fixed.up,
			(Vector2Fixed.up + Vector2Fixed.right).normalized,
			Vector2Fixed.right,
			(Vector2Fixed.down + Vector2Fixed.right).normalized,
			Vector2Fixed.down,
			(Vector2Fixed.down + Vector2Fixed.left).normalized,
			Vector2Fixed.left,
			(Vector2Fixed.up + Vector2Fixed.left).normalized
		};

		// Token: 0x04001675 RID: 5749
		private static readonly Vector2Fixed[] DirectionToTileEdgeVectorFixed = new Vector2Fixed[]
		{
			Vector2Fixed.up,
			Vector2Fixed.up + Vector2Fixed.right,
			Vector2Fixed.right,
			Vector2Fixed.down + Vector2Fixed.right,
			Vector2Fixed.down,
			Vector2Fixed.down + Vector2Fixed.left,
			Vector2Fixed.left,
			Vector2Fixed.up + Vector2Fixed.left
		};
	}
}
