using System;
using Server;

namespace Factory.Pools
{
	// Token: 0x02000325 RID: 805
	public class ModelPool<T> : ObjectPool<T> where T : IModel, new()
	{
		// Token: 0x06001374 RID: 4980 RVA: 0x000404DC File Offset: 0x0003E6DC
		protected override void OnObjectReleased(T obj, IScope context)
		{
			base.OnObjectReleased(obj, context);
		}

		// Token: 0x06001375 RID: 4981 RVA: 0x000404E6 File Offset: 0x0003E6E6
		public override void InspectEntry(object entryInstance)
		{
			base.InspectEntry(entryInstance);
		}
	}
}
