using System;
using System.Collections.Generic;
using Factory;
using Motorways;
using Motorways.Themes;
using Popups;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BE RID: 446
public class ColorblindCustomisationPanel : MonoBehaviour
{
	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06000A81 RID: 2689 RVA: 0x00022B28 File Offset: 0x00020D28
	// (remove) Token: 0x06000A82 RID: 2690 RVA: 0x00022B60 File Offset: 0x00020D60
	public event Action onUpdated;

	// Token: 0x06000A83 RID: 2691 RVA: 0x00022B95 File Offset: 0x00020D95
	public void Initialise(IScope scope, PopupStack popupStack)
	{
		this._appScope = scope;
		this._popupStack = popupStack;
		this.BuildVisualPanel();
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00022BAC File Offset: 0x00020DAC
	public void BuildVisualPanel()
	{
		Theme activeColorblindTheme = this._appScope.Get<MotorwaysThemeDatabase>().ActiveColorblindTheme;
		for (int groupIndex = 0; groupIndex < MotorwaysThemeDatabase.MAX_THEME_COLOR_GROUPS; groupIndex++)
		{
			this.ColorDisplays[groupIndex].color = activeColorblindTheme.buildingColorGroups[groupIndex].GetColor(ThemeComponentGroupTarget.BuildingBase);
		}
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00022BFD File Offset: 0x00020DFD
	public void OnPopupHidden()
	{
		this.BuildVisualPanel();
		Action action = this.onUpdated;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x00022C15 File Offset: 0x00020E15
	public void OnCustomisePressed()
	{
		this._popupStack.PushPopup<ColorblindCustomisePopup>(0f, false).Initialise(this._appScope, StringId.Colorblind_Popup_Description, new Action(this.OnPopupHidden));
	}

	// Token: 0x04000598 RID: 1432
	[SerializeField]
	private List<Image> ColorDisplays = new List<Image>();

	// Token: 0x04000599 RID: 1433
	[Dependency]
	private IScope _appScope;

	// Token: 0x0400059A RID: 1434
	[Dependency]
	private PopupStack _popupStack;
}
