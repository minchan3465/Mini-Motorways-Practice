using System;

namespace Motorways
{
	// Token: 0x0200041C RID: 1052
	[Flags]
	public enum RoadState
	{
		// Token: 0x040015C4 RID: 5572
		None = 0,
		// Token: 0x040015C5 RID: 5573
		Planned = 2,
		// Token: 0x040015C6 RID: 5574
		Pending = 4,
		// Token: 0x040015C7 RID: 5575
		Active = 8,
		// Token: 0x040015C8 RID: 5576
		Mothballed = 16,
		// Token: 0x040015C9 RID: 5577
		Live = 24,
		// Token: 0x040015CA RID: 5578
		VisiblyActive = 14,
		// Token: 0x040015CB RID: 5579
		ActiveOrPending = 12
	}
}
