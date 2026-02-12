using System;
using System.Collections.Generic;
using Client;
using Factory;
using Factory.Pools;
using Motorways;
using Motorways.Themes;
using Motorways.Views;
using Server;
using UnityEngine;

// Token: 0x0200007F RID: 127
public abstract class BaseInGameDevTool<DevToolType, CommandType> : IInGameDevTool, IReusable where DevToolType : BaseInGameDevTool<DevToolType, CommandType> where CommandType : BaseInGameDevToolCommand<CommandType>
{
	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000154 RID: 340 RVA: 0x00004F41 File Offset: 0x00003141
	// (set) Token: 0x06000155 RID: 341 RVA: 0x00004F49 File Offset: 0x00003149
	public bool ResetToNoneAfterUse { get; set; }

	// Token: 0x06000157 RID: 343 RVA: 0x00004FEB File Offset: 0x000031EB
	public IInGameDevTool SetCommandSerializationName(string newCommandSerializationName)
	{
		this.commandSerializationName = newCommandSerializationName;
		return this;
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00004FF5 File Offset: 0x000031F5
	public DevToolType SetEditorDisplayName(string newDisplayName)
	{
		this.editorDisplayName = newDisplayName;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00005004 File Offset: 0x00003204
	public DevToolType SetEditorIconPath(string newDisplayName)
	{
		this.editorIconPath = newDisplayName;
		return (DevToolType)((object)this);
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00005013 File Offset: 0x00003213
	public virtual DevToolType ActivateOnKeyPressed(KeyCode keyCode)
	{
		if (!this.keyCodes.Contains(keyCode))
		{
			this.keyCodes.Add(keyCode);
		}
		return (DevToolType)((object)this);
	}

	// Token: 0x0600015B RID: 347 RVA: 0x00005035 File Offset: 0x00003235
	public virtual DevToolType ActivateOnMouseButtonDown(int buttonIndex)
	{
		if (!this.mouseButtonIndicies.Contains(buttonIndex))
		{
			this.mouseButtonIndicies.Add(buttonIndex);
		}
		return (DevToolType)((object)this);
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00005057 File Offset: 0x00003257
	public virtual DevToolType ActivateOnLeftMouseButtonDown()
	{
		return this.ActivateOnMouseButtonDown(19);
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00005061 File Offset: 0x00003261
	public virtual DevToolType ActivateOnRightMouseButtonDown()
	{
		return this.ActivateOnMouseButtonDown(20);
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000506B File Offset: 0x0000326B
	public virtual DevToolType ActivateOnControllerLogicalAction(string logicalAction)
	{
		if (!this.controllerLogicalActions.Contains(logicalAction))
		{
			this.controllerLogicalActions.Add(logicalAction);
		}
		return (DevToolType)((object)this);
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000508D File Offset: 0x0000328D
	public virtual DevToolType ActivateOnDefaultActionInput()
	{
		return this.ActivateOnLeftMouseButtonDown().ActivateOnControllerLogicalAction("ActivateSelected");
	}

	// Token: 0x06000160 RID: 352 RVA: 0x000050A4 File Offset: 0x000032A4
	public virtual DevToolType ActivateOnInGameHotkey(KeyCode hotkey, KeyCode modifierKey = KeyCode.None)
	{
		this.hotkeyKeycode = hotkey;
		this.onHotkeyPressedCustomSetup = null;
		this.modifierKeycode = modifierKey;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000161 RID: 353 RVA: 0x000050C1 File Offset: 0x000032C1
	public virtual DevToolType ActivateOnInGameHotkeyCustomSetup(KeyCode hotkey, Action<CommandType, ISimulation> customSetup)
	{
		this.hotkeyKeycode = hotkey;
		this.onHotkeyPressedCustomSetup = customSetup;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000162 RID: 354 RVA: 0x000050D7 File Offset: 0x000032D7
	public virtual DevToolType ActivateOnInGameHotkeyWithIntParameter(KeyCode hotkey, string parameterName, int parameterValue)
	{
		if (Diagnostics.Verify(!this.keyCodesToIntParameters.ContainsKey(hotkey)))
		{
			this.keyCodesToIntParameters.Add(hotkey, new ValueTuple<string, int>(parameterName, parameterValue));
		}
		return (DevToolType)((object)this);
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00005108 File Offset: 0x00003308
	public virtual DevToolType ActivateOnEditorButton(string buttonText)
	{
		this.activateOnEditorButtonPress = true;
		this.editorButtonText = buttonText;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000511E File Offset: 0x0000331E
	public virtual DevToolType DefaultToResettingToNoneAfterUse()
	{
		this.ResetToNoneAfterUse = true;
		this.defaultsToNoneResetAfterUse = true;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000165 RID: 357 RVA: 0x00005134 File Offset: 0x00003334
	public DevToolType SetClientCodeToExecute(Action<DevToolType, ISimulation> newClientDelegate)
	{
		this.clientCodeToExecute = newClientDelegate;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000166 RID: 358 RVA: 0x00005143 File Offset: 0x00003343
	public virtual DevToolType SetCommandToExecute(Action<CommandType, ISimulation> newCommand)
	{
		this.commandToExecute = newCommand;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000167 RID: 359 RVA: 0x00005152 File Offset: 0x00003352
	public virtual DevToolType ShowGridWhenActive()
	{
		this.showGridWhenActive = true;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000168 RID: 360 RVA: 0x00005161 File Offset: 0x00003361
	public virtual DevToolType ExecuteOnToolSelected(Action<DevToolType> onToolSelected)
	{
		this.onSelected = onToolSelected;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000169 RID: 361 RVA: 0x00005170 File Offset: 0x00003370
	public virtual DevToolType ExecuteOnToolDeselected(Action<DevToolType> onToolDeselected)
	{
		this.onDeselected = onToolDeselected;
		return (DevToolType)((object)this);
	}

	// Token: 0x0600016A RID: 362 RVA: 0x0000517F File Offset: 0x0000337F
	public virtual DevToolType ExecuteOnHoveredTileChanged(Action<DevToolType, Vector2Int> onNewTileHovered)
	{
		this.onHoveredTileChanged = onNewTileHovered;
		return (DevToolType)((object)this);
	}

	// Token: 0x0600016B RID: 363 RVA: 0x0000518E File Offset: 0x0000338E
	public virtual DevToolType DrawOnTilesUnderCursor(Action<DevToolType, Vector2Int, DebugTileDataViewer> actionToDrawOnTiles)
	{
		this.drawOnTiles = actionToDrawOnTiles;
		return (DevToolType)((object)this);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x0000519D File Offset: 0x0000339D
	public DevToolType WithBoolParam(IngameDevToolBoolParameter boolParameter)
	{
		this.boolParameters.Add(boolParameter);
		this.parameterOrder.Add(InGameDevToolParameterType.Bool);
		return (DevToolType)((object)this);
	}

	// Token: 0x0600016D RID: 365 RVA: 0x000051BD File Offset: 0x000033BD
	public DevToolType WithIntParam(IngameDevToolIntParameter intParameter)
	{
		this.intParameters.Add(intParameter);
		this.parameterOrder.Add(InGameDevToolParameterType.Int);
		return (DevToolType)((object)this);
	}

	// Token: 0x0600016E RID: 366 RVA: 0x000051DD File Offset: 0x000033DD
	public DevToolType WithEnumParam(IInGameDevToolEnumParameter enumParameter)
	{
		this.enumParameters.Add(enumParameter);
		this.parameterOrder.Add(InGameDevToolParameterType.Enum);
		return (DevToolType)((object)this);
	}

	// Token: 0x0600016F RID: 367 RVA: 0x000051FD File Offset: 0x000033FD
	public DevToolType WithFloatParam(IngameDevToolFloatParameter floatParameter)
	{
		this.floatParameters.Add(floatParameter);
		this.parameterOrder.Add(InGameDevToolParameterType.Float);
		return (DevToolType)((object)this);
	}

	// Token: 0x06000170 RID: 368 RVA: 0x0000521D File Offset: 0x0000341D
	public DevToolType WithStringParam(IngameDevToolStringParameter stringParameter)
	{
		this.stringParameters.Add(stringParameter);
		this.parameterOrder.Add(InGameDevToolParameterType.String);
		return (DevToolType)((object)this);
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000523D File Offset: 0x0000343D
	public DevToolType OverrideDrawEditorToolFunction(Action<DevToolType> newDrawEditorTool)
	{
		return (DevToolType)((object)this);
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00005245 File Offset: 0x00003445
	public virtual string GetCommandSerializationName()
	{
		return this.commandSerializationName;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00005250 File Offset: 0x00003450
	public virtual string GetEditorToolDisplayName()
	{
		string result = this.editorDisplayName;
		if (this.hotkeyKeycode != KeyCode.None)
		{
			result = result + " (" + this.GetHotkeyString() + ")";
		}
		return result;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00005284 File Offset: 0x00003484
	public virtual string GetHotkeyString()
	{
		if (this.hotkeyKeycode == KeyCode.None)
		{
			return string.Empty;
		}
		string result = HotkeyDescription.GetHotkeyCharacter(this.hotkeyKeycode);
		if (this.modifierKeycode != KeyCode.None)
		{
			result = HotkeyDescription.GetHotkeyCharacter(this.modifierKeycode) + result;
		}
		return result;
	}

	// Token: 0x06000175 RID: 373 RVA: 0x000052C6 File Offset: 0x000034C6
	public virtual string GetEditorToolDisplayNameWithoutHotkeyCode()
	{
		return this.editorDisplayName;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x000052CE File Offset: 0x000034CE
	public virtual string GetEditorToolIconPath()
	{
		return this.editorIconPath;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x000052D6 File Offset: 0x000034D6
	public IEnumerable<IngameDevToolBoolParameter> BoolParameters()
	{
		return this.boolParameters;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x000052DE File Offset: 0x000034DE
	public IEnumerable<IngameDevToolIntParameter> IntParameters()
	{
		return this.intParameters;
	}

	// Token: 0x06000179 RID: 377 RVA: 0x000052E6 File Offset: 0x000034E6
	public IEnumerable<IInGameDevToolEnumParameter> EnumParameters()
	{
		return this.enumParameters;
	}

	// Token: 0x0600017A RID: 378 RVA: 0x000052EE File Offset: 0x000034EE
	public IEnumerable<IngameDevToolFloatParameter> FloatParameters()
	{
		return this.floatParameters;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x000052F6 File Offset: 0x000034F6
	public IEnumerable<IngameDevToolStringParameter> StringParameters()
	{
		return this.stringParameters;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00005300 File Offset: 0x00003500
	public virtual IngameDevToolBoolParameter GetBoolParameter(string parameterName)
	{
		foreach (IngameDevToolBoolParameter currentParam in this.boolParameters)
		{
			if (currentParam.ParameterName == parameterName)
			{
				return currentParam;
			}
		}
		return null;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00005364 File Offset: 0x00003564
	public virtual IngameDevToolIntParameter GetIntParameter(string parameterName)
	{
		foreach (IngameDevToolIntParameter currentParam in this.intParameters)
		{
			if (currentParam.ParameterName == parameterName)
			{
				return currentParam;
			}
		}
		return null;
	}

	// Token: 0x0600017E RID: 382 RVA: 0x000053C8 File Offset: 0x000035C8
	public virtual IngameDevToolEnumParameter<EnumType> GetEnumParameter<EnumType>(string parameterName) where EnumType : struct
	{
		foreach (IInGameDevToolEnumParameter currentParam in this.enumParameters)
		{
			if (currentParam.ParameterName == parameterName && typeof(IngameDevToolEnumParameter<EnumType>).IsAssignableFrom(currentParam.GetType()))
			{
				return (IngameDevToolEnumParameter<EnumType>)currentParam;
			}
		}
		return null;
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00005448 File Offset: 0x00003648
	public virtual string GetEnumParameterValueAsString(string parameterName)
	{
		foreach (IInGameDevToolEnumParameter currentParam in this.enumParameters)
		{
			if (currentParam.ParameterName == parameterName)
			{
				return currentParam.ParameterSerializationValue;
			}
		}
		Diagnostics.FailAssert("Can't find an enum parameter named {0} on tool {1}.", new object[]
		{
			parameterName,
			this.GetCommandSerializationName()
		});
		return null;
	}

	// Token: 0x06000180 RID: 384 RVA: 0x000054CC File Offset: 0x000036CC
	public virtual IngameDevToolFloatParameter GetFloatParameter(string parameterName)
	{
		foreach (IngameDevToolFloatParameter currentParam in this.floatParameters)
		{
			if (currentParam.ParameterName == parameterName)
			{
				return currentParam;
			}
		}
		return null;
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00005530 File Offset: 0x00003730
	public virtual IngameDevToolStringParameter GetStringParameter(string parameterName)
	{
		foreach (IngameDevToolStringParameter currentParam in this.stringParameters)
		{
			if (currentParam.ParameterName == parameterName)
			{
				return currentParam;
			}
		}
		return null;
	}

	// Token: 0x06000182 RID: 386 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void PrepareTool()
	{
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00005594 File Offset: 0x00003794
	public virtual void Tick(TimeInterval tickTime, float stepAlpha, out bool activatedThisTick)
	{
		Vector2Int newHoveredTile = this._tilemapView.GetMouseTilePosition();
		if (newHoveredTile != this.lastHoveredTile)
		{
			if (this.onHoveredTileChanged != null)
			{
				this.onHoveredTileChanged((DevToolType)((object)this), newHoveredTile);
			}
			if (this.drawOnTiles != null)
			{
				if (this.debugTileDataViewer == null)
				{
					this.debugTileDataViewer = new GameObject("Tile Drawing For " + this.GetCommandSerializationName() + " Tool").AddComponent<DebugTileDataViewer>();
					this.debugTileDataViewer.onlyDrawWhenSelected = false;
				}
				this.drawOnTiles((DevToolType)((object)this), newHoveredTile, this.debugTileDataViewer);
			}
			this.lastHoveredTile = newHoveredTile;
		}
		if (this.showGridWhenActive)
		{
			Color gridColor = (this.gameScope.Get<IThemeDatabase>() as MotorwaysThemeDatabase).GetGlobalColor(ThemedMaterialType.Dark);
			for (int gridLineIndex = -50; gridLineIndex < 50; gridLineIndex += 2)
			{
				Debug.DrawLine(new Vector3((float)(gridLineIndex + 1), -200f), new Vector3((float)(gridLineIndex + 1), 200f), gridColor, 0.1f, false);
				Debug.DrawLine(new Vector3(-200f, (float)(gridLineIndex + 1)), new Vector3(200f, (float)(gridLineIndex + 1)), gridColor, 0.1f, false);
			}
		}
		bool isActivated = this.wasButtonPressed;
		if (!isActivated)
		{
			isActivated = this.TryAssignHotkeyParameters();
		}
		if (!isActivated)
		{
			using (List<KeyCode>.Enumerator enumerator = this.keyCodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Input.GetKeyDown(enumerator.Current))
					{
						isActivated = true;
						break;
					}
				}
			}
		}
		if (!isActivated)
		{
			foreach (string text in this.controllerLogicalActions)
			{
			}
		}
		if (!isActivated)
		{
			IPointerState mousePointer = this._inputState.Mouse;
			foreach (int buttonIndex in this.mouseButtonIndicies)
			{
				if (mousePointer.GetButtonState(buttonIndex).CurrentState == InputEventButtonState.JustDown)
				{
					isActivated = true;
					break;
				}
			}
		}
		if (isActivated)
		{
			this.OnActivation();
		}
		activatedThisTick = isActivated;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x000057C8 File Offset: 0x000039C8
	private bool TryAssignHotkeyParameters()
	{
		foreach (KeyValuePair<KeyCode, ValueTuple<string, int>> keyCodesToIntParameter in this.keyCodesToIntParameters)
		{
			if (Input.GetKeyDown(keyCodesToIntParameter.Key))
			{
				foreach (IngameDevToolIntParameter intParameter in this.intParameters)
				{
					if (intParameter.ParameterName == keyCodesToIntParameter.Value.Item1)
					{
						intParameter.SetValue(keyCodesToIntParameter.Value.Item2);
						return true;
					}
				}
				Diagnostics.FailAssert("Couldn't find an int parameter named {0} which is assigned to {1}!", new object[]
				{
					keyCodesToIntParameter.Value.Item1,
					keyCodesToIntParameter.Key
				});
			}
		}
		return false;
	}

	// Token: 0x06000185 RID: 389 RVA: 0x000058CC File Offset: 0x00003ACC
	protected virtual void OnActivation()
	{
		if (this.clientCodeToExecute != null)
		{
			this.clientCodeToExecute((DevToolType)((object)this), this._simulation);
		}
		if (this.commandToExecute != null)
		{
			this._simulation.ScheduleCommand(this.GenerateCommand(false));
		}
		this.wasButtonPressed = false;
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000591C File Offset: 0x00003B1C
	public virtual Command GenerateCommand(bool useDefaultParameterValues = false)
	{
		CommandType commandType = this.gameScope.Get<CommandType>();
		commandType.InitializeFromDevTool(this, useDefaultParameterValues);
		commandType.commandSerializationName = this.commandSerializationName;
		commandType.commandToExecute = this.commandToExecute;
		commandType.cursorTilePosition = this._tilemapView.GetMouseTilePosition();
		return commandType;
	}

	// Token: 0x06000187 RID: 391 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void CleanupTool()
	{
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000597E File Offset: 0x00003B7E
	public virtual void OnToolSelected()
	{
		if (this.onSelected != null)
		{
			this.onSelected((DevToolType)((object)this));
		}
	}

	// Token: 0x06000189 RID: 393 RVA: 0x00005999 File Offset: 0x00003B99
	public virtual void OnToolDeselected()
	{
		if (this.debugTileDataViewer != null)
		{
			UnityEngine.Object.Destroy(this.debugTileDataViewer.gameObject);
			this.debugTileDataViewer = null;
		}
		if (this.onDeselected != null)
		{
			this.onDeselected((DevToolType)((object)this));
		}
	}

	// Token: 0x0600018A RID: 394 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void DrawEditorTool()
	{
	}

	// Token: 0x0600018B RID: 395 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void DrawBaseEditorTool(DevToolType devTool)
	{
	}

	// Token: 0x0600018C RID: 396 RVA: 0x000059DC File Offset: 0x00003BDC
	public virtual void Reset()
	{
		this.commandSerializationName = "";
		this.editorDisplayName = "";
		this.editorIconPath = "";
		this.keyCodes.Clear();
		this.keyCodesToIntParameters.Clear();
		this.mouseButtonIndicies.Clear();
		this.controllerLogicalActions.Clear();
		this.boolParameters.Clear();
		this.intParameters.Clear();
		this.enumParameters.Clear();
		this.floatParameters.Clear();
		this.stringParameters.Clear();
		this.parameterOrder.Clear();
		this.commandToExecute = null;
		this.clientCodeToExecute = null;
		this.activateOnEditorButtonPress = false;
		this.editorButtonText = "";
		this.wasButtonPressed = false;
		this.hotkeyKeycode = KeyCode.None;
		this.modifierKeycode = KeyCode.None;
		this.onHotkeyPressedCustomSetup = null;
		this.defaultsToNoneResetAfterUse = false;
		this.showGridWhenActive = false;
		this.lastHoveredTile = default(Vector2Int);
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00005ACE File Offset: 0x00003CCE
	public virtual Action<RequestedCommandType, ISimulation> GetActionWithCommandType<RequestedCommandType>()
	{
		if (typeof(RequestedCommandType) == typeof(CommandType))
		{
			return (Action<RequestedCommandType, ISimulation>)this.commandToExecute;
		}
		return null;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00005AF8 File Offset: 0x00003CF8
	public bool InGameHotkeyActivated()
	{
		return Input.GetKeyDown(this.hotkeyKeycode) && (this.modifierKeycode == KeyCode.None || Input.GetKey(this.modifierKeycode));
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00005B1E File Offset: 0x00003D1E
	public bool InGameParameterHotKeyActivated()
	{
		return this.TryAssignHotkeyParameters();
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00005B28 File Offset: 0x00003D28
	public void OnHotkeyActivated(bool useDefaultValues)
	{
		if (this.clientCodeToExecute != null)
		{
			this.clientCodeToExecute((DevToolType)((object)this), this._simulation);
		}
		if (this.commandToExecute != null)
		{
			CommandType newCommand = (CommandType)((object)this.GenerateCommand(useDefaultValues));
			if (this.onHotkeyPressedCustomSetup != null)
			{
				this.onHotkeyPressedCustomSetup(newCommand, this._simulation);
			}
			this._simulation.ScheduleCommand(newCommand);
		}
	}

	// Token: 0x06000191 RID: 401 RVA: 0x00005B95 File Offset: 0x00003D95
	public bool HasHotKey()
	{
		return this.hotkeyKeycode > KeyCode.None;
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00005BA0 File Offset: 0x00003DA0
	public KeyCode GetHotKey()
	{
		return this.hotkeyKeycode;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x00005BA8 File Offset: 0x00003DA8
	public KeyCode GetModifierHotKey()
	{
		return this.modifierKeycode;
	}

	// Token: 0x04000071 RID: 113
	[Dependency]
	public IScope gameScope;

	// Token: 0x04000072 RID: 114
	[Dependency]
	protected InputState _inputState;

	// Token: 0x04000073 RID: 115
	[Dependency]
	protected ISimulation _simulation;

	// Token: 0x04000074 RID: 116
	[Dependency]
	protected TilemapView _tilemapView;

	// Token: 0x04000075 RID: 117
	protected string commandSerializationName;

	// Token: 0x04000076 RID: 118
	protected string editorDisplayName;

	// Token: 0x04000077 RID: 119
	protected string editorIconPath;

	// Token: 0x04000078 RID: 120
	protected List<KeyCode> keyCodes = new List<KeyCode>();

	// Token: 0x04000079 RID: 121
	protected List<int> mouseButtonIndicies = new List<int>();

	// Token: 0x0400007A RID: 122
	protected List<string> controllerLogicalActions = new List<string>();

	// Token: 0x0400007B RID: 123
	protected List<IngameDevToolBoolParameter> boolParameters = new List<IngameDevToolBoolParameter>();

	// Token: 0x0400007C RID: 124
	protected List<IngameDevToolIntParameter> intParameters = new List<IngameDevToolIntParameter>();

	// Token: 0x0400007D RID: 125
	protected List<IInGameDevToolEnumParameter> enumParameters = new List<IInGameDevToolEnumParameter>();

	// Token: 0x0400007E RID: 126
	protected List<IngameDevToolFloatParameter> floatParameters = new List<IngameDevToolFloatParameter>();

	// Token: 0x0400007F RID: 127
	protected List<IngameDevToolStringParameter> stringParameters = new List<IngameDevToolStringParameter>();

	// Token: 0x04000080 RID: 128
	protected List<InGameDevToolParameterType> parameterOrder = new List<InGameDevToolParameterType>();

	// Token: 0x04000081 RID: 129
	protected Action<DevToolType, ISimulation> clientCodeToExecute;

	// Token: 0x04000082 RID: 130
	protected Action<CommandType, ISimulation> commandToExecute;

	// Token: 0x04000083 RID: 131
	protected bool activateOnEditorButtonPress;

	// Token: 0x04000084 RID: 132
	protected string editorButtonText = "";

	// Token: 0x04000085 RID: 133
	protected bool wasButtonPressed;

	// Token: 0x04000086 RID: 134
	protected KeyCode hotkeyKeycode;

	// Token: 0x04000087 RID: 135
	protected KeyCode modifierKeycode;

	// Token: 0x04000088 RID: 136
	protected Action<CommandType, ISimulation> onHotkeyPressedCustomSetup;

	// Token: 0x04000089 RID: 137
	protected Dictionary<KeyCode, ValueTuple<string, int>> keyCodesToIntParameters = new Dictionary<KeyCode, ValueTuple<string, int>>();

	// Token: 0x0400008B RID: 139
	protected bool defaultsToNoneResetAfterUse;

	// Token: 0x0400008C RID: 140
	protected bool showGridWhenActive;

	// Token: 0x0400008D RID: 141
	protected Action<DevToolType> onSelected;

	// Token: 0x0400008E RID: 142
	protected Action<DevToolType, Vector2Int> onHoveredTileChanged;

	// Token: 0x0400008F RID: 143
	protected Action<DevToolType, Vector2Int, DebugTileDataViewer> drawOnTiles;

	// Token: 0x04000090 RID: 144
	protected Action<DevToolType> onDeselected;

	// Token: 0x04000091 RID: 145
	protected Vector2Int lastHoveredTile = Vector2Int.zero;

	// Token: 0x04000092 RID: 146
	protected DebugTileDataViewer debugTileDataViewer;
}
