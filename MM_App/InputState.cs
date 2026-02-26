using System;
using System.Collections.Generic;
using Factory;
using Motorways;
using Motorways.Views;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000174 RID: 372
public class InputState : IInputState
{
	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06000829 RID: 2089 RVA: 0x00019874 File Offset: 0x00017A74
	// (set) Token: 0x0600082A RID: 2090 RVA: 0x0001987C File Offset: 0x00017A7C
	public float LastInputTimestamp { get; private set; }

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x0600082B RID: 2091 RVA: 0x00019885 File Offset: 0x00017A85
	// (set) Token: 0x0600082C RID: 2092 RVA: 0x0001988D File Offset: 0x00017A8D
	public int MaxRecognizedTouchCount { get; set; } = 1;

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x0600082D RID: 2093 RVA: 0x00019896 File Offset: 0x00017A96
	public IEnumerable<int> InputActionsToPoll
	{
		get
		{
			return this._keys.Keys;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x0600082E RID: 2094 RVA: 0x000198A3 File Offset: 0x00017AA3
	// (set) Token: 0x0600082F RID: 2095 RVA: 0x000198AB File Offset: 0x00017AAB
	public HashSet<int> InputActionsToIgnore { get; private set; }

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x06000830 RID: 2096 RVA: 0x000198B4 File Offset: 0x00017AB4
	public IEnumerable<int> AxisToPoll
	{
		get
		{
			return this._axis.Keys;
		}
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06000831 RID: 2097 RVA: 0x000198C1 File Offset: 0x00017AC1
	// (set) Token: 0x06000832 RID: 2098 RVA: 0x000198C9 File Offset: 0x00017AC9
	public HashSet<int> AxisToIgnore { get; private set; }

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06000833 RID: 2099 RVA: 0x000198D2 File Offset: 0x00017AD2
	// (set) Token: 0x06000834 RID: 2100 RVA: 0x000198DC File Offset: 0x00017ADC
	public DeviceInputType CurrentDeviceInputType
	{
		get
		{
			return this._currentDeviceInputType;
		}
		private set
		{
			DeviceInputType oldType = this._currentDeviceInputType;
			this._currentDeviceInputType = value;
			if (this._currentDeviceInputType != oldType)
			{
				InputState.Log.Info("Changing device input type to {0} from {1}", new object[]
				{
					this._currentDeviceInputType,
					oldType
				});
				foreach (InputState.IObserver observer in this.Observers)
				{
					observer.OnCurrentDeviceInputTypeChanged(this._currentDeviceInputType);
				}
				this.CurrentInputTypeRequiresFocus = InputState.DeviceInputTypeRequiresFocus(this._currentDeviceInputType);
			}
		}
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00019968 File Offset: 0x00017B68
	public virtual void SubscribeToControllerConnectionMessages(IControllerConnectionObserver controllerConnectionObserver)
	{
		this._controllerConnectionObservers.Subscribe(controllerConnectionObserver);
		for (int controllerIndex = 0; controllerIndex < this._controllers.Count; controllerIndex++)
		{
			controllerConnectionObserver.OnControllerConnected(this._controllers[controllerIndex]);
		}
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x000199A9 File Offset: 0x00017BA9
	public virtual void UnsubscribeFromControllerConnectionMessages(IControllerConnectionObserver controllerConnectionObserver)
	{
		this._controllerConnectionObservers.Unsubscribe(controllerConnectionObserver);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x000199B8 File Offset: 0x00017BB8
	public virtual void ControllerConnected(IController newController)
	{
		if (Diagnostics.Verify(!this._controllers.Contains(newController)))
		{
			this._controllers.Add(newController);
			newController.OnControllerConnected();
			foreach (IControllerConnectionObserver controllerConnectionObserver in this._controllerConnectionObservers)
			{
				controllerConnectionObserver.OnControllerConnected(newController);
			}
		}
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x00019A14 File Offset: 0x00017C14
	public virtual void ControllerDisconnected(IController oldController)
	{
		if (Diagnostics.Verify(this._controllers.Contains(oldController)))
		{
			this._controllers.Remove(oldController);
			oldController.OnControllerDisconnected();
			foreach (IControllerConnectionObserver controllerConnectionObserver in this._controllerConnectionObservers)
			{
				controllerConnectionObserver.OnControllerDisconnected(oldController);
			}
		}
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x00019A6C File Offset: 0x00017C6C
	public void OnGameLoseFocus()
	{
		foreach (ButtonState buttonState in this._keys.Values)
		{
			buttonState.SetState(buttonState.StateChangeTime + 0.0001f, InputEventButtonState.Up);
		}
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x00019AD0 File Offset: 0x00017CD0
	public virtual void EnsurePollingRewiredAction(int rewiredAction)
	{
		if (!this._keys.ContainsKey(rewiredAction))
		{
			this._keys.Add(rewiredAction, this._scope.Get<ButtonState>());
		}
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x00019AF8 File Offset: 0x00017CF8
	public virtual void IgnoreInputAction(int rewiredAction)
	{
		if (this.InputActionsToIgnore == null)
		{
			this.InputActionsToIgnore = new HashSet<int>();
		}
		this.InputActionsToIgnore.Add(rewiredAction);
		if (!this._keys.ContainsKey(rewiredAction))
		{
			this._keys.Add(rewiredAction, this._scope.Get<ButtonState>());
		}
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00019B4A File Offset: 0x00017D4A
	public virtual void EnsurePollingAxis(int rewiredAxis)
	{
		if (!this._axis.ContainsKey(rewiredAxis))
		{
			this._axis.Add(rewiredAxis, new AxisState());
		}
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x00019B6C File Offset: 0x00017D6C
	public virtual void IgnorePollingAxis(int axisName)
	{
		if (this.AxisToIgnore == null)
		{
			this.AxisToIgnore = new HashSet<int>();
		}
		this.AxisToIgnore.Add(axisName);
		if (!this._axis.ContainsKey(axisName))
		{
			this._axis.Add(axisName, new AxisState());
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x0600083E RID: 2110 RVA: 0x00019BB8 File Offset: 0x00017DB8
	public IPointerState Mouse
	{
		get
		{
			return this._mouse;
		}
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x00019BC0 File Offset: 0x00017DC0
	public void Start()
	{
		this._appHasWindowFocus = true;
		this._appHasInternalFocus = true;
		this._appHasFocus = true;
		this._mouse.Initialize(this._scope);
		IPointerState[] touches = this._touches;
		for (int i = 0; i < touches.Length; i++)
		{
			touches[i].Initialize(this._scope);
		}
		this.CurrentDeviceInputType = this._hardware.DefaultDeviceInputType;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x00019C27 File Offset: 0x00017E27
	public bool TryGetTouch(int touchIndex, out IPointerState result)
	{
		if (Diagnostics.Verify(touchIndex >= 0 && touchIndex < this.MaxTouchCount))
		{
			result = this._touches[touchIndex];
			return true;
		}
		result = null;
		return false;
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00019C50 File Offset: 0x00017E50
	public ButtonState GetKeyButtonState(int rewiredAction)
	{
		if (!this._keys.ContainsKey(rewiredAction))
		{
			return null;
		}
		return this._keys[rewiredAction];
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x00019C70 File Offset: 0x00017E70
	public bool GetButton(int inputAction)
	{
		ButtonState buttonState = this.GetKeyButtonState(inputAction);
		return buttonState != null && buttonState.IsDown;
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00019C90 File Offset: 0x00017E90
	public bool GetKeyUp(int inputAction)
	{
		ButtonState buttonState = this.GetKeyButtonState(inputAction);
		return buttonState != null && buttonState.CurrentState == InputEventButtonState.JustUp;
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x00019CB4 File Offset: 0x00017EB4
	public bool GetButtonDown(int keyCode)
	{
		ButtonState buttonState = this.GetKeyButtonState(keyCode);
		return buttonState != null && buttonState.CurrentState == InputEventButtonState.JustDown;
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00019CD7 File Offset: 0x00017ED7
	public float GetAxis(int axisName)
	{
		if (this._axis.ContainsKey(axisName))
		{
			return this._axis[axisName].GetAxisValue();
		}
		return 0f;
	}

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06000846 RID: 2118 RVA: 0x00019CFE File Offset: 0x00017EFE
	public bool MousePresent
	{
		get
		{
			return this.Mouse != null;
		}
	}

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x06000847 RID: 2119 RVA: 0x00019D0C File Offset: 0x00017F0C
	public int TouchCount
	{
		get
		{
			int touchCount = 0;
			foreach (IPointerState touch in this._touches)
			{
				if (touch != null && (touch.GetButtonState(0).IsDown || touch.GetButtonState(0).CurrentState == InputEventButtonState.JustUp))
				{
					touchCount++;
				}
			}
			return touchCount;
		}
	}

	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x06000848 RID: 2120 RVA: 0x00016FED File Offset: 0x000151ED
	public int MaxTouchCount
	{
		get
		{
			return 4;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x06000849 RID: 2121 RVA: 0x0000EFC6 File Offset: 0x0000D1C6
	public int MaxMouseButtonCount
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x0600084A RID: 2122 RVA: 0x00019D59 File Offset: 0x00017F59
	// (set) Token: 0x0600084B RID: 2123 RVA: 0x00019D71 File Offset: 0x00017F71
	public bool BlockUIInput
	{
		get
		{
			return this._blockInputFlags.HasFlag(InputState.BlockInput.BlockUI);
		}
		set
		{
			this._blockInputFlags = ((this._blockInputFlags & ~InputState.BlockInput.BlockUI) | (value ? InputState.BlockInput.BlockUI : InputState.BlockInput.AllowEverything));
			this._playerActionController.UpdateBlockFlags(this._blockInputFlags);
		}
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x0600084C RID: 2124 RVA: 0x00019D9B File Offset: 0x00017F9B
	// (set) Token: 0x0600084D RID: 2125 RVA: 0x00019DB3 File Offset: 0x00017FB3
	public bool BlockGameInput
	{
		get
		{
			return this._blockInputFlags.HasFlag(InputState.BlockInput.BlockGame);
		}
		set
		{
			this._blockInputFlags = ((this._blockInputFlags & ~InputState.BlockInput.BlockGame) | (value ? InputState.BlockInput.BlockGame : InputState.BlockInput.AllowEverything));
			this._playerActionController.UpdateBlockFlags(this._blockInputFlags);
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x0600084E RID: 2126 RVA: 0x00019DDD File Offset: 0x00017FDD
	// (set) Token: 0x0600084F RID: 2127 RVA: 0x00019DE8 File Offset: 0x00017FE8
	public bool BlockAllInput
	{
		get
		{
			return this._blockInputFlags == InputState.BlockInput.BlockEverything;
		}
		set
		{
			this._blockInputFlags = (value ? InputState.BlockInput.BlockEverything : InputState.BlockInput.AllowEverything);
			this._playerActionController.UpdateBlockFlags(this._blockInputFlags);
		}
	}

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x06000850 RID: 2128 RVA: 0x00019E08 File Offset: 0x00018008
	// (set) Token: 0x06000851 RID: 2129 RVA: 0x00019E20 File Offset: 0x00018020
	public bool BlockActions
	{
		get
		{
			return this._blockInputFlags.HasFlag(InputState.BlockInput.BlockActions);
		}
		set
		{
			this._blockInputFlags = ((this._blockInputFlags & ~InputState.BlockInput.BlockActions) | (value ? InputState.BlockInput.BlockActions : InputState.BlockInput.AllowEverything));
			this._playerActionController.UpdateBlockFlags(this._blockInputFlags);
		}
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x00019E4A File Offset: 0x0001804A
	public static bool DeviceInputTypeRequiresFocus(DeviceInputType type)
	{
		return type == DeviceInputType.Controller || type == DeviceInputType.Remote;
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x06000853 RID: 2131 RVA: 0x00019E56 File Offset: 0x00018056
	// (set) Token: 0x06000854 RID: 2132 RVA: 0x00019E5E File Offset: 0x0001805E
	public bool CurrentInputTypeRequiresFocus { get; set; }

	// Token: 0x06000855 RID: 2133 RVA: 0x00019E68 File Offset: 0x00018068
	public void Tick(float appTime)
	{
		this._mouse.Tick(appTime);
		IPointerState[] touches = this._touches;
		for (int i = 0; i < touches.Length; i++)
		{
			touches[i].Tick(appTime);
		}
		foreach (ButtonState buttonState in this._keys.Values)
		{
			buttonState.Tick(appTime);
		}
		foreach (AxisState axisState in this._axis.Values)
		{
			axisState.Tick(appTime);
		}
		if (this._unblockActionsNextTick)
		{
			this.BlockActions = false;
			this._unblockActionsNextTick = false;
		}
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x00019F44 File Offset: 0x00018144
	private void UpdateCurrentInputDevice(InputEvent lastInputEvent)
	{
		switch (lastInputEvent.Source)
		{
		case InputEventSource.Mouse:
			if (lastInputEvent.ButtonState == InputEventButtonState.JustDown)
			{
				this.CurrentDeviceInputType = DeviceInputType.Mouse;
				return;
			}
			break;
		case InputEventSource.Touch:
			this.CurrentDeviceInputType = DeviceInputType.Touch;
			return;
		case InputEventSource.Keyboard:
			this.CurrentDeviceInputType = DeviceInputType.Mouse;
			return;
		case InputEventSource.Generic:
			this.CurrentDeviceInputType = DeviceInputType.Controller;
			break;
		case InputEventSource.Remote:
			this.CurrentDeviceInputType = DeviceInputType.Remote;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00019FA3 File Offset: 0x000181A3
	private bool IsMouseButton(InputEvent inputEvent)
	{
		return inputEvent.InputAction == 19 || inputEvent.InputAction == 20 || inputEvent.InputAction == 30;
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x00019FC8 File Offset: 0x000181C8
	public void OnInputEvent(float appTime, InputEvent inputEvent)
	{
		if (inputEvent.ButtonState != InputEventButtonState.None)
		{
			this.LastInputTimestamp = appTime;
		}
		this.UpdateCurrentInputDevice(inputEvent);
		if ((inputEvent.Source == InputEventSource.Mouse || inputEvent.Source == InputEventSource.Touch) && this._onScreenToolManager.IsPointInsideTool(inputEvent.PointerPosition))
		{
			return;
		}
		if (!this.BlockUIInput)
		{
			if (this.IsMouseButton(inputEvent) || inputEvent.InputAction == 23)
			{
				this.UpdateMousePointerState(appTime, inputEvent.PointerPosition, PointerMoveToDeltaBehaviour.CalculateDelta);
				if (this.IsMouseButton(inputEvent) && inputEvent.ButtonState != InputEventButtonState.DoubleTapDown)
				{
					this.UpdateMouseButtonState(appTime, inputEvent.InputAction, inputEvent.ButtonState);
				}
			}
			else if (inputEvent.Source == InputEventSource.Touch)
			{
				PointerMoveToDeltaBehaviour pointerDeltaBehaviour = (inputEvent.ButtonState == InputEventButtonState.JustDown || inputEvent.ButtonState == InputEventButtonState.JustUp) ? PointerMoveToDeltaBehaviour.ResetDelta : PointerMoveToDeltaBehaviour.CalculateDelta;
				this.UpdateTouchPointerState(appTime, inputEvent.SourceIndex, inputEvent.PointerPosition, pointerDeltaBehaviour);
				this.UpdateTouchButtonState(appTime, inputEvent.SourceIndex, inputEvent.ButtonState);
			}
			else if (inputEvent.ButtonState == InputEventButtonState.Axis)
			{
				AxisInputEvent axisInputEvent = (AxisInputEvent)inputEvent;
				this.UpdateAxisState(appTime, axisInputEvent.InputAction, axisInputEvent.AxisValue);
			}
			else
			{
				this.UpdateButtonState(appTime, inputEvent.InputAction, inputEvent.ButtonState);
			}
		}
		if (!this.BlockActions)
		{
			this._playerActionController.OnInputEvent(appTime, inputEvent);
		}
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0001A104 File Offset: 0x00018304
	public bool IsInputEventOverUI(InputEvent inputEvent)
	{
		if (inputEvent is MotorwaysUIInputEvent)
		{
			return true;
		}
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		if (inputEvent.Source == InputEventSource.Mouse)
		{
			eventDataCurrentPosition.position = this.Mouse.Position;
		}
		else
		{
			if (inputEvent.Source != InputEventSource.Touch)
			{
				return false;
			}
			if (inputEvent.ButtonState == InputEventButtonState.Up)
			{
				return false;
			}
			IPointerState pointer;
			if (this.TryGetTouch(inputEvent.SourceIndex, out pointer))
			{
				eventDataCurrentPosition.position = pointer.Position;
			}
		}
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem current = EventSystem.current;
		if (current != null)
		{
			current.RaycastAll(eventDataCurrentPosition, results);
		}
		return results.Count > 0;
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0001A195 File Offset: 0x00018395
	public void OnWindowFocusChanged(bool appHasWindowFocus)
	{
		InputState.Log.Info("Window focus changing from {0} to {1}.", new object[]
		{
			this._appHasWindowFocus,
			appHasWindowFocus
		});
		this._appHasWindowFocus = appHasWindowFocus;
		this.UpdateAppFocus();
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0001A1D0 File Offset: 0x000183D0
	public void OnInternalFocusChanged(bool appHasInternalFocus)
	{
		InputState.Log.Info("Internal focus changing from {0} to {1}.", new object[]
		{
			this._appHasInternalFocus,
			appHasInternalFocus
		});
		this._appHasInternalFocus = appHasInternalFocus;
		this.UpdateAppFocus();
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0001A20B File Offset: 0x0001840B
	public void UpdateButtonState(float appTime, int rewiredInput, InputEventButtonState buttonState)
	{
		if (!this._keys.ContainsKey(rewiredInput))
		{
			this._keys.Add(rewiredInput, this._scope.Get<ButtonState>());
		}
		this._keys[rewiredInput].SetState(appTime, buttonState);
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x0001A245 File Offset: 0x00018445
	public void UpdateMouseButtonState(float appTime, int rewiredAction, InputEventButtonState buttonState)
	{
		this.Mouse.SetButtonState(appTime, rewiredAction, buttonState);
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0001A258 File Offset: 0x00018458
	public void UpdateTouchButtonState(float appTime, int touchIndex, InputEventButtonState buttonState)
	{
		IPointerState pointer;
		if (this.TryGetTouch(touchIndex, out pointer))
		{
			if (touchIndex >= this.MaxRecognizedTouchCount)
			{
				ButtonState currentButtonState = pointer.GetButtonState(0);
				if (currentButtonState.CurrentState == InputEventButtonState.Down)
				{
					buttonState = InputEventButtonState.JustUp;
				}
				else if (currentButtonState.CurrentState == InputEventButtonState.Up)
				{
					buttonState = InputEventButtonState.Up;
				}
			}
			pointer.SetButtonState(appTime, 0, buttonState);
		}
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0001A2A3 File Offset: 0x000184A3
	private void UpdateMousePointerState(float appTime, Vector2 position, PointerMoveToDeltaBehaviour deltaBehaviour = PointerMoveToDeltaBehaviour.CalculateDelta)
	{
		this.Mouse.MoveTo(appTime, position, deltaBehaviour);
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0001A2B4 File Offset: 0x000184B4
	private void UpdateTouchPointerState(float appTime, int touchIndex, Vector2 position, PointerMoveToDeltaBehaviour deltaBehaviour = PointerMoveToDeltaBehaviour.CalculateDelta)
	{
		IPointerState pointer;
		if (this.TryGetTouch(touchIndex, out pointer))
		{
			pointer.MoveTo(appTime, position, deltaBehaviour);
		}
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0001A2D6 File Offset: 0x000184D6
	private void UpdateAxisState(float appTime, int axisName, float newAxisValue)
	{
		if (Diagnostics.Verify(this._axis.ContainsKey(axisName)))
		{
			this._axis[axisName].SetAxisValue(newAxisValue);
		}
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0001A300 File Offset: 0x00018500
	public IPointerState GetPointerFromInputEvent(InputEvent inputEvent)
	{
		if (inputEvent.Source == InputEventSource.Mouse)
		{
			return this.Mouse;
		}
		if (inputEvent.Source == InputEventSource.Touch)
		{
			IPointerState pointer;
			this.TryGetTouch(inputEvent.SourceIndex, out pointer);
			return pointer;
		}
		if (inputEvent.Source == InputEventSource.Keyboard)
		{
			return this.Mouse;
		}
		return null;
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0001A348 File Offset: 0x00018548
	public ButtonState GetButtonFromInputEvent(InputEvent inputEvent)
	{
		if (inputEvent.Source == InputEventSource.Keyboard)
		{
			ButtonState state;
			if (this._keys.TryGetValue(inputEvent.InputAction, out state))
			{
				return state;
			}
			return null;
		}
		else
		{
			IPointerState pointer = this.GetPointerFromInputEvent(inputEvent);
			if (pointer != null)
			{
				int rewiredInputIndex = inputEvent.InputAction;
				if (inputEvent.Source == InputEventSource.Touch)
				{
					rewiredInputIndex = 0;
				}
				return pointer.GetButtonState(rewiredInputIndex);
			}
			return null;
		}
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x06000864 RID: 2148 RVA: 0x0001A39D File Offset: 0x0001859D
	protected ObserverList<InputState.IObserver> Observers
	{
		get
		{
			return this._observers;
		}
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0001A3A5 File Offset: 0x000185A5
	public void Subscribe(InputState.IObserver observer)
	{
		this._observers.Subscribe(observer);
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0001A3B3 File Offset: 0x000185B3
	public bool Unsubscribe(InputState.IObserver observer)
	{
		return this._observers.Unsubscribe(observer);
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0001A3C4 File Offset: 0x000185C4
	private void UpdateAppFocus()
	{
		bool appHasFocus = this._appHasWindowFocus && this._appHasInternalFocus;
		if (appHasFocus != this._appHasFocus)
		{
			InputState.Log.Info("App {0} input focus.", new object[]
			{
				appHasFocus ? "has gained" : "no longer has"
			});
			this._appHasFocus = appHasFocus;
			if (this._appHasFocus)
			{
				this._unblockActionsNextTick = true;
				return;
			}
			this.BlockActions = true;
			this._unblockActionsNextTick = false;
			this._playerActionController.CancelAllActions();
		}
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x000020AA File Offset: 0x000002AA
	public static bool HasAController()
	{
		return true;
	}

	// Token: 0x040003C5 RID: 965
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("Input");

	// Token: 0x040003C6 RID: 966
	private InputState.BlockInput _blockInputFlags;

	// Token: 0x040003C7 RID: 967
	private bool _unblockActionsNextTick;

	// Token: 0x040003C8 RID: 968
	private bool _appHasWindowFocus;

	// Token: 0x040003C9 RID: 969
	private bool _appHasInternalFocus;

	// Token: 0x040003CA RID: 970
	private bool _appHasFocus;

	// Token: 0x040003CB RID: 971
	private const int DefaultMaxTouchCount = 4;

	// Token: 0x040003CC RID: 972
	private const int DefaultMaxMouseButtonCount = 3;

	// Token: 0x040003CF RID: 975
	[Dependency]
	private PlayerActionController _playerActionController;

	// Token: 0x040003D0 RID: 976
	[Dependency]
	private readonly IPointerState _mouse;

	// Token: 0x040003D1 RID: 977
	[Dependency]
	private readonly IPointerState[] _touches = new IPointerState[4];

	// Token: 0x040003D2 RID: 978
	[Dependency]
	private IHardwareCapabilities _hardware;

	// Token: 0x040003D3 RID: 979
	[Dependency]
	private IScope _scope;

	// Token: 0x040003D4 RID: 980
	private readonly Dictionary<int, ButtonState> _keys = new Dictionary<int, ButtonState>();

	// Token: 0x040003D5 RID: 981
	private readonly Dictionary<int, AxisState> _axis = new Dictionary<int, AxisState>();

	// Token: 0x040003D6 RID: 982
	private readonly List<IController> _controllers = new List<IController>();

	// Token: 0x040003D7 RID: 983
	[Dependency]
	private IOnScreenToolManager _onScreenToolManager;

	// Token: 0x040003D8 RID: 984
	private readonly ObserverList<IControllerConnectionObserver> _controllerConnectionObservers = new ObserverList<IControllerConnectionObserver>(1);

	// Token: 0x040003DB RID: 987
	private DeviceInputType _currentDeviceInputType;

	// Token: 0x040003DD RID: 989
	[Serialize(false, null)]
	private readonly ObserverList<InputState.IObserver> _observers = new ObserverList<InputState.IObserver>(1);

	// Token: 0x040003DE RID: 990
	private static readonly ProfilerMarker Profiler_Tick = new ProfilerMarker("InputSystem.Tick");

	// Token: 0x040003DF RID: 991
	private static readonly ProfilerMarker Profiler_IsInputEventOverUI = new ProfilerMarker("InputSystem.IsInputEventOverUI");

	// Token: 0x02000175 RID: 373
	[Flags]
	public enum BlockInput
	{
		// Token: 0x040003E1 RID: 993
		AllowEverything = 0,
		// Token: 0x040003E2 RID: 994
		BlockUI = 1,
		// Token: 0x040003E3 RID: 995
		BlockGame = 2,
		// Token: 0x040003E4 RID: 996
		BlockActions = 4,
		// Token: 0x040003E5 RID: 997
		BlockEverything = 3
	}

	// Token: 0x02000176 RID: 374
	public interface IObserver
	{
		// Token: 0x0600086B RID: 2155
		void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType);
	}
}
