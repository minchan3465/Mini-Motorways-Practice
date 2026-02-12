using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000138 RID: 312
public static class FeatureToggle
{
	// Token: 0x0600071D RID: 1821 RVA: 0x00017330 File Offset: 0x00015530
	static FeatureToggle()
	{
		FeatureToggle.DynamicFeatures = new List<Feature>();
		string[] featureNames = Enum.GetNames(typeof(Feature));
		Array featureValues = Enum.GetValues(typeof(Feature));
		int featureCount = featureNames.Length;
		int maxFeatureValue = 0;
		for (int featureIndex = 0; featureIndex < featureCount; featureIndex++)
		{
			if (!featureNames[featureIndex].StartsWith("Group_"))
			{
				int featureValue = (int)featureValues.GetValue(featureIndex);
				maxFeatureValue = Mathf.Max(featureValue, maxFeatureValue);
				FeatureToggle.DynamicFeatures.Add((Feature)featureValue);
			}
		}
		FeatureToggle.FeatureStateCache = new List<bool>(maxFeatureValue);
		while (FeatureToggle.FeatureStateCache.Count <= maxFeatureValue)
		{
			FeatureToggle.FeatureStateCache.Add(false);
		}
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x000173F0 File Offset: 0x000155F0
	public static void AddSource(IFeatureToggleSettingSource newSource)
	{
		int insertIndex;
		for (insertIndex = 0; insertIndex < FeatureToggle.ToggleSettingSources.Count; insertIndex++)
		{
			if (FeatureToggle.ToggleSettingSources[insertIndex].SourcePriority == newSource.SourcePriority)
			{
				FeatureToggle.ToggleSettingSources[insertIndex].FeatureToggleStateChanged -= FeatureToggle.OnFeatureToggleStateChanged;
				FeatureToggle.ToggleSettingSources.RemoveAt(insertIndex);
				break;
			}
			if (FeatureToggle.ToggleSettingSources[insertIndex].SourcePriority > newSource.SourcePriority)
			{
				break;
			}
		}
		FeatureToggle.ToggleSettingSources.Insert(insertIndex, newSource);
		FeatureToggle.UpdateAllFeatureStates();
		newSource.FeatureToggleStateChanged += FeatureToggle.OnFeatureToggleStateChanged;
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00017490 File Offset: 0x00015690
	public static void RemoveAllSources()
	{
		foreach (IFeatureToggleSettingSource featureToggleSettingSource in FeatureToggle.ToggleSettingSources)
		{
			featureToggleSettingSource.FeatureToggleStateChanged -= FeatureToggle.OnFeatureToggleStateChanged;
		}
		FeatureToggle.ToggleSettingSources.Clear();
		FeatureToggle.UpdateAllFeatureStates();
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x000174FC File Offset: 0x000156FC
	public static bool IsFeatureEnabled(Feature featureToCheck)
	{
		return FeatureToggle.FeatureStateCache[(int)featureToCheck];
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00017509 File Offset: 0x00015709
	public static bool IsFeatureDisabled(Feature featureToCheck)
	{
		return !FeatureToggle.FeatureStateCache[(int)featureToCheck];
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x0001751C File Offset: 0x0001571C
	public static bool IsDynamicFeatureEnabled(Feature featureToCheck)
	{
		using (List<IFeatureToggleSettingSource>.Enumerator enumerator = FeatureToggle.ToggleSettingSources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current.GetFeatureToggleState(featureToCheck))
				{
				case FeatureToggleState.Enabled:
					return true;
				case FeatureToggleState.Disabled:
					return false;
				}
			}
		}
		return false;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x0001758C File Offset: 0x0001578C
	private static void OnFeatureToggleStateChanged(Feature feature, FeatureToggleState state)
	{
		FeatureToggle.FeatureStateCache[(int)feature] = FeatureToggle.IsDynamicFeatureEnabled(feature);
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x000175A0 File Offset: 0x000157A0
	private static void UpdateAllFeatureStates()
	{
		foreach (Feature feature in FeatureToggle.DynamicFeatures)
		{
			FeatureToggle.FeatureStateCache[(int)feature] = FeatureToggle.IsDynamicFeatureEnabled(feature);
		}
	}

	// Token: 0x04000300 RID: 768
	public const string GroupPrefix = "Group_";

	// Token: 0x04000301 RID: 769
	public const string FeatureToggleMenuItemDirectory = "Tools/Feature Toggles/";

	// Token: 0x04000302 RID: 770
	private static readonly List<IFeatureToggleSettingSource> ToggleSettingSources = new List<IFeatureToggleSettingSource>();

	// Token: 0x04000303 RID: 771
	private static readonly List<Feature> DynamicFeatures;

	// Token: 0x04000304 RID: 772
	private static readonly List<bool> FeatureStateCache;

	// Token: 0x04000305 RID: 773
	private static readonly ProfilerMarker Profiler_IsDynamicFeatureEnabled = new ProfilerMarker("FeatureToggle.IsDynamicFeatureEnabled");
}
