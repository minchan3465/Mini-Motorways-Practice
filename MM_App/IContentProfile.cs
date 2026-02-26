using System;

// Token: 0x020000E6 RID: 230
public interface IContentProfile
{
	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x060004B8 RID: 1208
	bool CanUseIncompleteLocales { get; }

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x060004B9 RID: 1209
	LocaleDatabase.LocaleId[] SupportedLocales { get; }

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x060004BA RID: 1210
	bool AllowSaving { get; }
}
