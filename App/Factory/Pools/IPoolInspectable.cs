using System;
using System.Collections.Generic;

namespace Factory.Pools
{
	// Token: 0x02000327 RID: 807
	public interface IPoolInspectable
	{
		// Token: 0x06001378 RID: 4984
		void GetAllElements(List<object> allocated, List<object> free);

		// Token: 0x06001379 RID: 4985
		void InspectEntryGrouping(object entryInstance, Dictionary<object, bool> expandedLookup);

		// Token: 0x0600137A RID: 4986
		void InspectEntry(object entryInstance);

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x0600137B RID: 4987
		int AllocatedObjectCount { get; }
	}
}
