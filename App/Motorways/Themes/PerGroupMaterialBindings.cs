using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Motorways.Themes
{
	// Token: 0x0200047F RID: 1151
	[Serializable]
	public class PerGroupMaterialBindings
	{
		// Token: 0x06001C95 RID: 7317 RVA: 0x0006A712 File Offset: 0x00068912
		public PerGroupMaterialBindings()
		{
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x0006A734 File Offset: 0x00068934
		public PerGroupMaterialBindings(PerGroupMaterialBindings copy)
		{
			this.materialBindings = copy.materialBindings;
			this.sharedBuildingMaterial = copy.sharedBuildingMaterial;
			this.sharedVehicleMaterial = copy.sharedVehicleMaterial;
			for (int groupIndex = 0; groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS; groupIndex++)
			{
				for (int targetIndex = 0; targetIndex < 10; targetIndex++)
				{
					ThemeComponentGroupTarget target = (ThemeComponentGroupTarget)targetIndex;
					this._renderersWithColors.Add(new ValueTuple<int, ThemeComponentGroupTarget>(groupIndex, target), new PerGroupMaterialBindings.RenderersWithColor());
				}
			}
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x0006A7B9 File Offset: 0x000689B9
		public void SetMaterialPropertyBlock(MaterialPropertyBlock materialPropertyBlock)
		{
			this._materialPropertyBlock = materialPropertyBlock;
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x0006A7C4 File Offset: 0x000689C4
		public Color GetBlendedColor(int groupIndex, ThemeComponentGroupTarget colorTarget, Theme oldTheme, Theme newTheme, float progress)
		{
			ColorGroup oldGroup = null;
			groupIndex %= Math.Min(oldTheme.buildingColorGroups.Count, newTheme.buildingColorGroups.Count);
			if (groupIndex >= 0 && groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS && groupIndex < oldTheme.buildingColorGroups.Count && Diagnostics.Verify(oldTheme.buildingColorGroups[groupIndex] != null, "Color Group not set in theme {0} for index {1}", oldTheme, groupIndex))
			{
				oldGroup = oldTheme.buildingColorGroups[groupIndex];
			}
			ColorGroup newGroup = null;
			if (groupIndex >= 0 && groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS && groupIndex < newTheme.buildingColorGroups.Count && Diagnostics.Verify(newTheme.buildingColorGroups[groupIndex] != null, "Color Group not set in theme {0} for index {1}", newTheme, groupIndex))
			{
				newGroup = newTheme.buildingColorGroups[groupIndex];
			}
			if (Diagnostics.Verify(oldGroup != null, "Old group is null for theme {0}!", oldTheme) && Diagnostics.Verify(newGroup != null, "New group is null for theme {0}!", newTheme))
			{
				Color color = oldGroup.GetColor(colorTarget);
				Color newColor = newGroup.GetColor(colorTarget);
				return Color.LerpUnclamped(color, newColor, progress);
			}
			return Color.magenta;
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x0006A8D8 File Offset: 0x00068AD8
		public void BindRendererToThemeTarget(Renderer renderer, int groupIndex, ThemeComponentGroupTarget themeTarget)
		{
			ValueTuple<int, ThemeComponentGroupTarget> rendererBindingKey = new ValueTuple<int, ThemeComponentGroupTarget>(groupIndex, themeTarget);
			PerGroupMaterialBindings.RenderersWithColor renderersWithColorForKey;
			if (this._renderersWithColors.TryGetValue(rendererBindingKey, out renderersWithColorForKey))
			{
				if (!renderersWithColorForKey.boundRenderers.Contains(renderer))
				{
					renderersWithColorForKey.boundRenderers.Add(renderer);
				}
				if (themeTarget != ThemeComponentGroupTarget.CarHeadlights && themeTarget != ThemeComponentGroupTarget.CarHeadlightBeams && renderer.name != "TrainCarriageNAMEMUSTBECHANGEDINCODETOO")
				{
					renderer.sharedMaterial = ((themeTarget == ThemeComponentGroupTarget.CarBase || themeTarget == ThemeComponentGroupTarget.CarWindows) ? this.sharedVehicleMaterial : this.sharedBuildingMaterial);
				}
				renderer.GetPropertyBlock(this._materialPropertyBlock);
				this._materialPropertyBlock.SetColor(PerGroupMaterialBindings.ColorId, renderersWithColorForKey.color);
				renderer.SetPropertyBlock(this._materialPropertyBlock);
			}
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x0006A988 File Offset: 0x00068B88
		public bool UnbindRendererFromThemeTarget(Renderer renderer, int groupIndex)
		{
			bool success = false;
			foreach (KeyValuePair<ValueTuple<int, ThemeComponentGroupTarget>, PerGroupMaterialBindings.RenderersWithColor> boundRendererGroup in this._renderersWithColors)
			{
				if (boundRendererGroup.Key.Item1 == groupIndex && boundRendererGroup.Value.boundRenderers.Remove(renderer))
				{
					success = true;
					renderer.sharedMaterial = this.materialBindings[(int)boundRendererGroup.Key.Item2];
				}
			}
			return success;
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x0006AA18 File Offset: 0x00068C18
		public void ApplyTheme(Theme oldTheme, Theme newTheme, float progress)
		{
			foreach (KeyValuePair<ValueTuple<int, ThemeComponentGroupTarget>, PerGroupMaterialBindings.RenderersWithColor> renderGroup in this._renderersWithColors)
			{
				Color blendedColor = this.GetBlendedColor(renderGroup.Key.Item1, renderGroup.Key.Item2, oldTheme, newTheme, progress);
				renderGroup.Value.color = blendedColor;
				foreach (Renderer renderer in renderGroup.Value.boundRenderers)
				{
					renderer.GetPropertyBlock(this._materialPropertyBlock);
					this._materialPropertyBlock.SetColor(PerGroupMaterialBindings.ColorId, blendedColor);
					renderer.SetPropertyBlock(this._materialPropertyBlock);
				}
			}
		}

		// Token: 0x0400188B RID: 6283
		[FormerlySerializedAs("sharedMat")]
		public Material sharedBuildingMaterial;

		// Token: 0x0400188C RID: 6284
		public Material sharedVehicleMaterial;

		// Token: 0x0400188D RID: 6285
		[EnumTypedArray(typeof(ThemeComponentGroupTarget))]
		[NonReorderable]
		public Material[] materialBindings = new Material[10];

		// Token: 0x0400188E RID: 6286
		private MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x0400188F RID: 6287
		private static readonly int ColorId = Shader.PropertyToID("_Color");

		// Token: 0x04001890 RID: 6288
		[TupleElementNames(new string[]
		{
			"groupIndex",
			"componentGroupTarget"
		})]
		private Dictionary<ValueTuple<int, ThemeComponentGroupTarget>, PerGroupMaterialBindings.RenderersWithColor> _renderersWithColors = new Dictionary<ValueTuple<int, ThemeComponentGroupTarget>, PerGroupMaterialBindings.RenderersWithColor>();

		// Token: 0x02000480 RID: 1152
		private class RenderersWithColor
		{
			// Token: 0x04001891 RID: 6289
			public Color color = Color.magenta;

			// Token: 0x04001892 RID: 6290
			public readonly List<Renderer> boundRenderers = new List<Renderer>();
		}
	}
}
