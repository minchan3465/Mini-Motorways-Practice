using System;
using Easing;
using Rendering.RenderFeatures;
using UnityEngine;

namespace Motorways.Views
{
	// Token: 0x020005DE RID: 1502
	[CreateAssetMenu(fileName = "New MotorwayVisualParameters", menuName = "MotorwayVisualParameters", order = 1000)]
	public class MotorwayVisualParameters : ScriptableObject
	{
		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06002A09 RID: 10761 RVA: 0x000B6C78 File Offset: 0x000B4E78
		// (remove) Token: 0x06002A0A RID: 10762 RVA: 0x000B6CB0 File Offset: 0x000B4EB0
		public event Action OnParameterChanged;

		// Token: 0x06002A0B RID: 10763 RVA: 0x000B6CE5 File Offset: 0x000B4EE5
		public void InvokeOnParameterChanged()
		{
			Action onParameterChanged = this.OnParameterChanged;
			if (onParameterChanged == null)
			{
				return;
			}
			onParameterChanged();
		}

		// Token: 0x040023F5 RID: 9205
		public float roadWidth = 0.1f;

		// Token: 0x040023F6 RID: 9206
		public float roadOutlineWidth = 0.2f;

		// Token: 0x040023F7 RID: 9207
		public int splineSegmentCount = 90;

		// Token: 0x040023F8 RID: 9208
		public float splineDistanceBetweenStripes = 2f;

		// Token: 0x040023F9 RID: 9209
		public float splineStripeRotationDegrees = 35f;

		// Token: 0x040023FA RID: 9210
		public float splineEndFadeoutDistance = 1f;

		// Token: 0x040023FB RID: 9211
		public float hazardStripeFadeoutOffset = 0.5f;

		// Token: 0x040023FC RID: 9212
		public float hazardFadeoutDistance = 1f;

		// Token: 0x040023FD RID: 9213
		public float maxHazardStripeWidth = 0.5f;

		// Token: 0x040023FE RID: 9214
		public float hazardStripeInDuration = 0.35f;

		// Token: 0x040023FF RID: 9215
		public float hazardStripeOutDuration = 0.35f;

		// Token: 0x04002400 RID: 9216
		public Easings.Functions hazardStripeAnimationFunction = Easings.Functions.SineEaseInOut;

		// Token: 0x04002401 RID: 9217
		[Tooltip("The time it takes to tween the hazard stripes to full opacity. This occurs when a drying motorway is mothballed.")]
		public float hazardStripeOpacityFactorFadeDuration = 0.3f;

		// Token: 0x04002402 RID: 9218
		public float editModeOpacity = 0.75f;

		// Token: 0x04002403 RID: 9219
		public float mothballedOpacity = 0.5f;

		// Token: 0x04002404 RID: 9220
		public float viewModeOpacityInDuration = 0.5f;

		// Token: 0x04002405 RID: 9221
		public float viewModeOpacityOutDuration = 0.5f;

		// Token: 0x04002406 RID: 9222
		public Easings.Functions viewModeOpacityAnimationFunction = Easings.Functions.SineEaseInOut;

		// Token: 0x04002407 RID: 9223
		public float blendingSize = 0.1f;

		// Token: 0x04002408 RID: 9224
		[NonReorderable]
		[EnumTypedArray(typeof(ShadowTypeRenderPass.ShadowType))]
		public ShadowTypeFadeouts[] shadowFadeouts = new ShadowTypeFadeouts[Enum.GetNames(typeof(ShadowTypeRenderPass.ShadowType)).Length];
	}
}
