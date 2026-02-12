using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C1 RID: 449
[ExecuteAlways]
[RequireComponent(typeof(ContentSizeFitter))]
public class ContentSizeFitterMax : MonoBehaviour
{
	// Token: 0x06000A96 RID: 2710 RVA: 0x00023119 File Offset: 0x00021319
	public void OnEnable()
	{
		this._rect = base.GetComponent<RectTransform>();
		this._fitter = base.GetComponent<ContentSizeFitter>();
		this._layout = base.GetComponent<ILayoutElement>();
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x0002313F File Offset: 0x0002133F
	public void Update()
	{
		if (this._layout.preferredWidth > this.maxWidth)
		{
			this._fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			this._rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.maxWidth);
			return;
		}
		this._fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0002317F File Offset: 0x0002137F
	public void OnValidate()
	{
		this.OnEnable();
	}

	// Token: 0x040005A7 RID: 1447
	public float maxWidth;

	// Token: 0x040005A8 RID: 1448
	private RectTransform _rect;

	// Token: 0x040005A9 RID: 1449
	private ContentSizeFitter _fitter;

	// Token: 0x040005AA RID: 1450
	private ILayoutElement _layout;
}
