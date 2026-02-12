using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D7 RID: 471
public class PassiveUpgradeIcon : MonoBehaviour
{
	// Token: 0x06000B3E RID: 2878 RVA: 0x000262B1 File Offset: 0x000244B1
	protected void Start()
	{
		this.Initialise();
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x000262BC File Offset: 0x000244BC
	private void Initialise()
	{
		this._upgradeRectTransform = this._upgradeImage.GetComponent<RectTransform>();
		this._counterRectTransform = this._counterImage.GetComponent<RectTransform>();
		this._upgradeImage.material = new Material(this._upgradeImage.material);
		this._counterImage.material = new Material(this._counterImage.material);
		this._upgradeRadius = this._upgradeImage.material.GetFloat(PassiveUpgradeIcon.CircleRadiusPropertyId);
		this._counterRadius = this._counterImage.material.GetFloat(PassiveUpgradeIcon.CircleRadiusPropertyId);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00026357 File Offset: 0x00024557
	protected void LateUpdate()
	{
		this.UpdateCutoutRect(this._upgradeImage, this._upgradeRectTransform, this._counterRadius, this._counterRectTransform);
		this.UpdateCutoutRect(this._counterImage, this._counterRectTransform, this._upgradeRadius, this._upgradeRectTransform);
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00026398 File Offset: 0x00024598
	private void UpdateCutoutRect(Image imageA, RectTransform transformA, float radiusB, RectTransform transformB)
	{
		Vector3 relativePosition = transformA.InverseTransformPoint(transformB.position) / (transformA.rect.size / 2f);
		relativePosition *= -1f;
		float relativeRadius = transformB.rect.size.x * transformB.lossyScale.x * radiusB / (transformA.rect.size.x * transformA.lossyScale.x);
		imageA.material.SetVector(PassiveUpgradeIcon.CutoutPositionPropertyId, relativePosition);
		imageA.material.SetFloat(PassiveUpgradeIcon.CutoutRadiusPropertyId, relativeRadius);
	}

	// Token: 0x04000660 RID: 1632
	[SerializeField]
	private Image _upgradeImage;

	// Token: 0x04000661 RID: 1633
	private RectTransform _upgradeRectTransform;

	// Token: 0x04000662 RID: 1634
	private float _upgradeRadius;

	// Token: 0x04000663 RID: 1635
	[SerializeField]
	private Image _counterImage;

	// Token: 0x04000664 RID: 1636
	private RectTransform _counterRectTransform;

	// Token: 0x04000665 RID: 1637
	private float _counterRadius;

	// Token: 0x04000666 RID: 1638
	private static readonly int CutoutPositionPropertyId = Shader.PropertyToID("_CutoutPosition");

	// Token: 0x04000667 RID: 1639
	private static readonly int CutoutRadiusPropertyId = Shader.PropertyToID("_CutoutRadius");

	// Token: 0x04000668 RID: 1640
	private static readonly int CircleRadiusPropertyId = Shader.PropertyToID("_CircleSize");
}
