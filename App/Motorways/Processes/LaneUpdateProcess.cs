using System;
using System.Collections.Generic;
using System.Linq;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways.Processes
{
	// Token: 0x02000491 RID: 1169
	public class LaneUpdateProcess : IProcess, IReusable
	{
		// Token: 0x06001CF7 RID: 7415 RVA: 0x0006F35C File Offset: 0x0006D55C
		public void Step(ISimulation simulation, Fix64 deltaTime)
		{
			this._tilemap.ActivateUnblockedPendingLanes();
			this._city = simulation.GetModel<CityModel>();
			bool didAddLane = false;
			foreach (TileModel tileModel in this._tilemap.ChangedTiles)
			{
				Tile tile = tileModel.Tile;
				if (tile.ContentType != TileContentType.Carpark)
				{
					RoadTileSignature activeSignature = tile.CreateSignature(RoadState.Active);
					RoadTileSignature liveSignature = tile.CreateSignature(RoadState.Live);
					List<RoadTileConnection> activeConnections = new List<RoadTileConnection>(activeSignature.Connections);
					List<RoadTileConnection> liveConnections = new List<RoadTileConnection>(liveSignature.Connections);
					foreach (LaneModel laneModel in tileModel.roadChunk.lanes)
					{
						bool isConnectionActive = activeConnections.Contains(laneModel.connection);
						bool isLaneActive = laneModel.state == RoadState.Active;
						if (isConnectionActive && !isLaneActive)
						{
							laneModel.state = RoadState.Active;
							this._lanesChanged.Add(laneModel);
							this.TryAddUpdatedTileCorner(tileModel, laneModel);
						}
						else if (!isConnectionActive && isLaneActive)
						{
							laneModel.state = RoadState.Mothballed;
							this._lanesChanged.Add(laneModel);
							this.TryAddUpdatedTileCorner(tileModel, laneModel);
						}
						if (!isConnectionActive && tile.ContentType != TileContentType.House && !liveSignature.HasConnection(laneModel.connection))
						{
							if (laneModel.connection.IsUTurn && !laneModel.isTemporary)
							{
								this._tilemap.TemporaryLanes.Add(laneModel);
								laneModel.isTemporary = true;
							}
						}
						else if (laneModel.isTemporary)
						{
							this._tilemap.TemporaryLanes.Remove(laneModel);
							laneModel.isTemporary = false;
						}
						if (isConnectionActive)
						{
							activeConnections.Remove(laneModel.connection);
						}
						liveConnections.Remove(laneModel.connection);
					}
					if (liveConnections.Count > 0)
					{
						RoadTileDefinition definition = this._atlas.GetDefinitionForSignature(liveSignature);
						if (Diagnostics.Verify(definition != null, "Tile at {0} has invalid live signature {1}.", tile.Coordinates, liveSignature))
						{
							foreach (RoadTileConnection newConnection in liveConnections)
							{
								RoadState newLaneState = RoadState.Mothballed;
								if (activeConnections.Contains(newConnection))
								{
									newLaneState = RoadState.Active;
									activeConnections.Remove(newConnection);
								}
								bool isEndpointLane = tile.ContentType == TileContentType.House;
								LaneModel newLane = tileModel.AddLane(newConnection, definition, newLaneState, isEndpointLane);
								this._lanesChanged.Add(newLane);
								didAddLane = true;
								TileCornerModel cornerModel = this.TryAddUpdatedTileCorner(tileModel, newLane);
								ModelList<TrainCrossingModel> crossingModels = simulation.GetModels<TrainCrossingModel>();
								bool isRepeatTile = false;
								ModelListEnumerator<TrainCrossingModel> enumerator4 = crossingModels.GetEnumerator();
								while (enumerator4.MoveNext())
								{
									if (enumerator4.Current.RoadChunkModel == tileModel.roadChunk)
									{
										isRepeatTile = true;
										break;
									}
								}
								if (!isRepeatTile)
								{
									if (cornerModel != null && !cornerModel.roadChunk.IsRoundabout && this._city.Mode != GameMode.Background && !isEndpointLane)
									{
										List<TileModel> tileModels = this._tilemap.ChangedTiles.ToList<TileModel>();
										int startingIndex = -1;
										for (int tileModelIndex = 0; tileModelIndex < tileModels.Count; tileModelIndex++)
										{
											if (tileModels[tileModelIndex] == tileModel)
											{
												startingIndex = tileModelIndex;
											}
										}
										if (startingIndex == -1)
										{
											continue;
										}
										if (tileModels.Count >= 2)
										{
											int secondaryIndex = startingIndex + 1;
											if (secondaryIndex >= tileModels.Count)
											{
												secondaryIndex = startingIndex - 1;
											}
											List<Vector2Int> possibleRailPositions = TileUtilities.GetThePerpendicularDiagonalPositions(tileModels[startingIndex].Coordinates, tileModels[secondaryIndex].Coordinates);
											if (possibleRailPositions == null)
											{
												continue;
											}
											int numberOfRailPositions = 0;
											foreach (Vector2Int currentTilePosition in possibleRailPositions)
											{
												TileModel adjacentTile = this._tilemap.GetTileModel(currentTilePosition);
												if (adjacentTile != null && adjacentTile.Tile.HasRailConnection)
												{
													numberOfRailPositions++;
												}
											}
											if (numberOfRailPositions != 0 && numberOfRailPositions == possibleRailPositions.Count && cornerModel.roadChunk.TrainCrossingModel == null && tileModel.roadChunk != cornerModel.roadChunk)
											{
												Vector2Int direction = tileModels[secondaryIndex].Coordinates - tileModels[startingIndex].Coordinates;
												TileDirection oppositeTileDirection = TileUtilities.GetClosestDirection(-direction);
												CornerAdjacencyReference otherDirectionAdjacencyReference = new CornerAdjacencyReference(tileModels[secondaryIndex].Coordinates, oppositeTileDirection);
												TileCornerModel tileCornerModel = this._tilemap.GetTileCornerModel(otherDirectionAdjacencyReference);
												bool flag;
												if (tileCornerModel == null)
												{
													flag = (null != null);
												}
												else
												{
													RoadChunkModel roadChunk = tileCornerModel.roadChunk;
													flag = (((roadChunk != null) ? roadChunk.TrainCrossingModel : null) != null);
												}
												if (flag)
												{
													break;
												}
												cornerModel.roadChunk.TrainCrossingModel = simulation.Scope.Get<TrainCrossingModel>();
												cornerModel.roadChunk.TrainCrossingModel.Initialize(tile, cornerModel.roadChunk, direction);
												simulation.AddModel(cornerModel.roadChunk.TrainCrossingModel);
											}
										}
									}
									this.TryAddUpdatedMotorway(newLane);
								}
							}
						}
					}
					if (tile.ContentType == TileContentType.Tree && (tile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Include).Count > 0 || tile.HasRoundabout(RoadState.Active)))
					{
						TreeModel tree = tile.ContentModel as TreeModel;
						if (Diagnostics.Verify(tree != null, "Tile {0} says it is a tree, but has a content of {1} instead.", tile, tile.ContentModel))
						{
							tree.Bulldoze();
						}
					}
					if (tile.ContentType != TileContentType.Carpark && tile.ContentType != TileContentType.Destination && tileModel.roadChunk.TrainCrossingModel == null && tileModel.RailTileModel != null && tileModel.roadChunk.lanes.Count > 0)
					{
						tileModel.roadChunk.TrainCrossingModel = simulation.Scope.Get<TrainCrossingModel>();
						tileModel.roadChunk.TrainCrossingModel.Initialize(tile, tileModel.roadChunk, Vector2Int.zero);
						simulation.AddModel(tileModel.roadChunk.TrainCrossingModel);
					}
					if (activeConnections.Count > 0)
					{
						RoadTileDefinition definition2 = this._atlas.GetDefinitionForSignature(activeSignature);
						if (Diagnostics.Verify(definition2 != null, "Tile at {0} has invalid active signature {1}.", tile.Coordinates, activeSignature))
						{
							foreach (RoadTileConnection newConnection2 in activeConnections)
							{
								bool isEndpointLane2 = tile.ContentType == TileContentType.House;
								LaneModel newLane2 = tileModel.AddLane(newConnection2, definition2, RoadState.Active, isEndpointLane2);
								this._lanesChanged.Add(newLane2);
								didAddLane = true;
								this.TryAddUpdatedTileCorner(tileModel, newLane2);
								this.TryAddUpdatedMotorway(newLane2);
							}
						}
					}
					this._scope.Release(activeSignature);
					this._scope.Release(liveSignature);
				}
			}
			this._tilemap.ClearChangedTiles();
			if (this._tileCornersToUpdate.Count > 0)
			{
				foreach (TileCornerModel cornerModel2 in this._tileCornersToUpdate)
				{
					RoadTileSignature activeSignature2 = cornerModel2.CreateTileSignature();
					List<RoadTileConnection> activeConnections2 = new List<RoadTileConnection>(activeSignature2.Connections);
					foreach (LaneModel laneModel2 in cornerModel2.roadChunk.lanes)
					{
						bool isConnectionActive2 = activeConnections2.Contains(laneModel2.connection);
						bool isLaneActive2 = laneModel2.state == RoadState.Active;
						if (isConnectionActive2)
						{
							activeConnections2.Remove(laneModel2.connection);
							if (!isLaneActive2)
							{
								laneModel2.state = RoadState.Active;
							}
						}
						else if (!isConnectionActive2 && isLaneActive2)
						{
							laneModel2.state = RoadState.Mothballed;
							this._lanesChanged.Add(laneModel2);
						}
					}
					if (activeConnections2.Count > 0)
					{
						RoadTileDefinition definition3 = this._atlas.GetCornerDefinitionForSignature(activeSignature2);
						foreach (RoadTileConnection connection in activeConnections2)
						{
							cornerModel2.AddLane(connection, definition3, RoadState.Active);
						}
					}
					this._scope.Release(activeSignature2);
				}
				this._tileCornersToUpdate.Clear();
			}
			if (this._motorwaysToUpdate.Count > 0)
			{
				foreach (MotorwayModel motorwayModel in this._motorwaysToUpdate)
				{
					TileModel startTile = this._tilemap.GetTileModel(motorwayModel.StartCoordinates);
					TileModel endTile = this._tilemap.GetTileModel(motorwayModel.EndCoordinates);
					if (motorwayModel.startToEndLane == null)
					{
						List<LaneModel> lanesEnteringMotorwayFromStartTile = startTile.roadChunk.GetLanesExitingInDirection(motorwayModel.StartDirection);
						List<LaneModel> lanesExitingMotorwayToEndTile = endTile.roadChunk.GetLanesEnteringFromDirection(motorwayModel.EndDirection);
						if (Diagnostics.Verify(lanesEnteringMotorwayFromStartTile.Count > 0 && lanesExitingMotorwayToEndTile.Count > 0, "Motorway has no connecting lanes!"))
						{
							RoadTileConnection connection2 = new RoadTileConnection(new RoadTileNode(TileUtilities.GetOppositeDirection(motorwayModel.StartDirection), RoadType.Motorway, motorwayModel.Id), new RoadTileNode(TileUtilities.GetOppositeDirection(motorwayModel.EndDirection), RoadType.Motorway, motorwayModel.Id));
							List<Vector2Fixed> path = new List<Vector2Fixed>
							{
								lanesEnteringMotorwayFromStartTile[0].EndPosition,
								lanesExitingMotorwayToEndTile[0].StartPosition
							};
							motorwayModel.startToEndLane = motorwayModel.roadChunk.AddBespokeLane(connection2, path, RoadState.Active, false, false);
						}
						List<LaneModel> lanesEnteringMotorwayFromEndTile = endTile.roadChunk.GetLanesExitingInDirection(motorwayModel.EndDirection);
						List<LaneModel> lanesExitingMotorwayToStartTile = startTile.roadChunk.GetLanesEnteringFromDirection(motorwayModel.StartDirection);
						if (Diagnostics.Verify(lanesEnteringMotorwayFromEndTile.Count > 0 && lanesExitingMotorwayToStartTile.Count > 0, "Motorway has no connecting lanes!"))
						{
							RoadTileConnection connection3 = new RoadTileConnection(new RoadTileNode(TileUtilities.GetOppositeDirection(motorwayModel.EndDirection), RoadType.Motorway, motorwayModel.Id), new RoadTileNode(TileUtilities.GetOppositeDirection(motorwayModel.StartDirection), RoadType.Motorway, motorwayModel.Id));
							List<Vector2Fixed> path2 = new List<Vector2Fixed>
							{
								lanesEnteringMotorwayFromEndTile[0].EndPosition,
								lanesExitingMotorwayToStartTile[0].StartPosition
							};
							motorwayModel.endToStartLane = motorwayModel.roadChunk.AddBespokeLane(connection3, path2, RoadState.Active, false, false);
						}
					}
					startTile.roadChunk.ConnectInboundLane(motorwayModel.endToStartLane);
					startTile.roadChunk.ConnectOutboundLane(motorwayModel.startToEndLane);
					endTile.roadChunk.ConnectInboundLane(motorwayModel.startToEndLane);
					endTile.roadChunk.ConnectOutboundLane(motorwayModel.endToStartLane);
				}
				this._motorwaysToUpdate.Clear();
			}
			if (didAddLane)
			{
				this._city.OnLanesAdded();
			}
			if (this._lanesChanged.Count > 0)
			{
				foreach (VehicleModel vehicle in simulation.GetModels<VehicleModel>())
				{
					if (!vehicle.IsWaitingAtHouse && !vehicle.IsRealigningOnDriveway && this._pathfinder.AreLanesConnected(vehicle.LastCommittedLane, this._lanesChanged, true))
					{
						vehicle.RequestPathfind(VehicleModel.PathfindUrgency.WhenPossible);
						vehicle.RequestReturnPathfind(VehicleModel.PathfindUrgency.WhenPossible);
					}
				}
				this._lanesChanged.Clear();
			}
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x0006FF1C File Offset: 0x0006E11C
		private TileCornerModel TryAddUpdatedTileCorner(TileModel tileModel, LaneModel lane)
		{
			if (tileModel != null && lane != null)
			{
				if (TileUtilities.IsDirectionDiagonal(lane.connection.input.direction))
				{
					TileCornerModel newCornerModel = this._tilemap.GetOrCreateTileCornerModel(new CornerAdjacencyReference(tileModel.Coordinates, lane.connection.input.direction));
					if (!this._tileCornersToUpdate.Contains(newCornerModel))
					{
						this._tileCornersToUpdate.Add(newCornerModel);
						return newCornerModel;
					}
				}
				if (TileUtilities.IsDirectionDiagonal(lane.connection.output.direction))
				{
					TileCornerModel newCornerModel2 = this._tilemap.GetOrCreateTileCornerModel(new CornerAdjacencyReference(tileModel.Coordinates, lane.connection.output.direction));
					if (!this._tileCornersToUpdate.Contains(newCornerModel2))
					{
						this._tileCornersToUpdate.Add(newCornerModel2);
						return newCornerModel2;
					}
				}
			}
			return null;
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x0006FFEC File Offset: 0x0006E1EC
		private void TryAddUpdatedMotorway(LaneModel lane)
		{
			if (lane.connection.input.type == RoadType.Motorway)
			{
				MotorwayModel motorway = this._tilemap.GetMotorwayModel(lane.connection.input.motorwayId);
				if (motorway != null && !this._motorwaysToUpdate.Contains(motorway))
				{
					this._motorwaysToUpdate.Add(motorway);
				}
			}
			if (lane.connection.output.type == RoadType.Motorway)
			{
				MotorwayModel motorway2 = this._tilemap.GetMotorwayModel(lane.connection.output.motorwayId);
				if (motorway2 != null && !this._motorwaysToUpdate.Contains(motorway2))
				{
					this._motorwaysToUpdate.Add(motorway2);
				}
			}
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x00070091 File Offset: 0x0006E291
		public void Reset()
		{
			this._lanesChanged.Clear();
			this._tileCornersToUpdate.Clear();
			this._motorwaysToUpdate.Clear();
		}

		// Token: 0x040018E7 RID: 6375
		[Serialize(false, null)]
		private readonly List<LaneModel> _lanesChanged = new List<LaneModel>();

		// Token: 0x040018E8 RID: 6376
		[Serialize(false, null)]
		private readonly List<TileCornerModel> _tileCornersToUpdate = new List<TileCornerModel>();

		// Token: 0x040018E9 RID: 6377
		[Serialize(false, null)]
		private readonly List<MotorwayModel> _motorwaysToUpdate = new List<MotorwayModel>();

		// Token: 0x040018EA RID: 6378
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("LaneUpdateProcess");

		// Token: 0x040018EB RID: 6379
		[Dependency]
		private IScope _scope;

		// Token: 0x040018EC RID: 6380
		[Dependency]
		private RoadTileAtlas _atlas;

		// Token: 0x040018ED RID: 6381
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x040018EE RID: 6382
		[Dependency]
		private Pathfinder _pathfinder;

		// Token: 0x040018EF RID: 6383
		private CityModel _city;
	}
}
