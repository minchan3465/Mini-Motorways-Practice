using System;
using System.IO;

// Token: 0x02000207 RID: 519
public interface IBinarySerializableSaveData : IStorable
{
	// Token: 0x06000C78 RID: 3192
	void InitializeWithBytes(byte[] saveDataAsBytes);

	// Token: 0x06000C79 RID: 3193
	byte[] GetBytesForSerializing();

	// Token: 0x06000C7A RID: 3194
	void OnSerializeBeforeData(BinaryWriter binaryWriter);

	// Token: 0x06000C7B RID: 3195
	IBinarySerializableSaveData.HeaderValidationResult ValidateHeader(BinaryReader binaryReader);

	// Token: 0x02000208 RID: 520
	public enum HeaderValidationResult
	{
		// Token: 0x0400071F RID: 1823
		Success,
		// Token: 0x04000720 RID: 1824
		HashCodesMismatched,
		// Token: 0x04000721 RID: 1825
		InvalidHeader
	}
}
