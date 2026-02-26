using System;

// Token: 0x02000072 RID: 114
public class AsyncRequestHandle
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060000F1 RID: 241 RVA: 0x00004B7A File Offset: 0x00002D7A
	// (set) Token: 0x060000F2 RID: 242 RVA: 0x00004B82 File Offset: 0x00002D82
	public bool IsActive { get; private set; } = true;

	// Token: 0x060000F3 RID: 243 RVA: 0x00004B8B File Offset: 0x00002D8B
	public void Cancel()
	{
		this.IsActive = false;
	}

	// Token: 0x04000061 RID: 97
	public static readonly AsyncRequestHandle CompletedRequestHandle = new AsyncRequestHandle();
}
