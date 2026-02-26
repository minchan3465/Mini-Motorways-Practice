using System;
using Factory.Allocators;

namespace Factory.Pools
{
	// Token: 0x02000326 RID: 806
	public class ObjectPool<T> : Pool<T> where T : IReusable, new()
	{
		// Token: 0x06001377 RID: 4983 RVA: 0x000404F7 File Offset: 0x0003E6F7
		public ObjectPool() : base(new HeapAllocator<T>())
		{
		}
	}
}
