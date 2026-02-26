using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using Unity.Profiling;
using UnityEngine;

namespace Motorways.Processes
{
	// Token: 0x02000494 RID: 1172
	public class ReleaseMothballedLanesProcess : IProcess, IReusable
	{
		// Token: 0x06001D04 RID: 7428 RVA: 0x0007083C File Offset: 0x0006EA3C
		public void Step(ISimulation simulation, Fix64 timestep)
		{
			this._connectionsToRelease.Clear();
			foreach (PassageModel passageModel in simulation.GetModels<PassageModel>())
			{
				if (passageModel.State == RoadState.Mothballed)
				{
					Passage passage = passageModel.Passage;
					IList<Vector2Int> crossingCoordinates = passage.CrossingCoordinates;
					ReleaseMothballedLanesProcess.MothballedConnection startConnection = new ReleaseMothballedLanesProcess.MothballedConnection(this._tilemap.GetTileModel(passage.StartCoordinates), TileUtilities.GetDirectionBetweenAdjacentCoordinates(passage.StartCoordinates, crossingCoordinates[0]));
					ReleaseMothballedLanesProcess.MothballedConnection endConnection = null;
					if (passage.IsComplete)
					{
						endConnection = new ReleaseMothballedLanesProcess.MothballedConnection(this._tilemap.GetTileModel(passage.EndCoordinates), TileUtilities.GetDirectionBetweenAdjacentCoordinates(passage.EndCoordinates, crossingCoordinates[crossingCoordinates.Count - 1]));
					}
					if (startConnection.CanRelease(simulation, null) && (endConnection == null || endConnection.CanRelease(simulation, null)))
					{
						this._connectionsToRelease.Add(startConnection);
						for (int crossingIndex = 1; crossingIndex < crossingCoordinates.Count; crossingIndex++)
						{
							TileDirection crossingDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(crossingCoordinates[crossingIndex], crossingCoordinates[crossingIndex - 1]);
							TileModel crossingTile = this._tilemap.GetTileModel(crossingCoordinates[crossingIndex]);
							this._connectionsToRelease.Add(new ReleaseMothballedLanesProcess.MothballedConnection(crossingTile, crossingDirection));
						}
						if (endConnection != null)
						{
							this._connectionsToRelease.Add(endConnection);
						}
						this._upgradeDatabase.ReleaseMothballedUpgrade(passage.UpgradeType, 1);
						simulation.RemoveModel(passageModel);
					}
				}
			}
			this.CollateMothballedConnections(simulation);
			HashSet<Tile> hotSwappableRoundaboutCenters = new HashSet<Tile>();
			foreach (RoundaboutModel roundaboutModel in simulation.GetModels<RoundaboutModel>())
			{
				if (roundaboutModel.State == RoadState.Planned)
				{
					TileModel roundaboutCenter = this._tilemap.GetTileModel(roundaboutModel.OriginCoordinates);
					if (this.CanPlannedRoundaboutHotswap(roundaboutCenter, this._mothballedConnections))
					{
						hotSwappableRoundaboutCenters.Add(roundaboutCenter.Tile);
					}
				}
			}
			foreach (ReleaseMothballedLanesProcess.MothballedConnection mothballedConnection in this._mothballedConnections)
			{
				if (mothballedConnection.CanRelease(simulation, hotSwappableRoundaboutCenters))
				{
					this._connectionsToRelease.Add(mothballedConnection);
				}
			}
			this._mothballedConnections.Clear();
			foreach (RoundaboutModel roundaboutModel2 in simulation.GetModels<RoundaboutModel>())
			{
				if (roundaboutModel2.State == RoadState.Mothballed)
				{
					TileModel centreTileModel = roundaboutModel2.CenterTileModel;
					bool isBlockedByMothballedDiagonalConnection = false;
					foreach (TileDirection diagonalDirection in TileUtilities.DiagonalDirections)
					{
						TileModel diagonalTileModel = centreTileModel.GetAdjacentTileModelInDirection(diagonalDirection);
						TileDirection oppositeDirection = TileUtilities.GetOppositeDirection(diagonalDirection);
						if (diagonalTileModel != null && diagonalTileModel.Tile.HasTwoLaneRoadInDirection(oppositeDirection, RoadState.Mothballed))
						{
							if (diagonalTileModel.roadChunk.GetLanesConnectedToDirection(RoadState.Mothballed, oppositeDirection).Exists((LaneModel lane) => !lane.CanHotswap))
							{
								isBlockedByMothballedDiagonalConnection = true;
								break;
							}
						}
					}
					if (isBlockedByMothballedDiagonalConnection)
					{
						bool flag;
						this.<Step>g__SetRoundaboutWantsHotswap|12_0(centreTileModel, false, null, out flag);
					}
					else
					{
						List<ReleaseMothballedLanesProcess.MothballedConnection> roundaboutConnectionsToRelease = new List<ReleaseMothballedLanesProcess.MothballedConnection>();
						foreach (Tile roundaboutTile in Roundabout.GetTilesInRoundabout(roundaboutModel2.CenterTileModel.Tile, RoadState.Mothballed))
						{
							TileModel roundaboutTileModel = this._tilemap.GetTileModel(roundaboutTile.Coordinates);
							if (Diagnostics.Verify(roundaboutTileModel != null, "There is no corresponding tile model at {0}. This roundabout will not release nicely.", roundaboutTile.Coordinates) && Diagnostics.Verify(roundaboutTile.GetRoundaboutConnection(RoadState.Mothballed).output.direction != TileDirection.None, "The tile at {0} does not have the expected mothballed roundabout connection.", roundaboutTile.Coordinates))
							{
								ReleaseMothballedLanesProcess.MothballedConnection roundaboutConnection = new ReleaseMothballedLanesProcess.MothballedConnection(roundaboutTileModel, roundaboutTile.GetRoundaboutConnection(RoadState.Mothballed));
								if (!roundaboutConnection.CanRelease(simulation, hotSwappableRoundaboutCenters))
								{
									roundaboutConnectionsToRelease.Clear();
									break;
								}
								roundaboutConnectionsToRelease.Add(roundaboutConnection);
							}
						}
						centreTileModel = roundaboutModel2.CenterTileModel;
						Tile centreTile = centreTileModel.Tile;
						if (roundaboutConnectionsToRelease.Count == 0)
						{
							bool canBeReplacedByPlannedRoads = true;
							TileModel eastTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.East));
							TileModel northWestTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.NorthWest));
							TileModel northTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.North));
							TileModel northEastTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.NorthEast));
							TileModel westTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.West));
							TileModel southWestTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.SouthWest));
							TileModel southTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.South));
							TileModel southEastTileModel = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, TileDirection.SouthEast));
							TileModel[] tileModels = new TileModel[]
							{
								eastTileModel,
								northEastTileModel,
								northTileModel,
								northWestTileModel,
								westTileModel,
								southWestTileModel,
								southTileModel,
								southEastTileModel
							};
							TileDirection[] directions = new TileDirection[]
							{
								TileDirection.East,
								TileDirection.NorthEast,
								TileDirection.North,
								TileDirection.NorthWest,
								TileDirection.West,
								TileDirection.SouthWest,
								TileDirection.South,
								TileDirection.SouthEast
							};
							TileDirection[] directionToNext = new TileDirection[]
							{
								TileDirection.North,
								TileDirection.West,
								TileDirection.West,
								TileDirection.South,
								TileDirection.South,
								TileDirection.East,
								TileDirection.East,
								TileDirection.North
							};
							TileDirection[] directionToPrevious = new TileDirection[]
							{
								TileDirection.South,
								TileDirection.East,
								TileDirection.East,
								TileDirection.North,
								TileDirection.North,
								TileDirection.West,
								TileDirection.West,
								TileDirection.South
							};
							TileDirection[] directionToNextAgain = new TileDirection[]
							{
								TileDirection.NorthWest,
								TileDirection.None,
								TileDirection.SouthWest,
								TileDirection.None,
								TileDirection.SouthEast,
								TileDirection.None,
								TileDirection.NorthEast,
								TileDirection.None
							};
							TileDirection[] directionToPreviousAgain = new TileDirection[]
							{
								TileDirection.SouthWest,
								TileDirection.None,
								TileDirection.NorthWest,
								TileDirection.None,
								TileDirection.NorthEast,
								TileDirection.None,
								TileDirection.SouthEast,
								TileDirection.None
							};
							TileDirection[] directionToCentre = new TileDirection[]
							{
								TileDirection.West,
								TileDirection.SouthWest,
								TileDirection.South,
								TileDirection.SouthEast,
								TileDirection.East,
								TileDirection.NorthEast,
								TileDirection.North,
								TileDirection.NorthWest
							};
							int i;
							int tileIndex;
							Predicate<LaneModel> <>9__2;
							for (tileIndex = 0; tileIndex < tileModels.Length - 1; tileIndex = i)
							{
								TileModel tileModel2 = tileModels[tileIndex];
								bool flag2;
								if (tileModel2 == null)
								{
									flag2 = false;
								}
								else
								{
									List<LaneModel> lanes = tileModel2.roadChunk.lanes;
									Predicate<LaneModel> match;
									if ((match = <>9__2) == null)
									{
										match = (<>9__2 = ((LaneModel lane) => ((lane.connection.input.type == RoadType.Roundabout ^ lane.connection.output.type == RoadType.Roundabout) || (tileIndex % 2 == 1 && (lane.connection.input.direction == directionToCentre[tileIndex] || lane.connection.output.direction == directionToCentre[tileIndex]))) && !lane.CanRelease));
									}
									flag2 = lanes.Exists(match);
								}
								if (flag2)
								{
									int otherTileIndex;
									for (otherTileIndex = tileIndex + 1; otherTileIndex < tileModels.Length; otherTileIndex = i)
									{
										TileModel tileModel3 = tileModels[otherTileIndex];
										if (tileModel3 != null && tileModel3.roadChunk.lanes.Exists((LaneModel lane) => ((lane.connection.input.type == RoadType.Roundabout ^ lane.connection.output.type == RoadType.Roundabout) || (otherTileIndex % 2 == 1 && (lane.connection.input.direction == directionToCentre[otherTileIndex] || lane.connection.output.direction == directionToCentre[otherTileIndex]))) && !lane.CanRelease))
										{
											bool isConnected = false;
											int indexDifferent = otherTileIndex - tileIndex;
											int previousIndex = (tileIndex > 0) ? (tileIndex - 1) : (tileModels.Length - 1);
											int nextIndex = (tileIndex + 1) % tileModels.Length;
											TileDirection directConnectionDirection;
											if (indexDifferent == 1)
											{
												directConnectionDirection = directionToNext[tileIndex];
											}
											else if (indexDifferent == tileModels.Length - 1)
											{
												directConnectionDirection = directionToPrevious[tileIndex];
											}
											else if (indexDifferent == 2)
											{
												directConnectionDirection = directionToNextAgain[tileIndex];
											}
											else if (indexDifferent == tileModels.Length - 2)
											{
												directConnectionDirection = directionToPreviousAgain[tileIndex];
											}
											else
											{
												directConnectionDirection = TileDirection.None;
											}
											if (directConnectionDirection != TileDirection.None && tileModels[tileIndex].Tile.HasTwoLaneRoadInDirection(directConnectionDirection, RoadState.ActiveOrPending))
											{
												isConnected = true;
											}
											else if (indexDifferent == 2 && tileModels[nextIndex] != null && tileModels[tileIndex].Tile.HasTwoLaneRoadInDirection(directionToNext[tileIndex], RoadState.ActiveOrPending) && tileModels[nextIndex].Tile.HasTwoLaneRoadInDirection(directionToNext[nextIndex], RoadState.ActiveOrPending))
											{
												isConnected = true;
											}
											else if (indexDifferent == tileModels.Length - 2 && tileModels[previousIndex] != null && tileModels[tileIndex].Tile.HasTwoLaneRoadInDirection(directionToPrevious[tileIndex], RoadState.ActiveOrPending) && tileModels[previousIndex].Tile.HasTwoLaneRoadInDirection(directionToPrevious[previousIndex], RoadState.ActiveOrPending))
											{
												isConnected = true;
											}
											else if (centreTileModel.Tile.HasTwoLaneRoadInDirection(directions[otherTileIndex], RoadState.ActiveOrPending) && centreTileModel.Tile.HasTwoLaneRoadInDirection(directions[tileIndex], RoadState.ActiveOrPending))
											{
												isConnected = true;
											}
											if (!isConnected)
											{
												canBeReplacedByPlannedRoads = false;
											}
										}
										i = otherTileIndex + 1;
									}
								}
								i = tileIndex + 1;
							}
							bool doesWantHotswap = canBeReplacedByPlannedRoads;
							bool canHotswapNow;
							this.<Step>g__SetRoundaboutWantsHotswap|12_0(centreTileModel, doesWantHotswap, roundaboutConnectionsToRelease, out canHotswapNow);
							foreach (LaneModel centreLane in from lane in centreTileModel.roadChunk.lanes
							where lane.connection.input.type == RoadType.Roundabout || lane.connection.output.type == RoadType.Roundabout
							select lane)
							{
								centreLane.IsAboutToHotswap = canBeReplacedByPlannedRoads;
								canHotswapNow &= centreLane.CanHotswap;
							}
							foreach (TileDirection diagonalDirection2 in TileUtilities.DiagonalDirections)
							{
								TileModel diagonalNeighbourTile = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreTile.Coordinates, diagonalDirection2));
								if (diagonalNeighbourTile != null)
								{
									foreach (LaneModel neighbouringLane in diagonalNeighbourTile.roadChunk.lanes)
									{
										if ((neighbouringLane.connection.input.type == RoadType.Roundabout && neighbouringLane.connection.input.direction == TileUtilities.GetOppositeDirection(diagonalDirection2)) || (neighbouringLane.connection.output.type == RoadType.Roundabout && neighbouringLane.connection.output.direction == TileUtilities.GetOppositeDirection(diagonalDirection2)))
										{
											neighbouringLane.IsAboutToHotswap = canBeReplacedByPlannedRoads;
											canHotswapNow &= neighbouringLane.CanHotswap;
										}
									}
								}
							}
							if (!canHotswapNow)
							{
								roundaboutConnectionsToRelease.Clear();
							}
						}
						Vector2Int centreCoordinates = roundaboutModel2.CenterCoordinates;
						foreach (TileDirection diagonalDirection3 in TileUtilities.DiagonalDirections)
						{
							TileModel diagonalNeighbourTile2 = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(centreCoordinates, diagonalDirection3));
							TileDirection oppositeDirection2 = TileUtilities.GetOppositeDirection(diagonalDirection3);
							if (diagonalNeighbourTile2 != null && diagonalNeighbourTile2.Tile.HasTwoLaneRoadInDirection(oppositeDirection2, RoadState.Mothballed) && !diagonalNeighbourTile2.Tile.IsNodePermanent(oppositeDirection2))
							{
								ReleaseMothballedLanesProcess.MothballedConnection mothballedConnection2 = new ReleaseMothballedLanesProcess.MothballedConnection(diagonalNeighbourTile2, oppositeDirection2);
								if (mothballedConnection2.CanRelease(simulation, hotSwappableRoundaboutCenters))
								{
									this._connectionsToRelease.Add(mothballedConnection2);
								}
							}
						}
						if (roundaboutConnectionsToRelease.Count > 0)
						{
							ReleaseMothballedLanesProcess.Log.Info("Releasing roundabout at {0}.", new object[]
							{
								centreCoordinates
							});
							foreach (ReleaseMothballedLanesProcess.MothballedConnection roundaboutConnection2 in roundaboutConnectionsToRelease)
							{
								this._connectionsToRelease.Add(roundaboutConnection2);
							}
						}
					}
				}
			}
			if (this._connectionsToRelease.Any<ReleaseMothballedLanesProcess.MothballedConnection>())
			{
				HashSet<TileModel> modifiedTiles = new HashSet<TileModel>();
				int totalConcreteReleased = 0;
				foreach (ReleaseMothballedLanesProcess.MothballedConnection connectionToRelease in this._connectionsToRelease)
				{
					connectionToRelease.Release();
					int concreteCost = connectionToRelease.ReleaseUpgrades(this._behaviour, this._upgradeDatabase, simulation);
					totalConcreteReleased += concreteCost;
					modifiedTiles.Add(connectionToRelease.GetTileModel(0));
					modifiedTiles.Add(connectionToRelease.GetTileModel(1));
				}
				if (totalConcreteReleased > 0 && this._city.Rules.RecordsGameStatistics())
				{
					this._player.AchievementStatistics.LogDeletedUpgrade(UpgradeType.Concrete, totalConcreteReleased, this._scope.Get<IAchievementHandler>());
				}
				foreach (TileModel tileModel in modifiedTiles)
				{
					if (tileModel.Tile.HasTrafficLight && !TileEditor.TileSupportsTrafficLight(tileModel.Tile))
					{
						tileModel.Tile.HasTrafficLight = false;
						this._upgradeDatabase.MothballUpgrade(UpgradeType.TrafficLight, 1);
						this._upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.TrafficLight, 1);
					}
					TileDirectionBitfield liveNodes = tileModel.Tile.GetTwoLaneRoads(RoadState.Live, Tile.MotorwayInclusion.Ignore);
					TileDirectionBitfield mothballedNodes = tileModel.Tile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore);
					if (mothballedNodes.Count == 1 && liveNodes.Equals(mothballedNodes))
					{
						TileDirection mothballedUTurnDirection = mothballedNodes[0];
						RoadTileConnection uTurnConnection = new RoadTileConnection(new RoadTileNode(mothballedUTurnDirection, RoadType.TwoLane, -1), new RoadTileNode(mothballedUTurnDirection, RoadType.TwoLane, -1));
						if (tileModel.roadChunk.HasLaneForConnection(uTurnConnection))
						{
							ReleaseMothballedLanesProcess.Log.Info("Not adding a u-turn lane on tile {0} in direction {1} because one already exists.", new object[]
							{
								tileModel.Coordinates,
								mothballedUTurnDirection
							});
						}
						else
						{
							RoadTileSignature uTurnSignature = this._scope.Get<RoadTileSignature>();
							uTurnSignature.AddConnection(uTurnConnection);
							RoadTileDefinition uTurnDefinition = this._roadTileAtlas.GetDefinitionForSignature(uTurnSignature);
							this._scope.Release(uTurnSignature);
							bool isEndpointLane = tileModel.Tile.ContentType == TileContentType.House;
							tileModel.AddLane(uTurnConnection, uTurnDefinition, RoadState.Mothballed, isEndpointLane);
						}
					}
				}
				this._connectionsToRelease.Clear();
				simulation.GetModel<CityModel>().OnLanesReleased();
			}
			if (this._tilemap.TemporaryLanes.Count > 0)
			{
				foreach (LaneModel temporaryLane in this._tilemap.TemporaryLanes)
				{
					if (temporaryLane.CanRelease)
					{
						this._temporaryLanesToRelease.Add(temporaryLane);
					}
				}
				if (this._temporaryLanesToRelease.Count > 0)
				{
					foreach (LaneModel temporaryLane2 in this._temporaryLanesToRelease)
					{
						temporaryLane2.roadChunk.RemoveLane(temporaryLane2);
					}
					this._temporaryLanesToRelease.Clear();
				}
			}
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x00071710 File Offset: 0x0006F910
		private bool CanPlannedRoundaboutHotswap(TileModel plannedRoundaboutCenter, HashSet<ReleaseMothballedLanesProcess.MothballedConnection> allMothballedConnections)
		{
			bool wantHotswap = true;
			bool canHotswapNow = true;
			HashSet<LaneModel> laneModelsToHotswap = new HashSet<LaneModel>();
			foreach (Tile roundaboutTile in Roundabout.GetTilesInRoundabout(plannedRoundaboutCenter.Tile, RoadState.Planned))
			{
				if (roundaboutTile.IsPlannedRoundaboutBlocked)
				{
					RoadTileConnection roundaboutConnection = roundaboutTile.GetRoundaboutConnection(RoadState.Planned);
					foreach (LaneModel roadChunkLane in this._tilemap.GetTileModel(roundaboutTile.Coordinates).roadChunk.lanes)
					{
						if (roadChunkLane.connection.input.type == RoadType.Roundabout || roadChunkLane.connection.output.type == RoadType.Roundabout)
						{
							wantHotswap = false;
							canHotswapNow = false;
							break;
						}
						TileDirectionBitfield invalidExitDirections = Roundabout.GetInvalidExitsForConnection(roundaboutConnection.input.direction, roundaboutConnection.output.direction);
						if (invalidExitDirections[roadChunkLane.connection.input.direction])
						{
							laneModelsToHotswap.Add(roadChunkLane);
							foreach (LaneModel inboundRoadChunkLane in roadChunkLane.InboundLanes)
							{
								laneModelsToHotswap.Add(inboundRoadChunkLane);
							}
							if (!roadChunkLane.CanHotswap)
							{
								canHotswapNow = false;
							}
						}
						if (invalidExitDirections[roadChunkLane.connection.output.direction])
						{
							laneModelsToHotswap.Add(roadChunkLane);
							foreach (LaneModel outboundRoadChunkLane in roadChunkLane.OutboundLanes)
							{
								laneModelsToHotswap.Add(outboundRoadChunkLane);
							}
							if (!roadChunkLane.CanHotswap)
							{
								canHotswapNow = false;
							}
						}
					}
				}
			}
			Tile centreTile = Roundabout.GetCenterTile(plannedRoundaboutCenter.Tile, RoadState.Planned);
			TileModel centreTileModel = this._tilemap.GetTileModel(centreTile.Coordinates);
			foreach (LaneModel centreChunkLane in centreTileModel.roadChunk.lanes)
			{
				if (Diagnostics.Verify(centreChunkLane.state == RoadState.Mothballed, "Found non-mothballed lane {0} for tile {1} which is supposed to be the centre tile of a planned roundabout!", centreChunkLane, centreTile))
				{
					laneModelsToHotswap.Add(centreChunkLane);
					if (centreChunkLane.connection.input.type == RoadType.Roundabout || centreChunkLane.connection.output.type == RoadType.Roundabout)
					{
						canHotswapNow = false;
						wantHotswap = false;
					}
					else
					{
						if (!centreChunkLane.CanHotswap)
						{
							canHotswapNow = false;
						}
						if (centreChunkLane.connection.IsRoundabout)
						{
							canHotswapNow = false;
						}
						if (TileUtilities.IsDirectionDiagonal(centreChunkLane.connection.output.direction))
						{
							foreach (LaneModel cornerTileLane in centreChunkLane.OutboundLanes)
							{
								laneModelsToHotswap.Add(cornerTileLane);
								if (!cornerTileLane.CanHotswap)
								{
									canHotswapNow = false;
								}
							}
						}
						if (TileUtilities.IsDirectionDiagonal(centreChunkLane.connection.input.direction))
						{
							foreach (LaneModel cornerTileLane2 in centreChunkLane.InboundLanes)
							{
								laneModelsToHotswap.Add(cornerTileLane2);
								if (!cornerTileLane2.CanHotswap)
								{
									canHotswapNow = false;
								}
							}
						}
					}
				}
			}
			if (allMothballedConnections != null)
			{
				foreach (ReleaseMothballedLanesProcess.MothballedConnection incomingMothballedConnection in from connection in allMothballedConnections
				where connection.GetTileModel(0) == centreTileModel || connection.GetTileModel(1) == centreTileModel
				select connection)
				{
					if (!incomingMothballedConnection.CanBeReplacedByRoundabout)
					{
						canHotswapNow = false;
					}
					if (TileUtilities.IsDirectionDiagonal(incomingMothballedConnection.GetDirection(0)))
					{
						if (((incomingMothballedConnection.GetTileModel(0) == centreTileModel) ? incomingMothballedConnection.GetTileModel(1) : incomingMothballedConnection.GetTileModel(0)).roadChunk.lanes.Exists((LaneModel lane) => !lane.connection.IsUTurn && lane.state == RoadState.Mothballed))
						{
							wantHotswap = false;
							canHotswapNow = false;
						}
					}
				}
			}
			foreach (LaneModel laneModel in laneModelsToHotswap)
			{
				laneModel.IsAboutToHotswap = wantHotswap;
			}
			return canHotswapNow;
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x00071C44 File Offset: 0x0006FE44
		private void CollateMothballedConnections(ISimulation simulation)
		{
			this._mothballedConnections.Clear();
			foreach (AdjacentTileConnection mothballedConnection in this._tilemap.MothballedTileConnections)
			{
				this._mothballedConnections.Add(new ReleaseMothballedLanesProcess.MothballedConnection(this._tilemap.GetTileModel(mothballedConnection.OriginCoordinates), mothballedConnection.OriginDirection));
			}
			foreach (RoundaboutModel roundaboutModel in simulation.GetModels<RoundaboutModel>())
			{
				foreach (TileDirection mothballedDirection in roundaboutModel.CenterTileModel.Tile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore))
				{
					if (roundaboutModel.CenterTileModel.GetAdjacentTileModelInDirection(mothballedDirection).Tile.GetTwoLaneRoadStateInDirection(TileUtilities.GetOppositeDirection(mothballedDirection)) != RoadState.Mothballed)
					{
						this._mothballedConnections.Add(new ReleaseMothballedLanesProcess.MothballedConnection(roundaboutModel.CenterTileModel, mothballedDirection));
					}
				}
			}
			foreach (MotorwayModel motorwayModel in simulation.GetModels<MotorwayModel>())
			{
				if (motorwayModel.State == RoadState.Mothballed)
				{
					this._mothballedConnections.Add(new ReleaseMothballedLanesProcess.MothballedConnection(motorwayModel));
				}
			}
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x00071D9C File Offset: 0x0006FF9C
		public void Reset()
		{
			this._mothballedConnections.Clear();
			this._connectionsToRelease.Clear();
			this._temporaryLanesToRelease.Clear();
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x00071E24 File Offset: 0x00070024
		[CompilerGenerated]
		private void <Step>g__SetRoundaboutWantsHotswap|12_0(TileModel roundaboutCentre, bool doesWantHotswap, List<ReleaseMothballedLanesProcess.MothballedConnection> roundaboutConnectionsToRelease, out bool canHotswapNow)
		{
			canHotswapNow = doesWantHotswap;
			foreach (Tile roundaboutTile in Roundabout.GetTilesInRoundabout(roundaboutCentre.Tile, RoadState.Mothballed))
			{
				TileModel roundaboutTileModel = this._tilemap.GetTileModel(roundaboutTile.Coordinates);
				foreach (LaneModel roundaboutLaneModel in from lane in roundaboutTileModel.roadChunk.lanes
				where lane.connection.input.type == RoadType.Roundabout || lane.connection.output.type == RoadType.Roundabout
				select lane)
				{
					roundaboutLaneModel.IsAboutToHotswap = doesWantHotswap;
					canHotswapNow &= roundaboutLaneModel.CanHotswap;
					if (roundaboutLaneModel.connection.IsRoundabout)
					{
						foreach (LaneModel inboundLane in roundaboutLaneModel.InboundLanes)
						{
							inboundLane.IsAboutToHotswap = doesWantHotswap;
							canHotswapNow &= inboundLane.CanHotswap;
						}
						foreach (LaneModel outboundLane in roundaboutLaneModel.OutboundLanes)
						{
							outboundLane.IsAboutToHotswap = doesWantHotswap;
							canHotswapNow &= outboundLane.CanHotswap;
						}
					}
				}
				if (canHotswapNow && Diagnostics.Verify(roundaboutConnectionsToRelease != null))
				{
					ReleaseMothballedLanesProcess.MothballedConnection roundaboutConnection = new ReleaseMothballedLanesProcess.MothballedConnection(roundaboutTileModel, roundaboutTile.GetRoundaboutConnection(RoadState.Mothballed));
					roundaboutConnectionsToRelease.Add(roundaboutConnection);
				}
			}
		}

		// Token: 0x040018F4 RID: 6388
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("ReleaseMothballedLanesProcess");

		// Token: 0x040018F5 RID: 6389
		[Serialize(false, null)]
		private readonly HashSet<ReleaseMothballedLanesProcess.MothballedConnection> _mothballedConnections = new HashSet<ReleaseMothballedLanesProcess.MothballedConnection>();

		// Token: 0x040018F6 RID: 6390
		[Serialize(false, null)]
		private readonly HashSet<ReleaseMothballedLanesProcess.MothballedConnection> _connectionsToRelease = new HashSet<ReleaseMothballedLanesProcess.MothballedConnection>();

		// Token: 0x040018F7 RID: 6391
		[Serialize(false, null)]
		private readonly List<LaneModel> _temporaryLanesToRelease = new List<LaneModel>();

		// Token: 0x040018F8 RID: 6392
		[Dependency]
		private IScope _scope;

		// Token: 0x040018F9 RID: 6393
		[Dependency]
		private UpgradeDatabaseModel _upgradeDatabase;

		// Token: 0x040018FA RID: 6394
		[Dependency]
		private RoadTileAtlas _roadTileAtlas;

		// Token: 0x040018FB RID: 6395
		[Dependency]
		private City _city;

		// Token: 0x040018FC RID: 6396
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x040018FD RID: 6397
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x040018FE RID: 6398
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x040018FF RID: 6399
		private static readonly ProfilerMarker Profiler_Step = new ProfilerMarker(ProfilerCategory.Scripts, "ReleaseMothballedLanesProcess.Step()");

		// Token: 0x04001900 RID: 6400
		private static readonly ProfilerMarker Profiler_CollateMothballedConnections = new ProfilerMarker(ProfilerCategory.Scripts, "ReleaseMothballedLanesProcess.CollateMothballedConnections()");

		// Token: 0x02000495 RID: 1173
		private class MothballedConnection
		{
			// Token: 0x06001D0B RID: 7435 RVA: 0x0007201C File Offset: 0x0007021C
			public TileModel GetTileModel(int index)
			{
				if (this._motorway == null)
				{
					if (index != 0)
					{
						return this._tileModel.GetAdjacentTileModelInDirection(this._direction);
					}
					return this._tileModel;
				}
				else
				{
					if (index != 0)
					{
						return this._motorway.EndTile;
					}
					return this._motorway.StartTile;
				}
			}

			// Token: 0x06001D0C RID: 7436 RVA: 0x0007205C File Offset: 0x0007025C
			public TileDirection GetDirection(int index)
			{
				if (this._motorway == null)
				{
					if (index != 0)
					{
						return TileUtilities.GetOppositeDirection(this._direction);
					}
					return this._direction;
				}
				else
				{
					if (index != 0)
					{
						return this._motorway.EndDirection;
					}
					return this._motorway.StartDirection;
				}
			}

			// Token: 0x06001D0D RID: 7437 RVA: 0x00072096 File Offset: 0x00070296
			public TileCornerModel GetTileCornerModel()
			{
				if (this._motorway == null && TileUtilities.IsDirectionDiagonal(this._direction))
				{
					return this._tileModel.GetAdjacentTileCornerModelInDirection(this._direction);
				}
				return null;
			}

			// Token: 0x1700056E RID: 1390
			// (get) Token: 0x06001D0E RID: 7438 RVA: 0x000720C0 File Offset: 0x000702C0
			private List<LaneModel> AllLanes
			{
				get
				{
					List<LaneModel> lanes = new List<LaneModel>();
					lanes.AddRange(this.GetTileModel(0).roadChunk.GetLanesConnectedToDirection(RoadState.Mothballed, this.GetDirection(0)));
					lanes.AddRange(this.GetTileModel(1).roadChunk.GetLanesConnectedToDirection(RoadState.Mothballed, this.GetDirection(1)));
					TileCornerModel tileCornerModel = this.GetTileCornerModel();
					if (tileCornerModel != null)
					{
						TileDirectionBitfield cornerDirections = default(TileDirectionBitfield);
						cornerDirections[this.GetDirection(0)] = true;
						cornerDirections[this.GetDirection(1)] = true;
						lanes.AddRange(tileCornerModel.roadChunk.GetLanesConnectedToDirections(RoadState.Mothballed, cornerDirections));
					}
					if (this._motorway != null)
					{
						lanes.AddRange(this._motorway.roadChunk.lanes);
					}
					return lanes;
				}
			}

			// Token: 0x1700056F RID: 1391
			// (get) Token: 0x06001D0F RID: 7439 RVA: 0x00072176 File Offset: 0x00070376
			public bool CanBeReplacedByRoundabout
			{
				get
				{
					return this.AllLanes.All((LaneModel laneModel) => laneModel.CanHotswap);
				}
			}

			// Token: 0x06001D10 RID: 7440 RVA: 0x000721A4 File Offset: 0x000703A4
			public void Release()
			{
				ReleaseMothballedLanesProcess.Log.Info("Releasing connection {0}.", new object[]
				{
					this
				});
				HashSet<VehicleModel> inboundVehiclesToRepath = new HashSet<VehicleModel>();
				HashSet<VehicleModel> returnInboundVehiclesToRepath = new HashSet<VehicleModel>();
				foreach (LaneModel lane in this.AllLanes)
				{
					foreach (RoadChunkModel.InboundVehicle inboundVehicle in lane.roadChunk.inboundVehicles)
					{
						if (inboundVehicle.chosenLane == lane)
						{
							inboundVehiclesToRepath.Add(inboundVehicle.vehicle);
						}
					}
					foreach (RoadChunkModel.InboundVehicle returningInboundVehicle in lane.roadChunk.returningInboundVehicles)
					{
						if (returningInboundVehicle.chosenLane == lane)
						{
							returnInboundVehiclesToRepath.Add(returningInboundVehicle.vehicle);
						}
					}
					ReleaseMothballedLanesProcess.Log.Info("Removing lane {0}.", new object[]
					{
						lane
					});
					lane.roadChunk.RemoveLane(lane);
				}
				if (inboundVehiclesToRepath.Count > 0 || returnInboundVehiclesToRepath.Count > 0)
				{
					ReleaseMothballedLanesProcess.Log.Info("Going to clear and request repaths of {0} incoming and {1} outgoing vehicles", new object[]
					{
						inboundVehiclesToRepath.Count,
						returnInboundVehiclesToRepath.Count
					});
				}
				foreach (VehicleModel vehicle in inboundVehiclesToRepath)
				{
					if (Diagnostics.Verify(vehicle != null, "Why does a lane on {0} have a null inbound vehicle?", this))
					{
						vehicle.ClearNonCommittedLanes();
						vehicle.RequestPathfind(VehicleModel.PathfindUrgency.AsSoonAsPossible);
					}
				}
				foreach (VehicleModel vehicle2 in returnInboundVehiclesToRepath)
				{
					if (Diagnostics.Verify(vehicle2 != null, "Why does a lane on {0} have a null inbound vehicle?", this))
					{
						vehicle2.ClearReturnPath();
						vehicle2.RequestReturnPathfind(VehicleModel.PathfindUrgency.AsSoonAsPossible);
					}
				}
				if (this._motorway != null)
				{
					if (this._replacementPendingMotorway != null)
					{
						this._replacementPendingMotorway.isHighBuildPriority = true;
						List<VehicleModel> vehiclesInboundToMotorway = new List<VehicleModel>();
						foreach (RoadChunkModel.InboundVehicle inboundVehicle2 in this._motorway.roadChunk.inboundVehicles)
						{
							vehiclesInboundToMotorway.Add(inboundVehicle2.vehicle);
						}
						foreach (VehicleModel vehicle3 in vehiclesInboundToMotorway)
						{
							ReleaseMothballedLanesProcess.Log.Info("Clearing paths for vehicle {0}.", new object[]
							{
								vehicle3.id
							});
							vehicle3.ClearNonCommittedLanes();
							vehicle3.RequestPathfind(VehicleModel.PathfindUrgency.AsSoonAsPossible);
						}
						vehiclesInboundToMotorway.Clear();
						foreach (RoadChunkModel.InboundVehicle inboundVehicle3 in this._motorway.roadChunk.returningInboundVehicles)
						{
							vehiclesInboundToMotorway.Add(inboundVehicle3.vehicle);
						}
						foreach (VehicleModel vehicle4 in vehiclesInboundToMotorway)
						{
							ReleaseMothballedLanesProcess.Log.Info("Clearing return paths for vehicle {0}.", new object[]
							{
								vehicle4.id
							});
							vehicle4.ClearReturnPath();
							vehicle4.RequestReturnPathfind(VehicleModel.PathfindUrgency.AsSoonAsPossible);
						}
					}
					this._motorway.SetMotorwayAndNodeState(RoadState.None);
					return;
				}
				if (this._roundaboutInputDirection != TileDirection.None)
				{
					this.GetTileModel(0).Tile.SetRoundaboutState(this._roundaboutInputDirection, this._direction, RoadState.None);
					return;
				}
				this.GetTileModel(0).Tile.SetNodeState(new RoadTileNode(this.GetDirection(0), RoadType.TwoLane, -1), RoadState.None, Tile.TileChangePermissions.Full);
				this.GetTileModel(1).Tile.SetNodeState(new RoadTileNode(this.GetDirection(1), RoadType.TwoLane, -1), RoadState.None, Tile.TileChangePermissions.Full);
			}

			// Token: 0x06001D11 RID: 7441 RVA: 0x00072614 File Offset: 0x00070814
			private bool IsReleasingOfConcreteHandledByRoundabout(ISimulation simulation)
			{
				foreach (RoundaboutModel roundaboutModel in simulation.GetModels<RoundaboutModel>())
				{
					foreach (AdjacentTileConnection handledConnection in roundaboutModel.ReplacedConnections)
					{
						if ((this.GetTileModel(0).Tile.Coordinates == handledConnection.OriginCoordinates && this.GetTileModel(1).Tile.Coordinates == handledConnection.DestinationCoordinates) | (this.GetTileModel(1).Tile.Coordinates == handledConnection.OriginCoordinates && this.GetTileModel(0).Tile.Coordinates == handledConnection.DestinationCoordinates))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x06001D12 RID: 7442 RVA: 0x0007270C File Offset: 0x0007090C
			public int ReleaseUpgrades(GameBehaviourModel behaviour, UpgradeDatabase upgradeDatabase, ISimulation simulation)
			{
				int concreteCost = 0;
				if (this._motorway == null)
				{
					if (this._roundaboutInputDirection == TileDirection.None)
					{
						if (this.IsReleasingOfConcreteHandledByRoundabout(simulation))
						{
							return 0;
						}
						Tile tile0 = this.GetTileModel(0).Tile;
						Tile tile = this.GetTileModel(1).Tile;
						bool flag = tile0.StateOfRoadInDirection(this._direction) != RoadState.None;
						RoadState stateOfRoadFromTile1ToTile0 = tile.StateOfRoadInDirection(TileUtilities.GetOppositeDirection(this._direction));
						if (!flag && stateOfRoadFromTile1ToTile0 == RoadState.None)
						{
							concreteCost = behaviour.GetConcreteCostForConnection(tile0, tile);
						}
					}
					else if (Roundabout.DoesConnectionOwnRoundabout(new RoadTileConnection(new RoadTileNode(this._roundaboutInputDirection, RoadType.Roundabout, -1), new RoadTileNode(this._direction, RoadType.Roundabout, -1))))
					{
						upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Roundabout, 1);
					}
				}
				if (concreteCost > 0)
				{
					upgradeDatabase.ReleaseMothballedUpgrade(UpgradeType.Concrete, concreteCost);
				}
				return concreteCost;
			}

			// Token: 0x06001D13 RID: 7443 RVA: 0x000727C0 File Offset: 0x000709C0
			private bool CanMotorwayRelease(ISimulation simulation)
			{
				bool motorwayHasInboundVehicles = false;
				foreach (LaneModel lane in this.AllLanes)
				{
					if (lane.HasTraversingOrCommittedVehicles)
					{
						return false;
					}
					motorwayHasInboundVehicles |= lane.roadChunk.DoesLaneHaveAnyInboundVehicles(lane);
				}
				if (!motorwayHasInboundVehicles)
				{
					ReleaseMothballedLanesProcess.Log.Info("Permitting motorway {0} to release because it has no traversing, committed, or inbound vehicles.", new object[]
					{
						this._motorway.Id
					});
					return true;
				}
				foreach (MotorwayModel otherMotorway in simulation.GetModels<MotorwayModel>())
				{
					if (otherMotorway.State == RoadState.Planned)
					{
						bool pendingMotorwayStartTileMatches = otherMotorway.StartCoordinates == this._motorway.StartCoordinates || otherMotorway.StartCoordinates == this._motorway.EndCoordinates;
						bool pendingMotorwayEndTileMatches = otherMotorway.EndCoordinates == this._motorway.StartCoordinates || otherMotorway.EndCoordinates == this._motorway.EndCoordinates;
						if (pendingMotorwayStartTileMatches || pendingMotorwayEndTileMatches)
						{
							if (pendingMotorwayStartTileMatches && pendingMotorwayEndTileMatches)
							{
								this._replacementPendingMotorway = otherMotorway;
								break;
							}
							int mothballedMotorwayOnStartTile = otherMotorway.StartTile.Tile.GetMotorwayInDirection(otherMotorway.StartDirection, RoadState.Mothballed);
							int mothballedMotorwayOnEndTile = otherMotorway.EndTile.Tile.GetMotorwayInDirection(otherMotorway.EndDirection, RoadState.Mothballed);
							bool flag = mothballedMotorwayOnStartTile == -1 || mothballedMotorwayOnStartTile == this._motorway.Id;
							bool canBuildEndNode = mothballedMotorwayOnEndTile == -1 || mothballedMotorwayOnEndTile == this._motorway.Id;
							if (flag && canBuildEndNode)
							{
								TileModel pendingMotorwayTile;
								TileModel mothballedMotorwayTile;
								if (pendingMotorwayStartTileMatches)
								{
									pendingMotorwayTile = otherMotorway.EndTile;
									mothballedMotorwayTile = ((otherMotorway.StartCoordinates == this._motorway.StartCoordinates) ? this._motorway.EndTile : this._motorway.StartTile);
								}
								else
								{
									pendingMotorwayTile = otherMotorway.StartTile;
									mothballedMotorwayTile = ((otherMotorway.EndCoordinates == this._motorway.StartCoordinates) ? this._motorway.EndTile : this._motorway.StartTile);
								}
								LaneModel startLane = null;
								foreach (LaneModel mothballedMotorwayLane in mothballedMotorwayTile.roadChunk.lanes)
								{
									if (mothballedMotorwayLane.state == RoadState.Active)
									{
										startLane = mothballedMotorwayLane;
										break;
									}
								}
								LaneModel endLane = null;
								foreach (LaneModel pendingMotorwayLane in pendingMotorwayTile.roadChunk.lanes)
								{
									if (pendingMotorwayLane.state == RoadState.Active)
									{
										endLane = pendingMotorwayLane;
										break;
									}
								}
								if (startLane != null && endLane != null && simulation.Scope.Get<Pathfinder>().AreLanesConnected(startLane, endLane, false))
								{
									this._replacementPendingMotorway = otherMotorway;
									break;
								}
							}
						}
					}
				}
				if (this._replacementPendingMotorway != null)
				{
					ReleaseMothballedLanesProcess.Log.Info("Permitting motorway {0} to release because it has no traversing or committed vehicles, and all inbound vehicles can use the pending motorway {1}.", new object[]
					{
						this._motorway.Id,
						this._replacementPendingMotorway.Id
					});
					return true;
				}
				return false;
			}

			// Token: 0x06001D14 RID: 7444 RVA: 0x00072B28 File Offset: 0x00070D28
			public bool CanRelease(ISimulation simulation, HashSet<Tile> hotSwappableRoundaboutCentres = null)
			{
				if (this._motorway != null)
				{
					return this.CanMotorwayRelease(simulation);
				}
				bool needsReplacementRoundabout = false;
				foreach (LaneModel lane in this.AllLanes)
				{
					if (!lane.CanRelease)
					{
						if (!lane.CanHotswap)
						{
							return false;
						}
						needsReplacementRoundabout = true;
					}
				}
				if (needsReplacementRoundabout)
				{
					Tile startTile = this.GetTileModel(0).Tile;
					Tile endTile = this.GetTileModel(1).Tile;
					bool isConnectionToCenter = Roundabout.IsTileCenterOfRoundabout(startTile, RoadState.Planned) || Roundabout.IsTileCenterOfRoundabout(endTile, RoadState.Planned);
					bool flag = isConnectionToCenter || startTile.HasRoundabout(RoadState.Planned) || endTile.HasRoundabout(RoadState.Planned);
					bool isDiagonal = TileUtilities.IsDirectionDiagonal(this._direction) && isConnectionToCenter;
					if (!flag)
					{
						return false;
					}
					if (((!startTile.HasRoundabout(RoadState.Planned) && !Roundabout.IsTileCenterOfRoundabout(startTile, RoadState.Planned)) || (!endTile.HasRoundabout(RoadState.Planned) && !Roundabout.IsTileCenterOfRoundabout(endTile, RoadState.Planned))) && !isDiagonal)
					{
						return false;
					}
					TileModel aRoundaboutTile = startTile.HasRoundabout(RoadState.Planned) ? this.GetTileModel(0) : (endTile.HasRoundabout(RoadState.Planned) ? this.GetTileModel(1) : (Roundabout.IsTileCenterOfRoundabout(startTile, RoadState.VisiblyActive) ? this.GetTileModel(0).GetAdjacentTileModelInDirection(TileDirection.North) : this.GetTileModel(1).GetAdjacentTileModelInDirection(TileDirection.North)));
					if (aRoundaboutTile == null)
					{
						return false;
					}
					Tile centreTile = Roundabout.GetCenterTile(aRoundaboutTile.Tile, RoadState.Planned);
					if (!Diagnostics.Verify(hotSwappableRoundaboutCentres != null, "hotSwappableRoundaboutCentres is null in CanRelease when it's needed") || !hotSwappableRoundaboutCentres.Contains(centreTile))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x06001D15 RID: 7445 RVA: 0x00072CBC File Offset: 0x00070EBC
			public MothballedConnection(TileModel tileModel, TileDirection direction)
			{
				this._tileModel = tileModel;
				this._direction = direction;
				this._roundaboutInputDirection = TileDirection.None;
				this._motorway = null;
			}

			// Token: 0x06001D16 RID: 7446 RVA: 0x00072CE0 File Offset: 0x00070EE0
			public MothballedConnection(TileModel tileModel, RoadTileConnection roundaboutConnection)
			{
				this._tileModel = tileModel;
				this._direction = roundaboutConnection.output.direction;
				this._roundaboutInputDirection = roundaboutConnection.input.direction;
				this._motorway = null;
			}

			// Token: 0x06001D17 RID: 7447 RVA: 0x00072D18 File Offset: 0x00070F18
			public MothballedConnection(MotorwayModel motorwayModel)
			{
				this._tileModel = null;
				this._direction = TileDirection.None;
				this._roundaboutInputDirection = TileDirection.None;
				this._motorway = motorwayModel;
			}

			// Token: 0x06001D18 RID: 7448 RVA: 0x00072D3C File Offset: 0x00070F3C
			public override bool Equals(object obj)
			{
				ReleaseMothballedLanesProcess.MothballedConnection mothballedConnection = obj as ReleaseMothballedLanesProcess.MothballedConnection;
				return mothballedConnection != null && this.Equals(mothballedConnection);
			}

			// Token: 0x06001D19 RID: 7449 RVA: 0x00072D5C File Offset: 0x00070F5C
			public bool Equals(ReleaseMothballedLanesProcess.MothballedConnection other)
			{
				return this._motorway == other._motorway && (this._motorway != null || ((this._tileModel == other.GetTileModel(0) && this._direction == other.GetDirection(0)) | (this._tileModel == other.GetTileModel(1) && this._direction == other.GetDirection(1))));
			}

			// Token: 0x06001D1A RID: 7450 RVA: 0x00072DC8 File Offset: 0x00070FC8
			public override int GetHashCode()
			{
				if (this._motorway == null)
				{
					return this._tileModel.Coordinates.GetHashCode() ^ this._tileModel.GetAdjacentTileModelInDirection(this._direction).Coordinates.GetHashCode();
				}
				return this._motorway.GetHashCode();
			}

			// Token: 0x06001D1B RID: 7451 RVA: 0x00072E28 File Offset: 0x00071028
			public override string ToString()
			{
				if (this._motorway != null)
				{
					return string.Format("[MothballedConnection Motorway={0}]", this._motorway);
				}
				if (this._roundaboutInputDirection != TileDirection.None)
				{
					return string.Format("[MothballedConnection Tile={0}, RoundaboutConnection={1} -> {2}]", this._tileModel.Coordinates, this._roundaboutInputDirection, this._direction);
				}
				return string.Format("[MothballedConnection Tile={0}, Direction={1}]", this._tileModel.Coordinates, this._direction);
			}

			// Token: 0x04001901 RID: 6401
			private readonly TileModel _tileModel;

			// Token: 0x04001902 RID: 6402
			private readonly TileDirection _direction;

			// Token: 0x04001903 RID: 6403
			private readonly TileDirection _roundaboutInputDirection;

			// Token: 0x04001904 RID: 6404
			private readonly MotorwayModel _motorway;

			// Token: 0x04001905 RID: 6405
			private MotorwayModel _replacementPendingMotorway;
		}
	}
}
