using System;
using System.Collections.Generic;
using Client;
using Factory;
using Motorways.Constants;
using Motorways.Themes;
using UnityEngine;

namespace Motorways.Views.MeshGeneration
{
	// Token: 0x0200061F RID: 1567
	public class CombinedMeshThemeComponent : IThemeComponent
	{
		// Token: 0x06002BE0 RID: 11232 RVA: 0x000C20C4 File Offset: 0x000C02C4
		public static void SetRelativeVertexColorIndexForMesh(Mesh mesh, ThemeComponentGroupTarget groupTarget)
		{
			int id = CombinedMeshThemeComponent.RelativeComponentGroupTargetIndex(groupTarget);
			CombinedMeshThemeComponent.SetVertexColorIndexForMesh(mesh, id);
		}

		// Token: 0x06002BE1 RID: 11233 RVA: 0x000C20DF File Offset: 0x000C02DF
		public static int RelativeThemeComponentGroupTargetOffsetForGroup(int groupIndex)
		{
			return groupIndex * CombinedMeshThemeComponent.ThemeComponentGroupTargets.Length;
		}

		// Token: 0x06002BE2 RID: 11234 RVA: 0x000C20EC File Offset: 0x000C02EC
		public static void SetAbsoluteVertexColorIndexForMesh(Mesh mesh, ThemedMaterialType themedMaterialType)
		{
			int id = CombinedMeshThemeComponent.AbsoluteThemedMaterialTypeIndex(themedMaterialType);
			CombinedMeshThemeComponent.SetVertexColorIndexForMesh(mesh, id);
		}

		// Token: 0x06002BE3 RID: 11235 RVA: 0x000C2108 File Offset: 0x000C0308
		private static void SetVertexColorIndexForMesh(Mesh mesh, int id)
		{
			List<Color> colors = new List<Color>();
			mesh.GetColors(colors);
			if (colors.Count == 0)
			{
				for (int vertexIndex = 0; vertexIndex < mesh.vertexCount; vertexIndex++)
				{
					colors.Add(new Color((float)id, 0f, 0f, 1f));
				}
			}
			else
			{
				for (int colorIndex = 0; colorIndex < colors.Count; colorIndex++)
				{
					colors[colorIndex] = new Color((float)id, 0f, 0f, colors[colorIndex].a);
				}
			}
			mesh.SetColors(colors);
		}

		// Token: 0x06002BE4 RID: 11236 RVA: 0x000C2198 File Offset: 0x000C0398
		private static int AbsoluteThemedMaterialTypeIndex(ThemedMaterialType themedMaterialType)
		{
			for (int themedMaterialTypeIndex = 0; themedMaterialTypeIndex < CombinedMeshThemeComponent.ThemedMaterialTypes.Length; themedMaterialTypeIndex++)
			{
				if (themedMaterialType == CombinedMeshThemeComponent.ThemedMaterialTypes[themedMaterialTypeIndex])
				{
					return themedMaterialTypeIndex;
				}
			}
			Diagnostics.FailAssert("ThemedMaterialType '{0}' is not registered in CombinedMeshThemeComponent", new object[]
			{
				themedMaterialType
			});
			return -1;
		}

		// Token: 0x06002BE5 RID: 11237 RVA: 0x000C21DD File Offset: 0x000C03DD
		private static int ThemeComponentGroupTargetOffsetForGroup(int groupIndex)
		{
			return CombinedMeshThemeComponent.ThemedMaterialTypes.Length + CombinedMeshThemeComponent.RelativeThemeComponentGroupTargetOffsetForGroup(groupIndex);
		}

		// Token: 0x06002BE6 RID: 11238 RVA: 0x000C21F0 File Offset: 0x000C03F0
		private static int RelativeComponentGroupTargetIndex(ThemeComponentGroupTarget groupTarget)
		{
			for (int themeComponentGroupTargetIndex = 0; themeComponentGroupTargetIndex < CombinedMeshThemeComponent.ThemeComponentGroupTargets.Length; themeComponentGroupTargetIndex++)
			{
				if (groupTarget == CombinedMeshThemeComponent.ThemeComponentGroupTargets[themeComponentGroupTargetIndex])
				{
					return CombinedMeshThemeComponent.ThemedMaterialTypes.Length + themeComponentGroupTargetIndex;
				}
			}
			return -1;
		}

		// Token: 0x06002BE7 RID: 11239 RVA: 0x000C2224 File Offset: 0x000C0424
		public void InitializeTheme(IThemeDatabase themeDatabase)
		{
			this._combinedMeshMaterials.vehicleMaterial.SetInt(ShaderConstants.ThemeComponentGroupTargetCount, CombinedMeshThemeComponent.ThemeComponentGroupTargets.Length);
		}

		// Token: 0x06002BE8 RID: 11240 RVA: 0x000C2244 File Offset: 0x000C0444
		public void ApplyTheme(ITheme theme)
		{
			Theme motorwaysTheme = (Theme)theme;
			for (int themedMaterialTypeIndex = 0; themedMaterialTypeIndex < CombinedMeshThemeComponent.ThemedMaterialTypes.Length; themedMaterialTypeIndex++)
			{
				ThemedMaterialType themedMaterialType = CombinedMeshThemeComponent.ThemedMaterialTypes[themedMaterialTypeIndex];
				this._colors[themedMaterialTypeIndex] = motorwaysTheme.GetColor(themedMaterialType, "_Color");
			}
			for (int groupIndex = 0; groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS; groupIndex++)
			{
				int groupColorOffset = CombinedMeshThemeComponent.ThemeComponentGroupTargetOffsetForGroup(groupIndex);
				for (int componentGroupIndex = 0; componentGroupIndex < CombinedMeshThemeComponent.ThemeComponentGroupTargets.Length; componentGroupIndex++)
				{
					ThemeComponentGroupTarget groupTarget = CombinedMeshThemeComponent.ThemeComponentGroupTargets[componentGroupIndex];
					this._colors[groupColorOffset + componentGroupIndex] = motorwaysTheme.GetBuildingColor(groupIndex, groupTarget);
				}
			}
			this._combinedMeshMaterials.vehicleMaterial.SetVectorArray(ShaderConstants.Colors, this._colors);
			this._combinedMeshMaterials.vertexColorMaterial.SetVectorArray(ShaderConstants.Colors, this._colors);
			this._combinedMeshMaterials.vehicleMaterial.SetColor(ShaderConstants.ShadowColor, motorwaysTheme.GetColor(ThemedMaterialType.Shadow, "_Color"));
		}

		// Token: 0x06002BE9 RID: 11241 RVA: 0x000C2340 File Offset: 0x000C0540
		public ThemeBlendingResult ApplyBlendedTheme(ITheme oldTheme, ITheme newTheme, float progress)
		{
			Theme oldMotorwaysTheme = (Theme)oldTheme;
			Theme newMotorwaysTheme = (Theme)newTheme;
			for (int typeIndex = 0; typeIndex < CombinedMeshThemeComponent.ThemedMaterialTypes.Length; typeIndex++)
			{
				ThemedMaterialType themedMaterialType = CombinedMeshThemeComponent.ThemedMaterialTypes[typeIndex];
				Color oldColor = oldMotorwaysTheme.GetColor(themedMaterialType, "_Color");
				Color newColor = newMotorwaysTheme.GetColor(themedMaterialType, "_Color");
				this._colors[typeIndex] = Color.Lerp(oldColor, newColor, progress);
			}
			for (int groupIndex = 0; groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS; groupIndex++)
			{
				int groupColorOffset = CombinedMeshThemeComponent.ThemeComponentGroupTargetOffsetForGroup(groupIndex);
				for (int componentGroupIndex = 0; componentGroupIndex < CombinedMeshThemeComponent.ThemeComponentGroupTargets.Length; componentGroupIndex++)
				{
					ThemeComponentGroupTarget groupTarget = CombinedMeshThemeComponent.ThemeComponentGroupTargets[componentGroupIndex];
					Color oldColor2 = oldMotorwaysTheme.GetBuildingColor(groupIndex, groupTarget);
					Color newColor2 = newMotorwaysTheme.GetBuildingColor(groupIndex, groupTarget);
					this._colors[groupColorOffset + componentGroupIndex] = Color.Lerp(oldColor2, newColor2, progress);
				}
			}
			this._combinedMeshMaterials.vehicleMaterial.SetVectorArray(ShaderConstants.Colors, this._colors);
			this._combinedMeshMaterials.vertexColorMaterial.SetVectorArray(ShaderConstants.Colors, this._colors);
			Color shadowColor = Color.LerpUnclamped(oldMotorwaysTheme.GetColor(ThemedMaterialType.Shadow, "_Color"), newMotorwaysTheme.GetColor(ThemedMaterialType.Shadow, "_Color"), progress);
			this._combinedMeshMaterials.vehicleMaterial.SetColor(ShaderConstants.ShadowColor, shadowColor);
			return ThemeBlendingResult.ContinueBlending;
		}

		// Token: 0x06002BEA RID: 11242 RVA: 0x000022F5 File Offset: 0x000004F5
		public void ReleaseTheme(IThemeDatabase themeDatabase)
		{
		}

		// Token: 0x04002610 RID: 9744
		private const int InvalidIndex = -1;

		// Token: 0x04002611 RID: 9745
		private static readonly ThemeComponentGroupTarget[] ThemeComponentGroupTargets = new ThemeComponentGroupTarget[]
		{
			ThemeComponentGroupTarget.CarBase,
			ThemeComponentGroupTarget.CarHeadlights,
			ThemeComponentGroupTarget.CarWindows,
			ThemeComponentGroupTarget.BuildingBase,
			ThemeComponentGroupTarget.BuildingSide,
			ThemeComponentGroupTarget.BuildingTop,
			ThemeComponentGroupTarget.BuildingSelfShadow,
			ThemeComponentGroupTarget.HouseBase,
			ThemeComponentGroupTarget.HouseShadow
		};

		// Token: 0x04002612 RID: 9746
		private static readonly ThemedMaterialType[] ThemedMaterialTypes = new ThemedMaterialType[]
		{
			ThemedMaterialType.RoadInner,
			ThemedMaterialType.CarparkDetail,
			ThemedMaterialType.CarparkOutline
		};

		// Token: 0x04002613 RID: 9747
		private readonly Vector4[] _colors = new Vector4[CombinedMeshThemeComponent.ThemedMaterialTypes.Length + MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS * CombinedMeshThemeComponent.ThemeComponentGroupTargets.Length];

		// Token: 0x04002614 RID: 9748
		[Dependency]
		private CombinedMeshMaterials _combinedMeshMaterials;
	}
}
