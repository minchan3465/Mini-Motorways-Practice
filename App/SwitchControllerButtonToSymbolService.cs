using System;
using Factory;

// Token: 0x02000154 RID: 340
public class SwitchControllerButtonToSymbolService : DefaultControllerButtonToSymbolService, InputState.IObserver
{
	// Token: 0x0600076E RID: 1902 RVA: 0x00018790 File Offset: 0x00016990
	public override string GetTextMeshProSymbolTextForControllerButton(ControllerButton buttonType)
	{
		string spriteName;
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
		case ControllerButton.ButtonShoulderLeft:
			if (this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConL || this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConR)
			{
				spriteName = "SPR_Switch_SL";
			}
			else
			{
				spriteName = "SPR_Switch_L";
			}
			break;
		case ControllerButton.ButtonShoulderRight:
			if (this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConL || this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConR)
			{
				spriteName = "SPR_Switch_SR";
			}
			else
			{
				spriteName = "SPR_Switch_R";
			}
			break;
		case ControllerButton.ButtonTriggerLeft:
			if (this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConL || this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConR)
			{
				spriteName = "SPR_Switch_SL";
			}
			else
			{
				spriteName = "SPR_Switch_ZL";
			}
			break;
		case ControllerButton.ButtonTriggerRight:
			if (this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConL || this._hardwareCapabilities.CurrentGamepadStyle == DeviceInputGamepadStyle.SwitchJoyConR)
			{
				spriteName = "SPR_Switch_SR";
			}
			else
			{
				spriteName = "SPR_Switch_ZR";
			}
			break;
		case ControllerButton.ButtonHome:
			spriteName = "SPR_Switch_Home";
			break;
		case ControllerButton.ButtonMenu:
			spriteName = "SPR_Switch_Home";
			break;
		case ControllerButton.ButtonOptions:
			spriteName = "SPR_Switch_Home";
			break;
		case ControllerButton.ButtonUp:
			spriteName = "SPR_Switch_DPad-Up";
			break;
		case ControllerButton.ButtonDown:
			spriteName = "SPR_Switch_DPad-Down";
			break;
		case ControllerButton.ButtonLeft:
			spriteName = "SPR_Switch_DPad-Down";
			break;
		case ControllerButton.ButtonRight:
			spriteName = "SPR_Switch_DPad-Right";
			break;
		case ControllerButton.ButtonThumbstickLeft:
			spriteName = "SPR_Switch_Joystick_Click-Left";
			break;
		case ControllerButton.ButtonThumbstickRight:
			spriteName = "SPR_Switch_Joystick_Click-Right";
			break;
		case ControllerButton.Dpad:
			spriteName = "SPR_Switch_DPad-Filled";
			break;
		case ControllerButton.ThumbstickLeft:
			spriteName = "SPR_Switch_Joystick-Left";
			break;
		case ControllerButton.ThumbstickRight:
			spriteName = "SPR_Switch_Joystick-Right";
			break;
		default:
			throw new ArgumentOutOfRangeException("buttonType", buttonType, null);
		}
		if (spriteName.Length <= 0)
		{
			return null;
		}
		return "<sprite name=\"" + spriteName + "\" tint=1>";
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x000022F5 File Offset: 0x000004F5
	public void OnCurrentDeviceInputTypeChanged(DeviceInputType newInputType)
	{
	}

	// Token: 0x04000376 RID: 886
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;
}
