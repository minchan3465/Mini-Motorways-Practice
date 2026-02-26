using System;
using Motorways.Themes;
using Motorways.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BD RID: 445
public class ChosenColorButton : MonoBehaviour
{
	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06000A7A RID: 2682 RVA: 0x00022A9D File Offset: 0x00020C9D
	public int Index
	{
		get
		{
			return this._index;
		}
	}

	// Token: 0x1700026B RID: 619
	// (set) Token: 0x06000A7B RID: 2683 RVA: 0x00022AA5 File Offset: 0x00020CA5
	public bool IsSelected
	{
		set
		{
			this._selectedIndicator.gameObject.SetActive(value);
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06000A7C RID: 2684 RVA: 0x00022AB8 File Offset: 0x00020CB8
	public Selectable FocusPoint
	{
		get
		{
			return this._touchToggle;
		}
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x00022AC0 File Offset: 0x00020CC0
	public void SwapColorGroupWith(ChosenColorButton otherChosenColorButton)
	{
		ColorGroup tempColorGroup = this._colorGroup;
		this.SetColorGroup(otherChosenColorButton._colorGroup);
		otherChosenColorButton.SetColorGroup(tempColorGroup);
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00022AE7 File Offset: 0x00020CE7
	public void Initialise(ColorGroup colorGroup)
	{
		this.SetColorGroup(colorGroup);
		this.IsSelected = false;
		this._index = base.transform.GetSiblingIndex();
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00022B08 File Offset: 0x00020D08
	public void SetColorGroup(ColorGroup colorGroup)
	{
		this._colorGroup = colorGroup;
		this._chosenColorImage.color = this._colorGroup.GetColor(ThemeComponentGroupTarget.BuildingBase);
	}

	// Token: 0x04000592 RID: 1426
	[SerializeField]
	private Image _chosenColorImage;

	// Token: 0x04000593 RID: 1427
	[SerializeField]
	private Image _selectedIndicator;

	// Token: 0x04000594 RID: 1428
	[SerializeField]
	public TouchToggle _touchToggle;

	// Token: 0x04000595 RID: 1429
	private ColorGroup _colorGroup;

	// Token: 0x04000596 RID: 1430
	private int _index;
}
