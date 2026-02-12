using System;
using Motorways.Models;
using Motorways.Views.MeshGeneration;

namespace Motorways.Views
{
	// Token: 0x020005BE RID: 1470
	public class DraftDestinationBuildingViewModel
	{
		// Token: 0x060028F9 RID: 10489 RVA: 0x000B0176 File Offset: 0x000AE376
		public DestinationMesh.Type GetMeshType(bool isTrainStation, BuildingLayout buildingLayout)
		{
			if (!isTrainStation)
			{
				if (this.upgradeLevel != 1)
				{
					return DestinationMesh.Type.Square;
				}
				return DestinationMesh.Type.Circle;
			}
			else
			{
				if (buildingLayout != BuildingLayout.BuildingAbove)
				{
					return DestinationMesh.Type.StationVertical;
				}
				return DestinationMesh.Type.StationHorizontal;
			}
		}

		// Token: 0x060028FA RID: 10490 RVA: 0x000B018E File Offset: 0x000AE38E
		public void Reset()
		{
			this.groupIndex = 0;
			this.upgradeLevel = 0;
		}

		// Token: 0x040022AF RID: 8879
		public int groupIndex;

		// Token: 0x040022B0 RID: 8880
		public int upgradeLevel;
	}
}
