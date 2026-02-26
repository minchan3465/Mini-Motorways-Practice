using System;
using Factory;

// Token: 0x02000079 RID: 121
public class ProcessInputEventCommand : AppCommand, IReleasedFromScopeHandler
{
	// Token: 0x0600012F RID: 303 RVA: 0x00004D27 File Offset: 0x00002F27
	public bool Configure(float timestamp, InputEvent inputEvent)
	{
		base.Timestamp = timestamp;
		this._inputEvent = inputEvent;
		return true;
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00004D38 File Offset: 0x00002F38
	public override void Reset()
	{
		this._inputEvent = null;
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00004D41 File Offset: 0x00002F41
	public override bool Execute(IApp receiver)
	{
		receiver.InputState.OnInputEvent(base.Timestamp, this._inputEvent);
		return true;
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00004D5B File Offset: 0x00002F5B
	public void OnReleasedFromScope(IScope scope)
	{
		if (this._inputEvent != null)
		{
			scope.Release(this._inputEvent);
			this._inputEvent = null;
		}
	}

	// Token: 0x0400006B RID: 107
	private InputEvent _inputEvent;
}
