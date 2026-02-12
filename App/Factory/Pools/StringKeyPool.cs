using System;

namespace Factory.Pools
{
	// Token: 0x0200032A RID: 810
	public class StringKeyPool<T> : ObjectPool<T> where T : StringKey, new()
	{
		// Token: 0x0600139D RID: 5021 RVA: 0x000404E6 File Offset: 0x0003E6E6
		public override void InspectEntry(object entryInstance)
		{
			base.InspectEntry(entryInstance);
		}
	}
}
