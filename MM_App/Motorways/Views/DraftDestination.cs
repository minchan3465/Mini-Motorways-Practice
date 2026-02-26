using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Factory;
using Factory.Pools;
using FixMath;
using Motorways.Models;
using Motorways.UI;
using Motorways.Views.MeshGeneration;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Motorways.Views
{
	// Token: 0x020005B9 RID: 1465
	[SelectionBase]
	public class DraftDestination : MonoBehaviour, IReusable, ICreativeModeEditableObject
	{
		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x060028C9 RID: 10441 RVA: 0x000AE001 File Offset: 0x000AC201
		public Vector2Int BottomLeftCoordinate
		{
			get
			{
				return this.viewModel.bottomLeft;
			}
		}

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x060028CA RID: 10442 RVA: 0x000AE00E File Offset: 0x000AC20E
		public bool IsDouble
		{
			get
			{
				return this.viewModel.isDouble;
			}
		}

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x060028CB RID: 10443 RVA: 0x000AE01B File Offset: 0x000AC21B
		public bool IsTrainStation
		{
			get
			{
				return this.viewModel.isTrainStation;
			}
		}

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x060028CC RID: 10444 RVA: 0x000AE028 File Offset: 0x000AC228
		public bool IsBoatTerminal
		{
			get
			{
				return this.viewModel.isBoatTerminal;
			}
		}

		// Token: 0x060028CD RID: 10445 RVA: 0x000AE035 File Offset: 0x000AC235
		public void Initialize(IScope scope, bool isDouble)
		{
			this._scope = scope;
			this._hasOriginal = false;
			this.viewModel.InitializeNew(isDouble, scope.Get<ColourWidget>().CurrentColour);
			this._isConfirmable = true;
			this.UpdateView(false);
		}

		// Token: 0x060028CE RID: 10446 RVA: 0x000AE06A File Offset: 0x000AC26A
		public void InitializeWithExistingView(IScope scope, DestinationView view)
		{
			this._scope = scope;
			this._hasOriginal = true;
			this._isConfirmable = true;
			this.viewModel.InitializeExisting(view.Model);
			this._originalViewModel.InitializeExisting(view.Model);
			this.UpdateView(true);
		}

		// Token: 0x060028CF RID: 10447 RVA: 0x000AE0AC File Offset: 0x000AC2AC
		public void UpdatePosition(Vector2Int bottomLeftCoordinate, bool isReplacement)
		{
			this.viewModel.bottomLeft = bottomLeftCoordinate;
			Vector3 carparkPosition = TilemapView.GetWorldPositionForCoordinates(bottomLeftCoordinate);
			base.transform.position = carparkPosition;
			this.UpdateView(isReplacement);
		}

		// Token: 0x060028D0 RID: 10448 RVA: 0x000AE0E0 File Offset: 0x000AC2E0
		public void Reset()
		{
			this.viewModel.Reset();
			this._originalViewModel.Reset();
			this._hasOriginal = false;
			this._isConfirmable = true;
			Transform transform = base.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		// Token: 0x060028D1 RID: 10449 RVA: 0x000271AA File Offset: 0x000253AA
		public void SetGameobjectActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}

		// Token: 0x060028D2 RID: 10450 RVA: 0x000AE138 File Offset: 0x000AC338
		public Bounds GetBounds()
		{
			float tileWidth = (float)TilemapModel.TileWidth;
			Vector3 minimumBounds = this.viewModel.isDouble ? ((this.viewModel.buildingLayout == BuildingLayout.BuildingAbove) ? (this.viewModel.GetWorldPositionBuilding1() + new Vector3(-tileWidth, -2f * tileWidth)) : (this.viewModel.GetWorldPositionBuilding2() + new Vector3(-2f * tileWidth, -tileWidth))) : ((this.viewModel.buildingLayout == BuildingLayout.BuildingAbove) ? (this.viewModel.GetWorldPositionBuilding1() + new Vector3(-tileWidth, -2f * tileWidth)) : (this.viewModel.GetWorldPositionBuilding1() + new Vector3(-2f * tileWidth, -tileWidth)));
			Vector3 maximumBounds = minimumBounds + (this.viewModel.isDouble ? ((this.viewModel.buildingLayout == BuildingLayout.BuildingAbove) ? new Vector3(4f * tileWidth, 3f * tileWidth) : new Vector3(3f * tileWidth, 4f * tileWidth)) : ((this.viewModel.buildingLayout == BuildingLayout.BuildingAbove) ? new Vector3(2f * tileWidth, 3f * tileWidth) : new Vector3(3f * tileWidth, 2f * tileWidth)));
			return new Bounds
			{
				min = minimumBounds,
				max = maximumBounds
			};
		}

		// Token: 0x060028D3 RID: 10451 RVA: 0x000AE290 File Offset: 0x000AC490
		public void Delete(bool isReplacement)
		{
			if (!this.viewModel.isDouble || !this.viewModel.hasSecondDestination)
			{
				this._scope.Release(this);
				return;
			}
			this.UpdateView(isReplacement);
			if (this.viewModel.activeBuilding == this.viewModel.building1)
			{
				this.RemoveFirstDestination(isReplacement, true);
				return;
			}
			this.RemoveSecondDestination(isReplacement, true);
		}

		// Token: 0x060028D4 RID: 10452 RVA: 0x000AE2F7 File Offset: 0x000AC4F7
		public bool IsConfirmable()
		{
			return this._isConfirmable;
		}

		// Token: 0x060028D5 RID: 10453 RVA: 0x000AE2FF File Offset: 0x000AC4FF
		public BuildingLayout GetBuildingLayout()
		{
			return this.viewModel.buildingLayout;
		}

		// Token: 0x060028D6 RID: 10454 RVA: 0x000AE30C File Offset: 0x000AC50C
		public Vector2 GetWorldPosition()
		{
			return base.transform.position + (this.viewModel.isDouble ? new Vector3(2f, 3f) : new Vector3(2f, 0f));
		}

		// Token: 0x060028D7 RID: 10455 RVA: 0x000AE001 File Offset: 0x000AC201
		public Vector2Int GetTilePosition()
		{
			return this.viewModel.bottomLeft;
		}

		// Token: 0x060028D8 RID: 10456 RVA: 0x000AE35C File Offset: 0x000AC55C
		public Vector2 GetCenterForEditMenuPosition()
		{
			Vector3 worldPositionForActiveBuilding = this.viewModel.GetWorldPositionForActiveBuilding();
			if (this.viewModel.isTrainStation && this.viewModel.carparkSide == TileDirection.North)
			{
				worldPositionForActiveBuilding += 0.75f * (float)TilemapModel.TileWidth * Vector3.up;
			}
			else if (this.viewModel.isTrainStation && this.viewModel.carparkSide == TileDirection.West)
			{
				worldPositionForActiveBuilding += 0.75f * (float)TilemapModel.TileWidth * Vector3.left;
			}
			return worldPositionForActiveBuilding;
		}

		// Token: 0x060028D9 RID: 10457 RVA: 0x000AE3F8 File Offset: 0x000AC5F8
		public bool CompletelyOutOfPlayArea(City city)
		{
			if (city == null || this.viewModel == null)
			{
				return false;
			}
			for (int coordX = this.viewModel.minCoordinates.x; coordX <= this.viewModel.maxCoordinates.x; coordX++)
			{
				for (int coordY = this.viewModel.minCoordinates.y; coordY <= this.viewModel.maxCoordinates.y; coordY++)
				{
					if (city.IsTileInPlayableArea(new Vector2Int(coordX, coordY), Fix64.MaxValue))
					{
						return false;
					}
				}
			}
			return !city.IsTileInPlayableArea(this.viewModel.drivewayCoordinates, Fix64.MaxValue);
		}

		// Token: 0x060028DA RID: 10458 RVA: 0x000AE495 File Offset: 0x000AC695
		public EditMenuButtonType GetEditOptions()
		{
			if (!this.IsBoatTerminal)
			{
				return this._editOptions;
			}
			if (this.viewModel.isDouble && this.viewModel.hasSecondDestination)
			{
				return this._boatTerminalEditOptions | EditMenuButtonType.Delete;
			}
			return this._boatTerminalEditOptions;
		}

		// Token: 0x060028DB RID: 10459 RVA: 0x000AE4D0 File Offset: 0x000AC6D0
		public void Confirm()
		{
			if (!Diagnostics.Verify(this.IsConfirmable(), "We should only confirm if the destination has a valid placement!"))
			{
				return;
			}
			this.SpawnDestination(this.viewModel);
			this._scope.Release(this);
		}

		// Token: 0x060028DC RID: 10460 RVA: 0x000AE500 File Offset: 0x000AC700
		private void StartUnplaceableView()
		{
			DraftDestination.Log.Info("Start unplaceable ghost view for" + this.ToString(), Array.Empty<object>());
			this._isConfirmable = false;
			this._renderTextureImage.color = new Color(this._renderTextureImage.color.r, this._renderTextureImage.color.g, this._renderTextureImage.color.b, this._ghostPreviewInvalidOpacity);
		}

		// Token: 0x060028DD RID: 10461 RVA: 0x000AE57C File Offset: 0x000AC77C
		private void EndUnplaceableView()
		{
			DraftDestination.Log.Info("End unplaceable ghost view for" + this.ToString(), Array.Empty<object>());
			this._isConfirmable = true;
			this._renderTextureImage.color = new Color(this._renderTextureImage.color.r, this._renderTextureImage.color.g, this._renderTextureImage.color.b, this._ghostPreviewNormalOpacity);
		}

		// Token: 0x060028DE RID: 10462 RVA: 0x000AE5F5 File Offset: 0x000AC7F5
		public void MakeDestinationTrainStation(bool isReplacement, TileDirection carparkSide)
		{
			this.viewModel.isTrainStation = true;
			this.viewModel.carparkSide = carparkSide;
			this.UpdateView(isReplacement);
		}

		// Token: 0x060028DF RID: 10463 RVA: 0x000AE618 File Offset: 0x000AC818
		public void MakeDestinationNotTrainStation(bool isReplacement)
		{
			this.viewModel.isTrainStation = false;
			if (this.viewModel.carparkSide == TileDirection.North || this.viewModel.carparkSide == TileDirection.East)
			{
				this.viewModel.carparkSide = TileUtilities.GetOppositeDirection(this.viewModel.carparkSide);
			}
			this.UpdateView(isReplacement);
		}

		// Token: 0x060028E0 RID: 10464 RVA: 0x000AE670 File Offset: 0x000AC870
		private void SpawnDestination(DraftDestinationCarparkViewModel viewModel)
		{
			CityPlanModel.ScheduledBuilding firstDestination = this._scope.Get<CityPlanModel.ScheduledBuilding>();
			viewModel.BuildScheduled(viewModel.building1, ref firstDestination);
			CityPlanModel cityPlanModel = this._scope.Get<CityPlanModel>();
			cityPlanModel.ScheduleBuilding(firstDestination);
			if (viewModel.hasSecondDestination)
			{
				CityPlanModel.ScheduledBuilding secondDestination = this._scope.Get<CityPlanModel.ScheduledBuilding>();
				viewModel.BuildScheduled(viewModel.building2, ref secondDestination);
				cityPlanModel.ScheduleBuilding(secondDestination);
			}
		}

		// Token: 0x060028E1 RID: 10465 RVA: 0x000AE6D3 File Offset: 0x000AC8D3
		public void Cancel()
		{
			if (this._hasOriginal)
			{
				this.SpawnDestination(this._originalViewModel);
			}
			this._scope.Release(this);
		}

		// Token: 0x060028E2 RID: 10466 RVA: 0x000AE6F6 File Offset: 0x000AC8F6
		public int GetGroupIndex()
		{
			return this.viewModel.activeBuilding.groupIndex;
		}

		// Token: 0x060028E3 RID: 10467 RVA: 0x000AE708 File Offset: 0x000AC908
		public void SetGroupIndex(int groupIndex, bool isReplacement)
		{
			this.viewModel.activeBuilding.groupIndex = groupIndex;
			this.UpdateView(isReplacement);
		}

		// Token: 0x060028E4 RID: 10468 RVA: 0x000AE722 File Offset: 0x000AC922
		public ICreativeModeEditableObject GetGhostPreview(out bool isOriginalDeleted)
		{
			isOriginalDeleted = false;
			return this;
		}

		// Token: 0x060028E5 RID: 10469 RVA: 0x000AE728 File Offset: 0x000AC928
		private void UpdateMesh(MeshFilter meshFilter, DestinationMesh.Type type, TileDirection direction, int groupIndex, int visualVariantIndex)
		{
			if (!Diagnostics.Verify(meshFilter != null, "DestinationMesh is null, set it in prefab"))
			{
				return;
			}
			DestinationMeshCombiner meshCombiner = this._scope.Get<DestinationMeshCombiner>();
			if (!Diagnostics.Verify(meshCombiner != null, "Cannot find DestinationMeshCombiner in scope"))
			{
				return;
			}
			Mesh mesh = meshCombiner.GetCombinedMesh(type, direction, groupIndex, visualVariantIndex);
			meshFilter.mesh = mesh;
		}

		// Token: 0x060028E6 RID: 10470 RVA: 0x000AE77C File Offset: 0x000AC97C
		public void Flip(bool isReplacement)
		{
			if (!Diagnostics.Verify(!this.viewModel.isDouble, "Flip called on a double destination, but it only makes sense on Single Destinations!"))
			{
				return;
			}
			this.viewModel.singleDestinationAboveDrivewayDirections = ((this.viewModel.singleDestinationAboveDrivewayDirections == DrivewayDirection.East) ? DrivewayDirection.West : DrivewayDirection.East);
			this.viewModel.singleDestinationToSideDrivewayDirections = ((this.viewModel.singleDestinationToSideDrivewayDirections == DrivewayDirection.North) ? DrivewayDirection.South : DrivewayDirection.North);
			this._animator.SetTrigger((this.viewModel.buildingLayout == BuildingLayout.BuildingAbove) ? DraftDestination.TriggerFlipHorizontal : DraftDestination.TriggerFlipVertical);
			this.UpdateView(isReplacement);
		}

		// Token: 0x060028E7 RID: 10471 RVA: 0x000AE809 File Offset: 0x000ACA09
		public void UpgradeOrDowngrade(bool isReplacement)
		{
			this.viewModel.activeBuilding.upgradeLevel = ((this.viewModel.activeBuilding.upgradeLevel == 0) ? 1 : 0);
			this.UpdateView(isReplacement);
		}

		// Token: 0x060028E8 RID: 10472 RVA: 0x000AE838 File Offset: 0x000ACA38
		public void Rotate(bool isReplacement)
		{
			switch (this.viewModel.carparkSide)
			{
			case TileDirection.North:
				this.viewModel.carparkSide = TileDirection.West;
				this._animator.SetTrigger(DraftDestination.TriggerRotateCounterClockWise);
				goto IL_D3;
			case TileDirection.East:
				this.viewModel.carparkSide = TileDirection.South;
				this._animator.SetTrigger(DraftDestination.TriggerRotateClockWise);
				goto IL_D3;
			case TileDirection.South:
				this.viewModel.carparkSide = TileDirection.West;
				this._animator.SetTrigger(DraftDestination.TriggerRotateClockWise);
				goto IL_D3;
			case TileDirection.West:
				this.viewModel.carparkSide = TileDirection.South;
				this._animator.SetTrigger(DraftDestination.TriggerRotateCounterClockWise);
				goto IL_D3;
			}
			DraftDestination.Log.Error("Invalid carpark side {0}!", new object[]
			{
				this.viewModel.carparkSide
			});
			IL_D3:
			this.viewModel.buildingLayout = ((this.viewModel.buildingLayout == BuildingLayout.BuildingAbove) ? BuildingLayout.BuildingToSide : BuildingLayout.BuildingAbove);
			if (this.IsDouble && this.viewModel.hasSecondDestination && this.viewModel.activeBuilding == this.viewModel.building2)
			{
				if (this.viewModel.buildingLayout == BuildingLayout.BuildingToSide)
				{
					this.viewModel.bottomLeft += new Vector2Int(2, 2);
				}
				else
				{
					this.viewModel.bottomLeft += new Vector2Int(-2, -2);
				}
			}
			this.UpdateView(isReplacement);
		}

		// Token: 0x060028E9 RID: 10473 RVA: 0x000AE9B4 File Offset: 0x000ACBB4
		private void UpdateView(bool isReplacement)
		{
			TileDirection? trainStationCarparkSide;
			bool destinationPlaceable = this.PlaceDestination(isReplacement, out trainStationCarparkSide);
			if (destinationPlaceable && !this._isConfirmable)
			{
				this.EndUnplaceableView();
			}
			else if (!destinationPlaceable && this._isConfirmable)
			{
				this.StartUnplaceableView();
			}
			EditMenuPanel editMenuPanel = this._scope.Get<EditMenuPanel>();
			if (editMenuPanel.IsOpen && editMenuPanel.isActiveAndEnabled)
			{
				editMenuPanel.RefreshView(false);
			}
			if (trainStationCarparkSide != null)
			{
				if (this.viewModel.isTrainStation)
				{
					TileDirection carparkSide = this.viewModel.carparkSide;
					TileDirection? tileDirection = trainStationCarparkSide;
					if (carparkSide == tileDirection.GetValueOrDefault() & tileDirection != null)
					{
						goto IL_A5;
					}
				}
				if (this.viewModel.isDouble)
				{
					this.MakeDestinationTrainStation(isReplacement, trainStationCarparkSide.Value);
					goto IL_CF;
				}
			}
			IL_A5:
			if (trainStationCarparkSide == null && this.viewModel.isTrainStation && this.viewModel.isDouble)
			{
				this.MakeDestinationNotTrainStation(isReplacement);
			}
			IL_CF:
			DrivewayDirection drivewayDirection = (this.viewModel.buildingLayout == BuildingLayout.BuildingAbove) ? this.viewModel.singleDestinationAboveDrivewayDirections : this.viewModel.singleDestinationToSideDrivewayDirections;
			this._carparkMeshes.SetVisibleCarparkMesh(this.viewModel.isDouble, drivewayDirection, this.viewModel.carparkSide, this.viewModel.isBoatTerminal, this.viewModel.activeBuilding == this.viewModel.building2);
			DraftDestinationBuildingViewModel building = this.viewModel.building1;
			this._destinationMesh1.gameObject.SetActive(true);
			this._destinationMesh1.transform.localPosition = this.viewModel.GetLocalPositionBuilding1();
			base.transform.position = this.viewModel.GetWorldPositionForActiveBuilding();
			this.UpdateMesh(this._destinationMesh1, building.GetMeshType(this.viewModel.isTrainStation, this.viewModel.buildingLayout), this.viewModel.carparkSide, building.groupIndex, 0);
			bool showBuilding2 = this.viewModel.isDouble && this.viewModel.hasSecondDestination;
			this._destinationMesh2.gameObject.SetActive(showBuilding2);
			if (showBuilding2)
			{
				DraftDestinationBuildingViewModel building2 = this.viewModel.building2;
				this._destinationMesh2.transform.localPosition = this.viewModel.GetLocalPositionBuilding2();
				this.UpdateMesh(this._destinationMesh2, building2.GetMeshType(this.viewModel.isTrainStation, this.viewModel.buildingLayout), this.viewModel.carparkSide, building2.groupIndex, 0);
			}
		}

		// Token: 0x060028EA RID: 10474 RVA: 0x000AEC20 File Offset: 0x000ACE20
		private Task RemoveFirstDestination(bool isPreplacement, bool animation = true)
		{
			DraftDestination.<RemoveFirstDestination>d__57 <RemoveFirstDestination>d__;
			<RemoveFirstDestination>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RemoveFirstDestination>d__.<>4__this = this;
			<RemoveFirstDestination>d__.isPreplacement = isPreplacement;
			<RemoveFirstDestination>d__.animation = animation;
			<RemoveFirstDestination>d__.<>1__state = -1;
			<RemoveFirstDestination>d__.<>t__builder.Start<DraftDestination.<RemoveFirstDestination>d__57>(ref <RemoveFirstDestination>d__);
			return <RemoveFirstDestination>d__.<>t__builder.Task;
		}

		// Token: 0x060028EB RID: 10475 RVA: 0x000AEC74 File Offset: 0x000ACE74
		private Task RemoveSecondDestination(bool isPreplacement, bool animation = true)
		{
			DraftDestination.<RemoveSecondDestination>d__58 <RemoveSecondDestination>d__;
			<RemoveSecondDestination>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RemoveSecondDestination>d__.<>4__this = this;
			<RemoveSecondDestination>d__.isPreplacement = isPreplacement;
			<RemoveSecondDestination>d__.animation = animation;
			<RemoveSecondDestination>d__.<>1__state = -1;
			<RemoveSecondDestination>d__.<>t__builder.Start<DraftDestination.<RemoveSecondDestination>d__58>(ref <RemoveSecondDestination>d__);
			return <RemoveSecondDestination>d__.<>t__builder.Task;
		}

		// Token: 0x060028EC RID: 10476 RVA: 0x000AECC8 File Offset: 0x000ACEC8
		private Task ShiftingAnimation()
		{
			DraftDestination.<ShiftingAnimation>d__59 <ShiftingAnimation>d__;
			<ShiftingAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ShiftingAnimation>d__.<>4__this = this;
			<ShiftingAnimation>d__.<>1__state = -1;
			<ShiftingAnimation>d__.<>t__builder.Start<DraftDestination.<ShiftingAnimation>d__59>(ref <ShiftingAnimation>d__);
			return <ShiftingAnimation>d__.<>t__builder.Task;
		}

		// Token: 0x060028ED RID: 10477 RVA: 0x000AED0C File Offset: 0x000ACF0C
		private Task ShrinkingAnimation(MeshFilter meshFilter)
		{
			DraftDestination.<ShrinkingAnimation>d__60 <ShrinkingAnimation>d__;
			<ShrinkingAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ShrinkingAnimation>d__.meshFilter = meshFilter;
			<ShrinkingAnimation>d__.<>1__state = -1;
			<ShrinkingAnimation>d__.<>t__builder.Start<DraftDestination.<ShrinkingAnimation>d__60>(ref <ShrinkingAnimation>d__);
			return <ShrinkingAnimation>d__.<>t__builder.Task;
		}

		// Token: 0x060028EE RID: 10478 RVA: 0x000AED50 File Offset: 0x000ACF50
		private bool PlaceDestination(bool isReplacement, out TileDirection? trainStationCarparkSide)
		{
			bool isDestinationPlaceable = true;
			string errorMessage = "";
			City city = this._scope.Get<City>();
			TilemapModel tilemapModel = this._scope.Get<TilemapModel>();
			TilemapView tilemapView = this._scope.Get<TilemapView>();
			Fix64 expansionTime = this._scope.Get<ClockModel>().ExpansionTime;
			Vector2Int oldDrivewayCoordinates = this.viewModel.drivewayCoordinates;
			Vector2Int oldSecondDrivewayCoordinates = this.viewModel.secondDrivewayCoordinates;
			Vector2Int oldMinCoordinates = this.viewModel.minCoordinates;
			Vector2Int oldMaxCoordinates = this.viewModel.maxCoordinates;
			List<Vector2Int> trainTrackCoordinates = new List<Vector2Int>();
			trainStationCarparkSide = null;
			if (this.viewModel.SetCoordinateData(ref errorMessage))
			{
				for (int coordX = this.viewModel.minCoordinates.x; coordX <= this.viewModel.maxCoordinates.x; coordX++)
				{
					int coordY = this.viewModel.minCoordinates.y;
					while (coordY <= this.viewModel.maxCoordinates.y)
					{
						Vector2Int tileCoordinates = new Vector2Int(coordX, coordY);
						if (!city.Definition.TileIsBuildable(tileCoordinates) || city.Definition.TileIsOverWater(tileCoordinates) || city.Definition.TileIsUnderAMountain(tileCoordinates))
						{
							string str = "Can't place destination over tile at ";
							Vector2Int vector2Int = tileCoordinates;
							errorMessage = str + vector2Int.ToString() + " because it's " + ((!city.Definition.TileIsBuildable(tileCoordinates)) ? " not buildable" : "Water or Mountain");
							isDestinationPlaceable = false;
						}
						Tile tile = tilemapView.GetTile(tileCoordinates);
						if (tile != null && (tile.IsCenterOfRoundabout || tile.HasRoundabout(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed)))
						{
							DraftDestination.Log.Info("Cannot build destination on tile {0} as it contains a roundabout", new object[]
							{
								tile.Coordinates
							});
							isDestinationPlaceable = false;
						}
						if (tile != null && tile.HasRailConnection)
						{
							trainTrackCoordinates.Add(tileCoordinates);
						}
						if (tile == null || tile.ContentType == TileContentType.None)
						{
							goto IL_297;
						}
						if (isReplacement)
						{
							TileContentType contentType = tile.ContentType;
							if ((contentType == TileContentType.Destination || contentType == TileContentType.Carpark) && tileCoordinates.x >= oldMinCoordinates.x && tileCoordinates.x <= oldMaxCoordinates.x && tileCoordinates.y >= oldMinCoordinates.y && tileCoordinates.y <= oldMaxCoordinates.y)
							{
								DraftDestination.Log.Info("Allowing placement over {0} because it's the old self which hasn't deleted yet.", new object[]
								{
									tileCoordinates
								});
								goto IL_3D1;
							}
						}
						if (tile.ContentType != TileContentType.Tree || !city.Rules.ShouldBuildingsBulldozeTrees)
						{
							string str2 = "Can't place destination over tile at ";
							Vector2Int vector2Int = tileCoordinates;
							errorMessage = str2 + vector2Int.ToString() + " with content type " + tile.ContentType.ToString();
							isDestinationPlaceable = false;
							goto IL_297;
						}
						DraftDestination.Log.Info("Allowing placement over tree at {0} as this will get bulldozed", new object[]
						{
							tileCoordinates
						});
						IL_3D1:
						coordY++;
						continue;
						IL_297:
						int twoLaneRoadCount = (tile != null) ? tile.GetTwoLaneRoadCount(RoadState.Planned | RoadState.Pending | RoadState.Active | RoadState.Mothballed, Tile.MotorwayInclusion.Include) : 0;
						if (tile != null && twoLaneRoadCount > 0)
						{
							if (isReplacement)
							{
								TileDirection oldDrivewayDirection = (this.viewModel.DrivewayDirection == TileDirection.East) ? TileDirection.North : TileDirection.East;
								if (twoLaneRoadCount == 1 && tileCoordinates == oldDrivewayCoordinates)
								{
									TileDirectionBitfield tileDirectionBitfield = tile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore);
									if (tileCoordinates == oldDrivewayCoordinates && tileDirectionBitfield[oldDrivewayDirection])
									{
										DraftDestination.Log.Info("Allowing placement at {0} because the only lane is the old driveway", new object[]
										{
											tileCoordinates
										});
										goto IL_3D1;
									}
								}
								else if (twoLaneRoadCount == 1 && this.IsDouble && tileCoordinates == oldSecondDrivewayCoordinates)
								{
									TileDirectionBitfield tileDirectionBitfield2 = tile.GetTwoLaneRoads(RoadState.Active, Tile.MotorwayInclusion.Ignore);
									if (tileCoordinates == oldSecondDrivewayCoordinates && tileDirectionBitfield2[TileUtilities.GetOppositeDirection(oldDrivewayDirection)])
									{
										DraftDestination.Log.Info("Allowing placement at {0} because the only lane is the old (second) driveway", new object[]
										{
											tileCoordinates
										});
										goto IL_3D1;
									}
								}
							}
							string[] array = new string[5];
							array[0] = "Can't place destination over tile at ";
							int num = 1;
							Vector2Int vector2Int = tileCoordinates;
							array[num] = vector2Int.ToString();
							array[2] = " because it has ";
							array[3] = twoLaneRoadCount.ToString();
							array[4] = " roads";
							errorMessage = string.Concat(array);
							isDestinationPlaceable = false;
							goto IL_3D1;
						}
						goto IL_3D1;
					}
				}
				isDestinationPlaceable &= (trainTrackCoordinates.Count == 0 || trainTrackCoordinates.Count == 4);
				if ((true || isDestinationPlaceable) && trainTrackCoordinates.Count == 4)
				{
					foreach (Vector2Int trainTrackCoordinate in trainTrackCoordinates)
					{
						if (this.viewModel.buildingLayout == BuildingLayout.BuildingAbove)
						{
							if (trainTrackCoordinate.y == this.viewModel.minCoordinates.y)
							{
								if (trainStationCarparkSide == null)
								{
									trainStationCarparkSide = new TileDirection?(TileDirection.North);
								}
								else
								{
									TileDirection? tileDirection = trainStationCarparkSide;
									TileDirection tileDirection2 = TileDirection.North;
									if (!(tileDirection.GetValueOrDefault() == tileDirection2 & tileDirection != null))
									{
										DraftDestination.Log.Info("Not making train station at {0} because train track at {1} is not at the correct y coordinate {2}", new object[]
										{
											this.viewModel.minCoordinates,
											trainTrackCoordinate,
											this.viewModel.minCoordinates.y + 2
										});
										isDestinationPlaceable = false;
										trainStationCarparkSide = null;
										break;
									}
								}
							}
							else
							{
								if (trainTrackCoordinate.y != this.viewModel.minCoordinates.y + 2)
								{
									DraftDestination.Log.Info("Not making train station at {0} because train track at {1} is not at a y coordinate {2} or {3}", new object[]
									{
										this.viewModel.minCoordinates,
										trainTrackCoordinate,
										this.viewModel.minCoordinates.y,
										this.viewModel.minCoordinates.y + 2
									});
									isDestinationPlaceable = false;
									trainStationCarparkSide = null;
									break;
								}
								if (trainStationCarparkSide == null)
								{
									trainStationCarparkSide = new TileDirection?(TileDirection.South);
								}
								else
								{
									TileDirection? tileDirection = trainStationCarparkSide;
									TileDirection tileDirection2 = TileDirection.South;
									if (!(tileDirection.GetValueOrDefault() == tileDirection2 & tileDirection != null))
									{
										DraftDestination.Log.Info("Not making train station at {0} because train track at {1} is not at the correct y coordinate {2}", new object[]
										{
											this.viewModel.minCoordinates,
											trainTrackCoordinate,
											this.viewModel.minCoordinates.y
										});
										isDestinationPlaceable = false;
										trainStationCarparkSide = null;
										break;
									}
								}
							}
						}
						else if (this.viewModel.buildingLayout == BuildingLayout.BuildingToSide)
						{
							if (trainTrackCoordinate.x == this.viewModel.minCoordinates.x)
							{
								if (trainStationCarparkSide == null)
								{
									trainStationCarparkSide = new TileDirection?(TileDirection.East);
								}
								else
								{
									TileDirection? tileDirection = trainStationCarparkSide;
									TileDirection tileDirection2 = TileDirection.East;
									if (!(tileDirection.GetValueOrDefault() == tileDirection2 & tileDirection != null))
									{
										DraftDestination.Log.Info("Not making train station at {0} because train track at {1} is not at the correct x coordinate {2}", new object[]
										{
											this.viewModel.minCoordinates,
											trainTrackCoordinate,
											this.viewModel.minCoordinates.x + 2
										});
										isDestinationPlaceable = false;
										trainStationCarparkSide = null;
										break;
									}
								}
							}
							else
							{
								if (trainTrackCoordinate.x != this.viewModel.minCoordinates.x + 2)
								{
									DraftDestination.Log.Info("Not making train station at {0} because train track at {1} is not at a x coordinate {2} or {3}", new object[]
									{
										this.viewModel.minCoordinates,
										trainTrackCoordinate,
										this.viewModel.minCoordinates.x,
										this.viewModel.minCoordinates.x + 2
									});
									isDestinationPlaceable = false;
									trainStationCarparkSide = null;
									break;
								}
								if (trainStationCarparkSide == null)
								{
									trainStationCarparkSide = new TileDirection?(TileDirection.West);
								}
								else
								{
									TileDirection? tileDirection = trainStationCarparkSide;
									TileDirection tileDirection2 = TileDirection.West;
									if (!(tileDirection.GetValueOrDefault() == tileDirection2 & tileDirection != null))
									{
										DraftDestination.Log.Info("Not making train station at {0} because train track at {1} is not at the correct x coordinate {2}", new object[]
										{
											this.viewModel.minCoordinates,
											trainTrackCoordinate,
											this.viewModel.minCoordinates.x
										});
										isDestinationPlaceable = false;
										trainStationCarparkSide = null;
										break;
									}
								}
							}
						}
					}
				}
				if ((trainStationCarparkSide ?? TileDirection.NorthEast) == TileDirection.North)
				{
					this.viewModel.drivewayCoordinates += 2 * Vector2Int.up;
					this.viewModel.secondDrivewayCoordinates += 2 * Vector2Int.up;
				}
				else if (!(trainStationCarparkSide != TileDirection.East))
				{
					this.viewModel.drivewayCoordinates += 2 * Vector2Int.right;
					this.viewModel.secondDrivewayCoordinates += 2 * Vector2Int.right;
				}
				if (city.IsTileInPlayableArea(this.viewModel.minCoordinates, expansionTime) && city.IsTileInPlayableArea(this.viewModel.maxCoordinates, expansionTime) && city.IsTileInPlayableArea(this.viewModel.drivewayCoordinates, expansionTime) && (!this.viewModel.isDouble || city.IsTileInPlayableArea(this.viewModel.secondDrivewayCoordinates, expansionTime)))
				{
					Tile drivewayTile = tilemapView.GetTile(this.viewModel.drivewayCoordinates);
					TileContentType drivewayContentType = (drivewayTile != null) ? drivewayTile.ContentType : TileContentType.None;
					if (drivewayContentType != TileContentType.None)
					{
						if (isReplacement && (drivewayContentType == TileContentType.Destination || drivewayContentType == TileContentType.Carpark) && this.viewModel.drivewayCoordinates.x >= oldMinCoordinates.x && this.viewModel.drivewayCoordinates.x <= oldMaxCoordinates.x && this.viewModel.drivewayCoordinates.y >= oldMinCoordinates.y && this.viewModel.drivewayCoordinates.y <= oldMaxCoordinates.y)
						{
							DraftDestination.Log.Info("Allowing driveway over {0} because it's the old self which hasn't deleted yet.", new object[]
							{
								this.viewModel.drivewayCoordinates
							});
						}
						else if (drivewayContentType == TileContentType.Tree && city.Rules.ShouldBuildingsBulldozeTrees)
						{
							DraftDestination.Log.Info("Allowing placement over tree at {0} as this will get bulldozed", new object[]
							{
								this.viewModel.drivewayCoordinates
							});
						}
						else
						{
							string str3 = "Not allowing placement at ";
							Vector2Int vector2Int = this.viewModel.drivewayCoordinates;
							errorMessage = str3 + vector2Int.ToString() + " because driveway tile has content type " + drivewayContentType.ToString();
							isDestinationPlaceable = false;
						}
					}
					else if (drivewayTile != null && drivewayTile.HasRailConnection)
					{
						string str4 = "Not allowing placement at ";
						Vector2Int vector2Int = this.viewModel.drivewayCoordinates;
						errorMessage = str4 + vector2Int.ToString() + " because driveway tile has rail connection";
						isDestinationPlaceable = false;
					}
					else
					{
						Tile secondDrivewayTile = tilemapView.GetTile(this.viewModel.secondDrivewayCoordinates);
						TileContentType secondDrivewayContentType = (secondDrivewayTile != null) ? secondDrivewayTile.ContentType : TileContentType.None;
						if (this.viewModel.isDouble && secondDrivewayContentType != TileContentType.None)
						{
							if (isReplacement && (secondDrivewayContentType == TileContentType.Destination || secondDrivewayContentType == TileContentType.Carpark) && this.viewModel.secondDrivewayCoordinates.x >= oldMinCoordinates.x && this.viewModel.secondDrivewayCoordinates.x <= oldMaxCoordinates.x && this.viewModel.secondDrivewayCoordinates.y >= oldMinCoordinates.y && this.viewModel.secondDrivewayCoordinates.y <= oldMaxCoordinates.y)
							{
								DraftDestination.Log.Info("Allowing second driveway over {0} because it's the old self which hasn't deleted yet.", new object[]
								{
									this.viewModel.drivewayCoordinates
								});
							}
							else if (secondDrivewayContentType == TileContentType.Tree && city.Rules.ShouldBuildingsBulldozeTrees)
							{
								DraftDestination.Log.Info("Allowing placement over tree at {0} as this will get bulldozed", new object[]
								{
									this.viewModel.secondDrivewayCoordinates
								});
							}
							else
							{
								string str5 = "Not allowing placement at ";
								Vector2Int vector2Int = this.viewModel.secondDrivewayCoordinates;
								errorMessage = str5 + vector2Int.ToString() + " because second driveway tile has content type " + secondDrivewayContentType.ToString();
								isDestinationPlaceable = false;
							}
						}
						else if (this.viewModel.isDouble && secondDrivewayTile != null && secondDrivewayTile.HasRailConnection)
						{
							string str6 = "Not allowing placement at ";
							Vector2Int vector2Int = this.viewModel.secondDrivewayCoordinates;
							errorMessage = str6 + vector2Int.ToString() + " because second driveway tile has rail connection";
							isDestinationPlaceable = false;
						}
					}
					if (!city.Definition.TileIsBuildable(this.viewModel.drivewayCoordinates) || city.Definition.TileIsOverWater(this.viewModel.drivewayCoordinates) || city.Definition.TileIsUnderAMountain(this.viewModel.drivewayCoordinates))
					{
						string str7 = "Can't place destination driveway over tile at ";
						Vector2Int vector2Int = this.viewModel.drivewayCoordinates;
						errorMessage = str7 + vector2Int.ToString() + " because it's " + (tilemapModel.IsTileReserved(this.viewModel.drivewayCoordinates) ? "Reserved" : "Water or Mountain");
						isDestinationPlaceable = false;
					}
					if (this.viewModel.isDouble && (!city.Definition.TileIsBuildable(this.viewModel.secondDrivewayCoordinates) || city.Definition.TileIsOverWater(this.viewModel.secondDrivewayCoordinates) || city.Definition.TileIsUnderAMountain(this.viewModel.secondDrivewayCoordinates)))
					{
						string str8 = "Can't place destination driveway over tile at ";
						Vector2Int vector2Int = this.viewModel.secondDrivewayCoordinates;
						errorMessage = str8 + vector2Int.ToString() + " because it's " + ((!city.Definition.TileIsBuildable(this.viewModel.secondDrivewayCoordinates)) ? "Not buildable" : "Water or Mountain");
						isDestinationPlaceable = false;
					}
				}
				else
				{
					isDestinationPlaceable = false;
				}
			}
			if (errorMessage != "")
			{
				Diagnostics.Log.Info("DraftDestination", errorMessage, Array.Empty<object>());
			}
			return isDestinationPlaceable;
		}

		// Token: 0x0400227E RID: 8830
		public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DraftDestination");

		// Token: 0x0400227F RID: 8831
		[SerializeField]
		private EditMenuButtonType _editOptions;

		// Token: 0x04002280 RID: 8832
		[SerializeField]
		[FormerlySerializedAs("BoatTerminalEditOptions")]
		private EditMenuButtonType _boatTerminalEditOptions;

		// Token: 0x04002281 RID: 8833
		[SerializeField]
		private MeshFilter _destinationMesh1;

		// Token: 0x04002282 RID: 8834
		[SerializeField]
		private MeshFilter _destinationMesh2;

		// Token: 0x04002283 RID: 8835
		[SerializeField]
		private DraftDestinationCarparkMeshes _carparkMeshes;

		// Token: 0x04002284 RID: 8836
		[SerializeField]
		private RawImage _renderTextureImage;

		// Token: 0x04002285 RID: 8837
		[SerializeField]
		private float _ghostPreviewNormalOpacity = 0.8f;

		// Token: 0x04002286 RID: 8838
		[SerializeField]
		private float _ghostPreviewInvalidOpacity = 0.5f;

		// Token: 0x04002287 RID: 8839
		[SerializeField]
		private Animator _animator;

		// Token: 0x04002288 RID: 8840
		private IScope _scope;

		// Token: 0x04002289 RID: 8841
		private Mesh _mesh;

		// Token: 0x0400228A RID: 8842
		private bool _isConfirmable;

		// Token: 0x0400228B RID: 8843
		private bool _hasOriginal;

		// Token: 0x0400228C RID: 8844
		public readonly DraftDestinationCarparkViewModel viewModel = new DraftDestinationCarparkViewModel();

		// Token: 0x0400228D RID: 8845
		private readonly DraftDestinationCarparkViewModel _originalViewModel = new DraftDestinationCarparkViewModel();

		// Token: 0x0400228E RID: 8846
		private static readonly int TriggerRotateClockWise = Animator.StringToHash("RotateCW");

		// Token: 0x0400228F RID: 8847
		private static readonly int TriggerRotateCounterClockWise = Animator.StringToHash("RotateCCW");

		// Token: 0x04002290 RID: 8848
		private static readonly int TriggerFlipHorizontal = Animator.StringToHash("FlipHorizontal");

		// Token: 0x04002291 RID: 8849
		private static readonly int TriggerFlipVertical = Animator.StringToHash("FlipVertical");
	}
}
