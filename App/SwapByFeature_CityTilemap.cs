using System;
using System.Collections.Generic;
using Motorways;
using UnityEngine;

// Token: 0x02000143 RID: 323
[Serializable]
public class SwapByFeature_CityTilemap
{
	// Token: 0x06000739 RID: 1849 RVA: 0x00017928 File Offset: 0x00015B28
	public static explicit operator CityTilemap(SwapByFeature_CityTilemap swapByFeature)
	{
		CityTilemap chosenValue = swapByFeature.defaultValue;
		int newIndex = -1;
		if (swapByFeature.priorityOrder.Count > 0)
		{
			for (int priorityIndex = 0; priorityIndex < swapByFeature.priorityOrder.Count; priorityIndex++)
			{
				SwapByFeature_CityTilemap.PerFeatureToggle featureToggle = swapByFeature.priorityOrder[priorityIndex];
				if (newIndex == -1 && FeatureToggle.IsDynamicFeatureEnabled(featureToggle.ifFeatureIsEnabledEnum))
				{
					chosenValue = featureToggle.useThisValue;
					newIndex = priorityIndex;
					break;
				}
			}
		}
		if (newIndex != swapByFeature.lastChosenIndex)
		{
			if (newIndex != -1)
			{
				swapByFeature.defaultValue.OnNotChosen();
			}
			for (int priorityIndex2 = 0; priorityIndex2 < swapByFeature.priorityOrder.Count; priorityIndex2++)
			{
				if (priorityIndex2 != newIndex)
				{
					swapByFeature.priorityOrder[priorityIndex2].useThisValue.OnNotChosen();
				}
			}
			if (chosenValue != null)
			{
				chosenValue.OnChosen();
			}
			swapByFeature.lastChosenIndex = newIndex;
		}
		return chosenValue;
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x000179EC File Offset: 0x00015BEC
	public SwapByFeature_CityTilemap SetValueToCurrentFeature(CityTilemap newValue)
	{
		if (this.priorityOrder.Count > 0)
		{
			foreach (SwapByFeature_CityTilemap.PerFeatureToggle featureToggle in this.priorityOrder)
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

	// Token: 0x0600073B RID: 1851 RVA: 0x00017A64 File Offset: 0x00015C64
	public void MigrateData(CityTilemap oldField)
	{
		this.defaultValue = oldField;
	}

	// Token: 0x04000322 RID: 802
	[SerializeField]
	public List<SwapByFeature_CityTilemap.PerFeatureToggle> priorityOrder = new List<SwapByFeature_CityTilemap.PerFeatureToggle>();

	// Token: 0x04000323 RID: 803
	[SerializeField]
	public CityTilemap defaultValue;

	// Token: 0x04000324 RID: 804
	private int lastChosenIndex = -2;

	// Token: 0x02000144 RID: 324
	[Serializable]
	public class PerFeatureToggle
	{
		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600073D RID: 1853 RVA: 0x00017A88 File Offset: 0x00015C88
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

		// Token: 0x04000325 RID: 805
		[SerializeField]
		[StringEnumSearch(typeof(Feature))]
		public string ifFeatureIsEnabled;

		// Token: 0x04000326 RID: 806
		private string lastFeatureConversionValue;

		// Token: 0x04000327 RID: 807
		private Feature convertedToFeature;

		// Token: 0x04000328 RID: 808
		[SerializeField]
		public CityTilemap useThisValue;
	}
}
