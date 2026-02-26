using System;

// Token: 0x0200008A RID: 138
public class IngameDevToolStringParameter : InGameDevToolParameter<string, IngameDevToolStringParameter>
{
	// Token: 0x060001D1 RID: 465 RVA: 0x0000632B File Offset: 0x0000452B
	public static IngameDevToolStringParameter DefineStringParameter(string withParameterName)
	{
		return new IngameDevToolStringParameter().SetParameterName(withParameterName);
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x00006338 File Offset: 0x00004538
	protected IngameDevToolStringParameter() : base(InGameDevToolParameterType.String)
	{
	}
}
