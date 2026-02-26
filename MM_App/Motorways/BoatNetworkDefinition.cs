using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000364 RID: 868
	public class BoatNetworkDefinition
	{
		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x00048B97 File Offset: 0x00046D97
		public IReadOnlyList<BoatPathLineDefinition> BoatLines
		{
			get
			{
				return this._boatLines;
			}
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x00048BA0 File Offset: 0x00046DA0
		public BoatPathLineDefinition CreateBoatLine()
		{
			BoatPathLineDefinition boatPathLineDefinition = new BoatPathLineDefinition();
			this._boatLines.Add(boatPathLineDefinition);
			return boatPathLineDefinition;
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00048BC0 File Offset: 0x00046DC0
		public static BoatNetworkDefinition CreateFromBoatPathTileCoordinates(Dictionary<Vector2Int, BoatPathType> boatPathTileCoordinates)
		{
			BoatNetworkDefinition boatNetworkDefinition = new BoatNetworkDefinition();
			Dictionary<Vector2Int, bool> visitedTile = new Dictionary<Vector2Int, bool>();
			foreach (Vector2Int tilePosition in boatPathTileCoordinates.Keys)
			{
				visitedTile.Add(tilePosition, false);
			}
			Stack<Vector2Int> openTileList = new Stack<Vector2Int>();
			for (;;)
			{
				Vector2Int? nextTileToStartSearchFromCandidate = BoatNetworkDefinition.FindNextTileToStartSearchFrom(boatPathTileCoordinates, visitedTile);
				if (nextTileToStartSearchFromCandidate == null)
				{
					break;
				}
				openTileList.Push(nextTileToStartSearchFromCandidate.Value);
				BoatPathLineDefinition currentBoatLine = boatNetworkDefinition.CreateBoatLine();
				while (openTileList.Count > 0)
				{
					Vector2Int currentTilePosition = openTileList.Pop();
					if (!visitedTile[currentTilePosition])
					{
						visitedTile[currentTilePosition] = true;
						currentBoatLine.AddBoatPath(new Vector2Int(currentTilePosition.x, currentTilePosition.y), boatPathTileCoordinates[currentTilePosition]);
						int alreadyVisitedCount = 0;
						int directionsAvailableAtTile = 0;
						TileDirection[] array = TileUtilities.NonDiagonalDirections;
						for (int i = 0; i < array.Length; i++)
						{
							Vector2Int directionVector = TileUtilities.GetAdjacencyOffsetForDirection(array[i]);
							Vector2Int tilePosition2 = new Vector2Int(currentTilePosition.x + directionVector.x, currentTilePosition.y + directionVector.y);
							if (boatPathTileCoordinates.ContainsKey(tilePosition2))
							{
								directionsAvailableAtTile++;
								if (visitedTile[tilePosition2])
								{
									alreadyVisitedCount++;
								}
								else
								{
									openTileList.Push(tilePosition2);
								}
							}
						}
						foreach (TileDirection diagonalDirection in TileUtilities.DiagonalDirections)
						{
							Vector2Int directionVector2 = TileUtilities.GetAdjacencyOffsetForDirection(diagonalDirection);
							Vector2Int diagonalTilePosition = new Vector2Int(currentTilePosition.x + directionVector2.x, currentTilePosition.y + directionVector2.y);
							if (boatPathTileCoordinates.ContainsKey(diagonalTilePosition))
							{
								TileDirection negativeDirection = TileUtilities.GetRotatedDirection(diagonalDirection, -1);
								TileDirection rotatedDirection = TileUtilities.GetRotatedDirection(diagonalDirection, 1);
								Vector2Int negativeDirectionVector = TileUtilities.GetAdjacencyOffsetForDirection(negativeDirection);
								Vector2Int positiveDirectionVector = TileUtilities.GetAdjacencyOffsetForDirection(rotatedDirection);
								Vector2Int negativeDirectionPosition = currentTilePosition + negativeDirectionVector;
								Vector2Int positiveDirectionPosition = currentTilePosition + positiveDirectionVector;
								bool flag = boatPathTileCoordinates.ContainsKey(negativeDirectionPosition);
								bool hasPositiveTileBase = boatPathTileCoordinates.ContainsKey(positiveDirectionPosition);
								if (!flag && !hasPositiveTileBase)
								{
									directionsAvailableAtTile++;
									if (visitedTile[diagonalTilePosition])
									{
										alreadyVisitedCount++;
									}
									else
									{
										openTileList.Push(diagonalTilePosition);
									}
								}
							}
						}
						currentBoatLine.isLoop = (alreadyVisitedCount == 2);
						currentBoatLine.isValid = (directionsAvailableAtTile <= 2);
					}
				}
			}
			return boatNetworkDefinition;
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00048E10 File Offset: 0x00047010
		private static Vector2Int? FindNextTileToStartSearchFrom(Dictionary<Vector2Int, BoatPathType> tilemap, Dictionary<Vector2Int, bool> visitedTiles)
		{
			foreach (KeyValuePair<Vector2Int, bool> tileVisitedInfo in visitedTiles)
			{
				if (!tileVisitedInfo.Value)
				{
					Vector2Int currentTilePosition = tileVisitedInfo.Key;
					int directionsAvailableAtTile = 0;
					TileDirection[] array = TileUtilities.NonDiagonalDirections;
					for (int i = 0; i < array.Length; i++)
					{
						Vector2Int directionVector = TileUtilities.GetAdjacencyOffsetForDirection(array[i]);
						Vector2Int tilePosition = new Vector2Int(currentTilePosition.x + directionVector.x, currentTilePosition.y + directionVector.y);
						if (tilemap.ContainsKey(tilePosition))
						{
							directionsAvailableAtTile++;
						}
						if (directionsAvailableAtTile >= 2)
						{
							break;
						}
					}
					if (directionsAvailableAtTile <= 1)
					{
						foreach (TileDirection diagonalDirection in TileUtilities.DiagonalDirections)
						{
							Vector2Int directionVector2 = TileUtilities.GetAdjacencyOffsetForDirection(diagonalDirection);
							Vector2Int diagonalTilePosition = new Vector2Int(currentTilePosition.x + directionVector2.x, currentTilePosition.y + directionVector2.y);
							if (tilemap.ContainsKey(diagonalTilePosition))
							{
								TileDirection negativeDirection = TileUtilities.GetRotatedDirection(diagonalDirection, -1);
								TileDirection rotatedDirection = TileUtilities.GetRotatedDirection(diagonalDirection, 1);
								Vector2Int negativeDirectionVector = TileUtilities.GetAdjacencyOffsetForDirection(negativeDirection);
								Vector2Int positiveDirectionVector = TileUtilities.GetAdjacencyOffsetForDirection(rotatedDirection);
								Vector2Int negativeDirectionPosition = new Vector2Int(currentTilePosition.x + negativeDirectionVector.x, currentTilePosition.y + negativeDirectionVector.y);
								Vector2Int positiveDirectionPosition = new Vector2Int(currentTilePosition.x + positiveDirectionVector.x, currentTilePosition.y + positiveDirectionVector.y);
								bool flag = tilemap.ContainsKey(negativeDirectionPosition);
								bool hasPositiveTileBase = tilemap.ContainsKey(positiveDirectionPosition);
								if (!flag && !hasPositiveTileBase)
								{
									directionsAvailableAtTile++;
								}
							}
						}
						if (directionsAvailableAtTile <= 1)
						{
							return new Vector2Int?(tileVisitedInfo.Key);
						}
					}
				}
			}
			foreach (KeyValuePair<Vector2Int, bool> tileVisitedInfo2 in visitedTiles)
			{
				if (!tileVisitedInfo2.Value)
				{
					return new Vector2Int?(tileVisitedInfo2.Key);
				}
			}
			return null;
		}

		// Token: 0x040011C8 RID: 4552
		private readonly List<BoatPathLineDefinition> _boatLines = new List<BoatPathLineDefinition>();
	}
}
