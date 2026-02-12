using System;
using Motorways;

// Token: 0x02000172 RID: 370
public class InputEventFilter : IComparable<InputEventFilter>
{
	// Token: 0x0600081A RID: 2074 RVA: 0x0001966E File Offset: 0x0001786E
	protected InputEventFilter(InputEventSource source, int sourceIndex, int rewiredAction, int buttonState)
	{
		this._source = (int)source;
		this._sourceIndex = sourceIndex;
		this._rewiredAction = rewiredAction;
		this._buttonState = buttonState;
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x00019693 File Offset: 0x00017893
	public static InputEventFilter CreateMouseEventFilter(int rewiredAction, InputEventButtonState buttonState)
	{
		return new InputEventFilter(InputEventSource.Mouse, InputEventFilter.AnySourceIndex, rewiredAction, (int)buttonState);
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x000196A4 File Offset: 0x000178A4
	public static InputEventFilter CreateTouchEventFilter(int touchIndex, InputEventButtonState touchState)
	{
		return new InputEventFilter(InputEventSource.Touch, touchIndex, -1, (int)touchState);
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x000196BE File Offset: 0x000178BE
	public static InputEventFilter CreateKeyboardEventFilter(int rewiredAction, InputEventButtonState buttonState)
	{
		return InputEventFilter.CreateEventFilter(InputEventSource.Keyboard, rewiredAction, buttonState);
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x000196C8 File Offset: 0x000178C8
	public static InputEventFilter CreateGenericEventFilter(int rewiredAction, InputEventButtonState buttonState)
	{
		return InputEventFilter.CreateEventFilter(InputEventSource.Generic, rewiredAction, buttonState);
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x000196D2 File Offset: 0x000178D2
	public static InputEventFilter CreateRemoteEventFilter(int rewiredAction, InputEventButtonState buttonState)
	{
		return InputEventFilter.CreateEventFilter(InputEventSource.Remote, rewiredAction, buttonState);
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x000196DC File Offset: 0x000178DC
	public static InputEventFilter CreateEventFilter(InputEventSource source, int rewiredAction, InputEventButtonState buttonState)
	{
		int sourceIndex = InputEventFilter.AnySourceIndex;
		return new InputEventFilter(source, sourceIndex, rewiredAction, (int)buttonState);
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x000196F8 File Offset: 0x000178F8
	public virtual bool MatchesEvent(InputEvent inputEvent)
	{
		return (!(inputEvent is MotorwaysUIInputEvent) || this is MotorwaysUIInputEventFilter) && (this._source == 5 || this._source == (int)inputEvent.Source) && (this._sourceIndex == InputEventFilter.AnySourceIndex || this._sourceIndex == inputEvent.SourceIndex) && (this._rewiredAction == InputEventFilter.AnyRewiredAction || this._rewiredAction == inputEvent.InputAction) && (this._buttonState == -1 || this._buttonState == (int)inputEvent.ButtonState);
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x00019784 File Offset: 0x00017984
	public virtual int CompareTo(InputEventFilter otherFilter)
	{
		if (this._source != otherFilter._source)
		{
			return otherFilter._source - this._source;
		}
		if (this._sourceIndex != otherFilter._sourceIndex)
		{
			return otherFilter._sourceIndex - this._sourceIndex;
		}
		if (this._rewiredAction != otherFilter._rewiredAction)
		{
			return otherFilter._rewiredAction - this._rewiredAction;
		}
		if (this._buttonState != otherFilter._buttonState)
		{
			return otherFilter._buttonState - this._buttonState;
		}
		return 0;
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00019804 File Offset: 0x00017A04
	public override bool Equals(object obj)
	{
		InputEventFilter filter = obj as InputEventFilter;
		return filter != null && this.CompareTo(filter) == 0;
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x00019827 File Offset: 0x00017A27
	public override int GetHashCode()
	{
		return this._source << 16 | this._sourceIndex << 12 | this._rewiredAction << 8 | this._buttonState << 4;
	}

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x06000825 RID: 2085 RVA: 0x0001984E File Offset: 0x00017A4E
	public int RewiredAction
	{
		get
		{
			return this._rewiredAction;
		}
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x06000826 RID: 2086 RVA: 0x00019856 File Offset: 0x00017A56
	public InputEventButtonState ExpectedButtonState
	{
		get
		{
			return (InputEventButtonState)this._buttonState;
		}
	}

	// Token: 0x040003BF RID: 959
	public static int AnySourceIndex = -1;

	// Token: 0x040003C0 RID: 960
	public static int AnyRewiredAction = -1;

	// Token: 0x040003C1 RID: 961
	private readonly int _source;

	// Token: 0x040003C2 RID: 962
	private readonly int _sourceIndex;

	// Token: 0x040003C3 RID: 963
	private readonly int _rewiredAction;

	// Token: 0x040003C4 RID: 964
	private readonly int _buttonState;
}
