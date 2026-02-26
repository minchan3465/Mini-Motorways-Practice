using System;
using System.Collections.Generic;
using Client;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005A3 RID: 1443
	public interface IDebugRenderSetManager
	{
		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06002844 RID: 10308
		IReadOnlyDictionary<string, DebugRendererSet> RendererSets { get; }

		// Token: 0x06002845 RID: 10309
		void Register(MonoBehaviour monoBehaviour);

		// Token: 0x06002846 RID: 10310
		void Unregister(MonoBehaviour monoBehaviour);

		// Token: 0x06002847 RID: 10311
		void RegisterView(IView view);

		// Token: 0x06002848 RID: 10312
		void UnregisterView(IView view);
	}
}
