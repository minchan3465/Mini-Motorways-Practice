using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x0200038F RID: 911
	public class TrainNetworkDefinition
	{
		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x060015E3 RID: 5603 RVA: 0x0004AF18 File Offset: 0x00049118
		public IReadOnlyList<TrainLineDefinition> TrainLines
		{
			get
			{
				return this._trainLines;
			}
		}

		// Token: 0x060015E4 RID: 5604 RVA: 0x0004AF20 File Offset: 0x00049120
		public TrainLineDefinition CreateTrainLine()
		{
			TrainLineDefinition trainLineDefinition = new TrainLineDefinition();
			this._trainLines.Add(trainLineDefinition);
			return trainLineDefinition;
		}

		// Token: 0x060015E5 RID: 5605 RVA: 0x0004AF40 File Offset: 0x00049140
		public static TrainNetworkDefinition CreateFromRailTileCoordinates(Dictionary<Vector2Int, RailType> railTileCoordinates)
		{
			TrainNetworkDefinition trainNetworkDefinition = new TrainNetworkDefinition();
			Dictionary<Vector2Int, bool> visitedTile = new Dictionary<Vector2Int, bool>();
			foreach (Vector2Int tilePosition in railTileCoordinates.Keys)
			{
				visitedTile.Add(tilePosition, false);
			}
			Stack<Vector2Int> openTileList = new Stack<Vector2Int>();
			for (;;)
			{
				Vector2Int? nextTileToStartSearchFromCandidate = TrainNetworkDefinition.FindNextTileToStartSearchFrom(railTileCoordinates, visitedTile);
				if (nextTileToStartSearchFromCandidate == null)
				{
					break;
				}
				openTileList.Push(nextTileToStartSearchFromCandidate.Value);
				TrainLineDefinition currentTrainLine = trainNetworkDefinition.CreateTrainLine();
				while (openTileList.Count > 0)
				{
					Vector2Int currentTilePosition = openTileList.Pop();
					if (!visitedTile[currentTilePosition])
					{
						visitedTile[currentTilePosition] = true;
						currentTrainLine.AddTrack(new Vector2Int(currentTilePosition.x, currentTilePosition.y), railTileCoordinates[currentTilePosition]);
						int alreadyVisitedCount = 0;
						int directionsAvailableAtTile = 0;
						TileDirection[] array = TileUtilities.NonDiagonalDirections;
						for (int i = 0; i < array.Length; i++)
						{
							Vector2Int directionVector = TileUtilities.GetAdjacencyOffsetForDirection(array[i]);
							Vector2Int tilePosition2 = new Vector2Int(currentTilePosition.x + directionVector.x, currentTilePosition.y + directionVector.y);
							if (railTileCoordinates.ContainsKey(tilePosition2))
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
							if (railTileCoordinates.ContainsKey(diagonalTilePosition))
							{
								TileDirection negativeDirection = TileUtilities.GetRotatedDirection(diagonalDirection, -1);
								TileDirection rotatedDirection = TileUtilities.GetRotatedDirection(diagonalDirection, 1);
								Vector2Int negativeDirectionVector = TileUtilities.GetAdjacencyOffsetForDirection(negativeDirection);
								Vector2Int positiveDirectionVector = TileUtilities.GetAdjacencyOffsetForDirection(rotatedDirection);
								Vector2Int negativeDirectionPosition = currentTilePosition + negativeDirectionVector;
								Vector2Int positiveDirectionPosition = currentTilePosition + positiveDirectionVector;
								bool flag = railTileCoordinates.ContainsKey(negativeDirectionPosition);
								bool hasPositiveTileBase = railTileCoordinates.ContainsKey(positiveDirectionPosition);
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
						currentTrainLine.isLoop = (alreadyVisitedCount == 2);
						currentTrainLine.isValid = (directionsAvailableAtTile <= 2);
					}
				}
			}
			return trainNetworkDefinition;
		}

		// Token: 0x060015E6 RID: 5606 RVA: 0x0004B190 File Offset: 0x00049390
		private static Vector2Int? FindNextTileToStartSearchFrom(Dictionary<Vector2Int, RailType> tilemap, Dictionary<Vector2Int, bool> visitedTiles)
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

		// Token: 0x04001299 RID: 4761
		private readonly List<TrainLineDefinition> _trainLines = new List<TrainLineDefinition>();
	}
}
