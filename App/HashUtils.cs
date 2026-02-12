using System;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

// Token: 0x0200025F RID: 607
public static class HashUtils
{
	// Token: 0x06000E62 RID: 3682 RVA: 0x00030BB8 File Offset: 0x0002EDB8
	public static string GetMD5([CanBeNull] string sourceString)
	{
		byte[] bytes = new UTF8Encoding().GetBytes(sourceString ?? "");
		byte[] hashBytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
		string hashString = "";
		for (int byteIndex = 0; byteIndex < hashBytes.Length; byteIndex++)
		{
			hashString += Convert.ToString(hashBytes[byteIndex], 16).PadLeft(2, '0');
		}
		return hashString.PadLeft(32, '0');
	}
}
