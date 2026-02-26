using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Factory;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class Locale
{
	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x06000894 RID: 2196 RVA: 0x0001B50A File Offset: 0x0001970A
	// (set) Token: 0x06000895 RID: 2197 RVA: 0x0001B512 File Offset: 0x00019712
	public LocaleDatabase.LocaleId Id { get; private set; }

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x06000896 RID: 2198 RVA: 0x0001B51B File Offset: 0x0001971B
	// (set) Token: 0x06000897 RID: 2199 RVA: 0x0001B523 File Offset: 0x00019723
	public string Name { get; private set; }

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x06000898 RID: 2200 RVA: 0x0001B52C File Offset: 0x0001972C
	// (set) Token: 0x06000899 RID: 2201 RVA: 0x0001B534 File Offset: 0x00019734
	public bool IsComplete { get; private set; }

	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x0600089A RID: 2202 RVA: 0x0001B53D File Offset: 0x0001973D
	// (set) Token: 0x0600089B RID: 2203 RVA: 0x0001B545 File Offset: 0x00019745
	public TextDirection TextDirection { get; private set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x0600089C RID: 2204 RVA: 0x0001B54E File Offset: 0x0001974E
	// (set) Token: 0x0600089D RID: 2205 RVA: 0x0001B556 File Offset: 0x00019756
	public DigitGrouping DigitGrouping { get; private set; }

	// Token: 0x170001EB RID: 491
	// (get) Token: 0x0600089E RID: 2206 RVA: 0x0001B55F File Offset: 0x0001975F
	// (set) Token: 0x0600089F RID: 2207 RVA: 0x0001B567 File Offset: 0x00019767
	public StartOfWeek StartOfWeek { get; private set; }

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x060008A0 RID: 2208 RVA: 0x0001B570 File Offset: 0x00019770
	// (set) Token: 0x060008A1 RID: 2209 RVA: 0x0001B578 File Offset: 0x00019778
	public bool CapitaliseNouns { get; private set; }

	// Token: 0x170001ED RID: 493
	// (get) Token: 0x060008A2 RID: 2210 RVA: 0x0001B581 File Offset: 0x00019781
	// (set) Token: 0x060008A3 RID: 2211 RVA: 0x0001B589 File Offset: 0x00019789
	public string Charset { get; private set; }

	// Token: 0x170001EE RID: 494
	// (get) Token: 0x060008A4 RID: 2212 RVA: 0x0001B592 File Offset: 0x00019792
	public bool IsSelectable
	{
		get
		{
			return this.Name != null;
		}
	}

	// Token: 0x170001EF RID: 495
	// (get) Token: 0x060008A5 RID: 2213 RVA: 0x0001B59D File Offset: 0x0001979D
	public LineBreakRule LineBreakRule
	{
		get
		{
			if (this._cannotStartLines != null || this._cannotEndLines != null || this._cannotSplit != null)
			{
				return LineBreakRule.EastAsian;
			}
			return LineBreakRule.Western;
		}
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0001B5BA File Offset: 0x000197BA
	public bool TryGetRawStrings(string stringId, out List<string> strings)
	{
		return Diagnostics.Verify(this._stringTable.TryGetValue(stringId, out strings), "No string id for '{0}' in locale '{1}'", stringId, this.Id);
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0001B5E4 File Offset: 0x000197E4
	public LocalizedString GetString(StringKey key)
	{
		string stringId = key.GetStringId();
		int formIndex = 0;
		if (key.IsPlural())
		{
			formIndex = this.GetPluralForm(key.GetCount());
		}
		if (!this._stringTable.ContainsKey(stringId))
		{
			Locale fallbackLocale = this._database.FallbackLocale;
			if (fallbackLocale != null && fallbackLocale != this)
			{
				return fallbackLocale.GetString(key);
			}
			Diagnostics.FailAssert("Failed to retrieve a string with key {0}", new object[]
			{
				key.GetStringId()
			});
			return new LocalizedString(fallbackLocale, stringId.ToUpperInvariant());
		}
		else
		{
			List<string> localisedStrings = this._stringTable[stringId];
			string localisedString = localisedStrings[Mathf.Min(formIndex, localisedStrings.Count - 1)];
			if (string.IsNullOrEmpty(localisedString))
			{
				for (int i = 0; i < localisedStrings.Count; i++)
				{
					if (localisedStrings[i] != null && localisedStrings[i].Length > 0)
					{
						localisedString = localisedStrings[i];
						break;
					}
				}
			}
			if (localisedString == null)
			{
				return new LocalizedString(this, "");
			}
			IControllerButtonToSymbolService controllerButtonToSymbolService = this._scope.Get<IControllerButtonToSymbolService>();
			foreach (KeyValuePair<string, ControllerButton> actionsToButton in Locale.ActionsToButtons)
			{
				string token = "{ActionIcon=" + actionsToButton.Key + "}";
				for (int indexOfToken = localisedString.IndexOf(token, StringComparison.InvariantCulture); indexOfToken >= 0; indexOfToken = localisedString.IndexOf(token, StringComparison.InvariantCulture))
				{
					string replacement = controllerButtonToSymbolService.GetTextMeshProSymbolTextForControllerButton(actionsToButton.Value);
					localisedString = localisedString.Substring(0, indexOfToken) + replacement + localisedString.Substring(indexOfToken + token.Length);
				}
			}
			if (key.GetParameters() == null)
			{
				return new LocalizedString(this, localisedString);
			}
			foreach (KeyValuePair<string, string> parameter in key.GetParameters())
			{
				string token2 = "{" + parameter.Key + "}";
				int nextIndex = localisedString.IndexOf(token2, StringComparison.InvariantCulture);
				string parameterValueLocaleAdjusted = parameter.Value;
				while (nextIndex >= 0)
				{
					bool duplicatePeriod = false;
					if (parameterValueLocaleAdjusted.Length > 0 && parameterValueLocaleAdjusted[parameterValueLocaleAdjusted.Length - 1] == '.')
					{
						int tokenEndIndex = nextIndex + token2.Length;
						if (tokenEndIndex < localisedString.Length && localisedString[tokenEndIndex] == '.')
						{
							duplicatePeriod = true;
						}
					}
					localisedString = localisedString.Substring(0, nextIndex) + parameterValueLocaleAdjusted + localisedString.Substring(nextIndex + token2.Length + (duplicatePeriod ? 1 : 0));
					nextIndex = localisedString.IndexOf(token2, nextIndex + parameterValueLocaleAdjusted.Length, StringComparison.InvariantCulture);
				}
			}
			return new LocalizedString(this, localisedString);
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0001B8A4 File Offset: 0x00019AA4
	protected static bool IsCJK(int code)
	{
		return code >= 11904 && ((code >= 11904 && code <= 55215) || (code >= 63744 && code <= 64255) || (code >= 65072 && code <= 65103) || (code >= 131072 && code <= 195103));
	}

	// Token: 0x060008A9 RID: 2217 RVA: 0x0001B904 File Offset: 0x00019B04
	protected static bool IsThai(int code)
	{
		return code >= 3584 && code <= 3711;
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x0001B91B File Offset: 0x00019B1B
	protected static bool IsHindi(int code)
	{
		return (code >= 2304 && code <= 2431) || (code >= 43232 && code <= 43263) || (code >= 7376 && code <= 7423);
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0001B954 File Offset: 0x00019B54
	protected static bool IsToneMark(int code)
	{
		return Locale.IsThai(code) && (code == 3633 || (code >= 3636 && code <= 3642) || (code >= 3655 && code <= 3662));
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0001B990 File Offset: 0x00019B90
	public string GetNoun(StringKey key)
	{
		LocalizedString localisedstring = this.GetString(key);
		if (localisedstring.localString == null)
		{
			return null;
		}
		if (this.CapitaliseNouns)
		{
			return localisedstring.localString;
		}
		return localisedstring.localString.ToLower();
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0001B9CC File Offset: 0x00019BCC
	public string DativeFormat(string noun)
	{
		if (this.Id == LocaleDatabase.LocaleId.hr || this.Id == LocaleDatabase.LocaleId.sr_Latin || this.Id == LocaleDatabase.LocaleId.sr || this.Id == LocaleDatabase.LocaleId.hu)
		{
			return this.ChangeEndingToDative(noun);
		}
		if (this.Id == LocaleDatabase.LocaleId.pl)
		{
			string[] nouns = noun.Split(' ', StringSplitOptions.None);
			for (int i = 0; i < nouns.Length; i++)
			{
				nouns[i] = this.ChangeEndingToDative(nouns[i]);
			}
			return string.Join(" ", nouns);
		}
		return noun;
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0001BA44 File Offset: 0x00019C44
	public string LocativeFormat(string noun)
	{
		if (this.Id == LocaleDatabase.LocaleId.hr || this.Id == LocaleDatabase.LocaleId.sr_Latin || this.Id == LocaleDatabase.LocaleId.sr)
		{
			return this.ChangeEndingToLocative(noun);
		}
		if (this.Id == LocaleDatabase.LocaleId.pl)
		{
			string[] nouns = noun.Split(' ', StringSplitOptions.None);
			for (int i = 0; i < nouns.Length; i++)
			{
				nouns[i] = this.ChangeEndingToLocative(nouns[i]);
			}
			return string.Join(" ", nouns);
		}
		return noun;
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0001BAB2 File Offset: 0x00019CB2
	public string IllativeFormat(string noun)
	{
		if (this.Id == LocaleDatabase.LocaleId.fi)
		{
			return this.ChangeEndingToIllative(noun);
		}
		return noun;
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x0001BAC8 File Offset: 0x00019CC8
	private string ChangeEndingToDative(string noun)
	{
		string dativestring = noun;
		if (this.Id == LocaleDatabase.LocaleId.hr || this.Id == LocaleDatabase.LocaleId.sr_Latin)
		{
			if (noun.Length < 1)
			{
				return noun;
			}
			char lastChar = noun[noun.Length - 1];
			char replacementChar = (lastChar == 'o') ? 'u' : ((lastChar == 'a') ? 'i' : ' ');
			if (replacementChar != ' ')
			{
				dativestring = noun.Remove(noun.Length - 1, 1) + replacementChar.ToString();
			}
			else
			{
				dativestring += "u";
			}
		}
		else if (this.Id == LocaleDatabase.LocaleId.sr)
		{
			if (noun.Length < 1)
			{
				return noun;
			}
			char lastChar2 = noun[noun.Length - 1];
			char replacementChar2 = (lastChar2 == 'o') ? 'y' : ((lastChar2 == 'a' || lastChar2 == 'а') ? 'и' : ' ');
			if (replacementChar2 != ' ')
			{
				dativestring = noun.Remove(noun.Length - 1, 1) + replacementChar2.ToString();
			}
			else
			{
				dativestring += "y";
			}
		}
		else if (this.Id == LocaleDatabase.LocaleId.hu)
		{
			if (noun.Length < 1)
			{
				return noun;
			}
			char lastChar3 = noun[noun.Length - 1];
			char replacementChar3 = ' ';
			if (lastChar3 == 'a')
			{
				replacementChar3 = 'á';
			}
			else if (lastChar3 == 'e')
			{
				replacementChar3 = 'é';
			}
			else if (lastChar3 == 'i')
			{
				replacementChar3 = 'í';
			}
			else if (lastChar3 == 'o')
			{
				replacementChar3 = 'ó';
			}
			else if (lastChar3 == 'ö')
			{
				replacementChar3 = 'ő';
			}
			else if (lastChar3 == 'u')
			{
				replacementChar3 = 'ú';
			}
			else if (lastChar3 == 'ü')
			{
				replacementChar3 = 'ű';
			}
			if (replacementChar3 != ' ')
			{
				dativestring = dativestring.Remove(dativestring.Length - 1, 1) + replacementChar3.ToString();
			}
			char firstVowel = ' ';
			string vowels = "aáeéiíoöóőuüúű";
			string dativestringLower = dativestring.ToLower();
			for (int i = 0; i < dativestring.Length; i++)
			{
				if (vowels.IndexOf(dativestringLower[i]) != -1)
				{
					firstVowel = dativestringLower[i];
					break;
				}
			}
			if ("eéuüúű".IndexOf(firstVowel) != -1)
			{
				dativestring += "ben";
			}
			else
			{
				dativestring += "ban";
			}
		}
		else if (this.Id == LocaleDatabase.LocaleId.pl)
		{
			if (noun.Length < 1)
			{
				return noun;
			}
			char lastChar4 = noun[noun.Length - 1];
			if (lastChar4 == 'n' || lastChar4 == 'm')
			{
				dativestring += "ie";
			}
			else if (lastChar4 == 'r')
			{
				dativestring += "ze";
			}
			else if (lastChar4 == 'l' || lastChar4 == 'k' || lastChar4 == 'ż' || lastChar4 == 'g')
			{
				dativestring += "u";
			}
			else if (lastChar4 == 'y')
			{
				dativestring += "m";
			}
			else
			{
				if (noun.Length < 2)
				{
					return noun;
				}
				string ending = noun.Substring(noun.Length - 2);
				if (ending == "ka")
				{
					dativestring = noun.Substring(0, noun.Length - 2) + "ce";
				}
				else if (ending == "na")
				{
					dativestring = noun.Substring(0, noun.Length - 2) + "nej";
				}
			}
		}
		return dativestring;
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x0001BE0C File Offset: 0x0001A00C
	private string ChangeEndingToLocative(string noun)
	{
		string locativestring = noun;
		if (this.Id == LocaleDatabase.LocaleId.hr || this.Id == LocaleDatabase.LocaleId.sr_Latin)
		{
			if (noun.Length < 2)
			{
				return noun;
			}
			if (noun.Substring(noun.Length - 2) == "in")
			{
				locativestring = noun.Substring(0, noun.Length - 2) + "nom";
			}
		}
		else if (this.Id == LocaleDatabase.LocaleId.sr)
		{
			if (noun.Length < 2)
			{
				return noun;
			}
			if (noun.Substring(noun.Length - 2) == "ин")
			{
				locativestring = noun.Substring(0, noun.Length - 2) + "ном";
			}
		}
		else if (this.Id == LocaleDatabase.LocaleId.pl)
		{
			return this.ChangeEndingToDative(noun);
		}
		return locativestring;
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x0001BED0 File Offset: 0x0001A0D0
	private string ChangeEndingToIllative(string noun)
	{
		if (this.Id != LocaleDatabase.LocaleId.fi)
		{
			return noun;
		}
		if (noun == null || noun.Length < 2)
		{
			return noun;
		}
		string illativestring = noun;
		string ending = noun.Substring(noun.Length - 1);
		if (ending == "a" || ending == "ä" || ending == "e" || ending == "i" || ending == "o" || ending == "ö" || ending == "u" || ending == "y")
		{
			illativestring = illativestring + ending + "n";
		}
		else
		{
			ending = noun.Substring(noun.Length - 2);
			if (ending == "er")
			{
				illativestring += "iin";
			}
		}
		return illativestring;
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x0001BFAB File Offset: 0x0001A1AB
	public string FormatNumber(int number)
	{
		return this.FormatNumber((long)number);
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x0001BFB8 File Offset: 0x0001A1B8
	public string FormatNumber(long number)
	{
		string formattedNumber;
		switch (this.DigitGrouping)
		{
		case DigitGrouping.SpaceThousands:
			formattedNumber = string.Format("{0:#,###0}", number).Replace(',', ' ');
			break;
		case DigitGrouping.PeriodThousands:
			formattedNumber = string.Format("{0:#,###0}", number).Replace(',', '.');
			break;
		case DigitGrouping.CommaTenThousands:
		{
			string unformattedstring = number.ToString();
			StringBuilder formattedstring = new StringBuilder();
			while (unformattedstring.Length > 4)
			{
				formattedstring.Insert(0, unformattedstring.Substring(unformattedstring.Length - 4));
				formattedstring.Insert(0, ',');
				unformattedstring = unformattedstring.Substring(0, unformattedstring.Length - 4);
			}
			formattedstring.Insert(0, unformattedstring);
			formattedNumber = formattedstring.ToString();
			break;
		}
		case DigitGrouping.CommaThousandsHundreds:
		{
			string unformattedstring2 = number.ToString();
			StringBuilder formattedstring2 = new StringBuilder();
			if (unformattedstring2.Length > 3)
			{
				formattedstring2.Insert(0, unformattedstring2.Substring(unformattedstring2.Length - 3));
				formattedstring2.Insert(0, ',');
				unformattedstring2 = unformattedstring2.Substring(0, unformattedstring2.Length - 3);
			}
			while (unformattedstring2.Length > 2)
			{
				formattedstring2.Insert(0, unformattedstring2.Substring(unformattedstring2.Length - 2));
				formattedstring2.Insert(0, ',');
				unformattedstring2 = unformattedstring2.Substring(0, unformattedstring2.Length - 2);
			}
			formattedstring2.Insert(0, unformattedstring2);
			formattedNumber = formattedstring2.ToString();
			break;
		}
		default:
			formattedNumber = string.Format("{0:#,###0}", number);
			break;
		}
		return formattedNumber;
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x0001C148 File Offset: 0x0001A348
	public StringKey FormatMinutes(int numMinutes)
	{
		if (numMinutes < 120)
		{
			string minutes = this.FormatNumber(numMinutes);
			StringKey stringKey = this._scope.Get<StringKey>();
			stringKey.InitWithString("Minutes", numMinutes, new Dictionary<string, string>
			{
				{
					"Num",
					minutes
				}
			});
			return stringKey;
		}
		int numHours = numMinutes / 60;
		string hours = this.FormatNumber(numHours);
		StringKey stringKey2 = this._scope.Get<StringKey>();
		stringKey2.InitWithString("Hours", numHours, new Dictionary<string, string>
		{
			{
				"Num",
				hours
			}
		});
		return stringKey2;
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x0001C1C0 File Offset: 0x0001A3C0
	public string FormatDate(DateTime date, bool formatForLocString = true)
	{
		return this.FormatDateTime(date, "d", formatForLocString);
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x0001C1CF File Offset: 0x0001A3CF
	public string FormatDateTime(DateTime dateTime, bool formatForLocString = true)
	{
		return this.FormatDateTime(dateTime, "g", formatForLocString);
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x0001C1E0 File Offset: 0x0001A3E0
	public Locale.DaysOfTheWeek GetDayLabel(int index)
	{
		int dayOffset = (this.StartOfWeek == StartOfWeek.Monday) ? 0 : 6;
		return Locale.DaysOfTheWeek.Monday + (index + dayOffset) % 7;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x0001C202 File Offset: 0x0001A402
	public bool HasString(StringKey key)
	{
		return this._stringTable.ContainsKey(key.GetStringId());
	}

	// Token: 0x060008BA RID: 2234 RVA: 0x0001C218 File Offset: 0x0001A418
	public string GetNextToken(string text, ref int nextCharIndex)
	{
		string nextToken = "";
		if (this.LineBreakRule == LineBreakRule.EastAsian)
		{
			if (nextCharIndex < text.Length)
			{
				nextToken += text[nextCharIndex].ToString();
				nextCharIndex++;
			}
			while (nextCharIndex < text.Length)
			{
				char lastChar = nextToken[nextToken.Length - 1];
				char nextChar = text[nextCharIndex];
				if ((this._cannotEndLines == null || this._cannotEndLines.IndexOf(lastChar) == -1) & (this._cannotStartLines == null || this._cannotStartLines.IndexOf(nextChar) == -1) & (this._cannotSplit == null || (this._cannotSplit.IndexOf(lastChar) == -1 && this._cannotSplit.IndexOf(nextChar) == -1)) & (!char.IsNumber(lastChar) || !char.IsNumber(nextChar)) & (!Locale.IsLatin(lastChar) || !Locale.IsLatin(nextChar)))
				{
					return nextToken;
				}
				nextToken += nextChar.ToString();
				nextCharIndex++;
			}
		}
		else
		{
			while (nextCharIndex < text.Length && text[nextCharIndex] != ' ' && text[nextCharIndex] != '\n')
			{
				nextToken += text[nextCharIndex].ToString();
				nextCharIndex++;
				if (text[nextCharIndex - 1] == '-')
				{
					break;
				}
			}
		}
		return nextToken;
	}

	// Token: 0x060008BB RID: 2235 RVA: 0x0001C380 File Offset: 0x0001A580
	private static bool IsLatin(char character)
	{
		return (character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z');
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0001C3A8 File Offset: 0x0001A5A8
	public static Locale FromJSON(JSON.Dictionary jsonDictionary, LocaleDatabase creatingDatabase, IScope newScope)
	{
		string localeId = jsonDictionary.GetString("id");
		if (localeId == null)
		{
			return null;
		}
		Locale newLocale = new Locale(localeId, creatingDatabase, newScope);
		if (jsonDictionary.ContainsKey("isComplete"))
		{
			newLocale.IsComplete = jsonDictionary.GetBool("isComplete", false);
		}
		else
		{
			newLocale.IsComplete = true;
		}
		string textDirection = jsonDictionary.GetString("textDirection");
		newLocale.TextDirection = ((textDirection != null && textDirection == "rtl") ? TextDirection.RightToLeft : TextDirection.LeftToRight);
		string digitGrouping = jsonDictionary.GetString("digitGrouping");
		newLocale.DigitGrouping = ((digitGrouping == null) ? DigitGrouping.CommaThousands : ((DigitGrouping)Enum.Parse(typeof(DigitGrouping), digitGrouping)));
		string pluralForm = jsonDictionary.GetString("pluralForm");
		newLocale._pluralForm = ((pluralForm == null) ? PluralForm.Latin : ((PluralForm)Enum.Parse(typeof(PluralForm), pluralForm)));
		string startOfWeek = jsonDictionary.GetString("startOfWeek");
		newLocale.StartOfWeek = ((startOfWeek == null) ? StartOfWeek.Sunday : ((StartOfWeek)Enum.Parse(typeof(StartOfWeek), startOfWeek)));
		newLocale.CapitaliseNouns = (jsonDictionary.ContainsKey("capitaliseNouns") && jsonDictionary.GetBool("capitaliseNouns", false));
		if (jsonDictionary.ContainsKey("charset"))
		{
			newLocale.Charset = jsonDictionary.GetString("charset");
		}
		else
		{
			newLocale.Charset = "latin";
		}
		string name = jsonDictionary.GetString("name");
		if (name != null)
		{
			newLocale.Name = name;
		}
		JSON.Dictionary jsonLineBreakRules = jsonDictionary.GetDictionary("lineBreakRules");
		if (jsonLineBreakRules != null)
		{
			newLocale._cannotStartLines = jsonLineBreakRules.GetString("cannotStartLines");
			newLocale._cannotEndLines = jsonLineBreakRules.GetString("cannotEndLines");
			newLocale._cannotSplit = jsonLineBreakRules.GetString("cannotSplit");
		}
		JSON.Dictionary jsonstringTable = jsonDictionary.GetDictionary("stringTable");
		if (jsonstringTable == null)
		{
			return null;
		}
		foreach (string id in jsonstringTable.Keys)
		{
			object entry = jsonstringTable[id];
			if (entry != null)
			{
				List<string> forms = new List<string>();
				if (entry is string)
				{
					forms.Add(entry as string);
				}
				else if (entry is JSON.Array)
				{
					JSON.Array jsonForms = entry as JSON.Array;
					if (jsonForms != null)
					{
						for (int i = 0; i < jsonForms.Count; i++)
						{
							string form = jsonForms.GetString(i);
							if (form != null)
							{
								forms.Add(form);
							}
						}
					}
				}
				if (forms.Count != 0)
				{
					newLocale._stringTable[id] = forms;
				}
			}
		}
		return newLocale;
	}

	// Token: 0x060008BD RID: 2237 RVA: 0x0001C640 File Offset: 0x0001A840
	private Locale(string newId, LocaleDatabase newDatabase, IScope newScope)
	{
		LocaleDatabase.LocaleId temp = LocaleDatabase.LocaleId.Unknown;
		Enum.TryParse<LocaleDatabase.LocaleId>(newId, out temp);
		this.Id = temp;
		this._stringTable = new Dictionary<string, List<string>>();
		this._database = newDatabase;
		this._scope = newScope;
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x0001C680 File Offset: 0x0001A880
	private int GetPluralForm(int n)
	{
		switch (this._pluralForm)
		{
		case PluralForm.Asian:
			return 0;
		case PluralForm.French:
			return (n > 1) ? 1 : 0;
		case PluralForm.Czech:
			return (n == 1) ? 0 : ((n >= 2 && n <= 4) ? 1 : 3);
		case PluralForm.Polish:
			return (n == 1) ? 0 : ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 12 || n % 100 > 14)) ? 1 : ((n % 10 == 0 || n % 10 == 1 || (n % 10 >= 5 && n % 10 <= 9) || (n % 100 >= 12 && n % 100 <= 14)) ? 2 : 3));
		case PluralForm.Serbian:
			return (n % 10 == 1 && n % 100 != 11) ? 0 : ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 10 || n % 100 >= 20)) ? 1 : 2);
		case PluralForm.Romanian:
			return (n == 1) ? 0 : ((n % 100 > 19 || (n % 100 == 0 && n != 0)) ? 2 : 1);
		case PluralForm.Ukrainian:
			return (n % 10 == 1 && n % 100 != 11) ? 0 : ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 12 || n % 100 > 14)) ? 1 : ((n % 10 == 0 || (n % 10 >= 5 && n % 10 <= 9) || (n % 100 >= 11 && n % 100 <= 14)) ? 2 : 3));
		case PluralForm.Welsh:
			return (n == 0) ? 0 : ((n == 1) ? 1 : ((n == 2) ? 2 : ((n == 3) ? 3 : ((n == 6) ? 4 : 5))));
		case PluralForm.Russian:
			return (n % 10 == 1 && n % 100 != 11) ? 0 : ((n % 10 >= 2 && n % 10 <= 4 && (n % 100 < 12 || n % 100 > 14)) ? 1 : ((n % 10 == 0 || (n % 10 >= 5 && n % 10 <= 9) || (n % 100 >= 11 && n % 100 <= 14)) ? 2 : 3));
		case PluralForm.Slovenian:
			return (n % 100 == 1) ? 1 : ((n % 100 == 2) ? 2 : ((n % 100 == 3 || n % 100 == 4) ? 3 : 0));
		case PluralForm.Gaelic:
			return (n == 1) ? 0 : ((n == 2) ? 1 : ((n < 7) ? 2 : ((n < 11) ? 3 : 4)));
		case PluralForm.Arabic:
			return (n == 0) ? 0 : ((n == 1) ? 1 : ((n == 2) ? 2 : ((n % 100 >= 3 && n % 100 <= 10) ? 3 : ((n % 100 >= 11 && n % 100 <= 99) ? 4 : 5))));
		}
		return (n != 1) ? 1 : 0;
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0001C93C File Offset: 0x0001AB3C
	private string FormatDateTime(DateTime timestamp, string formatCode, bool formatRtlForLocString)
	{
		string systemLocaleId = this.Id.ToString().Replace('_', '-');
		bool isArabic = false;
		if (systemLocaleId == "ar")
		{
			isArabic = true;
			systemLocaleId = "ar-EG";
		}
		CultureInfo culture = null;
		if (systemLocaleId == "en-US")
		{
			CultureInfo systemCulture = CultureInfo.CurrentCulture;
			if (systemCulture.TwoLetterISOLanguageName == "en")
			{
				culture = systemCulture;
			}
		}
		if (culture == null)
		{
			culture = new CultureInfo(systemLocaleId);
		}
		string formattedTimestamp = timestamp.ToString(formatCode, culture);
		if (isArabic)
		{
			if (formatRtlForLocString)
			{
				if (formattedTimestamp.Contains("م"))
				{
					formattedTimestamp = formattedTimestamp.Replace("م", "").Trim();
					formattedTimestamp = "ﻡ " + formattedTimestamp;
				}
				if (formattedTimestamp.Contains("ص"))
				{
					formattedTimestamp = formattedTimestamp.Replace("ص", "").Trim();
					formattedTimestamp = "ﺹ " + formattedTimestamp;
				}
			}
			else
			{
				formattedTimestamp = formattedTimestamp.Replace("م", "ﻡ");
				formattedTimestamp = formattedTimestamp.Replace("ص", "ﺹ");
			}
		}
		return formattedTimestamp;
	}

	// Token: 0x0400041E RID: 1054
	private static readonly Dictionary<string, ControllerButton> ActionsToButtons = new Dictionary<string, ControllerButton>
	{
		{
			"Build",
			ControllerButton.FaceButtonBottom
		},
		{
			"ToggleDeleteMode",
			ControllerButton.FaceButtonTop
		},
		{
			"Delete",
			ControllerButton.FaceButtonRight
		},
		{
			"IncreaseGameSpeed",
			ControllerButton.ButtonRight
		},
		{
			"DecreaseGameSpeed",
			ControllerButton.ButtonLeft
		}
	};

	// Token: 0x0400041F RID: 1055
	private IScope _scope;

	// Token: 0x04000420 RID: 1056
	private Dictionary<string, List<string>> _stringTable;

	// Token: 0x04000421 RID: 1057
	private PluralForm _pluralForm;

	// Token: 0x04000422 RID: 1058
	private string _cannotStartLines;

	// Token: 0x04000423 RID: 1059
	private string _cannotEndLines;

	// Token: 0x04000424 RID: 1060
	private string _cannotSplit;

	// Token: 0x04000425 RID: 1061
	private LocaleDatabase _database;

	// Token: 0x02000185 RID: 389
	public enum DaysOfTheWeek
	{
		// Token: 0x0400042F RID: 1071
		Monday,
		// Token: 0x04000430 RID: 1072
		Tuesday,
		// Token: 0x04000431 RID: 1073
		Wednesday,
		// Token: 0x04000432 RID: 1074
		Thursday,
		// Token: 0x04000433 RID: 1075
		Friday,
		// Token: 0x04000434 RID: 1076
		Saturday,
		// Token: 0x04000435 RID: 1077
		Sunday
	}
}
