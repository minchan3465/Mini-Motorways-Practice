using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000149 RID: 329
public class ToggleSettings
{
	// Token: 0x0600074B RID: 1867 RVA: 0x00017DF0 File Offset: 0x00015FF0
	public bool CanBeRemoved()
	{
		return this.featureToggleState == FeatureToggleState.NoOverride && this.buildFlagToggleSettings.Count == 0;
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00017E0C File Offset: 0x0001600C
	public ToggleSettings Duplicate()
	{
		ToggleSettings newToggleSettings = new ToggleSettings();
		newToggleSettings.feature = this.feature;
		newToggleSettings.featureToggleState = this.featureToggleState;
		newToggleSettings.buildFlagToggleSettings = new List<ToggleSettings.BuildFlagToggleSetting>(this.buildFlagToggleSettings.Count);
		foreach (ToggleSettings.BuildFlagToggleSetting currentSetting in this.buildFlagToggleSettings)
		{
			newToggleSettings.buildFlagToggleSettings.Add(currentSetting.Duplicate());
		}
		return newToggleSettings;
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00017EA0 File Offset: 0x000160A0
	[NotNull]
	public static ToggleSettings InitializeNewSettings(Feature forFeature, FeatureToggleState newToggleState)
	{
		return new ToggleSettings
		{
			feature = forFeature,
			featureToggleState = newToggleState
		};
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00017EB8 File Offset: 0x000160B8
	[NotNull]
	public static Dictionary<Feature, ToggleSettings> LoadSettingsFromFeatureConfigResource(string featureConfigResourceName, string configurationNameForLogging, bool errorOnFeatureNotFound = false)
	{
		string configPath = string.Format("{0}{1}{2}", "FeatureToggleConfigs", Path.DirectorySeparatorChar, featureConfigResourceName);
		TextAsset configAsset = Resources.Load(configPath, typeof(TextAsset)) as TextAsset;
		if (configAsset == null)
		{
			configPath = string.Format("{0}{1}BuildTimeConfigs{2}{3}", new object[]
			{
				"FeatureToggleConfigs",
				Path.DirectorySeparatorChar,
				Path.DirectorySeparatorChar,
				featureConfigResourceName
			});
			configAsset = (Resources.Load(configPath, typeof(TextAsset)) as TextAsset);
		}
		if (Diagnostics.Verify(configAsset != null, "Can't find the {0} in the resources folder!  Attempted path {1}.", configurationNameForLogging, configPath))
		{
			return ToggleSettings.LoadSettingsFromJsonString(configAsset.text, configurationNameForLogging, errorOnFeatureNotFound);
		}
		return new Dictionary<Feature, ToggleSettings>();
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x00017F74 File Offset: 0x00016174
	[NotNull]
	private static Dictionary<Feature, ToggleSettings> LoadSettingsFromJsonString(string toggleSettingsJson, string configurationNameForLogging, bool errorOnFeatureNotFound = false)
	{
		Dictionary<Feature, ToggleSettings> settingsDictionary = new Dictionary<Feature, ToggleSettings>();
		JSON.Dictionary branchConfigJson = JSON.LoadFromString(toggleSettingsJson) as JSON.Dictionary;
		if (!Diagnostics.Verify(branchConfigJson != null, "Failed to parse JSON from the {0}.", configurationNameForLogging))
		{
			return settingsDictionary;
		}
		JSON.Array featureToggles = branchConfigJson.GetArray("FeatureToggles");
		if (Diagnostics.Verify(featureToggles != null, "Couldn't find the feature toggle collection!"))
		{
			for (int featureToggleIndex = 0; featureToggleIndex < featureToggles.Count; featureToggleIndex++)
			{
				JSON.Dictionary currentFeatureToggle = featureToggles.GetDictionary(featureToggleIndex);
				if (Diagnostics.Verify(currentFeatureToggle != null, "Couldn't parse a dictionary out of index {0} in the feature toggle collection.", featureToggleIndex))
				{
					string featureString = currentFeatureToggle.GetString("Feature");
					Feature featureEnum = Feature.NotSelected;
					bool flag = !string.IsNullOrEmpty(featureString) && Enum.TryParse<Feature>(featureString, out featureEnum);
					Diagnostics.Verify(flag || !errorOnFeatureNotFound, "Failed to parse feature name from string {0}!", featureString);
					if (flag)
					{
						string toggleStateString = currentFeatureToggle.GetString("ToggleState");
						FeatureToggleState toggleState = FeatureToggleState.NoOverride;
						Diagnostics.Verify(!string.IsNullOrEmpty(toggleStateString) && Enum.TryParse<FeatureToggleState>(toggleStateString, out toggleState), "Failed to parse feature toggle state for feature {0} with value {1}!", featureString, toggleStateString);
						ToggleSettings toggleSettings = ToggleSettings.InitializeNewSettings(featureEnum, toggleState);
						if (currentFeatureToggle.ContainsKey("BuildFlags"))
						{
							JSON.Array buildFlagArray = currentFeatureToggle.GetArray("BuildFlags");
							for (int buildFlagIndex = 0; buildFlagIndex < buildFlagArray.Count; buildFlagIndex++)
							{
								ToggleSettings.BuildFlagToggleSetting newBuildFlagSetting = ToggleSettings.BuildFlagToggleSetting.LoadFromRootJson(buildFlagArray.GetDictionary(buildFlagIndex), configurationNameForLogging);
								if (newBuildFlagSetting != null)
								{
									toggleSettings.buildFlagToggleSettings.Add(newBuildFlagSetting);
								}
							}
						}
						settingsDictionary.Add(toggleSettings.feature, toggleSettings);
					}
				}
			}
		}
		return settingsDictionary;
	}

	// Token: 0x04000336 RID: 822
	public Feature feature;

	// Token: 0x04000337 RID: 823
	public FeatureToggleState featureToggleState;

	// Token: 0x04000338 RID: 824
	public List<ToggleSettings.BuildFlagToggleSetting> buildFlagToggleSettings = new List<ToggleSettings.BuildFlagToggleSetting>();

	// Token: 0x04000339 RID: 825
	private const string FeatureToggleKey = "FeatureToggles";

	// Token: 0x0400033A RID: 826
	private const string FeatureNameKey = "Feature";

	// Token: 0x0400033B RID: 827
	private const string ToggleStateKey = "ToggleState";

	// Token: 0x0400033C RID: 828
	private const string BuildFlagsKey = "BuildFlags";

	// Token: 0x0200014A RID: 330
	public class BuildFlagToggleSetting
	{
		// Token: 0x06000751 RID: 1873 RVA: 0x000180FC File Offset: 0x000162FC
		public static ToggleSettings.BuildFlagToggleSetting LoadFromRootJson(JSON.Dictionary rootDictionary, string configurationNameForLogging)
		{
			ToggleSettings.BuildFlagToggleSetting newBuildFlagSetting = new ToggleSettings.BuildFlagToggleSetting();
			if (rootDictionary.ContainsKey("BuildFlag"))
			{
				newBuildFlagSetting.BuildFlag = rootDictionary.GetString("BuildFlag");
			}
			if (rootDictionary.ContainsKey("FlagState"))
			{
				string useCaseString = rootDictionary.GetString("FlagState");
				if (!Diagnostics.Verify(Enum.TryParse<ToggleSettings.BuildFlagToggleSetting.BuildFlagState>(useCaseString, out newBuildFlagSetting.FlagState), "Failed to parse use case {0} for configuration {1}.", useCaseString, configurationNameForLogging))
				{
					return null;
				}
			}
			if (rootDictionary.ContainsKey("BuildAction"))
			{
				string buildActionString = rootDictionary.GetString("BuildAction");
				if (!Diagnostics.Verify(Enum.TryParse<ToggleSettings.BuildFlagToggleSetting.BuildFlagAction>(buildActionString, out newBuildFlagSetting.BuildAction), "Failed to parse build action {0} for configuration {1}.", buildActionString, configurationNameForLogging))
				{
					return null;
				}
			}
			return newBuildFlagSetting;
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00018198 File Offset: 0x00016398
		public Dictionary<string, object> GenerateJsonDictionaryForSetting()
		{
			return new Dictionary<string, object>
			{
				{
					"BuildFlag",
					this.BuildFlag.ToString()
				},
				{
					"FlagState",
					this.FlagState.ToString()
				},
				{
					"BuildAction",
					this.BuildAction.ToString()
				}
			};
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x000181F8 File Offset: 0x000163F8
		public ToggleSettings.BuildFlagToggleSetting Duplicate()
		{
			return new ToggleSettings.BuildFlagToggleSetting
			{
				BuildFlag = this.BuildFlag,
				FlagState = this.FlagState,
				BuildAction = this.BuildAction
			};
		}

		// Token: 0x0400033D RID: 829
		public string BuildFlag;

		// Token: 0x0400033E RID: 830
		public ToggleSettings.BuildFlagToggleSetting.BuildFlagState FlagState;

		// Token: 0x0400033F RID: 831
		public ToggleSettings.BuildFlagToggleSetting.BuildFlagAction BuildAction;

		// Token: 0x0200014B RID: 331
		public enum BuildFlagState
		{
			// Token: 0x04000341 RID: 833
			IsSet,
			// Token: 0x04000342 RID: 834
			IsNotSet
		}

		// Token: 0x0200014C RID: 332
		public enum BuildFlagAction
		{
			// Token: 0x04000344 RID: 836
			CompileOut,
			// Token: 0x04000345 RID: 837
			HideInOptions,
			// Token: 0x04000346 RID: 838
			ShowInOptions,
			// Token: 0x04000347 RID: 839
			DefaultToEnabled,
			// Token: 0x04000348 RID: 840
			DefaultToDisabled
		}
	}
}
