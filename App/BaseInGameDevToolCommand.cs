using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;
using FixMath;
using Server;
using UnityEngine;

// Token: 0x02000080 RID: 128
public abstract class BaseInGameDevToolCommand<CommandType> : Command, IDeserializedHandler, IReusable where CommandType : BaseInGameDevToolCommand<CommandType>
{
	// Token: 0x06000194 RID: 404 RVA: 0x00005BB0 File Offset: 0x00003DB0
	public override void Execute(ISimulation simulation)
	{
		if (this.commandToExecute == null && !Diagnostics.Verify(this.LoadCommand(), "Failed to lazily load command {0}.", this.commandSerializationName))
		{
			return;
		}
		this.commandToExecute((CommandType)((object)this), simulation);
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00005BE8 File Offset: 0x00003DE8
	public override void Reset()
	{
		base.Reset();
		this.commandSerializationName = "";
		this.cursorPosition = Vector2.zero;
		this.cursorTilePosition = Vector2Int.zero;
		this.deviceInputType = DeviceInputType.Touch;
		this.commandToExecute = null;
		this.boolParameters.Clear();
		this.intParameters.Clear();
		this.enumParameters.Clear();
		this.floatParameters.Clear();
		this.stringParameters.Clear();
		this.parameterNameToFieldName.Clear();
	}

	// Token: 0x06000196 RID: 406 RVA: 0x00005C6C File Offset: 0x00003E6C
	public virtual bool TryGetBoolParameter(string parameterName, out bool result)
	{
		return this.boolParameters.TryGetValue(parameterName, out result);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00005C7B File Offset: 0x00003E7B
	public virtual bool TryGetIntParameter(string parameterName, out int result)
	{
		return this.intParameters.TryGetValue(parameterName, out result);
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00005C8C File Offset: 0x00003E8C
	public virtual bool TryGetEnumParameter<EnumType>(string parameterName, out EnumType result) where EnumType : struct
	{
		string enumValue;
		if (this.enumParameters.TryGetValue(parameterName, out enumValue))
		{
			return Enum.TryParse<EnumType>(enumValue, out result);
		}
		result = default(EnumType);
		return false;
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00005CB9 File Offset: 0x00003EB9
	public virtual bool TryGetFloatParameter(string parameterName, out Fix64 result)
	{
		return this.floatParameters.TryGetValue(parameterName, out result);
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00005CC8 File Offset: 0x00003EC8
	public virtual bool GetStringParameter(string parameterName, out string result)
	{
		return this.stringParameters.TryGetValue(parameterName, out result);
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00005CD8 File Offset: 0x00003ED8
	public virtual void InitializeFromDevTool(IInGameDevTool devTool, bool useDefaultParameterValues)
	{
		this.boolParameters.Clear();
		foreach (IngameDevToolBoolParameter parameter in devTool.BoolParameters())
		{
			this.boolParameters.Add(parameter.ParameterName, useDefaultParameterValues ? parameter.DefaultValue : parameter.ParameterValue);
			if (!string.IsNullOrEmpty(parameter.ModelParameterFieldName) && parameter.ShouldSetValueOnField)
			{
				this.parameterNameToFieldName.Add(parameter.ParameterName, parameter.ModelParameterFieldName);
			}
		}
		this.intParameters.Clear();
		foreach (IngameDevToolIntParameter parameter2 in devTool.IntParameters())
		{
			this.intParameters.Add(parameter2.ParameterName, useDefaultParameterValues ? parameter2.DefaultValue : parameter2.ParameterValue);
			if (!string.IsNullOrEmpty(parameter2.ModelParameterFieldName) && parameter2.ShouldSetValueOnField)
			{
				this.parameterNameToFieldName.Add(parameter2.ParameterName, parameter2.ModelParameterFieldName);
			}
		}
		this.enumParameters.Clear();
		foreach (IInGameDevToolEnumParameter parameter3 in devTool.EnumParameters())
		{
			this.enumParameters.Add(parameter3.ParameterName, useDefaultParameterValues ? parameter3.ParameterSerializationDefaultValue : parameter3.ParameterSerializationValue);
			if (!string.IsNullOrEmpty(parameter3.ModelParameterFieldName) && parameter3.ShouldSetValueOnField)
			{
				this.parameterNameToFieldName.Add(parameter3.ParameterName, parameter3.ModelParameterFieldName);
			}
		}
		this.floatParameters.Clear();
		foreach (IngameDevToolFloatParameter parameter4 in devTool.FloatParameters())
		{
			this.floatParameters.Add(parameter4.ParameterName, useDefaultParameterValues ? parameter4.DefaultValue : parameter4.ParameterValue);
			if (!string.IsNullOrEmpty(parameter4.ModelParameterFieldName) && parameter4.ShouldSetValueOnField)
			{
				this.parameterNameToFieldName.Add(parameter4.ParameterName, parameter4.ModelParameterFieldName);
			}
		}
		this.stringParameters.Clear();
		foreach (IngameDevToolStringParameter parameter5 in devTool.StringParameters())
		{
			string paramValue = string.Copy(useDefaultParameterValues ? parameter5.DefaultValue : parameter5.ParameterValue);
			this.stringParameters.Add(parameter5.ParameterName, paramValue);
			if (!string.IsNullOrEmpty(parameter5.ModelParameterFieldName) && parameter5.ShouldSetValueOnField)
			{
				this.parameterNameToFieldName.Add(parameter5.ParameterName, parameter5.ModelParameterFieldName);
			}
		}
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00005FE0 File Offset: 0x000041E0
	public virtual void SetEnumParameter<EnumType>(string parameterName, EnumType enumValue) where EnumType : struct
	{
		this.enumParameters.Add(parameterName, enumValue.ToString());
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00005FFB File Offset: 0x000041FB
	public void OnDeserialized(IScope context)
	{
		this.LoadCommand();
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00006004 File Offset: 0x00004204
	private bool LoadCommand()
	{
		IInGameDevTool devTool = this._devToolsRegistry.GetDevToolByCommandSerializationName(this.commandSerializationName);
		if (devTool != null)
		{
			this.commandToExecute = devTool.GetActionWithCommandType<CommandType>();
			return this.commandToExecute != null;
		}
		return this.commandToExecute != null;
	}

	// Token: 0x04000093 RID: 147
	public string commandSerializationName;

	// Token: 0x04000094 RID: 148
	public Vector2 cursorPosition;

	// Token: 0x04000095 RID: 149
	public Vector2Int cursorTilePosition;

	// Token: 0x04000096 RID: 150
	public DeviceInputType deviceInputType;

	// Token: 0x04000097 RID: 151
	[Serialize(false, null)]
	public Action<CommandType, ISimulation> commandToExecute;

	// Token: 0x04000098 RID: 152
	[Serialize(true, null)]
	protected Dictionary<string, bool> boolParameters = new Dictionary<string, bool>();

	// Token: 0x04000099 RID: 153
	[Serialize(true, null)]
	protected Dictionary<string, int> intParameters = new Dictionary<string, int>();

	// Token: 0x0400009A RID: 154
	[Serialize(true, null)]
	protected Dictionary<string, string> enumParameters = new Dictionary<string, string>();

	// Token: 0x0400009B RID: 155
	[Serialize(true, null)]
	protected Dictionary<string, Fix64> floatParameters = new Dictionary<string, Fix64>();

	// Token: 0x0400009C RID: 156
	[Serialize(true, null)]
	protected Dictionary<string, string> stringParameters = new Dictionary<string, string>();

	// Token: 0x0400009D RID: 157
	[Serialize(true, null)]
	protected Dictionary<string, string> parameterNameToFieldName = new Dictionary<string, string>();

	// Token: 0x0400009E RID: 158
	[Dependency]
	private IInGameDevToolsRegistry _devToolsRegistry;
}
