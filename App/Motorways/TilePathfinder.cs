using System;
using System.Collections.Generic;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003C1 RID: 961
	public class TilePathfinder
	{
		// Token: 0x060016E6 RID: 5862 RVA: 0x000530C8 File Offset: 0x000512C8
		public TilePathfinder()
		{
			for (int nodeCount = 0; nodeCount < 100; nodeCount++)
			{
				this._pathfindNodePool.Add(new TilePathfinder.PathNode());
			}
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x00053134 File Offset: 0x00051334
		public IEnumerable<Vector2Int> GetPathBetweenPoints(Vector2Int start, Vector2Int end, ISimulation simulation, City city, ICollection<Vector2Int> blockedPositions = null)
		{
			this._openList.Clear();
			this._openListIndex.Clear();
			this._closedListIndex.Clear();
			this._usedNodeCount = 0;
			this._iterationCount = 0;
			TilePathfinder.PathNode currentNode = this.CreateNode(start, 0, TilePathfinder.CalculateMetroDistanceBetweenTiles(start, end), null);
			TilemapModel tilemapModel = simulation.GetModel<TilemapModel>();
			while (currentNode.position != end && this._iterationCount < 300)
			{
				this._iterationCount++;
				foreach (Vector2Int direction in TileUtilities.DirectionToTileAdjacencyOffset)
				{
					Vector2Int newPoint = currentNode.position + direction;
					if (city.Definition.TileIsBuildable(newPoint) && (blockedPositions == null || !blockedPositions.Contains(newPoint)))
					{
						if (newPoint != end)
						{
							Tile tile = tilemapModel.GetTile(newPoint);
							if (tile != null && !tile.CanDrawRoadsOn())
							{
								goto IL_20F;
							}
						}
						int estimatedRemainingCost = TilePathfinder.CalculateMetroDistanceBetweenTiles(newPoint, end);
						int directionDistance = (direction.x == 0 || direction.y == 0) ? 70 : 99;
						TilePathfinder.PathNode newNode = this.CreateNode(newPoint, currentNode.cost + directionDistance, estimatedRemainingCost, currentNode);
						bool removeSlowerOpenNode = false;
						TilePathfinder.PathNode openNode;
						TilePathfinder.PathNode closedNode;
						if (this._openListIndex.TryGetValue(newPoint, out openNode))
						{
							if (newNode.totalCost >= openNode.totalCost)
							{
								goto IL_20F;
							}
							this._openListIndex.Remove(newPoint);
							removeSlowerOpenNode = true;
						}
						else if (this._closedListIndex.TryGetValue(newPoint, out closedNode))
						{
							if (newNode.totalCost >= closedNode.totalCost)
							{
								goto IL_20F;
							}
							this._closedListIndex.Remove(newPoint);
						}
						int fringeIndex = 0;
						while (fringeIndex < this._openList.Count && this._openList[fringeIndex].totalCost <= newNode.totalCost)
						{
							fringeIndex++;
						}
						if (removeSlowerOpenNode)
						{
							for (int matchingOpenNodeIndex = fringeIndex; matchingOpenNodeIndex < this._openList.Count; matchingOpenNodeIndex++)
							{
								if (this._openList[matchingOpenNodeIndex].position == newPoint)
								{
									this._openList.RemoveAt(matchingOpenNodeIndex);
									break;
								}
							}
						}
						this._openList.Insert(fringeIndex, newNode);
						this._openListIndex.Add(newNode.position, newNode);
					}
					IL_20F:;
				}
				this._closedListIndex.Add(currentNode.position, currentNode);
				if (this._openList.Count <= 0)
				{
					break;
				}
				currentNode = this._openList[0];
				this._openList.RemoveAt(0);
				this._openListIndex.Remove(currentNode.position);
			}
			if (currentNode.position != end)
			{
				return null;
			}
			this._path.Clear();
			while (currentNode != null)
			{
				this._path.Add(currentNode.position);
				currentNode = currentNode.previousNode;
			}
			this._path.Reverse();
			return this._path;
		}

		// Token: 0x060016E8 RID: 5864 RVA: 0x00053410 File Offset: 0x00051610
		private TilePathfinder.PathNode CreateNode(Vector2Int position, int cost, int estimatedRemainingCost, TilePathfinder.PathNode previousNode)
		{
			if (this._usedNodeCount >= this._pathfindNodePool.Count)
			{
				for (int nodeCount = 0; nodeCount < 20; nodeCount++)
				{
					this._pathfindNodePool.Add(new TilePathfinder.PathNode());
				}
			}
			TilePathfinder.PathNode pathNode = this._pathfindNodePool[this._usedNodeCount];
			pathNode.position = position;
			pathNode.cost = cost;
			pathNode.totalCost = cost + estimatedRemainingCost;
			pathNode.previousNode = previousNode;
			this._usedNodeCount++;
			return pathNode;
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x0005348C File Offset: 0x0005168C
		private static int CalculateMetroDistanceBetweenTiles(Vector2Int startPosition, Vector2Int endPosition)
		{
			int a = Mathf.Abs(startPosition.x - endPosition.x);
			int yDiff = Mathf.Abs(startPosition.y - endPosition.y);
			int maxDiff = Mathf.Max(a, yDiff);
			int minDiff = Mathf.Min(a, yDiff);
			return minDiff * 99 + (maxDiff - minDiff) * 70;
		}

		// Token: 0x04001380 RID: 4992
		private const int MaximumIterations = 300;

		// Token: 0x04001381 RID: 4993
		private const int DiagonalCost = 99;

		// Token: 0x04001382 RID: 4994
		private const int StraightCost = 70;

		// Token: 0x04001383 RID: 4995
		private readonly List<TilePathfinder.PathNode> _pathfindNodePool = new List<TilePathfinder.PathNode>(100);

		// Token: 0x04001384 RID: 4996
		private const int NodesToAllocate = 100;

		// Token: 0x04001385 RID: 4997
		private int _usedNodeCount;

		// Token: 0x04001386 RID: 4998
		private readonly List<TilePathfinder.PathNode> _openList = new List<TilePathfinder.PathNode>();

		// Token: 0x04001387 RID: 4999
		private readonly Dictionary<Vector2Int, TilePathfinder.PathNode> _openListIndex = new Dictionary<Vector2Int, TilePathfinder.PathNode>();

		// Token: 0x04001388 RID: 5000
		private readonly Dictionary<Vector2Int, TilePathfinder.PathNode> _closedListIndex = new Dictionary<Vector2Int, TilePathfinder.PathNode>();

		// Token: 0x04001389 RID: 5001
		private readonly List<Vector2Int> _path = new List<Vector2Int>();

		// Token: 0x0400138A RID: 5002
		private int _iterationCount;

		// Token: 0x020003C2 RID: 962
		private class PathNode
		{
			// Token: 0x0400138B RID: 5003
			public Vector2Int position;

			// Token: 0x0400138C RID: 5004
			public int cost;

			// Token: 0x0400138D RID: 5005
			public int totalCost;

			// Token: 0x0400138E RID: 5006
			public TilePathfinder.PathNode previousNode;
		}
	}
}
