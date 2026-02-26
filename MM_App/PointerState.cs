using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

// Token: 0x02000179 RID: 377
public class PointerState : IPointerState
{
	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x06000874 RID: 2164 RVA: 0x0001A4D2 File Offset: 0x000186D2
	public Vector2 Position
	{
		get
		{
			return this._position;
		}
	}

	// Token: 0x170001E3 RID: 483
	// (get) Token: 0x06000875 RID: 2165 RVA: 0x0001A4DA File Offset: 0x000186DA
	public Vector2 PositionDelta
	{
		get
		{
			return this._delta;
		}
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0001A4E2 File Offset: 0x000186E2
	public void Initialize(IScope scope)
	{
		this._scope = scope;
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0001A4EC File Offset: 0x000186EC
	public void Tick(float appTime)
	{
		if (appTime > this._deltaTimestep)
		{
			this._delta = Vector2.zero;
		}
		foreach (ButtonState buttonState in this._buttons.Values)
		{
			buttonState.Tick(appTime);
		}
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0001A558 File Offset: 0x00018758
	public void MoveTo(float appTime, Vector2 position, PointerMoveToDeltaBehaviour deltaBehaviour = PointerMoveToDeltaBehaviour.CalculateDelta)
	{
		if (deltaBehaviour == PointerMoveToDeltaBehaviour.CalculateDelta)
		{
			this._delta = position - this._position;
		}
		else
		{
			this._delta = Vector2.zero;
		}
		this._position = position;
		this._deltaTimestep = appTime;
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0001A58A File Offset: 0x0001878A
	public ButtonState GetButtonState(int rewiredIndex)
	{
		if (this._buttons.ContainsKey(rewiredIndex))
		{
			return this._buttons[rewiredIndex];
		}
		return PointerState.DummyButtonState;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0001A5AC File Offset: 0x000187AC
	public void SetButtonState(float appTime, int rewiredIndex, InputEventButtonState newState)
	{
		ButtonState state;
		if (!this._buttons.TryGetValue(rewiredIndex, out state))
		{
			state = this._scope.Get<ButtonState>();
			this._buttons.Add(rewiredIndex, state);
		}
		state.SetState(appTime, newState);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0001A5EC File Offset: 0x000187EC
	public Touch ToUnityTouch()
	{
		Touch thisTouch = default(Touch);
		InputEventButtonState currentState = this.GetButtonState(0).CurrentState;
		thisTouch.position = this._position;
		thisTouch.deltaPosition = this._delta;
		switch (currentState)
		{
		case InputEventButtonState.Up:
			thisTouch.phase = TouchPhase.Canceled;
			thisTouch.type = TouchType.Indirect;
			break;
		case InputEventButtonState.JustUp:
			thisTouch.phase = TouchPhase.Ended;
			break;
		case InputEventButtonState.Down:
			if (this._delta.sqrMagnitude > 0f)
			{
				thisTouch.phase = TouchPhase.Moved;
			}
			else
			{
				thisTouch.phase = TouchPhase.Stationary;
			}
			break;
		case InputEventButtonState.JustDown:
			thisTouch.phase = TouchPhase.Began;
			break;
		}
		return thisTouch;
	}

	// Token: 0x040003E9 RID: 1001
	private Vector2 _position;

	// Token: 0x040003EA RID: 1002
	private Vector2 _delta;

	// Token: 0x040003EB RID: 1003
	private float _deltaTimestep;

	// Token: 0x040003EC RID: 1004
	private readonly Dictionary<int, ButtonState> _buttons = new Dictionary<int, ButtonState>();

	// Token: 0x040003ED RID: 1005
	private IScope _scope;

	// Token: 0x040003EE RID: 1006
	private static readonly ButtonState DummyButtonState = new ButtonState();
}
