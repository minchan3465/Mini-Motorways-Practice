using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000147 RID: 327
[Serializable]
public class SwapByFeatureWithObserver<UnderlyingType> where UnderlyingType : IFeatureSwapObserver
{
	// Token: 0x06000745 RID: 1861 RVA: 0x00017C2C File Offset: 0x00015E2C
	public static explicit operator UnderlyingType(SwapByFeatureWithObserver<UnderlyingType> swapByFeature)
	{
		UnderlyingType chosenValue = swapByFeature.defaultValue;
		int newIndex = -1;
		if (swapByFeature.priorityOrder.Count > 0)
		{
			for (int priorityIndex = 0; priorityIndex < swapByFeature.priorityOrder.Count; priorityIndex++)
			{
				SwapByFeatureWithObserver<UnderlyingType>.PerFeatureToggle featureToggle = swapByFeature.priorityOrder[priorityIndex];
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

	// Token: 0x06000746 RID: 1862 RVA: 0x00017D08 File Offset: 0x00015F08
	public SwapByFeatureWithObserver<UnderlyingType> SetValueToCurrentFeature(UnderlyingType newValue)
	{
		if (this.priorityOrder.Count > 0)
		{
			foreach (SwapByFeatureWithObserver<UnderlyingType>.PerFeatureToggle featureToggle in this.priorityOrder)
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

	// Token: 0x06000747 RID: 1863 RVA: 0x00017D80 File Offset: 0x00015F80
	public void MigrateData(UnderlyingType oldField)
	{
		this.defaultValue = oldField;
	}

	// Token: 0x0400032F RID: 815
	[SerializeField]
	public List<SwapByFeatureWithObserver<UnderlyingType>.PerFeatureToggle> priorityOrder = new List<SwapByFeatureWithObserver<UnderlyingType>.PerFeatureToggle>();

	// Token: 0x04000330 RID: 816
	[SerializeField]
	public UnderlyingType defaultValue;

	// Token: 0x04000331 RID: 817
	private int lastChosenIndex = -2;

	// Token: 0x02000148 RID: 328
	[Serializable]
	public class PerFeatureToggle
	{
		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x00017DA4 File Offset: 0x00015FA4
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

		// Token: 0x04000332 RID: 818
		[SerializeField]
		[StringEnumSearch(typeof(Feature))]
		public string ifFeatureIsEnabled;

		// Token: 0x04000333 RID: 819
		private string lastFeatureConversionValue;

		// Token: 0x04000334 RID: 820
		private Feature convertedToFeature;

		// Token: 0x04000335 RID: 821
		[SerializeField]
		public UnderlyingType useThisValue;
	}
}
