using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000137 RID: 311
public class OptionsMenuSettingSource : IFeatureToggleSettingSource
{
	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06000712 RID: 1810 RVA: 0x0000EFC6 File Offset: 0x0000D1C6
	public FeatureToggleSettingSourcePriority SourcePriority
	{
		get
		{
			return FeatureToggleSettingSourcePriority.InGameOptionsSource;
		}
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x000170DC File Offset: 0x000152DC
	public OptionsMenuSettingSource()
	{
		OptionsMenuSettingSource.Instance = this;
		bool inHiddenGroup = false;
		foreach (object obj in Enum.GetValues(typeof(Feature)))
		{
			Feature feature = (Feature)obj;
			if (feature == Feature.Group_Hidden)
			{
				inHiddenGroup = true;
			}
			else if (inHiddenGroup && feature.ToString().StartsWith("Group_", StringComparison.Ordinal))
			{
				inHiddenGroup = false;
			}
			else if (!inHiddenGroup)
			{
				this._toggleSettings.Add(feature, OptionsMenuSettingSource.GetPlayerPrefsFeatureToggleSettings(feature));
			}
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x00017190 File Offset: 0x00015390
	public static FeatureToggleState GetOptionsMenuFeatureState(Feature forFeature)
	{
		if (OptionsMenuSettingSource.Instance != null)
		{
			return OptionsMenuSettingSource.Instance.GetFeatureToggleState(forFeature);
		}
		return FeatureToggleState.NoOverride;
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x000171A6 File Offset: 0x000153A6
	public static void SetOptionsMenuFeatureState(Feature forFeature, FeatureToggleState newState)
	{
		OptionsMenuSettingSource instance = OptionsMenuSettingSource.Instance;
		if (instance == null)
		{
			return;
		}
		instance.SetFeatureToggleState(forFeature, newState);
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x000171BC File Offset: 0x000153BC
	public FeatureToggleState GetFeatureToggleState(Feature forFeature)
	{
		ToggleSettings foundSetting;
		if (this._toggleSettings.TryGetValue(forFeature, out foundSetting))
		{
			return foundSetting.featureToggleState;
		}
		return FeatureToggleState.NoOverride;
	}

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06000717 RID: 1815 RVA: 0x000171E4 File Offset: 0x000153E4
	// (remove) Token: 0x06000718 RID: 1816 RVA: 0x0001721C File Offset: 0x0001541C
	public event Action<Feature, FeatureToggleState> FeatureToggleStateChanged;

	// Token: 0x06000719 RID: 1817 RVA: 0x00017254 File Offset: 0x00015454
	private void SetFeatureToggleState(Feature forFeature, FeatureToggleState newState)
	{
		ToggleSettings foundSetting;
		if (this._toggleSettings.TryGetValue(forFeature, out foundSetting))
		{
			foundSetting.featureToggleState = newState;
		}
		else
		{
			if (newState == FeatureToggleState.NoOverride)
			{
				return;
			}
			foundSetting = ToggleSettings.InitializeNewSettings(forFeature, newState);
			this._toggleSettings.Add(forFeature, foundSetting);
		}
		PlayerPrefs.SetInt(OptionsMenuSettingSource.GetPlayerPrefsKeyForFeature(forFeature), (int)foundSetting.featureToggleState);
		Action<Feature, FeatureToggleState> featureToggleStateChanged = this.FeatureToggleStateChanged;
		if (featureToggleStateChanged == null)
		{
			return;
		}
		featureToggleStateChanged(forFeature, foundSetting.featureToggleState);
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x000172C0 File Offset: 0x000154C0
	private static string GetPlayerPrefsKeyForFeature(Feature feature)
	{
		string cachedKey;
		if (OptionsMenuSettingSource.FeatureToggleIdCache.TryGetValue(feature, out cachedKey))
		{
			return cachedKey;
		}
		string newKey = string.Format("PlayerPrefsFeatureToggle-{0}", feature);
		OptionsMenuSettingSource.FeatureToggleIdCache.Add(feature, newKey);
		return newKey;
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x000172FC File Offset: 0x000154FC
	private static ToggleSettings GetPlayerPrefsFeatureToggleSettings(Feature feature)
	{
		FeatureToggleState value = (FeatureToggleState)PlayerPrefs.GetInt(OptionsMenuSettingSource.GetPlayerPrefsKeyForFeature(feature), 0);
		return ToggleSettings.InitializeNewSettings(feature, value);
	}

	// Token: 0x040002FC RID: 764
	private readonly Dictionary<Feature, ToggleSettings> _toggleSettings = new Dictionary<Feature, ToggleSettings>();

	// Token: 0x040002FD RID: 765
	private static readonly Dictionary<Feature, string> FeatureToggleIdCache = new Dictionary<Feature, string>();

	// Token: 0x040002FE RID: 766
	private static OptionsMenuSettingSource Instance = null;
}
