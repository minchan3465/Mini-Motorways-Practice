using System;
using System.Collections.Generic;
using System.Diagnostics;
using Factory;

// Token: 0x0200021D RID: 541
public class PersistentStorageService : IPersistentStorageService, ICreatedInScopeHandler
{
	// Token: 0x170002CE RID: 718
	// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x00029EC2 File Offset: 0x000280C2
	public bool RequiresOptionsPanel
	{
		get
		{
			return this._provider.RequiresOptionsPanel;
		}
	}

	// Token: 0x1400002F RID: 47
	// (add) Token: 0x06000CE6 RID: 3302 RVA: 0x00029ED0 File Offset: 0x000280D0
	// (remove) Token: 0x06000CE7 RID: 3303 RVA: 0x00029F08 File Offset: 0x00028108
	public event Action<PersistentStorageServiceStatus> StatusChanged;

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00029F40 File Offset: 0x00028140
	public void LoadAll(Action loadCompletedCallback)
	{
		if (!this._hasRegisteredTick)
		{
			this._hasRegisteredTick = true;
			this._tickRegistry.AppTicking += this.Tick;
		}
		if (loadCompletedCallback != null)
		{
			this._loadCallbacks.Add(loadCompletedCallback);
		}
		using (this._auditTrail.OpenEvent("IPersistentStorageProvider.LoadAll", null))
		{
			this._provider.LoadAll(new Action(this.OnLoadCompleted));
		}
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00029FC8 File Offset: 0x000281C8
	public bool Store(IStorable storable, StoreCompleted storeCompletedCallback)
	{
		IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForStorable(storable);
		if (storableTypeHandler == null)
		{
			return false;
		}
		string filename = storableTypeHandler.GetFilename(storable);
		if (string.IsNullOrEmpty(filename))
		{
			PersistentStorageService.Log.Error("Unable to find type handler for storable {0}.", new object[]
			{
				storable
			});
			return false;
		}
		this._auditTrail.RecordEvent("PersistentStorageService.Store", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
			metadata["stackTrace"] = new StackTrace(4).ToString();
		});
		this._storablesToWrite[filename] = storable;
		this._filenamesToDelete.Remove(filename);
		string playerId;
		string text;
		if (this._playersToDelete.Count > 0 && storableTypeHandler.IsFilenameRecognized(filename, out playerId, out text))
		{
			this._playersToDelete.Remove(playerId);
		}
		if (storeCompletedCallback != null)
		{
			List<StoreCompleted> callbacks;
			if (!this._storeCallbacks.TryGetValue(filename, out callbacks))
			{
				callbacks = new List<StoreCompleted>();
				this._storeCallbacks.Add(filename, callbacks);
			}
			callbacks.Add(storeCompletedCallback);
		}
		return true;
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x0002A0CC File Offset: 0x000282CC
	public bool Delete(IStorable storable)
	{
		IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForStorable(storable);
		if (storableTypeHandler == null)
		{
			return false;
		}
		string filename = storableTypeHandler.GetFilename(storable);
		if (string.IsNullOrEmpty(filename))
		{
			return false;
		}
		this._auditTrail.RecordEvent("PersistentStorageService.Delete", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
			metadata["stackTrace"] = new StackTrace(4).ToString();
		});
		this._filenamesToDelete.Add(filename);
		this._storablesToWrite.Remove(filename);
		List<StoreCompleted> callbacks;
		if (this._storeCallbacks.TryGetValue(filename, out callbacks))
		{
			foreach (StoreCompleted storeCompleted in callbacks)
			{
				storeCompleted(StoreOperationResult.Cancelled);
			}
			this._storeCallbacks.Remove(filename);
		}
		return true;
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x0002A1B4 File Offset: 0x000283B4
	public void DeletePlayer(string playerId)
	{
		this._auditTrail.RecordEvent("PersistentStorageService.DeletePlayer", delegate(Dictionary<string, string> metadata)
		{
			metadata["playerId"] = playerId;
			metadata["stackTrace"] = new StackTrace(4).ToString();
		});
		this._playersToDelete.Add(playerId);
	}

	// Token: 0x170002CF RID: 719
	// (get) Token: 0x06000CEC RID: 3308 RVA: 0x0002A1FC File Offset: 0x000283FC
	public PersistentStorageServiceStatus Status
	{
		get
		{
			return this._status;
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x0002A204 File Offset: 0x00028404
	public void OnCreatedInScope(IScope scope)
	{
		this._provider.StatusChanged += this.OnStatusChanged;
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x0002A220 File Offset: 0x00028420
	private void Tick(float deltaTime)
	{
		if (this._hasProviderCompletedInitialLoad)
		{
			if (this._filenamesToDelete.Count > 0)
			{
				using (HashSet<string>.Enumerator enumerator = this._filenamesToDelete.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string filenameToDelete = enumerator.Current;
						using (this._auditTrail.OpenEvent("IPersistentStorageProvider.Delete", delegate(Dictionary<string, string> metadata)
						{
							metadata["filename"] = filenameToDelete;
						}))
						{
							if (!this._provider.Delete(filenameToDelete))
							{
								PersistentStorageService.Log.Warn("Failed to delete {0}. This is being ignored for now but is not ideal.", Array.Empty<object>());
							}
						}
					}
				}
				this._filenamesToDelete.Clear();
			}
			if (this._playersToDelete.Count > 0)
			{
				using (HashSet<string>.Enumerator enumerator = this._playersToDelete.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string playerToDelete = enumerator.Current;
						using (this._auditTrail.OpenEvent("IPersistentStorageProvider.DeletePlayer", delegate(Dictionary<string, string> metadata)
						{
							metadata["playerId"] = playerToDelete;
						}))
						{
							if (!this._provider.DeletePlayer(playerToDelete))
							{
								PersistentStorageService.Log.Warn("Failed to delete storables for player {0}. This is being ignored for now but is not ideal.", Array.Empty<object>());
							}
						}
					}
				}
				this._playersToDelete.Clear();
			}
			if (this._storablesToWrite.Count > 0)
			{
				foreach (KeyValuePair<string, IStorable> entry in this._storablesToWrite)
				{
					string filename = entry.Key;
					IStorable storable = entry.Value;
					using (this._auditTrail.OpenEvent("IPersistentStorageProvider.Store", delegate(Dictionary<string, string> metadata)
					{
						metadata["filename"] = filename;
					}))
					{
						IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForStorable(storable);
						if (storableTypeHandler == null)
						{
							PersistentStorageService.Log.Warn("Failed to type handler for storable {0}.", new object[]
							{
								storable
							});
						}
						else
						{
							byte[] data = storableTypeHandler.Store(storable);
							if (data == null)
							{
								PersistentStorageService.Log.Warn("Failed to store {0} as bytes. Data loss may occur", new object[]
								{
									storable
								});
							}
							else if (!this._provider.Store(filename, data, new NamedStoreCompleted(this.OnStoreCompleted)))
							{
								PersistentStorageService.Log.Warn("Failed to store {0} as {1}. Data loss may occur.", new object[]
								{
									storable,
									filename
								});
							}
						}
					}
				}
				this._storablesToWrite.Clear();
			}
		}
		this._provider.Tick();
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x0002A560 File Offset: 0x00028760
	private void OnLoadCompleted()
	{
		this._hasProviderCompletedInitialLoad = true;
		foreach (Action action in this._loadCallbacks)
		{
			action();
		}
		this._loadCallbacks.Clear();
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x0002A5C4 File Offset: 0x000287C4
	private void OnStoreCompleted(string filename, StoreOperationResult result)
	{
		PersistentStorageService.Log.Info("Write to {0} completed with result {1}", new object[]
		{
			filename,
			result.ToString()
		});
		this._auditTrail.RecordEvent("PersistentStorageService.OnStoreCompleted", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
			metadata["result"] = result.ToString();
		});
		List<StoreCompleted> callbacks;
		if (this._storeCallbacks.TryGetValue(filename, out callbacks))
		{
			foreach (StoreCompleted storeCompleted in callbacks)
			{
				storeCompleted(result);
			}
			this._storeCallbacks.Remove(filename);
		}
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x0002A6A0 File Offset: 0x000288A0
	private void OnStatusChanged(PersistentStorageServiceStatus status)
	{
		this._auditTrail.RecordEvent("PersistentStorageService.OnStatusChanged", delegate(Dictionary<string, string> metadata)
		{
			if (this._status.issues != status.issues)
			{
				metadata["oldIssues"] = this._status.issues.ToString();
				metadata["newIssues"] = status.issues.ToString();
			}
			if (this._status.messageKey != status.messageKey)
			{
				metadata["oldMessage"] = this._status.messageKey;
				metadata["newMessage"] = status.messageKey;
			}
		});
		this._status = status;
		Action<PersistentStorageServiceStatus> statusChanged = this.StatusChanged;
		if (statusChanged == null)
		{
			return;
		}
		statusChanged(status);
	}

	// Token: 0x0400073A RID: 1850
	private PersistentStorageServiceStatus _status;

	// Token: 0x0400073B RID: 1851
	private bool _hasRegisteredTick;

	// Token: 0x0400073C RID: 1852
	private bool _hasProviderCompletedInitialLoad;

	// Token: 0x0400073D RID: 1853
	private readonly List<Action> _loadCallbacks = new List<Action>();

	// Token: 0x0400073E RID: 1854
	private readonly Dictionary<string, IStorable> _storablesToWrite = new Dictionary<string, IStorable>();

	// Token: 0x0400073F RID: 1855
	private readonly Dictionary<string, List<StoreCompleted>> _storeCallbacks = new Dictionary<string, List<StoreCompleted>>();

	// Token: 0x04000740 RID: 1856
	private readonly HashSet<string> _filenamesToDelete = new HashSet<string>();

	// Token: 0x04000741 RID: 1857
	private readonly HashSet<string> _playersToDelete = new HashSet<string>();

	// Token: 0x04000743 RID: 1859
	[Dependency]
	private TickRegistry _tickRegistry;

	// Token: 0x04000744 RID: 1860
	[Dependency]
	private IStorableTypeHandlerRegistry _storableTypeHandlerRegistry;

	// Token: 0x04000745 RID: 1861
	[Dependency]
	private IPersistentStorageProvider _provider;

	// Token: 0x04000746 RID: 1862
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x04000747 RID: 1863
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("PersistentStorageService");
}
