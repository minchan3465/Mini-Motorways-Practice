using System;

// Token: 0x02000099 RID: 153
public interface IInGameModelDevTool : IInGameDevTool
{
	// Token: 0x06000252 RID: 594
	ToolModelType GetToolModelType();

	// Token: 0x06000253 RID: 595
	void OnModelActivation();
}
