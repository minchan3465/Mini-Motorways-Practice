using System;
using System.Collections.Generic;

// Token: 0x02000082 RID: 130
public abstract class InGameDevToolParameter<ParamType, DerivedType> where DerivedType : InGameDevToolParameter<ParamType, DerivedType>
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000609D File Offset: 0x0000429D
	// (set) Token: 0x060001A1 RID: 417 RVA: 0x000060A5 File Offset: 0x000042A5
	public InGameDevToolParameterType TypeOfParameter { get; protected set; }

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060001A2 RID: 418 RVA: 0x000060AE File Offset: 0x000042AE
	// (set) Token: 0x060001A3 RID: 419 RVA: 0x000060B6 File Offset: 0x000042B6
	public string ParameterName { get; protected set; }

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060001A4 RID: 420 RVA: 0x000060BF File Offset: 0x000042BF
	// (set) Token: 0x060001A5 RID: 421 RVA: 0x000060C7 File Offset: 0x000042C7
	public string ModelParameterFieldName { get; protected set; }

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060001A6 RID: 422 RVA: 0x000060D0 File Offset: 0x000042D0
	// (set) Token: 0x060001A7 RID: 423 RVA: 0x000060D8 File Offset: 0x000042D8
	public string ParameterEditorDisplayName { get; protected set; }

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060001A8 RID: 424 RVA: 0x000060E1 File Offset: 0x000042E1
	// (set) Token: 0x060001A9 RID: 425 RVA: 0x000060E9 File Offset: 0x000042E9
	public string ParameterEditorTooltip { get; protected set; }

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060001AA RID: 426 RVA: 0x000060F2 File Offset: 0x000042F2
	// (set) Token: 0x060001AB RID: 427 RVA: 0x000060FA File Offset: 0x000042FA
	public ParamType ParameterValue { get; protected set; }

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060001AC RID: 428 RVA: 0x00006103 File Offset: 0x00004303
	// (set) Token: 0x060001AD RID: 429 RVA: 0x0000610B File Offset: 0x0000430B
	public ParamType DefaultValue { get; protected set; }

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060001AE RID: 430 RVA: 0x00006114 File Offset: 0x00004314
	// (set) Token: 0x060001AF RID: 431 RVA: 0x0000611C File Offset: 0x0000431C
	public bool ShouldSetValueOnField { get; protected set; }

	// Token: 0x060001B0 RID: 432 RVA: 0x00006125 File Offset: 0x00004325
	public DerivedType SetParameterName(string parameterName)
	{
		this.ParameterName = parameterName;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00006134 File Offset: 0x00004334
	public DerivedType SetModelParameterFieldName(string modelParameterFieldName)
	{
		this.ModelParameterFieldName = modelParameterFieldName;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x00006143 File Offset: 0x00004343
	public DerivedType SetEditorDisplayName(string editorDisplayName)
	{
		this.ParameterEditorDisplayName = editorDisplayName;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x00006152 File Offset: 0x00004352
	public DerivedType SetEditorTooltip(string editorTooltip)
	{
		this.ParameterEditorTooltip = editorTooltip;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x00006161 File Offset: 0x00004361
	public DerivedType SetValue(ParamType newValue)
	{
		this.ParameterValue = newValue;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x00006170 File Offset: 0x00004370
	public DerivedType SetDefaultValueForHotkey(ParamType newValue)
	{
		this.DefaultValue = newValue;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000617F File Offset: 0x0000437F
	public DerivedType ShowConditionallyOnBool(string boolParameterNameToCheck, bool valueToCheck)
	{
		this.conditionallyShowOnBools.Add(new InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnBool
		{
			boolParameterName = boolParameterNameToCheck,
			valueToMatch = valueToCheck
		});
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x000061A5 File Offset: 0x000043A5
	public DerivedType ShowConditionallyOnEnum(string enumParameterNameToCheck, Enum valueToCheck)
	{
		this.conditionallyShowOnEnums.Add(new InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnEnum
		{
			enumParameterName = enumParameterNameToCheck,
			valueToMatch = valueToCheck.ToString()
		});
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x000061D0 File Offset: 0x000043D0
	public DerivedType ShowConditionallyOnFeature(Feature featureToCompare, bool valueToCheck)
	{
		this.conditionallyShowOnFeatures.Add(new InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnFeature
		{
			featureToCheck = featureToCompare,
			valueToMatch = valueToCheck
		});
		return (DerivedType)((object)this);
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x000061F6 File Offset: 0x000043F6
	public DerivedType DontSetValueOnApply()
	{
		this.ShouldSetValueOnField = false;
		return (DerivedType)((object)this);
	}

	// Token: 0x060001BA RID: 442 RVA: 0x00006205 File Offset: 0x00004405
	protected InGameDevToolParameter(InGameDevToolParameterType typeOfParameter)
	{
		this.TypeOfParameter = typeOfParameter;
		this.ShouldSetValueOnField = true;
	}

	// Token: 0x040000AF RID: 175
	protected List<InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnBool> conditionallyShowOnBools = new List<InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnBool>();

	// Token: 0x040000B0 RID: 176
	protected List<InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnEnum> conditionallyShowOnEnums = new List<InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnEnum>();

	// Token: 0x040000B1 RID: 177
	protected List<InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnFeature> conditionallyShowOnFeatures = new List<InGameDevToolParameter<ParamType, DerivedType>.ConditionallyShowOnFeature>();

	// Token: 0x02000083 RID: 131
	protected class ConditionallyShowOnBool
	{
		// Token: 0x040000B2 RID: 178
		public string boolParameterName;

		// Token: 0x040000B3 RID: 179
		public bool valueToMatch;
	}

	// Token: 0x02000084 RID: 132
	protected class ConditionallyShowOnEnum
	{
		// Token: 0x040000B4 RID: 180
		public string enumParameterName;

		// Token: 0x040000B5 RID: 181
		public string valueToMatch;
	}

	// Token: 0x02000085 RID: 133
	protected class ConditionallyShowOnFeature
	{
		// Token: 0x040000B6 RID: 182
		public Feature featureToCheck;

		// Token: 0x040000B7 RID: 183
		public bool valueToMatch;
	}
}
