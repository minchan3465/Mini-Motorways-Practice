using System;
using System.Collections.Generic;
using Factory;

// Token: 0x0200008F RID: 143
public class MotorwaysModelContainerTool : BaseInGameDevTool<MotorwaysModelContainerTool, MotorwaysDevToolCommand>, IReleasedFromScopeHandler
{
	// Token: 0x06000200 RID: 512 RVA: 0x0000736D File Offset: 0x0000556D
	public void RegisterNewTool(IInGameModelDevTool newTool)
	{
		this.toolsForModel.Add(newTool);
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000737B File Offset: 0x0000557B
	public void RemoveTool(IInGameModelDevTool oldTool)
	{
		this.toolsForModel.Remove(oldTool);
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000738C File Offset: 0x0000558C
	public void OnReleasedFromScope(IScope scope)
	{
		foreach (IInGameModelDevTool tool in this.toolsForModel)
		{
			scope.Release(tool);
		}
		this.toolsForModel.Clear();
	}

	// Token: 0x06000203 RID: 515 RVA: 0x000073EC File Offset: 0x000055EC
	public MotorwaysModelContainerTool SetModelType(ToolModelType newToolModelType)
	{
		this.toolModelType = newToolModelType;
		base.SetEditorDisplayName(this.toolModelType.ToString() + " Inspector");
		switch (this.toolModelType)
		{
		case ToolModelType.Unknown:
			base.SetEditorIconPath("Assets/Art/UI/Menus/Options/SPR_UI_MenuX.png");
			break;
		case ToolModelType.House:
			base.SetEditorIconPath("Assets/Art/UI/SPR_UI_Button_home.png");
			break;
		case ToolModelType.Destination:
			base.SetEditorIconPath("Assets/Art/UI/InGameBuilding/SPR_UI_Temp_InGameBuilding_00.png");
			break;
		}
		return this;
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00007466 File Offset: 0x00005666
	public List<IInGameModelDevTool> GetToolsForModel()
	{
		return this.toolsForModel;
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00007470 File Offset: 0x00005670
	protected override void OnActivation()
	{
		foreach (IInGameModelDevTool inGameModelDevTool in this.toolsForModel)
		{
			inGameModelDevTool.OnModelActivation();
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x000074C0 File Offset: 0x000056C0
	public override void OnToolDeselected()
	{
		foreach (IInGameModelDevTool inGameModelDevTool in this.toolsForModel)
		{
			inGameModelDevTool.OnToolDeselected();
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00007510 File Offset: 0x00005710
	public override void OnToolSelected()
	{
		foreach (IInGameModelDevTool inGameModelDevTool in this.toolsForModel)
		{
			inGameModelDevTool.OnToolSelected();
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x00007560 File Offset: 0x00005760
	public override void Tick(TimeInterval timeInterval, float stepAlpha, out bool activatedThisTick)
	{
		bool anyActivated = false;
		foreach (IInGameModelDevTool inGameModelDevTool in this.toolsForModel)
		{
			bool specificActivated;
			inGameModelDevTool.Tick(timeInterval, stepAlpha, out specificActivated);
			anyActivated = (anyActivated || specificActivated);
		}
		activatedThisTick = anyActivated;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x000075C0 File Offset: 0x000057C0
	public override void Reset()
	{
		base.Reset();
		this.toolModelType = ToolModelType.Unknown;
		this.toolsForModel.Clear();
	}

	// Token: 0x0600020A RID: 522 RVA: 0x000022F5 File Offset: 0x000004F5
	public override void DrawEditorTool()
	{
	}

	// Token: 0x040000C2 RID: 194
	protected ToolModelType toolModelType;

	// Token: 0x040000C3 RID: 195
	protected List<IInGameModelDevTool> toolsForModel = new List<IInGameModelDevTool>();
}
