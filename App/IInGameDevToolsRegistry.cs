using System;
using Factory.Pools;

// Token: 0x0200009A RID: 154
public interface IInGameDevToolsRegistry : IReusable
{
	// Token: 0x06000254 RID: 596
	void RegisterTools();

	// Token: 0x06000255 RID: 597
	void RespondToInGameToolUse();

	// Token: 0x06000256 RID: 598
	IInGameDevTool GetDevToolByCommandSerializationName(string commandSerializationName);

	// Token: 0x06000257 RID: 599
	IInGameModelDevTool GetModelDevToolByCommandSerializationName(string commandSerializationName);

	// Token: 0x06000258 RID: 600
	void UpdateEditorIfPresent();
}
