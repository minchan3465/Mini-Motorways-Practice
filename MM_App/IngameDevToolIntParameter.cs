using System;

// Token: 0x02000088 RID: 136
public class IngameDevToolIntParameter : InGameDevToolNumericParameter<int, IngameDevToolIntParameter>
{
	// Token: 0x060001CB RID: 459 RVA: 0x000062D9 File Offset: 0x000044D9
	public static IngameDevToolIntParameter DefineIntParameter(string withParameterName)
	{
		return new IngameDevToolIntParameter().SetParameterName(withParameterName);
	}

	// Token: 0x060001CC RID: 460 RVA: 0x000062E6 File Offset: 0x000044E6
	public static IngameDevToolIntParameter DefineIntParameter(string withParameterName, string modelFieldName)
	{
		return new IngameDevToolIntParameter().SetParameterName(withParameterName).SetModelParameterFieldName(modelFieldName);
	}

	// Token: 0x060001CD RID: 461 RVA: 0x000062F9 File Offset: 0x000044F9
	protected IngameDevToolIntParameter() : base(InGameDevToolParameterType.Int)
	{
	}
}
