using System;
using UnityEngine;

namespace Motorways.Constants
{
	// Token: 0x0200051A RID: 1306
	public static class ShaderConstants
	{
		// Token: 0x04001C7C RID: 7292
		public static readonly int HighestMotorwayTexture = Shader.PropertyToID("_HighestMotorwayTexture");

		// Token: 0x04001C7D RID: 7293
		public static readonly int ShadowTypeTexture = Shader.PropertyToID("_ShadowTypeTexture");

		// Token: 0x04001C7E RID: 7294
		public static readonly int ShadowType = Shader.PropertyToID("_ShadowType");

		// Token: 0x04001C7F RID: 7295
		public static readonly int HeadlightOcclusionTexture = Shader.PropertyToID("_HeadlightOcclusionTexture");

		// Token: 0x04001C80 RID: 7296
		public static readonly int ShadowColor = Shader.PropertyToID("_ShadowColor");

		// Token: 0x04001C81 RID: 7297
		public static readonly int Blend = Shader.PropertyToID("_Blend");

		// Token: 0x04001C82 RID: 7298
		public static readonly int TintTex = Shader.PropertyToID("_TintTex");

		// Token: 0x04001C83 RID: 7299
		public static readonly int PaperTex = Shader.PropertyToID("_PaperTex");

		// Token: 0x04001C84 RID: 7300
		public static readonly int VignetteColor = Shader.PropertyToID("_Vignette_Color");

		// Token: 0x04001C85 RID: 7301
		public static readonly int VignetteCenter = Shader.PropertyToID("_Vignette_Center");

		// Token: 0x04001C86 RID: 7302
		public static readonly int VignetteSettings = Shader.PropertyToID("_Vignette_Settings");

		// Token: 0x04001C87 RID: 7303
		public static readonly int Colors = Shader.PropertyToID("_Colors");

		// Token: 0x04001C88 RID: 7304
		public static readonly int GroupId = Shader.PropertyToID("_GroupId");

		// Token: 0x04001C89 RID: 7305
		public static readonly int UseColorsBuffer = Shader.PropertyToID("_UseColorsBuffer");

		// Token: 0x04001C8A RID: 7306
		public static readonly int ThemeComponentGroupTargetCount = Shader.PropertyToID("_ThemeComponentGroupTargetCount");

		// Token: 0x04001C8B RID: 7307
		public static readonly int HeadlightOcclusionTypeId = Shader.PropertyToID("_HeadlightOcclusionTypeId");

		// Token: 0x04001C8C RID: 7308
		public static readonly int MotorwayIdShaderId = Shader.PropertyToID("_MotorwayId");

		// Token: 0x04001C8D RID: 7309
		public static readonly int BeamLength = Shader.PropertyToID("_BeamLength");

		// Token: 0x04001C8E RID: 7310
		public static readonly int HalfBeamWidth = Shader.PropertyToID("_HalfBeamWidth");

		// Token: 0x04001C8F RID: 7311
		public static readonly int CircleOffset = Shader.PropertyToID("_CircleOffset");

		// Token: 0x04001C90 RID: 7312
		public static readonly int CircleRadius = Shader.PropertyToID("_CircleRadius");

		// Token: 0x04001C91 RID: 7313
		public static readonly int LeftCutPoint = Shader.PropertyToID("_LeftCutPoint");

		// Token: 0x04001C92 RID: 7314
		public static readonly int RightCutPoint = Shader.PropertyToID("_RightCutPoint");

		// Token: 0x04001C93 RID: 7315
		public static readonly int Intensity = Shader.PropertyToID("_Intensity");

		// Token: 0x04001C94 RID: 7316
		public static readonly int ObjectToLocalMatrix = Shader.PropertyToID("_ObjectToLocalMatrix");

		// Token: 0x04001C95 RID: 7317
		public static readonly int LeftHeadlightPosition = Shader.PropertyToID("_LeftHeadlightPosition");

		// Token: 0x04001C96 RID: 7318
		public static readonly int Alpha = Shader.PropertyToID("_Alpha");

		// Token: 0x04001C97 RID: 7319
		public static readonly float HeadlightNonVehicleOcclusionTypeId = float.MaxValue;

		// Token: 0x04001C98 RID: 7320
		public static readonly int TrailTime = Shader.PropertyToID("_TrailTime");

		// Token: 0x04001C99 RID: 7321
		public static readonly int TrailTimeEnd = Shader.PropertyToID("_TrailTimeEnd");

		// Token: 0x04001C9A RID: 7322
		public static readonly int OpacityThreshold = Shader.PropertyToID("_OpacityThreshold");

		// Token: 0x04001C9B RID: 7323
		public static readonly int WaveWidth = Shader.PropertyToID("_WaveWidth");

		// Token: 0x04001C9C RID: 7324
		public static readonly int WaveLength = Shader.PropertyToID("_WaveLength");

		// Token: 0x04001C9D RID: 7325
		public static readonly int OverallOpacity = Shader.PropertyToID("_OverallOpacity");

		// Token: 0x04001C9E RID: 7326
		public static readonly int MotorwayInnerOpacity = Shader.PropertyToID("_MotorwayInnerOpacity");

		// Token: 0x04001C9F RID: 7327
		public static readonly int MotorwayOuterOpacity = Shader.PropertyToID("_MotorwayOuterOpacity");

		// Token: 0x04001CA0 RID: 7328
		public static readonly int RoadWidth = Shader.PropertyToID("_RoadWidth");

		// Token: 0x04001CA1 RID: 7329
		public static readonly int RoadOutlineWidth = Shader.PropertyToID("_RoadOutlineWidth");

		// Token: 0x04001CA2 RID: 7330
		public static readonly int BlendingSize = Shader.PropertyToID("_BlendingSize");

		// Token: 0x04001CA3 RID: 7331
		public static readonly int HazardFadeoutOffset = Shader.PropertyToID("_HazardFadeoutOffset");

		// Token: 0x04001CA4 RID: 7332
		public static readonly int HazardFadeoutDistance = Shader.PropertyToID("_HazardFadeoutDistance");

		// Token: 0x04001CA5 RID: 7333
		public static readonly int HazardStripeWidth = Shader.PropertyToID("_HazardStripeWidth");

		// Token: 0x04001CA6 RID: 7334
		public static readonly int HazardStripeOpacity = Shader.PropertyToID("_HazardStripeOpacity");

		// Token: 0x04001CA7 RID: 7335
		public static readonly int HalfHazardStripeWidth = Shader.PropertyToID("_HalfHazardStripeWidth");

		// Token: 0x04001CA8 RID: 7336
		public static readonly int DistanceBetweenHazardStripes = Shader.PropertyToID("_DistanceBetweenHazardStripes");

		// Token: 0x04001CA9 RID: 7337
		public static readonly int FadeoutDistance = Shader.PropertyToID("_FadeoutDistance");

		// Token: 0x04001CAA RID: 7338
		public static readonly int SplineSegments = Shader.PropertyToID("_SplineSegments");

		// Token: 0x04001CAB RID: 7339
		public static readonly int LinearDistanceTable = Shader.PropertyToID("_LinearDistanceTable");

		// Token: 0x04001CAC RID: 7340
		public static readonly int LinearDistanceTableLength = Shader.PropertyToID("_LinearDistanceTableLength");

		// Token: 0x04001CAD RID: 7341
		public static readonly int DepthSegmentBuffer = Shader.PropertyToID("_DepthSegmentBuffer");

		// Token: 0x04001CAE RID: 7342
		public static readonly int DepthSegmentBufferLength = Shader.PropertyToID("_DepthSegmentBufferLength");

		// Token: 0x04001CAF RID: 7343
		public static readonly int MinMotorwayWorldHeight = Shader.PropertyToID("_MinMotorwayWorldHeight");

		// Token: 0x04001CB0 RID: 7344
		public static readonly int ShadowFadeoutBuffer = Shader.PropertyToID("_ShadowFadeoutBuffer");

		// Token: 0x04001CB1 RID: 7345
		public static readonly int HazardStripeSamples = Shader.PropertyToID("_HazardStripeSamples");

		// Token: 0x04001CB2 RID: 7346
		public static readonly int HazardStripeLastIndex = Shader.PropertyToID("_HazardStripeLastIndex");

		// Token: 0x04001CB3 RID: 7347
		public static readonly int RoadColor = Shader.PropertyToID("_RoadColor");

		// Token: 0x04001CB4 RID: 7348
		public static readonly int MotorwayColor = Shader.PropertyToID("_MotorwayColor");

		// Token: 0x04001CB5 RID: 7349
		public static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

		// Token: 0x04001CB6 RID: 7350
		public static readonly int MotorwayOutlineColor = Shader.PropertyToID("_MotorwayOutlineColor");

		// Token: 0x04001CB7 RID: 7351
		public static readonly int MotorwayId = Shader.PropertyToID("_MotorwayId");
	}
}
