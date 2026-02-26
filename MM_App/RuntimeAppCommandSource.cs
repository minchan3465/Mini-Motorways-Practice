using System;
using System.Collections.Generic;
using Factory;
using Rewired;
using UnityEngine;

// Token: 0x0200023A RID: 570
public class RuntimeAppCommandSource : IAppCommandSource
{
	// Token: 0x06000D79 RID: 3449 RVA: 0x0002C4E4 File Offset: 0x0002A6E4
	public void Start()
	{
		this._appHasFocus = true;
		this._hasAppFocusChanged = false;
		this._rewiredPlayer = ReInput.players.GetPlayer(0);
		ReInput.ControllerConnectedEvent += this.OnRewiredControllerConnected;
		ReInput.controllers.AddLastActiveControllerChangedDelegate(new ActiveControllerChangedDelegate(this.OnLastActiveControllerChanged));
		new GameObject("FocusListener").AddComponent<ApplicationFocusListener>().Initialize(this);
		Input.simulateMouseWithTouches = false;
		this._absoluteRealTimeAtStart = Time.time;
		this._absoluteTime = -1f;
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x0002C568 File Offset: 0x0002A768
	public IEnumerable<IAppCommand> GetFrameCommands()
	{
		this._frameCommands.Clear();
		float absoluteAppTime = Time.time - this._absoluteRealTimeAtStart;
		if (!this.hasInitialized)
		{
			this.hasInitialized = true;
			ConfigureDeviceCommand configureDeviceCommand = this._scope.Get<ConfigureDeviceCommand>();
			configureDeviceCommand.Initialize();
			this._frameCommands.Add(configureDeviceCommand);
			InitRandomCommand randomCommand = this._scope.Get<InitRandomCommand>();
			randomCommand.Configure((uint)(global::Random.Double() * 4294967295.0));
			this._frameCommands.Add(randomCommand);
		}
		if (this._hasAppFocusChanged)
		{
			this._hasAppFocusChanged = false;
			ChangeWindowFocusCommand windowFocusCommand = this._scope.Get<ChangeWindowFocusCommand>();
			windowFocusCommand.Configure(this._appHasFocus);
			this._frameCommands.Add(windowFocusCommand);
		}
		foreach (Touch touch in Input.touches)
		{
			InputEventButtonState newState = this.TouchPhaseToButtonState(touch.phase);
			InputEvent inputEvent = InputEvent.CreateTouchEvent(this._scope, touch.fingerId, newState, touch.position);
			ProcessInputEventCommand command = this._scope.Get<ProcessInputEventCommand>();
			command.Configure(absoluteAppTime, inputEvent);
			this._frameCommands.Add(command);
			if (newState == InputEventButtonState.JustDown)
			{
				this._framesSinceLastTouch = 0;
			}
		}
		bool mouseMocksTouchInput = FeatureToggle.IsFeatureEnabled(Feature.MockPhone);
		bool ignoreMouse = this._framesSinceLastTouch <= 30;
		ignoreMouse = (ignoreMouse || mouseMocksTouchInput);
		if (Input.mousePresent)
		{
			Vector2 newMousePosition = this._rewiredPlayer.controllers.Mouse.screenPosition;
			Vector2 oldMousePosition = this.inputState.Mouse.Position;
			if (!Mathf.Approximately(oldMousePosition.x, newMousePosition.x) || !Mathf.Approximately(oldMousePosition.y, newMousePosition.y))
			{
				if (mouseMocksTouchInput && this._rewiredPlayer.controllers.Mouse.GetButton(0) && !this._rewiredPlayer.controllers.Mouse.GetButtonDown(0))
				{
					InputEvent inputEvent2 = InputEvent.CreateTouchEvent(this._scope, 0, InputEventButtonState.Down, newMousePosition);
					ProcessInputEventCommand command2 = this._scope.Get<ProcessInputEventCommand>();
					command2.Configure(absoluteAppTime, inputEvent2);
					this._frameCommands.Add(command2);
					this._framesSinceLastTouch = 0;
				}
				InputEvent mousePositionEvent = InputEvent.CreateMouseEvent(this._scope, 23, InputEventButtonState.None, newMousePosition);
				ProcessInputEventCommand mousePositionCommand = this._scope.Get<ProcessInputEventCommand>();
				mousePositionCommand.Configure(absoluteAppTime, mousePositionEvent);
				this._frameCommands.Add(mousePositionCommand);
			}
		}
		if (this.inputState.AxisToPoll != null)
		{
			foreach (int axisName in this.inputState.AxisToPoll)
			{
				if (this.inputState.AxisToIgnore == null || !this.inputState.AxisToIgnore.Contains(axisName))
				{
					float currentAxisValue = this._rewiredPlayer.GetAxis(axisName);
					if (!Mathf.Approximately(this.inputState.GetAxis(axisName), currentAxisValue))
					{
						AxisInputEvent inputEvent3 = AxisInputEvent.CreateAxisEvent(this._scope, axisName, currentAxisValue, this._lastActiveInputSource);
						ProcessInputEventCommand command3 = this._scope.Get<ProcessInputEventCommand>();
						command3.Configure(absoluteAppTime, inputEvent3);
						this._frameCommands.Add(command3);
					}
				}
			}
		}
		if (this.inputState.InputActionsToPoll != null)
		{
			HashSet<int> inputActionsToIgnore = this.inputState.InputActionsToIgnore;
			foreach (int inputAction in this.inputState.InputActionsToPoll)
			{
				if (inputActionsToIgnore == null || !inputActionsToIgnore.Contains(inputAction))
				{
					if (this._rewiredPlayer.GetButtonDown(inputAction))
					{
						InputEvent inputEvent4 = this.CreateInputEventFor(inputAction, InputEventButtonState.JustDown, this._lastActiveInputSource);
						if (inputEvent4.Source != InputEventSource.Mouse || !ignoreMouse)
						{
							ProcessInputEventCommand command4 = this._scope.Get<ProcessInputEventCommand>();
							command4.Configure(absoluteAppTime, inputEvent4);
							this._frameCommands.Add(command4);
						}
					}
					if (this._rewiredPlayer.GetButtonDoublePressDown(inputAction))
					{
						InputEvent inputEvent5 = this.CreateInputEventFor(inputAction, InputEventButtonState.DoubleTapDown, this._lastActiveInputSource);
						if (inputEvent5.Source != InputEventSource.Mouse || !ignoreMouse)
						{
							ProcessInputEventCommand command5 = this._scope.Get<ProcessInputEventCommand>();
							command5.Configure(absoluteAppTime, inputEvent5);
							this._frameCommands.Add(command5);
						}
					}
					if (this._rewiredPlayer.GetButtonUp(inputAction))
					{
						InputEvent inputEvent6 = this.CreateInputEventFor(inputAction, InputEventButtonState.JustUp, this._lastActiveInputSource);
						if (inputEvent6.Source != InputEventSource.Mouse || !ignoreMouse)
						{
							ProcessInputEventCommand command6 = this._scope.Get<ProcessInputEventCommand>();
							command6.Configure(absoluteAppTime, inputEvent6);
							this._frameCommands.Add(command6);
						}
					}
				}
			}
		}
		if (mouseMocksTouchInput)
		{
			InputEventButtonState state = InputEventButtonState.None;
			if (this._rewiredPlayer.controllers.Mouse.GetButtonDown(0))
			{
				state = InputEventButtonState.JustDown;
			}
			else if (this._rewiredPlayer.controllers.Mouse.GetButtonUp(0))
			{
				state = InputEventButtonState.JustUp;
			}
			else if (this._rewiredPlayer.controllers.Mouse.GetButton(0))
			{
				state = InputEventButtonState.Down;
			}
			if (state != InputEventButtonState.None)
			{
				InputEvent inputEvent7 = InputEvent.CreateTouchEvent(this._scope, 0, state, this._rewiredPlayer.controllers.Mouse.screenPosition);
				ProcessInputEventCommand command7 = this._scope.Get<ProcessInputEventCommand>();
				command7.Configure(absoluteAppTime, inputEvent7);
				this._frameCommands.Add(command7);
			}
		}
		TickAppCommand tickCommand = this._scope.Get<TickAppCommand>();
		float frameTime = 0f;
		float timescale = 1f;
		if (this._absoluteTime >= 0f)
		{
			frameTime = (absoluteAppTime - this._absoluteTime) * timescale;
		}
		this._absoluteTime = absoluteAppTime;
		tickCommand.Configure(absoluteAppTime, frameTime);
		this._frameCommands.Add(tickCommand);
		this._framesSinceLastTouch++;
		return this._frameCommands;
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x0002CB38 File Offset: 0x0002AD38
	public static InputEventSource GetSourceForController(Controller controller)
	{
		ApplicationFocusListener.LastKnownController = controller;
		if (controller.type == ControllerType.Keyboard || controller.type == ControllerType.Mouse)
		{
			Diagnostics.Log.Info("MotorwaysInputEvent", string.Format("GetSourceForController {0}", controller.type), Array.Empty<object>());
			return InputEventSource.Keyboard;
		}
		if (FeatureToggle.IsFeatureEnabled(Feature.MockControllerAsRemote))
		{
			return InputEventSource.Remote;
		}
		return InputEventSource.Generic;
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x0002CB90 File Offset: 0x0002AD90
	private InputEvent CreateInputEventFor(int inputAction, InputEventButtonState state, InputEventSource source)
	{
		if (inputAction == 19 || inputAction == 20 || inputAction == 30)
		{
			return InputEvent.CreateMouseEvent(this._scope, inputAction, state, this._rewiredPlayer.controllers.Mouse.screenPosition);
		}
		return InputEvent.CreateEvent(this._scope, inputAction, state, source);
	}

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06000D7D RID: 3453 RVA: 0x0002CBDD File Offset: 0x0002ADDD
	// (set) Token: 0x06000D7E RID: 3454 RVA: 0x0002CBE5 File Offset: 0x0002ADE5
	public bool AppHasFocus
	{
		get
		{
			return this._appHasFocus;
		}
		set
		{
			if (this._appHasFocus != value)
			{
				this._appHasFocus = value;
				this._hasAppFocusChanged = true;
			}
		}
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x0002CBFE File Offset: 0x0002ADFE
	public void SetRewiredMode(int mode)
	{
		this._currentControllerMapCategory = mode;
		if (this._rewiredPlayer != null)
		{
			this._rewiredPlayer.controllers.maps.SetAllMapsEnabled(false);
			this._rewiredPlayer.controllers.maps.SetMapsEnabled(true, mode);
		}
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x0002CC3E File Offset: 0x0002AE3E
	private void OnRewiredControllerConnected(ControllerStatusChangedEventArgs eventArgs)
	{
		this.SetRewiredMode(this._currentControllerMapCategory);
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x0002CC4C File Offset: 0x0002AE4C
	private void OnLastActiveControllerChanged(Controller newController)
	{
		this._lastActiveInputSource = RuntimeAppCommandSource.GetSourceForController(newController);
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x0002CC5A File Offset: 0x0002AE5A
	private InputEventButtonState TouchPhaseToButtonState(TouchPhase phase)
	{
		switch (phase)
		{
		case TouchPhase.Began:
			return InputEventButtonState.JustDown;
		case TouchPhase.Moved:
			return InputEventButtonState.Down;
		case TouchPhase.Stationary:
			return InputEventButtonState.Down;
		case TouchPhase.Ended:
			return InputEventButtonState.JustUp;
		case TouchPhase.Canceled:
			return InputEventButtonState.Up;
		default:
			return InputEventButtonState.None;
		}
	}

	// Token: 0x040007A9 RID: 1961
	[Dependency]
	private Scope _scope;

	// Token: 0x040007AA RID: 1962
	private List<IAppCommand> _frameCommands = new List<IAppCommand>(8);

	// Token: 0x040007AB RID: 1963
	private float _absoluteRealTimeAtStart;

	// Token: 0x040007AC RID: 1964
	[Dependency]
	private InputState inputState;

	// Token: 0x040007AD RID: 1965
	private float _absoluteTime = -1f;

	// Token: 0x040007AE RID: 1966
	private bool hasInitialized;

	// Token: 0x040007AF RID: 1967
	private bool _appHasFocus;

	// Token: 0x040007B0 RID: 1968
	private bool _hasAppFocusChanged;

	// Token: 0x040007B1 RID: 1969
	private Rewired.Player _rewiredPlayer;

	// Token: 0x040007B2 RID: 1970
	private int _currentControllerMapCategory;

	// Token: 0x040007B3 RID: 1971
	private InputEventSource _lastActiveInputSource;

	// Token: 0x040007B4 RID: 1972
	private int _framesSinceLastTouch;

	// Token: 0x040007B5 RID: 1973
	private const int NumberOfFramesToIgnoreMouseAfterTouch = 30;

	// Token: 0x040007B6 RID: 1974
	private static bool _wasTouchingDs4Touchpad = false;

	// Token: 0x040007B7 RID: 1975
	private static readonly Guid SiriRemoteGuid = new Guid("bc043dba-df07-4135-929c-5b4398d29579");
}
