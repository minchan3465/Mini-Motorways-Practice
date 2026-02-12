using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class DefaultFileSystem : IFileSystem
{
	// Token: 0x06000380 RID: 896 RVA: 0x0000E764 File Offset: 0x0000C964
	public List<string> GetFilesInDirectory(string directory)
	{
		List<string> files = new List<string>();
		foreach (string filename in Directory.EnumerateFiles(DefaultFileSystem.GetAbsolutePath(directory)))
		{
			files.Add(filename);
		}
		return files;
	}

	// Token: 0x06000381 RID: 897 RVA: 0x0000E7C0 File Offset: 0x0000C9C0
	public List<string> GetDirectoriesInDirectory(string directory)
	{
		List<string> files = new List<string>();
		foreach (string filename in Directory.EnumerateDirectories(DefaultFileSystem.GetAbsolutePath(directory)))
		{
			files.Add(filename);
		}
		return files;
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0000E81C File Offset: 0x0000CA1C
	public byte[] ReadFile(string filepath)
	{
		string fullPath = DefaultFileSystem.GetAbsolutePath(filepath);
		byte[] result;
		try
		{
			using (FileStream fileStream = File.Open(fullPath, FileMode.Open))
			{
				long fileLengthLong = fileStream.Length;
				if (fileLengthLong > 2147483647L)
				{
					DefaultFileSystem.Log.Error("ReadFile({0}) failed! File is {1} bytes, which is larger than the maximum supported length of {2} bytes.", new object[]
					{
						filepath,
						fileLengthLong,
						int.MaxValue
					});
					result = null;
				}
				else
				{
					int fileLength = (int)fileLengthLong;
					byte[] data = new byte[fileLength];
					int bytesRead = fileStream.Read(data, 0, fileLength);
					if (bytesRead != fileLength)
					{
						DefaultFileSystem.Log.Warn("ReadFile({0}) only read {1} bytes, not the expected {2} bytes.", new object[]
						{
							filepath,
							bytesRead,
							fileLength
						});
						Array.Resize<byte>(ref data, bytesRead);
					}
					result = data;
				}
			}
		}
		catch (Exception exception)
		{
			DefaultFileSystem.Log.Error("ReadFile({0}) failed.\n{1}", new object[]
			{
				filepath,
				exception
			});
			result = null;
		}
		return result;
	}

	// Token: 0x06000383 RID: 899 RVA: 0x0000E924 File Offset: 0x0000CB24
	public bool WriteFile(string filepath, byte[] data)
	{
		string fullPath = DefaultFileSystem.GetAbsolutePath(filepath);
		try
		{
			using (FileStream stream = File.OpenWrite(fullPath))
			{
				stream.Write(data, 0, data.Length);
			}
		}
		catch (Exception exception)
		{
			DefaultFileSystem.Log.Error("WriteFile({0}) failed.\n{1}", new object[]
			{
				filepath,
				exception
			});
			return false;
		}
		return true;
	}

	// Token: 0x06000384 RID: 900 RVA: 0x0000E99C File Offset: 0x0000CB9C
	public bool DeleteFile(string filepath)
	{
		string fullPath = DefaultFileSystem.GetAbsolutePath(filepath);
		try
		{
			File.Delete(fullPath);
		}
		catch (Exception exception)
		{
			DefaultFileSystem.Log.Error("DeleteFile({0}) failed.\n{1}", new object[]
			{
				filepath,
				exception
			});
			return false;
		}
		return true;
	}

	// Token: 0x06000385 RID: 901 RVA: 0x0000E9F0 File Offset: 0x0000CBF0
	private static string GetAbsolutePath(string path)
	{
		return Path.Combine(Application.persistentDataPath, path);
	}

	// Token: 0x0400017F RID: 383
	private static readonly Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DefaultFileSystem");
}
