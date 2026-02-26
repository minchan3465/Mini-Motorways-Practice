using System;

// Token: 0x02000114 RID: 276
public class NullStorage : IPersistentStorageProvider
{
	// Token: 0x060005E2 RID: 1506 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Tick()
	{
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000157F0 File Offset: 0x000139F0
	public void LoadAll(Action loadCompleteCallback)
	{
		if (loadCompleteCallback != null)
		{
			loadCompleteCallback();
		}
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x000157FB File Offset: 0x000139FB
	public bool Store(string filename, byte[] data, NamedStoreCompleted storeCompleted)
	{
		storeCompleted(filename, StoreOperationResult.Failed);
		return false;
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x0000222C File Offset: 0x0000042C
	public bool Delete(string filename)
	{
		return false;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0000222C File Offset: 0x0000042C
	public bool DeletePlayer(string playerId)
	{
		return false;
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x060005E7 RID: 1511 RVA: 0x0000222C File Offset: 0x0000042C
	public bool RequiresOptionsPanel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x14000017 RID: 23
	// (add) Token: 0x060005E8 RID: 1512 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x060005E9 RID: 1513 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<PersistentStorageServiceStatus> StatusChanged
	{
		add
		{
		}
		remove
		{
		}
	}
}
