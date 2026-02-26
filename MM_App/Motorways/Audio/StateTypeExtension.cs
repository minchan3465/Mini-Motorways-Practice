using System;

namespace Motorways.Audio
{
	// Token: 0x02000645 RID: 1605
	public static class StateTypeExtension
	{
		// Token: 0x06002CD7 RID: 11479 RVA: 0x0001DABF File Offset: 0x0001BCBF
		public static bool Contains(this StateType superset, StateType subset)
		{
			return (superset & subset) == subset;
		}
	}
}
