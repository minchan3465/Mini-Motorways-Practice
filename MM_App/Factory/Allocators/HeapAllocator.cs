using System;

namespace Factory.Allocators
{
	// Token: 0x0200032E RID: 814
	public class HeapAllocator<T> : IAllocator<T>, IDisposable where T : new()
	{
		// Token: 0x060013A8 RID: 5032 RVA: 0x00040D40 File Offset: 0x0003EF40
		public T Allocate(IScope context)
		{
			T newT = Activator.CreateInstance<T>();
			this.OnObjectAllocated(newT, context);
			return newT;
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x00040D5C File Offset: 0x0003EF5C
		public bool Release(T obj, IScope context)
		{
			this.OnObjectReleased(obj, context);
			return true;
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnObjectAssembled(T obj, IScope context)
		{
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnObjectAllocated(T obj, IScope context)
		{
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnObjectReleased(T obj, IScope context)
		{
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Dispose()
		{
		}
	}
}
