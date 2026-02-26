using System;
using Factory.Allocators;
using UnityEngine;

namespace Factory.Pools
{
	// Token: 0x02000322 RID: 802
	public class GameObjectPool<T> : Pool<T> where T : Component, IReusable
	{
		// Token: 0x0600136E RID: 4974 RVA: 0x00040499 File Offset: 0x0003E699
		public GameObjectPool(string bundleName, string prefabName) : base(new GameObjectAllocator<T>(bundleName, prefabName))
		{
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x000404A8 File Offset: 0x0003E6A8
		public GameObjectPool(GameObject prefab) : base(new GameObjectAllocator<T>(prefab))
		{
		}

		// Token: 0x06001370 RID: 4976 RVA: 0x000404B6 File Offset: 0x0003E6B6
		protected override void OnObjectCreated(T obj, IScope context)
		{
			obj.gameObject.SetActive(false);
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x000404C9 File Offset: 0x0003E6C9
		protected override void OnObjectAllocated(T obj, IScope context)
		{
			obj.gameObject.SetActive(true);
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x000404B6 File Offset: 0x0003E6B6
		protected override void OnObjectReleased(T obj, IScope context)
		{
			obj.gameObject.SetActive(false);
		}
	}
}
