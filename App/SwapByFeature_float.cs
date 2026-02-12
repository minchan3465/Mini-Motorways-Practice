using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013A RID: 314
[Serializable]
public class SwapByFeature_float
{
	// Token: 0x06000726 RID: 1830 RVA: 0x00017620 File Offset: 0x00015820
	public static explicit operator float(SwapByFeature_float swapByFeature)
	{
		if (swapByFeature.priorityOrder.Count > 0)
		{
			foreach (SwapByFeature_float.PerFeatureToggle featureToggle in swapByFeature.priorityOrder)
			{
				if (FeatureToggle.IsDynamicFeatureEnabled(featureToggle.ifFeatureIsEnabledEnum))
				{
					return featureToggle.useThisValue;
				}
			}
		}
		return swapByFeature.defaultValue;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00017698 File Offset: 0x00015898
	public SwapByFeature_float SetValueToCurrentFeature(float newValue)
	{
		if (this.priorityOrder.Count > 0)
		{
			foreach (SwapByFeature_float.PerFeatureToggle featureToggle in this.priorityOrder)
			{
				if (FeatureToggle.IsDynamicFeatureEnabled(featureToggle.ifFeatureIsEnabledEnum))
				{
					featureToggle.useThisValue = newValue;
				}
			}
		}
		this.defaultValue = newValue;
		return this;
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00017710 File Offset: 0x00015910
	public void MigrateData(float oldField)
	{
		this.defaultValue = oldField;
	}

	// Token: 0x04000308 RID: 776
	[SerializeField]
	public List<SwapByFeature_float.PerFeatureToggle> priorityOrder = new List<SwapByFeature_float.PerFeatureToggle>();

	// Token: 0x04000309 RID: 777
	[SerializeField]
	public float defaultValue;

	// Token: 0x0200013B RID: 315
	[Serializable]
	public class PerFeatureToggle
	{
		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600072A RID: 1834 RVA: 0x0001772C File Offset: 0x0001592C
		public Feature ifFeatureIsEnabledEnum
		{
			get
			{
				if (this.ifFeatureIsEnabled != this.lastFeatureConversionValue)
				{
					this.lastFeatureConversionValue = this.ifFeatureIsEnabled;
					if (!Enum.TryParse<Feature>(this.lastFeatureConversionValue, out this.convertedToFeature))
					{
						this.convertedToFeature = Feature.NotSelected;
					}
				}
				return this.convertedToFeature;
			}
		}

		// Token: 0x0400030A RID: 778
		[StringEnumSearch(typeof(Feature))]
		[SerializeField]
		public string ifFeatureIsEnabled;

		// Token: 0x0400030B RID: 779
		private string lastFeatureConversionValue;

		// Token: 0x0400030C RID: 780
		private Feature convertedToFeature;

		// Token: 0x0400030D RID: 781
		[SerializeField]
		public float useThisValue;
	}
}
