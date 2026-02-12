using System;
using Factory;
using UnityEngine;

// Token: 0x02000178 RID: 376
public interface IPointerState
{
	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x0600086C RID: 2156
	Vector2 Position { get; }

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x0600086D RID: 2157
	Vector2 PositionDelta { get; }

	// Token: 0x0600086E RID: 2158
	void Initialize(IScope scope);

	// Token: 0x0600086F RID: 2159
	ButtonState GetButtonState(int buttonIndex);

	// Token: 0x06000870 RID: 2160
	void SetButtonState(float appTime, int buttonIndex, InputEventButtonState newState);

	// Token: 0x06000871 RID: 2161
	void MoveTo(float appTime, Vector2 position, PointerMoveToDeltaBehaviour deltaBehaviour = PointerMoveToDeltaBehaviour.CalculateDelta);

	// Token: 0x06000872 RID: 2162
	void Tick(float appTime);

	// Token: 0x06000873 RID: 2163
	Touch ToUnityTouch();
}
