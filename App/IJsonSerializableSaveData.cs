using System;
using System.Collections.Generic;

// Token: 0x0200020E RID: 526
public interface IJsonSerializableSaveData : IStorable
{
	// Token: 0x06000C9D RID: 3229
	void InitializeWithJson(JSON.Dictionary jsonSaveData);

	// Token: 0x06000C9E RID: 3230
	Dictionary<string, object> SerializeToJson();

	// Token: 0x06000C9F RID: 3231
	void Merge(IJsonSerializableSaveData otherData, bool autosave = true);

	// Token: 0x1400002C RID: 44
	// (add) Token: 0x06000CA0 RID: 3232
	// (remove) Token: 0x06000CA1 RID: 3233
	event Action DataChanged;
}
