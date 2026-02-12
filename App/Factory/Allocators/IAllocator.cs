using System;

namespace Factory.Allocators
{
	// Token: 0x0200032F RID: 815
	public interface IAllocator<T> : IDisposable
	{
		// Token: 0x060013AF RID: 5039
		T Allocate(IScope owningScope);

		// Token: 0x060013B0 RID: 5040
		bool Release(T obj, IScope owningScope);

		// Token: 0x060013B1 RID: 5041
		void OnObjectAssembled(T obj, IScope owningScope);
	}
}
