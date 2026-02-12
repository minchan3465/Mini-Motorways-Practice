using System;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class EnableByFeatureToggle : MonoBehaviour
{
	// Token: 0x06000700 RID: 1792 RVA: 0x00016E3C File Offset: 0x0001503C
	protected void OnEnable()
	{
		Feature featureToggle;
		if (Enum.TryParse<Feature>(this._ifFeatureIsEnabled, out featureToggle))
		{
			bool isEnabled;
			if (FeatureToggle.IsFeatureEnabled(featureToggle))
			{
				isEnabled = this._isEnabledFromFeature;
			}
			else
			{
				isEnabled = !this._isEnabledFromFeature;
			}
			GameObject[] targets = this._targets;
			for (int i = 0; i < targets.Length; i++)
			{
				targets[i].SetActive(isEnabled);
			}
		}
	}

	// Token: 0x0400029B RID: 667
	[SerializeField]
	[StringEnumSearch(typeof(Feature))]
	private string _ifFeatureIsEnabled = Feature.OptionsDebugMenu.ToString();

	// Token: 0x0400029C RID: 668
	[SerializeField]
	private bool _isEnabledFromFeature = true;

	// Token: 0x0400029D RID: 669
	[SerializeField]
	private GameObject[] _targets;
}
