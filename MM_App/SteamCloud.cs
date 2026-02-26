using System;
using System.Collections.Generic;

// Token: 0x02000115 RID: 277
public class SteamCloud : LocalFileStorage
{
	// Token: 0x060005EB RID: 1515 RVA: 0x00015808 File Offset: 0x00013A08
	public override void LoadAll(Action loadCompleteCallback)
	{
		foreach (string cloudFilename in SteamworksShared.GetCloudFiles())
		{
			string playerId;
			string deviceId;
			IStorableTypeHandler storableTypeHandler = this._storableTypeHandlerRegistry.GetHandlerForFilename(cloudFilename, out playerId, out deviceId);
			if (storableTypeHandler == null)
			{
				LocalFileStorage.Log.Info("Found unrecognised file {0} in Steam Cloud.", new object[]
				{
					cloudFilename
				});
			}
			else
			{
				byte[] data = SteamworksShared.ReadCloudFile(cloudFilename);
				if (data == null)
				{
					LocalFileStorage.Log.Warn("The file {0} could not be read from Steam Cloud.", new object[]
					{
						cloudFilename
					});
				}
				else
				{
					IStorable storable = storableTypeHandler.Load(data);
					if (storable == null)
					{
						LocalFileStorage.Log.Warn("The file {0} was unable to be parsed as the type {1}.", new object[]
						{
							cloudFilename,
							storableTypeHandler
						});
					}
					else
					{
						storable.IsAuthoritative = true;
						if (!storableTypeHandler.ProcessLoadedStorable(storable, playerId, deviceId))
						{
							this._scope.Release(storable);
						}
					}
				}
			}
		}
		base.LoadAll(loadCompleteCallback);
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x00015900 File Offset: 0x00013B00
	public override bool Store(string filename, byte[] data, NamedStoreCompleted storeCompleteCallback)
	{
		SteamworksShared.WriteCloudFile(filename, data);
		return base.Store(filename, data, storeCompleteCallback);
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00015913 File Offset: 0x00013B13
	public override bool Delete(string filename)
	{
		SteamworksShared.DeleteCloudFile(filename);
		return base.Delete(filename);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00015924 File Offset: 0x00013B24
	public override bool DeletePlayer(string playerIdToDelete)
	{
		List<string> filesToDelete = new List<string>();
		foreach (string cloudFilename in SteamworksShared.GetCloudFiles())
		{
			string playerId;
			string text;
			if (this._storableTypeHandlerRegistry.IsFilenameRecognized(cloudFilename, out playerId, out text) && playerId == playerIdToDelete)
			{
				filesToDelete.Add(cloudFilename);
			}
		}
		foreach (string filename in filesToDelete)
		{
			SteamworksShared.DeleteCloudFile(filename);
		}
		return base.DeletePlayer(playerIdToDelete);
	}
}
