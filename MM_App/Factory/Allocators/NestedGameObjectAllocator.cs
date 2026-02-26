using System;
using Client;
using UnityEngine;

namespace Factory.Allocators
{
	// Token: 0x02000330 RID: 816
	public class NestedGameObjectAllocator<ComponentType, PrefabType> : IAllocator<ComponentType>, IDisposable where ComponentType : class, IView where PrefabType : Component
	{
		// Token: 0x060013B2 RID: 5042 RVA: 0x00040D68 File Offset: 0x0003EF68
		public ComponentType Allocate(IScope context)
		{
			PrefabType prefabComponent = context.Get<PrefabType>();
			if (prefabComponent == null)
			{
				return default(ComponentType);
			}
			return prefabComponent.GetComponentInChildren<ComponentType>(true);
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x00040DA0 File Offset: 0x0003EFA0
		public bool Release(ComponentType obj, IScope context)
		{
			this.OnObjectReleased(obj, context);
			return true;
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnObjectAssembled(ComponentType obj, IScope context)
		{
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x000022F5 File Offset: 0x000004F5
		protected virtual void OnObjectReleased(ComponentType obj, IScope context)
		{
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Dispose()
		{
		}
	}
}
