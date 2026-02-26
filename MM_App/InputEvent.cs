using System;
using Factory;
using Factory.Pools;
using UnityEngine;

// Token: 0x0200016E RID: 366
[Factory.Serializable(1)]
public class InputEvent : IComparable<InputEvent>, IReusable
{
	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x0600080C RID: 2060 RVA: 0x00019412 File Offset: 0x00017612
	public InputEventSource Source
	{
		get
		{
			return (InputEventSource)this._source;
		}
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x0600080D RID: 2061 RVA: 0x0001941A File Offset: 0x0001761A
	// (set) Token: 0x0600080E RID: 2062 RVA: 0x00019422 File Offset: 0x00017622
	public int SourceIndex { get; protected set; }

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x0600080F RID: 2063 RVA: 0x0001942B File Offset: 0x0001762B
	// (set) Token: 0x06000810 RID: 2064 RVA: 0x00019433 File Offset: 0x00017633
	public int InputAction { get; protected set; }

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06000811 RID: 2065 RVA: 0x0001943C File Offset: 0x0001763C
	public InputEventButtonState ButtonState
	{
		get
		{
			return (InputEventButtonState)this._buttonState;
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x06000812 RID: 2066 RVA: 0x00019444 File Offset: 0x00017644
	public Vector2 PointerPosition
	{
		get
		{
			return this._pointerPosition;
		}
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0001944C File Offset: 0x0001764C
	public virtual void Reset()
	{
		this._source = -1;
		this.SourceIndex = -1;
		this.InputAction = -1;
		this._buttonState = -1;
		this._pointerPosition = Vector2.zero;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x00019475 File Offset: 0x00017675
	public static InputEvent CreateTouchEvent(IScope scope, int touchIndex, InputEventButtonState touchState, Vector2 touchPosition)
	{
		InputEvent inputEvent = scope.Get<InputEvent>();
		inputEvent._source = 1;
		inputEvent.SourceIndex = touchIndex;
		inputEvent.InputAction = -1;
		inputEvent._buttonState = (int)touchState;
		inputEvent._pointerPosition = touchPosition;
		return inputEvent;
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x000194A0 File Offset: 0x000176A0
	public static InputEvent CreateMouseEvent(IScope scope, int rewiredInput, InputEventButtonState buttonState, Vector2 mousePosition)
	{
		InputEvent inputEvent = scope.Get<InputEvent>();
		inputEvent._source = 0;
		inputEvent.InputAction = rewiredInput;
		inputEvent._buttonState = (int)buttonState;
		inputEvent._pointerPosition = mousePosition;
		return inputEvent;
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000194C4 File Offset: 0x000176C4
	public static InputEvent CreateEvent(IScope scope, int rewiredInput, InputEventButtonState buttonState, InputEventSource source)
	{
		InputEvent inputEvent = scope.Get<InputEvent>();
		inputEvent._source = (int)source;
		inputEvent.InputAction = rewiredInput;
		inputEvent._buttonState = (int)buttonState;
		return inputEvent;
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x000194E4 File Offset: 0x000176E4
	public virtual int CompareTo(InputEvent otherEvent)
	{
		if (this._source != otherEvent._source)
		{
			return otherEvent._source - this._source;
		}
		if (this.SourceIndex != otherEvent.SourceIndex)
		{
			return otherEvent.SourceIndex - this.SourceIndex;
		}
		if (this.InputAction != otherEvent.InputAction)
		{
			return otherEvent.InputAction - this.InputAction;
		}
		if (this._buttonState != otherEvent._buttonState)
		{
			return otherEvent._buttonState - this._buttonState;
		}
		if (!Mathf.Approximately(this._pointerPosition.x, otherEvent._pointerPosition.x))
		{
			if (this._pointerPosition.x >= otherEvent._pointerPosition.x)
			{
				return 1;
			}
			return -1;
		}
		else
		{
			if (Mathf.Approximately(this._pointerPosition.y, otherEvent._pointerPosition.y))
			{
				return 0;
			}
			if (this._pointerPosition.y >= otherEvent._pointerPosition.y)
			{
				return 1;
			}
			return -1;
		}
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x000195D4 File Offset: 0x000177D4
	public override string ToString()
	{
		return string.Format("{0} {1} -> Button {2} {3} -- {4}", new object[]
		{
			this.Source.ToString(),
			this.SourceIndex.ToString(),
			this.InputAction.ToString(),
			this.ButtonState.ToString(),
			this.PointerPosition.ToString()
		});
	}

	// Token: 0x040003A5 RID: 933
	protected int _source = -1;

	// Token: 0x040003A8 RID: 936
	protected int _buttonState = -1;

	// Token: 0x040003A9 RID: 937
	protected Vector2 _pointerPosition;

	// Token: 0x040003AA RID: 938
	public const int TouchInputActionId = -1;
}
