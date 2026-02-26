using System;
using Factory;

namespace Server
{
	// Token: 0x02000290 RID: 656
	public class EmptyModelFrame : IFrame
	{
		// Token: 0x0600101E RID: 4126 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Reset()
		{
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x000020AA File Offset: 0x000002AA
		public bool CloneInto(IFrame cloneState, IScope scope)
		{
			return true;
		}
	}
}
