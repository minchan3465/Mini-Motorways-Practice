using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;

// Token: 0x0200014D RID: 333
public class AppleSfSymbolService : IControllerButtonToSymbolService
{
	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06000755 RID: 1877 RVA: 0x000020AA File Offset: 0x000002AA
	public bool HasMappings
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00018224 File Offset: 0x00016424
	private void Initialize()
	{
		AppleSfSymbolService.NCSetSymbolStyle(64f, 3, false, false, 1f, 1f, 1f);
		this._symbolNameToGlyphTexture = new Dictionary<string, Texture2D>();
		this._customSpriteAssets = new TMP_SpriteAsset[10];
		string spriteAssetPath = TMP_Settings.defaultSpriteAssetPath;
		for (int spriteIndex = 0; spriteIndex < 10; spriteIndex++)
		{
			this._customSpriteAssets[spriteIndex] = Resources.Load<TMP_SpriteAsset>(spriteAssetPath + "appleSfSymbol" + spriteIndex.ToString());
		}
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x00018298 File Offset: 0x00016498
	public string GetTextMeshProSymbolTextForControllerButton(ControllerButton buttonType)
	{
		if (!this._hasInitialized)
		{
			this.Initialize();
			this._hasInitialized = true;
		}
		string symbolName = AppleSfSymbolService.GetGlyphSymbolName(buttonType);
		if (symbolName == null)
		{
			return this._defaultControllerButtonToSymbolService.GetTextMeshProSymbolTextForControllerButton(buttonType);
		}
		Texture2D glyphTexture;
		if (!this._symbolNameToGlyphTexture.ContainsKey(symbolName))
		{
			glyphTexture = AppleSfSymbolService.GetGlyph(symbolName);
			if (glyphTexture == null)
			{
				return this._defaultControllerButtonToSymbolService.GetTextMeshProSymbolTextForControllerButton(buttonType);
			}
			this._symbolNameToGlyphTexture.Add(symbolName, glyphTexture);
		}
		else
		{
			glyphTexture = this._symbolNameToGlyphTexture[symbolName];
		}
		TMP_SpriteAsset tmp_SpriteAsset = this._customSpriteAssets[this._currentSpriteIndex];
		tmp_SpriteAsset.material.mainTexture = glyphTexture;
		TMP_SpriteGlyph tmp_SpriteGlyph = tmp_SpriteAsset.spriteGlyphTable[0];
		GlyphRect glyphRect = tmp_SpriteGlyph.glyphRect;
		GlyphMetrics metrics = tmp_SpriteGlyph.metrics;
		metrics.width = (float)glyphRect.width * ((float)glyphTexture.width / (float)glyphTexture.height);
		metrics.height = (float)glyphRect.height;
		metrics.horizontalBearingX = 0f;
		metrics.horizontalAdvance = metrics.width;
		metrics.horizontalBearingY = 0.75f * metrics.height;
		this._customSpriteAssets[this._currentSpriteIndex].spriteGlyphTable[0].metrics = metrics;
		this._customSpriteAssets[this._currentSpriteIndex].spriteGlyphTable[0].scale = 1.25f;
		string result = "<sprite=\"appleSfSymbol" + this._currentSpriteIndex.ToString() + "\" name=\"glyph\" tint>";
		this._currentSpriteIndex++;
		if (this._currentSpriteIndex >= this._customSpriteAssets.Length)
		{
			this._currentSpriteIndex = 0;
		}
		return result;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00018424 File Offset: 0x00016624
	private static string GetGlyphSymbolName(ControllerButton buttonType)
	{
		string glyphSymbolName = AppleSfSymbolService.NCGetGlyphSymbolNameForInput((int)buttonType);
		if (!string.IsNullOrEmpty(glyphSymbolName))
		{
			return glyphSymbolName;
		}
		return null;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00018444 File Offset: 0x00016644
	public static Texture2D GetGlyph(string name)
	{
		long imageBufferLength = AppleSfSymbolService.NCGenerateGlyphForSymbolName(name);
		if (imageBufferLength <= 0L)
		{
			return null;
		}
		Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, true);
		byte[] imgBuffer = new byte[imageBufferLength];
		if (!AppleSfSymbolService.NCGetGeneratedGlyph(imgBuffer))
		{
			return null;
		}
		texture.LoadImage(imgBuffer, false);
		return texture;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x000022F5 File Offset: 0x000004F5
	private static void NCSetSymbolStyle(float pointSize, int weight, bool fill, bool forceSquare, float red, float green, float blue)
	{
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00004BD9 File Offset: 0x00002DD9
	public static string NCGetGlyphSymbolNameForInput(int buttonType)
	{
		return null;
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00018485 File Offset: 0x00016685
	public static long NCGenerateGlyphForSymbolName(string symbolName)
	{
		return -1L;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x0000222C File Offset: 0x0000042C
	private static bool NCGetGeneratedGlyph(byte[] imgBuffer)
	{
		return false;
	}

	// Token: 0x04000349 RID: 841
	private const int MaxSpriteCount = 10;

	// Token: 0x0400034A RID: 842
	private const string SpriteSymbolPrefix = "appleSfSymbol";

	// Token: 0x0400034B RID: 843
	private const int GlyphPointSize = 64;

	// Token: 0x0400034C RID: 844
	private const float GlyphSpriteScale = 1.25f;

	// Token: 0x0400034D RID: 845
	private Dictionary<string, Texture2D> _symbolNameToGlyphTexture;

	// Token: 0x0400034E RID: 846
	private TMP_SpriteAsset[] _customSpriteAssets;

	// Token: 0x0400034F RID: 847
	private bool _hasInitialized;

	// Token: 0x04000350 RID: 848
	private int _currentSpriteIndex;

	// Token: 0x04000351 RID: 849
	private readonly DefaultControllerButtonToSymbolService _defaultControllerButtonToSymbolService = new DefaultControllerButtonToSymbolService();

	// Token: 0x0200014E RID: 334
	private enum SymbolWeight
	{
		// Token: 0x04000353 RID: 851
		Ultralight,
		// Token: 0x04000354 RID: 852
		Thin,
		// Token: 0x04000355 RID: 853
		Light,
		// Token: 0x04000356 RID: 854
		Regular,
		// Token: 0x04000357 RID: 855
		Medium,
		// Token: 0x04000358 RID: 856
		Semibold,
		// Token: 0x04000359 RID: 857
		Bold,
		// Token: 0x0400035A RID: 858
		Heavy,
		// Token: 0x0400035B RID: 859
		Black
	}
}
