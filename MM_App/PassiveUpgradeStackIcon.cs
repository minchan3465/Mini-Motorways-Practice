using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D8 RID: 472
public class PassiveUpgradeStackIcon : MonoBehaviour
{
	// Token: 0x06000B44 RID: 2884 RVA: 0x00026480 File Offset: 0x00024680
	public void AddIcon(UpgradeIcon icon)
	{
		icon.fillRenderer.material = new Material(this._passiveMaterial);
		this._iconImages.Add(icon.fillRenderer);
		this._iconRectTransforms.Add(icon.fillRenderer.GetComponent<RectTransform>());
		if (this._circleRadius < 0f)
		{
			this._circleRadius = icon.fillRenderer.material.GetFloat(PassiveUpgradeStackIcon.CircleRadiusPropertyId);
		}
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x000264F4 File Offset: 0x000246F4
	public void RemoveIcon(UpgradeIcon icon)
	{
		int index = this._iconImages.IndexOf(icon.fillRenderer);
		if (index >= 0)
		{
			this._iconImages.RemoveAt(index);
			this._iconRectTransforms.RemoveAt(index);
		}
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00026530 File Offset: 0x00024730
	private void LateUpdate()
	{
		if (this._iconImages.Count > 1)
		{
			Image currentImage = this._iconImages[0];
			RectTransform currentTransform = this._iconRectTransforms[0];
			Image nextImage = this._iconImages[1];
			RectTransform nextTransform = this._iconRectTransforms[1];
			this.UpdateCutoutRect(currentImage, currentTransform, nextTransform, PassiveUpgradeStackIcon.BackCutoutPositionPropertyId, PassiveUpgradeStackIcon.BackCutoutRadiusPropertyId);
			int iconIndex = 1;
			RectTransform previousTransform;
			while (iconIndex + 1 < this._iconImages.Count)
			{
				previousTransform = currentTransform;
				currentImage = nextImage;
				currentTransform = nextTransform;
				nextImage = this._iconImages[iconIndex + 1];
				nextTransform = this._iconRectTransforms[iconIndex + 1];
				this.UpdateCutoutRect(currentImage, currentTransform, previousTransform, PassiveUpgradeStackIcon.FrontCutoutPositionPropertyId, PassiveUpgradeStackIcon.FrontCutoutRadiusPropertyId);
				this.UpdateCutoutRect(currentImage, currentTransform, nextTransform, PassiveUpgradeStackIcon.BackCutoutPositionPropertyId, PassiveUpgradeStackIcon.BackCutoutRadiusPropertyId);
				iconIndex++;
			}
			previousTransform = currentTransform;
			currentImage = this._iconImages[this._iconImages.Count - 1];
			currentTransform = this._iconRectTransforms[this._iconImages.Count - 1];
			this.UpdateCutoutRect(currentImage, currentTransform, previousTransform, PassiveUpgradeStackIcon.FrontCutoutPositionPropertyId, PassiveUpgradeStackIcon.FrontCutoutRadiusPropertyId);
			currentImage.material.SetFloat(PassiveUpgradeStackIcon.BackCutoutRadiusPropertyId, 0f);
			return;
		}
		if (this._iconImages.Count == 1)
		{
			Image image = this._iconImages[0];
			image.material.SetFloat(PassiveUpgradeStackIcon.FrontCutoutRadiusPropertyId, 0f);
			image.material.SetFloat(PassiveUpgradeStackIcon.BackCutoutRadiusPropertyId, 0f);
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x000266A4 File Offset: 0x000248A4
	private void UpdateCutoutRect(Image thisImage, RectTransform thisTransform, RectTransform otherCircleTransform, int otherCirclePositionId, int otherCircleRadiusId)
	{
		Vector2 thisRectSize = thisTransform.rect.size;
		Vector3 relativePosition = thisTransform.InverseTransformPoint(otherCircleTransform.position) / (thisRectSize / 2f);
		relativePosition *= -1f;
		float relativeRadius = otherCircleTransform.rect.size.x * otherCircleTransform.lossyScale.x * this._circleRadius / (thisRectSize.x * thisTransform.lossyScale.x);
		thisImage.material.SetVector(otherCirclePositionId, relativePosition);
		thisImage.material.SetFloat(otherCircleRadiusId, relativeRadius);
	}

	// Token: 0x04000669 RID: 1641
	[SerializeField]
	private Material _passiveMaterial;

	// Token: 0x0400066A RID: 1642
	private readonly List<Image> _iconImages = new List<Image>();

	// Token: 0x0400066B RID: 1643
	private readonly List<RectTransform> _iconRectTransforms = new List<RectTransform>();

	// Token: 0x0400066C RID: 1644
	private float _circleRadius = -1f;

	// Token: 0x0400066D RID: 1645
	private static readonly int FrontCutoutPositionPropertyId = Shader.PropertyToID("_FrontCutoutPosition");

	// Token: 0x0400066E RID: 1646
	private static readonly int FrontCutoutRadiusPropertyId = Shader.PropertyToID("_FrontCutoutRadius");

	// Token: 0x0400066F RID: 1647
	private static readonly int BackCutoutPositionPropertyId = Shader.PropertyToID("_BackCutoutPosition");

	// Token: 0x04000670 RID: 1648
	private static readonly int BackCutoutRadiusPropertyId = Shader.PropertyToID("_BackCutoutRadius");

	// Token: 0x04000671 RID: 1649
	private static readonly int CircleRadiusPropertyId = Shader.PropertyToID("_CircleSize");
}
