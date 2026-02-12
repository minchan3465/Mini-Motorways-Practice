using System;
using System.Collections.Generic;
using Factory;
using JetBrains.Annotations;
using Motorways;
using Motorways.Themes;
using Popups;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001BF RID: 447
public class ColorblindCustomisePopup : BasePopup
{
	// Token: 0x06000A88 RID: 2696 RVA: 0x00022C58 File Offset: 0x00020E58
	public void Initialise(IScope scope, StringId headerStringId, Action onConfirmed = null)
	{
		this._headerText.SetStringId(scope, headerStringId);
		this._onConfirmed = onConfirmed;
		MotorwaysThemeDatabase themeDatabase = this._appScope.Get<MotorwaysThemeDatabase>();
		Theme currentTheme = themeDatabase.ActiveColorblindTheme;
		List<int> currentSavedIndexes = this._appScope.Get<ActivePlayer>().MotorwaysExtendedUserProfile.PlayerColorblindPaletteIndexes;
		this._newChosenIndexes = new List<int>(currentSavedIndexes);
		for (int groupIndex = 0; groupIndex < currentTheme.buildingColorGroups.Count; groupIndex++)
		{
			this.ChosenColors[groupIndex].Initialise(currentTheme.buildingColorGroups[groupIndex]);
		}
		this.ChosenColors[this._selectedChosenColor].IsSelected = true;
		ColorGroup[] constantColors = themeDatabase.ActiveColorblindColorGroups;
		for (int availableColorIndex = 0; availableColorIndex < constantColors.Length; availableColorIndex++)
		{
			this.AvailableColors[availableColorIndex].Initialise(constantColors[availableColorIndex]);
		}
		foreach (int chosenIndex in currentSavedIndexes)
		{
			this.AvailableColors[chosenIndex].IsChosen = true;
		}
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x00022D80 File Offset: 0x00020F80
	public override void OnPopupClosed()
	{
		base.OnPopupClosed();
		Action onConfirmed = this._onConfirmed;
		if (onConfirmed == null)
		{
			return;
		}
		onConfirmed();
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x00022D98 File Offset: 0x00020F98
	public void ClosePressed()
	{
		this._popupStack.PopPopup(false);
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00022DA8 File Offset: 0x00020FA8
	public override void Reset()
	{
		base.Reset();
		if (this._selectedChosenColor >= 0)
		{
			this.ChosenColors[this._selectedChosenColor].IsSelected = false;
		}
		foreach (AvailableColorButton availableColorButton in this.AvailableColors)
		{
			availableColorButton.IsSelected = false;
			availableColorButton.IsChosen = false;
		}
		this._selectedChosenColor = 0;
		this._previouslySelectedAvailableColor = null;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00022E34 File Offset: 0x00021034
	[UsedImplicitly]
	public void OnSavePressed()
	{
		MotorwaysThemeDatabase motorwaysThemeDatabase = this._appScope.Get<MotorwaysThemeDatabase>();
		this._appScope.Get<ActivePlayer>().MotorwaysExtendedUserProfile.PlayerColorblindPaletteIndexes = this._newChosenIndexes;
		motorwaysThemeDatabase.UpdateColorblindThemesFromActiveUserProfile();
		this._popupStack.PopPopup(false);
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x00022E70 File Offset: 0x00021070
	[UsedImplicitly]
	public void OnColorButtonSelected(int index)
	{
		if (this._selectedChosenColor >= 0)
		{
			this.ChosenColors[this._selectedChosenColor].IsSelected = false;
		}
		this._selectedChosenColor = index;
		if (index < 0)
		{
			return;
		}
		this.ChosenColors[this._selectedChosenColor].IsSelected = true;
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00022EC0 File Offset: 0x000210C0
	[UsedImplicitly]
	public void OnTopColorButtonConfirmed()
	{
		if (this._newChosenIndexes.Count <= 0)
		{
			return;
		}
		this.navigation.SetNewFocus(this.AvailableColors[this._newChosenIndexes[this._selectedChosenColor]].TouchToggle);
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x00022EFD File Offset: 0x000210FD
	[UsedImplicitly]
	public void OnAvailableColorSelected(AvailableColorButton selectedColorButton)
	{
		if (this._previouslySelectedAvailableColor != null)
		{
			this._previouslySelectedAvailableColor.IsSelected = false;
		}
		this._previouslySelectedAvailableColor = selectedColorButton;
		this._previouslySelectedAvailableColor.IsSelected = true;
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00022F2C File Offset: 0x0002112C
	public void OnAvailableColorButtonConfirmed(AvailableColorButton confirmedColorButton)
	{
		if (!confirmedColorButton.TouchToggle.IsOn)
		{
			return;
		}
		if (this._selectedChosenColor >= 0 && this._selectedChosenColor < this.ChosenColors.Count)
		{
			ChosenColorButton selectedChosenButton = this.ChosenColors[this._selectedChosenColor];
			AvailableColorButton currentlyChosenAvailableColor = this.AvailableColors[this._newChosenIndexes[this._selectedChosenColor]];
			if (confirmedColorButton == currentlyChosenAvailableColor)
			{
				this.navigation.SetNewFocus(selectedChosenButton.FocusPoint);
			}
			else if (confirmedColorButton.IsChosen)
			{
				int confirmedChosenIndexPosition = this.GetChosenColorIndexFor(confirmedColorButton);
				if (confirmedChosenIndexPosition != -1)
				{
					this.ChosenColors[confirmedChosenIndexPosition].SwapColorGroupWith(selectedChosenButton);
					int tempIndex = this._newChosenIndexes[confirmedChosenIndexPosition];
					this._newChosenIndexes[confirmedChosenIndexPosition] = this._newChosenIndexes[this._selectedChosenColor];
					this._newChosenIndexes[this._selectedChosenColor] = tempIndex;
				}
				this.navigation.SetNewFocus(selectedChosenButton.FocusPoint);
			}
			else
			{
				currentlyChosenAvailableColor.IsChosen = false;
				confirmedColorButton.IsChosen = true;
				selectedChosenButton.SetColorGroup(confirmedColorButton.ColorGroup);
				this._newChosenIndexes[this._selectedChosenColor] = confirmedColorButton.Index;
				this.navigation.SetNewFocus(selectedChosenButton.FocusPoint);
			}
			confirmedColorButton.IsSelected = false;
			this._previouslySelectedAvailableColor = null;
		}
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0002307C File Offset: 0x0002127C
	private int GetChosenColorIndexFor(AvailableColorButton availableColorButton)
	{
		int confirmedChosenIndexPosition = -1;
		for (int chosenIndexPosition = 0; chosenIndexPosition < this._newChosenIndexes.Count; chosenIndexPosition++)
		{
			if (this._newChosenIndexes[chosenIndexPosition] == availableColorButton.Index)
			{
				confirmedChosenIndexPosition = chosenIndexPosition;
				break;
			}
		}
		return confirmedChosenIndexPosition;
	}

	// Token: 0x0400059B RID: 1435
	[Dependency]
	private PopupStack _popupStack;

	// Token: 0x0400059C RID: 1436
	[SerializeField]
	private LocalizedTextUI _headerText;

	// Token: 0x0400059D RID: 1437
	private Action _onConfirmed;

	// Token: 0x0400059E RID: 1438
	[Dependency]
	private IScope _appScope;

	// Token: 0x0400059F RID: 1439
	[FormerlySerializedAs("CurrentSelectedColors")]
	[SerializeField]
	private List<ChosenColorButton> ChosenColors = new List<ChosenColorButton>();

	// Token: 0x040005A0 RID: 1440
	private const int DefaultSelectedChosenColorIndex = 0;

	// Token: 0x040005A1 RID: 1441
	private int _selectedChosenColor;

	// Token: 0x040005A2 RID: 1442
	[FormerlySerializedAs("SelectableColors")]
	[SerializeField]
	private List<AvailableColorButton> AvailableColors = new List<AvailableColorButton>();

	// Token: 0x040005A3 RID: 1443
	private AvailableColorButton _previouslySelectedAvailableColor;

	// Token: 0x040005A4 RID: 1444
	private List<int> _newChosenIndexes = new List<int>();
}
