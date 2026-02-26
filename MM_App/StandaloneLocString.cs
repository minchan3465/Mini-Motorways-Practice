using System;
using System.Collections.Generic;
using Factory;
using Factory.Pools;

// Token: 0x02000189 RID: 393
public class StandaloneLocString : IReusable, IReleasedFromScopeHandler
{
	// Token: 0x060008DA RID: 2266 RVA: 0x0001D1C7 File Offset: 0x0001B3C7
	public void Init(StringKey newKey)
	{
		this._localizedKey = newKey;
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0001D1D0 File Offset: 0x0001B3D0
	public virtual void ChangeLocale(Locale newLocale)
	{
		if (Diagnostics.Verify(newLocale != null, "Can't change to a null locale!"))
		{
			this._localizedString = newLocale.GetString(this._localizedKey);
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0001D1F4 File Offset: 0x0001B3F4
	public override string ToString()
	{
		if (this._localizedKey == null)
		{
			return "";
		}
		if (this._localizedString == null || this._localizedString.locale == null)
		{
			this._localizedString = this._localeDatabase.CurrentLocale.GetString(this._localizedKey);
		}
		if (this.IsRightToLeft())
		{
			return StandaloneLocString.ReverseLeftToRightText(this._localizedString.ToString());
		}
		return this._localizedString.ToString();
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x060008DD RID: 2269 RVA: 0x0001D270 File Offset: 0x0001B470
	public Locale Locale
	{
		get
		{
			if (this._localizedString != null)
			{
				return this._localizedString.locale;
			}
			return null;
		}
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x0001D28D File Offset: 0x0001B48D
	public virtual bool IsRightToLeft()
	{
		return this._localizedString != null && this._localizedString.locale != null && this._localizedString.locale.TextDirection == TextDirection.RightToLeft;
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0001D2BF File Offset: 0x0001B4BF
	public void OnReleasedFromScope(IScope scope)
	{
		if (this._localizedKey != null)
		{
			scope.Release(this._localizedKey);
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0001D2DC File Offset: 0x0001B4DC
	public void Reset()
	{
		this._localizedKey = null;
		this._localizedString = null;
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x0001D2EC File Offset: 0x0001B4EC
	public static StandaloneLocString CreateString(IScope scope, StringKey key)
	{
		StandaloneLocString standaloneLocString = scope.Get<StandaloneLocString>();
		standaloneLocString.Init(key);
		return standaloneLocString;
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0001D2FC File Offset: 0x0001B4FC
	public static StandaloneLocString CreateString(IScope scope, StringId fromKey)
	{
		StringKey newKey = scope.Get<StringKey>();
		newKey.InitWithStringId(fromKey);
		return StandaloneLocString.CreateString(scope, newKey);
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0001D320 File Offset: 0x0001B520
	public static StandaloneLocString CreateString(IScope scope, string fromKey)
	{
		StringKey newKey = scope.Get<StringKey>();
		newKey.InitWithString(fromKey);
		return StandaloneLocString.CreateString(scope, newKey);
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0001D344 File Offset: 0x0001B544
	public static StandaloneLocString CreateNonLocalizedString(IScope scope, string nonLocalizedString)
	{
		StringKey newKey = scope.Get<StringKey>();
		newKey.InitWithNonLocalizedString(nonLocalizedString);
		return StandaloneLocString.CreateString(scope, newKey);
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0001D368 File Offset: 0x0001B568
	public static StandaloneLocString CreateLocalizedNumberString(IScope scope, int number)
	{
		StringKey newKey = scope.Get<StringKey>();
		string scoreString = scope.Get<LocaleDatabase>().CurrentLocale.FormatNumber(number);
		newKey.InitWithNonLocalizedString(scoreString);
		return StandaloneLocString.CreateString(scope, newKey);
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0001D39C File Offset: 0x0001B59C
	private static string ReverseLeftToRightText(string originalString)
	{
		if (originalString.Length == 0)
		{
			return originalString;
		}
		bool isXml = originalString[0] == '<';
		List<string> chunks = new List<string>();
		int chunkStart = 0;
		for (int charIndex = 1; charIndex < originalString.Length; charIndex++)
		{
			if (isXml)
			{
				if (originalString[charIndex] == '>')
				{
					chunks.Add(originalString.Substring(chunkStart, charIndex - chunkStart + 1));
					chunkStart = charIndex + 1;
					isXml = false;
				}
			}
			else if (originalString[charIndex] == '<')
			{
				chunks.Add(originalString.Substring(chunkStart, charIndex - chunkStart));
				chunkStart = charIndex;
				isXml = true;
			}
		}
		if (chunkStart < originalString.Length)
		{
			chunks.Add(originalString.Substring(chunkStart, originalString.Length - chunkStart));
		}
		string preparedText = "";
		foreach (string chunk in chunks)
		{
			if (chunk.Length != 0)
			{
				if (chunk[0] == '<')
				{
					preparedText += chunk;
				}
				else
				{
					int tokenStart = 0;
					int neutralTokenStart = -1;
					bool rtlToken = StandaloneLocString.IsArabic((int)chunk[0]) || StandaloneLocString.IsNeutralCharacter((int)chunk[0]);
					for (int charIndex2 = 1; charIndex2 < chunk.Length; charIndex2++)
					{
						if (StandaloneLocString.IsNeutralCharacter((int)chunk[charIndex2]))
						{
							if (neutralTokenStart == -1)
							{
								neutralTokenStart = charIndex2;
							}
						}
						else
						{
							if (rtlToken && !StandaloneLocString.IsArabic((int)chunk[charIndex2]))
							{
								preparedText += chunk.Substring(tokenStart, charIndex2 - tokenStart);
								tokenStart = charIndex2;
								rtlToken = false;
							}
							else if (!rtlToken && StandaloneLocString.IsArabic((int)chunk[charIndex2]))
							{
								int tokenEnd = (neutralTokenStart == -1) ? charIndex2 : neutralTokenStart;
								preparedText += StandaloneLocString.ReverseString(chunk.Substring(tokenStart, tokenEnd - tokenStart));
								tokenStart = tokenEnd;
								rtlToken = true;
							}
							neutralTokenStart = -1;
						}
					}
					if (rtlToken)
					{
						preparedText += chunk.Substring(tokenStart);
					}
					else if (neutralTokenStart == -1)
					{
						preparedText += StandaloneLocString.ReverseString(chunk.Substring(tokenStart));
					}
					else
					{
						preparedText += StandaloneLocString.ReverseString(chunk.Substring(tokenStart, neutralTokenStart - tokenStart));
						preparedText += StandaloneLocString.ReverseString(chunk.Substring(neutralTokenStart));
					}
				}
			}
		}
		preparedText = preparedText.Replace("‏", "");
		return preparedText;
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0001D60C File Offset: 0x0001B80C
	private static string ReverseString(string s)
	{
		char[] array = s.ToCharArray();
		Array.Reverse<char>(array);
		return new string(array);
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0001D61F File Offset: 0x0001B81F
	private static bool IsNeutralCharacter(int code)
	{
		return (code >= 0 && code <= 47) || (code >= 58 && code <= 64);
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x0001D63C File Offset: 0x0001B83C
	private static bool IsArabic(int code)
	{
		return code >= 1536 && ((code >= 1536 && code <= 1791) || (code >= 1872 && code <= 1919) || (code >= 2208 && code <= 2303) || (code >= 64336 && code <= 65023) || (code >= 65136 && code <= 65279));
	}

	// Token: 0x04000472 RID: 1138
	[Dependency]
	protected LocaleDatabase _localeDatabase;

	// Token: 0x04000473 RID: 1139
	protected StringKey _localizedKey;

	// Token: 0x04000474 RID: 1140
	protected LocalizedString _localizedString;
}
