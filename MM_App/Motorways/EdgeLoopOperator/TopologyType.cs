using System;

namespace Motorways.EdgeLoopOperator
{
	// Token: 0x0200052B RID: 1323
	[Flags]
	public enum TopologyType
	{
		// Token: 0x04001CF9 RID: 7417
		None = 0,
		// Token: 0x04001CFA RID: 7418
		Flat = 1,
		// Token: 0x04001CFB RID: 7419
		Concave = 2,
		// Token: 0x04001CFC RID: 7420
		Convex = 4,
		// Token: 0x04001CFD RID: 7421
		ComplexCorner = 8,
		// Token: 0x04001CFE RID: 7422
		Any = 15
	}
}
