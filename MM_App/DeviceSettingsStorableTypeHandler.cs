using System;
using Factory;

// Token: 0x02000201 RID: 513
public class DeviceSettingsStorableTypeHandler : IStorableTypeHandler
{
	// Token: 0x06000C20 RID: 3104 RVA: 0x0002947C File Offset: 0x0002767C
	public string GetFilename(IStorable storable)
	{
		IDeviceSettings deviceSettings = storable as IDeviceSettings;
		if (deviceSettings != null && Diagnostics.Verify(deviceSettings.Player != null, "You can't save a DeviceSettings that hasn't been assigned to a Player."))
		{
			return this.GetFilename(deviceSettings.Player.Id, this._hardwareCapabilities.UniqueDeviceId);
		}
		return null;
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x000294C6 File Offset: 0x000276C6
	public string GetFilename(string playerId, string deviceId)
	{
		return StorableUtilities.GenerateFilename("deviceSettings_", ".json", playerId, deviceId);
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x000294D9 File Offset: 0x000276D9
	public bool IsFilenameRecognized(string filename, out string playerId, out string deviceId)
	{
		return StorableUtilities.TryParseFilename(filename, "deviceSettings_", ".json", out playerId, out deviceId);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x000294F0 File Offset: 0x000276F0
	public IStorable Load(byte[] data)
	{
		IDeviceSettings newDeviceSettings = this._scope.Get<IDeviceSettings>();
		if (!StorableUtilities.LoadJsonStorable(newDeviceSettings, data))
		{
			this._scope.Release(newDeviceSettings);
			return null;
		}
		return newDeviceSettings;
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x00029524 File Offset: 0x00027724
	public byte[] Store(IStorable storable)
	{
		IDeviceSettings deviceSettings = storable as IDeviceSettings;
		if (deviceSettings != null)
		{
			return StorableUtilities.StoreJsonStorable(deviceSettings);
		}
		return null;
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x00029544 File Offset: 0x00027744
	public bool ProcessLoadedStorable(IStorable storable, string playerId, string deviceId)
	{
		IDeviceSettings deviceSettings = storable as IDeviceSettings;
		if (deviceSettings != null)
		{
			this._playerDatabase.AddDeviceSettings(deviceSettings, playerId, deviceId);
			return true;
		}
		return false;
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x000022F5 File Offset: 0x000004F5
	public void ProcessDeletedStorable(string playerId, string deviceId)
	{
	}

	// Token: 0x04000709 RID: 1801
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x0400070A RID: 1802
	[Dependency]
	private PlayerDatabase _playerDatabase;

	// Token: 0x0400070B RID: 1803
	[Dependency]
	private IScope _scope;

	// Token: 0x0400070C RID: 1804
	private const string FilenamePrefix = "deviceSettings_";

	// Token: 0x0400070D RID: 1805
	private const string FilenameExtension = ".json";
}
