using System;
using Factory;

// Token: 0x02000005 RID: 5
public class ActivateControllerSelectAction : PlayerAction
{
	// Token: 0x06000005 RID: 5 RVA: 0x0000208E File Offset: 0x0000028E
	public override void OnActionBegin(float timestamp)
	{
		base.OnActionBegin(timestamp);
		this._hardwareCapabilities.ActivateControllerSelect();
	}

	// Token: 0x06000006 RID: 6 RVA: 0x000020A2 File Offset: 0x000002A2
	public override void Tick(float frameTime)
	{
		this.OnActionComplete();
	}

	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000007 RID: 7 RVA: 0x000020AA File Offset: 0x000002AA
	public override bool IsInterruptible
	{
		get
		{
			return true;
		}
	}

	// Token: 0x04000003 RID: 3
	[Dependency]
	private SwitchHardwareCapabilities _hardwareCapabilities;
}
