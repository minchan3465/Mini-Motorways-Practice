using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x02000233 RID: 563
public static class StorableUtilities
{
	// Token: 0x06000D5E RID: 3422 RVA: 0x0002BE37 File Offset: 0x0002A037
	public static string GenerateFilename(string prefix, string extension, string playerId)
	{
		return string.Format("{0}{1}{2}", prefix, playerId, extension);
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x0002BE46 File Offset: 0x0002A046
	public static string GenerateFilename(string prefix, string extension, string playerId, string deviceId)
	{
		return string.Format("{0}{1}_{2}{3}", new object[]
		{
			prefix,
			deviceId,
			playerId,
			extension
		});
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x0002BE68 File Offset: 0x0002A068
	public static bool TryParseFilename(string filename, string prefix, string extension, out string playerId)
	{
		playerId = null;
		if (!filename.StartsWith(prefix))
		{
			return false;
		}
		if (!filename.EndsWith(extension))
		{
			return false;
		}
		int prefixLength = prefix.Length;
		int extensionLength = extension.Length;
		int identifierLength = filename.Length - (prefixLength + extensionLength);
		if (identifierLength < 1)
		{
			return false;
		}
		playerId = filename.Substring(prefixLength, identifierLength);
		return true;
	}

	// Token: 0x06000D61 RID: 3425 RVA: 0x0002BEB8 File Offset: 0x0002A0B8
	public static bool TryParseFilename(string filename, string prefix, string extension, out string playerId, out string deviceId)
	{
		playerId = null;
		deviceId = null;
		if (!filename.StartsWith(prefix))
		{
			return false;
		}
		if (!filename.EndsWith(extension))
		{
			return false;
		}
		int prefixLength = prefix.Length;
		int extensionLength = extension.Length;
		int identifierLength = filename.Length - (prefixLength + extensionLength);
		if (identifierLength < 3)
		{
			return false;
		}
		string[] identifiers = filename.Substring(prefixLength, identifierLength).Split('_', StringSplitOptions.None);
		if (identifiers.Length != 2)
		{
			return false;
		}
		deviceId = identifiers[0];
		playerId = identifiers[1];
		return true;
	}

	// Token: 0x06000D62 RID: 3426 RVA: 0x0002BF28 File Offset: 0x0002A128
	public static bool LoadJsonStorable(IJsonSerializableSaveData jsonStorable, byte[] data)
	{
		if (data == null || data.Length < 2)
		{
			return false;
		}
		bool utf8 = false;
		bool utf9 = false;
		if (data.Length >= 2)
		{
			bool flag = data[0] == 254 && data[1] == byte.MaxValue;
			bool utf16LE = data[0] == byte.MaxValue && data[1] == 254;
			utf9 = (flag || utf16LE);
			if (data.Length >= 3)
			{
				utf8 = (data[0] == 239 && data[1] == 187 && data[2] == 191);
			}
			int bytesToRemove = 0;
			if (utf9)
			{
				bytesToRemove = 2;
			}
			if (utf8)
			{
				bytesToRemove = 3;
			}
			if (bytesToRemove > 0)
			{
				byte[] newData = new byte[data.Length - bytesToRemove];
				Buffer.BlockCopy(data, bytesToRemove, newData, 0, data.Length - bytesToRemove);
				data = newData;
			}
		}
		if (!utf8 && !utf9 && data.Length >= 2)
		{
			utf9 = (data[1] == 0);
		}
		string jsonString;
		if (utf9)
		{
			jsonString = Encoding.Unicode.GetString(data);
		}
		else
		{
			jsonString = Encoding.UTF8.GetString(data);
		}
		JSON.Dictionary jsonDictionary = JSON.LoadFromString(jsonString) as JSON.Dictionary;
		if (jsonDictionary == null)
		{
			return false;
		}
		jsonStorable.InitializeWithJson(jsonDictionary);
		return true;
	}

	// Token: 0x06000D63 RID: 3427 RVA: 0x0002C028 File Offset: 0x0002A228
	public static StorableUtilities.LoadResult LoadBinaryStorable(IBinarySerializableSaveData binaryStorable, byte[] data)
	{
		StorableUtilities.LoadResult result;
		using (MemoryStream memoryStream = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				switch (binaryStorable.ValidateHeader(binaryReader))
				{
				case IBinarySerializableSaveData.HeaderValidationResult.Success:
				{
					byte[] savedGameBytes = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
					binaryStorable.InitializeWithBytes(savedGameBytes);
					result = StorableUtilities.LoadResult.Success;
					break;
				}
				case IBinarySerializableSaveData.HeaderValidationResult.HashCodesMismatched:
					result = StorableUtilities.LoadResult.Failed_HeaderHashMismatch;
					break;
				case IBinarySerializableSaveData.HeaderValidationResult.InvalidHeader:
					result = StorableUtilities.LoadResult.Failed_InvalidHeader;
					break;
				default:
					result = StorableUtilities.LoadResult.Failed_InvalidHeader;
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x06000D64 RID: 3428 RVA: 0x0002C0CC File Offset: 0x0002A2CC
	public static byte[] StoreJsonStorable(IJsonSerializableSaveData jsonStorable)
	{
		Dictionary<string, object> saveFileDictionary = jsonStorable.SerializeToJson();
		if (saveFileDictionary == null)
		{
			return null;
		}
		return Encoding.Unicode.GetBytes(Json.Serialize(saveFileDictionary, false));
	}

	// Token: 0x06000D65 RID: 3429 RVA: 0x0002C0F8 File Offset: 0x0002A2F8
	public static byte[] StoreBinaryStorable(IBinarySerializableSaveData binaryStorable)
	{
		byte[] result;
		try
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					binaryStorable.OnSerializeBeforeData(writer);
					writer.Write(binaryStorable.GetBytesForSerializing());
				}
				memoryStream.Close();
				result = memoryStream.ToArray();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = null;
		}
		return result;
	}

	// Token: 0x02000234 RID: 564
	public enum LoadResult
	{
		// Token: 0x0400078A RID: 1930
		Success,
		// Token: 0x0400078B RID: 1931
		Failed_HeaderHashMismatch,
		// Token: 0x0400078C RID: 1932
		Failed_InvalidHeader
	}
}
