using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000431 RID: 1073
	public class Roundabout
	{
		// Token: 0x06001A6E RID: 6766 RVA: 0x000605BF File Offset: 0x0005E7BF
		static Roundabout()
		{
			Roundabout.InitializeConstants();
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x000605C8 File Offset: 0x0005E7C8
		public static void InitializeConstants()
		{
			Roundabout.ConnectionsToCoordinatesOffset = new Dictionary<RoadTileConnection, Vector2Int>
			{
				{
					new RoadTileConnection(new RoadTileNode(TileDirection.NorthWest, RoadType.Roundabout, -1), new RoadTileNode(TileDirection.NorthEast, RoadType.Roundabout, -1)),
					new Vector2Int(0, -1)
				},
				{
					new RoadTileConnection(new RoadTileNode(TileDirection.SouthWest, RoadType.Roundabout, -1), new RoadTileNode(TileDirection.NorthWest, RoadType.Roundabout, -1)),
					new Vector2Int(1, 0)
				},
				{
					new RoadTileConnection(new RoadTileNode(TileDirection.SouthEast, RoadType.Roundabout, -1), new RoadTileNode(TileDirection.SouthWest, RoadType.Roundabout, -1)),
					new Vector2Int(0, 1)
				},
				{
					new RoadTileConnection(new RoadTileNode(TileDirection.NorthEast, RoadType.Roundabout, -1), new RoadTileNode(TileDirection.SouthEast, RoadType.Roundabout, -1)),
					new Vector2Int(-1, 0)
				}
			};
			Roundabout.CoordinatesOffsetsToConnection = new Dictionary<Vector2Int, RoadTileConnection>();
			foreach (RoadTileConnection connection in Roundabout.ConnectionsToCoordinatesOffset.Keys)
			{
				Roundabout.CoordinatesOffsetsToConnection[Roundabout.ConnectionsToCoordinatesOffset[connection]] = connection;
			}
			Roundabout.CoordinatesOffsets = new List<Vector2Int>(Roundabout.CoordinatesOffsetsToConnection.Keys);
			List<Vector2Int> adjacentTileOffsets = new List<Vector2Int>
			{
				Vector2Int.left,
				Vector2Int.right,
				Vector2Int.up,
				Vector2Int.down
			};
			Roundabout.NeighborCoordinatesOffsets = new List<Vector2Int>();
			foreach (Vector2Int tileOffset in Roundabout.CoordinatesOffsets)
			{
				foreach (Vector2Int adjacentTileOffset in adjacentTileOffsets)
				{
					Vector2Int neighboringTileOffset = tileOffset + adjacentTileOffset;
					if (!Roundabout.CoordinatesOffsets.Contains(neighboringTileOffset) && !Roundabout.NeighborCoordinatesOffsets.Contains(neighboringTileOffset))
					{
						Roundabout.NeighborCoordinatesOffsets.Add(neighboringTileOffset);
					}
				}
			}
			Roundabout.NeighborCoordinatesOffsetsToInvalidNodeDirections = new Dictionary<Vector2Int, TileDirectionBitfield>();
			foreach (Vector2Int neighbouringTileOffset in Roundabout.NeighborCoordinatesOffsets)
			{
				TileDirectionBitfield invalidNodeDirections = default(TileDirectionBitfield);
				foreach (Vector2Int roundaboutTileOffset in Roundabout.CoordinatesOffsets)
				{
					TileDirection directionFromRoundaboutToNeighbor = TileUtilities.GetDirectionBetweenAdjacentCoordinates(roundaboutTileOffset, neighbouringTileOffset);
					if (directionFromRoundaboutToNeighbor != TileDirection.None && !Roundabout.CanConnectionAddExitNode(Roundabout.CoordinatesOffsetsToConnection[roundaboutTileOffset], new RoadTileNode(directionFromRoundaboutToNeighbor, RoadType.TwoLane, -1)))
					{
						invalidNodeDirections[TileUtilities.GetOppositeDirection(directionFromRoundaboutToNeighbor)] = true;
					}
				}
				if (neighbouringTileOffset == Roundabout.GetCenterOffset())
				{
					invalidNodeDirections[TileDirection.NorthEast] = true;
					invalidNodeDirections[TileDirection.SouthEast] = true;
					invalidNodeDirections[TileDirection.SouthWest] = true;
					invalidNodeDirections[TileDirection.NorthWest] = true;
				}
				Roundabout.NeighborCoordinatesOffsetsToInvalidNodeDirections[neighbouringTileOffset] = invalidNodeDirections;
			}
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x000608CC File Offset: 0x0005EACC
		public static Vector2Int GetCenterOffset()
		{
			return Vector2Int.zero;
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x000608D3 File Offset: 0x0005EAD3
		public static IList<Vector2Int> GetCoordinatesOffsets()
		{
			return Roundabout.CoordinatesOffsets;
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x000608DA File Offset: 0x0005EADA
		public static bool IsCoordinatesOffsetInRoundabout(Vector2Int coordinatesOffset)
		{
			return Roundabout.CoordinatesOffsetsToConnection.ContainsKey(coordinatesOffset);
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x000608E7 File Offset: 0x0005EAE7
		public static RoadTileConnection GetConnectionForCoordinatesOffset(Vector2Int coordinatesOffset)
		{
			return Roundabout.CoordinatesOffsetsToConnection[coordinatesOffset];
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x000608F4 File Offset: 0x0005EAF4
		public static Vector2Int GetCoordinatesOffsetForConnection(RoadTileConnection roundaboutConnection)
		{
			Vector2Int offset;
			if (Diagnostics.Verify(Roundabout.ConnectionsToCoordinatesOffset.TryGetValue(roundaboutConnection, out offset), "{0} is not a recognised roundabout connection.", roundaboutConnection))
			{
				return offset;
			}
			return default(Vector2Int);
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0006092B File Offset: 0x0005EB2B
		public static IEnumerable<Vector2Int> GetNeighborCoordinatesOffsets()
		{
			return Roundabout.NeighborCoordinatesOffsets;
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x00060934 File Offset: 0x0005EB34
		public static TileDirectionBitfield GetInvalidNodeDirectionsForNeighbor(Vector2Int neighborCoordinatesOffset)
		{
			TileDirectionBitfield invalidNodeDirections;
			if (Roundabout.NeighborCoordinatesOffsetsToInvalidNodeDirections.TryGetValue(neighborCoordinatesOffset, out invalidNodeDirections))
			{
				return invalidNodeDirections;
			}
			return TileDirectionBitfield.None;
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x00060958 File Offset: 0x0005EB58
		public static bool DoesConnectionOwnRoundabout(RoadTileConnection roundaboutConnection)
		{
			TileDirection roundaboutInput = roundaboutConnection.input.direction;
			TileDirection roundaboutOutput = roundaboutConnection.output.direction;
			return (roundaboutInput == TileDirection.NorthWest && roundaboutOutput == TileDirection.NorthEast) || (roundaboutInput == TileDirection.NorthEast && roundaboutOutput == TileDirection.NorthWest);
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x00060991 File Offset: 0x0005EB91
		public static bool CanConnectionAddExitNode(RoadTileConnection roundaboutConnection, RoadTileNode exitNode)
		{
			return Roundabout.CanConnectionAddExitNode(roundaboutConnection.input.direction, roundaboutConnection.output.direction, exitNode);
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x000609B0 File Offset: 0x0005EBB0
		public static bool CanConnectionAddExitNode(TileDirection roundaboutInput, TileDirection roundaboutOutput, RoadTileNode exitNode)
		{
			return Roundabout.GetValidExitsForConnection(roundaboutInput, roundaboutOutput)[exitNode.direction];
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x000609D4 File Offset: 0x0005EBD4
		public static TileDirectionBitfield GetValidExitsForConnection(TileDirection roundaboutInput, TileDirection roundaboutOutput)
		{
			TileDirectionBitfield validExits = default(TileDirectionBitfield);
			if (roundaboutInput == TileUtilities.GetOppositeDirection(roundaboutOutput))
			{
				validExits[TileUtilities.GetRotatedDirection(roundaboutOutput, 2)] = true;
			}
			else if (Diagnostics.Verify(roundaboutOutput == TileUtilities.GetRotatedDirection(roundaboutInput, 2), "{0} -> {1} is a peculiar roundabout connection that is not a corner or straight.", roundaboutInput, roundaboutOutput))
			{
				TileDirection validExit = TileUtilities.GetOppositeDirection(roundaboutInput);
				validExits[validExit] = true;
				validExits[TileUtilities.GetRotatedDirection(validExit, -1)] = true;
				validExits[TileUtilities.GetRotatedDirection(validExit, 1)] = true;
				validExits[TileUtilities.GetRotatedDirection(validExit, 2)] = true;
				validExits[TileUtilities.GetRotatedDirection(validExit, 3)] = true;
			}
			return validExits;
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x00060A72 File Offset: 0x0005EC72
		public static TileDirectionBitfield GetInvalidExitsForConnection(TileDirection roundaboutInput, TileDirection roundaboutOutput)
		{
			return ~Roundabout.GetValidExitsForConnection(roundaboutInput, roundaboutOutput);
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x00060A80 File Offset: 0x0005EC80
		public static Tile GetCenterTile(Tile roundaboutTile, RoadState roundaboutState)
		{
			if (Roundabout.IsTileCenterOfRoundabout(roundaboutTile, roundaboutState))
			{
				return roundaboutTile;
			}
			RoadTileConnection roundaboutConnection = roundaboutTile.GetRoundaboutConnection(roundaboutState);
			if (!Diagnostics.Verify(roundaboutConnection.IsRoundabout, "Tile {0} is not a roundabout!", roundaboutTile))
			{
				return null;
			}
			return roundaboutTile.Tilemap.GetTile(roundaboutTile.Coordinates - Roundabout.GetCoordinatesOffsetForConnection(roundaboutConnection));
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x00060AD2 File Offset: 0x0005ECD2
		public static IEnumerable<Tile> GetTilesInRoundabout(Tile roundaboutTile, RoadState roundaboutState)
		{
			ITilemap tilemap = roundaboutTile.Tilemap;
			if (Roundabout.IsTileCenterOfRoundabout(roundaboutTile, roundaboutState))
			{
				roundaboutTile = tilemap.GetTile(TileUtilities.GetAdjacentCoordinates(roundaboutTile.Coordinates, TileDirection.North));
			}
			RoadTileConnection roundaboutConnection = roundaboutTile.GetRoundaboutConnection(roundaboutState);
			if (Diagnostics.Verify(roundaboutConnection.IsRoundabout, "GetTilesOnRoundabout called on a non-roundabout tile."))
			{
				foreach (Tile tile in Roundabout.GetTilesInRoundabout(roundaboutTile, roundaboutConnection))
				{
					yield return tile;
				}
				IEnumerator<Tile> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06001A7E RID: 6782 RVA: 0x00060AE9 File Offset: 0x0005ECE9
		public static IEnumerable<Tile> GetTilesInRoundabout(Tile roundaboutTile, RoadTileConnection roundaboutConnection)
		{
			ITilemap tilemap = roundaboutTile.Tilemap;
			Vector2Int offset = Roundabout.GetCoordinatesOffsetForConnection(roundaboutConnection);
			Vector2Int roundaboutOrigin = roundaboutTile.Coordinates - offset;
			foreach (Vector2Int coordinatesOffset in Roundabout.CoordinatesOffsets)
			{
				yield return tilemap.GetTile(roundaboutOrigin + coordinatesOffset);
			}
			List<Vector2Int>.Enumerator enumerator = default(List<Vector2Int>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06001A7F RID: 6783 RVA: 0x00060B00 File Offset: 0x0005ED00
		public static bool IsTileCenterOfRoundabout(Tile tile, RoadState roadState = RoadState.VisiblyActive)
		{
			bool result;
			using (Dictionary<Vector2Int, RoadTileConnection>.Enumerator offsetsEnumerator = Roundabout.CoordinatesOffsetsToConnection.GetEnumerator())
			{
				offsetsEnumerator.MoveNext();
				KeyValuePair<Vector2Int, RoadTileConnection> offsetToConnection = offsetsEnumerator.Current;
				Tile offsetTile = tile.Tilemap.GetTile(tile.Coordinates + offsetToConnection.Key);
				if (offsetTile != null && offsetTile.HasRoundabout(roadState))
				{
					result = offsetTile.GetRoundaboutConnection(roadState).Equals(offsetToConnection.Value);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04001617 RID: 5655
		private static List<Vector2Int> CoordinatesOffsets;

		// Token: 0x04001618 RID: 5656
		private static Dictionary<RoadTileConnection, Vector2Int> ConnectionsToCoordinatesOffset;

		// Token: 0x04001619 RID: 5657
		private static Dictionary<Vector2Int, RoadTileConnection> CoordinatesOffsetsToConnection;

		// Token: 0x0400161A RID: 5658
		private static List<Vector2Int> NeighborCoordinatesOffsets;

		// Token: 0x0400161B RID: 5659
		private static Dictionary<Vector2Int, TileDirectionBitfield> NeighborCoordinatesOffsetsToInvalidNodeDirections;
	}
}
