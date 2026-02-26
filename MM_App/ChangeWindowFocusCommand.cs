using System;

// Token: 0x02000076 RID: 118
public class ChangeWindowFocusCommand : AppCommand
{
	// Token: 0x06000122 RID: 290 RVA: 0x00004C72 File Offset: 0x00002E72
	public void Configure(bool hasWindowFocus)
	{
		this._hasWindowFocus = hasWindowFocus;
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00004C7B File Offset: 0x00002E7B
	public override void Reset()
	{
		this._hasWindowFocus = false;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00004C84 File Offset: 0x00002E84
	public override bool Execute(IApp receiver)
	{
		receiver.InputState.OnWindowFocusChanged(this._hasWindowFocus);
		return true;
	}

	// Token: 0x04000065 RID: 101
	private bool _hasWindowFocus;
}
