using System;
using System.Collections.Generic;
using System.Reflection;
using Factory;
using FixMath;
using Motorways.Models;
using Server;

// Token: 0x02000094 RID: 148
public class MotorwaysModelDevToolCommand : BaseInGameDevToolCommand<MotorwaysModelDevToolCommand>
{
	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000229 RID: 553 RVA: 0x00007C98 File Offset: 0x00005E98
	// (set) Token: 0x0600022A RID: 554 RVA: 0x00007CA0 File Offset: 0x00005EA0
	[Dependency]
	public IScope Scope { get; protected set; }

	// Token: 0x0600022B RID: 555 RVA: 0x00007CAC File Offset: 0x00005EAC
	public override void Execute(ISimulation simulation)
	{
		IInGameDevToolsRegistry inGameDevToolsRegistry = this.Scope.Get<IInGameDevToolsRegistry>();
		if (inGameDevToolsRegistry != null)
		{
			IInGameModelDevTool inGameDevTool = inGameDevToolsRegistry.GetModelDevToolByCommandSerializationName(this.commandSerializationName);
			if (Diagnostics.Verify(inGameDevTool != null))
			{
				Action<MotorwaysModelDevToolCommand, ISimulation> actionToExecute = inGameDevTool.GetActionWithCommandType<MotorwaysModelDevToolCommand>();
				if (Diagnostics.Verify(actionToExecute != null))
				{
					actionToExecute(this, simulation);
				}
			}
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x00007CFC File Offset: 0x00005EFC
	public virtual void SyncValuesToModel<ModelType>(ModelType selectedModel)
	{
		if (selectedModel == null)
		{
			return;
		}
		foreach (KeyValuePair<string, bool> currentBoolParam in this.boolParameters)
		{
			if (this.parameterNameToFieldName.ContainsKey(currentBoolParam.Key))
			{
				FieldInfo fieldInfo = typeof(ModelType).GetField(this.parameterNameToFieldName[currentBoolParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo != null)
				{
					fieldInfo.SetValue(selectedModel, currentBoolParam.Value);
				}
				else
				{
					PropertyInfo propertyInfo = typeof(ModelType).GetProperty(this.parameterNameToFieldName[currentBoolParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo != null))
					{
						propertyInfo.SetValue(selectedModel, currentBoolParam.Value);
					}
				}
			}
		}
		foreach (KeyValuePair<string, int> currentIntParam in this.intParameters)
		{
			if (this.parameterNameToFieldName.ContainsKey(currentIntParam.Key))
			{
				FieldInfo fieldInfo2 = typeof(ModelType).GetField(this.parameterNameToFieldName[currentIntParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo2 != null)
				{
					fieldInfo2.SetValue(selectedModel, currentIntParam.Value);
				}
				else
				{
					PropertyInfo propertyInfo2 = typeof(ModelType).GetProperty(this.parameterNameToFieldName[currentIntParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo2 != null))
					{
						propertyInfo2.SetValue(selectedModel, currentIntParam.Value);
					}
				}
			}
		}
		foreach (KeyValuePair<string, string> currentEnumParam in this.enumParameters)
		{
			if (this.parameterNameToFieldName.ContainsKey(currentEnumParam.Key))
			{
				FieldInfo fieldInfo3 = typeof(ModelType).GetField(this.parameterNameToFieldName[currentEnumParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo3 != null)
				{
					object enumValue = null;
					Type enumType = fieldInfo3.FieldType;
					try
					{
						enumValue = Enum.Parse(enumType, currentEnumParam.Value);
					}
					catch (Exception)
					{
						Diagnostics.FailAssert("Failed to parse enum value {0}.{1}", new object[]
						{
							currentEnumParam.Key,
							currentEnumParam.Value
						});
					}
					fieldInfo3.SetValue(selectedModel, enumValue);
				}
				else
				{
					PropertyInfo propertyInfo3 = typeof(ModelType).GetProperty(this.parameterNameToFieldName[currentEnumParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo3 != null))
					{
						object enumValue2 = null;
						Type enumType2 = propertyInfo3.PropertyType;
						if (Diagnostics.Verify(enumType2 != null))
						{
							try
							{
								enumValue2 = Enum.Parse(enumType2, currentEnumParam.Value);
							}
							catch (Exception)
							{
								Diagnostics.FailAssert("Failed to parse enum value {0}.{1}", new object[]
								{
									currentEnumParam.Key,
									currentEnumParam.Value
								});
							}
						}
						propertyInfo3.SetValue(selectedModel, enumValue2);
					}
				}
			}
		}
		foreach (KeyValuePair<string, Fix64> currentFloatParam in this.floatParameters)
		{
			if (this.parameterNameToFieldName.ContainsKey(currentFloatParam.Key))
			{
				FieldInfo fieldInfo4 = typeof(ModelType).GetField(this.parameterNameToFieldName[currentFloatParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo4 != null)
				{
					fieldInfo4.SetValue(selectedModel, currentFloatParam.Value);
				}
				else
				{
					PropertyInfo propertyInfo4 = typeof(ModelType).GetProperty(this.parameterNameToFieldName[currentFloatParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo4 != null))
					{
						propertyInfo4.SetValue(selectedModel, currentFloatParam.Value);
					}
				}
			}
		}
		foreach (KeyValuePair<string, string> currentStringParam in this.stringParameters)
		{
			if (this.parameterNameToFieldName.ContainsKey(currentStringParam.Key))
			{
				FieldInfo fieldInfo5 = typeof(ModelType).GetField(this.parameterNameToFieldName[currentStringParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (fieldInfo5 != null)
				{
					fieldInfo5.SetValue(selectedModel, currentStringParam.Value);
				}
				else
				{
					PropertyInfo propertyInfo5 = typeof(ModelType).GetProperty(this.parameterNameToFieldName[currentStringParam.Key], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (Diagnostics.Verify(propertyInfo5 != null))
					{
						propertyInfo5.SetValue(selectedModel, currentStringParam.Value);
					}
				}
			}
		}
	}

	// Token: 0x040000CD RID: 205
	[Dependency]
	protected CityPlanModel _cityPlanModel;

	// Token: 0x040000CE RID: 206
	[Dependency]
	protected ClockModel _clock;

	// Token: 0x040000CF RID: 207
	[Dependency]
	protected ISimulation _simulation;
}
