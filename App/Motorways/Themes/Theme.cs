using System;
using System.Collections.Generic;
using System.Diagnostics;
using Client;
using Factory;
using NaughtyAttributes;
using UnityEngine;

namespace Motorways.Themes
{
	// Token: 0x02000474 RID: 1140
	[CreateAssetMenu(menuName = "Motorways/Themes/Theme")]
	public class Theme : ScriptableObject, ITheme
	{
		// Token: 0x06001C6A RID: 7274 RVA: 0x00069658 File Offset: 0x00067858
		public Color GetColor(ThemedMaterialType type, string propertyToChange = "_Color")
		{
			if (!this._colorsInitialized)
			{
				this.Initialize();
			}
			List<ThemedColor> colors;
			if (this._typeToColors.TryGetValue(type, out colors))
			{
				if (colors.Count == 1)
				{
					return colors[0].color;
				}
				foreach (ThemedColor themedColor in colors)
				{
					if (themedColor.propertyToChange == propertyToChange)
					{
						return themedColor.color;
					}
				}
			}
			Diagnostics.Log.Error("Themes", "Theme {0} doesn't have a color of property {1} for {2}! Defaulting to magenta", new object[]
			{
				base.name,
				propertyToChange,
				type
			});
			return Color.magenta;
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x0006971C File Offset: 0x0006791C
		public List<ThemedColor> GetColors(ThemedMaterialType type)
		{
			if (!this._colorsInitialized)
			{
				this.Initialize();
			}
			List<ThemedColor> returnColors;
			if (this._typeToColors.TryGetValue(type, out returnColors))
			{
				return returnColors;
			}
			Diagnostics.Log.Error("Themes", "Theme {0} doesn't have any colors for {1}! Returning an empty list.", new object[]
			{
				base.name,
				type
			});
			return new List<ThemedColor>();
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x00069778 File Offset: 0x00067978
		public Color GetDeleteModeColor(ThemedMaterialType type, string propertyToChange = "_Color")
		{
			if (!this._deleteModeColorsInitialized)
			{
				this.Initialize();
			}
			List<ThemedColor> colors;
			if (this._typeToDeleteModeColor.TryGetValue(type, out colors))
			{
				if (colors.Count == 1)
				{
					return colors[0].color;
				}
				foreach (ThemedColor themedColor in colors)
				{
					if (themedColor.propertyToChange == propertyToChange)
					{
						return themedColor.color;
					}
				}
			}
			Diagnostics.Log.Error("Themes", "Theme {0} doesn't have a delete mode color of property {1} for {2}! Defaulting to magenta", new object[]
			{
				base.name,
				propertyToChange,
				type
			});
			return Color.magenta;
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x0006983C File Offset: 0x00067A3C
		private Color CalculateDeleteModeColor(ThemedMaterialType type, string propertyToChange = "_Color")
		{
			Color baseColor = this.GetColor(type, propertyToChange);
			if (this.deleteModeOverrides != null && this.deleteModeOverrides.themeTypesToOverride.Contains(type))
			{
				float h;
				float s;
				float v;
				Color.RGBToHSV(baseColor, out h, out s, out v);
				Vector3 overrides = this.GetAdditionalDeleteModeOverrideValues(type);
				h += overrides.x;
				s *= this.overrideSaturationMultiplier + overrides.y;
				v *= this.overrideDarkenMultiplier + overrides.z;
				Color newColor = Color.HSVToRGB(h, s, v);
				newColor.a = baseColor.a;
				baseColor = newColor;
			}
			return baseColor;
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x000698D0 File Offset: 0x00067AD0
		private Vector3 GetAdditionalDeleteModeOverrideValues(ThemedMaterialType type)
		{
			foreach (DeleteModeOverride deleteModeOverride in this.additionalDeleteModeOverrides)
			{
				if (deleteModeOverride.type == type.ToString())
				{
					return new Vector3(deleteModeOverride.hueOverride, deleteModeOverride.additionalSaturationMultiplier, deleteModeOverride.additionalDarkenMultiplier);
				}
			}
			return Vector3.zero;
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x00069958 File Offset: 0x00067B58
		public Color GetBuildingColor(int groupIndex, ThemeComponentGroupTarget groupTheme)
		{
			if (groupIndex == -1)
			{
				return Color.white;
			}
			if (Diagnostics.Verify(groupIndex < this.buildingColorGroups.Count, this, "Unable to find matching building color group for index: {0} - targets.Length: {1}", groupIndex, this.buildingColorGroups.Count))
			{
				return this.buildingColorGroups[groupIndex].GetColor(groupTheme);
			}
			return Color.magenta;
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x000699B8 File Offset: 0x00067BB8
		public void Initialize()
		{
			this._typeToColors = new Dictionary<ThemedMaterialType, List<ThemedColor>>();
			foreach (ThemeGroup themeGroup in this.themedColors)
			{
				foreach (ThemedColor themedColor3 in themeGroup.themedColors)
				{
					if (!this._typeToColors.ContainsKey(themedColor3.MaterialType))
					{
						this._typeToColors.Add(themedColor3.MaterialType, new List<ThemedColor>());
					}
					this._typeToColors[themedColor3.MaterialType].Add(themedColor3);
				}
			}
			this._colorsInitialized = true;
			this._typeToDeleteModeColor = new Dictionary<ThemedMaterialType, List<ThemedColor>>();
			foreach (ThemedColor themedColor2 in this.customDeleteModeOverrides.themedColors)
			{
				if (!this._typeToDeleteModeColor.ContainsKey(themedColor2.MaterialType))
				{
					this._typeToDeleteModeColor.Add(themedColor2.MaterialType, new List<ThemedColor>());
				}
				this._typeToDeleteModeColor[themedColor2.MaterialType].Add(themedColor2);
			}
			foreach (ThemeGroup themeGroup2 in this.themedColors)
			{
				using (List<ThemedColor>.Enumerator enumerator2 = themeGroup2.themedColors.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ThemedColor themedColor = enumerator2.Current;
						if (!this._typeToDeleteModeColor.ContainsKey(themedColor.MaterialType))
						{
							this._typeToDeleteModeColor.Add(themedColor.MaterialType, new List<ThemedColor>());
						}
						if (this._typeToDeleteModeColor[themedColor.MaterialType].Find((ThemedColor existingColor) => existingColor.propertyToChange == themedColor.propertyToChange) == null)
						{
							Color color = this.CalculateDeleteModeColor(themedColor.MaterialType, themedColor.propertyToChange);
							this._typeToDeleteModeColor[themedColor.MaterialType].Add(new ThemedColor(themedColor.MaterialType, color, themedColor.propertyToChange));
						}
					}
				}
			}
			this._deleteModeColorsInitialized = true;
		}

		// Token: 0x06001C71 RID: 7281 RVA: 0x000022F5 File Offset: 0x000004F5
		[Button(null)]
		public void UpdateTheme()
		{
		}

		// Token: 0x06001C72 RID: 7282 RVA: 0x00069CA8 File Offset: 0x00067EA8
		[Conditional("UNITY_EDITOR")]
		public static void UpdateInGameTheme()
		{
			if (Application.isPlaying && Application.isEditor)
			{
				AppRuntime appRuntime = UnityEngine.Object.FindObjectOfType<AppRuntime>();
				MotorwaysThemeDatabase motorwaysThemeDatabase;
				if (appRuntime == null)
				{
					motorwaysThemeDatabase = null;
				}
				else
				{
					IApp app = appRuntime.App;
					if (app == null)
					{
						motorwaysThemeDatabase = null;
					}
					else
					{
						IScope scope = app.Scope;
						motorwaysThemeDatabase = ((scope != null) ? scope.Get<MotorwaysThemeDatabase>() : null);
					}
				}
				MotorwaysThemeDatabase themeDatabase = motorwaysThemeDatabase;
				if (themeDatabase != null)
				{
					themeDatabase.UpdateThemeFromCurrentDefinition(true);
				}
			}
		}

		// Token: 0x04001806 RID: 6150
		public const string UpdateThemeAutomaticallyEditorPrefKey = "UpdateThemesAutomatically";

		// Token: 0x04001807 RID: 6151
		[Space(10f)]
		public List<ColorGroup> buildingColorGroups;

		// Token: 0x04001808 RID: 6152
		public List<ThemeGroup> themedColors;

		// Token: 0x04001809 RID: 6153
		[Space(10f)]
		public DeleteModeOverrideList deleteModeOverrides;

		// Token: 0x0400180A RID: 6154
		[Slider(-1, 1)]
		public float overrideDarkenMultiplier;

		// Token: 0x0400180B RID: 6155
		[Slider(-1, 1)]
		public float overrideSaturationMultiplier;

		// Token: 0x0400180C RID: 6156
		public List<DeleteModeOverride> additionalDeleteModeOverrides;

		// Token: 0x0400180D RID: 6157
		public ThemeGroup customDeleteModeOverrides;

		// Token: 0x0400180E RID: 6158
		[Space(10f)]
		public Theme.BlurSettings screenBackgroundBlur;

		// Token: 0x0400180F RID: 6159
		[NonSerialized]
		private bool _colorsInitialized;

		// Token: 0x04001810 RID: 6160
		[NonSerialized]
		private bool _deleteModeColorsInitialized;

		// Token: 0x04001811 RID: 6161
		[NonSerialized]
		private Dictionary<ThemedMaterialType, List<ThemedColor>> _typeToColors;

		// Token: 0x04001812 RID: 6162
		[NonSerialized]
		private Dictionary<ThemedMaterialType, List<ThemedColor>> _typeToDeleteModeColor;

		// Token: 0x02000475 RID: 1141
		[System.Serializable]
		public class BlurSettings
		{
			// Token: 0x04001813 RID: 6163
			public float blurLevelsRange = 0.7f;

			// Token: 0x04001814 RID: 6164
			public float blurLevelsOffset = -0.1f;
		}
	}
}
