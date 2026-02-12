using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Processes;
using Motorways.Utility;
using Server;
using UnityEngine;

namespace Motorways.Models
{
	// Token: 0x020004D8 RID: 1240
	public class CarparkModel : Model<EmptyModelFrame, CarparkModel.IObserver>
	{
		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06002040 RID: 8256 RVA: 0x0007EA97 File Offset: 0x0007CC97
		// (set) Token: 0x06002041 RID: 8257 RVA: 0x0007EA9F File Offset: 0x0007CC9F
		[Serialize(true, null)]
		public List<TileModel> TileModels { get; private set; }

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06002042 RID: 8258 RVA: 0x0007EAA8 File Offset: 0x0007CCA8
		public Vector2Int TopLeftWorldCoordinate
		{
			get
			{
				return this.origin + new Vector2Int(0, this.footprint.y - 1);
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06002043 RID: 8259 RVA: 0x0007EAC8 File Offset: 0x0007CCC8
		public Vector2Int TopLeftCarparkTileCoordinate
		{
			get
			{
				if (this.Alignment == TileAlignment.Horizontal)
				{
					if (this.carparkTiles[0].y == 0)
					{
						return this.origin;
					}
					return this.TopLeftWorldCoordinate;
				}
				else
				{
					if (this.carparkTiles[0].x == 0)
					{
						return this.TopLeftWorldCoordinate;
					}
					return this.TopLeftWorldCoordinate + new Vector2Int(this.footprint.x - 1, 0);
				}
			}
		}

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06002044 RID: 8260 RVA: 0x0007EB40 File Offset: 0x0007CD40
		public Vector2Int BottomRightCarparkTileCoordinate
		{
			get
			{
				if (this.Alignment == TileAlignment.Horizontal)
				{
					if (this.carparkTiles[0].y == 0)
					{
						return this.origin + new Vector2Int(this.footprint.x - 1, 0);
					}
					return this.origin + new Vector2Int(this.footprint.x - 1, this.footprint.y - 1);
				}
				else
				{
					if (this.carparkTiles[0].x == 0)
					{
						return this.origin;
					}
					return this.origin + new Vector2Int(this.footprint.x - 1, 0);
				}
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06002045 RID: 8261 RVA: 0x0007EBF1 File Offset: 0x0007CDF1
		public TileAlignment Alignment
		{
			get
			{
				if (this.carparkSide != TileDirection.North && this.carparkSide != TileDirection.South)
				{
					return TileAlignment.Vertical;
				}
				return TileAlignment.Horizontal;
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06002046 RID: 8262 RVA: 0x0007EC07 File Offset: 0x0007CE07
		public bool SupportsTwoDestinations
		{
			get
			{
				return this.footprint == BuildingSpawningProcess.VerticalDoubleCarparkFootprint || this.footprint == BuildingSpawningProcess.HorizontalDoubleCarparkFootprint || this.footprint == BuildingSpawningProcess.HorizontalDoubleCarparkBoatFootprint;
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x06002047 RID: 8263 RVA: 0x0007EC40 File Offset: 0x0007CE40
		public int ActiveDestinationCount
		{
			get
			{
				int count = 0;
				using (List<DestinationModel>.Enumerator enumerator = this.destinations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.isActive)
						{
							count++;
						}
					}
				}
				return count;
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06002048 RID: 8264 RVA: 0x0007EC9C File Offset: 0x0007CE9C
		public TileDirection TopLeftDrivewayDirection
		{
			get
			{
				if (this.Alignment != TileAlignment.Horizontal)
				{
					return TileDirection.North;
				}
				return TileDirection.West;
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06002049 RID: 8265 RVA: 0x0007ECAA File Offset: 0x0007CEAA
		public TileDirection BottomRightDrivewayDirection
		{
			get
			{
				if (this.Alignment != TileAlignment.Horizontal)
				{
					return TileDirection.South;
				}
				return TileDirection.East;
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x0600204A RID: 8266 RVA: 0x0007ECB8 File Offset: 0x0007CEB8
		public Vector2Int TopLeftDrivewayTileCoordinates
		{
			get
			{
				if (Diagnostics.Verify(this.entranceAtTopLeft, "Trying to get the driveway for a position we don't have!"))
				{
					return TileUtilities.GetAdjacentCoordinates(this.TopLeftCarparkTileCoordinate, this.TopLeftDrivewayDirection);
				}
				return Vector2Int.zero;
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x0600204B RID: 8267 RVA: 0x0007ECE4 File Offset: 0x0007CEE4
		public Vector2Int BottomRightDrivewayTileCoordinates
		{
			get
			{
				if (!Diagnostics.Verify(this.entranceAtBottomRight, "Trying to get the driveway for a position we don't have!"))
				{
					return Vector2Int.zero;
				}
				Vector2Int offset = TileUtilities.GetAdjacencyOffsetForDirection(this.BottomRightDrivewayDirection);
				if (this.Alignment == TileAlignment.Horizontal)
				{
					return this.TopLeftCarparkTileCoordinate + new Vector2Int(this.footprint.x - 1, 0) + offset;
				}
				return this.TopLeftCarparkTileCoordinate + new Vector2Int(0, -(this.footprint.y - 1)) + offset;
			}
		}

		// Token: 0x0600204C RID: 8268 RVA: 0x0007ED68 File Offset: 0x0007CF68
		public static List<Vector2Int> GenerateCarparkPositions(Vector2Int destinationFootprint, TileDirection carparkSide)
		{
			List<Vector2Int> carparkPositions = new List<Vector2Int>();
			int num = (carparkSide == TileDirection.North || carparkSide == TileDirection.South) ? 1 : 2;
			int carparkTileCount = (num == 1) ? destinationFootprint.x : destinationFootprint.y;
			TileDirection carparkDirection = (num == 1) ? TileDirection.East : TileDirection.South;
			Vector2Int carparkStartingTile = default(Vector2Int);
			if (num == 1)
			{
				if (carparkSide == TileDirection.North)
				{
					carparkStartingTile.y = destinationFootprint.y - 1;
				}
			}
			else
			{
				carparkStartingTile.y = destinationFootprint.y - 1;
				if (carparkSide == TileDirection.East)
				{
					carparkStartingTile.x = destinationFootprint.x - 1;
				}
			}
			for (int carparkTileIndex = 0; carparkTileIndex < carparkTileCount; carparkTileIndex++)
			{
				Vector2Int carparkPosition = carparkStartingTile + TileUtilities.GetAdjacencyOffsetForDirection(carparkDirection) * carparkTileIndex;
				carparkPositions.Add(carparkPosition);
			}
			return carparkPositions;
		}

		// Token: 0x0600204D RID: 8269 RVA: 0x0007EE16 File Offset: 0x0007D016
		public static List<Vector2Int> GenerateDestinationPositions(Vector2Int destinationFootprint, TileDirection carparkSide)
		{
			return CarparkModel.GenerateDestinationPositions((destinationFootprint.x > 3 || destinationFootprint.y > 3) ? 2 : 1, carparkSide);
		}

		// Token: 0x0600204E RID: 8270 RVA: 0x0007EE38 File Offset: 0x0007D038
		public static List<Vector2Int> GenerateDestinationPositions(int destinationCount, TileDirection carparkSide)
		{
			TileAlignment alignment = (carparkSide == TileDirection.North || carparkSide == TileDirection.South) ? TileAlignment.Horizontal : TileAlignment.Vertical;
			Vector2Int destinationOffset = (alignment == TileAlignment.Horizontal) ? new Vector2Int(2, 0) : new Vector2Int(0, 2);
			Vector2Int destinationStartingPosition = default(Vector2Int);
			if (carparkSide == TileDirection.South)
			{
				destinationStartingPosition.y = 1;
			}
			else if (carparkSide == TileDirection.West)
			{
				destinationStartingPosition.x = 1;
			}
			List<Vector2Int> destinationPositions = new List<Vector2Int>();
			for (int i = 0; i < destinationCount; i++)
			{
				destinationPositions.Add(destinationStartingPosition + destinationOffset * i);
			}
			if (alignment == TileAlignment.Vertical)
			{
				destinationPositions.Reverse();
			}
			return destinationPositions;
		}

		// Token: 0x0600204F RID: 8271 RVA: 0x0007EEC0 File Offset: 0x0007D0C0
		public bool Initialize(CarparkEntrance entrances, CarparkPreference carparkPreference, BuildingPlacer.Placement placement)
		{
			this.origin = placement.coordinates;
			this.carparkSide = placement.layout.carparkSide;
			this.footprint = placement.layout.footprint;
			this.entranceAtTopLeft = ((entrances & CarparkEntrance.TopLeft) == CarparkEntrance.TopLeft);
			this.entranceAtBottomRight = ((entrances & CarparkEntrance.BottomRight) == CarparkEntrance.BottomRight);
			this.supportsBoats = (carparkPreference == CarparkPreference.BoatTerminal || carparkPreference == CarparkPreference.JoinBoatTerminal);
			this.carparkTiles = CarparkModel.GenerateCarparkPositions(this.footprint, placement.layout.carparkSide);
			this.destinationOffsets = CarparkModel.GenerateDestinationPositions(this.footprint, placement.layout.carparkSide);
			if (!Diagnostics.Verify(this.carparkTiles.Count >= 2, string.Format("CarparkModel must have at least 2 tiles, not {0}.", this.carparkTiles.Count)))
			{
				return false;
			}
			int length = (this.Alignment == TileAlignment.Horizontal) ? this.footprint.x : this.footprint.y;
			if (!Diagnostics.Verify(length >= 2, "CarparkModel must be at least 2 tiles in length, not {0}.", length))
			{
				return false;
			}
			foreach (Vector2Int destinationOffset in this.destinationOffsets)
			{
				Vector2Int destinationPosition = this.origin + destinationOffset;
				for (int x = 0; x < 2; x++)
				{
					for (int y = 0; y < 2; y++)
					{
						Vector2Int destinationTilePosition = destinationPosition + new Vector2Int(x, y);
						TileModel destinationTile = this._tilemap.GetOrCreateTileModel(destinationTilePosition);
						destinationTile.Tile.SetContentType(TileContentType.Destination, null);
						if (destinationTile.RailTileModel != null)
						{
							destinationTile.RailTileModel.carpark = this;
						}
					}
				}
			}
			foreach (Vector2Int a in placement.layout.boatTerminalTiles)
			{
				Vector2Int worldBoatTerminalTile = a + this.origin;
				TileModel tile = this._tilemap.GetOrCreateTileModel(worldBoatTerminalTile);
				tile.Tile.SetContentType(TileContentType.BoatTerminal, null);
				foreach (TileDirection direction in TileUtilities.DiagonalDirections)
				{
					TileModel adjacentTile = tile.GetAdjacentTileModelInDirection(direction);
					if (adjacentTile != null)
					{
						Tile tile2 = adjacentTile.Tile;
						if (tile2 != null && tile2.HasBoatPathConnection)
						{
							adjacentTile.BoatPathTileModel.carpark = this;
						}
					}
				}
			}
			Vector2Int topLeftCoordinates = new Vector2Int(int.MaxValue, int.MinValue);
			Vector2Int bottomRightCoordinates = new Vector2Int(int.MinValue, int.MaxValue);
			List<TileModel> tiles = new List<TileModel>();
			for (int tileIndex = 0; tileIndex < this.carparkTiles.Count; tileIndex++)
			{
				Vector2Int carparkTileOffset = this.carparkTiles[tileIndex];
				Vector2Int carparkTilePosition = this.origin + carparkTileOffset;
				TileModel tileModel = this._tilemap.GetOrCreateTileModel(carparkTilePosition);
				Tile carparkTile = tileModel.Tile;
				if (Diagnostics.Verify(carparkTile.CanSetContentType(TileContentType.Carpark), string.Format("Unable to set TileContentType.Carpark on one of a carpark's tiles: already set to {0}.", carparkTile.ContentType)))
				{
					carparkTile.SetContentType(TileContentType.Carpark, this);
				}
				tiles.Add(tileModel);
				TileDirection outboundDirection = TileDirection.None;
				if (tileIndex == 0 && this.entranceAtTopLeft)
				{
					outboundDirection = this.TopLeftDrivewayDirection;
				}
				else if (tileIndex == length - 1 && this.entranceAtBottomRight)
				{
					outboundDirection = this.BottomRightDrivewayDirection;
				}
				if (outboundDirection != TileDirection.None)
				{
					carparkTile.SetNodeState(new RoadTileNode(outboundDirection, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
					Tile orCreateTile = this._tilemap.GetOrCreateTile(TileUtilities.GetAdjacentCoordinates(carparkTilePosition, outboundDirection));
					TileDirection inboundDirection = TileUtilities.GetOppositeDirection(outboundDirection);
					orCreateTile.SetNodeState(new RoadTileNode(inboundDirection, RoadType.TwoLane, -1), RoadState.Pending, Tile.TileChangePermissions.Full);
					orCreateTile.SetNodeImmutability(inboundDirection, true);
				}
				if (carparkTilePosition.x < topLeftCoordinates.x || carparkTilePosition.y > topLeftCoordinates.y)
				{
					topLeftCoordinates = carparkTilePosition;
				}
				if (carparkTilePosition.x > bottomRightCoordinates.x || carparkTilePosition.y < bottomRightCoordinates.y)
				{
					bottomRightCoordinates = carparkTilePosition;
				}
			}
			this.TileModels = tiles;
			Fix64 spaceWidth = TilemapModel.TileWidth / (Fix64)3L;
			Fix64 x2 = spaceWidth;
			spaceWidth * (Fix64)0.5f;
			Fix64 spaceOffset = (Fix64)0.25f;
			Vector2Fixed worldOrigin = TilemapModel.GetWorldPositionForCoordinates(this.TopLeftCarparkTileCoordinate);
			TileDirection legacyDirection = (this.Alignment == TileAlignment.Horizontal) ? TileDirection.East : TileDirection.South;
			Vector2Int centreToBaseLane = TileUtilities.GetAdjacencyOffsetForDirection(TileUtilities.GetRotatedDirection(legacyDirection, 2));
			Vector2Int spaceToSpaceDirection = TileUtilities.GetAdjacencyOffsetForDirection(legacyDirection);
			Vector2Int adjacencyOffsetForDirection = TileUtilities.GetAdjacencyOffsetForDirection(TileUtilities.GetRotatedDirection(legacyDirection, -1));
			Vector2Fixed spaceStartToCarparkEnd = new Vector2Fixed(adjacencyOffsetForDirection) * spaceWidth;
			Fix64 carparkShrinkage = (Fix64)0.3f;
			Vector2Fixed carparkStartToLane = -new Vector2Fixed(adjacencyOffsetForDirection) * spaceOffset;
			Vector2Fixed pointTwoSpaceToSpace = new Vector2Fixed(spaceToSpaceDirection) * spaceWidth * (Fix64)0.2f;
			Vector2Fixed pointEightSpaceToSpace = new Vector2Fixed(spaceToSpaceDirection) * spaceWidth * (Fix64)0.8f;
			Fix64 outerLaneOffset = x2 * Fix64Consts.OneHalf;
			if (this.carparkSide == TileDirection.North || this.carparkSide == TileDirection.East)
			{
				outerLaneOffset += (Fix64)0.1f;
			}
			else
			{
				outerLaneOffset -= (Fix64)0.1f;
			}
			Vector2Fixed carparkBase = worldOrigin + new Vector2Fixed(centreToBaseLane) * outerLaneOffset;
			TileDirection secondaryDirection = TileUtilities.GetOppositeDirection(legacyDirection);
			TileDirection carparkAngleDirOut = TileUtilities.GetRotatedDirection(legacyDirection, -1);
			TileDirection carparkAngleDirIn = TileUtilities.GetOppositeDirection(carparkAngleDirOut);
			Vector2Fixed parkDirectionVec = new Vector2Fixed(TileUtilities.DirectionToTileAdjacencyOffset[(int)carparkAngleDirIn]).normalized;
			Vector2Fixed parkNormalVec = new Vector2Fixed(TileUtilities.DirectionToTileAdjacencyOffset[(int)TileUtilities.GetRotatedDirection(carparkAngleDirIn, 2)]).normalized;
			int carparkSpaces = (length - 1) * 3;
			Spline.BezierSplineFixed pathSpline;
			for (int spaceIndex = 0; spaceIndex < carparkSpaces; spaceIndex++)
			{
				Vector2Fixed spaceStart = carparkBase + new Vector2Fixed(spaceToSpaceDirection) * spaceWidth * (Fix64)((long)spaceIndex);
				Vector2Fixed spaceStartIn = spaceStart + carparkStartToLane - pointTwoSpaceToSpace;
				Vector2Fixed spaceStartOut = spaceStart + carparkStartToLane + pointEightSpaceToSpace;
				Vector2Fixed spaceEnd = spaceStart + spaceStartToCarparkEnd;
				Vector2Fixed spaceEndIn = spaceEnd - carparkStartToLane + pointTwoSpaceToSpace;
				Vector2Fixed spaceEndOut = spaceEnd - carparkStartToLane - pointEightSpaceToSpace;
				spaceStart += spaceStartToCarparkEnd * carparkShrinkage;
				spaceEnd -= spaceStartToCarparkEnd * carparkShrinkage;
				bool isFirstSpace = false;
				bool isLastSpace = false;
				Vector2Fixed spaceToParkGentleSpaceHandleDirection = new Vector2Fixed(spaceToSpaceDirection);
				Vector2Fixed parkToSpaceGentleSpaceHandleDirection = new Vector2Fixed(spaceToSpaceDirection);
				if (spaceIndex == 0)
				{
					isFirstSpace = true;
					spaceStartIn -= new Vector2Fixed(centreToBaseLane) * spaceOffset;
					parkToSpaceGentleSpaceHandleDirection = -parkNormalVec;
				}
				else if (spaceIndex == carparkSpaces - 1)
				{
					isLastSpace = true;
					spaceEndIn += new Vector2Fixed(centreToBaseLane) * spaceOffset;
					spaceToParkGentleSpaceHandleDirection = -parkNormalVec;
				}
				CarparkModel.ParkingSpace space = this._scope.Get<CarparkModel.ParkingSpace>();
				space.outerRoadChunk = this._scope.Get<RoadChunkModel>();
				Fix64 SpaceToParkGentleSpaceHandleLength = (Fix64)0.2;
				Fix64 SpaceToParkGentleParkHandleLength = (Fix64)0.2;
				Fix64 SpaceToParkTightSpaceHandleLength = (Fix64)0.2;
				Fix64 SpaceToParkTightParkHandleLength = (Fix64)0.3;
				pathSpline = new Spline.BezierSplineFixed(spaceStartIn, spaceStartIn + parkToSpaceGentleSpaceHandleDirection * SpaceToParkGentleSpaceHandleLength, spaceStart + parkDirectionVec * SpaceToParkGentleParkHandleLength, spaceStart);
				LaneModel previousSpaceToParkLane = space.outerRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(secondaryDirection, RoadType.Carpark, -1), new RoadTileNode(carparkAngleDirOut, RoadType.ParkingSpace, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
				pathSpline = new Spline.BezierSplineFixed(spaceStart, spaceStart + parkDirectionVec * SpaceToParkTightParkHandleLength, spaceStartOut - new Vector2Fixed(spaceToSpaceDirection) * SpaceToParkTightSpaceHandleLength, spaceStartOut);
				space.outerRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(carparkAngleDirOut, RoadType.ParkingSpace, -1), new RoadTileNode(legacyDirection, RoadType.Carpark, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
				List<Vector2Fixed> path;
				if (!isFirstSpace)
				{
					path = new List<Vector2Fixed>
					{
						spaceStartIn,
						spaceStartOut
					};
				}
				else
				{
					pathSpline = new Spline.BezierSplineFixed(spaceStartIn, spaceStartIn - parkNormalVec * SpaceToParkTightParkHandleLength, spaceStartOut - new Vector2Fixed(spaceToSpaceDirection) * SpaceToParkTightSpaceHandleLength, spaceStartOut);
					path = pathSpline.Rasterize(10);
				}
				LaneModel previousSpaceToNextSpaceLane = space.outerRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(secondaryDirection, RoadType.Carpark, -1), new RoadTileNode(legacyDirection, RoadType.Carpark, -1)), path, RoadState.Active, true, false);
				space.innerRoadChunk = this._scope.Get<RoadChunkModel>();
				pathSpline = new Spline.BezierSplineFixed(spaceEndIn, spaceEndIn - spaceToParkGentleSpaceHandleDirection * SpaceToParkGentleSpaceHandleLength, spaceEnd - parkDirectionVec * SpaceToParkGentleParkHandleLength, spaceEnd);
				space.innerRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(legacyDirection, RoadType.Carpark, -1), new RoadTileNode(carparkAngleDirIn, RoadType.ParkingSpace, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
				pathSpline = new Spline.BezierSplineFixed(spaceEnd, spaceEnd - parkDirectionVec * SpaceToParkTightParkHandleLength, spaceEndOut + new Vector2Fixed(spaceToSpaceDirection) * SpaceToParkTightSpaceHandleLength, spaceEndOut);
				LaneModel parkToPreviousSpaceLane = space.innerRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(carparkAngleDirIn, RoadType.ParkingSpace, -1), new RoadTileNode(secondaryDirection, RoadType.Carpark, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
				if (!isLastSpace)
				{
					path = new List<Vector2Fixed>
					{
						spaceEndIn,
						spaceEndOut
					};
				}
				else
				{
					pathSpline = new Spline.BezierSplineFixed(spaceEndIn, spaceEndIn + parkNormalVec * SpaceToParkTightParkHandleLength, spaceEndOut + new Vector2Fixed(spaceToSpaceDirection) * SpaceToParkTightSpaceHandleLength, spaceEndOut);
					path = pathSpline.Rasterize(10);
				}
				LaneModel nextSpaceToPreviousSpaceLane = space.innerRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(legacyDirection, RoadType.Carpark, -1), new RoadTileNode(secondaryDirection, RoadType.Carpark, -1)), path, RoadState.Active, true, false);
				space.parkRoadChunk = this._scope.Get<RoadChunkModel>();
				path = new List<Vector2Fixed>
				{
					spaceStart,
					spaceEnd
				};
				LaneModel startToEndLane = space.parkRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(carparkAngleDirIn, RoadType.ParkingSpace, -1), new RoadTileNode(carparkAngleDirOut, RoadType.ParkingSpace, -1)), path, RoadState.Active, true, false);
				space.outerRoadChunk.ConnectOutboundLane(startToEndLane);
				space.innerRoadChunk.ConnectInboundLane(startToEndLane);
				path = new List<Vector2Fixed>
				{
					spaceEnd,
					spaceStart
				};
				LaneModel endToStartLane = space.parkRoadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(carparkAngleDirOut, RoadType.ParkingSpace, -1), new RoadTileNode(carparkAngleDirIn, RoadType.ParkingSpace, -1)), path, RoadState.Active, true, false);
				space.outerRoadChunk.ConnectInboundLane(endToStartLane);
				space.innerRoadChunk.ConnectOutboundLane(endToStartLane);
				if (spaceIndex > 0)
				{
					CarparkModel.ParkingSpace parkingSpace = this.spaces[spaceIndex - 1];
					parkingSpace.innerRoadChunk.ConnectInboundLane(parkToPreviousSpaceLane);
					parkingSpace.innerRoadChunk.ConnectInboundLane(nextSpaceToPreviousSpaceLane);
					parkingSpace.outerRoadChunk.ConnectOutboundLane(previousSpaceToParkLane);
					parkingSpace.outerRoadChunk.ConnectOutboundLane(previousSpaceToNextSpaceLane);
				}
				this.spaces.Add(space);
				this._simulation.AddModel(space.outerRoadChunk);
				this._simulation.AddModel(space.innerRoadChunk);
				this._simulation.AddModel(space.parkRoadChunk);
			}
			Fix64 TightEntranceRoadHandleLength = (Fix64)0.2;
			Fix64 TightEntranceCarparkHandleLength = (Fix64)0.2;
			Fix64 ShortEntranceCarparkHandleLength = (Fix64)0.1;
			Fix64 WideEntranceCarparkHandleLength = (Fix64)1.0;
			Fix64 WideEntranceRoadHandleLength = (Fix64)0.75;
			Fix64 ShortUturnHandleLength = (Fix64)0.7;
			Fix64 LongUturnHandleLength = (Fix64)0.3;
			TileModel topLeftTile = this._tilemap.GetTileModel(this.TopLeftCarparkTileCoordinate);
			if (topLeftTile == null)
			{
				CarparkModel.Log.Error("Top left tile is null!! Coordinates: {0}", new object[]
				{
					this.TopLeftCarparkTileCoordinate
				});
				return false;
			}
			Vector2Fixed topLeftEdge = new Vector2Fixed(TileUtilities.DirectionToTileAdjacencyOffset[(int)secondaryDirection]);
			Vector2Fixed topLeftEdgeTangent = new Vector2Fixed(topLeftEdge.y, -topLeftEdge.x);
			Vector2Fixed topLeftIn = topLeftEdge - topLeftEdgeTangent * RoadTileAtlas.LaneOffsetScale + topLeftTile.WorldPosition;
			Vector2Fixed topLeftOut = topLeftEdge + topLeftEdgeTangent * RoadTileAtlas.LaneOffsetScale + topLeftTile.WorldPosition;
			CarparkModel.ParkingSpace firstSpace = this.spaces[0];
			Vector2Fixed outerChunkIn = firstSpace.outerRoadChunk.GetLanesEnteringFromDirection(secondaryDirection)[0].StartPosition;
			Vector2Fixed innerChunkOut = firstSpace.innerRoadChunk.GetLanesExitingInDirection(secondaryDirection)[0].EndPosition;
			pathSpline = new Spline.BezierSplineFixed(innerChunkOut, innerChunkOut - new Vector2Fixed(spaceToSpaceDirection) * LongUturnHandleLength, outerChunkIn + parkNormalVec * ShortUturnHandleLength, outerChunkIn);
			LaneModel topLeftUTurnLane = topLeftTile.roadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(legacyDirection, RoadType.Carpark, -1), new RoadTileNode(legacyDirection, RoadType.Carpark, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
			firstSpace.innerRoadChunk.ConnectOutboundLane(topLeftUTurnLane);
			firstSpace.outerRoadChunk.ConnectInboundLane(topLeftUTurnLane);
			if (this.entranceAtTopLeft)
			{
				pathSpline = new Spline.BezierSplineFixed(topLeftIn, topLeftIn + new Vector2Fixed(spaceToSpaceDirection) * TightEntranceRoadHandleLength, outerChunkIn + parkNormalVec * ShortEntranceCarparkHandleLength, outerChunkIn);
				LaneModel topLeftInLane = topLeftTile.roadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(secondaryDirection, RoadType.TwoLane, -1), new RoadTileNode(legacyDirection, RoadType.Carpark, -1)), pathSpline.Rasterize(10), RoadState.Active, true, true);
				firstSpace.outerRoadChunk.ConnectInboundLane(topLeftInLane);
				this.entranceLanes.Add(topLeftInLane);
				this._cityPlanModel.destinationLanes.Add(topLeftInLane);
				pathSpline = new Spline.BezierSplineFixed(innerChunkOut, innerChunkOut - new Vector2Fixed(spaceToSpaceDirection) * WideEntranceCarparkHandleLength, topLeftOut + new Vector2Fixed(spaceToSpaceDirection) * WideEntranceRoadHandleLength, topLeftOut);
				LaneModel topLeftOutLane = topLeftTile.roadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(legacyDirection, RoadType.Carpark, -1), new RoadTileNode(secondaryDirection, RoadType.TwoLane, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
				firstSpace.innerRoadChunk.ConnectOutboundLane(topLeftOutLane);
			}
			TileModel bottomRightTile = this._tilemap.GetTileModel(bottomRightCoordinates);
			Vector2Fixed bottomRightEdge = new Vector2Fixed(TileUtilities.DirectionToTileAdjacencyOffset[(int)legacyDirection]);
			Vector2Fixed bottomRightEdgeTangent = new Vector2Fixed(bottomRightEdge.y, -bottomRightEdge.x);
			Vector2Fixed bottomRightIn = bottomRightEdge - bottomRightEdgeTangent * RoadTileAtlas.LaneOffsetScale + bottomRightTile.WorldPosition;
			Vector2Fixed bottomRightOut = bottomRightEdge + bottomRightEdgeTangent * RoadTileAtlas.LaneOffsetScale + bottomRightTile.WorldPosition;
			CarparkModel.ParkingSpace lastSpace = this.spaces[this.spaces.Count - 1];
			Vector2Fixed innerChunkIn = lastSpace.innerRoadChunk.GetLanesEnteringFromDirection(legacyDirection)[0].StartPosition;
			Vector2Fixed outerChunkOut = lastSpace.outerRoadChunk.GetLanesExitingInDirection(legacyDirection)[0].EndPosition;
			pathSpline = new Spline.BezierSplineFixed(outerChunkOut, outerChunkOut + new Vector2Fixed(spaceToSpaceDirection) * LongUturnHandleLength, innerChunkIn - parkNormalVec * ShortUturnHandleLength, innerChunkIn);
			LaneModel bottomRightUTurnLane = bottomRightTile.roadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(secondaryDirection, RoadType.Carpark, -1), new RoadTileNode(secondaryDirection, RoadType.Carpark, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
			lastSpace.outerRoadChunk.ConnectOutboundLane(bottomRightUTurnLane);
			lastSpace.innerRoadChunk.ConnectInboundLane(bottomRightUTurnLane);
			if (this.entranceAtBottomRight)
			{
				pathSpline = new Spline.BezierSplineFixed(bottomRightIn, bottomRightIn - new Vector2Fixed(spaceToSpaceDirection) * TightEntranceCarparkHandleLength, innerChunkIn - parkNormalVec * TightEntranceRoadHandleLength, innerChunkIn);
				LaneModel bottomRightInLane = bottomRightTile.roadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(legacyDirection, RoadType.TwoLane, -1), new RoadTileNode(secondaryDirection, RoadType.Carpark, -1)), pathSpline.Rasterize(10), RoadState.Active, true, true);
				lastSpace.innerRoadChunk.ConnectInboundLane(bottomRightInLane);
				this.entranceLanes.Add(bottomRightInLane);
				this._cityPlanModel.destinationLanes.Add(bottomRightInLane);
				pathSpline = new Spline.BezierSplineFixed(outerChunkOut, outerChunkOut + new Vector2Fixed(spaceToSpaceDirection) * WideEntranceCarparkHandleLength, bottomRightOut - new Vector2Fixed(spaceToSpaceDirection) * WideEntranceRoadHandleLength, bottomRightOut);
				LaneModel bottomRightOutLane = bottomRightTile.roadChunk.AddBespokeLane(new RoadTileConnection(new RoadTileNode(secondaryDirection, RoadType.Carpark, -1), new RoadTileNode(legacyDirection, RoadType.TwoLane, -1)), pathSpline.Rasterize(10), RoadState.Active, true, false);
				lastSpace.outerRoadChunk.ConnectOutboundLane(bottomRightOutLane);
			}
			return true;
		}

		// Token: 0x06002050 RID: 8272 RVA: 0x0007FF0C File Offset: 0x0007E10C
		public void AddDestination(DestinationModel model)
		{
			this.destinations.Add(model);
			foreach (CarparkModel.IObserver observer in base.Observers)
			{
				observer.OnDestinationAdded();
			}
		}

		// Token: 0x06002051 RID: 8273 RVA: 0x0007FF48 File Offset: 0x0007E148
		public LaneModel GetClosestEntranceLane(Vector2Fixed worldPosition)
		{
			if (!Diagnostics.Verify(this.entranceLanes.Count > 0, "Cannot get distance from a carpark with no entrances."))
			{
				return null;
			}
			LaneModel closestEntranceLane = this.entranceLanes[0];
			Fix64 closestSquaredDistance = (this.entranceLanes[0].StartPosition - worldPosition).sqrMagnitude;
			for (int entranceIndex = 1; entranceIndex < this.entranceLanes.Count; entranceIndex++)
			{
				Fix64 squaredDistance = (this.entranceLanes[entranceIndex].StartPosition - worldPosition).sqrMagnitude;
				if (squaredDistance < closestSquaredDistance)
				{
					closestEntranceLane = this.entranceLanes[entranceIndex];
					closestSquaredDistance = squaredDistance;
				}
			}
			return closestEntranceLane;
		}

		// Token: 0x06002052 RID: 8274 RVA: 0x0007FFF0 File Offset: 0x0007E1F0
		public Fix64 GetSquaredDistanceToClosestEntrance(Vector2Fixed worldPosition)
		{
			LaneModel closestEntranceLane = this.GetClosestEntranceLane(worldPosition);
			if (!Diagnostics.Verify(closestEntranceLane != null, "Cannot get distance from a carpark with no entrances."))
			{
				return -Fix64.One;
			}
			return (closestEntranceLane.StartPosition - worldPosition).sqrMagnitude;
		}

		// Token: 0x06002053 RID: 8275 RVA: 0x00080034 File Offset: 0x0007E234
		public DestinationModel GetNeighboringDestination(DestinationModel destination)
		{
			foreach (DestinationModel neighborDestination in this.destinations)
			{
				if (neighborDestination != destination)
				{
					return neighborDestination;
				}
			}
			return null;
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x0008008C File Offset: 0x0007E28C
		public void Remove()
		{
			foreach (DestinationModel destination in this.destinations)
			{
				if (destination.isActive)
				{
					destination.Remove();
				}
			}
			foreach (Vector2Int destinationOffset in this.destinationOffsets)
			{
				Vector2Int destinationPosition = this.origin + destinationOffset;
				for (int x = 0; x < 2; x++)
				{
					for (int y = 0; y < 2; y++)
					{
						Vector2Int destinationTilePosition = destinationPosition + new Vector2Int(x, y);
						TileModel destinationTile = this._tilemap.GetTileModel(destinationTilePosition);
						if (destinationTile != null)
						{
							Tile tile = destinationTile.Tile;
							TileContentType? tileContentType = (tile != null) ? new TileContentType?(tile.ContentType) : null;
							TileContentType tileContentType2 = TileContentType.Destination;
							if (tileContentType.GetValueOrDefault() == tileContentType2 & tileContentType != null)
							{
								destinationTile.Tile.SetContentType(TileContentType.None, null);
							}
						}
						if (((destinationTile != null) ? destinationTile.RailTileModel : null) != null)
						{
							destinationTile.RailTileModel.carpark = null;
						}
					}
				}
			}
			foreach (LaneModel entranceLane in this.entranceLanes)
			{
				CarparkModel.<Remove>g__SendInboundVehiclesHome|50_0(entranceLane.roadChunk, entranceLane);
				this._cityPlanModel.destinationLanes.Remove(entranceLane);
			}
			int length = (this.Alignment == TileAlignment.Horizontal) ? this.footprint.x : this.footprint.y;
			Vector2Int topLeftCoordinates = new Vector2Int(int.MaxValue, int.MinValue);
			Vector2Int bottomRightCoordinates = new Vector2Int(int.MinValue, int.MaxValue);
			for (int tileIndex = 0; tileIndex < this.carparkTiles.Count; tileIndex++)
			{
				TileDirection outboundDirection = TileDirection.None;
				if (tileIndex == 0 && this.entranceAtTopLeft)
				{
					outboundDirection = this.TopLeftDrivewayDirection;
				}
				else if (tileIndex == length - 1 && this.entranceAtBottomRight)
				{
					outboundDirection = this.BottomRightDrivewayDirection;
				}
				if (outboundDirection != TileDirection.None)
				{
					Vector2Int drivewayCoordinate = (outboundDirection == TileDirection.South || outboundDirection == TileDirection.East) ? this.BottomRightCarparkTileCoordinate : this.TopLeftCarparkTileCoordinate;
					TileModel drivewayTile = this._tilemap.GetTileModel(drivewayCoordinate);
					TileModel connectedTile = this._tilemap.GetTileModel(TileUtilities.GetAdjacentCoordinates(drivewayCoordinate, outboundDirection));
					if (connectedTile == null || drivewayTile == null)
					{
						CarparkModel.Log.Error("connectedTile or drivewayTile were null in CarparkModel.Remove!!", Array.Empty<object>());
					}
					else
					{
						TileDirection inboundDirection = TileUtilities.GetOppositeDirection(outboundDirection);
						connectedTile.Tile.SetNodeImmutability(inboundDirection, false);
						connectedTile.Tile.SetNodeState(new RoadTileNode(inboundDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
						drivewayTile.Tile.SetNodeState(new RoadTileNode(outboundDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
						connectedTile.Tile.SetNodeState(new RoadTileNode(inboundDirection, RoadType.TwoLane, -1), RoadState.None, Tile.TileChangePermissions.Full);
						drivewayTile.Tile.SetNodeState(new RoadTileNode(outboundDirection, RoadType.TwoLane, -1), RoadState.None, Tile.TileChangePermissions.Full);
						List<LaneModel> entranceLaneModels = connectedTile.roadChunk.GetLanesConnectedToDirection(RoadState.Pending | RoadState.Active | RoadState.Mothballed, inboundDirection);
						for (int entranceLaneIndex = entranceLaneModels.Count - 1; entranceLaneIndex >= 0; entranceLaneIndex--)
						{
							LaneModel lane = entranceLaneModels[entranceLaneIndex];
							CarparkModel.<Remove>g__SendInboundVehiclesHome|50_0(lane.roadChunk, lane);
							for (int vehicleIndex = lane.Vehicles.Count - 1; vehicleIndex >= 0; vehicleIndex--)
							{
								lane.Vehicles[vehicleIndex].ResetToHouse();
							}
							lane.roadChunk.RemoveLane(lane);
						}
					}
				}
				Vector2Int carparkTileOffset = this.carparkTiles[tileIndex];
				Vector2Int carparkTilePosition = this.origin + carparkTileOffset;
				Tile carparkTile = this._tilemap.GetTile(carparkTilePosition);
				if (Diagnostics.Verify(carparkTile != null && carparkTile.CanSetContentType(TileContentType.None), "Unable to unset TileContentType on one of a carpark's tiles at {0} (was null: {1})!", this.TopLeftCarparkTileCoordinate, carparkTile == null))
				{
					carparkTile.SetContentType(TileContentType.None, null);
				}
				if (carparkTilePosition.x < topLeftCoordinates.x || carparkTilePosition.y > topLeftCoordinates.y)
				{
					topLeftCoordinates = carparkTilePosition;
				}
				if (carparkTilePosition.x > bottomRightCoordinates.x || carparkTilePosition.y < bottomRightCoordinates.y)
				{
					bottomRightCoordinates = carparkTilePosition;
				}
			}
			foreach (CarparkModel.ParkingSpace parkingSpace in this.spaces)
			{
				CarparkModel.<Remove>g__SendInboundVehiclesHome|50_0(parkingSpace.parkRoadChunk, null);
				parkingSpace.parkRoadChunk.RemoveAllLanes();
				parkingSpace.outerRoadChunk.RemoveAllLanes();
				parkingSpace.innerRoadChunk.RemoveAllLanes();
			}
			this.spaces.Clear();
			TileModel tileModel = this._tilemap.GetTileModel(this.TopLeftCarparkTileCoordinate);
			RoadChunkModel roadChunkModel = (tileModel != null) ? tileModel.roadChunk : null;
			if (roadChunkModel != null)
			{
				roadChunkModel.RemoveAllLanes();
			}
			TileModel tileModel2 = this._tilemap.GetTileModel(bottomRightCoordinates);
			RoadChunkModel roadChunkModel2 = (tileModel2 != null) ? tileModel2.roadChunk : null;
			if (roadChunkModel2 != null)
			{
				roadChunkModel2.RemoveAllLanes();
			}
			foreach (CarparkModel.IObserver observer in base.Observers)
			{
				observer.OnCarparkRemoved(this);
			}
			this._simulation.RemoveModel(this);
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x000805E4 File Offset: 0x0007E7E4
		public override void Reset()
		{
			base.Reset();
			this.origin.x = 0;
			this.origin.y = 0;
			this.footprint.x = 0;
			this.footprint.y = 0;
			this.carparkSide = TileDirection.None;
			this.entranceAtTopLeft = false;
			this.entranceAtBottomRight = false;
			this.destinationOffsets = null;
			this.carparkTiles = null;
			this.TileModels = null;
			this.supportsBoats = false;
		}

		// Token: 0x06002056 RID: 8278 RVA: 0x00080658 File Offset: 0x0007E858
		public override void OnReleasedFromScope(IScope scope)
		{
			base.OnReleasedFromScope(scope);
			foreach (CarparkModel.ParkingSpace space in this.spaces)
			{
				scope.Release(space);
			}
			this.spaces.Clear();
			this.entranceLanes.Clear();
			this.vehiclesEntering.Clear();
			this.vehiclesDrivingThrough.Clear();
			this.destinations.Clear();
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x000806EC File Offset: 0x0007E8EC
		public CarparkModel() : base(1)
		{
		}

		// Token: 0x06002059 RID: 8281 RVA: 0x0008073D File Offset: 0x0007E93D
		[CompilerGenerated]
		internal static void <Remove>g__SendInboundVehiclesHome|50_0(RoadChunkModel roadChunk, LaneModel laneModel = null)
		{
			CarparkModel.<Remove>g__SendListedInboundVehiclesHome|50_1(roadChunk.inboundVehicles, laneModel);
			CarparkModel.<Remove>g__SendListedInboundVehiclesHome|50_1(roadChunk.returningInboundVehicles, laneModel);
		}

		// Token: 0x0600205A RID: 8282 RVA: 0x00080758 File Offset: 0x0007E958
		[CompilerGenerated]
		internal static void <Remove>g__SendListedInboundVehiclesHome|50_1(List<RoadChunkModel.InboundVehicle> inboundVehicles, LaneModel laneModel = null)
		{
			for (int index = inboundVehicles.Count - 1; index >= 0; index--)
			{
				if (index < inboundVehicles.Count && (laneModel == null || inboundVehicles[index].chosenLane == laneModel))
				{
					inboundVehicles[index].vehicle.ResetToHouse();
				}
			}
		}

		// Token: 0x04001ACC RID: 6860
		private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("CarparkModel");

		// Token: 0x04001ACE RID: 6862
		[Dependency]
		private CityPlanModel _cityPlanModel;

		// Token: 0x04001ACF RID: 6863
		public Vector2Int origin;

		// Token: 0x04001AD0 RID: 6864
		public TileDirection carparkSide;

		// Token: 0x04001AD1 RID: 6865
		public Vector2Int footprint;

		// Token: 0x04001AD2 RID: 6866
		public bool entranceAtTopLeft;

		// Token: 0x04001AD3 RID: 6867
		public bool entranceAtBottomRight;

		// Token: 0x04001AD4 RID: 6868
		public bool supportsBoats;

		// Token: 0x04001AD5 RID: 6869
		public List<Vector2Int> carparkTiles;

		// Token: 0x04001AD6 RID: 6870
		public List<Vector2Int> destinationOffsets;

		// Token: 0x04001AD7 RID: 6871
		public List<LaneModel> entranceLanes = new List<LaneModel>();

		// Token: 0x04001AD8 RID: 6872
		public List<CarparkModel.ParkingSpace> spaces = new List<CarparkModel.ParkingSpace>();

		// Token: 0x04001AD9 RID: 6873
		public List<VehicleModel> vehiclesEntering = new List<VehicleModel>();

		// Token: 0x04001ADA RID: 6874
		public List<VehicleModel> vehiclesDrivingThrough = new List<VehicleModel>();

		// Token: 0x04001ADB RID: 6875
		public List<DestinationModel> destinations = new List<DestinationModel>();

		// Token: 0x04001ADC RID: 6876
		[Dependency]
		private IScope _scope;

		// Token: 0x04001ADD RID: 6877
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x04001ADE RID: 6878
		[Dependency]
		private TilemapModel _tilemap;

		// Token: 0x020004D9 RID: 1241
		[Factory.Serializable(1)]
		public class ParkingSpace : IReusable
		{
			// Token: 0x0600205B RID: 8283 RVA: 0x000807A4 File Offset: 0x0007E9A4
			public void Reset()
			{
				this.parkRoadChunk = null;
				this.innerRoadChunk = null;
				this.outerRoadChunk = null;
				this.vehicle = null;
				this.timeVehicleParked = -Fix64.One;
			}

			// Token: 0x04001ADF RID: 6879
			public RoadChunkModel parkRoadChunk;

			// Token: 0x04001AE0 RID: 6880
			public RoadChunkModel innerRoadChunk;

			// Token: 0x04001AE1 RID: 6881
			public RoadChunkModel outerRoadChunk;

			// Token: 0x04001AE2 RID: 6882
			public VehicleModel vehicle;

			// Token: 0x04001AE3 RID: 6883
			public Fix64 timeVehicleParked;
		}

		// Token: 0x020004DA RID: 1242
		public interface IObserver
		{
			// Token: 0x0600205D RID: 8285
			void OnCarparkRemoved(CarparkModel carparkModel);

			// Token: 0x0600205E RID: 8286
			void OnDestinationAdded();
		}
	}
}
