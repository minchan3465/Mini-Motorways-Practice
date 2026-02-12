using System;

// Token: 0x020000BE RID: 190
public class DemoContentProfile : IContentProfile
{
	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000377 RID: 887 RVA: 0x0000222C File Offset: 0x0000042C
	public bool CanUseIncompleteLocales
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000378 RID: 888 RVA: 0x0000222C File Offset: 0x0000042C
	public bool AllowSaving
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000379 RID: 889 RVA: 0x0000E747 File Offset: 0x0000C947
	public LocaleDatabase.LocaleId[] SupportedLocales
	{
		get
		{
			return new LocaleDatabase.LocaleId[]
			{
				LocaleDatabase.LocaleId.en_US,
				LocaleDatabase.LocaleId.ar,
				LocaleDatabase.LocaleId.bg,
				LocaleDatabase.LocaleId.ca,
				LocaleDatabase.LocaleId.cs,
				LocaleDatabase.LocaleId.cy,
				LocaleDatabase.LocaleId.da,
				LocaleDatabase.LocaleId.de,
				LocaleDatabase.LocaleId.el,
				LocaleDatabase.LocaleId.en_GB,
				LocaleDatabase.LocaleId.eo,
				LocaleDatabase.LocaleId.es_ES,
				LocaleDatabase.LocaleId.es_MX,
				LocaleDatabase.LocaleId.fi,
				LocaleDatabase.LocaleId.fr,
				LocaleDatabase.LocaleId.ga_IE,
				LocaleDatabase.LocaleId.hi,
				LocaleDatabase.LocaleId.hr,
				LocaleDatabase.LocaleId.hu,
				LocaleDatabase.LocaleId.id,
				LocaleDatabase.LocaleId.it,
				LocaleDatabase.LocaleId.ja,
				LocaleDatabase.LocaleId.ko,
				LocaleDatabase.LocaleId.mi,
				LocaleDatabase.LocaleId.ms,
				LocaleDatabase.LocaleId.nl,
				LocaleDatabase.LocaleId.nn_NO,
				LocaleDatabase.LocaleId.no,
				LocaleDatabase.LocaleId.pl,
				LocaleDatabase.LocaleId.pt_BR,
				LocaleDatabase.LocaleId.pt_PT,
				LocaleDatabase.LocaleId.ru,
				LocaleDatabase.LocaleId.sk,
				LocaleDatabase.LocaleId.sr,
				LocaleDatabase.LocaleId.sr_CS,
				LocaleDatabase.LocaleId.sv_SE,
				LocaleDatabase.LocaleId.sv_FI,
				LocaleDatabase.LocaleId.tr,
				LocaleDatabase.LocaleId.tg,
				LocaleDatabase.LocaleId.th,
				LocaleDatabase.LocaleId.uk,
				LocaleDatabase.LocaleId.zh_CN,
				LocaleDatabase.LocaleId.zh_HK,
				LocaleDatabase.LocaleId.zh_TW
			};
		}
	}
}
