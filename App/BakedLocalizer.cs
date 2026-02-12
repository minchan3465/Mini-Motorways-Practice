using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class BakedLocalizer : MonoBehaviour
{
	// Token: 0x06000E32 RID: 3634 RVA: 0x0002FF94 File Offset: 0x0002E194
	public bool GetLocalization(StringId fromId, out string localizedString, out TMP_FontAsset fontAsset)
	{
		LocaleDatabase.LocaleId localeId = this.GetLocaleId();
		string fromString = fromId.ToString();
		foreach (BakedLocalizer.MappingEntry kvp in this._localizationMapping)
		{
			if (kvp.StringId == fromString)
			{
				localizedString = kvp.Value.Get(localeId);
				if (string.IsNullOrEmpty(localizedString))
				{
					Diagnostics.FailAssert("Unable to find localization {0} in language {1}. Defaulting to English", new object[]
					{
						fromId,
						localeId
					});
					localeId = LocaleDatabase.LocaleId.en_US;
					localizedString = kvp.Value.Get(localeId);
				}
				fontAsset = this.GetFontAsset(localeId);
				return true;
			}
		}
		localizedString = null;
		fontAsset = null;
		return false;
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x00030064 File Offset: 0x0002E264
	private TMP_FontAsset GetFontAsset(LocaleDatabase.LocaleId fromLocale)
	{
		string fontCharSet;
		if (fromLocale <= LocaleDatabase.LocaleId.ja)
		{
			if (fromLocale == LocaleDatabase.LocaleId.ar)
			{
				fontCharSet = "ar";
				goto IL_4F;
			}
			if (fromLocale == LocaleDatabase.LocaleId.ja)
			{
				fontCharSet = "jp";
				goto IL_4F;
			}
		}
		else
		{
			if (fromLocale == LocaleDatabase.LocaleId.ko)
			{
				fontCharSet = "kr";
				goto IL_4F;
			}
			if (fromLocale == LocaleDatabase.LocaleId.zh_CN)
			{
				fontCharSet = "sc";
				goto IL_4F;
			}
			if (fromLocale == LocaleDatabase.LocaleId.zh_TW)
			{
				fontCharSet = "tc";
				goto IL_4F;
			}
		}
		fontCharSet = "latin";
		IL_4F:
		return this._fontDatabase.GetFont(fontCharSet).FontAsset;
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x000300D1 File Offset: 0x0002E2D1
	private LocaleDatabase.LocaleId GetLocaleId()
	{
		return UnityLocaleQuery.GetLocaleId(Application.systemLanguage);
	}

	// Token: 0x04000854 RID: 2132
	[SerializeField]
	private FontDatabase _fontDatabase;

	// Token: 0x04000855 RID: 2133
	[SerializeField]
	private List<BakedLocalizer.MappingEntry> _localizationMapping;

	// Token: 0x02000254 RID: 596
	[Serializable]
	private class LocalizedValues
	{
		// Token: 0x06000E36 RID: 3638 RVA: 0x000300E0 File Offset: 0x0002E2E0
		public string Get(LocaleDatabase.LocaleId localeId)
		{
			if (localeId <= LocaleDatabase.LocaleId.nl)
			{
				if (localeId <= LocaleDatabase.LocaleId.ar)
				{
					if (localeId != LocaleDatabase.LocaleId.en_US)
					{
						if (localeId != LocaleDatabase.LocaleId.ar)
						{
							goto IL_EF;
						}
						return this.Arabic;
					}
				}
				else
				{
					switch (localeId)
					{
					case LocaleDatabase.LocaleId.de:
						return this.German;
					case LocaleDatabase.LocaleId.el:
					case LocaleDatabase.LocaleId.eo:
					case LocaleDatabase.LocaleId.es_MX:
					case LocaleDatabase.LocaleId.fi:
						goto IL_EF;
					case LocaleDatabase.LocaleId.en_AU:
					case LocaleDatabase.LocaleId.en_GB:
						break;
					case LocaleDatabase.LocaleId.es_ES:
						return this.Spanish;
					case LocaleDatabase.LocaleId.fr:
						return this.French;
					default:
						switch (localeId)
						{
						case LocaleDatabase.LocaleId.it:
							return this.Italian;
						case LocaleDatabase.LocaleId.ja:
							return this.Japanese;
						case LocaleDatabase.LocaleId.ko:
							return this.Korean;
						case LocaleDatabase.LocaleId.mi:
						case LocaleDatabase.LocaleId.ms:
							goto IL_EF;
						case LocaleDatabase.LocaleId.nl:
							return this.Dutch;
						default:
							goto IL_EF;
						}
						break;
					}
				}
				return this.English;
			}
			if (localeId <= LocaleDatabase.LocaleId.ru)
			{
				if (localeId == LocaleDatabase.LocaleId.pt_BR)
				{
					return this.Portugues;
				}
				if (localeId == LocaleDatabase.LocaleId.ru)
				{
					return this.Russian;
				}
			}
			else
			{
				if (localeId == LocaleDatabase.LocaleId.tr)
				{
					return this.Turkish;
				}
				if (localeId == LocaleDatabase.LocaleId.zh_CN)
				{
					return this.ChineseSimplified;
				}
				if (localeId == LocaleDatabase.LocaleId.zh_TW)
				{
					return this.ChineseTraditional;
				}
			}
			IL_EF:
			return this.English;
		}

		// Token: 0x04000856 RID: 2134
		public string Arabic;

		// Token: 0x04000857 RID: 2135
		public string German;

		// Token: 0x04000858 RID: 2136
		public string English;

		// Token: 0x04000859 RID: 2137
		public string Spanish;

		// Token: 0x0400085A RID: 2138
		public string French;

		// Token: 0x0400085B RID: 2139
		public string Italian;

		// Token: 0x0400085C RID: 2140
		public string Japanese;

		// Token: 0x0400085D RID: 2141
		public string Korean;

		// Token: 0x0400085E RID: 2142
		public string Dutch;

		// Token: 0x0400085F RID: 2143
		public string Portugues;

		// Token: 0x04000860 RID: 2144
		public string Russian;

		// Token: 0x04000861 RID: 2145
		public string Turkish;

		// Token: 0x04000862 RID: 2146
		public string ChineseSimplified;

		// Token: 0x04000863 RID: 2147
		public string ChineseTraditional;
	}

	// Token: 0x02000255 RID: 597
	[Serializable]
	private class MappingEntry
	{
		// Token: 0x04000864 RID: 2148
		[EnumSearch(typeof(StringId), false, isString = true)]
		public string StringId;

		// Token: 0x04000865 RID: 2149
		public BakedLocalizer.LocalizedValues Value;
	}
}
