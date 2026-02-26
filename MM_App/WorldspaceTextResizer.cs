using System;
using UnityEngine;

// Token: 0x02000271 RID: 625
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class WorldspaceTextResizer : MonoBehaviour
{
	// Token: 0x06000EEC RID: 3820 RVA: 0x00032695 File Offset: 0x00030895
	private void Awake()
	{
		this._rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x000326A3 File Offset: 0x000308A3
	private void Update()
	{
		this._rectTransform.sizeDelta = this.parentRenderer.size - new Vector2(this.horizontalPadding, this.verticalPadding);
	}

	// Token: 0x04000DAB RID: 3499
	public SpriteRenderer parentRenderer;

	// Token: 0x04000DAC RID: 3500
	private RectTransform _rectTransform;

	// Token: 0x04000DAD RID: 3501
	public float verticalPadding;

	// Token: 0x04000DAE RID: 3502
	public float horizontalPadding;
}
