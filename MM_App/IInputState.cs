using System;

// Token: 0x0200016D RID: 365
public interface IInputState
{
	// Token: 0x170001BF RID: 447
	// (get) Token: 0x060007EF RID: 2031
	IPointerState Mouse { get; }

	// Token: 0x060007F0 RID: 2032
	bool TryGetTouch(int touchIndex, out IPointerState result);

	// Token: 0x060007F1 RID: 2033
	float GetAxis(int rewiredInputAxisId);

	// Token: 0x060007F2 RID: 2034
	bool GetButton(int rewiredInputAction);

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x060007F3 RID: 2035
	int TouchCount { get; }

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x060007F4 RID: 2036
	int MaxTouchCount { get; }

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x060007F5 RID: 2037
	bool MousePresent { get; }

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x060007F6 RID: 2038
	// (set) Token: 0x060007F7 RID: 2039
	bool BlockUIInput { get; set; }

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x060007F8 RID: 2040
	// (set) Token: 0x060007F9 RID: 2041
	bool BlockGameInput { get; set; }

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x060007FA RID: 2042
	// (set) Token: 0x060007FB RID: 2043
	bool BlockAllInput { get; set; }

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x060007FC RID: 2044
	// (set) Token: 0x060007FD RID: 2045
	bool BlockActions { get; set; }

	// Token: 0x060007FE RID: 2046
	void Start();

	// Token: 0x060007FF RID: 2047
	void Tick(float appTime);

	// Token: 0x06000800 RID: 2048
	void OnInputEvent(float appTime, InputEvent inputEvent);

	// Token: 0x06000801 RID: 2049
	bool IsInputEventOverUI(InputEvent inputEvent);

	// Token: 0x06000802 RID: 2050
	void OnWindowFocusChanged(bool appHasWindowFocus);

	// Token: 0x06000803 RID: 2051
	void OnInternalFocusChanged(bool appHasInternalFocus);

	// Token: 0x06000804 RID: 2052
	void SubscribeToControllerConnectionMessages(IControllerConnectionObserver controllerConnectionObserver);

	// Token: 0x06000805 RID: 2053
	void UnsubscribeFromControllerConnectionMessages(IControllerConnectionObserver controllerConnectionObserver);

	// Token: 0x06000806 RID: 2054
	void ControllerConnected(IController newController);

	// Token: 0x06000807 RID: 2055
	void ControllerDisconnected(IController oldController);

	// Token: 0x06000808 RID: 2056
	void EnsurePollingRewiredAction(int rewiredInputAction);

	// Token: 0x06000809 RID: 2057
	void IgnoreInputAction(int rewiredInputAction);

	// Token: 0x0600080A RID: 2058
	void EnsurePollingAxis(int rewiredInputAxisId);

	// Token: 0x0600080B RID: 2059
	void IgnorePollingAxis(int rewiredInputAxisId);
}
