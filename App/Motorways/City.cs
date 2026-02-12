using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Server;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000360 RID: 864
	public class City : IReleasedFromScopeHandler, IReusable
	{
		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x0600151F RID: 5407 RVA: 0x00047D7F File Offset: 0x00045F7F
		// (set) Token: 0x06001520 RID: 5408 RVA: 0x00047D87 File Offset: 0x00045F87
		[Dependency]
		public IScope Scope { get; private set; }

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06001521 RID: 5409 RVA: 0x00047D90 File Offset: 0x00045F90
		public GameMode GameMode
		{
			get
			{
				return this._cityModel.Mode;
			}
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x00047DA0 File Offset: 0x00045FA0
		public bool Initialize(CityDefinition cityDefinition, GameRules rules)
		{
			if (!Diagnostics.Verify(this._definition == null || this._definition == cityDefinition, "Unable to reinitialize City with a new definition."))
			{
				return false;
			}
			this.Definition = cityDefinition;
			cityDefinition.CompileTilemap();
			this.SetGameRules(rules);
			if (this.Rules.ShouldGameStartFullyExpanded && this._clockModel.ExpansionTime < ClockModel.DaysToSeconds(this.Definition.cameraZoom.durationInDays))
			{
				this._clockModel.SetExpansionTimeToDay(this.Definition.cameraZoom.durationInDays);
			}
			this._nextMotorwayId = 1;
			if (this._simulation != null)
			{
				foreach (MotorwayModel motorwayModel in this._simulation.GetModels<MotorwayModel>())
				{
					this._nextMotorwayId = Mathf.Max(this._nextMotorwayId, motorwayModel.Id + 1);
				}
				foreach (TileModel tileModel in this._simulation.GetModels<TileModel>())
				{
					int unbuiltMotorwayId = tileModel.Tile.UnbuiltMotorwayId;
					if (unbuiltMotorwayId != -1)
					{
						this._nextMotorwayId = Mathf.Max(this._nextMotorwayId, unbuiltMotorwayId + 1);
					}
				}
			}
			return true;
		}

		// Token: 0x06001523 RID: 5411 RVA: 0x00047EDB File Offset: 0x000460DB
		public void SetGameRules(GameRules newRules)
		{
			if (this.Rules != null && this.Rules != newRules)
			{
				this.Scope.Release(this.Rules);
				this.Rules = null;
			}
			this.Rules = newRules;
		}

		// Token: 0x06001524 RID: 5412 RVA: 0x00047F0E File Offset: 0x0004610E
		public void Reset()
		{
			this.Rules = null;
			this._definition = null;
			this._nextMotorwayId = 1;
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x00047F25 File Offset: 0x00046125
		public void OnReleasedFromScope(IScope scope)
		{
			if (this.Rules != null)
			{
				scope.Release(this.Rules);
				this.Rules = null;
			}
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06001526 RID: 5414 RVA: 0x00047F43 File Offset: 0x00046143
		// (set) Token: 0x06001527 RID: 5415 RVA: 0x00047F4B File Offset: 0x0004614B
		public CityDefinition Definition
		{
			get
			{
				return this._definition;
			}
			set
			{
				this._definition = value;
				this._definition.CompileTilemap();
			}
		}

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06001528 RID: 5416 RVA: 0x00047F5F File Offset: 0x0004615F
		// (set) Token: 0x06001529 RID: 5417 RVA: 0x00047F67 File Offset: 0x00046167
		public GameRules Rules { get; private set; }

		// Token: 0x0600152A RID: 5418 RVA: 0x00047F70 File Offset: 0x00046170
		public int GetNextMotorwayIdAndIncrement()
		{
			this._nextMotorwayId++;
			return this._nextMotorwayId - 1;
		}

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x0600152B RID: 5419 RVA: 0x00047F88 File Offset: 0x00046188
		public int NextMotorwayId
		{
			get
			{
				return this._nextMotorwayId;
			}
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x00047F90 File Offset: 0x00046190
		public Fix64 GetCameraSizeAtTime(Fix64 time)
		{
			if (this.Definition == null)
			{
				return (Fix64)10L;
			}
			AnimationCurve zoomCurve = this.Definition.cameraZoom.velocity;
			if (zoomCurve == null)
			{
				return (Fix64)10L;
			}
			Fix64 days = time / ((Fix64)0.8333333333333334 * (Fix64)24L);
			if (days <= this.Definition.cameraZoom.delayInDays)
			{
				return this.Definition.cameraZoom.startSize;
			}
			if (days >= this.Definition.cameraZoom.delayInDays + this.Definition.cameraZoom.durationInDays)
			{
				return this.Definition.cameraZoom.endSize;
			}
			Fix64 zoom = (Fix64)zoomCurve.Evaluate((float)days);
			if (Diagnostics.Verify(zoom > Fix64.Zero, "Camera curve is either missing or contains non-positive values."))
			{
				return zoom;
			}
			Fix64 t = (days - this.Definition.cameraZoom.delayInDays) / this.Definition.cameraZoom.durationInDays;
			return this.Definition.cameraZoom.startSize * (Fix64.One - t) + this.Definition.cameraZoom.endSize * t;
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x000480F0 File Offset: 0x000462F0
		public RectFixed GetSimulationPlayableAreaAtZoom(Fix64 zoom, City.PlayableAreaRoundingType roundingType = City.PlayableAreaRoundingType.AllowPartialTiles)
		{
			Fix64 rawWidth = City.PlayableRatio * zoom;
			Vector3Fixed rawCenter = this.GetPlayableAreaPositionAtZoom(zoom) / TilemapModel.TileWidth;
			if (roundingType == City.PlayableAreaRoundingType.AllowPartialTiles)
			{
				return new RectFixed
				{
					x = rawCenter.x - rawWidth * Fix64Consts.OneHalf,
					y = rawCenter.y - zoom * Fix64Consts.OneHalf,
					width = rawWidth,
					height = zoom
				};
			}
			Fix64 minX = Fix64.Ceiling(rawCenter.x - rawWidth * Fix64Consts.OneHalf);
			Fix64 minY = Fix64.Ceiling(rawCenter.y - zoom * Fix64Consts.OneHalf);
			Fix64 maxX = Fix64.Floor(rawCenter.x + rawWidth * Fix64Consts.OneHalf);
			Fix64 maxY = Fix64.Floor(rawCenter.y + zoom * Fix64Consts.OneHalf);
			return new RectFixed
			{
				x = minX,
				y = minY,
				width = maxX - minX,
				height = maxY - minY
			};
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x00048224 File Offset: 0x00046424
		public RectFixed GetSimulationPlayableAreaAtTime(Fix64 time, City.PlayableAreaRoundingType roundingType = City.PlayableAreaRoundingType.AllowPartialTiles)
		{
			Fix64 currentZoom = this.GetCameraSizeAtTime(time);
			return this.GetSimulationPlayableAreaAtZoom(currentZoom, roundingType);
		}

		// Token: 0x0600152F RID: 5423 RVA: 0x00048244 File Offset: 0x00046444
		public RectFixed GetClientPlayableAreaAtZoom(Fix64 zoom, City.PlayableAreaRoundingType roundingType = City.PlayableAreaRoundingType.AllowPartialTiles)
		{
			RectFixed playableArea = this.GetSimulationPlayableAreaAtZoom(zoom, roundingType);
			return new RectFixed
			{
				x = playableArea.xMin * TilemapModel.TileWidth,
				y = playableArea.yMin * TilemapModel.TileWidth,
				width = playableArea.width * TilemapModel.TileWidth,
				height = playableArea.height * TilemapModel.TileWidth
			};
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x000482C4 File Offset: 0x000464C4
		public RectFixed GetClientPlayableAreaAtTime(Fix64 time, City.PlayableAreaRoundingType roundingType = City.PlayableAreaRoundingType.AllowPartialTiles)
		{
			Fix64 zoom = this.GetCameraSizeAtTime(time);
			return this.GetClientPlayableAreaAtZoom(zoom, roundingType);
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x000482E4 File Offset: 0x000464E4
		public Vector3Fixed GetPlayableAreaPositionAtZoom(Fix64 zoom)
		{
			if (this.Definition != null)
			{
				Vector3Fixed startOffset = Vector3Fixed.zero;
				if (this._cityModel != null)
				{
					startOffset = this._cityModel.startOffset;
				}
				return Vector3Fixed.Lerp(startOffset, Vector3Fixed.zero, this.GetLinearProgressOfZoom(zoom));
			}
			return Vector3Fixed.zero;
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x00048334 File Offset: 0x00046534
		public Vector3Fixed GetPlayableAreaPositionAtTime(Fix64 time)
		{
			Fix64 zoom = this.GetCameraSizeAtTime(time);
			return this.GetPlayableAreaPositionAtZoom(zoom);
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x00048350 File Offset: 0x00046550
		public Fix64 GetLinearProgressOfZoom(Fix64 zoom)
		{
			return Fix64.InverseLerp(this.Definition.cameraZoom.startSize, this.Definition.cameraZoom.endSize, zoom);
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x00048378 File Offset: 0x00046578
		public bool IsTileInPlayableArea(Vector2Int coordinates, Fix64 time)
		{
			return this.GetSimulationPlayableAreaAtTime(time, City.PlayableAreaRoundingType.ForceWholeTiles).Contains(coordinates);
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x00048398 File Offset: 0x00046598
		public void PopulateTrees(ISimulation simulation)
		{
			foreach (Tuple<Vector2Int, int> treeData in this.Definition.GetTreeData(this._behaviour.UsesBonusTrees))
			{
				simulation.AddModel(this.CreateTree(treeData.Item2, treeData.Item1));
			}
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x00048408 File Offset: 0x00046608
		public TreeModel CreateTree(int prefabIndex, Vector2Int position)
		{
			TreeModel treeModel = this.Scope.Get<TreeModel>();
			treeModel.Initialize(prefabIndex, this.Scope.Get<TilemapModel>().GetOrCreateTileModel(position));
			return treeModel;
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x00048430 File Offset: 0x00046630
		public void SetupTrainNetwork(ISimulation simulation)
		{
			if (this.Definition == null)
			{
				return;
			}
			TrainNetworkDefinition trainNetworkDefinition = this.Definition.GetTrainNetworkDefinition();
			if (trainNetworkDefinition == null)
			{
				return;
			}
			foreach (TrainLineDefinition trainLineDefinition in trainNetworkDefinition.TrainLines)
			{
				if (Diagnostics.Verify(trainLineDefinition.isValid || trainLineDefinition.TileCount <= 2))
				{
					TilemapModel tilemap = this._simulation.GetModel<TilemapModel>();
					TrainLineModel trainLineModel = this.Scope.Get<TrainLineModel>();
					trainLineModel.Initialize(trainLineDefinition.isLoop);
					for (int trackPositionIndex = 0; trackPositionIndex < trainLineDefinition.TileCount; trackPositionIndex++)
					{
						Vector2Int tilePosition = trainLineDefinition.GetRailTileCoordinates(trackPositionIndex);
						TileModel tileModel = tilemap.GetOrCreateTileModel(tilePosition);
						TileDirection inputDirection = TileDirection.None;
						TileDirection outputDirection = TileDirection.None;
						if (trackPositionIndex > 0 || trainLineDefinition.isLoop)
						{
							Vector2Int previousTilePosition = trainLineDefinition.GetRailTileCoordinates((trackPositionIndex == 0) ? (trainLineDefinition.TileCount - 1) : (trackPositionIndex - 1));
							inputDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(tilePosition, previousTilePosition);
						}
						if (trackPositionIndex < trainLineDefinition.TileCount - 1 || trainLineDefinition.isLoop)
						{
							Vector2Int nextTilePosition = trainLineDefinition.GetRailTileCoordinates((trackPositionIndex == trainLineDefinition.TileCount - 1) ? 0 : (trackPositionIndex + 1));
							outputDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(tilePosition, nextTilePosition);
						}
						tileModel.Tile.SetRailConnection(new RailTileConnection(inputDirection, outputDirection));
						trainLineModel.AddTile(tileModel.RailTileModel, trainLineDefinition.GetRailTileType(trackPositionIndex));
					}
					simulation.AddModel(trainLineModel);
				}
			}
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x000485C4 File Offset: 0x000467C4
		public void SetupBoatPathNetwork(ISimulation simulation)
		{
			if (this.Definition == null)
			{
				return;
			}
			BoatNetworkDefinition boatNetworkDefinition = this.Definition.GetBoatPathNetworkDefinition();
			if (boatNetworkDefinition == null)
			{
				return;
			}
			foreach (BoatPathLineDefinition boatLineDefinition in boatNetworkDefinition.BoatLines)
			{
				if (Diagnostics.Verify(boatLineDefinition.isValid || boatLineDefinition.TileCount <= 2))
				{
					TilemapModel tilemap = this._simulation.GetModel<TilemapModel>();
					BoatPathModel boatPathModel = this.Scope.Get<BoatPathModel>();
					boatPathModel.Initialize(boatLineDefinition.isLoop);
					for (int trackPositionIndex = 0; trackPositionIndex < boatLineDefinition.TileCount; trackPositionIndex++)
					{
						Vector2Int tilePosition = boatLineDefinition.GetBoatPathTileCoordinates(trackPositionIndex);
						TileModel tileModel = tilemap.GetOrCreateTileModel(tilePosition);
						TileDirection inputDirection = TileDirection.None;
						TileDirection outputDirection = TileDirection.None;
						if (trackPositionIndex > 0 || boatLineDefinition.isLoop)
						{
							Vector2Int previousTilePosition = boatLineDefinition.GetBoatPathTileCoordinates((trackPositionIndex == 0) ? (boatLineDefinition.TileCount - 1) : (trackPositionIndex - 1));
							inputDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(tilePosition, previousTilePosition);
						}
						if (trackPositionIndex < boatLineDefinition.TileCount - 1 || boatLineDefinition.isLoop)
						{
							Vector2Int nextTilePosition = boatLineDefinition.GetBoatPathTileCoordinates((trackPositionIndex == boatLineDefinition.TileCount - 1) ? 0 : (trackPositionIndex + 1));
							outputDirection = TileUtilities.GetDirectionBetweenAdjacentCoordinates(tilePosition, nextTilePosition);
						}
						tileModel.Tile.SetBoatPathConnection(new BoatPathTileConnection(inputDirection, outputDirection));
						boatPathModel.AddTile(tileModel.BoatPathTileModel, boatLineDefinition.GetBoatPathTileType(trackPositionIndex));
					}
					simulation.AddModel(boatPathModel);
				}
			}
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x00048758 File Offset: 0x00046958
		public virtual void GenerateCityLayout()
		{
			if (this.Definition == null)
			{
				return;
			}
			bool onlyGenerateBoatTerminal = false;
			if (this.Rules.HasDisabledAutomaticSpawn())
			{
				if (this.Definition.GetBoatPathNetworkDefinition() == null)
				{
					return;
				}
				onlyGenerateBoatTerminal = true;
			}
			PseudorandomGenerator pseudorandomGenerator = this._cityModel.pseudorandomGenerator;
			foreach (ScheduleChunk chunk in this.Definition.schedulePlanner.scheduleChunks)
			{
				if (chunk.plannedBuildings.Count > 0)
				{
					Fix64 startTime = (Fix64)0.8333333333333334 * (Fix64)24L * (Fix64)chunk.startDay;
					Fix64 endTime = (Fix64)0.8333333333333334 * (Fix64)24L * (Fix64)(chunk.startDay + chunk.duration);
					Fix64 timeStep = (endTime - startTime) / (Fix64)((long)chunk.plannedBuildings.Count);
					Fix64 nextTime = startTime + timeStep * Fix64Consts.OneHalf;
					Fix64[] randomTimes = new Fix64[chunk.plannedBuildings.Count];
					for (int buildingCount = 0; buildingCount < chunk.plannedBuildings.Count; buildingCount++)
					{
						randomTimes[buildingCount] = startTime + pseudorandomGenerator.Fix64(endTime - startTime);
					}
					Array.Sort<Fix64>(randomTimes);
					List<PlannedBuilding> buildingsInChunk = new List<PlannedBuilding>();
					buildingsInChunk.AddRange(chunk.plannedBuildings);
					int plannedBuildingCount = chunk.plannedBuildings.Count;
					int plannedBuildingNumber = 0;
					while (plannedBuildingNumber < plannedBuildingCount && buildingsInChunk.Count > 0)
					{
						int buildingIndex = plannedBuildingNumber;
						if (!chunk.buildingsAreOrdered)
						{
							buildingIndex = pseudorandomGenerator.Int(buildingsInChunk.Count);
						}
						PlannedBuilding plannedBuilding = buildingsInChunk[buildingIndex];
						if (!this.Rules.HasDisabledAutomaticSpawn())
						{
							goto IL_1CB;
						}
						if (onlyGenerateBoatTerminal && plannedBuilding.carparkPreference == CarparkPreference.BoatTerminal)
						{
							onlyGenerateBoatTerminal = false;
							goto IL_1CB;
						}
						IL_334:
						plannedBuildingNumber++;
						continue;
						IL_1CB:
						if (!chunk.buildingsAreOrdered)
						{
							buildingsInChunk.RemoveAt(buildingIndex);
						}
						Fix64 scheduleTimingRandomness = this.Rules.HasSpawnScheduleVariation() ? chunk.spawnVariability : Fix64Consts.Zero;
						Fix64 spawnTime = nextTime * (Fix64Consts.One - scheduleTimingRandomness) + randomTimes[plannedBuildingNumber] * scheduleTimingRandomness;
						nextTime += timeStep;
						if (plannedBuilding.type != CityTileType.Supply || plannedBuilding.useFixedPosition || plannedBuilding.useFixedParameters)
						{
							CityPlanModel.ScheduledBuilding scheduledBuilding = this.Scope.Get<CityPlanModel.ScheduledBuilding>();
							scheduledBuilding.time = spawnTime;
							scheduledBuilding.spawnAttempts = 0;
							scheduledBuilding.type = plannedBuilding.type;
							scheduledBuilding.groupIndex = plannedBuilding.groupIndex;
							scheduledBuilding.carparkPreference = plannedBuilding.carparkPreference;
							scheduledBuilding.grouping = plannedBuilding.grouping;
							scheduledBuilding.demandMultiplier = Fix64.One + (Fix64)plannedBuilding.additionalDemandMultiplier;
							scheduledBuilding.initialUpgradeLevel = 0;
							scheduledBuilding.useFixedParameters = plannedBuilding.useFixedParameters;
							scheduledBuilding.positionOverride = plannedBuilding.positionOverride;
							scheduledBuilding.entranceOverride = plannedBuilding.entranceOverride;
							scheduledBuilding.drivewayDirectionOverride = ((scheduledBuilding.type == CityTileType.Demand) ? plannedBuilding.directionOverride : plannedBuilding.drivewayDirectionOverride);
							scheduledBuilding.tutorialIdentifier = plannedBuilding.tutorialIdentifier;
							scheduledBuilding.carparkSideOverride = TileDirection.None;
							this._cityPlanModel.ScheduleBuilding(scheduledBuilding);
							goto IL_334;
						}
						goto IL_334;
					}
				}
			}
		}

		// Token: 0x040011B4 RID: 4532
		private CityDefinition _definition;

		// Token: 0x040011B5 RID: 4533
		private int _nextMotorwayId = 1;

		// Token: 0x040011B6 RID: 4534
		public static readonly Fix64 PlayableRatio = (Fix64)16f / (Fix64)9f;

		// Token: 0x040011B8 RID: 4536
		[Dependency]
		private ISimulation _simulation;

		// Token: 0x040011B9 RID: 4537
		[Dependency]
		private CityModel _cityModel;

		// Token: 0x040011BA RID: 4538
		[Dependency]
		private CityPlanModel _cityPlanModel;

		// Token: 0x040011BB RID: 4539
		[Dependency]
		private ClockModel _clockModel;

		// Token: 0x040011BC RID: 4540
		[Dependency]
		private GameBehaviourModel _behaviour;

		// Token: 0x02000361 RID: 865
		public enum PlayableAreaRoundingType
		{
			// Token: 0x040011BF RID: 4543
			ForceWholeTiles,
			// Token: 0x040011C0 RID: 4544
			AllowPartialTiles
		}
	}
}
