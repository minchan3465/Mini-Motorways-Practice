using System;
using Factory;

namespace Motorways
{
	// Token: 0x020003B0 RID: 944
	[Factory.Serializable(1)]
	public class MotorwaysUIInputEvent : InputEvent
	{
		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001671 RID: 5745 RVA: 0x0004DB38 File Offset: 0x0004BD38
		public GameUIButtonType UIButtonType
		{
			get
			{
				return this._uiButtonType;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001672 RID: 5746 RVA: 0x0004DB40 File Offset: 0x0004BD40
		// (set) Token: 0x06001673 RID: 5747 RVA: 0x0004DB48 File Offset: 0x0004BD48
		public int UIButtonIndex { get; protected set; }

		// Token: 0x06001674 RID: 5748 RVA: 0x0004DB51 File Offset: 0x0004BD51
		public override void Reset()
		{
			base.Reset();
			this._uiButtonType = GameUIButtonType.None;
			this.UIButtonIndex = -1;
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x0004DB67 File Offset: 0x0004BD67
		public static MotorwaysUIInputEvent CreateMouseUIEvent(IScope scope, InputEventMouseButtonType mouseButtonType, InputEventButtonState mouseButtonState, GameUIButtonType uiButtonType, int uiButtonIndex = 0)
		{
			MotorwaysUIInputEvent motorwaysUIInputEvent = scope.Get<MotorwaysUIInputEvent>();
			motorwaysUIInputEvent._source = 0;
			motorwaysUIInputEvent.SourceIndex = 0;
			motorwaysUIInputEvent._buttonState = (int)mouseButtonState;
			motorwaysUIInputEvent.InputAction = BaseInputOverride.GetRewiredActionForMouseButtonIndex((int)mouseButtonType);
			motorwaysUIInputEvent._uiButtonType = uiButtonType;
			motorwaysUIInputEvent.UIButtonIndex = uiButtonIndex;
			return motorwaysUIInputEvent;
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x0004DB9F File Offset: 0x0004BD9F
		public static MotorwaysUIInputEvent CreateTouchUIEvent(IScope scope, int touchIndex, InputEventButtonState buttonState, GameUIButtonType uiButtonType, int uiButtonIndex = 0)
		{
			MotorwaysUIInputEvent motorwaysUIInputEvent = scope.Get<MotorwaysUIInputEvent>();
			motorwaysUIInputEvent._source = 1;
			motorwaysUIInputEvent.SourceIndex = touchIndex;
			motorwaysUIInputEvent.InputAction = -1;
			motorwaysUIInputEvent._buttonState = (int)buttonState;
			motorwaysUIInputEvent._uiButtonType = uiButtonType;
			motorwaysUIInputEvent.UIButtonIndex = uiButtonIndex;
			return motorwaysUIInputEvent;
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x0004DBD2 File Offset: 0x0004BDD2
		public static MotorwaysUIInputEvent CreateGenericUIEvent(IScope scope, int rewiredAction, InputEventSource inputSource, InputEventButtonState buttonState, GameUIButtonType uiButtonType, int uiButtonIndex = 0)
		{
			MotorwaysUIInputEvent motorwaysUIInputEvent = scope.Get<MotorwaysUIInputEvent>();
			motorwaysUIInputEvent._source = (int)inputSource;
			motorwaysUIInputEvent.SourceIndex = 0;
			motorwaysUIInputEvent._buttonState = (int)buttonState;
			motorwaysUIInputEvent.InputAction = rewiredAction;
			motorwaysUIInputEvent._uiButtonType = uiButtonType;
			motorwaysUIInputEvent.UIButtonIndex = uiButtonIndex;
			return motorwaysUIInputEvent;
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x0004DC08 File Offset: 0x0004BE08
		public override int CompareTo(InputEvent otherEvent)
		{
			int baseResult = base.CompareTo(otherEvent);
			if (baseResult != 0)
			{
				return baseResult;
			}
			MotorwaysUIInputEvent motorwaysUIInputEvent = otherEvent as MotorwaysUIInputEvent;
			if (this._uiButtonType != GameUIButtonType.None && motorwaysUIInputEvent._uiButtonType != this._uiButtonType)
			{
				return motorwaysUIInputEvent._uiButtonType - this._uiButtonType;
			}
			if (this.UIButtonIndex != motorwaysUIInputEvent.UIButtonIndex)
			{
				return motorwaysUIInputEvent.UIButtonIndex - this.UIButtonIndex;
			}
			return 0;
		}

		// Token: 0x0400131A RID: 4890
		protected GameUIButtonType _uiButtonType;
	}
}
