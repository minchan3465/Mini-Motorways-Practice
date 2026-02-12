using System;
using Factory;

// Token: 0x0200015D RID: 349
public class AxisInputEvent : InputEvent
{
	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x060007AF RID: 1967 RVA: 0x00018E78 File Offset: 0x00017078
	// (set) Token: 0x060007B0 RID: 1968 RVA: 0x00018E80 File Offset: 0x00017080
	public float AxisValue { get; private set; }

	// Token: 0x060007B1 RID: 1969 RVA: 0x00018E89 File Offset: 0x00017089
	public static AxisInputEvent CreateAxisEvent(IScope scope, int rewiredActionAxis, float newValue, InputEventSource source)
	{
		AxisInputEvent axisInputEvent = scope.Get<AxisInputEvent>();
		axisInputEvent._source = (int)source;
		axisInputEvent._buttonState = 5;
		axisInputEvent.InputAction = rewiredActionAxis;
		axisInputEvent.AxisValue = newValue;
		return axisInputEvent;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00018EAD File Offset: 0x000170AD
	public override void Reset()
	{
		base.Reset();
		this.AxisValue = 0f;
	}
}
