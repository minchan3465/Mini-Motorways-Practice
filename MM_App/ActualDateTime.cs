using System;

// Token: 0x0200007B RID: 123
public class ActualDateTime : IGameDateTime
{
	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000138 RID: 312 RVA: 0x00004DAC File Offset: 0x00002FAC
	public DateTime LocalNow
	{
		get
		{
			return DateTime.Now;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000139 RID: 313 RVA: 0x00004DB3 File Offset: 0x00002FB3
	public DateTime LocalToday
	{
		get
		{
			return DateTime.Today;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600013A RID: 314 RVA: 0x00004DBA File Offset: 0x00002FBA
	public DateTime UtcNow
	{
		get
		{
			return DateTime.UtcNow;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x0600013B RID: 315 RVA: 0x00004DC4 File Offset: 0x00002FC4
	public DateTime UtcToday
	{
		get
		{
			return DateTime.UtcNow.Date;
		}
	}
}
