using System;
using UnityEngine.EventSystems;

// Token: 0x020001AA RID: 426
public class ControllerInputEventData : BaseEventData
{
	// Token: 0x0600097C RID: 2428 RVA: 0x0001F5DA File Offset: 0x0001D7DA
	public ControllerInputEventData(EventSystem eventSystem, IController onController) : base(eventSystem)
	{
		this.instigatingController = onController;
	}

	// Token: 0x040004FE RID: 1278
	public IController instigatingController;
}
