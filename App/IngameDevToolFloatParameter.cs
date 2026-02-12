using System;
using FixMath;

// Token: 0x02000089 RID: 137
public class IngameDevToolFloatParameter : InGameDevToolNumericParameter<Fix64, IngameDevToolFloatParameter>
{
	// Token: 0x060001CE RID: 462 RVA: 0x00006302 File Offset: 0x00004502
	public static IngameDevToolFloatParameter DefineFloatParameter(string withParameterName)
	{
		return new IngameDevToolFloatParameter().SetParameterName(withParameterName);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000630F File Offset: 0x0000450F
	public static IngameDevToolFloatParameter DefineFloatParameter(string withParameterName, string modelFieldName)
	{
		return new IngameDevToolFloatParameter().SetParameterName(withParameterName).SetModelParameterFieldName(modelFieldName);
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00006322 File Offset: 0x00004522
	protected IngameDevToolFloatParameter() : base(InGameDevToolParameterType.Float)
	{
	}
}
