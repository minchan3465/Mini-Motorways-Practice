using System;
using System.Collections.Generic;
using Factory.Pools;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x02000384 RID: 900
	public class TileMatrix<T> : IReusable
	{
		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x0004AA46 File Offset: 0x00048C46
		public RectInt Dimensions
		{
			get
			{
				return this._dimensions;
			}
		}

		// Token: 0x060015C0 RID: 5568 RVA: 0x0004AA50 File Offset: 0x00048C50
		public void Initialize(RectInt dimensions, T defaultValue)
		{
			this._dimensions = dimensions;
			this._defaultValue = defaultValue;
			int arraySize = this._dimensions.width * this._dimensions.height;
			int existingElementCount = Mathf.Min(arraySize, this._data.Count);
			for (int index = 0; index < existingElementCount; index++)
			{
				this._data[index] = this._defaultValue;
			}
			if (arraySize > this._data.Count)
			{
				this._data.Capacity = arraySize;
				for (int index2 = this._data.Count; index2 < arraySize; index2++)
				{
					this._data.Add(defaultValue);
				}
			}
		}

		// Token: 0x060015C1 RID: 5569 RVA: 0x0004AAEF File Offset: 0x00048CEF
		public void Reset()
		{
			this._dimensions = new RectInt(Vector2Int.zero, Vector2Int.zero);
			this._defaultValue = default(T);
		}

		// Token: 0x17000440 RID: 1088
		public T this[Vector2Int tileCoordinates]
		{
			get
			{
				if (this._dimensions.Contains(tileCoordinates))
				{
					int arrayindex = this.ConvertCoordinatesToArrayIndex(tileCoordinates);
					return this._data[arrayindex];
				}
				return this._defaultValue;
			}
			set
			{
				if (this._dimensions.Contains(tileCoordinates))
				{
					int arrayindex = this.ConvertCoordinatesToArrayIndex(tileCoordinates);
					this._data[arrayindex] = value;
				}
			}
		}

		// Token: 0x060015C4 RID: 5572 RVA: 0x0004AB7C File Offset: 0x00048D7C
		public void FillFromTilemap(Tilemap tilemap, TileMatrix<T>.GenerateDataFromTile generator)
		{
			int dataIndex = 0;
			for (int tileY = 0; tileY < this._dimensions.height; tileY++)
			{
				for (int tileX = 0; tileX < this._dimensions.width; tileX++)
				{
					this._data[dataIndex] = generator(tilemap.GetTile(new Vector3Int(this._dimensions.xMin + tileX, this._dimensions.yMin + tileY, 0)));
					dataIndex++;
				}
			}
		}

		// Token: 0x060015C5 RID: 5573 RVA: 0x0004ABF4 File Offset: 0x00048DF4
		public void FillFromCoordinates(TileMatrix<T>.GenerateDataFromCoordinates generator)
		{
			int dataIndex = 0;
			for (int tileY = 0; tileY < this._dimensions.height; tileY++)
			{
				for (int tileX = 0; tileX < this._dimensions.width; tileX++)
				{
					this._data[dataIndex] = generator(new Vector2Int(this._dimensions.xMin + tileX, this._dimensions.yMin + tileY));
					dataIndex++;
				}
			}
		}

		// Token: 0x060015C6 RID: 5574 RVA: 0x0004AC64 File Offset: 0x00048E64
		public void FloodFill(List<Vector2Int> startCoordinates, T startColor, TileMatrix<T>.GetAdjacentFloodFillColor getAdjacentColor, TileMatrix<T>.CanFloodFillEnterTile canEnterTile)
		{
			HashSet<Vector2Int> blockedCoordinates = new HashSet<Vector2Int>();
			Queue<TileMatrix<T>.FloodFillFringeNode> fringe = new Queue<TileMatrix<T>.FloodFillFringeNode>();
			using (List<Vector2Int>.Enumerator enumerator = startCoordinates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Vector2Int startCoordinate = enumerator.Current;
					if (this._dimensions.Contains(startCoordinate))
					{
						fringe.Enqueue(new TileMatrix<T>.FloodFillFringeNode
						{
							coordinates = startCoordinate,
							stepCount = 0,
							color = startColor
						});
						blockedCoordinates.Add(startCoordinate);
					}
				}
				goto IL_15A;
			}
			IL_77:
			TileMatrix<T>.FloodFillFringeNode node = fringe.Dequeue();
			Vector2Int nodeCoordinates = node.coordinates;
			int arrayIndex = this.ConvertCoordinatesToArrayIndex(nodeCoordinates);
			this._data[arrayIndex] = node.color;
			int adjacentNodeStepCount = node.stepCount + 1;
			T adjacentNodeColor = getAdjacentColor(node.color);
			foreach (Vector2Int adjacencyOffset in TileMatrix<T>.FloodFillAdjacencyOffsets)
			{
				Vector2Int adjacentCoordinates = nodeCoordinates + adjacencyOffset;
				if (this._dimensions.Contains(adjacentCoordinates) && !blockedCoordinates.Contains(adjacentCoordinates) && canEnterTile(adjacentCoordinates, adjacentNodeStepCount, this[adjacentCoordinates], adjacentNodeColor))
				{
					fringe.Enqueue(new TileMatrix<T>.FloodFillFringeNode
					{
						coordinates = adjacentCoordinates,
						stepCount = adjacentNodeStepCount,
						color = adjacentNodeColor
					});
					blockedCoordinates.Add(adjacentCoordinates);
				}
			}
			IL_15A:
			if (fringe.Count <= 0)
			{
				return;
			}
			goto IL_77;
		}

		// Token: 0x060015C7 RID: 5575 RVA: 0x0004ADE8 File Offset: 0x00048FE8
		private int ConvertCoordinatesToArrayIndex(Vector2Int coordinates)
		{
			return (coordinates.y - this._dimensions.yMin) * this._dimensions.width + (coordinates.x - this._dimensions.xMin);
		}

		// Token: 0x04001288 RID: 4744
		private RectInt _dimensions;

		// Token: 0x04001289 RID: 4745
		private List<T> _data = new List<T>();

		// Token: 0x0400128A RID: 4746
		private T _defaultValue;

		// Token: 0x0400128B RID: 4747
		private static readonly Vector2Int[] FloodFillAdjacencyOffsets = new Vector2Int[]
		{
			Vector2Int.up,
			Vector2Int.right,
			Vector2Int.down,
			Vector2Int.left
		};

		// Token: 0x02000385 RID: 901
		// (Invoke) Token: 0x060015CB RID: 5579
		public delegate T GenerateDataFromCoordinates(Vector2Int tileCoordinates);

		// Token: 0x02000386 RID: 902
		// (Invoke) Token: 0x060015CF RID: 5583
		public delegate T GenerateDataFromTile(TileBase tile);

		// Token: 0x02000387 RID: 903
		// (Invoke) Token: 0x060015D3 RID: 5587
		public delegate T GetAdjacentFloodFillColor(T color);

		// Token: 0x02000388 RID: 904
		// (Invoke) Token: 0x060015D7 RID: 5591
		public delegate bool CanFloodFillEnterTile(Vector2Int tileCoordinates, int stepCount, T existingColor, T replacementColor);

		// Token: 0x02000389 RID: 905
		private struct FloodFillFringeNode
		{
			// Token: 0x0400128C RID: 4748
			public Vector2Int coordinates;

			// Token: 0x0400128D RID: 4749
			public int stepCount;

			// Token: 0x0400128E RID: 4750
			public T color;
		}
	}
}
