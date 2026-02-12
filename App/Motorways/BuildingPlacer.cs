using System;
using System.Collections.Generic;
using System.Diagnostics;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.Processes;
using Motorways.Views;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000357 RID: 855
	public class BuildingPlacer : IReusable, IReleasedFromScopeHandler
	{
		// Token: 0x060014F7 RID: 5367 RVA: 0x00045D74 File Offset: 0x00043F74
		public BuildingPlacer()
		{
			while (this._placementPool.Count < 100)
			{
				this._placementPool.Add(new BuildingPlacer.Placement());
			}
		}

		// Token: 0x060014F8 RID: 5368 RVA: 0x00045E04 File Offset: 0x00044004
		public void Reset()
		{
			this._baseTileData = null;
			this._placeableArea = default(RectInt);
			this._buildingType = TileContentType.None;
			this._groupIndex = 0;
			this._grouping = GroupingStyle.Normal;
			this._possiblePlacements.Clear();
			this._usedPlacementCount = 0;
			this._placeableTiles.Clear();
			this._placeableTileWeights.Clear();
			this._placeableTileDriveabilities.Clear();
			this._placeableTileWeightsContext.Clear();
			this._placeableTileRails.Clear();
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x00045E82 File Offset: 0x00044082
		public void SetTileData(CitySpawningLayerData tileData)
		{
			this._baseTileData = tileData;
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x00045E8C File Offset: 0x0004408C
		private bool IsTileBuildable(Vector2Int tileCoordinates, bool ignoreUnzoneableTiles = false)
		{
			bool isTileBuildable = !this._tilemapModel.IsTileReserved(tileCoordinates) && (this._city.Definition.TileIsZoneable(tileCoordinates) || ignoreUnzoneableTiles);
			TileModel tileModel = this._tilemapModel.GetTileModel(tileCoordinates);
			if (tileModel != null && isTileBuildable)
			{
				isTileBuildable &= tileModel.Tile.CanSetContentType(this._buildingType);
				if (this._city.Rules.CanBuildingsDemolishUnusedRoads && !isTileBuildable)
				{
					isTileBuildable = !tileModel.Tile.AnyRoadHasPermanenceBelowValue(this._constants.PercentageOfPermanenceTimerWhereRoadsCannotBeDemolished, RoadState.Live);
					this._testTile = this.CreateDemolishedTestTileFrom(tileModel.Tile);
					isTileBuildable &= this._testTile.CanSetContentType(this._buildingType);
					isTileBuildable &= !this.IsTileConnectedToBuildingAndHouse(tileModel);
				}
			}
			return isTileBuildable;
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x00045F4C File Offset: 0x0004414C
		public void StartPlacing(TileContentType buildingType, int groupIndex, GroupingStyle grouping, BuildingPlacer.WeightEvaluationLevel weightEvaluationLevel = BuildingPlacer.WeightEvaluationLevel.ExclusivelyUseWeightedTiles, BuildingPlacer.WeightSource weightSource = BuildingPlacer.WeightSource.Default)
		{
			this._possiblePlacements.Clear();
			this._usedPlacementCount = 0;
			this._buildingType = buildingType;
			this._placeableTiles.Clear();
			this._placeableTileWeights.Clear();
			this._placeableTileDriveabilities.Clear();
			this._placeableTileWeightsContext.Clear();
			this._cachedHouseLanes.Clear();
			this._placeableTileRails.Clear();
			this._groupIndex = groupIndex;
			this._grouping = grouping;
			if (this._city.Rules.DoesIgnorePlayableArea())
			{
				this._placeableArea.SetMinMax(Vector2Int.zero, Vector2Int.zero);
			}
			else
			{
				Fix64 time = this._clock.ExpansionTime;
				RectFixed placeableAreaFixed = this._city.GetSimulationPlayableAreaAtTime(time, City.PlayableAreaRoundingType.ForceWholeTiles);
				Vector2Int placeableAreaMinimum = new Vector2Int((int)((long)placeableAreaFixed.xMin), (int)((long)placeableAreaFixed.yMin));
				if (this._city.Rules.AllowSpawningAtMapEdges)
				{
					this._placeableArea.SetMinMax(placeableAreaMinimum, new Vector2Int((int)((long)placeableAreaFixed.xMax) + 1, (int)((long)placeableAreaFixed.yMax) + 1));
				}
				else
				{
					this._placeableArea.SetMinMax(placeableAreaMinimum + Vector2Int.one, new Vector2Int((int)((long)placeableAreaFixed.xMax), (int)((long)placeableAreaFixed.yMax)));
				}
				int tilemapLayerId = CityTilemap.LayerIdFor((this._buildingType == TileContentType.House) ? CityTileType.Supply : CityTileType.Demand, groupIndex);
				BuildingSpawningTileWeights tileWeights = null;
				if (weightEvaluationLevel != BuildingPlacer.WeightEvaluationLevel.IgnoreWeights)
				{
					switch (weightSource)
					{
					case BuildingPlacer.WeightSource.Default:
						Diagnostics.Verify(this._baseTileData != null && this._baseTileData.weights.TryGetValue(tilemapLayerId, out tileWeights), "There is no layer present for {0} {1}!", this._buildingType, groupIndex);
						break;
					case BuildingPlacer.WeightSource.Station:
					{
						CitySpawningLayerData baseTileData = this._baseTileData;
						if (Diagnostics.Verify(((baseTileData != null) ? baseTileData.stationWeights : null) != null, "Trying to place station but there are no station weights available."))
						{
							tileWeights = this._baseTileData.stationWeights;
						}
						break;
					}
					case BuildingPlacer.WeightSource.BoatTerminal:
					{
						CitySpawningLayerData baseTileData2 = this._baseTileData;
						if (Diagnostics.Verify(((baseTileData2 != null) ? baseTileData2.boatTerminalWeights : null) != null, "Trying to place ferry terminal but there are no ferry terminal weights available."))
						{
							tileWeights = this._baseTileData.boatTerminalWeights;
						}
						break;
					}
					default:
						throw new ArgumentOutOfRangeException("weightSource", weightSource, null);
					}
				}
				for (int y = 0; y < this._placeableArea.height; y++)
				{
					for (int x = 0; x < this._placeableArea.width; x++)
					{
						Vector2Int tileCoordinates = new Vector2Int(this._placeableArea.xMin + x, this._placeableArea.yMin + y);
						bool isTileDriveable = this._city.Definition.TileIsDriveable(tileCoordinates);
						bool isRailTile = this._city.Definition.TileIsOverRail(tileCoordinates);
						bool flag = isTileDriveable && this.IsTileBuildable(tileCoordinates, this._city.Rules.AllowPlacingBuildingsOnUnzoneableTiles);
						Tile tile = this._tilemapModel.GetTile(tileCoordinates);
						this._placeableTiles.Add(tile);
						this._placeableTileRails.Add(isRailTile);
						if (flag)
						{
							Fix64 baseWeight = this.GetBaseWeightForTile(tileCoordinates, weightEvaluationLevel, tileWeights);
							if (baseWeight > Fix64.Zero && this._behaviour.BuildingSpawnsAreAffectedByOtherBuildings())
							{
								baseWeight = this.ScaleTileWeightByBuildingInfluence(baseWeight, groupIndex, tileCoordinates, buildingType, this._placeableTileWeightsContext.Count - 1);
							}
							this._placeableTileWeights.Add((baseWeight >= Fix64.Zero) ? baseWeight : (-Fix64.One));
						}
						else
						{
							this._placeableTileWeights.Add(-Fix64.One);
						}
						bool isTileFree = tile == null || tile.ContentType == TileContentType.None || (tile.ContentType == TileContentType.Tree && this._city.Rules.ShouldBuildingsBulldozeTrees);
						this._placeableTileDriveabilities.Add(isTileDriveable && isTileFree);
					}
				}
				TilemapModel tilemap = this._simulation.GetModel<TilemapModel>();
				foreach (Vector2Int tilePosition in tilemap.GetAllTileCoordinates())
				{
					if (tilemap.GetTile(tilePosition).IsCenterOfRoundabout)
					{
						for (int x2 = -1; x2 <= 1; x2++)
						{
							for (int y2 = -1; y2 <= 1; y2++)
							{
								if (Math.Abs(x2) != 1 || Math.Abs(y2) != 1 || !this._city.Rules.AllowSpawnsOnRoundaboutDeadzone)
								{
									Vector2Int absolutePosition = tilePosition + new Vector2Int(x2, y2);
									if (this._placeableArea.Contains(absolutePosition))
									{
										Vector2Int relativePosition = absolutePosition - this._placeableArea.min;
										int index = relativePosition.x + relativePosition.y * this._placeableArea.width;
										this._placeableTileWeights[index] = -Fix64.One;
									}
								}
							}
						}
					}
				}
			}
			foreach (HouseModel house in this._simulation.GetModels<HouseModel>())
			{
				Vector2Int basePosition = house.tileModel.Coordinates;
				DensityGroup densityGroup = this._city.Definition.DensityForPosition(basePosition);
				int densityRadius = (int)(densityGroup * DensityGroup.Low);
				string tileWeightChangeContext = "";
				switch (densityGroup)
				{
				case DensityGroup.High:
					tileWeightChangeContext = "high density tile";
					break;
				case DensityGroup.Medium:
					tileWeightChangeContext = "medium density tile";
					break;
				case DensityGroup.Low:
					tileWeightChangeContext = "low density tile";
					break;
				}
				this.ChangeTileWeightsAroundBuilding(basePosition, densityRadius, Fix64Consts.OneHalf, BuildingSpawningProcess.HouseFootprint, tileWeightChangeContext);
				if (this._behaviour.UseDestinationDeadzonesFor(CityTileType.Supply) && (buildingType == TileContentType.Destination || buildingType == TileContentType.Carpark))
				{
					foreach (Vector2Int position in TileUtilities.GetBoundsWithBoundary(house.tileModel.Coordinates, BuildingSpawningProcess.HouseFootprint, 1).allPositionsWithin)
					{
						if (this._placeableArea.Contains(position))
						{
							Vector2Int relativePosition2 = position - this._placeableArea.min;
							int index2 = relativePosition2.x + relativePosition2.y * this._placeableArea.width;
							this._placeableTileWeights[index2] = -Fix64.One;
						}
					}
				}
			}
			foreach (DestinationModel destination in this._simulation.GetModels<DestinationModel>())
			{
				if (destination.isActive)
				{
					if (this._behaviour.BuildingSpawnsAreAffectedByOtherBuildings() && (buildingType == TileContentType.Carpark || buildingType == TileContentType.Destination) && destination.GroupIndex == groupIndex)
					{
						Vector2Int basePosition2 = destination.Carpark.origin;
						this.ChangeTileWeightsAroundBuilding(basePosition2, 5, Fix64Consts.OneHalf, destination.Carpark.footprint, "proximity to destination");
					}
					bool addDeadzone = false;
					if (this._behaviour.UseDestinationDeadzonesFor(CityTileType.Supply))
					{
						addDeadzone |= (buildingType == TileContentType.House);
					}
					if (this._behaviour.UseDestinationDeadzonesFor(CityTileType.Demand))
					{
						addDeadzone |= (buildingType == TileContentType.Carpark || buildingType == TileContentType.Destination);
					}
					if (addDeadzone)
					{
						foreach (Vector2Int position2 in TileUtilities.GetBoundsWithBoundary(destination.Carpark.TopLeftWorldCoordinate, destination.Carpark.footprint, 1).allPositionsWithin)
						{
							if (this._placeableArea.Contains(position2))
							{
								Vector2Int relativePosition3 = position2 - this._placeableArea.min;
								int index3 = relativePosition3.x + relativePosition3.y * this._placeableArea.width;
								this._placeableTileWeights[index3] = -Fix64.One;
							}
						}
					}
					if ((this._buildingType == TileContentType.Destination || this._buildingType == TileContentType.Carpark) && !this._city.Rules.AllowConnectingDriveways)
					{
						if (destination.Carpark.entranceAtTopLeft)
						{
							this.ForcePositionInvalidDueToDriveway(destination.Carpark.TopLeftDrivewayTileCoordinates);
						}
						if (destination.Carpark.entranceAtBottomRight)
						{
							this.ForcePositionInvalidDueToDriveway(destination.Carpark.BottomRightDrivewayTileCoordinates);
						}
					}
				}
			}
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x00046798 File Offset: 0x00044998
		private Fix64 GetBaseWeightForTile(Vector2Int tileCoordinates, BuildingPlacer.WeightEvaluationLevel weightEvaluationLevel, BuildingSpawningTileWeights tileWeights)
		{
			Fix64 baseWeight = -Fix64.One;
			if (weightEvaluationLevel == BuildingPlacer.WeightEvaluationLevel.IgnoreWeights)
			{
				baseWeight = Fix64.One;
			}
			else if (tileWeights != null && !tileWeights.weights.TryGetValue((Vector3Int)tileCoordinates, out baseWeight))
			{
				if (weightEvaluationLevel == BuildingPlacer.WeightEvaluationLevel.ExclusivelyUseWeightedTiles)
				{
					baseWeight = -Fix64.One;
				}
				else if (weightEvaluationLevel == BuildingPlacer.WeightEvaluationLevel.AllowNonWeightedTiles)
				{
					baseWeight = Fix64.Zero;
				}
				else
				{
					Diagnostics.FailAssert("This should never logically happen!", Array.Empty<object>());
				}
			}
			return baseWeight;
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x00046800 File Offset: 0x00044A00
		private void ChangeTileWeightsAroundBuilding(Vector2Int buildingOrigin, int radius, Fix64 blurFactor, Vector2Int tileFootprint, string context)
		{
			for (int radiusOut = 1; radiusOut < radius; radiusOut++)
			{
				Fix64 exponent = (Fix64)((long)(radius - radiusOut));
				Fix64 blurAmount = Fix64.Pow(blurFactor, exponent);
				Vector2Int topRight = Vector2Int.up * radiusOut + Vector2Int.right * radiusOut + buildingOrigin + (tileFootprint - Vector2Int.one);
				Vector2Int bottomLeft = Vector2Int.down * radiusOut + Vector2Int.left * radiusOut + buildingOrigin;
				Vector2Int topLeft = new Vector2Int(bottomLeft.x, topRight.y);
				Vector2Int bottomRight = new Vector2Int(topRight.x, bottomLeft.y);
				int horizontalDiff = topRight.x - bottomLeft.x;
				for (int distance = 0; distance < horizontalDiff; distance++)
				{
					Vector2Int bottom = bottomRight + Vector2Int.left * distance;
					Vector2Int top = topLeft + Vector2Int.right * distance;
					if (this._placeableArea.Contains(bottom))
					{
						bottom -= this._placeableArea.min;
						int index = bottom.x + bottom.y * this._placeableArea.width;
						if (this._placeableTileWeights[index] >= Fix64.Zero)
						{
							this._placeableTileWeights[index] = this._placeableTileWeights[index] * blurAmount;
						}
					}
					if (this._placeableArea.Contains(top))
					{
						top -= this._placeableArea.min;
						int index2 = top.x + top.y * this._placeableArea.width;
						if (this._placeableTileWeights[index2] >= Fix64.Zero)
						{
							this._placeableTileWeights[index2] = this._placeableTileWeights[index2] * blurAmount;
						}
					}
				}
				int verticalDiff = topRight.y - bottomLeft.y;
				for (int distance2 = 0; distance2 < verticalDiff; distance2++)
				{
					Vector2Int left = bottomLeft + Vector2Int.up * distance2;
					Vector2Int right = topRight + Vector2Int.down * distance2;
					if (this._placeableArea.Contains(right))
					{
						right -= this._placeableArea.min;
						int index3 = right.x + right.y * this._placeableArea.width;
						if (this._placeableTileWeights[index3] >= Fix64.Zero)
						{
							this._placeableTileWeights[index3] = this._placeableTileWeights[index3] * blurAmount;
						}
					}
					if (this._placeableArea.Contains(left))
					{
						left -= this._placeableArea.min;
						int index4 = left.x + left.y * this._placeableArea.width;
						if (this._placeableTileWeights[index4] >= Fix64.Zero)
						{
							this._placeableTileWeights[index4] = this._placeableTileWeights[index4] * blurAmount;
						}
					}
				}
			}
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x00046B38 File Offset: 0x00044D38
		public bool GenerateFixedPlacement(BuildingPlacer.Layout layout, Vector2Int fixedCoordinates)
		{
			Fix64 fix;
			if ((this._city.Rules.DoesIgnorePlayableArea() || this.TryCalculateAverageWeightOverTiles(layout, fixedCoordinates - this._placeableArea.min, out fix)) && this.TryGeneratePlacementForLayoutAtCoordinates(layout, fixedCoordinates, Fix64.One) && (this._city.Rules.DoesIgnorePlayableArea() || this.PlacementDrivewaysAreFree(this._possiblePlacements[0])))
			{
				return true;
			}
			if (!Diagnostics.Verify(!this._city.Rules.DoesIgnorePlayableArea(), "We couldn't place a fixed building in a city that ignores the playable area (i.e. menu city). Try repositioning it."))
			{
				return false;
			}
			this.GeneratePlacements(new List<BuildingPlacer.Layout>
			{
				layout
			});
			this._possiblePlacements.Sort((BuildingPlacer.Placement a, BuildingPlacer.Placement b) => Vector2Int.Distance(a.coordinates, fixedCoordinates).CompareTo(Vector2Int.Distance(b.coordinates, fixedCoordinates)));
			return this._possiblePlacements.Count > 0;
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x00046C20 File Offset: 0x00044E20
		public Vector2Int GetLocalPosition(Vector2Int fixedCoordinates)
		{
			return fixedCoordinates - this._placeableArea.min;
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x00046C34 File Offset: 0x00044E34
		public bool GeneratePlacements(List<BuildingPlacer.Layout> possibleLayouts)
		{
			bool didGeneratePlacement = false;
			this._city.Rules.DoesIgnorePlayableArea();
			int minLayoutWidth = possibleLayouts[0].footprint.x;
			int minLayoutHeight = possibleLayouts[0].footprint.y;
			foreach (BuildingPlacer.Layout layout in possibleLayouts)
			{
				minLayoutWidth = Mathf.Min(minLayoutWidth, layout.footprint.x);
				minLayoutHeight = Mathf.Min(minLayoutHeight, layout.footprint.y);
			}
			int maxPlaceableHeight = this._placeableArea.height - Mathf.Max(0, minLayoutHeight - 1);
			int maxPlaceableWidth = this._placeableArea.width - Mathf.Max(0, minLayoutWidth - 1);
			for (int y = 0; y < maxPlaceableHeight; y++)
			{
				for (int x = 0; x < maxPlaceableWidth; x++)
				{
					if (!(this._placeableTileWeights[y * this._placeableArea.width + x] < Fix64.Zero))
					{
						Vector2Int lastLayoutFootprint = Vector2Int.zero;
						bool wasLastLayoutValid = false;
						Fix64 placementWeight = Fix64.One;
						foreach (BuildingPlacer.Layout possibleLayout in possibleLayouts)
						{
							bool isPositionValid = true;
							if (lastLayoutFootprint == possibleLayout.footprint)
							{
								isPositionValid = wasLastLayoutValid;
							}
							else
							{
								isPositionValid &= this.TryCalculateAverageWeightOverTiles(possibleLayout, new Vector2Int(x, y), out placementWeight);
								lastLayoutFootprint = possibleLayout.footprint;
								wasLastLayoutValid = isPositionValid;
							}
							if (isPositionValid && this.TryGeneratePlacementForLayoutAtCoordinates(possibleLayout, new Vector2Int(this._placeableArea.xMin + x, this._placeableArea.yMin + y), placementWeight))
							{
								didGeneratePlacement = true;
							}
						}
					}
				}
			}
			this._possiblePlacements.Sort((BuildingPlacer.Placement a, BuildingPlacer.Placement b) => b.weight.CompareTo(a.weight));
			return didGeneratePlacement;
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x00046E48 File Offset: 0x00045048
		private bool TryCalculateAverageWeightOverTiles(BuildingPlacer.Layout possibleLayout, Vector2Int localPosition, out Fix64 weightFromTiles)
		{
			weightFromTiles = Fix64.Zero;
			bool isPositionValid = true;
			int numMatchedPlatforms = 0;
			int buildingX = 0;
			while (buildingX < possibleLayout.footprint.x && isPositionValid)
			{
				for (int buildingY = 0; buildingY < possibleLayout.footprint.y; buildingY++)
				{
					int tileX = localPosition.x + buildingX;
					int tileY = localPosition.y + buildingY;
					if (tileX < 0 || tileX >= this._placeableArea.width)
					{
						isPositionValid = false;
						break;
					}
					if (tileY < 0 || tileY >= this._placeableArea.height)
					{
						isPositionValid = false;
						break;
					}
					int tileIndex = tileY * this._placeableArea.width + tileX;
					Fix64 tileWeight = this._placeableTileWeights[tileIndex];
					if (tileWeight < Fix64.Zero)
					{
						isPositionValid = false;
						break;
					}
					bool currentDrivewayIsOnRail = false;
					Vector2Int zero = Vector2Int.zero;
					foreach (BuildingPlacer.Driveway driveway in possibleLayout.driveways)
					{
						Vector2Int drivewayEndCoordinates = TileUtilities.GetAdjacentCoordinates(new Vector2Int(localPosition.x + driveway.coordinatesOffset.x, localPosition.y + driveway.coordinatesOffset.y), driveway.direction);
						Vector2Int currentCoordinates = new Vector2Int(tileX, tileY);
						if (drivewayEndCoordinates == currentCoordinates && this._placeableTileRails[tileIndex])
						{
							currentDrivewayIsOnRail = true;
							break;
						}
					}
					if (currentDrivewayIsOnRail)
					{
						isPositionValid = false;
						break;
					}
					if (possibleLayout.platforms.Count == 0)
					{
						if (tileIndex < this._placeableTileRails.Count && tileIndex >= 0 && this._placeableTileRails[tileIndex])
						{
							isPositionValid = false;
							break;
						}
					}
					else if (this._placeableTileRails[tileIndex])
					{
						bool platformIsOnCurrentRailPosition = false;
						foreach (BuildingPlacer.RailPlatform platform in possibleLayout.platforms)
						{
							Vector2Int lhs = new Vector2Int(localPosition.x + platform.coordinatesOffset.x, localPosition.y + platform.coordinatesOffset.y);
							Vector2Int currentCoordinates2 = new Vector2Int(tileX, tileY);
							if (lhs == currentCoordinates2)
							{
								platformIsOnCurrentRailPosition = true;
								break;
							}
						}
						if (!platformIsOnCurrentRailPosition)
						{
							isPositionValid = false;
							break;
						}
						numMatchedPlatforms++;
					}
					weightFromTiles += tileWeight;
				}
				buildingX++;
			}
			if (numMatchedPlatforms != possibleLayout.platforms.Count)
			{
				isPositionValid = false;
			}
			weightFromTiles /= (Fix64)((long)(possibleLayout.footprint.x * possibleLayout.footprint.y));
			return isPositionValid;
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x00047114 File Offset: 0x00045314
		private void ForcePositionInvalidDueToDriveway(Vector2Int position)
		{
			if (this._placeableArea.size.magnitude > 0f)
			{
				Vector2Int relativePosition = position - this._placeableArea.min;
				int index = relativePosition.x + relativePosition.y * this._placeableArea.width;
				this._placeableTileDriveabilities[index] = false;
			}
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x00047178 File Offset: 0x00045378
		private bool TryGeneratePlacementForLayoutAtCoordinates(BuildingPlacer.Layout layout, Vector2Int coordinates, Fix64 weight)
		{
			bool didGeneratePlacement = false;
			bool canLayoutBePlaced = true;
			if (this._placeableArea.width > 0 && this._placeableArea.height > 0)
			{
				Vector2Int placeableCoordinates = coordinates - this._placeableArea.min;
				foreach (BuildingPlacer.Driveway driveway in layout.driveways)
				{
					Vector2Int drivewayStart = placeableCoordinates + driveway.coordinatesOffset;
					Vector2Int drivewayEnd = TileUtilities.GetAdjacentCoordinates(drivewayStart, driveway.direction);
					if (drivewayEnd.x < 0 || drivewayEnd.x >= this._placeableArea.width || drivewayEnd.y < 0 || drivewayEnd.y >= this._placeableArea.height)
					{
						canLayoutBePlaced = false;
						break;
					}
					int tileIndexEnd = drivewayEnd.y * this._placeableArea.width + drivewayEnd.x;
					int tileIndexStart = drivewayStart.y * this._placeableArea.width + drivewayStart.x;
					Tile drivewayEndTile = this._placeableTiles[tileIndexEnd];
					Tile tile = this._placeableTiles[tileIndexEnd];
					if (!this._placeableTileDriveabilities[tileIndexEnd])
					{
						canLayoutBePlaced = false;
						break;
					}
					if (this._placeableTileRails[tileIndexEnd] || this._placeableTileRails[tileIndexStart])
					{
						canLayoutBePlaced = false;
						break;
					}
					RoadTileNode drivewayNode = new RoadTileNode(TileUtilities.GetOppositeDirection(driveway.direction), RoadType.Driveway, -1);
					if (this._city.Rules.CanBuildingsDemolishUnusedRoads && drivewayEndTile != null)
					{
						drivewayEndTile = this.CreateDemolishedTestTileFrom(drivewayEndTile);
					}
					if (drivewayEndTile != null && (!drivewayEndTile.CanSetNodeState(drivewayNode, RoadState.Pending, Tile.TileChangePermissions.Full) || drivewayEndTile.IsNodeBlocked(drivewayNode)))
					{
						canLayoutBePlaced = false;
						break;
					}
				}
			}
			if (canLayoutBePlaced)
			{
				if (this._usedPlacementCount >= this._placementPool.Count)
				{
					for (int newPlacementCount = 0; newPlacementCount < 20; newPlacementCount++)
					{
						this._placementPool.Add(new BuildingPlacer.Placement());
					}
				}
				BuildingPlacer.Placement placement = this._placementPool[this._usedPlacementCount];
				this._usedPlacementCount++;
				placement.coordinates = coordinates;
				placement.layout = layout;
				placement.layout.boatTerminalTiles = layout.boatTerminalTiles;
				placement.weight = this._cityModel.pseudorandomGenerator.Fix64(weight);
				this._possiblePlacements.Add(placement);
				didGeneratePlacement = true;
			}
			return didGeneratePlacement;
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x000473FC File Offset: 0x000455FC
		private Fix64 ScaleTileWeightByBuildingInfluence(Fix64 baseWeight, int groupIndex, Vector2Int tileCoordinates, TileContentType buildingType, int contextIndex)
		{
			int distanceToNearestDemand = this._cityPlanModel.GetDistanceToNearestDemand(tileCoordinates);
			int distanceToNearestDemandOfSameGroup = this._cityPlanModel.GetDistanceToNearestDemandOfGroup(tileCoordinates, groupIndex);
			int distanceToNearestSupplyOfSameGroup = this._cityPlanModel.GetDistanceToNearestSupplyOfGroup(tileCoordinates, groupIndex);
			int distanceToNearestSupplyOfOtherGroup = this._cityPlanModel.GetDistanceToNearestSupplyNotOfGroup(tileCoordinates, groupIndex);
			if (distanceToNearestDemand <= 1 || distanceToNearestSupplyOfSameGroup == 0 || distanceToNearestSupplyOfOtherGroup == 0)
			{
				baseWeight = Fix64.Zero;
			}
			else if (buildingType == TileContentType.Destination || buildingType == TileContentType.Carpark)
			{
				if (distanceToNearestDemandOfSameGroup > 0 && distanceToNearestDemandOfSameGroup < 12)
				{
					Fix64 exponent = (Fix64)((long)(12 - distanceToNearestDemandOfSameGroup));
					if (this._grouping == GroupingStyle.Far)
					{
						Fix64 blurAmount = Fix64.Pow(BuildingPlacer.SpawnPushFactorStrong, exponent);
						baseWeight *= blurAmount;
					}
					else if (this._grouping == GroupingStyle.Near)
					{
						Fix64 blurAmount2 = Fix64.Pow(BuildingPlacer.SpawnPullFactor, exponent);
						baseWeight *= blurAmount2;
					}
				}
				if (distanceToNearestSupplyOfSameGroup > 0 && distanceToNearestSupplyOfSameGroup < 7 && distanceToNearestSupplyOfSameGroup > 7)
				{
					Fix64 exponent2 = (Fix64)((long)(distanceToNearestSupplyOfSameGroup - 7));
					Fix64 blurAmount3 = Fix64.Pow(BuildingPlacer.SpawnPushFactorVeryStrong, exponent2);
					baseWeight *= blurAmount3;
				}
			}
			else if (buildingType == TileContentType.House)
			{
				if (distanceToNearestSupplyOfSameGroup > 0 && distanceToNearestSupplyOfSameGroup < 7)
				{
					Fix64 exponent3 = (Fix64)((long)(7 - distanceToNearestSupplyOfSameGroup));
					if (this._grouping == GroupingStyle.Far)
					{
						Fix64 blurAmount4 = Fix64.Pow(BuildingPlacer.SpawnPushFactorStrong, exponent3);
						baseWeight *= blurAmount4;
					}
					else if (this._grouping == GroupingStyle.Near)
					{
						Fix64 pullFactor = BuildingPlacer.SpawnPullFactor;
						int numberOfNeighbouringHousesInSameGroup = this._cityPlanModel.GetNearbyHouseCountOfGroup(tileCoordinates, groupIndex);
						if (numberOfNeighbouringHousesInSameGroup > 0)
						{
							pullFactor -= (Fix64)((long)numberOfNeighbouringHousesInSameGroup) * BuildingPlacer.PullFactorPerNeighbourDecrease;
							pullFactor = Fix64.Max(pullFactor, BuildingPlacer.PullFactorMinimum);
						}
						Fix64 blurAmount5 = Fix64.Pow(pullFactor, exponent3);
						baseWeight *= blurAmount5;
					}
				}
				if (distanceToNearestDemandOfSameGroup > 0 && distanceToNearestDemandOfSameGroup < 7)
				{
					Fix64 exponent4 = (Fix64)((long)(7 - distanceToNearestDemandOfSameGroup));
					Fix64 blurAmount6 = Fix64.Pow(BuildingPlacer.SpawnPushFactorVeryStrong, exponent4);
					baseWeight *= blurAmount6;
				}
				if (distanceToNearestSupplyOfOtherGroup > 0 && distanceToNearestSupplyOfOtherGroup < 7)
				{
					Fix64 exponent5 = (Fix64)((float)(7 - distanceToNearestSupplyOfOtherGroup) * 0.5f);
					Fix64 blurAmount7 = Fix64.Pow(BuildingPlacer.SpawnPushFactorWeak, exponent5);
					baseWeight *= blurAmount7;
				}
			}
			return baseWeight;
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x000475F8 File Offset: 0x000457F8
		public BuildingPlacer.Placement ChoosePlacement()
		{
			if (this._possiblePlacements.Count > 0)
			{
				while (this._possiblePlacements.Count > 0)
				{
					BuildingPlacer.Placement placement = this._possiblePlacements[0];
					if (this._city.Rules.DoesIgnorePlayableArea() || this.PlacementDrivewaysAreFree(placement))
					{
						this.OnPlacementFound(0);
						return placement;
					}
					this._possiblePlacements.RemoveAt(0);
				}
			}
			this.OnFailedPlacement();
			return null;
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x00047668 File Offset: 0x00045868
		private bool PlacementDrivewaysAreFree(BuildingPlacer.Placement placement)
		{
			List<Vector2Int> buildingWorldCoordinates = new List<Vector2Int>();
			foreach (BuildingPlacer.Driveway driveway in placement.layout.driveways)
			{
				Vector2Int drivewayPosition = placement.coordinates + driveway.coordinatesOffset + TileUtilities.GetAdjacencyOffsetForDirection(driveway.direction);
				buildingWorldCoordinates.Clear();
				for (int x = 0; x < placement.layout.footprint.x; x++)
				{
					for (int y = 0; y < placement.layout.footprint.y; y++)
					{
						buildingWorldCoordinates.Add(placement.coordinates + new Vector2Int(x, y));
					}
				}
				if (!this._city.Rules.AllowBlockingSpawns && !this.BuildingCanFindPathAwayFromDriveway(drivewayPosition, driveway.direction, buildingWorldCoordinates))
				{
					return false;
				}
				Vector2Int placeableCoordinates = drivewayPosition - this._placeableArea.min;
				int tileIndex = placeableCoordinates.y * this._placeableArea.width + placeableCoordinates.x;
				if (this._placeableTileRails[tileIndex])
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x000477C0 File Offset: 0x000459C0
		private bool BuildingCanFindPathAwayFromDriveway(Vector2Int drivewayPosition, TileDirection drivewayDirection, ICollection<Vector2Int> footprintTiles)
		{
			int directionCount = 0;
			foreach (TileDirection direction in TileUtilities.GetRadiatedDirections(drivewayDirection, true))
			{
				directionCount++;
				Vector2Int vectorDirection = TileUtilities.DirectionToTileAdjacencyOffset[(int)direction];
				Vector2Int pathTestPoint = vectorDirection * 5 + drivewayPosition;
				for (int additionalProjectionIndex = 0; additionalProjectionIndex < 5; additionalProjectionIndex++)
				{
					Tile tile = this._tilemapModel.GetTile(pathTestPoint);
					if (tile == null || tile.CanDrawRoadsOn())
					{
						break;
					}
					pathTestPoint += vectorDirection;
				}
				Tile resultingTile = this._tilemapModel.GetTile(pathTestPoint);
				if (resultingTile == null || resultingTile.CanDrawRoadsOn())
				{
					if (this._pathfinder.GetPathBetweenPoints(drivewayPosition, pathTestPoint, this._simulation, this._city, footprintTiles) != null)
					{
						return true;
					}
					if (directionCount >= 5)
					{
						break;
					}
				}
			}
			return false;
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x000478AC File Offset: 0x00045AAC
		private Tile CreateDemolishedTestTileFrom(Tile tile)
		{
			if (this._testTile == null)
			{
				this._testTile = this._scope.Get<Tile>();
			}
			this._testTile.Initialize(this._tilemapModel, tile.Coordinates, tile.ContentType);
			tile.CloneInto(this._testTile);
			TileModel tileModel = this._tilemapModel.GetTileModel(tile.Coordinates);
			if (!Diagnostics.Verify(tileModel != null) || Passage.DoesTileHavePassage(this._city.Definition, this._tilemapModel, tile.Coordinates, RoadState.ActiveOrPending))
			{
				return this._testTile;
			}
			foreach (TileDirection twoLaneRoadDirection in tileModel.Tile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore))
			{
				if (tileModel.AreAllLanesInDirectionUnused(twoLaneRoadDirection, RoadState.Active) && tileModel.GetAdjacentTileModelInDirection(twoLaneRoadDirection).Tile.ContentType != TileContentType.House)
				{
					this._testTile.SetNodeState(new RoadTileNode(twoLaneRoadDirection, RoadType.TwoLane, -1), RoadState.Mothballed, Tile.TileChangePermissions.Full);
					this._testTile.SetNodeState(new RoadTileNode(twoLaneRoadDirection, RoadType.TwoLane, -1), RoadState.None, Tile.TileChangePermissions.Full);
				}
			}
			return this._testTile;
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x000479BC File Offset: 0x00045BBC
		private bool IsTileConnectedToBuildingAndHouse(TileModel tile)
		{
			if (tile.Tile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) == 0)
			{
				return false;
			}
			LaneModel laneToTest = tile.roadChunk.lanes[0];
			return this._lanePathfinder.AreLanesConnected(laneToTest, this._cityPlanModel.destinationLanes, true) && this._lanePathfinder.AreLanesConnected(laneToTest, this.GetHouseLanes(), true);
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00047A1C File Offset: 0x00045C1C
		private IEnumerable<LaneModel> GetHouseLanes()
		{
			if (this._cachedHouseLanes.Count == 0)
			{
				foreach (HouseModel house in this._simulation.GetModels<HouseModel>())
				{
					if (house.tileModel.Tile.GetTwoLaneRoadCount(RoadState.Active, Tile.MotorwayInclusion.Ignore) > 0)
					{
						this._cachedHouseLanes.Add(house.DrivewayLane);
					}
				}
			}
			return this._cachedHouseLanes;
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x000022F5 File Offset: 0x000004F5
		private void OnPlacementFound(int placementIndex)
		{
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x000022F5 File Offset: 0x000004F5
		private void OnFailedPlacement()
		{
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00047A8A File Offset: 0x00045C8A
		[Conditional("UNITY_EDITOR")]
		private void AddTileContext(Fix64 weight, string context)
		{
			this._placeableTileWeightsContext.Add(string.Format("{0:F3} {1}\n", (float)weight, context));
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00047AB0 File Offset: 0x00045CB0
		[Conditional("UNITY_EDITOR")]
		private void AddToTileContext(int index, Fix64 newWeight, string context)
		{
			List<string> placeableTileWeightsContext = this._placeableTileWeightsContext;
			placeableTileWeightsContext[index] += string.Format("{0:F3} {1}\n", (float)newWeight, context);
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00047AF0 File Offset: 0x00045CF0
		private string GetTileContext(int index)
		{
			return string.Empty;
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x00047AF7 File Offset: 0x00045CF7
		public void OnReleasedFromScope(IScope scope)
		{
			if (this._testTile != null)
			{
				this._scope.Release(this._testTile);
				this._testTile = null;
			}
		}

		// Token: 0x04001176 RID: 4470
		[Dependency]
		private ClockModel _clock;

		// Token: 0x04001177 RID: 4471
		[Dependency]
		private City _city;

		// Token: 0x04001178 RID: 4472
		[Dependency]
		private CityModel _cityModel;

		// Token: 0x04001179 RID: 4473
		[Dependency]
		private CityPlanModel _cityPlanModel;

		// Token: 0x0400117A RID: 4474
		[Dependency]
		private TilemapModel _tilemapModel;

		// Token: 0x0400117B RID: 4475
		[Dependency]
		private TilePathfinder _pathfinder;

		// Token: 0x0400117C RID: 4476
		[Dependency]
		private Pathfinder _lanePathfinder;

		// Token: 0x0400117D RID: 4477
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x0400117E RID: 4478
		[Dependency]
		private IScope _scope;

		// Token: 0x0400117F RID: 4479
		[Dependency]
		private CitySpawningView _spawningView;

		// Token: 0x04001180 RID: 4480
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x04001181 RID: 4481
		[Dependency]
		private SimulationConstantsData _constants;

		// Token: 0x04001182 RID: 4482
		private RectInt _placeableArea;

		// Token: 0x04001183 RID: 4483
		private readonly List<Tile> _placeableTiles = new List<Tile>();

		// Token: 0x04001184 RID: 4484
		private readonly List<Fix64> _placeableTileWeights = new List<Fix64>();

		// Token: 0x04001185 RID: 4485
		private readonly List<string> _placeableTileWeightsContext = new List<string>();

		// Token: 0x04001186 RID: 4486
		private readonly List<bool> _placeableTileRails = new List<bool>();

		// Token: 0x04001187 RID: 4487
		private readonly HashSet<LaneModel> _cachedHouseLanes = new HashSet<LaneModel>();

		// Token: 0x04001188 RID: 4488
		private readonly List<bool> _placeableTileDriveabilities = new List<bool>();

		// Token: 0x04001189 RID: 4489
		private TileContentType _buildingType;

		// Token: 0x0400118A RID: 4490
		private int _groupIndex;

		// Token: 0x0400118B RID: 4491
		private GroupingStyle _grouping;

		// Token: 0x0400118C RID: 4492
		private Tile _testTile;

		// Token: 0x0400118D RID: 4493
		private readonly List<BuildingPlacer.Placement> _possiblePlacements = new List<BuildingPlacer.Placement>();

		// Token: 0x0400118E RID: 4494
		[Unscrubbed]
		private readonly List<BuildingPlacer.Placement> _placementPool = new List<BuildingPlacer.Placement>(100);

		// Token: 0x0400118F RID: 4495
		private const int PlacementsToAllocate = 100;

		// Token: 0x04001190 RID: 4496
		private int _usedPlacementCount;

		// Token: 0x04001191 RID: 4497
		private CitySpawningLayerData _baseTileData;

		// Token: 0x04001192 RID: 4498
		public const int GroupingStyleRadius = 7;

		// Token: 0x04001193 RID: 4499
		public const int DemandGroupingStyleRadius = 12;

		// Token: 0x04001194 RID: 4500
		public const int DestinationDeadzoneRadius = 5;

		// Token: 0x04001195 RID: 4501
		public static readonly Fix64 SpawnPushFactorVeryStrong = (Fix64)0.425;

		// Token: 0x04001196 RID: 4502
		public static readonly Fix64 SpawnPushFactorStrong = (Fix64)0.6;

		// Token: 0x04001197 RID: 4503
		public static readonly Fix64 SpawnPushFactorWeak = (Fix64)0.9;

		// Token: 0x04001198 RID: 4504
		public static readonly Fix64 SpawnPullFactor = (Fix64)1.3;

		// Token: 0x04001199 RID: 4505
		public const int SuburbRadius = 5;

		// Token: 0x0400119A RID: 4506
		public static readonly Fix64 PullFactorPerNeighbourDecrease = (Fix64)0.035;

		// Token: 0x0400119B RID: 4507
		public static readonly Fix64 PullFactorMinimum = (Fix64)1.05;

		// Token: 0x0400119C RID: 4508
		private const int MinimumProjectedDistanceToPathTo = 5;

		// Token: 0x02000358 RID: 856
		public class Driveway
		{
			// Token: 0x06001512 RID: 5394 RVA: 0x00047B9B File Offset: 0x00045D9B
			public override string ToString()
			{
				return string.Format("Driveway [{0}, {1}]", this.coordinatesOffset, this.direction);
			}

			// Token: 0x0400119D RID: 4509
			public Vector2Int coordinatesOffset;

			// Token: 0x0400119E RID: 4510
			public TileDirection direction;
		}

		// Token: 0x02000359 RID: 857
		public class RailPlatform
		{
			// Token: 0x06001514 RID: 5396 RVA: 0x00047BBD File Offset: 0x00045DBD
			public override string ToString()
			{
				return string.Format("RailPlatform [{0}, {1}]", this.coordinatesOffset, this.connection);
			}

			// Token: 0x0400119F RID: 4511
			public Vector2Int coordinatesOffset;

			// Token: 0x040011A0 RID: 4512
			public TileDirectionBitfield connection;
		}

		// Token: 0x0200035A RID: 858
		public class Layout
		{
			// Token: 0x06001516 RID: 5398 RVA: 0x00047BE0 File Offset: 0x00045DE0
			public override string ToString()
			{
				string drivewayString = "";
				foreach (BuildingPlacer.Driveway driveway in this.driveways)
				{
					drivewayString += string.Format("{0}", driveway);
				}
				string platformString = "";
				foreach (BuildingPlacer.RailPlatform platform in this.platforms)
				{
					platformString += string.Format("{0}", platform);
				}
				return string.Format("Layout [{0}, {1}, {2}, {3}]", new object[]
				{
					this.footprint,
					drivewayString,
					platformString,
					this.carparkSide
				});
			}

			// Token: 0x040011A1 RID: 4513
			public Vector2Int footprint;

			// Token: 0x040011A2 RID: 4514
			public List<BuildingPlacer.Driveway> driveways = new List<BuildingPlacer.Driveway>();

			// Token: 0x040011A3 RID: 4515
			public List<BuildingPlacer.RailPlatform> platforms = new List<BuildingPlacer.RailPlatform>();

			// Token: 0x040011A4 RID: 4516
			public List<Vector2Int> boatTerminalTiles = new List<Vector2Int>();

			// Token: 0x040011A5 RID: 4517
			public TileDirection carparkSide = TileDirection.None;
		}

		// Token: 0x0200035B RID: 859
		public class Placement
		{
			// Token: 0x06001518 RID: 5400 RVA: 0x00047D00 File Offset: 0x00045F00
			public override string ToString()
			{
				return string.Format("Placement [{0}, {1}, {2}]", this.coordinates, this.layout, this.weight);
			}

			// Token: 0x040011A6 RID: 4518
			public Vector2Int coordinates;

			// Token: 0x040011A7 RID: 4519
			public BuildingPlacer.Layout layout;

			// Token: 0x040011A8 RID: 4520
			public Fix64 weight;
		}

		// Token: 0x0200035C RID: 860
		public enum WeightSource
		{
			// Token: 0x040011AA RID: 4522
			Default,
			// Token: 0x040011AB RID: 4523
			Station,
			// Token: 0x040011AC RID: 4524
			BoatTerminal
		}

		// Token: 0x0200035D RID: 861
		public enum WeightEvaluationLevel
		{
			// Token: 0x040011AE RID: 4526
			ExclusivelyUseWeightedTiles,
			// Token: 0x040011AF RID: 4527
			AllowNonWeightedTiles,
			// Token: 0x040011B0 RID: 4528
			IgnoreWeights
		}
	}
}
