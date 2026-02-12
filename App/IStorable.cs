using System;

// Token: 0x02000214 RID: 532
public interface IStorable
{
	// Token: 0x170002C8 RID: 712
	// (get) Token: 0x06000CC1 RID: 3265
	// (set) Token: 0x06000CC2 RID: 3266
	DateTime UtcTimestamp { get; set; }

	// Token: 0x170002C9 RID: 713
	// (get) Token: 0x06000CC3 RID: 3267
	// (set) Token: 0x06000CC4 RID: 3268
	bool IsAuthoritative { get; set; }
}
