using System;
using System.Collections.Generic;
using Motorways;
using UnityEngine;

// Token: 0x02000141 RID: 321
[Serializable]
public class SwapByFeature_ZoomParameters
{
	// Token: 0x06000733 RID: 1843 RVA: 0x000177D0 File Offset: 0x000159D0
	public static explicit operator ZoomParameters(SwapByFeature_ZoomParameters swapByFeature)
	{
		if (swapByFeature.priorityOrder.Count > 0)
		{
			foreach (SwapByFeature_ZoomParameters.PerFeatureToggle featureToggle in swapByFeature.priorityOrder)
			{
				if (FeatureToggle.IsDynamicFeatureEnabled(featureToggle.ifFeatureIsEnabledEnum))
				{
					return featureToggle.useThisValue;
				}
			}
		}
		return swapByFeature.defaultValue;
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x00017848 File Offset: 0x00015A48
	public SwapByFeature_ZoomParameters SetValueToCurrentFeature(ZoomParameters newValue)
	{
		if (this.priorityOrder.Count > 0)
		{
			foreach (SwapByFeature_ZoomParameters.PerFeatureToggle featureToggle in this.priorityOrder)
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

	// Token: 0x06000735 RID: 1845 RVA: 0x000178C0 File Offset: 0x00015AC0
	public void MigrateData(ZoomParameters oldField)
	{
		this.defaultValue = oldField;
	}

	// Token: 0x0400031C RID: 796
	[SerializeField]
	public List<SwapByFeature_ZoomParameters.PerFeatureToggle> priorityOrder = new List<SwapByFeature_ZoomParameters.PerFeatureToggle>();

	// Token: 0x0400031D RID: 797
	[SerializeField]
	public ZoomParameters defaultValue;

	// Token: 0x02000142 RID: 322
	[Serializable]
	public class PerFeatureToggle
	{
		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000737 RID: 1847 RVA: 0x000178DC File Offset: 0x00015ADC
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

		// Token: 0x0400031E RID: 798
		[StringEnumSearch(typeof(Feature))]
		[SerializeField]
		public string ifFeatureIsEnabled;

		// Token: 0x0400031F RID: 799
		private string lastFeatureConversionValue;

		// Token: 0x04000320 RID: 800
		private Feature convertedToFeature;

		// Token: 0x04000321 RID: 801
		[SerializeField]
		public ZoomParameters useThisValue;
	}
}
