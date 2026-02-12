using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x0200037C RID: 892
	public class CitySpawningLayerData
	{
		// Token: 0x060015A1 RID: 5537 RVA: 0x0004A4C0 File Offset: 0x000486C0
		public CitySpawningLayerData()
		{
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x0004A4D4 File Offset: 0x000486D4
		public CitySpawningLayerData(List<CityTileTypeDefinition> definitions, Tilemap stationTiles, Tilemap boatTerminalTiles)
		{
			foreach (CityTileTypeDefinition definition in definitions)
			{
				this.weights[CityTilemap.LayerIdFor(definition.type, definition.groupIndex)] = new BuildingSpawningTileWeights(definition.tiles);
			}
			if (stationTiles != null)
			{
				this.stationWeights = new BuildingSpawningTileWeights(stationTiles);
			}
			if (boatTerminalTiles != null)
			{
				this.boatTerminalWeights = new BuildingSpawningTileWeights(boatTerminalTiles);
			}
		}

		// Token: 0x0400124B RID: 4683
		public Dictionary<int, BuildingSpawningTileWeights> weights = new Dictionary<int, BuildingSpawningTileWeights>();

		// Token: 0x0400124C RID: 4684
		public BuildingSpawningTileWeights stationWeights;

		// Token: 0x0400124D RID: 4685
		public BuildingSpawningTileWeights boatTerminalWeights;
	}
}
