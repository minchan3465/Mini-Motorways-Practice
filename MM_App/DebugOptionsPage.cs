using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200023F RID: 575
public class DebugOptionsPage : MonoBehaviour
{
	// Token: 0x06000D9D RID: 3485 RVA: 0x0002CDCE File Offset: 0x0002AFCE
	public void InitializeButtons()
	{
		if (this.buttonPanel.childCount == 0)
		{
			this.SetupButtons();
		}
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0002CDE4 File Offset: 0x0002AFE4
	public static string CapitalsToSpacePlusCaps(string originalString)
	{
		string newString = "";
		foreach (char currentChar in originalString)
		{
			if (char.IsUpper(currentChar) && newString.Length > 0)
			{
				newString += " ";
			}
			newString += currentChar.ToString();
		}
		return newString;
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x0002CE40 File Offset: 0x0002B040
	private void SetupButtons()
	{
		this.debugButtons.Clear();
		if (FeatureToggle.IsFeatureEnabled(Feature.OptionsDebugMenu))
		{
			Array values = Enum.GetValues(typeof(Feature));
			this.firstDebugButton = null;
			bool inHiddenGroup = false;
			foreach (object obj in values)
			{
				Feature currentFeature = (Feature)obj;
				if (currentFeature == Feature.Group_Hidden)
				{
					inHiddenGroup = true;
				}
				else if (currentFeature.ToString().StartsWith("Group_"))
				{
					inHiddenGroup = false;
					DebugOptionHeader debugOptionHeader = UnityEngine.Object.Instantiate<DebugOptionHeader>(this.debugOptionHeaderPrefab);
					string headerName = DebugOptionsPage.CapitalsToSpacePlusCaps(currentFeature.ToString().Substring("Group_".Length));
					debugOptionHeader.Initialize(headerName);
					debugOptionHeader.transform.SetParent(this.buttonPanel);
					debugOptionHeader.transform.localScale = Vector3.one;
				}
				else if (!inHiddenGroup)
				{
					DebugToggleButton newButton = UnityEngine.Object.Instantiate<DebugToggleButton>(this.debugToggleButtonPrefab);
					newButton.Initialize(DebugOptionsPage.CapitalsToSpacePlusCaps(currentFeature.ToString()), currentFeature, this, null);
					newButton.transform.SetParent(this.buttonPanel);
					newButton.transform.localScale = Vector3.one;
					this.firstDebugButton = (this.firstDebugButton ?? newButton.GetComponent<Selectable>());
					this.debugButtons.Add(newButton);
				}
			}
		}
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x0002CFC0 File Offset: 0x0002B1C0
	public void SetDebugOptionEnabled(string optionName, FeatureToggleState newState)
	{
		Feature parsedFeature;
		if (!optionName.StartsWith("Group_") && Diagnostics.Verify(Enum.TryParse<Feature>(optionName, out parsedFeature), "Failed to parse enum from string {0}.", optionName))
		{
			OptionsMenuSettingSource.SetOptionsMenuFeatureState(parsedFeature, newState);
		}
	}

	// Token: 0x040007BB RID: 1979
	public RectTransform buttonPanel;

	// Token: 0x040007BC RID: 1980
	public DebugToggleButton debugToggleButtonPrefab;

	// Token: 0x040007BD RID: 1981
	public DebugOptionHeader debugOptionHeaderPrefab;

	// Token: 0x040007BE RID: 1982
	private Selectable firstDebugButton;

	// Token: 0x040007BF RID: 1983
	private List<DebugToggleButton> debugButtons = new List<DebugToggleButton>();
}
