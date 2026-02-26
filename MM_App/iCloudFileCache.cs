using System;
using System.Collections.Generic;
using System.IO;
using Factory;

// Token: 0x020000F8 RID: 248
public class iCloudFileCache : IiCloudCache
{
	// Token: 0x06000522 RID: 1314 RVA: 0x00011EDA File Offset: 0x000100DA
	public bool HasFile(string filepath)
	{
		return File.Exists(this.GetAbsolutePath(filepath));
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00011EE8 File Offset: 0x000100E8
	public byte[] ReadFile(string filepath)
	{
		byte[] result;
		try
		{
			result = File.ReadAllBytes(this.GetAbsolutePath(filepath));
		}
		catch (Exception exception)
		{
			iCloudFileCache.Log.Error("Unable to read from {0}.\n{1}", new object[]
			{
				filepath,
				exception
			});
			result = null;
		}
		return result;
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00011F38 File Offset: 0x00010138
	public bool WriteFile(string filepath, byte[] data)
	{
		string absolutePath = this.GetAbsolutePath(filepath);
		string absoluteDirectory = Path.GetDirectoryName(absolutePath);
		if (!string.IsNullOrEmpty(absoluteDirectory))
		{
			try
			{
				Directory.CreateDirectory(absoluteDirectory);
			}
			catch (Exception exception)
			{
				iCloudFileCache.Log.Error("Unable to create directory {0}.\n{1}", new object[]
				{
					absoluteDirectory,
					exception
				});
			}
		}
		try
		{
			File.WriteAllBytes(absolutePath, data);
		}
		catch (Exception exception2)
		{
			iCloudFileCache.Log.Error("Unable to write to {0}.\n{1}", new object[]
			{
				absolutePath,
				exception2
			});
			return false;
		}
		return true;
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x00011FD4 File Offset: 0x000101D4
	public bool HasSpaceToWriteFile(string filepath, int dataLength, out int bytesNeededToDelete)
	{
		bytesNeededToDelete = 0;
		return true;
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x00011FDC File Offset: 0x000101DC
	public IEnumerable<string> GetFilenamesInDirectory(string directory)
	{
		List<string> filenames = new List<string>();
		try
		{
			string absoluteDirectory = this.GetAbsolutePath(directory);
			if (!Directory.Exists(absoluteDirectory))
			{
				iCloudFileCache.Log.Info("Directory {0} does not exist yet.", new object[]
				{
					absoluteDirectory
				});
				return filenames;
			}
			iCloudFileCache.Log.Info("Enumerating files in directory {0}.", new object[]
			{
				absoluteDirectory
			});
			foreach (string filepath in Directory.EnumerateFiles(absoluteDirectory))
			{
				string filename = Path.GetFileName(filepath);
				iCloudFileCache.Log.Info("Found file {0}.", new object[]
				{
					filename
				});
				filenames.Add(Path.GetFileName(filepath));
			}
		}
		catch (Exception exception)
		{
			iCloudFileCache.Log.Error("Unable to enumerate files in {0}.\n{1}", new object[]
			{
				directory,
				exception
			});
		}
		return filenames;
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x000120D8 File Offset: 0x000102D8
	public IEnumerable<string> GetDirectoriesInDirectory(string directory)
	{
		List<string> directories = new List<string>();
		try
		{
			foreach (string path in Directory.EnumerateDirectories(this.GetAbsolutePath(directory)))
			{
				string subdirectory = Path.GetFileName(path);
				if (!string.IsNullOrEmpty(subdirectory) && !directories.Contains(subdirectory))
				{
					directories.Add(subdirectory);
				}
			}
		}
		catch (Exception exception)
		{
			iCloudFileCache.Log.Error("Unable to enumerate directories in {0}.\n{1}", new object[]
			{
				directory,
				exception
			});
		}
		return directories;
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00012178 File Offset: 0x00010378
	public int GetFileSize(string filepath)
	{
		int result;
		try
		{
			long fileLength = new FileInfo(this.GetAbsolutePath(filepath)).Length;
			result = ((fileLength > 2147483647L) ? int.MaxValue : ((int)fileLength));
		}
		catch (Exception)
		{
			result = 0;
		}
		return result;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x000121C4 File Offset: 0x000103C4
	public bool MoveFile(string filepath, string directory)
	{
		string absoluteDestinationDirectory = this.GetAbsolutePath(directory);
		if (string.IsNullOrEmpty(absoluteDestinationDirectory))
		{
			iCloudFileCache.Log.Error("Could not move file at {0} to {1}.", new object[]
			{
				filepath,
				directory
			});
			return false;
		}
		try
		{
			Directory.CreateDirectory(absoluteDestinationDirectory);
		}
		catch (Exception exception)
		{
			iCloudFileCache.Log.Error("Unable to create directory {0}.\n{1}", new object[]
			{
				absoluteDestinationDirectory,
				exception
			});
		}
		string filename = Path.GetFileName(filepath);
		if (string.IsNullOrEmpty(filename))
		{
			iCloudFileCache.Log.Error("Could not extract filename from filepath {0}.", new object[]
			{
				filepath
			});
			return false;
		}
		string absoluteSourceFilepath = this.GetAbsolutePath(filepath);
		try
		{
			string absoluteDestinationFilepath = Path.Combine(absoluteDestinationDirectory, filename);
			File.Move(absoluteSourceFilepath, absoluteDestinationFilepath);
			iCloudFileCache.Log.Info("Moved file {0} to {1}.", new object[]
			{
				absoluteSourceFilepath,
				absoluteDestinationFilepath
			});
			return true;
		}
		catch (Exception exception2)
		{
			iCloudFileCache.Log.Error("Unable to move file {0} to {1}.\n{2}", new object[]
			{
				absoluteSourceFilepath,
				directory,
				exception2
			});
		}
		return false;
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x000122D8 File Offset: 0x000104D8
	public bool DeleteFile(string filepath)
	{
		string absoluteFilepath = this.GetAbsolutePath(filepath);
		try
		{
			File.Delete(absoluteFilepath);
			iCloudFileCache.Log.Info("Deleted file at {0}.", new object[]
			{
				absoluteFilepath
			});
			return true;
		}
		catch (Exception exception)
		{
			iCloudFileCache.Log.Error("Unable to delete file at {0}.\n{1}", new object[]
			{
				absoluteFilepath,
				exception
			});
		}
		return false;
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x00012344 File Offset: 0x00010544
	public void CopyNewFilesInDirectory(string sourceDirectory, string destinationDirectory)
	{
		string absoluteSourceDirectory = this.GetAbsolutePath(sourceDirectory);
		string absoluteDestinationDirectory = this.GetAbsolutePath(destinationDirectory);
		iCloudFileCache.Log.Info("Copying all files from {0} to {1}.", new object[]
		{
			absoluteSourceDirectory,
			absoluteDestinationDirectory
		});
		if (!Directory.Exists(absoluteSourceDirectory))
		{
			return;
		}
		try
		{
			bool createdDestinationDirectory = false;
			using (IEnumerator<string> enumerator = Directory.EnumerateFiles(absoluteSourceDirectory).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string sourceFilepath = enumerator.Current;
					string filename = Path.GetFileName(sourceFilepath);
					string destinationFilepath = Path.Combine(absoluteDestinationDirectory, filename);
					if (File.Exists(destinationFilepath))
					{
						iCloudFileCache.Log.Info("Skipping {0}, because it already exists in the destination directory.", new object[]
						{
							filename
						});
					}
					else
					{
						if (!createdDestinationDirectory)
						{
							createdDestinationDirectory = true;
							Directory.CreateDirectory(absoluteDestinationDirectory);
						}
						File.Copy(sourceFilepath, destinationFilepath, false);
						iCloudFileCache.Log.Info("Copied file {0} to {1}.", new object[]
						{
							sourceFilepath,
							destinationFilepath
						});
						this._auditTrail.RecordEvent("iCloudFileCache.CopyFile", delegate(Dictionary<string, string> metadata)
						{
							metadata["fromFilepath"] = sourceFilepath;
							metadata["toFilepath"] = destinationFilepath;
						});
					}
				}
			}
		}
		catch (Exception exception)
		{
			iCloudFileCache.Log.Error("Failed to copy files.\n{0}", new object[]
			{
				exception
			});
		}
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x000124B0 File Offset: 0x000106B0
	public DateTime GetFileModifiedTime(string filepath)
	{
		return File.GetLastWriteTimeUtc(this.GetAbsolutePath(filepath));
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x000124BE File Offset: 0x000106BE
	private string GetAbsolutePath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return this._hardwareCapabilities.PersistentStoragePath;
		}
		return Path.Combine(this._hardwareCapabilities.PersistentStoragePath, path);
	}

	// Token: 0x04000229 RID: 553
	[Dependency]
	private IHardwareCapabilities _hardwareCapabilities;

	// Token: 0x0400022A RID: 554
	[Dependency]
	private Diagnostics.StorageAuditTrail _auditTrail;

	// Token: 0x0400022B RID: 555
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("iCloudFileCache");
}
