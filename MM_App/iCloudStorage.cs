using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using Factory;

// Token: 0x020000FB RID: 251
public class iCloudStorage : IPersistentStorageProvider, ICreatedInScopeHandler, IReleasedFromScopeHandler
{
	// Token: 0x0600055D RID: 1373 RVA: 0x00012DE4 File Offset: 0x00010FE4
	public void Tick()
	{
		if (!this._hasInitialLoadCompleted)
		{
			if (this.CanNotifyInitialLoad)
			{
				iCloudStorage.Log.Info("Processing data for initial load.", Array.Empty<object>());
				this.MigrateLegacyFiles();
				if (this.IsSignedIn)
				{
					iCloudStorage.Log.Info("Device is signed in to iCloud.", Array.Empty<object>());
					this.LoadiCloudFiles();
					if (this.HasRecentlyModifiedIcloudData(this._userId, false))
					{
						this.SetStatusIssues(PersistentStorageServiceIssues.AuthenticatedButOtherUsersiCloudData);
					}
					using (IEnumerator<Player> enumerator = this._playerDatabase.Players.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Player player = enumerator.Current;
							IExtendedUserProfile extendedProfile = player.ExtendedUserProfile;
							if (extendedProfile.iCloudProvenance == iCloudProvenance.Unknown)
							{
								iCloudStorage.Log.Info("Confirming provenance of migrated player '{0}'.", new object[]
								{
									player.Id
								});
								DateTime originalTimestamp = extendedProfile.UtcTimestamp;
								extendedProfile.iCloudProvenance = iCloudProvenance.Confirmed;
								extendedProfile.UtcTimestamp = originalTimestamp;
								this._auditTrail.RecordEvent("iCloudStorage.ConfirmProvenance", delegate(Dictionary<string, string> metadata)
								{
									metadata["playerId"] = player.Id;
								});
							}
						}
					}
					foreach (string legacyFilename in this._localCache.GetFilenamesInDirectory(iCloudStorage.LegacyCachePath))
					{
						string playerId;
						string text;
						if (this._storableTypeHandlerRegistry.IsFilenameRecognized(legacyFilename, out playerId, out text))
						{
							Player player2 = this._playerDatabase.GetPlayer(playerId);
							if (player2 != null)
							{
								iCloudStorage.Log.Info("Importing legacy file '{0}'.", new object[]
								{
									legacyFilename
								});
								this.LoadStorableFromCache(iCloudStorage.LegacyCachePath, legacyFilename);
								if (player2.ExtendedUserProfile.iCloudProvenance == iCloudProvenance.Confirmed)
								{
									iCloudStorage.Log.Info("Deleting legacy file '{0}' because its provenance has been confirmed.", new object[]
									{
										legacyFilename
									});
									string legacyFilepath = Path.Combine(iCloudStorage.LegacyCachePath, legacyFilename);
									this._localCache.DeleteFile(legacyFilepath);
									this._auditTrail.RecordEvent("iCloudCache.DeleteFile", delegate(Dictionary<string, string> metadata)
									{
										metadata["filepath"] = legacyFilepath;
									});
								}
							}
							else
							{
								iCloudStorage.Log.Info("Legacy file '{0}' was found, but ignoring because it does not match any known iCloud player.", new object[]
								{
									legacyFilename
								});
							}
						}
					}
					this.LoadCachedFiles(this.iCloudUserCachePath);
					if (this._playerDatabase.PlayerCount != 0)
					{
						goto IL_411;
					}
					iCloudStorage.Log.Info("No players imported.", Array.Empty<object>());
					if (this._reachability.Connectivity == InternetConnectivity.Connected && this._hasLoadCompleted && this._wasLoadSuccessful)
					{
						goto IL_411;
					}
					iCloudStorage.Log.Info("This device cannot connect to iCloud, so all legacy files will be assumed to be owned by the current iCloud account.", Array.Empty<object>());
					using (this._auditTrail.OpenEvent("iCloudStorage.CopyLegacyFiles", delegate(Dictionary<string, string> metadata)
					{
						metadata["toDirectory"] = this.iCloudUserCachePath;
					}))
					{
						this._localCache.CopyNewFilesInDirectory(iCloudStorage.LegacyCachePath, this.iCloudUserCachePath);
					}
					this.LoadCachedFiles(this.iCloudUserCachePath);
					using (IEnumerator<Player> enumerator = this._playerDatabase.Players.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Player player = enumerator.Current;
							iCloudStorage.Log.Info("Marking the provenance of legacy player '{0}' as presumed.", new object[]
							{
								player.Id
							});
							IExtendedUserProfile extendedUserProfile = player.ExtendedUserProfile;
							DateTime originalTimestamp2 = extendedUserProfile.UtcTimestamp;
							extendedUserProfile.iCloudProvenance = iCloudProvenance.Presumed;
							extendedUserProfile.UtcTimestamp = originalTimestamp2;
							this._auditTrail.RecordEvent("iCloudStorage.PresumeProvenance", delegate(Dictionary<string, string> metadata)
							{
								metadata["playerId"] = player.Id;
							});
						}
						goto IL_411;
					}
				}
				iCloudStorage.Log.Info("Device is not signed in to iCloud.", Array.Empty<object>());
				if (this.HasRecentlyModifiedIcloudData(string.Empty, false))
				{
					this.SetStatusIssues(PersistentStorageServiceIssues.RecentUnauthenticatedData);
				}
				using (this._auditTrail.OpenEvent("iCloudStorage.CopyLegacyFiles", delegate(Dictionary<string, string> metadata)
				{
					metadata["toDirectory"] = iCloudStorage.UnsyncedCachePath;
				}))
				{
					this._localCache.CopyNewFilesInDirectory(iCloudStorage.LegacyCachePath, iCloudStorage.UnsyncedCachePath);
				}
				this.LoadCachedFiles(iCloudStorage.UnsyncedCachePath);
				IL_411:
				this._hasInitialLoadCompleted = true;
				if (this._loadCallback != null)
				{
					this._loadCallback();
					this._loadCallback = null;
				}
			}
			return;
		}
		if (this._haveFilesChanged)
		{
			iCloudStorage.Log.Info("Processing changes to iCloud data.", Array.Empty<object>());
			this._haveFilesChanged = false;
			using (this._auditTrail.OpenEvent("iCloudStorage.LoadChangedFiles", null))
			{
				iCloudStorage.iCloudForEachChangedFile(Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudStorage.OnFileChangedDelegate)));
			}
		}
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x00013314 File Offset: 0x00011514
	public void LoadAll(Action loadCompleteCallback)
	{
		if (!this._hasConnected)
		{
			this._kernel.Connect();
			this._hasConnected = true;
			this._kernel.UserChanged += this.OnUserChanged;
			this._kernel.FilesChanged += this.OnFilesChanged;
			this._kernel.LoadCompleted += this.OnLoadCompleted;
			this._kernel.FileDeleted += this.OnFileDeleted;
			this._kernel.UserMessageChanged += this.OnUserMessageChanged;
			this._kernel.FileStored += this.OnFileStored;
		}
		if (!this._hasInitialLoadCompleted)
		{
			this._loadCallback = loadCompleteCallback;
			this._isInitialLoadRequested = true;
			return;
		}
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x000133E0 File Offset: 0x000115E0
	public bool Store(string filename, byte[] data, NamedStoreCompleted storeCompleteCallback)
	{
		if (storeCompleteCallback != null)
		{
			List<NamedStoreCompleted> callbacks;
			if (!this._storeCallbacks.TryGetValue(filename, out callbacks))
			{
				callbacks = new List<NamedStoreCompleted>();
				this._storeCallbacks.Add(filename, callbacks);
			}
			callbacks.Add(storeCompleteCallback);
		}
		GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		bool didiCloudWriteSucceed = iCloudStorage.iCloudWriteFile(filename, dataHandle.AddrOfPinnedObject(), data.Length);
		if (!didiCloudWriteSucceed)
		{
			iCloudStorage.Log.Warn("Failed to write {0} to iCloud.", new object[]
			{
				filename
			});
			if (storeCompleteCallback != null)
			{
				storeCompleteCallback(filename, StoreOperationResult.Failed);
			}
		}
		dataHandle.Free();
		this._auditTrail.RecordEvent("iCloudWriteFile", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
			metadata["didSucceed"] = didiCloudWriteSucceed.ToString();
		});
		string cachedFilepath = Path.Combine(this.ActiveCachePath, filename);
		bool canWriteToCache = true;
		int bytesNeededToDelete;
		if (!this._localCache.HasSpaceToWriteFile(cachedFilepath, data.Length, out bytesNeededToDelete))
		{
			iCloudStorage.Log.Warn("Unable to write {0} ({1} bytes) to the cache. We need to delete {2} bytes.", new object[]
			{
				filename,
				data.Length,
				bytesNeededToDelete
			});
			IStorableTypeHandler savedGameHandler = this._storableTypeHandlerRegistry.GetHandlerForType<IGameJournalSave>();
			if (savedGameHandler != null)
			{
				iCloudStorage.Log.Info("Deleting legacy game journals to free space.", Array.Empty<object>());
				foreach (string legacyFilename in this._localCache.GetFilenamesInDirectory(iCloudStorage.LegacyCachePath))
				{
					string text;
					string text2;
					if (savedGameHandler.IsFilenameRecognized(legacyFilename, out text, out text2))
					{
						string legacyFilepath = Path.Combine(iCloudStorage.LegacyCachePath, legacyFilename);
						int fileSize = this._localCache.GetFileSize(legacyFilepath);
						if (this._localCache.DeleteFile(legacyFilepath))
						{
							iCloudStorage.Log.Info("Deleted {0}, freeing up {1} bytes.", new object[]
							{
								legacyFilepath,
								fileSize
							});
							bytesNeededToDelete -= fileSize;
						}
						else
						{
							iCloudStorage.Log.Warn("Failed to delete {0}!", new object[]
							{
								legacyFilepath
							});
						}
					}
				}
			}
			if (bytesNeededToDelete > 0)
			{
				iCloudStorage.Log.Info("Deleting the local cache for other iCloud accounts.", Array.Empty<object>());
				foreach (string iCloudUserId in this._localCache.GetDirectoriesInDirectory(iCloudStorage.iCloudCachePath))
				{
					if (bytesNeededToDelete <= 0)
					{
						break;
					}
					if (iCloudUserId != this._userId)
					{
						iCloudStorage.Log.Info("Deleting the local cache for iCloud account {0}.", new object[]
						{
							iCloudUserId
						});
						string foreignCachePath = Path.Combine(iCloudStorage.iCloudCachePath, iCloudUserId);
						foreach (string foreignFilename in this._localCache.GetFilenamesInDirectory(foreignCachePath))
						{
							string foreignFilepath = Path.Combine(foreignCachePath, foreignFilename);
							int foreignFileSize = this._localCache.GetFileSize(foreignFilepath);
							if (this._localCache.DeleteFile(foreignFilepath))
							{
								iCloudStorage.Log.Info("Deleted {0}, freeing up {1} bytes.", new object[]
								{
									foreignFilepath,
									foreignFileSize
								});
								bytesNeededToDelete -= foreignFileSize;
							}
							else
							{
								iCloudStorage.Log.Warn("Failed to delete {0}.", new object[]
								{
									foreignFilepath
								});
							}
						}
					}
				}
			}
			if (bytesNeededToDelete > 0 && this.IsSignedIn && savedGameHandler != null)
			{
				iCloudStorage.Log.Info("Deleting the local cache of saved games.", Array.Empty<object>());
				foreach (string existingCachedFilename in this._localCache.GetFilenamesInDirectory(this.ActiveCachePath))
				{
					string text;
					string text2;
					if (existingCachedFilename != filename && savedGameHandler.IsFilenameRecognized(existingCachedFilename, out text2, out text))
					{
						string existingCachedFilepath = Path.Combine(this.ActiveCachePath, existingCachedFilename);
						int existingFileSize = this._localCache.GetFileSize(existingCachedFilepath);
						if (this._localCache.DeleteFile(existingCachedFilepath))
						{
							iCloudStorage.Log.Info("Deleted {0}, freeing up {1} bytes.", new object[]
							{
								existingCachedFilepath,
								existingFileSize
							});
							bytesNeededToDelete -= existingFileSize;
						}
						else
						{
							iCloudStorage.Log.Warn("Failed to delete {0}.", new object[]
							{
								existingCachedFilepath
							});
						}
					}
				}
			}
			canWriteToCache = (bytesNeededToDelete <= 0);
			if (!canWriteToCache)
			{
				iCloudStorage.Log.Warn("Unable to write {0}! We needed to free {1} additional bytes to make space for it.", new object[]
				{
					filename,
					bytesNeededToDelete
				});
			}
		}
		bool didCacheWriteSucceed = false;
		if (canWriteToCache)
		{
			didCacheWriteSucceed = this._localCache.WriteFile(cachedFilepath, data);
			if (!didCacheWriteSucceed)
			{
				iCloudStorage.Log.Warn("Failed to write {0} to the iCloud cache.", new object[]
				{
					filename
				});
			}
		}
		this._auditTrail.RecordEvent("iCloudCache.WriteFile", delegate(Dictionary<string, string> metadata)
		{
			metadata["filepath"] = cachedFilepath;
			metadata["didSucceed"] = didCacheWriteSucceed.ToString();
		});
		return didiCloudWriteSucceed & didCacheWriteSucceed;
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00013948 File Offset: 0x00011B48
	public bool Delete(string filename)
	{
		bool didiCloudDeleteSucceed = iCloudStorage.iCloudDeleteFile(filename);
		if (!didiCloudDeleteSucceed)
		{
			iCloudStorage.Log.Warn("Failed to delete {0} from iCloud.", new object[]
			{
				filename
			});
		}
		this._auditTrail.RecordEvent("iCloudDeleteFile", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
			metadata["didSucceed"] = didiCloudDeleteSucceed.ToString();
		});
		string cachedFilepath = Path.Combine(this.ActiveCachePath, filename);
		bool didCacheDeleteSucceed = this._localCache.DeleteFile(cachedFilepath);
		if (!didCacheDeleteSucceed)
		{
			iCloudStorage.Log.Warn("Failed to delete {0} from the iCloud cache.", new object[]
			{
				filename
			});
		}
		this._auditTrail.RecordEvent("iCloudCache.DeleteFile", delegate(Dictionary<string, string> metadata)
		{
			metadata["filepath"] = cachedFilepath;
			metadata["didSucceed"] = didCacheDeleteSucceed.ToString();
		});
		return didiCloudDeleteSucceed & didCacheDeleteSucceed;
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x00013A34 File Offset: 0x00011C34
	public bool DeletePlayer(string playerIdToDelete)
	{
		this._playerIdToDelete = playerIdToDelete;
		this._filenamesToDelete.Clear();
		iCloudStorage.iCloudForEachFile(Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudStorage.OnFileQueriedForDeletionDelegate)));
		using (List<string>.Enumerator enumerator = this._filenamesToDelete.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string filenameToDelete = enumerator.Current;
				iCloudStorage.iCloudDeleteFile(filenameToDelete);
				this._auditTrail.RecordEvent("iCloudDeleteFile", delegate(Dictionary<string, string> metadata)
				{
					metadata["filename"] = filenameToDelete;
				});
			}
		}
		this._filenamesToDelete.Clear();
		foreach (string cachedFilename in this._localCache.GetFilenamesInDirectory(this.ActiveCachePath))
		{
			string playerId;
			string text;
			if (this._storableTypeHandlerRegistry.IsFilenameRecognized(cachedFilename, out playerId, out text) && playerId == playerIdToDelete)
			{
				string cachedFilepath = Path.Combine(this.ActiveCachePath, cachedFilename);
				this._localCache.DeleteFile(cachedFilepath);
				this._auditTrail.RecordEvent("iCloudCache.DeleteFile", delegate(Dictionary<string, string> metadata)
				{
					metadata["filepath"] = cachedFilepath;
				});
			}
		}
		return true;
	}

	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000562 RID: 1378 RVA: 0x00013B90 File Offset: 0x00011D90
	// (remove) Token: 0x06000563 RID: 1379 RVA: 0x00013BC8 File Offset: 0x00011DC8
	public event Action<PersistentStorageServiceStatus> StatusChanged;

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000564 RID: 1380 RVA: 0x000020AA File Offset: 0x000002AA
	public bool RequiresOptionsPanel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x00013BFD File Offset: 0x00011DFD
	public void OnCreatedInScope(IScope scope)
	{
		iCloudStorage.Instance = this;
		this._reachability.ConnectivityChanged += this.OnInternetConnectivityChanged;
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x00013C1C File Offset: 0x00011E1C
	public void OnReleasedFromScope(IScope scope)
	{
		iCloudStorage.Instance = null;
		this._kernel.UserChanged -= this.OnUserChanged;
		this._kernel.FilesChanged -= this.OnFilesChanged;
		this._kernel.LoadCompleted -= this.OnLoadCompleted;
		this._kernel.FileDeleted -= this.OnFileDeleted;
		this._kernel.UserMessageChanged -= this.OnUserMessageChanged;
		this._kernel.FileStored -= this.OnFileStored;
		this._reachability.ConnectivityChanged -= this.OnInternetConnectivityChanged;
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000567 RID: 1383 RVA: 0x00013CD0 File Offset: 0x00011ED0
	private bool CanNotifyInitialLoad
	{
		get
		{
			if (!this._isInitialLoadRequested)
			{
				return false;
			}
			float timeSinceConnection = this._kernel.TimeSinceConnection;
			if (timeSinceConnection > iCloudStorage.ConnectionTimeout)
			{
				iCloudStorage.Log.Info("Can notify because the connection has timed out after {0}s.", new object[]
				{
					timeSinceConnection
				});
				return true;
			}
			if (!this._hasUserChanged)
			{
				return false;
			}
			if (!this.IsSignedIn)
			{
				iCloudStorage.Log.Info("Can notify because no user is signed in.", Array.Empty<object>());
				return true;
			}
			if (this._reachability.Connectivity == InternetConnectivity.Disconnected)
			{
				this.SetStatusIssues(PersistentStorageServiceIssues.NotAvailable);
				iCloudStorage.Log.Info("Can notify because we have no internet connection.", Array.Empty<object>());
				return true;
			}
			if (this._hasLoadCompleted)
			{
				iCloudStorage.Log.Info("Can notify because the load has completed in {0}s.", new object[]
				{
					timeSinceConnection
				});
				return true;
			}
			return false;
		}
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x00013D98 File Offset: 0x00011F98
	private void LoadCachedFiles(string localDirectory)
	{
		using (this._auditTrail.OpenEvent("iCloudStorage.LoadCachedFiles", delegate(Dictionary<string, string> metadata)
		{
			metadata["directory"] = localDirectory;
		}))
		{
			foreach (string localFilename in this._localCache.GetFilenamesInDirectory(localDirectory))
			{
				this.LoadStorableFromCache(localDirectory, localFilename);
			}
		}
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x00013E38 File Offset: 0x00012038
	private IStorable LoadStorableFromCache(string cacheDirectory, string cacheFilename)
	{
		string filepath = cacheFilename;
		if (!string.IsNullOrEmpty(cacheDirectory))
		{
			filepath = Path.Combine(cacheDirectory, filepath);
		}
		IStorable result;
		using (this._auditTrail.OpenEvent("iCloudStorage.LoadStorableFromCache", delegate(Dictionary<string, string> metadata)
		{
			metadata["filepath"] = filepath;
		}))
		{
			string playerId;
			string deviceId;
			IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForFilename(cacheFilename, out playerId, out deviceId);
			if (storableTypeHandler == null)
			{
				iCloudStorage.Log.Warn("Could not file storable type handler for {0}.", new object[]
				{
					cacheFilename
				});
				result = null;
			}
			else
			{
				byte[] cacheData = this._localCache.ReadFile(filepath);
				if (cacheData == null)
				{
					iCloudStorage.Log.Warn("Could not load data from cached file at path {0}.", new object[]
					{
						filepath
					});
					result = null;
				}
				else
				{
					IStorable storable = storableTypeHandler.Load(cacheData);
					if (storable == null)
					{
						iCloudStorage.Log.Warn("Failed to load cached storable from path {0}.", new object[]
						{
							filepath
						});
						result = null;
					}
					else
					{
						storable.IsAuthoritative = false;
						storableTypeHandler.ProcessLoadedStorable(storable, playerId, deviceId);
						iCloudStorage.Log.Info("Processed storable {0} for player {1}.", new object[]
						{
							storable,
							playerId
						});
						result = storable;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x00013F80 File Offset: 0x00012180
	private void LoadiCloudFiles()
	{
		using (this._auditTrail.OpenEvent("iCloudStorage.LoadiCloudFiles", null))
		{
			iCloudStorage.iCloudForEachFile(Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudStorage.OnFileChangedDelegate)));
		}
		this._haveFilesChanged = false;
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x00013FDC File Offset: 0x000121DC
	private void MigrateLegacyFiles()
	{
		using (this._auditTrail.OpenEvent("iCloudStorage.MigrateLegacyFiles", null))
		{
			using (IEnumerator<string> enumerator = this._localCache.GetFilenamesInDirectory("").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string legacyFilename = enumerator.Current;
					iCloudStorage.Log.Info("Found legacy file at {0}.", new object[]
					{
						legacyFilename
					});
					if (this._storableTypeHandlerRegistry.IsFilenameRecognized(legacyFilename))
					{
						iCloudStorage.Log.Info("Recognised filename {0}.", new object[]
						{
							legacyFilename
						});
						bool didMoveSucceed = this._localCache.MoveFile(legacyFilename, iCloudStorage.LegacyCachePath);
						if (!didMoveSucceed)
						{
							iCloudStorage.Log.Warn("Unable to move file {0} to {1}.", new object[]
							{
								legacyFilename,
								iCloudStorage.LegacyCachePath
							});
						}
						this._auditTrail.RecordEvent("iCloudCache.MoveFile", delegate(Dictionary<string, string> metadata)
						{
							metadata["fromFilepath"] = legacyFilename;
							metadata["toDirectory"] = iCloudStorage.LegacyCachePath;
							metadata["didSucceed"] = didMoveSucceed.ToString();
						});
					}
				}
			}
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x0600056C RID: 1388 RVA: 0x00014120 File Offset: 0x00012320
	private bool IsSignedIn
	{
		get
		{
			return !string.IsNullOrEmpty(this._userId);
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x0600056D RID: 1389 RVA: 0x00014130 File Offset: 0x00012330
	private string iCloudUserCachePath
	{
		get
		{
			return Path.Combine(iCloudStorage.iCloudCachePath, this._userId);
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x0600056E RID: 1390 RVA: 0x00014142 File Offset: 0x00012342
	private string ActiveCachePath
	{
		get
		{
			if (!this.IsSignedIn)
			{
				return iCloudStorage.UnsyncedCachePath;
			}
			return this.iCloudUserCachePath;
		}
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x00014158 File Offset: 0x00012358
	private void SetStatusIssues(PersistentStorageServiceIssues issuesToSet)
	{
		PersistentStorageServiceIssues newIssues = this._status.issues | issuesToSet;
		if (newIssues != this._status.issues)
		{
			iCloudStorage.Log.Info("Status issues changed from {0} to {1}.", new object[]
			{
				this._status.issues,
				newIssues
			});
			this._status.issues = newIssues;
			Action<PersistentStorageServiceStatus> statusChanged = this.StatusChanged;
			if (statusChanged == null)
			{
				return;
			}
			statusChanged(this._status);
		}
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x000141D4 File Offset: 0x000123D4
	private void ClearStatusIssues(PersistentStorageServiceIssues issuesToClear)
	{
		PersistentStorageServiceIssues newIssues = this._status.issues & ~issuesToClear;
		if (newIssues != this._status.issues)
		{
			iCloudStorage.Log.Info("Status issues changed from {0} to {1}.", new object[]
			{
				this._status.issues,
				newIssues
			});
			this._status.issues = newIssues;
			Action<PersistentStorageServiceStatus> statusChanged = this.StatusChanged;
			if (statusChanged == null)
			{
				return;
			}
			statusChanged(this._status);
		}
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00014254 File Offset: 0x00012454
	private void OnInternetConnectivityChanged(InternetConnectivity connectivity)
	{
		using (this._auditTrail.OpenEvent("iCloudStorage.OnInternetConnectivityChanged", delegate(Dictionary<string, string> metadata)
		{
			metadata["connectivity"] = connectivity.ToString();
		}))
		{
			if (connectivity == InternetConnectivity.Disconnected)
			{
				this.SetStatusIssues(PersistentStorageServiceIssues.NotAvailable);
			}
		}
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x000142B8 File Offset: 0x000124B8
	private void OnUserChanged(string newUserId)
	{
		if (this._userId == newUserId && this._hasUserChanged)
		{
			return;
		}
		this._hasUserChanged = true;
		this._userId = newUserId;
		using (this._auditTrail.OpenEvent("iCloudStorage.OnUserChanged", delegate(Dictionary<string, string> metadata)
		{
			metadata["userId"] = (string.IsNullOrEmpty(newUserId) ? "none" : newUserId);
		}))
		{
			if (string.IsNullOrEmpty(this._userId))
			{
				iCloudStorage.Log.Info("iCloud user disconnected.", Array.Empty<object>());
				this.SetStatusIssues(PersistentStorageServiceIssues.NotAuthenticated | PersistentStorageServiceIssues.NotAvailable);
			}
			else
			{
				iCloudStorage.Log.Info("iCloud user connected with id {0}.", new object[]
				{
					this._userId
				});
				this.ClearStatusIssues(PersistentStorageServiceIssues.NotAuthenticated);
			}
		}
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x0001438C File Offset: 0x0001258C
	private void OnFilesChanged()
	{
		iCloudStorage.Log.Info("Data changed, processing new files.", Array.Empty<object>());
		this._haveFilesChanged = true;
		using (this._auditTrail.OpenEvent("iCloudStorage.OnFilesChanged", null))
		{
			this.ClearStatusIssues(PersistentStorageServiceIssues.NotAvailable);
		}
	}

	// Token: 0x06000574 RID: 1396 RVA: 0x000143EC File Offset: 0x000125EC
	private void OnLoadCompleted(bool didSucceed)
	{
		if (didSucceed)
		{
			iCloudStorage.Log.Info("Load completed with no errors.", Array.Empty<object>());
		}
		else
		{
			iCloudStorage.Log.Info("Load completed with errors. Until we hear otherwise, we will continue but parse data assuming the data we have from iCloud is non-canonical.", Array.Empty<object>());
		}
		this._hasLoadCompleted = true;
		this._wasLoadSuccessful = didSucceed;
		using (this._auditTrail.OpenEvent("iCloudStorage.OnLoadCompleted", delegate(Dictionary<string, string> metadata)
		{
			metadata["didSucceed"] = didSucceed.ToString();
		}))
		{
			if (didSucceed)
			{
				this.ClearStatusIssues(PersistentStorageServiceIssues.NotAvailable);
			}
			else
			{
				this.SetStatusIssues(PersistentStorageServiceIssues.NotAvailable);
			}
		}
	}

	// Token: 0x06000575 RID: 1397 RVA: 0x0001449C File Offset: 0x0001269C
	private void OnFileDeleted(string deletedFilename)
	{
		using (this._auditTrail.OpenEvent("iCloudStorage.OnFileDeleted", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = deletedFilename;
		}))
		{
			iCloudStorage.Log.Info("File {0} has been deleted from the database.", new object[]
			{
				deletedFilename
			});
			string cachedFilepath = Path.Combine(this.ActiveCachePath, deletedFilename);
			bool didCacheDeleteSucceed = this._localCache.DeleteFile(cachedFilepath);
			if (!didCacheDeleteSucceed)
			{
				iCloudStorage.Log.Warn("Failed to delete {0} from the iCloud cache.", new object[]
				{
					deletedFilename
				});
			}
			this._auditTrail.RecordEvent("iCloudCache.DeleteFile", delegate(Dictionary<string, string> metadata)
			{
				metadata["filepath"] = cachedFilepath;
				metadata["didSucceed"] = didCacheDeleteSucceed.ToString();
			});
			string playerId;
			string deviceId;
			IStorableTypeHandler deletedStorableHander = this._storableTypeHandlerRegistry.GetHandlerForFilename(deletedFilename, out playerId, out deviceId);
			if (deletedStorableHander == null)
			{
				iCloudStorage.Log.Info("Unable to determine the file's type from the name; ignoring.", Array.Empty<object>());
			}
			else
			{
				deletedStorableHander.ProcessDeletedStorable(playerId, deviceId);
				this.ClearStatusIssues(PersistentStorageServiceIssues.NotAvailable);
			}
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x000145C4 File Offset: 0x000127C4
	private void OnUserMessageChanged(string messageStringKey)
	{
		iCloudStorage.Log.Info("Received message {0}.", new object[]
		{
			messageStringKey
		});
		using (this._auditTrail.OpenEvent("iCloudStorage.OnUserMessageChanged", delegate(Dictionary<string, string> metadata)
		{
			metadata["message"] = messageStringKey;
		}))
		{
			if (string.IsNullOrEmpty(messageStringKey))
			{
				this.ClearStatusIssues(PersistentStorageServiceIssues.NotAvailable);
				if (!string.IsNullOrEmpty(this._status.messageKey))
				{
					this._status.messageKey = null;
					Action<PersistentStorageServiceStatus> statusChanged = this.StatusChanged;
					if (statusChanged != null)
					{
						statusChanged(this._status);
					}
				}
			}
			else if (messageStringKey != this._status.messageKey)
			{
				if (messageStringKey == StringId.iCloudQuotaExceeded.ToString())
				{
					this.SetStatusIssues(PersistentStorageServiceIssues.QuotaExceeded);
				}
				this._status.messageKey = messageStringKey;
				Action<PersistentStorageServiceStatus> statusChanged2 = this.StatusChanged;
				if (statusChanged2 != null)
				{
					statusChanged2(this._status);
				}
			}
		}
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x000146E8 File Offset: 0x000128E8
	private void OnFileChanged(string filename)
	{
		using (this._auditTrail.OpenEvent("iCloudStorage.OnFileChanged", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
		}))
		{
			iCloudStorage.Log.Info("Attempting to load iCloud file '{0}'.", new object[]
			{
				filename
			});
			iCloudStorage.iCloudMarkCurrentVersionAsDownloaded(filename);
			string playerId;
			string deviceId;
			IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForFilename(filename, out playerId, out deviceId);
			if (storableTypeHandler == null)
			{
				iCloudStorage.Log.Info("Unable to determine the file's type from the name; ignoring.", Array.Empty<object>());
			}
			else
			{
				int dataLength = 0;
				if (!iCloudStorage.iCloudReadFile(filename, IntPtr.Zero, ref dataLength))
				{
					iCloudStorage.Log.Warn("Could not determine the file's length.", Array.Empty<object>());
				}
				else if (dataLength <= 0)
				{
					iCloudStorage.Log.Warn("File had invalid length {0}.", new object[]
					{
						dataLength
					});
				}
				else
				{
					byte[] data = new byte[dataLength];
					GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
					int readLength = dataLength;
					bool flag = iCloudStorage.iCloudReadFile(filename, dataHandle.AddrOfPinnedObject(), ref readLength);
					dataHandle.Free();
					if (!flag)
					{
						iCloudStorage.Log.Warn("Failed to read file from iCloud.", Array.Empty<object>());
					}
					else
					{
						if (readLength != dataLength)
						{
							iCloudStorage.Log.Warn("We were expecting to read {0} bytes, but only read {0}.", new object[]
							{
								dataLength,
								readLength
							});
							Array.Resize<byte>(ref data, readLength);
						}
						IStorable storable = storableTypeHandler.Load(data);
						if (storable == null)
						{
							iCloudStorage.Log.Warn("Failed to import storable.", Array.Empty<object>());
						}
						else
						{
							storable.IsAuthoritative = true;
							iCloudStorage.Log.Info("Processing storable {0} with player id {1} and device id {2}.", new object[]
							{
								storable,
								playerId,
								deviceId
							});
							storableTypeHandler.ProcessLoadedStorable(storable, playerId, deviceId);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x000148DC File Offset: 0x00012ADC
	private void OnFileStored(string filename)
	{
		iCloudStorage.Log.Info("File {0} was stored successfully.", new object[]
		{
			filename
		});
		using (this._auditTrail.OpenEvent("iCloudStorage.OnFileStored", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
		}))
		{
			List<NamedStoreCompleted> callbacks;
			if (this._storeCallbacks.TryGetValue(filename, out callbacks))
			{
				foreach (NamedStoreCompleted namedStoreCompleted in callbacks)
				{
					namedStoreCompleted(filename, StoreOperationResult.Succeeded);
				}
				this._storeCallbacks.Remove(filename);
			}
			this.ClearStatusIssues(PersistentStorageServiceIssues.NotAvailable);
		}
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x000149BC File Offset: 0x00012BBC
	private void OnFileQueriedForDeletion(string filename)
	{
		string playerId;
		string text;
		if (this._storableTypeHandlerRegistry.IsFilenameRecognized(filename, out playerId, out text) && playerId == this._playerIdToDelete)
		{
			iCloudStorage.Log.Info("Deleting file {0} associated with player {1}.", new object[]
			{
				filename,
				this._playerIdToDelete
			});
			this._filenamesToDelete.Add(filename);
		}
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00014A18 File Offset: 0x00012C18
	private bool HasRecentlyModifiedIcloudData(string ignorediCloudUser = null, bool checkAgainstLocalData = false)
	{
		DateTime localSaveModifiedTime = DateTime.MinValue;
		foreach (string saveFileName in this._localCache.GetFilenamesInDirectory(iCloudStorage.UnsyncedCachePath))
		{
			if (saveFileName.StartsWith("userProfile_"))
			{
				string combinedFileName = Path.Combine(iCloudStorage.UnsyncedCachePath, saveFileName);
				localSaveModifiedTime = this._localCache.GetFileModifiedTime(combinedFileName);
			}
		}
		foreach (string iCloudUserDirectory in this._localCache.GetDirectoriesInDirectory(iCloudStorage.iCloudCachePath))
		{
			if (!(ignorediCloudUser == iCloudUserDirectory))
			{
				foreach (string saveFileName2 in this._localCache.GetFilenamesInDirectory(Path.Combine(iCloudStorage.iCloudCachePath, iCloudUserDirectory)))
				{
					if (saveFileName2.StartsWith("userProfile_"))
					{
						string combinedFileName2 = Path.Combine(iCloudStorage.iCloudCachePath, iCloudUserDirectory, saveFileName2);
						DateTime modifiedTime = this._localCache.GetFileModifiedTime(combinedFileName2);
						if ((modifiedTime > localSaveModifiedTime || !checkAgainstLocalData) && DateTime.UtcNow - modifiedTime < this._recentSaveThreshold)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00014B90 File Offset: 0x00012D90
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnFileChangedDelegate(string filename)
	{
		iCloudStorage instance = iCloudStorage.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnFileChanged(filename);
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x00014BA2 File Offset: 0x00012DA2
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnFileQueriedForDeletionDelegate(string filename)
	{
		iCloudStorage instance = iCloudStorage.Instance;
		if (instance == null)
		{
			return;
		}
		instance.OnFileQueriedForDeletion(filename);
	}

	// Token: 0x0600057D RID: 1405
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool iCloudWriteFile(string filename, IntPtr data, int dataLength);

	// Token: 0x0600057E RID: 1406
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern int iCloudForEachFile(IntPtr fileHandler);

	// Token: 0x0600057F RID: 1407
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern int iCloudForEachChangedFile(IntPtr fileHandler);

	// Token: 0x06000580 RID: 1408
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool iCloudMarkCurrentVersionAsDownloaded(string filename);

	// Token: 0x06000581 RID: 1409
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool iCloudReadFile(string filename, IntPtr data, ref int dataLength);

	// Token: 0x06000582 RID: 1410
	[DllImport("dpcPlatform", CallingConvention = CallingConvention.Cdecl)]
	private static extern bool iCloudDeleteFile(string filename);

	// Token: 0x0400023F RID: 575
	private PersistentStorageServiceStatus _status;

	// Token: 0x04000240 RID: 576
	private bool _hasConnected;

	// Token: 0x04000241 RID: 577
	private bool _isInitialLoadRequested;

	// Token: 0x04000242 RID: 578
	private bool _hasInitialLoadCompleted;

	// Token: 0x04000243 RID: 579
	private Action _loadCallback;

	// Token: 0x04000244 RID: 580
	private bool _haveFilesChanged;

	// Token: 0x04000245 RID: 581
	private bool _hasLoadCompleted;

	// Token: 0x04000246 RID: 582
	private bool _wasLoadSuccessful;

	// Token: 0x04000247 RID: 583
	private string _userId;

	// Token: 0x04000248 RID: 584
	private bool _hasUserChanged;

	// Token: 0x04000249 RID: 585
	private string _playerIdToDelete;

	// Token: 0x0400024A RID: 586
	private readonly List<string> _filenamesToDelete = new List<string>();

	// Token: 0x0400024B RID: 587
	private readonly Dictionary<string, List<NamedStoreCompleted>> _storeCallbacks = new Dictionary<string, List<NamedStoreCompleted>>();

	// Token: 0x0400024C RID: 588
	[Dependency]
	private iCloudKernel _kernel;

	// Token: 0x0400024D RID: 589
	[Dependency]
	private PlayerDatabase _playerDatabase;

	// Token: 0x0400024E RID: 590
	[Dependency]
	private IReachability _reachability;

	// Token: 0x0400024F RID: 591
	[Dependency]
	private IStorableTypeHandlerRegistry _storableTypeHandlerRegistry;

	// Token: 0x04000250 RID: 592
	[Dependency]
	private IiCloudCache _localCache;

	// Token: 0x04000251 RID: 593
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x04000252 RID: 594
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("iCloudStorage");

	// Token: 0x04000253 RID: 595
	private static string LegacyCachePath = "players/legacy";

	// Token: 0x04000254 RID: 596
	private static string UnsyncedCachePath = "players/local";

	// Token: 0x04000255 RID: 597
	private static string iCloudCachePath = "players/iCloud";

	// Token: 0x04000256 RID: 598
	private static float ConnectionTimeout = 15f;

	// Token: 0x04000257 RID: 599
	private static iCloudStorage Instance;

	// Token: 0x04000258 RID: 600
	private readonly TimeSpan _recentSaveThreshold = TimeSpan.FromDays(90.0);
}
