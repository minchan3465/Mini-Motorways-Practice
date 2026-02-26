using System;
using System.Collections.Generic;
using Factory;
using FixMath;
using Motorways.Models;
using Motorways.Utility;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000411 RID: 1041
	public class TileEditor
	{
		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x06001980 RID: 6528 RVA: 0x0005ABF4 File Offset: 0x00058DF4
		private GameRules Rules
		{
			get
			{
				return this._city.Rules;
			}
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x0005AC04 File Offset: 0x00058E04
		public TileEditResult AddRoad(ITilemap tilemap, Vector2Int originCoordinates, TileDirection direction)
		{
			TileEditor.Log.Info("Attempting to add road from {0} in direction {1}.", new object[]
			{
				originCoordinates,
				direction
			});
			Tile originTile = tilemap.GetOrCreateTile(originCoordinates);
			Vector2Int destinationCoordinates = TileUtilities.GetAdjacentCoordinates(originCoordinates, direction);
			Tile destinationTile = tilemap.GetOrCreateTile(destinationCoordinates);
			if (originTile == null || destinationTile == null)
			{
				if (originTile == null)
				{
					TileEditor.Log.Error("Unable to add road, no origin tile with coordinates {0}.", new object[]
					{
						originCoordinates
					});
				}
				if (destinationTile == null)
				{
					TileEditor.Log.Error("Unable to add road, no destination tile with coordinates {0}.", new object[]
					{
						destinationCoordinates
					});
				}
				return TileEditResult.InvalidTileCoordinate(originCoordinates);
			}
			if (!this._city.Definition.TileIsBuildable(originCoordinates) || !this._city.Definition.TileIsBuildable(destinationCoordinates))
			{
				TileEditor.Log.Info("Cannot connect an unbuildable tile.", Array.Empty<object>());
				return TileEditResult.InvalidTileCoordinate(originCoordinates);
			}
			bool isOriginCarpark = originTile.ContentType == TileContentType.Carpark || originTile.ContentType == TileContentType.Destination;
			bool isDestinationCarpark = destinationTile.ContentType == TileContentType.Carpark || destinationTile.ContentType == TileContentType.Destination;
			if (isOriginCarpark || isDestinationCarpark)
			{
				if (isOriginCarpark && isDestinationCarpark)
				{
					return TileEditResult.InvalidTileCoordinate(originCoordinates);
				}
				if ((isDestinationCarpark && originTile.HasTwoLaneRoadInDirection(direction, RoadState.Active)) || (isOriginCarpark && destinationTile.HasTwoLaneRoadInDirection(TileUtilities.GetOppositeDirection(direction), RoadState.Active)))
				{
					return TileEditResult.Success;
				}
				TileEditor.Log.Info("Cannot connect directly to a carpark.", Array.Empty<object>());
				return TileEditResult.CannotConnectToCarpark(originCoordinates);
			}
			else
			{
				if (this._city.Definition.TileIsOverWater(originCoordinates) && this._city.Definition.TileIsOverRail(originCoordinates))
				{
					return TileEditResult.CannotCreateBridge(originCoordinates);
				}
				if (this._city.Definition.TileIsOverWater(destinationCoordinates) && this._city.Definition.TileIsOverRail(destinationCoordinates))
				{
					return TileEditResult.CannotCreateBridge(destinationCoordinates);
				}
				if (this._city.Definition.TileIsOverWater(originCoordinates) && this._city.Definition.TileIsUnderAMountain(destinationCoordinates))
				{
					return TileEditResult.CannotCreateTunnel(destinationCoordinates);
				}
				if (this._city.Definition.TileIsUnderAMountain(originCoordinates) && this._city.Definition.TileIsOverWater(destinationCoordinates))
				{
					return TileEditResult.CannotCreateBridge(destinationCoordinates);
				}
				if (!this.WouldConnectionPassPassageConstraints(originTile, destinationTile, originCoordinates, destinationCoordinates, direction, tilemap, UpgradeType.Bridge, new Func<Vector2Int, bool>(this._city.Definition.TileIsOverWater)))
				{
					return TileEditResult.CannotCreateBridge(originCoordinates);
				}
				if (!this.WouldConnectionPassPassageConstraints(originTile, destinationTile, originCoordinates, destinationCoordinates, direction, tilemap, UpgradeType.Tunnel, new Func<Vector2Int, bool>(this._city.Definition.TileIsUnderAMountain)))
				{
					return TileEditResult.CannotCreateTunnel(originCoordinates);
				}
				if (originTile.ContentType == TileContentType.House || destinationTile.ContentType == TileContentType.House)
				{
					if (originTile.ContentType == TileContentType.House && destinationTile.ContentType == TileContentType.House)
					{
						TileEditor.Log.Info("Ignoring edit between two houses.", Array.Empty<object>());
						return new TileEditResult
						{
							resultCode = TileEditResultCode.Success,
							edit = null
						};
					}
					Tile houseTile;
					Tile drivewayTile;
					TileDirection drivewayDirection;
					if (originTile.ContentType == TileContentType.House)
					{
						houseTile = originTile;
						drivewayTile = destinationTile;
						drivewayDirection = direction;
					}
					else
					{
						houseTile = destinationTile;
						drivewayTile = originTile;
						drivewayDirection = TileUtilities.GetOppositeDirection(direction);
					}
					TileEditResult drivewayObstructionCheck;
					if (!TileEditor.WouldDrivewayPassPassageConstraints(houseTile, drivewayTile, this._city.Definition, out drivewayObstructionCheck))
					{
						return drivewayObstructionCheck;
					}
					RoadTileNode houseNode = new RoadTileNode(drivewayDirection, RoadType.Driveway, -1);
					RoadTileNode destinationNode = new RoadTileNode(TileUtilities.GetOppositeDirection(drivewayDirection), RoadType.TwoLane, -1);
					TileEdit realignedRoadEdit = null;
					if (houseTile.CanSetNodeState(houseNode, RoadState.Pending, Tile.TileChangePermissions.Full) && drivewayTile.CanSetNodeState(destinationNode, RoadState.Pending, Tile.TileChangePermissions.Full) && drivewayTile.CanDrawRoadsOn())
					{
						if (drivewayTile.HasRoundabout(RoadState.Mothballed) && !Roundabout.CanConnectionAddExitNode(drivewayTile.GetRoundaboutConnection(RoadState.Mothballed), destinationNode))
						{
							TileEditor.Log.Info("Cannot realign driveway from house at tile {0} to direction {1} because it is blocked by a mothballed roundabouts.", new object[]
							{
								houseTile.Coordinates,
								drivewayDirection
							});
						}
						else
						{
							TileEditor.Log.Info("Realigning driveway from house at tile {0} to direction {1}.", new object[]
							{
								houseTile.Coordinates,
								drivewayDirection
							});
							realignedRoadEdit = AlignDrivewayEdit.Create(this._scope, tilemap, houseTile.Coordinates, drivewayDirection);
						}
					}
					return new TileEditResult
					{
						resultCode = TileEditResultCode.Success,
						edit = realignedRoadEdit
					};
				}
				else
				{
					if (!this._behaviour.CanDrawRoadOn(originTile.ContentType) || !this._behaviour.CanDrawRoadOn(destinationTile.ContentType))
					{
						TileEditor.Log.Info("Cannot draw onto one of content type {0} or content type {1}", new object[]
						{
							originTile.ContentType,
							destinationTile.ContentType
						});
						return TileEditResult.InvalidTileCoordinate(originCoordinates);
					}
					if (originTile.ContentType == TileContentType.Destination || destinationTile.ContentType == TileContentType.Destination)
					{
						TileEditor.Log.Info("Ignoring attempt to connect directly to destination tile.", Array.Empty<object>());
						return new TileEditResult
						{
							resultCode = TileEditResultCode.Success,
							edit = null
						};
					}
					if (Passage.DoesTileHavePassage(this._city.Definition, tilemap, originCoordinates, RoadState.Mothballed) && originTile.GetTwoLaneRoads(RoadState.Mothballed, Tile.MotorwayInclusion.Ignore)[direction])
					{
						RestoreMothballedPassageEdit edit = RestoreMothballedPassageEdit.Create(this._scope, tilemap, originCoordinates, direction, this._city);
						if (edit == null)
						{
							return TileEditResult.InvalidTileCoordinate(originCoordinates);
						}
						return new TileEditResult
						{
							resultCode = TileEditResultCode.Success,
							edit = edit
						};
					}
					else
					{
						if (originTile.HasRailConnection && destinationTile.HasRailConnection)
						{
							return TileEditResult.CannotCreateCrossing(originCoordinates);
						}
						RoadTileNode newOriginNode = new RoadTileNode(direction, RoadType.TwoLane, -1);
						RoadTileNode newDestinationNode = new RoadTileNode(TileUtilities.GetOppositeDirection(direction), RoadType.TwoLane, -1);
						TileEdit addRoadEdit = null;
						if (!originTile.CanSetNodeState(newOriginNode, RoadState.Pending, Tile.TileChangePermissions.Full) || !destinationTile.CanSetNodeState(newDestinationNode, RoadState.Pending, Tile.TileChangePermissions.Full))
						{
							return new TileEditResult
							{
								resultCode = TileEditResultCode.EditAlreadyExists,
								edit = addRoadEdit
							};
						}
						int concreteCost = this._behaviour.GetConcreteCostForConnection(tilemap, originCoordinates, destinationCoordinates);
						if (concreteCost == 0 || this._upgradeDatabase.HasUpgradeAvailable(UpgradeType.Concrete, concreteCost) || originTile.GetTwoLaneRoadStateInDirection(direction) == RoadState.Mothballed)
						{
							addRoadEdit = AddRoadEdit.Create(this._scope, tilemap, originCoordinates, direction, this._city.Definition);
							return new TileEditResult
							{
								resultCode = TileEditResultCode.Success,
								edit = addRoadEdit
							};
						}
						TileEditor.Log.Info("Out of concrete.", Array.Empty<object>());
						return TileEditResult.NotEnoughConcrete(originCoordinates);
					}
				}
			}
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x0005B1FC File Offset: 0x000593FC
		private bool WouldConnectionPassPassageConstraints(Tile originTile, Tile destinationTile, Vector2Int originCoordinates, Vector2Int destinationCoordinates, TileDirection direction, ITilemap tilemap, UpgradeType upgradeType, Func<Vector2Int, bool> isOverObstruction)
		{
			bool originOverObstruction = isOverObstruction(originCoordinates);
			bool destinationOverObstruction = isOverObstruction(destinationCoordinates);
			if ((originOverObstruction || destinationOverObstruction) && originTile.HasTwoLaneRoadInDirection(direction, RoadState.Mothballed))
			{
				return true;
			}
			if ((originOverObstruction && originTile.GetTwoLaneRoadCount(RoadState.Pending | RoadState.Active | RoadState.Mothballed, Tile.MotorwayInclusion.Ignore) > 1) || (destinationOverObstruction && destinationTile.GetTwoLaneRoadCount(RoadState.Pending | RoadState.Active | RoadState.Mothballed, Tile.MotorwayInclusion.Ignore) > 1))
			{
				TileEditor.Log.Info("Cannot build a {0} that would create an intersection.", new object[]
				{
					upgradeType
				});
				return false;
			}
			if ((originOverObstruction || destinationOverObstruction) && TileUtilities.IsDirectionDiagonal(direction))
			{
				Vector2Int offset = TileUtilities.GetAdjacencyOffsetForDirection(direction);
				Vector2Int xOffsetPosition = originCoordinates + Vector2Int.right * offset.x;
				Vector2Int yOffsetPosition = originCoordinates + Vector2Int.up * offset.y;
				Tile xOffsetTile = tilemap.GetTile(xOffsetPosition);
				Tile yOffsetTile = tilemap.GetTile(yOffsetPosition);
				TileDirection oppositeDiagonalDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(xOffsetPosition, yOffsetPosition);
				if (xOffsetTile != null && yOffsetTile != null && xOffsetTile.HasTwoLaneRoadInDirection(oppositeDiagonalDirection, RoadState.ActiveOrPending))
				{
					TileEditor.Log.Info("Cannot build a diagonal {0} that would create a corner intersection.", new object[]
					{
						upgradeType
					});
					return false;
				}
			}
			if (originOverObstruction && originTile.GetTwoLaneRoadCount(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore) == 0)
			{
				TileEditor.Log.Info("Cannot build a new {0} from an empty obstruction tile.", new object[]
				{
					upgradeType
				});
				return false;
			}
			if (!originOverObstruction && destinationOverObstruction && destinationTile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) == 0 && !this._upgradeDatabase.HasUpgradeAvailable(upgradeType, 1))
			{
				TileEditor.Log.Info("Out of {0}.", new object[]
				{
					upgradeType
				});
				return false;
			}
			return true;
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x0005B384 File Offset: 0x00059584
		private static bool WouldDrivewayPassPassageConstraints(Tile houseTile, Tile drivewayTile, CityDefinition cityDefinition, out TileEditResult onFailure)
		{
			if (cityDefinition.TileIsOverWater(drivewayTile.Coordinates))
			{
				onFailure = TileEditResult.CannotConnectHouseToBridge(drivewayTile.Coordinates);
				return false;
			}
			if (cityDefinition.TileIsUnderAMountain(drivewayTile.Coordinates))
			{
				onFailure = TileEditResult.CannotConnectHouseToTunnel(drivewayTile.Coordinates);
				return false;
			}
			if (cityDefinition.TileIsOverRail(drivewayTile.Coordinates))
			{
				onFailure = TileEditResult.CannotConnectHouseToRail(drivewayTile.Coordinates);
				return false;
			}
			Vector2Int adjacentTile = new Vector2Int(houseTile.Coordinates.x, drivewayTile.Coordinates.y);
			Vector2Int adjacentTile2 = new Vector2Int(drivewayTile.Coordinates.x, houseTile.Coordinates.y);
			if (adjacentTile.x != adjacentTile2.x && adjacentTile.y != adjacentTile2.y && cityDefinition.TileIsOverRail(adjacentTile) && cityDefinition.TileIsOverRail(adjacentTile2))
			{
				onFailure = TileEditResult.CannotConnectHouseToRail(drivewayTile.Coordinates);
				return false;
			}
			onFailure = TileEditResult.Success;
			return true;
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x0005B48B File Offset: 0x0005968B
		public TileEditResult ClearTile(ITilemap tilemap, Vector2Int coordinates, Tile.TileChangePermissions changePermissions = Tile.TileChangePermissions.Full)
		{
			return this.ClearTileExplicit(tilemap, coordinates, TileEditor.ClearTileOfType.Any, changePermissions);
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x0005B497 File Offset: 0x00059697
		private TileEditResultCode ResultCodeForType(TileEditor.ClearTileOfType editType, TileEditor.ClearTileOfType requestedType, TileEditResultCode successCode = TileEditResultCode.Success, TileEditResultCode mismatchedCode = TileEditResultCode.ClearForSpecificTypeNotNeeded)
		{
			if (requestedType != TileEditor.ClearTileOfType.Any && requestedType != editType)
			{
				return mismatchedCode;
			}
			return successCode;
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x0005B4A5 File Offset: 0x000596A5
		private bool ShouldAttemptClearingType(TileEditor.ClearTileOfType clearingType, TileEditor.ClearTileOfType requestedType)
		{
			return requestedType == TileEditor.ClearTileOfType.Any || requestedType == TileEditor.ClearTileOfType.Roads || clearingType == requestedType;
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x0005B4B8 File Offset: 0x000596B8
		private bool CanClearMotorwayOnTile(Tile tile, ITilemap tilemap, Tile.TileChangePermissions changePermissions)
		{
			if (changePermissions != Tile.TileChangePermissions.RespectPermanence)
			{
				return true;
			}
			foreach (TileDirection direction in tile.GetMotorwayRamps(RoadState.VisiblyActive))
			{
				int motorwayId = tile.GetMotorwayInDirection(direction, RoadState.VisiblyActive);
				if (motorwayId != -1 && tilemap.GetMotorway(motorwayId).IsPermanent)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x0005B50E File Offset: 0x0005970E
		private bool CanClearTrafficLight(Tile tile, Tile.TileChangePermissions changePermissions)
		{
			return changePermissions != Tile.TileChangePermissions.RespectPermanence || !tile.IsTrafficLightPermanent;
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x0005B51F File Offset: 0x0005971F
		private bool CanClearRoundaboutOnTile(Tile tileToClear, Tile.TileChangePermissions changePermissions)
		{
			return changePermissions != Tile.TileChangePermissions.RespectPermanence || !tileToClear.IsRoundaboutPermanent;
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x0005B530 File Offset: 0x00059730
		public TileEditResult ClearTileExplicit(ITilemap tilemap, Vector2Int coordinates, TileEditor.ClearTileOfType typeToClear, Tile.TileChangePermissions changePermissions = Tile.TileChangePermissions.Full)
		{
			TileEditor.Log.Info("Attempting to clear tile at {0} of type {1}.", new object[]
			{
				coordinates,
				typeToClear
			});
			Tile tileToClear = tilemap.GetTile(coordinates);
			if (tileToClear == null || tileToClear.IsEmpty())
			{
				TileEditor.Log.Info("No tile to clear.", Array.Empty<object>());
				return new TileEditResult
				{
					resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.Any, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded)
				};
			}
			if (tileToClear.ContentType == TileContentType.House || tileToClear.ContentType == TileContentType.Destination)
			{
				return new TileEditResult
				{
					resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.Any, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded)
				};
			}
			if (tileToClear.HasTrafficLight && this.ShouldAttemptClearingType(TileEditor.ClearTileOfType.TrafficLight, typeToClear) && this.CanClearTrafficLight(tileToClear, changePermissions))
			{
				return new TileEditResult
				{
					resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.TrafficLight, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded),
					edit = RemoveTrafficLightEdit.Create(this._scope, coordinates)
				};
			}
			if (tileToClear.UnbuiltMotorwayId != -1 && this.ShouldAttemptClearingType(TileEditor.ClearTileOfType.UnbuiltMotorway, typeToClear))
			{
				return new TileEditResult
				{
					resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.UnbuiltMotorway, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded),
					edit = RemoveUnbuiltMotorwaysEdit.Create(this._scope, coordinates)
				};
			}
			if (tileToClear.GetMotorwayRamps(RoadState.VisiblyActive).Count > 0 && this.ShouldAttemptClearingType(TileEditor.ClearTileOfType.BuiltMotorways, typeToClear) && this.CanClearMotorwayOnTile(tileToClear, tilemap, changePermissions))
			{
				return new TileEditResult
				{
					resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.BuiltMotorways, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded),
					edit = RemoveMotorwaysEdit.Create(this._scope, coordinates)
				};
			}
			if (tileToClear.IsCenterOfRoundabout && this.ShouldAttemptClearingType(TileEditor.ClearTileOfType.Roundabout, typeToClear) && this.CanClearRoundaboutOnTile(tileToClear, changePermissions))
			{
				return new TileEditResult
				{
					resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.Roundabout, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded),
					edit = RemoveRoundaboutEdit.Create(this._scope, coordinates, tilemap, this._city.Definition)
				};
			}
			if (Passage.DoesTileHavePassage(this._city.Definition, tilemap, coordinates, RoadState.ActiveOrPending) && this.ShouldAttemptClearingType(TileEditor.ClearTileOfType.Passages, typeToClear) && this.CanClearAnyOfPassagesOnTile(coordinates, tilemap, changePermissions))
			{
				return new TileEditResult
				{
					resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.Passages, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded),
					edit = RemovePassagesEdit.Create(this._scope, tilemap, coordinates, this._city.Definition, changePermissions)
				};
			}
			if (tileToClear.IsDrivewayOnly)
			{
				return new TileEditResult
				{
					resultCode = TileEditResultCode.CannotClearTile
				};
			}
			bool allRoadsOnTileArePermanent = changePermissions == Tile.TileChangePermissions.RespectPermanence && tileToClear.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) > 0 && !tileToClear.AnyRoadHasPermanenceBelowValue(Fix64.One, RoadState.Active);
			bool hasPermanentUpgradeOnTile = tileToClear.HasTrafficLight || tileToClear.GetMotorwayRamps(RoadState.VisiblyActive).Count > 0 || this.IsRoundaboutPermanentWithNoDeletableRoads(tileToClear, tilemap);
			if (allRoadsOnTileArePermanent && !hasPermanentUpgradeOnTile)
			{
				return new TileEditResult
				{
					resultCode = TileEditResultCode.NoDeletableRoads,
					errorPosition = coordinates
				};
			}
			if (hasPermanentUpgradeOnTile && (tileToClear.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) == 0 || allRoadsOnTileArePermanent))
			{
				return new TileEditResult
				{
					resultCode = TileEditResultCode.NoDeletableUpgrade,
					errorPosition = coordinates
				};
			}
			return new TileEditResult
			{
				resultCode = this.ResultCodeForType(TileEditor.ClearTileOfType.Roads, typeToClear, TileEditResultCode.Success, TileEditResultCode.ClearForSpecificTypeNotNeeded),
				edit = ClearTileEdit.Create(this._scope, coordinates, tilemap, changePermissions)
			};
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x0005B864 File Offset: 0x00059A64
		private bool IsRoundaboutPermanentWithNoDeletableRoads(Tile tile, ITilemap tilemap)
		{
			if (tile.IsCenterOfRoundabout && tile.IsRoundaboutPermanent)
			{
				foreach (TileDirection direction in TileUtilities.Directions)
				{
					Tile neighbourTile = tilemap.GetTile(TileUtilities.GetAdjacentCoordinates(tile.Coordinates, direction));
					TileDirection oppositeDirection = TileUtilities.GetOppositeDirection(direction);
					if (neighbourTile != null && neighbourTile.ContentType != TileContentType.House && neighbourTile.ContentType != TileContentType.Destination && neighbourTile.ContentType != TileContentType.Carpark && neighbourTile.CanSetNodeState(new RoadTileNode(oppositeDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.RespectPermanence))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x0005B8EC File Offset: 0x00059AEC
		public TileEditResult AddUnbuiltMotorway(ITilemap tilemap, int motorwayId, int motorwayNumber, Vector2Int coordinates)
		{
			TileEdit edit = AddUnbuiltMotorwayEdit.Create(this._scope, coordinates, motorwayId, motorwayNumber);
			if (!this._city.Definition.TileIsBuildable(coordinates) || this._city.Definition.TileIsOverWater(coordinates) || this._city.Definition.TileIsUnderAMountain(coordinates) || this._city.Definition.TileIsOverRail(coordinates))
			{
				edit.CanApplyToSimulation = false;
				return new TileEditResult
				{
					resultCode = TileEditResultCode.InvalidTileCoordinate,
					edit = edit
				};
			}
			if (this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.Motorway) < 1)
			{
				return TileEditResult.NotEnoughUpgrades;
			}
			Tile tile = tilemap.GetTile(coordinates);
			if (tile != null && !TileEditor.TileSupportsUnbuiltMotorway(tile, motorwayId))
			{
				edit.CanApplyToSimulation = false;
				return new TileEditResult
				{
					resultCode = TileEditResultCode.InvalidTileCoordinate,
					edit = edit
				};
			}
			edit.CanApplyToSimulation = true;
			return new TileEditResult
			{
				resultCode = TileEditResultCode.Success,
				edit = edit
			};
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x0005B9E8 File Offset: 0x00059BE8
		public TileEditResult AddMotorway(ITilemap tilemap, int motorwayId, int motorwayNumber, Vector2Int startCoordinates, TileDirection startDirection, Vector2Int endCoordinates, TileDirection endDirection, int replacedMotorwayId)
		{
			if (GameRules.GetMotorwayLength(startCoordinates, endCoordinates) < this.Rules.MinimumMotorwayLength)
			{
				return TileEditResult.MotorwayTooShort;
			}
			Motorway replacedMotorway = tilemap.GetMotorway(replacedMotorwayId);
			if (replacedMotorway != null && ((replacedMotorway.StartCoordinates.Equals(startCoordinates) && replacedMotorway.EndCoordinates.Equals(endCoordinates)) || (replacedMotorway.StartCoordinates.Equals(endCoordinates) && replacedMotorway.EndCoordinates.Equals(startCoordinates))))
			{
				TileEditResult result = new TileEditResult
				{
					resultCode = TileEditResultCode.Success
				};
				return result;
			}
			TileEdit edit = AddMotorwayEdit.Create(this._scope, motorwayId, motorwayNumber, startCoordinates, startDirection, endCoordinates, endDirection, replacedMotorwayId);
			CityDefinition cityDefinition = this._city.Definition;
			if (!cityDefinition.TileIsBuildable(startCoordinates) || cityDefinition.TileIsOverWater(startCoordinates) || cityDefinition.TileIsUnderAMountain(startCoordinates) || cityDefinition.TileIsOverRail(startCoordinates) || !cityDefinition.TileIsBuildable(endCoordinates) || cityDefinition.TileIsOverWater(endCoordinates) || cityDefinition.TileIsUnderAMountain(endCoordinates) || cityDefinition.TileIsOverRail(endCoordinates))
			{
				edit.CanApplyToSimulation = false;
				TileEditResult result = new TileEditResult
				{
					resultCode = TileEditResultCode.InvalidTileCoordinate,
					edit = edit
				};
				return result;
			}
			foreach (Vector2Int crossedCoordinate in Geometry.GetTileCoordinatesUnderLine(startCoordinates, endCoordinates))
			{
				if (cityDefinition.TileIsUnderAMountain(crossedCoordinate))
				{
					bool isMountainFringe = false;
					foreach (TileDirection currentDirection in TileUtilities.NonDiagonalDirections)
					{
						isMountainFringe |= !cityDefinition.TileIsUnderAMountain(TileUtilities.GetAdjacentCoordinates(crossedCoordinate, currentDirection));
					}
					if (!isMountainFringe)
					{
						edit.CanApplyToSimulation = false;
						TileEditResult result = default(TileEditResult);
						result.resultCode = TileEditResultCode.MotorwayBlockedByMountain;
						result.edit = edit;
						return result;
					}
				}
			}
			int concreteCost = this._behaviour.GetConcreteCostForMotorway(startCoordinates, endCoordinates);
			if (replacedMotorwayId != -1 && Diagnostics.Verify(replacedMotorway != null, "Non-existent motorway is being replaced."))
			{
				concreteCost -= replacedMotorway.ConcreteCost;
			}
			if (concreteCost > 0 && this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.Concrete) < concreteCost && !this._behaviour.HasUnlimitedOfUpgrade(UpgradeType.Concrete))
			{
				edit.CanApplyToSimulation = false;
				return new TileEditResult
				{
					resultCode = TileEditResultCode.NotEnoughConcreteForMotorway,
					edit = edit
				};
			}
			Tile startTile = tilemap.GetTile(startCoordinates);
			if (startTile != null && !TileEditor.TileSupportsMotorwayInDirection(startTile, startDirection, motorwayId))
			{
				edit.CanApplyToSimulation = false;
				return new TileEditResult
				{
					resultCode = TileEditResultCode.InvalidTileCoordinate,
					edit = edit
				};
			}
			Tile endTile = tilemap.GetTile(endCoordinates);
			if (endTile != null && !TileEditor.TileSupportsMotorwayInDirection(endTile, endDirection, motorwayId))
			{
				edit.CanApplyToSimulation = false;
				return new TileEditResult
				{
					resultCode = TileEditResultCode.InvalidTileCoordinate,
					edit = edit
				};
			}
			return new TileEditResult
			{
				resultCode = TileEditResultCode.Success,
				edit = edit
			};
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x0005BCD4 File Offset: 0x00059ED4
		public TileEditResult AddTrafficLight(ITilemap tilemap, Vector2Int coordinates)
		{
			Tile tile = tilemap.GetTile(coordinates);
			if (tile == null || !TileEditor.TileSupportsTrafficLight(tile) || tile.HasTrafficLight)
			{
				TileEdit edit = AddTrafficLightEdit.Create(this._scope, coordinates);
				edit.CanApplyToSimulation = false;
				return new TileEditResult
				{
					resultCode = TileEditResultCode.InvalidTileCoordinate,
					edit = edit
				};
			}
			if (this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.TrafficLight) < 1)
			{
				return TileEditResult.NotEnoughUpgrades;
			}
			return new TileEditResult
			{
				resultCode = TileEditResultCode.Success,
				edit = AddTrafficLightEdit.Create(this._scope, coordinates)
			};
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x0005BD64 File Offset: 0x00059F64
		public TileEditResult AddRoundabout(ITilemap tilemap, Vector2Int coordinates)
		{
			bool canAddRoundabout = true;
			TileEdit edit = AddRoundaboutEdit.Create(this._scope, coordinates, tilemap);
			if (this._upgradeDatabase.GetAvailableOrDraftUpgradeCount(UpgradeType.Roundabout) < 1)
			{
				return TileEditResult.NotEnoughUpgrades;
			}
			Tile centreTile = tilemap.GetTile(coordinates);
			if (centreTile != null && (centreTile.HasRoundabout(RoadState.VisiblyActive) || centreTile.HasTrafficLight || centreTile.HasRailConnection || centreTile.ContentType == TileContentType.House || centreTile.UnbuiltMotorwayId != -1 || centreTile.GetMotorwayRamps(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed).Bits != 0))
			{
				canAddRoundabout = false;
			}
			else
			{
				foreach (Vector2Int coordinatesOffset in Roundabout.GetCoordinatesOffsets())
				{
					RoadTileConnection roundaboutConnection = Roundabout.GetConnectionForCoordinatesOffset(coordinatesOffset);
					if (!this.TileSupportsRoundabout(tilemap, coordinates + coordinatesOffset, roundaboutConnection.input.direction, roundaboutConnection.output.direction))
					{
						canAddRoundabout = false;
						break;
					}
				}
			}
			if (canAddRoundabout)
			{
				return new TileEditResult
				{
					resultCode = TileEditResultCode.Success,
					edit = edit
				};
			}
			edit.CanApplyToSimulation = false;
			return new TileEditResult
			{
				resultCode = TileEditResultCode.InvalidTileCoordinate,
				edit = edit
			};
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x0005BE98 File Offset: 0x0005A098
		public static bool TileSupportsTrafficLight(Tile tile)
		{
			return tile.GetTwoLaneRoadCount(RoadState.Pending | RoadState.Active | RoadState.Mothballed, Tile.MotorwayInclusion.Include) >= 3 && tile.GetMotorwayRamps(RoadState.Active).Count == 0 && tile.UnbuiltMotorwayId == -1 && !tile.HasRailConnection;
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x0005BED8 File Offset: 0x0005A0D8
		private bool CanClearAnyOfPassagesOnTile(Vector2Int clearingPosition, ITilemap tilemap, Tile.TileChangePermissions permissions)
		{
			if (permissions != Tile.TileChangePermissions.RespectPermanence)
			{
				return true;
			}
			List<Passage> passages = Passage.GetPassagesOnTile(this._scope, this._city.Definition, tilemap, clearingPosition, RoadState.Active);
			bool result;
			try
			{
				using (List<Passage>.Enumerator enumerator = passages.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.CanBeCleared(tilemap, permissions))
						{
							return true;
						}
					}
				}
				result = false;
			}
			finally
			{
				foreach (Passage passage in passages)
				{
					this._scope.Release(passage);
				}
			}
			return result;
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0005BFA0 File Offset: 0x0005A1A0
		public static bool TileSupportsMotorwayInDirection(Tile tile, TileDirection direction, int motorwayId)
		{
			return !tile.HasRoundabout(RoadState.Planned | RoadState.Active | RoadState.Mothballed) && !tile.HasTrafficLight && tile.CanSetNodeState(new RoadTileNode(direction, RoadType.Motorway, motorwayId), RoadState.Planned, Tile.TileChangePermissions.Full);
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0005BFC8 File Offset: 0x0005A1C8
		public static bool TileSupportsMotorwayInAnyDirection(Tile tile, int motorwayId)
		{
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				if (TileEditor.TileSupportsMotorwayInDirection(tile, (TileDirection)directionIndex, motorwayId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0005BFF0 File Offset: 0x0005A1F0
		public static bool TileSupportsUnbuiltMotorway(Tile tile, int motorwayId)
		{
			if (tile.UnbuiltMotorwayId != -1 && tile.UnbuiltMotorwayId != motorwayId)
			{
				return false;
			}
			for (int directionIndex = 0; directionIndex < 8; directionIndex++)
			{
				if (TileEditor.TileSupportsMotorwayInDirection(tile, (TileDirection)directionIndex, motorwayId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x0005C02C File Offset: 0x0005A22C
		private bool TileSupportsRoundabout(ITilemap tilemap, Vector2Int coordinates, TileDirection roundaboutInput, TileDirection roundaboutOutput)
		{
			if (!this._city.Definition.TileIsBuildable(coordinates) || this._city.Definition.TileIsOverWater(coordinates) || this._city.Definition.TileIsUnderAMountain(coordinates) || this._city.Definition.TileIsOverRail(coordinates))
			{
				return false;
			}
			Tile tile = tilemap.GetTile(coordinates);
			if (tile != null)
			{
				if (!this._behaviour.CanDrawRoadOn(tile.ContentType))
				{
					return false;
				}
				if (tile.UnbuiltMotorwayId != -1)
				{
					return false;
				}
				if (!tile.CanSetRoundaboutState(roundaboutInput, roundaboutOutput, RoadState.Planned))
				{
					return false;
				}
				TileDirectionBitfield invalidExitDirections = Roundabout.GetInvalidExitsForConnection(roundaboutInput, roundaboutOutput);
				foreach (TileDirection exitDirection in tile.GetTwoLaneRoads(RoadState.ActiveOrPending, Tile.MotorwayInclusion.Ignore))
				{
					if (invalidExitDirections[exitDirection])
					{
						Tile connectedTile = tilemap.GetTile(TileUtilities.GetAdjacentCoordinates(coordinates, exitDirection));
						if (Diagnostics.Verify(connectedTile != null, "Somehow {0} is connected to a null tile.", tile) && (connectedTile.ContentType == TileContentType.House || connectedTile.ContentType == TileContentType.Carpark))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x04001591 RID: 5521
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("TileEditor");

		// Token: 0x04001592 RID: 5522
		[Dependency]
		private IScope _scope;

		// Token: 0x04001593 RID: 5523
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04001594 RID: 5524
		[Dependency]
		private City _city;

		// Token: 0x04001595 RID: 5525
		[Dependency]
		private ClientUpgradeDatabase _upgradeDatabase;

		// Token: 0x02000412 RID: 1042
		public enum ClearTileOfType
		{
			// Token: 0x04001597 RID: 5527
			TrafficLight,
			// Token: 0x04001598 RID: 5528
			UnbuiltMotorway,
			// Token: 0x04001599 RID: 5529
			BuiltMotorways,
			// Token: 0x0400159A RID: 5530
			Roundabout,
			// Token: 0x0400159B RID: 5531
			Passages,
			// Token: 0x0400159C RID: 5532
			Roads,
			// Token: 0x0400159D RID: 5533
			Any
		}
	}
}
