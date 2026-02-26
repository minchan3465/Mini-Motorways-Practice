using System;

// Token: 0x0200007A RID: 122
public class TickAppCommand : AppCommand
{
	// Token: 0x06000134 RID: 308 RVA: 0x00004D79 File Offset: 0x00002F79
	public bool Configure(float timestamp, float frameTime)
	{
		base.Timestamp = timestamp;
		this._frameTime = frameTime;
		return true;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00004D8A File Offset: 0x00002F8A
	public override void Reset()
	{
		this._frameTime = 0f;
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00004D97 File Offset: 0x00002F97
	public override bool Execute(IApp receiver)
	{
		receiver.Tick(base.Timestamp, this._frameTime);
		return true;
	}

	// Token: 0x0400006C RID: 108
	private float _frameTime;
}
