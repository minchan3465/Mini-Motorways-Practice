using System;
using System.Collections.Generic;
using System.Reflection;

// Token: 0x0200008C RID: 140
public class IngameDevToolEnumParameter<EnumType> : InGameDevToolParameter<EnumType, IngameDevToolEnumParameter<EnumType>>, IInGameDevToolEnumParameter where EnumType : struct
{
	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060001DD RID: 477 RVA: 0x00006344 File Offset: 0x00004544
	// (set) Token: 0x060001DE RID: 478 RVA: 0x00006368 File Offset: 0x00004568
	public string ParameterSerializationValue
	{
		get
		{
			EnumType parameterValue = base.ParameterValue;
			return parameterValue.ToString();
		}
		set
		{
			EnumType parsedValue;
			if (Enum.TryParse<EnumType>(value, out parsedValue))
			{
				base.ParameterValue = parsedValue;
			}
		}
	}

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060001DF RID: 479 RVA: 0x00006388 File Offset: 0x00004588
	// (set) Token: 0x060001E0 RID: 480 RVA: 0x000063AC File Offset: 0x000045AC
	public string ParameterSerializationDefaultValue
	{
		get
		{
			EnumType defaultValue = base.DefaultValue;
			return defaultValue.ToString();
		}
		set
		{
			EnumType parsedValue;
			if (Enum.TryParse<EnumType>(value, out parsedValue))
			{
				base.DefaultValue = parsedValue;
			}
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x000063CC File Offset: 0x000045CC
	public IngameDevToolEnumParameter<EnumType> SetAllowedValues(List<EnumType> valuesToAllow)
	{
		this.allowedValues = new List<Enum>();
		foreach (EnumType enumType in valuesToAllow)
		{
			Enum castValue = (Enum)((object)enumType);
			if (!this.allowedValues.Contains(castValue))
			{
				this.allowedValues.Add(castValue);
			}
		}
		return this;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00006444 File Offset: 0x00004644
	public static IngameDevToolEnumParameter<EnumType> DefineEnumParameter(string withParameterName)
	{
		return new IngameDevToolEnumParameter<EnumType>().SetParameterName(withParameterName);
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00006451 File Offset: 0x00004651
	public IngameDevToolEnumParameter() : base(InGameDevToolParameterType.Enum)
	{
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000645C File Offset: 0x0000465C
	public void UpdateParameterValueFromModelField<ModelType>(ModelType modelInstance)
	{
		FieldInfo fieldInfo = typeof(ModelType).GetField(base.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (fieldInfo != null)
		{
			EnumType currentValue = (EnumType)((object)fieldInfo.GetValue(modelInstance));
			base.SetValue(currentValue);
			return;
		}
		PropertyInfo propertyInfo = typeof(ModelType).GetProperty(base.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (Diagnostics.Verify(propertyInfo != null))
		{
			EnumType currentValue2 = (EnumType)((object)propertyInfo.GetValue(modelInstance));
			base.SetValue(currentValue2);
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x000064E8 File Offset: 0x000046E8
	public void UpdateModelFieldFromParameterValue<ModelType>(ModelType modelInstance)
	{
		if (!string.IsNullOrEmpty(base.ModelParameterFieldName))
		{
			FieldInfo fieldInfo = typeof(ModelType).GetField(base.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(modelInstance, base.ParameterValue);
				return;
			}
			PropertyInfo propertyInfo = typeof(ModelType).GetProperty(base.ModelParameterFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (Diagnostics.Verify(propertyInfo != null))
			{
				propertyInfo.SetValue(modelInstance, base.ParameterValue);
			}
		}
	}

	// Token: 0x040000BC RID: 188
	protected List<Enum> allowedValues;
}
