using System;
using System.Reflection;
using FixMath;
using Server;
using UnityEngine;

// Token: 0x02000091 RID: 145
public abstract class MotorwaysModelDevTool<ModelType, DevToolType> : MotorwaysSharedDevTool<DevToolType, MotorwaysModelDevToolCommand>, IInGameModelDevTool, IInGameDevTool where ModelType : class, IModel where DevToolType : MotorwaysModelDevTool<ModelType, DevToolType>
{
	// Token: 0x0600020C RID: 524 RVA: 0x000075ED File Offset: 0x000057ED
	public virtual ToolModelType GetToolModelType()
	{
		return this._toolModelType;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x000075F5 File Offset: 0x000057F5
	[Obsolete("If you're using a MotorwaysModelDevTool you should use SetModelCommandToExecute()", true)]
	public override DevToolType SetCommandToExecute(Action<MotorwaysModelDevToolCommand, ISimulation> newCommand)
	{
		throw new InvalidOperationException("If you're using a MotorwaysModelDevTool you should use SetModelCommandToExecute()");
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnKeyPressed(KeyCode keyCode)
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x0600020F RID: 527 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnMouseButtonDown(int buttonIndex)
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnLeftMouseButtonDown()
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnRightMouseButtonDown()
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnControllerLogicalAction(string logicalAction)
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000213 RID: 531 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnDefaultActionInput()
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnInGameHotkey(KeyCode hotkey, KeyCode modifierHotKey = KeyCode.None)
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnInGameHotkeyCustomSetup(KeyCode hotkey, Action<MotorwaysModelDevToolCommand, ISimulation> customSetup)
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000216 RID: 534 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public virtual DevToolType ActivateOnInGameHotkeyWithIntParameter(KeyCode hotkey, int parameterValue, string parameterName)
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00007601 File Offset: 0x00005801
	[Obsolete("In model tools, all input and activation code is handled internally.", true)]
	public override DevToolType ActivateOnEditorButton(string buttonText)
	{
		throw new InvalidOperationException("In model tools, all input and activation code is handled internally.");
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000760D File Offset: 0x0000580D
	public virtual DevToolType SetModelCommandToExecute(Action<MotorwaysModelDevToolCommand, ModelType, ISimulation> newCommand)
	{
		this.modelCommandToExecute = newCommand;
		return (DevToolType)((object)this);
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000761C File Offset: 0x0000581C
	public virtual DevToolType SetOnModelSelectedCommandToExecute(Action<MotorwaysModelDevTool<ModelType, DevToolType>, ModelType> newCommand)
	{
		this.onSelectedModelCommandToExecute = newCommand;
		return (DevToolType)((object)this);
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000762B File Offset: 0x0000582B
	public MotorwaysModelDevTool()
	{
		this.mouseButtonIndicies.Add(0);
		this.editorButtonText = "Apply";
		this.activateOnEditorButtonPress = true;
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x0600021B RID: 539 RVA: 0x00007651 File Offset: 0x00005851
	public ModelType SelectedModel
	{
		get
		{
			return this.selectedModel;
		}
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00007659 File Offset: 0x00005859
	public override Command GenerateCommand(bool useDefaultParameterValues = false)
	{
		MotorwaysModelDevToolCommand motorwaysModelDevToolCommand = (MotorwaysModelDevToolCommand)base.GenerateCommand(useDefaultParameterValues);
		motorwaysModelDevToolCommand.SetEnumParameter<ToolModelType>("ToolModelType", this._toolModelType);
		motorwaysModelDevToolCommand.cursorTilePosition = this.selectedModelCoordinates;
		return motorwaysModelDevToolCommand;
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00007684 File Offset: 0x00005884
	public override Action<CommandType, ISimulation> GetActionWithCommandType<CommandType>()
	{
		if (typeof(CommandType).IsAssignableFrom(typeof(MotorwaysModelDevToolCommand)))
		{
			return (Action<CommandType, ISimulation>)new Action<MotorwaysModelDevToolCommand, ISimulation>(this.CallUserFunction);
		}
		return null;
	}

	// Token: 0x0600021E RID: 542 RVA: 0x000076B4 File Offset: 0x000058B4
	protected void CallUserFunction(MotorwaysModelDevToolCommand modelDevToolCommand, ISimulation simulation)
	{
		ToolModelType toolModelType;
		ModelType targetModel;
		if (Diagnostics.Verify(modelDevToolCommand.TryGetEnumParameter<ToolModelType>("ToolModelType", out toolModelType), "We should always have the ToolModelType parameter in a MotorwaysModelDevTool!") && this.TryGetModelAtCoordinates(modelDevToolCommand.cursorTilePosition, out targetModel))
		{
			modelDevToolCommand.SyncValuesToModel<ModelType>(targetModel);
			if (this.modelCommandToExecute != null)
			{
				this.modelCommandToExecute(modelDevToolCommand, targetModel, simulation);
			}
		}
	}

	// Token: 0x0600021F RID: 543
	protected abstract bool TryGetModelAtCoordinates(Vector2Int modelCoordinates, out ModelType foundModel);

	// Token: 0x06000220 RID: 544 RVA: 0x00007707 File Offset: 0x00005907
	public void OnModelActivation()
	{
		this.OnActivation();
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00007710 File Offset: 0x00005910
	protected override void OnActivation()
	{
		if (this.wasButtonPressed)
		{
			if (this.clientCodeToExecute != null)
			{
				this.clientCodeToExecute((DevToolType)((object)this), this._simulation);
			}
			this._simulation.ScheduleCommand(this.GenerateCommand(false));
			this.wasButtonPressed = false;
			return;
		}
		this.AttemptToSelectModelUnderCursor();
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00007768 File Offset: 0x00005968
	protected virtual void AttemptToSelectModelUnderCursor()
	{
		Vector2Int newModelCoordinates = this._tilemapView.GetMouseTilePosition();
		ModelType newModel;
		if (this.TryGetModelAtCoordinates(newModelCoordinates, out newModel))
		{
			this.selectedModel = newModel;
			this.selectedModelCoordinates = newModelCoordinates;
			this.SyncValuesFromModel();
			this.SelectedNewModel();
			this.gameScope.Get<InGameDevToolsRegistry>().UpdateEditorIfPresent();
		}
	}

	// Token: 0x06000223 RID: 547 RVA: 0x000077B6 File Offset: 0x000059B6
	protected virtual void SelectedNewModel()
	{
		if (this.onSelectedModelCommandToExecute != null)
		{
			this.onSelectedModelCommandToExecute(this, this.SelectedModel);
		}
	}

	// Token: 0x06000224 RID: 548 RVA: 0x000077D4 File Offset: 0x000059D4
	protected virtual void SyncValuesFromModel()
	{
		if (this.selectedModel == null)
		{
			return;
		}
		foreach (IngameDevToolBoolParameter currentBoolParam in this.boolParameters)
		{
			if (!string.IsNullOrEmpty(currentBoolParam.ModelParameterFieldName))
			{
				FieldInfo fieldInfo = typeof(ModelType).GetField(currentBoolParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo != null)
				{
					bool currentValue = (bool)fieldInfo.GetValue(this.selectedModel);
					currentBoolParam.SetValue(currentValue);
				}
				else
				{
					PropertyInfo propertyInfo = typeof(ModelType).GetProperty(currentBoolParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo != null))
					{
						bool currentValue2 = (bool)propertyInfo.GetValue(this.selectedModel);
						currentBoolParam.SetValue(currentValue2);
					}
				}
			}
		}
		foreach (IngameDevToolIntParameter currentIntParam in this.intParameters)
		{
			if (!string.IsNullOrEmpty(currentIntParam.ModelParameterFieldName))
			{
				FieldInfo fieldInfo2 = typeof(ModelType).GetField(currentIntParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo2 != null)
				{
					int currentValue3 = (int)fieldInfo2.GetValue(this.selectedModel);
					currentIntParam.SetValue(currentValue3);
				}
				else
				{
					PropertyInfo propertyInfo2 = typeof(ModelType).GetProperty(currentIntParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo2 != null))
					{
						int currentValue4 = (int)propertyInfo2.GetValue(this.selectedModel);
						currentIntParam.SetValue(currentValue4);
					}
				}
			}
		}
		foreach (IInGameDevToolEnumParameter inGameDevToolEnumParameter in this.enumParameters)
		{
			inGameDevToolEnumParameter.UpdateParameterValueFromModelField<ModelType>(this.selectedModel);
		}
		foreach (IngameDevToolFloatParameter currentFloatParam in this.floatParameters)
		{
			if (!string.IsNullOrEmpty(currentFloatParam.ModelParameterFieldName))
			{
				FieldInfo fieldInfo3 = typeof(ModelType).GetField(currentFloatParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo3 != null)
				{
					Fix64 currentValue5 = (Fix64)fieldInfo3.GetValue(this.selectedModel);
					currentFloatParam.SetValue(currentValue5);
				}
				else
				{
					PropertyInfo propertyInfo3 = typeof(ModelType).GetProperty(currentFloatParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo3 != null))
					{
						Fix64 currentValue6 = (Fix64)propertyInfo3.GetValue(this.selectedModel);
						currentFloatParam.SetValue(currentValue6);
					}
				}
			}
		}
		foreach (IngameDevToolStringParameter currentStringParam in this.stringParameters)
		{
			if (!string.IsNullOrEmpty(currentStringParam.ModelParameterFieldName))
			{
				FieldInfo fieldInfo4 = typeof(ModelType).GetField(currentStringParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo4 != null)
				{
					string currentValue7 = (string)fieldInfo4.GetValue(this.selectedModel);
					currentStringParam.SetValue(currentValue7);
				}
				else
				{
					PropertyInfo propertyInfo4 = typeof(ModelType).GetProperty(currentStringParam.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo4 != null))
					{
						string currentValue8 = (string)propertyInfo4.GetValue(this.selectedModel);
						currentStringParam.SetValue(currentValue8);
					}
				}
			}
		}
	}

	// Token: 0x040000C8 RID: 200
	protected Action<MotorwaysModelDevToolCommand, ModelType, ISimulation> modelCommandToExecute;

	// Token: 0x040000C9 RID: 201
	protected Action<MotorwaysModelDevTool<ModelType, DevToolType>, ModelType> onSelectedModelCommandToExecute;

	// Token: 0x040000CA RID: 202
	protected ToolModelType _toolModelType;

	// Token: 0x040000CB RID: 203
	protected Vector2Int selectedModelCoordinates;

	// Token: 0x040000CC RID: 204
	protected ModelType selectedModel;
}
