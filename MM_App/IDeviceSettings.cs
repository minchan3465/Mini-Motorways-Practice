using System;
using System.Collections.Generic;

// Token: 0x0200020A RID: 522
public interface IDeviceSettings : IJsonSerializableSaveData, IStorable
{
	// Token: 0x170002B5 RID: 693
	// (get) Token: 0x06000C7C RID: 3196
	// (set) Token: 0x06000C7D RID: 3197
	Player Player { get; set; }

	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x06000C7E RID: 3198
	// (set) Token: 0x06000C7F RID: 3199
	DateTime LastPlayedUtcTime { get; set; }

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x06000C80 RID: 3200
	// (set) Token: 0x06000C81 RID: 3201
	LocaleDatabase.LocaleId LastLocaleId { get; set; }

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06000C82 RID: 3202
	// (set) Token: 0x06000C83 RID: 3203
	bool SyncToCloud { get; set; }

	// Token: 0x06000C84 RID: 3204
	Dictionary<string, string> GetDeviceControlMapping(string deviceName);

	// Token: 0x06000C85 RID: 3205
	void SetDeviceControlMappings(string deviceName, Dictionary<string, string> deviceControlMappings);
}
