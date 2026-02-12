using System;
using System.Collections.Generic;
using Factory;
using Rewired;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class tvOSHardwareCapabilities : IHardwareCapabilities
{
	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x0600040A RID: 1034 RVA: 0x0000EA0E File Offset: 0x0000CC0E
	public RuntimePlatform Platform
	{
		get
		{
			return Application.platform;
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x0600040B RID: 1035 RVA: 0x0000F371 File Offset: 0x0000D571
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return CoreFoundationLocaleQuery.GetLocaleId(this._localeDatabase);
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x0600040C RID: 1036 RVA: 0x0000EA22 File Offset: 0x0000CC22
	public string PersistentStoragePath
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x0600040D RID: 1037 RVA: 0x0000F37E File Offset: 0x0000D57E
	public string UniqueDeviceId
	{
		get
		{
			if (this._deviceId == null)
			{
				this._deviceId = HashUtils.GetMD5(tvOSHardwareCapabilities.GetDeviceId());
			}
			return this._deviceId;
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x0600040E RID: 1038 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHapticFeedback
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x000022F5 File Offset: 0x000004F5
	public void GenerateHapticFeedback(HapticFeedbackType feedback)
	{
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06000410 RID: 1040 RVA: 0x0000222C File Offset: 0x0000042C
	// (set) Token: 0x06000411 RID: 1041 RVA: 0x000022F5 File Offset: 0x000004F5
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

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06000412 RID: 1042 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsManualExit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06000413 RID: 1043 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsChangingResolution
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x06000414 RID: 1044 RVA: 0x0000ED50 File Offset: 0x0000CF50
	public Vector2Int DefaultMaximumResolution
	{
		get
		{
			return new Vector2Int(-1, -1);
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000415 RID: 1045 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsAntiAliasingOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000416 RID: 1046 RVA: 0x0000222C File Offset: 0x0000042C
	public int DefaultAntiAliasingLevel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000417 RID: 1047 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMultipleDisplays
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000418 RID: 1048 RVA: 0x000020AA File Offset: 0x000002AA
	public int DisplayCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x0000F39E File Offset: 0x0000D59E
	public void Exit()
	{
		Diagnostics.FailAssert("Exit() not supported on tvOS.", Array.Empty<object>());
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x0600041A RID: 1050 RVA: 0x0000F3B0 File Offset: 0x0000D5B0
	public DeviceInputType DefaultDeviceInputType
	{
		get
		{
			DeviceInputType result = DeviceInputType.Remote;
			using (IEnumerator<Controller> enumerator = ReInput.controllers.Controllers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (RuntimeAppCommandSource.GetSourceForController(enumerator.Current) == InputEventSource.Generic)
					{
						result = DeviceInputType.Controller;
					}
				}
			}
			return result;
		}
	}

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x0600041B RID: 1051 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x0600041C RID: 1052 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<DeviceInputGamepadStyle> OnGamepadStyleChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x0600041D RID: 1053 RVA: 0x000020AA File Offset: 0x000002AA
	public DeviceInputGamepadStyle CurrentGamepadStyle
	{
		get
		{
			return DeviceInputGamepadStyle.Generic;
		}
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x0000F408 File Offset: 0x0000D608
	public virtual void OnAppStart()
	{
		int targetFrameRate = 60;
		if (SystemInfo.deviceModel == "AppleTV5,3")
		{
			targetFrameRate = 30;
		}
		Application.targetFrameRate = targetFrameRate;
		this._scope.Get<IInputState>().ControllerConnected(this._scope.Get<IGamepadController>());
		this._scope.Get<IInputState>().ControllerConnected(this._scope.Get<IAppleTVRemoteController>());
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x0000EDF4 File Offset: 0x0000CFF4
	private static string GetDeviceId()
	{
		return SystemInfo.deviceUniqueIdentifier;
	}

	// Token: 0x040001A4 RID: 420
	[Dependency]
	private IScope _scope;

	// Token: 0x040001A5 RID: 421
	[Dependency]
	private LocaleDatabase _localeDatabase;

	// Token: 0x040001A6 RID: 422
	private string _deviceId;
}
