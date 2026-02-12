using System;
using Motorways.UI;
using TMPro;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class DebugToggleButton : MonoBehaviour
{
	// Token: 0x06000DA2 RID: 3490 RVA: 0x0002D00C File Offset: 0x0002B20C
	public void Initialize(string newDebugOptionName, Feature newFeature, DebugOptionsPage newDebugOptionsPage, ToggleButtonGroup group)
	{
		this.debugOptionName = newDebugOptionName;
		this.text.text = this.debugOptionName;
		this.featureToToggle = newFeature;
		this.debugOptionsPage = newDebugOptionsPage;
		if (group != null)
		{
			group.RegisterToggle(this.touchToggle);
		}
		this._currentState = OptionsMenuSettingSource.GetOptionsMenuFeatureState(this.featureToToggle);
		this.UpdateButtonState();
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x0002D068 File Offset: 0x0002B268
	public void UpdateButtonState()
	{
		bool allAroundSetting = FeatureToggle.IsDynamicFeatureEnabled(this.featureToToggle);
		this.touchToggle.Set(allAroundSetting, false);
		this.toggleFill.SetActive(this.touchToggle.IsOn);
		this.indicator.SetActive(this._currentState > FeatureToggleState.NoOverride);
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0002D0B8 File Offset: 0x0002B2B8
	public void OnClick()
	{
		this._currentState = (this._currentState + 1) % (FeatureToggleState)DebugToggleButton.FeatureToggleStateCount;
		this.debugOptionsPage.SetDebugOptionEnabled(this.debugOptionName.Replace(" ", ""), this._currentState);
		this.UpdateButtonState();
	}

	// Token: 0x040007C0 RID: 1984
	public TextMeshProUGUI text;

	// Token: 0x040007C1 RID: 1985
	public TouchToggle touchToggle;

	// Token: 0x040007C2 RID: 1986
	public GameObject toggleFill;

	// Token: 0x040007C3 RID: 1987
	public GameObject indicator;

	// Token: 0x040007C4 RID: 1988
	public string debugOptionName;

	// Token: 0x040007C5 RID: 1989
	public Feature featureToToggle;

	// Token: 0x040007C6 RID: 1990
	public DebugOptionsPage debugOptionsPage;

	// Token: 0x040007C7 RID: 1991
	private static readonly int FeatureToggleStateCount = typeof(FeatureToggleState).GetEnumNames().Length;

	// Token: 0x040007C8 RID: 1992
	private FeatureToggleState _currentState;
}
