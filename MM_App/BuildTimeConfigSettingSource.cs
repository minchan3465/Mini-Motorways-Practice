using System;
using System.Collections.Generic;

// Token: 0x02000135 RID: 309
public class BuildTimeConfigSettingSource : IFeatureToggleSettingSource
{
	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06000702 RID: 1794 RVA: 0x00016EC3 File Offset: 0x000150C3
	public FeatureToggleSettingSourcePriority SourcePriority
	{
		get
		{
			return FeatureToggleSettingSourcePriority.BuildTimeSource;
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00016EC6 File Offset: 0x000150C6
	public BuildTimeConfigSettingSource(IEnvironment environment)
	{
		this._toggleSettings = ToggleSettings.LoadSettingsFromFeatureConfigResource("DefaultConfig", "default environment configuration asset", false);
		this.LoadToggleSettingsFromFiles(environment.FeatureConfigs);
		this.LoadToggleSettingsFromFiles(BuildTimeConfigSettingSource.BuildSystemFeatureConfigs);
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x00016EFC File Offset: 0x000150FC
	private void LoadToggleSettingsFromFiles(List<string> featureConfigFilenames)
	{
		if (featureConfigFilenames == null || featureConfigFilenames.Count <= 0)
		{
			return;
		}
		foreach (string featureConfigFilename in featureConfigFilenames)
		{
			Dictionary<Feature, ToggleSettings> configFeatureSettings = ToggleSettings.LoadSettingsFromFeatureConfigResource(featureConfigFilename, "environment '" + featureConfigFilename + "' configuration asset", false);
			foreach (Feature featureOverride in configFeatureSettings.Keys)
			{
				this._toggleSettings[featureOverride] = configFeatureSettings[featureOverride];
			}
		}
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x00016FBC File Offset: 0x000151BC
	public FeatureToggleState GetFeatureToggleState(Feature forFeature)
	{
		ToggleSettings foundSetting;
		if (this._toggleSettings.TryGetValue(forFeature, out foundSetting))
		{
			return foundSetting.featureToggleState;
		}
		return FeatureToggleState.NoOverride;
	}

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06000706 RID: 1798 RVA: 0x000022F5 File Offset: 0x000004F5
	// (remove) Token: 0x06000707 RID: 1799 RVA: 0x000022F5 File Offset: 0x000004F5
	public event Action<Feature, FeatureToggleState> FeatureToggleStateChanged
	{
		add
		{
		}
		remove
		{
		}
	}

	// Token: 0x040002F6 RID: 758
	private readonly Dictionary<Feature, ToggleSettings> _toggleSettings;

	// Token: 0x040002F7 RID: 759
	private const string DefaultConfigResourceName = "DefaultConfig";

	// Token: 0x040002F8 RID: 760
	public static readonly List<string> BuildSystemFeatureConfigs = new List<string>();
}
