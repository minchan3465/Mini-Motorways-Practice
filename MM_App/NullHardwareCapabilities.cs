using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class NullHardwareCapabilities : IHardwareCapabilities
{
	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x060003D1 RID: 977 RVA: 0x0000EA0E File Offset: 0x0000CC0E
	public RuntimePlatform Platform
	{
		get
		{
			return Application.platform;
		}
	}

	// Token: 0x170000B3 RID: 179
	// (get) Token: 0x060003D2 RID: 978 RVA: 0x000020AA File Offset: 0x000002AA
	public LocaleDatabase.LocaleId PreferredLocaleId
	{
		get
		{
			return LocaleDatabase.LocaleId.en_US;
		}
	}

	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x060003D3 RID: 979 RVA: 0x0000EA22 File Offset: 0x0000CC22
	public string PersistentStoragePath
	{
		get
		{
			return Application.persistentDataPath;
		}
	}

	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x060003D4 RID: 980 RVA: 0x0000222C File Offset: 0x0000042C
	public DeviceInputType DefaultDeviceInputType
	{
		get
		{
			return DeviceInputType.Touch;
		}
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060003D5 RID: 981 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x060003D6 RID: 982 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<DeviceInputGamepadStyle> OnGamepadStyleChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x060003D7 RID: 983 RVA: 0x0000222C File Offset: 0x0000042C
	public DeviceInputGamepadStyle CurrentGamepadStyle
	{
		get
		{
			return DeviceInputGamepadStyle.None;
		}
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x060003D8 RID: 984 RVA: 0x0000EE8C File Offset: 0x0000D08C
	public string UniqueDeviceId
	{
		get
		{
			return HashUtils.GetMD5(SystemInfo.deviceUniqueIdentifier);
		}
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x060003D9 RID: 985 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsHapticFeedback
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060003DA RID: 986 RVA: 0x000022F5 File Offset: 0x000004F5
	public void GenerateHapticFeedback(HapticFeedbackType feedback)
	{
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x060003DB RID: 987 RVA: 0x000020AA File Offset: 0x000002AA
	public bool SupportsManualExit
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x060003DC RID: 988 RVA: 0x0000222C File Offset: 0x0000042C
	// (set) Token: 0x060003DD RID: 989 RVA: 0x000022F5 File Offset: 0x000004F5
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

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x060003DE RID: 990 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsChangingResolution
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x060003DF RID: 991 RVA: 0x0000ED50 File Offset: 0x0000CF50
	public Vector2Int DefaultMaximumResolution
	{
		get
		{
			return new Vector2Int(-1, -1);
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x060003E0 RID: 992 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsAntiAliasingOptions
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x060003E1 RID: 993 RVA: 0x0000222C File Offset: 0x0000042C
	public int DefaultAntiAliasingLevel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x060003E2 RID: 994 RVA: 0x0000222C File Offset: 0x0000042C
	public bool SupportsMultipleDisplays
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x060003E3 RID: 995 RVA: 0x000020AA File Offset: 0x000002AA
	public int DisplayCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x0000EA9A File Offset: 0x0000CC9A
	public void Exit()
	{
		Application.Quit();
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnAppStart()
	{
	}
}
