using System;
using Factory;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class iOSHardwareCapabilities : IHardwareCapabilities
{
	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060003AA RID: 938 RVA: 0x0000EA0E File Offset: 0x0000CC0E
	public RuntimePlatform Platform
	{
		get
		{
			return Application.platform;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x060003AB RID: 939 RVA: 0x0000ECB8 File Offset: 0x0000CEB8
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return CoreFoundationLocaleQuery.GetLocaleId(this._localeDatabase);
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x060003AC RID: 940 RVA: 0x0000EA22 File Offset: 0x0000CC22
	public string PersistentStoragePath
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x060003AD RID: 941 RVA: 0x0000ECC5 File Offset: 0x0000CEC5
	public string UniqueDeviceId
	{
		get
		{
			if (this._deviceId == null)
			{
				this._deviceId = HashUtils.GetMD5(iOSHardwareCapabilities.GetDeviceId());
			}
			return this._deviceId;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060003AE RID: 942 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsHapticFeedback
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0000ECE8 File Offset: 0x0000CEE8
	public void GenerateHapticFeedback(HapticFeedbackType feedback)
	{
		switch (feedback)
		{
		case HapticFeedbackType.LightImpact:
			iOSHardwareCapabilities.TriggerLightImpact();
			return;
		case HapticFeedbackType.MediumImpact:
			iOSHardwareCapabilities.TriggerMediumImpact();
			return;
		case HapticFeedbackType.HeavyImpact:
			iOSHardwareCapabilities.TriggerHeavyImpact();
			return;
		case HapticFeedbackType.Selection:
			iOSHardwareCapabilities.TriggerSelection();
			return;
		case HapticFeedbackType.Success:
			iOSHardwareCapabilities.TriggerSuccess();
			return;
		case HapticFeedbackType.Warning:
			iOSHardwareCapabilities.TriggerWarning();
			return;
		case HapticFeedbackType.Error:
			iOSHardwareCapabilities.TriggerError();
			return;
		default:
			return;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060003B0 RID: 944 RVA: 0x0000ED41 File Offset: 0x0000CF41
	// (set) Token: 0x060003B1 RID: 945 RVA: 0x0000ED48 File Offset: 0x0000CF48
	public bool IsPreventingSleep
	{
		get
		{
			return iOSHardwareCapabilities.IsIdleTimerDisabled();
		}
		set
		{
			iOSHardwareCapabilities.SetIdleTimerDisabled(value);
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060003B2 RID: 946 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsManualExit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x060003B3 RID: 947 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsChangingResolution
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x060003B4 RID: 948 RVA: 0x0000ED50 File Offset: 0x0000CF50
	public Vector2Int DefaultMaximumResolution
	{
		get
		{
			return new Vector2Int(-1, -1);
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x060003B5 RID: 949 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsAntiAliasingOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x060003B6 RID: 950 RVA: 0x0000222C File Offset: 0x0000042C
	public int DefaultAntiAliasingLevel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x060003B7 RID: 951 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMultipleDisplays
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000AF RID: 175
	// (get) Token: 0x060003B8 RID: 952 RVA: 0x000020AA File Offset: 0x000002AA
	public int DisplayCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0000ED59 File Offset: 0x0000CF59
	public void Exit()
	{
		Diagnostics.FailAssert("Exit() not supported on iOS.", Array.Empty<object>());
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x060003BA RID: 954 RVA: 0x0000222C File Offset: 0x0000042C
	public DeviceInputType DefaultDeviceInputType
	{
		get
		{
			return DeviceInputType.Touch;
		}
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x060003BB RID: 955 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x060003BC RID: 956 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<DeviceInputGamepadStyle> OnGamepadStyleChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x060003BD RID: 957 RVA: 0x000020AA File Offset: 0x000002AA
	public DeviceInputGamepadStyle CurrentGamepadStyle
	{
		get
		{
			return DeviceInputGamepadStyle.Generic;
		}
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0000ED6C File Offset: 0x0000CF6C
	public virtual void OnAppStart()
	{
		int targetFrameRate = 60;
		if (SystemInfo.deviceModel.StartsWith("iPad5,") || SystemInfo.deviceModel.StartsWith("iPhone8,"))
		{
			targetFrameRate = 30;
		}
		Application.targetFrameRate = targetFrameRate;
		if (FeatureToggle.IsFeatureEnabled(Feature.MockControllerAsRemote))
		{
			this._inputState.ControllerConnected(this._scope.Get<IAppleTVRemoteController>());
		}
		else
		{
			this._inputState.ControllerConnected(this._scope.Get<IGamepadController>());
		}
		this._inputState.ControllerConnected(this._scope.Get<ITouchScreenController>());
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0000EDF4 File Offset: 0x0000CFF4
	private static string GetDeviceId()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void SetIdleTimerDisabled(bool disabled)
	{
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x0000222C File Offset: 0x0000042C
	private static bool IsIdleTimerDisabled()
	{
		return false;
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void TriggerLightImpact()
	{
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void TriggerMediumImpact()
	{
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void TriggerHeavyImpact()
	{
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void TriggerSelection()
	{
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void TriggerSuccess()
	{
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void TriggerWarning()
	{
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void TriggerError()
	{
	}

	// Token: 0x0400018E RID: 398
	[Dependency]
	protected IScope _scope;

	// Token: 0x0400018F RID: 399
	[Dependency]
	private LocaleDatabase _localeDatabase;

	// Token: 0x04000190 RID: 400
	[Dependency]
	private IInputState _inputState;

	// Token: 0x04000191 RID: 401
	private string _deviceId;
}
