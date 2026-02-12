using System;

// Token: 0x02000210 RID: 528
public interface IPersistentStorageProvider
{
	// Token: 0x06000CA9 RID: 3241
	void Tick();

	// Token: 0x06000CAA RID: 3242
	void LoadAll(Action loadCompleteCallback);

	// Token: 0x06000CAB RID: 3243
	bool Store(string filename, byte[] data, NamedStoreCompleted storeCompleteCallback);

	// Token: 0x06000CAC RID: 3244
	bool Delete(string filename);

	// Token: 0x06000CAD RID: 3245
	bool DeletePlayer(string playerId);

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x06000CAE RID: 3246
	// (remove) Token: 0x06000CAF RID: 3247
	event Action<PersistentStorageServiceStatus> StatusChanged;

	// Token: 0x170002C5 RID: 709
	// (get) Token: 0x06000CB0 RID: 3248
	bool RequiresOptionsPanel { get; }
}
