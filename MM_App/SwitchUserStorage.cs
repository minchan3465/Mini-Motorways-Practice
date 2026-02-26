using System;
using System.Collections.Generic;
using Factory;

// Token: 0x02000116 RID: 278
public class SwitchUserStorage : IPersistentStorageProvider
{
	// Token: 0x060005F0 RID: 1520 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Tick()
	{
	}

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x060005F1 RID: 1521 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x060005F2 RID: 1522 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<PersistentStorageServiceStatus> StatusChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x060005F3 RID: 1523 RVA: 0x0000222C File Offset: 0x0000042C
	public bool RequiresOptionsPanel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x000159D4 File Offset: 0x00013BD4
	public void LoadAll(Action loadCompleteCallback)
	{
		foreach (string filename in this._fileSystem.GetFilesInDirectory(string.Empty))
		{
			string playerId;
			string deviceId;
			IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForFilename(filename, out playerId, out deviceId);
			if (storableTypeHandler == null)
			{
				SwitchUserStorage.Log.Info("Found unrecognised file {0} in Switch user storage.", new object[]
				{
					filename
				});
			}
			else
			{
				byte[] data = this._fileSystem.ReadFile(filename);
				if (data != null)
				{
					IStorable storable = storableTypeHandler.Load(data);
					if (storable == null)
					{
						SwitchUserStorage.Log.Warn("The file {0} was unable to be parsed as the type {1}.", new object[]
						{
							filename,
							storableTypeHandler
						});
					}
					else
					{
						storableTypeHandler.ProcessLoadedStorable(storable, playerId, deviceId);
					}
				}
			}
		}
		if (loadCompleteCallback != null)
		{
			loadCompleteCallback();
		}
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00015AB4 File Offset: 0x00013CB4
	public bool Store(string filename, byte[] data, NamedStoreCompleted storeCompleteCallback)
	{
		bool didStoreSucceed = this._fileSystem.WriteFile(filename, data);
		this._auditTrail.RecordEvent("SwitchFileStorage.Store", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
			metadata["success"] = didStoreSucceed.ToString();
		});
		if (storeCompleteCallback != null)
		{
			storeCompleteCallback(filename, didStoreSucceed ? StoreOperationResult.Succeeded : StoreOperationResult.Failed);
		}
		return didStoreSucceed;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00015B24 File Offset: 0x00013D24
	public bool Delete(string filename)
	{
		this._auditTrail.RecordEvent("SwitchFileStorage.Delete", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
		});
		if (this._fileSystem.DeleteFile(filename))
		{
			return true;
		}
		SwitchUserStorage.Log.Warn("Unable to delete {0}.", new object[]
		{
			filename
		});
		return false;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00015B90 File Offset: 0x00013D90
	public bool DeletePlayer(string playerIdToDelete)
	{
		foreach (string filename in this._fileSystem.GetFilesInDirectory(string.Empty))
		{
			string playerId;
			string text;
			if (this._storableTypeHandlerRegistry.IsFilenameRecognized(filename, out playerId, out text) && playerId == playerIdToDelete)
			{
				this.Delete(filename);
			}
		}
		return true;
	}

	// Token: 0x04000284 RID: 644
	[Dependency]
	private IFileSystem _fileSystem;

	// Token: 0x04000285 RID: 645
	[Dependency]
	private IStorableTypeHandlerRegistry _storableTypeHandlerRegistry;

	// Token: 0x04000286 RID: 646
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x04000287 RID: 647
	private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("SwitchUserStorage");
}
