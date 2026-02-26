using System;
using Factory;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005B1 RID: 1457
	public interface IOnScreenTool
	{
		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06002891 RID: 10385
		Rect InputBlockingRect { get; }

		// Token: 0x06002892 RID: 10386
		void OnGUI(IScope scope);

		// Token: 0x06002893 RID: 10387
		void Update();
	}
}
