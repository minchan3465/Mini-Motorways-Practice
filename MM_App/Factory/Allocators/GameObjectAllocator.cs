using System;
using UnityEngine;

namespace Factory.Allocators
{
	// Token: 0x0200032D RID: 813
	public class GameObjectAllocator<T> : IAllocator<T>, IDisposable where T : Component
	{
		// Token: 0x060013A2 RID: 5026 RVA: 0x00040CB7 File Offset: 0x0003EEB7
		public GameObjectAllocator(string bundleName, string prefabName)
		{
			this._prefab = AssetBundleUtility.LoadPrefab(bundleName, prefabName);
		}

		// Token: 0x060013A3 RID: 5027 RVA: 0x00040CCC File Offset: 0x0003EECC
		public GameObjectAllocator(GameObject prefab)
		{
			this._prefab = prefab;
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Dispose()
		{
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x00040CDB File Offset: 0x0003EEDB
		public T Allocate(IScope context)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._prefab);
			gameObject.SetActive(true);
			return (T)((object)gameObject.GetComponent(typeof(T)));
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x00040D03 File Offset: 0x0003EF03
		public bool Release(T obj, IScope context)
		{
			obj.transform.SetParent(null);
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(obj.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(obj.gameObject);
			}
			return true;
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x000022F5 File Offset: 0x000004F5
		public virtual void OnObjectAssembled(T obj, IScope context)
		{
		}

		// Token: 0x0400107B RID: 4219
		private readonly GameObject _prefab;
	}
}
