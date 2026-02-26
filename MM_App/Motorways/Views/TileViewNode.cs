using System;

namespace Motorways.Views
{
	// Token: 0x02000606 RID: 1542
	public class TileViewNode
	{
		// Token: 0x06002B1C RID: 11036 RVA: 0x000BDB3B File Offset: 0x000BBD3B
		public void Reset()
		{
			this.roadState = RoadState.None;
			this.isDynamic = false;
			this.deadEndRoad = null;
			this.isDeadEndConnectedToMotorway = false;
			this.isDeadEndConnectedToEditingMotorway = false;
		}

		// Token: 0x0400251C RID: 9500
		public RoadState roadState;

		// Token: 0x0400251D RID: 9501
		public bool isDynamic;

		// Token: 0x0400251E RID: 9502
		public DeadEndRoadView deadEndRoad;

		// Token: 0x0400251F RID: 9503
		public bool isDeadEndConnectedToMotorway;

		// Token: 0x04002520 RID: 9504
		public bool isDeadEndConnectedToEditingMotorway;
	}
}
