using System;
using System.Collections.Generic;
using FixMath;

namespace Motorways
{
	// Token: 0x0200036E RID: 878
	[Serializable]
	public class ScheduleChunk
	{
		// Token: 0x040011F5 RID: 4597
		public float startDay;

		// Token: 0x040011F6 RID: 4598
		public float duration;

		// Token: 0x040011F7 RID: 4599
		public bool buildingsAreOrdered;

		// Token: 0x040011F8 RID: 4600
		public Fix64 spawnVariability = Fix64Consts.OneHalf;

		// Token: 0x040011F9 RID: 4601
		public List<PlannedBuilding> plannedBuildings;
	}
}
