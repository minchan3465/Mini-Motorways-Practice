using System;
using System.Collections.Generic;

// Token: 0x020001FD RID: 509
public class BaseDeviceSettings : ForwardCompatibleJsonSaveData, IDeviceSettings, IJsonSerializableSaveData, IStorable
{
	// Token: 0x1700028E RID: 654
	// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x00028A02 File Offset: 0x00026C02
	// (set) Token: 0x06000BE8 RID: 3048 RVA: 0x00028A0A File Offset: 0x00026C0A
	public Player Player { get; set; }

	// Token: 0x1700028F RID: 655
	// (get) Token: 0x06000BE9 RID: 3049 RVA: 0x00028A13 File Offset: 0x00026C13
	// (set) Token: 0x06000BEA RID: 3050 RVA: 0x00028A1B File Offset: 0x00026C1B
	public DateTime LastPlayedUtcTime
	{
		get
		{
			return this._lastPlayedUtcTimestamp;
		}
		set
		{
			if (value > this._lastPlayedUtcTimestamp)
			{
				this._lastPlayedUtcTimestamp = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000290 RID: 656
	// (get) Token: 0x06000BEB RID: 3051 RVA: 0x00028A38 File Offset: 0x00026C38
	// (set) Token: 0x06000BEC RID: 3052 RVA: 0x00028A40 File Offset: 0x00026C40
	public LocaleDatabase.LocaleId LastLocaleId
	{
		get
		{
			return this._lastLocaleId;
		}
		set
		{
			if (this._lastLocaleId != value)
			{
				this._lastLocaleId = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x17000291 RID: 657
	// (get) Token: 0x06000BED RID: 3053 RVA: 0x00028A58 File Offset: 0x00026C58
	// (set) Token: 0x06000BEE RID: 3054 RVA: 0x00028A60 File Offset: 0x00026C60
	public bool SyncToCloud
	{
		get
		{
			return this._syncToCloud;
		}
		set
		{
			if (this._syncToCloud != value)
			{
				this._syncToCloud = value;
				base.OnValueChanged();
			}
		}
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x00028A78 File Offset: 0x00026C78
	protected override void LoadFromJson(JSON.Dictionary asJson)
	{
		this._version = asJson.GetInt("_version", 0);
		if (this._version == 1)
		{
			this._version = 0;
		}
		this._lastLocaleId = LocaleDatabase.LocaleId.Unknown;
		string lastLocaleStringId = asJson.GetString("LastLocaleId");
		Diagnostics.Verify(Enum.TryParse<LocaleDatabase.LocaleId>(lastLocaleStringId, out this._lastLocaleId), "Failed to parse the last locale from the device settings. Found the string '{0}'.", lastLocaleStringId);
		this._lastPlayedUtcTimestamp = asJson.GetDateTime("LastPlayedUtcTimestamp");
		this._syncToCloud = asJson.GetBool("SyncToCloud", false);
		JSON.Dictionary deviceNameToMappings = asJson.GetDictionary("_controllerDeviceNameToMappings");
		if (this._version < 2)
		{
			deviceNameToMappings = null;
		}
		if (deviceNameToMappings != null)
		{
			foreach (string deviceName in deviceNameToMappings.Keys)
			{
				JSON.Dictionary deviceMappings = deviceNameToMappings.GetDictionary(deviceName);
				Dictionary<string, string> deviceMappingsCSDictionary = new Dictionary<string, string>();
				if (deviceMappings != null)
				{
					foreach (string logicalAction in deviceMappings.Keys)
					{
						deviceMappingsCSDictionary.Add(logicalAction, deviceMappings.GetString(logicalAction));
					}
				}
				this._controllerDeviceNameToMappings.Add(deviceName, deviceMappingsCSDictionary);
			}
		}
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x00028BC4 File Offset: 0x00026DC4
	protected override void SaveToJson(Dictionary<string, object> jsonDictionary)
	{
		jsonDictionary["_version"] = this._version;
		jsonDictionary["LastPlayedUtcTimestamp"] = this._lastPlayedUtcTimestamp;
		jsonDictionary["LastLocaleId"] = this._lastLocaleId.ToString();
		jsonDictionary["SyncToCloud"] = this._syncToCloud;
		jsonDictionary["_controllerDeviceNameToMappings"] = this._controllerDeviceNameToMappings;
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x00028C40 File Offset: 0x00026E40
	protected override void MergeValues(ForwardCompatibleJsonSaveData otherSaveData)
	{
		BaseDeviceSettings otherDeviceSettings = otherSaveData as BaseDeviceSettings;
		if (otherDeviceSettings != null)
		{
			this.LastPlayedUtcTime = base.ChooseMax<DateTime>(this._lastPlayedUtcTimestamp, otherDeviceSettings._lastPlayedUtcTimestamp);
			this.SyncToCloud = base.ChooseLatest<bool>(this._syncToCloud, otherDeviceSettings._syncToCloud, otherDeviceSettings.UtcTimestamp);
			this.LastLocaleId = base.ChooseLatest<LocaleDatabase.LocaleId>(this._lastLocaleId, otherDeviceSettings.LastLocaleId, otherDeviceSettings.UtcTimestamp);
		}
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00028CAB File Offset: 0x00026EAB
	public Dictionary<string, string> GetDeviceControlMapping(string deviceName)
	{
		if (this._controllerDeviceNameToMappings.ContainsKey(deviceName))
		{
			return this._controllerDeviceNameToMappings[deviceName];
		}
		return null;
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x00028CCC File Offset: 0x00026ECC
	public void SetDeviceControlMappings(string deviceName, Dictionary<string, string> deviceControlMappings)
	{
		if (this._controllerDeviceNameToMappings.ContainsKey(deviceName))
		{
			bool hasChanges = deviceControlMappings.Count != this._controllerDeviceNameToMappings[deviceName].Count;
			if (!hasChanges)
			{
				foreach (KeyValuePair<string, string> currentSetting in this._controllerDeviceNameToMappings[deviceName])
				{
					if (!deviceControlMappings.ContainsKey(currentSetting.Key) || deviceControlMappings[currentSetting.Key] != currentSetting.Value)
					{
						hasChanges = true;
						break;
					}
				}
			}
			if (hasChanges)
			{
				this._controllerDeviceNameToMappings[deviceName] = deviceControlMappings;
				base.OnValueChanged();
				return;
			}
		}
		else
		{
			this._controllerDeviceNameToMappings.Add(deviceName, deviceControlMappings);
			base.OnValueChanged();
		}
	}

	// Token: 0x17000292 RID: 658
	// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x00028DA8 File Offset: 0x00026FA8
	private static int LatestVersion
	{
		get
		{
			return 2;
		}
	}

	// Token: 0x040006E0 RID: 1760
	private int _version = BaseDeviceSettings.LatestVersion;

	// Token: 0x040006E1 RID: 1761
	private DateTime _lastPlayedUtcTimestamp = DateTime.MinValue;

	// Token: 0x040006E2 RID: 1762
	private LocaleDatabase.LocaleId _lastLocaleId;

	// Token: 0x040006E3 RID: 1763
	private bool _syncToCloud = true;

	// Token: 0x040006E4 RID: 1764
	private readonly Dictionary<string, Dictionary<string, string>> _controllerDeviceNameToMappings = new Dictionary<string, Dictionary<string, string>>();

	// Token: 0x040006E5 RID: 1765
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("BaseDeviceSettings");

	// Token: 0x040006E6 RID: 1766
	private const string VersionKey = "_version";

	// Token: 0x040006E7 RID: 1767
	private const string LastPlayedUtcTimestampKey = "LastPlayedUtcTimestamp";

	// Token: 0x040006E8 RID: 1768
	private const string LocaleKey = "LastLocaleId";

	// Token: 0x040006E9 RID: 1769
	private const string CloudKey = "SyncToCloud";

	// Token: 0x040006EA RID: 1770
	private const string ControllerMappingsKey = "_controllerDeviceNameToMappings";

	// Token: 0x020001FE RID: 510
	public enum DeviceSettingsSerializationVersion
	{
		// Token: 0x040006ED RID: 1773
		InitialVersion,
		// Token: 0x040006EE RID: 1774
		DummyVersionFixup,
		// Token: 0x040006EF RID: 1775
		AddedSiriRemote,
		// Token: 0x040006F0 RID: 1776
		Count
	}
}
