using System;

// Token: 0x02000087 RID: 135
public class IngameDevToolBoolParameter : InGameDevToolParameter<bool, IngameDevToolBoolParameter>
{
	// Token: 0x060001C9 RID: 457 RVA: 0x000062C3 File Offset: 0x000044C3
	public static IngameDevToolBoolParameter DefineBoolParameter(string withParameterName)
	{
		return new IngameDevToolBoolParameter().SetParameterName(withParameterName);
	}

	// Token: 0x060001CA RID: 458 RVA: 0x000062D0 File Offset: 0x000044D0
	protected IngameDevToolBoolParameter() : base(InGameDevToolParameterType.Bool)
	{
	}
}
