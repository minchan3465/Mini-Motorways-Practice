using System;
using System.Collections.Generic;
using Factory;

// Token: 0x020001AF RID: 431
public class MotorwaysDeviceSettings : BaseDeviceSettings, ICreatedInScopeHandler
{
	// Token: 0x17000226 RID: 550
	// (get) Token: 0x060009B3 RID: 2483 RVA: 0x0001FEDC File Offset: 0x0001E0DC
	// (set) Token: 0x060009B4 RID: 2484 RVA: 0x0001FEE4 File Offset: 0x0001E0E4
	public string ColorfulOption
	{
		get
		{
			return this._colorfulOption;
		}
		set
		{
			if (this._colorfulOption != value)
			{
				this._colorfulOption = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x060009B5 RID: 2485 RVA: 0x0001FF01 File Offset: 0x0001E101
	// (set) Token: 0x060009B6 RID: 2486 RVA: 0x0001FF09 File Offset: 0x0001E109
	public bool IsNightModeEnabled
	{
		get
		{
			return this._isNightModeEnabled;
		}
		set
		{
			if (this._isNightModeEnabled != value)
			{
				this._isNightModeEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x060009B7 RID: 2487 RVA: 0x0001FF21 File Offset: 0x0001E121
	// (set) Token: 0x060009B8 RID: 2488 RVA: 0x0001FF42 File Offset: 0x0001E142
	public int AntiAliasingLevel
	{
		get
		{
			if (!this._hardwareCapabilities.SupportsAntiAliasingOptions)
			{
				return this._hardwareCapabilities.DefaultAntiAliasingLevel;
			}
			return this._antiAliasingLevel;
		}
		set
		{
			if (!this._hardwareCapabilities.SupportsAntiAliasingOptions)
			{
				return;
			}
			this._isUsingDefaultAntiAliasing = false;
			if (this._antiAliasingLevel != value)
			{
				this._antiAliasingLevel = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x060009B9 RID: 2489 RVA: 0x0001FF6F File Offset: 0x0001E16F
	// (set) Token: 0x060009BA RID: 2490 RVA: 0x0001FF86 File Offset: 0x0001E186
	public int SelectedDisplay
	{
		get
		{
			if (!this._hardwareCapabilities.SupportsMultipleDisplays)
			{
				return 1;
			}
			return this._selectedDisplay;
		}
		set
		{
			if (!this._hardwareCapabilities.SupportsMultipleDisplays)
			{
				return;
			}
			if (this._selectedDisplay != value)
			{
				this._selectedDisplay = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x060009BB RID: 2491 RVA: 0x0001FFAC File Offset: 0x0001E1AC
	// (set) Token: 0x060009BC RID: 2492 RVA: 0x0001FFB4 File Offset: 0x0001E1B4
	public bool IsZoomEnabled
	{
		get
		{
			return this._isZoomEnabled;
		}
		set
		{
			if (this._isZoomEnabled != value)
			{
				this._isZoomEnabled = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x060009BD RID: 2493 RVA: 0x0001FFCC File Offset: 0x0001E1CC
	// (set) Token: 0x060009BE RID: 2494 RVA: 0x0001FFD4 File Offset: 0x0001E1D4
	public int ZoomLevel
	{
		get
		{
			return this._zoomLevel;
		}
		set
		{
			if (this._zoomLevel != value)
			{
				this._zoomLevel = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x060009BF RID: 2495 RVA: 0x0001FFEC File Offset: 0x0001E1EC
	// (set) Token: 0x060009C0 RID: 2496 RVA: 0x0001FFF4 File Offset: 0x0001E1F4
	public int VolumeSetting
	{
		get
		{
			return this._volumeSetting;
		}
		set
		{
			int newVolumeSetting = this._audioSystem.RequiresVolumeControl ? value : 3;
			if (this._volumeSetting != newVolumeSetting)
			{
				this._volumeSetting = newVolumeSetting;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700022D RID: 557
	// (get) Token: 0x060009C1 RID: 2497 RVA: 0x00020029 File Offset: 0x0001E229
	// (set) Token: 0x060009C2 RID: 2498 RVA: 0x00020031 File Offset: 0x0001E231
	public int Soundscape
	{
		get
		{
			return this._soundscape;
		}
		set
		{
			if (this._soundscape != value)
			{
				this._soundscape = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x060009C3 RID: 2499 RVA: 0x00020049 File Offset: 0x0001E249
	// (set) Token: 0x060009C4 RID: 2500 RVA: 0x00020051 File Offset: 0x0001E251
	public bool IsChallengeRemindersEnabledSetting
	{
		get
		{
			return this._isChallengeRemindersEnabledSetting;
		}
		set
		{
			if (this._isChallengeRemindersEnabledSetting != value)
			{
				this._isChallengeRemindersEnabledSetting = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x060009C5 RID: 2501 RVA: 0x00020069 File Offset: 0x0001E269
	// (set) Token: 0x060009C6 RID: 2502 RVA: 0x00020071 File Offset: 0x0001E271
	public bool IsContentRemindersEnabledSetting
	{
		get
		{
			return this._isContentRemindersEnabledSetting;
		}
		set
		{
			if (this._isContentRemindersEnabledSetting != value)
			{
				this._isContentRemindersEnabledSetting = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00020089 File Offset: 0x0001E289
	public void OnCreatedInScope(IScope scope)
	{
		this._antiAliasingLevel = this._hardwareCapabilities.DefaultAntiAliasingLevel;
		if (this._antiAliasingLevel > 0)
		{
			this._isUsingDefaultAntiAliasing = false;
		}
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x000200AC File Offset: 0x0001E2AC
	protected override void LoadFromJson(JSON.Dictionary asJson)
	{
		base.LoadFromJson(asJson);
		this._colorfulOption = asJson.GetString("ColorfulOption");
		this._isNightModeEnabled = asJson.GetBool("NightMode", false);
		this._antiAliasingLevel = asJson.GetInt("AntiAliasingLevel", this._hardwareCapabilities.DefaultAntiAliasingLevel);
		this._isUsingDefaultAntiAliasing = asJson.GetBool("IsDefaultAntiAliasing", this._hardwareCapabilities.DefaultAntiAliasingLevel == 0);
		this._volumeSetting = asJson.GetInt("VolumeSetting", 3);
		this._soundscape = asJson.GetInt("Soundscape", 2);
		this._isChallengeRemindersEnabledSetting = asJson.GetBool("IsChallengeRemindersEnabled", false);
		this._isContentRemindersEnabledSetting = asJson.GetBool("IsContentRemindersEnabled", false);
		this._zoomLevel = asJson.GetInt("ZoomLevel", 2);
		this._isZoomEnabled = asJson.GetBool("TouchZoomEnabled", true);
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00020190 File Offset: 0x0001E390
	protected override void SaveToJson(Dictionary<string, object> jsonDictionary)
	{
		base.SaveToJson(jsonDictionary);
		jsonDictionary["ColorfulOption"] = this._colorfulOption;
		jsonDictionary["NightMode"] = this._isNightModeEnabled;
		jsonDictionary["AntiAliasingLevel"] = this._antiAliasingLevel;
		jsonDictionary["IsDefaultAntiAliasing"] = this._isUsingDefaultAntiAliasing;
		jsonDictionary["VolumeSetting"] = this._volumeSetting;
		jsonDictionary["Soundscape"] = this._soundscape;
		jsonDictionary["IsChallengeRemindersEnabled"] = this._isChallengeRemindersEnabledSetting;
		jsonDictionary["IsContentRemindersEnabled"] = this._isContentRemindersEnabledSetting;
		jsonDictionary["ZoomLevel"] = this._zoomLevel;
		jsonDictionary["TouchZoomEnabled"] = this._isZoomEnabled;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0002027C File Offset: 0x0001E47C
	protected override void MergeValues(ForwardCompatibleJsonSaveData otherSaveData)
	{
		base.MergeValues(otherSaveData);
		MotorwaysDeviceSettings otherDeviceSettings = otherSaveData as MotorwaysDeviceSettings;
		if (otherDeviceSettings != null)
		{
			this.ColorfulOption = base.ChooseLatest<string>(this._colorfulOption, otherDeviceSettings._colorfulOption, otherDeviceSettings.UtcTimestamp);
			this.IsNightModeEnabled = base.ChooseLatest<bool>(this._isNightModeEnabled, otherDeviceSettings._isNightModeEnabled, otherDeviceSettings.UtcTimestamp);
			this.VolumeSetting = base.ChooseLatest<int>(this._volumeSetting, otherDeviceSettings._volumeSetting, otherDeviceSettings.UtcTimestamp);
			this.Soundscape = base.ChooseLatest<int>(this._soundscape, otherDeviceSettings._soundscape, otherDeviceSettings.UtcTimestamp);
			this.IsChallengeRemindersEnabledSetting = base.ChooseLatest<bool>(this._isChallengeRemindersEnabledSetting, otherDeviceSettings._isChallengeRemindersEnabledSetting, otherDeviceSettings.UtcTimestamp);
			this.IsContentRemindersEnabledSetting = base.ChooseLatest<bool>(this._isContentRemindersEnabledSetting, otherDeviceSettings._isContentRemindersEnabledSetting, otherDeviceSettings.UtcTimestamp);
			this.SelectedDisplay = base.ChooseLatest<int>(this._selectedDisplay, otherDeviceSettings._selectedDisplay, otherDeviceSettings.UtcTimestamp);
			this.ZoomLevel = base.ChooseLatest<int>(this._zoomLevel, otherDeviceSettings._zoomLevel, otherDeviceSettings.UtcTimestamp);
			this.IsZoomEnabled = base.ChooseLatest<bool>(this._isZoomEnabled, otherDeviceSettings._isZoomEnabled, otherDeviceSettings.UtcTimestamp);
			if (!this._isUsingDefaultAntiAliasing || !otherDeviceSettings._isUsingDefaultAntiAliasing)
			{
				this._isUsingDefaultAntiAliasing = false;
				this.AntiAliasingLevel = base.ChooseLatest<int>(this._antiAliasingLevel, otherDeviceSettings._antiAliasingLevel, otherDeviceSettings.UtcTimestamp);
			}
		}
	}

	// Token: 0x04000516 RID: 1302
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x04000517 RID: 1303
	[Dependency]
	private IAudioSystem _audioSystem;

	// Token: 0x04000518 RID: 1304
	private string _colorfulOption = "Colorful";

	// Token: 0x04000519 RID: 1305
	private bool _isNightModeEnabled;

	// Token: 0x0400051A RID: 1306
	private int _antiAliasingLevel;

	// Token: 0x0400051B RID: 1307
	private bool _isUsingDefaultAntiAliasing = true;

	// Token: 0x0400051C RID: 1308
	private int _selectedDisplay;

	// Token: 0x0400051D RID: 1309
	private const bool DefaultZoomEnabled = true;

	// Token: 0x0400051E RID: 1310
	private bool _isZoomEnabled = true;

	// Token: 0x0400051F RID: 1311
	private const int DefaultZoomLevel = 2;

	// Token: 0x04000520 RID: 1312
	private int _zoomLevel = 2;

	// Token: 0x04000521 RID: 1313
	private const int DefaultVolume = 3;

	// Token: 0x04000522 RID: 1314
	private int _volumeSetting = 3;

	// Token: 0x04000523 RID: 1315
	private const int DefaultSoundscape = 2;

	// Token: 0x04000524 RID: 1316
	private int _soundscape = 2;

	// Token: 0x04000525 RID: 1317
	private bool _isChallengeRemindersEnabledSetting = true;

	// Token: 0x04000526 RID: 1318
	private bool _isContentRemindersEnabledSetting = true;

	// Token: 0x04000527 RID: 1319
	private const string ColorfulKey = "ColorfulOption";

	// Token: 0x04000528 RID: 1320
	private const string NightModeKey = "NightMode";

	// Token: 0x04000529 RID: 1321
	private const string AntiAliasingLevelKey = "AntiAliasingLevel";

	// Token: 0x0400052A RID: 1322
	private const string DefaultAntiAliasingLevelKey = "IsDefaultAntiAliasing";

	// Token: 0x0400052B RID: 1323
	private const string VolumeKey = "VolumeSetting";

	// Token: 0x0400052C RID: 1324
	private const string SoundscapeKey = "Soundscape";

	// Token: 0x0400052D RID: 1325
	private const string IsChallengeRemindersEnabledKey = "IsChallengeRemindersEnabled";

	// Token: 0x0400052E RID: 1326
	private const string IsContentRemindersEnabledKey = "IsContentRemindersEnabled";

	// Token: 0x0400052F RID: 1327
	private const string ZoomEnabledKey = "TouchZoomEnabled";

	// Token: 0x04000530 RID: 1328
	private const string ZoomLevelKey = "ZoomLevel";
}
