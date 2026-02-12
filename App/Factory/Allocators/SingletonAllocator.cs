using System;

namespace Factory.Allocators
{
	// Token: 0x02000331 RID: 817
	public class SingletonAllocator<T> : IAllocator<T>, IDisposable
	{
		// Token: 0x060013B8 RID: 5048 RVA: 0x00040DAB File Offset: 0x0003EFAB
		public SingletonAllocator(T instance)
		{
			this._instance = instance;
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x00040DBA File Offset: 0x0003EFBA
		public T Allocate(IScope context)
		{
			return this._instance;
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x000020AA File Offset: 0x000002AA
		public bool Release(T obj, IScope context)
		{
			return true;
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnObjectAssembled(T obj, IScope context)
		{
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Dispose()
		{
		}

		// Token: 0x0400107C RID: 4220
		private readonly T _instance;
	}
}
