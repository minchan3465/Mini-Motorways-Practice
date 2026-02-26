using System;
using System.Collections.Generic;

// Token: 0x0200017C RID: 380
public class ArabicHelper
{
	// Token: 0x06000889 RID: 2185 RVA: 0x0001A87C File Offset: 0x00018A7C
	private ArabicHelper()
	{
		this.BuildCharacterMaps();
	}

	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x0600088A RID: 2186 RVA: 0x0001A8AB File Offset: 0x00018AAB
	public static ArabicHelper Instance
	{
		get
		{
			if (ArabicHelper._instance == null)
			{
				ArabicHelper._instance = new ArabicHelper();
			}
			return ArabicHelper._instance;
		}
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0001A8C4 File Offset: 0x00018AC4
	public string ConvertArabic(string normal)
	{
		int length = normal.Length;
		string shaped = "";
		for (int index = 0; index < length; index++)
		{
			char current = normal[index];
			if (this.CharacterMapContains(current))
			{
				char prevCode = '\0';
				char nextCode = '\0';
				int prevIndex = index - 1;
				int nextIndex = index + 1;
				while (prevIndex >= 0 && this.IsTransparent(normal[prevIndex]))
				{
					prevIndex--;
				}
				if (prevIndex >= 0)
				{
					prevCode = normal[prevIndex];
					if (this.CharacterMapContains(prevCode))
					{
						ArabicHelper.CharRep prep = this.GetCharRep(prevCode);
						if (prep.initial == '\0' || prep.medial == '\0')
						{
							prevCode = '\0';
						}
					}
					else
					{
						prevCode = '\0';
					}
				}
				while (nextIndex < length && this.IsTransparent(normal[nextIndex]))
				{
					nextIndex++;
				}
				if (nextIndex < length)
				{
					nextCode = normal[nextIndex];
					if (this.CharacterMapContains(nextCode))
					{
						ArabicHelper.CharRep nrep = this.GetCharRep(nextCode);
						if (nrep.medial == '\0' && nrep.final == '\0' && nextCode != 'ـ')
						{
							nextCode = '\0';
						}
					}
					else
					{
						nextCode = '\0';
					}
				}
				if (current == 'ل' && nextCode != '\0' && (nextCode == 'آ' || nextCode == 'أ' || nextCode == 'إ' || nextCode == 'ا'))
				{
					ArabicHelper.CharRep combcrep = this.GetCombCharRep(current, nextCode);
					if (prevCode != '\0')
					{
						shaped += combcrep.final.ToString();
					}
					else
					{
						shaped += combcrep.isolated.ToString();
					}
					index += 2;
					continue;
				}
				ArabicHelper.CharRep crep = this.GetCharRep(current);
				if (prevCode != '\0' && nextCode != '\0' && crep.medial != '\0')
				{
					shaped += crep.medial.ToString();
					index++;
					continue;
				}
				if (prevCode != '\0' && crep.final != '\0')
				{
					shaped += crep.final.ToString();
					index++;
					continue;
				}
				if (nextCode != '\0' && crep.initial != '\0')
				{
					shaped += crep.initial.ToString();
					index++;
					continue;
				}
				shaped += crep.isolated.ToString();
			}
			else
			{
				shaped += current.ToString();
			}
		}
		return shaped;
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x0001AAE4 File Offset: 0x00018CE4
	private void BuildCharacterMaps()
	{
		this._charMap.Add(new ArabicHelper.CharRep('ء', 'ﺀ', '\0', '\0', '\0'));
		this._charMap.Add(new ArabicHelper.CharRep('آ', 'ﺁ', '\0', '\0', 'ﺂ'));
		this._charMap.Add(new ArabicHelper.CharRep('أ', 'ﺃ', '\0', '\0', 'ﺄ'));
		this._charMap.Add(new ArabicHelper.CharRep('ؤ', 'ﺅ', '\0', '\0', 'ﺆ'));
		this._charMap.Add(new ArabicHelper.CharRep('إ', 'ﺇ', '\0', '\0', 'ﺈ'));
		this._charMap.Add(new ArabicHelper.CharRep('ئ', 'ﺉ', 'ﺋ', 'ﺌ', 'ﺊ'));
		this._charMap.Add(new ArabicHelper.CharRep('ا', 'ﺍ', '\0', '\0', 'ﺎ'));
		this._charMap.Add(new ArabicHelper.CharRep('ب', 'ﺏ', 'ﺑ', 'ﺒ', 'ﺐ'));
		this._charMap.Add(new ArabicHelper.CharRep('ة', 'ﺓ', '\0', '\0', 'ﺔ'));
		this._charMap.Add(new ArabicHelper.CharRep('ت', 'ﺕ', 'ﺗ', 'ﺘ', 'ﺖ'));
		this._charMap.Add(new ArabicHelper.CharRep('ث', 'ﺙ', 'ﺛ', 'ﺜ', 'ﺚ'));
		this._charMap.Add(new ArabicHelper.CharRep('ج', 'ﺝ', 'ﺟ', 'ﺠ', 'ﺞ'));
		this._charMap.Add(new ArabicHelper.CharRep('ح', 'ﺡ', 'ﺣ', 'ﺤ', 'ﺢ'));
		this._charMap.Add(new ArabicHelper.CharRep('خ', 'ﺥ', 'ﺧ', 'ﺨ', 'ﺦ'));
		this._charMap.Add(new ArabicHelper.CharRep('د', 'ﺩ', '\0', '\0', 'ﺪ'));
		this._charMap.Add(new ArabicHelper.CharRep('ذ', 'ﺫ', '\0', '\0', 'ﺬ'));
		this._charMap.Add(new ArabicHelper.CharRep('ر', 'ﺭ', '\0', '\0', 'ﺮ'));
		this._charMap.Add(new ArabicHelper.CharRep('ز', 'ﺯ', '\0', '\0', 'ﺰ'));
		this._charMap.Add(new ArabicHelper.CharRep('س', 'ﺱ', 'ﺳ', 'ﺴ', 'ﺲ'));
		this._charMap.Add(new ArabicHelper.CharRep('ش', 'ﺵ', 'ﺷ', 'ﺸ', 'ﺶ'));
		this._charMap.Add(new ArabicHelper.CharRep('ص', 'ﺹ', 'ﺻ', 'ﺼ', 'ﺺ'));
		this._charMap.Add(new ArabicHelper.CharRep('ض', 'ﺽ', 'ﺿ', 'ﻀ', 'ﺾ'));
		this._charMap.Add(new ArabicHelper.CharRep('ط', 'ﻁ', 'ﻃ', 'ﻄ', 'ﻂ'));
		this._charMap.Add(new ArabicHelper.CharRep('ظ', 'ﻅ', 'ﻇ', 'ﻈ', 'ﻆ'));
		this._charMap.Add(new ArabicHelper.CharRep('ع', 'ﻉ', 'ﻋ', 'ﻌ', 'ﻊ'));
		this._charMap.Add(new ArabicHelper.CharRep('غ', 'ﻍ', 'ﻏ', 'ﻐ', 'ﻎ'));
		this._charMap.Add(new ArabicHelper.CharRep('ـ', 'ـ', '\0', '\0', '\0'));
		this._charMap.Add(new ArabicHelper.CharRep('ف', 'ﻑ', 'ﻓ', 'ﻔ', 'ﻒ'));
		this._charMap.Add(new ArabicHelper.CharRep('ق', 'ﻕ', 'ﻗ', 'ﻘ', 'ﻖ'));
		this._charMap.Add(new ArabicHelper.CharRep('ك', 'ﻙ', 'ﻛ', 'ﻜ', 'ﻚ'));
		this._charMap.Add(new ArabicHelper.CharRep('ل', 'ﻝ', 'ﻟ', 'ﻠ', 'ﻞ'));
		this._charMap.Add(new ArabicHelper.CharRep('م', 'ﻡ', 'ﻣ', 'ﻤ', 'ﻢ'));
		this._charMap.Add(new ArabicHelper.CharRep('ن', 'ﻥ', 'ﻧ', 'ﻨ', 'ﻦ'));
		this._charMap.Add(new ArabicHelper.CharRep('ه', 'ﻩ', 'ﻫ', 'ﻬ', 'ﻪ'));
		this._charMap.Add(new ArabicHelper.CharRep('و', 'ﻭ', '\0', '\0', 'ﻮ'));
		this._charMap.Add(new ArabicHelper.CharRep('ى', 'ﻯ', '\0', '\0', 'ﻰ'));
		this._charMap.Add(new ArabicHelper.CharRep('ي', 'ﻱ', 'ﻳ', 'ﻴ', 'ﻲ'));
		this._nilCharRep = new ArabicHelper.CharRep('\0', '\0', '\0', '\0', '\0');
		this._combCharsMap.Add(new ArabicHelper.CharRep('ل', 'آ', 'ﻵ', '\0', '\0', 'ﻶ'));
		this._combCharsMap.Add(new ArabicHelper.CharRep('ل', 'أ', 'ﻷ', '\0', '\0', 'ﻸ'));
		this._combCharsMap.Add(new ArabicHelper.CharRep('ل', 'إ', 'ﻹ', '\0', '\0', 'ﻺ'));
		this._combCharsMap.Add(new ArabicHelper.CharRep('ل', 'ا', 'ﻻ', '\0', '\0', 'ﻼ'));
		this._nilCombCharRep = new ArabicHelper.CharRep('\0', '\0', '\0', '\0', '\0', '\0');
		this._transparentChars.Add('ؐ');
		this._transparentChars.Add('ؒ');
		this._transparentChars.Add('ؓ');
		this._transparentChars.Add('ؔ');
		this._transparentChars.Add('ؕ');
		this._transparentChars.Add('ً');
		this._transparentChars.Add('ٌ');
		this._transparentChars.Add('ٍ');
		this._transparentChars.Add('َ');
		this._transparentChars.Add('ُ');
		this._transparentChars.Add('ِ');
		this._transparentChars.Add('ّ');
		this._transparentChars.Add('ْ');
		this._transparentChars.Add('ٓ');
		this._transparentChars.Add('ٔ');
		this._transparentChars.Add('ٕ');
		this._transparentChars.Add('ٖ');
		this._transparentChars.Add('ٗ');
		this._transparentChars.Add('٘');
		this._transparentChars.Add('ٰ');
		this._transparentChars.Add('ۖ');
		this._transparentChars.Add('ۗ');
		this._transparentChars.Add('ۘ');
		this._transparentChars.Add('ۙ');
		this._transparentChars.Add('ۚ');
		this._transparentChars.Add('ۛ');
		this._transparentChars.Add('ۜ');
		this._transparentChars.Add('۟');
		this._transparentChars.Add('۠');
		this._transparentChars.Add('ۡ');
		this._transparentChars.Add('ۢ');
		this._transparentChars.Add('ۣ');
		this._transparentChars.Add('ۤ');
		this._transparentChars.Add('ۧ');
		this._transparentChars.Add('ۨ');
		this._transparentChars.Add('۪');
		this._transparentChars.Add('۫');
		this._transparentChars.Add('۬');
		this._transparentChars.Add('ۭ');
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x0001B390 File Offset: 0x00019590
	private bool CharacterMapContains(char c)
	{
		for (int charIndex = 0; charIndex < this._charMap.Count; charIndex++)
		{
			if (this._charMap[charIndex].code == c)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0001B3CC File Offset: 0x000195CC
	private ArabicHelper.CharRep GetCharRep(char c)
	{
		for (int charIndex = 0; charIndex < this._charMap.Count; charIndex++)
		{
			if (this._charMap[charIndex].code == c)
			{
				return this._charMap[charIndex];
			}
		}
		return this._nilCharRep;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0001B418 File Offset: 0x00019618
	private ArabicHelper.CharRep GetCombCharRep(char c1, char c2)
	{
		for (int charIndex = 0; charIndex < this._combCharsMap.Count; charIndex++)
		{
			if (this._combCharsMap[charIndex].code == c1 && this._combCharsMap[charIndex].code2 == c2)
			{
				return this._combCharsMap[charIndex];
			}
		}
		return this._nilCombCharRep;
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x0001B478 File Offset: 0x00019678
	private bool IsTransparent(char c)
	{
		for (int charIndex = 0; charIndex < this._transparentChars.Count; charIndex++)
		{
			if (this._transparentChars[charIndex] == c)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040003F4 RID: 1012
	private const char NullChar = '\0';

	// Token: 0x040003F5 RID: 1013
	private static ArabicHelper _instance;

	// Token: 0x040003F6 RID: 1014
	private List<ArabicHelper.CharRep> _charMap = new List<ArabicHelper.CharRep>();

	// Token: 0x040003F7 RID: 1015
	private ArabicHelper.CharRep _nilCharRep;

	// Token: 0x040003F8 RID: 1016
	private List<ArabicHelper.CharRep> _combCharsMap = new List<ArabicHelper.CharRep>();

	// Token: 0x040003F9 RID: 1017
	private ArabicHelper.CharRep _nilCombCharRep;

	// Token: 0x040003FA RID: 1018
	private List<char> _transparentChars = new List<char>();

	// Token: 0x0200017D RID: 381
	private struct CharRep
	{
		// Token: 0x06000891 RID: 2193 RVA: 0x0001B4AD File Offset: 0x000196AD
		public CharRep(char newCode, char newIsolated, char newInitial, char newMedial, char newFinal)
		{
			this.code = newCode;
			this.code2 = '\0';
			this.isolated = newIsolated;
			this.initial = newInitial;
			this.medial = newMedial;
			this.final = newFinal;
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x0001B4DB File Offset: 0x000196DB
		public CharRep(char newCode, char newCode2, char newIsolated, char newInitial, char newMedial, char newFinal)
		{
			this.code = newCode;
			this.code2 = newCode2;
			this.isolated = newIsolated;
			this.initial = newInitial;
			this.medial = newMedial;
			this.final = newFinal;
		}

		// Token: 0x040003FB RID: 1019
		public char code;

		// Token: 0x040003FC RID: 1020
		public char code2;

		// Token: 0x040003FD RID: 1021
		public char isolated;

		// Token: 0x040003FE RID: 1022
		public char initial;

		// Token: 0x040003FF RID: 1023
		public char medial;

		// Token: 0x04000400 RID: 1024
		public char final;
	}
}
