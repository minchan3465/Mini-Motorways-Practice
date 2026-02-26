using System;

namespace Motorways
{
	// Token: 0x020003B1 RID: 945
	public class MotorwaysUIInputEventFilter : InputEventFilter
	{
		// Token: 0x0600167A RID: 5754 RVA: 0x0004DC6A File Offset: 0x0004BE6A
		private MotorwaysUIInputEventFilter(InputEventSource source, int sourceIndex, int rewiredAction, int buttonState, GameUIButtonType uiButtonType) : base(source, sourceIndex, rewiredAction, buttonState)
		{
			this._uiButtonType = uiButtonType;
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x0004DC7F File Offset: 0x0004BE7F
		public static InputEventFilter CreateMouseUIEventFilter(int rewiredAction, GameUIButtonType uiButtonType, InputEventButtonState mouseButtonState)
		{
			return new MotorwaysUIInputEventFilter(InputEventSource.Mouse, 0, rewiredAction, (int)mouseButtonState, uiButtonType);
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x0004DC8B File Offset: 0x0004BE8B
		public static InputEventFilter CreateTouchUIEventFilter(int touchIndex, GameUIButtonType uiButtonType, InputEventButtonState buttonState)
		{
			return new MotorwaysUIInputEventFilter(InputEventSource.Touch, touchIndex, -1, (int)buttonState, uiButtonType);
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x0004DC97 File Offset: 0x0004BE97
		public static InputEventFilter CreateGenericUIEventFilter(int rewiredAction, GameUIButtonType uiButtonType, InputEventButtonState mouseButtonState)
		{
			return new MotorwaysUIInputEventFilter(InputEventSource.Generic, 0, rewiredAction, (int)mouseButtonState, uiButtonType);
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x0004DCA3 File Offset: 0x0004BEA3
		public static InputEventFilter CreateRemoteUIEventFilter(int rewiredAction, GameUIButtonType uiButtonType, InputEventButtonState mouseButtonState)
		{
			return new MotorwaysUIInputEventFilter(InputEventSource.Remote, 0, rewiredAction, (int)mouseButtonState, uiButtonType);
		}

		// Token: 0x0600167F RID: 5759 RVA: 0x0004DCB0 File Offset: 0x0004BEB0
		public override bool MatchesEvent(InputEvent inputEvent)
		{
			if (!base.MatchesEvent(inputEvent))
			{
				return false;
			}
			MotorwaysUIInputEvent motorwaysUIInputEvent = inputEvent as MotorwaysUIInputEvent;
			return this._uiButtonType == GameUIButtonType.None || (motorwaysUIInputEvent != null && this._uiButtonType == motorwaysUIInputEvent.UIButtonType);
		}

		// Token: 0x06001680 RID: 5760 RVA: 0x0004DCEC File Offset: 0x0004BEEC
		public override int CompareTo(InputEventFilter otherFilter)
		{
			int baseCompare = base.CompareTo(otherFilter);
			if (baseCompare != 0)
			{
				return baseCompare;
			}
			MotorwaysUIInputEventFilter motorwaysUIInputEvent = otherFilter as MotorwaysUIInputEventFilter;
			if (this._uiButtonType != GameUIButtonType.None && motorwaysUIInputEvent._uiButtonType != this._uiButtonType)
			{
				return motorwaysUIInputEvent._uiButtonType - this._uiButtonType;
			}
			return 0;
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x0004DD34 File Offset: 0x0004BF34
		public override bool Equals(object obj)
		{
			InputEventFilter filter = obj as InputEventFilter;
			return filter != null && this.CompareTo(filter) == 0;
		}

		// Token: 0x06001682 RID: 5762 RVA: 0x0004DD57 File Offset: 0x0004BF57
		public override int GetHashCode()
		{
			return base.GetHashCode() | (int)this._uiButtonType;
		}

		// Token: 0x0400131C RID: 4892
		private readonly GameUIButtonType _uiButtonType;
	}
}
