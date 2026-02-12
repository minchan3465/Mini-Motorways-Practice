using System;

// Token: 0x0200007C RID: 124
public class AdjustableGameDateTime : IGameDateTime
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x0600013D RID: 317 RVA: 0x00004DDE File Offset: 0x00002FDE
	// (set) Token: 0x0600013E RID: 318 RVA: 0x00004DE6 File Offset: 0x00002FE6
	public TimeSpan UtcOffset { get; set; }

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600013F RID: 319 RVA: 0x00004DEF File Offset: 0x00002FEF
	public DateTime LocalNow
	{
		get
		{
			return TimeZoneInfo.ConvertTimeFromUtc(this.UtcNow, this._localTimeZoneInfo);
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000140 RID: 320 RVA: 0x00004E04 File Offset: 0x00003004
	public DateTime LocalToday
	{
		get
		{
			return this.LocalNow.Date;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000141 RID: 321 RVA: 0x00004E20 File Offset: 0x00003020
	public DateTime UtcNow
	{
		get
		{
			DateTime? frozenUtcNow = this._frozenUtcNow;
			if (frozenUtcNow == null)
			{
				return DateTime.UtcNow + this.UtcOffset;
			}
			return frozenUtcNow.GetValueOrDefault();
		}
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000142 RID: 322 RVA: 0x00004E58 File Offset: 0x00003058
	public DateTime UtcToday
	{
		get
		{
			return this.UtcNow.Date;
		}
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00004E73 File Offset: 0x00003073
	public void SetUtcNow(DateTime newUtcNow)
	{
		if (this._frozenUtcNow != null)
		{
			this._frozenUtcNow = new DateTime?(newUtcNow);
			return;
		}
		this.UtcOffset += newUtcNow - this.UtcNow;
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00004EAC File Offset: 0x000030AC
	public void UseActualUtcNow()
	{
		this.UtcOffset = TimeSpan.Zero;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00004EB9 File Offset: 0x000030B9
	public void SetLocalTimeZoneInfo(TimeZoneInfo timeZoneInfo)
	{
		this._localTimeZoneInfo = timeZoneInfo;
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00004EC2 File Offset: 0x000030C2
	public void Freeze()
	{
		this._frozenUtcNow = new DateTime?(this.UtcNow);
	}

	// Token: 0x06000147 RID: 327 RVA: 0x00004ED5 File Offset: 0x000030D5
	public void Unfreeze()
	{
		this._frozenUtcNow = null;
	}

	// Token: 0x0400006D RID: 109
	private DateTime? _frozenUtcNow;

	// Token: 0x0400006E RID: 110
	private TimeZoneInfo _localTimeZoneInfo = TimeZoneInfo.Local;
}
