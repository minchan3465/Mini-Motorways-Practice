using System;
using Motorways.Themes;
using Motorways.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020001BA RID: 442
public class AvailableColorButton : MonoBehaviour
{
	// Token: 0x17000266 RID: 614
	// (get) Token: 0x06000A6D RID: 2669 RVA: 0x00022865 File Offset: 0x00020A65
	public int Index
	{
		get
		{
			return this._index;
		}
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06000A6E RID: 2670 RVA: 0x0002286D File Offset: 0x00020A6D
	// (set) Token: 0x06000A6F RID: 2671 RVA: 0x0002287F File Offset: 0x00020A7F
	public bool IsSelected
	{
		get
		{
			return this._selectedIndicator.gameObject.activeSelf;
		}
		set
		{
			this._selectedIndicator.gameObject.SetActive(value);
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06000A70 RID: 2672 RVA: 0x00022892 File Offset: 0x00020A92
	// (set) Token: 0x06000A71 RID: 2673 RVA: 0x000228A4 File Offset: 0x00020AA4
	public bool IsChosen
	{
		get
		{
			return this._chosenIndicator.gameObject.activeSelf;
		}
		set
		{
			this._chosenIndicator.gameObject.SetActive(value);
		}
	}

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06000A72 RID: 2674 RVA: 0x000228B7 File Offset: 0x00020AB7
	public ColorGroup ColorGroup
	{
		get
		{
			return this._colorGroup;
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x000228BF File Offset: 0x00020ABF
	public void Initialise(ColorGroup colorGroup)
	{
		this._colorGroup = colorGroup;
		this._availableColorImage.color = this._colorGroup.GetColor(ThemeComponentGroupTarget.BuildingBase);
		this._index = base.transform.GetSiblingIndex();
	}

	// Token: 0x04000584 RID: 1412
	[SerializeField]
	private Image _availableColorImage;

	// Token: 0x04000585 RID: 1413
	[FormerlySerializedAs("_isChosenIndicator")]
	[SerializeField]
	private Image _chosenIndicator;

	// Token: 0x04000586 RID: 1414
	[SerializeField]
	private Image _selectedIndicator;

	// Token: 0x04000587 RID: 1415
	[FormerlySerializedAs("_touchToggle")]
	public TouchToggle TouchToggle;

	// Token: 0x04000588 RID: 1416
	private ColorGroup _colorGroup;

	// Token: 0x04000589 RID: 1417
	private int _index;
}
