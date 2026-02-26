using System;
using Factory;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class DesktopHardwareCapabilities : IHardwareCapabilities
{
	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000388 RID: 904 RVA: 0x0000EA0E File Offset: 0x0000CC0E
	public RuntimePlatform Platform
	{
		get
		{
			return Application.platform;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000389 RID: 905 RVA: 0x0000EA15 File Offset: 0x0000CC15
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return UnityLocaleQuery.GetLocaleId(this._localeDatabase);
		}
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x0600038A RID: 906 RVA: 0x0000EA22 File Offset: 0x0000CC22
	public string PersistentStoragePath
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x0600038B RID: 907 RVA: 0x0000EA29 File Offset: 0x0000CC29
	public string UniqueDeviceId
	{
		get
		{
			if (this._deviceId == null)
			{
				this._deviceId = HashUtils.GetMD5(DesktopHardwareCapabilities.GetDeviceId());
			}
			return this._deviceId;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x0600038C RID: 908 RVA: 0x000020AA File Offset: 0x000002AA
	public DeviceInputType DefaultDeviceInputType
	{
		get
		{
			return DeviceInputType.Mouse;
		}
	}

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x0600038D RID: 909 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x0600038E RID: 910 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<DeviceInputGamepadStyle> OnGamepadStyleChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x0600038F RID: 911 RVA: 0x000020AA File Offset: 0x000002AA
	public DeviceInputGamepadStyle CurrentGamepadStyle
	{
		get
		{
			return DeviceInputGamepadStyle.Generic;
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000390 RID: 912 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHapticFeedback
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000391 RID: 913 RVA: 0x000022F5 File Offset: 0x000004F5
	public void GenerateHapticFeedback(HapticFeedbackType feedback)
	{
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000392 RID: 914 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsManualExit
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000393 RID: 915 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsChangingResolution
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000394 RID: 916 RVA: 0x0000EA49 File Offset: 0x0000CC49
	public Vector2Int DefaultMaximumResolution
	{
		get
		{
			if (!this._hasHighPowerGpu)
			{
				return new Vector2Int(1920, 1080);
			}
			return new Vector2Int(-1, -1);
		}
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000395 RID: 917 RVA: 0x0000EA6A File Offset: 0x0000CC6A
	public bool SupportsMultipleDisplays
	{
		get
		{
			return this.DisplayCount > 1;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000396 RID: 918 RVA: 0x0000EA75 File Offset: 0x0000CC75
	public int DisplayCount
	{
		get
		{
			return MultiDisplayCapabilitiesBridge.GetDisplayCount();
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x06000397 RID: 919 RVA: 0x0000EA7C File Offset: 0x0000CC7C
	public bool SupportsAntiAliasingOptions
	{
		get
		{
			return !DesktopHardwareCapabilities.UsingOpenGL;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x06000398 RID: 920 RVA: 0x0000EA86 File Offset: 0x0000CC86
	public int DefaultAntiAliasingLevel
	{
		get
		{
			if (!this._hasHighPowerGpu || DesktopHardwareCapabilities.UsingOpenGL)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000399 RID: 921 RVA: 0x0000222C File Offset: 0x0000042C
	// (set) Token: 0x0600039A RID: 922 RVA: 0x000022F5 File Offset: 0x000004F5
	public bool IsPreventingSleep
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	// Token: 0x0600039B RID: 923 RVA: 0x0000EA9A File Offset: 0x0000CC9A
	public void Exit()
	{
		Application.Quit();
	}

	// Token: 0x0600039C RID: 924 RVA: 0x0000EAA4 File Offset: 0x0000CCA4
	public virtual void OnAppStart()
	{
		this._hasHighPowerGpu = DesktopHardwareCapabilities.HasHighPowerGpu;
		if (FeatureToggle.IsFeatureEnabled(Feature.MockControllerAsRemote))
		{
			this._inputState.ControllerConnected(this._scope.Get<IAppleTVRemoteController>());
		}
		else
		{
			this._inputState.ControllerConnected(this._scope.Get<IGamepadController>());
		}
		this._inputState.ControllerConnected(this._scope.Get<IMouseController>());
		this._inputState.ControllerConnected(this._scope.Get<IKeyboardController>());
		this._inputState.ControllerConnected(this._scope.Get<ITouchScreenController>());
		DesktopHardwareCapabilities.SetMinimumWindowAspectRatio(1.3333334f);
		DesktopHardwareCapabilities.SetMaximumWindowAspectRatio(2.1666667f);
	}

	// Token: 0x0600039D RID: 925 RVA: 0x0000EB4C File Offset: 0x0000CD4C
	private static string GetDeviceId()
	{
		string deviceId = SystemInfo.deviceUniqueIdentifier;
		if (string.IsNullOrEmpty(deviceId))
		{
			return "";
		}
		return deviceId;
	}

	// Token: 0x0600039E RID: 926 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void HideWindow()
	{
	}

	// Token: 0x0600039F RID: 927 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void SetMinimumWindowSize(int width, int height)
	{
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void SetMinimumWindowAspectRatio(float minimumAspectRatio)
	{
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void SetMaximumWindowAspectRatio(float maximumAspectRatio)
	{
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x0000222C File Offset: 0x0000042C
	private static int GetSafeAreaHeight()
	{
		return 0;
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060003A3 RID: 931 RVA: 0x0000EB6E File Offset: 0x0000CD6E
	private static bool UsingOpenGL
	{
		get
		{
			return SystemInfo.graphicsDeviceVersion.Contains("OpenGL");
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x060003A4 RID: 932 RVA: 0x0000EB80 File Offset: 0x0000CD80
	public static Vector2Int SafeAreaDimensions
	{
		get
		{
			if (Screen.resolutions.Length != 0)
			{
				Resolution nativeResolution = Screen.resolutions[Screen.resolutions.Length - 1];
				return new Vector2Int(nativeResolution.width, nativeResolution.height - DesktopHardwareCapabilities.SafeAreaHeight);
			}
			return Vector2Int.zero;
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060003A5 RID: 933 RVA: 0x0000EBC8 File Offset: 0x0000CDC8
	public static int SafeAreaHeight
	{
		get
		{
			return DesktopHardwareCapabilities.GetSafeAreaHeight();
		}
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0000EBD0 File Offset: 0x0000CDD0
	public static Vector2Int GetClosestResolution(Vector2Int resolution)
	{
		float bestResolutionSuitability = -1f;
		Vector2Int bestResolution = Vector2Int.zero;
		foreach (Resolution availableResolution in Screen.resolutions)
		{
			float resolutionSuitability = (new Vector2Int(availableResolution.width, availableResolution.height) - resolution).magnitude;
			if (bestResolutionSuitability < 0f || bestResolutionSuitability > resolutionSuitability)
			{
				bestResolutionSuitability = resolutionSuitability;
				bestResolution = new Vector2Int(availableResolution.width, availableResolution.height);
			}
		}
		return bestResolution;
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060003A7 RID: 935 RVA: 0x0000EC50 File Offset: 0x0000CE50
	public static bool HasHighPowerGpu
	{
		get
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				string deviceModel = SystemInfo.deviceModel;
				if (deviceModel.StartsWith("Macmini"))
				{
					string[] version = deviceModel.Substring(7).Split(new char[]
					{
						','
					});
					int majorVersion;
					if (version.Length != 0 && int.TryParse(version[0], out majorVersion) && majorVersion <= 8)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	// Token: 0x04000188 RID: 392
	[Dependency]
	private IScope _scope;

	// Token: 0x04000189 RID: 393
	[Dependency]
	private LocaleDatabase _localeDatabase;

	// Token: 0x0400018A RID: 394
	[Dependency]
	private IInputState _inputState;

	// Token: 0x0400018B RID: 395
	private string _deviceId;

	// Token: 0x0400018C RID: 396
	private bool _hasHighPowerGpu;

	// Token: 0x0400018D RID: 397
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DesktopHardwareCapabilities");
}
