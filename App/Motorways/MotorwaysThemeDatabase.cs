using System;
using System.Collections.Generic;
using Client;
using Factory;
using Motorways.Audio;
using Motorways.Themes;
using Motorways.Views;
using Screens;
using Unity.Profiling;
using UnityEngine;

namespace Motorways
{
	// Token: 0x020003DE RID: 990
	public class MotorwaysThemeDatabase : IThemeDatabase
	{
		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x060017ED RID: 6125 RVA: 0x000554FF File Offset: 0x000536FF
		public Theme TargetTheme
		{
			get
			{
				return this._targetTheme;
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x060017EE RID: 6126 RVA: 0x00055507 File Offset: 0x00053707
		public MotorwaysThemePreference ThemePreference
		{
			get
			{
				return this._themePreference;
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x060017EF RID: 6127 RVA: 0x0005550F File Offset: 0x0005370F
		public float TransitionDuration
		{
			get
			{
				return this._transitionDuration;
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x060017F0 RID: 6128 RVA: 0x00055517 File Offset: 0x00053717
		private float LerpPercentage
		{
			get
			{
				if (this._transitionDuration <= 1E-45f)
				{
					return 1f;
				}
				return this._transitionProgress / this._transitionDuration;
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x060017F1 RID: 6129 RVA: 0x00055539 File Offset: 0x00053739
		public bool IsDirty
		{
			get
			{
				return this._dirty;
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x060017F2 RID: 6130 RVA: 0x00055541 File Offset: 0x00053741
		public bool ApplyThemeOverrides
		{
			get
			{
				return this._applyThemeOverrides;
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x060017F3 RID: 6131 RVA: 0x00055549 File Offset: 0x00053749
		public bool IsBlendingOverrides
		{
			get
			{
				return this._isBlendingOverrides;
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x060017F4 RID: 6132 RVA: 0x00055551 File Offset: 0x00053751
		public MaterialPropertyBlock MaterialPropertyBlock
		{
			get
			{
				return this._materialPropertyBlock;
			}
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x00055559 File Offset: 0x00053759
		public MotorwaysThemeDatabase(MotorwaysThemeDatabaseBindings themeBindings)
		{
			this.bindings = themeBindings;
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x060017F6 RID: 6134 RVA: 0x00055589 File Offset: 0x00053789
		public Theme ActiveColorblindTheme
		{
			get
			{
				if (!this._activePlayer.IsNightModeEnabled)
				{
					return this.bindings.colorblindThemeColorful;
				}
				return this.bindings.colorblindThemeDark;
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x060017F7 RID: 6135 RVA: 0x000555AF File Offset: 0x000537AF
		public ColorGroup[] ActiveColorblindColorGroups
		{
			get
			{
				if (!this._activePlayer.IsNightModeEnabled)
				{
					return this._visualConstants.AvailableColorfulColorBlindColorGroups;
				}
				return this._visualConstants.AvailableDarkColorBlindColorGroups;
			}
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x000555D8 File Offset: 0x000537D8
		public void Start()
		{
			this._themePreference = MotorwaysThemePreference.Colorful;
			this._activePlayer.DataChanged += this.OnPlayerDataChanged;
			MotorwaysThemePreference themePreference = this._themePreference;
			if (themePreference == MotorwaysThemePreference.Dark || themePreference == MotorwaysThemePreference.DarkColorblind)
			{
				Get.State |= StateType.ModeNight;
			}
			else
			{
				Get.State &= ~StateType.ModeNight;
			}
			MotorwaysThemeDatabase.Log.Info("Theme database initialised: {0}", new object[]
			{
				this.bindings.name
			});
			this._materialPropertyBlock = new MaterialPropertyBlock();
			this.materialCollection = this.bindings.materialCollection;
			this.materialCollection.ConstructPropertyBlocks();
			MotorwaysThemeDatabase.Log.Info("Instantiated material collection", Array.Empty<object>());
			this.perGroupMaterials = new PerGroupMaterialBindings(this.bindings.perGroupMaterials);
			this.perGroupMaterials.SetMaterialPropertyBlock(this._materialPropertyBlock);
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x000556BC File Offset: 0x000538BC
		public void Tick(float deltaTime)
		{
			if (this._dirty)
			{
				if (this._oldTheme == null)
				{
					this._transitionProgress = this._transitionDuration;
					this._oldTheme = this._targetTheme;
				}
				if (this.BlendBetweenThemes(this._oldTheme, this._targetTheme, ((this._transitionStyle == TransitionStyle.Snap || !this._player.HasActivePlayer || this._player.IsSkipTransitionsEnabled) && !this._forceBlend) ? this._transitionDuration : deltaTime))
				{
					this._dirty = false;
					this._forceBlend = false;
					this._oldTheme = this._targetTheme;
					this._transitionStyle = TransitionStyle.Tween;
				}
			}
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x00055763 File Offset: 0x00053963
		public void SnapCurrentTransition()
		{
			if (this._dirty)
			{
				this._transitionStyle = TransitionStyle.Snap;
			}
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x00055774 File Offset: 0x00053974
		private bool BlendBetweenThemes(Theme oldTheme, Theme targetTheme, float deltaTime)
		{
			bool complete = false;
			if (this._transitionDelay > Mathf.Epsilon)
			{
				this._transitionDelay -= deltaTime;
				return false;
			}
			this._transitionProgress += deltaTime;
			if (this._transitionProgress >= this._transitionDuration)
			{
				this._transitionProgress = this._transitionDuration;
				this._isBlendingOverrides = false;
				this._transitionDelay = 0f;
				complete = true;
			}
			float lerpPercentage = this.LerpPercentage;
			this.materialCollection.ApplyTheme(oldTheme, targetTheme, lerpPercentage, this._applyThemeOverrides, this._isBlendingOverrides);
			this.perGroupMaterials.ApplyTheme(oldTheme, targetTheme, lerpPercentage);
			foreach (MotorwaysClient motorwaysClient in this._viewClients)
			{
				((IClient)motorwaysClient).ApplyBlendedTheme(oldTheme, targetTheme, lerpPercentage);
			}
			if (oldTheme != targetTheme)
			{
				if (this._screenStack == null)
				{
					goto IL_144;
				}
				using (IEnumerator<IScreen> enumerator2 = this._screenStack.GetActiveScreens().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IScreen activeScreen = enumerator2.Current;
						if (activeScreen != null)
						{
							BaseScalingScreen currentScreen = activeScreen as BaseScalingScreen;
							if (currentScreen != null)
							{
								currentScreen.ApplyBlendedTheme(oldTheme, targetTheme, lerpPercentage);
							}
						}
					}
					goto IL_144;
				}
			}
			if (this._isBlendingOverrides)
			{
				GameContainerScreen screen = this._screenStack.GetActiveScreen<GameContainerScreen>();
				if (screen != null)
				{
					screen.ApplyBlendedTheme(oldTheme, targetTheme, lerpPercentage);
				}
			}
			IL_144:
			if (this._gameCamera.customBlur != null)
			{
				float newRange = Mathf.Lerp(oldTheme.screenBackgroundBlur.blurLevelsRange, targetTheme.screenBackgroundBlur.blurLevelsRange, lerpPercentage);
				float newOffset = Mathf.Lerp(oldTheme.screenBackgroundBlur.blurLevelsOffset, targetTheme.screenBackgroundBlur.blurLevelsOffset, lerpPercentage);
				this._gameCamera.customBlur.LevelsRange = newRange;
				this._gameCamera.customBlur.LevelsOffset = newOffset;
			}
			return complete;
		}

		// Token: 0x060017FC RID: 6140 RVA: 0x00055958 File Offset: 0x00053B58
		public void SetNightMode(bool nightModeOn, bool forceBlend = false)
		{
			if (this.IsInNightMode == nightModeOn)
			{
				return;
			}
			if (this.IsInColorblindMode)
			{
				this.SetThemePreference(nightModeOn ? MotorwaysThemePreference.DarkColorblind : MotorwaysThemePreference.Colorblind, true, true, forceBlend);
				return;
			}
			if (nightModeOn)
			{
				this._dayThemePreference = this._themePreference;
				this.SetThemePreference(MotorwaysThemePreference.Dark, true, true, forceBlend);
				return;
			}
			this.SetThemePreference(this._dayThemePreference, true, true, forceBlend);
		}

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x060017FD RID: 6141 RVA: 0x000559B1 File Offset: 0x00053BB1
		public bool IsInNightMode
		{
			get
			{
				return this._themePreference == MotorwaysThemePreference.Dark || this._themePreference == MotorwaysThemePreference.DarkColorblind;
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x060017FE RID: 6142 RVA: 0x000559C7 File Offset: 0x00053BC7
		public bool IsInColorblindMode
		{
			get
			{
				return this._themePreference == MotorwaysThemePreference.Colorblind || this._themePreference == MotorwaysThemePreference.DarkColorblind;
			}
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x000559E0 File Offset: 0x00053BE0
		public void SetColorblindMode(bool colorblindOn, bool forceBlend = false)
		{
			if (this.IsInColorblindMode == colorblindOn)
			{
				return;
			}
			if (colorblindOn)
			{
				this.SetThemePreference(this.IsInNightMode ? MotorwaysThemePreference.DarkColorblind : MotorwaysThemePreference.Colorblind, true, true, forceBlend);
				return;
			}
			this._dayThemePreference = MotorwaysThemePreference.Colorful;
			this.SetThemePreference(this.IsInNightMode ? MotorwaysThemePreference.Dark : MotorwaysThemePreference.Colorful, true, true, forceBlend);
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x00055A2C File Offset: 0x00053C2C
		public void SetThemePreference(MotorwaysThemePreference newPreference, bool saveThemePreference = true, bool playAudio = true, bool forceBlend = false)
		{
			this._transitionDuration = this._themeTransitionTime;
			if (newPreference != this._themePreference)
			{
				if (this._targetTheme != null)
				{
					this._dirty = true;
				}
				this._forceBlend = forceBlend;
			}
			this._themePreference = newPreference;
			if (this._currentMapDefinition != null && this._dirty)
			{
				this.UpdateThemeFromCurrentDefinition(false);
			}
			if (saveThemePreference)
			{
				this.SaveThemePreference();
			}
			MotorwaysThemePreference themePreference = this._themePreference;
			if (themePreference == MotorwaysThemePreference.Dark || themePreference == MotorwaysThemePreference.DarkColorblind)
			{
				Get.State |= StateType.ModeNight;
			}
			else
			{
				Get.State &= ~StateType.ModeNight;
			}
			if (playAudio)
			{
				AudioSystem.Instance.ScheduleEvent(AudioEvent.CreateEvent(AudioSystem.Instance.DspTime, AudioEventType.NightMode, 0.5f, -1f, true, null));
			}
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00055AFC File Offset: 0x00053CFC
		private void SaveThemePreference()
		{
			this._activePlayer.DataChanged -= this.OnPlayerDataChanged;
			if (this._themePreference == MotorwaysThemePreference.Colorful)
			{
				this._activePlayer.ColorfulOption = MotorwaysThemeColorfulOptions.Colorful.ToString();
			}
			else if (this._themePreference == MotorwaysThemePreference.Maps)
			{
				this._activePlayer.ColorfulOption = MotorwaysThemeColorfulOptions.Maps.ToString();
			}
			bool nightMode = this._themePreference == MotorwaysThemePreference.Dark || this._themePreference == MotorwaysThemePreference.DarkColorblind;
			bool colorblindMode = this._themePreference == MotorwaysThemePreference.Colorblind || this._themePreference == MotorwaysThemePreference.DarkColorblind;
			this._activePlayer.IsColorblindModeEnabled = colorblindMode;
			this._activePlayer.IsNightModeEnabled = nightMode;
			this._activePlayer.DataChanged += this.OnPlayerDataChanged;
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x00055BC4 File Offset: 0x00053DC4
		private MotorwaysThemePreference GetThemePreferenceFromSave()
		{
			if (this._activePlayer.IsColorblindModeEnabled)
			{
				if (this._activePlayer.IsNightModeEnabled)
				{
					return MotorwaysThemePreference.DarkColorblind;
				}
				return MotorwaysThemePreference.Colorblind;
			}
			else
			{
				if (this._activePlayer.IsNightModeEnabled)
				{
					return MotorwaysThemePreference.Dark;
				}
				MotorwaysThemeColorfulOptions colorfulOption;
				if (Enum.TryParse<MotorwaysThemeColorfulOptions>(this._activePlayer.ColorfulOption, out colorfulOption))
				{
					if (colorfulOption == MotorwaysThemeColorfulOptions.Colorful)
					{
						return MotorwaysThemePreference.Colorful;
					}
					if (colorfulOption == MotorwaysThemeColorfulOptions.Maps)
					{
						return MotorwaysThemePreference.Maps;
					}
				}
				return MotorwaysThemePreference.Colorful;
			}
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x00055C20 File Offset: 0x00053E20
		public void OnPlayerDataChanged()
		{
			MotorwaysThemePreference themePreference = this.ThemePreference;
			MotorwaysThemePreference newPreference = this.GetThemePreferenceFromSave();
			if (themePreference != newPreference)
			{
				this.SetThemePreference(newPreference, false, false, false);
			}
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x00055C48 File Offset: 0x00053E48
		private void ApplyTheme(Theme newTheme, bool forceApply = false)
		{
			MotorwaysThemeDatabase.Log.Info("Applying theme!", Array.Empty<object>());
			if (Diagnostics.Verify(newTheme != null, "Trying to set a theme which is null! Current preference: {0}", this._themePreference))
			{
				this.UpdateColorblindThemesFromActiveUserProfile();
				if (newTheme != this._targetTheme || forceApply)
				{
					MotorwaysThemeDatabase.Log.Info("Applying theme {0}", new object[]
					{
						newTheme.name
					});
					this._oldTheme = this._targetTheme;
					this._targetTheme = newTheme;
					this._transitionProgress = 0f;
					this._dirty = true;
					if (this._oldTheme == null)
					{
						this._oldTheme = this._targetTheme;
						this._transitionProgress = this._transitionDuration;
					}
				}
			}
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x00055D08 File Offset: 0x00053F08
		public void SetDrawMode(RoadDrawMode currentRoadDrawMode)
		{
			bool applyThemeOverrides = this._applyThemeOverrides;
			this._applyThemeOverrides = (currentRoadDrawMode == RoadDrawMode.Remove);
			if (applyThemeOverrides != this._applyThemeOverrides)
			{
				if (this._isBlendingOverrides)
				{
					this._transitionProgress = Mathf.Clamp(this._transitionDuration - this._transitionProgress, 0f, this._transitionDuration);
				}
				else
				{
					this._transitionProgress = 0f;
					this._transitionDelay = 0.1f;
				}
				this._transitionDuration = this._themeTransitionTime;
				this._isBlendingOverrides = true;
				this._forceBlend = true;
				this._dirty = true;
			}
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x00055D94 File Offset: 0x00053F94
		public void UpdateThemeFromCurrentDefinition(bool forceUpdate = false)
		{
			if (Diagnostics.Verify(this._currentMapDefinition != null, "No currentMapDefinition set in the ThemeDatabase. Call SetCurrentMapDefinition()"))
			{
				Theme newTheme = this._currentMapDefinition.themes[(int)this._themePreference];
				if (forceUpdate)
				{
					newTheme.Initialize();
				}
				if (Diagnostics.Verify(newTheme != null, "{0} doesn't have a theme for {1}!", this._currentMapDefinition, this._themePreference))
				{
					this.ApplyTheme(newTheme, forceUpdate);
				}
			}
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x00055E01 File Offset: 0x00054001
		public void SetCurrentMapDefinition(MapDefinition newMapDefinition, float blendDuration)
		{
			this._transitionDuration = blendDuration;
			MotorwaysThemeDatabase.Log.Info("Setting new map definition: {0}", new object[]
			{
				newMapDefinition.cityName
			});
			this._currentMapDefinition = newMapDefinition;
			this.UpdateThemeFromCurrentDefinition(false);
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x00055E38 File Offset: 0x00054038
		public Color GetGlobalColor(ThemedMaterialType target)
		{
			if (this._dirty)
			{
				return this.materialCollection.GetBlendedColor(target, this._oldTheme, this._targetTheme, this.LerpPercentage, this._applyThemeOverrides, this._isBlendingOverrides, "_Color");
			}
			return this._targetTheme.GetColor(target, "_Color");
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x00055E90 File Offset: 0x00054090
		public void RegisterGameObjectToThemeByGroupIndex(GameObject gameObject, int groupIndex)
		{
			foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
			{
				int themeTarget = this.bindings.GetPerGroupThemeTargetForMaterial(renderer.sharedMaterial);
				if (themeTarget >= 0)
				{
					this.perGroupMaterials.BindRendererToThemeTarget(renderer, groupIndex, (ThemeComponentGroupTarget)themeTarget);
				}
			}
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x00055EDC File Offset: 0x000540DC
		public void RegisterGameObjectToThemeByGroupIndex(GameObject gameObject, int groupIndex, ThemeComponentGroupTarget groupTarget)
		{
			foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
			{
				this.perGroupMaterials.BindRendererToThemeTarget(renderer, groupIndex, groupTarget);
			}
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x00055F14 File Offset: 0x00054114
		public bool UnregisterGameObjectFromThemeByGroupIndex(GameObject gameObject, int groupIndex)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			bool success = true;
			foreach (Renderer renderer in componentsInChildren)
			{
				success = (this.perGroupMaterials.UnbindRendererFromThemeTarget(renderer, groupIndex) && success);
			}
			return success;
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x00055F4D File Offset: 0x0005414D
		public ITheme GetTheme()
		{
			return this.TargetTheme;
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x00055F58 File Offset: 0x00054158
		public void AddView(IClient view)
		{
			if (typeof(MotorwaysClient).IsAssignableFrom(view.GetType()))
			{
				MotorwaysClient motorwaysClient = (MotorwaysClient)view;
				if (!this._viewClients.Contains(motorwaysClient))
				{
					this._viewClients.Add(motorwaysClient);
				}
			}
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x00055FA0 File Offset: 0x000541A0
		public void RemoveView(IClient view)
		{
			if (typeof(MotorwaysClient).IsAssignableFrom(view.GetType()))
			{
				MotorwaysClient motorwaysClient = (MotorwaysClient)view;
				this._viewClients.Remove(motorwaysClient);
			}
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x00055FD8 File Offset: 0x000541D8
		public void DisableDeleteModeOverrides()
		{
			bool applyThemeOverrides = this._applyThemeOverrides;
			this._applyThemeOverrides = false;
			if (applyThemeOverrides != this._applyThemeOverrides)
			{
				if (this._isBlendingOverrides)
				{
					this._transitionProgress = Mathf.Clamp(this._transitionDuration - this._transitionProgress, 0f, this._transitionDuration);
				}
				else
				{
					this._transitionProgress = 0f;
				}
				this._transitionDuration = this._themeTransitionTime;
				this._isBlendingOverrides = true;
				this._dirty = true;
			}
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x0005604C File Offset: 0x0005424C
		public void UpdateColorblindThemesFromActiveUserProfile()
		{
			Theme originalColorsColorful = this.bindings.colorblindThemeColorful;
			Theme originalColorsDark = this.bindings.colorblindThemeDark;
			List<int> currentSavedIndexes = this._activePlayer.MotorwaysExtendedUserProfile.PlayerColorblindPaletteIndexes;
			ColorGroup[] constantColorsColorful = this._visualConstants.AvailableColorfulColorBlindColorGroups;
			ColorGroup[] constantColorsDark = this._visualConstants.AvailableDarkColorBlindColorGroups;
			for (int groupIndex = 0; groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS; groupIndex++)
			{
				originalColorsColorful.buildingColorGroups[groupIndex] = constantColorsColorful[currentSavedIndexes[groupIndex]];
				originalColorsDark.buildingColorGroups[groupIndex] = constantColorsDark[currentSavedIndexes[groupIndex]];
			}
		}

		// Token: 0x0400147F RID: 5247
		public static int MAX_THEME_COLOR_GROUPS = 6;

		// Token: 0x04001480 RID: 5248
		private List<MotorwaysClient> _viewClients = new List<MotorwaysClient>();

		// Token: 0x04001481 RID: 5249
		[Dependency]
		private ActivePlayer _activePlayer;

		// Token: 0x04001482 RID: 5250
		[Dependency]
		private ScreenStack _screenStack;

		// Token: 0x04001483 RID: 5251
		[Dependency]
		private GameCamera _gameCamera;

		// Token: 0x04001484 RID: 5252
		[Dependency]
		private ActivePlayer _player;

		// Token: 0x04001485 RID: 5253
		[Dependency]
		private VisualConstantsData _visualConstants;

		// Token: 0x04001486 RID: 5254
		public static Diagnostics.Log.Channel Log = new Diagnostics.Log.Channel("Theme Database");

		// Token: 0x04001487 RID: 5255
		private Theme _targetTheme;

		// Token: 0x04001488 RID: 5256
		private Theme _oldTheme;

		// Token: 0x04001489 RID: 5257
		public MotorwaysThemeDatabaseBindings bindings;

		// Token: 0x0400148A RID: 5258
		public PerGroupMaterialBindings perGroupMaterials;

		// Token: 0x0400148B RID: 5259
		public ThemeMaterialCollection materialCollection;

		// Token: 0x0400148C RID: 5260
		private MotorwaysThemePreference _themePreference;

		// Token: 0x0400148D RID: 5261
		private MotorwaysThemePreference _dayThemePreference;

		// Token: 0x0400148E RID: 5262
		private TransitionStyle _transitionStyle;

		// Token: 0x0400148F RID: 5263
		private float _transitionDuration = 1f;

		// Token: 0x04001490 RID: 5264
		private float _themeTransitionTime = 0.3f;

		// Token: 0x04001491 RID: 5265
		private float _transitionDelay;

		// Token: 0x04001492 RID: 5266
		private const float OverridesTransitionDelay = 0.1f;

		// Token: 0x04001493 RID: 5267
		private float _transitionProgress;

		// Token: 0x04001494 RID: 5268
		private bool _dirty;

		// Token: 0x04001495 RID: 5269
		private bool _forceBlend;

		// Token: 0x04001496 RID: 5270
		private bool _applyThemeOverrides;

		// Token: 0x04001497 RID: 5271
		private bool _isBlendingOverrides;

		// Token: 0x04001498 RID: 5272
		private MapDefinition _currentMapDefinition;

		// Token: 0x04001499 RID: 5273
		private MaterialPropertyBlock _materialPropertyBlock;

		// Token: 0x0400149A RID: 5274
		private static readonly ProfilerMarker Profiler_BlendBetweenThemes = new ProfilerMarker(ProfilerCategory.Scripts, "MotorwaysThemeDatabase.BlendBetweenThemes()");
	}
}
