using System;
using Factory.Pools;

// Token: 0x0200009E RID: 158
public class NullInGameDevToolsRegistry : IInGameDevToolsRegistry, IReusable
{
	// Token: 0x060002A6 RID: 678 RVA: 0x000022F5 File Offset: 0x000004F5
	public void RegisterTools()
	{
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x000022F5 File Offset: 0x000004F5
	public void RespondToInGameToolUse()
	{
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public IInGameDevTool GetDevToolByCommandSerializationName(string commandSerializationName)
	{
		return null;
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public IInGameModelDevTool GetModelDevToolByCommandSerializationName(string commandSerializationName)
	{
		return null;
	}

	// Token: 0x060002AA RID: 682 RVA: 0x000022F5 File Offset: 0x000004F5
	public void UpdateEditorIfPresent()
	{
	}

	// Token: 0x060002AB RID: 683 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Reset()
	{
	}
}
