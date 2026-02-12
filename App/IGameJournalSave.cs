using System;

// Token: 0x0200020C RID: 524
public interface IGameJournalSave : IBinarySerializableSaveData, IStorable
{
	// Token: 0x170002C0 RID: 704
	// (get) Token: 0x06000C98 RID: 3224
	// (set) Token: 0x06000C99 RID: 3225
	Player Player { get; set; }

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06000C9A RID: 3226
	// (set) Token: 0x06000C9B RID: 3227
	string DeviceId { get; set; }

	// Token: 0x170002C2 RID: 706
	// (get) Token: 0x06000C9C RID: 3228
	bool CanDelete { get; }
}
