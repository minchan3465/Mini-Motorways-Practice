using System;
using System.Collections.Generic;
using System.IO;
using Factory;

// Token: 0x02000112 RID: 274
public class LocalFileStorage : IPersistentStorageProvider
{
	// Token: 0x060005D3 RID: 1491 RVA: 0x00015390 File Offset: 0x00013590
	public virtual void LoadAll(Action loadCompleteCallback)
	{
		if (Directory.Exists(this._hardwareCapabilities.PersistentStoragePath))
		{
			foreach (string filepath in Directory.GetFiles(this._hardwareCapabilities.PersistentStoragePath))
			{
				string filename = Path.GetFileName(filepath);
				string playerId;
				string deviceId;
				IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForFilename(filename, out playerId, out deviceId);
				if (storableTypeHandler == null)
				{
					LocalFileStorage.Log.Info("Found unrecognised file {0} in local file storage.", new object[]
					{
						filename
					});
				}
				else
				{
					byte[] data = this.Read(filepath);
					if (data == null)
					{
						LocalFileStorage.Log.Warn("The file {0} could not be loaded.", new object[]
						{
							filename
						});
					}
					else
					{
						IStorable storable = storableTypeHandler.Load(data);
						if (storable == null)
						{
							LocalFileStorage.Log.Warn("The file {0} was unable to be parsed as the type {1}.", new object[]
							{
								filename,
								storableTypeHandler
							});
						}
						else
						{
							storable.IsAuthoritative = true;
							if (storableTypeHandler.ProcessLoadedStorable(storable, playerId, deviceId))
							{
								if (!(deviceId == PlayerDatabase.LegacyDeviceId))
								{
									goto IL_183;
								}
								string newFilename = storableTypeHandler.GetFilename(playerId, this._hardwareCapabilities.UniqueDeviceId);
								string newFilepath = Path.Combine(this._hardwareCapabilities.PersistentStoragePath, newFilename);
								LocalFileStorage.Log.Info("Migrating file {0} with legacy device id to {1}.", new object[]
								{
									filepath,
									newFilepath
								});
								try
								{
									if (File.Exists(newFilepath))
									{
										File.Delete(filepath);
									}
									else
									{
										File.Move(filepath, newFilepath);
									}
									goto IL_183;
								}
								catch (Exception exception)
								{
									LocalFileStorage.Log.Warn("Failed to migrate.\n{0}", new object[]
									{
										exception
									});
									goto IL_183;
								}
							}
							this._scope.Release(storable);
						}
					}
				}
				IL_183:;
			}
		}
		if (loadCompleteCallback != null)
		{
			loadCompleteCallback();
		}
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x000022F5 File Offset: 0x000004F5
	public void Tick()
	{
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00015548 File Offset: 0x00013748
	public virtual bool Store(string filename, byte[] data, NamedStoreCompleted storeCompleteCallback)
	{
		string filepath = Path.Combine(this._hardwareCapabilities.PersistentStoragePath, filename);
		StoreOperationResult result = this.Write(filepath, data) ? StoreOperationResult.Succeeded : StoreOperationResult.Failed;
		if (storeCompleteCallback != null)
		{
			storeCompleteCallback(filename, result);
		}
		return result == StoreOperationResult.Succeeded;
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00015588 File Offset: 0x00013788
	public virtual bool Delete(string filename)
	{
		this._auditTrail.RecordEvent("LocalFileStorage.Delete", delegate(Dictionary<string, string> metadata)
		{
			metadata["filename"] = filename;
		});
		string filepath = Path.Combine(this._hardwareCapabilities.PersistentStoragePath, filename);
		bool result;
		try
		{
			File.Delete(filepath);
			result = true;
		}
		catch (Exception exception)
		{
			LocalFileStorage.Log.Warn("Unable to delete {0}.\n{1}", new object[]
			{
				filepath,
				exception
			});
			result = false;
		}
		return result;
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00015614 File Offset: 0x00013814
	public virtual bool DeletePlayer(string playerIdToDelete)
	{
		if (Directory.Exists(this._hardwareCapabilities.PersistentStoragePath))
		{
			foreach (string filepath in Directory.GetFiles(this._hardwareCapabilities.PersistentStoragePath))
			{
				string filename = Path.GetFileName(filepath);
				string playerId;
				string text;
				if (this._storableTypeHandlerRegistry.IsFilenameRecognized(filename, out playerId, out text) && playerId == playerIdToDelete)
				{
					try
					{
						File.Delete(filepath);
					}
					catch (Exception exception)
					{
						LocalFileStorage.Log.Warn("Unable to delete {0}.\n{1}", new object[]
						{
							filepath,
							exception
						});
					}
				}
			}
		}
		return true;
	}

	// Token: 0x14000016 RID: 22
	// (add) Token: 0x060005D8 RID: 1496 RVA: 0x000156B4 File Offset: 0x000138B4
	// (remove) Token: 0x060005D9 RID: 1497 RVA: 0x000156EC File Offset: 0x000138EC
	public event Action<PersistentStorageServiceStatus> StatusChanged;

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x060005DA RID: 1498 RVA: 0x0000222C File Offset: 0x0000042C
	public bool RequiresOptionsPanel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00015721 File Offset: 0x00013921
	protected void SetStatus(PersistentStorageServiceStatus status)
	{
		Action<PersistentStorageServiceStatus> statusChanged = this.StatusChanged;
		if (statusChanged == null)
		{
			return;
		}
		statusChanged(status);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x00015734 File Offset: 0x00013934
	private byte[] Read(string filepath)
	{
		byte[] result;
		try
		{
			result = File.ReadAllBytes(filepath);
		}
		catch (Exception exception)
		{
			LocalFileStorage.Log.Warn("Unable to read from {0}.\n{1}", new object[]
			{
				filepath,
				exception
			});
			result = null;
		}
		return result;
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00015780 File Offset: 0x00013980
	private bool Write(string filepath, byte[] data)
	{
		bool result;
		try
		{
			File.WriteAllBytes(filepath, data);
			result = true;
		}
		catch (Exception exception)
		{
			LocalFileStorage.Log.Warn("Unable to write to {0}.\n{1}", new object[]
			{
				filepath,
				exception
			});
			result = false;
		}
		return result;
	}

	// Token: 0x0400027D RID: 637
	[Dependency]
	protected IScope _scope;

	// Token: 0x0400027E RID: 638
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x0400027F RID: 639
	[Dependency]
	protected IStorableTypeHandlerRegistry _storableTypeHandlerRegistry;

	// Token: 0x04000280 RID: 640
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x04000281 RID: 641
	protected static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("LocalFileStorage");
}
