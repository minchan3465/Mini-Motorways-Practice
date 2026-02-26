using System;
using System.IO;

// Token: 0x02000200 RID: 512
public abstract class BaseGameJournalSave : IGameJournalSave, IBinarySerializableSaveData, IStorable
{
	// Token: 0x1700029B RID: 667
	// (get) Token: 0x06000C11 RID: 3089 RVA: 0x00029408 File Offset: 0x00027608
	// (set) Token: 0x06000C12 RID: 3090 RVA: 0x00029410 File Offset: 0x00027610
	public Player Player { get; set; }

	// Token: 0x1700029C RID: 668
	// (get) Token: 0x06000C13 RID: 3091 RVA: 0x00029419 File Offset: 0x00027619
	// (set) Token: 0x06000C14 RID: 3092 RVA: 0x00029421 File Offset: 0x00027621
	public bool IsAuthoritative { get; set; }

	// Token: 0x1700029D RID: 669
	// (get) Token: 0x06000C15 RID: 3093 RVA: 0x0002942A File Offset: 0x0002762A
	public bool CanDelete
	{
		get
		{
			return this.Player != null;
		}
	}

	// Token: 0x06000C16 RID: 3094 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void InitializeWithBytes(byte[] saveDataAsBytes)
	{
	}

	// Token: 0x06000C17 RID: 3095
	public abstract byte[] GetBytesForSerializing();

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06000C18 RID: 3096 RVA: 0x00029435 File Offset: 0x00027635
	// (set) Token: 0x06000C19 RID: 3097 RVA: 0x0002943D File Offset: 0x0002763D
	public DateTime UtcTimestamp { get; set; } = DateTime.MinValue;

	// Token: 0x06000C1A RID: 3098 RVA: 0x000022F5 File Offset: 0x000004F5
	public virtual void OnSerializeBeforeData(BinaryWriter binaryWriter)
	{
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0000222C File Offset: 0x0000042C
	public virtual IBinarySerializableSaveData.HeaderValidationResult ValidateHeader(BinaryReader binaryReader)
	{
		return IBinarySerializableSaveData.HeaderValidationResult.Success;
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06000C1C RID: 3100 RVA: 0x00029446 File Offset: 0x00027646
	// (set) Token: 0x06000C1D RID: 3101 RVA: 0x0002944E File Offset: 0x0002764E
	public string DeviceId { get; set; }

	// Token: 0x04000704 RID: 1796
	public static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("BaseUserProfile");
}
