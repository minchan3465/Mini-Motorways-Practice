using System;

// Token: 0x0200007E RID: 126
public interface IGameDateTime
{
	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000150 RID: 336
	DateTime LocalNow { get; }

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000151 RID: 337
	DateTime LocalToday { get; }

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000152 RID: 338
	DateTime UtcNow { get; }

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000153 RID: 339
	DateTime UtcToday { get; }
}
