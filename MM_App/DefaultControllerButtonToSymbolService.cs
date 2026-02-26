using System;

// Token: 0x0200014F RID: 335
public class DefaultControllerButtonToSymbolService : IControllerButtonToSymbolService
{
	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x0600075F RID: 1887 RVA: 0x0000222C File Offset: 0x0000042C
	public bool HasMappings
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0001849C File Offset: 0x0001669C
	public virtual string GetTextMeshProSymbolTextForControllerButton(ControllerButton buttonType)
	{
		string spriteName = "";
		switch (buttonType)
		{
		case ControllerButton.FaceButtonBottom:
			spriteName = "SPR_Switch_LetterButtons-Down";
			break;
		case ControllerButton.FaceButtonRight:
			spriteName = "SPR_Switch_LetterButtons-Right";
			break;
		case ControllerButton.FaceButtonLeft:
			spriteName = "SPR_Switch_LetterButtons-Left";
			break;
		case ControllerButton.FaceButtonTop:
			spriteName = "SPR_Switch_LetterButtons-Up";
			break;
		case ControllerButton.ButtonUp:
			spriteName = "SPR_PC_DPad-Up";
			break;
		case ControllerButton.ButtonDown:
			spriteName = "SPR_PC_DPad-Down";
			break;
		case ControllerButton.ButtonLeft:
			spriteName = "SPR_PC_DPad-Left";
			break;
		case ControllerButton.ButtonRight:
			spriteName = "SPR_PC_DPad-Right";
			break;
		}
		if (spriteName.Length <= 0)
		{
			return null;
		}
		return "<sprite name=\"" + spriteName + "\" tint=1>";
	}
}
