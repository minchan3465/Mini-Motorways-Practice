using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000145 RID: 325
[Serializable]
public class SwapByFeature<UnderlyingType>
{
	// Token: 0x0600073F RID: 1855 RVA: 0x00017AD4 File Offset: 0x00015CD4
	public static explicit operator UnderlyingType(SwapByFeature<UnderlyingType> swapByFeature)
	{
		if (swapByFeature.priorityOrder.Count > 0)
		{
			foreach (SwapByFeature<UnderlyingType>.PerFeatureToggle featureToggle in swapByFeature.priorityOrder)
			{
				if (FeatureToggle.IsDynamicFeatureEnabled(featureToggle.ifFeatureIsEnabledEnum))
				{
					return featureToggle.useThisValue;
				}
			}
		}
		return swapByFeature.defaultValue;
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x00017B4C File Offset: 0x00015D4C
	public SwapByFeature<UnderlyingType> SetValueToCurrentFeature(UnderlyingType newValue)
	{
		if (this.priorityOrder.Count > 0)
		{
			foreach (SwapByFeature<UnderlyingType>.PerFeatureToggle featureToggle in this.priorityOrder)
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

	// Token: 0x06000741 RID: 1857 RVA: 0x00017BC4 File Offset: 0x00015DC4
	public void MigrateData(UnderlyingType oldField)
	{
		this.defaultValue = oldField;
	}

	// Token: 0x04000329 RID: 809
	[SerializeField]
	public List<SwapByFeature<UnderlyingType>.PerFeatureToggle> priorityOrder = new List<SwapByFeature<UnderlyingType>.PerFeatureToggle>();

	// Token: 0x0400032A RID: 810
	[SerializeField]
	public UnderlyingType defaultValue;

	// Token: 0x02000146 RID: 326
	[Serializable]
	public class PerFeatureToggle
	{
		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x00017BE0 File Offset: 0x00015DE0
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

		// Token: 0x0400032B RID: 811
		[StringEnumSearch(typeof(Feature))]
		[SerializeField]
		public string ifFeatureIsEnabled;

		// Token: 0x0400032C RID: 812
		private string lastFeatureConversionValue;

		// Token: 0x0400032D RID: 813
		private Feature convertedToFeature;

		// Token: 0x0400032E RID: 814
		[SerializeField]
		public UnderlyingType useThisValue;
	}
}
