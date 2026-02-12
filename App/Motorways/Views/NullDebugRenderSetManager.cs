using System;
using System.Collections.Generic;
using Client;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005A7 RID: 1447
	public class NullDebugRenderSetManager : IDebugRenderSetManager
	{
		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06002860 RID: 10336 RVA: 0x00004BD9 File Offset: 0x00002DD9
		public IReadOnlyDictionary<string, DebugRendererSet> RendererSets
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002861 RID: 10337 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Register(MonoBehaviour monoBehaviour)
		{
		}

		// Token: 0x06002862 RID: 10338 RVA: 0x000022F5 File Offset: 0x000004F5
		public void Unregister(MonoBehaviour monoBehaviour)
		{
		}

		// Token: 0x06002863 RID: 10339 RVA: 0x000022F5 File Offset: 0x000004F5
		public void RegisterView(IView view)
		{
		}

		// Token: 0x06002864 RID: 10340 RVA: 0x000022F5 File Offset: 0x000004F5
		public void UnregisterView(IView view)
		{
		}
	}
}
