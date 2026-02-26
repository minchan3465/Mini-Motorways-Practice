using System;
using System.Collections.Generic;
using UnityEngine;

namespace Motorways.Themes
{
	// Token: 0x0200047E RID: 1150
	[Serializable]
	public class ThemeMaterialCollection
	{
		// Token: 0x06001C8C RID: 7308 RVA: 0x0006A2D4 File Offset: 0x000684D4
		public void ConstructPropertyBlocks()
		{
			for (int materialIndex = 0; materialIndex < this.materialProperties.Length; materialIndex++)
			{
				this.materialProperties[materialIndex] = new MaterialPropertyBlock();
			}
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x0006A304 File Offset: 0x00068504
		public void BindRendererToThemeTarget(Renderer renderer, ThemedMaterialType themeTarget)
		{
			if (!this._boundRenderers.ContainsKey(themeTarget))
			{
				this._boundRenderers.Add(themeTarget, new List<Renderer>());
			}
			if (!this._boundRenderers[themeTarget].Contains(renderer))
			{
				this._boundRenderers[themeTarget].Add(renderer);
			}
			if (themeTarget < (ThemedMaterialType)this.materialProperties.Length && this.materialProperties[(int)themeTarget] != null)
			{
				renderer.SetPropertyBlock(this.materialProperties[(int)themeTarget]);
			}
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x0006A37B File Offset: 0x0006857B
		public bool UnbindRendererFromThemeTarget(Renderer renderer, ThemedMaterialType themeTarget)
		{
			return this._boundRenderers.ContainsKey(themeTarget) && this._boundRenderers[themeTarget].Remove(renderer);
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x0006A3A0 File Offset: 0x000685A0
		public Color GetBlendedColor(ThemedMaterialType colorTarget, Theme oldTheme, Theme newTheme, float progress, bool applyOverrides, bool blendOverrides, string propertyToChange = "_Color")
		{
			Color oldColor;
			if (blendOverrides)
			{
				oldColor = ((!applyOverrides) ? oldTheme.GetDeleteModeColor(colorTarget, propertyToChange) : oldTheme.GetColor(colorTarget, propertyToChange));
			}
			else
			{
				oldColor = (applyOverrides ? oldTheme.GetDeleteModeColor(colorTarget, propertyToChange) : oldTheme.GetColor(colorTarget, propertyToChange));
			}
			Color newColor = applyOverrides ? newTheme.GetDeleteModeColor(colorTarget, propertyToChange) : newTheme.GetColor(colorTarget, propertyToChange);
			return Color.LerpUnclamped(oldColor, newColor, progress);
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x0006A408 File Offset: 0x00068608
		public void ApplyTheme(Theme oldTheme, Theme newTheme, float progress, bool applyOverrides, bool blendOverrides)
		{
			for (int targetIndex = 0; targetIndex < 86; targetIndex++)
			{
				ThemedMaterialType targetEnum = (ThemedMaterialType)targetIndex;
				foreach (ThemedColor themedColor in newTheme.GetColors(targetEnum))
				{
					if (Diagnostics.Verify(this.materialBindings.Length > targetIndex, "There aren't enough material bindings for enums! Check for an earlier assert for more information.") && this.materialBindings[targetIndex] != null)
					{
						Color color = this.GetBlendedColor(targetEnum, oldTheme, newTheme, progress, applyOverrides, blendOverrides, themedColor.propertyToChange);
						this.materialBindings[targetIndex].SetColor(themedColor.propertyToChange, color);
						this.materialProperties[targetIndex].SetColor(themedColor.propertyToChange, color);
					}
				}
			}
			for (int targetIndex2 = 0; targetIndex2 < 86; targetIndex2++)
			{
				ThemedMaterialType targetEnum2 = (ThemedMaterialType)targetIndex2;
				if (this._boundRenderers.ContainsKey(targetEnum2))
				{
					foreach (Renderer renderer in this._boundRenderers[targetEnum2])
					{
						SpriteRenderer sr = renderer as SpriteRenderer;
						if (sr != null)
						{
							this.materialProperties[targetIndex2].SetTexture(ThemeMaterialCollection.MainTex, sr.sprite.texture);
						}
						renderer.SetPropertyBlock(this.materialProperties[targetIndex2]);
					}
				}
			}
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x0006A578 File Offset: 0x00068778
		public void SetWorldGridThickness(float thickness)
		{
			if (!this._boundRenderers.ContainsKey(ThemedMaterialType.WorldGrid))
			{
				return;
			}
			for (int rendererIndex = 0; rendererIndex < this._boundRenderers[ThemedMaterialType.WorldGrid].Count; rendererIndex++)
			{
				Renderer renderer = this._boundRenderers[ThemedMaterialType.WorldGrid][rendererIndex];
				if (renderer == null)
				{
					this._boundRenderers[ThemedMaterialType.WorldGrid].RemoveAt(rendererIndex);
					rendererIndex--;
				}
				else
				{
					renderer.sharedMaterial.SetFloat("_Thickness", thickness);
				}
			}
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x0006A5FC File Offset: 0x000687FC
		public void SetMountainDotDiagonalRatio(float newRatio)
		{
			for (int targetMountainIndex = 32; targetMountainIndex <= 34; targetMountainIndex++)
			{
				this.materialBindings[targetMountainIndex].GetFloat("_DiagonalThickness");
				this.materialBindings[targetMountainIndex].SetFloat("_DotDiagonalRatio", Mathf.Lerp(0.2f, 1f, newRatio));
				this.materialProperties[targetMountainIndex].SetFloat("_DotDiagonalRatio", Mathf.Lerp(0.2f, 1f, newRatio));
				if (this._boundRenderers.ContainsKey((ThemedMaterialType)targetMountainIndex))
				{
					foreach (Renderer renderer in this._boundRenderers[(ThemedMaterialType)targetMountainIndex])
					{
						renderer.SetPropertyBlock(this.materialProperties[targetMountainIndex]);
					}
				}
			}
		}

		// Token: 0x04001886 RID: 6278
		[EnumTypedArray(typeof(ThemedMaterialType))]
		[NonReorderable]
		public Material[] materialBindings = new Material[86];

		// Token: 0x04001887 RID: 6279
		private Dictionary<ThemedMaterialType, List<Renderer>> _boundRenderers = new Dictionary<ThemedMaterialType, List<Renderer>>();

		// Token: 0x04001888 RID: 6280
		[EnumTypedArray(typeof(ThemedMaterialType))]
		[NonReorderable]
		public MaterialPropertyBlock[] materialProperties = new MaterialPropertyBlock[86];

		// Token: 0x04001889 RID: 6281
		private static readonly int MainTex = Shader.PropertyToID("_MainTex");

		// Token: 0x0400188A RID: 6282
		private const float MountainDotSize = 0.2f;
	}
}
