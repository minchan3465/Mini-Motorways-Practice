using System;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class UnityLocaleQuery
{
	// Token: 0x06000512 RID: 1298 RVA: 0x00011AC0 File Offset: 0x0000FCC0
	public static LocaleDatabase.LocaleId GetLocaleId(LocaleDatabase localeDatabase)
	{
		LocaleDatabase.LocaleId systemLocaleId = UnityLocaleQuery.GetLocaleId(Application.systemLanguage);
		if (localeDatabase.IsLocaleSelectable(systemLocaleId))
		{
			return systemLocaleId;
		}
		return LocaleDatabase.LocaleId.en_US;
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x00011AE4 File Offset: 0x0000FCE4
	public static LocaleDatabase.LocaleId GetLocaleId(SystemLanguage systemLanguage)
	{
		LocaleDatabase.LocaleId systemLocaleId = LocaleDatabase.LocaleId.Unknown;
		switch (systemLanguage)
		{
		case SystemLanguage.Arabic:
			systemLocaleId = LocaleDatabase.LocaleId.ar;
			break;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
			systemLocaleId = LocaleDatabase.LocaleId.zh_CN;
			break;
		case SystemLanguage.Czech:
			systemLocaleId = LocaleDatabase.LocaleId.cs;
			break;
		case SystemLanguage.Danish:
			systemLocaleId = LocaleDatabase.LocaleId.da;
			break;
		case SystemLanguage.Dutch:
			systemLocaleId = LocaleDatabase.LocaleId.nl;
			break;
		case SystemLanguage.English:
			systemLocaleId = LocaleDatabase.LocaleId.en_US;
			break;
		case SystemLanguage.Finnish:
			systemLocaleId = LocaleDatabase.LocaleId.fi;
			break;
		case SystemLanguage.French:
			systemLocaleId = LocaleDatabase.LocaleId.fr;
			break;
		case SystemLanguage.German:
			systemLocaleId = LocaleDatabase.LocaleId.de;
			break;
		case SystemLanguage.Italian:
			systemLocaleId = LocaleDatabase.LocaleId.it;
			break;
		case SystemLanguage.Japanese:
			systemLocaleId = LocaleDatabase.LocaleId.ja;
			break;
		case SystemLanguage.Korean:
			systemLocaleId = LocaleDatabase.LocaleId.ko;
			break;
		case SystemLanguage.Norwegian:
			systemLocaleId = LocaleDatabase.LocaleId.no;
			break;
		case SystemLanguage.Polish:
			systemLocaleId = LocaleDatabase.LocaleId.pl;
			break;
		case SystemLanguage.Portuguese:
			systemLocaleId = LocaleDatabase.LocaleId.pt_BR;
			break;
		case SystemLanguage.Russian:
			systemLocaleId = LocaleDatabase.LocaleId.ru;
			break;
		case SystemLanguage.Spanish:
			systemLocaleId = LocaleDatabase.LocaleId.es_ES;
			break;
		case SystemLanguage.Swedish:
			systemLocaleId = LocaleDatabase.LocaleId.sv_SE;
			break;
		case SystemLanguage.Thai:
			systemLocaleId = LocaleDatabase.LocaleId.th;
			break;
		case SystemLanguage.Turkish:
			systemLocaleId = LocaleDatabase.LocaleId.tr;
			break;
		case SystemLanguage.Ukrainian:
			systemLocaleId = LocaleDatabase.LocaleId.uk;
			break;
		case SystemLanguage.ChineseTraditional:
			systemLocaleId = LocaleDatabase.LocaleId.zh_TW;
			break;
		}
		return systemLocaleId;
	}
}
