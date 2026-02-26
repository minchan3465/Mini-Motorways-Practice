using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003CC RID: 972
	public class RoadTileConstantsData : ScriptableObject
	{
		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001710 RID: 5904 RVA: 0x00053E1F File Offset: 0x0005201F
		public int RoadTileConfigurationCount
		{
			get
			{
				return this.roadTileConfigurations.Count;
			}
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x00053E2C File Offset: 0x0005202C
		public RoadTileConstantsData.RoadTileConfiguration FindOrCreateRoadTileConfiguration(int definitionIndex)
		{
			RoadTileConstantsData.RoadTileConfiguration configuration = this.roadTileConfigurations.FirstOrDefault((RoadTileConstantsData.RoadTileConfiguration possibleConfig) => possibleConfig.definitionIndex == definitionIndex);
			if (configuration == null)
			{
				configuration = new RoadTileConstantsData.RoadTileConfiguration
				{
					definitionIndex = definitionIndex
				};
				this.roadTileConfigurations.Add(configuration);
			}
			return configuration;
		}

		// Token: 0x040013C0 RID: 5056
		public float trafficLightRadiusOffset = 0.75f;

		// Token: 0x040013C1 RID: 5057
		[NonReorderable]
		public List<RoadTileConstantsData.RoadTileConfiguration> roadTileConfigurations = new List<RoadTileConstantsData.RoadTileConfiguration>();

		// Token: 0x020003CD RID: 973
		[Serializable]
		public class RoadTileConfiguration
		{
			// Token: 0x040013C2 RID: 5058
			public int definitionIndex;

			// Token: 0x040013C3 RID: 5059
			public Vector2 interactionCircleOffset;

			// Token: 0x040013C4 RID: 5060
			public Vector2[] trafficLightOffsets;
		}
	}
}
