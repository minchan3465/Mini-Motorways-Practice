using System;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class FontDatabase : MonoBehaviour
{
	// Token: 0x0600076A RID: 1898 RVA: 0x00018744 File Offset: 0x00016944
	public FontDefinition GetFont(string charset)
	{
		foreach (FontDefinition font in this._fonts)
		{
			if (font.Charset == charset)
			{
				return font;
			}
		}
		Diagnostics.FailAssert("Unable to find font for charset '{0}'.", new object[]
		{
			charset
		});
		return null;
	}

	// Token: 0x04000360 RID: 864
	[SerializeField]
	private FontDefinition[] _fonts;
}
