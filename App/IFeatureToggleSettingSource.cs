using System;

// Token: 0x0200013F RID: 319
public interface IFeatureToggleSettingSource
{
	// Token: 0x1700019A RID: 410
	// (get) Token: 0x0600072E RID: 1838
	FeatureToggleSettingSourcePriority SourcePriority { get; }

	// Token: 0x0600072F RID: 1839
	FeatureToggleState GetFeatureToggleState(Feature forFeature);

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06000730 RID: 1840
	// (remove) Token: 0x06000731 RID: 1841
	event Action<Feature, FeatureToggleState> FeatureToggleStateChanged;
}
