using System;

// Token: 0x02000212 RID: 530
public interface IPersistentStorageService
{
	// Token: 0x06000CB5 RID: 3253
	void LoadAll(Action loadCompletedCallback);

	// Token: 0x06000CB6 RID: 3254
	bool Store(IStorable storable, StoreCompleted storeCompletedCallback = null);

	// Token: 0x06000CB7 RID: 3255
	bool Delete(IStorable storable);

	// Token: 0x06000CB8 RID: 3256
	void DeletePlayer(string playerId);

	// Token: 0x170002C6 RID: 710
	// (get) Token: 0x06000CB9 RID: 3257
	PersistentStorageServiceStatus Status { get; }

	// Token: 0x170002C7 RID: 711
	// (get) Token: 0x06000CBA RID: 3258
	bool RequiresOptionsPanel { get; }

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x06000CBB RID: 3259
	// (remove) Token: 0x06000CBC RID: 3260
	event Action<PersistentStorageServiceStatus> StatusChanged;
}
