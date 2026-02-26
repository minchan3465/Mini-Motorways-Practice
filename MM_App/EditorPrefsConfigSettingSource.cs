using System;
using System.Collections.Generic;

// Token: 0x02000136 RID: 310
public class EditorPrefsConfigSettingSource : IFeatureToggleSettingSource
{
	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06000709 RID: 1801 RVA: 0x00016FED File Offset: 0x000151ED
	public FeatureToggleSettingSourcePriority SourcePriority
	{
		get
		{
			return FeatureToggleSettingSourcePriority.EditorPrefsSource;
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x00016FF0 File Offset: 0x000151F0
	public EditorPrefsConfigSettingSource()
	{
		EditorPrefsConfigSettingSource.Instance = this;
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00016FFE File Offset: 0x000151FE
	public FeatureToggleState GetFeatureToggleState(Feature forFeature)
	{
		return EditorPrefsConfigSettingSource.GetEditorPrefsFeatureState(forFeature);
	}

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x0600070C RID: 1804 RVA: 0x00017008 File Offset: 0x00015208
	// (remove) Token: 0x0600070D RID: 1805 RVA: 0x00017040 File Offset: 0x00015240
	public event Action<Feature, FeatureToggleState> FeatureToggleStateChanged;

	// Token: 0x0600070E RID: 1806 RVA: 0x00017075 File Offset: 0x00015275
	public static void SetEditorPrefsFeatureState(Feature forFeature, FeatureToggleState newState)
	{
		EditorPrefsConfigSettingSource instance = EditorPrefsConfigSettingSource.Instance;
		if (instance == null)
		{
			return;
		}
		Action<Feature, FeatureToggleState> featureToggleStateChanged = instance.FeatureToggleStateChanged;
		if (featureToggleStateChanged == null)
		{
			return;
		}
		featureToggleStateChanged(forFeature, newState);
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x0000222C File Offset: 0x0000042C
	public static FeatureToggleState GetEditorPrefsFeatureState(Feature feature)
	{
		return FeatureToggleState.NoOverride;
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00017094 File Offset: 0x00015294
	private static string GetEditorPrefsKeyForFeature(Feature feature)
	{
		string cachedKey;
		if (EditorPrefsConfigSettingSource.FeatureToggleIdCache.TryGetValue(feature, out cachedKey))
		{
			return cachedKey;
		}
		string newKey = string.Format("UnityEditorFeatureToggle-{0}", feature);
		EditorPrefsConfigSettingSource.FeatureToggleIdCache.Add(feature, newKey);
		return newKey;
	}

	// Token: 0x040002F9 RID: 761
	private static readonly Dictionary<Feature, string> FeatureToggleIdCache = new Dictionary<Feature, string>();

	// Token: 0x040002FA RID: 762
	private static EditorPrefsConfigSettingSource Instance;
}
