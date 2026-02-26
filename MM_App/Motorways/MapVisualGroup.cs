using System;
using System.Collections.Generic;
using Motorways.EdgeLoopOperator;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Motorways
{
	// Token: 0x0200039F RID: 927
	public class MapVisualGroup
	{
		// Token: 0x040012D6 RID: 4822
		public MapVisualGroupType groupType;

		// Token: 0x040012D7 RID: 4823
		public Tilemap sourceTilemap;

		// Token: 0x040012D8 RID: 4824
		public Dictionary<MapMeshLayer, TileLayer> tileLayers = new Dictionary<MapMeshLayer, TileLayer>();

		// Token: 0x040012D9 RID: 4825
		public MapMeshLayer[] containedLayers;

		// Token: 0x040012DA RID: 4826
		public Dictionary<MapMeshLayer, List<EdgeLoop>> edgeLoops = new Dictionary<MapMeshLayer, List<EdgeLoop>>();

		// Token: 0x040012DB RID: 4827
		public Dictionary<MapMeshLayer, Mesh> generatedMeshes = new Dictionary<MapMeshLayer, Mesh>();
	}
}
