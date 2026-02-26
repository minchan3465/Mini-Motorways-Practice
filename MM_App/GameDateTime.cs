using System;

// Token: 0x0200007D RID: 125
public static class GameDateTime
{
	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000149 RID: 329 RVA: 0x00004EF6 File Offset: 0x000030F6
	// (set) Token: 0x0600014A RID: 330 RVA: 0x00004EFD File Offset: 0x000030FD
	public static IGameDateTime Backend { get; set; } = new ActualDateTime();

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x0600014B RID: 331 RVA: 0x00004F05 File Offset: 0x00003105
	public static DateTime LocalNow
	{
		get
		{
			return GameDateTime.Backend.LocalNow;
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x0600014C RID: 332 RVA: 0x00004F11 File Offset: 0x00003111
	public static DateTime UtcNow
	{
		get
		{
			return GameDateTime.Backend.UtcNow;
		}
	}

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600014D RID: 333 RVA: 0x00004F1D File Offset: 0x0000311D
	public static DateTime LocalToday
	{
		get
		{
			return GameDateTime.Backend.LocalToday;
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x0600014E RID: 334 RVA: 0x00004F29 File Offset: 0x00003129
	public static DateTime UtcToday
	{
		get
		{
			return GameDateTime.Backend.UtcToday;
		}
	}
}
