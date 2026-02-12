using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using Factory;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class iCloudUserDefaultsCache : IiCloudCache, ICreatedInScopeHandler
{
	// Token: 0x060005AF RID: 1455 RVA: 0x00014E90 File Offset: 0x00013090
	public bool HasFile(string filepath)
	{
		int dataLength = 0;
		if (!iCloudUserDefaultsCache.UserDefaultsReadData(filepath, IntPtr.Zero, ref dataLength))
		{
			return false;
		}
		if (dataLength <= 0)
		{
			iCloudUserDefaultsCache.Log.Error("Key {0} reported data of invalid length {1}.", new object[]
			{
				filepath,
				dataLength
			});
			return false;
		}
		return true;
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x00014EDC File Offset: 0x000130DC
	public byte[] ReadFile(string filepath)
	{
		int dataLength = 0;
		if (!iCloudUserDefaultsCache.UserDefaultsReadData(filepath, IntPtr.Zero, ref dataLength))
		{
			iCloudUserDefaultsCache.Log.Error("Unable to find key {0}.", new object[]
			{
				filepath
			});
			return null;
		}
		if (dataLength <= 0)
		{
			iCloudUserDefaultsCache.Log.Error("Key {0} reported data of invalid length {1}.", new object[]
			{
				filepath,
				dataLength
			});
			return null;
		}
		byte[] data = new byte[dataLength];
		GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
		int readLength = dataLength;
		bool flag = iCloudUserDefaultsCache.UserDefaultsReadData(filepath, pinnedData.AddrOfPinnedObject(), ref readLength);
		pinnedData.Free();
		if (!flag || readLength != dataLength)
		{
			iCloudUserDefaultsCache.Log.Error("Read of key {0} failed; expected {1} bytes, but read {2}.", new object[]
			{
				filepath,
				dataLength,
				readLength
			});
			return null;
		}
		return data;
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00014F98 File Offset: 0x00013198
	public bool WriteFile(string filepath, byte[] data)
	{
		GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
		bool flag = iCloudUserDefaultsCache.UserDefaultsWriteData(filepath, pinnedData.AddrOfPinnedObject(), data.Length);
		pinnedData.Free();
		if (!flag)
		{
			iCloudUserDefaultsCache.Log.Error("Failed to write data to key {0}.", new object[]
			{
				filepath
			});
			return false;
		}
		return true;
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00014FE4 File Offset: 0x000131E4
	public bool HasSpaceToWriteFile(string filepath, int dataLength, out int bytesNeededToDelete)
	{
		bytesNeededToDelete = 0;
		if (this._sizeLimit == -1)
		{
			return true;
		}
		int newFileSize = filepath.Length + dataLength;
		int oldFileSize = iCloudUserDefaultsCache.UserDefaultsGetObjectSize(filepath);
		if (oldFileSize >= newFileSize)
		{
			return true;
		}
		int num = iCloudUserDefaultsCache.UserDefaultsGetTotalSize();
		int bytesAdded = newFileSize - oldFileSize;
		int excessBytes = num + bytesAdded - this._sizeLimit;
		if (excessBytes <= 0)
		{
			return true;
		}
		bytesNeededToDelete = excessBytes;
		return false;
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00015034 File Offset: 0x00013234
	public IEnumerable<string> GetFilenamesInDirectory(string directory)
	{
		iCloudUserDefaultsCache.Keys.Clear();
		iCloudUserDefaultsCache.UserDefaultsForEachKey(Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudUserDefaultsCache.OnKey)));
		List<string> filenamesInDirectory = new List<string>();
		foreach (string filepath in iCloudUserDefaultsCache.Keys)
		{
			if (Path.GetDirectoryName(filepath) == directory)
			{
				filenamesInDirectory.Add(Path.GetFileName(filepath));
			}
		}
		return filenamesInDirectory;
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x000150C0 File Offset: 0x000132C0
	public IEnumerable<string> GetDirectoriesInDirectory(string directory)
	{
		iCloudUserDefaultsCache.Keys.Clear();
		iCloudUserDefaultsCache.UserDefaultsForEachKey(Marshal.GetFunctionPointerForDelegate<Action<string>>(new Action<string>(iCloudUserDefaultsCache.OnKey)));
		List<string> directoriesInDirectory = new List<string>();
		foreach (string path in iCloudUserDefaultsCache.Keys)
		{
			string fileDirectory = Path.GetDirectoryName(path);
			if (fileDirectory != null && fileDirectory.StartsWith(directory))
			{
				fileDirectory = fileDirectory.Substring(directory.Length);
				string[] subDirectories = fileDirectory.Split(new char[]
				{
					Path.DirectorySeparatorChar,
					Path.AltDirectorySeparatorChar
				}, StringSplitOptions.RemoveEmptyEntries);
				if (subDirectories.Length != 0)
				{
					string subDirectory = subDirectories[0];
					if (!directoriesInDirectory.Contains(subDirectory))
					{
						directoriesInDirectory.Add(subDirectory);
					}
				}
			}
		}
		return directoriesInDirectory;
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x0001518C File Offset: 0x0001338C
	public int GetFileSize(string filepath)
	{
		return iCloudUserDefaultsCache.UserDefaultsGetObjectSize(filepath);
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x00015194 File Offset: 0x00013394
	public bool MoveFile(string filepath, string directory)
	{
		string destinationFilepath = Path.GetFileName(filepath);
		if (!string.IsNullOrEmpty(directory))
		{
			destinationFilepath = Path.Combine(directory, destinationFilepath);
		}
		if (iCloudUserDefaultsCache.UserDefaultsRenameObject(filepath, destinationFilepath))
		{
			iCloudUserDefaultsCache.Log.Info("Renamed {0} to {1}.", new object[]
			{
				filepath,
				destinationFilepath
			});
			return true;
		}
		iCloudUserDefaultsCache.Log.Error("Failed to rename {0} to {1}.", new object[]
		{
			filepath,
			destinationFilepath
		});
		return false;
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x000151FE File Offset: 0x000133FE
	public bool DeleteFile(string filepath)
	{
		if (iCloudUserDefaultsCache.UserDefaultsDeleteObject(filepath))
		{
			iCloudUserDefaultsCache.Log.Info("Deleted object {0}.", new object[]
			{
				filepath
			});
			return true;
		}
		iCloudUserDefaultsCache.Log.Error("Failed to delete object {0}.", new object[]
		{
			filepath
		});
		return false;
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00015240 File Offset: 0x00013440
	public void CopyNewFilesInDirectory(string sourceDirectory, string destinationDirectory)
	{
		foreach (string sourceFilename in this.GetFilenamesInDirectory(sourceDirectory))
		{
			string sourceFilepath = sourceFilename;
			if (!string.IsNullOrEmpty(sourceDirectory))
			{
				sourceFilepath = Path.Combine(sourceDirectory, sourceFilepath);
			}
			string destinationFilepath = Path.Combine(destinationDirectory, sourceFilename);
			iCloudUserDefaultsCache.Log.Info("Copying {0} to {1}.", new object[]
			{
				sourceFilepath,
				destinationFilepath
			});
			iCloudUserDefaultsCache.UserDefaultsCopyObject(sourceFilepath, destinationFilepath);
			this._auditTrail.RecordEvent("iCloudUserDefaultsCache.CopyFile", delegate(Dictionary<string, string> metadata)
			{
				metadata["fromFilepath"] = sourceFilepath;
				metadata["toFilepath"] = destinationFilepath;
			});
		}
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x00015318 File Offset: 0x00013518
	public DateTime GetFileModifiedTime(string filepath)
	{
		return DateTime.MinValue;
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x0001531F File Offset: 0x0001351F
	public void OnCreatedInScope(IScope scope)
	{
		if (Application.platform == RuntimePlatform.tvOS)
		{
			this._sizeLimit = 943713;
		}
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00015335 File Offset: 0x00013535
	[MonoPInvokeCallback(typeof(Action<string>))]
	private static void OnKey(string key)
	{
		iCloudUserDefaultsCache.Keys.Add(key);
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void UserDefaultsForEachKey(IntPtr keyHandler)
	{
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0000222C File Offset: 0x0000042C
	private static bool UserDefaultsReadData(string filename, IntPtr data, ref int dataLength)
	{
		return false;
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x0000222C File Offset: 0x0000042C
	private static bool UserDefaultsWriteData(string filename, IntPtr data, int dataLength)
	{
		return false;
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0000222C File Offset: 0x0000042C
	private static bool UserDefaultsCopyObject(string existingKey, string newKey)
	{
		return false;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x0000222C File Offset: 0x0000042C
	private static bool UserDefaultsRenameObject(string oldKey, string newKey)
	{
		return false;
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0000222C File Offset: 0x0000042C
	private static bool UserDefaultsDeleteObject(string key)
	{
		return false;
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x0000222C File Offset: 0x0000042C
	private static int UserDefaultsGetObjectSize(string key)
	{
		return 0;
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0000222C File Offset: 0x0000042C
	private static int UserDefaultsGetTotalSize()
	{
		return 0;
	}

	// Token: 0x04000276 RID: 630
	private int _sizeLimit = -1;

	// Token: 0x04000277 RID: 631
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x04000278 RID: 632
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("iCloudUserDefaultsCache");

	// Token: 0x04000279 RID: 633
	private static readonly List<string> Keys = new List<string>();

	// Token: 0x0400027A RID: 634
	private const int NoSizeLimit = -1;
}
