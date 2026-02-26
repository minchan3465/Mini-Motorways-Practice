using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000150 RID: 336
[Serializable]
public class FontDefinition
{
	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06000762 RID: 1890 RVA: 0x0001854C File Offset: 0x0001674C
	public string Charset
	{
		get
		{
			return this._charset;
		}
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06000763 RID: 1891 RVA: 0x00018554 File Offset: 0x00016754
	public TMP_FontAsset FontAsset
	{
		get
		{
			return this._font;
		}
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0001855C File Offset: 0x0001675C
	public Material GetCustomMaterial(FontStyles style, Material baseCustomMaterial)
	{
		long customMaterialHashCode = (long)baseCustomMaterial.GetHashCode() << 32 | (long)style;
		Material customMaterial;
		if (this._customMaterials.TryGetValue(customMaterialHashCode, out customMaterial))
		{
			return customMaterial;
		}
		Material defaultMaterial = this._font.material;
		if ((style & FontStyles.Bold) == FontStyles.Bold)
		{
			for (int weightIndex = this._font.fontWeightTable.Length - 1; weightIndex >= 0; weightIndex--)
			{
				TMP_FontAsset boldAsset = this._font.fontWeightTable[weightIndex].regularTypeface;
				if (boldAsset != null)
				{
					defaultMaterial = boldAsset.material;
					break;
				}
			}
		}
		Material styledMaterial = new Material(defaultMaterial);
		this.CopyKeyword(styledMaterial, baseCustomMaterial, ShaderUtilities.Keyword_Bevel);
		this.CopyKeyword(styledMaterial, baseCustomMaterial, ShaderUtilities.Keyword_Glow);
		this.CopyKeyword(styledMaterial, baseCustomMaterial, ShaderUtilities.Keyword_Underlay);
		this.CopyKeyword(styledMaterial, baseCustomMaterial, ShaderUtilities.Keyword_Ratios);
		this.CopyKeyword(styledMaterial, baseCustomMaterial, ShaderUtilities.Keyword_Outline);
		this.CopyColor(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_UnderlayColor);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_UnderlayOffsetX);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_UnderlayOffsetY);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_UnderlayDilate);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_UnderlaySoftness);
		this.CopyColor(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_GlowColor);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_GlowOffset);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_GlowPower);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_GlowOuter);
		this.CopyFloat(styledMaterial, baseCustomMaterial, FontDefinition.ID_GlowInner);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_ScaleRatio_A);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_ScaleRatio_B);
		this.CopyFloat(styledMaterial, baseCustomMaterial, ShaderUtilities.ID_ScaleRatio_C);
		this._customMaterials.Add(customMaterialHashCode, styledMaterial);
		return styledMaterial;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x000186E5 File Offset: 0x000168E5
	private void CopyKeyword(Material targetMaterial, Material sourceMaterial, string keyword)
	{
		if (sourceMaterial.IsKeywordEnabled(keyword))
		{
			targetMaterial.EnableKeyword(keyword);
			return;
		}
		targetMaterial.DisableKeyword(keyword);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x000186FF File Offset: 0x000168FF
	private void CopyColor(Material targetMaterial, Material sourceMaterial, int nameId)
	{
		targetMaterial.SetColor(nameId, sourceMaterial.GetColor(nameId));
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x0001870F File Offset: 0x0001690F
	private void CopyFloat(Material targetMaterial, Material sourceMaterial, int nameId)
	{
		targetMaterial.SetFloat(nameId, sourceMaterial.GetFloat(nameId));
	}

	// Token: 0x0400035C RID: 860
	[SerializeField]
	private string _charset;

	// Token: 0x0400035D RID: 861
	[SerializeField]
	private TMP_FontAsset _font;

	// Token: 0x0400035E RID: 862
	private Dictionary<long, Material> _customMaterials = new Dictionary<long, Material>();

	// Token: 0x0400035F RID: 863
	private static int ID_GlowInner = Shader.PropertyToID("_GlowInner");
}
