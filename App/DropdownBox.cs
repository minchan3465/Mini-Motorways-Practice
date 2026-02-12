using System;
using System.Collections.Generic;
using Factory;
using Motorways.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001CA RID: 458
public class DropdownBox : MonoBehaviour
{
	// Token: 0x06000AC5 RID: 2757 RVA: 0x00023B5E File Offset: 0x00021D5E
	private void Start()
	{
		this.SetDropdownListActive(false);
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00023B68 File Offset: 0x00021D68
	public void PopulateList(List<string> options, int initiallySelectedOption, IScope scope, bool loadAsStringIds = false)
	{
		this._scope = scope;
		this.group.ClearToggles();
		this._buttons.Clear();
		this.selectedOption = initiallySelectedOption;
		DropdownBox.Log.Info("Initialising DropdownBox with {0} options of which option {1} is chosen, which is {2}", new object[]
		{
			options.Count,
			this.selectedOption,
			(this.selectedOption > 0 && this.selectedOption < options.Count) ? options[this.selectedOption] : "Invalid"
		});
		while (this.itemParent.childCount > 0)
		{
			Transform child = this.itemParent.GetChild(0);
			child.SetParent(null);
			UnityEngine.Object.DestroyImmediate(child.gameObject);
		}
		for (int buttonIndex = 0; buttonIndex < options.Count; buttonIndex++)
		{
			string idString = options[buttonIndex];
			TouchToggle newOptionToggle = UnityEngine.Object.Instantiate<TouchToggle>(this.itemTemplate, this.itemParent, false);
			LocalizedTextUI text = newOptionToggle.GetComponentInChildren<LocalizedTextUI>();
			text.startingStringIdString = (loadAsStringIds ? idString : StringId.None.ToString());
			if (loadAsStringIds)
			{
				text.Awake();
				text.HandleParentAllocated(scope);
			}
			else
			{
				text.TextField.text = idString;
			}
			this._buttons.Add(newOptionToggle);
			this.group.RegisterToggle(newOptionToggle);
			if (buttonIndex == this.selectedOption)
			{
				newOptionToggle.IsOn = true;
				this.OnOptionButtonPressed(newOptionToggle, false);
			}
			newOptionToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.OnOptionButtonPressed(newOptionToggle, true);
				}
			});
			newOptionToggle.AddOnSelectedEvent(delegate
			{
				this.OnOptionSelected(newOptionToggle);
			});
			newOptionToggle.name = string.Format("Option {0}: {1}", buttonIndex, idString);
			Navigation nav = newOptionToggle.navigation;
			nav.mode = Navigation.Mode.Explicit;
			if (buttonIndex > 0)
			{
				TouchToggle previousButton = this._buttons[buttonIndex - 1];
				nav.selectOnUp = previousButton;
				Navigation previousNav = previousButton.navigation;
				previousNav.selectOnDown = newOptionToggle;
				previousButton.navigation = previousNav;
			}
			newOptionToggle.navigation = nav;
		}
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06000AC7 RID: 2759 RVA: 0x00023DA1 File Offset: 0x00021FA1
	private bool IsOpen
	{
		get
		{
			return this.dropdownList.activeInHierarchy;
		}
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00023DB0 File Offset: 0x00021FB0
	public void SetDropdownListActive(bool active)
	{
		this.dropdownList.SetActive(active);
		this.headerButton.Set(active, false);
		if (this.selectedOption < 0 || this.selectedOption >= this._buttons.Count)
		{
			this.selectedOption = 0;
		}
		MenuNavigation menuNavigation = this._scope.Get<MenuNavigation>();
		if (active)
		{
			this.SetScrollToCurrentOption();
			if (menuNavigation.GetCurrentFocus() != this._buttons[this.selectedOption])
			{
				menuNavigation.SetNewFocus(this._buttons[this.selectedOption]);
				return;
			}
		}
		else if (menuNavigation.GetCurrentFocus() != this.headerButton)
		{
			menuNavigation.SetNewFocus(this.headerButton);
		}
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x00023E63 File Offset: 0x00022063
	private void SetScrollToCurrentOption()
	{
		this._scrollRect.verticalNormalizedPosition = 1f - (float)this.selectedOption / ((float)this._buttons.Count - 1f);
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x00023E90 File Offset: 0x00022090
	private void OnOptionButtonPressed(TouchToggle button, bool invokeOptionSelected = true)
	{
		this.SetDropdownListActive(false);
		LocalizedTextUI selectedTextUI = button.GetComponentInChildren<LocalizedTextUI>();
		this.selectedElementText.LocString = selectedTextUI.LocString;
		this.selectedElementText.TextField.text = selectedTextUI.TextField.text;
		this.OnOptionSelected(this._buttons.IndexOf(button), invokeOptionSelected);
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x00023EEA File Offset: 0x000220EA
	public void OnOptionSelected(TouchToggle button)
	{
		if (this._scrollToOptionSelected)
		{
			this._scrollRect.verticalNormalizedPosition = 1f - (float)this._buttons.IndexOf(button) / ((float)this._buttons.Count - 1f);
		}
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00023F25 File Offset: 0x00022125
	private void OnOptionSelected(int option, bool invokeOptionSelected)
	{
		this.selectedOption = option;
		if (invokeOptionSelected)
		{
			this.onOptionSelected.Invoke(option);
		}
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00023F3D File Offset: 0x0002213D
	public void SetSelectedOption(int newSelectedOption)
	{
		if (!Diagnostics.Verify(newSelectedOption >= 0 && newSelectedOption < this._buttons.Count, "{0} is an invalid option! Defaulting to zero", newSelectedOption))
		{
			newSelectedOption = 0;
		}
		this.OnOptionButtonPressed(this._buttons[newSelectedOption], false);
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x00023B5E File Offset: 0x00021D5E
	public void DismissDropdown()
	{
		this.SetDropdownListActive(false);
	}

	// Token: 0x040005DD RID: 1501
	public LocalizedTextUI selectedElementText;

	// Token: 0x040005DE RID: 1502
	public GameObject dropdownList;

	// Token: 0x040005DF RID: 1503
	public TouchToggle itemTemplate;

	// Token: 0x040005E0 RID: 1504
	public Transform itemParent;

	// Token: 0x040005E1 RID: 1505
	public int selectedOption;

	// Token: 0x040005E2 RID: 1506
	public TouchToggle headerButton;

	// Token: 0x040005E3 RID: 1507
	public TMP_Dropdown.DropdownEvent onOptionSelected;

	// Token: 0x040005E4 RID: 1508
	public ToggleButtonGroup group;

	// Token: 0x040005E5 RID: 1509
	[SerializeField]
	private ScrollRect _scrollRect;

	// Token: 0x040005E6 RID: 1510
	[SerializeField]
	private bool _scrollToOptionSelected;

	// Token: 0x040005E7 RID: 1511
	private readonly List<TouchToggle> _buttons = new List<TouchToggle>();

	// Token: 0x040005E8 RID: 1512
	private Selectable _oldNavigationTargetDown;

	// Token: 0x040005E9 RID: 1513
	private IScope _scope;

	// Token: 0x040005EA RID: 1514
	private static Diagnostics.Log.Channel Log = Diagnostics.Log.OpenChannel("DropdownBox");
}
