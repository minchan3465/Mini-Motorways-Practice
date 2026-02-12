using System;

// Token: 0x0200015C RID: 348
public class ButtonState
{
	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x060007A8 RID: 1960 RVA: 0x00018E0A File Offset: 0x0001700A
	public InputEventButtonState CurrentState
	{
		get
		{
			return this._currentState;
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060007A9 RID: 1961 RVA: 0x00018E12 File Offset: 0x00017012
	public float StateChangeTime
	{
		get
		{
			return this._stateChangeTime;
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060007AA RID: 1962 RVA: 0x00018E1A File Offset: 0x0001701A
	public bool IsDown
	{
		get
		{
			return this.CurrentState == InputEventButtonState.Down || this.CurrentState == InputEventButtonState.JustDown;
		}
	}

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x060007AB RID: 1963 RVA: 0x00018E30 File Offset: 0x00017030
	public bool IsUp
	{
		get
		{
			return !this.IsDown;
		}
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x00018E3B File Offset: 0x0001703B
	public void SetState(float stateTime, InputEventButtonState newState)
	{
		this._stateChangeTime = stateTime;
		this._currentState = newState;
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x00018E4B File Offset: 0x0001704B
	public void Tick(float appTime)
	{
		if (appTime > this.StateChangeTime)
		{
			if (this.CurrentState == InputEventButtonState.JustUp)
			{
				this.SetState(appTime, InputEventButtonState.Up);
			}
			if (this.CurrentState == InputEventButtonState.JustDown)
			{
				this.SetState(appTime, InputEventButtonState.Down);
			}
		}
	}

	// Token: 0x04000387 RID: 903
	private InputEventButtonState _currentState;

	// Token: 0x04000388 RID: 904
	private float _stateChangeTime;
}
