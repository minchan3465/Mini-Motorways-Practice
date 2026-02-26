using System;

namespace Factory.Pools
{
	// Token: 0x0200032B RID: 811
	public class StringPool<T> : ObjectPool<T> where T : StandaloneLocString, new()
	{
		// Token: 0x0600139F RID: 5023 RVA: 0x000404E6 File Offset: 0x0003E6E6
		public override void InspectEntry(object entryInstance)
		{
			base.InspectEntry(entryInstance);
		}
	}
}
