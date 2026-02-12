using System;
using Factory;

namespace Server
{
	// Token: 0x0200028E RID: 654
	public interface IFrame
	{
		// Token: 0x06001012 RID: 4114
		void Reset();

		// Token: 0x06001013 RID: 4115
		bool CloneInto(IFrame cloneState, IScope scope);
	}
}
