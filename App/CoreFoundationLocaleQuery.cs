using System;

// Token: 0x020000F3 RID: 243
public class CoreFoundationLocaleQuery
{
	// Token: 0x0600050E RID: 1294 RVA: 0x00011A80 File Offset: 0x0000FC80
	public static LocaleDatabase.LocaleId GetLocaleId(LocaleDatabase localeDatabase)
	{
		int localeCount = CoreFoundationLocaleQuery.GetLocaleCount();
		for (int localeIndex = 0; localeIndex < localeCount; localeIndex++)
		{
			string iOSLocaleId = CoreFoundationLocaleQuery.GetLocale(localeIndex);
			Locale matchedLocale = localeDatabase.MatchLocale(iOSLocaleId);
			if (matchedLocale != null)
			{
				return matchedLocale.Id;
			}
		}
		return LocaleDatabase.LocaleId.en_US;
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x000020AA File Offset: 0x000002AA
	private static int GetLocaleCount()
	{
		return 1;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00011AB9 File Offset: 0x0000FCB9
	private static string GetLocale(int index)
	{
		return "en-US";
	}
}
