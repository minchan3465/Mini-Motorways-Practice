using System;

// Token: 0x02000215 RID: 533
public interface IStorableTypeHandler
{
	// Token: 0x06000CC5 RID: 3269
	bool IsFilenameRecognized(string filename, out string playerId, out string deviceId);

	// Token: 0x06000CC6 RID: 3270
	string GetFilename(IStorable storable);

	// Token: 0x06000CC7 RID: 3271
	string GetFilename(string playerId, string deviceId);

	// Token: 0x06000CC8 RID: 3272
	IStorable Load(byte[] data);

	// Token: 0x06000CC9 RID: 3273
	byte[] Store(IStorable storable);

	// Token: 0x06000CCA RID: 3274
	bool ProcessLoadedStorable(IStorable storable, string playerId, string deviceId);

	// Token: 0x06000CCB RID: 3275
	void ProcessDeletedStorable(string playerId, string deviceId);
}
