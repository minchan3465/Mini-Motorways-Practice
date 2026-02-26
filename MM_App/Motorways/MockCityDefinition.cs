using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways
{
	// Token: 0x02000382 RID: 898
	public class MockCityDefinition : CityDefinition
	{
		// Token: 0x060015B2 RID: 5554 RVA: 0x0004A834 File Offset: 0x00048A34
		protected override void Awake()
		{
			this.cameraZoom.velocity.AddKey(0f, (float)this.cameraZoom.startSize);
			this.cameraZoom.velocity.AddKey((float)this.cameraZoom.durationInDays, (float)this.cameraZoom.endSize);
			base.Awake();
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x0004A8A1 File Offset: 0x00048AA1
		public override bool TileIsBuildable(Vector2Int position)
		{
			return !this.mockUnbuildablePositions.Contains(position);
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x0004A8B2 File Offset: 0x00048AB2
		public override bool TileIsOverWater(Vector2Int position)
		{
			return this.mockWaterPositions.Contains(position);
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x0004A8C0 File Offset: 0x00048AC0
		public override bool TileIsUnderAMountain(Vector2Int position)
		{
			return this.mockMountainPositions.Contains(position);
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x0004A8CE File Offset: 0x00048ACE
		public override bool TileIsOverRail(Vector2Int position)
		{
			return this.mockRailPositions.Contains(position);
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x0004A8DC File Offset: 0x00048ADC
		public override bool TileIsDriveable(Vector2Int tileCoordinates)
		{
			return !this.TileIsOverWater(tileCoordinates) && !this.TileIsUnderAMountain(tileCoordinates) && this.TileIsBuildable(tileCoordinates);
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x0004A8F9 File Offset: 0x00048AF9
		public override bool TileIsZoneable(Vector2Int position)
		{
			return !this.mockUnzoneablePositions.Contains(position);
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x0004A90A File Offset: 0x00048B0A
		public override bool TileSupportsCircleDestinations(int groupIndex, Vector2Int position)
		{
			return this.mockValidCirclePositions.Contains(position);
		}

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x060015BA RID: 5562 RVA: 0x0004A918 File Offset: 0x00048B18
		public override CitySpawningLayerData TileWeightData
		{
			get
			{
				return this.mockCitySpawningLayerData;
			}
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x0004A920 File Offset: 0x00048B20
		public override DensityGroup DensityForPosition(Vector2Int position)
		{
			DensityGroup densityGroup;
			if (this.mockDensityGroup.TryGetValue(position, out densityGroup))
			{
				return densityGroup;
			}
			return DensityGroup.High;
		}

		// Token: 0x060015BC RID: 5564 RVA: 0x0004A940 File Offset: 0x00048B40
		protected override Dictionary<Vector2Int, RailType> CompileRailTileCoordinates()
		{
			Dictionary<Vector2Int, RailType> railTileCoordinates = new Dictionary<Vector2Int, RailType>();
			foreach (Vector2Int railTileCoordinate in this.mockRailPositions)
			{
				railTileCoordinates.Add(railTileCoordinate, this.mockTrainPositions.Contains(railTileCoordinate) ? RailType.TrainOrigin : RailType.Normal);
			}
			return railTileCoordinates;
		}

		// Token: 0x0400127D RID: 4733
		public readonly List<Vector2Int> mockWaterPositions = new List<Vector2Int>();

		// Token: 0x0400127E RID: 4734
		public readonly List<Vector2Int> mockUnbuildablePositions = new List<Vector2Int>();

		// Token: 0x0400127F RID: 4735
		public readonly List<Vector2Int> mockMountainPositions = new List<Vector2Int>();

		// Token: 0x04001280 RID: 4736
		public readonly List<Vector2Int> mockRailPositions = new List<Vector2Int>();

		// Token: 0x04001281 RID: 4737
		public readonly List<Vector2Int> mockTrainPositions = new List<Vector2Int>();

		// Token: 0x04001282 RID: 4738
		public readonly List<Vector2Int> mockUnzoneablePositions = new List<Vector2Int>();

		// Token: 0x04001283 RID: 4739
		public readonly List<Vector2Int> mockValidCirclePositions = new List<Vector2Int>();

		// Token: 0x04001284 RID: 4740
		public CitySpawningLayerData mockCitySpawningLayerData = new CitySpawningLayerData();

		// Token: 0x04001285 RID: 4741
		public readonly Dictionary<Vector2Int, DensityGroup> mockDensityGroup = new Dictionary<Vector2Int, DensityGroup>();
	}
}
