using System;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class DisableUITouchInputOverride : BaseInputOverride
{
	// Token: 0x170001BE RID: 446
	// (get) Token: 0x060007EC RID: 2028 RVA: 0x0000222C File Offset: 0x0000042C
	public override int touchCount
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x000193F4 File Offset: 0x000175F4
	public override Touch GetTouch(int index)
	{
		return default(Touch);
	}
}
