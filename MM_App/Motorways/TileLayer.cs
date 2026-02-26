using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Motorways.EdgeLoopOperator;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Motorways
{
	// Token: 0x020003A2 RID: 930
	[Nullable(0)]
	[NullableContext(1)]
	public class TileLayer
	{
		// Token: 0x0600161A RID: 5658 RVA: 0x0004BFA0 File Offset: 0x0004A1A0
		public static TileLayer FromTilemap(Tilemap tilemap, MapVisualGroupType tileGroupType, int trimEdge, int subdivisions)
		{
			TileLayer tileLayer = new TileLayer();
			tileLayer._subdivisions = subdivisions;
			SubdividableTilemap subdividableTilemap = new SubdividableTilemap(tilemap, subdivisions);
			HashSet<Vector3Int> closedPositions = new HashSet<Vector3Int>();
			foreach (Vector3Int cellPosition in subdividableTilemap.AllPositionsWithin)
			{
				if (TileLayer.IsSolid(subdividableTilemap, cellPosition, tileGroupType, trimEdge) && !closedPositions.Contains(cellPosition))
				{
					Queue<Vector3Int> openPositions = new Queue<Vector3Int>();
					openPositions.Enqueue(cellPosition);
					closedPositions.Add(cellPosition);
					TileLayer.TileChunk tileChunk = new TileLayer.TileChunk();
					tileLayer._tileChunks.Add(tileChunk);
					while (openPositions.Any<Vector3Int>())
					{
						Vector3Int currentPosition = openPositions.Dequeue();
						tileChunk.Add(currentPosition);
						for (int directionIndex = 0; directionIndex < TileLayer.DirectionsDiagonal.Length; directionIndex++)
						{
							Vector3Int direction = TileLayer.DirectionsDiagonal[directionIndex];
							Vector3Int candidatePosition = currentPosition + direction;
							if (!TileLayer.IsSolid(subdividableTilemap, candidatePosition, tileGroupType, trimEdge))
							{
								if (directionIndex < 4)
								{
									tileChunk.allBorderSegments.Add(new TileLayer.BorderSegment(currentPosition, direction));
								}
							}
							else if (!closedPositions.Contains(candidatePosition))
							{
								openPositions.Enqueue(candidatePosition);
								closedPositions.Add(candidatePosition);
							}
						}
					}
				}
			}
			return tileLayer;
		}

		// Token: 0x0600161B RID: 5659 RVA: 0x0004C0F4 File Offset: 0x0004A2F4
		private static bool IsSolid(SubdividableTilemap tilemap, Vector3Int position, MapVisualGroupType groupType, int trimEdge)
		{
			bool invert;
			if (groupType != MapVisualGroupType.Land)
			{
				if (groupType != MapVisualGroupType.Mountains)
				{
					Diagnostics.FailAssert(string.Format("Unsupported tile layer type: {0}", groupType), Array.Empty<object>());
					return false;
				}
				invert = false;
			}
			else
			{
				invert = true;
			}
			if (trimEdge == 0)
			{
				return TileLayer.CheckTilePosition(tilemap, position, invert);
			}
			Queue<ValueTuple<Vector3Int, int>> openSet = new Queue<ValueTuple<Vector3Int, int>>();
			HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();
			openSet.Enqueue(new ValueTuple<Vector3Int, int>(position, 0));
			closedSet.Add(position);
			while (openSet.Count > 0)
			{
				ValueTuple<Vector3Int, int> valueTuple = openSet.Dequeue();
				Vector3Int currentPosition = valueTuple.Item1;
				int currentDistance = valueTuple.Item2;
				if (!TileLayer.CheckTilePosition(tilemap, currentPosition, invert))
				{
					return false;
				}
				if (currentDistance < trimEdge)
				{
					foreach (Vector3Int direction in TileLayer.DirectionsDiagonal)
					{
						Vector3Int candidate = currentPosition + direction;
						if (!closedSet.Contains(candidate))
						{
							openSet.Enqueue(new ValueTuple<Vector3Int, int>(candidate, currentDistance + 1));
							closedSet.Add(candidate);
						}
					}
				}
			}
			return true;
		}

		// Token: 0x0600161C RID: 5660 RVA: 0x0004C1E3 File Offset: 0x0004A3E3
		private static bool CheckTilePosition(SubdividableTilemap tilemap, Vector3Int position, bool invert)
		{
			if (invert)
			{
				return tilemap.Contains(position) && !tilemap.HasTile(position);
			}
			return tilemap.Contains(position) && tilemap.HasTile(position);
		}

		// Token: 0x0600161D RID: 5661 RVA: 0x0004C210 File Offset: 0x0004A410
		public List<EdgeLoop> BuildEdgeLoops(MapVisualGroupType visualGroupType, MapMeshLayer meshLayer)
		{
			List<EdgeLoop> result = new List<EdgeLoop>();
			foreach (TileLayer.TileChunk landmass in this._tileChunks)
			{
				while (landmass.allBorderSegments.Any<TileLayer.BorderSegment>())
				{
					EdgeLoop edgeLoop = new EdgeLoop(visualGroupType, meshLayer);
					result.Add(edgeLoop);
					TileLayer.BorderSegment borderSegment = landmass.allBorderSegments.Last<TileLayer.BorderSegment>();
					Vector3Int currentBorderTilePosition = borderSegment.landTilePosition;
					Vector3Int currentBoundaryNormal = borderSegment.borderNormal;
					Vector3Int currentBoundaryDirection = currentBoundaryNormal.RotateCW2D();
					Vector3Int startPosition = currentBorderTilePosition;
					Vector3Int startNormal = currentBoundaryNormal;
					do
					{
						Vector3 vertexPosition = currentBorderTilePosition + (currentBoundaryNormal + currentBoundaryDirection) / 2f;
						landmass.allBorderSegments.Remove(new TileLayer.BorderSegment(currentBorderTilePosition, currentBoundaryNormal));
						Vector3Int p0 = currentBorderTilePosition + currentBoundaryNormal + currentBoundaryDirection;
						Vector3Int p = currentBorderTilePosition + currentBoundaryDirection;
						TopologyType vertexTopology;
						if (landmass.Contains(p0))
						{
							vertexTopology = TopologyType.Concave;
							currentBorderTilePosition = p0;
							currentBoundaryDirection = currentBoundaryDirection.RotateCCW2D();
							currentBoundaryNormal = currentBoundaryNormal.RotateCCW2D();
						}
						else if (landmass.Contains(p))
						{
							vertexTopology = TopologyType.Flat;
							currentBorderTilePosition = p;
						}
						else
						{
							vertexTopology = TopologyType.Convex;
							currentBoundaryDirection = currentBoundaryDirection.RotateCW2D();
							currentBoundaryNormal = currentBoundaryNormal.RotateCW2D();
						}
						if (this._subdivisions == 2)
						{
							vertexPosition += new Vector3(-0.5f, -0.5f, 0f);
						}
						edgeLoop.AddPoint(vertexPosition / (float)this._subdivisions, vertexTopology);
					}
					while (startPosition != currentBorderTilePosition || startNormal != currentBoundaryNormal);
				}
			}
			return result;
		}

		// Token: 0x040012E2 RID: 4834
		private static readonly Vector3Int[] Directions = new Vector3Int[]
		{
			Vector3Int.up,
			Vector3Int.right,
			Vector3Int.down,
			Vector3Int.left
		};

		// Token: 0x040012E3 RID: 4835
		private static readonly Vector3Int[] DirectionsDiagonal = new Vector3Int[]
		{
			Vector3Int.up,
			Vector3Int.right,
			Vector3Int.down,
			Vector3Int.left,
			new Vector3Int(1, 1, 0),
			new Vector3Int(1, -1, 0),
			new Vector3Int(-1, -1, 0),
			new Vector3Int(-1, 1, 0)
		};

		// Token: 0x040012E4 RID: 4836
		private readonly List<TileLayer.TileChunk> _tileChunks = new List<TileLayer.TileChunk>();

		// Token: 0x040012E5 RID: 4837
		private int _subdivisions;

		// Token: 0x020003A3 RID: 931
		[NullableContext(0)]
		private class BorderSegment : IEquatable<TileLayer.BorderSegment>
		{
			// Token: 0x06001620 RID: 5664 RVA: 0x0004C497 File Offset: 0x0004A697
			public BorderSegment(Vector3Int landTilePosition, Vector3Int borderNormal)
			{
				this.landTilePosition = landTilePosition;
				this.borderNormal = borderNormal;
			}

			// Token: 0x06001621 RID: 5665 RVA: 0x0004C4B0 File Offset: 0x0004A6B0
			[NullableContext(1)]
			public bool Equals(TileLayer.BorderSegment other)
			{
				return other != null && (this == other || (this.landTilePosition.Equals(other.landTilePosition) && this.borderNormal.Equals(other.borderNormal)));
			}

			// Token: 0x06001622 RID: 5666 RVA: 0x0004C4F4 File Offset: 0x0004A6F4
			public override int GetHashCode()
			{
				return this.landTilePosition.GetHashCode() * 397 ^ this.borderNormal.GetHashCode();
			}

			// Token: 0x040012E6 RID: 4838
			public readonly Vector3Int landTilePosition;

			// Token: 0x040012E7 RID: 4839
			public readonly Vector3Int borderNormal;
		}

		// Token: 0x020003A4 RID: 932
		[NullableContext(0)]
		private class TileChunk : HashSet<Vector3Int>
		{
			// Token: 0x040012E8 RID: 4840
			[Nullable(1)]
			public readonly HashSet<TileLayer.BorderSegment> allBorderSegments = new HashSet<TileLayer.BorderSegment>();
		}
	}
}
