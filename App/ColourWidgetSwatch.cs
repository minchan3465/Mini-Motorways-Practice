using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C0 RID: 448
public class ColourWidgetSwatch : MonoBehaviour
{
	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06000A93 RID: 2707 RVA: 0x000230E3 File Offset: 0x000212E3
	// (set) Token: 0x06000A94 RID: 2708 RVA: 0x000230F0 File Offset: 0x000212F0
	public Color SwatchColor
	{
		get
		{
			return this._colourImage.color;
		}
		set
		{
			this._colourImage.color = new Color(value.r, value.g, value.b, 1f);
		}
	}

	// Token: 0x040005A5 RID: 1445
	[SerializeField]
	private Image _colourImage;

	// Token: 0x040005A6 RID: 1446
	public int SwatchSlot;
}
